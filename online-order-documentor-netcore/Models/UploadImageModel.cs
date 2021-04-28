using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace online_order_documentor_netcore.Models
{
    public class UploadImageModel
    {
        public string Name { get; set; }

        public IFormFile[] Images { get; set; }
    }
}
