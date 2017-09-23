using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnergyMonitor.Models.LinqEntity;
using EnergyMonitor.Models.Repository.Interface;
using System.Collections;


namespace EnergyMonitor.Models.Repository.Implement
{
    public class PowerClassRepos : IPowerClassRepos
    {
        private EnergyMonitorDataContext _dataContext;

        public PowerClassRepos()
        {
            _dataContext = new EnergyMonitorDataContext();
        } 

        #region IPowerClassRepos Members

        /// <summary>
        /// 获取指定能耗类型的名称
        /// </summary>
        /// <returns></returns>
        public PowerClass GetPowerTypeName(string powerType)
        {           
            return _dataContext.PowerClasses.Where(x => x.PC_ID == powerType).SingleOrDefault();
        }

        /// <summary>
        /// 获取所有用电类型
        /// </summary>
        /// <returns></returns>
        public IList GetElec()
        {
            var list = from pc in _dataContext.PowerClasses
                       where pc.PC_ID.StartsWith("001")
                       select pc;
            return list.ToList();
        }

        /// <summary>
        /// 获取所有的电水气能耗类型
        /// </summary>
        /// <returns></returns>
        public IList<PowerClass> GetThreeType()
        {
            var list = _dataContext.PowerClasses;
            IList<PowerClass> typeList = list.Where(x => x.PC_ID.StartsWith("001") || x.PC_ID.StartsWith("002") || x.PC_ID.StartsWith("003")).ToList();
            return typeList;
        }

        /// <summary>
        /// 得到所有能耗类型
        /// </summary>
        /// <returns></returns>
        public IList GetAll()
        {
            return _dataContext.PowerClasses.ToList();
        }

        /// <summary>
        /// 添加类型
        /// </summary>
        /// <param name="pc"></param>
        /// <returns></returns>
        public bool AddPower(PowerClass pc)
        {
            try
            {
                _dataContext.PowerClasses.InsertOnSubmit(pc);
                _dataContext.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 修改能耗类型
        /// </summary>
        /// <param name="pc"></param>
        /// <returns></returns>
        public bool ModifyPower(PowerClass pc)
        {
            try
            {
                PowerClass oldPC = _dataContext.PowerClasses.Single(x => x.PC_ID == pc.PC_ID);
                oldPC.PC_ID = pc.PC_ID;
                oldPC.PC_Name = pc.PC_Name;
                oldPC.PC_Type = pc.PC_Type;
                oldPC.PC_LocalCode = pc.PC_LocalCode;
                oldPC.PC_Unit = pc.PC_Unit;
                oldPC.PC_Remark = pc.PC_Remark;
                _dataContext.SubmitChanges();
                // 检查AMP表中是否已经使用该类型，如果使用同时需要修改能耗类型名称
                if (_dataContext.AnalogMeasurePoints.Where(x => x.AMP_PowerType == pc.PC_ID).Count() > 0)
                {
                    _dataContext.ExecuteCommand("update AnalogMeasurePoint set AMP_PowerName={0} where AMP_PowerType={1}", pc.PC_Name, pc.PC_ID);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 删除指定能耗类型及其所有子类型
        /// </summary>
        /// <param name="powerID"></param>
        /// <returns></returns>
        public bool DeletePower(string powerID)
        {
            try
            {
                if (_dataContext.ExecuteCommand(@"delete from PowerClass where PC_ID like '" + powerID + "%' ") > 0)
                {
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
        /// 得到类别的所有子类别
        /// </summary>
        /// <param name="parentPowerName">父类别ID</param>
        /// <returns>所有的子类别</returns>
        public IQueryable<PowerClass> GetSubPowers(string parentPowerID)
        {
            return _dataContext.PowerClasses.Where(x => x.PC_ID.StartsWith(parentPowerID) && x.PC_ID.Length >= 6);
        }

        /// <summary>
        /// 验证能耗类型及其子类型是否被AMP表使用
        /// </summary>
        /// <param name="powerId"></param>
        /// <returns></returns>
        public bool IsUsedByAMP(string powerId)
        {
            if (_dataContext.AnalogMeasurePoints.Where(x => x.AMP_PowerType.StartsWith(powerId)).Count() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 获取所有需要查询统计分析的能耗类型
        /// </summary>
        /// <returns></returns>
        public IList<PowerClass> GetStatisTypes()
        {
            var list = _dataContext.PowerClasses;
            IList<PowerClass> typeList = list.Where(x => x.PC_ID.StartsWith("001") || x.PC_ID.StartsWith("002") || x.PC_ID.StartsWith("003")).ToList();
            return typeList;
        }

        /// <summary>
        /// 获取非统计能耗类型
        /// </summary>
        /// <returns></returns>
        public IList<PowerClass> GetNonStatisTypes()
        {
            var statisTypes = _dataContext.PowerClasses.Where(x => x.PC_ID.StartsWith("001") || x.PC_ID.StartsWith("002") || x.PC_ID.StartsWith("003"));
            var query = _dataContext.PowerClasses.Except(statisTypes);
            return query.ToList();
        }

        #endregion
    }
}
