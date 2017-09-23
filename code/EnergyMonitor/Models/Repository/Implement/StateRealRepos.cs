using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnergyMonitor.Models.LinqEntity;
using EnergyMonitor.Models.Repository.Interface;
using System.Collections;
using EnergyMonitor.Models.Repository.Entity;
 

namespace EnergyMonitor.Models.Repository.Implement
{
    public class StateRealRepos : IStateRealRepos
    {
        private EnergyMonitorDataContext _dataContext;

        public StateRealRepos()
        {
            _dataContext = new EnergyMonitorDataContext();
        }

        #region IStateRealRepos Members

        /// <summary>
        /// 获取RDS状态
        /// </summary>
        /// <returns></returns>
        public StateEntity GetRDSState()
        {
            var query = from sr in _dataContext.StateReals
                        where sr.SR_Type == "S"
                        select new StateEntity
                        {
                            Type = sr.SR_Type,
                            Info = "远程数据服务",
                            Status = sr.SR_Status,
                            Time = sr.SR_Time,
                            StateNo = sr.SR_No,
                            PositionX = sr.SR_PositionX,
                            PositionY = sr.SR_PositionY
                        };
            return query.FirstOrDefault();
        }

        /// <summary>
        /// 获取所有的Workstation状态
        /// </summary>
        /// <returns></returns>
        public IQueryable<StateEntity> GetAllWorkstationsState()
        {
            var query = from sr in _dataContext.StateReals
                        join w in _dataContext.Workstations on sr.SR_No equals w.WS_No
                        where sr.SR_Type == "W" && w.WS_Type == 2 && w.WS_State == 1
                        select new StateEntity
                        {
                            Type = sr.SR_Type,
                            Info = w.WS_Name,
                            Status = sr.SR_Status,
                            Time = sr.SR_Time,
                            StateNo = sr.SR_No,
                            PositionX = sr.SR_PositionX,
                            PositionY = sr.SR_PositionY
                        };
            return query;
        }

        /// <summary>
        /// 根据Workstation获取对应的RTU状态
        /// </summary>
        /// <param name="workstationNo"></param>
        /// <returns></returns>
        public IQueryable<StateEntity> GetRTUStateByWorkstation(int workstationNo)
        {
            var query = from sr in _dataContext.StateReals
                        join rtu in _dataContext.RTUs on sr.SR_No equals rtu.RTU_ChanSerial
                        where sr.SR_Type == "R" && rtu.RTU_WSNO == workstationNo && rtu.RTU_State != 0
                        select new StateEntity
                        {
                            Type = sr.SR_Type,
                            Info = rtu.RTU_Info,
                            Status = sr.SR_Status,
                            Time = sr.SR_Time,
                            StateNo = sr.SR_No,
                            PositionX = sr.SR_PositionX,
                            PositionY = sr.SR_PositionY
                        };
            return query;
        }

        /// <summary>
        /// 根据RTU获取对应的仪表状态
        /// </summary>
        /// <param name="rtuNo"></param>
        /// <returns></returns>
        public IQueryable<StateEntity> GetInstrumentStateByRTU(int rtuNo)
        {
            var query = from sr in _dataContext.StateReals
                        join ii in _dataContext.InstrumentInfos on sr.SR_No equals ii.InI_Serial
                        where sr.SR_Type == "C" && ii.RTU_No == rtuNo
                        select new StateEntity
                        {
                            Type = sr.SR_Type,
                            Info = ii.InI_Name,
                            Status = sr.SR_Status,
                            Time = sr.SR_Time,
                            StateNo = sr.SR_No,
                            PositionX = sr.SR_PositionX,
                            PositionY = sr.SR_PositionY
                        };
            return query;
        }

        /// <summary>
        /// 查询所有的设备状态，包含W、R和C
        /// </summary>
        /// <returns></returns>
        public IQueryable<StateEntity> GetAllState()
        {
            var workstationQuery = from sr in _dataContext.StateReals
                                   join w in _dataContext.Workstations on sr.SR_No equals w.WS_No
                                   where sr.SR_Type == "W" && w.WS_Type == 2 && w.WS_State == 1 
                                   select new StateEntity
                                   {
                                       Type = sr.SR_Type,
                                       Info = w.WS_Name,
                                       Status = sr.SR_Status,
                                       Time = sr.SR_Time,
                                       StateNo = sr.SR_No,
                                       ParentNo = 0,
                                       PositionX = sr.SR_PositionX,
                                       PositionY = sr.SR_PositionY
                                   };
            var rtuQuery = from sr in _dataContext.StateReals
                           join rtu in _dataContext.RTUs on sr.SR_No equals rtu.RTU_ChanSerial
                           where sr.SR_Type == "R" && rtu.RTU_State != 0 
                           select new StateEntity
                           {
                               Type = sr.SR_Type,
                               Info = rtu.RTU_Info,
                               Status = sr.SR_Status,
                               Time = sr.SR_Time,
                               StateNo = sr.SR_No,
                               ParentNo = rtu.RTU_WSNO,
                               PositionX = sr.SR_PositionX,
                               PositionY = sr.SR_PositionY
                           };
            var instrumentQuery = from sr in _dataContext.StateReals
                                  join ii in _dataContext.InstrumentInfos on sr.SR_No equals ii.InI_Serial
                                  where sr.SR_Type == "C"
                                  select new StateEntity
                                  {
                                      Type = sr.SR_Type,
                                      Info = ii.InI_Name,
                                      Status = sr.SR_Status,
                                      Time = sr.SR_Time,
                                      StateNo = sr.SR_No,
                                      ParentNo = ii.RTU_No,
                                      PositionX = sr.SR_PositionX,
                                      PositionY = sr.SR_PositionY
                                  };
            var query = workstationQuery.Union(rtuQuery).Union(instrumentQuery);
            return query;
        }

