using System;
using System.Collections.Generic;
using System.Text;

namespace WashMachine.Octopus
{
    public class DataSetting
    {
        public static string OctopusConnectionString
        {
            get
            {
                var jsonStr = Libraries.FileUtilities.ReadFile("/appsettings.json");
                Newtonsoft.Json.Linq.JObject json = Newtonsoft.Json.Linq.JObject.Parse(jsonStr);
                var connectionString = json["ConnectionStrings"]["OctopusConnection"].ToString();
                return connectionString;
            }
        }

    }
}
