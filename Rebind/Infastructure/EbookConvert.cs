using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Rebind.Models;
using SignalR;
using SignalR.Hubs;

namespace Rebind.Infastructure
{
    public class EbookConvert
    {

        public IHubContext HubContext
        {
            get { return GlobalHost.ConnectionManager.GetHubContext<ConvertHub>(); }
        }
        public ConversionOptions Options { get; set; }
        public String Root { get; set; }

        public EbookConvert(String root, ConversionOptions options)
        {
            Root = root;
            Options = options;
        }
    



        /// <summary>
        /// Saves the file, depending on the options.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="extension">The extension.</param>
        /// <returns>Input file saved local location.</returns>
        public String Start( String extension)
        {
            ClientLogging( LogLevel.Info, "Processing Input Options...");

            String saveFolder = Path.Combine(Root, @"uploads\", Options.Guid);
            Directory.CreateDirectory(saveFolder);
            String saveFile;

            if (Options.EbookFile != null)
            {
                saveFile = Path.Combine(saveFolder, Path.GetFileName(Options.EbookFile.FileName));

                Options.EbookFile.SaveAs(saveFile);
                ClientLogging(LogLevel.Info, "Finished Processing Input Options...");
                if(Options.isAsync)
                {
                    Task.Factory.StartNew(() =>
                    {
                        Convert(saveFile);
                    });
                }
                else
                {
                    Convert(saveFile);
                }
                
            }
            else
            {
                using (WebClient client = new WebClient())
                {
                    saveFile = Path.Combine(saveFolder, "downloadedfile" + extension);
                    if(Options.isAsync)
                    {
                        ClientLogging(LogLevel.Info, "Downloading Input File...");
                        client.DownloadDataCompleted += (sender, args) =>
                            {
                                ClientLogging(LogLevel.Info, "Downloading File Async Complete");
                                ClientLogging(LogLevel.Info, "Finished Processing Input Options...");
                                Task.Factory.StartNew(() =>
                                    {
                                        Convert(saveFile);
                                    });
                                
                            };
                        client.DownloadDataAsync(Options.EbookUrl);
                    }
                    else
                    {
                        ClientLogging(LogLevel.Info, "Downloading Input File...");
                        client.DownloadFile(Options.EbookUrl, saveFile);
                        ClientLogging(LogLevel.Info, "Downloading File Complete");
                        ClientLogging(LogLevel.Info, "Finished Processing Input Options...");
                        Convert(saveFile);
                    }
                    
                }

            }
            
            return saveFile;
        }


        private void Convert( String inputFile)
        {
            if(String.IsNullOrEmpty(inputFile))
            {
                throw new ArgumentNullException("inputFile");
            }

            string uploadRoot = Path.Combine(Root, @"uploads\");
            String appRoot = Path.Combine(Root, @"App_Data\");
            String calibreEbookConvert = Path.Combine(appRoot, @"Calibre\Calibre\ebook-convert.exe");
            String output = Path.Combine(Path.GetDirectoryName(inputFile), Path.GetFileNameWithoutExtension(inputFile) +".converted" + Options.OutputExtension.Value());


            using (Process p = new Process())
            {
                ClientLogging( LogLevel.Sticky|LogLevel.Info, "Converting Ebook, Please Wait...");
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.FileName = calibreEbookConvert;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                p.StartInfo.Arguments = String.Format("{0} {1}", inputFile, output);
                p.Start();
                p.BeginOutputReadLine();
                p.BeginErrorReadLine();
                p.OutputDataReceived += new DataReceivedEventHandler(p_OutputDataReceived);
                p.ErrorDataReceived += new DataReceivedEventHandler(p_ErrorDataReceived);
                p.WaitForExit();
                ClientLogging( LogLevel.Sticky|LogLevel.Info, "File Conversion Completed.");
                ClientLogging( LogLevel.Sticky|LogLevel.Success, String.Format("<a href='{0}'>Click here to download your ebook.</a>", @"/uploads/" + Options.Guid +@"/" + Path.GetFileName(output)  ));

            }



        }

        void p_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            //Handle errors here
            if (!String.IsNullOrEmpty(e.Data))
            {
                HubContext.Clients[Options.Guid].logError(String.Format("[Error] {0}", e.Data));

            }

        }

        void p_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            //output.Add(e.Data);
            HubContext.Clients[Options.Guid].logInfo(String.Format("ebook-convert > {0}", e.Data));
        }




        [Flags]
        private enum LogLevel
        {
            None = 0,
            Sticky = 1,
            Warning = 2,
            Error = 4,
            Success = 8,
            Info = 16
        }

        private void ClientLogging( LogLevel logLevel, String message)
        {
            if (Options.isAsync)
            {
                if (logLevel.HasFlag(LogLevel.Success))
                {
                    HubContext.Clients[Options.Guid].logSuccess(message, logLevel.HasFlag(LogLevel.Sticky));
                }
                else if (logLevel.HasFlag(LogLevel.Warning))
                {
                    HubContext.Clients[Options.Guid].logWarning(message, logLevel.HasFlag(LogLevel.Sticky));
                }
                else if (logLevel.HasFlag(LogLevel.Info))
                {
                    HubContext.Clients[Options.Guid].logSuccess(message, logLevel.HasFlag(LogLevel.Sticky));
                }
                else if (logLevel.HasFlag(LogLevel.Error))
                {
                    HubContext.Clients[Options.Guid].logError(message, logLevel.HasFlag(LogLevel.Sticky));
                }
            }
        }

    }
}