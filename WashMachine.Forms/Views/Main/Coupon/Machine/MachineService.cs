using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WashMachine.Forms.Views.Main.Coupon.Machine
{
    public class MachineService : IMachineService
    {
        SerialPort _serialPort;
        public event EventHandler<string> CodeCouponRecived;
        string codeBuilder;

        public MachineService()
        {
            codeBuilder = string.Empty;
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
                return true;
            }
            catch (Exception ex)
            {
                Logger.Log($"CONNECT END: FAILED");
                Logger.Log(ex);
            }

            return false;
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (CodeCouponRecived != null)
            {
                SerialPort sp = (SerialPort)sender;
                string data = sp.ReadExisting();
                CodeCouponRecived.Invoke(sp, data);
                Console.WriteLine($"Data received: {data}");
            }
        }

        public void Disconect()
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

        public void ExecCommand(string command)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(command))
                {
                    throw new ArgumentNullException(nameof(command));
                }

                if (_serialPort != null && _serialPort.IsOpen)
                {
                    _serialPort.Write(command);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public void ExecHexCommand(string hexCommand)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(hexCommand))
                {
                    throw new ArgumentNullException(nameof(hexCommand));
                }

                if (_serialPort != null && _serialPort.IsOpen)
                {
                    byte[] buffer = HexStringToByteArray(hexCommand);
                    _serialPort.Write(buffer, 0, buffer.Length);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public byte[] HexStringToByteArray(string hex)
        {
            int numberChars = hex.Length;
            byte[] bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }

        public string HexByteArrayToString(byte[] bytes)
        {
            return string.Empty;
        }

        public List<string> CreateNewHexCommandByCounter(int counter, List<string> hexCommandsDefault)
        {
            if (hexCommandsDefault == null)
            {
                throw new ArgumentNullException(nameof(hexCommandsDefault));
            }

            if (hexCommandsDefault.Count == 0)
            {
                throw new Exception($"{nameof(hexCommandsDefault)} can not be empty.");
            }

            if (counter == 0)
            {
                throw new Exception($"{nameof(counter)} can not be Zero.");
            }

            int startIndex = 9;

            List<string> hexList = hexCommandsDefault;
            string xorHexValue = hexList[startIndex].ToString();
            int value = Convert.ToInt32(xorHexValue, 16);

            if (value < int.MaxValue)
            {
                value += counter;
                string hexValue = value.ToString("X4");
                Console.WriteLine($"HEX VALUE: {hexValue}");
                int step = 1;
                string reverseHexData = string.Empty;
                int hexValueLength = hexValue.Length;

                while (hexValueLength > 0)
                {
                    if (step <= 2)
                    {
                        char charHex = hexValue[hexValueLength - 1];
                        reverseHexData += charHex;
                        step++;
                        hexValueLength -= 1;
                    }

                    if (step > 2)
                    {
                        IEnumerable<char> reverseHexs = reverseHexData.Reverse();
                        hexList[startIndex] = $"0x" + string.Concat(reverseHexs);
                        startIndex -= 1;
                        step = 1;
                        reverseHexData = string.Empty;
                    }
                }

                return hexList;
            }
            else
            {
                throw new Exception($"The counter value {counter} exceed int.MaxValue");
            }
        }

        public List<int> XorHexCommand(List<string> hexCommands)
        {
            if (hexCommands == null)
            {
                throw new ArgumentNullException(nameof(hexCommands));
            }

            if (hexCommands.Count == 0)
            {
                throw new Exception($"{nameof(hexCommands)} can not be empty.");
            }

            List<int> xorHexCommands = new List<int>();
            string xorHexCommandData = string.Empty;

            for (int i = 0; i < hexCommands.Count;)
            {
                if (xorHexCommands.Count == 0)
                {
                    Console.WriteLine($"XorHexCommand CASE 1::: {hexCommands[i]} {hexCommands[i + 1]}");
                    int firstHex = Convert.ToInt32(hexCommands[i], 16);
                    int lastHex = Convert.ToInt32(hexCommands[i + 1], 16);
                    int resultXorHex = firstHex ^ lastHex;
                    xorHexCommands.Add(resultXorHex);
                    xorHexCommandData += resultXorHex.ToString("X2") + " ";
                    i += 2;
                }
                else
                {
                    Console.WriteLine($"XorHexCommand CASE 2::: {xorHexCommands[i - 2]} {hexCommands[i]}");

                    int firstHex = xorHexCommands[i - 2];
                    int lastHex = Convert.ToInt32(hexCommands[i], 16);
                    int resultXorHex = firstHex ^ lastHex;
                    xorHexCommands.Add(resultXorHex);
                    xorHexCommandData += resultXorHex.ToString("X2") + " ";
                    i++;
                }

                Console.WriteLine($"XorHexCommand RESULT: {xorHexCommandData}");
            }

            return xorHexCommands;
        }

        public string[] ConvertNumberHexToCommand(int number)
        {
            string hexValue = number.ToString("X4");

            string first = hexValue.Substring(0, 2);
            string last = hexValue.Substring(2, 2);

            return new string[] { last, first };
        }

        public Task<bool> ConnectAsync(Form mainForm)
        {
            mainForm.KeyPreview = true;
            mainForm.KeyPress += MainForm_KeyPress;
            //_ = Task.Run(() =>
            //{
            //    System.Threading.Thread.Sleep(3000);
            //    mainForm.Invoke(new Action(() =>
            //    {
            //        var codes = "G102050100001010123456721".ToCharArray().ToList();
            //        codes.Add('\r');
            //        foreach (var code in codes)
            //        {
            //            MainForm_KeyPress(null, new KeyPressEventArgs(code));
            //            System.Threading.Thread.Sleep(50);
            //        }
            //    }));
            //});
            return Task.FromResult(true);
        }

        private void MainForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            Logger.Log($"Coupon MainForm_KeyPress {e.KeyChar.ToString()}");
            if (e.KeyChar == '\r')
            {
                if (CodeCouponRecived != null)
                {
                    CodeCouponRecived.Invoke(sender, codeBuilder);
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
            mainForm.KeyPress -= MainForm_KeyPress;
            return Task.FromResult(true);
        }
    }
}
