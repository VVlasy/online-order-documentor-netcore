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
    [Route("api/[controller]")]
    public class LevenhukController : Controller
    {
        public static Dictionary<string, string> NodeConversions = new Dictionary<string, string>()
        {
            {"KOD", "CODE" },
            {"NAZEV", "NAME" },
            {"OBRAZEK", "IMAGE" },
            {"ZASOBA", "AMOUNT" },
            {"POPIS", "DESCRIPTION" },
        };

        [HttpGet]
        [Route("feed.xml")]
        public IActionResult RawFeed()
        {
            string url = "http://www.levenhukshop.cz/seznam-zbozi.aspx";
            var feed = Tools.GetRawXmlFeed(url);
            return this.Xml(feed.OuterXml);
        }

        [HttpGet]
        [Route("feed-shoptet.xml")]
        public IActionResult ShoptetFeed()
        {
            var shoptetFeed = GetShoptetFeed();

            return this.Xml(shoptetFeed.OuterXml);
        }

        [HttpGet]
        [Route("feed-raised.xml")]
        public IActionResult RaisedFeed()
        {

            return base.Ok();
        }

        public static XmlDocument GetShoptetFeed(double priceRaise = 1)
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
                            XmlNode imagesNode = shoptetFeed.CreateNode(XmlNodeType.Element, "IMAGES", string.Empty);
                            XmlNode imageNode = shoptetFeed.CreateNode(XmlNodeType.Element, "IMAGE", string.Empty);
                            imageNode.InnerText = polozkaProperty.InnerText;
                            imagesNode.AppendChild(imageNode);

                            newNode.AppendChild(imagesNode);
                            break;
                        case "ZASOBA":
                            XmlNode stockNode = shoptetFeed.CreateNode(XmlNodeType.Element, "STOCK", string.Empty);
                            XmlNode amountNode = shoptetFeed.CreateNode(XmlNodeType.Element, "AMOUNT", string.Empty);
                            stockNode.InnerText = polozkaProperty.InnerText;
                            amountNode.AppendChild(stockNode);

                            newNode.AppendChild(amountNode);
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

                            priceNode.InnerText = (double.Parse(polozkaProperty.InnerText) / (1 - (priceRaise - 1))).ToString("N2", System.Globalization.CultureInfo.InvariantCulture);
                            priceVatNode.InnerText = (double.Parse(polozkaProperty.InnerText) / (1 - (priceRaise - 1)) * 1.21).ToString("N2", System.Globalization.CultureInfo.InvariantCulture);

                            newNode.AppendChild(priceNode);
                            newNode.AppendChild(priceVatNode);
                            break;
                        case "CENA_S_DPH":
                        case "CENA_DOPORUCENA":
                        case "ZARUKA":
                        case "NACESTE":
                            break;
                        default:
                            string newNodePropertyName = NodeConversions.ContainsKey(polozkaProperty.Name) ? NodeConversions[polozkaProperty.Name] : polozkaProperty.Name;
                            XmlNode newNodeProperty = shoptetFeed.CreateNode(XmlNodeType.Element, newNodePropertyName, string.Empty);

                            newNodeProperty.InnerText = polozkaProperty.InnerText;
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
    }
}
