using WashMachine.Business.Interfaces;
using WashMachine.Business.Models;
using WashMachine.Business.Services;
using Libraries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace WashMachine.Web
{

    public static class ViewExtends
    {

        public static string GetBaseUrl(this HttpContext httpContext)
        {
            var endding = "____";
            var url = httpContext.Request.GetDisplayUrl() + endding;
            var uri = httpContext.Request.Path.Value + httpContext.Request.QueryString.Value + endding;
            var indexPath = url.IndexOf(uri);
            var baseUrl = url.Substring(0, indexPath);
            return baseUrl;
        }

        public static string GetUri(this Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper html)
        {
            var uri = html.ViewContext.HttpContext.Request.Path.Value + html.ViewContext.HttpContext.Request.QueryString.Value;
            return uri;
        }

        public static string GetUriEncode(this Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper html)
        {
            var uri = html.ViewContext.HttpContext.Request.Path.Value + html.ViewContext.HttpContext.Request.QueryString.Value;
            return HttpUtility.UrlEncode(uri);
        }

        public static UserInfo GetUserInfo(this HttpContext httpContext)
        {
            return httpContext.User.Identity.GetUserInfo();
        }

        public static UserInfo GetUserInfo(this IIdentity identity)
        {
            if (!identity.IsAuthenticated)
            {
                return new UserInfo();
            }

            var user = new UserInfo
            {
                Name = identity.Name
            };

            ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;
            var sidClaim = claimsIdentity?.FindFirst(ClaimTypes.Sid);
            user.Id = sidClaim.Value;

            var dataClame = claimsIdentity?.FindFirst(ClaimTypes.UserData);
            var userModel = TextUtilities.DeserializeObject<UserModel>(dataClame.Value);
            user.ShopCode = userModel.ShopOwner?.Code;
            user.ShopName = userModel.ShopOwner?.Name;

            var locality = claimsIdentity?.FindFirst(ClaimTypes.Locality);
            user.ImagePath = locality?.Value;
            if (string.IsNullOrEmpty(user.ImagePath)) user.ImagePath = Business.Constants.DefaultAvatar;
            return user;
        }

        public static void SetMessage(this ITempDataDictionary tempData, string message)
        {
            tempData["Message"] = message;
        }

        public static object GetMessage(this ITempDataDictionary tempData)
        {
            return tempData["Message"];
        }

        public static T GetService<T>(this IEnumerable<Business.Interfaces.IBusiness> businesses)
        {
            foreach (var item in businesses)
            {
                if (item is T) return (T)item;
            }
            return default;
        }

        public static SystemInfo GetSystemInfo(this HttpContext httpContext, int userId = 0)
        {
            httpContext.Items.TryGetValue("SystemInfo", out object value);
            var info = value as SystemInfo;
            if (info == null)
            {
                var services = httpContext.RequestServices.GetService(typeof(IEnumerable<IBusiness>)) as IEnumerable<IBusiness>;
                var accessService = services.GetService<AccessService>();
                info = accessService.GetSystemInfo(userId);
                httpContext.Items.Add("SystemInfo", info);
            }
            return info;
        }
    }

    public class UserInfo
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string ShopCode { get; set; }
        public string ShopName { get; set; }
        public string ImagePath { get; set; }
    }
}
