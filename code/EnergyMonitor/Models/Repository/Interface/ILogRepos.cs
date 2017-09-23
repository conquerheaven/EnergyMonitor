using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnergyMonitor.Models.LinqEntity;
using System.Collections;

namespace EnergyMonitor.Models.Repository.Interface
{
    public interface ILogRepos
    {
        /// <summary>
        /// 增加日志
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        bool AddLog(Log log);
    }
}
