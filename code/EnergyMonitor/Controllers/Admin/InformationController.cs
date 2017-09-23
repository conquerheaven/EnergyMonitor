using System;
using System.Collections.Generic;
using System.Web.Mvc;
using EnergyMonitor.Controllers.Admin.Filters;
using EnergyMonitor.Models.Repository.Interface;
using EnergyMonitor.Models.Repository.Implement;
using EnergyMonitor.Controllers.Utils;
using System.Collections;
using System.Linq;
using EnergyMonitor.Models.Repository.Entity;
using EnergyMonitor.Models.LinqEntity;
using System.Web;
using System.IO;
using System.Data.OleDb;
using System.Data;
using System.ComponentModel;



namespace EnergyMonitor.Controllers.Admin
{
    /// <summary>
    /// 信息维护类
    /// </summary>
    [AdminFilter]
    public class InformationController : Controller
    {
        private IUserRepos _userRepos = null;
        private ISchoolRepos _schoolRepos = null;
        private ISchoolAreaRepos _schoolAreaRepos = null;
        private IBuildingRepos _buildingRepos = null;
        private IRoomRepos _roomRepos = null;
        private IAMPRepos _ampRepos = null;
        private IPowerClassRepos _powerClassRepos = null;
        private IDepartmentRepos _departmentRepos = null;
        private IAnalogHistoryRepos _analogHistoryRepos = null;
        private IAnalogInfoRepos _analogInfoRepos = null;
        private IRTURepos _rtuRepos = null;
        private IBECRepos _becRepos = null;
        private IBuildingTypesRepos _buildingTypesRepos = null;
        private IAnalogMeasurePoint _analogMeasurePointRepos = null;
        private ITransactionRepos _transactionRepos = null;
        private IUpdateInfoRepos _updateInfoRepos = null;

        public InformationController()
            : this(new UserRepos(), new SchoolRepos(), new SchoolAreaRepos(),
            new BuildingRepos(), new RoomRepos(), new AMPRepos(),
            new PowerClassRepos(), new DepartmentRepos(), new AnalogHistoryRepos(),
            new AnalogInfoRepos(), new RTURepos(), new BECRepos(), new BuildingTypesRepos() , 
            new AnalogMeasurePointRepos() , new TransactionRepos() , new UpdateInfoRepos())
        {
        }

        public InformationController(IUserRepos userRepos, ISchoolRepos schoolRepos, ISchoolAreaRepos schoolAreaRepos,
            IBuildingRepos buildingRepos, IRoomRepos roomRepos, IAMPRepos ampRepos,
            IPowerClassRepos powerClassRepos, IDepartmentRepos departmentRepos, IAnalogHistoryRepos analogHistoryRepos,
            IAnalogInfoRepos analogInfoRepos, IRTURepos rtuRepos, IBECRepos becRepos, IBuildingTypesRepos buildingTypesRepos , 
            IAnalogMeasurePoint analogMeasurePointRepos , ITransactionRepos transactionRepos , IUpdateInfoRepos updateInfoRepos)
        {
            _userRepos = userRepos;
            _schoolRepos = schoolRepos;
            _schoolAreaRepos = schoolAreaRepos;
            _buildingRepos = buildingRepos;
            _roomRepos = roomRepos;
            _ampRepos = ampRepos;
            _powerClassRepos = powerClassRepos;
            _departmentRepos = departmentRepos;
            _analogHistoryRepos = analogHistoryRepos;
            _analogInfoRepos = analogInfoRepos;
            _rtuRepos = rtuRepos;
            _becRepos = becRepos;
            _buildingTypesRepos = buildingTypesRepos;
            _analogMeasurePointRepos = analogMeasurePointRepos;
            _transactionRepos = transactionRepos;
            _updateInfoRepos = updateInfoRepos;
        }

        /// <summary>
        /// 跳转校区管理
        /// </summary>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult QuerySchool()
        {
            return View();
        }

        /// <summary>
        /// 跳转三级电表管理页面
        /// </summary>
        /// <returns></returns>
        public ActionResult AddThirdPointVal()
        {
            return View();
        }

