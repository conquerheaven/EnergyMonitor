using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace EnergyMonitor.Controllers.Admin.Filters
{
    class AdminFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            EnergyMonitor.Models.LinqEntity.User u = filterContext.HttpContext.Session["loginUser"] as EnergyMonitor.Models.LinqEntity.User;     
            string actionName = filterContext.RouteData.Values["action"].ToString();
            //验证是否管理员用户登录
            if ((u == null || u.USR_RoleID == 1) && (!"GetSpecifiedBuildingEnergy".Equals(actionName)) && (!"GetElecAjaxForMobile".Equals(actionName)) && (!"GetHElecAjaxForMobile".Equals(actionName)) && (!"GetQueryElecAjaxForMobile".Equals(actionName)) && (!"GetHGElecListAjaxForMobile".Equals(actionName)) && (!"GetElecAllAjaxForMobile".Equals(actionName)))
            {
                if (!filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Homes", action = "NoLogin", Area = "" }));
                }
                else
                {
                    filterContext.Result = new EmptyResult();
                }
            }
        }
    }
}
