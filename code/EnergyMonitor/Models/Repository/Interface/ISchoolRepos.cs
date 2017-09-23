using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnergyMonitor.Models.LinqEntity;
using System.Collections;

namespace EnergyMonitor.Models.Repository.Interface
{
    public interface ISchoolRepos
    {
        /// <summary>
        /// 查询校区
        /// </summary>
        /// <param name="schoolCode"></param>
        /// <param name="schoolName"></param>
        /// <param name="skipItems"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        IList QuerySchool(string schoolCode, string schoolName, int skipItems, int pageSize);

        /// <summary>
        /// 查询校区个数
        /// </summary>
        /// <param name="schoolCode"></param>
        /// <param name="schoolName"></param>
        /// <returns></returns>
        int QuerySchoolCount(string schoolCode, string schoolName);

        /// <summary>
        /// 获取指定校区
        /// </summary>
        /// <param name="schoolID"></param>
        /// <returns></returns>
        SchoolInfo QuerySchool(int schoolID);

        /// <summary>
        /// 获取所有校区的信息
        /// </summary>
        /// <returns></returns>
        List<SchoolInfo> QueryAllSchool();

        /// <summary>
        /// 删除校区
        /// </summary>
        /// <param name="schoolID"></param>
        /// <returns></returns>
        bool DeleteSchool(int schoolID);

        /// <summary>
        /// 修改校区
        /// </summary>
        /// <param name="schoolID"></param>
        /// <param name="schoolCode"></param>
        /// <param name="schoolName"></param>
        /// <param name="schoolAddr"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        bool ModifySchoolPartInfo(int schoolID, string schoolCode, string schoolName, string schoolAddr, string remark, int buildingArea, int groudArea);

        /// <summary>
        /// 增加校区
        /// </summary>
        /// <param name="schoolCode"></param>
        /// <param name="schoolName"></param>
        /// <param name="schoolAddr"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        bool AddSchoolPart(string schoolCode, string schoolName, string schoolAddr, string remark);

        /// <summary>
        /// 查询指定校区拥有的区域个数
        /// </summary>
        /// <param name="schoolID"></param>
        /// <returns></returns>
        int QueryAreaCount(int schoolID);

        /// <summary>
        /// 查询校区名称是否已经存在
        /// </summary>
        /// <param name="schoolName"></param>
        /// <returns></returns>
        bool IsSchoolNameExist(string schoolName);

        /// <summary>
        /// 获取全校信息
        /// </summary>
        /// <returns></returns>
        UniversityInfo GetUniversityInfo();

        /// <summary>
        /// 修改全校信息
        /// </summary>
        /// <param name="StudentCount"></param>
        /// <param name="Area"></param>
        void ModifyUniversityInfo(int StudentCount, float Area);
    }
}
