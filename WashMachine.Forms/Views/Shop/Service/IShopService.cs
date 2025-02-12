using WashMachine.Forms.Views.Shop.Model;
using System.Threading.Tasks;

namespace WashMachine.Forms.Views.Shop.Service
{
    public interface IShopService
    {
        Task<ShopModel> SignIn(string shopCode);
        Task<ShopSettingModel> GetSetting(string shopCode);
        Task<byte[]> DownloadImageAsByteArrayAsync(string url);
    }
}
