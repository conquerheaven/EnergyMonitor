using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpolation.Model.Interface
{
    interface IRedisRepos
    {
        IList<AnalogHistoryHour> getByAnalogNoAndTimeGap(int AnalogNo, DateTime startTime, DateTime endTime);
    }
}
