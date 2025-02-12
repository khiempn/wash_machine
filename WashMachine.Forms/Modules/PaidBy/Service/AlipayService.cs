using WashMachine.Forms.Modules.PaidBy.Machine;
using WashMachine.Forms.Modules.PaidBy.PaidByItems;
using WashMachine.Forms.Modules.PaidBy.Service.Eft;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WashMachine.Forms.Modules.PaidBy.Service
{
    public class AlipayService : IPaidByService
    {
        MachineService machineService;
        EftPayService eftPayService;
        bool IsCancelPaymentRequest { get; set; }
        public event EventHandler<string> CodeRecivedHandler;
        Timer timer;

        public AlipayService()
        {
            machineService = new MachineService();
            eftPayService = new EftPayService();

            machineService.CodeRecived += MachineService_CodeRecived;
            timer = new Timer();
            timer.Tick += Timer_Tick;
            timer.Interval = 1000;
            timer.Tag = 1000 * Program.AppConfig.ScanTimeout;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            int currentTime = (int)timer.Tag;
            if (currentTime > 0)
            {
                currentTime -= 1000;
                timer.Tag = currentTime;
            }
            else
            {
                timer.Stop();
                machineService.Disconnect();
                CodeRecivedHandler?.Invoke(sender, "No action in 20 seconds");
            }
        }

        private void MachineService_CodeRecived(object sender, string barCode)
        {
            try
            {
                timer.Stop();
                Logger.Log($"START PAYMENT FOR {barCode}");
                CodeRecivedHandler?.Invoke(sender, barCode);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                MessageBox.Show("Can not complete payment, please try again.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public async Task StartScanning()
        {
            try
            {
                AppConfigModel appConfig = Program.AppConfig;
                bool isConnected = await machineService.ConnectAsync(appConfig.CouponCom, appConfig.CouponBaudRate, appConfig.CouponData, appConfig.CouponParity, appConfig.CouponStopBits);
                if (isConnected)
                {
                    IsCancelPaymentRequest = false;
                    timer.Start();
                }
                else
                {
                    MessageBox.Show($"Can not connect device.! \n CONFIG: {JsonConvert.SerializeObject(appConfig)}", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public void StopScan()
        {
            try
            {
                machineService.Disconnect();
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public async Task StartScanning(Form mainForm)
        {
            try
            {
                bool isConnected = await machineService.ConnectAsync(mainForm);
                if (isConnected)
                {
                    //timer.Start();
                }
                else
                {
                    MessageBox.Show($"Can not connect device.!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public void StopScan(Form mainForm)
        {
            try
            {
                machineService.Disconnect(mainForm);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public async Task<EftPayResponseModel> EftSale(EftPayRequestModel eftPayParameter)
        {
            try
            {
                Logger.Log($"{nameof(AliPayPaidByItem)} Step 2 {JsonConvert.SerializeObject(eftPayParameter)}");
                EftPayResponseModel eftPayResponse = await eftPayService.Sale(new EftPayRequestModel()
                {
                    Amount = eftPayParameter.Amount,
                    Barcode = eftPayParameter.Barcode,
                    EcrRefNo = eftPayParameter.EcrRefNo,
                    PaymentType = eftPayParameter.PaymentType,
                    TilNumber = eftPayParameter.TilNumber,
                });
                Logger.Log($"{nameof(AliPayPaidByItem)} Step 12 {JsonConvert.SerializeObject(eftPayResponse)}");
                return eftPayResponse;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return new EftPayResponseModel() { 
                    IsSuccess = false,
                    Message = $"{ex.Message}", 
                    ReturnId = 0, 
                    TransactionRecord = null 
                };
            }
        }

        public bool IsCancelPayment()
        {
            return IsCancelPaymentRequest;
        }

        public void CancelPayment()
        {
            IsCancelPaymentRequest = true;
        }
    }
}
