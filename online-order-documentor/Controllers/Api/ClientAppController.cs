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

namespace online_order_documentor_netcore.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientAppController : Controller
    {
        private IWebHostEnvironment _env;

        public ClientAppController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpGet]
        [Route("version")]
        public IActionResult Index()
        {
            NpmPackage pkg = GetPackageJson(Path.Combine("ClientApp"));

            return this.Json(pkg);
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
