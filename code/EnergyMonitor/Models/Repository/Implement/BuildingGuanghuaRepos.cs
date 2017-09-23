using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnergyMonitor.Models.LinqEntity;
using EnergyMonitor.Models.Repository.Interface;
using EnergyMonitor.Models.Repository.Entity;
using EnergyMonitor.Models.Entity;

namespace EnergyMonitor.Models.Repository.Implement
{
    public class BuildingGuanghuaRepos : IBuildingGuanghuaRepos
    {
        private EnergyMonitorDataContext _dataContext; 

        public BuildingGuanghuaRepos()
        {
            _dataContext = new EnergyMonitorDataContext();           
        }

        #region IBuildingGuanghua Members

        /// <summary>
        /// 根据选择的对象粒度对光华楼进行报表管理
        /// </summary>
        /// <param name="queryObjType"></param>
        /// 1：建筑
        /// 2：配电室
        /// 3：变压器            
        /// <param name="buildingIDObj"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public IList<BuildingGuanghuaEntity> GetBuildingGuanghuaEnergy(int queryObjType,int buildingIDObj, DateTime startTime, DateTime endTime)
        {
            IQueryable<BuildingGuanghuaEntity> list = null;
            switch(queryObjType)
            {
                case 1:
                          list = from bg in _dataContext.BuildingGuanghua
                                   join amp in _dataContext.AnalogMeasurePoints on bg.BG_No equals amp.AMP_AnalogNo                       
                                   select new BuildingGuanghuaEntity
                                   {
                                       SwitchingRoom = (string)((_dataContext.ElecDistributionInfo.Where(x => x.ED_ID == bg.ED_ID).Select(x => x.ED_Name)).ToList())[0],
                                       Transformer = (string)((_dataContext.ElecTSInfo.Where(x => x.TS_ID == bg.TS_ID).Select(x => x.TS_Name)).ToList())[0],                                        
                                       CntrNo = bg.CntrNo,
                                       Name = amp.AMP_Name,
                                       StartTimeVal = (_dataContext.AnalogHistories.Where(x => x.AH_AnalogNo == bg.BG_No && x.AH_Time <= startTime).Select(x => x.AH_Value)).Max(),
                                       EndTimeVal = (_dataContext.AnalogHistories.Where(x => x.AH_AnalogNo == bg.BG_No && x.AH_Time <= endTime).Select(x => x.AH_Value)).Max()                                   
                                   };
                          break;                  
                case 2:
                          list = from bg in _dataContext.BuildingGuanghua
                                   join amp in _dataContext.AnalogMeasurePoints on bg.BG_No equals amp.AMP_AnalogNo
                                   where bg.ED_ID == buildingIDObj
                                   select new BuildingGuanghuaEntity
                                   {
                                       SwitchingRoom = (string)((_dataContext.ElecDistributionInfo.Where(x => x.ED_ID == bg.ED_ID).Select(x => x.ED_Name)).ToList())[0],
                                       Transformer = (string)((_dataContext.ElecTSInfo.Where(x => x.TS_ID == bg.TS_ID).Select(x => x.TS_Name)).ToList())[0],                                          
                                       CntrNo = bg.CntrNo,
                                       Name = amp.AMP_Name,
                                       StartTimeVal = (_dataContext.AnalogHistories.Where(x => x.AH_AnalogNo == bg.BG_No && x.AH_Time <= startTime).Select(x => x.AH_Value)).Max(),
                                       EndTimeVal = (_dataContext.AnalogHistories.Where(x => x.AH_AnalogNo == bg.BG_No && x.AH_Time <= endTime).Select(x => x.AH_Value)).Max()                                     
                                   };
                          var testc = list.ToList();
                          break;
                case 3:
                        //  var test = (_dataContext.AnalogHistories.Where(x => x.AH_AnalogNo == 1681 && x.AH_Time <= DateTime.Now).Select(x => x.AH_Value)).Max();
                          list = from bg in _dataContext.BuildingGuanghua
                                   join amp in _dataContext.AnalogMeasurePoints on bg.BG_No equals amp.AMP_AnalogNo
                                   where bg.TS_ID == buildingIDObj                               
                                   select new BuildingGuanghuaEntity
                                   {                                            
                                       SwitchingRoom =(string) ((_dataContext.ElecDistributionInfo.Where(x => x.ED_ID == bg.ED_ID).Select(x => x.ED_Name)).ToList())[0],
                                       Transformer = (string)((_dataContext.ElecTSInfo.Where(x => x.TS_ID == bg.TS_ID).Select(x => x.TS_Name)).ToList())[0],                                               
                                       CntrNo = bg.CntrNo,
                                       Name = amp.AMP_Name,
                                       StartTimeVal = (_dataContext.AnalogHistories.Where(x => x.AH_AnalogNo == bg.BG_No && x.AH_Time <= startTime).Select(x => x.AH_Value)).Max(),
                                       EndTimeVal = (_dataContext.AnalogHistories.Where(x => x.AH_AnalogNo == bg.BG_No && x.AH_Time <= endTime).Select(x => x.AH_Value)).Max()                          
                                   };
                          break; 
                default:
                         break;
            }
            return list.ToList();  
        }   

        /// <summary>
        /// 根据光华楼变压器查询具体测点
        /// </summary>
        /// <param name="TransformerID"></param>        
        /// <returns></returns>
        public IList<EnergyEntity> GetPointsByTransformerID(int TransformerID)
        {
            try
            {             
                var list = from bgh in _dataContext.BuildingGuanghua
                           join amp in _dataContext.AnalogMeasurePoints on bgh.BG_No equals amp.AMP_AnalogNo
                           where bgh.TS_ID == TransformerID
                           select new EnergyEntity
                           {
                               PNO = amp.AMP_AnalogNo,
                               PowerName = amp.AMP_PowerName,
                               RealTime = amp.AMP_Date,
                               PName = amp.AMP_Name
                           };
                return list.ToList();
            }
            catch(Exception e)
            {
                return null;
            }
        }
        #endregion
    }
}
