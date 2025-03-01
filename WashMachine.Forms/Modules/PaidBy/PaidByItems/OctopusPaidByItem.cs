using WashMachine.Forms.Common.Http;
using WashMachine.Forms.Common.UI;
using WashMachine.Forms.Modules.PaidBy.Dialog;
using WashMachine.Forms.Modules.PaidBy.Machine.Octopus;
using WashMachine.Forms.Modules.PaidBy.PaidByItems.Enum;
using WashMachine.Forms.Modules.PaidBy.Service;
using WashMachine.Forms.Modules.PaidBy.Service.Eft;
using WashMachine.Forms.Modules.PaidBy.Service.Model;
using WashMachine.Forms.Modules.Payment;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WashMachine.Forms.Modules.PaidBy.PaidByItems
{
    public class OctopusPaidByItem : IPaidByItem
    {
        ScanWaitingUI waitingUI;
        HttpService httpService;
        IPaymentItem paymentItem;
        Form mainForm;
        Service.OctopusService octopusService;
        ShopService shopService;
        ShopConfigModel shopConfig;
        PaymentModel _payment;
        bool IsCancelPayment { get; set; }

        public OctopusPaidByItem(Form parent, IPaymentItem paymentItem)
        {
            octopusService = new Service.OctopusService();
            octopusService.PaymentProgressHandler += OctopusService_PaymentProgressHandler;
            octopusService.PaymentLoopingHandler += OctopusService_PaymentLoopingHandler;
            octopusService.CreateOrderIncompleteHandler += OctopusService_CreateOrderIncompleteHandler;
            shopService = new ShopService();
            shopConfig = Program.AppConfig.GetShopConfig();

            httpService = new HttpService();
            waitingUI = new ScanWaitingUI(PaymentType.Octopus, paymentItem.PaymentAmount);
            waitingUI.SetParent(parent);
            waitingUI.CancelHandler += WaitingUI_CancelHandlerAsync;
            waitingUI.HomeHandler += WaitingUI_HomeHandler;

            this.paymentItem = paymentItem;
            mainForm = parent;
        }

        private async void OctopusService_CreateOrderIncompleteHandler(object sender, CardInfo cardInfo)
        {
            try
            {
                OrderModel orderRequest = new OrderModel()
                {
                    ShopCode = cardInfo.ShopCode,
                    ShopName = cardInfo.ShopName,
                    Amount = cardInfo.Amount,
                    Quantity = 1,
                    PaymentId = cardInfo.PaymentId,
                    PaymentTypeId = cardInfo.PaymentTypeId,
                    PaymentTypeName = cardInfo.PaymentTypeName,
                    DeviceId = cardInfo.DeviceId,
                    CardJson = cardInfo.CardJson,
                    InsertTime = DateTime.Now,
                    PaymentStatus = (int)PaymentStatus.InCompleted,
                    OctopusNo = cardInfo.OctopusNo,
                    Message = "Incompleted with 100022 status"
                };

                await shopService.CreateIncompletedPayment(orderRequest);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        private void OctopusService_PaymentLoopingHandler(object sender, bool e)
        {
            waitingUI.DisabledButtons();
        }

        private async void WaitingUI_HomeHandler(object sender, bool e)
        {
            try
            {
                ProgressUI progressUI = new ProgressUI();
                progressUI.SetParent(mainForm);
                progressUI.Show();

                octopusService.StopScan();
                IsCancelPayment = true;
                await shopService.CancelPayment(new PaymentModel()
                {
                    Id = _payment.Id,
                    Message = "Cancel request payment by press back home button"
                });

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

        private async void OctopusService_PaymentProgressHandler(object sender, Machine.Octopus.OctopusPaymentResponseModel e)
        {
            try
            {
                Logger.Log($"PaymentProgressHandler Step 8 ");

                if (IsCancelPayment)
                {
                    waitingUI.StartTimerCloseAlert();
                    octopusService.StopScan();
                    Program.octopusService.SetUserIsUsingApp(false);
                    return;
                }

                if (e.IsStop)
                {
                    octopusService.StopScan();
                }

                if (e.Status)
                {
                    Program.octopusService.SetUserIsUsingAppWithScanning();
                    OrderModel orderRequest = new OrderModel()
                    {
                        ShopCode = e.CardInfo.ShopCode,
                        ShopName = e.CardInfo.ShopName,
                        Amount = e.CardInfo.Amount,
                        Quantity = 1,
                        PaymentId = e.CardInfo.PaymentId,
                        PaymentTypeId = e.CardInfo.PaymentTypeId,
                        PaymentTypeName = e.CardInfo.PaymentTypeName,
                        DeviceId = e.CardInfo.DeviceId,
                        CardJson = e.CardInfo.CardJson,
                        InsertTime = DateTime.Now,
                        OctopusNo = e.CardInfo.OctopusNo,
                        Message = "Payment successfully!"
                    };

                    PaidByForm paidByForm = (PaidByForm)mainForm;

                    if (paidByForm.FollowType == Login.FollowType.TestPaymentOnly)
                    {
                        waitingUI.Hide();

                        AlertSuccessfullyUI paymentAlertUI = new AlertSuccessfullyUI();
                        paymentAlertUI.SetParent(mainForm);
                        paymentAlertUI.SetPrintOrderModel(orderRequest);
                        paymentAlertUI.Show();
                        paymentAlertUI.HomeClick += PaymentAlertUI_HomeClick;
                        paymentAlertUI.PrinterClick += PaymentAlertUI_PrinterClick;
                        paymentAlertUI.SetOctopusInvoice(orderRequest);
                        if (e.MessageCodes.Count > 0)
                        {
                            int messageCode = e.MessageCodes.Last();
                            string message = e.GetMessage(messageCode);
                            waitingUI.SetSuccessMessage(message);
                        }
                        else
                        {
                            waitingUI.SetSuccessMessage("付款成功\nPayment successfully!");
                        }

                        await shopService.CompletePayment(orderRequest);

                        await shopService.UpdatePayment(new PaymentModel()
                        {
                            Id = _payment.Id,
                            PaymentStatus = (int)PaymentStatus.Completed,
                            Message = "付款成功\nPayment successfully!"
                        });
                        paymentItem.PaymentCompletedCallBack.Invoke(orderRequest);
                    }
                    else if (paidByForm.FollowType == Login.FollowType.Normal)
                    {
                        Logger.Log($"PaymentProgressHandler Step 9 ");
                        waitingUI.Hide();

                        ProgressUI progressUI = new ProgressUI();
                        progressUI.SetParent(mainForm);
                        progressUI.Show();

                        AlertSuccessfullyUI paymentAlertUI = new AlertSuccessfullyUI();
                        paymentAlertUI.SetParent(mainForm);
                        paymentAlertUI.SetPrintOrderModel(orderRequest);
                        paymentAlertUI.Show();
                        paymentAlertUI.HomeClick += PaymentAlertUI_HomeClick;
                        paymentAlertUI.PrinterClick += PaymentAlertUI_PrinterClick;
                        paymentAlertUI.SetOctopusInvoice(orderRequest);

                        if (e.MessageCodes.Count > 0)
                        {
                            int messageCode = e.MessageCodes.Last();
                            string message = e.GetMessage(messageCode);
                            waitingUI.SetSuccessMessage(message);
                        }
                        else
                        {
                            waitingUI.SetSuccessMessage("付款成功\nPayment successfully!");
                        }

                        Logger.Log($"PaymentProgressHandler Step 10 ");
                        await shopService.CompletePayment(orderRequest);

                        await shopService.UpdatePayment(new PaymentModel()
                        {
                            Id = _payment.Id,
                            PaymentStatus = 3,
                            Message = "付款成功\nPayment successfully!"
                        });
                        progressUI.Hide();
                        mainForm.Controls.Remove(progressUI);
                        paymentItem.PaymentCompletedCallBack.Invoke(orderRequest);
                    }
                }
                else
                {
                    List<string> messageList = new List<string>();
                    foreach (int errorCode in e.MessageCodes)
                    {
                        messageList.Add(e.GetMessage(errorCode));
                    }

                    if (messageList.Count > 0)
                    {
                        messageList.Insert(0, $"ERROR CODE: {e.Rs}");
                        string message = string.Join("\n", messageList);
                        if (e.CardInfo != null)
                        {
                            message = message.Replace("88888888", e.CardInfo.CardId);
                        }
                        waitingUI.SetErrorMessage(message);
                    }
                    else
                    {
                        string message = new OctopusPaymentResponseModel().GetMessageDefault();
                        waitingUI.SetErrorMessage(message);
                    }
                    waitingUI.StartTimerCloseAlert();
                    Program.octopusService.SetUserIsUsingApp(false);
                }
            }
            catch (Exception ex)
            {
                string message = new OctopusPaymentResponseModel().GetMessageDefault();
                waitingUI.StartTimerCloseAlert();
                waitingUI.SetErrorMessage(message);
                Logger.Log(ex);
                Program.octopusService.SetUserIsUsingApp(false);
            }
        }

        private void PaymentAlertUI_PrinterClick(object sender, EventArgs e)
        {
            OrderModel orderModel = (sender as Control).Tag as OrderModel;
            octopusService.Printer(orderModel);
        }

        private async void PaymentAlertUI_HomeClick(object sender, EventArgs e)
        {
            try
            {
                ProgressUI progressUI = new ProgressUI();
                progressUI.SetParent(mainForm);
                progressUI.Show();

                octopusService.StopScan();
                IsCancelPayment = true;
                await shopService.CancelPayment(new PaymentModel()
                {
                    Id = _payment.Id,
                    Message = "Cancel request payment by press back button"
                });
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

        private async void WaitingUI_CancelHandlerAsync(object sender, bool e)
        {
            try
            {
                ProgressUI progressUI = new ProgressUI();
                progressUI.SetParent(mainForm);
                progressUI.Show();

                octopusService.StopScan();
                IsCancelPayment = true;
                await shopService.CancelPayment(new PaymentModel()
                {
                    Id = _payment.Id,
                    Message = "Cancel request payment by press back button"
                });

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
                if (shopConfig.ShopSetting.OctopusPaymentImg != null)
                {
                    imgBase64 = Convert.ToBase64String(shopConfig.ShopSetting.OctopusPaymentImg);
                }
            }

            CardItemProperty cardItem = new CardItemProperty()
            {
                Title = "Octopus",
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
            pnCover.Resize += PnCover_Resize;
            Label lbTitle = new Label()
            {
                Text = "Octopus",
                BackColor = Color.Transparent,
                Width = 100,
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Enabled = false
            };

            pnCover.Controls.Add(lbTitle);

            cardButton.Controls.Add(pnCover);

            return cardButton;
        }

        private void PnCover_Resize(object sender, EventArgs e)
        {
            Control control = sender as Control;
            Label pnCover = control.Controls[0] as Label;
            pnCover.Location = new Point((control.Width - pnCover.Width) / 2, (control.Height - pnCover.Height) / 2);
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
                waitingUI = new ScanWaitingUI(PaymentType.Octopus, paymentItem.PaymentAmount);
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
                    PaymentTypeName = nameof(PaymentType.Octopus),
                    PaymentTypeId = (int)PaymentType.Octopus,
                    Amount = paymentItem.PaymentAmount
                });
                _payment = payment;

                if (payment != null)
                {
                    await octopusService.StartScanning(payment);
                }
                else
                {
                    waitingUI.StartTimerCloseAlert();
                    waitingUI.SetErrorMessage("Can not complete Payment");
                    Program.octopusService.SetUserIsUsingApp(false);
                }
            }
            catch (Exception ex)
            {
                waitingUI.StartTimerCloseAlert();
                waitingUI.SetErrorMessage("Can not complete Payment");
                Program.octopusService.SetUserIsUsingApp(false);
                Logger.Log(ex);
            }
        }

        public Task<EftPayResponseModel> EftSale(EftPayRequestModel eftPayParameter)
        {
            throw new NotImplementedException();
        }
    }
}
