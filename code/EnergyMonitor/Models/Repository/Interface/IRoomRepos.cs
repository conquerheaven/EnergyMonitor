using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnergyMonitor.Models.LinqEntity;
using System.Collections;
using EnergyMonitor.Models.Repository.Entity;

namespace EnergyMonitor.Models.Repository.Interface
{
    /// <summary>
    /// UserMeasurePoint实体操作接口
    /// </summary>
    public interface IRoomRepos
    {
        /// <summary>
        /// 根据用户id获得用户的房间id（这个其实是在UserRoom表中，因为就这一个查询所以放到room这边）
        /// </summary>
        /// <param name="roomID"></param>
        /// <returns></returns>
        String GetRoomByUserID(String UserID);

        /// <summary>
        /// 修改用户所属房间
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="oldRoomID">原来房间ID</param>
        /// <param name="newRoomID">新房间ID</param>
        /// <returns></returns>
        bool ModifyUserRoom(string userID, int oldRoomID, int newRoomID);
        /// <summary>
        /// 获取房间剩余电量
        /// </summary>
        /// <param name="roomID"></param>
        /// <returns></returns>
        double GetCurrentRemVal_Android(int roomID); 
        /// <summary>
        /// 得到所有校区信息
        /// </summary>
        /// <returns></returns>
        IList<SchoolInfo> GetAllSchool();

        /// <summary>
        /// 根据学校ID得到所有的该校区区域
        /// </summary>
        /// <param name="schoolID">校区ID</param>
        /// <returns>校区区域集合</returns>
        IList<SchoolAreaInfo> GetAreaBySchoolID(int schoolID);

        /// <summary>
        /// 根据区域得到该区域的建筑
        /// </summary>
        /// <param name="areaID">区域ID</param>
        /// <returns>建筑集合</returns>
        IList<BuildingBriefInfo> GetBuildingByAreaID(int areaID);

        /// <summary>
        /// 根据建筑ID得到所有房间
        /// </summary>
        /// <param name="buildingID">建筑ID</param>
        /// <returns>房间集合</returns>
        IList<RoomInfo> GetRooomByBuildingID(int buildingID);

        /// <summary>
        /// 增加用户房间
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="roomID">房间号</param>
        /// <returns></returns>
        bool AddUserRoom(string userID, int roomID);

        /// <summary>
        /// 查询房间
        /// </summary>
        /// <param name="roomCode"></param>
        /// <param name="buildingID">非正数查询所有</param>
        /// <returns></returns>
        IList GetRoom(string roomCode, int buildingID);

        /// <summary>
        /// 查询房间
        /// </summary>
        /// <param name="roomCode"></param>
        /// <param name="buildingID">非正数查询所有</param>
        /// <returns></returns>
        IList GetRoom(string roomCode, int buildingID, int skipItems, int pageSize);

        /// <summary>
        /// 查询房间个数
        /// </summary>
        /// <param name="roomCode"></param>
        /// <param name="buildingID">非正数查询所有</param>
        /// <returns></returns>
        int GetRoomCount(string roomCode, int buildingID);

        /// <summary>
        /// 查询房间建筑详细
        /// </summary>
        /// <param name="roomID"></param>
        /// <returns></returns>
        RoomAndBuilding GetRoomAndBuilding(int roomID);

        /// <summary>
        /// 增加房间
        /// </summary>
        /// <param name="roomCode"></param>
        /// <param name="buildingID"></param>
        /// <param name="floor"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        bool AddRoomPart(string roomCode, int buildingID, int? floor, string remark);

        /// <summary>
        /// 修改房间
        /// </summary>
        /// <param name="roomID"></param>
        /// <param name="roomCode"></param>
        /// <param name="buildingID"></param>
        /// <param name="floor"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        bool ModifyRoomPart(int roomID, string roomCode, int buildingID, int? floor, string remark);

        /// <summary>
        /// 删除房间
        /// </summary>
        /// <param name="roomID"></param>
        /// <returns></returns>
        bool DeleteRoom(int roomID);

        /// <summary>
        /// 查询拥有指定房间的用户数
        /// </summary>
        /// <param name="roomID"></param>
        /// <returns></returns>
        int QueryUserCountByRoom(int roomID);

        /// <summary>
        /// 查询房间名称是否存在
        /// </summary>
        /// <param name="roomName"></param>
        /// <param name="buildingID"></param>
        /// <returns></returns>
        bool IsRoomNameExist(string roomName, int? buildingID);

        /// <summary>
        /// 根据房间ID获取房间
        /// </summary>
        /// <param name="roomID"></param>
        /// <returns></returns>
        RoomInfo GetRoomByRoomID(int roomID);
    }
}
