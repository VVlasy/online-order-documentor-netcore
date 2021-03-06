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
using System.Reflection;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using Microsoft.Extensions.Hosting;
using online_order_documentor_netcore.Providers;
using System.Net.Http;
using System.Threading.Tasks;

namespace online_order_documentor_netcore.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientAppController : Controller
    {
        private IWebHostEnvironment _env;

        private PremierApiService _premierApiService;

        public ClientAppController(IWebHostEnvironment env, PremierApiService premierApiService)
        {
            _env = env;
            _premierApiService = premierApiService;
        }

        [HttpGet]
        [Route("version")]
        public IActionResult Index()
        {
            NpmPackage pkg = GetPackageJson(Path.Combine("ClientApp"));

            return this.Json(pkg);
        }

        [HttpGet]
        [Route("premier")]
        public IActionResult PremierInfo()
        {
            return this.Json(PremierApiService.ApiVersion);
        }

        private class PublicIpObject
        {
            public string Ip { get; set; }
        }

        [HttpGet]
        [Route("publicIp")]
        public async Task<IActionResult> PublicIp()
        {
            HttpResponseMessage IpData = await new HttpClient().GetAsync("https://api.ipify.org?format=json");

            PublicIpObject ipObj = JsonConvert.DeserializeObject<PublicIpObject>(await IpData.Content.ReadAsStringAsync());

            return this.Json(ipObj);
        }


        private NpmPackage GetPackageJson(string directory)
        {
            PhysicalFileProvider provider = new PhysicalFileProvider(Path.Combine(_env.ContentRootPath, directory)); ;

            var pkg = provider.GetFileInfo("package.json");
            var serializer = new JsonSerializer();

            using (var sr = new StreamReader(pkg.CreateReadStream()))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                return serializer.Deserialize<NpmPackage>(jsonTextReader);
            }
        }

        private class NpmPackage
        {
            public string Version { get; set; }
        }
    }
}
