namespace WashMachine.Business.Models
{
    public class EmailModel
    {
        public string To { get; set; }
        public string Cc { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Attachments { get; set; }
    }
}
