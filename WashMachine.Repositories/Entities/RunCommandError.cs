using System;

namespace WashMachine.Repositories.Entities
{
    public partial class RunCommandError
    {
        public int Id { get; set; }
        public string Command { get; set; }
        public int OrderId { get; set; }
        public string MachineName { get; set; }
        public string ShopCode { get; set; }
        public string ShopName { get; set; }
        public float Amount { get; set; }
        public string OctopusNo { get; set; }
        public string BuyerAccountID { get; set; }
        public string PaymentTypeName { get; set; }
        public DateTime InsertTime { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
