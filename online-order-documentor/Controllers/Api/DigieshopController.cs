using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeTypes;
using online_order_documentor_netcore.Models;
using online_order_documentor_netcore.Providers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace online_order_documentor_netcore.Controllers.Api
{
    [ApiController]
    [Route("api/digi-eshop")]
    public class DigieshopController : Controller
    {
        private ResponseCache<object> _cache;

        public DigieshopController(CacheProvider cacheProvider)
        {
            _cache = cacheProvider.GetCache<object>("Digieshop");
        }

        [HttpGet]
        [Route("{**filter}")]
        public async Task<IActionResult> Crossroad(string filter)
        {
            string[] filters = filter.Split('/');
            string destination = filters[filters.Length - 1];
            Array.Resize(ref filters, filters.Length - 1);

            List<string> brands = new List<string>();
            List<string> eans = new List<string>();


            foreach (string item in filters)
            {
                if (item.StartsWith("EAN="))
                {
                    eans.Add(item.Substring(4).ToLower().PadLeft(13, '0'));
                }
                else
                {
                    brands.Add(item.ToLower());
                }
            }

            string key;
            switch (destination)
            {
                case "data.xml":
                    key = $"data.xml&{string.Join(";", brands)}&{string.Join(";", eans)}";
                    _cache.Register(key, () => DataFeed(brands, eans));
                    return await _cache.Get(key) as IActionResult;
                case "availability.xml":
                    key = $"availability.xml&{string.Join(";", brands)}&{string.Join(";", eans)}";
                    _cache.Register(key, () => AvailabilityFeed(brands, eans));
                    return await _cache.Get(key) as IActionResult;
                case "availability-shoptet.xml":
                    key = $"availability-shoptet.xml&{string.Join(";", brands)}&{string.Join(";", eans)}";
                    _cache.Register(key, () => ShoptetAvailabilityFeed(brands, eans));
                    return await _cache.Get(key) as IActionResult;
                case "availability-heureka.xml":
                    key = $"availability-heureka.xml&{string.Join(";", brands)}&{string.Join(";", eans)}";
                    _cache.Register(key, () => HeurekaAvailabilityFeed(brands, eans));
                    return await _cache.Get(key) as IActionResult;
                default:
                    return base.BadRequest("Invalid Path");
            }
        }

        private IActionResult DataFeed(List<string> brands, List<string> eans)
        {
            var url = string.Format("https://www.digi-eshop.cz/universal.xml?hash={0}", AppVariables.DigiEshopHash);
            var feed = Tools.StripByBrandsAndEans(Tools.GetRawXmlFeed(url), brands, eans);

            return this.Xml(feed.OuterXml);
        }

        private IActionResult AvailabilityFeed(List<string> brands, List<string> eans)
        {
            var url = string.Format("https://www.digi-eshop.cz/universal.xml?hash={0}", AppVariables.DigiEshopHash);
            var feed = Tools.StripByBrandsAndEans(Tools.GetRawXmlFeed(url), brands, eans);

            feed = Tools.CreateAvailabilityFeed(feed);

            return this.Xml(feed.OuterXml);
        }

        private IActionResult ShoptetAvailabilityFeed(List<string> brands, List<string> eans)
        {
            var url = string.Format("https://www.digi-eshop.cz/universal.xml?hash={0}", AppVariables.DigiEshopHash);
            var feed = Tools.StripByBrandsAndEans(Tools.GetRawXmlFeed(url), brands, eans);

            IEnumerable<string> levenhukEans = LevenhukController.GetData().ZBOZI.POLOZKA.Select(x=>x.EAN);

            List<string> eansToRemove = levenhukEans.ToList();

            feed = Tools.CreateAvailabilityFeedShoptet(feed, eansToRemove);

            return this.Xml(feed.OuterXml);
        }

        private IActionResult HeurekaAvailabilityFeed(List<string> brands, List<string> eans)
        {
            var url = string.Format("https://www.digi-eshop.cz/universal.xml?hash={0}", AppVariables.DigiEshopHash);
            var feed = Tools.StripByBrandsAndEans(Tools.GetRawXmlFeed(url), brands, eans);

            IEnumerable<string> levenhukEans = LevenhukController.GetData().ZBOZI.POLOZKA.Select(x => x.EAN);

            List<string> eansToRemove = levenhukEans.ToList();

            feed = Tools.CreateAvailabilityFeedHeureka(feed, eansToRemove);

            return this.Xml(feed.OuterXml);
        }
    }
}
