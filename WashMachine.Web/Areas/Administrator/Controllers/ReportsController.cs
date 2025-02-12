using AutoMapper;
using WashMachine.Business;
using WashMachine.Business.Interfaces;
using WashMachine.Business.Models;
using WashMachine.Business.Services;
using WashMachine.Web.AppCode.Attributes;
using WashMachine.Web.Models;
using Libraries.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;

namespace WashMachine.Web.Areas.Administrator.Controllers
{
    [UserAuthorize]
    public class ReportsController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IEnumerable<IBusiness> _business;
        private readonly IExportExcelService _exportExcelService;

        public ReportsController(IMapper mapper, IEnumerable<IBusiness> business, ExportExcelService exportExcelService)
        {
            _mapper = mapper;
            _business = business;
            _exportExcelService = exportExcelService;
        }

        [HttpGet]
        public IActionResult Coupon(CouponReportDataModel couponReportData)
        {
            if (couponReportData == null)
            {
                couponReportData = new CouponReportDataModel();
            }
            ShopService shopService = _business.GetService<ShopService>();

            couponReportData.Filter.ShopCodeList = shopService.GetShops().Select(s => s.Code).ToList();

            DateTime? fromDate = null;
            if (!string.IsNullOrWhiteSpace(couponReportData.Filter.FromDate))
            {
                fromDate = DateTime.Parse(couponReportData.Filter.FromDate, new CultureInfo("en-CA"));
            }

            DateTime? toDate = null;
            if (!string.IsNullOrWhiteSpace(couponReportData.Filter.ToDate))
            {
                toDate = DateTime.Parse(couponReportData.Filter.ToDate, new CultureInfo("en-CA"));
            }

            CouponService couponService = _business.GetService<CouponService>();
            List<CouponModel> coupons = couponService.GetCoupons()
                .Where(w => string.IsNullOrWhiteSpace(couponReportData.Filter.SearchCriteria) || $"{w.Code} {w.ShopName} {w.ShopCode}".IndexOf(couponReportData.Filter.SearchCriteria, StringComparison.OrdinalIgnoreCase) >= 0)
                .Where(w => fromDate == null || w.UsedDate >= fromDate.Value)
                .Where(w => toDate == null || w.UsedDate <= toDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59))
                .Where(w => string.IsNullOrWhiteSpace(couponReportData.Filter.ShopCode) || w.ShopCode.IndexOf(couponReportData.Filter.ShopCode, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();

            couponReportData.Coupons = coupons;
            couponReportData.Total = coupons.Count;
            return View(couponReportData);
        }

        [HttpPost]
        public IActionResult CouponExportToExcel(CouponReportDataModel couponReportData)
        {
            if (couponReportData == null)
            {
                couponReportData = new CouponReportDataModel();
            }

            DateTime? fromDate = null;
            if (!string.IsNullOrWhiteSpace(couponReportData.Filter.FromDate))
            {
                fromDate = DateTime.Parse(couponReportData.Filter.FromDate, new CultureInfo("en-CA"));
            }

            DateTime? toDate = null;
            if (!string.IsNullOrWhiteSpace(couponReportData.Filter.ToDate))
            {
                toDate = DateTime.Parse(couponReportData.Filter.ToDate, new CultureInfo("en-CA"));
            }

            CouponService couponService = _business.GetService<CouponService>();
            List<CouponModel> coupons = couponService.GetCoupons()
                .Where(w => string.IsNullOrWhiteSpace(couponReportData.Filter.SearchCriteria) || $"{w.Code} {w.ShopName} {w.ShopCode}".IndexOf(couponReportData.Filter.SearchCriteria, StringComparison.OrdinalIgnoreCase) >= 0)
                .Where(w => fromDate == null || w.UsedDate >= fromDate.Value)
                .Where(w => toDate == null || w.UsedDate <= toDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59))
                .Where(w => string.IsNullOrWhiteSpace(couponReportData.Filter.ShopCode) || w.ShopCode.IndexOf(couponReportData.Filter.SearchCriteria, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();

            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("No", typeof(int));
            dataTable.Columns.Add("Shop Code", typeof(string));
            dataTable.Columns.Add("Shop Name", typeof(string));
            dataTable.Columns.Add("Coupon Code", typeof(string));
            dataTable.Columns.Add("Amount", typeof(string));
            dataTable.Columns.Add("Is Used", typeof(string));
            dataTable.Columns.Add("Used Date", typeof(string));

            int index = 1;
            foreach (CouponModel coupon in coupons)
            {
                dataTable.Rows.Add(index,
                    coupon.ShopCode,
                    coupon.ShopName,
                    coupon.Code,
                    coupon.Discount.ToString(),
                    coupon.IsUsed ? "Used" : string.Empty,
                    coupon.UsedDate.ToString("MM/dd/yyyy hh:mm:ss tt")
                );
                index++;
            }

            byte[] fileContents = _exportExcelService.Export(dataTable);

            return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "CouponReport.xlsx");
        }

        [HttpGet]
        public IActionResult Order(OrderReportDataModel orderReportData)
        {
            try
            {
                if (orderReportData == null)
                {
                    orderReportData = new OrderReportDataModel();
                }
                ShopService shopService = _business.GetService<ShopService>();

                orderReportData.Filter.ShopCodeList = shopService.GetShops().Select(s => s.Code).ToList();

                DateTime? fromDate = null;
                if (!string.IsNullOrWhiteSpace(orderReportData.Filter.FromDate))
                {
                    fromDate = DateTime.Parse(orderReportData.Filter.FromDate, new CultureInfo("en-CA"));
                }

                DateTime? toDate = null;
                if (!string.IsNullOrWhiteSpace(orderReportData.Filter.ToDate))
                {
                    toDate = DateTime.Parse(orderReportData.Filter.ToDate, new CultureInfo("en-CA"));
                }

                OrderService orderService = _business.GetService<OrderService>();
                List<OrderModel> orders = orderService.GetOrders()
                    .Where(w => w.PaymentTypeName != "Coupon")
                    .Where(w => string.IsNullOrWhiteSpace(orderReportData.Filter.SearchCriteria) || $"{w.PaymentTypeName} {w.ShopName} {w.ShopCode}".IndexOf(orderReportData.Filter.SearchCriteria, StringComparison.OrdinalIgnoreCase) >= 0)
                    .Where(w => fromDate == null || w.InsertTime >= fromDate.Value)
                    .Where(w => toDate == null || w.InsertTime <= toDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59))
                    .Where(w => string.IsNullOrWhiteSpace(orderReportData.Filter.ShopCode) || w.ShopCode.IndexOf(orderReportData.Filter.ShopCode, StringComparison.OrdinalIgnoreCase) >= 0)
                    .Where(w => string.IsNullOrWhiteSpace(orderReportData.Filter.PaymentMethod) || w.PaymentTypeName.IndexOf(orderReportData.Filter.PaymentMethod, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();

                orderReportData.Orders = orders;
                orderReportData.Total = orders.Count;
                return View(orderReportData);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpPost]
        public IActionResult OrderExportToExcel(OrderReportDataModel orderReportData)
        {
            if (orderReportData == null)
            {
                orderReportData = new OrderReportDataModel();
            }

            DateTime? fromDate = null;
            if (!string.IsNullOrWhiteSpace(orderReportData.Filter.FromDate))
            {
                fromDate = DateTime.Parse(orderReportData.Filter.FromDate, new CultureInfo("en-CA"));
            }

            DateTime? toDate = null;
            if (!string.IsNullOrWhiteSpace(orderReportData.Filter.ToDate))
            {
                toDate = DateTime.Parse(orderReportData.Filter.ToDate, new CultureInfo("en-CA"));
            }

            OrderService orderService = _business.GetService<OrderService>();
            List<OrderModel> orders = orderService.GetOrders()
                .Where(w => w.PaymentTypeName != "Coupon")
                .Where(w => string.IsNullOrWhiteSpace(orderReportData.Filter.SearchCriteria) || $"{w.PaymentTypeName} {w.ShopName} {w.ShopCode}".IndexOf(orderReportData.Filter.SearchCriteria, StringComparison.OrdinalIgnoreCase) >= 0)
                .Where(w => fromDate == null || w.InsertTime >= fromDate.Value)
                .Where(w => toDate == null || w.InsertTime <= toDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59))
                .Where(w => string.IsNullOrWhiteSpace(orderReportData.Filter.ShopCode) || w.ShopCode.IndexOf(orderReportData.Filter.SearchCriteria, StringComparison.OrdinalIgnoreCase) >= 0)
                .Where(w => string.IsNullOrWhiteSpace(orderReportData.Filter.PaymentMethod) || w.PaymentTypeName.IndexOf(orderReportData.Filter.PaymentMethod, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();

            DataTable dataTable = new DataTable();

            dataTable.Columns.Add("No", typeof(int));
            dataTable.Columns.Add("Shop Code", typeof(string));
            dataTable.Columns.Add("Shop Name", typeof(string));
            dataTable.Columns.Add("Machine Code", typeof(string));
            dataTable.Columns.Add("Date", typeof(string));
            dataTable.Columns.Add("Location", typeof(string));
            dataTable.Columns.Add("Payment Method", typeof(string));
            dataTable.Columns.Add("Amount", typeof(string));
            dataTable.Columns.Add("Status", typeof(string));
            dataTable.Columns.Add("Messenger", typeof(string));

            int index = 1;
            foreach (OrderModel order in orders)
            {
                dataTable.Rows.Add(index,
                    order.ShopCode,
                    order.ShopName,
                    order.DeviceId,
                    order.InsertTime.Value.ToString("MM/dd/yyyy hh:mm:ss tt"),
                    order.Location,
                    order.PaymentTypeName,
                    order.Amount,
                    order.PaymentStatus == (int)(PaymentStatus.Paid) ? "Success" : "Failed",
                    order.PaymentStatus == (int)(PaymentStatus.Paid) ? "OK" : ""
                );
                index++;
            }

            byte[] fileContents = _exportExcelService.Export(dataTable);

            return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "OrdersReport.xlsx");
        }
    }
}
