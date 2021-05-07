using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Xml;
using System.Linq;
using System.Globalization;
using System.Threading;

namespace online_order_documentor_netcore.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class SetosController : Controller
    {
        public static Dictionary<string, string> NodeConversions = new Dictionary<string, string>()
        {
            {"Cislo", "CODE" },
            {"Popis", "NAME" },
            {"Kratky_popis", "SHORT_DESCRIPTION" },
            {"Vyrobce", "MANUFACTURER" },
            {"Individualni_cena_zakaznika", "PURCHASE_PRICE" },
            {"Doporucena_cena", "PRICE" },
            {"Kategorie_zbozi", "CATEGORY" }
        };

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
            var shoptetFeed = GetShoptetFeed();

            return this.Xml(shoptetFeed.OuterXml);
        }

        public static XmlDocument GetShoptetFeed()
        {
            var setosCulture = new CultureInfo("cs-CZ");

            string url = "http://www.setos.cz/upload/xml/promchat83125.xml";
            var feed = Tools.GetRawXmlFeed(url);

            XmlDocument shoptetFeed = new XmlDocument();
            XmlNode shoptetFeedItems = shoptetFeed.CreateNode(XmlNodeType.Element, "SHOP", string.Empty);

            // convert children of raw feed
            foreach (XmlNode polozka in feed.ChildNodes[2].ChildNodes)
            {
                XmlNode newNode = shoptetFeed.CreateNode(XmlNodeType.Element, "SHOPITEM", string.Empty);

                foreach (XmlNode polozkaProperty in polozka.ChildNodes)
                {
                    switch (polozkaProperty.Name)
                    {
                        case "Obrazek":
                            XmlNode imagesNode = shoptetFeed.CreateNode(XmlNodeType.Element, "IMAGES", string.Empty);
                            XmlNode imageNode = shoptetFeed.CreateNode(XmlNodeType.Element, "IMAGE", string.Empty);
                            imageNode.InnerText = polozkaProperty.InnerText;
                            imagesNode.AppendChild(imageNode);

                            newNode.AppendChild(imagesNode);
                            break;
                        case "Dostupnost":
                            XmlNode stockNode = shoptetFeed.CreateNode(XmlNodeType.Element, "STOCK", string.Empty);
                            XmlNode amountNode = shoptetFeed.CreateNode(XmlNodeType.Element, "AMOUNT", string.Empty);
                            amountNode.InnerText = polozkaProperty.InnerText;
                            stockNode.AppendChild(amountNode);

                            newNode.AppendChild(stockNode);
                            break;
                        case "Kategorie_zbozi":
                            XmlNode catgsNode = shoptetFeed.CreateNode(XmlNodeType.Element, "CATEGORIES", string.Empty);
                            XmlNode catgNode = shoptetFeed.CreateNode(XmlNodeType.Element, "CATEGORY", string.Empty);
                            catgNode.InnerText = polozkaProperty.InnerText;
                            catgsNode.AppendChild(catgNode);

                            newNode.AppendChild(catgsNode);
                            break;
                        case "Technicke_parametry":
                            XmlNode paramsNode = shoptetFeed.CreateNode(XmlNodeType.Element, "INFORMATION_PARAMETERS", string.Empty);

                            foreach (XmlNode srcParamsNode in polozkaProperty.ChildNodes)
                            {
                                XmlNode paramNode = shoptetFeed.CreateNode(XmlNodeType.Element, "INFORMATION_PARAMETER", string.Empty);

                                XmlNode paramNameNode = shoptetFeed.CreateNode(XmlNodeType.Element, "NAME", string.Empty);
                                XmlNode paramValueNode = shoptetFeed.CreateNode(XmlNodeType.Element, "VALUE", string.Empty);

                                paramNameNode.InnerText = srcParamsNode.ChildNodes[1].InnerText;
                                paramValueNode.InnerText = srcParamsNode.ChildNodes[2].InnerText;

                                paramNode.AppendChild(paramNameNode);
                                paramNode.AppendChild(paramValueNode);

                                if (!string.IsNullOrEmpty(paramNameNode.InnerText))
                                    paramsNode.AppendChild(paramNode);
                            }

                            newNode.AppendChild(paramsNode);
                            break;
                        case "Doporucena_cena":
                            XmlNode priceNode = shoptetFeed.CreateNode(XmlNodeType.Element, "PRICE", string.Empty);

                            double price;

                            if (!double.TryParse(polozkaProperty.InnerText, NumberStyles.Any, setosCulture, out price))
                                break;

                            priceNode.InnerText = price.ToString("F2", CultureInfo.InvariantCulture);

                            newNode.AppendChild(priceNode);
                            break;
                        case "Individualni_cena_zakaznika":
                            XmlNode purchasePriceNode = shoptetFeed.CreateNode(XmlNodeType.Element, "PURCHASE_PRICE", string.Empty);

                            double purchasePrice;

                            if (!double.TryParse(polozkaProperty.InnerText, NumberStyles.Any, setosCulture, out purchasePrice))
                                break;

                            purchasePriceNode.InnerText = (purchasePrice * 1.21).ToString("F2", CultureInfo.InvariantCulture);
                            newNode.AppendChild(purchasePriceNode);
                            break;
                        case "EAN":
                            XmlNode eanNode = shoptetFeed.CreateNode(XmlNodeType.Element, "EAN", string.Empty);

                            eanNode.InnerText = polozkaProperty.InnerText.Length > 14 ? polozkaProperty.InnerText.Substring(0, 14) : polozkaProperty.InnerText;

                            if (!string.IsNullOrEmpty(eanNode.InnerText))
                                newNode.AppendChild(eanNode);
                            break;
                        case "Popis_2":
                        case "Elektr__odpad_-_poplatek":
                        case "OSA_-_poplatek":
                        case "Obsah_baleni":
                        case "Ikonka":
                        case "Stav":
                            break;
                        default:
                            string newNodePropertyName = NodeConversions.ContainsKey(polozkaProperty.Name) ? NodeConversions[polozkaProperty.Name] : polozkaProperty.Name;
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
                var vatText = shoptetFeed.CreateNode(XmlNodeType.Element, "VAT", string.Empty);

                outOfStockText.InnerText = "očekáváme naskladnění";
                inStockText.InnerText = "Skladem";
                vatText.InnerText = "21";

                newNode.AppendChild(outOfStockText);
                newNode.AppendChild(inStockText);
                newNode.AppendChild(vatText);

                if (newNode.ChildNodes.Cast<XmlNode>().Any(x => x.Name == "NAME" && x.InnerText.ToLower().Contains("lowepro")) ||
                    newNode.ChildNodes.Cast<XmlNode>().Any(x => x.Name == "NAME" && x.InnerText.ToLower().Contains("joby")) ||
                    newNode.ChildNodes.Cast<XmlNode>().Any(x => x.Name == "MANUFACTURER" && x.InnerText.ToLower().Contains("lowepro")) ||
                    newNode.ChildNodes.Cast<XmlNode>().Any(x => x.Name == "MANUFACTURER" && x.InnerText.ToLower().Contains("joby")) ||
                    newNode.ChildNodes.Cast<XmlNode>().Any(x => x.Name == "SHORT_DESCRIPTION" && x.InnerText.ToLower().Contains("lowepro")) ||
                    newNode.ChildNodes.Cast<XmlNode>().Any(x => x.Name == "SHORT_DESCRIPTION" && x.InnerText.ToLower().Contains("joby")))
                    continue;
                shoptetFeedItems.AppendChild(newNode);
            }

            shoptetFeed.AppendChild(shoptetFeedItems);

            return shoptetFeed;
        }
    }
}
