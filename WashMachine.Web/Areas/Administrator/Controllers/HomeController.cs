using AutoMapper;
using WashMachine.Business;
using WashMachine.Business.Interfaces;
using WashMachine.Web.AppCode.Attributes;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace WashMachine.Web.Areas.Administrator.Controllers
{
    [UserAuthorize]
    public class HomeController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IEnumerable<IBusiness> _business;
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult AccessDenied()
        {
            return Content(Messages.AccessDenied);
        }
    }
}
