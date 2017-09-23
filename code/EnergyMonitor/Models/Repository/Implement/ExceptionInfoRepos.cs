using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnergyMonitor.Models.Repository.Interface;
using EnergyMonitor.Models.LinqEntity;
using EnergyMonitor.Models.Repository.Entity;
using System.Collections;


namespace EnergyMonitor.Models.Repository.Implement
{
    public class ExceptionInfoRepos : IExceptionInfoRepos
    {
        private EnergyMonitorDataContext _dataContext;

        public ExceptionInfoRepos()
        {
            _dataContext = new EnergyMonitorDataContext();
        }

        #region IAnalogHistory Members

        /// <summary>
        /// 根据异常类型查询指定时间段的电力异常值
        /// </summary>
        /// <param name="exceptionType"></param>         
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>      
        /// <returns></returns>        
        public IList<ExceptionInfoEntity> GetExceptionInfo(string exceptionType, DateTime startTime, DateTime endTime)
        {
            // 修改时间使得与给出界面上的时间范围一致
            startTime = new DateTime(startTime.Year, startTime.Month, startTime.Day, 0, 0, 0);
            endTime = new DateTime(endTime.Year, endTime.Month, endTime.Day, 23, 59, 59);
             IQueryable<ExceptionInfoEntity> list = null;
             switch (exceptionType)
             {
                 case "1":
                           list = from ei in _dataContext.ExceptionInfo
                                    where ei.EI_ExceptionType == exceptionType && ei.EI_Date >= startTime && ei.EI_Date <= endTime
                                    orderby ei.EI_AnalogNo , ei.EI_Date descending
                                    select new ExceptionInfoEntity
                                    {
                                        AnalogNo = ei.EI_AnalogNo,
                                        AnalogName = ei.EI_Name,
                                        ExceptionDate = ei.EI_Date,
                                        ExceptionName = ei.EI_ExceptionName
                                    };
                           break;                
                 case "3":                   
                          list = from ei in _dataContext.ExceptionInfo
                                 join ai in _dataContext.AnalogMeasurePoints on ei.EI_AnalogNo equals ai.AMP_AnalogNo into tempai
                                 from amp in tempai.DefaultIfEmpty()
                                 join si in _dataContext.SchoolInfos on amp.AMP_SchooldID equals si.SI_ID into tempsi
                                from si2 in tempsi.DefaultIfEmpty()
                                join sai in _dataContext.SchoolAreaInfos on amp.AMP_SAreaID equals sai.SAI_ID into tempsai
                                from sai2 in tempsai.DefaultIfEmpty()
                                join bbi in _dataContext.BuildingBriefInfos on amp.AMP_BuildingID equals bbi.BDI_ID into tempbbi
                                from bbi2 in tempbbi.DefaultIfEmpty()
                                join ri in _dataContext.RoomInfos on amp.AMP_RoomID equals ri.RI_ID into tempri
                                from ri2 in tempri.DefaultIfEmpty()
                                   where ei.EI_ExceptionType == exceptionType && ei.EI_Date >= startTime && ei.EI_Date <= endTime
                                 orderby ei.EI_AnalogNo, ei.EI_Date descending
                                   select new ExceptionInfoEntity
                                    {
                                        AnalogNo = ei.EI_AnalogNo,
                                        AnalogName = ei.EI_Name,
                                        ExceptionDate = ei.EI_Date,
                                        HistoryVal = (double)ei.EI_HistoryValue,
                                        CurrentVal = (double)ei.EI_CurrentValue,
                                        ExceptionName = ei.EI_ExceptionName,
                                        SName = si2.SI_Name,
                                        AName = sai2.SAI_Name,
                                        BName = bbi2.BDI_Name,
                                        RName = ri2.RI_RoomCode
                                    };                          
                          break;            
                 case "4":
                          list = from ei in _dataContext.ExceptionInfo
                                 join ai in _dataContext.AnalogMeasurePoints on ei.EI_AnalogNo equals ai.AMP_AnalogNo into tempai
                                 from amp in tempai.DefaultIfEmpty()
                                 join si in _dataContext.SchoolInfos on amp.AMP_SchooldID equals si.SI_ID into tempsi
                                 from si2 in tempsi.DefaultIfEmpty()
                                 join sai in _dataContext.SchoolAreaInfos on amp.AMP_SAreaID equals sai.SAI_ID into tempsai
                                 from sai2 in tempsai.DefaultIfEmpty()
                                 join bbi in _dataContext.BuildingBriefInfos on amp.AMP_BuildingID equals bbi.BDI_ID into tempbbi
                                 from bbi2 in tempbbi.DefaultIfEmpty()
                                 join ri in _dataContext.RoomInfos on amp.AMP_RoomID equals ri.RI_ID into tempri
                                 from ri2 in tempri.DefaultIfEmpty()
                                   where ei.EI_ExceptionType == exceptionType && ei.EI_Date >= startTime && ei.EI_Date <= endTime
                                 orderby ei.EI_AnalogNo, ei.EI_Date descending
                                   select new ExceptionInfoEntity
                                    {
                                        AnalogNo = ei.EI_AnalogNo,
                                        AnalogName = ei.EI_Name,
                                        ExceptionDate = ei.EI_Date,
                                        HistoryVal = (double)ei.EI_HistoryValue,
                                        CurrentVal = (double)ei.EI_CurrentValue,
                                        ExceptionName = ei.EI_ExceptionName,
                                        SName = si2.SI_Name,
                                        AName = sai2.SAI_Name,
                                        BName = bbi2.BDI_Name,
                                        RName = ri2.RI_RoomCode
                                    };                          
                          break;                            
                 default:
                          break;
             }           
             return list.ToList();
        }

