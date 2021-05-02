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
            string url = "http://www.setos.cz/upload/xml/promchat83125.xml";
            var feed = Tools.GetRawXmlFeed(url);
            return this.Xml(feed.OuterXml);
        }

        [HttpGet]
        [Route("feed-shoptet.xml")]
        public IActionResult ShoptetFeed()
        {

            return base.Ok();
        }
    }
}
