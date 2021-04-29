using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeTypes;
using online_order_documentor_netcore.Models;
using System;
using System.IO;
using System.Text;

namespace online_order_documentor_netcore.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class LevenhukController : Controller
    {
        [HttpGet]
        [Route("feed.xml")]
        public IActionResult RawFeed([FromForm] UploadImageModel model)
        {
            
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
