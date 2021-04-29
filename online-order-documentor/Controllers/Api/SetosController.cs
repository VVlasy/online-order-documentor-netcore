using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeTypes;
using online_order_documentor_netcore.Models;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;

namespace online_order_documentor_netcore.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class SetosController : Controller
    {
        [HttpGet]
        [Route("feed.xml")]
        public IActionResult RawFeed()
        {
            XmlDocument digiEshopFeed = new XmlDocument();

            using (WebClient client = new WebClient())
            {
                digiEshopFeed.Load(client.OpenRead(string.Format("https://{0}/universal.xml?hash={1}", "www.digi-eshop.cz", AppVariables.DigiEshopHash)));
            }

            return base.Ok();
        }

        [HttpGet]
        [Route("feed-shoptet.xml")]
        public IActionResult ShoptetFeed()
        {

            return base.Ok();
        }
    }
}
