namespace WashMachine.Business.Models
{
    public class OctopusSettingModel
    {
        public string XFileHour { get; set; }
        public string XFileMinute { get; set; }
        public string UploadHour { get; set; }
        public string UploadMinute { get; set; }
        public string DownloadHour { get; set; }
        public string DownloadMinute { get; set; }
        public string OctopusUploadFolder { get; set; }
        public string OctopusDownloadFolder { get; set; }
        public string XFileTime
        {
            get { return $"{XFileHour}{XFileMinute}"; }
        }
        public string UploadTime
        {
            get { return $"{UploadHour}{UploadMinute}"; }
        }
        public string DownloadTime
        {
            get { return $"{DownloadHour}{DownloadMinute}"; }
        }
    }
}
