using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WashMachine.Web.Areas.Administrator.Controllers
{
    [Area("Administrator")]
    public class BaseController : Controller
    {
        protected IActionResult ViewNotFound()
        {
            return Content("View not found");
        }
    }
}
