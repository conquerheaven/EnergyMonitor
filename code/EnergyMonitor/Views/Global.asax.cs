using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace EnergyMonitor
{
    public class MvcApplication : System.Web.HttpApplication 
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new EnergyMonitor.Controllers.Utils.Filters.GlobalizationFilter());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //激活邮箱route
            routes.MapRoute(
                "ActiveMail",
                "Homes/ActiveMail/{userID}/{activeStr}",
                new { controller = "Homes", action = "ActiveMail" }
            );
            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{param}", // URL with parameters
                new { controller = "Homes", action = "Index2", param = UrlParameter.Optional } // Parameter defaults
            );
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            //支持国际化
            //SupportMultiLanguage();
        }

        /// <summary>
        /// 支持国际化多语言
        /// </summary>
        private void SupportMultiLanguage()
        {
            try
            {
                string languages = EnergyMonitor.Controllers.Utils.Util.GetConfigValueObj("supportLanguage") as string;
                string[] lang = languages.Split(',');
                Dictionary<string, string> langDic = new Dictionary<string, string>();
                foreach (string language in lang)
                {
                    string[] splitStrs = language.Split(':');
                    string languageCode = splitStrs[0];
                    string languageName = splitStrs[1];
                    Application[languageCode] = System.Globalization.CultureInfo.CreateSpecificCulture(languageCode);
                    langDic[languageCode] = languageName;
                }
                Application["supportLanguage"] = langDic;
            }
            catch (Exception)
            {
            }
        }
    }
}