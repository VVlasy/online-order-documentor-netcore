using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeTypes;
using online_order_documentor_netcore.Models;
using System;
using System.IO;
using System.Text;

namespace online_order_documentor_netcore.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class UploadImageController : Controller
    {
        [HttpPost]
        public IActionResult Index([FromForm] UploadImageModel model)
        {
            if (string.IsNullOrEmpty(model.Name))
            {
                return base.BadRequest("Missing name parameter");
            }

            if (model.Images == null || model.Images.Length == 0)
            {
                return base.BadRequest("Missing images parameter");
            }

            string imageFolderUri = AppVariables.FtpPhotosFolder + "/" + model.Name;

            Providers.IStorageProvider storage = Providers.StorageProviderFactory.Create();
            if (!storage.FolderExists(imageFolderUri))
            {
                storage.CreateFolder(imageFolderUri);
            }
            else
            {
                storage.ClearFolder(imageFolderUri);
            }

            int nameIndex = 1;
            foreach (IFormFile file in model.Images)
            {
                using (var s = file.OpenReadStream())
                {
                    storage.Upload(s, string.Format("{0}/{1}{2}", imageFolderUri, nameIndex, MimeTypeMap.GetExtension(file.ContentType)));
                }

                nameIndex++;
            }

            return base.Ok();
        }
    }
}
