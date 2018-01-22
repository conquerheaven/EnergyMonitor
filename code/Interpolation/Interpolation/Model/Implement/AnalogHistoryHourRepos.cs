using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interpolation.Model.Interface;
using Interpolation.Tools;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace Interpolation.Model.Implement
{
    class AnalogHistoryHourRepos : IAnalogHistoryHourRepos
    {
        private EnergyMonitorEntitiesDataContext dataEntities;
        private Log log;

        public AnalogHistoryHourRepos()
        {
            log = new Log();
            dataEntities = new EnergyMonitorEntitiesDataContext();
        }

        public DateTime getLastTime()
        {
            DateTime lastTime = dataEntities.AnalogHistoryHour.OrderByDescending(x => x.AHH_HTime).Select(x => x.AHH_HTime).FirstOrDefault();
            return lastTime;
        }

        public bool insertList(IList<AnalogHistoryHour> list)
        {
            try
            {
                SqlConnection sqlConn = new SqlConnection("server=10.131.200.70;database=EnergyMonitor;uid=sa;pwd=LabTel55664465");

                SqlBulkCopy bulkCopy = new SqlBulkCopy(sqlConn);
                try
                {
                    //dataEntities.AnalogHistoryHour.InsertAllOnSubmit(list);
                    //dataEntities.SubmitChanges();
                    DataTable dt = new DataTable();
                        dt.Columns.AddRange(new DataColumn[]{
                        new DataColumn("AHH_AnalogNo", typeof(int)),
                        new DataColumn("AHH_HTime", typeof(DateTime)),
                        new DataColumn("AHH_Value", typeof(double))
                    });
                    for (int i = 0; i < list.Count; i++)
                    {
                        DataRow r = dt.NewRow();
                        r[0] = list[i].AHH_AnalogNo;
                        r[1] = list[i].AHH_HTime;
                        r[2] = list[i].AHH_Value;
                        dt.Rows.Add(r);
                    }
                    
                    bulkCopy.DestinationTableName = "AnalogHistoryHour";
                    bulkCopy.BatchSize = dt.Rows.Count;
                    sqlConn.Open();
                    if (dt != null && dt.Rows.Count != 0)
                    {
                        bulkCopy.WriteToServer(dt);
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    sqlConn.Close();
                    if (bulkCopy != null)
                    {
                        bulkCopy.Close();
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                log.write("Func:insertList;" + e.StackTrace);
                return false;
            }
        }
    }
}
