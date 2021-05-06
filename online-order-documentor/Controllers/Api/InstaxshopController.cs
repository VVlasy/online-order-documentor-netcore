using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeTypes;
using online_order_documentor_netcore.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;

namespace online_order_documentor_netcore.Controllers.Api
{
    [ApiController]
    [Route("api/instax")]
    public class InstaxshopController : Controller
    {
        [HttpGet]
        [Route("{**filter}")]
        public IActionResult Crossroad(string filter)
        {
            string[] filters = filter.Split('/');
            string destination = filters[filters.Length - 1];
            Array.Resize(ref filters, filters.Length - 1);

            List<string> brands = new List<string>();
            List<string> eans = new List<string>();


            foreach (string item in filters)
            {
                if (item.StartsWith("EAN="))
                {
                    eans.Add(item.Substring(4).ToLower());
                }
                else
                {
                    brands.Add(item.ToLower());
                }
            }

            switch (destination)
            {
                case "data.xml":
                    return DataFeed(brands, eans);
                case "availability.xml":
                    return AvailabilityFeed(brands, eans);
                case "availability-shoptet.xml":
                    return ShoptetAvailabilityFeed(brands, eans);
                default:
                    return base.BadRequest("Invalid Path");
            }
        }

        private IActionResult DataFeed(List<string> brands, List<string> eans)
        {
            var url = string.Format("https://www.instaxshop.cz/heureka/export/products.xml?hash={0}", AppVariables.InstaxShopHash);
            var feed = Tools.StripByBrandsAndEans(Tools.GetRawXmlFeed(url), brands, eans);

            return this.Xml(feed.OuterXml);
        }

        private IActionResult AvailabilityFeed(List<string> brands, List<string> eans)
        {
            var url = string.Format("https://www.instaxshop.cz/universal.xml?hash={0}", AppVariables.InstaxShopHash);
            var feed = Tools.StripByBrandsAndEans(Tools.GetRawXmlFeed(url), brands, eans);

            feed = Tools.CreateAvailabilityFeed(feed);

            return this.Xml(feed.OuterXml);
        }

        private IActionResult ShoptetAvailabilityFeed(List<string> brands, List<string> eans)
        {
            var url = string.Format("https://www.instaxshop.cz/universal.xml?hash={0}", AppVariables.InstaxShopHash);
            var feed = Tools.StripByBrandsAndEans(Tools.GetRawXmlFeed(url), brands, eans);

            feed = Tools.CreateAvailabilityFeedShoptet(feed);

            return this.Xml(feed.OuterXml);
        }
    }
}
