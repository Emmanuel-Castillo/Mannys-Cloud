using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Mannys_Cloud_Backend_Tests.Controllers.Common
{
    public static class HTTPUserContext
    {
        public static ControllerContext SetTestControllerContext(string? id = "1")
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, id),
            };

            var identity = new ClaimsIdentity(claims, "TestUserType");
            var principal = new ClaimsPrincipal(identity);
            var httpContext = new DefaultHttpContext { User = principal };
            var controllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            return controllerContext;
        }

        public static ControllerContext SetContextWithEmptyClaims()
        {
            var claims = new List<Claim>();

            var identity = new ClaimsIdentity(claims, "TestUserType");
            var principal = new ClaimsPrincipal(identity);
            var httpContext = new DefaultHttpContext { User = principal };
            var controllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            return controllerContext;
        }

        public static ControllerContext SetContextWithInvalidClaims()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "NaN"),
            };

            var identity = new ClaimsIdentity(claims, "TestUserType");
            var principal = new ClaimsPrincipal(identity);
            var httpContext = new DefaultHttpContext { User = principal };
            var controllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            return controllerContext;
        }
    }
}
