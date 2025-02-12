using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WashMachine.Web.AppCode.Attributes
{

    public class UserAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public string Mission { get; set; }

        public UserAuthorizeAttribute(string mission)
        {
            Mission = mission;
        }

        public UserAuthorizeAttribute() : base()
        {

        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                var returnUrl = "/login?returnUrl=" + context.HttpContext.Request.Path.Value;
                context.Result = new RedirectResult(returnUrl);
                return;
            }

            var systemInfo = context.HttpContext.GetSystemInfo();
            var user = systemInfo.User;
            if (user.Active == false)
            {
                var returnUrl = "/login?returnUrl=" + context.HttpContext.Request.Path.Value;
                context.Result = new RedirectResult(returnUrl);
                return;
            }

            if (Mission != null)
            {
                var roles = Mission.Split(',', StringSplitOptions.RemoveEmptyEntries);
                var existedItem = user.Rights.FirstOrDefault(c => roles.Contains(c));
                if (existedItem != null) return;

                if (roles.Length > 0 && existedItem == null)
                {
                    var loginRoute = new RouteValueDictionary(new { controller = "Home", action = "AccessDenied", returnUrl = $"{context.HttpContext.Request.Path.Value}{context.HttpContext.Request.QueryString.Value}" });
                    context.Result = new RedirectToRouteResult(loginRoute);
                    return;
                }
            }
        }

    }
}

