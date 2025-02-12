using System;
using System.Drawing;
using System.Threading.Tasks;

namespace WashMachine.Forms.Views.Shop.Model
{
    public class ShopSettingModel
    {
        public byte[] CouponScanImg { get; set; }
        public byte[] OctopusScanImg { get; set; }
        public byte[] PaymeScanImg { get; set; }
        public byte[] AlipayScanImg { get; set; }

        public string CouponScanImgUrl { get; set; }
        public string OctopusScanImgUrl { get; set; }
        public string PaymeScanImgUrl { get; set; }
        public string AlipayScanImgUrl { get; set; }

        public string OctopusPaymentImgUrl { get; set; }
        public string PaymePaymentImgUrl { get; set; }
        public string AlipayPaymentImgUrl { get; set; }

        public byte[] OctopusPaymentImg { get; set; }
        public byte[] PaymePaymentImg { get; set; }
        public byte[] AlipayPaymentImg { get; set; }

        public async Task LoadImages()
        {
            Shop.Service.ShopService shopService = new Service.ShopService();

            if (!string.IsNullOrWhiteSpace(CouponScanImgUrl))
            {
                byte[] imgBytes = await shopService.DownloadImageAsByteArrayAsync($"{Program.AppConfig.AppHost}/{CouponScanImgUrl}");
                CouponScanImg = imgBytes;
            }

            if (!string.IsNullOrWhiteSpace(AlipayScanImgUrl))
            {
                byte[] imgBytes = await shopService.DownloadImageAsByteArrayAsync($"{Program.AppConfig.AppHost}/{AlipayScanImgUrl}");
                AlipayScanImg = imgBytes;
            }

            if (!string.IsNullOrWhiteSpace(OctopusScanImgUrl))
            {
                byte[] imgBytes = await shopService.DownloadImageAsByteArrayAsync($"{Program.AppConfig.AppHost}/{OctopusScanImgUrl}");
                OctopusScanImg = imgBytes;
            }

            if (!string.IsNullOrWhiteSpace(PaymeScanImgUrl))
            {
                byte[] imgBytes = await shopService.DownloadImageAsByteArrayAsync($"{Program.AppConfig.AppHost}/{PaymeScanImgUrl}");
                PaymeScanImg = imgBytes;
            }

            if (!string.IsNullOrWhiteSpace(OctopusPaymentImgUrl))
            {
                byte[] imgBytes = await shopService.DownloadImageAsByteArrayAsync($"{Program.AppConfig.AppHost}/{OctopusPaymentImgUrl}");
                OctopusPaymentImg = imgBytes;
            }

            if (!string.IsNullOrWhiteSpace(PaymePaymentImgUrl))
            {
                byte[] imgBytes = await shopService.DownloadImageAsByteArrayAsync($"{Program.AppConfig.AppHost}/{PaymePaymentImgUrl}");
                PaymePaymentImg = imgBytes;
            }

            if (!string.IsNullOrWhiteSpace(AlipayPaymentImgUrl))
            {
                byte[] imgBytes = await shopService.DownloadImageAsByteArrayAsync($"{Program.AppConfig.AppHost}/{AlipayPaymentImgUrl}");
                AlipayPaymentImg = imgBytes;
            }
        }
    }
}
