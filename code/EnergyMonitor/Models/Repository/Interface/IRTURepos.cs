using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnergyMonitor.Models.LinqEntity;
using System.Collections;
using EnergyMonitor.Models.Repository.Entity;

namespace EnergyMonitor.Models.Repository.Interface
{
    public interface IRTURepos
    {
        /// <summary>
        /// 获取所有的RTU
        /// </summary>
        /// <returns></returns>
        IQueryable<RTUInfo> GetAll();
    }
}
