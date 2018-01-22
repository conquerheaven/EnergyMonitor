using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interpolation.Model.Interface;
using Interpolation.Tools;
using System.Data.Linq.SqlClient;

namespace Interpolation.Model.Implement
{
    class AMPRepos : IAMPRepos
    {
        private EnergyMonitorEntitiesDataContext dataEntities;
        private Log log;

        public AMPRepos()
        {
            log = new Log();
            dataEntities = new EnergyMonitorEntitiesDataContext();
        }

        public IList<int> queryAllPointID()
        {
            var query = from amp in dataEntities.AnalogMeasurePoint where SqlMethods.Like(amp.AMP_PowerType,"001%") select amp.AMP_AnalogNo;// amp.AMP_PowerType.Contains("001");
            return query.ToList();
        }
    }
}
