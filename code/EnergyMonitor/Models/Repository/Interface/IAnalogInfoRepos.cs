using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnergyMonitor.Models.LinqEntity;
using System.Collections;

namespace EnergyMonitor.Models.Repository.Interface
{
    public interface IAnalogInfoRepos
    {
        /// <summary>
        /// 返回所有的模拟量测点 
        /// </summary>
        /// <returns></returns>
        IQueryable<AnalogInfo> GetAll();

        /// <summary>
        /// 获取下个模拟量编号
        /// </summary>
        /// <returns></returns>
        int GetNextAnalogNo();

        /// <summary>
        /// 增加模拟量
        /// </summary>
        /// <param name="ai"></param>
        /// <returns>增加成功返回大于0的编号，否则返回0</returns>
        int AddAnalogInfo(AnalogInfo ai);

        /// <summary>
        /// 修改模拟量
        /// </summary>
        /// <param name="newAI"></param>
        /// <returns></returns>
        bool ModifyAnalogInfo(AnalogInfo newAI);


        /// <summary>
        /// 修改AnalogInfo
        /// </summary>
        /// <param name="ai"></param>
        /// <returns></returns>
        bool ModifyAI(AnalogInfo ai);

        /// <summary>
        /// 根据RTU_No获取AnalogInfo
        /// </summary>
        /// <param name="RTU_No"></param>
        /// <returns></returns>
        IList GetAnalogInfoByRTU_No(short RTU_No);

        /// <summary>
        /// 批量修改AnalogInfo
        /// </summary>
        /// <param name="aiList"></param>
        /// <returns></returns>
        bool BatchModifyAi(IList<AnalogInfo> aiList);
    }
}
