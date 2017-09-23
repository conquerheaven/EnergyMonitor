using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnergyMonitor.Models.LinqEntity;
using System.Collections;

namespace EnergyMonitor.Models.Repository.Interface
{
    public interface IPowerClassRepos
    {
        /// <summary>
        /// 获取指定能耗类型的名称
        /// </summary>
        /// <returns></returns>
        PowerClass GetPowerTypeName(string powerType);

        /// <summary>
        /// 获取所有用电类型
        /// </summary>
        /// <returns></returns>
        IList GetElec();

        /// <summary>
        /// 获取所有的电水气能耗类型
        /// </summary>
        /// <returns></returns>
        IList<PowerClass> GetThreeType();

        /// <summary>
        /// 得到所有能耗类型
        /// </summary>
        /// <returns></returns>
        IList GetAll();

        /// <summary>
        /// 添加类型
        /// </summary>
        /// <param name="pc"></param>
        /// <returns></returns>
        bool AddPower(PowerClass pc);

        /// <summary>
        /// 修改能耗类型
        /// </summary>
        /// <param name="pc"></param>
        /// <returns></returns>
        bool ModifyPower(PowerClass pc);

        /// <summary>
        /// 删除指定能耗类型及其所有子类型
        /// </summary>
        /// <param name="powerID"></param>
        /// <returns></returns>
        bool DeletePower(string powerID);

        /// <summary>
        /// 得到类别的所有子类别
        /// </summary>
        /// <param name="parentPowerName">父类别ID</param>
        /// <returns>所有的子类别</returns>
        IQueryable<PowerClass> GetSubPowers(string parentPowerID);

        /// <summary>
        /// 验证能耗类型及其子类型是否被AMP表使用
        /// </summary>
        /// <param name="powerId"></param>
        /// <returns></returns>
        bool IsUsedByAMP(string powerId);

        /// <summary>
        /// 获取所有需要查询统计分析的能耗类型
        /// </summary>
        /// <returns></returns>
        IList<PowerClass> GetStatisTypes();

        /// <summary>
        /// 获取非统计能耗类型
        /// </summary>
        /// <returns></returns>
        IList<PowerClass> GetNonStatisTypes();
    }
}
