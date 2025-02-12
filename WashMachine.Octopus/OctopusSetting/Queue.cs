using System;
using System.Collections.Generic;

namespace WashMachine.Octopus.OctopusSetting
{
    public partial class Queue
    {
        public int Id { get; set; }
        public string ShopCode { get; set; }
        public string Name { get; set; }
        public int CostId { get; set; }
        public int AgeType { get; set; }
        public decimal? Price { get; set; }
        public string SecureCode { get; set; }
        public string Notes { get; set; }
        public int? Status { get; set; }
        public bool? Active { get; set; }
        public int? InsertUser { get; set; }
        public DateTime? InsertTime { get; set; }
        public int? UpdateUser { get; set; }
        public DateTime? UpdateTime { get; set; }
    }
}
