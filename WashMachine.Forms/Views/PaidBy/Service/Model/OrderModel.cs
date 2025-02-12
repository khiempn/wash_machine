using System;

namespace WashMachine.Forms.Views.PaidBy.Service.Model
{
    public class OrderModel
    {
        public int Id { get; set; }
        public string ShopCode { get; set; }
        public string ShopName { get; set; }
        public string Location { get; set; }
        public float Amount { get; set; }
        public int Quantity { get; set; }
        public int PaymentId { get; set; }
        public int PaymentTypeId { get; set; }
        public string PaymentTypeName { get; set; }
        public int PaymentStatus { get; set; }
        public string DeviceId { get; set; }
        public string CardJson { get; set; }
        public DateTime? InsertTime { get; set; } = DateTime.Now;
        public DateTime? UpdateTime { get; set; } = DateTime.Now;
    }
}
