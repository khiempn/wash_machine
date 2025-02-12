using System;
using System.Collections.Generic;

namespace WashMachine.Octopus.OctopusSetting
{
    public partial class Order
    {
        public Order()
        {
            OrderDetail = new HashSet<OrderDetail>();
            Payment = new HashSet<Payment>();
        }

        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string SecureCode { get; set; }
        public string ShopCode { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public decimal? Price { get; set; }
        public int? Quantity { get; set; }
        public decimal? Amount { get; set; }
        public decimal? Discount { get; set; }
        public decimal? Tax { get; set; }
        public decimal? Paid { get; set; }
        public int? PaymentStatus { get; set; }
        public int? Status { get; set; }
        public int? OrderType { get; set; }
        public bool? IsFirstTime { get; set; }
        public int? PaymentType { get; set; }
        public int? PersonType { get; set; }
        public bool? Active { get; set; }
        public int? InsertUser { get; set; }
        public DateTime? InsertTime { get; set; }
        public int? UpdateUser { get; set; }
        public DateTime? UpdateTime { get; set; }
        public string ShopName { get; set; }
        public int? QueueNumber { get; set; }
        public string TicketCode { get; set; }
        public DateTime? BookingTime { get; set; }
        public string PaymentId { get; set; }
        public DateTime? StartTime { get; set; }

        public ICollection<OrderDetail> OrderDetail { get; set; }
        public ICollection<Payment> Payment { get; set; }
    }
}
