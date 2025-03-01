using WashMachine.Forms.Common.Http;
using WashMachine.Forms.Modules.PaidBy.Dialog;
using WashMachine.Forms.Modules.PaidBy.Service.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WashMachine.Forms.Modules.PaidBy.Machine.Octopus
{
    public class OctopusService : IOctopusService
    {
        OctopusLibrary octopusLibrary;
        Timer timer;
        HttpService httpService;
        Timer pollTimer;
        int pollCounter { get; set; } = 1000 * Program.AppConfig.ScanTimeout;
        public event EventHandler<OctopusPaymentResponseModel> PaymentProgressHandler;
        public event EventHandler<bool> PaymentLoopingHandler;
        public event EventHandler<CardInfo> CreateOrderIncompleteHandler;

        List<int> MessageCodes { get; set; } = new List<int>();

        List<int> ErrorCodes = new List<int> {
            100019
            , 100021
            , 100024
            , 100035

            , 100048
            , 100049

            , 100001
            , int.MinValue
        };

        List<int> ErrorRetryAgainCodes = new List<int> {
            100016
            , 100017
            , 100020
            , 100032
            , 100034

            , 100022
        };

        private bool IsUserUsingApp { get; set; }

        public bool IsCancelPaymentRequest { get; set; } = false;

        public bool InitialStatus { get; set; }

        private Form currentForm;

        private Timer timerUserUsingAppCounter;

        public int LastRsInitialCOM { get; set; }

        public OctopusService()
        {
            octopusLibrary = new OctopusLibrary();
            octopusLibrary.LogStart();
            httpService = new HttpService();
        }

        public bool Initial()
        {
            if (Program.AppConfig.ScanOctopusMode == 0)
            {
                InitialStatus = true;
                return true;
            }
            else
            {
                // Initialize Octopus payment
                Logger.Log("Initial Step 1");
                int r = octopusLibrary.ExecuteInitComm();
                LastRsInitialCOM = r;
                if (r == 0)
                {
                    Logger.Log($"Initial Step 10 {r}");
                    Logger.Log($"InitComm = {r} Octopus message");
                    InitialStatus = true;
                    return true;
                }
                else
                {
                    Logger.Log($"Initial Step 9 {r}");
                    PaymentProgressHandler?.Invoke(false, new OctopusPaymentResponseModel() { Rs = r });
                    return false;
                }
            }
        }

        public OctopusService StartWaitingPayment(PaymentModel payment)
        {
            Logger.Log($"StartScanning Step 3 {JsonConvert.SerializeObject(payment)}");
            pollCounter = 1000 * Program.AppConfig.ScanTimeout;
            IsCancelPaymentRequest = false;
            MessageCodes = new List<int>();

            pollTimer = new Timer();
            pollTimer.Tick += PollTimer_Tick;
            pollTimer.Enabled = true;
            pollTimer.Interval = 1000;
            pollTimer.Tag = payment;
            pollTimer.Start();
            Logger.Log($"StartScanning Step 4");
            return this;
        }

        private void PollTimer_Tick(object sender, EventArgs e)
        {
            PaymentModel paymentInfo = (PaymentModel)pollTimer.Tag;
            PaymentLoopingHandler?.Invoke(sender, true);

            //Logger.Log($"PollTimer_Tick start ExcecutePoll {JsonConvert.SerializeObject(paymentInfo)}.");
            if (Program.AppConfig.ScanOctopusMode == 0)
            {
                CardInfo cardInfo = new CardInfo()
                {
                    Amount = paymentInfo.Amount,
                    CardId = "232",
                    CreateDated = DateTime.Now,
                    DeviceId = "2",
                    ShopName = paymentInfo.ShopName,
                    ShopCode = paymentInfo.ShopCode,
                    LastAddType = "1",
                    LastAddDate = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt"),
                    PaymentId = paymentInfo.Id,
                    PaymentTypeId = paymentInfo.PaymentTypeId,
                    PaymentTypeName = paymentInfo.PaymentTypeName,
                };
                cardInfo.CardJson = JsonConvert.SerializeObject(cardInfo);
                CreateOrderIncompleteHandler?.Invoke(sender, cardInfo);

                PaymentProgressHandler?.Invoke(true, new OctopusPaymentResponseModel()
                {
                    Status = true,
                    Rs = (int)OctopusPaymentStatus.SUCCESS,
                    Message = "Payment successfully",
                    CardInfo = cardInfo,
                    MessageCodes = new List<int>() { (int)OctopusPaymentStatus.SUCCESS },
                    IsStop = true
                });
                return;
            }
            else
            {
                Logger.Log($"ExcecutePoll Start Step 1");
                CardInfo cardInfo = octopusLibrary.ExcecutePoll(2, (byte)Program.AppConfig.ScanTimeout);
                Logger.Log($"ExcecutePoll End Step 2 {JsonConvert.SerializeObject(cardInfo)}");
                int currentTime = pollCounter;
                if (currentTime > 0)
                {
                    currentTime -= 1000;
                    pollCounter = currentTime;

                    //Detect card info
                    if (cardInfo != null)
                    {
                        cardInfo.Amount = paymentInfo.Amount;
                        cardInfo.PaymentId = paymentInfo.Id;
                        cardInfo.CreateDated = DateTime.Now;
                        cardInfo.ShopCode = paymentInfo.ShopCode;
                        cardInfo.ShopName = paymentInfo.ShopName;
                        cardInfo.CardJson = JsonConvert.SerializeObject(cardInfo);
                        cardInfo.PaymentTypeId = paymentInfo.PaymentTypeId;
                        cardInfo.PaymentTypeName = paymentInfo.PaymentTypeName;

                        Logger.Log($"PollTimer_Tick {cardInfo.Rs}");

                        #region clear error code
                        MessageCodes.RemoveAll(x => x != 100022);
                        MessageCodes = MessageCodes.Distinct().ToList();
                        #endregion

                        if (ErrorRetryAgainCodes.Exists(a => a == cardInfo.Rs))
                        {
                            MessageCodes.Add(cardInfo.Rs);

                            if (MessageCodes.Exists(x => x == 100022))
                            {
                                CreateOrderIncompleteHandler?.Invoke(sender, cardInfo);
                                MessageCodes.Add(-100022); // need to show message 請重試(八達通號碼 88888888)
                            }

                            PaymentProgressHandler?.Invoke(false, new OctopusPaymentResponseModel()
                            {
                                Rs = cardInfo.Rs,
                                Message = "Retry Payment",
                                CardInfo = cardInfo,
                                IsStop = false,
                                MessageCodes = MessageCodes,
                            });

                            ResetPollCounterToRetryAgain();
                        }
                        else if (ErrorCodes.Any(a => a == cardInfo.Rs))
                        {
                            MessageCodes.Add(cardInfo.Rs);

                            if (MessageCodes.Exists(x => x == 100022))
                            {
                                MessageCodes.Add(-100022); // need to show message 請重試(八達通號碼 88888888)
                            }

                            PaymentProgressHandler?.Invoke(false, new OctopusPaymentResponseModel()
                            {
                                Rs = cardInfo.Rs,
                                Message = "Can not complete Payment",
                                CardInfo = cardInfo,
                                MessageCodes = new List<int> { cardInfo.Rs },
                                IsStop = true
                            });
                        }
                        else
                        {
                            Logger.Log($"ExcecuteDeduct Start Step 3 {JsonConvert.SerializeObject(cardInfo)}");
                            cardInfo.Rs = octopusLibrary.ExcecuteDeduct((int)paymentInfo.Amount, paymentInfo.Id);
                            Logger.Log($"ExcecuteDeduct End Step 4 {JsonConvert.SerializeObject(cardInfo)}");

                            if (ErrorRetryAgainCodes.Exists(a => a == cardInfo.Rs))
                            {
                                MessageCodes.Add(cardInfo.Rs);

                                if (MessageCodes.Exists(x => x == 100022))
                                {
                                    CreateOrderIncompleteHandler?.Invoke(sender, cardInfo);
                                    MessageCodes.Add(-100022); // need to show message 請重試(八達通號碼 88888888)
                                }

                                PaymentProgressHandler?.Invoke(false, new OctopusPaymentResponseModel()
                                {
                                    Rs = cardInfo.Rs,
                                    Message = "Retry Payment",
                                    CardInfo = cardInfo,
                                    IsStop = false,
                                    MessageCodes = MessageCodes,
                                });

                                ResetPollCounterToRetryAgain();
                            }
                            else if (ErrorCodes.Any(a => a == cardInfo.Rs))
                            {
                                MessageCodes.Add(cardInfo.Rs);

                                if (MessageCodes.Exists(x => x == 100022))
                                {
                                    MessageCodes.Add(-100022); // need to show message 請重試(八達通號碼 88888888)
                                }

                                PaymentProgressHandler?.Invoke(false, new OctopusPaymentResponseModel()
                                {
                                    Rs = cardInfo.Rs,
                                    Message = "Can not complete Payment",
                                    CardInfo = cardInfo,
                                    IsStop = true,
                                    MessageCodes = MessageCodes
                                });
                            }
                            else
                            {
                                Logger.Log($"ExcecuteGetExtraInfo Start Step 5 {JsonConvert.SerializeObject(cardInfo)}");
                                ExtraInfo extraInfo = octopusLibrary.ExcecuteGetExtraInfo();
                                Logger.Log($"ExcecuteGetExtraInfo End Step 6 {JsonConvert.SerializeObject(cardInfo)}");

                                cardInfo.LastAddType = extraInfo.LastAddType;
                                cardInfo.LastAddDate = extraInfo.LastAddDate;
                                cardInfo.CardJson = JsonConvert.SerializeObject(cardInfo);
                                Logger.Log($"PaymentProgressHandler End Step 7 {JsonConvert.SerializeObject(cardInfo)}");

                                PaymentProgressHandler?.Invoke(true, new OctopusPaymentResponseModel()
                                {
                                    Status = true,
                                    Rs = cardInfo.Rs,
                                    Message = "Payment successfully",
                                    CardInfo = cardInfo,
                                    IsStop = true,
                                    MessageCodes = new List<int>() { (int)OctopusPaymentStatus.SUCCESS }
                                });
                            }
                        }
                    }
                }
                else
                {
                    PaymentProgressHandler?.Invoke(false, new OctopusPaymentResponseModel()
                    {
                        Rs = 100022,
                        Message = "Timeout when waiting scan card.",
                        CardInfo = cardInfo,
                        IsStop = true,
                        MessageCodes = new List<int>() { (int)OctopusPaymentStatus.MANUAL_TIMEOUT }
                    });
                }
            }
        }

        private void ResetPollCounterToRetryAgain()
        {
            pollCounter = 1000 * Program.AppConfig.ScanTimeout;
        }

        private bool LoopSetTimerInterval()
        {
            var jumping = 10;
            var time = DateTime.Now;
            var jump = time.Second % jumping;
            if (jump > 0)
            {
                timer.Interval = jump;
                return true;
            }
            timer.Interval = jumping * 1000;
            return false;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (LoopSetTimerInterval()) return;

            octopusLibrary.UploadFiles((status, message) =>
            {
                if (status)
                {
                    Logger.Log("UploadFiles successfully");
                }
            });
            octopusLibrary.DownloadFiles((status, message) =>
            {
                if (status)
                {
                    Logger.Log("Dowload Files Successfully!");
                }
            });

            octopusLibrary.ExcecuteXFile();
            //Task.Run(() =>
            //{
            //    RefreshConfig();
            //});
        }

        public void RunAJobToCompleteFiles()
        {
            RefreshConfig().GetAwaiter().OnCompleted(() =>
            {
                timer = new Timer();
                timer.Tick += Timer_Tick;
                timer.Enabled = true;
                timer.Start();
            });
        }

        public async Task RefreshConfig()
        {
            try
            {
                string result = await httpService.Get($"{Program.AppConfig.AppHost}/OctopusApi/GetConfig");
                ResponseModel response = httpService.ConvertTo<ResponseModel>(result);
                if (response != null && response.Success)
                {
                    string octopusConfig = JsonConvert.SerializeObject(response.Data);
                    OctopusConfigModel octopus = JsonConvert.DeserializeObject<OctopusConfigModel>(octopusConfig);
                    OctopusLibrary.OctopusConfig = octopus;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public bool Discope()
        {
            try
            {
                if (timer != null)
                {
                    timer.Stop();
                    timer.Dispose();
                }

                if (pollTimer != null)
                {
                    pollTimer.Stop();
                    pollTimer.Dispose();
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return false;
        }

        public bool IsCancelPayment()
        {
            return IsCancelPaymentRequest;
        }

        public void CancelPayment()
        {
            IsCancelPaymentRequest = true;
        }

        public void ShowOutOfService()
        {
            try
            {
                if (currentForm != null)
                {
                    HideOutOfService();

                    OutOfServiceUI outOfServiceUI = new OutOfServiceUI()
                    {
                        Width = currentForm.Width,
                        Height = currentForm.Height,
                        Parent = currentForm
                    };
                    currentForm.Controls.Add(outOfServiceUI);
                    currentForm.Controls.SetChildIndex(outOfServiceUI, 0);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public void HideOutOfService()
        {
            try
            {
                if (currentForm != null)
                {
                    currentForm.Invoke(new Action(() =>
                    {
                        if (currentForm.Controls.Find("pnOutOfService", true).Any())
                        {
                            OutOfServiceUI outOfServiceUI = (OutOfServiceUI)currentForm.Controls.Find("pnOutOfService", true)[0];
                            currentForm.Controls.Remove(outOfServiceUI);
                        }
                    }));
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public void SetCurrentForm(Form form)
        {
            if (currentForm != null)
            {
                HideOutOfService();
            }
            currentForm = form;
        }

        public bool IsUserUsingApplication()
        {
            return IsUserUsingApp;
        }

        public void SetUserIsUsingApp(bool isUsingApp)
        {
            IsUserUsingApp = isUsingApp;

            if (timerUserUsingAppCounter != null)
            {
                timerUserUsingAppCounter.Stop();
                timerUserUsingAppCounter.Dispose();
            }

            if (IsUserUsingApp)
            {
                timerUserUsingAppCounter = new Timer();
                timerUserUsingAppCounter.Tick += TimerUserUsingAppCounter_Tick;
                timerUserUsingAppCounter.Enabled = true;
                timerUserUsingAppCounter.Interval = 1000;
                //after 30s will be set user is offline
                timerUserUsingAppCounter.Tag = 1000 * 15;
                timerUserUsingAppCounter.Start();
            }
        }

        /// <summary>
        /// If user is scanning something then alway is set true
        /// </summary>
        public void SetUserIsUsingAppWithScanning()
        {
            IsUserUsingApp = true;
            if (timerUserUsingAppCounter != null)
            {
                timerUserUsingAppCounter.Stop();
                timerUserUsingAppCounter.Dispose();
            }
        }

        private void TimerUserUsingAppCounter_Tick(object sender, EventArgs e)
        {
            int currentTime = (int)timerUserUsingAppCounter.Tag;
            if (currentTime > 0)
            {
                currentTime -= 1000;
                timerUserUsingAppCounter.Tag = currentTime;
                Debug.WriteLine($"TimerUserUsingAppCounter_Tick {timerUserUsingAppCounter.Tag}");
            }
            else
            {
                SetUserIsUsingApp(false);
            }
        }
    }
}
