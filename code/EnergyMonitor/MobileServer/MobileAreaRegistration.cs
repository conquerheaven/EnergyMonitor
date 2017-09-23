using System.Web.Mvc;

namespace EnergyMonitor.Controllers.Mobile
{
    public class MobileAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Mobile";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            //默认route
            context.MapRoute(
                "Mobile_default",
                "Mobile/{controller}/{action}/{param}",
                new { controller = "Home", action = "Index", param = UrlParameter.Optional }
            );
            
        }
    }
}
