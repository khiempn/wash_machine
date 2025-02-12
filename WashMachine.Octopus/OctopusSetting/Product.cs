using System;
using System.Collections.Generic;

namespace WashMachine.Octopus.OctopusSetting
{
    public partial class Product
    {
        public int Id { get; set; }
        public string ShopCode { get; set; }
        public int? CategoryId { get; set; }
        public int? Bout { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Name2 { get; set; }
        public decimal? Price { get; set; }
        public decimal? Sale { get; set; }
        public string Keywords { get; set; }
        public string Description { get; set; }
        public string Brief { get; set; }
        public string Contents { get; set; }
        public string Photos { get; set; }
        public string Notes { get; set; }
        public int? Type { get; set; }
        public int? Status { get; set; }
        public bool? Active { get; set; }
        public int? InsertUser { get; set; }
        public DateTime? InsertTime { get; set; }
        public int? UpdateUser { get; set; }
        public DateTime? UpdateTime { get; set; }
    }
}
