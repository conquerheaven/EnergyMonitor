using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnergyMonitor.Models.LinqEntity;

namespace EnergyMonitor.Models.Repository.Interface
{
    public interface ITransactionRepos
    {
        bool BatchAddRealPoints(IList<AnalogInfo> aiList, IList<AnalogMeasurePoint> ampList);

    }
}
