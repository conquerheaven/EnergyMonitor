using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnergyMonitor.Models.LinqEntity;
using System.Collections;
using EnergyMonitor.Models.Repository.Entity;

namespace EnergyMonitor.Models.Repository.Interface
{
    public interface IStateRealRepos
    {
        /// <summary>
        /// 获取RDS状态
        /// </summary>
        /// <returns></returns>
        StateEntity GetRDSState();

        /// <summary>
        /// 获取所有的Workstation状态
        /// </summary>
        /// <returns></returns>
        IQueryable<StateEntity> GetAllWorkstationsState();

        /// <summary>
        /// 根据Workstation获取对应的RTU状态
        /// </summary>
        /// <param name="workstationNo"></param>
        /// <returns></returns>
        IQueryable<StateEntity> GetRTUStateByWorkstation(int workstationNo);

        /// <summary>
        /// 根据RTU获取对应的仪表状态
        /// </summary>
        /// <param name="rtuNo"></param>
        /// <returns></returns>
        IQueryable<StateEntity> GetInstrumentStateByRTU(int rtuNo);

        /// <summary>
        /// 查询所有的设备状态，包含W、R和C
        /// </summary>
        /// <returns></returns>
        IQueryable<StateEntity> GetAllState();

        /// <summary>
        /// 查询所有的设备状态，包含W、R和C
        /// </summary>
        /// <param name="stateStatus">1为正常设备，0为故障设备</param>
        /// <returns></returns>
        IQueryable<StateEntity> GetAllState(int stateStatus);

        /// <summary>
        /// 得到所有前置机和RTU状态
        /// </summary>
        /// <returns></returns>
        IQueryable<StateEntity> GetWordAndRTUState();

        /// <summary>
        /// 更新图表中的所有状态的位置
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        bool UpdateStatePosition(List<StateReal> list);
    }
}
