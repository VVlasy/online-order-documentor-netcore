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
        private IWebHostEnvironment _env;

        public ClientAppController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpGet]
        [Route("version")]
        public IActionResult Index()
        {
            NpmPackage pkg;

            try
            {
                try
                {
                    pkg = GetPackageJson(Path.Combine("ClientApp", "build"));
                }
                catch (DirectoryNotFoundException)
                {
                    pkg = GetPackageJson("ClientApp");
                }
                catch (FileNotFoundException)
                {
                    pkg = GetPackageJson("ClientApp");
                }
            }
            catch (Exception)
            {
                throw;
            }
            

            return this.Json(pkg);
        }

        [HttpGet]
        [Route("GetFiles")]
        public IActionResult GetFiles([FromQuery(Name = "dir")] string dir)
        {
            return this.Json(Directory.GetFiles(dir));
        }

        [HttpGet]
        [Route("GetFile")]
        public IActionResult GetFile([FromQuery(Name = "file")] string file)
        {
            return this.Json(System.IO.File.ReadAllText(file));
        }

        [HttpGet]
        [Route("GetDirectories")]
        public IActionResult GetDirectories([FromQuery(Name = "dir")] string dir)
        {
            return this.Json(Directory.GetDirectories(dir));
        }

        [HttpGet]
        [Route("GetCurrentDir")]
        public IActionResult GetCurrentDir()
        {
            return this.Json(Directory.GetCurrentDirectory());
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
