using System.Web;

namespace CarriesFrugalLiving.utils
{
    public static class HtmlEncodeDecode
    {
        public static string Encode(string sVulnerableString) {
            return HttpContext.Current.Server.HtmlEncode(sVulnerableString).ToString(); 
        }

        public static string Decode(string sVulnerableString, bool noScript = true)
        {
            string rc = "";
            rc = HttpContext.Current.Server.HtmlDecode(sVulnerableString).ToString();
            if (noScript) {
               // HttpContext.Current.Server.
            }
            return rc; 

        }

    }
   

}
