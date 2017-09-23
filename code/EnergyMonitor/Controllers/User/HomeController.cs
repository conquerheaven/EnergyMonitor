using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EnergyMonitor.Controllers.User.Filters;
using System.Collections;
using EnergyMonitor.Controllers.Utils;
using EnergyMonitor.Models.Repository.Interface;
using EnergyMonitor.Models.Repository.Implement;


namespace EnergyMonitor.Controllers.User
{
    /// <summary>
    /// 用户信息业务逻辑层控制类
    /// </summary>
    /// <author>WangWei</author>
    /// <date>2010-11-26</date>
    [UserFilter]
    public class HomeController : Controller
    {
        public HomeController()
        {
        }

        /// <summary>
        /// 学生用户首页
        /// </summary>
        /// <returns></returns>
        public ActionResult Welcome()
        {
            return View();
        }

        public ActionResult Map()
        {
            return View();
        }
    }
}
