using System;
using System.Collections.Generic;

namespace WashMachine.Octopus.OctopusSetting
{
    public partial class Permission
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string Mission { get; set; }
        public bool? Access { get; set; }
        public bool? Append { get; set; }
        public bool? Edit { get; set; }
        public bool? Delete { get; set; }
        public bool? Active { get; set; }
        public DateTime? InsertTime { get; set; }
        public int? InsertUser { get; set; }
        public DateTime? UpdateTime { get; set; }
        public int? UpdateUser { get; set; }

        public User User { get; set; }
    }
}
