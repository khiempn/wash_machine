using System.Collections.Generic;

namespace WashMachine.Web.Models
{
    public class ReportFilterModel
    {
        public string SearchCriteria { get; set; } = string.Empty;
        public string FromDate { get; set; } = string.Empty;
        public string ToDate { get; set; } = string.Empty;
    }

    public class CouponFilterModel : ReportFilterModel
    {
        public string ShopCode { get; set; } = string.Empty;
        public List<string> ShopCodeList { get; set; } = new List<string>();
    }

    public class OrderFilterModel : ReportFilterModel
    {
        public string ShopCode { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public List<string> ShopCodeList { get; set; } = new List<string>();
    }

    public class RunCommandErrorFilterModel : ReportFilterModel
    {
        public string ShopCode { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public List<string> ShopCodeList { get; set; } = new List<string>();
    }
}
