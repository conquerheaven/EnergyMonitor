using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EnergyMonitor.Controllers.Admin.Filters;
using System.Collections;
using EnergyMonitor.Controllers.Utils;
using EnergyMonitor.Models.Repository.Interface;
using EnergyMonitor.Models.Repository.Implement;
using EnergyMonitor.Models.LinqEntity;


namespace EnergyMonitor.Controllers.Admin
{
    /// <summary>
    /// 能耗公示
    /// </summary>
    [AdminFilter]
    public class PublicityController : Controller
    {
        private IPowerClassRepos _powerClassRepos = null;
        private IAnalogHistoryRepos _analogHistoryRepos = null;
        private IBuildingRepos _buildingRepos = null;
        private IAnnouncementInfoRepos _announcementInfoRepos = null;

        public PublicityController()
            : this(new PowerClassRepos(), new AnalogHistoryRepos(),new BuildingRepos(),new AnnouncementInfoRepos())
        {
        }

        public PublicityController(IPowerClassRepos powerClassRepos, IAnalogHistoryRepos analogHistoryRepos,IBuildingRepos buildingRepos, IAnnouncementInfoRepos announcementInfoRepos)
        {
            _powerClassRepos = powerClassRepos;
            _analogHistoryRepos = analogHistoryRepos;
            _buildingRepos = buildingRepos;
            _announcementInfoRepos = announcementInfoRepos;
        }

        /// <summary>
        /// 用电公示
        /// </summary>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult Elec()
        {
            var powerList = _powerClassRepos.GetStatisTypes();
            return View(powerList);
        }

