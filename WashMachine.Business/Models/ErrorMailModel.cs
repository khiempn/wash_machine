namespace WashMachine.Business.Models
{
    public class ErrorMailModel
    {
        public static string DisconnectError => "DisconnectError";
        public static string GenerationError => "GenerationError";
        public static string UploadFileError => "UploadFileError";
        public static string DownloadtError => "DownloadtError";

        public string EmailType { get; set; }

        public string ShopCode { get; set; }
        public string Name { get; set; }
        public string ShopName { get; set; }
        public string Time { get; set; }
        public string DeviceId { get; set; }

        public string FunctionName { get; set; }
    }

    public class MachineEmailModel : ErrorMailModel
    {
        public string Command { get; set; }
        public string MachineName { get; set; }
    }
}
