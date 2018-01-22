using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpolation.Model.Entities
{
    class TimeGap
    {
        DateTime startTime;
        DateTime endTime;
        public TimeGap(DateTime startTime, DateTime endTime)
        {
            this.startTime = startTime;
            this.endTime = endTime;
        }

        public DateTime getStartTime()
        {
            return this.startTime;
        }

        public DateTime getEndTime()
        {
            return this.endTime;
        }
    }
}
