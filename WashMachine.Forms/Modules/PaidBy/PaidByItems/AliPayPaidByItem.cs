using EFTSolutions;
using WashMachine.Forms.Common.Http;
using WashMachine.Forms.Common.UI;
using WashMachine.Forms.Modules.PaidBy.Dialog;
using WashMachine.Forms.Modules.PaidBy.Machine;
using WashMachine.Forms.Modules.PaidBy.PaidByItems.Enum;
using WashMachine.Forms.Modules.PaidBy.Service;
using WashMachine.Forms.Modules.PaidBy.Service.Eft;
using WashMachine.Forms.Modules.PaidBy.Service.Model;
using Newtonsoft.Json;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using WashMachine.Forms.Modules.Payment;

namespace WashMachine.Forms.Modules.PaidBy.PaidByItems
{
    public class AliPayPaidByItem : IPaidByItem
    {
        AlipayService alipayService;
        ScanWaitingUI waitingUI;
        HttpService httpService;
        IPaymentItem paymentItem;
        Form mainForm;
        EftPayService eftPayService;
        ShopService shopService;
        ShopConfigModel shopConfig;
        PaymentModel _payment;
        bool IsCancelPayment { get; set; }

        public AliPayPaidByItem(Form parent, IPaymentItem paymentItem)
        {
            shopService = new ShopService();
            shopConfig = Program.AppConfig.GetShopConfig();

            alipayService = new AlipayService();
            alipayService.CodeRecivedHandler += AlipayService_CodeRecivedHandlerAsync;
            alipayService.PaymentLoopingHandler += AlipayService_PaymentLoopingHandler;
            httpService = new HttpService();
            eftPayService = new EftPayService();
            waitingUI = new ScanWaitingUI(PaymentType.Alipay, paymentItem.PaymentAmount);
            waitingUI.SetParent(parent);
            waitingUI.CancelHandler += WaitingUI_CancelHandlerAsync;
            waitingUI.HomeHandler += WaitingUI_HomeHandler;

            this.paymentItem = paymentItem;
            mainForm = parent;
        }

        private void AlipayService_PaymentLoopingHandler(object sender, bool e)
        {
            waitingUI.DisabledButtons();
        }

