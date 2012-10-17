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

        public ConversionOptions Options { get; set; }
        
        public IHubContext HubContext { get { return  GlobalHost.ConnectionManager.GetHubContext<ConvertHub>();}
        }


        [HttpGet]
        public ActionResult Index()
        {
            return View(new ConversionOptions());
        }
        [HttpPost]
        public ActionResult Index(ConversionOptions options)
        {
            //Handle the file upload.
            Options = options;
            if (!ModelState.IsValid || (options.EbookFile == null && options.EbookUrl == null))
            {
                throw new HttpException(500, "Posted Data Invalid. Please provide an Ebook Url or File");
            }

            String saveFolder = Path.Combine(HttpContext.Server.MapPath("~/App_Data/"), "uploads", options.Guid);
            Directory.CreateDirectory(saveFolder);
            String saveFile = "";
            String extension = "";

            if (options.EbookFile != null)
            {
                HubContext.Clients[Options.Guid].logInfo(String.Format("File Uploaded. Validating... "));

                saveFile = Path.Combine(saveFolder, Path.GetFileName(options.EbookFile.FileName));
                extension = Path.GetExtension(saveFile);

                CheckExtension(extension, options.OutputExtension);

                options.EbookFile.SaveAs(saveFile);

                Convert(options, saveFile);

            }
            else
            {
                //File Downloading Begin, Inform user.
                HubContext.Clients[Options.Guid].logInfo(String.Format("Validating Url to download... "));
                //Download and parse an ebook file. 
                
                try
                {
                    extension = System.IO.Path.GetExtension(options.EbookUrl.GetLeftPart(UriPartial.Path));
                }
                catch (Exception ex)
                {
                    extension = ".epub"; //assuming epub.
                }
                CheckExtension(extension, options.OutputExtension);
                saveFile = Path.Combine(saveFolder, "tempfile" + extension);

                using (WebClient client = new WebClient())
                {
                    if (options.isAsync)
                    {
                        HubContext.Clients[Options.Guid].logInfo(String.Format("Downloading Ebook Async... "));
                        client.DownloadFileCompleted += (s, args) =>
                                    {
                                        //File Downloaded, inform user.
                                        HubContext.Clients[Options.Guid].logInfo(String.Format("Download Completed. "));
                                    };

                        client.DownloadFileAsync(options.EbookUrl, saveFile);
                        Convert(options, saveFile);
                    }
                    else
                    {
                        HubContext.Clients[Options.Guid].logInfo(String.Format("Downloading Ebook..."));
                        client.DownloadFile(options.EbookUrl, saveFile);
                        HubContext.Clients[Options.Guid].logInfo(String.Format("Download Completed. "));
                        Convert(options, saveFile);
                    }
                }

            }

            return View(options);
        }

        private static void CheckExtension(String fileExt, OutputExtension extension)
        {
            if (String.Equals(fileExt, extension.Value(), StringComparison.InvariantCultureIgnoreCase))
            {
                throw new ArgumentException("Output filetype is the same as the current filetype.");
            }

        }

        private void Convert(ConversionOptions options, String filePath)
        {

            Task.Factory.StartNew(() =>
                {
                    
                    string root = HttpContext.Server.MapPath("~/App_Data/");
                    String calibreEbookConvert = Path.Combine(root, "Calibre/Calibre/ebook-convert.exe");
                    String output = Path.Combine(Path.GetDirectoryName(filePath), "convertedfile." + options.OutputExtension);


                    using (Process p = new Process())
                    {
                        p.StartInfo.UseShellExecute = false;
                        p.StartInfo.FileName = calibreEbookConvert;
                        p.StartInfo.RedirectStandardError = true;
                        p.StartInfo.RedirectStandardOutput = true;
                        p.StartInfo.CreateNoWindow = true;
                        p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        p.StartInfo.Arguments = String.Format("{0} {1}", filePath, output);
                        HubContext.Clients[Options.Guid].logSticky(String.Format("Beginning Process to Convert Ebook... Please Wait "));
                        p.Start();
                        p.BeginOutputReadLine();
                        p.BeginErrorReadLine();
                        p.OutputDataReceived += new DataReceivedEventHandler(p_OutputDataReceived);
                        p.ErrorDataReceived += new DataReceivedEventHandler(p_ErrorDataReceived);
                        p.WaitForExit();
                        HubContext.Clients[Options.Guid].logSticky(String.Format("File Conversion Completed."));
                        HubContext.Clients[Options.Guid].logSuccess(String.Format("<a href='http://www.google.ca'>Click here to download your ebook.</a>"));
                    }
            

                });

            
        }

        void p_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            //Handle errors here
            if(!String.IsNullOrEmpty(e.Data))
            {
                HubContext.Clients[Options.Guid].logError(String.Format("[Error] {0}", e.Data));

            }
            
        }

        void p_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            //output.Add(e.Data);
            HubContext.Clients[Options.Guid].logInfo(String.Format("ebook-convert > {0}", e.Data));
        }

    }
}
