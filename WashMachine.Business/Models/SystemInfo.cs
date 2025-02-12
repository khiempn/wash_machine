using Libraries;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace WashMachine.Business.Models
{
    public class SystemInfo
    {
        public SettingModel Setting { get; set; }
        public UserModel User { get; set; }
        public List<SelectItem> ListShops { get; set; }
        public ShopModel ClientShop { get; set; }
    }

    public class SettingModel
    {
        public string OctopusPaymentPath { get; set; }
        public string PaymePaymentPath { get; set; }
        public string AlipayPaymentnPath { get; set; }
        public IFormFile OctopusPaymentFile { get; set; }
        public IFormFile PaymePaymentFile { get; set; }
        public IFormFile AlipayPaymentFile { get; set; }

        public string CouponScanPath { get; set; }
        public string OctopusScanPath { get; set; }
        public string PaymeScanPath { get; set; }
        public string AlipayScanPath { get; set; }

        public IFormFile CouponScanFile { get; set; }
        public IFormFile OctopusScanFile { get; set; }
        public IFormFile PaymeScanFile { get; set; }
        public IFormFile AlipayScanFile { get; set; }

        public string XFileHour { get; set; }
        public string XFileMinute { get; set; }
        public string UploadHour { get; set; }
        public string UploadMinute { get; set; }
        public string DownloadHour { get; set; }
        public string DownloadMinute { get; set; }

        public string OctopusUploadFolder { get; set; }
        public string OctopusDownloadFolder { get; set; }

        //ServerEmailAddress
        public string ServerEmailAddress { get; set; }
        public string ServerEmailPassword { get; set; }
        public string ServerEmailReceiver { get; set; }
        public string ServerEmailHost { get; set; }
        public string ServerEmailPort { get; set; }
        public string ServerEmailFrom { get; set; }
    }
}
