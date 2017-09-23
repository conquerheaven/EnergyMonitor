using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnergyMonitor.Models.Repository.Interface;
using EnergyMonitor.Models.LinqEntity;

namespace EnergyMonitor.Models.Repository.Implement
{
    public class TransactionRepos : ITransactionRepos
    {
        private EnergyMonitorDataContext _dataContext;

        public TransactionRepos()
        {
            _dataContext = new EnergyMonitorDataContext();
        }

        public bool BatchAddRealPoints(IList<Models.LinqEntity.AnalogInfo> aiList, IList<Models.LinqEntity.AnalogMeasurePoint> ampList)
        {
            try
            {
                _dataContext.AnalogInfos.InsertAllOnSubmit(aiList);
                for (int i = 0; i < aiList.Count; i++)
                {
                    ampList[i].AMP_AnalogNo = aiList[i].AI_No;
                }
                _dataContext.AnalogMeasurePoints.InsertAllOnSubmit(ampList);
                _dataContext.SubmitChanges();
                return true;
            }
            catch(Exception e)
            {
                throw e;
            }
        }
    }
}
