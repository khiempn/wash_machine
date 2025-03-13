using WashMachine.Forms.Modules.PaidBy.Machine;
using WashMachine.Forms.Modules.PaidBy.Machine.Octopus;
using WashMachine.Forms.Modules.PaidBy.Service.Model;
using WashMachine.Forms.Modules.PaidBy.Service.Octopus;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Threading.Tasks;

namespace WashMachine.Forms.Modules.PaidBy.Service
{
    public class OctopusService : IPaidByOctopusItem
    {
        MachineService machineService;
        public event EventHandler<OctopusPaymentResponseModel> PaymentProgressHandler;
        public event EventHandler<bool> PaymentLoopingHandler;
        public event EventHandler<CardInfo> CreateOrderIncompleteHandler;

        OrderModel _orderModel;

        public OctopusService()
        {
            machineService = new MachineService();
            machineService.PaymentProgressHandler += MachineService_PaymentProgressHandler;
            machineService.PaymentLoopingHandler += MachineService_PaymentLoopingHandler;
            machineService.CreateOrderIncompleteHandler += MachineService_CreateOrderIncompleteHandler;
        }

        private void MachineService_CreateOrderIncompleteHandler(object sender, CardInfo e)
        {
            CreateOrderIncompleteHandler?.Invoke(sender, e);
        }

        private void MachineService_PaymentLoopingHandler(object sender, bool e)
        {
            PaymentLoopingHandler?.Invoke(sender, e);
        }

        private void MachineService_PaymentProgressHandler(object sender, OctopusPaymentResponseModel e)
        {
            PaymentProgressHandler?.Invoke(sender, e);
        }

        public void StopScan()
        {
            machineService.DisconnectTimer();
        }

        public Task StartScanning(PaymentModel payment)
        {
            try
            {
                Logger.Log($"StartScanning Step 1");
                var octopusService = machineService.ConnectOctopusAsync();
                if (octopusService != null)
                {
                    Logger.Log($"StartScanning Step 2");
                    machineService.StartWaitingPayment(payment);
                }
                else
                {
                    Logger.Log("ConnectOctopusAsync is Failed");
                }
            }
            catch (Exception ex)
            {
                PaymentProgressHandler?.Invoke(false, new OctopusPaymentResponseModel()
                {
                    Rs = 10001,
                    MessageCodes = new List<int>() { 10001 },
                    Message = ex.Message,
                    IsStop = true
                });
                Logger.Log(ex);
            }
            return Task.FromResult(true);
        }

        /// <summary>
        /// <Last Add Value Date> It is the last add value date which is 10-byte long with format yyyy-mm-dd
        //  <Last Add Value Type> It is the last add value type which is 1-byte long
        //    1 Cash
        //    2 Online
        //    3 Refund
        //    4 AAVS
        //    Other values Others
        //    <Last Add Value Device ID> It is the ID of Octopus device involved in the
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private void PrintPage(object o, PrintPageEventArgs e)
        {
            Machine.Octopus.CardInfo cardInfo = JsonConvert.DeserializeObject<Machine.Octopus.CardInfo>(_orderModel.CardJson);
            int typeCash = 1;
            int typeOnline = 2;
            int typeAAVS = 4;

            var lastAddValueMessage = string.Empty;
            var lastAddValueMessageEn = string.Empty;
            if (cardInfo.LastAddType == typeCash.ToString())
            {
                lastAddValueMessage = $"上一次於{cardInfo.LastAddDate}現金增值";
                lastAddValueMessageEn = $"Last add value by Cash on {cardInfo.LastAddDate}";
            }
            else if (cardInfo.LastAddType == typeOnline.ToString())
            {
                lastAddValueMessage = $"上一次於{cardInfo.LastAddDate}網上增值";
                lastAddValueMessageEn = $"Last add value by Online on {cardInfo.LastAddDate}";
            }
            else if (cardInfo.LastAddType == typeAAVS.ToString())
            {
                lastAddValueMessage = $"上一次於{cardInfo.LastAddDate}自動增值";
                lastAddValueMessageEn = $"Last add value by AAVS on {cardInfo.LastAddDate}";
            }

            List<string> lines = new List<string> {
                $"日期時間 (Date/Time): {_orderModel.InsertTime.Value.ToString("@yyyy-MM-dd HH:mm:ss")}",
                $"店號 (Shop no.): {_orderModel.ShopCode}",
                $"機號 (Device no.): {_orderModel.DeviceId}",
                $"收據號碼 (Receipt no.): {_orderModel.PaymentId}",
                $"總額 (Total): HKD {FormatDecimal(_orderModel.Amount)}",
                "八達通付款(Octopus payment)",
                $"八達通號碼(Octopus no.): {cardInfo.OctopusNo}",
                $"扣除金額 (Amount deducted): -HKD {FormatDecimal(_orderModel.Amount, 1)}",
                $"餘額 (Remaining Value)： HKD {FormatDecimal(cardInfo.RemainValue, 1)}"
            };

            if (!string.IsNullOrEmpty(lastAddValueMessageEn))
            {
                lines.Add(lastAddValueMessageEn);
            }

            if (!string.IsNullOrEmpty(lastAddValueMessage))
            {
                lines.Add(lastAddValueMessage);
            }

            Font font = new Font("Arial", 12);
            float yPosition = 10;
            float lineHeight = font.GetHeight(e.Graphics);

            foreach (string line in lines)
            {
                e.Graphics.DrawString(line, font, Brushes.Black, new PointF(10, yPosition));
                yPosition += lineHeight;
            }
        }

        public string FormatDecimal(float? value, int n = 2)
        {
            if (value == null) return "0";
            return value.Value.ToString("n" + n);
        }

        public void Printer(OrderModel orderModel)
        {
            _orderModel = orderModel;
            PrintDocument pd = new PrintDocument();
            pd.PrintPage += PrintPage;
            pd.Print();
            //PrintPreviewDialog previewDialog = new PrintPreviewDialog
            //{
            //    Document = pd,
            //};

            //previewDialog.ShowDialog();
        }

        public void DisconnectOcotopus()
        {
            machineService.DisconnectOctopus();
        }
    }
}
