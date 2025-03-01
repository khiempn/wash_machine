using WashMachine.Forms.Modules.PaidBy.Machine.Octopus.Email;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace WashMachine.Forms.Modules.PaidBy.Machine.Octopus
{
    public class OctopusLibrary
    {

        String OCTOPUS_LOG;
        const Int32 BUFSIZE = 1024;

        private static StreamWriter gLogfile;
        public static OctopusConfigModel OctopusConfig { get; set; } = new OctopusConfigModel();
        public stTimeVer TimeOver { get; set; }

        static byte[] gBuf = new byte[BUFSIZE];
        static IntPtr gBufPtr = Marshal.AllocHGlobal(BUFSIZE);
        StringBuilder gSBLog = new StringBuilder();
        StringBuilder gSBTmp = new StringBuilder();
        StringBuilder gSBDisplay = new StringBuilder();
        DateTime gGMT2000 = new DateTime(2000, 1, 1);
        UInt32 gDeviceId = 0;

        [DllImport("rwl.dll")]
        public static extern Int32 AddValue(Int32 val, byte avType, IntPtr ai);
        [DllImport("rwl.dll")]
        public static extern Int32 Deduct(Int32 val, IntPtr ai);
        [DllImport("rwl.dll")]
        public static extern Int32 GetExtraInfo(UInt32 com, UInt32 parm, IntPtr result);
        [DllImport("rwl.dll")]
        public static extern Int32 GetSysInfo(IntPtr result);
        [DllImport("rwl.dll")]
        public static extern Int32 HouseKeeping();
        [DllImport("rwl.dll")]
        public static extern Int32 InitComm(byte com, UInt32 subCom);
        [DllImport("rwl.dll")]
        public static extern Int32 Poll(byte subCom, byte timeout, IntPtr pollData);
        [DllImport("rwl.dll")]
        public static extern Int32 PollEx(byte subCom, byte timeout, IntPtr pollData);
        [DllImport("rwl.dll")]
        public static extern Int32 PortClose();
        [DllImport("rwl.dll")]
        public static extern Int32 Reset();
        [DllImport("rwl.dll")]
        public static extern Int32 TimeVer(IntPtr result);
        [DllImport("rwl.dll")]
        public static extern Int32 TxnAmt(Int32 val, Int32 rv, byte led, byte sound);
        [DllImport("rwl.dll")]
        public static extern Int32 WriteID(UInt32 id);
        [DllImport("rwl.dll")]
        public static extern Int32 XFile(IntPtr filename);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct stTimeVer
        {
            public UInt32 devID;
            public Int32 operID;
            public Int32 devTime;
            public Int32 compID;
            public Int32 keyVer;
            public Int32 eodVer;
            public Int32 blacklistVer;
            public Int32 firmVer;
            public Int32 cchsVer;
            public Int32 locID;
            public Int32 intBlacklistVer;
            public Int32 funcBlacklistVer;

            public string DeviceId { get { return devID.ToString("X"); } }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct stSysInfo
        {
            public Int32 actionListVer;
            public Int32 blkUpToDate;
            public Int32 rfu03;
            public Int32 rfu04;
            public Int32 rfu05;
            public Int32 rfu06;
            public Int32 rfu07;
            public Int32 rfu08;
            public Int32 rfu09;
            public Int32 rfu10;
            public Int32 rfu11;
            public Int32 rfu12;
        }

        private string GetOctopusLogPath()
        {
            return Path.Combine(Directory.GetCurrentDirectory(), "Octopus");
        }

        private void TryCreateLogFolder()
        {
            string logFolderPath = GetOctopusLogPath();
            if (Directory.Exists(logFolderPath))
            {
                Directory.CreateDirectory(logFolderPath);
            }
        }

        public void LogStart()
        {
            // Try to create the directory.
            TryCreateLogFolder();
            string logFolderPath = GetOctopusLogPath();
            var fileLogName = "eventlog_";
            ShopConfigModel shopConfig = Program.AppConfig.GetShopConfig();
            if (shopConfig != null)
            {
                fileLogName = shopConfig.Code + "_eventlog_";
            }
            OCTOPUS_LOG = Path.Combine(logFolderPath, $"{fileLogName}_{DateTime.Now.ToString("yyyyMMdd")}.log");
            if (gLogfile == null)
            {
                gLogfile = new StreamWriter(OCTOPUS_LOG, true);
            }
            LogEvent2("Log Start");
        }

        private void LogEvent1(String apiName, int rv)
        {
            gSBLog.Clear();
            gSBLog.Append(DateTime.Now.ToString("yy-MM-dd HH:mm:ss.fff")).Append(",").Append(apiName)
                    .Append(",<").Append(rv).Append(">");

            Console.WriteLine(gSBLog.ToString());
            gLogfile.WriteLine(gSBLog.ToString());
            gLogfile.Flush();
        }

        public void LogEvent2(String str)
        {
            gSBLog.Clear();
            gSBLog.Append(DateTime.Now.ToString("yy-MM-dd HH:mm:ss.fff")).Append(",").Append(str);

            Console.WriteLine(gSBLog.ToString());
            gLogfile.WriteLine(gSBLog.ToString());
            gLogfile.Flush();
        }

        private static void MemsetHGlobal(IntPtr ptr, Int32 len)
        {
            for (int i = 0; i < len; i++)
            {
                Marshal.WriteByte(ptr, i, 0x00);
            }
        }

        private string cvDate(int date)
        {
            DateTime dt = gGMT2000.AddSeconds(date);

            return dt.ToLocalTime().ToString("dd-MM-yyyy HH:mm:ss");
        }

        private byte getByteFromString(string str, int offset, int len)
        {
            byte b = 0;

            int val = Convert.ToInt32(str.Substring(offset, len), 16);
            b = (byte)val;
            return b;
        }

        private bool CheckPortAvailble(string comName, int baudrate)
        {
            try
            {
                SerialPort port = new SerialPort(comName, baudrate);
                var name = port.PortName;
                port.Open();
                port.Close();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public void ReInitComm(bool alway = false)
        {
            if (TimeOver.DeviceId == "0" || alway)
            {
                LogEvent2("ReInitComm");
                ExecuteInitComm();
                return;
            }
            var config = OctopusINI.ReadFileConfig();
            if (config == null) return;

            var comName = "COM" + config.Port;
            var comAvailable = CheckPortAvailble(comName, config.Port);
            if (comAvailable)
            {
                LogEvent2("ReInitComm.");
                ExecuteInitComm();
            }
        }

        public int ExecuteInitComm()
        {
            Logger.Log("Initial Step 2");
            var config = OctopusINI.ReadFileConfig();
            if (config == null)
            {
                return int.MinValue;
            }
            Logger.Log($"Initial Step 3 {JsonConvert.SerializeObject(config)}");
            try
            {
                byte port = config.Port;
                uint baudrate = config.Baudrate;

                Int32 r = InitComm(port, baudrate);
                Logger.Log($"Initial Step 4 {r}");

                gSBTmp.Clear();
                gSBTmp.Append("InitComm(").Append(port).Append(",").Append(baudrate).Append("),").Append(r);
                LogEvent2(gSBTmp.ToString());
                Logger.Log($"Initial Step 5 {r}");
                if (r == 0)
                {
                    TimeOver = ExcecuteTimeVer();
                    Logger.Log($"Initial Step 6 {r}");
                }
                else if (r == 100001)
                {
                    Logger.Log($"Initial Step 7 {r}");
                    IEmailService emailService = new EmailService();
                    emailService.SendDisconnectError();
                }
                Logger.Log($"Initial Step 8 {r}");
                return r;
            }
            catch (Exception ex)
            {
                LogEvent2("InitComm ERROR");
                Logger.Log(ex);
                return int.MinValue;
            }
        }

        private stTimeVer ExcecuteTimeVer()
        {
            Int32 r = 0;

            stTimeVer tv = new stTimeVer();
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(tv));

            try
            {
                r = TimeVer(ptr);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return tv;
            }

            LogEvent1("TimeVer()", r);
            if (r == 0)
            {
                tv = (stTimeVer)Marshal.PtrToStructure(ptr, typeof(stTimeVer));
                gDeviceId = tv.devID;
                gSBTmp.Clear();
                gSBTmp.Append("TimeVer(),").Append(r).Append(",")
                        .Append("Device ID = ").Append(tv.devID).Append(",")
                        .Append("Operator ID = ").Append(tv.operID).Append(",")
                        .Append("Device Time = ").Append(tv.devTime).Append(",")
                        .Append("Company ID = ").Append(tv.compID).Append(",")
                        .Append("Key Version = ").Append(tv.keyVer).Append(",")
                        .Append("EOD Version = ").Append(tv.eodVer).Append(",")
                        .Append("Blacklist Version = ").Append(tv.blacklistVer).Append(",")
                        .Append("Firmware Version = ").Append(tv.firmVer >> 16).Append("-")
                                .Append(String.Format("{0,5:00000}", tv.firmVer & 0xFFFF))
                                .Append(" (").Append(tv.firmVer).Append(")").Append(",")
                        .Append("CCHS Version = ").Append(tv.cchsVer).Append(",")
                        .Append("Location ID = ").Append(tv.locID).Append(",")
                        .Append("Interim Blacklist Version = ").Append(tv.intBlacklistVer).Append(",")
                        .Append("Functional Blacklist Version = ").Append(tv.funcBlacklistVer);
                LogEvent2(gSBTmp.ToString());
            }
            Marshal.FreeHGlobal(ptr);
            return tv;
        }

        public CardInfo ExcecutePoll(byte subcom = 0, byte timeout = 30)
        {
            try
            {
                Logger.Log($"ExcecutePoll Step 1 {subcom} {timeout}");

                int idx = 0;
                int i = 0;

                subcom = Convert.ToByte(subcom);
                timeout = Convert.ToByte(timeout);

                MemsetHGlobal(gBufPtr, BUFSIZE);
                Int32 r = Poll(subcom, timeout, gBufPtr);
                Logger.Log($"ExcecutePoll Step 2 {r}");

                CardInfo card = new CardInfo
                {
                    Rs = r,
                    DeviceId = TimeOver.DeviceId,
                    Transactions = new List<TransactionInfo>()
                };

                gSBTmp.Clear();
                gSBTmp.Append("Poll(").Append(subcom).Append(",").Append(timeout).Append(")");
                LogEvent1(gSBTmp.ToString(), r);

                gSBDisplay.Clear();
                gSBDisplay.Append("Poll(").Append(subcom).Append(",").Append(timeout).Append(") = ")
                        .Append(r).Append(Environment.NewLine);

                Int32 ERR_BASE = 100000;
                if (r <= ERR_BASE)
                {
                    Array.Clear(gBuf, 0, gBuf.Length);
                    Marshal.Copy(gBufPtr, gBuf, 0, gBuf.Length);
                    string pollDataString = System.Text.ASCIIEncoding.ASCII.GetString(gBuf).TrimEnd('\0');

                    gSBTmp.Clear();
                    gSBTmp.Append("Poll(").Append(subcom).Append(",").Append(timeout).Append("),").Append(pollDataString);
                    LogEvent2(gSBTmp.ToString());

                    char[] delimiter = { ',' };
                    string[] tokens = pollDataString.Split(delimiter);

                    gSBDisplay.Append("Card ID = ").Append(tokens[0]).Append(Environment.NewLine)
                            .Append("Customer Info = ").Append(tokens[1]).Append(Environment.NewLine);
                    card.CardId = tokens[0];
                    card.CustomerInfo = tokens[1];
                    if (tokens.Length > 2)
                    {
                        card.IDm = tokens[2];
                    }

                    if (subcom > 0)
                    {
                        gSBDisplay.Append("IDm = ").Append(tokens[2]).Append(Environment.NewLine);
                    }

                    if (subcom >= 2)
                    {
                        idx = 3;
                        gSBDisplay.Append(Environment.NewLine)
                            .Append(" SP |Amt     |Transaction Date/Time|Mach|SI        ").Append(Environment.NewLine)
                            .Append("----|--------|---------------------|----|----------").Append(Environment.NewLine);

                        for (i = 0; i < 10; i++)
                        {
                            var tran = new TransactionInfo();

                            UInt32 devId = Convert.ToUInt32(tokens[idx + 3]);
                            if ((gDeviceId & 0xFFFF) == devId)
                            {
                                gSBDisplay.Append("#");
                            }
                            else
                            {
                                gSBDisplay.Append(" ");
                            }
                            gSBDisplay.Append(String.Format("{0,3}|", tokens[idx]));
                            tran.SP = String.Format("{0,3}", tokens[idx]);

                            Int32 amt = Convert.ToInt32(tokens[idx + 1]);
                            gSBDisplay.Append(String.Format("{0:+$0.0;-$0.0;$0.0}|", ((Double)amt) / 10).PadLeft(9, ' '));
                            gSBDisplay.Append(cvDate(Convert.ToInt32(tokens[idx + 2]))).Append("  |")
                                    .Append(String.Format("{0,4}|", devId.ToString("X").PadLeft(4, '0')))
                                    .Append(tokens[idx + 4]).Append(Environment.NewLine);

                            tran.Amt = String.Format("{0:+$0.0;-$0.0;$0.0}", ((Double)amt) / 10).PadLeft(9, ' ');
                            tran.Date = cvDate(Convert.ToInt32(tokens[idx + 2]));
                            tran.Mach = string.Format("{0,4}", devId.ToString("X").PadLeft(4, '0'));
                            tran.SI = string.Format("{0,3}", tokens[idx + 4]);
                            card.Transactions.Add(tran);

                            idx += 5;
                        }
                    }
                }
                Logger.Log($"ExcecutePoll Step 3 {JsonConvert.SerializeObject(card)}");

                return card;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }

            return null;
        }

        public int ExcecuteDeduct(int val, int id)
        {
            string hexText = "0000" + id.ToString("X");
            string textBoxAddInfo = hexText.Substring(hexText.Length - 4);

            var textAI = textBoxAddInfo + "000000" + textBoxAddInfo;

            try
            {
                Array.Clear(gBuf, 0, gBuf.Length);
                gBuf[0] = getByteFromString(textAI, 0, 2);
                gBuf[1] = getByteFromString(textAI, 2, 2);
                gBuf[2] = getByteFromString(textAI, 4, 2);
                gBuf[3] = getByteFromString(textAI, 6, 2);
                gBuf[4] = getByteFromString(textAI, 8, 2);
                gBuf[5] = getByteFromString(textAI, 0, 2);
                gBuf[6] = getByteFromString(textAI, 2, 2);

            }
            catch (OverflowException ex)
            {
                Logger.Log("OverflowException");
                Logger.Log(ex);
                return -1;
            }
            catch (FormatException ex)
            {
                Logger.Log("FormatException");
                Logger.Log(ex);
                return -1;
            }

            MemsetHGlobal(gBufPtr, BUFSIZE);
            Marshal.Copy(gBuf, 0, gBufPtr, 7);
            var r = Deduct(val, gBufPtr);
            gSBTmp.Clear();
            gSBTmp.Append("Deduct(").Append(val).Append(",").Append(textAI).Append(")");
            LogEvent1(gSBTmp.ToString(), r);

            return r;
        }

        public ExtraInfo ExcecuteGetExtraInfo()
        {
            var info = new ExtraInfo();
            Int32 r = 0;
            UInt32 p1 = 0;
            UInt32 p2 = 1;

            MemsetHGlobal(gBufPtr, BUFSIZE);
            r = GetExtraInfo(p1, p2, gBufPtr);
            gSBTmp.Clear();
            gSBTmp.Append("GetExtraInfo(").Append(p1).Append(",").Append(p2).Append(")");
            LogEvent1(gSBTmp.ToString(), r);
            gSBDisplay.Clear();
            gSBDisplay.Append(gSBTmp.ToString()).Append(" = ").Append(r).Append(Environment.NewLine).Append(Environment.NewLine);
            if (r == 0)
            {
                Array.Clear(gBuf, 0, gBuf.Length);
                Marshal.Copy(gBufPtr, gBuf, 0, gBuf.Length);

                if (p1 == 0)
                { // LAVD
                    if (p2 == 1)
                    {
                        string lavd = System.Text.ASCIIEncoding.ASCII.GetString(gBuf).TrimEnd('\0');
                        gSBTmp.Clear();
                        gSBTmp.Append("GetExtraInfo(").Append(p1).Append(",").Append(p2).Append("),").Append(lavd);
                        LogEvent2(gSBTmp.ToString());

                        char[] delimiter = { ',' };
                        string[] tokens = lavd.Split(delimiter);
                        gSBDisplay.Append("Last Add Value Date = ").Append(tokens[0]).Append(Environment.NewLine)
                                .Append("Last Add Value Type = ").Append(tokens[1]).Append(Environment.NewLine)
                                .Append("Last AAVS Device ID (in hex) = ").Append(tokens[2]);

                        info.LastAddDate = tokens[0];
                        info.LastAddType = tokens[1];
                        info.LastAddDeviceId = tokens[2];
                        return info;
                    }
                    else
                    {
                        gSBDisplay.Append("Empty String is returned");
                    }
                }
                else if (p1 == 1)
                { // Alert message
                    UInt32 msgCode = BitConverter.ToUInt32(gBuf, 0);
                    gSBDisplay.Append("Message Code = ").Append(msgCode).Append(Environment.NewLine);

                    if (msgCode != 0)
                    {
                        byte[] b = new byte[204];
                        Buffer.BlockCopy(gBuf, 4, b, 0, 204); // Refer to OCL spec for the numbers
                        string msg = null;
                        if (p2 == 0)
                        { // UTF-16LE
                            msg = System.Text.UnicodeEncoding.Unicode.GetString(b).TrimEnd('\0');
                        }
                        else if (p2 == 1)
                        { // English
                            msg = System.Text.ASCIIEncoding.ASCII.GetString(b).TrimEnd('\0');
                        }
                        else if (p2 == 2)
                        { // Big-5
                            msg = System.Text.Encoding.GetEncoding(950).GetString(b).TrimEnd('\0');
                        }
                        else if (p2 == 3)
                        { // UTF-8
                            msg = System.Text.UTF8Encoding.UTF8.GetString(b).TrimEnd('\0');
                        }
                        else
                        {
                            msg = "";
                        }

                        gSBTmp.Clear();
                        gSBTmp.Append("GetExtraInfo(").Append(p1).Append(",").Append(p2).Append(")")
                                .Append(",Message Code = ").Append(msgCode).Append(",").Append(msg);
                        LogEvent2(gSBTmp.ToString());
                        gSBDisplay.Append(msg);
                    }
                    else
                    {
                        gSBDisplay.Append("Empty String is returned");
                    }
                }
                else if (p1 == 3)
                {
                    if (p2 == 1)
                    {
                        Array.Clear(gBuf, 0, gBuf.Length);
                        Marshal.Copy(gBufPtr, gBuf, 0, gBuf.Length);
                        UInt32 maxAllowAddValueAmt = BitConverter.ToUInt32(gBuf, 0);

                        gSBTmp.Clear();
                        gSBTmp.Append("GetExtraInfo(").Append(p1).Append(",").Append(p2).Append(")")
                                .Append(",Max Allowable Add Value Amount = ").Append(maxAllowAddValueAmt);
                        LogEvent2(gSBTmp.ToString());
                        gSBDisplay.Append("Max Allowable Add Value Amount = ").Append(maxAllowAddValueAmt);
                    }
                }
            }

            return info;
        }

        private bool IsCanRunXFile(DateTime dateTimeJob)
        {
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "jobs_tracking");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            if (DateTime.Now.CompareTo(dateTimeJob) >= 0)
            {
                string filePath = Path.Combine(folderPath, $"xfile_{dateTimeJob.ToString("MMddyyyy_hhmm")}.txt");
                if (File.Exists(filePath))
                {
                    return false;
                }
                else
                {
                    using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate))
                    {
                        byte[] info = new UTF8Encoding(true).GetBytes($"RUN JOB AT: {DateTime.Now}");
                        fs.Write(info, 0, info.Length);
                    };
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        private DateTime ParseDatetimeJob(string hour, string minute)
        {
            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, int.Parse(hour), int.Parse(minute), 0);
        }

        public void ExcecuteXFile()
        {
            if (!string.IsNullOrWhiteSpace(OctopusConfig.XFileHour)
                && !string.IsNullOrWhiteSpace(OctopusConfig.XFileMinute) 
                && IsCanRunXFile(ParseDatetimeJob(OctopusConfig.XFileHour, OctopusConfig.XFileMinute))
                && !Program.octopusService.IsUserUsingApplication())
            {
                Program.octopusService.ShowOutOfService();
                //HomeFrom.Browser.InvokeScript("xFileLocked");
                LogEvent2("Xfile is going to generate, will be executed when system is idle.");

                System.Threading.Thread thread = new System.Threading.Thread(ThreadXFile);
                //thread.IsBackground = true;            
                thread.Start();
            }
        }

        private void ThreadXFile()
        {
            System.Threading.Thread.Sleep(5000);
            try
            {
                LogEvent2("System idle, MPS File is generating, Displaying \"Out of service\"");

                Int32 r = 0;

                MemsetHGlobal(gBufPtr, BUFSIZE);
                r = XFile(gBufPtr);
                LogEvent1("XFile()", r);

                gSBTmp.Clear();

                if (r == 0)
                {
                    Array.Clear(gBuf, 0, gBuf.Length);
                    Marshal.Copy(gBufPtr, gBuf, 0, gBuf.Length);
                    String filename = System.Text.ASCIIEncoding.ASCII.GetString(gBuf).TrimEnd('\0');
                    gSBTmp.Append("XFile(),").Append(filename);
                    LogEvent2(gSBTmp.ToString());
                    LogEvent2("MPS File generation completed, system resuming");
                }
                else
                {
                    LogEvent2("MPS File generation has occurred an error");
                    IEmailService emailService = new EmailService();
                    var timeOver = ExcecuteTimeVer();
                    emailService.SendGenerationError(timeOver.DeviceId);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            finally
            {
                Program.octopusService.HideOutOfService();
            }
        }

        private bool IsCanRunUpload(DateTime dateTimeJob)
        {
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "jobs_tracking");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            if (DateTime.Now.CompareTo(dateTimeJob) >= 0)
            {
                string filePath = Path.Combine(folderPath, $"upload_{dateTimeJob.ToString("MMddyyyy_hhmm")}.txt");
                if (File.Exists(filePath))
                {
                    return false;
                }
                else
                {
                    using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate))
                    {
                        byte[] info = new UTF8Encoding(true).GetBytes($"RUN JOB AT: {DateTime.Now}");
                        fs.Write(info, 0, info.Length);
                    }
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        private bool IsCanRunDownload(DateTime dateTimeJob)
        {
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "jobs_tracking");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            if (DateTime.Now.CompareTo(dateTimeJob) >= 0)
            {
                string filePath = Path.Combine(folderPath, $"download_{dateTimeJob.ToString("MMddyyyy_hhmm")}.txt");
                if (File.Exists(filePath))
                {
                    return false;
                }
                else
                {
                    using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate))
                    {
                        byte[] info = new UTF8Encoding(true).GetBytes($"RUN JOB AT: {DateTime.Now}");
                        fs.Write(info, 0, info.Length);
                    };
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        private void DeleteEncryptFiles(string folder)
        {
            var fileNames = Directory.GetFiles(folder, "*.enc");
            try
            {
                foreach (string item in fileNames)
                {
                    File.Delete(item);
                }
            }
            catch { }
        }

        private List<string> MoveLogFileUploaded(string[] files)
        {
            var uploadedFolder = "OctopusUploaded";
            var folder = Directory.CreateDirectory(uploadedFolder);

            var moved = new List<string>();
            foreach (var path in files)
            {
                if (!File.Exists(path)) continue;

                var fileName = Path.GetFileName(path);
                var path2 = uploadedFolder + "\\" + fileName;

                try
                {
                    // Ensure that the target does not exist.
                    if (File.Exists(path2)) File.Delete(path2);

                    // Move the file.
                    File.Move(path, path2);
                    moved.Add(folder.FullName + "\\" + fileName);

                    // See if the original exists now.
                    if (!File.Exists(path))
                    {
                        Console.WriteLine("File: " + path + "no longer exists in upload folder");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
            }
            return moved;

        }

        private List<string> MoveFileUploaded(string[] files)
        {
            var moved = new List<string>();
            var config = OctopusINI.ReadFileConfig();
            var folderPath = config.EXCHANGE;
            if (!Directory.Exists(folderPath)) return moved;

            var uploadedFolder = Regex.Replace(folderPath, @"(\w+)$", "uploaded");
            var folder = Directory.CreateDirectory(uploadedFolder);

            foreach (var path in files)
            {
                if (!File.Exists(path)) continue;

                var fileName = Path.GetFileName(path);
                var path2 = uploadedFolder + "\\" + fileName;

                try
                {
                    // Ensure that the target does not exist.
                    if (File.Exists(path2)) File.Delete(path2);

                    // Move the file.
                    File.Move(path, path2);
                    moved.Add(folder.FullName + "\\" + fileName);

                    // See if the original exists now.
                    if (!File.Exists(path))
                    {
                        Console.WriteLine("File: " + path + "no longer exists in upload folder");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
            }
            return moved;

        }

        public void UploadFiles(Action<bool, string> done)
        {
            if (!string.IsNullOrWhiteSpace(OctopusConfig.UploadHour) && !string.IsNullOrWhiteSpace(OctopusConfig.UploadMinute) 
                && IsCanRunUpload(ParseDatetimeJob(OctopusConfig.UploadHour, OctopusConfig.UploadMinute)))
            {
                var config = OctopusINI.ReadFileConfig();
                var folderPath = config.EXCHANGE;
                if (!Directory.Exists(folderPath))
                {
                    Logger.Log(folderPath + " is not exists");
                    return;
                }

                var files = Directory.GetFiles(folderPath);
                if (files.Length == 0) return;

                var multiForm = new MultipartFormDataContent();
                multiForm.Add(new StringContent("DropCoin"), name: "FolderName");

                var logFolder = "Octopus";
                Directory.CreateDirectory(logFolder);
                var logFiles = Directory.GetFiles(logFolder, "*.log");
                var todayText = DateTime.Now.ToString("yyyyMMdd");
                var targetUrl = Program.AppConfig.AppHost + "/OctopusApi/UploadFiles";
                var uploadedFiles = new List<string>();
                try
                {
                    foreach (var item in files)
                    {
                        if (Regex.IsMatch(item, @"(\.enc)$")) continue;

                        var encryptFile = CryptLibrary.EncryptFile(item);
                        FileStream fs = File.OpenRead(encryptFile);
                        multiForm.Add(new StreamContent(fs), "Files", Path.GetFileName(encryptFile));
                        uploadedFiles.Add(item);
                    }

                    foreach (var item in logFiles)
                    {
                        if (Regex.IsMatch(item, @"(\.enc)$")) continue;
                        if (item.Contains(todayText)) continue;

                        var encryptFile = CryptLibrary.EncryptFile(item);
                        FileStream fs = File.OpenRead(encryptFile);
                        multiForm.Add(new StreamContent(fs), "Files", Path.GetFileName(encryptFile));
                        uploadedFiles.Add(item);
                    }
                    using (var client1 = new HttpClient())
                    {
                        var requestContent = new StringContent("{}", Encoding.UTF8, "application/json");
                        var response = client1.PostAsync(targetUrl, multiForm);
                        var content = response.Result.Content.ReadAsStringAsync();
                        var text = content.Result;
                        var result = JsonConvert.DeserializeObject<Respondent>(text);
                        if (!result.Success)
                        {
                            IEmailService emailService = new EmailService();
                            emailService.SendUploadFileError();
                            //Form1.Browser.InvokeScript("sendEmailUploadFail", MpsFileInfo.UploadFileError);
                        }
                        else
                        {
                            DeleteEncryptFiles(folderPath);
                            DeleteEncryptFiles(logFolder);

                            var moved = MoveFileUploaded(files);
                            var moved01 = MoveLogFileUploaded(logFiles);
                            moved.AddRange(moved01);
                            if (uploadedFiles.Count > 0)
                            {
                                var message = $"Uploaded {uploadedFiles.Count} files: {string.Join(", ", uploadedFiles)}";
                                if (uploadedFiles.Count == 1) message = $"Uploaded 1 file: {uploadedFiles[0]}";
                                LogEvent2(message);

                                var message2 = $"Moved {moved.Count} files to: {string.Join(", ", moved)}";
                                if (moved.Count == 1) message2 = $"Moved 1 file to: {moved[0]}";
                                LogEvent2(message2);
                                uploadedFiles.Clear();
                            }
                        }
                    }

                    done.Invoke(true, "Upload file(s) successfully!");
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                    IEmailService emailService = new EmailService();
                    emailService.SendUploadFileError();
                    //Form1.Browser.InvokeScript("sendEmailUploadFail", MpsFileInfo.UploadFileError);
                }
                if (uploadedFiles.Count > 0)
                {
                    var message = $"Fail to upload {uploadedFiles.Count} files: {string.Join(", ", uploadedFiles)}";
                    if (uploadedFiles.Count == 1) message = $"Fail to upload 1 file: {uploadedFiles[0]}";
                    LogEvent2(message);
                }
            }
        }

        public void DownloadFiles(Action<bool, string> done)
        {
            if (!string.IsNullOrWhiteSpace(OctopusConfig.DownloadHour)
                && !string.IsNullOrWhiteSpace(OctopusConfig.DownloadMinute)
                && IsCanRunDownload(ParseDatetimeJob(OctopusConfig.DownloadHour, OctopusConfig.DownloadMinute)))
            {
                var downloadMock = DateTime.Now.ToString("HHmm");
                var config = OctopusINI.ReadFileConfig();
                try
                {
                    using (var client = new HttpClient())
                    {
                        var urlFiles = Program.AppConfig.AppHost + "/OctopusApi/GetDownloadFilesInfo?time=" + downloadMock;
                        var responseInfo = client.GetAsync(urlFiles);
                        var textInfo = responseInfo.Result.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<Respondent>(textInfo.Result);
                        var listFiles = JsonConvert.DeserializeObject<List<string>>(result.Data.ToString());

                        if (listFiles.Count > 0)
                        {
                            var message = $"System will download {listFiles.Count} files.";
                            if (listFiles.Count == 1) message = $"System will download {listFiles.Count} file.";
                            LogEvent2(message);
                            foreach (var fileName in listFiles)
                            {
                                var url = Program.AppConfig.AppHost + "/OctopusApi/DownloadFile?filePath=" + fileName;
                                var fileBytes = client.GetByteArrayAsync(url).GetAwaiter().GetResult();
                                var outputPath = config.BLACKLIST + "\\" + fileName;
                                var encryptPath = outputPath + ".enc";
                                File.WriteAllBytes(encryptPath, fileBytes);
                                LogEvent2($"Downloaded: {outputPath}");

                                if (Regex.IsMatch(encryptPath, @"(\.enc)$"))
                                {
                                    CryptLibrary.DecryptFile(encryptPath);
                                    CryptLibrary.DeleteFile(encryptPath);
                                }
                            }
                        }
                    }

                    done.Invoke(true, "Download successfully!");
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                    IEmailService emailService = new EmailService();
                    emailService.SendDownloadError();
                    LogEvent2("An error occurred while downloading the files.");
                }
            }
        }
    }

    public class CardInfo
    {
        public string ShopCode { get; set; }
        public string ShopName { get; set; }
        public string DeviceId { get; set; }
        public int PaymentId { get; set; }
        public int PaymentTypeId { get; set; }
        public string PaymentTypeName { get; set; }
        public float Amount { get; set; }

        public int Rs { get; set; }
        public string CardId { get; set; }
        public DateTime CreateDated { get; set; }
        public string OctopusNo { get { return CardId; } }
        public int RemainValue { get { return Rs; } }
        public List<TransactionInfo> Transactions { get; set; }
        public string CustomerInfo { get; set; }
        public string IDm { get; set; }
        public string CardType
        {
            get
            {
                string info = CustomerInfo + string.Empty;
                string[] items = info.Split('-');
                if (items.Length > 0)
                {
                    return items[0];
                }
                return string.Empty;
            }
        }
        public string LastAddType { get; set; }
        public string LastAddDate { get; set; }
        public string CardJson { get; set; }
        public List<int> MessageCodes { get; set; }
    }

    public class ExtraInfo
    {
        public string LastAddDate { get; set; }
        public string LastAddType { get; set; }
        public string LastAddDeviceId { get; set; }
    }

    public class TransactionInfo
    {
        public string SP { get; set; }
        public string Amt { get; set; }
        public string Date { get; set; }
        public string Mach { get; set; }
        public string SI { get; set; }
    }

    public class OctopusINI
    {
        public byte Port { get; set; }
        public uint Baudrate { get; set; }
        public string Message { get; set; }
        public string EXCHANGE { get; set; }
        public string BLACKLIST { get; set; }
        public string FIRMWARE { get; set; }
        public string EOD { get; set; }
        public string CCHS { get; set; }
        //EXCHANGE
        //BLACKLIST
        //FIRMWARE
        //EOD
        public static OctopusINI ReadFileConfig()
        {
            var config = new OctopusINI();
            try
            {
                string fileName = Program.AppConfig.OctopusRwlInitPath;
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    fileName = @"C:\Windows\rwl.ini";
                }

                if (!File.Exists(fileName))
                {
                    return config;
                }
                var lines = File.ReadAllLines(fileName);
                foreach (var item in lines)
                {
                    var couple = item.Split('=');
                    if (couple.Length != 2) continue;
                    if (couple[0] == "PORT")
                    {
                        config.Port = byte.Parse(couple[1]);
                    }
                    if (couple[0] == "BAUD")
                    {
                        config.Baudrate = uint.Parse(couple[1]);
                    }
                }
                config.EXCHANGE = GetConfigValue(lines, "EXCHANGE");
                config.BLACKLIST = GetConfigValue(lines, "BLACKLIST");
                config.FIRMWARE = GetConfigValue(lines, "FIRMWARE");
                config.EOD = GetConfigValue(lines, "EOD");
                config.CCHS = GetConfigValue(lines, "CCHS");

            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return null;
            }
            return config;
        }
        private static string GetConfigValue(string[] lines, string key)
        {
            var bracketKey = $"[{key}]";
            for (var i = 0; i < lines.Length - 1; i++)
            {
                var line = lines[i].Trim();
                if (line == bracketKey)
                {
                    for (var j = i + 1; j < lines.Length; j++)
                    {
                        var lineValue = lines[j].Trim();
                        if (lineValue.Contains("="))
                        {
                            var couple = lineValue.Split('=');
                            return couple[1].Trim();
                        }
                        if (lineValue.Contains("[")) return string.Empty;
                    }
                }
            }
            return string.Empty;
        }
    }

    public class Respondent
    {
        public Respondent()
        {
            Name = string.Empty;
            Message = string.Empty;
        }

        public bool Success { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }
        public string Code { get; set; }
        public object Data { get; set; }
        public int DataId { get; set; }
    }
}
