using System.Web;
using System.Web.Mvc;

namespace CarriesFrugalLiving
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
