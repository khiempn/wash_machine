using System;
using System.Collections.Generic;
using System.Text;

namespace Libraries
{
    public class SelectItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Value { get; set; }
        public bool Selected { get; set; }
        public string Slug { get {
                return TextUtilities.ConvertToSlug(Name);
            }
        }
    }
}
