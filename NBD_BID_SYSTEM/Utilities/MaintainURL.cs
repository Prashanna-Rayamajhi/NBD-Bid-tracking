using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NBD_BID_SYSTEM.Utilities
{
    public static class MaintainURL
    {
        //method to return the URL passed to it 
        public static string ReturnURL(HttpContext context, string controllerName)
        {
            string cookieName = controllerName + "URL";
            string searchText = "/" + controllerName + "?";

            //getting the URL of page that sent us here
            string returnURL = context.Request.Headers["Referer"].ToString();
            if (returnURL.Contains(searchText))
            {
                //Came here from the Index with some parameters
                //Save the Parameters in a Cookie
                returnURL = returnURL[returnURL.LastIndexOf(searchText)..];
                CookieHelper.SetCookieOptions(context, cookieName, returnURL, 30);
                return returnURL;

            }
            else
            {
                //get it from cookie
                returnURL = context.Request.Cookies[cookieName];
                return returnURL ?? "/" + controllerName;
            }
        }
    }
}
