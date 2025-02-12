using System;

namespace WashMachine.Business.Models
{
    public class PaymentModel
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string ShopCode { get; set; }
        public string ShopName { get; set; }
        public float Amount { get; set; }
        public int PaymentTypeId { get; set; }
        public string PaymentTypeName { get; set; }
        public int PaymentStatus { get; set; }
        public string Message { get; set; }
        public DateTime InsertTime { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
