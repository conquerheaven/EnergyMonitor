using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using EnergyMonitor.Models.LinqEntity;
using EnergyMonitor.Models.Repository.Implement;

namespace EnergyMonitor.Controllers.Admin.Filters
{
    /// <summary>
    /// 权限过滤器
    /// </summary>
    class AuthenticationFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string controllerName = filterContext.RouteData.Values["controller"].ToString();
            string actionName = filterContext.RouteData.Values["action"].ToString();
            string controllerActionStr = controllerName + "/" + actionName;
            IList<string> functionLinkList = filterContext.HttpContext.Session["userFunctionLinks"] as IList<string>;
            if (functionLinkList == null || !functionLinkList.Contains(controllerActionStr)) // 没有权限
            {
                if (!filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Shared", action = "NoRight" }));
                }
                else
                {
                    filterContext.Result = new EmptyResult();
                }
            }
            else
            {
                // 记录日志
                var fullSubFunctionList = filterContext.HttpContext.Session["fullSubFunctionList"] as IList<Function>;
                var function = fullSubFunctionList.Where(x => x.FN_LinkLocation == controllerActionStr).FirstOrDefault();
                string funcionId = "";
                string functionDesc = "";
                if (function != null)
                {
                    funcionId = function.FN_ID;
                    functionDesc = function.FN_Description;
                }
                var user = filterContext.HttpContext.Session["loginUser"] as EnergyMonitor.Models.LinqEntity.User;
                var request = filterContext.RequestContext.HttpContext.Request;
                string fullRequestUrl = request.Url.PathAndQuery;
                string hostAddress = request.UserHostAddress;
                string hostName = request.UserHostName;
                string userAgent = request.UserAgent;
                string localLoginIP;
                if (request.ServerVariables["HTTP_VIA"] != null)
                {
                    localLoginIP = request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
                }
                else
                {
                    localLoginIP = request.ServerVariables["REMOTE_ADDR"].ToString();
                }
                string desc = String.Format("用户【{0}】操作【{1}】，当前访问本地IP为【{2}】", user.USR_Name, functionDesc, localLoginIP);
                if (hostAddress != localLoginIP)
                {
                    desc += String.Format("，远程IP为【{0}】", hostAddress);
                }
                if (hostName != hostAddress)
                {
                    desc += String.Format("，远程主机名为【{0}】", hostName);
                    
                }
                desc += String.Format("，其他信息【{0}】", userAgent);

                Log userLog = new Log();
                userLog.LOG_UserID = user.USR_ID;
                userLog.LOG_FuctionID = funcionId;
                userLog.LOG_actionType = 0;
                userLog.LOG_actionDate = DateTime.Now;
                userLog.LOG_actionDesc = desc;
                var logRepos = LogRepos.LogInstance();
                logRepos.AddLog(userLog);
            }
        }
    }
}
