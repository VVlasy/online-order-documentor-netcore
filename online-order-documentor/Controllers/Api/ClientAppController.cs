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

namespace online_order_documentor_netcore.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientAppController : Controller
    {
        [HttpGet]
        [Route("version")]
        public IActionResult Index([FromServices] IWebHostEnvironment env)
        {
            var files = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "ClientApp"));
            var pkg = files.GetFileInfo("package.json");
            
            var serializer = new JsonSerializer();

            using (var sr = new StreamReader(pkg.CreateReadStream()))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                return this.Json(serializer.Deserialize<NpmPackage>(jsonTextReader).Version);
            }           
        }

        private class NpmPackage
        {
            public string Version { get; set; }
        }
    }    
}
