using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml;

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

        public static XmlDocument GetRawXmlFeed(string url)
        {
            XmlDocument feed = new XmlDocument();
            using (WebClient client = new WebClient())
            {
                feed.Load(client.OpenRead(url));
            }

            return feed;
        }
    }
}
