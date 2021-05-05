using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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
                StatusCode = 200
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
    }
}
