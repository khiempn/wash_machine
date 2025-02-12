using WashMachine.Business.Models;
using System.Collections.Generic;

namespace WashMachine.Web.Models
{
    public class ReportDataModel
    {
    }

    public class CouponReportDataModel : ReportDataModel
    {
        public CouponFilterModel Filter { get; set; } = new CouponFilterModel();
        public List<CouponModel> Coupons { get; set; } = new List<CouponModel>();
        public int Total { get; set; }
    }

    public class OrderReportDataModel : ReportDataModel
    {
        public OrderFilterModel Filter { get; set; } = new OrderFilterModel();
        public List<OrderModel> Orders { get; set; } = new List<OrderModel>();
        public int Total { get; set; }
    }
}
