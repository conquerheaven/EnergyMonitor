using System;
using System.Collections.Generic;
using System.Web.Mvc;
using EnergyMonitor.Controllers.User.Filters;

namespace EnergyMonitor.Controllers.User
{
    /// <summary>
    /// 用户公用控制类
    /// </summary>
    public class SharedController : Controller
    {

        /// <summary>
        /// 没有登录
        /// </summary>
        /// <returns></returns>
        public ActionResult NoLogin()
        {
            return View();
        }

        /// <summary>
        /// 错误页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Error()
        {
            return View();
        }

        /// <summary>
        /// 选择语言
        /// </summary>
        /// <param name="language">语言名称</param>
        /// <param name="previousUrl">选择前页面地址</param>
        /// <returns>根据选择的语言显示选择前页面</returns>
        public ActionResult ChangeLanguage(string language, string previousUrl)
        {
            string[] subUrls = previousUrl.Split('/');
            Session["selectedLanguage"] = language;
            if (subUrls.Length > 1)
            {
                string controllerName = subUrls[subUrls.Length - 2];
                string actionName = subUrls[subUrls.Length - 1];
                return RedirectToAction(actionName, controllerName, new {r = DateTime.Now.Ticks });
            }
            return RedirectToAction("Error");
        }

    }
}
