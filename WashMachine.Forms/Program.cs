using WashMachine.Forms.Views.Login;
using WashMachine.Forms.Views.PaidBy.Dialog;
using WashMachine.Forms.Views.Shop;
using System;
using System.IO;
using System.Windows.Forms;

namespace WashMachine.Forms
{
    static class Program
    {
        public static AppConfigModel AppConfig;
        public static ShopConfigModel ShopConfig;

        public static int Counter { get; set; } = 1;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            AppConfig = AppConfigModel.ReadAppConfig();
            AppConfig.ShopConfigPath = Path.Combine(Directory.GetCurrentDirectory(), "shop.config");
            if (File.Exists(AppConfig.ShopConfigPath))
            {
                Application.Run(new LoginForm());
            }
            else
            {
                Application.Run(new SignInShopForm());
            }
        }
    }
}
