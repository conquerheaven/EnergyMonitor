using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interpolation.Model.Interface
{
    interface IAnalogHistoryHourRepos
    {
        DateTime getLastTime();

        bool insertList(IList<AnalogHistoryHour> list);
    }
}
