using AutoMapper;
using WashMachine.Business;
using WashMachine.Business.Interfaces;
using WashMachine.Business.Models;
using WashMachine.Business.Services;
using WashMachine.Web.AppCode;
using WashMachine.Web.AppCode.Attributes;
using WashMachine.Web.Models;
using Libraries;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace WashMachine.Web.Areas.Administrator.Controllers
{
    [UserAuthorize]
    public class AccessController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IEnumerable<IBusiness> _business;

        public AccessController(IMapper mapper, IEnumerable<IBusiness> business)
        {
            _mapper = mapper;
            _business = business;
        }
        [UserAuthorize(Mission = Roles.Admin)]
        public IActionResult ListNormalUsers(ParamFilter filter)
        {
            var service = _business.GetService<AccessService>();
            var model = service.FindShopUsers(filter);
            ViewBag.Filter = filter;
            return View(model);
        }

        [UserAuthorize(Mission = Roles.Admin)]
        public IActionResult ListUsers(UserManagerModel userManagerData)
        {
            if (userManagerData == null)
            {
                userManagerData = new UserManagerModel();
            }

            DateTime? fromDate = null;
            if (!string.IsNullOrWhiteSpace(userManagerData.Filter.FromDate))
            {
                fromDate = DateTime.Parse(userManagerData.Filter.FromDate, new CultureInfo("en-CA"));
            }

            DateTime? toDate = null;
            if (!string.IsNullOrWhiteSpace(userManagerData.Filter.ToDate))
            {
                toDate = DateTime.Parse(userManagerData.Filter.ToDate, new CultureInfo("en-CA"));
            }

            AccessService accessService = _business.GetService<AccessService>();

            List<UserModel> coupons = accessService.GetUsers()
                .Where(w => string.IsNullOrWhiteSpace(userManagerData.Filter.SearchCriteria) || $"{w.Username} {w.Email} {w.FullName} {w.ShopCode}".IndexOf(userManagerData.Filter.SearchCriteria, StringComparison.OrdinalIgnoreCase) >= 0)
                .Where(w => fromDate == null || w.InsertTime >= fromDate.Value)
                .Where(w => toDate == null || w.InsertTime <= toDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59))
                .ToList();

            userManagerData.Users = coupons;
            userManagerData.Total = coupons.Count;
            return View(userManagerData);
        }

        [UserAuthorize(Mission = Roles.Admin)]
        public IActionResult UserEditor(int id, string backLink = "")
        {
            var service = _business.GetService<AccessService>();
            var model = service.GetUserModel(id);
            model.BackLink = backLink;
            return View(model);
        }

        [UserAuthorize(Mission = Roles.Admin)]
        [HttpPost]
        public IActionResult UserEditor(UserModel model)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            if (!ModelState.IsValid) return View(model);

            var service = _business.GetService<AccessService>();

            var result = service.SaveUser(model);
            if (!result.Success)
            {
                ModelState.AddModelError(result.Name, result.Message);
                return View(model);
            }
            TempData.SetMessage(result.Message);
            if (string.IsNullOrEmpty(model.BackLink)) model.BackLink = "/administrator/access/listusers";
            return Redirect(model.BackLink);
        }

        public IActionResult Profile()
        {
            var user = HttpContext.GetUserInfo();
            var userId = TextUtilities.GetInt(user.Id);
            var service = _business.GetService<AccessService>();
            var model = service.GetUserModel(userId);
            return View(model);
        }

        [HttpPost]
        public IActionResult Profile(UserModel model)
        {
            //var errors = ModelState.Values.SelectMany(v => v.Errors);
            //if (!ModelState.IsValid) return View(model);
            var service = _business.GetService<AccessService>();
            var result = service.SaveUser(model);
            if (!result.Success)
            {
                ModelState.AddModelError(result.Name, result.Message);
                return View(model);

            }
            TempData.SetMessage(result.Message);
            if (string.IsNullOrEmpty(model.BackLink)) model.BackLink = "/administrator/access/listusers";
            var systemInfo = HttpContext.GetSystemInfo();
            var user = service.GetUserModel(systemInfo.User.Id);
            AccessManager.SignInAsync(HttpContext, user);
            return Redirect(model.BackLink);
        }

        public IActionResult ChangePassword()
        {
            var user = HttpContext.GetUserInfo();
            var userId = TextUtilities.GetInt(user.Id);
            var service = _business.GetService<AccessService>();
            var userModel = service.GetUserModel(userId);
            var model = new ChangePasswordModel
            {
                Email = userModel.Email,
                Username = userModel.Username
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordModel model)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            if (!ModelState.IsValid) return View(model);

            var service = _business.GetService<AccessService>();

            var result = service.ChangePassword(model);
            if (!result.Success)
            {
                ModelState.AddModelError(result.Name, result.Message);
                return View(model);
            }
            TempData.SetMessage(result.Message);
            return View(model);
        }

        [UserAuthorize(Mission = Roles.Admin)]
        public Respondent ResetPassword(int id)
        {
            var service = _business.GetService<AccessService>();
            var result = service.ResetPassword(id);
            TempData.SetMessage(result.Message);
            return result;
        }

        [UserAuthorize(Mission = Roles.Admin)]
        public Respondent DeleteUser(int id)
        {
            var service = _business.GetService<AccessService>();
            var result = service.DeleteUser(id);
            TempData.SetMessage(result.Message);
            return result;
        }
    }
}
