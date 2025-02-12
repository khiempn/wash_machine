using Libraries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WashMachine.Web
{
    public class AppConfigurations
    {
        private static AppConfigModel _appConfig;
        public static AppConfigModel AppConfig
        {
            get
            {
                if (_appConfig == null) {
                    var jsonStr = Libraries.FileUtilities.ReadFile("/appsettings.json");
                    _appConfig = TextUtilities.DeserializeObject<AppConfigModel>(jsonStr);
                }

                return _appConfig;
            }
        }
        public class AppConfigModel
        {
            public bool PreventSendEmail { get; set; }
            public bool IsDevelopment { get; set; }
        }

    }
}
