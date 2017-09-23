using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnergyMonitor.Models.LinqEntity;
using System.Collections;

namespace EnergyMonitor.Models.Repository.Interface
{
    public interface ISystemProfileRepos
    {
        /// <summary>
        /// 根据开始字符串获取系统数据
        /// </summary>
        /// <param name="str">开始字符串</param>
        /// <returns></returns>
        IQueryable<SystemProfile> GetByStartStr(string str);

        /// <summary>
        /// 获取所有的价格参数
        /// </summary>
        /// <returns></returns>
        IQueryable<SystemProfile> GetAllPrice();

        /// <summary>
        /// 根据键值对修改系统参数
        /// </summary>
        /// <param name="newDic"></param>
        /// <returns></returns>
        bool Modify(IDictionary newDic);
    }
}
