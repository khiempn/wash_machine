using System;
using System.Collections.Generic;

namespace WashMachine.Octopus.OctopusSetting
{
    public partial class Pricing
    {
        public Pricing()
        {
            Cost = new HashSet<Cost>();
        }

        public int Id { get; set; }
        public string ShopCode { get; set; }
        public string Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Status { get; set; }
        public bool? Active { get; set; }
        public DateTime? InsertTime { get; set; }
        public int? InsertUser { get; set; }
        public DateTime? UpdateTime { get; set; }
        public int? UpdateUser { get; set; }

        public ICollection<Cost> Cost { get; set; }
    }
}
