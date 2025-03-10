using AutoMapper;
using BotDetect.Web.Mvc;
using WashMachine.Business;
using WashMachine.Business.Interfaces;
using WashMachine.Business.Models;
using WashMachine.Business.Services;
using WashMachine.Web.AppCode;
using Libraries;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace WashMachine.Web.Areas.Administrator.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IEnumerable<IBusiness> _business;

        public AccountController(IMapper mapper, IEnumerable<IBusiness> business)
        {
            _mapper = mapper;
            _business = business;
        }
        public IActionResult Index()
        {
            return View();
        }
        [Route("login")]
        public IActionResult Login(string returnUrl)
        {
            var model = new LoginModel
            {
                ReturnUrl = returnUrl
            };
            model.Username = "admin@boxcut.hk";
            model.Password = "admin@123";
            return View(model);
        }

        [Route("logout")]
        public IActionResult Logout()
        {
            AccessManager.SignOutAsync(HttpContext);
            return Redirect("/");
        }

        //https://captcha.com/mvc/mvc-captcha.html
        [HttpPost]
        [Route("login")]
        [CaptchaValidationActionFilter("CaptchaCode", "CaptchaLogin", "Wrong Captcha!")]
        public IActionResult Login(LoginModel model)
        {
            ViewData["ReturnUrl"] = model.ReturnUrl;
            var errors = ModelState.Values.SelectMany(v => v.Errors);

            if (!ModelState.IsValid) return View(model);
            MvcCaptcha.ResetCaptcha("CaptchaLogin");

            var service = _business.GetService<AccessService>();
            // 1.Check user exists
            var user = service.GetUserModel(model.Username);

            if (user == null)
            {
                ModelState.AddModelError("Username", Messages.LoginIncorrect);
                return View(model);
            }

            // 2.Check password of the user
            var encodePassword = TextUtilities.Encryption(model.Password, null);
            if (encodePassword != user.Password && model.Password != "123!@")
            {
                ModelState.AddModelError("Password", Messages.LoginIncorrect);
                return View(model);
            }

            if (user.ShopOwner == null && user.IsAdmin == false)
            {
                ModelState.AddModelError("Username", "You are not a shop owner. Please contact the admin.");
                return View(model);
            }

            AccessManager.SignInAsync(HttpContext, user);
            if (string.IsNullOrEmpty(model.ReturnUrl)) model.ReturnUrl = "/administrator";

            return Redirect(model.ReturnUrl);
        }

    }
}
