using WashMachine.Forms.Common.Http;
using WashMachine.Forms.Modules.Shop.Model;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace WashMachine.Forms.Modules.Shop.Service
{
    public class ShopService : IShopService
    {
        HttpService httpService;

        public ShopService()
        {
            httpService = new HttpService();
        }

        public async Task<byte[]> DownloadImageAsByteArrayAsync(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                return await client.GetByteArrayAsync(url);
            }
        }

        public async Task<ShopSettingModel> GetSetting(string shopCode)
        {
            string result = await httpService.Get($"{Program.AppConfig.AppHost}/ShopApi/GetSetting?code={shopCode}");
            ResponseModel response = httpService.ConvertTo<ResponseModel>(result);
            if (response == null || response.Success == false)
            {
                return null;
            }
            else
            {
                string shopConfig = JsonConvert.SerializeObject(response.Data);

                ShopSettingModel shop = JsonConvert.DeserializeObject<ShopSettingModel>(shopConfig);
                return shop;
            }
        }

        public async Task<ShopModel> SignIn(string shopCode)
        {
            string result = await httpService.Get($"{Program.AppConfig.AppHost}/ShopApi/SignIn?code={shopCode}");
            ResponseModel response = httpService.ConvertTo<ResponseModel>(result);
            if (response == null || response.Success == false)
            {
                return null;
            }
            else
            {
                string shopConfig = JsonConvert.SerializeObject(response.Data);

                ShopModel shop = JsonConvert.DeserializeObject<ShopModel>(shopConfig);
                ShopSettingModel settingModel = await GetSetting(shopCode);
                shop.ShopSetting = settingModel;
                return shop;
            }
        }
    }
}
