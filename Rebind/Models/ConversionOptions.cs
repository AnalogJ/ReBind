using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rebind.Models
{
    public class ConversionOptions
    {

        public String Guid { get; set; }
        public Uri EbookUrl { get; set; }
        public HttpPostedFileBase EbookFile { get; set; }
        public OutputExtension OutputExtension
        {
            get { return (OutputExtension) Extension; }
        }
        public OutputProfile OutputProfile { get; set; }

        public String Extension { get; set; }

        public Boolean isAsync { get; set; }
        public Boolean isStarted { get; set; }

        public ConversionOptions()
        {
            isAsync = true;
            isStarted = false;
            OutputProfile = OutputProfile.Default;
            Extension = OutputExtension.Epub.Value();
            Guid = System.Guid.NewGuid().ToString("N");
        }

    }
}