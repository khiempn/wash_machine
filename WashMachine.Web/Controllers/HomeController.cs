using WashMachine.Web.Models;
using Libraries;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace WashMachine.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return Redirect("/administrator");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // Retrieve error information in case of internal errors
            var error = HttpContext.Features.Get<IExceptionHandlerFeature>();
            if (error is ExceptionHandlerFeature handlerException)
            {
                var url = Request.GetDisplayUrl();
                var errorUrl = url.Replace(Request.Path.Value, handlerException.Path);

                FileUtilities.LogFile(error.Error, errorUrl);
            }
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