        /// <summary>
        /// 查询三级电表测点(已经录入数据的测点)
        /// </summary>
        /// <returns></returns>
        public ActionResult QueryThirdPointHasValueAjax(int? analogNo, String ampName, DateTime month)
        {
            var list = _ampRepos.QueryThirdAMPHasValue(analogNo, ampName, month).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 查询三级电表测点（未录入数据的测点）
        /// </summary>
        /// <param name="analogNo"></param>
        /// <param name="ampName"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public ActionResult QueryThirdPointNeedValueAjax(int? analogNo, String ampName, DateTime month)
        {
            var list = _ampRepos.QueryThirdPointNeedValue(analogNo, ampName, month);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 添加三级电表月份用电量数据
        /// </summary>
        /// <param name="value"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public ActionResult AddThirdPointValue(String value, DateTime month)
        {
            if (Request.IsAjaxRequest() && value.Length != 0)
            {
                if (_analogHistoryRepos.AddThirdPointHistory(value, month))
                {
                    return Json(new { ifSucceed = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { ifSucceed = false }, JsonRequestBehavior.AllowGet);
                }
            }
            return null;
        }

        /// <summary>
        /// 修改三级电表月份用电量数据
        /// </summary>
        /// <param name="value"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public ActionResult ModifyThirdPointValue(String value, DateTime month)
        {
            if (Request.IsAjaxRequest() && value.Length != 0)
            {
                if (_analogHistoryRepos.ModifyThirdPointHistory(value, month))
                {
                    return Json(new { ifSucceed = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { ifSucceed = false }, JsonRequestBehavior.AllowGet);
                }
            }
            return null;
        }

        /// <summary>
        /// 删除三级电表某月用电量数据。
        /// </summary>
        /// <param name="pointID"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public ActionResult DeleteThirdPointValue(int pointID, DateTime month)
        {
            if (Request.IsAjaxRequest())
            {
                if (_analogHistoryRepos.DeleteThirdPointHistory(pointID, month))
                {
                    return Json(new { ifSucceed = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { ifSucceed = false }, JsonRequestBehavior.AllowGet);
                }
            }
            return null;
        }
        /// <summary>
        /// Ajax查询校区数据
        /// </summary>
        /// <param name="schoolCode"></param>
        /// <param name="schoolName"></param>
        /// <param name="currentPage"></param>
        /// <param name="totalPages"></param>
        /// <returns></returns>
        public ActionResult QuerySchoolAjax(string schoolCode, string schoolName, int? currentPage, int? totalPages)
        {
            if (Request.IsAjaxRequest() && currentPage.HasValue && totalPages.HasValue)
            {
                Pager pager = null;
                if (totalPages == -1)
                {
                    int totalRows = _schoolRepos.QuerySchoolCount(schoolCode, schoolName);
                    pager = new Pager(1, totalRows);
                }
                else
                {
                    pager = new Pager(currentPage.Value, totalPages.Value, false);
                }
                IList list = null;
                if (pager.TotalPages > 0)
                {
                    list = _schoolRepos.QuerySchool(schoolCode, schoolName, pager.StartRow, pager.PageSize);
                }
                var reData = new
                {
                    totalPages = pager.TotalPages,
                    data = list
                };
                return Json(reData, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// Ajax获取校区详细信息（废弃）
        /// </summary>
        /// <param name="schoolID"></param>
        /// <returns></returns>
        public ActionResult QuerySchoolDetailAjax(int? schoolID)
        {
            if (Request.IsAjaxRequest() && schoolID.HasValue)
            {
                var obj = _schoolRepos.QuerySchool(schoolID.Value);
                var resData = new
                {
                    schoolCode = obj.SI_Code,
                    schoolName = obj.SI_Name,
                    schoolAddr = obj.SI_Address,
                    remark = obj.SI_Remark,
                    logOperator = obj.SI_LogOperator,
                    createTime = obj.SI_CreateDate.HasValue ? obj.SI_CreateDate.Value.ToString("yyyy-MM-dd") : "",
                    updateOperator = obj.SI_UpdateOperator,
                    updateDate = obj.SI_UpdateDate.HasValue ? obj.SI_UpdateDate.Value.ToString("yyyy-MM-dd") : ""
                };
                return Json(resData, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// Ajax查询指定校区拥有的区域个数
        /// </summary>
        /// <param name="schoolID"></param>
        /// <returns></returns>
        public ActionResult QuerySchoolAreaCountAjax(int? schoolID)
        {
            if (Request.IsAjaxRequest() && schoolID.HasValue)
            {
                int count = _schoolRepos.QueryAreaCount(schoolID.Value);
                return Json(count, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 删除校区
        /// </summary>
        /// <param name="schoolID"></param>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult DeleteSchoolAjax(int? schoolID)
        {
            if (Request.IsAjaxRequest() && schoolID.HasValue)
            {
                bool flag = _schoolRepos.DeleteSchool(schoolID.Value);
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 跳转修改校区
        /// </summary>
        /// <param name="s">校区ID</param>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult ModifySchool(int? s)
        {
            if (s.HasValue && s.Value > 0)
            {
                var obj = _schoolRepos.QuerySchool(s.Value);
                ViewBag.schoolID = s;
                return View(obj);
            }
            return View();
        }

        /// <summary>
        /// 修改校区
        /// </summary>
        /// <param name="schoolID"></param>
        /// <param name="schoolCode"></param>
        /// <param name="schoolName"></param>
        /// <param name="schoolAddr"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public ActionResult ModifiedSchool(int? schoolID, string schoolCode, string schoolName, string schoolAddr, string remark, double? buildingArea, double? groudArea)
        {
            if (schoolID.HasValue)
            {

                int buildingArea2 = buildingArea.HasValue ? Convert.ToInt32(buildingArea.Value) : 0;
                int groudArea2 = groudArea.HasValue ? Convert.ToInt32(groudArea.Value) : 0;
                bool flag = _schoolRepos.ModifySchoolPartInfo(schoolID.Value, schoolCode, schoolName, schoolAddr, remark, buildingArea2, groudArea2);
                ViewBag.flag = flag;
            }
            return View();
        }

        /// <summary>
        /// 跳转增加校区
        /// </summary>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult AddSchool()
        {
            return View();
        }

        /// <summary>
        /// 增加校区
        /// </summary>
        /// <param name="schoolCode"></param>
        /// <param name="schoolName"></param>
        /// <param name="schoolAddr"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public ActionResult AddSchoolInfo(string schoolCode, string schoolName, string schoolAddr, string remark)
        {
            if (!String.IsNullOrWhiteSpace(schoolName))
            {
                bool flag = _schoolRepos.AddSchoolPart(schoolCode, schoolName, schoolAddr, remark);
                ViewBag.flag = flag;
            }
            return View();
        }

        /// <summary>
        /// 查询校区名称是否不存在
        /// </summary>
        /// <param name="schoolName"></param>
        /// <param name="oldSchoolName"></param>
        /// <returns></returns>
        public ActionResult QuerySchoolNotExistAjax(string schoolName, string oldSchoolName)
        {
            if (Request.IsAjaxRequest())
            {
                bool flag = true;
                if (oldSchoolName == null || oldSchoolName != schoolName)
                {
                    flag = !_schoolRepos.IsSchoolNameExist(schoolName);
                }
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 导出校区Excel
        /// </summary>
        /// <param name="schoolCode"></param>
        /// <param name="schoolName"></param>
        /// <returns></returns>
        public ActionResult GetSchoolExcel(string schoolCode, string schoolName)
        {
            var list = _schoolRepos.QuerySchool(schoolCode, schoolName, -1, -1);
            if (list != null)
            {
                string[] headers = { "校区名称", "校区代码", "校区地址", "校区备注" };
                string[] properties = { "SI_Name", "SI_Code", "SI_Address", "SI_Remark" };
                return this.Excel(list, "校区.xls", headers, properties);
            }
            return null;
        }

        /// <summary>
        /// 跳转区域
        /// </summary>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult QuerySchoolArea()
        {
            return View();
        }

        /// <summary>
        /// Ajax查询区域数据
        /// </summary>
        /// <param name="areaName"></param>
        /// <param name="schoolID"></param>
        /// <param name="currentPage"></param>
        /// <param name="totalPages"></param>
        /// <returns></returns>
        public ActionResult QuerySchoolAreaAjax(string areaName, int? schoolID, int? currentPage, int? totalPages)
        {
            if (Request.IsAjaxRequest() && schoolID.HasValue && currentPage.HasValue && totalPages.HasValue)
            {
                Pager pager = null;
                if (totalPages == -1)
                {
                    int totalRows = _schoolAreaRepos.GetSchoolAreaCount(areaName, schoolID.Value);
                    pager = new Pager(1, totalRows);
                    totalPages = pager.TotalPages;
                }
                else
                {
                    pager = new Pager(currentPage.Value, totalPages.Value, false);
                }
                if (pager.TotalPages > 0)
                {
                    var list = _schoolAreaRepos.GetSchoolArea(areaName, schoolID.Value, pager.StartRow, pager.PageSize);
                    var reData = new
                    {
                        totalPages = totalPages,
                        data = list
                    };
                    return Json(reData, JsonRequestBehavior.AllowGet);
                }
                return Json(new { totalPages = pager.TotalPages }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 获取区域拥有的楼宇数
        /// </summary>
        /// <param name="areaID"></param>
        /// <returns></returns>
        public ActionResult QueryAreaBuildingCountAjax(int? areaID)
        {
            if (Request.IsAjaxRequest() && areaID.HasValue)
            {
                int count = _schoolAreaRepos.GetBuildingCountByArea(areaID.Value);
                return Json(count, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 删除区域
        /// </summary>
        /// <param name="areaID"></param>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult DeleteSchoolAreaAjax(int? areaID)
        {
            if (Request.IsAjaxRequest() && areaID.HasValue)
            {
                bool flag = _schoolAreaRepos.DeleteSchoolArea(areaID.Value);
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 跳转增加区域
        /// </summary>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult AddSchoolArea()
        {
            return View();
        }

        /// <summary>
        /// 增加区域
        /// </summary>
        /// <param name="areaName"></param>
        /// <param name="schoolID"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public ActionResult AddSchoolAreaInfo(string areaName, int? schoolID, string remark)
        {
            if (!String.IsNullOrWhiteSpace(areaName) && schoolID.HasValue && schoolID.Value > 0)
            {
                bool flag = _schoolAreaRepos.AddSchoolAreaPart(areaName, schoolID.Value, remark);
                ViewBag.flag = flag;
            }
            return View();
        }

        /// <summary>
        /// 查询区域名称是否已经存在
        /// </summary>
        /// <param name="areaName"></param>
        /// <param name="oldAreaName"></param>
        /// <param name="schoolID"></param>
        /// <param name="oldSchoolID"></param>
        /// <returns></returns>
        public ActionResult QueryAreaNotExistAjax(string areaName, string oldAreaName, int? schoolID, int? oldSchoolID)
        {
            if (Request.IsAjaxRequest())
            {
                bool flag = true;
                if (oldAreaName == null || !(oldAreaName == areaName && oldSchoolID.Value == schoolID.Value))
                {
                    flag = !_schoolAreaRepos.IsAreaNameExist(areaName, schoolID);
                }
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 跳转修改区域
        /// </summary>
        /// <param name="a">区域ID</param>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult ModifySchoolArea(int? a)
        {
            if (a.HasValue && a.Value > 0)
            {
                var obj = _schoolAreaRepos.GetAreaAndSchool(a.Value);
                return View(obj);
            }
            return View();
        }

        /// <summary>
        /// 修改区域
        /// </summary>
        /// <param name="areaID"></param>
        /// <param name="schoolID"></param>
        /// <param name="areaName"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public ActionResult ModifySchoolAreaInfo(int? areaID, int? schoolID, string areaName, string remark)
        {
            if (areaID.HasValue && areaID.Value > 0 && schoolID.HasValue)
            {
                bool flag = _schoolAreaRepos.ModifySchoolAreaPartInfo(areaID.Value, schoolID.Value, areaName, remark);
                ViewBag.flag = flag;
            }
            return View();
        }

        /// <summary>
        /// 导出区域数据Excel
        /// </summary>
        /// <param name="areaName"></param>
        /// <param name="schoolID"></param>
        /// <returns></returns>
        public ActionResult GetAreaExcel(string areaName, int? schoolID)
        {
            var list = _schoolAreaRepos.GetSchoolArea(areaName, schoolID.Value);
            if (list != null)
            {
                string[] headers = { "区域名称", "所属校区", "区域备注" };
                string[] properties = { "AreaName", "SchoolName", "AreaRemark" };
                return this.Excel(list, "区域.xls", headers, properties);
            }
            return null;
        }

        /// <summary>
        /// 跳转楼宇管理
        /// </summary>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult QueryBuilding()
        {
            return View();
        }

        /// <summary>
        /// 查询楼宇数据
        /// </summary>
        /// <param name="buildingName"></param>
        /// <param name="areaID"></param>
        /// <param name="currentPage"></param>
        /// <param name="totalPages"></param>
        /// <returns></returns>
        public ActionResult QueryBuildingAjax(string buildingName, int? areaID, int? currentPage, int? totalPages)
        {
            if (Request.IsAjaxRequest() && areaID.HasValue && currentPage.HasValue && totalPages.HasValue)
            {
                Pager pager = null;
                if (totalPages == -1)
                {
                    int totalRows = _buildingRepos.GetBuildingCount(buildingName, areaID.Value);
                    pager = new Pager(1, totalRows);
                    totalPages = pager.TotalPages;
                }
                else
                {
                    pager = new Pager(currentPage.Value, totalPages.Value, false);
                }
                if (pager.TotalPages > 0)
                {
                    var list = _buildingRepos.GetBuilding(buildingName, areaID.Value, pager.StartRow, pager.PageSize);
                    var reData = new
                    {
                        totalPages = totalPages,
                        data = list
                    };
                    return Json(reData, JsonRequestBehavior.AllowGet);
                }
                return Json(new { totalPages = pager.TotalPages }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 查询建筑名称
        /// </summary>
        /// <param name="buildingId"></param>
        /// <returns></returns>
        public ActionResult QueryBuildingNameAjax(int buildingId)
        {
            if (buildingId != 0)
            {
                var buildingName = _buildingRepos.GetBuildingAndArea(buildingId).BuildingName;
                return Json(new { buildingName = buildingName }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 跳转增加楼宇
        /// </summary>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult AddBuilding()
        {
            var buildingTypesList = _buildingTypesRepos.GetAllBuildingTypes();
            ViewBag.buildingTypesList = buildingTypesList;
            return View();
        }

        /// <summary>
        /// 增加楼宇
        /// </summary>
        /// <param name="buildingName"></param>
        /// <param name="areaID"></param>
        /// <param name="b"></param>
        /// <param name="l"></param>
        /// <param name="e"></param>
        /// <param name="wp"></param>
        /// <param name="wm"></param>
        /// <param name="ke"></param>
        /// <param name="oe"></param>
        /// <returns></returns>
        public ActionResult AddBuildingInfo(int? areaID, HttpPostedFileBase uploadImg, BuildingDetailInfo b, List<LightDetail> l, List<ElevatorDetail> e, List<WaterPumpDetail> wp, List<WindMachDetail> wm, List<KitchenEquipDetail> ke, List<OfficeEquipDetail> oe)
        {
            if (!String.IsNullOrWhiteSpace(b.BDI_Name) && areaID.HasValue && areaID.Value > 0)
            {
                if (uploadImg != null)
                {
                    string saveImgUrl = "~/Content/images/buildingimgs/" + areaID.Value.ToString();
                    string baseDir = Server.MapPath(saveImgUrl);
                    if (!Directory.Exists(baseDir))
                    {
                        Directory.CreateDirectory(baseDir);
                    }
                    string fileName = DateTime.Now.Ticks + Path.GetExtension(uploadImg.FileName);
                    string filePath = Path.Combine(baseDir, fileName);
                    uploadImg.SaveAs(filePath);
                    saveImgUrl = saveImgUrl + "/" + fileName;
                    b.BDI_ImageUrl = saveImgUrl;
                }
                int newBuildingID = _buildingRepos.AddBuilding(b, areaID.Value);
                if (newBuildingID > 0)
                {
                    _buildingRepos.AddLight(l, newBuildingID);
                    _buildingRepos.AddElevator(e, newBuildingID);
                    _buildingRepos.AddWaterPump(wp, newBuildingID);
                    _buildingRepos.AddWindMach(wm, newBuildingID);
                    _buildingRepos.AddKitchenEquip(ke, newBuildingID);
                    _buildingRepos.AddOfficeEquip(oe, newBuildingID);

                    if (Session["AddBuilding.buildingAppendix"] != null)
                    {
                        List<BuildingAppendix> buildingAppendixList = Session["AddBuilding.buildingAppendix"] as List<BuildingAppendix>;
                        _buildingRepos.AddBuildingAppendix(buildingAppendixList, newBuildingID);
                    }
                    ViewBag.flag = true;
                }
                else
                {
                    ViewBag.flag = false;
                }
            }
            return View();
        }

        /// <summary>
        /// 得到增加建筑临时保存附属信息表
        /// </summary>
        /// <param name="appendixNo"></param>
        /// <returns></returns>
        public ActionResult GetBuildingAppendix(int buildingID, string appendixNo)
        {
            if (Request.IsAjaxRequest())
            {
                var dataList = _buildingRepos.GetBuildingAppendix(buildingID, appendixNo).ToList();
                List<IList> list = new List<IList>();
                if (dataList != null && dataList.Count > 0)
                {
                    var nameList = dataList.Select(x => x.BA_Name).Distinct().ToList();
                    foreach (var name in nameList)
                    {
                        var subList = dataList.Where(x => x.BA_Name == name).ToList();
                        list.Add(subList);
                    }
                }
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 添加建筑附属信息
        /// </summary>
        /// <param name="addAppendixNo"></param>
        /// <param name="ba0"></param>
        /// <param name="ba1"></param>
        /// <param name="ba2"></param>
        /// <param name="ba3"></param>
        /// <param name="ba4"></param>
        /// <returns></returns>
        public ActionResult AddBuildingAppendix(string addAppendixNo, List<BuildingAppendix> ba0, List<BuildingAppendix> ba1, List<BuildingAppendix> ba2, List<BuildingAppendix> ba3, List<BuildingAppendix> ba4)
        {
            if (Request.IsAjaxRequest())
            {
                List<BuildingAppendix> buildingAppendixList = null;
                if (Session["AddBuilding.buildingAppendix"] != null)
                {
                    buildingAppendixList = Session["AddBuilding.buildingAppendix"] as List<BuildingAppendix>;
                    buildingAppendixList.RemoveAll(x => x.BA_AppendixNo == addAppendixNo);
                }
                else
                {
                    buildingAppendixList = new List<BuildingAppendix>();
                }
                if (ba0 != null && ba0.Count > 0)
                {
                    buildingAppendixList.AddRange(ba0);
                }
                if (ba1 != null && ba1.Count > 0)
                {
                    buildingAppendixList.AddRange(ba1);
                }
                if (ba2 != null && ba2.Count > 0)
                {
                    buildingAppendixList.AddRange(ba2);
                }
                if (ba3 != null && ba3.Count > 0)
                {
                    buildingAppendixList.AddRange(ba3);
                }
                if (ba4 != null && ba4.Count > 0)
                {
                    buildingAppendixList.AddRange(ba4);
                }
                //foreach (BuildingAppendix ba in ba1)
                //{
                //    if (ba.BA_Type != null || ba.BA_Num != null || ba.BA_ManufacturerAndModel != null || ba.BA_CoolingCapacity != null || ba.BA_HeatingCapacity != null || ba.BA_Power != null || ba.BA_PowerSource != null || ba.BA_WorkPerDay != null || ba.BA_WorkPerYear != null)
                //    {
                //        buildingAppendixList.Add(ba);
                //    }
                //}
                //foreach (BuildingAppendix ba in ba2)
                //{
                //    if (ba.BA_Type != null || ba.BA_Num != null || ba.BA_ManufacturerAndModel != null || ba.BA_AirVolume != null || ba.BA_WindPressure != null || ba.BA_Power != null || ba.BA_DayWorkStart != null || ba.BA_DayWorkEnd != null || ba.BA_WorkPerYear != null)
                //    {
                //        buildingAppendixList.Add(ba);
                //    }
                //}
                //foreach (BuildingAppendix ba in ba3)
                //{
                //    if (ba.BA_Type != null || ba.BA_Num != null || ba.BA_ManufacturerAndModel != null || ba.BA_AirVolume != null || ba.BA_WindPressure != null || ba.BA_Power != null || ba.BA_DayWorkStart != null || ba.BA_DayWorkEnd != null || ba.BA_WorkPerYear != null)
                //    {
                //        buildingAppendixList.Add(ba);
                //    }
                //}
                //foreach (BuildingAppendix ba in ba4)
                //{
                //    if (ba.BA_Type != null || ba.BA_Num != null || ba.BA_ManufacturerAndModel != null || ba.BA_AirVolume != null || ba.BA_WindPressure != null || ba.BA_Power != null || ba.BA_DayWorkStart != null || ba.BA_DayWorkEnd != null || ba.BA_WorkPerYear != null)
                //    {
                //        buildingAppendixList.Add(ba);
                //    }
                //}
                //foreach (BuildingAppendix ba in ba5)
                //{
                //    if (ba.BA_Type != null || ba.BA_Num != null || ba.BA_ManufacturerAndModel != null || ba.BA_AirVolume != null || ba.BA_WindPressure != null || ba.BA_Power != null || ba.BA_DayWorkStart != null || ba.BA_DayWorkEnd != null || ba.BA_WorkPerYear != null)
                //    {
                //        buildingAppendixList.Add(ba);
                //    }
                //}
                Session["AddBuilding.buildingAppendix"] = buildingAppendixList;
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 修改建筑附属信息
        /// </summary>
        /// <param name="buildingID"></param>
        /// <param name="modifyAppendixNo"></param>
        /// <param name="ba0"></param>
        /// <param name="ba1"></param>
        /// <param name="ba2"></param>
        /// <param name="ba3"></param>
        /// <param name="ba4"></param>
        /// <returns></returns>
        public ActionResult ModifydBuildingAppendix(int buildingID, string modifyAppendixNo, List<BuildingAppendix> ba0, List<BuildingAppendix> ba1, List<BuildingAppendix> ba2, List<BuildingAppendix> ba3, List<BuildingAppendix> ba4)
        {
            if (Request.IsAjaxRequest())
            {
                List<BuildingAppendix> buildingAppendixList = new List<BuildingAppendix>();
                if (ba0 != null && ba0.Count > 0)
                {
                    buildingAppendixList.AddRange(ba0);
                }
                if (ba1 != null && ba1.Count > 0)
                {
                    buildingAppendixList.AddRange(ba1);
                }
                if (ba2 != null && ba2.Count > 0)
                {
                    buildingAppendixList.AddRange(ba2);
                }
                if (ba3 != null && ba3.Count > 0)
                {
                    buildingAppendixList.AddRange(ba3);
                }
                if (ba4 != null && ba4.Count > 0)
                {
                    buildingAppendixList.AddRange(ba4);
                }
                if (buildingAppendixList.Count > 0)
                {
                    _buildingRepos.ModifyBuildingAppendix(buildingAppendixList, buildingID, modifyAppendixNo);
                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 查询楼宇名称是否不存在
        /// </summary>
        /// <param name="buildingName"></param>
        /// <param name="oldBuildingName"></param>
        /// <param name="areaID"></param>
        /// <param name="oldAreaID"></param>
        /// <returns></returns>
        public ActionResult QueryBuildingNotExistAjax(string buildingName, string oldBuildingName, int? areaID, int? oldAreaID)
        {
            if (Request.IsAjaxRequest())
            {
                bool flag = true;
                if (oldBuildingName == null || !(oldBuildingName == buildingName && oldAreaID.Value == areaID.Value))
                {
                    flag = !_buildingRepos.IsBuildingNameExist(buildingName, areaID);
                }
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 跳转修改楼宇
        /// </summary>
        /// <param name="b">楼宇ID</param>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult ModifyBuilding(int? b)
        { 
            var buildingTypesList = _buildingTypesRepos.GetAllBuildingTypes();
            ViewBag.buildingTypesList = buildingTypesList;            
            if (b.HasValue && b.Value > 0)
            {
                var building = _buildingRepos.GetBuilding(b.Value);
                var areaInfo = _buildingRepos.GetBuildingAndArea(b.Value);

                var lightList = _buildingRepos.GetLight(b.Value);
                var elevatorList = _buildingRepos.GetElevator(b.Value);
                var waterPumpList = _buildingRepos.GetWaterPump(b.Value);
                var windMachList = _buildingRepos.GetWindMach(b.Value);
                var kitchenEquipList = _buildingRepos.GetKitchenEquip(b.Value);
                var officeEquipList = _buildingRepos.GetOfficeEquip(b.Value);

                ViewBag.areaInfo = areaInfo;
                ViewBag.lightList = lightList;
                ViewBag.elevatorList = elevatorList;
                ViewBag.waterPumpList = waterPumpList;
                ViewBag.windMachList = windMachList;
                ViewBag.kitchenEquipList = kitchenEquipList;
                ViewBag.officeEquipList = officeEquipList;

                return View(building);
            }
            return View();
        }

        /// <summary>
        /// 修改楼宇
        /// </summary>
        /// <param name="buildingID"></param>
        /// <param name="areaID"></param>
        /// <param name="buildingName"></param>
        /// <param name="buildingCode"></param>
        /// <returns></returns>
        public ActionResult ModifyBuildingInfo(int? areaID, HttpPostedFileBase uploadImg, BuildingDetailInfo b, List<LightDetail> l, List<ElevatorDetail> e, List<WaterPumpDetail> wp, List<WindMachDetail> wm, List<KitchenEquipDetail> ke, List<OfficeEquipDetail> oe, List<string> modifyFlag)
        {
            if (!String.IsNullOrWhiteSpace(b.BDI_Name) && b.BDI_ID > 0 && areaID.HasValue && areaID.Value > 0)
            {
                if (uploadImg != null)
                {
                    if (!string.IsNullOrWhiteSpace(b.BDI_ImageUrl))
                    {
                        string oldFilePath = Server.MapPath(b.BDI_ImageUrl);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }
                    string saveImgUrl = "~/Content/images/buildingimgs/" + areaID.Value.ToString();
                    string baseDir = Server.MapPath(saveImgUrl);
                    if (!Directory.Exists(baseDir))
                    {
                        Directory.CreateDirectory(baseDir);
                    }
                    string fileName = DateTime.Now.Ticks + Path.GetExtension(uploadImg.FileName);
                    string filePath = Path.Combine(baseDir, fileName);
                    uploadImg.SaveAs(filePath);
                    saveImgUrl = saveImgUrl + "/" + fileName;
                    b.BDI_ImageUrl = saveImgUrl;
                }

                bool flag = _buildingRepos.ModifyBuilding(b, areaID.Value);
                if (modifyFlag != null && modifyFlag.Count > 0)
                {
                    if (modifyFlag.Contains("l"))
                    {
                        _buildingRepos.ModifyLight(l, b.BDI_ID);
                    }
                    if (modifyFlag.Contains("e"))
                    {
                        _buildingRepos.ModifyElevator(e, b.BDI_ID);
                    }
                    if (modifyFlag.Contains("wp"))
                    {
                        _buildingRepos.ModifyWaterPump(wp, b.BDI_ID);
                    }
                    if (modifyFlag.Contains("wm"))
                    {
                        _buildingRepos.ModifyWindMach(wm, b.BDI_ID);
                    }
                    if (modifyFlag.Contains("ke"))
                    {
                        _buildingRepos.ModifyKitchenEquip(ke, b.BDI_ID);
                    }
                    if (modifyFlag.Contains("oe"))
                    {
                        _buildingRepos.ModifyOfficeEquip(oe, b.BDI_ID);
                    }
                }
                ViewBag.flag = flag;
            }
            return View();
        }

        /// <summary>
        /// 查询楼宇拥有的房间数
        /// </summary>
        /// <param name="buildingID"></param>
        /// <returns></returns>
        public ActionResult QueryBuildingRoomCountAjax(int? buildingID)
        {
            if (buildingID.HasValue)
            {
                int count = _buildingRepos.GetRoomCountByBuilding(buildingID.Value);
                return Json(count, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 删除楼宇
        /// </summary>
        /// <param name="buildingID"></param>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult DeleteBuildingAjax(int? buildingID)
        {
            if (Request.IsAjaxRequest() && buildingID.HasValue)
            {
                bool flag = _buildingRepos.DeleteBuilding(buildingID.Value);
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 查询楼宇详细
        /// </summary>
        /// <param name="buildingID"></param>
        /// <returns></returns>
        public ActionResult QueryBuildingDetailAjax(int? buildingID)
        {
            if (Request.IsAjaxRequest() && buildingID.HasValue)
            {
                var obj = _buildingRepos.GetBuilding(buildingID.Value);
                var resData = new
                {
                    buildingName = obj.BDI_Name,
                    buildingCode = obj.BDI_Code,
                    buildingAddr = obj.BDI_Address
                };
                return Json(resData, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 建筑详情
        /// </summary>
        /// <param name="b">建筑id</param>
        /// <returns></returns>
        public ActionResult DetailBuilding(int? b)
        {
            if (b.HasValue && b.Value > 0)
            {
                return View(b.Value);
            }
            return View();
        }

        /// <summary>
        /// 导出楼宇Excel
        /// </summary>
        /// <param name="buildingName"></param>
        /// <param name="areaID"></param>
        /// <returns></returns>
        public ActionResult GetBuildingExcel(string buildingName, int areaID)
        {
            var list = _buildingRepos.GetBuilding(buildingName, areaID);
            if (list != null)
            {
                string[] headers = { "楼宇名称", "所属区域" };
                string[] properties = { "BuildingName", "AreaName" };
                return this.Excel(list, "楼宇.xls", headers, properties);
            }
            return null;
        }

        /// <summary>
        /// 跳转房间管理
        /// </summary>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult QueryRoom()
        {
            return View();
        }

        /// <summary>
        /// 查询房间数据
        /// </summary>
        /// <param name="roomCode"></param>
        /// <param name="buildingID"></param>
        /// <param name="currentPage"></param>
        /// <param name="totalPages"></param>
        /// <returns></returns>
        public ActionResult QueryRoomAjax(string roomCode, int? buildingID, int? currentPage, int? totalPages)
        {
            if (Request.IsAjaxRequest() && buildingID.HasValue && currentPage.HasValue && totalPages.HasValue)
            {
                Pager pager = null;
                if (totalPages == -1)
                {
                    int totalRows = _roomRepos.GetRoomCount(roomCode, buildingID.Value);
                    pager = new Pager(1, totalRows, true);
                    totalPages = pager.TotalPages;
                }
                else
                {
                    pager = new Pager(currentPage.Value, totalPages.Value, false);
                }
                if (pager.TotalPages > 0)
                {
                    var list = _roomRepos.GetRoom(roomCode, buildingID.Value, pager.StartRow, pager.PageSize);
                    var reData = new
                    {
                        totalPages = totalPages,
                        data = list
                    };
                    return Json(reData, JsonRequestBehavior.AllowGet);
                }
                return Json(new { totalPages = pager.TotalPages }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 跳转增加房间
        /// </summary>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult AddRoom()
        {
            return View();
        }

        /// <summary>
        /// 增加房间
        /// </summary>
        /// <param name="roomCode"></param>
        /// <param name="buildingID"></param>
        /// <param name="floor"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public ActionResult AddRoomInfo(string roomCode, int? buildingID, int? floor, string remark)
        {
            if (!String.IsNullOrWhiteSpace(roomCode) && buildingID.HasValue && buildingID.Value > 0)
            {
                bool flag = _roomRepos.AddRoomPart(roomCode, buildingID.Value, floor, remark);//_buildingRepos.AddBuildingPart(buildingName, areaID.Value, buildingCode);
                ViewBag.flag = flag;
            }
            return View();
        }

        /// <summary>
        /// 查询房间名是否不存在
        /// </summary>
        /// <param name="roomName"></param>
        /// <param name="oldRoomName"></param>
        /// <param name="buildingID"></param>
        /// <param name="oldBuildingID"></param>
        /// <returns></returns>
        public ActionResult QueryRoomNotExistAjax(string roomName, string oldRoomName, int? buildingID, int? oldBuildingID)
        {
            if (Request.IsAjaxRequest())
            {
                bool flag = true;
                if (oldRoomName == null || !(oldRoomName == roomName && oldBuildingID.Value == buildingID.Value))
                {
                    flag = !_roomRepos.IsRoomNameExist(roomName, buildingID);
                }
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 跳转修改房间
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult ModifyRoom(int? r)
        {
            if (r.HasValue && r.Value > 0)
            {
                var obj = _roomRepos.GetRoomAndBuilding(r.Value);
                return View(obj);
            }
            return View();
        }

        /// <summary>
        /// 修改房间
        /// </summary>
        /// <param name="roomID"></param>
        /// <param name="roomCode"></param>
        /// <param name="buildingID"></param>
        /// <param name="floor"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public ActionResult ModifyRoomInfo(int? roomID, string roomCode, int? buildingID, int? floor, string remark)
        {
            if (!String.IsNullOrWhiteSpace(roomCode) && roomID.HasValue && roomID.Value > 0 && buildingID.HasValue && buildingID.Value > 0)
            {
                bool flag = _roomRepos.ModifyRoomPart(roomID.Value, roomCode, buildingID.Value, floor, remark);
                ViewBag.flag = flag;
            }
            return View();
        }

        /// <summary>
        /// 查询房间详细
        /// </summary>
        /// <param name="roomID"></param>
        /// <returns></returns>
        public ActionResult QueryRoomDetailAjax(int? roomID)
        {
            if (Request.IsAjaxRequest() && roomID.HasValue && roomID.Value > 0)
            {
                var obj = _roomRepos.GetRoomAndBuilding(roomID.Value);
                return Json(obj, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 查询拥有指定房间的用户数
        /// </summary>
        /// <param name="roomID"></param>
        /// <returns></returns>
        public ActionResult QueryRoomUsersCount(int? roomID)
        {
            if (roomID.HasValue && roomID.Value > 0)
            {
                int count = _roomRepos.QueryUserCountByRoom(roomID.Value);
                return Json(count, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 删除房间
        /// </summary>
        /// <param name="roomID"></param>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult DeleteRoomAjax(int? roomID)
        {
            if (Request.IsAjaxRequest() && roomID.HasValue && roomID.Value > 0)
            {
                bool flag = _roomRepos.DeleteRoom(roomID.Value);
                _ampRepos.ClearObjIDByRoom(roomID.Value);
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 导出房间Excel
        /// </summary>
        /// <param name="roomCode"></param>
        /// <param name="buildingID"></param>
        /// <returns></returns>
        public ActionResult GetRoomExcel(string roomCode, int? buildingID)
        {
            if (buildingID.HasValue)
            {
                var list = _roomRepos.GetRoom(roomCode, buildingID.Value);
                if (list != null)
                {
                    string[] headers = { "房间号", "所属楼宇", "楼层", "备注" };
                    string[] properties = { "RoomCode", "BuildingName", "Floor", "Remark" };
                    return this.Excel(list, "房间.xls", headers, properties);
                }
            }
            return null;
        }

        /// <summary>
        /// 跳转测点管理
        /// </summary>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult QueryPoint()
        {
            var powerList = _powerClassRepos.GetAll();
            var RTUList = _rtuRepos.GetAll().ToList();
            ViewBag.RTUList = RTUList;
            return View(powerList);
        }

        /// <summary>
        /// 查询测点
        /// </summary>
        /// <param name="pointID"></param>
        /// <param name="pointName"></param>
        /// <param name="objType"></param>
        /// <param name="objIDs"></param>
        /// <param name="powerType"></param>
        /// <param name="realFlag"></param>
        /// <param name="statFlag"></param>
        /// <param name="currentPage"></param>
        /// <param name="totalPages"></param>
        /// <returns></returns>
        public ActionResult QueryPointAjax(string pointID, string pointName, int? objType, int? objIDs, string powerType, int? realFlag, int? statFlag, int? RTU_No, int? AI_Serial, int currentPage, int totalPages)
        {
            if (Request.IsAjaxRequest())
            {
                var query = _ampRepos.GetAllAMP();
                if (objType.HasValue && objType != 0)
                {
                    switch (objType)
                    {
                        case 1:
                            query = query.Where(x => x.SchoolID == objIDs);
                            break;
                        case 2:
                            query = query.Where(x => x.AreaID == objIDs);
                            break;
                        case 3:
                            query = query.Where(x => x.BuildingID == objIDs);
                            break;
                        case 4:
                            query = query.Where(x => x.RoomID == objIDs);
                            break;
                        default:
                            break;
                    }
                }
                if (!string.IsNullOrWhiteSpace(pointID))
                {
                    int pointIDInt = 0;
                    Int32.TryParse(pointID, out pointIDInt);
                    query = query.Where(x => x.PNO == pointIDInt);
                }
                if (!string.IsNullOrWhiteSpace(pointName))
                {
                    query = query.Where(x => x.PName.Contains(pointName));
                }
                if (!string.IsNullOrWhiteSpace(powerType))
                {
                    String[] powerTypes = null;
                    if (powerType.Length == 3)
                    {
                        powerTypes = _powerClassRepos.GetSubPowers(powerType).Select(x => x.PC_ID).ToArray();
                        powerTypes = powerTypes.Union(new string[] { powerType }).ToArray();
                    }
                    else
                    {
                        powerTypes = new string[] { powerType };
                    }
                    query = query.Where(x => powerTypes.Contains(x.PowerType));
                }
                if (realFlag.HasValue && realFlag != -1)
                {
                    query = query.Where(x => x.RealFlag == realFlag);
                }
                if (statFlag.HasValue && statFlag != -1)
                {
                    query = query.Where(x => x.StatFlag == statFlag);
                }
                if (RTU_No.HasValue && RTU_No != -1)
                {
                    query = query.Where(x => x.RTU_NO == RTU_No);
                }
                if (AI_Serial.HasValue && AI_Serial != -1)
                {
                    query = query.Where(x => x.AI_Serial == AI_Serial);
                }
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
                if (pager.TotalPages > 0)
                {
                    var list = query.Skip(pager.StartRow).Take(pager.PageSize).ToList();
                    var resultData = new
                    {
                        totalPages = pager.TotalPages,
                        data = list
                    };
                    return Json(resultData, JsonRequestBehavior.AllowGet);
                }
                return Json(new { totalPages = pager.TotalPages }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 跳转增加测点
        /// </summary>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult AddPoint()
        {
            var powerList = _powerClassRepos.GetAll();
            var newID = _ampRepos.GetAMPMaxNo() + 1;
            ViewBag.newID = newID;
            var RTUList = _rtuRepos.GetAll();
            ViewBag.RTUList = RTUList;
            return View(powerList);
        }

        /// <summary>
        /// 跳转增加真实测点页面
        /// </summary>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult AddRealPoint()
        {
            var powerList = _powerClassRepos.GetAll();
            var newID = _analogInfoRepos.GetNextAnalogNo();
            ViewBag.newID = newID;
            var RTUList = _rtuRepos.GetAll();
            ViewBag.RTUList = RTUList;
            return View(powerList);
        }



        /// <summary>
        /// 增加测点
        /// </summary>
        /// <param name="pointID"></param>
        /// <param name="pointName"></param>
        /// <param name="objType"></param>
        /// <param name="objIDs"></param>
        /// <param name="powerType"></param>
        /// <param name="powerName"></param>
        /// <param name="realFlag"></param>
        /// <param name="statFlag"></param>
        ///  <param name="encoding"></param>
        /// <returns></returns>
        public ActionResult AddPointInfo(int? pointID, string pointName, int? schoolID, int? areaID, int? buildingID, int? roomID, string powerType, string powerName, int? realFlag, int? statFlag, int? parentPointId, int? RTU_No, int? AI_Serial, int? AI_Base, int? AI_Rate,string encoding)
        {
            if (pointID.HasValue && pointName.Length > 0 && realFlag.HasValue && statFlag.HasValue)
            {
                if (parentPointId == null || parentPointId.Value < 0)
                {
                    parentPointId = 0;
                }
                AnalogMeasurePoint amp = new AnalogMeasurePoint();
                amp.AMP_AnalogNo = pointID.Value;
                amp.AMP_Name = pointName;
                amp.AMP_CptFlag = Convert.ToByte(realFlag.Value);
                amp.AMP_Statistic = Convert.ToByte(statFlag.Value);
                amp.AMP_SchooldID = schoolID.HasValue ? schoolID.Value : 0;
                amp.AMP_SAreaID = areaID.HasValue ? areaID.Value : 0;
                amp.AMP_BuildingID = buildingID.HasValue ? buildingID.Value : 0;
                amp.AMP_RoomID = roomID.HasValue ? roomID.Value : 0;
                amp.AMP_PowerType = powerType;
                amp.AMP_PowerName = powerName;
                amp.AMP_Unit = "-";
                amp.AMP_Date = DateTime.Now;
                amp.AMP_Val = 0;
                amp.AMP_Timespan = 5;
                amp.AMP_ParentNo = parentPointId;
                amp.AMP_State = true;
                amp.AMP_ValRem = 0;
                amp.AMP_DepartID = 0;
                amp.AMP_OperationRule = "0";
                amp.AMP_OperationParameter = 0;
                amp.AMP_Encoding = encoding;
                bool pointFlag = _ampRepos.AddAMP(amp);
                bool AIFlag = true;
                if (RTU_No.HasValue && RTU_No != 0)
                {
                    AnalogInfo ai = new AnalogInfo();
                    ai.RTU_No = Convert.ToInt16(RTU_No.Value);
                    ai.AI_No = pointID.Value;
                    ai.AI_Serial = AI_Serial.HasValue ? Convert.ToInt16(AI_Serial.Value) : Convert.ToInt16(0);
                    ai.AI_Name = pointName;
                    ai.AI_LogicalLow = 0;
                    ai.AI_LogicalUp = 100000;
                    ai.AI_Decimal = 2;
                    ai.AI_Cptflag = 0;
                    ai.AI_Base = AI_Base.HasValue ? AI_Base.Value : 0;
                    ai.AI_Rate = AI_Rate.HasValue ? AI_Rate.Value : 1;
                    ai.AI_LockVal = 0;
                    ai.AI_LockFlag = 0;
                    ai.AI_Timespace = 5;
                    String power = powerType.Substring(0, 3);
                    if (power == "001")
                    {
                        ai.AI_Unit = "度";
                    }
                    else if (power == "002")
                    {
                        ai.AI_Unit = "吨";
                    }
                    else if (power == "003")
                    {
                        ai.AI_Unit = "立方米";
                    }
                    ai.AI_State = 1;
                    ai.AI_Level = 0;
                    ai.AI_Type = 0;
                    int no = _analogInfoRepos.AddAnalogInfo(ai);
                    if (no == 0) { AIFlag = false; }
                }
                ViewBag.flag = pointFlag & AIFlag;
                ViewBag.realFlag = realFlag.Value;
            }
            return View();
        }

        /// <summary>
        /// 查询测点历史值
        /// </summary>
        /// <param name="analogNo"></param>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult QueryHistoryVal(int? analogNo)
        {
            if (analogNo != null)
            {
                AnalogMeasurePoint ampInfo = _ampRepos.QueryAMPInfo(analogNo);
                return View(ampInfo);
            }
            else
            {
                return View();
            }
        }

        /// <summary>
        /// 查询测点是否被使用
        /// </summary>
        /// <param name="pointID"></param>
        /// <returns></returns>
        public ActionResult QueryAMPIsUsedAjax(int? pointID)
        {
            if (Request.IsAjaxRequest() && pointID.HasValue)
            {
                bool flag = _ampRepos.IsUsedByObj(pointID.Value);
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 删除测点
        /// </summary>
        /// <param name="pointID"></param>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult DeleteAMPAjax(int? pointID)
        {
            if (Request.IsAjaxRequest() && pointID.HasValue)
            {
                bool flag = _ampRepos.DeleteAMP(pointID.Value);
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 跳转修改测点
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult ModifyPoint(int? p)
        {
            if (p.HasValue)
            {
                var powerList = _powerClassRepos.GetAll();
                ViewBag.powerList = powerList;
                var RTUList = _rtuRepos.GetAll();
                ViewBag.RTUList = RTUList;
                var obj = _ampRepos.GetAllAMP().Where(x => x.PNO == p.Value).FirstOrDefault();
                if (obj != null)
                {
                    if (obj.IName == "未知对象")
                    {
                        obj.IName = "";
                    }
                    return View(obj);
                }
            }
            return View();
        }

        /// <summary>
        /// 修改测点
        /// </summary>
        /// <param name="pointID"></param>
        /// <param name="pointName"></param>
        /// <param name="objType"></param>
        /// <param name="objIDs"></param>
        /// <param name="powerType"></param>
        /// <param name="powerName"></param>
        /// <param name="realFlag"></param>
        /// <param name="statFlag"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public ActionResult ModifyPointInfo(int? pointID, string pointName, int? schoolID, int? areaID, int? buildingID, int? roomID, string powerType, string powerName, int? realFlag, int? statFlag, int? parentPointId, int? RTU_No, int? AI_Serial, double? AI_Rate, double? AI_Base,string encoding)
        {
            if (pointID.HasValue && realFlag.HasValue && statFlag.HasValue)
            {
                if (parentPointId == null || parentPointId.Value < 0)
                {
                    parentPointId = 0;
                }
                //for (int num = 200539; num <= 200547; num++) {
                //    _ampRepos.UpdateValueOfParentPoint(num);
                //}
                AnalogMeasurePoint oldAMP = _ampRepos.GetAMP(pointID.Value);
                int oldParentID = oldAMP.AMP_ParentNo.HasValue ? oldAMP.AMP_ParentNo.Value : 0;
                AnalogMeasurePoint amp = new AnalogMeasurePoint();
                amp.AMP_AnalogNo = pointID.Value;
                amp.AMP_Name = pointName;
                amp.AMP_CptFlag = Convert.ToByte(realFlag.Value);
                amp.AMP_Statistic = Convert.ToByte(statFlag.Value);
                amp.AMP_PowerType = powerType;
                amp.AMP_PowerName = powerName;
                //检查新的所属对象的父对象是否有测点与其关联，若没有需要新建一个虚拟测点与父对象关联，并且该虚拟测点作为当前测点的父测点
                //if (!parentPointId.HasValue || parentPointId.Value == 0)
                //{
                //    if (roomID.HasValue && roomID.Value != 0)
                //    {
                //        var parentBuilding = _roomRepos.GetRoomAndBuilding(roomID.Value);
                //        AnalogMeasurePoint newVirtualPoint = new AnalogMeasurePoint();
                //        newVirtualPoint.AMP_AnalogNo = _ampRepos.GetAMPMaxNo() + 1;
                //        newVirtualPoint.AMP_Name = parentBuilding.BuildingName + "-" + powerName + "（虚拟统计）";
                //        newVirtualPoint.AMP_CptFlag = 0;
                //        newVirtualPoint.AMP_Statistic = 1;
                //        newVirtualPoint.AMP_Date = DateTime.Now;
                //        newVirtualPoint.AMP_Val = 0;
                //        newVirtualPoint.AMP_Unit = "";
                //        newVirtualPoint.AMP_SchooldID = parentBuilding.SchoolID;
                //        newVirtualPoint.AMP_SAreaID = parentBuilding.AreaID;
                //        newVirtualPoint.AMP_BuildingID = parentBuilding.BuildingID;
                //        newVirtualPoint.AMP_RoomID = 0;
                //        newVirtualPoint.AMP_DepartID = 0;
                //        newVirtualPoint.AMP_PowerType = powerType;
                //        newVirtualPoint.AMP_PowerName = powerName;
                //        newVirtualPoint.AMP_Timespan = 5;
                //        newVirtualPoint.AMP_ParentNo = _ampRepos.GetAMPNoByArea(parentBuilding.AreaID, powerType);
                //        newVirtualPoint.AMP_OperationParameter = 1;
                //        newVirtualPoint.AMP_OperationRule = "1";
                //        newVirtualPoint.AMP_State = true;
                //        bool insertParentPointFlag = _ampRepos.AddAMP(newVirtualPoint);
                //        amp.AMP_ParentNo = newVirtualPoint.AMP_AnalogNo;
                //    }
                //}
                //else {
                //    amp.AMP_ParentNo = parentPointId;
                //}
                amp.AMP_ParentNo = parentPointId;
                amp.AMP_SchooldID = schoolID.HasValue ? schoolID.Value : 0;
                amp.AMP_SAreaID = areaID.HasValue ? areaID.Value : 0;
                amp.AMP_BuildingID = buildingID.HasValue ? buildingID.Value : 0;
                amp.AMP_RoomID = roomID.HasValue ? roomID.Value : 0;
                amp.AMP_Encoding = encoding;
                bool aiflag = true;
                if (RTU_No.HasValue || AI_Serial.HasValue || AI_Rate.HasValue || AI_Base.HasValue)
                {
                    AnalogInfo ai = new AnalogInfo();
                    ai.AI_No = pointID.Value;
                    ai.AI_Name = pointName;
                    ai.RTU_No = RTU_No.HasValue ? Convert.ToInt16(RTU_No) : Convert.ToInt16(0);
                    ai.AI_Serial = AI_Serial.HasValue ? Convert.ToInt32(AI_Serial) : Convert.ToInt32(0);
                    ai.AI_Rate = AI_Rate.HasValue ? AI_Rate.Value : 1;
                    ai.AI_Base = AI_Base.HasValue ? AI_Base.Value : 0;
                    aiflag = _analogInfoRepos.ModifyAI(ai);
                }

                bool ampflag = _ampRepos.ModifyAMP(amp);
                ViewBag.flag = aiflag & ampflag;

                //如果测点父测点有改变，则需要刷新新的父测点的历史值和旧的父测点的历史值
                //if (amp.AMP_ParentNo != oldParentID)
                //{
                //    if ( amp.AMP_ParentNo.HasValue && amp.AMP_ParentNo != 0 )
                //    {
                //        _ampRepos.UpdateValueOfParentPoint(amp.AMP_ParentNo.Value);
                //    }
                //    if (oldParentID != 0)
                //    {
                //        _ampRepos.UpdateValueOfParentPoint(oldParentID);
                //    }
                //}
            }
            return View();
        }

        /// <summary>
        /// 查询父测点编号
        /// </summary>
        /// <param name="schoolID">校区Id</param>
        /// <param name="areaID">区域Id</param>
        /// <param name="buildingID">建筑Id</param>
        /// <param name="roomID">房间Id</param>
        /// <param name="powerId">能耗类型</param>
        /// <returns></returns>
        public ActionResult QueryParentAMP(int? schoolID, int? areaID, int? buildingID, int? roomID, string powerId)
        {
            if (Request.IsAjaxRequest())
            {
                var query = _ampRepos.GetAllAMP();
                if (roomID.HasValue && roomID.Value > 0)
                {
                    query = query.Where(x => x.RealFlag == 0 && x.StatFlag == 1 && x.PowerType == powerId && x.BuildingID == buildingID.Value && x.RoomID == 0);
                }
                else if (buildingID.HasValue && buildingID.Value > 0)
                {
                    query = query.Where(x => x.RealFlag == 0 && x.StatFlag == 1 && x.PowerType == powerId && x.AreaID == areaID.Value && x.BuildingID == 0);
                }
                else if (areaID.HasValue && areaID.Value > 0)
                {
                    query = query.Where(x => x.RealFlag == 0 && x.StatFlag == 1 && x.PowerType == powerId && x.SchoolID == schoolID.Value && x.AreaID == 0);
                }
                else
                {
                    return Json(new { totalPages = 0 }, JsonRequestBehavior.AllowGet);
                }
                var list = query.ToList();
                var resultData = new
                {
                    totalPages = list.Count,
                    data = list
                };
                return Json(resultData, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public ActionResult UpdateHistoryValue(int analogNo)
        {
            if (Request.IsAjaxRequest())
            {
                bool ifSucceed = _ampRepos.UpdateValueOfParentPoint(analogNo);
                if (ifSucceed)
                {
                    return Json(new { ifSucceed = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { ifSucceed = false }, JsonRequestBehavior.AllowGet);
                }
            }
            return null;
        }

        /// <summary>
        /// 获取对象和类型已经关联的测点
        /// </summary>
        /// <param name="schoolID"></param>
        /// <param name="areaID"></param>
        /// <param name="buildingID"></param>
        /// <param name="roomID"></param>
        /// <param name="powerId"></param>
        /// <returns></returns>
        public ActionResult QueryBoundAMP(int? schoolID, int? areaID, int? buildingID, int? roomID, string powerId)
        {
            if (Request.IsAjaxRequest())
            {
                var query = _ampRepos.GetAllAMP();
                if (schoolID.HasValue && areaID.HasValue && buildingID.HasValue && roomID.HasValue)
                {
                    query = query.Where(x => x.PowerType == powerId && x.SchoolID == schoolID.Value && x.AreaID == areaID.Value && x.BuildingID == buildingID.Value && x.RoomID == roomID.Value);
                }
                else
                {
                    return Json(new { totalPages = 0 }, JsonRequestBehavior.AllowGet);
                }
                var list = query.ToList();
                var resultData = new
                {
                    totalPages = list.Count,
                    data = list
                };
                return Json(resultData, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 导出测点数据Excel
        /// </summary>
        /// <param name="pointID"></param>
        /// <param name="pointName"></param>
        /// <param name="objType"></param>
        /// <param name="objIDs"></param>
        /// <param name="powerType"></param>
        /// <param name="realFlag"></param>
        /// <param name="statFlag"></param>
        /// <returns></returns>
        public ActionResult GetPointExcel(string pointID, string pointName, int? objType, int? objIDs, string powerType, int? realFlag, int? statFlag)
        {
            var query = _ampRepos.GetAllAMP();
            if (objType.HasValue && objType != 0)
            {
                switch (objType)
                {
                    case 1:
                        query = query.Where(x => x.SchoolID == objIDs);
                        break;
                    case 2:
                        query = query.Where(x => x.AreaID == objIDs);
                        break;
                    case 3:
                        query = query.Where(x => x.BuildingID == objIDs);
                        break;
                    case 4:
                        query = query.Where(x => x.RoomID == objIDs);
                        break;
                    default:
                        break;
                }
            }
            if (!string.IsNullOrWhiteSpace(pointID))
            {
                int pointIDInt = 0;
                Int32.TryParse(pointID, out pointIDInt);
                query = query.Where(x => x.PNO == pointIDInt);
            }
            if (!string.IsNullOrWhiteSpace(pointName))
            {
                query = query.Where(x => x.PName.Contains(pointName));
            }
            if (!string.IsNullOrWhiteSpace(powerType))
            {
                string[] powerTypes = powerType.Split(new char[] { '_' });
                query = query.Where(x => powerTypes.Contains(x.PowerType));
            }
            if (realFlag.HasValue && realFlag != -1)
            {
                query = query.Where(x => x.RealFlag == realFlag);
            }
            if (statFlag.HasValue && statFlag != -1)
            {
                query = query.Where(x => x.StatFlag == statFlag);
            }
            var list = query.ToList();
            if (list != null)
            {
                string[] headers = { "测点编号", "测点名称", "所属对象", "最新取值时间", "表值（度）", "剩余电量（度）", "所属能耗类型", "父测点编号", "是否真实点", "是否计算点" };
                string[] properties = { "PNO", "PName", "IName", "Time", "ValStr", "RemValStr", "PowerName", "ParentNoStr", "RealFlagStr", "StatFlagStr" };
                return this.Excel(list, "测点.xls", headers, properties);
            }
            return null;
        }

        /// <summary>
        /// 跳转导入历史数据
        /// </summary>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult ImportPoint()
        {
            return View();
        }

        /// <summary>
        /// 下载导入的Excel模板
        /// </summary>
        /// <returns></returns>
        public ActionResult DownloadImportExcel()
        {
            var excelPath = Server.MapPath("~/Content/downloads/importtemplate.xls");           
            return File(excelPath, "application/vnd.ms-excel", "导入历史数据模板.xls");           
        }


        /// <summary>
        /// 导入历史数据
        /// </summary>
        /// <param name="uploadExcel"></param>
        /// <returns></returns>
        public ActionResult ImportingPoint(HttpFileCollectionBase uploadExcel)
        {
            // 成功处理的excel个数
            int excelCount = 0;
            DataSet OleDsExcel = new DataSet();
            foreach (string upload in Request.Files)
            {
                if (!Request.Files[upload].HasFile())
                {
                    continue;
                }
                string fileExtension = Path.GetExtension(Request.Files[upload].FileName);
                // 判断是否上传的后缀是否存在
                string uploadExcelExtens = Util.GetConfigValue("uploadExcelExtens");
                string[] uploadExcelExtensions = uploadExcelExtens.Split(new char[] { ',' });
                string preExtension = "";
                if (fileExtension.Length > 0)
                {
                    preExtension = fileExtension.Substring(1);
                }
                if (!uploadExcelExtensions.Contains(preExtension))
                {
                    continue;
                }
                string tempFileName = DateTime.Now.Ticks + fileExtension;
                string uploadsPath = Server.MapPath("~/Content/uploads/");
                string fullTempFilePath = Path.Combine(uploadsPath, tempFileName);
                // 保存为临时的Excel文件
                Request.Files[upload].SaveAs(fullTempFilePath);

                string strConn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fullTempFilePath + ";Extended Properties='Excel 8.0;HDR=NO;IMEX=1';";
                OleDbConnection oleConn = new OleDbConnection(strConn);
                oleConn.Open();
                String excelSql = "SELECT * FROM  [Sheet1$]";
                OleDbDataAdapter OleDaExcel = new OleDbDataAdapter(excelSql, oleConn);
                OleDaExcel.Fill(OleDsExcel, "Sheet1");
                oleConn.Close();
                // 删除文件
                System.IO.File.Delete(fullTempFilePath);
                excelCount++;
            }
            int importedPointCount = 0;
            if (excelCount > 0)
            {
                importedPointCount = _analogHistoryRepos.ImportMonthData(OleDsExcel);
            }
            ViewBag.importedPointCount = importedPointCount;
            return View();
        }

        /// <summary>
        /// 跳转院系管理
        /// </summary>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult QueryDepart()
        {
            return View();
        }

        /// <summary>
        /// 查询院系
        /// </summary>
        /// <param name="departName"></param>
        /// <param name="currentPage"></param>
        /// <param name="totalPages"></param>
        /// <returns></returns>
        public ActionResult QueryDepartAjax(string departName, int currentPage, int totalPages)
        {
            if (Request.IsAjaxRequest())
            {
                Pager pager = null;
                if (totalPages == -1)
                {
                    int totalRows = _departmentRepos.GetDepartmentsByName(departName).Count();
                    pager = new Pager(1, totalRows);
                }
                else
                {
                    pager = new Pager(currentPage, totalPages, false);
                }
                if (pager.TotalPages > 0)
                {
                    var list = _departmentRepos.GetDepartmentsByName(departName).Skip(pager.StartRow).Take(pager.PageSize).ToList();

                    var reData = new
                    {
                        totalPages = pager.TotalPages,
                        data = list
                    };
                    return Json(reData, JsonRequestBehavior.AllowGet);
                }
                return Json(new { totalPages = pager.TotalPages }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 跳转增加院系
        /// </summary>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult AddDepart()
        {
            return View();
        }

        /// <summary>
        /// 查询院系名称是否已经存在
        /// </summary>
        /// <param name="departName"></param>
        /// <returns></returns>
        public ActionResult QueryDepartNameAjax(string departName)
        {
            bool flag = true;
            if (Request.IsAjaxRequest())
            {
                int count = _departmentRepos.GetDepartmentsWithName(departName).Count();
                if (count > 0)
                {
                    flag = false;
                }
            }
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 查询修改时院系名称是否已经存在
        /// </summary>
        /// <param name="departName"></param>
        /// <returns></returns>
        public ActionResult QueryModifyDepartNameAjax(string departName, string oldDepartName)
        {
            if (Request.IsAjaxRequest())
            {
                bool flag = true;
                if (departName != oldDepartName)
                {
                    int count = _departmentRepos.GetDepartmentsWithName(departName).Count();
                    if (count > 0)
                    {
                        flag = false;
                    }
                }
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 增加院系
        /// </summary>
        /// <param name="departName"></param>
        /// <param name="linkMan"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public ActionResult AddDepartInfo(string departName, string linkMan, string remark)
        {
            if (!string.IsNullOrWhiteSpace(departName))
            {
                DepartmentInfo depart = new DepartmentInfo();
                depart.DI_Name = departName;
                depart.DI_LinkMan = linkMan;
                depart.DI_Remark = remark;
                bool flag = _departmentRepos.AddDepartment(depart);
                ViewBag.flag = flag;
            }
            return View();
        }

        /// <summary>
        /// 跳转修改院系
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult ModifyDepart(int? d)
        {
            if (d.HasValue)
            {
                var obj = _departmentRepos.GetDepartmentByID(d.Value);
                if (obj != null)
                {
                    return View(obj);
                }
            }
            return View();
        }

        /// <summary>
        /// 修改院系
        /// </summary>
        /// <param name="departID"></param>
        /// <param name="departName"></param>
        /// <param name="linkMan"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public ActionResult ModifyDepartInfo(int? departID, string departName, string linkMan, string remark)
        {
            if (departID.HasValue && !string.IsNullOrWhiteSpace(departName))
            {
                DepartmentInfo depart = new DepartmentInfo();
                depart.DI_ID = departID.Value;
                depart.DI_Name = departName;
                depart.DI_LinkMan = linkMan;
                depart.DI_Remark = remark;
                bool flag = _departmentRepos.ModifyDepartment(depart);
                ViewBag.flag = flag;
            }
            return View();
        }

        /// <summary>
        /// 查询院系的用户数
        /// </summary>
        /// <param name="departID"></param>
        /// <returns></returns>
        public ActionResult QueryDepartUsersCountAjax(int? departID)
        {
            if (Request.IsAjaxRequest() && departID.HasValue)
            {
                int count = _departmentRepos.QueryUserCountByDepart(departID.Value);
                return Json(count, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 删除院系
        /// </summary>
        /// <param name="departID"></param>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult DeleteDepartAjax(int? departID)
        {
            if (Request.IsAjaxRequest() && departID.HasValue)
            {
                bool flag = _departmentRepos.DeleteDepartment(departID.Value); ;
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 导出院系查询数据Excel
        /// </summary>
        /// <param name="departName"></param>
        /// <returns></returns>
        public ActionResult GetDepartExcel(string departName)
        {
            var list = _departmentRepos.GetDepartmentsByName(departName).ToList();
            if (list != null)
            {
                string[] headers = { "院系名称", "联系人", "备注" };
                string[] properties = { "DI_Name", "DI_LinkMan", "DI_Remark" };
                return this.Excel(list, "院系.xls", headers, properties);
            }
            return null;
        }

        /// <summary>
        /// 跳转能耗类型管理
        /// </summary>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult QueryPower()
        {
            var powerList = _powerClassRepos.GetAll();
            return View(powerList);
        }

        /// <summary>
        /// 添加类型
        /// </summary>
        /// <param name="powerID"></param>
        /// <param name="powerName"></param>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult AddNewPowerAjax(string powerID, string powerName)
        {
            if (Request.IsAjaxRequest())
            {
                PowerClass pc = new PowerClass();
                pc.PC_ID = powerID;
                pc.PC_Name = powerName;
                pc.PC_Type = "-";
                pc.PC_LocalCode = "-";
                pc.PC_Unit = "-";
                bool flag = _powerClassRepos.AddPower(pc);
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 修改能耗名称
        /// </summary>
        /// <param name="powerID"></param>
        /// <param name="powerName"></param>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult ModifyPowerNameAjax(string powerID, string powerName)
        {
            if (Request.IsAjaxRequest())
            {
                PowerClass pc = new PowerClass();
                pc.PC_ID = powerID;
                pc.PC_Name = powerName;
                pc.PC_Type = "-";
                pc.PC_LocalCode = "-";
                pc.PC_Unit = "-";
                bool flag = _powerClassRepos.ModifyPower(pc);
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 查询能耗类型及其子类型是否已经被使用
        /// </summary>
        /// <param name="powerID"></param>
        /// <returns></returns>
        public ActionResult IsPowerUsed(string powerID)
        {
            if (Request.IsAjaxRequest())
            {
                bool flag = _powerClassRepos.IsUsedByAMP(powerID);
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 删除能耗
        /// </summary>
        /// <param name="powerID"></param>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult RemovePowerAjax(string powerID)
        {
            if (Request.IsAjaxRequest())
            {
                bool flag = _powerClassRepos.DeletePower(powerID);
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 查询指定测点某个时刻的值范围
        /// </summary>
        /// <param name="analogId"></param>
        /// <param name="inputDateTime"></param>
        /// <returns></returns>
        public ActionResult QueryValRange(int analogId, DateTime inputDateTime)
        {
            if (Request.IsAjaxRequest())
            {
                IDictionary<string, string> dic = _analogHistoryRepos.GetTwoEndpointVal(analogId, inputDateTime);
                return Json(dic, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 查询指定测点某个时刻的值范围
        /// </summary>
        /// <param name="analogId"></param>
        /// <param name="inputDateTimeLong">时间戳</param>
        /// <returns></returns>
        public ActionResult QueryValRangeAlt(int analogId, long inputDateTimeLong)
        {
            if (Request.IsAjaxRequest())
            {
                DateTime inputDateTime = new DateTime(inputDateTimeLong);
                IDictionary<string, string> dic = _analogHistoryRepos.GetTwoEndpointValAlt(analogId, inputDateTime);
                return Json(dic, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 添加测点历史值
        /// </summary>
        /// <param name="analogNo"></param>
        /// <param name="time"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult AddEnergyHistoryAjax(int analogNo, DateTime time, double value)
        {
            if (Request.IsAjaxRequest())
            {
                if (_analogHistoryRepos.AddEnergyHistory(analogNo, time, value))
                {
                    var resultData = new
                    {
                        ifSucceed = true
                    };
                    return Json(resultData, JsonRequestBehavior.AllowGet);
                }
                return Json(new { ifSucceed = false }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 获取测点的历史值，分页
        /// </summary>
        /// <param name="analogNo"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public ActionResult QueryEnergyHistoryAjax(int currentPage, int totalPages, int analogNo, DateTime startTime, DateTime endTime, int? granularity)
        {
            if (Request.IsAjaxRequest())
            {
                Pager pager = null;
                IList historyValList = null;
                if (totalPages == -1)
                {
                    int totalRows = 0;
                    if (granularity == null || granularity == 0) totalRows = _analogHistoryRepos.GetEnergyHistory(analogNo, startTime, endTime).Count();
                    else if (granularity == 1) totalRows = _analogHistoryRepos.GetHourEnergyHistoryByAnalogNo(analogNo, startTime, endTime).Count();
                    else totalRows = _analogHistoryRepos.GetdayEnergyHistoryByAnalogNo(analogNo, startTime, endTime).Count();
                    pager = new Pager(1, totalRows);
                }
                else
                {
                    pager = new Pager(currentPage, totalPages, false);
                }
                if (pager.TotalPages > 0)
                {
                    if (granularity == null || granularity == 0) historyValList = _analogHistoryRepos.GetEnergyHistory(analogNo, startTime, endTime).Skip(pager.StartRow).Take(pager.PageSize).ToList();
                    else if (granularity == 1) historyValList = _analogHistoryRepos.GetHourEnergyHistoryByAnalogNo(analogNo, startTime, endTime).Skip(pager.StartRow).Take(pager.PageSize).ToList();
                    else historyValList = _analogHistoryRepos.GetdayEnergyHistoryByAnalogNo(analogNo, startTime, endTime).Skip(pager.StartRow).Take(pager.PageSize).ToList();
                }
                var resultData = new
                {
                    totalPages = pager.TotalPages,
                    data = historyValList
                };
                return Json(resultData, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 根据时间和粒度，查询指定测点的历史能耗数据
        /// 粒度：
        /// 0：原始
        /// 1：按每小时
        /// 2：按天
        /// </summary>
        /// <param name="analogNo"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="granularity"></param>
        /// <returns></returns>
        public ActionResult QueryAllEnergyHistoryAjax(int analogNo, DateTime startTime, DateTime endTime, int? granularity)
        {
            if (Request.IsAjaxRequest())
            {
                if (granularity == null || granularity == 0)
                {
                    IList historyValList = _analogHistoryRepos.GetEnergyHistory(analogNo, startTime, endTime).ToList();
                    var resultData = new
                    {
                        data = historyValList
                    };
                    return Json(resultData, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    IList historyValList = new List<EnergyEntity>();
                    IList temList;

                    if (granularity == 1)
                    {
                        temList = _analogHistoryRepos.GetHourEnergyHistoryByAnalogNo(analogNo, startTime, endTime).ToList();
                        startTime = new DateTime(startTime.Year , startTime.Month , startTime.Day , startTime.Hour , 0 , 0);
                        endTime = new DateTime(endTime.Year, endTime.Month, endTime.Day, endTime.Hour, 0, 0);
                        
                    }
                    else
                    {
                        temList = _analogHistoryRepos.GetdayEnergyHistoryByAnalogNo(analogNo, startTime, endTime).ToList();
                        startTime = new DateTime(startTime.Year, startTime.Month, startTime.Day, 0, 0, 0);
                        endTime = new DateTime(endTime.Year, endTime.Month, endTime.Day, 0, 0, 0);
                    }
                    //填补缺失时间点数据, 将值设为0.0表示缺失
                    for (int i = 0; i < temList.Count || startTime <= endTime; )
                    {
                        EnergyEntity ee = null;
                        if(i < temList.Count) ee = (EnergyEntity)temList[i];
                        if (i < temList.Count && ee.RealTime == startTime)
                        {
                            historyValList.Add(ee);
                            i++;
                        }
                        else
                        {
                            historyValList.Add(new EnergyEntity
                            {
                                PNO = analogNo,
                                STime = startTime,
                                RealTime = startTime,
                                Val = 0.0
                            });
                        }
                        if (granularity == 1) startTime = startTime.AddHours(1);
                        else startTime = startTime.AddDays(1);
                    }

                    IList processedHistoryValList = new ArrayList();
                    for (int i = 1; i < historyValList.Count; i++)
                    {
                        EnergyEntity ee = (EnergyEntity)historyValList[i];
                        EnergyEntity preEe = (EnergyEntity)historyValList[i - 1];
                        EnergyEntity newEe = new EnergyEntity();
                        newEe.PNO = ee.PNO;
                        newEe.STime = ee.RealTime;
                        newEe.RealTime = ee.RealTime;
                        if (ee.Val > 0 && preEe.Val > 0) newEe.Val = ee.Val - preEe.Val;
                        else newEe.Val = 0;
                        processedHistoryValList.Add(newEe);
                    }
                    var resultData = new
                    {
                        data = historyValList,
                        processedData = processedHistoryValList
                    };
                    return Json(resultData, JsonRequestBehavior.AllowGet);
                }
            }
            return null;
        }

        /// <summary>
        /// 修改测点值
        /// </summary>
        /// <param name="analogNo"></param>
        /// <param name="time"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult ModifyAnalogHistory(int analogNo, long timeLongVal, double value)
        {
            if (Request.IsAjaxRequest())
            {
                DateTime time = new DateTime(timeLongVal);
                var flag = _analogHistoryRepos.Modify(analogNo, time, value);
                return Json(new { isSucceed = flag }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        ///  给指定时间段内的所有历史值增加或减小一定数值
        /// </summary>
        /// <param name="analogNo"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ActionResult ModifyAnalogHistoryByTimePeriod(int analogNo, DateTime startTime, DateTime endTime, double value)
        {
            if (Request.IsAjaxRequest())
            {
                Boolean ifSucceed = _analogHistoryRepos.ModifyByTimePeriod(analogNo, startTime, endTime, value);
                this.AddUpdateInfo(analogNo, startTime, endTime);
                return Json(new { ifSucceed = ifSucceed }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 删除指定时间段内的所有历史值
        /// </summary>
        /// <param name="analogNo"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public ActionResult DeleteAnalogHistoryByTimePeriod(int analogNo, DateTime startTime, DateTime endTime)
        {
            if (Request.IsAjaxRequest())
            {
                Boolean ifSucceed = _analogHistoryRepos.DeleteByTimePeriod(analogNo, startTime, endTime);
                this.AddUpdateInfo(analogNo, startTime, endTime);
                return Json(new { ifSucceed = ifSucceed }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }


        /// <summary>
        /// 获取指定时间段内测点的历史值个数
        /// </summary>
        /// <param name="analogNo"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public ActionResult AICountByTimePeriod(int analogNo, DateTime startTime, DateTime endTime)
        {
            if (Request.IsAjaxRequest())
            {
                int AICount = _analogHistoryRepos.AICountByTimePeriod(analogNo, startTime, endTime);
                return Json(new { AICount = AICount }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 获取对象关联测点的所有能耗类型
        /// </summary>
        /// <param name="objID"></param>
        /// <param name="ObjType"></param>
        /// <returns></returns>
        public ActionResult PowerTypesOfObj(String objID, int ObjType)
        {
            if (Request.IsAjaxRequest())
            {
                var powerTypes = _ampRepos.GetPowerTypesOfObj(objID, ObjType).OrderBy(x => x.PowerTypeID).ToList();
                return Json(powerTypes, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 删除测点值
        /// </summary>
        /// <param name="analogNo"></param>
        /// <param name="timeLongVal"></param>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult DeleteAnalogHistory(int analogNo, long timeLongVal)
        {
            if (Request.IsAjaxRequest())
            {
                DateTime time = new DateTime(timeLongVal);
                var flag = _analogHistoryRepos.Delete(analogNo, time);
                return Json(new { isSucceed = flag }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 跳转模拟量管理
        /// </summary>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult QueryAnalogInfo()
        {
            return View();
        }

        /// <summary>
        /// 获取模拟量数据
        /// </summary>
        /// <param name="analogNo"></param>
        /// <param name="rtuNo"></param>
        /// <param name="currentPage"></param>
        /// <param name="totalPages"></param>
        /// <returns></returns>
        public ActionResult QueryAnalogInfoData(int? analogNo, int? rtuNo, int currentPage, int totalPages)
        {
            if (Request.IsAjaxRequest())
            {
                var query = _analogInfoRepos.GetAll();
                if (analogNo.HasValue && analogNo.Value > 0)
                {
                    query = query.Where(x => x.AI_No == analogNo.Value);
                }
                if (rtuNo.HasValue && rtuNo.Value > 0)
                {
                    query = query.Where(x => x.RTU_No == Convert.ToInt16(rtuNo.Value));
                }
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
                if (pager.TotalPages > 0)
                {
                    //var list = query.Skip(pager.StartRow).Take(pager.PageSize).ToList();
                    List<AnalogInfo> list = query.ToList<AnalogInfo>();
                    var resultData = new
                    {
                        totalPages = pager.TotalPages,
                        data = list
                    };
                    return Json(resultData, JsonRequestBehavior.AllowGet);
                }
                return Json(new { totalPages = pager.TotalPages }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 导出模拟量
        /// </summary>
        /// <param name="analogNo"></param>
        /// <param name="rtuNo"></param>
        /// <returns></returns>
        public ActionResult ExportAnalogInfoExcel(int? analogNo, int? rtuNo)
        {
            var query = _analogInfoRepos.GetAll();
            if (analogNo.HasValue && analogNo.Value > 0)
            {
                query = query.Where(x => x.AI_No == analogNo.Value);
            }
            if (rtuNo.HasValue && rtuNo.Value > 0)
            {
                query = query.Where(x => x.RTU_No == rtuNo.Value);
            }
            var list = query.ToList();
            if (list != null)
            {
                string[] headers = { "模拟量编号", "RTU编号", "模拟量序号", "模拟量名称", "合理下限", "合理上限", "小数点位数", "计算点标志", "基数", "比率", "锁定值", "锁定标志", "时间间隔", "量纲", "模拟量状态" };
                string[] properties = { "AI_No", "RTU_No", "AI_Serial", "AI_Name", "AI_LogicalLow", "AI_LogicalUp", "AI_Decimal", "AI_Cptflag", "AI_Base", "AI_Rate", "AI_LockVal", "AI_LockFlag", "AI_Timespace", "AI_Unit", "AI_State" };
                return this.Excel(list, "模拟量.xls", headers, properties);
            }
            return null;
        }

        /// <summary>
        /// 查询模拟量详细
        /// </summary>
        /// <param name="analogNo"></param>
        /// <returns></returns>
        public ActionResult GetAnalogInfoDetail(int? analogNo)
        {
            if (Request.IsAjaxRequest())
            {
                var query = _analogInfoRepos.GetAll().Where(x => x.AI_No == analogNo).SingleOrDefault();
                if (query != null)
                {
                    return Json(new { data = query }, JsonRequestBehavior.AllowGet);
                }
            }
            return null;
        }

        /// <summary>
        /// 跳转增加模拟量
        /// </summary>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult AddAnalogInfo()
        {
            var rtuList = _rtuRepos.GetAll().ToList();
            return View(rtuList);
        }

        /// <summary>
        /// 增加模拟量
        /// </summary>
        /// <param name="ai"></param>
        /// <returns></returns>
        public ActionResult AddAnalogInfoData(AnalogInfo ai)
        {
            var flag = _analogInfoRepos.AddAnalogInfo(ai);
            ViewBag.flag = flag;
            return View();
        }

        /// <summary>
        /// 跳转修改模拟量
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult ModifyAnalogInfo(int? a)
        {
            if (a.HasValue)
            {
                var ai = _analogInfoRepos.GetAll().Where(x => x.AI_No == a.Value).SingleOrDefault();
                if (ai != null)
                {
                    View(ai);
                }
            }
            var rtuList = _rtuRepos.GetAll().ToList();
            ViewBag.rtuList = rtuList;
            return View();
        }

        /// <summary>
        /// 修改模拟量
        /// </summary>
        /// <param name="ai"></param>
        /// <returns></returns>
        public ActionResult ModifyAnalogInfoData(AnalogInfo ai)
        {
            var flag = _analogInfoRepos.ModifyAnalogInfo(ai);
            ViewBag.flag = flag;
            return View();
        }


        /// <summary>
        /// 跳转全校信息管理界面
        /// </summary>
        /// <returns></returns>
        public ActionResult ModifyUniversity()
        {
            UniversityInfo uniInfo = _schoolRepos.GetUniversityInfo();
            return View(uniInfo);
        }

        public ActionResult ModifyUniverityInfoAjax(int? StudentCount, float? Area)
        {
            if (Request.IsAjaxRequest())
            {
                if (StudentCount.HasValue && Area.HasValue)
                {
                    _schoolRepos.ModifyUniversityInfo(StudentCount.Value, Area.Value);
                    return Json(new { ifSucceed = true }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { ifSucceed = false }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 跳转建筑额定能耗管理界面
        /// </summary>
        /// <returns></returns>
        public ActionResult BuildingEnergyConsum() {
            return View();
        }

        /// <summary>
        /// 查询某栋建筑的所有额定能耗信息
        /// </summary>
        /// <param name="buildingID"></param>
        /// <returns></returns>
        public ActionResult QueryBEC(int? buildingID) {
            if (Request.IsAjaxRequest())
            {
                if (buildingID.HasValue && buildingID != 0)
                {
                    var BECInfo = _becRepos.QueryBEC(buildingID.Value).OrderBy(x => x.year).ToList();
                    if (BECInfo.Count() > 0)
                    {
                        return Json(new { ifSucceed = true, data = BECInfo }, JsonRequestBehavior.AllowGet);
                    }
                    else {
                        return Json(new { ifSucceed = false, data = new { } }, JsonRequestBehavior.AllowGet); 
                    }
                }
                else {
                    return Json(new { ifSucceed = false, data = new { } }, JsonRequestBehavior.AllowGet); 
                }
            }
            return null;
        }

        /// <summary>
        /// 修改或添加某条建筑额定能耗记录
        /// </summary>
        /// <param name="buildingID"></param>
        /// <param name="year"></param>
        /// <param name="powerType"></param>
        /// <param name="Val"></param>
        /// <returns></returns>
        public ActionResult ModifyOrAddBEC(int buildingID, int year, string powerType, double Val) {
            if (Request.IsAjaxRequest()) {
                bool ifSucceed = _becRepos.ModifyOrAddBEC(buildingID, year, powerType, Val);
                return Json(new { ifSucceed = ifSucceed }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public ActionResult GetBECValue(int buildingID, int year, string powerType)
        {
            if (Request.IsAjaxRequest()) {
                double val = _becRepos.GetBuildingConsum(buildingID, year, powerType);
                return Json(val, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        #region 维护工具页面

        /// <summary>
        /// 跳转所属对象列表
        /// </summary>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult QueryPointRelation()
        {
            var schools = _roomRepos.GetAllSchool();
            IList list = new ArrayList();
            foreach (var item in schools)
            {
                list.Add(new
                {
                    dataID = item.SI_ID,
                    dataValue = item.SI_Name
                });
            }
            ViewBag.schoolList = list;
            return View();
        }

        /// <summary>
        /// 跳转测点关联列表
        /// </summary>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult QueryPointList()
        {
            //获取校区级测点
            var schoolPoint = _analogMeasurePointRepos.GetMeasurePointByLevel(1);
            IList splist = new ArrayList();
            foreach (var item in schoolPoint)
            {
                splist.Add(new
                {
                    pointID = item.AMP_AnalogNo,
                    pointName = item.AMP_Name,
                    pointCptFlag = item.AMP_CptFlag,
                    pointLevel = 1
                });
            }
            //添加哨兵测点，为了使区域级且父测点为0的测点能够显示
            splist.Add(new { 
                pointID = 0,
                pointName = "其他",
                pointCptFlag = 0,
                pointLevel = 1
            });
            ViewBag.schoolPointList = splist;

            //获取建筑列表
            var bBriefInfo = _buildingRepos.GetBuildingHasOperateRule();
            IList ruleList = GetRuleListByBuildingBriefInfoList(bBriefInfo);//保存所有公式和公式中测点的信息
            ViewBag.ruleList = ruleList;

            return View();
        }

        /// <summary>
        /// 查询指定测点关联关系
        /// </summary>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult QueryXPointRelation()
        {
            var powerList = _powerClassRepos.GetAll();
            var RTUList = _rtuRepos.GetAll().ToList();
            ViewBag.RTUList = RTUList;
            return View(powerList);
        }

        /// <summary>
        /// 批量导入真实测点
        /// </summary>
        /// <param name="uploadExcel"></param>
        /// <returns></returns>
        public ActionResult BatchImportingPoint(HttpFileCollectionBase uploadExcel)
        {
            IList filePathList = SaveExcelFiles(Request);
            GetParentPointOfImportPoints(filePathList[0].ToString());
            ViewBag.filePath = filePathList[0].ToString();
            return View();
        }

        /// <summary>
        /// 下载批量导入的Excel模板
        /// </summary>
        /// <returns></returns>
        public ActionResult DownloadBatchImportPointsExcel()
        {
            var excelPath = Server.MapPath("~/Content/downloads/importRealPointsTemplate.xls");
            return File(excelPath, "application/vnd.ms-excel", "导入真实测点模板.xls");
        }


        /// <summary>
        /// 批量测点管理页面
        /// </summary>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult BatchPointManagement()
        {
            var schools = _roomRepos.GetAllSchool();
            IList list = new ArrayList();
            foreach (var item in schools)
            {
                list.Add(new
                {
                    dataID = item.SI_ID,
                    dataValue = item.SI_Name
                });
            }
            ViewBag.schoolList = list;
            var powerList = _powerClassRepos.GetAll();
            ViewBag.powerList = powerList;
            return View();
        }

        /// <summary>
        /// 下载指定RTU下的所有测点信息，excel文件
        /// </summary>
        /// <param name="RTU_No"></param>
        /// <returns></returns>
        public ActionResult DownloadPointsAtRTU(short RTU_No)
        {
            string downloadPath = Server.MapPath(Path.Combine("~/Content/downloads/" + DateTime.Now.Ticks + ".xls"));
            System.IO.File.Copy(Server.MapPath("~/Content/downloads/PointsAtRTUTemplate.xls") , downloadPath);
            GetPointsAtRTUToFile(downloadPath , RTU_No);
            return File(downloadPath, "application/vnd.ms-excel", "RTU" + RTU_No + "测点.xls");
        }

        /// <summary>
        /// 上传批量转移测点Excel文件
        /// </summary>
        /// <param name="uploadExcel"></param>
        /// <returns></returns>
        public ActionResult BatchTransferingPoints(HttpFileCollectionBase uploadExcel)
        {
            IList filePathList = SaveExcelFiles(Request);
            ViewBag.filePath = filePathList[0].ToString();
            return View();
        }

        /// <summary>
        /// 批量转移测点页面
        /// </summary>
        /// <returns></returns>
        [AuthenticationFilter]
        public ActionResult BatchPointTrans()
        {
            var RTUList = _rtuRepos.GetAll().ToList();
            ViewBag.RTUList = RTUList;
            return View();
        }

        [AuthenticationFilter]
        public ActionResult UpdateList()
        {
            var uiList = this.GetUpdateInfo();
            ViewBag.uiList = uiList;
            return View();
        }

        /// <summary>
        /// 缺失能耗填补页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Interpolation()
        {
            return View();
        }

        public ActionResult TimingState()
        {
            return View();
        }
        #endregion



        #region Ajax请求

        /// <summary>
        /// 使用Ajax根据父测点编号获取子测点列表
        /// </summary>
        /// <param name="ParentNo">父测点编号</param>
        /// <param name="Level">父测点层级</param>
        /// <returns>子测点列表</returns>
        public ActionResult GetMeasurePointByParentNoAjax(int ParentNo, int Level, int CptFlag = 0)
        {
            if (Request.IsAjaxRequest())
            {
                IList<AnalogMeasurePoint> measurePoint = _analogMeasurePointRepos.GetMeasurePointByParentNo(ParentNo);
                AnalogMeasurePoint parentPoint = _analogMeasurePointRepos.GetMeasurePointByAnalogNo(ParentNo);
                IList list = this.GetSonInfoListByParentNo(ParentNo, Level, CptFlag);
                return Json(new { ifSucceed = true, data = list }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 使用Ajax修改指定建筑公式
        /// </summary>
        /// <param name="buildingID"></param>
        /// <param name="operateRule"></param>
        /// <returns></returns>
        public ActionResult ModifyBuildingOperateRule(int buildingID, String operateRule)
        {
            if (Request.IsAjaxRequest())
            {
                bool ifSuccess = _buildingRepos.ModifyBuildingOperateRule(buildingID, operateRule);
                return Json(new { ifSuccess = ifSuccess }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }


        /// <summary>
        /// 使用Ajax启用或取消指定建筑公式的使用
        /// </summary>
        /// <param name="buildingID"></param>
        /// <param name="hJFlag"></param>
        /// <returns></returns>
        public ActionResult ModifyBuildingHJFlag(int buildingID, byte hJFlag)
        {
            if (Request.IsAjaxRequest())
            {
                bool ifSuccess = _buildingRepos.ModifyBuildingHJFlag(buildingID, hJFlag);
                return Json(new { ifSuccess = ifSuccess }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 使用Ajax根据测点ID，获取上级所有关联测点
        /// </summary>
        /// <param name="AnalogNo">测点ID</param>
        /// <returns>返回测点列表，按照低级到高级的顺序返回（包含当前测点）</returns>
        public ActionResult GetParentPointByAnalogNo(int AnalogNo)
        {
            if (Request.IsAjaxRequest())
            {
                IList pointList = this.GetParentInfoListByAnalogNo(AnalogNo);
                return Json(new { pointList = pointList }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 使用Ajax获取与测点AnalogNo相关的建筑公式
        /// </summary>
        /// <param name="AnalogNo">测点ID</param>
        /// <returns></returns>
        public ActionResult GetOperateRuleContainsAnalogNo(int AnalogNo)
        {
            if (Request.IsAjaxRequest())
            {
                var bBriefInfo = _buildingRepos.GetBuildingRelateToAnalogNo(AnalogNo);
                IList tempRuleList = this.GetRuleListByBuildingBriefInfoList(bBriefInfo);
                IList ruleList = new ArrayList();
                foreach (var item in tempRuleList)
                {
                    PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(item);
                    PropertyDescriptor pdPointList = pdc.Find("pointList", true);
                    IList pointList = (IList)pdPointList.GetValue(item);
                    foreach (var po in pointList)
                    {
                        PropertyDescriptorCollection ppdc = TypeDescriptor.GetProperties(po);
                        PropertyDescriptor ppdPointNo = ppdc.Find("pointNo", true);
                        int pointNo = Convert.ToInt32(ppdPointNo.GetValue(po));
                        if (pointNo == AnalogNo)
                        {
                            ruleList.Add(item);
                        }
                    }
                }
                return Json(new { ruleList = ruleList }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }


        /// <summary>
        /// 使用Ajax根据文件路径，获取需要批量导入的测点（分页）
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="currentPage"></param>
        /// <param name="totalPages"></param>
        /// <returns></returns>
        public ActionResult GetBatchImportPointsAjax(string filePath, int currentPage, int totalPages)
        {
            if (Request.IsAjaxRequest())
            {
                string fileExtension = Path.GetExtension(filePath);
                DataSet OleDsExcel = new DataSet();
                string strConn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties='Excel 12.0;HDR=YES;IMEX=1';";
                if (fileExtension == ".xls") strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filePath + ";Extended Properties='Excel 8.0;HDR=YES;IMEX=1';";
                OleDbConnection oleConn = new OleDbConnection(strConn);
                try
                {
                    oleConn.Open();
                    String excelSql = "SELECT * FROM  [Sheet1$A1:O] where [ID] is not null";
                    OleDbDataAdapter OleDaExcel = new OleDbDataAdapter(excelSql, oleConn);
                    OleDaExcel.Fill(OleDsExcel, "Sheet1");
                    oleConn.Close();
                    DataTable dataTable = OleDsExcel.Tables[0];

                    Pager pager = null;
                    if (totalPages == -1)
                    {
                        int totalRows = dataTable.Rows.Count;
                        pager = new Pager(1, totalRows);
                    }
                    else
                    {
                        pager = new Pager(currentPage, totalPages, false);
                    }
                    IList list = new ArrayList();
                    for (int row = 0; row < pager.PageSize && row + pager.PageSize * (currentPage - 1) < dataTable.Rows.Count; row++)
                    {
                        int r = row + pager.PageSize * (currentPage - 1);
                        if (dataTable.Rows[r][0] == null || dataTable.Rows[r][0].ToString().Length == 0) break;
                        int ID = Convert.ToInt32(dataTable.Rows[r][0].ToString());
                        int RTU_No = Convert.ToInt32(dataTable.Rows[r][1].ToString());
                        int AI_Serial = Convert.ToInt32(dataTable.Rows[r][2].ToString());
                        string AI_Name = dataTable.Rows[r][3].ToString();
                        double AI_Base = Convert.ToDouble(dataTable.Rows[r][5].ToString());
                        int AMP_SchoolID = Convert.ToInt32(dataTable.Rows[r][6].ToString());
                        int AMP_SAreaID = Convert.ToInt32(dataTable.Rows[r][7].ToString());
                        int AMP_BuildingID = Convert.ToInt32(dataTable.Rows[r][8].ToString());
                        int AMP_RoomID = Convert.ToInt32(dataTable.Rows[r][9].ToString());
                        string AMP_PowerType = dataTable.Rows[r][11].ToString();
                        int AMP_ParentNo = Convert.ToInt32(dataTable.Rows[r][12].ToString());
                        string AMP_ParentName = dataTable.Rows[r][13].ToString();
                        int AMP_ParentFlag = Convert.ToInt32(dataTable.Rows[r][14].ToString());

                        string theObject = GetObject(AMP_SchoolID, AMP_SAreaID, AMP_BuildingID, AMP_RoomID);

                        list.Add(new
                        {
                            ID = ID,
                            RTU_No = RTU_No,
                            AI_Serial = AI_Serial,
                            AI_Name = AI_Name,
                            AI_Base = AI_Base,
                            theObject = theObject,
                            powerType = AMP_PowerType,
                            parentNo = AMP_ParentNo,
                            parentName = AMP_ParentName,
                            parentFlag = AMP_ParentFlag
                        });
                    }
                    var resultData = new
                    {
                        totalPages = pager.TotalPages,
                        data = list
                    };
                    return Json(new { ifSuccess = true, resultData = resultData }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception e)
                {
                    return Json(new { ifSuccess = false, message = e.Message }, JsonRequestBehavior.AllowGet);
                }
            }
            return null;
        }

        /// <summary>
        /// 使用Ajax选择或取消父测点
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="ID"></param>
        /// <param name="Flag"></param>
        /// <returns></returns>
        public ActionResult ChooseOrCancelParentPoint(string filePath , int ID , int Flag)
        {
            if (Request.IsAjaxRequest())
            {
                try
                {
                    string fileExtension = Path.GetExtension(filePath);
                    DataSet OleDsExcel = new DataSet();
                    string strConn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties='Excel 12.0;HDR=YES;IMEX=0';";
                    if (fileExtension == ".xls") strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filePath + ";Extended Properties='Excel 8.0;HDR=YES;IMEX=0';";
                    OleDbConnection oleConn = new OleDbConnection(strConn);
                    oleConn.Open();
                    String excelSql = "SELECT * FROM  [Sheet1$A1:O] where [ID] is not null";
                    OleDbDataAdapter OleDaExcel = new OleDbDataAdapter(excelSql, oleConn);
                    OleDbCommandBuilder odcb = new OleDbCommandBuilder(OleDaExcel);
                    OleDaExcel.Fill(OleDsExcel);
                    DataTable dataTable = OleDsExcel.Tables[0];
                    dataTable.PrimaryKey = new DataColumn[] { dataTable.Columns["ID"] };//创建索引列
                    string sql = string.Format("Update [Sheet1$] set [AMP_ParentNo_Flag]='{0}' where [ID]='{1}'", Flag.ToString(), ID.ToString());
                    OleDbCommand myCommand = new OleDbCommand(sql, oleConn);
                    myCommand.ExecuteNonQuery();
                    oleConn.Close();
                    return Json(new { ifSuccess = true}, JsonRequestBehavior.AllowGet);
                }
                catch (Exception e)
                {
                    return Json(new { ifSuccess = false , message = e.Message}, JsonRequestBehavior.AllowGet);
                }
            }
            return null;
        }

        /// <summary>
        /// 使用Ajax将指定文件测点批量导入到数据库
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>是否导入成功</returns>
        public ActionResult BatchAddRealPoints(string filePath)
        {
            if (Request.IsAjaxRequest())
            {
                try
                {
                    string fileExtension = Path.GetExtension(filePath);
                    DataSet OleDsExcel = new DataSet();
                    string strConn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties='Excel 12.0;HDR=YES;IMEX=1';";
                    if (fileExtension == ".xls") strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filePath + ";Extended Properties='Excel 8.0;HDR=YES;IMEX=1';";
                    OleDbConnection oleConn = new OleDbConnection(strConn);
                    oleConn.Open();
                    String excelSql = "SELECT * FROM  [Sheet1$A1:O] where [ID] is not null";
                    OleDbDataAdapter OleDaExcel = new OleDbDataAdapter(excelSql, oleConn);
                    OleDbCommandBuilder odcb = new OleDbCommandBuilder(OleDaExcel);
                    OleDaExcel.Fill(OleDsExcel);
                    DataTable dataTable = OleDsExcel.Tables[0];
                    oleConn.Close();

                    int AI_No = _analogInfoRepos.GetNextAnalogNo();
                    IList<AnalogInfo> aiList = new List<AnalogInfo>();
                    IList<AnalogMeasurePoint> ampList = new List<AnalogMeasurePoint>();
                    for (int row = 0; row < dataTable.Rows.Count; row++)
                    {
                        short RTU_No = Convert.ToInt16(dataTable.Rows[row][1].ToString());
                        int AI_Serial = Convert.ToInt32(dataTable.Rows[row][2].ToString());
                        string AI_Name = dataTable.Rows[row][3].ToString();
                        string AI_Unit = dataTable.Rows[row][4].ToString();
                        double AI_Base = Convert.ToDouble(dataTable.Rows[row][5].ToString());
                        int AMP_SchoolID = Convert.ToInt32(dataTable.Rows[row][6].ToString());
                        int AMP_SAreaID = Convert.ToInt32(dataTable.Rows[row][7].ToString());
                        int AMP_BuildingID = Convert.ToInt32(dataTable.Rows[row][8].ToString());
                        int AMP_RoomID = Convert.ToInt32(dataTable.Rows[row][9].ToString());
                        string AMP_PowerType = dataTable.Rows[row][10].ToString();
                        string AMP_PowerName = dataTable.Rows[row][11].ToString();
                        int AMP_ParentNo = Convert.ToInt32(dataTable.Rows[row][12].ToString());
                        int AMP_Parent_Flag = Convert.ToInt32(dataTable.Rows[row][14].ToString());
                        if (AMP_Parent_Flag == 0) AMP_ParentNo = 0;

                        AnalogInfo ai = new AnalogInfo();
                        ai.AI_No = AI_No;
                        ai.RTU_No = RTU_No;
                        ai.AI_Serial = AI_Serial;
                        ai.AI_Name = AI_Name;
                        ai.AI_Unit = AI_Unit;
                        ai.AI_Base = AI_Base;
                        ai.AI_LogicalLow = 0;
                        ai.AI_LogicalUp = 100000;
                        ai.AI_Decimal = 2;
                        ai.AI_Cptflag = 0;
                        ai.AI_Rate = 1;
                        ai.AI_LockVal = 0;
                        ai.AI_LockFlag = 0;
                        ai.AI_Timespace = 5;
                        ai.AI_State = 1;
                        ai.AI_Level = 0;
                        ai.AI_Type = 0;

                        AnalogMeasurePoint amp = new AnalogMeasurePoint();
                        amp.AMP_AnalogNo = AI_No;
                        amp.AMP_Name = AI_Name;
                        amp.AMP_CptFlag = 1;
                        amp.AMP_Statistic = 1;
                        amp.AMP_ValRem = 0;
                        amp.AMP_Unit = AI_Unit;
                        amp.AMP_SchooldID = AMP_SchoolID;
                        amp.AMP_SAreaID = AMP_SAreaID;
                        amp.AMP_BuildingID = AMP_BuildingID;
                        amp.AMP_RoomID = AMP_RoomID;
                        amp.AMP_PowerType = AMP_PowerType;
                        amp.AMP_PowerName = AMP_PowerName;
                        amp.AMP_Timespan = 5;
                        amp.AMP_ParentNo = AMP_ParentNo;
                        amp.AMP_OperationRule = "1";
                        amp.AMP_OperationParameter = 1;
                        amp.AMP_State = true;
                        amp.AMP_Date = DateTime.Now;

                        AI_No++;
                        aiList.Add(ai);
                        ampList.Add(amp);
                    }
                    bool ifSuccess = _transactionRepos.BatchAddRealPoints(aiList, ampList);
                    return Json(new { ifSuccess = ifSuccess }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception e)
                {
                    return Json(new { ifSuccess = false, message = e.Message }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    System.IO.File.Delete(filePath);
                }
            }
            return null;
        }

        /// <summary>
        /// 使用Ajax获取所有建筑公式
        /// </summary>
        /// <returns></returns>
        public ActionResult GetBuildingRule()
        {
            if (Request.IsAjaxRequest())
            {
                //获取建筑列表
                var bBriefInfo = _buildingRepos.GetBuildingHasOperateRule();
                IList ruleList = GetRuleListByBuildingBriefInfoList(bBriefInfo);//保存所有公式和公式中测点的信息
                return Json(new { ruleList = ruleList }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 使用Ajax转移指定文件中的测点
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public ActionResult BatchTransferPointsAjax(string filePath)
        {
            if (Request.IsAjaxRequest())
            {
                try
                {
                    string fileExtension = Path.GetExtension(filePath);
                    DataSet OleDsExcel = new DataSet();
                    string strConn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties='Excel 12.0;HDR=YES;IMEX=1';";
                    if (fileExtension == ".xls") strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filePath + ";Extended Properties='Excel 8.0;HDR=YES;IMEX=1';";
                    OleDbConnection oleConn = new OleDbConnection(strConn);
                    oleConn.Open();
                    String excelSql = "SELECT * FROM  [Sheet1$] where [AI_No] is not null";
                    OleDbDataAdapter OleDaExcel = new OleDbDataAdapter(excelSql, oleConn);
                    OleDbCommandBuilder odcb = new OleDbCommandBuilder(OleDaExcel);
                    OleDaExcel.Fill(OleDsExcel);
                    DataTable dataTable = OleDsExcel.Tables[0];
                    oleConn.Close();

                    IList<AnalogInfo> aiList = new List<AnalogInfo>();
                    IList<AnalogMeasurePoint> ampList = new List<AnalogMeasurePoint>();
                    for (int row = 0; row < dataTable.Rows.Count; row++)
                    {
                        if (dataTable.Rows[row]["NewRTU"] == null || dataTable.Rows[row]["NewSerial"] == null || dataTable.Rows[row]["NewName"] == null) continue;
                        int AI_No = Convert.ToInt32(dataTable.Rows[row]["AI_No"].ToString());
                        short RTU_No = Convert.ToInt16(dataTable.Rows[row]["NewRTU"].ToString());
                        int AI_Serial = Convert.ToInt32(dataTable.Rows[row]["NewSerial"].ToString());
                        string AI_Name = dataTable.Rows[row]["NewName"].ToString();
                        AnalogInfo ai = new AnalogInfo();
                        ai.AI_No = AI_No;
                        ai.RTU_No = RTU_No;
                        ai.AI_Serial = AI_Serial;
                        ai.AI_Name = AI_Name;

                        aiList.Add(ai);
                    }
                    bool ifSuccess = _analogInfoRepos.BatchModifyAi(aiList);
                    return Json(new { ifSuccess = ifSuccess }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception e)
                {
                    return Json(new { ifSuccess = false, message = e.Message }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    System.IO.File.Delete(filePath);
                }
            }
            return null;
        }

        /// <summary>
        /// 使用Ajax根据文件名获取要转移的测点，分页
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="currentPage"></param>
        /// <param name="totalPages"></param>
        /// <returns></returns>
        public ActionResult GetBatchTransferPointsAjax(string filePath, int currentPage, int totalPages)
        {
            if (Request.IsAjaxRequest())
            {
                try
                {
                    string fileExtension = Path.GetExtension(filePath);
                    DataSet OleDsExcel = new DataSet();
                    string strConn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties='Excel 12.0;HDR=YES;IMEX=1';";
                    if (fileExtension == ".xls") strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filePath + ";Extended Properties='Excel 8.0;HDR=YES;IMEX=1';";
                    OleDbConnection oleConn = new OleDbConnection(strConn);
                    oleConn.Open();
                    String excelSql = "SELECT * FROM  [Sheet1$] where [AI_No] is not null";
                    OleDbDataAdapter OleDaExcel = new OleDbDataAdapter(excelSql, oleConn);
                    OleDbCommandBuilder odcb = new OleDbCommandBuilder(OleDaExcel);
                    OleDaExcel.Fill(OleDsExcel);
                    DataTable dataTable = OleDsExcel.Tables[0];
                    oleConn.Close();

                    Pager pager = null;
                    if (totalPages == -1)
                    {
                        int totalRows = dataTable.Rows.Count;
                        pager = new Pager(1, totalRows);
                    }
                    else
                    {
                        pager = new Pager(currentPage, totalPages, false);
                    }

                    IList list = new ArrayList();
                    for (int row = pager.PageSize*(currentPage-1); row < pager.PageSize*currentPage && row < dataTable.Rows.Count; row++)
                    {
                        list.Add(new { 
                            AI_No = dataTable.Rows[row]["AI_No"].ToString(),
                            OldRTU = dataTable.Rows[row]["OldRTU"].ToString(),
                            OldSerial = dataTable.Rows[row]["OldSerial"].ToString(),
                            OldName = dataTable.Rows[row]["OldName"].ToString(),
                            NewRTU = dataTable.Rows[row]["NewRTU"].ToString(),
                            NewSerial = dataTable.Rows[row]["NewSerial"].ToString(),
                            NewName = dataTable.Rows[row]["NewName"].ToString()
                        });
                    }
                    var resultData = new
                    {
                        totalPages = pager.TotalPages,
                        data = list
                    };
                    return Json(new { ifSuccess = true, resultData = resultData }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception e)
                {
                    return Json(new { ifSuccess = false, message = e.Message }, JsonRequestBehavior.AllowGet);
                }
            }
            return null;
        }

        /// <summary>
        /// 使用Ajax更新指定测点某时间区间内历史数据
        /// </summary>
        /// <param name="AI_No"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public ActionResult UpdateDataByAINo(int ID , int AI_No , DateTime startTime , DateTime endTime)
        {
            if(Request.IsAjaxRequest())
            {
                try
                {
                    AnalogMeasurePoint amp = _analogMeasurePointRepos.GetMeasurePointByAnalogNo(AI_No);
                    int level = this.GetMeasurePointLevel(amp);
                    IList list = this.GetSonInfoListByParentNo(AI_No, level, 0);
                    IList<AnalogMeasurePoint> sonPointList = new List<AnalogMeasurePoint>();
                    foreach (var item in list)
                    {
                        PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(item);
                        PropertyDescriptor pdPointID = pdc.Find("pointID", true);
                        int pointID = Convert.ToInt32(pdPointID.GetValue(item));
                        PropertyDescriptor pdPointIsCount = pdc.Find("pointIsCount", true);
                        int pointIsCount = Convert.ToInt32(pdPointIsCount.GetValue(item));
                        if (pointIsCount == 1)
                        {
                            sonPointList.Add(_analogMeasurePointRepos.GetMeasurePointByAnalogNo(pointID));
                        }
                    }
                    bool ifSuccess = _analogHistoryRepos.UpdateHistoryValOfAnalogNo(AI_No, startTime, endTime, sonPointList);
                    if (ifSuccess == true) _updateInfoRepos.ModifyUpdateInfoOfState(ID, true);
                    return Json(new { ifSuccess = ifSuccess }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception e)
                {
                    return Json(new { ifSuccess = false, message = e.Message }, JsonRequestBehavior.AllowGet);
                }
            }
            return null;
        }
        #endregion



        #region 工具方法

        /// <summary>
        /// 分离BuildingBriefInfo数组中的公式，然后返回公式信息
        /// </summary>
        /// <param name="bBriefInfo"></param>
        /// <returns></returns>
        IList GetRuleListByBuildingBriefInfoList(IList<BuildingBriefInfo> bBriefInfo)
        {
            IList ruleList = new ArrayList();//保存所有公式和公式中测点的信息
            foreach (var item in bBriefInfo)//遍历所有公式
            {
                IList pointList = new ArrayList();//保存测点的信息
                string ruleStr = "";//保存去掉|和_的公式字符串
                string operateRule = item.BDI_OperateRule;
                string[] pointStr = operateRule.Split('|');
                foreach (var str in pointStr)//遍历公式中所有测点
                {
                    string[] IDOpt = str.Split('_');
                    ruleStr += IDOpt[0] + IDOpt[1];
                    AnalogMeasurePoint amp = _analogMeasurePointRepos.GetMeasurePointByAnalogNo(Convert.ToInt32(IDOpt[1]));
                    if (amp == null) continue;
                    pointList.Add(new
                    {
                        Opt = IDOpt[0],
                        pointNo = amp.AMP_AnalogNo,
                        pointName = amp.AMP_Name
                    });
                }
                ruleList.Add(new
                {
                    buildingID = item.BDI_ID,
                    buildingName = item.BDI_Name,
                    OperateRule = ruleStr,
                    HJFlag = item.BDI_HJFlag,//是否启用公式
                    pointList = pointList
                });
            }
            return ruleList;
        }



        /// <summary>
        /// 根据测点所属对象，返回测点层级
        /// 0：不属于任何校区
        /// 1：校区级
        /// 2：区域级
        /// 3：楼宇级
        /// 4：楼宇以下
        /// </summary>
        /// <param name="amp">测点信息</param>
        /// <returns>测点层级</returns>
        int GetMeasurePointLevel(AnalogMeasurePoint amp)
        {
            if (amp.AMP_SchooldID == 0) return 0;
            else if (amp.AMP_SAreaID == 0) return 1;
            else if (amp.AMP_BuildingID == 0) return 2;
            else if (amp.AMP_RoomID == 0) return 3;
            else return 4;
        }

        /// <summary>
        /// 根据测点ID，获取上级所有关联测点
        /// </summary>
        /// <param name="AnalogNo">测点ID</param>
        /// <returns>返回测点列表，按照低级到高级的顺序返回（包含当前测点）</returns>
        IList GetParentInfoListByAnalogNo(int AnalogNo)
        {
            IList pointList = new ArrayList();
            AnalogMeasurePoint amp = _analogMeasurePointRepos.GetMeasurePointByAnalogNo(AnalogNo);
            int level = this.GetMeasurePointLevel(amp);
            int ParentNo = Convert.ToInt32(amp.AMP_ParentNo);
            while (ParentNo != 0)
            {
                //获取父测点信息
                AnalogMeasurePoint parentAmp = _analogMeasurePointRepos.GetMeasurePointByAnalogNo(ParentNo);
                int pLevel = this.GetMeasurePointLevel(parentAmp);
                //判断能耗值是否加到父测点
                int pointIsCount = 1;
                if (amp.AMP_Statistic == 0 || parentAmp.AMP_Statistic == 0) pointIsCount = 0;
                if (level - pLevel != 1 && ParentNo != 200580 && ParentNo != 200579 && ParentNo != 200124 && ParentNo != 200148) pointIsCount = 0;
                if (parentAmp.AMP_CptFlag == 1) pointIsCount = 0;
                //将当前测点放入list
                pointList.Add(new
                {
                    pointID = amp.AMP_AnalogNo,
                    pointName = amp.AMP_Name,
                    pointCptFlag = amp.AMP_CptFlag,
                    pointParentID = ParentNo,
                    pointLevel = level,//测点层级
                    pointIsCount = pointIsCount//该测点能耗是否被加到父测点
                });

                ParentNo = Convert.ToInt32(parentAmp.AMP_ParentNo);
                level = pLevel;
                amp = parentAmp;
            }
            pointList.Add(new
            {
                pointID = amp.AMP_AnalogNo,
                pointName = amp.AMP_Name,
                pointCptFlag = amp.AMP_CptFlag,
                pointParentID = 0,
                pointLevel = level,//测点层级
                pointIsCount = 0//该测点能耗是否被加到父测点
            });
            return pointList;
        }

        /// <summary>
        /// 使用Ajax根据父测点编号获取子测点列表
        /// </summary>
        /// <param name="ParentNo">父测点编号</param>
        /// <param name="Level">父测点层级</param>
        /// <returns>子测点列表</returns>
        IList GetSonInfoListByParentNo(int ParentNo , int Level , int CptFlag)
        {
            IList<AnalogMeasurePoint> measurePoint = _analogMeasurePointRepos.GetMeasurePointByParentNo(ParentNo);
            AnalogMeasurePoint parentPoint = _analogMeasurePointRepos.GetMeasurePointByAnalogNo(ParentNo);
            IList list = new ArrayList();
            foreach (var item in measurePoint)
            {
                int pLevel = 0;
                pLevel = GetMeasurePointLevel(item);

                if (ParentNo == 0)//如果父测点为0，那么只显示下一层级测点
                {
                    if (pLevel - Level != 1) continue;
                    list.Add(new
                    {
                        pointID = item.AMP_AnalogNo,
                        pointName = item.AMP_Name,
                        pointCptFlag = item.AMP_CptFlag,
                        pointLevel = pLevel,//测点层级
                        pointIsCount = 0//该测点能耗是否被加到父测点
                    });
                }
                else
                {
                    //判断测点能耗是否被加到父测点，必须满足三个条件1：父子均为是统计点，2：是父测点下一层级的测点（父测点是200580、200579、200124、200148的除外），3：父测点为虚拟点
                    int pIsCount = 1;
                    if (item.AMP_Statistic == 0 || parentPoint.AMP_Statistic == 0) pIsCount = 0;
                    if (pLevel - Level != 1 && ParentNo != 200580 && ParentNo != 200579 && ParentNo != 200124 && ParentNo != 200148) pIsCount = 0;
                    if (CptFlag == 1) pIsCount = 0;

                    list.Add(new
                    {
                        pointID = item.AMP_AnalogNo,
                        pointName = item.AMP_Name,
                        pointCptFlag = item.AMP_CptFlag,
                        pointLevel = pLevel,//测点层级
                        pointIsCount = pIsCount//该测点能耗是否被加到父测点
                    });
                }
            }
            if (ParentNo == 0)//如果父测点为0，添加哨兵测点
            {
                list.Add(new
                {
                    pointID = 0,
                    pointName = "其他",
                    pointCptFlag = 0,
                    pointLevel = Level + 1,//测点层级
                    pointIsCount = 0//该测点能耗是否被加到父测点
                });
            }
            return list;
        }
        /// <summary>
        /// 保存上传文件
        /// </summary>
        /// <param name="Request"></param>
        /// <returns>文件路径list</returns>
        IList SaveExcelFiles(HttpRequestBase Request)
        {
            IList filePathList = new ArrayList();
            foreach (string upload in Request.Files)
            {
                if (!Request.Files[upload].HasFile())
                {
                    continue;
                }
                string fileExtension = Path.GetExtension(Request.Files[upload].FileName);
                // 判断是否上传的后缀是否存在
                string uploadExcelExtens = Util.GetConfigValue("uploadExcelExtens");
                string[] uploadExcelExtensions = uploadExcelExtens.Split(new char[] { ',' });
                string preExtension = "";
                if (fileExtension.Length > 0)
                {
                    preExtension = fileExtension.Substring(1);
                }
                if (!uploadExcelExtensions.Contains(preExtension))
                {
                    continue;
                }
                string tempFileName = DateTime.Now.Ticks + fileExtension;
                string uploadsPath = Server.MapPath("~/Content/uploads/");
                string fullTempFilePath = Path.Combine(uploadsPath, tempFileName);
                // 保存为临时的Excel文件
                Request.Files[upload].SaveAs(fullTempFilePath);
                filePathList.Add(fullTempFilePath);
            }
            return filePathList;
        }

        /// <summary>
        /// 获取批量导入真实测点的父测点
        /// </summary>
        /// <param name="filePath"></param>
        void GetParentPointOfImportPoints(string filePath)
        {
            string fileExtension = Path.GetExtension(filePath);
            DataSet OleDsExcel = new DataSet();
            string strConn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties='Excel 12.0;HDR=YES;IMEX=0';";
            if (fileExtension == ".xls") strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filePath + ";Extended Properties='Excel 8.0;HDR=YES;IMEX=0';";
            OleDbConnection oleConn = new OleDbConnection(strConn);
            oleConn.Open();
            String excelSql = "SELECT * FROM  [Sheet1$A1:O] where [ID] is not null";
            OleDbDataAdapter OleDaExcel = new OleDbDataAdapter(excelSql, oleConn);
            OleDbCommandBuilder odcb = new OleDbCommandBuilder(OleDaExcel);
            OleDaExcel.Fill(OleDsExcel);



            DataTable dataTable = OleDsExcel.Tables[0];
            dataTable.PrimaryKey = new DataColumn[] { dataTable.Columns["ID"] };//创建索引列
            for (int row = 0; row < dataTable.Rows.Count; row++)
            {
                if (dataTable.Rows[row][0] == null || dataTable.Rows[row][0].ToString().Length == 0) break;
                string AMP_PowerType = dataTable.Rows[row][10].ToString();
                int AMP_SchoolID = Convert.ToInt32(dataTable.Rows[row][6].ToString());
                int AMP_SAreaID = Convert.ToInt32(dataTable.Rows[row][7].ToString());
                int AMP_BuildingID = Convert.ToInt32(dataTable.Rows[row][8].ToString());
                int AMP_RoomID = Convert.ToInt32(dataTable.Rows[row][9].ToString());
                IList parentPoints = QueryParentAMPForServer(AMP_SchoolID, AMP_SAreaID, AMP_BuildingID, AMP_RoomID, AMP_PowerType);

                if (parentPoints.Count == 0) continue;

                AMPExtEntity parentPoint = (AMPExtEntity)parentPoints[0];

                string sql = string.Format("Update [Sheet1$] set [AMP_ParentNo]='{0}',[AMP_ParentName]='{1}' where [ID]='{2}'", parentPoint.PNO.ToString(), parentPoint.PName, dataTable.Rows[row][0].ToString());
                OleDbCommand myCommand = new OleDbCommand(sql, oleConn);
                myCommand.ExecuteNonQuery();

            }
            //OleDaExcel.Update(dataTable);
            oleConn.Close();
        }

        /// <summary>
        /// 查询父测点编号，提供服务器端调用
        /// </summary>
        /// <param name="schoolID">校区Id</param>
        /// <param name="areaID">区域Id</param>
        /// <param name="buildingID">建筑Id</param>
        /// <param name="roomID">房间Id</param>
        /// <param name="powerId">能耗类型</param>
        /// <returns></returns>
        IList QueryParentAMPForServer(int? schoolID, int? areaID, int? buildingID, int? roomID, string powerId)
        {
            IList parentPoins = new ArrayList();
            var query = _ampRepos.GetAllAMP();
            if (roomID.HasValue && roomID.Value > 0)
            {
                query = query.Where(x => x.RealFlag == 0 && x.StatFlag == 1 && x.PowerType == powerId && x.BuildingID == buildingID.Value && x.RoomID == 0);
            }
            else if (buildingID.HasValue && buildingID.Value > 0)
            {
                query = query.Where(x => x.RealFlag == 0 && x.StatFlag == 1 && x.PowerType == powerId && x.AreaID == areaID.Value && x.BuildingID == 0);
            }
            else if (areaID.HasValue && areaID.Value > 0)
            {
                query = query.Where(x => x.RealFlag == 0 && x.StatFlag == 1 && x.PowerType == powerId && x.SchoolID == schoolID.Value && x.AreaID == 0);
            }
            else
            {
                return parentPoins;
            }
            parentPoins = query.ToList();
            return parentPoins;
        }

        /// <summary>
        /// 根据校区ID、区域ID、建筑ID、房间ID，返回所属对象
        /// </summary>
        /// <param name="schoolID"></param>
        /// <param name="areaID"></param>
        /// <param name="buildingID"></param>
        /// <param name="roomID"></param>
        /// <returns></returns>
        string GetObject(int schoolID, int areaID, int buildingID, int roomID)
        {
            string theObject = "";
            if (schoolID > 0)
            {
                SchoolInfo schoolInfo = _schoolRepos.QuerySchool(schoolID);
                theObject += schoolInfo.SI_Name;
            }

            if (areaID > 0)
            {
                SchoolAreaInfo schoolAreaInfo = _schoolAreaRepos.GetSchoolArea(areaID);
                theObject += ">" + schoolAreaInfo.SAI_Name;
            }

            if (buildingID > 0)
            {
                BuildingBriefInfo bBriefInfo = _buildingRepos.GetBuildingByBuildingID(buildingID);
                theObject += ">" + bBriefInfo.BDI_Name;
            }

            if (roomID > 0)
            {
                RoomInfo roomInfo = _roomRepos.GetRoomByRoomID(roomID);
                theObject += ">" + roomInfo.RI_RoomCode;
            }

            return theObject;

        }

        /// <summary>
        /// 将指定RTU下的测点数据导出到Excel文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="RTU_No"></param>
        void GetPointsAtRTUToFile(string filePath , short RTU_No)
        {
            IList aiList = _analogInfoRepos.GetAnalogInfoByRTU_No(RTU_No);
            string fileExtension = Path.GetExtension(filePath);
            DataSet OleDsExcel = new DataSet();
            string strConn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties='Excel 12.0;HDR=YES;IMEX=0';";
            if (fileExtension == ".xls") strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filePath + ";Extended Properties='Excel 8.0;HDR=YES;IMEX=0';";
            OleDbConnection oleConn = new OleDbConnection(strConn);
            oleConn.Open();
            for(int i = 0; i < aiList.Count; i++)
            {
                AnalogInfo ai = (AnalogInfo)aiList[i];

                string sql = string.Format("insert into [Sheet1$] values('{0}' , '{1}' , '{2}' , '{3}' , '' , '' , '')" , ai.AI_No.ToString() , ai.RTU_No.ToString() , ai.AI_Serial.ToString() , ai.AI_Name.ToString());
                OleDbCommand myCommand = new OleDbCommand(sql, oleConn);
                myCommand.ExecuteNonQuery();
            }
            oleConn.Close();
        }

        /// <summary>
        /// 把与AnalogNo关联的父测点插入更新列表中，不包括AnalogNo本身
        /// </summary>
        /// <param name="AnalogNo"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        void AddUpdateInfo(int AnalogNo, DateTime startTime, DateTime endTime)
        {
            IList pointList = this.GetParentInfoListByAnalogNo(AnalogNo);
            foreach (var item in pointList)
            {
                PropertyDescriptorCollection ppdc = TypeDescriptor.GetProperties(item);
                PropertyDescriptor ppdPointNo = ppdc.Find("pointID", true);
                int pointNo = Convert.ToInt32(ppdPointNo.GetValue(item));
                PropertyDescriptor ppdPointIsCount = ppdc.Find("pointIsCount", true);
                int pointIsCount = Convert.ToInt32(ppdPointIsCount.GetValue(item));
                PropertyDescriptor ppdPointParentID = ppdc.Find("pointParentID", true);
                int pointParentID = Convert.ToInt32(ppdPointParentID.GetValue(item));
                if (pointNo != AnalogNo)//pointList中第一个为当前测点，需要将其排除
                {
                    IList<UpdateInfo> uiList = _updateInfoRepos.GetUpdateInfoByAINo(pointNo);
                    bool isUnion = false;
                    //查找该测点已存在的更新信息，如果两个更新信息的时间段存在重合，则结合两个更新信息
                    foreach (UpdateInfo ui in uiList)
                    {
                        if (ui.State == false && !(ui.Start_Date > endTime || ui.End_Date < startTime))
                        {
                            ui.Start_Date = ui.Start_Date < startTime ? ui.Start_Date : startTime;
                            ui.End_Date = ui.End_Date > endTime ? ui.End_Date : endTime;
                            _updateInfoRepos.ModifyUpdateInfoOfTime(ui);
                            isUnion = true;
                            break;
                        }
                    }
                    if (isUnion == false)
                    {
                        UpdateInfo ui = new UpdateInfo();
                        AnalogMeasurePoint amp = _analogMeasurePointRepos.GetMeasurePointByAnalogNo(pointNo);
                        ui.AI_No = pointNo;
                        ui.AI_Name = amp.AMP_Name;
                        ui.Start_Date = startTime;
                        ui.End_Date = endTime;
                        ui.Parent_No = pointParentID;
                        ui.State = false;
                        _updateInfoRepos.InsertUpdateInfo(ui);
                    }
                }
                if (pointIsCount == 0) break;//如果当前测点的能耗值不加到父测点，则不将父测点添加到更新列表
            }

        }

        /// <summary>
        /// 获取所有更新列表
        /// </summary>
        /// <returns></returns>
        IList GetUpdateInfo()
        {
            IList list = new ArrayList();
            IList<UpdateInfo> uiList = _updateInfoRepos.GetAllUpdateInfo();            
            foreach (UpdateInfo ui in uiList)
            {
                int state = 1;
                string relatedIDs = "";
                if (ui.State == false)
                {
                    IList<UpdateInfo> relatedUiList = _updateInfoRepos.GetRelatedUpdateInfoByUi(ui);
                    state = 0;
                    foreach (UpdateInfo uii in relatedUiList)
                    {
                        relatedIDs += "[" + uii.ID + "]";
                        state = 2;
                    }
                }
                list.Add(new { 
                    ID = ui.ID,
                    AI_No = ui.AI_No,
                    AI_Name = ui.AI_Name,
                    startTime = ui.Start_Date,
                    endTime = ui.End_Date,
                    relatedIDs = relatedIDs,
                    parentNo = ui.Parent_No,
                    state = state
                });
            }
            return list;
        }
        #endregion
        

        

        

    }
}
