using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NBD_BID_SYSTEM.Utilities
{
    public static class CookieHelper
    {
        //this helper class sets the cookie with expiration time with in http context 
        public static void SetCookieOptions(HttpContext _context, string key, string value, int? expire)
        {
            CookieOptions options = new CookieOptions();
            if (expire.HasValue)
            {
                var expireTime = expire.GetValueOrDefault();
                options.Expires = DateTime.Now.AddMinutes(expireTime);
            }
            else
            {
                options.Expires = DateTime.Now.AddMilliseconds(1000);
            }
            _context.Response.Cookies.Append(key, value, options);
        }
    }
}
