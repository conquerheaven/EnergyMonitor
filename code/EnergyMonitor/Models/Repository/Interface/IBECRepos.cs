using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnergyMonitor.Models.LinqEntity;
using EnergyMonitor.Models.Repository.Entity;
using System.Collections;

namespace EnergyMonitor.Models.Repository.Interface
{
    public interface IBECRepos
    {
        /// <summary>
        /// 返回指定建筑某年的额定能耗。
        /// </summary>
        /// <param name="buildingID"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        double GetBuildingConsum(int buildingID, int year, string powerType);

        /// <summary>
        /// 查询某栋建筑的所有额定能耗信息
        /// </summary>
        /// <param name="buildingID"></param>
        /// <returns></returns>
        IQueryable<BECEntity> QueryBEC(int buildingID);

        /// <summary>
        /// 修改或添加某条建筑额定能耗记录
        /// </summary>
        /// <param name="buildingID"></param>
        /// <param name="year"></param>
        /// <param name="powerType"></param>
        /// <param name="Val"></param>
        /// <returns></returns>
        bool ModifyOrAddBEC(int buildingID, int year, string powerType, double Val);
    }
}
