using WashMachine.Forms.Common.Http;
using WashMachine.Forms.Views.Main.Coupon.Machine;
using WashMachine.Forms.Views.PaidBy.PaidByItems.Enum;
using WashMachine.Forms.Views.PaidBy.Service;
using WashMachine.Forms.Views.PaidBy.Service.Model;
using WashMachine.Forms.Views.PaidBy.Service.Octopus;
using WashMachine.Forms.Views.Payment;
using WashMachine.Forms.Views.Payment.PaymentItems;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WashMachine.Forms.Views.Main.Coupon
{
    public class CouponService : ICouponService
    {
        MachineService machineService;
        public event EventHandler<bool> DropCoinCompletedHandler;
        public event EventHandler<string> CodeCouponRecivedHandler;
        Dictionary<string, IPaymentItem> codePayments = new Dictionary<string, IPaymentItem>();
        HttpService httpService;
        ShopService shopService;
        ShopConfigModel shopConfig;

        public CouponService()
        {
            shopService = new ShopService();
            shopConfig = Program.AppConfig.GetShopConfig();

            httpService = new HttpService();
            machineService = new MachineService();
            machineService.CodeCouponRecived += MachineService_CodeCouponRecived;
            Hkd10PaymentItem hkd10Payment = new Hkd10PaymentItem();
            hkd10Payment.DropCoinCompleted += DropcoinCompletedAction;

            Hkd20PaymentItem hkd20Payment = new Hkd20PaymentItem();
            hkd20Payment.DropCoinCompleted += DropcoinCompletedAction;

            Hkd50PaymentItem hkd50Payment = new Hkd50PaymentItem();
            hkd50Payment.DropCoinCompleted += DropcoinCompletedAction;

            Hkd100PaymentItem hkd100Payment = new Hkd100PaymentItem();
            hkd100Payment.DropCoinCompleted += DropcoinCompletedAction;

            Hkd500PaymentItem hkd500Payment = new Hkd500PaymentItem();
            hkd500Payment.DropCoinCompleted += DropcoinCompletedAction;

            codePayments.Add("010", hkd10Payment);
            codePayments.Add("020", hkd20Payment);
            codePayments.Add("050", hkd50Payment);
            codePayments.Add("100", hkd100Payment);
            codePayments.Add("500", hkd500Payment);
        }

        private async void MachineService_CodeCouponRecived(object sender, string couponCode)
        {
            try
            {
                CodeCouponRecivedHandler?.Invoke(sender, couponCode);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                MessageBox.Show("Your coupon is invalid, please try again.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public async Task StartScanning()
        {
            try
            {
                AppConfigModel appConfig = Program.AppConfig;
                bool isConnected = await machineService.ConnectAsync(appConfig.CouponCom, appConfig.CouponBaudRate, appConfig.CouponData, appConfig.CouponParity, appConfig.CouponStopBits);
                if (isConnected == false)
                {
                    MessageBox.Show($"Can not connect device.! \n CONFIG: {JsonConvert.SerializeObject(appConfig)}", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public async Task<Dictionary<string, string>> ValidateAsync(string code)
        {
            Dictionary<string, string> errors = new Dictionary<string, string>();

            try
            {
                if (string.IsNullOrWhiteSpace(code))
                {
                    errors.Add("EMPTY: ", "Coupon can not be empty.");
                    return errors;
                }

                char[] couponChars = code.ToCharArray();
                if (couponChars.Length != 25)
                {
                    errors.Add("INVALID_LENGTH: ", "Coupon code invalid length. required 25 length.");
                    return errors;
                }

                int amountChars = couponChars.Skip(10).Take(couponChars.Length - 1).Count(s => !char.IsDigit(s));
                if (amountChars > 0)
                {
                    errors.Add("INVALID_DIGIT: ", "There exists more than one char.");
                    return errors;
                }

                #region validate coupon code represent for dollar
                string codeDollar = string.Join(string.Empty, couponChars.Skip(13).Take(3));
                if (!codePayments.ContainsKey(codeDollar))
                {
                    errors.Add("INVALID_DOLLAR: ", "The value of the coupon represent for dollars is invalid.");
                    return errors;
                }
                #endregion

                #region validate result index 24,25 based on formula

                List<int> data = new List<int>();
                int multipler = 3;
                int totaStepMultipler = couponChars.Count() - 2;
                for (int i = 10; i < totaStepMultipler; i++)
                {
                    int value = Convert.ToInt32(couponChars[i].ToString()) * multipler;
                    data.Add(value);
                    multipler++;
                }

                int valueExpected = data.Sum() % 71;
                int valueInput = Convert.ToInt32(string.Join(string.Empty, couponChars.Skip(23)));
                if (valueExpected != valueInput)
                {
                    errors.Add("INVALID_VALUE_FORMULA: ", $"The value after apply formula invalid EXPECTED: {valueExpected}, VALUE INPUT: {valueInput}.");
                    return errors;
                }

                #endregion

                #region check exists shop code
                string shopCode = string.Join(string.Empty, couponChars.Skip(10).Take(3));
                string result = await httpService.Get($"{Program.AppConfig.AppHost}/ShopApi/CheckExistShopCode?code={shopCode}");
                ResponseModel response = httpService.ConvertTo<ResponseModel>(result);
                if (response == null || response.Success == false)
                {
                    errors.Add("INVALID_SHOP_CODE: ", $"Shop code {shopCode} is invalid.");
                    return errors;
                }
                #endregion

                #region check coupon is available
                result = await httpService.Get($"{Program.AppConfig.AppHost}/CouponApi/CheckCouponUsed?couponCode={code}");
                response = httpService.ConvertTo<ResponseModel>(result);
                if (response == null || response.Success == false)
                {
                    errors.Add("INVALID_COUPON_CODE: ", $"The coupon code has been used.");
                    return errors;
                }
                #endregion
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }

            return errors;
        }

        public async Task<bool> DropCoinAsync(string code)
        {
            try
            {
                char[] couponChars = code.ToCharArray();
                string codeDollar = string.Join(string.Empty, couponChars.Skip(13).Take(3));
                if (codePayments.ContainsKey(codeDollar))
                {
                    IPaymentItem paymentItem = codePayments[codeDollar];

                    PaymentModel payment = await shopService.CreateNewPayment(new PaymentModel()
                    {
                        ShopCode = shopConfig.Code,
                        ShopName = shopConfig.Name,
                        PaymentStatus = 0,
                        PaymentTypeName = nameof(PaymentType.Coupon),
                        PaymentTypeId = (int)PaymentType.Coupon,
                        Amount = paymentItem.PaymentAmount
                    });

                    if (payment != null)
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
                            CardJson = string.Empty,
                        });

                        if (orderModel != null)
                        {
                            if (Program.AppConfig.ScanCouponMode == 1)
                            {
                                await paymentItem.DropCoinAsync(orderModel.Id);
                                return true;
                            }
                            else
                            {
                                return true;
                                //MessageBox.Show("Apply coupon successfully.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }

            return false;
        }

        public void StopScan()
        {
            try
            {
                machineService.Disconect();
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        private async void DropcoinCompletedAction(bool isStatus, string message)
        {
            DropCoinCompletedHandler?.Invoke(message, isStatus);
        }

        public async Task StartScanning(Form mainForm)
        {
            try
            {
                bool isConnected = await machineService.ConnectAsync(mainForm);
                if (isConnected == false)
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

        public async Task SetCouponIsUsed(string couponCode)
        {
            string shopCode = string.Join(string.Empty, couponCode.Skip(10).Take(3));
            await httpService.Post($"{Program.AppConfig.AppHost}/CouponApi/AddCouponUsed?couponCode={couponCode}&shopCode={shopCode}", new { });
        }
    }
}
