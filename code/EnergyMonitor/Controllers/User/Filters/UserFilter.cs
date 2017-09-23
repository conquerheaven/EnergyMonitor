using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace EnergyMonitor.Controllers.User.Filters
{
    class UserFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            EnergyMonitor.Models.LinqEntity.User u = filterContext.HttpContext.Session["loginUser"] as EnergyMonitor.Models.LinqEntity.User;
            string actionName = filterContext.RouteData.Values["action"].ToString();
            //验证用户是否登录
            if (filterContext.HttpContext.Session["loginUser"] == null && (!"QueryBuyHistoryAjax".Equals(actionName)) && (!"getDepartmentList".Equals(actionName)) && (!"Reactive".Equals(actionName)) && (!"EnergyBriefVal".Equals(actionName)) && (!"GetRoomEnergyAjax".Equals(actionName)) && (!"GetDetailRoomEnergyAjax".Equals(actionName)) && (!"GetAllShoolAjax".Equals(actionName)) && (!"GetAreasBySchoolIDAjax".Equals(actionName)) && (!"GetBuildingByAreaAjax".Equals(actionName)) && (!"GetRoomsByBIDAjax".Equals(actionName)) && (!"Registered".Equals(actionName)) && (!"GetAreasBySchoolIDAjax".Equals(actionName)) && (!"GetPointsByBuildingAjax".Equals(actionName)) && (!"GetBuildingByAreaAjax".Equals(actionName)) && (!"Register".Equals(actionName)) && (!"EnergyBriefValForMobile".Equals(actionName)) && (!"GetAllDepartment".Equals(actionName)) && (!"RegisteredForMobile".Equals(actionName)) && (!"QueryBuyHistoryAjaxForMobile".Equals(actionName)))
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Homes", action = "NoLogin", Area = "" }));
                return;
            }
        }
    }
}