        private async void WaitingUI_HomeHandler(object sender, bool e)
        {
            try
            {
                waitingUI.StopTimerCloseAlert();
                IsCancelPayment = true;
                ProgressUI progressUI = new ProgressUI();
                progressUI.SetParent(mainForm);
                progressUI.Show();

                if (Program.AppConfig.ScanWithDeviceType == (int)MachineType.USB)
                {
                    alipayService.StopScan(mainForm);
                }
                else
                {
                    alipayService.StopScan();
                }

                if (_payment != null)
                {
                    await shopService.CancelPayment(new PaymentModel()
                    {
                        Id = _payment.Id,
                        Message = "Cancel the payment request by pressing the home button."
                    });
                }

                progressUI.Hide();
                mainForm.Controls.Remove(progressUI);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            waitingUI.Hide();
            Program.octopusService.SetUserIsUsingApp(false);
            mainForm.Close();
        }

        private async void AlipayService_CodeRecivedHandlerAsync(object sender, string message)
        {
            try
            {
                if (Program.AppConfig.ScanWithDeviceType == (int)MachineType.USB)
                {
                    alipayService.StopScan(mainForm);
                }
                else
                {
                    alipayService.StopScan();
                }

                if (IsCancelPayment)
                {
                    return;
                }

                if (string.IsNullOrWhiteSpace(message))
                {
                    waitingUI.SetErrorMessage("Unable to complete the payment.");
                    waitingUI.StartTimerCloseAlert();
                    Program.octopusService.SetUserIsUsingApp(false);
                    return;
                }

                if (message == "No action in 20 seconds")
                {
                    waitingUI.SetErrorMessage("TIMEOUT: Please place your QR code or barcode on the scanner.");
                    waitingUI.StartTimerCloseAlert();
                    Program.octopusService.SetUserIsUsingApp(false);
                }
                else
                {
                    EftPayResponseModel paymentResponse;

                    if (Program.AppConfig.ScanAlipayMode == 1)
                    {
                        paymentResponse = await EftSale(new EftPayRequestModel()
                        {
                            TilNumber = Program.AppConfig.EftTilNumber,
                            Amount = paymentItem.PaymentAmount,
                            PaymentType = (short)PaymentType.Alipay,
                            Barcode = message,
                            EcrRefNo = Program.AppConfig.EftEcrRefNo
                        });
                    }
                    else
                    {
                        waitingUI.SetSuccessMessage("付款成功\nPayment successfully!");
                        waitingUI.StopTimerCloseAlert();
                        OrderModel orderModel = new OrderModel()
                        {
                            ShopCode = _payment.ShopCode,
                            ShopName = _payment.ShopName,
                            Amount = _payment.Amount,
                            Quantity = 1,
                            PaymentId = _payment.Id,
                            PaymentTypeId = _payment.PaymentTypeId,
                            PaymentTypeName = _payment.PaymentTypeName,
                            DeviceId = System.Environment.MachineName,
                            CardJson = JsonConvert.SerializeObject(new TransactionRecord() { originalTraceNum = "123" }),
                            Message = "Payment successfully!"
                        };

                        AlertSuccessfullyUI paymentAlertUI = new AlertSuccessfullyUI();
                        paymentAlertUI.SetParent(mainForm);
                        paymentAlertUI.SetPrintOrderModel(orderModel);
                        paymentAlertUI.HomeClick += PaymentAlertUI_HomeClick;
                        paymentAlertUI.PrinterClick += PaymentAlertUI_PrinterClick;
                        paymentAlertUI.SetAlipayInvoice(orderModel);
                        Program.octopusService.SetUserIsUsingApp(false);
                        paymentAlertUI.Show();
                        return;
                    }

                    PaidByForm paidByForm = (PaidByForm)mainForm;

                    if (paidByForm.FollowType == Login.FollowType.Normal)
                    {
                        if (paymentResponse.IsSuccess)
                        {
                            if (paymentResponse.IsPaymentCompleted())
                            {
                                Program.octopusService.SetUserIsUsingAppWithScanning();
                                waitingUI.SetSuccessMessage("付款成功\nPayment successfully!");
                                waitingUI.StopTimerCloseAlert();
                                ProgressUI progressUI = new ProgressUI();
                                progressUI.SetParent(mainForm);
                                progressUI.Show();

                                OrderModel orderRequest = new OrderModel()
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
                                    Message = "Payment successfully"
                                };
                                await shopService.CompletePayment(orderRequest);
                                await shopService.UpdatePayment(new PaymentModel()
                                {
                                    Id = _payment.Id,
                                    PaymentStatus = (int)PaymentStatus.Completed,
                                    Message = "Payment successfully!"
                                });

                                paymentItem.PaymentCompletedCallBack.Invoke(mainForm, () =>
                                {
                                    progressUI.Hide();
                                    mainForm.Controls.Remove(progressUI);

                                    AlertSuccessfullyUI paymentAlertUI = new AlertSuccessfullyUI();
                                    paymentAlertUI.SetParent(mainForm);
                                    paymentAlertUI.SetPrintOrderModel(orderRequest);
                                    paymentAlertUI.HomeClick += PaymentAlertUI_HomeClick;
                                    paymentAlertUI.PrinterClick += PaymentAlertUI_PrinterClick;
                                    paymentAlertUI.SetAlipayInvoice(orderRequest);
                                    paymentAlertUI.Show();
                                });
                            }
                            else
                            {
                                ResponseCodeModel responseCode = paymentResponse.GetErrorResposeMessage();
                                string messagesResponse = "Can not complete payment Alipay, please try again.";
                                if (responseCode != null)
                                {
                                    messagesResponse = $"RESPONSE CODE: {responseCode.Code}\n{responseCode.En_Message}\n{responseCode.Cn_Message}";


                                }
                                waitingUI.SetErrorMessage(messagesResponse);
                                await shopService.UpdatePayment(new PaymentModel()
                                {
                                    Id = _payment.Id,
                                    PaymentStatus = (int)PaymentStatus.Failed,
                                    Message = messagesResponse
                                });
                                waitingUI.StartTimerCloseAlert();
                                Program.octopusService.SetUserIsUsingApp(false);
                            }
                        }
                        else
                        {
                            waitingUI.SetErrorMessage("Can not complete payment Alipay, please try again.");
                            waitingUI.StartTimerCloseAlert();
                            await shopService.UpdatePayment(new PaymentModel()
                            {
                                Id = _payment.Id,
                                PaymentStatus = (int)PaymentStatus.Failed,
                                Message = "Can not complete payment Alipay, please try again."
                            });
                            Program.octopusService.SetUserIsUsingApp(false);
                        }
                    }
                    else
                    {
                        if (paymentResponse.IsSuccess)
                        {
                            if (paymentResponse.IsPaymentCompleted())
                            {
                                Program.octopusService.SetUserIsUsingAppWithScanning();
                                waitingUI.SetSuccessMessage("付款成功\nPayment successfully!");
                                waitingUI.StopTimerCloseAlert();
                                ProgressUI progressUI = new ProgressUI();
                                progressUI.SetParent(mainForm);
                                progressUI.Show();

                                OrderModel orderRequest = new OrderModel()
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
                                    Message = "Payment successfully"
                                };
                                await shopService.CompletePayment(orderRequest);
                                await shopService.UpdatePayment(new PaymentModel()
                                {
                                    Id = _payment.Id,
                                    PaymentStatus = (int)PaymentStatus.Completed,
                                    Message = $"Payment successfully!"
                                });

                                progressUI.Hide();
                                mainForm.Controls.Remove(progressUI);

                                AlertSuccessfullyUI paymentAlertUI = new AlertSuccessfullyUI();
                                paymentAlertUI.SetParent(mainForm);
                                paymentAlertUI.SetPrintOrderModel(orderRequest);
                                paymentAlertUI.HomeClick += PaymentAlertUI_HomeClick;
                                paymentAlertUI.PrinterClick += PaymentAlertUI_PrinterClick;
                                paymentAlertUI.SetAlipayInvoice(orderRequest);
                                paymentAlertUI.Show();
                            }
                            else
                            {
                                ResponseCodeModel responseCode = paymentResponse.GetErrorResposeMessage();
                                string messageResponse = $"Can not complete payment Alipay, please try again.";
                                if (messageResponse != null)
                                {
                                    messageResponse = $"RESPONSE CODE: {responseCode.Code}\n{responseCode.En_Message}\n{responseCode.Cn_Message}";
                                }

                                waitingUI.SetErrorMessage(messageResponse);
                                await shopService.UpdatePayment(new PaymentModel()
                                {
                                    Id = _payment.Id,
                                    PaymentStatus = (int)PaymentStatus.Failed,
                                    Message = messageResponse
                                });
                                waitingUI.StartTimerCloseAlert();
                                Program.octopusService.SetUserIsUsingApp(false);
                            }
                        }
                        else
                        {
                            ResponseCodeModel responseCode = paymentResponse.GetErrorResposeMessage();
                            ErrorCodeModel errorCode = paymentResponse.GetErrorMessage();
                            string messageResponse = $"Payment Failed!.";
                            if (responseCode != null)
                            {
                                messageResponse = $"RESPONSE CODE: {responseCode.Code}\n{responseCode.En_Message}\n{responseCode.Cn_Message}";
                            }
                            else if (errorCode != null)
                            {
                                messageResponse = $"ERROR CODE: {errorCode.Code}\n{errorCode.En_Message}\n{errorCode.Cn_Message}";
                            }
                            waitingUI.SetErrorMessage(messageResponse);
                            await shopService.UpdatePayment(new PaymentModel()
                            {
                                Id = _payment.Id,
                                PaymentStatus = (int)PaymentStatus.Failed,
                                Message = messageResponse
                            });
                            waitingUI.StartTimerCloseAlert();
                            Program.octopusService.SetUserIsUsingApp(false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                waitingUI.SetErrorMessage("Can not complete payment Alipay, please try again.");
                waitingUI.StartTimerCloseAlert();
                Program.octopusService.SetUserIsUsingApp(false);
            }
        }

        private void PaymentAlertUI_PrinterClick(object sender, EventArgs e)
        {
            OrderModel orderModel = (sender as Control).Tag as OrderModel;
            alipayService.Printer(orderModel);
        }

        private void PaymentAlertUI_HomeClick(object sender, EventArgs e)
        {
            try
            {
                ProgressUI progressUI = new ProgressUI();
                progressUI.SetParent(mainForm);
                progressUI.Show();

                if (Program.AppConfig.ScanWithDeviceType == (int)MachineType.USB)
                {
                    alipayService.StopScan(mainForm);
                }
                else
                {
                    alipayService.StopScan();
                }
                IsCancelPayment = false;
                progressUI.Hide();
                mainForm.Controls.Remove(progressUI);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }

            Program.octopusService.SetUserIsUsingApp(true);
            waitingUI.Hide();
            mainForm.Close();
        }

        private async void WaitingUI_CancelHandlerAsync(object sender, bool e)
        {
            try
            {
                waitingUI.StopTimerCloseAlert();
                IsCancelPayment = true;
                ProgressUI progressUI = new ProgressUI();
                progressUI.SetParent(mainForm);
                progressUI.Show();

                if (Program.AppConfig.ScanWithDeviceType == (int)MachineType.USB)
                {
                    alipayService.StopScan(mainForm);
                }
                else
                {
                    alipayService.StopScan();
                }

                if (_payment != null)
                {
                    await shopService.CancelPayment(new PaymentModel()
                    {
                        Id = _payment.Id,
                        Message = "Cancel the payment request by pressing the back button."
                    });
                }

                progressUI.Hide();
                mainForm.Controls.Remove(progressUI);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            waitingUI.Hide();
            Program.octopusService.SetUserIsUsingApp(false);
        }

        public Control GetTemplate()
        {
            string imgBase64 = "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";
            ShopConfigModel shopConfig = Program.ShopConfig;
            if (shopConfig != null && shopConfig.ShopSetting != null)
            {
                if (shopConfig.ShopSetting.AlipayPaymentImg != null)
                {
                    imgBase64 = Convert.ToBase64String(shopConfig.ShopSetting.AlipayPaymentImg);
                }
            }

            CardItemProperty cardItem = new CardItemProperty()
            {
                Title = "AliPay",
                BackgroundColor = "#ffffff",
                TitleColor = "#ffffff",
                CoverImageBase64 = imgBase64
            };

            CardButtonRoundedUI cardButton = new CardButtonRoundedUI()
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10, 40, 10, 40),
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
                BackColor = Color.Transparent,
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
                Program.octopusService.SetUserIsUsingAppWithScanning();
                waitingUI = new ScanWaitingUI(PaymentType.Alipay, paymentItem.PaymentAmount);
                waitingUI.SetParent(mainForm);
                waitingUI.CancelHandler += WaitingUI_CancelHandlerAsync;
                waitingUI.HomeHandler += WaitingUI_HomeHandler;

                waitingUI.ResetDefault();
                waitingUI.DisabledButtons();
                waitingUI.Show();

                _payment = null;
                IsCancelPayment = false;
                PaymentModel payment = await shopService.CreateNewPayment(new PaymentModel()
                {
                    ShopCode = shopConfig.Code,
                    ShopName = shopConfig.Name,
                    PaymentStatus = 1,
                    PaymentTypeName = nameof(PaymentType.Alipay),
                    PaymentTypeId = (int)PaymentType.Alipay,
                    Amount = paymentItem.PaymentAmount,
                    Message = string.Empty
                });

                _payment = payment;
              
                if (_payment == null)
                {
                    waitingUI.SetErrorMessage("Unable to create a payment for Alipay. Please try again.");
                    waitingUI.StartTimerCloseAlert();
                    Program.octopusService.SetUserIsUsingApp(false);
                    return;
                }

                if (Program.AppConfig.ScanAlipayMode == 1)
                {
                    if (Program.AppConfig.ScanWithDeviceType == (int)MachineType.USB)
                    {
                        await alipayService.StartScanning(mainForm);
                    }
                    else
                    {
                        await alipayService.StartScanning();
                    }
                }
                else
                {
                    _ = Task.Run(() =>
                    {
                        System.Threading.Thread.Sleep(3000);
                        mainForm.Invoke(new Action(() =>
                        {
                            AlipayService_CodeRecivedHandlerAsync(null, Program.AppConfig.EftAlipayBarCode);
                        }));
                    });
                }
            }
            catch (Exception ex)
            {
                waitingUI.SetErrorMessage("There is an exception during the initial payment.");
                waitingUI.StartTimerCloseAlert();
                Program.octopusService.SetUserIsUsingApp(false);
                Logger.Log(ex);
            }
        }

        public async Task<EftPayResponseModel> EftSale(EftPayRequestModel eftPayParameter)
        {
            return await alipayService.EftSale(eftPayParameter);
        }
    }
}
