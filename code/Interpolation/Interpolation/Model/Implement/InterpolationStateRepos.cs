using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interpolation.Model.Interface;
using Interpolation.Tools;

namespace Interpolation.Model.Implement
{
    class InterpolationStateRepos:IInterpolationStateRepos
    {
        private EnergyMonitorEntitiesDataContext dataEntities;
        private Log log;

        public InterpolationStateRepos()
        {
            log = new Log();
            dataEntities = new EnergyMonitorEntitiesDataContext();
        }

        public IList<InterpolationState> queryIntersect(InterpolationState IpS)
        {
            var query = (from ips1 in dataEntities.InterpolationState where ips1.AnalogNo == IpS.AnalogNo && ips1.Status == 0 select ips1).Except(
                from ips2 in dataEntities.InterpolationState where ips2.AnalogNo == IpS.AnalogNo && ips2.Status == 0 && 
                (ips2.EndTime < IpS.StartTime.AddHours(-1) || ips2.StartTime > IpS.EndTime.AddHours(1)) select ips2);

            return query.ToList();
        }

        /*
         * 获取子测点与父测点的重合缺失区间，包括已填补和未填补
         * */
        public IList<InterpolationState> queryIntersectWithParent(InterpolationState fatherIps , int sonNo)
        {
            var query = (from ips1 in dataEntities.InterpolationState where ips1.AnalogNo == sonNo select ips1).Except(
                from ips2 in dataEntities.InterpolationState
                where ips2.AnalogNo == sonNo &&
                    (ips2.EndTime < fatherIps.StartTime || ips2.StartTime > fatherIps.EndTime)
                select ips2);

            return query.ToList();
        }

        public bool insertList(IList<InterpolationState> list)
        {
            try
            {
                dataEntities.InterpolationState.InsertAllOnSubmit(list);
                dataEntities.SubmitChanges();
                return true;
            }
            catch (Exception e)
            {
                log.write("Func:insertList;" + e.StackTrace);
                return false;
            }
        }

        public bool modifyList(IList<InterpolationState> list)
        {
            try
            {
                for (int i = 0; i < list.Count; i++)
                {
                    InterpolationState IpS = dataEntities.InterpolationState.Where(x => x.ID == list[i].ID).FirstOrDefault();
                    if (IpS == null) continue;
                    IpS.StartTime = list[i].StartTime;
                    IpS.EndTime = list[i].EndTime;
                    IpS.Lvalue = list[i].Lvalue;
                    IpS.Rvalue = list[i].Rvalue;
                    IpS.Status = list[i].Status;
                }
                dataEntities.SubmitChanges();
                return true;
            }
            catch (Exception e)
            {
                log.write("Func:modifyList;" + e.StackTrace);
                return false;
            }
        }


        public bool insert(InterpolationState IpS)
        {
            try
            {
                dataEntities.InterpolationState.InsertOnSubmit(IpS);
                dataEntities.SubmitChanges();
                return true;
            }
            catch (Exception e)
            {
                log.write("Func:insert;" + e.StackTrace);
                return false;
            }
        }
    }
}
