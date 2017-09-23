using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnergyMonitor.Models.LinqEntity;
using EnergyMonitor.Models.Repository.Entity;
using System.Collections;
using System.Data;
using EnergyMonitor.Repository.Entity;

namespace EnergyMonitor.Models.Repository.Interface
{
    public interface IExceptionReps
    {
         /// <summary>
         /// 根据异常类型查询指定时间段的电力异常值
         /// </summary>
         /// <param name="exceptionType"></param>         
         /// <param name="startTime"></param>
         /// <param name="endTime"></param>      
         /// <returns></returns>
         /// <summary>
         IList<ChartStatisEntity> GetExceptionInfo(int exceptionType, DateTime startTime, DateTime endTime);        
    }
}
