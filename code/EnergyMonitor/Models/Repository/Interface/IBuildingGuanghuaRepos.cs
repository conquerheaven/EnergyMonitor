using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnergyMonitor.Models.Repository.Entity;


namespace EnergyMonitor.Models.Repository.Interface
{
    public interface IBuildingGuanghuaRepos
    {
         /// <summary>
        /// 根据选择的对象粒度对光华楼进行报表管理
        /// </summary>
        /// <param name="queryObjType"></param>
        /// 1：建筑
        /// 2：配电室
        /// 3：变压器  
        /// <param name="buildingIDObj"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        IList<BuildingGuanghuaEntity> GetBuildingGuanghuaEnergy(int queryObjType, int buildingIDObj, DateTime startTime, DateTime endTime);

       /// <summary>
        /// 根据光华楼变压器查询具体测点
        /// </summary>
        /// <param name="TransformerID"></param>        
        /// <returns></returns>
        IList<EnergyEntity> GetPointsByTransformerID(int TransformerID);
    }
}
