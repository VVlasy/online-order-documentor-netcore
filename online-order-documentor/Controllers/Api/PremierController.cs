﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeTypes;
using online_order_documentor_netcore.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace online_order_documentor_netcore.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class PremierController : Controller
    {
        [HttpGet]
        [Route("{filename}.xml")]
        public IActionResult Index(string filename)
        {
            // TODO: return feed from premier feeds folder in ftp
            Providers.IStorageProvider storage = Providers.StorageProviderFactory.Create();

            string file = $"homes/sklad premier/Public feed/{filename}.xml";

            using (StreamReader sr = new StreamReader(storage.Download(file)))
            {
                string s = sr.ReadToEnd();

                if (filename == "mallczcfeed")
                {
                    s = s.Replace("VFPData", "SHOP");
                    s = s.Replace("que_txt", "SHOPITEM");

                    s = s.Replace("sloupec01", "ID");
                    s = s.Replace("sloupec02", "NAME");
                    s = s.Replace("sloupec03", "EAN");
                    s = s.Replace("sloupec04", "STOCK");
                    s = s.Replace("sloupec05", "PURCHASEPRICE");
                    s = s.Replace("sloupec06", "PRICE_NO_VAT");

                    s = s.ModifyTag("PURCHASEPRICE", (s) => s.Replace(" ", string.Empty));
                    s = s.ModifyTag("PRICE_NO_VAT", (s) => s.Replace(" ", string.Empty));
                }

                return this.Xml(s);
            }
        }
    }
}
