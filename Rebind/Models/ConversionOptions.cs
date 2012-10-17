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
        public OutputExtension OutputExtension { get; set; }
        public OutputProfile OutputProfile { get; set; }

        public Boolean isAsync { get; set; }

        public ConversionOptions()
        {
            isAsync = true;
            OutputProfile = OutputProfile.Default;
            OutputExtension = OutputExtension.Epub;
            Guid = System.Guid.NewGuid().ToString("N");
        }

    }
}