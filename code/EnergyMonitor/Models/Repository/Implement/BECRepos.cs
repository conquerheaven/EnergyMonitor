using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnergyMonitor.Models.LinqEntity;
using EnergyMonitor.Models.Repository.Interface;
using EnergyMonitor.Models.Repository.Entity;
using System.Collections;

namespace EnergyMonitor.Models.Repository.Implement
{
    public class BECRepos : IBECRepos
    {
        private EnergyMonitorDataContext _dataContext;

        public BECRepos() 
        {
            _dataContext = new EnergyMonitorDataContext();
        }

        public double GetBuildingConsum(int buildingID, int year, string powerType) {
            try
            {
                var consum = _dataContext.BuildingEnergyConsums.Where(x => x.BEC_BID == buildingID && x.BEC_Year == year && x.BEC_PowerType == powerType).Select(x => x.BEC_Consum).Single();
                return consum.HasValue ? consum.Value : 0;
            }
            catch (Exception e) {
                return 0;
            }
        }

        public IQueryable<BECEntity > QueryBEC(int buildingID) {
            try
            {
                var BECInfo = from bec in _dataContext.BuildingEnergyConsums
                              from bbi in _dataContext.BuildingBriefInfos
                              from pc in _dataContext.PowerClasses
                              where bec.BEC_BID == buildingID && bbi.BDI_ID == bec.BEC_BID && pc.PC_ID == bec.BEC_PowerType
                              select new BECEntity
                              {
                                  BuildingID = buildingID,
                                  BuildingName = bbi.BDI_Name,
                                  powerType = bec.BEC_PowerType,
                                  powerTypeName = pc.PC_Name,
                                  year = bec.BEC_Year,
                                  Val = bec.BEC_Consum.Value,
                                  powerUnit = pc.PC_Unit
                              };
                return BECInfo;
            }
            catch (Exception e) {
                return null;
            }
        }

        public bool ModifyOrAddBEC(int buildingID, int year, string powerType, double Val) {
            try
            {
                var count = _dataContext.BuildingEnergyConsums.Where(x => x.BEC_BID == buildingID && x.BEC_Year == year && x.BEC_PowerType == powerType).Count();
                if (count > 0)
                {
                    var item = _dataContext.BuildingEnergyConsums.Where(x => x.BEC_BID == buildingID && x.BEC_Year == year && x.BEC_PowerType == powerType).Single();
                    item.BEC_Consum = Val;
                }
                else
                {
                    var newItem = new BuildingEnergyConsum();
                    newItem.BEC_BID = buildingID;
                    newItem.BEC_Year = year;
                    newItem.BEC_PowerType = powerType;
                    newItem.BEC_Consum = Val;
                    _dataContext.BuildingEnergyConsums.InsertOnSubmit(newItem);
                }
                _dataContext.SubmitChanges();
                return true;
            }
            catch (Exception e) {
                return false;
            }
        }
    }
}
