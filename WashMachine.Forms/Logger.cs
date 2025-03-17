using WashMachine.Forms.Common.Http;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace WashMachine.Forms
{
    public class Logger
    {
        private static readonly string LOG_FILE_NAME = "logs.txt";

        public static void Log(Exception ex)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(ex.Message))
                {
                    var path = GetPath();
                    using (var file = new StreamWriter(path, true))
                    {
                        if (ex.InnerException != null)
                        {
                            file.Write(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt") + ": " + ex.InnerException.Source + "\n" + ex.InnerException.Message + Environment.NewLine + ex.Message + Environment.NewLine);
                        }
                        else
                        {
                            file.Write(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt") + ": " + ex.Message + Environment.NewLine);
                        }
                        string message = JsonConvert.SerializeObject(ex) + Environment.NewLine;

                        file.Write(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt") + ": " + message);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        public static void Log(string v)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(v))
                {
                    var path = GetPath();
                    using (var file = new StreamWriter(path, true))
                    {
                        file.Write(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt") + ": " + v + Environment.NewLine);
                    }
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private static string GetPath()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var subFolderPath = Path.Combine(path, "WashMachine\\Application");
            if (!Directory.Exists(subFolderPath))
            {
                Directory.CreateDirectory(subFolderPath);
            }
            var fullPathDb = subFolderPath + "\\" + LOG_FILE_NAME;
            return fullPathDb;
        }
    }

}
