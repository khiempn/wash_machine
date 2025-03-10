using AutoMapper;
using WashMachine.Business;
using WashMachine.Business.Interfaces;
using WashMachine.Business.Models;
using WashMachine.Business.Services;
using WashMachine.Web.AppCode.Attributes;
using WashMachine.Web.Models;
using Libraries;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace WashMachine.Web.Areas.Administrator.Controllers
{
    [UserAuthorize]
    public class ShopsController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IEnumerable<IBusiness> _business;

        public ShopsController(IMapper mapper, IEnumerable<IBusiness> business)
        {
            _mapper = mapper;
            _business = business;
        }

        public IActionResult ListShops(ShopManagerModel shopManagerData)
        {
            if (shopManagerData == null)
            {
                shopManagerData = new ShopManagerModel();
            }

            DateTime? fromDate = null;
            if (!string.IsNullOrWhiteSpace(shopManagerData.Filter.FromDate))
            {
                fromDate = DateTime.Parse(shopManagerData.Filter.FromDate, new CultureInfo("en-CA"));
            }

            DateTime? toDate = null;
            if (!string.IsNullOrWhiteSpace(shopManagerData.Filter.ToDate))
            {
                toDate = DateTime.Parse(shopManagerData.Filter.ToDate, new CultureInfo("en-CA"));
            }

            ShopService shopService = _business.GetService<ShopService>();

            List<ShopModel> shops = shopService.GetShops()
                .Where(w => string.IsNullOrWhiteSpace(shopManagerData.Filter.SearchCriteria) || $"{w.Name} {w.Email} {w.Code}".IndexOf(shopManagerData.Filter.SearchCriteria, StringComparison.OrdinalIgnoreCase) >= 0)
                .Where(w => fromDate == null || w.InsertTime >= fromDate.Value)
                .Where(w => toDate == null || w.InsertTime <= toDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59))
                .ToList();

            var systemInfo = HttpContext.GetSystemInfo();
            
            if (systemInfo.User.IsAdmin == false)
            {
                shops = shops.Where(w => w.Code == systemInfo.User.ShopOwner?.Code).ToList();
            }

            shopManagerData.Shops = shops;
            shopManagerData.Total = shops.Count;
            return View(shopManagerData);
        }

        public IActionResult ShopEditor(int id, string backLink = "")
        {
            var shopService = _business.GetService<ShopService>();
            var systemInfo = HttpContext.GetSystemInfo();
            if (!systemInfo.User.IsAdmin && !shopService.GetShops().Any(c => c.Id == id)) return Content(Messages.AccessDenied);

            var accessService = _business.GetService<AccessService>();
            var model = shopService.GetShopModel(id);

            model.UserSources = accessService.GetUsers();
            model.BackLink = backLink;
            return View(model);
        }

        [HttpPost]
        public IActionResult ShopEditor(ShopModel model)
        {
            var shopService = _business.GetService<ShopService>();

            var systemInfo = HttpContext.GetSystemInfo();
            if (!systemInfo.User.IsAdmin && !shopService.GetShops().Any(c => c.Id == model.Id)) return Content(Messages.AccessDenied);

            var errors = ModelState.Values.SelectMany(v => v.Errors);
            if (!ModelState.IsValid) return View(model);

            try
            {
                var result = shopService.SaveShop(model);
                if (!result.Success)
                {
                    ModelState.AddModelError(result.Name, result.Message);
                    return View(model);
                }
                TempData.SetMessage(result.Message);
            }
            catch (Exception ex)
            {

            }

            return Redirect(model.BackLink);
        }
    }
}
