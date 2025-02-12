using WashMachine.Forms.Common.Http;
using System;
using System.Threading.Tasks;

namespace WashMachine.Forms.Views.PaidBy.Machine.Octopus.Email
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

        public void SendDisconnectError()
        {
            Task.Run(() =>
            {
                httpService.Post($"{Program.AppConfig.AppHost}/EmailApi/SendDisconnectError", new
                {
                    ShopCode = shopConfig.Code,
                    Time = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt")
                }).GetAwaiter();
            });
        }

        public void SendDownloadError()
        {
            Task.Run(() =>
            {
                httpService.Post($"{Program.AppConfig.AppHost}/EmailApi/SendDownloadError", new
                {
                    ShopCode = shopConfig.Code,
                    Time = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt")
                }).GetAwaiter();
            });
        }

        public void SendGenerationError(string deviceId)
        {
            Task.Run(() =>
            {
                httpService.Post($"{Program.AppConfig.AppHost}/EmailApi/SendGenerationError", new
                {
                    ShopCode = shopConfig.Code,
                    Time = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt"),
                    DeviceId = deviceId
                }).GetAwaiter();
            });
        }

        public void SendUploadFileError()
        {
            Task.Run(() =>
            {
                httpService.Post($"{Program.AppConfig.AppHost}/EmailApi/SendUploadFileError", new
                {
                    ShopCode = shopConfig.Code,
                    Time = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt")
                }).GetAwaiter();
            });
        }
    }
}