        /// <summary>
        /// 查询所有的设备状态，包含W、R和C
        /// </summary>
        /// <param name="stateStatus">1为正常设备，0为故障设备</param>
        /// <returns></returns>
        public IQueryable<StateEntity> GetAllState(int stateStatus)
        {
            var workstationQuery = from sr in _dataContext.StateReals
                                   join w in _dataContext.Workstations on sr.SR_No equals w.WS_No
                                   where sr.SR_Type == "W" && w.WS_Type == 2 && w.WS_State == 1 && sr.SR_Status == stateStatus
                                   select new StateEntity
                                   {
                                       Type = sr.SR_Type,
                                       Info = w.WS_Name,
                                       Status = sr.SR_Status,
                                       Time = sr.SR_Time,
                                       StateNo = sr.SR_No,
                                       ParentNo = 0,
                                       PositionX = sr.SR_PositionX,
                                       PositionY = sr.SR_PositionY
                                   };
            var rtuQuery = from sr in _dataContext.StateReals
                           join rtu in _dataContext.RTUs on sr.SR_No equals rtu.RTU_ChanSerial
                           where sr.SR_Type == "R" && rtu.RTU_State != 0 && sr.SR_Status == stateStatus
                           select new StateEntity
                           {
                               Type = sr.SR_Type,
                               Info = rtu.RTU_Info,
                               Status = sr.SR_Status,
                               Time = sr.SR_Time,
                               StateNo = sr.SR_No,
                               ParentNo = rtu.RTU_WSNO,
                               PositionX = sr.SR_PositionX,
                               PositionY = sr.SR_PositionY
                           };
            var instrumentQuery = from sr in _dataContext.StateReals
                                  join ii in _dataContext.InstrumentInfos on sr.SR_No equals ii.InI_Serial
                                  where sr.SR_Type == "C" && sr.SR_Status == stateStatus
                                  select new StateEntity
                                  {
                                      Type = sr.SR_Type,
                                      Info = ii.InI_Name,
                                      Status = sr.SR_Status,
                                      Time = sr.SR_Time,
                                      StateNo = sr.SR_No,
                                      ParentNo = ii.RTU_No,
                                      PositionX = sr.SR_PositionX,
                                      PositionY = sr.SR_PositionY
                                  };
            var query = workstationQuery.Union(rtuQuery).Union(instrumentQuery);
            return query;         
        }

        /// <summary>
        /// 得到所有前置机和RTU状态
        /// </summary>
        /// <returns></returns>
        public IQueryable<StateEntity> GetWordAndRTUState()
        {
            var workstationQuery = from sr in _dataContext.StateReals
                                   join w in _dataContext.Workstations on sr.SR_No equals w.WS_No
                                   where sr.SR_Type == "W" && w.WS_Type == 2 && w.WS_State == 1
                                   select new StateEntity
                                   {
                                       Type = sr.SR_Type,
                                       Info = w.WS_Name,
                                       Status = sr.SR_Status,
                                       Time = sr.SR_Time,
                                       StateNo = sr.SR_No,
                                       ParentNo = 4,
                                       PositionX = sr.SR_PositionX,
                                       PositionY = sr.SR_PositionY
                                   };
            var rtuQuery = from sr in _dataContext.StateReals
                           join rtu in _dataContext.RTUs on sr.SR_No equals rtu.RTU_ChanSerial
                           where sr.SR_Type == "R" && rtu.RTU_State != 0
                           select new StateEntity
                           {
                               Type = sr.SR_Type,
                               Info = rtu.RTU_Info,
                               Status = sr.SR_Status,
                               Time = sr.SR_Time,
                               StateNo = sr.SR_No,
                               ParentNo = rtu.RTU_WSNO,
                               PositionX = sr.SR_PositionX,
                               PositionY = sr.SR_PositionY
                           };
            var query = workstationQuery.Union(rtuQuery);
            return query;
        }

        /// <summary>
        /// 更新图表中的所有状态的位置
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public bool UpdateStatePosition(List<StateReal> list)
        {
            try
            {
                var modifyList = _dataContext.StateReals.Where(x => x.SR_Type != "C").ToList();
                foreach (var item in list)
                {
                    var modify = modifyList.SingleOrDefault(x => x.SR_Type == item.SR_Type && x.SR_No == item.SR_No);
                    if (modify != null)
                    {
                        modify.SR_PositionX = item.SR_PositionX;
                        modify.SR_PositionY = item.SR_PositionY;
                    }
                }
                _dataContext.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion
    }
}
