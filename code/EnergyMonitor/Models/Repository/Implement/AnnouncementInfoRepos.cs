using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnergyMonitor.Models.Repository.Interface;
using EnergyMonitor.Models.LinqEntity;

namespace EnergyMonitor.Models.Repository.Implement
{
    public class AnnouncementInfoRepos : IAnnouncementInfoRepos
    {
        private EnergyMonitorDataContext _dataContext;

        public AnnouncementInfoRepos()
        {
            _dataContext = new EnergyMonitorDataContext();
        }

        public IQueryable<AnnouncementInfo> GetAllAnnouncementInfoQueryable()
        {
            return _dataContext.AnnouncementInfos.Select(x => x);
        }


        public bool AddAnnouncementInfo(IList<AnnouncementInfo> amiList)
        {
            try
            {
                _dataContext.AnnouncementInfos.InsertAllOnSubmit(amiList);
                _dataContext.SubmitChanges();
                return true;
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
                return false;
            }
        }


        public bool ModifyAnnouncementInfo(AnnouncementInfo newAmi)
        {
            try
            {
                AnnouncementInfo ami = _dataContext.AnnouncementInfos.Where(x => x.ID == newAmi.ID).SingleOrDefault();
                if (ami != null)
                {
                    ami.Title = newAmi.Title;
                    ami.Content = newAmi.Content;
                    ami.DeadLine = newAmi.DeadLine;
                    if (newAmi.Remark != null) ami.Remark = newAmi.Remark;
                }
                _dataContext.SubmitChanges();
                return true;
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
                return false;
            }
        }

        public bool DeleteAnnouncementInfoByID(int ID)
        {
            try
            {
                AnnouncementInfo ami = _dataContext.AnnouncementInfos.Where(x => x.ID == ID).SingleOrDefault();
                if (ami != null)
                {
                    _dataContext.AnnouncementInfos.DeleteOnSubmit(ami);
                    _dataContext.SubmitChanges();
                }
                return true;
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
                return false;
            }
        }


        public AnnouncementInfo GetAnnouncementInfoByID(int ID)
        {
            return _dataContext.AnnouncementInfos.Where(x => x.ID == ID).SingleOrDefault();
        }


        public List<AnnouncementInfo> GetAvailableAnnouncementFromStarttime(DateTime starttime)
        {
            DateTime deadline = DateTime.Now;
            return _dataContext.AnnouncementInfos.Where(x => x.CreateTime >= starttime && x.DeadLine > deadline).ToList();
        }
    }
}
