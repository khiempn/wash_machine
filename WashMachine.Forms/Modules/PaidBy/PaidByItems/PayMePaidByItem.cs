using WashMachine.Forms.Common.Http;
using WashMachine.Forms.Common.UI;
using WashMachine.Forms.Modules.Main.Coupon.Dialog;
using WashMachine.Forms.Modules.PaidBy.Dialog;
using WashMachine.Forms.Modules.PaidBy.Machine;
using WashMachine.Forms.Modules.PaidBy.Machine.Octopus;
using WashMachine.Forms.Modules.PaidBy.PaidByItems.Enum;
using WashMachine.Forms.Modules.PaidBy.PaidByItems.Model;
using WashMachine.Forms.Modules.PaidBy.Service;
using WashMachine.Forms.Modules.PaidBy.Service.Eft;
using WashMachine.Forms.Modules.PaidBy.Service.Model;
using WashMachine.Forms.Modules.PaidBy.Service.Octopus;
using WashMachine.Forms.Modules.Payment;
using Newtonsoft.Json;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScanWaitingUI = WashMachine.Forms.Modules.PaidBy.Dialog.ScanWaitingUI;

namespace WashMachine.Forms.Modules.PaidBy.PaidByItems
{
    public class PayMePaidByItem : IPaidByItem
    {
        PaymeService paymeService;
        ScanWaitingUI waitingUI;
        HttpService httpService;
        IPaymentItem paymentItem;
        Form mainForm;
        EftPayService eftPayService;
        ShopService shopService;
        ShopConfigModel shopConfig;
        PaymentModel _payment;
        bool IsCancelPayment { get; set; }

        public PayMePaidByItem(Form parent, IPaymentItem paymentItem)
        {
            shopService = new ShopService();
            shopConfig = Program.AppConfig.GetShopConfig();

            paymeService = new PaymeService();
            paymeService.CodeRecivedHandler += PaymeService_CodeRecivedHandlerAsync;
            httpService = new HttpService();
            eftPayService = new EftPayService();
            waitingUI = new ScanWaitingUI(PaymentType.Payme, paymentItem.PaymentAmount);
            waitingUI.SetParent(parent);
            waitingUI.CancelHandler += WaitingUI_CancelHandler;
            waitingUI.HomeHandler += WaitingUI_HomeHandler;

            this.paymentItem = paymentItem;
            mainForm = parent;
        }

