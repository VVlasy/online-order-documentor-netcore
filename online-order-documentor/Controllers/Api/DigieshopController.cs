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
    [Route("api/digi-eshop")]
    public class DigieshopController : Controller
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
                    eans.Add(item.Substring(4));
                }
                else
                {
                    brands.Add(item);
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
            return base.Ok();
        }

        private IActionResult AvailabilityFeed(List<string> brands, List<string> eans)
        {

            return base.Ok();
        }

        private IActionResult ShoptetAvailabilityFeed(List<string> brands, List<string> eans)
        {


            return base.Ok();
        }

        public static XmlDocument GetRawFeed()
        {
            XmlDocument digiEshopFeed = new XmlDocument();
            string url = "www.digi-eshop.cz";

            using (WebClient client = new WebClient())
            {
                digiEshopFeed.Load(client.OpenRead(string.Format("https://{0}/universal.xml?hash={1}", url, AppVariables.DigiEshopHash)));
            }

            return digiEshopFeed;
        }
    }
}
