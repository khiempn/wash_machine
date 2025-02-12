using WashMachine.Forms.Common.Http;
using WashMachine.Forms.Views.PaidBy.Service.Octopus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WashMachine.Forms.Views.PaidBy.Machine.Octopus
{
    public class OctopusService : IOctopusService
    {
        OctopusLibrary octopusLibrary;
        Timer timer;
        HttpService httpService;
        Timer pollTimer;
        int pollCounter { get; set; } = 1000 * Program.AppConfig.ScanTimeout;
        public event EventHandler<OctopusPaymentResponseModel> PaymentProgressHandler;
        List<CardInfo> CardInfos { get; set; } = new List<CardInfo>();
        List<int> ErrorCodes = new List<int> {
            100016
            , 100017
            , 100020
            , 100032
            , 100034

            , 100019
            , 100021
            , 100024
            , 100035

            , 100048
            , 100049

            , 100001
            , int.MinValue
        };

        public bool IsCancelPaymentRequest { get; set; } = false;
        
        public OctopusService()
        {
            octopusLibrary = new OctopusLibrary();
            octopusLibrary.LogStart();
            httpService = new HttpService();
        }

        public void FileLocked()
        {

        }

        public bool Initial()
        {
            if (Program.AppConfig.ScanCouponMode == 0)
            {
                return true;
            }
            else
            {
                // Initialize Octopus payment
                Logger.Log("Initial Step 1");
                int r = octopusLibrary.ExecuteInitComm();
                if (ErrorCodes.Any(a => a == r))
                {
                    Logger.Log($"Initial Step 9 {r}");
                    PaymentProgressHandler?.Invoke(false, new OctopusPaymentResponseModel() { Rs = r });
                    return false;
                }
                else if (r != 0)
                {
                    Logger.Log($"Initial Step 10 {r}");
                    Logger.Log($"InitComm = {r} Octopus message");
                    return true;
                }
                return false;
            }
        }

        public OctopusService StartWaitingPayment(PaymentModel payment)
        {
            Logger.Log($"StartScanning Step 3 {JsonConvert.SerializeObject(payment)}");
            CardInfos = new List<CardInfo>();
            pollCounter = 1000 * Program.AppConfig.ScanTimeout;
            IsCancelPaymentRequest = false;

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
            Logger.Log($"PollTimer_Tick start ExcecutePoll {JsonConvert.SerializeObject(paymentInfo)}.");
            if (Program.AppConfig.ScanCouponMode == 0)
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

                PaymentProgressHandler?.Invoke(true, new OctopusPaymentResponseModel()
                {
                    Status = true,
                    Rs = 1002,
                    Message = "Payment successfully",
                    CardInfo = cardInfo
                });
                pollTimer.Stop();
                pollTimer.Dispose();
                return;
            }
            else
            {
                CardInfo cardInfo = octopusLibrary.ExcecutePoll(2, 20);
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
                        if (cardInfo.Rs == 100022)
                        {
                            //Continue poll again
                            CardInfos.Add(cardInfo);
                        }
                        //error code
                        else if (ErrorCodes.Any(a => a == cardInfo.Rs))
                        {
                            pollTimer.Stop();
                            pollTimer.Dispose();
                            CardInfos.Add(cardInfo);
                            if (CardInfos.Count > 1)
                            {
                                var firstCard = CardInfos[0];
                                var lastCard = CardInfos[CardInfos.Count - 1];
                                if (firstCard.CardId != lastCard.CardId)
                                {
                                    PaymentProgressHandler?.Invoke(false, new OctopusPaymentResponseModel() { SpecificScenario = true, Rs = 100001, Message = "Can not complete Payment", CardInfo = cardInfo });
                                }
                                else
                                {
                                    PaymentProgressHandler?.Invoke(false, new OctopusPaymentResponseModel() { Rs = 100001, Message = "Can not complete Payment", CardInfo = cardInfo });
                                }
                            }
                            else
                            {
                                PaymentProgressHandler?.Invoke(false, new OctopusPaymentResponseModel() { Rs = 100001, Message = "Can not complete Payment", CardInfo = cardInfo });
                            }
                        }
                        else
                        {
                            Logger.Log($"PollTimer_Tick start ExcecuteDeduct {JsonConvert.SerializeObject(paymentInfo)}.");

                            cardInfo.Rs = octopusLibrary.ExcecuteDeduct((int)paymentInfo.Amount, paymentInfo.Id);
                            if (cardInfo.Rs == 100022)
                            {
                                //Incomplete transaction. Please retry with the same Octopus
                                //Continue poll again
                                CardInfos.Add(cardInfo);
                            }

                            else if (ErrorCodes.Any(a => a == cardInfo.Rs))
                            {
                                pollTimer.Stop();
                                pollTimer.Dispose();
                                CardInfos.Add(cardInfo);
                                if (CardInfos.Count > 1)
                                {
                                    CardInfo firstCard = CardInfos[0];
                                    CardInfo lastCard = CardInfos[CardInfos.Count - 1];
                                    if (firstCard.CardId != lastCard.CardId)
                                    {
                                        PaymentProgressHandler?.Invoke(false, new OctopusPaymentResponseModel() { SpecificScenario = true, Rs = 100001, Message = "Can not complete Payment", CardInfo = cardInfo });
                                    }
                                    else
                                    {
                                        PaymentProgressHandler?.Invoke(false, new OctopusPaymentResponseModel() { Rs = cardInfo.Rs, Message = "Can not complete Payment", CardInfo = cardInfo });
                                    }
                                }
                                else
                                {
                                    PaymentProgressHandler?.Invoke(false, new OctopusPaymentResponseModel() { Rs = cardInfo.Rs, Message = "Can not complete Payment", CardInfo = cardInfo });
                                }
                            }
                            else
                            {
                                ExtraInfo extraInfo = octopusLibrary.ExcecuteGetExtraInfo();
                                cardInfo.LastAddType = extraInfo.LastAddType;
                                cardInfo.LastAddDate = extraInfo.LastAddDate;
                                cardInfo.CardJson = JsonConvert.SerializeObject(cardInfo);
                                pollTimer.Stop();
                                pollTimer.Dispose();
                                PaymentProgressHandler?.Invoke(true, new OctopusPaymentResponseModel() { Status = true, Rs = cardInfo.Rs, Message = "Payment successfully", CardInfo = cardInfo });
                            }
                        }
                    }
                }
                else
                {
                    Logger.Log($"PollTimer_Tick Timeout when waiting scan card.");

                    pollTimer.Stop();
                    pollTimer.Dispose();
                    PaymentProgressHandler?.Invoke(true, new OctopusPaymentResponseModel() { Rs = 100001, Message = "Timeout when waiting scan card.", CardInfo = cardInfo });
                }
            }
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
            Task.Run(() =>
            {
                RefreshConfig();
            });
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
    }
}
