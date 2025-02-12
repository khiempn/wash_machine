using System;
using System.Collections.Generic;

namespace WashMachine.Repositories.Entities
{
    public partial class Setting
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public int? Type { get; set; }
    }
}
