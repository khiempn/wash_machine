using WashMachine.Business.Models;
using Libraries;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace WashMachine.Web.AppCode
{
    public class AccessManager
    {
        public static void SignInAsync(HttpContext httpContext, UserModel user)
        {
            //Create the identity for the user  
            var claime = new Claim(ClaimTypes.Sid, user.Id + string.Empty);
            //foreach (var item in user.Missions)
            //{
            //    claime.Properties.Add(item, "true");
            //}
            //var roles = string.Join(";", user.Missions);

            var identity = new ClaimsIdentity(new[] {
                claime,
                new Claim(ClaimTypes.Sid, user.Id + string.Empty),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.GivenName, user.FullName),
                new Claim(ClaimTypes.Actor, user.Email + string.Empty),
                new Claim(UserClaimTypes.Type, user.Type + string.Empty),
                new Claim(ClaimTypes.UserData, TextUtilities.SerializeObject(user))
            }, CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            var check = httpContext.GetBaseUrl();
            if (check == "http://WashMachine")
            {
                httpContext.SignInAsync(
                   CookieAuthenticationDefaults.AuthenticationScheme,
                   new ClaimsPrincipal(principal),
                   new AuthenticationProperties
                   {
                       IsPersistent = true
                   });
                return;
            }
            httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        }

        public static void SignOutAsync(HttpContext httpContext)
        {
            httpContext.Response.Cookies.Delete("authenticate__");
            httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public class UserClaimTypes
        {
            public static string Type => "Type";
        }
    }
}
