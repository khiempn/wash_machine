using EFTSolutions;
using WashMachine.Forms.Modules.PaidBy.Machine;
using WashMachine.Forms.Modules.PaidBy.PaidByItems;
using WashMachine.Forms.Modules.PaidBy.Service.Eft;
using WashMachine.Forms.Modules.PaidBy.Service.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WashMachine.Forms.Modules.PaidBy.Service
{
    public class AlipayService : IPaidByService
    {
        MachineService machineService;
        EftPayService eftPayService;
        public event EventHandler<string> CodeRecivedHandler;
        public event EventHandler<bool> PaymentLoopingHandler;
        static Timer aliPaytimer;
        OrderModel _orderModel;

        public AlipayService()
        {
            machineService = new MachineService();
            eftPayService = new EftPayService();

            machineService.CodeRecived += MachineService_CodeRecived;
            machineService.PaymentLoopingHandler += MachineService_PaymentLoopingHandler;
            aliPaytimer = new Timer();
            aliPaytimer.Tick += Timer_Tick;
            aliPaytimer.Interval = 1000;
            aliPaytimer.Tag = 1000 * Program.AppConfig.ScanTimeout;
        }

        private void MachineService_PaymentLoopingHandler(object sender, bool e)
        {
            
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            PaymentLoopingHandler?.Invoke(sender, true);

            int currentTime = (int)aliPaytimer.Tag;
            currentTime -= 1000;
            if (currentTime > 0)
            {
                aliPaytimer.Tag = currentTime;
            }
            else
            {
                aliPaytimer.Stop();
                machineService.Disconnect();
                CodeRecivedHandler?.Invoke(sender, "No action in 20 seconds");
            }
        }

        private void MachineService_CodeRecived(object sender, string barCode)
        {
            try
            {
                aliPaytimer.Stop();
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
                aliPaytimer.Start();
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
                    aliPaytimer.Tag = 1000 * Program.AppConfig.ScanTimeout;
                    aliPaytimer.Start();
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

        public void Printer(OrderModel orderModel)
        {
            _orderModel = orderModel;
            PrintDocument pd = new PrintDocument();
            pd.PrintPage += PrintPage;
            pd.Print();
        }

        private void PrintPage(object sender, PrintPageEventArgs e)
        {
            TransactionRecord cardInfo = JsonConvert.DeserializeObject<TransactionRecord>(_orderModel.CardJson);

            List<string> lines = new List<string> {
                "----------------------------------------------------------------------------------------------------------------",
                $"Shop Name:　{ _orderModel.ShopName }",
                $"Shop Code:　{ _orderModel.ShopCode }",
                $"Print Time:　{ DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt") }",
                "----------------------------------------------------------------------------------------------------------------",
                $"交易編號 Transaction No.:　{ cardInfo.OrderNumber }",
                $"付款方法 Payment Method : Alipay",
                $"總額 (Total): HKD { FormatDecimal(_orderModel.Amount) }"
            };

            Font font = new Font("Arial", 12);
            float yPosition = 10;
            float lineHeight = font.GetHeight(e.Graphics);

            foreach (string line in lines)
            {
                e.Graphics.DrawString(line, font, Brushes.Black, new PointF(10, yPosition));
                yPosition += lineHeight;
            }
        }

        private string FormatDecimal(float? value, int n = 2)
        {
            if (value == null) return "0";
            return value.Value.ToString("n" + n);
        }
    }
}
