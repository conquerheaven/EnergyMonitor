using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnergyMonitor.Models.LinqEntity;

namespace EnergyMonitor.Models.Repository.Interface
{
    public interface IAnalogMeasurePoint
    {
        /// <summary>
        /// 获得各个校区的能耗统计点
        /// </summary>        
        /// <returns></returns>
        IList<int> GetAllSchoolMeasurePoint();

        /// <summary>
        /// 根据层级获取测点
        /// 0：不属于任何校区
        /// 1：校区级
        /// 2：区域级
        /// 3：楼宇级
        /// 4：楼宇以下级
        /// </summary>
        /// <returns>测点列表</returns>
        IList<AnalogMeasurePoint> GetMeasurePointByLevel(int level);

        /// <summary>
        /// 根据父测点编号获取测点
        /// </summary>
        /// <param name="ParentNo">父测点编号</param>
        /// <returns>测点列表</returns>
        IList<AnalogMeasurePoint> GetMeasurePointByParentNo(int ParentNo);

        /// <summary>
        /// 根据测点ID获取测点
        /// </summary>
        /// <param name="AnalogNo"></param>
        /// <returns></returns>
        AnalogMeasurePoint GetMeasurePointByAnalogNo(int AnalogNo);
    }
}
