//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.IO;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Threading.Tasks;
//using System.Web;
//using System.Web.Http;

//namespace Rebind.Controllers
//{
//    public class ConvertController : ApiController
//    {

//        public class CustomMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
//        {
//            public CustomMultipartFormDataStreamProvider(string path)
//                : base(path)
//            { }

//            public override string GetLocalFileName(System.Net.Http.Headers.HttpContentHeaders headers)
//            {
//                var name = !string.IsNullOrWhiteSpace(headers.ContentDisposition.FileName) ? headers.ContentDisposition.FileName : "NoName";
//                return name.Replace("\"", string.Empty); //this is here because Chrome submits files in quotation marks which get treated as part of the filename and get escaped
//            }
//        }

//        public async Task<String> Post()
//        {

//            // Check if the request contains multipart/form-data.
//            if (!Request.Content.IsMimeMultipartContent())
//            {
//                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
//            }

//            string root = HttpContext.Current.Server.MapPath("~/App_Data/");
//            var provider = new CustomMultipartFormDataStreamProvider(root + "uploads/");

//            try
//            {
//                // Read the form data.
//                var bodyparts = Request.Content.ReadAsMultipartAsync(provider).ContinueWith(o =>
//                    {
//                        var parts = o.Result;

//                        HttpContent namePart = parts.FormData["createdByName"];
//                        if (namePart == null)
//                        {
//                            throw new HttpResponseException(HttpStatusCode.BadRequest);
//                        }
//                        string name = namePart.ReadAsStringAsync().Result;
//                        //string file1 = provider.BodyPartFileNames.First().Value;
//                        // this is the file name on the server where the file was saved 

//                        return new HttpResponseMessage()
//                            {
//                                Content = new StringContent("File uploaded.")
//                            };
//                    }); 
                

//                //copy the file 
//                return "";
//            }
//            catch (System.Exception e)
//            {
//                Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
//                return "Error + "+ e.Message;
//            }



//            /*
//             * 

//                String calibreEbookConvert = Path.Combine(root, "Calibre/Calibre/ebook-convert.exe");

//                //
//                // Setup the process with the ProcessStartInfo class.
//                //
//                ProcessStartInfo start = new ProcessStartInfo(calibreEbookConvert, @"C:\tmp\dragon.epub C:\tmp\dragon.mobi");
//                start.UseShellExecute = false;
//                start.RedirectStandardOutput = true;
                
                

//                // Start the process.
//                String output;
//                using (Process process = Process.Start(start))
//                {
//                    //
//                    // Read in all the text from the process with the StreamReader.
//                    //
//                    using (StreamReader reader = process.StandardOutput)
//                    {
//                        string result = reader.ReadToEnd();
//                        output = result;
//                    }
//                }


                

//                //// This illustrates how to get the file names.
//                ////foreach (MultipartFileData file in provider.FileData)
//                ////{
//                ////    Trace.WriteLine(file.Headers.ContentDisposition.FileName);
//                ////    Trace.WriteLine("Server file path: " + file.LocalFileName);
//                ////}
//                return output;
//             * */


//        }

//    }
//}