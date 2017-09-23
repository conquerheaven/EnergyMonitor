using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnergyMonitor.Models.Repository.Interface;
using EnergyMonitor.Models.LinqEntity;
using EnergyMonitor.Models.Repository.Entity;

namespace EnergyMonitor.Repository.Implement
{
    public class ExceptionRepos : IExceptionReps
    {
        private EnergyMonitorDataContext _dataContext;

        public ExceptionRepos()
        {
            _dataContext = new EnergyMonitorDataContext();
        }

        #region IAnalogHistory Members

        /// <summary>
        /// 根据异常类型查询指定时间段的电力异常值
        /// </summary>
        /// <param name="exceptionType"></param>         
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>      
        /// <returns></returns>
        /// <summary>
        public IList<ExceptionEntity> GetExceptionInfo(int exceptionType, DateTime startTime, DateTime endTime)
        {               
                var list =  from ei in _dataContext.Exception                               
                               where ei.EI_ExceptionType == exceptionType && ei.EI_Date>= startTime && ei.EI_Date< endTime                               
                               select new ExceptionEntity
                               {
                                   AnalogNo=ei.EI_AnalogNo,
                                   AnalogName = ei.EI_Name,
                                   ExceptionDate = ei.EI_Date,
                                   ExceptionName = ei.EI_ExceptionName
                               };    
              return list.ToList();
        }
        #endregion
    }
}
