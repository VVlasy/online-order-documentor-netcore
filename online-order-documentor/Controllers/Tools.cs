using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace online_order_documentor_netcore.Controllers
{
    public static class Tools
    {
        public static ContentResult Xml(this Controller ctrl, string content)
        {
            return new ContentResult
            {
                ContentType = "application/xml",
                Content = content,
                StatusCode = 200
            };
        }
    }
}
