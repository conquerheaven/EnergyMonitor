using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Collections;
using EnergyMonitor.Controllers.Utils;
using EnergyMonitor.Controllers.Admin.Filters;
using EnergyMonitor.Models.Repository.Interface;
using EnergyMonitor.Models.Repository.Implement;

namespace EnergyMonitor.Controllers.Admin
{
    /// <summary>
    /// 工具
    /// </summary>
    [AdminFilter]
    public class UtilController : Controller
    {
        private IPowerClassRepos _powerClassRepos = null;

        public UtilController()
            : this(new PowerClassRepos())
        {
        }

        public UtilController(IPowerClassRepos powerClassRepos)
        {
            _powerClassRepos = powerClassRepos;
        }

        
        public ActionResult Elec()
        {
            var powerList = _powerClassRepos.GetElec();
            return View(powerList);
        }

    }
}
