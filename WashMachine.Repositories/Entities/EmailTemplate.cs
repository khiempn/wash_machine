namespace WashMachine.Repositories.Entities
{
    public partial class EmailTemplate
    {
        public EmailTemplate()
        {
        }

        public int Id { get; set; }
        public string Type { get; set; }
        public string Subject { get; set; }
        public string Template { get; set; }
    }
}
