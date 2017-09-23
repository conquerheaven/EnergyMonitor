using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnergyMonitor.Models.Repository.Interface;
using EnergyMonitor.Models.LinqEntity;
using System.Collections;

namespace EnergyMonitor.Models.Repository.Implement
{

    public class UpdateInfoRepos : IUpdateInfoRepos
    {
        private EnergyMonitorDataContext _dataContext;

        public UpdateInfoRepos()
        {
            _dataContext = new EnergyMonitorDataContext();
        }

        public IList<UpdateInfo> GetAllUpdateInfo()
        {
            IList<UpdateInfo> uiList = (from ui in _dataContext.UpdateInfos select ui).ToList();
            return uiList;
        }

        public IList<UpdateInfo> GetUpdateInfoByAINo(int AI_No)
        {
            IList<UpdateInfo> uiList = _dataContext.UpdateInfos.Where(x => x.AI_No == AI_No).ToList();
            return uiList;
        }


        public bool ModifyUpdateInfoOfTime(UpdateInfo ui)
        {
            try
            {
                UpdateInfo uii = _dataContext.UpdateInfos.Where(x => x.ID == ui.ID).Single();
                uii.Start_Date = ui.Start_Date;
                uii.End_Date = ui.End_Date;
                _dataContext.SubmitChanges();
                return true;
            }
            catch(Exception e)
            {
                System.Console.Write(e.Message);
                return false;
            }
        }

        public bool InsertUpdateInfo(UpdateInfo ui)
        {
            try
            {
                _dataContext.UpdateInfos.InsertOnSubmit(ui);
                _dataContext.SubmitChanges();
                return true;
            }
            catch (Exception e)
            {
                System.Console.Write(e.Message);
                return false;
            }
        }


        public IList<UpdateInfo> GetRelatedUpdateInfoByUi(UpdateInfo ui)
        {
            IList<UpdateInfo> uiList = (from uii in _dataContext.UpdateInfos where uii.State == false && uii.Parent_No == ui.AI_No && 
                                            !(uii.Start_Date > ui.End_Date || uii.End_Date < ui.Start_Date) select uii).ToList();
            return uiList;
        }


        public bool ModifyUpdateInfoOfState(int ID, bool State)
        {
            try
            {
                UpdateInfo ui = _dataContext.UpdateInfos.Where(x => x.ID == ID).Single();
                ui.State = State;
                _dataContext.SubmitChanges();
                return true;
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
                return false;
            }
        }
    }
}
