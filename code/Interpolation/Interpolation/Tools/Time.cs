using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interpolation.Tools
{
    class Time
    {
        public DateTime YYMMDD_hhmmss()
        {
            return DateTime.Now;
        }

        public DateTime YYMMDD_hhmm00(DateTime datetime)
        {
            return new DateTime(datetime.Year, datetime.Month, datetime.Day, datetime.Hour, datetime.Minute, 0);
        }

        public DateTime YYMMDD_hh0000(DateTime datetime)
        {
            return new DateTime(datetime.Year, datetime.Month, datetime.Day, datetime.Hour, 0, 0);
        }

        public DateTime YYMMDD_000000(DateTime datetime)
        {
            return new DateTime(datetime.Year, datetime.Month, datetime.Day, 0, 0, 0);
        }

        public DateTime YYMM00_000000(DateTime datetime)
        {
            return new DateTime(datetime.Year, datetime.Month, 0, 0, 0, 0);
        }

        public DateTime YY0000_000000(DateTime datetime)
        {
            return new DateTime(datetime.Year, 0, 0, 0, 0, 0);
        }
    }
}
