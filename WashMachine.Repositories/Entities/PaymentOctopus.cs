using System;
using System.Collections.Generic;

namespace WashMachine.Repositories.Entities
{
    public partial class PaymentOctopus
    {
        public int Id { get; set; }
        public string ShopCode { get; set; }
        public int? OrderId { get; set; }
        public string Message { get; set; }
        public string Received { get; set; }
        public int? PaymentStatus { get; set; }
        public bool Enabled { get; set; }
        public DateTime? InsertTime { get; set; }
        public DateTime? UpdateTime { get; set; }
    }
}
