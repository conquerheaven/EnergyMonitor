using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnergyMonitor.Models.LinqEntity;
using System.Collections;
using EnergyMonitor.Models.Repository.Entity;

namespace EnergyMonitor.Models.Repository.Interface
{
    public interface ISchoolAreaRepos
    {
        /// <summary>
        /// 查询区域
        /// </summary>
        /// <param name="areaName"></param>
        /// <param name="schoolID">若为非正数查询所有校区</param>
        /// <returns></returns>
        IList GetSchoolArea(string areaName, int schoolID);

        /// <summary>
        /// 查询区域
        /// </summary>
        /// <param name="areaName"></param>
        /// <param name="schoolID">若为非正数查询所有校区</param>
        /// <param name="skipItems"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        IList GetSchoolArea(string areaName, int schoolID, int skipItems, int pageSize);

        /// <summary>
        /// 获取区域和校区
        /// </summary>
        /// <param name="areaID"></param>
        /// <returns></returns>
        AreaAndSchool GetSchool(int schoolID);

        /// <summary>
        /// 查询校区下所有区域
        /// </summary>
        /// <param name="schoolID"></param>
        /// <returns></returns>
        IList<SchoolAreaInfo> GetSchoolAllArea(int schoolID);

        /// <summary>
        /// 获取区域和校区
        /// </summary>
        /// <param name="areaID"></param>
        /// <returns></returns>
        AreaAndSchool GetAreaAndSchool(int areaID);

        /// <summary>
        /// 获取区域个数
        /// </summary>
        /// <param name="areaName"></param>
        /// <param name="schoolID">若为非正数查询所有校区</param>
        /// <returns></returns>
        int GetSchoolAreaCount(string areaName, int schoolID);

        /// <summary>
        /// 删除区域
        /// </summary>
        /// <param name="areaID"></param>
        /// <returns></returns>
        bool DeleteSchoolArea(int areaID);

        /// <summary>
        /// 增加区域
        /// </summary>
        /// <param name="areaName"></param>
        /// <param name="schoolID"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        bool AddSchoolAreaPart(string areaName, int schoolID, string remark);

        /// <summary>
        /// 修改区域
        /// </summary>
        /// <param name="areaID"></param>
        /// <param name="schoolID"></param>
        /// <param name="areaName"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        bool ModifySchoolAreaPartInfo(int areaID, int schoolID, string areaName, string remark);

        /// <summary>
        /// 获取区域拥有的楼宇数
        /// </summary>
        /// <param name="areaID"></param>
        /// <returns></returns>
        int GetBuildingCountByArea(int areaID);

        /// <summary>
        /// 查询区域名称是否已经存在
        /// </summary>
        /// <param name="areaName"></param>
        /// <param name="schoolID"></param>
        /// <returns></returns>
        bool IsAreaNameExist(string areaName, int? schoolID);

        /// <summary>
        /// 得到校区区域
        /// </summary>
        /// <param name="areaId">区域Id</param>
        /// <returns></returns>
        SchoolAreaInfo GetSchoolArea(int areaId);
    }
}
