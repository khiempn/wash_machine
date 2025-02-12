namespace WashMachine.Repositories.Entities
{
    public class ShopCom
    {
        public int Id { get; set; }
        public int ShopId { get; set; }
        public string ComType { get; set; }
        public string ComName { get; set; }
        public int BaudRate { get; set; }
        public string Parity { get; set; }
        public int Data { get; set; }
        public string StopBits { get; set; }

        public Shop Shop { get; set; }
    }
}
