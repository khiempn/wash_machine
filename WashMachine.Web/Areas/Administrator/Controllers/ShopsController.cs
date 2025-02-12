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

        public IActionResult ShopUsers(int id, string backLink = "")
        {
            var service = _business.GetService<ShopService>();
            var model = service.GetShopModel(id);
            var accessService = _business.GetService<AccessService>();
            model.BackLink = backLink;
            return View(model);
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

            shopManagerData.Shops = shops;
            shopManagerData.Total = shops.Count;
            return View(shopManagerData);
        }

        public IActionResult ShopEditor(int id, string backLink = "")
        {
            var systemInfo = HttpContext.GetSystemInfo();
            if (!systemInfo.User.IsAdmin && !systemInfo.ListShops.Any(c => c.Id == id)) return Content(Messages.AccessDenied);
            var service = _business.GetService<ShopService>();
            var model = service.GetShopModel(id);
            model.BackLink = backLink;
            return View(model);
        }

        [HttpPost]
        public IActionResult ShopEditor(ShopModel model)
        {
            var systemInfo = HttpContext.GetSystemInfo();
            if (!systemInfo.User.IsAdmin && !systemInfo.ListShops.Any(c => c.Id == model.Id)) return Content(Messages.AccessDenied);

            var errors = ModelState.Values.SelectMany(v => v.Errors);
            if (!ModelState.IsValid) return View(model);

            var service = _business.GetService<ShopService>();
            try
            {
                var result = service.SaveShop(model);
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

        [HttpPost]
        public Respondent SaveShopowner(ShopModel model)
        {
            var service = _business.GetService<ShopService>();

            var result = service.SaveShopowner(model);
            return result;
        }
    }
}
