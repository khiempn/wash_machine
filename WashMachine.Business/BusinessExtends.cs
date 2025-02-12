using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;

namespace WashMachine.Business
{
    public static class BusinessExtends
    {
        public static int GetId(this HttpContext httpContext)
        {
            ClaimsIdentity claimsIdentity = httpContext.User.Identity as ClaimsIdentity;
            Claim claim = claimsIdentity?.FindFirst(ClaimTypes.Sid);
            var value = claim?.Value ?? "0";
            return int.Parse(value);
        }
    }
}
