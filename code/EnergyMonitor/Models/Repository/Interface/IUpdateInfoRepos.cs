using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnergyMonitor.Models.LinqEntity;

namespace EnergyMonitor.Models.Repository.Interface
{
    public interface IUpdateInfoRepos
    {
        IList<UpdateInfo> GetAllUpdateInfo();

        IList<UpdateInfo> GetUpdateInfoByAINo(int AI_No);

        bool ModifyUpdateInfoOfTime(UpdateInfo ui);

        bool InsertUpdateInfo(UpdateInfo ui);

        IList<UpdateInfo> GetRelatedUpdateInfoByUi(UpdateInfo ui);

        bool ModifyUpdateInfoOfState(int ID , bool State);
    }
}
