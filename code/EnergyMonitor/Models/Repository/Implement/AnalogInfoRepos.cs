using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnergyMonitor.Models.LinqEntity;
using EnergyMonitor.Models.Repository.Interface;
using EnergyMonitor.Models.Repository.Entity;
using System.Collections;

 
namespace EnergyMonitor.Models.Repository.Implement
{
    public class AnalogInfoRepos : IAnalogInfoRepos
    {
        private EnergyMonitorDataContext _dataContext;

        public AnalogInfoRepos()
        {
            _dataContext = new EnergyMonitorDataContext();
        }

        #region IAnalogInfoRepos Members

        /// <summary>
        /// 返回所有的模拟量测点
        /// </summary>
        /// <returns></returns>
        public IQueryable<AnalogInfo> GetAll()
        {
            return _dataContext.AnalogInfos.OrderBy(x=>x.AI_No);
        }

        /// <summary>
        /// 获取下个模拟量编号
        /// </summary>
        /// <returns></returns>
        public int GetNextAnalogNo()
        {
            /*int max = _dataContext.AnalogInfos.Select(x => x.AI_No).Max();
            return max + 1;*/
            string strSql = "SELECT IDENT_CURRENT('[AnalogInfo]')";
            var maxList = _dataContext.ExecuteQuery<Decimal>(strSql);
            int max = 0;
            foreach (var m in maxList)
            {
                if (max < Convert.ToInt32(m)) max = Convert.ToInt32(m);
            }
            return max+1;
        }

        /// <summary>
        /// 增加模拟量
        /// </summary>
        /// <param name="ai"></param>
        /// <returns>增加成功返回大于0的编号，否则返回0</returns>
        public int AddAnalogInfo(AnalogInfo ai)
        {
            try
            {
                _dataContext.AnalogInfos.InsertOnSubmit(ai);
                _dataContext.SubmitChanges();
                return ai.AI_No;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// 修改模拟量
        /// </summary>
        /// <param name="newAI"></param>
        /// <returns></returns>
        public bool ModifyAnalogInfo(AnalogInfo newAI)
        {
            try
            {
                AnalogInfo ai = _dataContext.AnalogInfos.SingleOrDefault(x => x.AI_No == newAI.AI_No);
                if (ai != null)
                {
                    ai.RTU_No = newAI.RTU_No;
                    ai.AI_Serial = newAI.AI_Serial;
                    ai.AI_Name = newAI.AI_Name;
                    ai.AI_LogicalLow = newAI.AI_LogicalLow;
                    ai.AI_LogicalUp = newAI.AI_LogicalUp;
                    ai.AI_Decimal = newAI.AI_Decimal;
                    ai.AI_Cptflag = newAI.AI_Cptflag;
                    ai.AI_Base = newAI.AI_Base;
                    ai.AI_Rate = newAI.AI_Rate;
                    ai.AI_LockVal = newAI.AI_LockVal;
                    ai.AI_LockFlag = newAI.AI_LockFlag;
                    ai.AI_Timespace = newAI.AI_Timespace;
                    ai.AI_Unit = newAI.AI_Unit;
                    ai.AI_State = newAI.AI_State;

                    _dataContext.SubmitChanges();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 修改AnalogInfo
        /// </summary>
        /// <param name="ai"></param>
        /// <returns></returns>
        public bool ModifyAI(AnalogInfo ai)
        {
            try
            {
                AnalogInfo oldAI = _dataContext.AnalogInfos.SingleOrDefault(x => x.AI_No == ai.AI_No);

                if (oldAI.RTU_No != ai.RTU_No) oldAI.RTU_No = ai.RTU_No;
                if (oldAI.AI_Serial != ai.AI_Serial) oldAI.AI_Serial = ai.AI_Serial;
                if (oldAI.AI_Base != ai.AI_Base) oldAI.AI_Base = ai.AI_Base;
                if (oldAI.AI_Rate != ai.AI_Rate) oldAI.AI_Rate = ai.AI_Rate;
                if (oldAI.AI_Name != ai.AI_Name) oldAI.AI_Name = ai.AI_Name;
                _dataContext.SubmitChanges();
                return true;
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
                return false;
            }
        }
        #endregion

        /// <summary>
        /// 根据RTU_No获取AnalogInfo
        /// </summary>
        /// <param name="RTU_No"></param>
        /// <returns></returns>
        public IList GetAnalogInfoByRTU_No(short RTU_No)
        {
            IList aiList = _dataContext.AnalogInfos.Where(x => x.RTU_No == RTU_No).ToList();
            return aiList;
        }

        /// <summary>
        /// 批量修改AnalogInfo
        /// </summary>
        /// <param name="aiList"></param>
        /// <returns></returns>
        public bool BatchModifyAi(IList<AnalogInfo> aiList)
        {
            try
            {
                foreach (AnalogInfo ai in aiList)
                {
                    AnalogInfo oldAI = _dataContext.AnalogInfos.SingleOrDefault(x => x.AI_No == ai.AI_No);
                    if (oldAI.RTU_No != ai.RTU_No) oldAI.RTU_No = ai.RTU_No;
                    if (oldAI.AI_Serial != ai.AI_Serial) oldAI.AI_Serial = ai.AI_Serial;
                    if (oldAI.AI_Name != ai.AI_Name) oldAI.AI_Name = ai.AI_Name;
                }
                _dataContext.SubmitChanges();
                return true;
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
                return false;
            }
        }
    }
}
