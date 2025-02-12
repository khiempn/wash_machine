using WashMachine.Forms.Views.PaidBy.Machine.Octopus;
using WashMachine.Forms.Views.PaidBy.Service.Octopus;
using System;
using System.IO.Ports;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WashMachine.Forms.Views.PaidBy.Machine
{
    public class MachineService : IMachineService
    {
        SerialPort _serialPort;
        public event EventHandler<string> CodeRecived;
        string codeBuilder;
        OctopusService octopusService;
        public event EventHandler<OctopusPaymentResponseModel> PaymentProgressHandler;
        public MachineService()
        {
            codeBuilder = string.Empty;
            octopusService = new OctopusService();
            octopusService.PaymentProgressHandler += OctopusService_PaymentProgressHandler; ;
        }

        private void OctopusService_PaymentProgressHandler(object sender, OctopusPaymentResponseModel e)
        {
            PaymentProgressHandler?.Invoke(sender, e);
        }

        public async Task<bool> ConnectAsync(string portName, int baudRate, int data, Parity parity = Parity.None, StopBits step = StopBits.One)
        {
            if (string.IsNullOrWhiteSpace(portName))
            {
                throw new ArgumentNullException(nameof(portName));
            }

            try
            {
                Logger.Log($"CONNECT BEGIN: portName {portName} baudRate {baudRate} parity {parity} StopBits {step}");
                if (_serialPort == null)
                {
                    _serialPort = new SerialPort(portName, baudRate, parity, data, step);
                }

                if (_serialPort.IsOpen == false)
                {
                    _serialPort.Open();
                }
                _serialPort.DataReceived += SerialPort_DataReceived;
                Logger.Log($"CONNECT END: SUCCESSFULLY");
                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Logger.Log($"CONNECT END: FAILED");
                Logger.Log(ex);
            }

            return await Task.FromResult(false);
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (CodeRecived != null)
            {
                SerialPort sp = (SerialPort)sender;
                string data = sp.ReadExisting();
                CodeRecived.Invoke(sp, data);
                Logger.Log($"{nameof(MachineService)} SerialPort_DataReceived code {data}");
            }
        }

        public void Disconnect()
        {
            try
            {
                if (_serialPort != null && _serialPort.IsOpen)
                {
                    _serialPort.Close();
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public Task<bool> ConnectAsync(Form mainForm)
        {
            mainForm.KeyPreview = true;
            mainForm.KeyPress += MainForm_DataReceived;
            return Task.FromResult(true);
        }

        private void MainForm_DataReceived(object sender, KeyPressEventArgs e)
        {
            Logger.Log($"Payment MainForm_KeyPress {e.KeyChar.ToString()}");

            if (e.KeyChar == '\r')
            {
                if (CodeRecived != null)
                {
                    CodeRecived.Invoke(sender, codeBuilder);
                    Logger.Log($"{nameof(MachineService)} MainForm_KeyPress code {codeBuilder}");
                }
                codeBuilder = string.Empty;
            }
            else
            {
                codeBuilder += e.KeyChar.ToString();
            }
        }

        public Task<bool> Disconnect(Form mainForm)
        {
            mainForm.KeyPreview = false;
            mainForm.KeyPress -= MainForm_DataReceived;
            return Task.FromResult(true);
        }

        public OctopusService ConnectOctopusAsync()
        {
            try
            {
                bool status = octopusService.Initial();
                if (status)
                {
                    return octopusService;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return null;
        }

        public OctopusService StartWaitingPayment(PaymentModel payment)
        {
            try
            {
                return octopusService.StartWaitingPayment(payment);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return null;
        }

        public Task<bool> DisconnectOctopus()
        {
            try
            {
                bool status = octopusService.Discope();
                if (status)
                {
                    return Task.FromResult(true);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return Task.FromResult(false);
        }

        public bool IsCancelPayment()
        {
            return octopusService.IsCancelPayment();
        }

        public void CancelPayment()
        {
            octopusService.CancelPayment();
        }
    }
}