        /// <summary>
        /// 查询前一天断值测点信息
        /// </summary>           
        /// <returns></returns>   
        public IList<ExceptionInfoEntity> GetDiscontinuousValueInfo( )
        {
            IQueryable<ExceptionInfoEntity> list = null;
            list = from ei in _dataContext.ExceptionInfo
                   where ei.EI_ExceptionType == "2"
                   select new ExceptionInfoEntity
                   {
                       AnalogNo = ei.EI_AnalogNo,
                       AnalogName = ei.EI_Name,
                       ExceptionDate = ei.EI_Date,
                       ExceptionName = ei.EI_ExceptionName
                   };
            return list.ToList();
        }

        /// <summary>
        /// 获取所有机器管理人员的邮箱
        /// </summary>          
        /// <returns></returns>    
        public IList  GetAllEmail()
        {
            return _dataContext.SetEmail.ToList();
        }

        /// <summary>
        /// 获取所有联系人手机号码
        /// </summary>          
        /// <returns></returns>    
        public IList GetAllMessage()
        {
            return _dataContext.setMessages.ToList();
        }

        /// <summary>
        /// 根据键值对修改机器管理人员邮箱
        /// </summary>
        /// <param name="newDic"></param>
        /// <returns></returns>
        public bool ModifyEmail(IDictionary newDic)
        {
            try
            {
                foreach (var key in newDic.Keys)
                {
                    var query = _dataContext.SetEmail.SingleOrDefault(x => (x.SE_EmailNo-1).ToString() == key.ToString());
                    var newVal = newDic[key].ToString();
                    if (query.SE_Email != newVal)
                    {
                        query.SE_Email = newVal;
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
        /// 设置的机器管理人员的邮箱值
        /// </summary>    
        /// <param name="email"></param>   
        /// <returns></returns>   
        public void SetEmail(string email)
        {
            var result = from se in _dataContext.SetEmail
                         where se.SE_EmailNo == 1
                         select se;
            foreach (var se in result)
            {
                if (se.SE_EmailNo == 1)
                    se.SE_Email = email;
            }
            _dataContext.SubmitChanges();
      }

        /// <summary>
        /// 添加邮箱
        /// </summary>
        /// <param name="se"></param>
        /// <returns></returns>
        public bool AddEmail(SetEmail  se)
        {
            try
            {
                _dataContext.SetEmail.InsertOnSubmit(se);
                _dataContext.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 添加联系人
        /// </summary>
        /// <param name="se"></param>
        /// <returns></returns>
        public bool AddMessage(setMessages se)
        {
            try
            {
                _dataContext.setMessages.InsertOnSubmit(se);
                _dataContext.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 修改邮箱名称
        /// </summary>
        /// <param name="formerName"></param>
        /// <param name="emailName"></param>
        /// <returns></returns>
        public bool ModifyEmail(string formerName, string emailName)
        {
            try
            {
                SetEmail oldPC = _dataContext.SetEmail.Single(x => x.SE_Email == formerName);
                oldPC.SE_Email = emailName;                
                _dataContext.SubmitChanges();               
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 修改联系人
        /// </summary>
        /// <param name="formerName"></param>
        /// <param name="emailName"></param>
        /// <returns></returns>
        public bool ModifyMessage(string formerName, string messageName)
        {
            try
            {
                setMessages oldPC = _dataContext.setMessages.Single(x => x.SE_Value == formerName);
                oldPC.SE_Value = messageName;
                _dataContext.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 删除指定邮箱
        /// </summary>
        /// <param name="emailName"></param>
        /// <returns></returns>
        public bool DeleteEmail(string emailName)
        {
            try
            {
                if (_dataContext.ExecuteCommand(@"delete from SetEmail where SE_Email = '" + emailName + "' ") > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 删除联系人
        /// </summary>
        /// <param name="emailName"></param>
        /// <returns></returns>
        public bool DeleteMessage(string messageName)
        {
            try
            {
                if (_dataContext.ExecuteCommand(@"delete from setMessages where  SE_Value = '" + messageName + "' ") > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
      #endregion
    }
}
