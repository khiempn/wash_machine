using System;
using System.Threading.Tasks;
using WashMachine.Forms.Common.Http;
using WashMachine.Forms.Modules.PaidBy.Service.Model;

namespace WashMachine.Forms.Modules.LaundryWashOption.Email
{
    public class EmailService : IEmailService
    {
        HttpService httpService;
        ShopConfigModel shopConfig;

        public EmailService()
        {
            httpService = new HttpService();
            shopConfig = Program.AppConfig.GetShopConfig();
        }

        public void SendEmailHealthCheckError(string machineName, string commandError)
        {
            Task.Run(() =>
            {
                httpService.Post($"{Program.AppConfig.AppHost}/EmailApi/SendEmailHealthCheckError", new
                {
                    ShopCode = shopConfig.Code,
                    Time = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt"),
                    Command = commandError,
                    MachineName = machineName
                }).GetAwaiter();
            });
        }

        public void SendEmailStartError(OrderModel order, string machineName, string commandError)
        {
            Task.Run(() =>
            {
                httpService.Post($"{Program.AppConfig.AppHost}/EmailApi/SendEmailStartError", new
                {
                    ShopCode = shopConfig.Code,
                    Time = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt"),
                    Command = commandError,
                    PaymentId = order.PaymentId,
                    MachineName = machineName
                }).GetAwaiter();
            });
        }
    }
}
