using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnergyMonitor.Models.Repository.Entity;
using EnergyMonitor.Models.LinqEntity;
using System.Collections;

namespace EnergyMonitor.Models.Repository.Interface
{
    public interface IExceptionInfoRepos
    {
        /// <summary>
        /// 获取所有联系人手机号码
        /// </summary>          
        /// <returns></returns>    
        IList GetAllMessage();

        /// <summary>
        /// 删除联系人
        /// </summary>
        /// <param name="emailName"></param>
        /// <returns></returns>
        bool DeleteMessage(string messageName);

        /// <summary>
        /// 修改联系人
        /// </summary>
        /// <param name="formerName"></param>
        /// <param name="emailName"></param>
        /// <returns></returns>
        bool ModifyMessage(string formerName, string messageName);

         /// <summary>
        /// 添加联系人
        /// </summary>
        /// <param name="se"></param>
        /// <returns></returns>
        bool AddMessage(setMessages se);

        /// <summary>
        /// 根据异常类型查询指定时间段的电力异常值
        /// </summary>
        /// <param name="exceptionType"></param>         
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>      
        /// <returns></returns>      
        IList<ExceptionInfoEntity> GetExceptionInfo(string exceptionType, DateTime startTime, DateTime endTime);

        /// <summary>
        /// 查询前一天断值测点信息
        /// </summary>           
        /// <returns></returns>   
        IList<ExceptionInfoEntity> GetDiscontinuousValueInfo( );

        /// <summary>
        /// 设置的机器管理人员的邮箱值
        /// </summary>      
        /// <param name="email"></param>   
        /// <returns></returns>   
        void SetEmail(string email);

        /// <summary>
        /// 获取所有机器管理人员的邮箱
        /// </summary>          
        /// <returns></returns>    
        IList GetAllEmail();

         /// <summary>
        /// 根据键值对修改机器管理人员邮箱
        /// </summary>
        /// <param name="newDic"></param>
        /// <returns></returns>
        bool ModifyEmail(IDictionary newDic);

        /// <summary>
        /// 添加邮箱
        /// </summary>
        /// <param name="se"></param>
        /// <returns></returns>
        bool AddEmail(SetEmail se);

       /// <summary>
        /// 修改邮箱名称
        /// </summary>
        /// <param name="formerName"></param>
        /// <param name="emailName"></param>
        /// <returns></returns>
        bool ModifyEmail(string formerName, string emailName);

        /// <summary>
        /// 删除指定邮箱
        /// </summary>
        /// <param name="emailName"></param>
        /// <returns></returns>
        bool DeleteEmail(string emailName);
    }
}
