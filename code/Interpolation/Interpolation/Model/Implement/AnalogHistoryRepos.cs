using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interpolation.Model.Interface;
using Interpolation.Tools;
using Interpolation.Model.Entities;

namespace Interpolation.Model.Implement
{
    class AnalogHistoryRepos : IAnalogHistoryRepos
    {
        private EnergyMonitorEntitiesDataContext dataEntities;
        private Log log;

        public AnalogHistoryRepos()
        {
            log = new Log();
            dataEntities = new EnergyMonitorEntitiesDataContext();
        }

        public IList<AnalogHistory> queryByAnalogNoAndTimeInterval(int analogNo, DateTime startTime, DateTime endTime)
        {
            IList<AnalogHistory> result = new List<AnalogHistory>();
            try
            {
                result = dataEntities.AnalogHistory.Where(x => x.AH_AnalogNo == analogNo && x.AH_Time >= startTime && x.AH_Time <= endTime).ToList();
            }
            catch (Exception e)
            {
                log.write("Func:queryByAnalogNoAndTimeInterval;" + e.StackTrace);
            }
            return result;
        }

        public IList<AnalogHistoryHour> getHourEnergyHistoryByAnalogNo(int analogNo, DateTime startTime, DateTime endTime)
        {
            IList<AnalogHistoryHour> result = new List<AnalogHistoryHour>();
            try
            {
                var tempQuery = from ah in dataEntities.AnalogHistory
                                where ah.AH_AnalogNo == analogNo && ah.AH_Time >= startTime && ah.AH_Time <= endTime
                                select new
                                {
                                    PNO = ah.AH_AnalogNo,
                                    Time = new DateTime(ah.AH_Time.Year, ah.AH_Time.Month, ah.AH_Time.Day, ah.AH_Time.Hour, 0, 0),
                                    Val = ah.AH_Value
                                };
                var resultQuery = from ah1 in tempQuery
                                  group ah1.Val by new { ah1.PNO, ah1.Time } into g
                                  orderby g.Key.Time
                                  select new HourEntity
                                  {
                                      AnalogNo = g.Key.PNO,
                                      AnalogTime = g.Key.Time,
                                      value = g.Max()
                                  };
                IList<HourEntity> tmpList = resultQuery.ToList();
                for (int i = 0; i < tmpList.Count; i++)
                {
                    AnalogHistoryHour ahh = new AnalogHistoryHour();
                    ahh.AHH_AnalogNo = tmpList[i].AnalogNo;
                    ahh.AHH_HTime = tmpList[i].AnalogTime;
                    ahh.AHH_Value = tmpList[i].value;
                    result.Add(ahh);
                }
            }
            catch (Exception e)
            {
                log.write("Func:getHourEnergyHistoryByAnalogNo;" + e.StackTrace);
            }
            return result;
        }
    }
}
