using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Collections;
using EnergyMonitor.Controllers.Utils;
using System.Linq.Dynamic;
using EnergyMonitor.Controllers.Admin.Filters;
using EnergyMonitor.Models.Repository.Interface;
using EnergyMonitor.Models.Repository.Implement;
using EnergyMonitor.Models.LinqEntity;
using EnergyMonitor.Models.Repository.Entity;

namespace EnergyMonitor.Controllers.Admin
{
    /// <summary>
    /// 工具
    /// </summary>
    public class UtilController : Controller
    {
        private IPowerClassRepos _powerClassRepos = null;
        private IRoomRepos _roomRepos = null;
        private IAMPRepos _ampRepos = null;
        private IElecDistributionInfoRepos _elecDistributionInfoRepos = null;
        private IElecTSInfoRepos _elecTSInfoRepos = null;
        private IBuildingRepos _buildingRepos = null;
        private IBuildingGuanghuaRepos _buildingGuanghuaRepos = null;
        private IBuildingTypesRepos _buildingTypesRepos = null;
        public UtilController()
            : this(new PowerClassRepos(), new RoomRepos(), new AMPRepos(),new ElecDistributionInfoRepos(),new ElecTSInfoRepos(),new BuildingRepos(),new BuildingGuanghuaRepos())
        {
        }

        public UtilController(IPowerClassRepos powerClassRepos, IRoomRepos roomRepos, IAMPRepos ampRepos,IElecDistributionInfoRepos elecDistributionInfoRepos,IElecTSInfoRepos elecTSInfoRepos,IBuildingRepos buildingRepos,IBuildingGuanghuaRepos buildingGuanghuaRepos)
        {
            _powerClassRepos = powerClassRepos;
            _roomRepos = roomRepos;
            _ampRepos = ampRepos;
            _elecDistributionInfoRepos = elecDistributionInfoRepos;
            _elecTSInfoRepos = elecTSInfoRepos;
            _buildingRepos = buildingRepos;
            _buildingGuanghuaRepos = buildingGuanghuaRepos;           
        }

        /// <summary>
        /// 使用Ajax得到校区信息
        /// </summary>
        /// <returns>Json格式所有校区信息</returns>
        [OutputCache(Duration = 3600, VaryByParam = "none")]
        public ActionResult GetAllShoolAjax()
        {
            if (Request.IsAjaxRequest())
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
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 使用Ajax得到校区信息（专门提供给其他应用调用）
        /// </summary>
        /// <returns>Json格式所有校区信息</returns>
        public ActionResult GetAllShoolAjaxForMobile()
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
            return Json(list, JsonRequestBehavior.AllowGet);
        }       

