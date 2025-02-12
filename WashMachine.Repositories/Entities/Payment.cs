using System;

namespace WashMachine.Repositories.Entities
{
    public partial class Payment
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string ShopCode { get; set; }
        public string ShopName { get; set; }
        public float Amount { get; set; }
        public int PaymentTypeId { get; set; }
        public string PaymentTypeName { get; set; }
        //1 Pendding 2 Cancel 3 Success
        public int PaymentStatus { get; set; }
        public string Message { get; set; }
        public DateTime InsertTime { get; set; }
        public DateTime UpdateTime { get; set; }
        public Order Order { get; set; }
    }
}
