using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
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
            {"KOD", "CODE" },
            {"NAZEV", "NAME" },
            {"OBRAZEK", "IMAGE" },
            {"ZASOBA", "AMOUNT" },
            {"POPIS", "DESCRIPTION" },
            {"HMOTNOST", "WEIGHT" },
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
            XmlDocument shoptetFeed = GetShoptetFeed(1.016);

            return this.Xml(shoptetFeed.OuterXml);
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

        public static DATA GetData(double priceRaise = 1)
        {
            string url = "http://www.levenhukshop.cz/seznam-zbozi.aspx";
            var feed = Tools.GetRawXmlFeed(url);

            XmlSerializer serializer = new XmlSerializer(typeof(DATA));
            using (XmlReader reader = new XmlNodeReader(feed))
            {
                return (DATA)serializer.Deserialize(reader);
            }
        }


        // XML Model
        [XmlRoot(ElementName = "PARAMETR")]
        public class PARAMETR
        {

            [XmlAttribute(AttributeName = "TYP")]
            public string TYP { get; set; }

            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "PARAMETRY")]
        public class PARAMETRY
        {

            [XmlElement(ElementName = "PARAMETR")]
            public List<PARAMETR> PARAMETR { get; set; }
        }

        [XmlRoot(ElementName = "POLOZKA")]
        public class POLOZKA
        {

            [XmlElement(ElementName = "NAZEV")]
            public string NAZEV { get; set; }

            [XmlElement(ElementName = "KOD")]
            public string KOD { get; set; }

            [XmlElement(ElementName = "EAN")]
            public string EAN { get; set; }

            [XmlElement(ElementName = "OBRAZEK")]
            public string OBRAZEK { get; set; }

            [XmlElement(ElementName = "CENA_DOPORUCENA")]
            public string CENADOPORUCENA { get; set; }

            [XmlElement(ElementName = "CENA_BEZ_DPH")]
            public string CENABEZDPH { get; set; }

            [XmlElement(ElementName = "CENA_S_DPH")]
            public string CENASDPH { get; set; }

            [XmlElement(ElementName = "POPIS")]
            public string POPIS { get; set; }

            [XmlElement(ElementName = "ZASOBA")]
            public int ZASOBA { get; set; }

            [XmlElement(ElementName = "NACESTE")]
            public int NACESTE { get; set; }

            [XmlElement(ElementName = "ZARUKA")]
            public int ZARUKA { get; set; }

            [XmlElement(ElementName = "HMOTNOST")]
            public string HMOTNOST { get; set; }

            [XmlElement(ElementName = "PARAMETRY")]
            public PARAMETRY PARAMETRY { get; set; }

            [XmlElement(ElementName = "RECYKLACNI_PRISPEVEK")]
            public string RECYKLACNIPRISPEVEK { get; set; }

            [XmlElement(ElementName = "OBRAZKY_DALSI")]
            public OBRAZKYDALSI OBRAZKYDALSI { get; set; }
        }

        [XmlRoot(ElementName = "OBRAZKY_DALSI")]
        public class OBRAZKYDALSI
        {

            [XmlElement(ElementName = "OBRAZEK")]
            public List<string> OBRAZEK { get; set; }
        }

        [XmlRoot(ElementName = "ZBOZI")]
        public class ZBOZI
        {

            [XmlElement(ElementName = "POLOZKA")]
            public List<POLOZKA> POLOZKA { get; set; }
        }

        [XmlRoot(ElementName = "DATA")]
        public class DATA
        {

            [XmlElement(ElementName = "ZBOZI")]
            public ZBOZI ZBOZI { get; set; }
        }
    }
}
