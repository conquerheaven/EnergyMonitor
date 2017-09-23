using System;
using System.Web.Mvc;
using System.Data.Linq;
using System.Collections;
using System.Web.UI.WebControls;
using System.Linq;

namespace EnergyMonitor.Controllers.Utils
{
    /// <summary>
    /// 得到资源文件
    /// </summary>
    public static class ResourcesControllerExtensions
    {
        public static string GetResourceStr(this Controller controller, string resFile, string resourceKey)
        {
            try
            {
                return controller.HttpContext.GetGlobalResourceObject(resFile, resourceKey) as string;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
