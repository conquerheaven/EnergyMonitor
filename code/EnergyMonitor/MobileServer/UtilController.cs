using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Collections;
using EnergyMonitor.Models.Repository.Interface;
using EnergyMonitor.Models.Repository.Implement;
using EnergyMonitor.Models.Repository.Entity;
using EnergyMonitor.Models.LinqEntity;
using System.Linq;
using System.Security.Cryptography;
using System.IO;
using System.Web;
using System.Text;

namespace EnergyMonitor.Controllers.Mobile
{
    /// <summary>
    /// 工具
    /// </summary>
    public class UtilController : Controller
    {
        private IPowerClassRepos _powerClassRepos = null;
        private IRoomRepos _roomRepos = null;
        private IAMPRepos _ampRepos = null;
        private IAnalogHistoryRepos _analogHistoryRepos = null;
        private IBuyHistoryRepos _buyHistoryRepos = null;
        private ISchoolAreaRepos _schoolAreaRepos = null;
        private IUserRepos _userRepos = null;
        private IFunctionRepos _functionRepos = null;
        

        public UtilController()
            : this(new UserRepos(), new FunctionRepos(), new PowerClassRepos(), new RoomRepos(), new SchoolAreaRepos(),new AnalogHistoryRepos())
        {
        }
        public UtilController(IUserRepos userRepos, IFunctionRepos functionRepos, IPowerClassRepos powerClassRepos, IRoomRepos roomRepos, ISchoolAreaRepos schoolAreaRepos, IAnalogHistoryRepos analogHistoryRepos)
        {
            _userRepos = userRepos;
            _functionRepos = functionRepos;
            _powerClassRepos = powerClassRepos;
            _roomRepos = roomRepos;
            _analogHistoryRepos = analogHistoryRepos;
        }
        /// <summary>
        /// SHA1加密字符串
        /// </summary>
        /// <param name="sourceStr">待加密字符串</param>
        /// <returns></returns>
        public static string SHA1Encrypt(string sourceStr)
        {
            byte[] strRes = Encoding.UTF8.GetBytes(sourceStr);
            HashAlgorithm iSHA = new SHA1CryptoServiceProvider();
            strRes = iSHA.ComputeHash(strRes);
            StringBuilder EnText = new StringBuilder();
            foreach (byte iByte in strRes)
            {
                EnText.AppendFormat("{0:x2}", iByte);
            }
            return EnText.ToString();
        }
        /// <summary>
        /// 验证登录用户是否存在
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public JsonResult CheckLoginAjax(string userID,string password) {
            IList list = new ArrayList();
            Log userLog = new Log();
            userLog.LOG_UserID = userID;
            userLog.LOG_FuctionID = "";
            userLog.LOG_actionType = 1;
            userLog.LOG_actionDate = DateTime.Now;
            userLog.LOG_actionDesc = String.Format("手机登陆");
            var logRepos = LogRepos.LogInstance();
            logRepos.AddLog(userLog);
            //用户名为空返回-1
            if (String.IsNullOrEmpty(userID))
            {
                list.Add(new
                {
                    status = -1,
                });
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            string encryptPassword = SHA1Encrypt(userID + password);
            var user = _userRepos.GetUserByIDAndPassword(userID, encryptPassword);
            //用户名不存在返回0
            if (user == null)
            {
                list.Add(new
                {
                    status = 0,
                });
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            else
            {
                list.Add(new
                {
                    status = 1,
                });
                 return Json(list, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// 使用Ajax得到剩余电量
        /// </summary>
        /// <returns>Json格式所有校区信息</returns>
        [OutputCache(Duration = 5, VaryByParam = "none")]
        public JsonResult GetRemainEnergyAjax(string userID, int type, int startHour, int endHour)
        {
            //建筑数目
            int BUILDINGNO = 140;
            int ROOMNUM = 14;
            //统计小时数目
            int HOURS = -8;
            // if (Request.IsAjaxRequest())
            if (1 == 1)
            {
                IList<UserRoomFullName> userRoomList = null;
                IList<ChartStatisEntity> statisList = null;
                IList<ChartStatisEntity> northList = null;
                IList list = new ArrayList();
                IList<ChartStatisEntity> tempList = null;
               
                userRoomList = _userRepos.GetUserRelatedRooms(userID);
                Session["userRoomInfo"] = userRoomList;

                if (type == 1)
                {
                    foreach (var item in userRoomList)
                    {
                        int roomID = item.RIID;
                        statisList = GetElecAjax(4, roomID, DateTime.Now.AddHours(HOURS - 1), DateTime.Now, "hour", "001_001001_001002_001003_001004_001005_001006", 0, 0, startHour, endHour);
                        northList = (GetElecAjax(3, 1, DateTime.Now.AddHours(HOURS - 1), DateTime.Now, "hour", "001_001001_001002_001003_001004_001005_001006", 0, 0, startHour, endHour).ToList());
                        for (int i = 1; i < 140; i++)
                        {
                            tempList = (GetElecAjax(3, i, DateTime.Now.AddHours(HOURS - 1), DateTime.Now, "hour", "001_001001_001002_001003_001004_001005_001006", 0, 0, startHour, endHour));
                            int j = 0;
                            foreach (var tempItem in tempList)
                            {
                                if (((ChartStatisEntity)tempItem).StatisVal > 200 || ((ChartStatisEntity)tempItem).StatisVal <= 0)
                                {
                                    BUILDINGNO--;
                                    break;
                                }
                                Console.WriteLine("northList[i]" + ((ChartStatisEntity)(northList[j])).StatisVal);
                                ((ChartStatisEntity)(northList[j])).StatisVal = ((ChartStatisEntity)(northList[j])).StatisVal + ((ChartStatisEntity)tempItem).StatisVal;
                                j++;
                            }
                        }
                        double remVal = _roomRepos.GetCurrentRemVal_Android(roomID);
                        String remValStr = remVal.ToString("f2");
                        remVal = System.Double.Parse(remValStr);
                        //remDic.Add(roomID, remVal);
                        list.Add(new
                        {
                            remVal = remVal,

                            //status = 2
                        });
                        IList partList = new ArrayList();
                        IList partNorthList = new ArrayList();
                        foreach (var statisitem in statisList)
                        {
                            partList.Add(new
                            {
                                data = statisitem.TimeBlock,
                                energysum = statisitem.StatisVal.ToString("f2"),
                            });
                        }
                        foreach (var northItem in northList)
                        {
                            partNorthList.Add(new
                            {
                                data = ((ChartStatisEntity)northItem).TimeBlock,
                                energysum = (((ChartStatisEntity)northItem).StatisVal / (BUILDINGNO * ROOMNUM)).ToString("f2"),
                            });
                        }

                        list.Add(new { dataList = partList, });
                        list.Add(new { northList = partNorthList });
                    }
                    return Json(list, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    foreach (var item in userRoomList)
                    {
                        int roomID = item.RIID;
                        double remVal = _roomRepos.GetCurrentRemVal_Android(roomID);
                        String remValStr = remVal.ToString("f2");
                        remVal = System.Double.Parse(remValStr);
                        //remDic.Add(roomID, remVal);
                        list.Add(new
                        {
                            remVal = remVal,
                            //status = 2
                        });
                    }
                    return Json(list, JsonRequestBehavior.AllowGet);
                }

                //var schools = _roomRepos.GetAllSchool();
                //double remVal = _roomRepos.GetCurrentRemVal_Android(702);

                //foreach (var item in schools)
                //{
                //     list.Add(new
                //    {
                //dataID = item.SI_ID,
                //dataValue = item.SI_Name,
                //remVal = remVal
                //    });
                // }

            }
            return null;
        }
        /// <summary>
        /// 使用Ajax得到校区信息
        /// </summary>
        /// <returns>Json格式所有校区信息</returns>
        [OutputCache(Duration = 5, VaryByParam = "none")]
        public JsonResult GetAllShoolAjax(string userID, int type, int startHour, int endHour)
        {
            //建筑数目
            int BUILDINGNO = 140;
            int ROOMNUM = 14;
            //统计小时数目
            int HOURS = -8;
            int POSHOURS = 8;
            // if (Request.IsAjaxRequest())
            if (1 == 1)
            {
                IList<UserRoomFullName> userRoomList = null;
                IList<ChartStatisEntity> statisList = null;
                IList<ChartStatisEntity> northList = null;
                IList list = new ArrayList();
                IList<ChartStatisEntity> tempList = null;
                /**
                //用户名为空返回-1
                if (String.IsNullOrEmpty(userID)) {
                    list.Add(new { 
                        status = -1,
                    });
                    return Json(list, JsonRequestBehavior.AllowGet);
                }
                var user = _userRepos.GetUserByID(userID);
                //用户名不存在返回0
                if (user == null)
                {
                    list.Add(new
                    {
                        status = 0,
                    });
                    return Json(list, JsonRequestBehavior.AllowGet);
                }else {
                    list.Add(new
                    {
                        status = 1,
                    });
                   // return Json(list, JsonRequestBehavior.AllowGet);
                }
                **/
                userRoomList = _userRepos.GetUserRelatedRooms(userID);
                Session["userRoomInfo"] = userRoomList;
                
                if (type == 1)
                {
                    foreach (var item in userRoomList)
                    {
                        int roomID = item.RIID;
                        statisList = GetElecAjax(4, roomID, DateTime.Now.AddHours(HOURS-1), DateTime.Now, "hour", "001_001001_001002_001003_001004_001005_001006", 0, 0, startHour, endHour);
                        //获取第一个满足条件的楼宇，有8个值
                        int it = 0;
                        for (it = 0; it < 140; it++) 
                        {

                            northList = (GetElecAjax(3, it, DateTime.Now.AddHours(HOURS - 1), DateTime.Now, "hour", "001_001001_001002_001003_001004_001005_001006", 0, 0, startHour, endHour).ToList());
                            if (northList != null && northList.Count() == POSHOURS)
                            {
                                int j = 0;
                                int normalflag = 1;
                                foreach (var tempItem in northList)
                                {
                                    if (((ChartStatisEntity)tempItem).StatisVal > 100 || ((ChartStatisEntity)tempItem).StatisVal < 0)
                                    {
                                        normalflag = 0;
                                        BUILDINGNO--;
                                        break;
                                    }
                                }
                                if (normalflag == 1)
                                {
                                    break;
                                }
                               
                            }
                            else {
                                BUILDINGNO--;
                            }
                        }
                            
                        for (int i = it; i < 140; i++) {
                            tempList = (GetElecAjax(3, i, DateTime.Now.AddHours(HOURS-1), DateTime.Now, "hour", "001_001001_001002_001003_001004_001005_001006", 0, 0, startHour, endHour));
                            
                            if (tempList==null || (tempList != null && tempList.Count() < POSHOURS))
                            {
                                BUILDINGNO--;
                               
                                
                            }else {
                                int j = 0;
                                int normalflag = 1;
                                foreach (var tempItem in tempList)
                                {
                                    if (((ChartStatisEntity)tempItem).StatisVal > 100 || ((ChartStatisEntity)tempItem).StatisVal < 0)
                                    {
                                        normalflag = 0;
                                        BUILDINGNO--;
                                        break;
                                    }
                                }
                                if (normalflag == 1) 
                                {
                                    j = 0;
                                    foreach (var tempItem in tempList)
                                    {
                                       // Console.WriteLine("northList[i]" + ((ChartStatisEntity)(northList[j])).StatisVal);
                                        ((ChartStatisEntity)(northList[j])).StatisVal = ((ChartStatisEntity)(northList[j])).StatisVal + ((ChartStatisEntity)tempItem).StatisVal;
                                        j++;  
                                    }
                                    normalflag = 0;
                                }
                            }
                        }
                        double remVal = _roomRepos.GetCurrentRemVal_Android(roomID);
                        String remValStr = remVal.ToString("f2");
                        remVal = System.Double.Parse(remValStr);
                        //remDic.Add(roomID, remVal);
                        list.Add(new
                        {
                            remVal = remVal,
                            
                            //status = 2
                        });
                        IList partList = new ArrayList();
                        IList partNorthList = new ArrayList();
                        foreach (var statisitem in statisList) {
                            partList.Add(new
                            {
                                data = statisitem.TimeBlock,
                                energysum = statisitem.StatisVal.ToString("f2"),
                            });   
                        }
                        foreach (var northItem in northList) {
                            partNorthList.Add(new
                            {
                                data = ((ChartStatisEntity)northItem).TimeBlock,
                                energysum = (((ChartStatisEntity)northItem).StatisVal / (BUILDINGNO * ROOMNUM)).ToString("f2"),
                            });
                        }

                        list.Add(new { dataList = partList, });
                        list.Add(new { northList = partNorthList });
                    }
                    return Json(list, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    foreach (var item in userRoomList)
                    {
                        int roomID = item.RIID;
                        double remVal = _roomRepos.GetCurrentRemVal_Android(roomID);
                        String remValStr = remVal.ToString("f2");
                        remVal = System.Double.Parse(remValStr);
                        //remDic.Add(roomID, remVal);
                        list.Add(new
                        {
                            remVal = remVal,
                            //status = 2
                        });
                    }
                    return Json(list, JsonRequestBehavior.AllowGet);
                }

                //var schools = _roomRepos.GetAllSchool();
                //double remVal = _roomRepos.GetCurrentRemVal_Android(702);

                //foreach (var item in schools)
                //{
                //     list.Add(new
                //    {
                //dataID = item.SI_ID,
                //dataValue = item.SI_Name,
                //remVal = remVal
                //    });
                // }
                
            }
            return null;
        }
        /// <summary>
        /// android获取用电统计数据
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="objIDs"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="granularity"></param>
        /// <param name="powerType"></param>
        /// <param name="weekType"></param>
        /// <param name="hourType"></param>
        /// <param name="startHour"></param>
        /// <param name="endHour"></param>
        /// <returns></returns>
        public IList<ChartStatisEntity> GetElecAjax(int objType, int objIDs, DateTime startTime, DateTime endTime, string granularity, string powerType, int? weekType, int? hourType, int startHour, int endHour)
        {
            //开始点
            int startPoint = 0;
            int duration = 8;
            if (1 == 1)
            {
                string[] powerTypes = null;
                if (!string.IsNullOrWhiteSpace(powerType))
                {
                    powerTypes = powerType.Split(new char[] { '_' });
                }
                DayOfWeek? dw = null;
                double average = 0;
                IList<ChartStatisEntity> list = null;
                if (1 == 1)
                {
                    if (granularity == "day")//按天统计
                    {
                        list = _analogHistoryRepos.GetEnergyStatisDay(objType, objIDs, startTime, endTime, powerTypes, 1).Skip(startPoint).Take(duration).ToList();
                    }
                    else if (granularity == "month")//按月统计
                    {
                        list = _analogHistoryRepos.GetEnergyStatisMonth(objType, objIDs, startTime, endTime, powerTypes, 1).Skip(startPoint).Take(duration).ToList();
                    }
                    else if (granularity == "year")//按年统计
                    {
                        list = _analogHistoryRepos.GetEnergyStatisYear(objType, objIDs, startTime, endTime, powerTypes, 1).Skip(startPoint).Take(duration).ToList();
                    }
                    else if (granularity == "week") //星期
                    {
                        var query = _analogHistoryRepos.GetEnergyStatisDay(objType, objIDs, startTime, endTime, powerTypes, 1);
                        if (dw != null)
                        {
                            query = query.Where(x => x.Time.DayOfWeek == dw.Value);
                        }
                        list = query.Skip(startPoint).Take(duration).ToList();
                    }
                    else //小时
                    {
                        if (hourType != null && hourType == 1 && startHour < endHour) // 指定小时段
                        {
                            list = _analogHistoryRepos.GetEnergyStatisSpecialHours(objType, objIDs, startTime, endTime, powerTypes, startHour, endHour, 1).Skip(startPoint).Take(duration).ToList();
                        }
                        else // 每小时
                        {
                            list = _analogHistoryRepos.GetEnergyStatisHour(objType, objIDs, startTime, endTime, powerTypes, 1).Skip(startPoint).Take(duration).ToList();
                        }
                    }
                    if (list != null)
                    {
                        average = list.Sum(x => x.StatisVal) / list.Count;
                    }
                }
                return list;
            }
        }

        /// <summary>
        /// 当前用电剩余额
        /// </summary>
        /// <returns></returns>
        public JsonResult EnergyBrief(string userID)
        {
            if (Session["loginUser"] == null)//已经登录
            {
                if (String.IsNullOrEmpty(userID))
                {
                    var reData = new
                    {
                        //没有填写用户名
                        status = -1
                    };
                    return Json(reData, JsonRequestBehavior.AllowGet);
                }
                var user = _userRepos.GetUserByID(userID);

                if (user != null)
                {

                    Session["loginUser"] = user;
                    Session["loginUserName"] = user.USR_Name;
                    Session["userRoomInfo"] = null;

                    // 记录用户登录时间IP和登录时间
                    DateTime loginTime = DateTime.Now;
                    Session["loginTime"] = loginTime;
                    string loginIP;
                    if (HttpContext.Request.ServerVariables["HTTP_VIA"] != null)
                    {
                        loginIP = HttpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
                    }
                    else
                    {
                        loginIP = HttpContext.Request.ServerVariables["REMOTE_ADDR"].ToString();
                    }
                    Session["loginIP"] = loginIP;
                    if (user != null)
                    {
                        _userRepos.ModifyUserInfo(user.USR_ID, loginTime.ToString(), loginIP);
                    }

                    if (user.USR_RoleID == 1)//先写死为1认为是学生用户
                    {
                        var reData = new
                        {
                            //非管理员用户
                            status = 1
                        };
                        return Json(reData, JsonRequestBehavior.AllowGet);
                        //return RedirectToAction("Welcome", "Home", new { area = "User" });   
                        //return RedirectToRoute("~/User/Home/Welcome");
                    }
                    else//非学生用户
                    {
                        

                        var reData = new
                        {
                            //管理员用户
                            status = 2
                        };
                       
                    }
                }
                else
                {
                    Session["userID"] = userID;
                    Session["userLoginTime"] = DateTime.Now;
                    //return RedirectToAction("ForwardAddInfo", "User", new { area = "User" });
                    var reData = new
                    {
                        //用户没有注册
                        status = 0
                    };
                    return Json(reData, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {

                var user = Session["loginUser"] as EnergyMonitor.Models.LinqEntity.User;
                if (user.USR_RoleID == 1)//先写死为1认为是学生用户
                {
                    //return RedirectToAction("Welcome", "Home", new { area = "User" });
                    return null;
                }
                else//非学生用户
                {
                    IList<UserRoomFullName> userRoomList = null;
                    if (Session["userRoomInfo"] != null)
                    {
                        userRoomList = Session["userRoomInfo"] as IList<UserRoomFullName>;
                    }
                    else
                    {
                        EnergyMonitor.Models.LinqEntity.User loginUser = user;
                        userRoomList = _userRepos.GetUserRelatedRooms(loginUser.USR_ID);
                        Session["userRoomInfo"] = userRoomList;
                    }
                   // ViewBag.userRoomList = userRoomList;
                    DateTime endTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-1"));
                    // 开始时间为前三个月
                    DateTime startTime = endTime.AddMonths(-3);
                    IDictionary<int, IList<ChartStatisEntity>> dic = new Dictionary<int, IList<ChartStatisEntity>>();
                    // 当月用电
                    IDictionary<int, double> currentDic = new Dictionary<int, double>();
                    // 剩余用电
                    IDictionary<int, double> remDic = new Dictionary<int, double>();
                    // 当前表值
                    IDictionary<int, IList> ampDic = new Dictionary<int, IList>();
                    IList jsonList = new ArrayList();
                    foreach (var item in userRoomList)
                    {
                        int roomID = item.RIID;
                        //var ampList = _ampRepos.GetRealTimeEnergy(roomID);

                        //ampDic.Add(roomID, ampList);
                       // var list = _analogHistoryRepos.GetEnergyMonthConsume(roomID, startTime, endTime);
                        //double val = _analogHistoryRepos.GetCurrentMonthEnergy(roomID);
                        //currentDic.Add(roomID, val);
                       // dic.Add(roomID, list);
                        double remVal = _analogHistoryRepos.GetCurrentRemVal(roomID);
                        //remDic.Add(roomID, remVal);
                        jsonList.Add(new
                        {
                            remainEnergy = remVal,
                            status = 2
                        });
                    }
                   // ViewBag.currentDic = currentDic;
                   // ViewBag.remDic = remDic;
                   // ViewBag.ampDic = ampDic;

                    return Json(jsonList, JsonRequestBehavior.AllowGet);
                    //return RedirectToAction("Welcome", "Home", new { area = "Admin" });
                    //return null;
                }
            }
            return null;
            //return View(dic);
        }

        /// <summary>
        /// 使用Ajax根据校区ID得到区域
        /// </summary>
        /// <param name="schoolID">校区ID</param>
        /// <returns>Json格式区域集合</returns>
        public ActionResult GetAreasBySchoolIDAjax(int schoolID)
        {
            if (Request.IsAjaxRequest() && schoolID > 0)
            {
                var areas = _roomRepos.GetAreaBySchoolID(schoolID);
                IList list = new ArrayList();
                foreach (var item in areas)
                {
                    list.Add(new
                    {
                        dataID = item.SAI_ID,
                        dataValue = item.SAI_Name
                    });
                }
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 根据区域得到建筑
        /// </summary>
        /// <param name="areaID">区域ID</param>
        /// <returns></returns>
        public ActionResult GetBuildingByAreaAjax(int areaID)
        {
            if (areaID > 0)
            {
                var buildings = _roomRepos.GetBuildingByAreaID(areaID);
                IList list = new ArrayList();
                foreach (var item in buildings)
                {
                    list.Add(new
                    {
                        dataID = item.BDI_ID,
                        dataValue = item.BDI_Name
                    });
                }
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 根据建筑ID得到所有房间
        /// </summary>
        /// <param name="buildingID">建筑ID</param>
        /// <returns></returns>
        public ActionResult GetRoomsByBIDAjax(int buildingID)
        {
            if (Request.IsAjaxRequest() && buildingID > 0)
            {
                var rooms = _roomRepos.GetRooomByBuildingID(buildingID);
                IList list = new ArrayList();
                foreach (var item in rooms)
                {
                    list.Add(new
                    {
                        dataID = item.RI_ID,
                        dataValue = item.RI_RoomCode
                    });
                }
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

    }
}
