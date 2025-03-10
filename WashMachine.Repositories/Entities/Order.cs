using System;
using System.Collections.Generic;

namespace WashMachine.Repositories.Entities
{
    public partial class Order
    {
        public Order()
        {
            Payment = new HashSet<Payment>();
        }
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
        public string OctopusNo { get; set; }
        public string Message { get; set; }
        public DateTime? InsertTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public ICollection<Payment> Payment { get; set; }
    }
}
