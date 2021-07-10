using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeTypes;
using online_order_documentor_netcore.Models;
using System;
using System.Globalization;
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
            Providers.IStorageProvider storage = Providers.StorageProviderFactory.Create(true);

            string file = $"homes/sklad premier/Public feed/{filename}.xml";
            using (var downloadedFile = storage.Download(file))
            {
                using (StreamReader sr = new StreamReader(downloadedFile))
                {
                    string s = sr.ReadToEnd();

                    if (filename == "mallfeed")
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
                    else if (filename == "czcfeed")
                    {
                        s = s.Replace("VFPData", "SHOP");
                        s = s.Replace("que_txt", "SHOPITEM");

                        s = s.Replace("sloupec01", "ID");
                        s = s.Replace("sloupec02", "NAME");
                        s = s.Replace("sloupec03", "EAN");
                        s = s.Replace("sloupec04", "QUANTITY");
                        s = s.Replace("sloupec05", "PRICE");
                        s = s.Replace("sloupec06", "LIST_PRICE");
                        s = s.Replace("sloupec07", "WEIGHT");
                        s = s.Replace("sloupec08", "RECYCLE_FEE");
                        s = s.Replace("sloupec09", "SIZE_Y");
                        s = s.Replace("sloupec10", "SIZE_X");
                        s = s.Replace("sloupec11", "SIZE_Z");

                        s = s.ModifyTag("QUANTITY", (s) => s.Replace(" ", string.Empty).Replace(',', '.'));
                        s = s.ModifyTag("PRICE", (s) => s.Replace(" ", string.Empty).Replace(',', '.'));
                        s = s.ModifyTag("LIST_PRICE", (s) => s.Replace(" ", string.Empty).Replace(',', '.'));
                        s = s.ModifyTag("WEIGHT", (s) => s.Replace(" ", string.Empty).Replace(',', '.'));
                        s = s.ModifyTag("RECYCLE_FEE", (s) => s.Replace(" ", string.Empty).Replace(',', '.'));
                        s = s.ModifyTag("SIZE_Y", (s) => s.Replace(" ", string.Empty).Replace(',', '.'));
                        s = s.ModifyTag("SIZE_X", (s) => s.Replace(" ", string.Empty).Replace(',', '.'));
                        s = s.ModifyTag("SIZE_Z", (s) => s.Replace(" ", string.Empty).Replace(',', '.'));

                        // Add VAT
                        s = s.ModifyTag("LIST_PRICE", (s) =>
                        {
                            double val;
                            if (double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out val))
                            {
                                return (Math.Truncate((val * 1.21) * 100) / 100).ToString("0.##", CultureInfo.InvariantCulture);
                            }

                            return null;
                        });
                    }


                    return this.Xml(s);
                }
            }
        }
    }
}
