using System;
using System.Collections.Generic;

namespace WashMachine.Repositories.Entities
{
    public partial class EmailLog
    {
        public int Id { get; set; }
        public string ShopCode { get; set; }
        public string Name { get; set; }
        public string DeviceId { get; set; }
        public string Time { get; set; }
        public string FunctionName { get; set; }
        public string Contents { get; set; }
        public int? Count { get; set; }
        public bool? Sent { get; set; }
        public DateTime? SentTime { get; set; }
        public DateTime? InsertTime { get; set; }
    }
}
