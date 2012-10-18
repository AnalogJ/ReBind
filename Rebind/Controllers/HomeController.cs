using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Rebind.Infastructure;
using Rebind.Models;
using SignalR;
using SignalR.Hubs;

namespace Rebind.Controllers
{
    public class HomeController : Controller
    {

        


        [HttpGet]
        public ActionResult Index()
        {
            return View(new ConversionOptions());
        }
        [HttpPost]
        public ActionResult Index(ConversionOptions options)
        {
            //Handle the file upload.
            if (!ModelState.IsValid || (options.EbookFile == null && options.EbookUrl == null))
            {
                throw new HttpException(500, "Posted Data Invalid. Please provide an Ebook Url or File");
            }

            
            String extension = "";

            //Validate extensions
            if (options.EbookFile != null)
            {
                extension = Path.GetExtension(options.EbookFile.FileName);
                CheckExtension(extension, options.OutputExtension);
            }
            else
            {
                try
                {
                    extension = System.IO.Path.GetExtension(options.EbookUrl.GetLeftPart(UriPartial.Path));
                }
                catch (Exception ex)
                {
                    extension = ".epub"; //assuming epub.
                }
                CheckExtension(extension, options.OutputExtension);
            }


            String root = HttpContext.Server.MapPath("~/");
            EbookConvert converter = new EbookConvert(root, options);
            String localPath = converter.Start(extension);

            options.isStarted = true;


            return View(options);
        }

        [HttpGet]
        public ActionResult Convert(Uri postedUrl, String outputExtension)
        {
            String extension;
            ConversionOptions options = new ConversionOptions();
            options.EbookUrl = postedUrl;
            options.Extension = outputExtension;
            options.isAsync = false;
            options.isStarted = true;

            if (!ModelState.IsValid || ( options.EbookUrl == null))
            {
                throw new HttpException(500, "Posted Data Invalid. Please provide an Ebook Url or File");
            }
            
            try
            {
                extension = System.IO.Path.GetExtension(options.EbookUrl.GetLeftPart(UriPartial.Path));
            }
            catch (Exception ex)
            {
                extension = ".epub"; //assuming epub.
            }
            CheckExtension(extension, options.OutputExtension);
            String root = HttpContext.Server.MapPath("~/");
            EbookConvert converter = new EbookConvert(root, options);
            String localPath = converter.Start(extension);

            options.isStarted = true;


            Uri outputLocation = new Uri(HttpContext.Request.Url,
                                         @"/uploads/" + options.Guid + @"/" + Path.GetFileName(localPath));
            return Redirect(outputLocation.ToString());
        }


        private static void CheckExtension(String fileExt, OutputExtension extension)
        {
            if (String.Equals(fileExt, extension.Value(), StringComparison.InvariantCultureIgnoreCase))
            {
                throw new ArgumentException("Output filetype is the same as the current filetype.");
            }



        }

        
    }
}
