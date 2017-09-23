using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnergyMonitor.Models.Repository.Interface;
using EnergyMonitor.Models.LinqEntity;

namespace EnergyMonitor.Models.Repository.Implement
{
    public class AnalogMeasurePointRepos : IAnalogMeasurePoint
    {
        private EnergyMonitorDataContext _dataContext;

        public AnalogMeasurePointRepos()
        {
            _dataContext = new EnergyMonitorDataContext();
        }

        #region IAnalogMeasurePoint Members

        /// <summary>
        /// 获得各个校区的能耗统计点
        /// </summary>        
        /// <returns></returns>
        public IList<int> GetAllSchoolMeasurePoint()
        {
            IList<int> allSchoolMeasurePoint = new List<int>();
            for(int i = 1; i <5;i++)
            {
                int schoolMeasurePoint = Convert.ToInt32( _dataContext.AnalogMeasurePoints.Where(x => x.AMP_SchooldID == i && x.AMP_SAreaID == 0 && x.AMP_PowerType == "001005").Select(x =>x.AMP_AnalogNo).SingleOrDefault());
                allSchoolMeasurePoint.Add(schoolMeasurePoint);
            }
            return allSchoolMeasurePoint;
        }
        #endregion

        /// <summary>
        /// 根据层级获取测点
        /// 0：不属于任何校区
        /// 1：校区级
        /// 2：区域级
        /// 3：楼宇级
        /// 4：楼宇以下级
        /// </summary>
        /// <returns>测点列表</returns>
        public IList<AnalogMeasurePoint> GetMeasurePointByLevel(int level)
        {
            IList<AnalogMeasurePoint> measurePoint = new List<AnalogMeasurePoint>();

            switch (level)
            {
                case 0:
                    measurePoint = _dataContext.AnalogMeasurePoints.Where(x => x.AMP_SchooldID == 0).OrderBy(x => x.AMP_Name).ToList();
                    break;
                case 1:
                    measurePoint = _dataContext.AnalogMeasurePoints.Where(x => x.AMP_SchooldID > 0 && x.AMP_SAreaID == 0).OrderBy(x => x.AMP_Name).ToList();
                    break;
                case 2:
                    measurePoint = _dataContext.AnalogMeasurePoints.Where(x => x.AMP_SAreaID > 0 && x.AMP_BuildingID == 0).OrderBy(x => x.AMP_Name).ToList();
                    break;
                case 3:
                    measurePoint = _dataContext.AnalogMeasurePoints.Where(x => x.AMP_BuildingID > 0 && x.AMP_RoomID == 0).OrderBy(x => x.AMP_Name).ToList();
                    break;
                case 4:
                    measurePoint = _dataContext.AnalogMeasurePoints.Where(x => x.AMP_RoomID > 0).OrderBy(x => x.AMP_Name).ToList();
                    break;
                default:
                    break;
            }

            return measurePoint;
        }

        /// <summary>
        /// 根据父测点编号获取测点
        /// </summary>
        /// <param name="ParentNo">父测点编号</param>
        /// <returns>测点列表</returns>
        public IList<AnalogMeasurePoint> GetMeasurePointByParentNo(int ParentNo)
        {
            IList<AnalogMeasurePoint> measurePoint = new List<AnalogMeasurePoint>();

            measurePoint = (from amp in _dataContext.AnalogMeasurePoints where amp.AMP_ParentNo == ParentNo && !(new string[] { "V", "A", "Kw" }).Contains(amp.AMP_Unit) select amp).ToList();

            return measurePoint;
        }


        public AnalogMeasurePoint GetMeasurePointByAnalogNo(int AnalogNo)
        {
            AnalogMeasurePoint amp = _dataContext.AnalogMeasurePoints.Where(x => x.AMP_AnalogNo == AnalogNo).SingleOrDefault();
            return amp;
        }
    }
}
