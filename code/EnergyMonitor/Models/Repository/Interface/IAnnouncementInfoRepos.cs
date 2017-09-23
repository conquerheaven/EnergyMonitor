using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnergyMonitor.Models.LinqEntity;

namespace EnergyMonitor.Models.Repository.Interface
{
    public interface IAnnouncementInfoRepos
    {
        IQueryable<AnnouncementInfo> GetAllAnnouncementInfoQueryable();

        bool AddAnnouncementInfo(IList<AnnouncementInfo> amiList);

        bool ModifyAnnouncementInfo(AnnouncementInfo newAmi);

        bool DeleteAnnouncementInfoByID(int ID);

        AnnouncementInfo GetAnnouncementInfoByID(int ID);

        List<AnnouncementInfo> GetAvailableAnnouncementFromStarttime(DateTime starttime);
    }
}
