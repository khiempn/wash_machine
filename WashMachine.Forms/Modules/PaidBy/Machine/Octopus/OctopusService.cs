using WashMachine.Forms.Common.Http;
using WashMachine.Forms.Modules.PaidBy.Dialog;
using WashMachine.Forms.Modules.PaidBy.Service.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WashMachine.Forms.Modules.PaidBy.Machine.Octopus
{
    public class OctopusService : IOctopusService
    {
        OctopusLibrary octopusLibrary;
        static Timer jobTimer;
        HttpService httpService;
        static Timer pollTimer;
        static Timer healthCheckTimer;
        int pollCounter { get; set; }
        public event EventHandler<OctopusPaymentResponseModel> PaymentProgressHandler;
        public event EventHandler<bool> PaymentLoopingHandler;
        public event EventHandler<CardInfo> CreateOrderIncompleteHandler;

        List<int> MessageCodes { get; set; } = new List<int>();

        List<Code> PollErrors = new List<Code>()
        {
            new Code { Number = 100001, Retry = false, MustRetry = false, Comment = "R/W not connected" },
            new Code { Number = 100005, Retry = true, MustRetry = false, Comment = "Invalid response from R/W" },
            new Code { Number = 100016, Retry = true, MustRetry = false, Comment = "Card read error" },
            new Code { Number = 100017, Retry = true, MustRetry = false, Comment = "Card write error" },
            new Code { Number = 100019, Retry = false, MustRetry = false, Comment = "Card is blocked" },
            new Code { Number = 100021, Retry = false, MustRetry = false, Comment = "The last add value date of card is greater than 1000 days" },
            new Code { Number = 100023, Retry = false, MustRetry = false, Comment = "Transaction Log full" },
            new Code { Number = 100024, Retry = false, MustRetry = false, Comment = "Card is blocked by this call. In this case, PollData contains the UD" },
            new Code { Number = 100032, Retry = true, MustRetry = false, Comment = "No card present" },
            new Code { Number = 100034, Retry = true, MustRetry = false, Comment = "Card authentication error" },
            new Code { Number = 100035, Retry = false, MustRetry = false, Comment = "Card recover error" },
            new Code { Number = 100066, Retry = false, MustRetry = false, Comment = "System time error" }
        };

        List<Code> DeductErrors = new List<Code>()
        {
            new Code { Number = 100001, Retry = false, MustRetry = false, Comment = "R/W not connected" },
            new Code { Number = 100003, Retry = false, MustRetry = false, Comment = "Invalid parameters" },
            new Code { Number = 100005, Retry = true, MustRetry = false, Comment = "Invalid response from R/W" },
            new Code { Number = 100016, Retry = true, MustRetry = false, Comment = "Card read error" },
            new Code { Number = 100017, Retry = true, MustRetry = false, Comment = "The card polled before is not on the target or card communication has been\r\n\r\ninterrupted since last poll" },
            new Code { Number = 100019, Retry = false, MustRetry = false, Comment = "Card is blocked. (A blocked card is a blacklisted card that should not be\r\n\r\naccepted for any transactions.)" },
            new Code { Number = 100020, Retry = true, MustRetry = false, Comment = "Card has not been polled before or no card present" },
            new Code { Number = 100022, Retry = true, MustRetry = true, Comment = "Incomplete transaction. Must retry\r\n\r\nNote: A 100022 (Must retry) occurs when the customer pull out the card too\r\nquickly. The R/W is not sure if the data is successfully written to the card or\r\nnot. Hence, the R/W returns with this error code to advise the customer to\r\npresent the card again. The R/W will correctly handle the different cases of\r\nretry; the same transaction will not be deducted twice. Refer to Volume A1 on\r\nIncomplete Transaction Handling." },
            new Code { Number = 100025, Retry = true, MustRetry = true, Comment = "Incomplete transaction, retry please" },
            new Code { Number = 100023, Retry = false, MustRetry = false, Comment = "Transaction Log full" },
            new Code { Number = 100033, Retry = false, MustRetry = false, Comment = "The transaction amount is different from previous incomplete transaction." },
            new Code { Number = 100048, Retry = false, MustRetry = false, Comment = "Card has insufficient fund" }
        };

        List<Code> SpecificScenarioErrors = new List<Code>
        {
            new Code { Number = 100022, Retry = true, MustRetry = true, Comment = "Incomplete transaction. Must retry\r\n\r\nNote: A 100022 (Must retry) occurs when the customer pull out the card too\r\nquickly. The R/W is not sure if the data is successfully written to the card or\r\nnot. Hence, the R/W returns with this error code to advise the customer to\r\npresent the card again. The R/W will correctly handle the different cases of\r\nretry; the same transaction will not be deducted twice. Refer to Volume A1 on\r\nIncomplete Transaction Handling." },
            new Code { Number = 100025, Retry = true, MustRetry = true, Comment = "Incomplete transaction, retry please" },
        };

        private bool IsUserUsingApp { get; set; }

        public bool IsInitialSuccessfully { get; set; }

        private Form currentForm;

        private Timer timerUserUsingAppCounter;

        public int LastRsInitialCOM { get; set; }

        private CardInfo cardPollFirstly { get; set; }

        public OctopusService()
        {
            octopusLibrary = new OctopusLibrary();
            octopusLibrary.LogStart();
            httpService = new HttpService();

            pollTimer = new Timer();

            pollTimer.Enabled = true;
            pollTimer.Interval = 1000;
        }

        public bool Initial()
        {
            if (Program.AppConfig.ScanOctopusMode == 0)
            {
                IsInitialSuccessfully = true;
                return true;
            }
            else
            {
                Logger.Log("InitComm");
                int r = octopusLibrary.ExecuteInitCom();
                Logger.Log($"InitComm = {r} Octopus message");
                LastRsInitialCOM = r;
                if (r == 0)
                {
                    IsInitialSuccessfully = true;
                    return true;
                }
                else
                {
                    PaymentProgressHandler?.Invoke(false, new OctopusPaymentResponseModel()
                    {
                        Rs = r,
                        MessageCodes = new List<int> { r },
                        IsStop = true
                    });
                    return false;
                }
            }
        }

        public OctopusService StartWaitingPayment(PaymentModel payment)
        {
            Logger.Log($"StartWaitingPayment {JsonConvert.SerializeObject(payment)}");
            pollCounter = 1000 * Program.AppConfig.ScanTimeout;
            cardPollFirstly = null;
            MessageCodes = new List<int>();
            pollTimer.Stop();
            pollTimer.Enabled = true;
            pollTimer.Tag = payment;
            pollTimer.Tick -= PollTimer_Tick;
            pollTimer.Tick += PollTimer_Tick;
            pollTimer.Start();

            Logger.Log($"StartWaitingPayment Start");
            return this;
        }

        private void PollTimer_Tick(object sender, EventArgs e)
        {
            PaymentModel paymentInfo = (PaymentModel)pollTimer.Tag;
            PaymentLoopingHandler?.Invoke(sender, true);

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
                    CustomerInfo = "0-10-2-3-4"
                };
                cardInfo.CardJson = JsonConvert.SerializeObject(cardInfo);
                CreateOrderIncompleteHandler?.Invoke(sender, cardInfo);

                cardPollFirstly = null;
                pollTimer.Stop();

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
                Logger.Log($"1");
                CardInfo cardInfo = octopusLibrary.ExcecutePoll(2, (byte)Program.AppConfig.ScanTimeout);
                Logger.Log($"2 {JsonConvert.SerializeObject(cardInfo)}");
                int currentTime = pollCounter;
                currentTime -= 1000;
                Logger.Log($"{currentTime}");
                Int32 ERR_BASE = 100000;
                if (currentTime > 0)
                {
                    Logger.Log($"3");
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

                        Logger.Log($"PollTimer_Tick rs: {cardInfo.Rs} card_id:{cardInfo.CardId}");
                        Logger.Log($"4");

                        #region clear error code
                        MessageCodes.RemoveAll(x => x != 100022);
                        MessageCodes = MessageCodes.Distinct().ToList();
                        #endregion

                        if (PollErrors.Exists(a => a.Number == cardInfo.Rs))
                        {
                            MessageCodes.Add(cardInfo.Rs);

                            //For second time retry with must try
                            if (MessageCodes.Exists(x => SpecificScenarioErrors.Exists(a => a.Number == x)))
                            {
                                if (cardInfo.Rs == 100032) //Please tap card again
                                {
                                    MessageCodes.Remove(cardInfo.Rs);
                                }

                                if (cardPollFirstly != null)
                                {
                                    if (!string.IsNullOrWhiteSpace(cardInfo.CardId) && cardInfo.CardId != cardPollFirstly.CardId)
                                    {
                                        AddLog("Different card presented");
                                    }
                                    cardInfo = cardPollFirstly;
                                }

                                MessageCodes.Add(-100022); // need to show message 請重試(八達通號碼 88888888)
                            }

                            Code pollError = PollErrors.First(f => f.Number == cardInfo.Rs);
                            Logger.Log($"pollError {JsonConvert.SerializeObject(pollError)}");

                            if (pollError.Retry)
                            {
                                PaymentProgressHandler?.Invoke(true, new OctopusPaymentResponseModel()
                                {
                                    Rs = cardInfo.Rs,
                                    Message = "Retry Payment",
                                    CardInfo = cardInfo,
                                    IsStop = false,
                                    MessageCodes = MessageCodes,
                                });
                                pollTimer.Start();
                            }
                            else
                            {
                                cardPollFirstly = null;
                                pollTimer.Stop();
                                PaymentProgressHandler?.Invoke(true, new OctopusPaymentResponseModel()
                                {
                                    Rs = cardInfo.Rs,
                                    Message = "Stop Payment",
                                    CardInfo = cardInfo,
                                    IsStop = true,
                                    MessageCodes = MessageCodes,
                                });
                            }
                        }
                        else if (cardInfo.Rs <= ERR_BASE)
                        {
                            System.Threading.Thread.Sleep(Program.AppConfig.DelayDeduct);
                            cardInfo.Rs = octopusLibrary.ExcecuteDeduct((int)paymentInfo.Amount, paymentInfo.Id);
                            Logger.Log($"8 {JsonConvert.SerializeObject(cardInfo)}");

                            if (DeductErrors.Exists(a => a.Number == cardInfo.Rs))
                            {
                                MessageCodes.Add(cardInfo.Rs);

                                if (MessageCodes.Exists(x => SpecificScenarioErrors.Exists(a => a.Number == x)))
                                {
                                    CreateOrderIncompleteHandler?.Invoke(sender, cardInfo);
                                    MessageCodes.Add(-100022); // need to show message 請重試(八達通號碼 88888888)
                                }

                                Code deductError = DeductErrors.First(f => f.Number == cardInfo.Rs);
                                Logger.Log($"deductError {JsonConvert.SerializeObject(deductError)}");
                                if (deductError.MustRetry)
                                {
                                    AddLog($"{deductError.Number} - Enter retry loop");
                                    if (cardPollFirstly == null)
                                    {
                                        cardPollFirstly = cardInfo;
                                    }
                                    ResetPollCounterToRetryAgain();
                                    PaymentProgressHandler?.Invoke(true, new OctopusPaymentResponseModel()
                                    {
                                        Rs = cardInfo.Rs,
                                        Message = "Must Retry Payment",
                                        CardInfo = cardInfo,
                                        IsStop = false,
                                        MessageCodes = MessageCodes,
                                    });
                                    pollTimer.Start();
                                }
                                else if (deductError.Retry)
                                {
                                    PaymentProgressHandler?.Invoke(true, new OctopusPaymentResponseModel()
                                    {
                                        Rs = cardInfo.Rs,
                                        Message = "Retry Payment",
                                        CardInfo = cardInfo,
                                        IsStop = false,
                                        MessageCodes = MessageCodes,
                                    });
                                    pollTimer.Start();
                                }
                                else
                                {
                                    cardPollFirstly = null;
                                    pollTimer.Stop();
                                    PaymentProgressHandler?.Invoke(true, new OctopusPaymentResponseModel()
                                    {
                                        Rs = cardInfo.Rs,
                                        Message = "Stop Payment",
                                        CardInfo = cardInfo,
                                        IsStop = true,
                                        MessageCodes = MessageCodes,
                                    });
                                }
                                Logger.Log($"9");
                            }
                            else if (cardInfo.Rs <= ERR_BASE)
                            {
                                Logger.Log($"11");
                                ExtraInfo extraInfo = octopusLibrary.ExcecuteGetExtraInfo();
                                cardInfo.LastAddType = extraInfo.LastAddType;
                                cardInfo.LastAddDate = extraInfo.LastAddDate;
                                cardInfo.CardJson = JsonConvert.SerializeObject(cardInfo);
                                cardPollFirstly = null;
                                pollTimer.Stop();
                                PaymentProgressHandler?.Invoke(true, new OctopusPaymentResponseModel()
                                {
                                    Status = true,
                                    Rs = cardInfo.Rs,
                                    Message = "Payment successfully",
                                    CardInfo = cardInfo,
                                    IsStop = true,
                                    MessageCodes = new List<int>() { (int)OctopusPaymentStatus.SUCCESS }
                                });
                                Logger.Log($"12 {JsonConvert.SerializeObject(cardInfo)}");
                            }
                            else
                            {
                                cardPollFirstly = null;
                                pollTimer.Stop();
                                PaymentProgressHandler?.Invoke(true, new OctopusPaymentResponseModel()
                                {
                                    Status = false,
                                    Rs = cardInfo.Rs,
                                    Message = "Payment error",
                                    CardInfo = cardInfo,
                                    IsStop = true,
                                    MessageCodes = new List<int>() { (int)OctopusPaymentStatus.FAILURE }
                                });
                                Logger.Log($"13");
                            }
                        }
                        else
                        {
                            cardPollFirstly = null;
                            pollTimer.Stop();
                            PaymentProgressHandler?.Invoke(true, new OctopusPaymentResponseModel()
                            {
                                Status = false,
                                Rs = cardInfo.Rs,
                                Message = "Payment error",
                                CardInfo = cardInfo,
                                IsStop = true,
                                MessageCodes = new List<int>() { (int)OctopusPaymentStatus.FAILURE }
                            });
                            Logger.Log($"14");
                        }
                    }
                    else
                    {
                        cardPollFirstly = null;
                        pollTimer.Stop();
                        PaymentProgressHandler?.Invoke(true, new OctopusPaymentResponseModel()
                        {
                            Rs = (int)OctopusPaymentStatus.MANUAL_TIMEOUT,
                            Message = "Timeout when waiting scan card.",
                            CardInfo = cardInfo,
                            IsStop = true,
                            MessageCodes = new List<int>() { (int)OctopusPaymentStatus.MANUAL_TIMEOUT }
                        });
                    }
                }
                else
                {
                    cardPollFirstly = null;
                    pollTimer.Stop();
                    if (MessageCodes.Exists(x => SpecificScenarioErrors.Exists(a => a.Number == x)))
                    {
                        AddLog("Incomplete transaction retry loop end");
                    }

                    AddLog("Incomplete transaction cancelled by system (timeout)");

                    PaymentProgressHandler?.Invoke(true, new OctopusPaymentResponseModel()
                    {
                        Rs = (int)OctopusPaymentStatus.MANUAL_TIMEOUT,
                        Message = "Timeout when waiting scan card.",
                        CardInfo = cardInfo,
                        IsStop = true,
                        MessageCodes = new List<int>() { (int)OctopusPaymentStatus.MANUAL_TIMEOUT }
                    });
                    Logger.Log($"15");
                }
            }
        }

        public void AddLog(string message)
        {
            octopusLibrary.AddLog(message);
        }

        private void ResetPollCounterToRetryAgain()
        {
            pollCounter = 1000 * Program.AppConfig.ScanTimeout;
        }

        private bool LoopSetJobTimerInterval()
        {
            var jumping = 10;
            var time = DateTime.Now;
            var jump = time.Second % jumping;
            if (jump > 0)
            {
                jobTimer.Interval = jump;
                return true;
            }
            jobTimer.Interval = jumping * 1000;
            return false;
        }

        private void JobTimer_Tick(object sender, EventArgs e)
        {
            if (LoopSetJobTimerInterval()) return;

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
                jobTimer = new Timer();
                jobTimer.Tick += JobTimer_Tick;
                jobTimer.Enabled = true;
                jobTimer.Start();
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

        public bool StopWaitingPayment()
        {
            try
            {
                cardPollFirstly = null;
                pollTimer?.Stop();
                return true;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return false;
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
            timerUserUsingAppCounter?.Stop();
            if (IsUserUsingApp)
            {
                timerUserUsingAppCounter = new Timer();
                timerUserUsingAppCounter.Tick += TimerUserUsingAppCounter_Tick;
                timerUserUsingAppCounter.Enabled = true;
                timerUserUsingAppCounter.Interval = 1000;
                //after 10s will be set user is offline
                timerUserUsingAppCounter.Tag = 1000 * 10;
                timerUserUsingAppCounter.Start();
            }
        }

        /// <summary>
        /// If user is scanning something then alway is set true
        /// </summary>
        public void SetUserIsUsingAppWithScanning()
        {
            IsUserUsingApp = true;
            timerUserUsingAppCounter?.Stop();
        }

        private void TimerUserUsingAppCounter_Tick(object sender, EventArgs e)
        {
            int currentTime = (int)timerUserUsingAppCounter.Tag;
            if (currentTime > 0)
            {
                currentTime -= 1000;
                timerUserUsingAppCounter.Tag = currentTime;
            }
            else
            {
                SetUserIsUsingApp(false);
            }
        }

        public void Disconect()
        {
            octopusLibrary.Disconnect();
        }

        public void RunTimerHealthCheck()
        {
            healthCheckTimer = new Timer();
            healthCheckTimer.Tick += HealthCheckTimer_Tick;
            healthCheckTimer.Enabled = true;
            // Set up the timer to trigger every 30 minutes (30 * 60 * 1000 milliseconds)
            healthCheckTimer.Interval = 30 * 60 * 1000;
            healthCheckTimer.Start();
        }

        private void HealthCheckTimer_Tick(object sender, EventArgs e)
        {
            Logger.Log($"HealthCheckTimer_Tick called");
            if (octopusLibrary.IsConnected() == false)
            {
                IsInitialSuccessfully = false;
                LastRsInitialCOM = 0;

                Email.IEmailService emailService = new Email.EmailService();
                emailService.SendDisconnectError();
            }
        }

        public bool IsConnected()
        {
            return octopusLibrary.IsConnected();
        }
    }

    public class Code
    {
        public int Number { get; set; }
        public bool Retry { get; set; }
        public bool MustRetry { get; set; }
        public string Comment { get; set; }
    }
}
