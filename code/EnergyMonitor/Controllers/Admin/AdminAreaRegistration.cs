using System.Web.Mvc;

namespace EnergyMonitor.Controllers.Admin
{
    public class AdminAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            //激活邮箱route
            context.MapRoute(
                "Admin_activeMail",
                "Admin/User/ActiveMail/{userID}/{activeStr}",
                new { controller = "User", action = "ActiveMail" }
            );
            //默认route
            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{param}",
                new { controller = "Home", action = "Index", param = UrlParameter.Optional }
            );
        }
    }
}