        /// <summary>
        /// 使用Ajax得到光华楼信息
        /// </summary>
        /// <returns>Json格式所有校区信息</returns>     
        public ActionResult GetGuanghuaBuildingAjax()
        {
            if (Request.IsAjaxRequest())
            {
                var buildingGuanghua = _buildingRepos.GetGuanghuaBuilding();
                IList list = new ArrayList();
                foreach (var item in buildingGuanghua)
                {
                    list.Add(new
                    {
                        dataID = item.BDI_ID,
                        dataValue = item.BDI_Name
                    });
                }
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 使用Ajax根据校区ID得到区域
        /// </summary>
        /// <param name="schoolID">校区ID</param>
        /// <returns>Json格式区域集合</returns>        
        public ActionResult GetAreasBySchoolIDAjax(int schoolID)
        {
            if (Request.IsAjaxRequest() && schoolID > 0)
            {
                var areas = _roomRepos.GetAreaBySchoolID(schoolID);
                IList list = new ArrayList();
                foreach (var item in areas)
                {
                    list.Add(new
                    {
                        dataID = item.SAI_ID,
                        dataValue = item.SAI_Name
                    });
                }
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 使用Ajax根据校区ID得到区域（专门提供给其他应用调用）
        /// </summary>
        /// <param name="schoolID">校区ID</param>
        /// <returns>Json格式区域集合</returns>        
        public ActionResult GetAreasBySchoolIDAjaxForMobile(int schoolID)
        {
            var areas = _roomRepos.GetAreaBySchoolID(schoolID);
            IList list = new ArrayList();
            foreach (var item in areas)
            {
                list.Add(new
                {
                    dataID = item.SAI_ID,
                    dataValue = item.SAI_Name
                });
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 使用Ajax根据建筑ID得到配电室
        /// </summary>
        /// <param name="buildingID">建筑ID</param>
        /// <returns>Json格式区域集合</returns>         
         public ActionResult GetSwitchingRoomsByBuildingIDAjax(int buildingID)
        {
            if (Request.IsAjaxRequest() && buildingID > 0)
            {
                var transformers = _elecDistributionInfoRepos.GetSwitchingRoomsByBuildingID(buildingID);
                IList list = new ArrayList();
                foreach(var item in transformers)
                {
                    list.Add(new
                    {
                        dataID = item.ED_ID,
                        dataValue = item.ED_Name
                    });
                }
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 使用Ajax根据配电室得到变压器
        /// </summary>
        /// <param name="SwitchingRoomID">配电室ID</param>
        /// <returns>Json格式区域集合</returns>         
         public ActionResult GetTransformersBySwitchingRoomIDAjax(int SwitchingRoomID)
        {
            if (Request.IsAjaxRequest() && SwitchingRoomID > 0)
            {
                var transformers = _elecTSInfoRepos.GetTransformersBySwitchingRoomID(SwitchingRoomID);
                IList list = new ArrayList();
                foreach (var item in transformers)
                {
                    list.Add(new
                    {
                        dataID = item.TS_ID,
                        dataValue = item.TS_Name
                    });
                }
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            return null;
        }        

        /// <summary>
        /// 根据区域得到建筑
        /// </summary>
        /// <param name="areaID">区域ID</param>
        /// <returns></returns>
        public ActionResult GetBuildingByAreaAjax(int areaID)
        {
            if (Request.IsAjaxRequest() && areaID > 0)
            {
                var buildings = _roomRepos.GetBuildingByAreaID(areaID);
                IList list = new ArrayList();
                foreach (var item in buildings)
                {
                    list.Add(new
                    {
                        dataID = item.BDI_ID,
                        HJFlag = item.BDI_HJFlag,
                        dataValue = item.BDI_Name
                    });
                }
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 根据建筑名称，模糊查询建筑
        /// </summary>
        /// <param name="areaID">建筑名称</param>
        /// <returns></returns>
        public ActionResult GetBuildingByBuildingNameFuzzyAjax(string buildingName)
        {
            if (Request.IsAjaxRequest())
            {
                var buildings = _buildingRepos.GetBuildingByNameFuzzily(buildingName);
                IList list = new ArrayList();
                foreach (var item in buildings)
                {
                    list.Add(new
                    {
                        dataID = item.BDI_ID,
                        HJFlag = item.BDI_HJFlag,
                        dataValue = item.BDI_Name
                    });
                }
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 根据区域得到建筑（专门提供给其他应用调用）
        /// </summary>
        /// <param name="areaID">区域ID</param>
        /// <returns></returns>
        public ActionResult GetBuildingByAreaAjaxForMobile(int areaID)
        {
            var buildings = _roomRepos.GetBuildingByAreaID(areaID);
            IList list = new ArrayList();
            foreach (var item in buildings)
            {
                list.Add(new
                {
                    dataID = item.BDI_ID,
                    HJFlag = item.BDI_HJFlag,
                    dataValue = item.BDI_Name
                });
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据建筑ID得到所有房间
        /// </summary>
        /// <param name="buildingID">建筑ID</param>
        /// <returns></returns>
        public ActionResult GetRoomsByBIDAjax(int buildingID)
        {
            if (Request.IsAjaxRequest() && buildingID > 0)
            {
                var rooms = _roomRepos.GetRooomByBuildingID(buildingID);
                IList list = new ArrayList();
                foreach (var item in rooms)
                {
                    list.Add(new
                    {
                        dataID = item.RI_ID,
                        dataValue = item.RI_RoomCode
                    });
                }
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 根据建筑ID获得真实测点信息
        /// </summary>
        /// <param name="buildingID"></param>
        /// <returns></returns>
        public ActionResult GetPointsByBuildingAjax(int buildingID)
        {
            if (Request.IsAjaxRequest() && buildingID > 0)
            {
                List<AnalogMeasurePoint> amps = _ampRepos.GetAMPbyBuildingID(buildingID);
                if (amps != null && amps.Count > 0)
                {
                    IList result = new ArrayList();
                    foreach (var item in amps)
                    {
                        result.Add(new
                        {
                            analogNo = item.AMP_AnalogNo,
                            powerType = item.AMP_PowerName,
                            updateTime = item.AMP_Date.ToString("yyyy-MM-dd HH:mm:ss"),
                            analogName = item.AMP_Name
                        });
                    }
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else {
                    return null;
                }
            }
            return null;
        }

        /// <summary>
        /// 根据建筑ID获得真实测点信息（专门提供给其他应用调用）
        /// </summary>
        /// <param name="buildingID"></param>
        /// <returns></returns>
        public ActionResult GetPointsByBuildingAjaxForMobile(int buildingID)
        {
            List<AnalogMeasurePoint> amps = _ampRepos.GetAMPbyBuildingID(buildingID);
            if (amps != null && amps.Count > 0)
            {
                IList result = new ArrayList();
                foreach (var item in amps)
                {
                    result.Add(new
                    {
                        analogNo = item.AMP_AnalogNo,
                        powerType = item.AMP_PowerName,
                        updateTime = item.AMP_Date.ToString("yyyy-MM-dd HH:mm:ss"),
                        analogName = item.AMP_Name
                    });
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 根据变压器ID获得真实测点信息
        /// </summary>
        /// <param name="TransformerID"></param>
        /// <returns></returns>
        public ActionResult GetPointsByTransformerAjax(int TransformerID)
        {
            if (Request.IsAjaxRequest() && TransformerID > 0)
            {
                IList<EnergyEntity> amps = _buildingGuanghuaRepos.GetPointsByTransformerID(TransformerID);
                if (amps != null && amps.Count > 0)
                {
                    IList result = new ArrayList();
                    foreach (var item in amps)
                    {
                        result.Add(new
                        {
                            analogNo = item.PNO,
                            powerName = item.PowerName,
                            updateTime = item.RealTime.ToString("yyyy-MM-dd HH:mm:ss"),
                            analogName = item.PName
                        });
                    }
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return null;
                }
            }
            return null;
        }

        public ActionResult GetPointsBySchoolAjax(int schoolID)
        {
            if (Request.IsAjaxRequest() && schoolID > 0)
            {
                List<AnalogMeasurePoint> amps = _ampRepos.GetAMPbySchoolID(schoolID);
                if (amps != null && amps.Count > 0)
                {
                    IList result = new ArrayList();
                    foreach (var item in amps)
                    {
                        result.Add(new
                        {
                            analogNo = item.AMP_AnalogNo,
                            powerType = item.AMP_PowerName,
                            updateTime = item.AMP_Date.ToString("yyyy-MM-dd HH:mm:ss"),
                            analogName = item.AMP_Name
                        });
                    }
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return null;
                }
            }
            return null;
        }


    }
}
