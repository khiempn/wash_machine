using WashMachine.Forms.Common.Http;
using WashMachine.Forms.Modules.PaidBy.Service.Model;
using WashMachine.Forms.Modules.PaidBy.Service.Octopus;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace WashMachine.Forms.Modules.PaidBy.Service
{
    public class ShopService : IShopService
    {
        HttpService httpService;
        public ShopService()
        {
            httpService = new HttpService();
        }

        public async Task<PaymentModel> CancelPayment(PaymentModel paymentModel)
        {
            string result = await httpService.Post($"{Program.AppConfig.AppHost}/ShopApi/CancelPayment", paymentModel);

            ResponseModel response = httpService.ConvertTo<ResponseModel>(result);
            if (response != null && response.Success)
            {
                PaymentModel payment = JsonConvert.DeserializeObject<PaymentModel>(JsonConvert.SerializeObject(response.Data));
                return payment;
            }

            return null;
        }

        public async Task<OrderModel> CompletePayment(OrderModel orderModel)
        {
            string result = await httpService.Post($"{Program.AppConfig.AppHost}/ShopApi/CompletePayment", orderModel);

            ResponseModel response = httpService.ConvertTo<ResponseModel>(result);
            if (response != null && response.Success)
            {
                OrderModel order = JsonConvert.DeserializeObject<OrderModel>(JsonConvert.SerializeObject(response.Data));
                return order;
            }

            return null;
        }

        public async Task<PaymentModel> CreateNewPayment(PaymentModel paymentModel)
        {
            string result = await httpService.Post($"{Program.AppConfig.AppHost}/ShopApi/CreateNewPayment", paymentModel);

            ResponseModel response = httpService.ConvertTo<ResponseModel>(result);
            if (response != null && response.Success)
            {
                PaymentModel payment = JsonConvert.DeserializeObject<PaymentModel>(JsonConvert.SerializeObject(response.Data));
                return payment;
            }

            return null;
        }

        public async Task<PaymentModel> UpdatePayment(PaymentModel paymentModel)
        {
            string result = await httpService.Post($"{Program.AppConfig.AppHost}/ShopApi/UpdatePayment", paymentModel);

            ResponseModel response = httpService.ConvertTo<ResponseModel>(result);
            if (response != null && response.Success)
            {
                PaymentModel payment = JsonConvert.DeserializeObject<PaymentModel>(JsonConvert.SerializeObject(response.Data));
                return payment;
            }

            return null;
        }

        public async Task<OrderModel> CreateIncompletedPayment(OrderModel orderModel)
        {
            string result = await httpService.Post($"{Program.AppConfig.AppHost}/ShopApi/CreateIncompletedPayment", orderModel);

            ResponseModel response = httpService.ConvertTo<ResponseModel>(result);
            if (response != null && response.Success)
            {
                OrderModel order = JsonConvert.DeserializeObject<OrderModel>(JsonConvert.SerializeObject(response.Data));
                return order;
            }

            return null;
        }
    }
}
