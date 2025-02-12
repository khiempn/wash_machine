using System;
using System.Collections.Generic;

namespace WashMachine.Octopus.OctopusSetting
{
    public partial class Payment
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string Message { get; set; }
        public string Received { get; set; }
        public int? PaymentType { get; set; }
        public int? PaymentStatus { get; set; }
        public bool? Enabled { get; set; }
        public DateTime? InsertTime { get; set; }
        public DateTime? UpdateTime { get; set; }

        public Order Order { get; set; }
    }
}
