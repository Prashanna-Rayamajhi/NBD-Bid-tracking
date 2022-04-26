using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NBD_BID_SYSTEM.Utilities
{
    public static class PageSizeHelper
    {
        //method to set the page size and add it to response of http context 
        public static int SetPageSize(HttpContext context, int? pageSizeID)
        {
            int pageSize;
            if (pageSizeID.HasValue)
            {
                pageSize = pageSizeID.GetValueOrDefault();
                CookieHelper.SetCookieOptions(context, "PageSize", pageSize.ToString(), 30);
            }
            else //if no pageSizeID is found ..looking at incoming request of cookie
            {
                pageSize = Convert.ToInt32(context.Request.Cookies["PageSize"]);
            }
            return pageSize == 0 ? 5 : pageSize; 
        }

        //method for returning the ddl
        public static SelectList PageSizeList(int? pageSize)
        {
            return new SelectList(new[] { "3", "5", "10", "20", "30", "40", "50", "100", "500" }, pageSize.ToString());
        }

    }
}
