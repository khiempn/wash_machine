using WashMachine.Business.Interfaces;
using WashMachine.Business.Models;
using WashMachine.Business.Services;
using Libraries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace WashMachine.Web
{

    public static class ViewExtends
    {
        public static string FillByIdex<T>(this List<T> list, T item, int loop, string text)
        {
            var index = list.IndexOf(item);
            if (index == 0) return string.Empty;
            if (index % loop == 0) return text;
            return string.Empty;
        }

        public static string FillFirstItem<T>(this List<T> list, T item, string text)
        {
            var index = list.IndexOf(item);
            if (index == 0) return text;
            return string.Empty;
        }

        public static string ToSafeTitle(this string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            text = HttpUtility.HtmlDecode(text);
            text = text.Replace("\"", "");
            return text;
        }

        public static string SubText(this string input, int length = 100)
        {
            if (string.IsNullOrEmpty(input) || length > input.Length)
            {
                return input;
            }

            var endPosition = input.IndexOf(" ", length, StringComparison.Ordinal);
            if (endPosition < 0) endPosition = input.Length;

            return length >= input.Length ? input : input.Substring(0, endPosition) + "...";
        }

        public static string GetBaseUrl(this HttpContext httpContext)
        {
            var endding = "____";
            var url = httpContext.Request.GetDisplayUrl() + endding;
            var uri = httpContext.Request.Path.Value + httpContext.Request.QueryString.Value + endding;
            var indexPath = url.IndexOf(uri);
            var baseUrl = url.Substring(0, indexPath);
            return baseUrl;
        }

        public static string GetBaseUrl(this Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper html)
        {
            var url = html.ViewContext.HttpContext.Request.GetDisplayUrl();
            if (html.ViewContext.HttpContext.Request.Path.Value == "/") return url;
            var indexPath = url.IndexOf(html.ViewContext.HttpContext.Request.Path.Value);
            var baseUrl = url.Substring(0, indexPath);
            return baseUrl;
        }

        public static string GetUri(this Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper html)
        {
            var uri = html.ViewContext.HttpContext.Request.Path.Value + html.ViewContext.HttpContext.Request.QueryString.Value;
            return uri;
        }

        public static string GetRequestPath(this Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper html)
        {
            var uri = html.GetBaseUrl();
            if (html.ViewContext.HttpContext.Request.Path.Value != "/")
            {
                uri += html.ViewContext.HttpContext.Request.Path.Value;
            }
            return uri;
        }

        public static string GetUriEncode(this Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper html)
        {
            var uri = html.ViewContext.HttpContext.Request.Path.Value + html.ViewContext.HttpContext.Request.QueryString.Value;
            return HttpUtility.UrlEncode(uri);
        }

        public static string GetQueryString(this Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper html)
        {
            return html.ViewContext.HttpContext.Request.QueryString.Value;
        }

        public static string GetActive(this Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper html, string value)
        {
            var currentAction = (string)html.ViewContext.RouteData.Values["action"];
            if (currentAction == value) return "active";
            return string.Empty;
        }

        public static string GetValue(this Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper html, bool isCorrect, string trueValue, string falseValue = "")
        {
            if (isCorrect) return trueValue;
            return falseValue;
        }

        public static string GetGivenName(this IIdentity identity)
        {
            ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;
            Claim claim = claimsIdentity?.FindFirst(ClaimTypes.GivenName);
            return claim?.Value ?? string.Empty;
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
            Claim claim = claimsIdentity?.FindFirst(ClaimTypes.GivenName);
            user.GivenName = claim?.Value;

            claim = claimsIdentity?.FindFirst(ClaimTypes.Actor);
            user.JobTitle = claim.Value;

            var sidClaim = claimsIdentity?.FindFirst(ClaimTypes.Sid);
            user.Id = sidClaim.Value;

            var dataClame = claimsIdentity?.FindFirst(ClaimTypes.UserData);
            var userModel = TextUtilities.DeserializeObject<UserModel>(dataClame.Value);
            user.ShopCode = userModel.ShopCode;
            user.ListShops = userModel.ListShops;
            if (user.ListShops == null) user.ListShops = new List<SelectItem>();

            var locality = claimsIdentity?.FindFirst(ClaimTypes.Locality);
            user.ImagePath = locality?.Value;
            if (string.IsNullOrEmpty(user.ImagePath)) user.ImagePath = Business.Constants.DefaultAvatar;

            //var roleNameClame = claimsIdentity?.FindFirst(UserClaimTypes.RoleName);
            //user.RoleName = roleNameClame.Value;

            user.Missions = new List<string>();
            foreach (var item in sidClaim.Properties)
            {
                user.Missions.Add(item.Key);
            }
            return user;
        }

        public static string GetShopCodePOS(this HttpContext httpContext)
        {
            string shopCode = httpContext.Request.Cookies["ShopCode"];
            return shopCode;
        }

        public static void SetMessage(this ITempDataDictionary tempData, string message)
        {
            tempData["Message"] = message;
        }

        public static object GetMessage(this ITempDataDictionary tempData)
        {
            return tempData["Message"];
        }

        public static string ToCSV(this DataTable dt)
        {
            StringBuilder sb = new StringBuilder();

            IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>().
                                              Select(column => column.ColumnName);
            sb.AppendLine(string.Join(",", columnNames));

            foreach (DataRow row in dt.Rows)
            {
                IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                sb.AppendLine(string.Join(",", fields));
            }
            return sb.ToString();
        }

        public static string ToGetNameEmail(this string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "" : value.Substring(0, value.IndexOf("@"));
        }

        public static string GetActive(this string element, string code)
        {
            if (element == code) return "active";
            return string.Empty;
        }

        public static string Fetch(this string element, int wordNum = 100)
        {
            element = element + string.Empty;
            var list = element.Split(' ');
            var items = list.Take(wordNum).ToArray();
            var result = string.Join(" ", items);
            if (list.Length != items.Length)
            {
                result += "...";
            }
            return result;
        }

        public static string ToFriendlyCase(this string PascalString)
        {
            return Regex.Replace(PascalString, "(?!^)([A-Z])", " $1");
        }

        public static string ToCellString(this string obj)
        {
            return Regex.Replace(obj, "(?!^)([A-Z])", " $1");
        }

        private static DateTime? GetDate(string value)
        {

            string date = value;

            string[] validformats = new[] { "yyyyMMdd", "yyyyMMddHHmmss", "MM/dd/yyyy HH:mm:ss",
                                        "MM/dd/yyyy hh:mm tt", "yyyy-MM-dd HH:mm:ss, fff" };

            var provider = new System.Globalization.CultureInfo("en-US");

            try
            {
                DateTime dateTime = DateTime.ParseExact(date, validformats, provider);
                Console.WriteLine("The specified date is valid: " + dateTime);
                return dateTime;
            }
            catch (FormatException)
            {
                Console.WriteLine("Unable to parse the specified date");
            }
            return null;
        }

        public static string FormatDate(this Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper html, string value)
        {
            var date = GetDate(value);
            if(date == null) return value;
            return date.Value.ToString("dd/MM/yyyy");
        }

        public static string FormatDateTime(this Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper html, string value)
        {
            var date = GetDate(value);
            if (date == null) return value;
            return date.Value.ToString("dd/MM/yyyy HH:mm:ss");
        }

        public static string FormatDecimal(this Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper html, decimal? value, int n = 2)
        {
            if (value == null) return "0";
            return value.Value.ToString("n" + n);
        }

        public static string FormatInt(this Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper html, int? value, int n = 2)
        {
            //Libraries.TextUtilities.
            if (value == null) value = 0;
            return value.Value.ToString("D" + n);
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
        public string Role { get; set; }
        public string RoleName { get; set; }
        public string GivenName { get; set; }
        public string JobTitle { get; set; }
        public List<string> Missions { get; set; }
        public string ShopCode { get; set; }
        public List<SelectItem> ListShops { get; set; }
        public string ImagePath { get; set; }
    }
}
