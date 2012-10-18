using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rebind
{
    public static class Constants
    {
    }

    public sealed class OutputExtension
    {
        //cybookg3, cybook_opus, default, generic_eink, generic_eink_large, hanlinv3, hanlinv5, illiad, ipad, ipad3, irexdr1000, irexdr800, jetbook5, kindle, kindle_dx, kindle_fire, kobo, msreader, mobipocket, nook, nook_color, pocketbook_900, galaxy, bambook, sony, sony300, sony900, sony-landscape, tablet
        private readonly String name;
        private readonly String value;

        private static readonly Dictionary<string, OutputExtension> instance = new Dictionary<string, OutputExtension>();
        public static readonly OutputExtension Epub = new OutputExtension("ePub", ".epub");
        public static readonly OutputExtension Html = new OutputExtension("Html", ".html");
        public static readonly OutputExtension Lit = new OutputExtension("Lit", ".lit");
        public static readonly OutputExtension Mobi = new OutputExtension("Mobi", ".mobi");
        public static readonly OutputExtension Pdb = new OutputExtension("Pdb", ".pdb");
        public static readonly OutputExtension Pdf = new OutputExtension("Pdf", ".pdf");
        public static readonly OutputExtension Rtf = new OutputExtension("Rtf", ".rtf");
        public static readonly OutputExtension Txt = new OutputExtension("Txt", ".txt");
        

        private OutputExtension(String name, String value)
        {
            this.name = name;
            this.value = value;
            instance[value] = this;
        }

        public override String ToString()
        {
            return name;
        }
        public String Name() { return name; }
        public String Value()
        {
            return value;
        }
        
        public static explicit operator OutputExtension(string str)
        {
            OutputExtension result;
            if (instance.TryGetValue(str, out result))
                return result;
            else
                throw new InvalidCastException();
        }
    }
    public sealed class OutputProfile
    {
        //cybookg3, cybook_opus, default, generic_eink, generic_eink_large, hanlinv3, hanlinv5, illiad, ipad, ipad3, irexdr1000, irexdr800, jetbook5, kindle, kindle_dx, kindle_fire, kobo, msreader, mobipocket, nook, nook_color, pocketbook_900, galaxy, bambook, sony, sony300, sony900, sony-landscape, tablet
        private readonly String name;
        private readonly String value;

        public static readonly OutputProfile Default = new OutputProfile("Default", "default");
        public static readonly OutputProfile GenericEInk = new OutputProfile("Generic EInk", "generic_eink");
        public static readonly OutputProfile IPad = new OutputProfile("IPad", "ipad");
        public static readonly OutputProfile IPad3 = new OutputProfile("IPad 3", "ipad3");
        public static readonly OutputProfile Kindle = new OutputProfile("Kindle", "kindle");
        public static readonly OutputProfile KindleDx = new OutputProfile("Kindle Dx", "kindle_dx");
        public static readonly OutputProfile KindleFire = new OutputProfile("Kindle Fire", "kindle_fire");
        public static readonly OutputProfile Kobo = new OutputProfile("Kobo", "kobo");
        public static readonly OutputProfile MSReader = new OutputProfile("MS Reader", "msreader");
        public static readonly OutputProfile Nook = new OutputProfile("Nook", "nook");
        public static readonly OutputProfile NookColor = new OutputProfile("Nook Color", "nook_color");
        public static readonly OutputProfile Tablet = new OutputProfile("Tablet", "tablet");


        private OutputProfile(String name, String value)
        {
            this.name = name;
            this.value = value;
        }

        public override String ToString()
        {
            return name;
        }

        public String Name() { return name;}
        public String Value()
        {
            return value;
        }

    }
}