        private void WaitingUI_HomeHandler(object sender, bool e)
        {
            try
            {
                if (Program.AppConfig.ScanWithDeviceType == (int)MachineType.USB)
                {
                    paymeService.StopScan(mainForm);
                }
                else
                {
                    paymeService.StopScan();
                }
                IsCancelPayment = true;

                if (_payment != null)
                {
                    shopService.CancelPayment(new PaymentModel()
                    {
                        Id = _payment.Id,
                        Message = "Cancel request payment by press back home button"
                    }).GetAwaiter();
                    _payment = null;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            waitingUI.Hide();
            mainForm.Close();
        }

        private async void PaymeService_CodeRecivedHandlerAsync(object sender, string message)
        {
            try
            {
                if (Program.AppConfig.ScanWithDeviceType == (int)MachineType.USB)
                {
                    paymeService.StopScan(mainForm);
                }
                else
                {
                    paymeService.StopScan();
                }

                if (IsCancelPayment)
                {
                    return;
                }

                if (string.IsNullOrWhiteSpace(message))
                {
                    Logger.Log("SCAN BAR CODE IS EMPTY");
                    waitingUI.SetMessage("Can not complete payment Payme, please try again.", true);
                    return;
                }

                if (message == "No action in 20 seconds")
                {
                    waitingUI.SetMessage("TIMEOUT: Please put your QR Code or Barcode to scanner.", true);
                }
                else
                {
                    Logger.Log($"{nameof(PayMePaidByItem)} Step 1");
                    EftPayResponseModel paymentResponse = await EftSale(new EftPayRequestModel()
                    {
                        TilNumber = Program.AppConfig.EftTilNumber,
                        Amount = paymentItem.PaymentAmount,
                        PaymentType = (short)PaymentType.Payme,
                        Barcode = message,
                        EcrRefNo = Program.AppConfig.EftEcrRefNo
                    });
                    Logger.Log($"{nameof(PayMePaidByItem)} Step 13");
                    PaidByForm paidByForm = (PaidByForm)mainForm;

                    if (paidByForm.FollowType == Login.FollowType.Normal)
                    {
                        if (paymentResponse.IsSuccess)
                        {
                            if (_payment != null)
                            {
                                Logger.Log($"{nameof(PayMePaidByItem)} Step 14");
                                Logger.Log($"{nameof(PayMePaidByItem)} Payment Successfully!.\n  {JsonConvert.SerializeObject(paymentResponse)}");
                                Service.Eft.ResponseCodeModel responseCode = paymentResponse.GetResposeMessage();
                                if (responseCode != null)
                                {
                                    string messagesResponse = $"RESPONSE CODE: {responseCode.Code}\n{responseCode.En_Message}\n{responseCode.Cn_Message}";

                                    waitingUI.SetMessage(messagesResponse, true);
                                    shopService.UpdatePayment(new PaymentModel()
                                    {
                                        Id = _payment.Id,
                                        PaymentStatus = 4,
                                        Message = messagesResponse
                                    }).GetAwaiter();
                                }
                                else
                                {
                                    OrderModel orderModel = await shopService.CompletePayment(new OrderModel()
                                    {
                                        ShopCode = _payment.ShopCode,
                                        ShopName = _payment.ShopName,
                                        Amount = _payment.Amount,
                                        Quantity = 1,
                                        PaymentId = _payment.Id,
                                        PaymentTypeId = _payment.PaymentTypeId,
                                        PaymentTypeName = _payment.PaymentTypeName,
                                        DeviceId = System.Environment.MachineName,
                                        CardJson = JsonConvert.SerializeObject(paymentResponse.TransactionRecord),
                                    });

                                    if (orderModel != null)
                                    {
                                        Logger.Log($"{nameof(PayMePaidByItem)} {JsonConvert.SerializeObject(paymentResponse)}");
                                        Logger.Log($"{nameof(PayMePaidByItem)} Step 14");
                                        ProgressUI progressUI = new ProgressUI();
                                        progressUI.SetParent(mainForm);
                                        progressUI.Show();
                                        Logger.Log($"{nameof(PayMePaidByItem)} Step 15");
                                        Logger.Log($"{nameof(PayMePaidByItem)} Payment Successfully!.\n  {JsonConvert.SerializeObject(paymentResponse)}");

                                        await paymentItem.DropCoinAsync(orderModel.Id);

                                        progressUI.Hide();
                                        mainForm.Controls.Remove(progressUI);
                                        Logger.Log($"{nameof(PayMePaidByItem)} Step 16");

                                        AlertSuccessfullyUI paymentAlertUI = new AlertSuccessfullyUI();
                                        paymentAlertUI.SetParent(mainForm);
                                        paymentAlertUI.SetPrintOrderModel(orderModel);
                                        paymentAlertUI.Show();
                                        paymentAlertUI.HomeClick += PaymentAlertUI_HomeClick;
                                        paymentAlertUI.PrinterClick += PaymentAlertUI_PrinterClick;
                                        waitingUI.SetMessage("Payment successfully!", false);
                                        waitingUI.DisabledButtons();

                                        shopService.UpdatePayment(new PaymentModel()
                                        {
                                            Id = _payment.Id,
                                            PaymentStatus = 3,
                                            Message = "Payment successfully!"
                                        }).GetAwaiter();
                                    }
                                    else
                                    {
                                        Logger.Log($"Can not complete payment Payme, please try again.");
                                        waitingUI.SetMessage("Can not complete payment Payme, please try again.", true);
                                        shopService.UpdatePayment(new PaymentModel()
                                        {
                                            Id = _payment.Id,
                                            PaymentStatus = 4,
                                            Message = $"Can not complete payment Payme, please try again."
                                        }).GetAwaiter();
                                    }
                                }
                            }
                            else
                            {
                                waitingUI.SetMessage("Can not complete payment Payme, please try again.", true);
                                Logger.Log("Can not complete payment Payme, please try again.");
                            }
                        }
                        else
                        {
                            Logger.Log($"{nameof(PayMePaidByItem)} Can not complete payment. {JsonConvert.SerializeObject(paymentResponse)}");
                            waitingUI.SetMessage("Can not complete payment Payme, please try again.", true);

                            if (_payment != null)
                            {
                                shopService.UpdatePayment(new PaymentModel()
                                {
                                    Id = _payment.Id,
                                    PaymentStatus = 4,
                                    Message = $"Can not complete payment Payme, please try again."
                                }).GetAwaiter();
                            }
                        }
                    }
                    else
                    {
                        if (paymentResponse.IsSuccess)
                        {
                            PaymentModel payment = await shopService.CreateNewPayment(new PaymentModel()
                            {
                                ShopCode = shopConfig.Code,
                                ShopName = shopConfig.Name,
                                PaymentStatus = 1,
                                PaymentTypeName = nameof(PaymentType.Payme),
                                PaymentTypeId = (int)PaymentType.Payme,
                                Amount = paymentItem.PaymentAmount
                            });
                            _payment = payment;

                            if (payment != null)
                            {
                                Logger.Log($"{nameof(PayMePaidByItem)} Step 14");
                                Logger.Log($"{nameof(PayMePaidByItem)} Payment Successfully!.\n  {JsonConvert.SerializeObject(paymentResponse)}");
                                Service.Eft.ResponseCodeModel responseCode = paymentResponse.GetResposeMessage();
                                if (responseCode != null)
                                {
                                    string messageResponse = $"RESPONSE CODE: {responseCode.Code}\n{responseCode.En_Message}\n{responseCode.Cn_Message}";

                                    waitingUI.SetMessage(messageResponse, true);
                                    shopService.UpdatePayment(new PaymentModel()
                                    {
                                        Id = _payment.Id,
                                        PaymentStatus = 4,
                                        Message = messageResponse
                                    }).GetAwaiter();
                                }
                                else
                                {
                                    OrderModel orderModel = await shopService.CompletePayment(new OrderModel()
                                    {
                                        ShopCode = payment.ShopCode,
                                        ShopName = payment.ShopName,
                                        Amount = payment.Amount,
                                        Quantity = 1,
                                        PaymentId = payment.Id,
                                        PaymentTypeId = payment.PaymentTypeId,
                                        PaymentTypeName = payment.PaymentTypeName,
                                        DeviceId = System.Environment.MachineName,
                                        CardJson = JsonConvert.SerializeObject(paymentResponse.TransactionRecord),
                                    });

                                    if (orderModel != null)
                                    {
                                        AlertSuccessfullyUI paymentAlertUI = new AlertSuccessfullyUI();
                                        paymentAlertUI.SetParent(mainForm);
                                        paymentAlertUI.SetPrintOrderModel(orderModel);
                                        paymentAlertUI.Show();
                                        paymentAlertUI.HomeClick += PaymentAlertUI_HomeClick;
                                        paymentAlertUI.PrinterClick += PaymentAlertUI_PrinterClick;
                                        waitingUI.SetMessage("Payment successfully!", false);
                                        waitingUI.DisabledButtons();
                                        shopService.UpdatePayment(new PaymentModel()
                                        {
                                            Id = _payment.Id,
                                            PaymentStatus = 3,
                                            Message = "Payment successfully!"
                                        }).GetAwaiter();
                                    }
                                    else
                                    {
                                        Logger.Log($"Can not complete payment Payme, please try again. {JsonConvert.SerializeObject(orderModel)}");
                                        waitingUI.SetMessage("Can not complete payment Payme, please try again.", true);
                                        shopService.UpdatePayment(new PaymentModel()
                                        {
                                            Id = _payment.Id,
                                            PaymentStatus = 4,
                                            Message = "Can not complete payment Payme, please try again."
                                        }).GetAwaiter();
                                    }
                                }
                            }
                            else
                            {
                                waitingUI.SetMessage("Can not complete payment Payme, please try again.", true);
                                Logger.Log("Can not create payment for payme");
                            }
                        }
                        else
                        {
                            Logger.Log($"{nameof(PayMePaidByItem)} Step 14");
                            Logger.Log($"{nameof(PayMePaidByItem)} Payment Failed!. {JsonConvert.SerializeObject(paymentResponse)}");
                            Service.Eft.ResponseCodeModel responseCode = paymentResponse.GetResposeMessage();
                            ErrorCodeModel errorCode = paymentResponse.GetErrorMessage();
                            if (responseCode != null)
                            {
                                string messageResponse = $"RESPONSE CODE: {responseCode.Code}\n{responseCode.En_Message}\n{responseCode.Cn_Message}";

                                waitingUI.SetMessage(messageResponse, true);
                                if (_payment != null)
                                {
                                    shopService.UpdatePayment(new PaymentModel()
                                    {
                                        Id = _payment.Id,
                                        PaymentStatus = 4,
                                        Message = messageResponse
                                    }).GetAwaiter();
                                }
                            }
                            else if (errorCode != null)
                            {
                                string messageResponse = $"ERROR CODE: {errorCode.Code}\n{errorCode.En_Message}\n{errorCode.Cn_Message}";
                                waitingUI.SetMessage(messageResponse, true);
                                if (_payment != null)
                                {
                                    shopService.UpdatePayment(new PaymentModel()
                                    {
                                        Id = _payment.Id,
                                        PaymentStatus = 4,
                                        Message = messageResponse
                                    }).GetAwaiter();
                                }
                            }
                            else
                            {
                                waitingUI.SetMessage($"Payment Failed!.\n {JsonConvert.SerializeObject(paymentResponse)}", true);
                                if (_payment != null)
                                {
                                    shopService.UpdatePayment(new PaymentModel()
                                    {
                                        Id = _payment.Id,
                                        PaymentStatus = 4,
                                        Message = "Payment Failed!."
                                    }).GetAwaiter();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                waitingUI.SetMessage("Can not complete payment Payme, please try again.", true);
            }
        }

        private void PaymentAlertUI_PrinterClick(object sender, EventArgs e)
        {

        }

        private void PaymentAlertUI_HomeClick(object sender, EventArgs e)
        {
            mainForm.Close();
        }

        private void WaitingUI_CancelHandler(object sender, bool e)
        {
            try
            {
                if (Program.AppConfig.ScanWithDeviceType == (int)MachineType.USB)
                {
                    paymeService.StopScan(mainForm);
                }
                else
                {
                    paymeService.StopScan();
                }
                IsCancelPayment = true;

                if (_payment != null)
                {
                    shopService.CancelPayment(new PaymentModel()
                    {
                        Id = _payment.Id,
                        Message = "Cancel request payment by press back button"
                    }).GetAwaiter();
                    _payment = null;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            waitingUI.Hide();
        }

        public Control GetTemplate()
        {
            string imgBase64 = "iVBORw0KGgoAAAANSUhEUgAAAS0AAACoCAMAAACCN0gDAAAAAXNSR0IB2cksfwAAAAlwSFlzAAALEwAACxMBAJqcGAAAALpQTFRFAAAA+NPV+trd7Zue2gAA2wAR2gAG30NH+t3f/Ojq6YyO5W5w3y428aaq/e3v3BIc3yIt3zY87qqr2wAL2wAN6IOF9sbJ+M7R4BQo4TxE8ry942Nm7pec++Dj2wAA5lRc7YSL64eN87K28J6j6Hd83w8j4k9V++Dj/vb3////7HyE9Li8/vn5/fHy/e3v/Ojq9sLG6Wtz4Ck06nJ65EhR3QAY75KY6GFq5ltj4EhM4zA+6FBc/e3v/OjqHMg5qQAAAD50Uk5TAP//////////cP////8g///////////////////w////////////////////////////////////////D1ly6CgYAAAWI0lEQVR4nO2di3faRrPAVxIIDEZrsDEOdvzIC7dpWqdN+93799/v3HNumzT50pdxEhPHj/gFlsC8kfbu7EogCSFQbCct1hwfLISQtD92Z2dnZlcC8hSB/lHx/nDKhZa7jVC86fWRx764CId7f3YbRDDoS9yQasOfuHco9VlUv7WgBkKRzQp61bXTdVCmirCgfbZ7+luLoqG0cGrf46SViNXmQlQ2IclW2/bWTitnXIjkc9/P31ywutAYNEcbrVw5VFceQrK9M2t7wCd/GjZCTxGM3KG1ae1brISwRgmxcFm08qe4OvLoWy8kVmf/TVq5cliz/MTExWmFsMYJ6cErp5W9CHtDfxEyWtOkleiEsMYJUSomrXQ9NErHClk8YrQy1bBqjRelpwEtpRGq+EmELB1QWtIt9foFFSI3KCgcCe3SSUTpKUJoPUwshAih+TC5UFrJdkhrUhEEKYQ1sYS0gkhIK4iEtIKIEAlhTS5C9EvfwT9JQlpBJKQVREJaQSSkFURCWkEkpBVEPpUWrhr2t2LqVvjIPokWVlG+K59nudNV0CNNY7aEbkGwOzgtRUP5eO8y7fBPG6o094agaU9oCkpLMGbxrA6gpEEWL2EORSkivMkcTfVAKhgtRfv29KhAN4zI9pbw9uFL2JmPXS69XX23QTfl/bo+zfUrCC2sbqSbPahK2T/alImZoIpJzUC4vSm2OwhF6qet6eUVgJaw1MxTHuWFyyMkKq4QJCZU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";
            ShopConfigModel shopConfig = Program.ShopConfig;
            if (shopConfig != null && shopConfig.ShopSetting != null)
            {
                if (shopConfig.ShopSetting.PaymePaymentImg != null)
                {
                    imgBase64 = Convert.ToBase64String(shopConfig.ShopSetting.PaymePaymentImg);
                }
            }

            CardItemProperty cardItem = new CardItemProperty()
            {
                Title = "Payme",
                BackgroundColor = "#ffffff",
                TitleColor = "#ffffff",
                CoverImageBase64 = imgBase64
            };

            CardButtonRoundedUI cardButton = new CardButtonRoundedUI()
            {
                Dock = DockStyle.Fill,
                Height = 170,
                Width = 50,
                Padding = new Padding(10),
                ShapeBackgroudColor = ColorTranslator.FromHtml(cardItem.BackgroundColor),
                ShapeBorderColor = Color.Black,
                CornerRadius = 50,

            };
            cardButton.Click += CardItem_Click;
            Panel pnCover = new Panel()
            {
                Dock = DockStyle.Fill,
                BackgroundImage = cardItem.GetCoverImage(),
                BackgroundImageLayout = ImageLayout.Stretch,
                Enabled = false
            };
            Label lbTitle = new Label()
            {
                Text = "",
                BackColor = Color.Transparent,
                Width = 100,
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Tag = cardButton,
                Enabled = false
            };

            pnCover.Controls.Add(lbTitle);

            cardButton.Controls.Add(pnCover);

            return cardButton;
        }

        private void CardItem_Click(object sender, EventArgs e)
        {
            Click();
        }

        public async void Click()
        {
            try
            {
                waitingUI = new ScanWaitingUI(PaymentType.Payme, paymentItem.PaymentAmount);
                waitingUI.SetParent(mainForm);
                waitingUI.CancelHandler += WaitingUI_CancelHandler;
                waitingUI.HomeHandler += WaitingUI_HomeHandler;

                waitingUI.ResetDefault();
                waitingUI.Show();
                _payment = null;
                IsCancelPayment = false;
                PaymentModel payment = await shopService.CreateNewPayment(new PaymentModel()
                {
                    ShopCode = shopConfig.Code,
                    ShopName = shopConfig.Name,
                    PaymentStatus = 1,
                    PaymentTypeName = nameof(PaymentType.Payme),
                    PaymentTypeId = (int)PaymentType.Payme,
                    Amount = paymentItem.PaymentAmount
                });
                _payment = payment;

                if (Program.AppConfig.ScanPaymeMode == 1)
                {
                    if (Program.AppConfig.ScanWithDeviceType == (int)MachineType.USB)
                    {
                        await paymeService.StartScanning(mainForm);
                    }
                    else
                    {
                        await paymeService.StartScanning();
                    }
                }
                else
                {
                    _ = Task.Run(() =>
                    {
                        System.Threading.Thread.Sleep(3000);
                        mainForm.Invoke(new Action(() =>
                        {
                            PaymeService_CodeRecivedHandlerAsync(null, Program.AppConfig.EftPaymeBarCode);
                        }));
                    });
                }
            }
            catch (Exception ex)
            {
                waitingUI.SetMessage("Can not complete payment Payme, please try again.", true);
                Logger.Log(ex);
            }
        }

        public Task TrackingStatistics(StatisticsModel statistics)
        {
            throw new NotImplementedException();
        }

        public async Task<EftPayResponseModel> EftSale(EftPayRequestModel eftPayParameter)
        {
            return await paymeService.EftSale(eftPayParameter);
        }
    }
}
