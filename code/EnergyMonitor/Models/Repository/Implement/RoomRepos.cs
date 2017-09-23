 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnergyMonitor.Models.LinqEntity;
using System.Collections;
using System.Data.Linq;
using EnergyMonitor.Models.Repository.Interface;
using EnergyMonitor.Models.Repository.Entity;

namespace EnergyMonitor.Models.Repository.Implement
{
    /// <summary>
    /// UserMeasurePoint实体类数据操作类
    /// </summary>
    public class RoomRepos : IRoomRepos
    {
        private EnergyMonitorDataContext _dataContext;

        public RoomRepos()
        {
            _dataContext = new EnergyMonitorDataContext();
        }

        #region UMPRepos Members

        /// <summary>
        /// 修改用户所属房间
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="oldRoomID">原来房间ID</param>
        /// <param name="newRoomID">新房间ID</param>
        /// <returns></returns>
        public bool ModifyUserRoom(string userID, int oldRoomID, int newRoomID)
        {
            UserRoom ur = null;
            if (newRoomID == 0)
            {
                return false;
            }
            if (oldRoomID == 0)
            {
                ur = new UserRoom
                {
                    USR_ID = userID,
                    RI_ID = newRoomID
                };
                _dataContext.UserRooms.InsertOnSubmit(ur);
            }
            else
            {
                ur = _dataContext.UserRooms.FirstOrDefault(p => p.USR_ID == userID && p.RI_ID == oldRoomID);
                ur.RI_ID = newRoomID;
            }
            try
            {
                _dataContext.SubmitChanges();
                return true;
            }
            catch (ChangeConflictException)
            {
                return false;
            }
        }
        /// <summary>
        /// 获取房间剩余电量
        /// </summary>
        /// <param name="roomID"></param>
        /// <returns></returns>
        public double GetCurrentRemVal_Android(int roomID)
        {
            try
            {
                double? remVal = (from amp in _dataContext.AnalogMeasurePoints
                                  where amp.AMP_RoomID == roomID
                                  select amp.AMP_ValRem).Sum();
                if (remVal.HasValue)
                {
                    return remVal.Value;
                }
                return 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }
        /// <summary>
        /// 得到所有校区信息
        /// </summary>
        /// <returns></returns>
        public IList<SchoolInfo> GetAllSchool()
        {
            var list = from ur in _dataContext.SchoolInfos
                       select ur;
            return list.ToList<SchoolInfo>();
        }

        /// <summary>
        /// 根据学校ID得到所有的该校区区域
        /// </summary>
        /// <param name="schoolID">校区ID</param>
        /// <returns>校区区域集合</returns>
        public IList<SchoolAreaInfo> GetAreaBySchoolID(int schoolID)
        {
            var list = from sa in _dataContext.SchoolAreaInfos
                       where sa.SI_ID == schoolID
                       select sa;

            return list.ToList<SchoolAreaInfo>();
        }

        /// <summary>
        /// 根据区域得到该区域的建筑
        /// </summary>
        /// <param name="areaID">区域ID</param>
        /// <returns>建筑集合</returns>
        public IList<BuildingBriefInfo> GetBuildingByAreaID(int areaID)
        {
            var list = from bb in _dataContext.BuildingBriefInfos
                       where bb.SAI_ID == areaID && bb.BDI_Flag == 1
                       select bb;
            return list.ToList<BuildingBriefInfo>();
        }

        /// <summary>
        /// 根据建筑ID得到所有房间
        /// </summary>
        /// <param name="buildingID">建筑ID</param>
        /// <returns>房间集合</returns>
        public IList<RoomInfo> GetRooomByBuildingID(int buildingID)
        {
            var list = from ri in _dataContext.RoomInfos
                       where ri.BDI_ID == buildingID
                       select ri;
            return list.ToList<RoomInfo>();
        }

        /// <summary>
        /// 增加用户房间
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="roomID">房间号</param>
        /// <returns></returns>
        public bool AddUserRoom(string userID, int roomID)
        {
            try
            {
                UserRoom ur = new UserRoom();
                ur.USR_ID = userID;
                ur.RI_ID = roomID;
                _dataContext.UserRooms.InsertOnSubmit(ur);
                _dataContext.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 查询房间
        /// </summary>
        /// <param name="roomCode"></param>
        /// <param name="buildingID">非正数查询所有</param>
        /// <returns></returns>
        public IList GetRoom(string roomCode, int buildingID)
        {
            if (buildingID > 0)
            {
                var list = from r in _dataContext.RoomInfos
                           from bb in _dataContext.BuildingBriefInfos
                           where r.BDI_ID == bb.BDI_ID && r.BDI_ID == buildingID && r.RI_RoomCode.Contains(roomCode)
                           select new RoomAndBuilding
                           {
                               RoomID = r.RI_ID,
                               RoomCode = r.RI_RoomCode,
                               BuildingID = r.BDI_ID,
                               BuildingName = bb.BDI_Name,
                               Floor = r.RI_Floor,
                               Remark = r.RI_Remark
                           };
                return list.ToList();
            }
            else
            {
                var list = from r in _dataContext.RoomInfos
                           from bb in _dataContext.BuildingBriefInfos
                           where r.BDI_ID == bb.BDI_ID && r.RI_RoomCode.Contains(roomCode)
                           select new RoomAndBuilding
                           {
                               RoomID = r.RI_ID,
                               RoomCode = r.RI_RoomCode,
                               BuildingID = r.BDI_ID,
                               BuildingName = bb.BDI_Name,
                               Floor = r.RI_Floor,
                               Remark = r.RI_Remark
                           };
                return list.ToList();
            }
        }

        /// <summary>
        /// 查询房间
        /// </summary>
        /// <param name="roomCode"></param>
        /// <param name="buildingID">非正数查询所有</param>
        /// <returns></returns>
        public IList GetRoom(string roomCode, int buildingID, int skipItems, int pageSize)
        {
            if (buildingID > 0)
            {
                var list = (from r in _dataContext.RoomInfos
                            from bb in _dataContext.BuildingBriefInfos
                            where r.BDI_ID == bb.BDI_ID && r.BDI_ID == buildingID && r.RI_RoomCode.Contains(roomCode)
                            select new RoomAndBuilding
                            {
                                RoomID = r.RI_ID,
                                RoomCode = r.RI_RoomCode,
                                BuildingID = r.BDI_ID,
                                BuildingName = bb.BDI_Name,
                                Floor = r.RI_Floor,
                                Remark = r.RI_Remark
                            }).Skip(skipItems).Take(pageSize);
                return list.ToList();
            }
            else
            {
                var list = (from r in _dataContext.RoomInfos
                            from bb in _dataContext.BuildingBriefInfos
                            where r.BDI_ID == bb.BDI_ID && r.RI_RoomCode.Contains(roomCode)
                            select new RoomAndBuilding
                            {
                                RoomID = r.RI_ID,
                                RoomCode = r.RI_RoomCode,
                                BuildingID = r.BDI_ID,
                                BuildingName = bb.BDI_Name,
                                Floor = r.RI_Floor,
                                Remark = r.RI_Remark
                            }).Skip(skipItems).Take(pageSize);
                return list.ToList();
            }
        }

        /// <summary>
        /// 查询房间个数
        /// </summary>
        /// <param name="roomCode"></param>
        /// <param name="buildingID">非正数查询所有</param>
        /// <returns></returns>
        public int GetRoomCount(string roomCode, int buildingID)
        {
            if (buildingID > 0)
            {
                var count = (from r in _dataContext.RoomInfos
                            from bb in _dataContext.BuildingBriefInfos
                            where r.BDI_ID == bb.BDI_ID && r.BDI_ID == buildingID && r.RI_RoomCode.Contains(roomCode)
                            select new RoomAndBuilding
                            {
                                RoomID = r.RI_ID,
                                RoomCode = r.RI_RoomCode,
                                BuildingID = r.BDI_ID,
                                BuildingName = bb.BDI_Name
                            }).Count();
                return count;
            }
            else
            {
                var count = (from r in _dataContext.RoomInfos
                            from bb in _dataContext.BuildingBriefInfos
                            where r.BDI_ID == bb.BDI_ID && r.RI_RoomCode.Contains(roomCode)
                            select new RoomAndBuilding
                            {
                                RoomID = r.RI_ID,
                                RoomCode = r.RI_RoomCode,
                                BuildingID = r.BDI_ID,
                                BuildingName = bb.BDI_Name
                            }).Count();
                return count;
            }
        }

        /// <summary>
        /// 查询房间建筑详细
        /// </summary>
        /// <param name="roomID"></param>
        /// <returns></returns>
        public RoomAndBuilding GetRoomAndBuilding(int roomID)
        {
            var list = from r in _dataContext.RoomInfos
                       from bb in _dataContext.BuildingBriefInfos
                       from sai in _dataContext.SchoolAreaInfos
                       from si in _dataContext.SchoolInfos
                       where r.BDI_ID == bb.BDI_ID && r.RI_ID == roomID && bb.SAI_ID == sai.SAI_ID && sai.SI_ID == si.SI_ID
                       select new RoomAndBuilding
                       {
                           RoomID = r.RI_ID,
                           RoomCode = r.RI_RoomCode,
                           Floor = r.RI_Floor,
                           Remark = r.RI_Remark,
                           BuildingID = r.BDI_ID,
                           BuildingName = bb.BDI_Name,
                           AreaID = sai.SAI_ID,
                           AreaName = sai.SAI_Name,
                           SchoolID = si.SI_ID,
                           SchoolName = si.SI_Name
                       };
            return list.Single();
        }

        /// <summary>
        /// 增加房间
        /// </summary>
        /// <param name="roomCode"></param>
        /// <param name="buildingID"></param>
        /// <param name="floor"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public bool AddRoomPart(string roomCode, int buildingID, int? floor, string remark)
        {
            try
            {
                RoomInfo room = new RoomInfo();
                room.RI_RoomCode = roomCode;
                room.BDI_ID = buildingID;
                room.RI_Floor = floor;
                room.RI_Remark = remark;
                _dataContext.RoomInfos.InsertOnSubmit(room);
                _dataContext.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 修改房间
        /// </summary>
        /// <param name="roomID"></param>
        /// <param name="roomCode"></param>
        /// <param name="buildingID"></param>
        /// <param name="floor"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public bool ModifyRoomPart(int roomID, string roomCode, int buildingID, int? floor, string remark)
        {
            try
            {
                RoomInfo room = _dataContext.RoomInfos.Single(x => x.RI_ID == roomID);
                room.RI_RoomCode = roomCode;
                if (room.BDI_ID != buildingID)
                {
                    room.BDI_ID = buildingID;
                    // 修改AMP表中对应的房间建筑
                    if (_dataContext.AnalogMeasurePoints.Where(x => x.AMP_RoomID == roomID).Count() > 0)
                    {
                        _dataContext.ExecuteCommand("update AnalogMeasurePoint set AMP_BuildingID={0} where AMP_RoomID={1}", buildingID, roomID);
                        // 修改父测点编号
                        var modifiedAMPs = _dataContext.AnalogMeasurePoints.Where(x => x.AMP_RoomID == roomID && x.AMP_ParentNo > 1).ToList();
                        foreach (var item in modifiedAMPs)
                        {
                            var parentPoint = _dataContext.AnalogMeasurePoints.Where(x => x.AMP_CptFlag == 0 && x.AMP_Statistic == 1 && x.AMP_BuildingID == buildingID && x.AMP_RoomID == 0 && x.AMP_PowerType == item.AMP_PowerType).SingleOrDefault();
                            if (parentPoint != null)
                            {
                                item.AMP_ParentNo = parentPoint.AMP_AnalogNo;
                            }
                            else
                            {
                                item.AMP_ParentNo = 0;
                            }
                        }
                    }
                }
                room.RI_Floor = floor;
                room.RI_Remark = remark;
                _dataContext.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 根据用户id获得用户的房间id（这个其实是在UserRoom表中，因为就这一个查询所以放到room这边）
        /// </summary>
        /// <param name="roomID"></param>
        /// <returns></returns>
        public String GetRoomByUserID(String UserID)
        {  
            return _dataContext.UserRooms.Where(x => x.USR_ID == UserID).Select(x => x.RI_ID).FirstOrDefault().ToString();
        }

        /// <summary>
        /// 删除房间
        /// </summary>
        /// <param name="roomID"></param>
        /// <returns></returns>
        public bool DeleteRoom(int roomID)
        {
            try
            {
                _dataContext.RoomInfos.DeleteOnSubmit(_dataContext.RoomInfos.Single(x => x.RI_ID == roomID));
                _dataContext.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 查询拥有指定房间的用户数
        /// </summary>
        /// <param name="roomID"></param>
        /// <returns></returns>
        public int QueryUserCountByRoom(int roomID)
        {
            return _dataContext.UserRooms.Where(x => x.RI_ID == roomID).Count();
        }

        /// <summary>
        /// 查询房间名称是否存在
        /// </summary>
        /// <param name="roomName"></param>
        /// <param name="buildingID"></param>
        /// <returns></returns>
        public bool IsRoomNameExist(string roomName, int? buildingID)
        {
            var query = _dataContext.RoomInfos.Where(x => x.RI_RoomCode == roomName);
            if (buildingID != null && buildingID.Value > 0)
            {
                query = query.Where(x => x.BDI_ID == buildingID.Value);
            }
            if (query.Count() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion


        public RoomInfo GetRoomByRoomID(int roomID)
        {
            RoomInfo roomInfo = _dataContext.RoomInfos.Where(x => x.RI_ID == roomID).SingleOrDefault();
            return roomInfo;
        }
    }
}
