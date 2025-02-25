using Newtonsoft.Json;
using System;
using System.Configuration;
using System.IO;
using System.IO.Ports;
using System.Reflection;

namespace WashMachine.Forms
{
    public class AppConfigModel
    {
        public static AppConfigModel ReadAppConfig()
        {
            var obj = new AppConfigModel();
            var appSettings = ConfigurationManager.AppSettings;
            foreach (PropertyInfo propertyInfo in obj.GetType().GetProperties())
            {
                var name = propertyInfo.Name;
                var value = appSettings[name] ?? "";

                if (name.Equals("DollarParity", System.StringComparison.OrdinalIgnoreCase)
                    || name.Equals("CouponParity", System.StringComparison.OrdinalIgnoreCase)
                    )
                {
                    Parity parity = (Parity)Enum.Parse(typeof(Parity), value, true);
                    propertyInfo.SetValue(obj, parity);
                }
                else if (name.Equals("DollarStopBits", System.StringComparison.OrdinalIgnoreCase)
                    || name.Equals("CouponStopBits", System.StringComparison.OrdinalIgnoreCase)
                    )
                {
                    StopBits parity = (StopBits)Enum.Parse(typeof(StopBits), value, true);
                    propertyInfo.SetValue(obj, parity);
                }
                else if (propertyInfo.PropertyType == typeof(string))
                {
                    propertyInfo.SetValue(obj, value);
                }
                else if (propertyInfo.PropertyType == typeof(bool))
                {
                    bool.TryParse(value, out bool configValue);
                    propertyInfo.SetValue(obj, configValue);
                }
                else if (propertyInfo.PropertyType == typeof(int))
                {
                    int.TryParse(value, out int configValue);
                    propertyInfo.SetValue(obj, configValue);
                }
            }
            return obj;
        }
        public string AppHost { get; set; }
        public string DollarCom { get; set; }
        public int DollarBaudRate { get; set; }
        public Parity DollarParity { get; set; }
        public int DollarData { get; set; }
        public StopBits DollarStopBits { get; set; }
        public string CouponCom { get; set; }
        public int CouponBaudRate { get; set; }
        public Parity CouponParity { get; set; }
        public int CouponData { get; set; }
        public StopBits CouponStopBits { get; set; }
        public int XorMethod { get; set; } = 0;
        public int HexTrimMethod { get; set; } = 0;
        public string EftTilNumber { get; set; }
        public string EftEcrRefNo { get; set; }
        public string EftAlipayBarCode { get; set; }
        public string EftPaymeBarCode { get; set; }
        public int ScanWithDeviceType { get; set; }
        public string CouponCode { get; set; }
        public int ScanCouponMode { get; set; }
        public int ScanAlipayMode { get; set; }
        public int ScanPaymeMode { get; set; }
        public int ScanOctopusMode { get; set; }
        public string OctopusRwlInitPath { get; set; }
        public string ShopConfigPath { get; set; }
        public int ScanTimeout { get; set; }

        public string WashMachineCom { get; set; }
        public int WashMachineBaudRate { get; set; }
        public Parity WashMachineParity { get; set; }
        public int WashMachineData { get; set; }
        public StopBits WashMachineStopBits { get; set; }
        public ShopConfigModel GetShopConfig()
        {
            string shopConfigPath = Path.Combine(Directory.GetCurrentDirectory(), "shop.config");
            if (File.Exists(shopConfigPath))
            {
                string shopConfigText = File.ReadAllText(shopConfigPath);
                if (!string.IsNullOrEmpty(shopConfigText))
                {
                    ShopConfigModel shopConfig = JsonConvert.DeserializeObject<ShopConfigModel>(shopConfigText);
                    if (shopConfig.ShopSetting == null)
                    {
                        shopConfig.ShopSetting = new Modules.Shop.Model.ShopSettingModel();
                    }
                    return shopConfig;
                }
            }
            return new ShopConfigModel() { ShopSetting = new Modules.Shop.Model.ShopSettingModel() };
        }
    }
}
