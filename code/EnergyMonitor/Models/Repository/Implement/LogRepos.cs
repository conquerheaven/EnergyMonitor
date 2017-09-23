using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnergyMonitor.Models.LinqEntity;
using EnergyMonitor.Models.Repository.Interface;
using System.Collections;

 
namespace EnergyMonitor.Models.Repository.Implement
{
    public class LogRepos : ILogRepos
    {
        private EnergyMonitorDataContext _dataContext;
        private static ILogRepos _logRepos;

        public LogRepos()
        {
            _dataContext = new EnergyMonitorDataContext();
        }

        /// <summary>
        /// Log静态工厂
        /// </summary>
        /// <returns></returns>
        public static ILogRepos LogInstance()
        {
            if (_logRepos != null)
            {
                return _logRepos;
            }
            else
            {
                return new LogRepos();
            }
        }

        #region ILogRepos Members

        /// <summary>
        /// 增加日志
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        public bool AddLog(Log log)
        {
            try
            {
                _dataContext.Logs.InsertOnSubmit(log);
                _dataContext.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion
    }
}
