using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace EnergyMonitor.Controllers.Utils.Filters
{
    public class GlobalizationFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //国际化
            if (filterContext.HttpContext.Session["selectedLanguage"] != null)
            {
                string language = filterContext.HttpContext.Session["selectedLanguage"] as string;
                System.Threading.Thread.CurrentThread.CurrentCulture = filterContext.HttpContext.Application[language] as System.Globalization.CultureInfo;
                System.Threading.Thread.CurrentThread.CurrentUICulture = filterContext.HttpContext.Application[language] as System.Globalization.CultureInfo;
            }
        }
    }
}
