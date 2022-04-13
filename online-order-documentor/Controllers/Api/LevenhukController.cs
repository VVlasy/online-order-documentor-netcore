using Microsoft.AspNetCore.Mvc;
using online_order_documentor_netcore.Models.Xml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace online_order_documentor_netcore.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class LevenhukController : Controller
    {
        public static Dictionary<string, string> NodeConversions = new Dictionary<string, string>()
        {
            {"OBRAZEK", "IMAGE" },
            {"ZASOBA", "AMOUNT" }
        };

        [HttpGet]
        [Route("feed.xml")]
        public IActionResult RawFeed()
        {
            string url = "https://cdn.t1.levenhuk.com/media/feed/znj_cz_leven_bress_meade_wsp1-2-3.xml";
            var feed = Tools.GetRawXmlFeed(url);
            return this.Xml(feed.OuterXml);
        }

        [HttpGet]
        [Route("feed-shoptet.xml")]
        public IActionResult ShoptetFeed()
        {
            var shoptetFeed = GetShoptetFeed();

            XmlSerializer serializer = new XmlSerializer(typeof(ShoptetXmlFeed));
            using (Utf8StringWriter xmlSw = new Utf8StringWriter())
            {
                serializer.Serialize(xmlSw, shoptetFeed);

                return this.Xml(xmlSw.ToString());
            }
        }

        [HttpGet]
        [Route("feed-raised.xml")]
        public IActionResult RaisedFeed()
        {
            var shoptetFeed = GetShoptetFeed(true);

            XmlSerializer serializer = new XmlSerializer(typeof(ShoptetXmlFeed));
            using (Utf8StringWriter xmlSw = new Utf8StringWriter())
            {
                serializer.Serialize(xmlSw, shoptetFeed);

                return this.Xml(xmlSw.ToString());
            }
        }

        public static XmlDocument GetShoptetXmlDocumentFeed(double priceRaise = 1)
        {
            string url = "http://www.levenhukshop.cz/seznam-zbozi.aspx";
            var feed = Tools.GetRawXmlFeed(url);

            XmlDocument shoptetFeed = new XmlDocument();
            XmlNode shoptetFeedItems = shoptetFeed.CreateNode(XmlNodeType.Element, "SHOP", string.Empty);

            // convert children of raw feed
            foreach (XmlNode polozka in feed.ChildNodes[1].ChildNodes[0].ChildNodes)
            {
                XmlNode newNode = shoptetFeed.CreateNode(XmlNodeType.Element, "SHOPITEM", string.Empty);

                foreach (XmlNode polozkaProperty in polozka.ChildNodes)
                {
                    switch (polozkaProperty.Name)
                    {
                        case "OBRAZEK":
                            {
                                XmlNodeList imagesNodeList = newNode.SelectNodes("IMAGES");
                                XmlNode imagesNode;
                                if (imagesNodeList.Count == 0)
                                {
                                    imagesNode = shoptetFeed.CreateNode(XmlNodeType.Element, "IMAGES", string.Empty);
                                    newNode.AppendChild(imagesNode);
                                }
                                else
                                {
                                    imagesNode = imagesNodeList.Item(0);
                                }

                                XmlNode imageNode = shoptetFeed.CreateNode(XmlNodeType.Element, "IMAGE", string.Empty);
                                imageNode.InnerText = polozkaProperty.InnerText;
                                imagesNode.AppendChild(imageNode);
                            }

                            break;
                        case "OBRAZKY_DALSI":
                            foreach (XmlNode obrazek in polozkaProperty.ChildNodes)
                            {
                                XmlNodeList imagesNodeList = newNode.SelectNodes("IMAGES");
                                XmlNode imagesNode;
                                if (imagesNodeList.Count == 0)
                                {
                                    imagesNode = shoptetFeed.CreateNode(XmlNodeType.Element, "IMAGES", string.Empty);
                                    newNode.AppendChild(imagesNode);
                                }
                                else
                                {
                                    imagesNode = imagesNodeList.Item(0);
                                }

                                XmlNode imageNode = shoptetFeed.CreateNode(XmlNodeType.Element, "IMAGE", string.Empty);
                                imageNode.InnerText = obrazek.InnerText;
                                imagesNode.AppendChild(imageNode);
                            }
                            break;
                        case "ZASOBA":
                            XmlNode stockNode = shoptetFeed.CreateNode(XmlNodeType.Element, "STOCK", string.Empty);
                            XmlNode amountNode = shoptetFeed.CreateNode(XmlNodeType.Element, "AMOUNT", string.Empty);
                            amountNode.InnerText = polozkaProperty.InnerText;
                            stockNode.AppendChild(amountNode);

                            newNode.AppendChild(stockNode);
                            break;
                        case "PARAMETRY":
                            XmlNode paramsNode = shoptetFeed.CreateNode(XmlNodeType.Element, "INFORMATION_PARAMETERS", string.Empty);

                            foreach (XmlNode srcParamsNode in polozkaProperty.ChildNodes)
                            {
                                XmlNode paramNode = shoptetFeed.CreateNode(XmlNodeType.Element, "INFORMATION_PARAMETER", string.Empty);

                                XmlNode paramNameNode = shoptetFeed.CreateNode(XmlNodeType.Element, "NAME", string.Empty);
                                XmlNode paramValueNode = shoptetFeed.CreateNode(XmlNodeType.Element, "VALUE", string.Empty);

                                paramNameNode.InnerText = srcParamsNode.Attributes.GetNamedItem("TYP").InnerText;
                                paramValueNode.InnerText = srcParamsNode.FirstChild.InnerText;

                                paramNode.AppendChild(paramNameNode);
                                paramNode.AppendChild(paramValueNode);

                                paramsNode.AppendChild(paramNode);
                            }

                            newNode.AppendChild(paramsNode);
                            break;
                        case "CENA_BEZ_DPH":
                            var priceNode = shoptetFeed.CreateNode(XmlNodeType.Element, "PRICE", string.Empty);
                            var priceVatNode = shoptetFeed.CreateNode(XmlNodeType.Element, "PRICE_VAT", string.Empty);

                            priceNode.InnerText = (double.Parse(polozkaProperty.InnerText) / (1 - (priceRaise - 1))).ToString("F2", System.Globalization.CultureInfo.InvariantCulture);
                            priceVatNode.InnerText = (double.Parse(polozkaProperty.InnerText) / (1 - (priceRaise - 1)) * 1.21).ToString("F2", System.Globalization.CultureInfo.InvariantCulture);

                            newNode.AppendChild(priceNode);
                            newNode.AppendChild(priceVatNode);
                            break;
                        case "CENA_S_DPH":
                        case "CENA_DOPORUCENA":
                        case "ZARUKA":
                        case "NACESTE":
                        case "RECYKLACNI_PRISPEVEK":
                            break;
                        default:
                            if (!NodeConversions.ContainsKey(polozkaProperty.Name))
                            {
                                break;
                            }
                            string newNodePropertyName = NodeConversions[polozkaProperty.Name];
                            XmlNode newNodeProperty = shoptetFeed.CreateNode(XmlNodeType.Element, newNodePropertyName, string.Empty);

                            newNodeProperty.InnerText = polozkaProperty.InnerText;
                            if (!string.IsNullOrEmpty(newNodeProperty.InnerText))
                                newNode.AppendChild(newNodeProperty);
                            break;
                    }
                }

                // Add static parameters
                var outOfStockText = shoptetFeed.CreateNode(XmlNodeType.Element, "AVAILABILITY_OUT_OF_STOCK", string.Empty);
                var inStockText = shoptetFeed.CreateNode(XmlNodeType.Element, "AVAILABILITY_IN_STOCK", string.Empty);

                outOfStockText.InnerText = "Vyprodáno";
                inStockText.InnerText = "Skladem";

                newNode.AppendChild(outOfStockText);
                newNode.AppendChild(inStockText);

                shoptetFeedItems.AppendChild(newNode);
            }

            shoptetFeed.AppendChild(shoptetFeedItems);

            return shoptetFeed;
        }

        // New stuff
        public static ShoptetXmlFeed GetShoptetFeed(bool exportFeed = false)
        {
            double priceRaise = 1.016;
            var sourceFeed = GetData();

            ShoptetXmlFeed shoptetFeed = new ShoptetXmlFeed() { SHOPITEM = new List<SHOPITEM>() };

            foreach (Entry entry in sourceFeed.Entry)
            {
                var newShopItem = new SHOPITEM()
                // Copy some normal values
                {
                    CODE = entry.Article,
                    NAME = entry.ProductTitle,
                    DESCRIPTION = entry.Description.ToString(),
                    EAN = entry.Ean,
                    WEIGHT = entry.Weight.Replace(',', '.'),
                    MANUFACTURER = entry.Brand,
                    AVAILABILITYOUTOFSTOCK = "Vyprodáno",
                    AVAILABILITYINSTOCK = "Skladem",
                    INFORMATIONPARAMETERS = new INFORMATIONPARAMETERS()
                };

                if (!exportFeed)
                {
                    newShopItem.PURCHASEPRICE = entry.Wsp3NoVat.Replace(',', '.').Split(' ').FirstOrDefault();
                }


                if (int.TryParse(entry.StockQuantity, NumberStyles.Any, CultureInfo.InvariantCulture, out int quantity))
                {
                    newShopItem.STOCK = new STOCK() { AMOUNT = quantity };
                }

                // Handle images
                if (!string.IsNullOrWhiteSpace(entry.ImageUrl))
                {
                    newShopItem.IMAGES = newShopItem.IMAGES ?? new IMAGES() { IMAGE = new List<string>() };
                    newShopItem.IMAGES.IMAGE.Add(entry.ImageUrl);
                }

                foreach (var additionalImageUrl in entry.AdditionalImageUrl)
                {
                    if (!string.IsNullOrWhiteSpace(additionalImageUrl))
                    {
                        newShopItem.IMAGES = newShopItem.IMAGES ?? new IMAGES() { IMAGE = new List<string>() };
                        newShopItem.IMAGES.IMAGE.Add(additionalImageUrl);
                    }
                }


                // Handle pricing
                if (exportFeed)
                {
                    newShopItem.PRICE = (double.Parse(entry.Wsp1NoVat.Replace(',', '.').Split(' ').FirstOrDefault().Trim(), NumberStyles.Any, CultureInfo.InvariantCulture) / (1 - (priceRaise - 1))).ToString("F2", System.Globalization.CultureInfo.InvariantCulture);
                    newShopItem.PRICEVAT = (double.Parse(entry.Wsp1NoVat.Replace(',', '.').Split(' ').FirstOrDefault().Trim(), NumberStyles.Any, CultureInfo.InvariantCulture) / (1 - (priceRaise - 1)) * 1.21).ToString("F2", System.Globalization.CultureInfo.InvariantCulture);
                }
                else
                {
                    newShopItem.PRICEVAT = entry.RetailPrice.Replace(',', '.').Split(' ').FirstOrDefault();
                    newShopItem.STANDARDPRICE = entry.RetailPrice.Replace(',', '.').Split(' ').FirstOrDefault();
                }

                shoptetFeed.SHOPITEM.Add(newShopItem);
            }

            return shoptetFeed;
        }


        public static LevenhukNewXmlModel GetData(double priceRaise = 1)
        {
            string url = "https://cdn.t1.levenhuk.com/media/feed/znj_cz_leven_bress_meade_wsp1-2-3.xml";
            var feed = Tools.GetRawXmlFeed(url);

            XmlSerializer serializer = new XmlSerializer(typeof(LevenhukNewXmlModel));
            using (XmlReader reader = new XmlNodeReader(feed))
            {
                return (LevenhukNewXmlModel)serializer.Deserialize(reader);
            }
        }
    }
}
