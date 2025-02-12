namespace WashMachine.Business.Models
{
    public class EmailSettingModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        /// <summary>
        /// using for cc email
        /// </summary>
        public string Receiver { get; set; }
        public string From { get; set; }
        public int Port { get; set; } = 587;
    }
}
