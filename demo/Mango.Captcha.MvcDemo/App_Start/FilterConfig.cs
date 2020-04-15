using System.Web;
using System.Web.Mvc;

namespace Mango.Captcha.MvcDemo
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
