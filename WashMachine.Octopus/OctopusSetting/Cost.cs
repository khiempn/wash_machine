using System;
using System.Collections.Generic;

namespace WashMachine.Octopus.OctopusSetting
{
    public partial class Cost
    {
        public int Id { get; set; }
        public string ShopCode { get; set; }
        public int PricingId { get; set; }
        public int? CostType { get; set; }
        public string Name { get; set; }
        public decimal? Adult { get; set; }
        public decimal? Elderly { get; set; }
        public decimal? Child { get; set; }
        public int? InsertUser { get; set; }
        public DateTime? InsertTime { get; set; }
        public int? UpdateUser { get; set; }
        public DateTime? UpdateTime { get; set; }

        public Pricing Pricing { get; set; }
    }
}
