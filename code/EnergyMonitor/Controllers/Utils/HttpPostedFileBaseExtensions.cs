using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using EnergyMonitor.Models.LinqEntity;

namespace EnergyMonitor.Controllers.Utils
{
    /// <summary>
    /// HttpPostedFileBase类扩展
    /// </summary>
    public static class HttpPostedFileBaseExtensions
    {
        /// <summary>
        /// 检查是否有文件
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static bool HasFile(this HttpPostedFileBase file)
        {
            return (file != null && file.ContentLength > 0) ? true : false;
        }
    }
}
