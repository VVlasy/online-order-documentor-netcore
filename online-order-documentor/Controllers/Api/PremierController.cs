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
using System.Xml.Serialization;
using System.Xml;
using online_order_documentor_netcore.Models.Xml;
using online_order_documentor_netcore.Providers;

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
            AssignColumnsToProviderBasedOnFile(storage as SplitFilesFtpProvider, filename);

            string file = $"homes/sklad premier/Public feed/{filename}.xml";

            using (var downloadedFile = storage.Download(file))
            {
                using (StreamReader sr = new StreamReader(downloadedFile))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(PremierExportXmlModel));
                    using (XmlReader reader = XmlReader.Create(sr))
                    {
                        var data = (PremierExportXmlModel)serializer.Deserialize(reader);
                        using (Utf8StringWriter xmlSw = new Utf8StringWriter())
                        {
                            if (filename == "mallfeed")
                            {
                                var mallFeedData = new MallSupplierXmlModel()
                                {
                                    ShopItems = new List<MallShopItem>()
                                };

                                foreach (var premierItem in data.que_txt)
                                {
                                    var mallShopItem = new MallShopItem()
                                    {
                                        ID = premierItem.Sloupec01,
                                        Name = premierItem.Sloupec02,
                                        EAN = premierItem.Sloupec03,
                                        Stock = premierItem.Sloupec04,
                                        PurchasePrice = premierItem.Sloupec05.Replace(" ", string.Empty),
                                        PriceNoVAT = premierItem.Sloupec06.Replace(" ", string.Empty),

                                        TODOSloupec07 = premierItem.Sloupec07
                                    };

                                    mallFeedData.ShopItems.Add(mallShopItem);
                                }

                                serializer = new XmlSerializer(typeof(MallSupplierXmlModel));
                                serializer.Serialize(xmlSw, mallFeedData);
                            }
                            else if (filename == "czcfeed")
                            {
                                var czcFeedData = new CZCSupplierXmlModel()
                                {
                                    ShopItems = new List<CZCShopItem>()
                                };

                                foreach (var premierItem in data.que_txt)
                                {
                                    var czcShopItem = new CZCShopItem()
                                    {
                                        ID = premierItem.Sloupec01,
                                        Name = premierItem.Sloupec02,
                                        EAN = premierItem.Sloupec03,
                                        Quantity = premierItem.Sloupec04.Replace(" ", string.Empty).Replace(',', '.'),
                                        Price = premierItem.Sloupec05.Replace(" ", string.Empty).Replace(',', '.'),
                                        ListPrice = premierItem.Sloupec06.Replace(" ", string.Empty).Replace(',', '.'),
                                        Weight = premierItem.Sloupec07.Replace(" ", string.Empty).Replace(',', '.'),
                                        RecycleFee = premierItem.Sloupec08.Replace(" ", string.Empty).Replace(',', '.'),
                                        SizeY = premierItem.Sloupec09.Replace(" ", string.Empty).Replace(',', '.'),
                                        SizeX = premierItem.Sloupec10.Replace(" ", string.Empty).Replace(',', '.'),
                                        SizeZ = premierItem.Sloupec11.Replace(" ", string.Empty).Replace(',', '.')
                                    };

                                    double val;
                                    if (double.TryParse(czcShopItem.ListPrice, NumberStyles.Any, CultureInfo.InvariantCulture, out val))
                                    {
                                        czcShopItem.ListPrice = (Math.Truncate((val * 1.21) * 100) / 100).ToString("0.##", CultureInfo.InvariantCulture);
                                    }
                                    else
                                    {
                                        czcShopItem.ListPrice = null;
                                    }

                                    czcFeedData.ShopItems.Add(czcShopItem);
                                }

                                serializer = new XmlSerializer(typeof(CZCSupplierXmlModel));
                                serializer.Serialize(xmlSw, czcFeedData);

                            }
                            else if (filename == "alzafeed")
                            {
                                var alzaFeedData = new AlzaSupplierXmlModel()
                                {
                                    Items = new List<AlzaItem>()
                                };

                                foreach (var premierItem in data.que_txt)
                                {
                                    var alzaItem = new AlzaItem()
                                    {
                                        Pricing = new AlzaItemPricing()
                                        {
                                            PriceWithFee = premierItem.Sloupec08.Replace(" ", string.Empty).Replace(',', '.'),
                                            PriceWithoutFee = premierItem.Sloupec08.Replace(" ", string.Empty).Replace(',', '.'),
                                            RecycleFee = premierItem.Sloupec09.Replace(" ", string.Empty).Replace(',', '.'),
                                            CopyrightFee = string.Empty, //0.ToString(),
                                            Currency = "CZK"
                                        },
                                        Storage = new AlzaItemStorage()
                                        {
                                            StoredQuantity = premierItem.Sloupec11.Replace(" ", string.Empty).Replace(',', '.').Replace(".00", string.Empty)
                                        },
                                        Product = new AlzaItemProduct()
                                        {
                                            Name = premierItem.Sloupec02,
                                            DealerCode = premierItem.Sloupec01,
                                            PartNumber = premierItem.Sloupec03,
                                            Ean = premierItem.Sloupec05
                                        }
                                    };

                                    if (alzaItem.Pricing.PriceWithFee == "0.00" || alzaItem.Pricing.PriceWithoutFee == "0.00")
                                    {
                                        continue;
                                    }

                                    alzaFeedData.Items.Add(alzaItem);
                                }

                                serializer = new XmlSerializer(typeof(AlzaSupplierXmlModel));
                                serializer.Serialize(xmlSw, alzaFeedData);
                            }
                            else if (filename == "expando")
                            {
                                var expandoFeedData = new ExpandoXmlModel()
                                {
                                    Products = new List<ExpandoItem>()
                                };

                                foreach (var premierItem in data.que_txt)
                                {
                                    var expandoItem = new ExpandoItem()
                                    {
                                        Code = premierItem.Sloupec01,
                                        Name = premierItem.Sloupec02,
                                        Stock = premierItem.Sloupec11.Replace(".00", string.Empty).Replace(",00", string.Empty),
                                        EAN = premierItem.Sloupec05,
                                        PriceVat = premierItem.Sloupec08,
                                    };


                                    if (int.TryParse(expandoItem.Stock, NumberStyles.Any, CultureInfo.InvariantCulture, out int stockAmount) && stockAmount > 0)
                                    {
                                        expandoFeedData.Products.Add(expandoItem);
                                    }
                                }

                                serializer = new XmlSerializer(typeof(ExpandoXmlModel));
                                serializer.Serialize(xmlSw, expandoFeedData);

                            }
                            return this.Xml(xmlSw.ToString());
                        }
                    }
                }
            }
        }

        private void AssignColumnsToProviderBasedOnFile(SplitFilesFtpProvider provider, string filename)
        {
            if (provider == null)
            {
                return;
            }

            switch (filename)
            {
                case "alzafeed":
                    provider.StockColumnName = "sloupec11";
                    break;
                case "expando":
                    provider.StockColumnName = "sloupec11";
                    break;
                default:
                    break;
            }
        }
    }
}
