using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WashMachine.Web.AppCode
{
    public class ApiLogger
    {
        public static void LogException(HttpContext httpContext)
        {
            var exceptionHandlerPathFeature = httpContext.Features.Get<IExceptionHandlerPathFeature>();
            var exception = exceptionHandlerPathFeature.Error as Exception;
            if (exception != null)
            {
                Libraries.FileUtilities.LogFile(exception, $"{httpContext.GetBaseUrl()}{exceptionHandlerPathFeature.Path}{httpContext.Request.QueryString}");
            }
        }
    }
}
