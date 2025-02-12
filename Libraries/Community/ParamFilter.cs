using System;
using System.Collections.Generic;

namespace Libraries
{
    public partial class ParamFilter
    {
        public ParamFilter()
        {
            Page = 1;
            PageSize = 10;
            SortBy = "ID";
            OrderBy = "DESC";
            Keyword = "";
            Enabled = true;
        }

        public string SortBy { get; set; }
        public string OrderBy { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }

        public string Status { get; set; }
        public string ParentCode { get; set; }

        public string Keyword { get; set; }
        public string Type { get; set; }
        public int ReferId { get; set; }
        public List<SelectItem> Refers { get; set; }
        public string Language { get; set; }
        public string Data { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? UserId { get; set; }
        public int TeamId { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public bool Enabled { get; set; }
        public string Key { get; set; }
        public int Id { get; set; }
        public string Month { get; set; }
        public string Include { get; set; }
        public string Exclude { get; set; }
    }
}
