using AutoMapper;
using WashMachine.Business;
using WashMachine.Business.Interfaces;
using WashMachine.Business.Services;
using WashMachine.Web.AppCode.Attributes;
using WashMachine.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace WashMachine.Web.Areas.Administrator.Controllers
{
    [UserAuthorize]
    public class HomeController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IEnumerable<IBusiness> _business;
   
        public HomeController(IMapper mapper, IEnumerable<IBusiness> business)
        {
            _mapper = mapper;
            _business = business;
        }

        public IActionResult Index(DashboardDataModel dashboardData)
        {
            OrderService orderService = _business.GetService<OrderService>();

            dashboardData.OrderByYear = dashboardData.OrderByYear ?? new OrderByYearDataModel();
            dashboardData.OrderByYear.OrderModels = orderService.GetOrders().Where(w => w.InsertTime.Value.Year == dashboardData.OrderByYear.Filter.Year).ToList();
            var systemInfo = HttpContext.GetSystemInfo();

            if (systemInfo.User.IsAdmin == false)
            {
                dashboardData.OrderByYear.OrderModels = dashboardData.OrderByYear.OrderModels.Where(w => w.ShopCode == systemInfo.User.ShopOwner?.Code).ToList();
            }

            dashboardData.OrderByYear.OrderModels = dashboardData.OrderByYear.OrderModels ?? new List<Business.Models.OrderModel>();
            return View(dashboardData);
        }

        public IActionResult AccessDenied()
        {
            return Content(Messages.AccessDenied);
        }
    }
}
