using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml;

namespace online_order_documentor_netcore.Controllers
{
    public static class Tools
    {
        public static ContentResult Xml(this Controller ctrl, string content)
        {
            return new ContentResult
            {
                ContentType = "application/xml",
                Content = content,
                StatusCode = 200,
            };
        }

        public static XmlDocument GetRawXmlFeed(string url)
        {
            XmlDocument feed = new XmlDocument();
            using (WebClient client = new WebClient())
            {
                feed.Load(client.OpenRead(url));
            }

            return feed;
        }

        public static XmlDocument StripByBrandsAndEans(this XmlDocument sourceShoptetFeed, List<string> brands, List<string> eans)
        {
            XmlDocument feed = new XmlDocument();
            XmlNode shoptetFeedItems = feed.CreateNode(XmlNodeType.Element, "SHOP", string.Empty);

            if (eans.Count == 0 && brands.Count == 0)
            {
                return sourceShoptetFeed;
            }

            // brands removal
            var brandsToKeep = sourceShoptetFeed.ChildNodes[1].ChildNodes.Cast<XmlNode>().Where(x => x.ChildNodes.Cast<XmlNode>().Any(y => y.Name == "MANUFACTURER" && brands.Contains(y.InnerText.ToLower())));
            var eansToKeep = sourceShoptetFeed.ChildNodes[1].ChildNodes.Cast<XmlNode>().Where(x => x.ChildNodes.Cast<XmlNode>().Any(y => y.Name == "EAN" && eans.Contains(y.InnerText.ToLower())));

            foreach (var item in brandsToKeep)
            {
                XmlNode importNode = feed.ImportNode(item, true);
                shoptetFeedItems.AppendChild(importNode);
            }
            foreach (var item in eansToKeep)
            {
                XmlNode importNode = feed.ImportNode(item, true);
                shoptetFeedItems.AppendChild(importNode);
            }

            feed.AppendChild(shoptetFeedItems);
            return feed;
        }

        public static XmlDocument CreateAvailabilityFeed(XmlDocument sourceFeed, bool includeEans = false)
        {
            var stockData = GetStockData();

            XmlDocument availabilityFeed = new XmlDocument();
            XmlNode shoptetFeedItems = availabilityFeed.CreateNode(XmlNodeType.Element, "item_list", string.Empty);

            foreach (XmlNode polozka in sourceFeed.ChildNodes.Cast<XmlNode>().FirstOrDefault(x=> x.Name == "SHOP").ChildNodes)
            {
                var newNode = availabilityFeed.CreateNode(XmlNodeType.Element, "item", string.Empty);
                var newNodeAttr = availabilityFeed.CreateAttribute("id");

                var ean = polozka.ChildNodes.Cast<XmlNode>().FirstOrDefault(x => x.Name == "EAN").InnerText.PadLeft(13, '0');
                var id = polozka.ChildNodes.Cast<XmlNode>().FirstOrDefault(x => x.Name == "ITEM_ID").InnerText;

                newNodeAttr.Value = id;
                newNode.Attributes.Append(newNodeAttr);

                if (!string.IsNullOrEmpty(ean) && stockData.ContainsKey(ean))
                {
                    var eanNode = availabilityFeed.CreateNode(XmlNodeType.Element, "EAN", string.Empty);
                    var quantityNode = availabilityFeed.CreateNode(XmlNodeType.Element, "stock_quantity", string.Empty);

                    eanNode.InnerText = ean;
                    quantityNode.InnerText = stockData[ean].ToString();

                    if (includeEans)
                    {
                        newNode.AppendChild(eanNode);
                    }
                    newNode.AppendChild(quantityNode);

                    shoptetFeedItems.AppendChild(newNode);
                }
            }

            availabilityFeed.AppendChild(shoptetFeedItems);

            return availabilityFeed;
        }

