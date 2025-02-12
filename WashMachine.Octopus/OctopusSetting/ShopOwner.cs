using System;
using System.Collections.Generic;

namespace WashMachine.Octopus.OctopusSetting
{
    public partial class ShopOwner
    {
        public int Id { get; set; }
        public int ShopId { get; set; }
        public int OwnerId { get; set; }
        public bool? Active { get; set; }
        public int? InsertUser { get; set; }
        public DateTime? InsertTime { get; set; }
        public int? UpdateUser { get; set; }
        public DateTime? UpdateTime { get; set; }

        public User Owner { get; set; }
        public Shop Shop { get; set; }
    }
}
