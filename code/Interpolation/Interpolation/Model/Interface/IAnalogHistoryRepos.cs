using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interpolation.Model.Interface
{
    interface IAnalogHistoryRepos
    {
        IList<AnalogHistory> queryByAnalogNoAndTimeInterval(int analogNo , DateTime startTime , DateTime endTime);

        IList<AnalogHistoryHour> getHourEnergyHistoryByAnalogNo(int analogNo, DateTime startTime, DateTime endTime);
    }
}
