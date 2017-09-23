using System.Web.Mvc;

namespace EnergyMonitor.Controllers.User
{
    public class UserAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "User";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            //激活邮箱route
            context.MapRoute(
                "User_activeMail",
                "User/User/ActiveMail/{userID}/{activeStr}",
                new { controller = "User", action = "ActiveMail" }
            );
            //默认route
            context.MapRoute(
                "User_default",
                "User/{controller}/{action}/{param}",
                new { controller = "Home", action = "Index", param = UrlParameter.Optional }
            );
        }
    }
}
