using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using EnergyMonitor.Controllers.Admin.Filters;
using EnergyMonitor.Models.Repository.Interface;
using EnergyMonitor.Models.Repository.Implement;
using EnergyMonitor.Controllers.Utils;
using System.Collections;
using EnergyMonitor.Models.LinqEntity;

namespace EnergyMonitor.Controllers.Admin
{
    /// <summary>
    /// 异常检测
    /// </summary>
     [AdminFilter]
    public class ExceptionInfoController : Controller
    {
         private IExceptionInfoRepos _exceptionInfoRepos = null;
         public ExceptionInfoController() : this(new ExceptionInfoRepos())
        {
        }
         public ExceptionInfoController(IExceptionInfoRepos exceptionInfoRepos)
        {
             _exceptionInfoRepos = exceptionInfoRepos;           
        }

         /// <summary>
         /// 跳转值突降
         /// </summary>
         /// <returns></returns>
         [OutputCache(Duration = 10, VaryByParam = "none")]
         [AuthenticationFilter]
         public ActionResult ValueDecline( )
         {
             return View( );
         }

         /// <summary>
         /// 跳转值突增
         /// </summary>
         /// <returns></returns>
         [OutputCache(Duration = 10, VaryByParam = "none")]
         [AuthenticationFilter]
         public ActionResult ValueIncrease()
         {
             return View();
         }

         /// <summary>
         /// 跳转机器故障
         /// </summary>
         /// <returns></returns>
         [OutputCache(Duration = 10, VaryByParam = "none")]
         [AuthenticationFilter]
         public ActionResult MachineException()
         {
             return View();
         }

         /// <summary>
         /// 跳转断值
         /// </summary>
         /// <returns></returns>
         [OutputCache(Duration = 10, VaryByParam = "none")]
         [AuthenticationFilter]
         public ActionResult DiscontinuousValue()
         {
             return View();
         }

         /// <summary>
         /// 跳转邮箱设置
         /// </summary>
         /// <returns></returns>
         [OutputCache(Duration = 10, VaryByParam = "none")]      
         public ActionResult SetEmailInfo()
         {
             var emailInfo = _exceptionInfoRepos.GetAllEmail();
             return View(emailInfo);
         }

         /// <summary>
         /// 跳转邮箱设置
         /// </summary>
         /// <returns></returns>
         [OutputCache(Duration = 10, VaryByParam = "none")]
         public ActionResult SetMessageInfo()
         {
             var messageInfo = _exceptionInfoRepos.GetAllMessage();
             return View(messageInfo);
         }

         /// <summary>
        /// 修改机器管理人员邮箱
        /// </summary>
        /// <returns></returns>
         [OutputCache(Duration = 10, VaryByParam = "none")]
         public ActionResult ModifyEmailInfo()
         {
             IDictionary dic = new Dictionary<int, string>();
             int totalEmailNum = Convert.ToInt32(Request.Form["emailNum"]);
             for (int i = 0; i < totalEmailNum; i++)
             {
                 dic.Add(i, Request.Form[i]);
             }
             var flag = _exceptionInfoRepos.ModifyEmail(dic);
             return View(flag);
         }