        public static XmlDocument CreateAvailabilityFeedShoptet(XmlDocument sourceFeed, List<string> eansToIgnore = null) 
        {
            var stockData = GetStockData();

            XmlDocument availabilityFeed = new XmlDocument();
            XmlNode shoptetFeedItems = availabilityFeed.CreateNode(XmlNodeType.Element, "SHOP", string.Empty);

            foreach (XmlNode polozka in sourceFeed.Cast<XmlNode>().FirstOrDefault(x => x.Name == "SHOP").ChildNodes)
            {
                var newNode = availabilityFeed.CreateNode(XmlNodeType.Element, "SHOPITEM", string.Empty);

                var ean = polozka.ChildNodes.Cast<XmlNode>().FirstOrDefault(x => x.Name == "EAN").InnerText.PadLeft(13, '0');
                var id = polozka.ChildNodes.Cast<XmlNode>().FirstOrDefault(x => x.Name == "ITEM_ID").InnerText;

                if (!string.IsNullOrEmpty(ean) && stockData.ContainsKey(ean))
                {
                    var codeNode = availabilityFeed.CreateNode(XmlNodeType.Element, "CODE", string.Empty);

                    var stockNode = availabilityFeed.CreateNode(XmlNodeType.Element, "STOCK", string.Empty);
                    var amountNode = availabilityFeed.CreateNode(XmlNodeType.Element, "AMOUNT", string.Empty);

                    codeNode.InnerText = id;

                    amountNode.InnerText = stockData[ean].ToString();

                    if (ean == "4046628844363")
                    {

                    }

                    stockNode.AppendChild(amountNode);
                    newNode.AppendChild(codeNode);
                    newNode.AppendChild(stockNode);

                    if (eansToIgnore == null || !eansToIgnore.Contains(ean))
                    {
                        shoptetFeedItems.AppendChild(newNode);
                    }
                }
            }

            availabilityFeed.AppendChild(shoptetFeedItems);

            return availabilityFeed;
        }

        public static XmlDocument CreateAvailabilityFeedHeureka(XmlDocument sourceFeed, List<string> eansToIgnore = null)
        {
            var stockData = GetStockData();

            XmlDocument availabilityFeed = new XmlDocument();
            XmlNode heaurekaFeedItems = availabilityFeed.CreateNode(XmlNodeType.Element, "item_list", string.Empty);

            foreach (XmlNode polozka in sourceFeed.Cast<XmlNode>().FirstOrDefault(x => x.Name == "SHOP").ChildNodes)
            {
                var newNode = availabilityFeed.CreateNode(XmlNodeType.Element, "item", string.Empty);

                var ean = polozka.ChildNodes.Cast<XmlNode>().FirstOrDefault(x => x.Name == "EAN").InnerText.PadLeft(13, '0');
                var id = polozka.ChildNodes.Cast<XmlNode>().FirstOrDefault(x => x.Name == "ITEM_ID").InnerText;

                if (!string.IsNullOrEmpty(ean) && stockData.ContainsKey(ean))
                {
                    var itemIdAttribute = availabilityFeed.CreateAttribute("id");

                    var stockQuantityNode = availabilityFeed.CreateNode(XmlNodeType.Element, "stock_quantity", string.Empty);

                    itemIdAttribute.InnerText = id;
                    stockQuantityNode.InnerText = stockData[ean].ToString();

                    newNode.Attributes.Append(itemIdAttribute);
                    newNode.AppendChild(stockQuantityNode);

                    if (eansToIgnore == null || !eansToIgnore.Contains(ean))
                    {
                        heaurekaFeedItems.AppendChild(newNode);
                    }
                }
            }

            availabilityFeed.AppendChild(heaurekaFeedItems);

            return availabilityFeed;
        }


        public static Dictionary<string, int> GetStockData()
        {
            XmlDocument availabilityFeed = new XmlDocument();

            var storage = Providers.StorageProviderFactory.Create(true);
            using (var downloadedFile = storage.Download(AppVariables.FtpAvailabilityXmlPath))
            {
                availabilityFeed.Load(downloadedFile);
            }

            //Dictionary by int
            Dictionary<string, int> stockData = new Dictionary<string, int>();

            foreach (XmlNode polozka in availabilityFeed.ChildNodes[1].ChildNodes)
            {
                var key = polozka.ChildNodes.Cast<XmlNode>().FirstOrDefault(x => x.Name == "sloupec03").InnerText.PadLeft(13, '0');
                var stringAmount = polozka.ChildNodes.Cast<XmlNode>().FirstOrDefault(x => x.Name == "sloupec04").InnerText.Replace(",00", string.Empty);

                int amount;
                if (!string.IsNullOrEmpty(key) && !stockData.ContainsKey(key) && int.TryParse(stringAmount, out amount))
                {
                    if (key == "4046628844363")
                    {

                    }

                    if (amount < 0)
                    {  // prevent stock amount to go below 0, fix for accounting system deficiencies
                        var infoText = $"Normalized stock amount: [{key}]: {amount} -> 0";
                        //Trace.WriteLine(infoText);
                        //Debug.WriteLine(infoText);
                        // Console.WriteLine(infoText);
                        amount = 0;
                    }

                    stockData.Add(key, amount);
                }
            }

            return stockData;
        }
    }
}
