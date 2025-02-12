using Newtonsoft.Json;
using System;
using System.IO;

namespace Libraries
{
    public class FileUtilities
    {
        public const string OK = "OK";
        public static void LogFile(Exception e, string path = "")
        {
            if (e == null) return;
            var exception = e;
            var i = 0;
            while (exception.InnerException != null)
            {
                i++;
                if (i > 10) break;
                exception = exception.InnerException;
            }
            WriteFile(exception.Message, string.Format("Url: {0}{1}StackTrace: {2}", path, Environment.NewLine, exception.StackTrace));
        }

        public static void LogFile(string text)
        {
            WriteFile(text);
        }

        public static void LogFile(string text, object obj)
        {
            WriteFile(text + JsonConvert.SerializeObject(obj));
        }

        public static void LogFile(string note, object obj, string fileLogName)
        {
            var text = note;
            if (obj != null) text += " " + JsonConvert.SerializeObject(obj);
            WriteFile(text, "", fileLogName);
        }

        private static void WriteFile(string text, string stackTrace = "", string fileLogName = "")
        {
            var logFolder = string.Format("{0}/wwwroot/Logs", System.IO.Directory.GetCurrentDirectory());
            var fileName = string.Format("{0}/Exception_{1}.txt", logFolder, DateTime.Now.ToString("yyyyMMdd"));
            if (!string.IsNullOrEmpty(fileLogName))
            {
                fileName = string.Format("{0}/{1}_{2}.txt", logFolder, fileLogName, DateTime.Now.ToString("yyyyMMdd"));
            }

            try
            {
                // Try to create the directory.
                Directory.CreateDirectory(logFolder);

                // This text is added only once to the file.
                if (!File.Exists(fileName))
                {
                    // Create a file to write to.
                    using (StreamWriter sw = File.CreateText(fileName))
                    {
                        sw.WriteLine(string.Format("Created time: {0} Time Zone {1}", DateTime.Now.ToString("HH:mm:ss"), TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now)));
                    }
                }

                // if it is not deleted.
                using (StreamWriter sw = File.AppendText(fileName))
                {

                    var url = string.Empty;
                    var clientIp = "0.0.0.0";

                    var message = string.Format("{0}Msg: {1} Time: {2} IP: {3}", url, text, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), clientIp);
                    sw.WriteLine(message);
                    if (!string.IsNullOrEmpty(stackTrace))
                    {
                        //sw.WriteLine(exception.StackTrace.Substring(0, 900));
                        sw.WriteLine(stackTrace);
                    }
                    sw.Flush();
                }
            }
            catch (Exception ex) { }
        }

        public static void ClearLog()
        {
            var logFolder = string.Format("{0}/wwwroot/Logs", System.IO.Directory.GetCurrentDirectory());
            var fileName = string.Format("{0}/Exception_{1}.txt", logFolder, DateTime.Now.ToString("yyyyMMdd"));

            try
            {
                // Try to create the directory.
                Directory.CreateDirectory(logFolder);

                // This text is added only once to the file.
                if (!File.Exists(fileName))
                {
                    // Create a file to write to.
                    using (StreamWriter sw = File.CreateText(fileName))
                    {
                        sw.WriteLine(string.Format("Created time: {0} Time Zone {1}", DateTime.Now.ToString("HH:mm:ss"), TimeZone.CurrentTimeZone.GetUtcOffset(System.DateTime.Now)));
                    }
                }

                File.WriteAllText(fileName, String.Empty);
            }
            catch (Exception) { }
        }

        public static string ReadFile(string filePath)
        {
            var fileName = string.Format("{0}/{1}", Directory.GetCurrentDirectory(), filePath);
            var text = string.Empty;
            if (File.Exists(fileName))
            {
                text = File.ReadAllText(fileName);
            }
            return text;
        }

        public static bool FileExists(string filePath)
        {
            var fileName = string.Format("{0}/wwwroot{1}", Directory.GetCurrentDirectory(), filePath);
            if (!File.Exists(fileName))
            {
                fileName = string.Format("{0}{1}", Directory.GetCurrentDirectory(), filePath);
                if (!File.Exists(fileName))
                {
                    LogFile("File not exists: " + fileName);
                    return false;
                }
            }
            return true;
        }
    }
}