        /// <summary>
        /// 根据异常类型（机器故障、值突增、值突降）查询指定时间段的电力异常值并分页显示
        /// </summary>
        /// <param name="exceptionType"></param>   
         /// <param name="currentPage"></param>
         /// <param name="totalPages"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>      
        /// <returns></returns>        
        public ActionResult GetExceptionListAjax(string exceptionType, int currentPage, int totalPages, DateTime startTime, DateTime endTime)
        {
            if (Request.IsAjaxRequest())
            {
                Pager pager = null;
                if (totalPages == -1)
                {
                    int totalRows = 0;
                    totalRows = _exceptionInfoRepos.GetExceptionInfo(exceptionType, startTime, endTime).Count();
                    pager = new Pager(1, totalRows);
                }
                else
                {
                    pager = new Pager(currentPage, totalPages, false);
                }
                IList list = null;
                if (pager.TotalPages > 0)
                {
                    list = _exceptionInfoRepos.GetExceptionInfo(exceptionType, startTime, endTime).Skip(pager.StartRow).Take(pager.PageSize).ToList();
                }
                var resultData = new
                {
                    totalPages = pager.TotalPages,
                    data = list
                };
                return Json(resultData, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 根据异常类型（机器故障、值突增、值突降）查询指定时间段的电力异常值并且不分页显示
        /// </summary>
        /// <param name="exceptionType"></param>       
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>      
        /// <returns></returns>     
        public ActionResult GetExceptionListAjaxNoPage(string exceptionType, DateTime startTime, DateTime endTime)
        {
            if (Request.IsAjaxRequest())
            {
                int totalRows = 0;
                totalRows = _exceptionInfoRepos.GetExceptionInfo(exceptionType, startTime, endTime).Count();
                IList list = null;
                list = _exceptionInfoRepos.GetExceptionInfo(exceptionType, startTime, endTime).ToList();
                var resultData = new
                {
                    count = totalRows,
                    data = list
                };
                return Json(resultData, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 查询前一天断值测点信息并且分页显示
        /// </summary>
        /// <param name="exceptionType"></param> 
        /// <param name="currentPage"></param>
        /// <param name="totalPages"></param>           
        /// <returns></returns>    
        public ActionResult GetDiscontinuousValueListAjax( int currentPage, int totalPages)
        {
            if (Request.IsAjaxRequest())
            {
                Pager pager = null;
                if (totalPages == -1)
                {
                    int totalRows = 0;
                    totalRows = _exceptionInfoRepos.GetDiscontinuousValueInfo( ).Count();
                    pager = new Pager(1, totalRows);
                }
                else
                {
                    pager = new Pager(currentPage, totalPages, false);
                }
                IList list = null;
                if (pager.TotalPages > 0)
                {
                    list = _exceptionInfoRepos.GetDiscontinuousValueInfo( ).Skip(pager.StartRow).Take(pager.PageSize).ToList();
                }
                var resultData = new
                {
                    totalPages = pager.TotalPages,
                    data = list
                };
                return Json(resultData, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 查询前一天断值测点信息并且不分页显示
        /// </summary>           
        /// <returns></returns>
        public ActionResult GetDiscontinuousValueListAjaxNoPage( )
        {
            if (Request.IsAjaxRequest())
            {
                int totalRows = 0;
                totalRows = _exceptionInfoRepos.GetDiscontinuousValueInfo( ).Count();
                IList list = null;
                list = _exceptionInfoRepos.GetDiscontinuousValueInfo( ).ToList();
                var resultData = new
                {
                    count = totalRows,
                    data = list
                };
                return Json(resultData, JsonRequestBehavior.AllowGet);
            }
            return null;
        } 

        /// <summary>
        /// 添加邮箱
        /// </summary>
        /// <param name="emailID"></param>
        /// <param name="emailName"></param>
        /// <returns></returns>
       
        public ActionResult AddNewEmailAjax(string emailID, string emailName)
        {
            if (Request.IsAjaxRequest())
            {
                SetEmail se = new SetEmail();                
                se.SE_Email = emailName;
                bool flag = _exceptionInfoRepos.AddEmail(se);
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 添加联系人
        /// </summary>
        /// <param name="emailID"></param>
        /// <param name="emailName"></param>
        /// <returns></returns>

        public ActionResult AddNewMessageAjax(string messageID, string messageName)
        {
            if (Request.IsAjaxRequest())
            {
                setMessages se = new setMessages();
                se.SE_Value = messageName;
                bool flag = _exceptionInfoRepos.AddMessage(se);
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 修改邮箱名称
        /// </summary>
        /// <param name="formerName"></param>
        /// <param name="emailName"></param>
        /// <returns></returns>

        public ActionResult ModifyEmailNameAjax(string formerName, string emailName)
        {
            if (Request.IsAjaxRequest())
            {                
                bool flag = _exceptionInfoRepos.ModifyEmail(formerName,emailName);
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 修改联系人
        /// </summary>
        /// <param name="formerName"></param>
        /// <param name="emailName"></param>
        /// <returns></returns>

        public ActionResult ModifyMessageNameAjax(string formerName, string messageName)
        {
            if (Request.IsAjaxRequest())
            {
                bool flag = _exceptionInfoRepos.ModifyMessage(formerName, messageName);
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 删除邮箱
        /// </summary>
        /// <param name="emailName"></param>
        /// <returns></returns>
        
        public ActionResult RemoveEmailAjax(string emailName)
        {
            if (Request.IsAjaxRequest())
            {
                bool flag = _exceptionInfoRepos.DeleteEmail(emailName);
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 删除联系人
        /// </summary>
        /// <param name="emailName"></param>
        /// <returns></returns>

        public ActionResult RemoveMessageAjax(string messageName)
        {
            if (Request.IsAjaxRequest())
            {
                bool flag = _exceptionInfoRepos.DeleteMessage(messageName);
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            return null;
        }
    }
}