        /// <summary>
        /// 获取公示数据
        /// </summary>
        /// <param name="dateType"></param>
        /// <param name="buildingId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="powerType"></param>
        /// <returns></returns>
        public ActionResult GetQueryElecAjax(int dateType, int? buildingId, DateTime? startTime, DateTime? endTime, string powerType)
        {
            if (Request.IsAjaxRequest() && buildingId.HasValue && buildingId.Value > 0)
            {
                IList list = new ArrayList() ;
                string[] powerTypes = null;
                if (!string.IsNullOrWhiteSpace(powerType))
                {
                    powerTypes = powerType.Split(new char[] { '_' });
                }
                string[] powerTypesEach = new string[1];
                double sum = -1;
                var days = 0;
                BuildingDetailInfo bdi = _buildingRepos.GetBuilding(buildingId.Value);
                if (dateType == 1)
                {
                    DateTime endDate =  DateTime.Today;
                    DateTime startDate = DateTime.Parse(endDate.ToString("yyyy-MM-01"));
                    days = (endDate - startDate).Days;
                    sum = _analogHistoryRepos.GetEnergy(3, buildingId.Value, powerTypes, startDate, endDate);
                    for(int i = 0; i < powerTypes.Length;i++)
                    {
                        if (powerTypes[i] != "001" && powerTypes[i] != "002" && powerTypes[i] != "003" && powerTypes[i] != "001007" && powerTypes[i] != "001006" && powerTypes[i] != "002005")
                        {
                            powerTypesEach[0] = powerTypes[i];
                            double powerTypeSum = _analogHistoryRepos.GetEnergy(3, buildingId.Value, powerTypesEach, startDate, endDate);
                            list.Add(new
                            {
                                powerTypeName = _powerClassRepos.GetPowerTypeName(powerTypes[i]).PC_Name,
                                powerTypeSum = powerTypeSum.ToString("f1"),
                                average = bdi.BDI_Area.HasValue ? (powerTypeSum / bdi.BDI_Area.Value).ToString("f1") : null
                            });
                        }
                    }
                }
                else
                {
                    DateTime startDate = startTime.Value;
                    DateTime endDate = endTime.Value;
                    days = (endDate - startDate).Days;
                    sum = _analogHistoryRepos.GetEnergy(3, buildingId.Value, powerTypes, startDate, endDate);
                    for(int i = 0; i < powerTypes.Length;i++)
                    {
                        if (powerTypes[i] != "001" && powerTypes[i] != "002" && powerTypes[i] != "003" && powerTypes[i] != "001007" && powerTypes[i] != "001006" && powerTypes[i] != "002005")
                        {
                            powerTypesEach[0] = powerTypes[i];
                            double powerTypeSum = _analogHistoryRepos.GetEnergy(3, buildingId.Value, powerTypesEach, startDate, endDate);
                            list.Add(new
                            {
                                powerTypeName = _powerClassRepos.GetPowerTypeName(powerTypes[i]).PC_Name,
                                powerTypeSum = powerTypeSum.ToString("f1"),
                                average = bdi.BDI_Area.HasValue ? (powerTypeSum / bdi.BDI_Area.Value).ToString("f1") : null
                            });
                        }
                    }
                }
                days++;               
                string average = null;
                if (bdi.BDI_Area != null)
                {
                    average = bdi.BDI_Area.HasValue ? (sum / bdi.BDI_Area.Value).ToString("f1"):null;
                }                
                var data = new 
                {
                    sum = sum.ToString("f1"),
                    days = days,
                    average = average,
                    list = list
                };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        #region 信息公示页面
        /// <summary>
        /// 信息公示
        /// </summary>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult QueryNews()
        {
            var loginUserName = Session["loginUserName"];
            ViewBag.loginUserName = loginUserName;
            return View();
        }

        /// <summary>
        /// 添加公示
        /// </summary>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult AddNews()
        {
            var loginUserName = Session["loginUserName"];
            ViewBag.loginUserName = loginUserName;
            return View();
        }

        /// <summary>
        /// 修改公示
        /// </summary>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult ModifyNews(int ID)
        {
            ViewBag.ID = ID;
            return View();
        }

        #endregion

        #region ajax接口

        /// <summary>
        /// 使用Ajax获取公示信息，分页
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="totalPages"></param>
        /// <returns></returns>
        public ActionResult GetAnnouncementInfoAjax(int currentPage, int totalPages)
        {
            if (Request.IsAjaxRequest())
            {
                var query = _announcementInfoRepos.GetAllAnnouncementInfoQueryable();
                Pager pager = null;
                if (totalPages == -1)
                {
                    int totalRows = query.Count();
                    pager = new Pager(1, totalRows);
                }
                else
                {
                    pager = new Pager(currentPage, totalPages, false);
                }
                IList list = query.Skip(pager.StartRow).Take(pager.PageSize).ToList();
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
        /// 使用Ajax添加公示信息
        /// </summary>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="createTime"></param>
        /// <param name="deadLine"></param>
        /// <param name="author"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        [ValidateInput(false)] 
        public ActionResult AddAnnouncementInfoAjax(string title , string content , DateTime createTime , DateTime deadLine , string author , int? remark)
        {
            if (Request.IsAjaxRequest())
            {
                AnnouncementInfo ami = new AnnouncementInfo();
                ami.Title = title;
                ami.Content = content;
                ami.CreateTime = createTime;
                ami.DeadLine = deadLine;
                ami.Author = author;
                if (remark != null) ami.Remark = remark;
                IList<AnnouncementInfo> amiList = new List<AnnouncementInfo>();
                amiList.Add(ami);
                bool ifSuccess = _announcementInfoRepos.AddAnnouncementInfo(amiList);
                return Json(new {ifSuccess=ifSuccess }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 使用Ajax修改公示信息
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="deadLine"></param>
        /// <param name="createTime"></param>
        /// <param name="author"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public ActionResult ModifyAnnouncementInfoAjax(int ID , string title, string content, DateTime deadLine, int? remark)
        {
            if (Request.IsAjaxRequest())
            {
                AnnouncementInfo ami = new AnnouncementInfo();
                ami.ID = ID;
                ami.Title = title;
                ami.Content = content;
                ami.DeadLine = deadLine;
                if (remark != null) ami.Remark = remark;
                bool ifSuccess = _announcementInfoRepos.ModifyAnnouncementInfo(ami);
                return Json(new { ifSuccess = ifSuccess }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 使用Ajax根据ID删除公示信息
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ActionResult DeleteAnnouncementInfoByIDAjax(int ID)
        {
            if (Request.IsAjaxRequest())
            {
                bool ifSuccess = _announcementInfoRepos.DeleteAnnouncementInfoByID(ID);
                return Json(new { ifSuccess = ifSuccess }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 使用Ajax根据ID查询公示信息
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ActionResult GetAnnouncementInfoByIDAjax(int ID)
        {
            if (Request.IsAjaxRequest())
            {
                IList resultList = new ArrayList();
                AnnouncementInfo ai = _announcementInfoRepos.GetAnnouncementInfoByID(ID);
                if (ai != null)
                {
                    resultList.Add(new
                    {
                        ID = ai.ID,
                        Title = ai.Title,
                        Content = ai.Content,
                        CreateTime = ai.CreateTime.ToShortDateString(),
                        DeadLine = ai.DeadLine.ToShortDateString(),
                        Author = ai.Author,
                        Remark = ai.Remark
                    });
                }
                return Json(new { resultList = resultList }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }
        #endregion

        #region 工具函数
        #endregion
    }
}
