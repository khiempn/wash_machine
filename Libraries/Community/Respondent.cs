using System;
using System.Collections.Generic;
using System.Text;

namespace Libraries
{
    public class Respondent
    {
        public Respondent()
        {
            Name = string.Empty;
            Message = string.Empty;
        }
        
        public bool Success { get; set; }

        public string Name { get; set; }

        public string Message { get; set; }

        public string Code { get; set; }

        public object Data { get; set; }

        public int DataId { get; set; }
    }
}
