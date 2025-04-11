using AutoMapper;
using WashMachine.Business;
using WashMachine.Business.Interfaces;
using WashMachine.Business.Models;
using WashMachine.Business.Services;
using WashMachine.Repositories.Entities;
using WashMachine.Web.AppCode.Attributes;
using WashMachine.Web.Models;
using Libraries.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using OfficeOpenXml.Style;

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
            var systemInfo = HttpContext.GetSystemInfo();

            couponReportData.Filter.ShopCodeList = shopService.GetShops().Select(s => s.Code).ToList();
            if (systemInfo.User.IsAdmin == false)
            {
                couponReportData.Filter.ShopCodeList = couponReportData.Filter.ShopCodeList.Where(w => w == systemInfo.User.ShopOwner?.Code).ToList();
                couponReportData.Filter.ShopCode = systemInfo.User.ShopOwner?.Code;
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
                .Where(w => string.IsNullOrWhiteSpace(couponReportData.Filter.ShopCode) || w.ShopCode.IndexOf(couponReportData.Filter.ShopCode, StringComparison.OrdinalIgnoreCase) >= 0)
                .OrderByDescending(o => o.Id)
                .ToList();

            if (systemInfo.User.IsAdmin == false)
            {
                coupons = coupons.Where(w => w.ShopCode == systemInfo.User.ShopOwner?.Code).ToList();
            }

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
                .OrderByDescending(o => o.Id)
                .ToList();

            var systemInfo = HttpContext.GetSystemInfo();

            if (systemInfo.User.IsAdmin == false)
            {
                coupons = coupons.Where(w => w.ShopCode == systemInfo.User.ShopOwner?.Code).ToList();
            }

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

                var systemInfo = HttpContext.GetSystemInfo();

                orderReportData.Filter.ShopCodeList = shopService.GetShops().Select(s => s.Code).ToList();

                if (systemInfo.User.IsAdmin == false)
                {
                    orderReportData.Filter.ShopCodeList = orderReportData.Filter.ShopCodeList.Where(w => w == systemInfo.User.ShopOwner?.Code).ToList();
                    orderReportData.Filter.ShopCode = systemInfo.User.ShopOwner?.Code;
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
                    .Where(w => string.IsNullOrWhiteSpace(orderReportData.Filter.ShopCode) || w.ShopCode.IndexOf(orderReportData.Filter.ShopCode, StringComparison.OrdinalIgnoreCase) >= 0)
                    .Where(w => string.IsNullOrWhiteSpace(orderReportData.Filter.PaymentMethod) || w.PaymentTypeName.IndexOf(orderReportData.Filter.PaymentMethod, StringComparison.OrdinalIgnoreCase) >= 0)
                    .OrderByDescending(o => o.Id)
                    .ToList();


                if (systemInfo.User.IsAdmin == false)
                {
                    orders = orders.Where(w => w.ShopCode == systemInfo.User.ShopOwner?.Code).ToList();
                }

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
                .OrderByDescending(o => o.Id)
                .ToList();


            var systemInfo = HttpContext.GetSystemInfo();

            if (systemInfo.User.IsAdmin == false)
            {
                orders = orders.Where(w => w.ShopCode == systemInfo.User.ShopOwner?.Code).ToList();
            }

            DataTable dataTable = new DataTable();

            dataTable.Columns.Add("No", typeof(int));
            dataTable.Columns.Add("Shop Code", typeof(string));
            dataTable.Columns.Add("Shop Name", typeof(string));
            dataTable.Columns.Add("Location", typeof(string));
            dataTable.Columns.Add("Payment Method", typeof(string));
            dataTable.Columns.Add("Receipt No", typeof(string));
            dataTable.Columns.Add("Transaction Date / Time", typeof(string));
            dataTable.Columns.Add("Device ID", typeof(string));
            dataTable.Columns.Add("Octopus ID", typeof(string));
            dataTable.Columns.Add("Buyer Account ID (For Eft)", typeof(string));
            dataTable.Columns.Add("Usage", typeof(string));
            dataTable.Columns.Add("Transaction Amount", typeof(string));
            dataTable.Columns.Add("Status", typeof(string));
            dataTable.Columns.Add("Messenger", typeof(string));

            int index = 1;
            foreach (OrderModel order in orders)
            {
                string status = string.Empty;

                if (order.PaymentStatus == (int)(PaymentStatus.Completed))
                {
                    status = "Completed";
                }
                else if (order.PaymentStatus == (int)(PaymentStatus.Failed))
                {
                    status = "Failed";
                }
                else if (order.PaymentStatus == (int)(PaymentStatus.InCompleted))
                {
                    status = "Incompleted";
                }
                else if (order.PaymentStatus == (int)(PaymentStatus.Cancel))
                {
                    status = "Cancel";
                }
                else if (order.PaymentStatus == (int)(PaymentStatus.Pending))
                {
                    status = "Pending";
                }

                dataTable.Rows.Add(index,
                    order.ShopCode,
                    order.ShopName,
                    order.Location,
                    order.PaymentTypeName,
                    order.Id.ToString().PadLeft(8, '0'),
                    order.InsertTime.Value.ToString("MM/dd/yyyy hh:mm:ss tt"),
                    order.DeviceId,
                    order.OctopusNo,
                    order.BuyerAccountID,
                    order.PaymentTypeName == "Octopus" ? "Deduct" : "Eft",
                    order.Amount,
                    status,
                    order.Message
                );
                index++;
            }

            byte[] fileContents = _exportExcelService.Export(dataTable);

            return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "OrdersReport.xlsx");
        }

        [HttpGet]
        public IActionResult RunCommandError(RunCommandErrorDataModel runCommandErrorData)
        {
            try
            {
                if (runCommandErrorData == null)
                {
                    runCommandErrorData = new RunCommandErrorDataModel();
                }
                ShopService shopService = _business.GetService<ShopService>();

                var systemInfo = HttpContext.GetSystemInfo();

                runCommandErrorData.Filter.ShopCodeList = shopService.GetShops().Select(s => s.Code).ToList();

                if (systemInfo.User.IsAdmin == false)
                {
                    runCommandErrorData.Filter.ShopCodeList = runCommandErrorData.Filter.ShopCodeList.Where(w => w == systemInfo.User.ShopOwner?.Code).ToList();
                    runCommandErrorData.Filter.ShopCode = systemInfo.User.ShopOwner?.Code;
                }

                DateTime? fromDate = null;
                if (!string.IsNullOrWhiteSpace(runCommandErrorData.Filter.FromDate))
                {
                    fromDate = DateTime.Parse(runCommandErrorData.Filter.FromDate, new CultureInfo("en-CA"));
                }

                DateTime? toDate = null;
                if (!string.IsNullOrWhiteSpace(runCommandErrorData.Filter.ToDate))
                {
                    toDate = DateTime.Parse(runCommandErrorData.Filter.ToDate, new CultureInfo("en-CA"));
                }

                RunCommandErrorService runCommandErrorService = _business.GetService<RunCommandErrorService>();
                List<RunCommandErrorModel> runCommandErrors = runCommandErrorService.GetRunCommandErrors()
                    .Where(w => w.PaymentTypeName != "Coupon")
                    .Where(w => $"{w.PaymentTypeName} {w.ShopName} {w.ShopCode}".IndexOf(runCommandErrorData.Filter.SearchCriteria, StringComparison.OrdinalIgnoreCase) >= 0)
                    .Where(w => fromDate == null || w.InsertTime >= fromDate.Value)
                    .Where(w => toDate == null || w.InsertTime <= toDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59))
                    .Where(w => string.IsNullOrWhiteSpace(runCommandErrorData.Filter.ShopCode) || w.ShopCode.IndexOf(runCommandErrorData.Filter.ShopCode, StringComparison.OrdinalIgnoreCase) >= 0)
                    .Where(w => string.IsNullOrWhiteSpace(runCommandErrorData.Filter.PaymentMethod) || w.PaymentTypeName.IndexOf(runCommandErrorData.Filter.PaymentMethod, StringComparison.OrdinalIgnoreCase) >= 0)
                    .OrderByDescending(o => o.Id)
                    .ToList();


                if (systemInfo.User.IsAdmin == false)
                {
                    runCommandErrors = runCommandErrors.Where(w => w.ShopCode == systemInfo.User.ShopOwner?.Code).ToList();
                }

                runCommandErrorData.RunCommandErrors = runCommandErrors;
                runCommandErrorData.Total = runCommandErrors.Count;
                return View(runCommandErrorData);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpPost]
        public IActionResult RunCommandErrorExportToExcel(RunCommandErrorDataModel runCommandErrorData)
        {
            try
            {
                if (runCommandErrorData == null)
                {
                    runCommandErrorData = new RunCommandErrorDataModel();
                }
                ShopService shopService = _business.GetService<ShopService>();

                var systemInfo = HttpContext.GetSystemInfo();

                runCommandErrorData.Filter.ShopCodeList = shopService.GetShops().Select(s => s.Code).ToList();

                if (systemInfo.User.IsAdmin == false)
                {
                    runCommandErrorData.Filter.ShopCodeList = runCommandErrorData.Filter.ShopCodeList.Where(w => w == systemInfo.User.ShopOwner?.Code).ToList();
                    runCommandErrorData.Filter.ShopCode = systemInfo.User.ShopOwner?.Code;
                }

                DateTime? fromDate = null;
                if (!string.IsNullOrWhiteSpace(runCommandErrorData.Filter.FromDate))
                {
                    fromDate = DateTime.Parse(runCommandErrorData.Filter.FromDate, new CultureInfo("en-CA"));
                }

                DateTime? toDate = null;
                if (!string.IsNullOrWhiteSpace(runCommandErrorData.Filter.ToDate))
                {
                    toDate = DateTime.Parse(runCommandErrorData.Filter.ToDate, new CultureInfo("en-CA"));
                }

                RunCommandErrorService runCommandErrorService = _business.GetService<RunCommandErrorService>();
                List<RunCommandErrorModel> runCommandErrors = runCommandErrorService.GetRunCommandErrors()
                    .Where(w => w.PaymentTypeName != "Coupon")
                    .Where(w => $"{w.PaymentTypeName} {w.ShopName} {w.ShopCode}".IndexOf(runCommandErrorData.Filter.SearchCriteria, StringComparison.OrdinalIgnoreCase) >= 0)
                    .Where(w => fromDate == null || w.InsertTime >= fromDate.Value)
                    .Where(w => toDate == null || w.InsertTime <= toDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59))
                    .Where(w => string.IsNullOrWhiteSpace(runCommandErrorData.Filter.ShopCode) || w.ShopCode.IndexOf(runCommandErrorData.Filter.ShopCode, StringComparison.OrdinalIgnoreCase) >= 0)
                    .Where(w => string.IsNullOrWhiteSpace(runCommandErrorData.Filter.PaymentMethod) || w.PaymentTypeName.IndexOf(runCommandErrorData.Filter.PaymentMethod, StringComparison.OrdinalIgnoreCase) >= 0)
                    .OrderByDescending(o => o.Id)
                    .ToList();


                DataTable dataTable = new DataTable();

                dataTable.Columns.Add("No", typeof(int));
                dataTable.Columns.Add("Receipt No", typeof(string));
                dataTable.Columns.Add("Shop Code", typeof(string));
                dataTable.Columns.Add("Shop Name", typeof(string));
                dataTable.Columns.Add("Machine Name", typeof(string));
                dataTable.Columns.Add("Command", typeof(string));
                dataTable.Columns.Add("Payment Type Name", typeof(string));
                dataTable.Columns.Add("Transaction Amount", typeof(string));
                dataTable.Columns.Add("Octopus ID", typeof(string));
                dataTable.Columns.Add("Buyer Account ID (For Eft)", typeof(string));
                dataTable.Columns.Add("Error Date", typeof(string));

                int index = 1;
                foreach (RunCommandErrorModel runCommand in runCommandErrors)
                {
                    dataTable.Rows.Add(index,
                        runCommand.OrderId.ToString().PadLeft(8, '0'),
                        runCommand.ShopCode,
                        runCommand.ShopName,
                        runCommand.MachineName,
                        runCommand.Command,
                        runCommand.PaymentTypeName == "Octopus" ? "Deduct" : "Eft",
                        runCommand.Amount,
                        runCommand.OctopusNo,
                        runCommand.BuyerAccountID,
                        runCommand.InsertTime.ToString("MM/dd/yyyy hh:mm:ss tt")
                    );
                    index++;
                }

                byte[] fileContents = _exportExcelService.Export(dataTable);

                return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "RunCommandErrorReport.xlsx");
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}
