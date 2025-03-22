using System;
using System.IO;
using System.Windows.Forms;
using WashMachine.Forms.Modules.Login;

namespace WashMachine.Forms
{
    static class Program
    {
        public static AppConfigModel AppConfig;
        public static ShopConfigModel ShopConfig;
        public static Modules.PaidBy.Machine.Octopus.OctopusService octopusService;
        public static string ShopConfigFile = "shop.config";

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
            AppConfig.ShopConfigPath = Path.Combine(Directory.GetCurrentDirectory(), ShopConfigFile);
            Application.Run(new LoginForm());
        }
    }
}
