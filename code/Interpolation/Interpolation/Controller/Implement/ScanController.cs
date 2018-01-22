using Interpolation.Controller.Interface;
using Interpolation.Model.Entities;
using Interpolation.Model.Implement;
using Interpolation.Model.Interface;
using Interpolation.RedisTool;
using Interpolation.Tools;
using ServiceStack.Redis;
using ServiceStack.Redis.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpolation.Controller
{
    class ScanController : IScanController
    {
        private InterpolationStateRepos iplStateRepos;
        private IAnalogHistoryRepos analogHistoryRepos;
        private IAnalogHistoryHourRepos aHHRepos;
        private IAMPRepos ampRepos;
        private Time timeTool;
        private Log log;
        private IRedisClient redis;
        private ObjectSerializer ser;

        private TimeGap timeGap;
        public ScanController()
        {
            iplStateRepos = new InterpolationStateRepos();
            analogHistoryRepos = new AnalogHistoryRepos();
            aHHRepos = new AnalogHistoryHourRepos();
            ampRepos = new AMPRepos();
            timeTool = new Time();
            log = new Log();

            redis = RedisManager.redis;
            ser = new ObjectSerializer();
        }

        public void setTimeGap(TimeGap timeGap)
        {
            this.timeGap = timeGap;
        }

        //调用此函数前，必须先调用setTimeGap
        public void scanTask()
        {
            //获取所有测点ID
            IList<int> pointList = ampRepos.queryAllPointID();
            //将当前时间往前推24小时，作为起始时间点
            DateTime startTime = timeGap.getStartTime();
            //终点为当前时间小时减1秒
            DateTime endTime = timeGap.getEndTime();
            string logMsg = "";
            try
            {
                DateTime testS = DateTime.Now;
                log.write("定时时间点到，开始检测...");
                log.write("起止时间为："+timeGap.getStartTime().ToString() + " " + timeGap.getEndTime().ToString());
                for (int i = 0; i < pointList.Count; i++)
                {
                    //log.write("获取测点小时数据：AnalogNo="+pointList[i].ToString());
                    //获取该测点每小时读数
                    IList<AnalogHistoryHour> hourList = getHourData(startTime, endTime, pointList[i]);
                    if (hourList == null) continue;


                    /*
                     * Redis
                     * 1、取出现有数据
                     * 2、更新现有数据
                     */
                     
                    //1.取出现有数据
                    IList<AnalogHistoryHour> oldHourList = ser.Deserialize(redis.Get<byte[]>(pointList[i].ToString())) as List<AnalogHistoryHour>;
                    //2.更新现有数据
                    IList<AnalogHistoryHour> newHourList = new List<AnalogHistoryHour>();
                    if (oldHourList != null)
                    {
                        for (int j = hourList.Count; j < oldHourList.Count; j++)
                        {
                            newHourList.Add(oldHourList[j]);
                        }
                    }

                    for (int j = 0; j < hourList.Count; j++)
                    {
                        newHourList.Add(hourList[j]);
                    }
                    logMsg = pointList[i].ToString() + newHourList.Count.ToString();
                    redis.Set<byte[]>(pointList[i].ToString(), ser.Serialize(newHourList));



                    //log.write("扫描缺失区间：AnalogNo=" + pointList[i].ToString());
                    scanMissingGap(hourList);
                    //log.write("扫描结束！");
                }
                DateTime testE = DateTime.Now;
                log.write("总用时：" + (testE - testS).TotalSeconds);
            }
            catch (Exception ex)
            {
                log.write("Func:scanTask:" + ex.StackTrace + "\n" + logMsg);
            }
        }

        void scanMissingGap(IList<AnalogHistoryHour> hourList)
        {
            try
            {
                int l = -1;
                int r = -1;
                IList<InterpolationState> gapList = new List<InterpolationState>();//保存缺失区间
                //Console.WriteLine("小时时间序列扫描："+hourList.Count);
                //扫描连续缺失区间
                for (int i = 0; i < hourList.Count; i++)
                {
                    //获取缺失起点
                    if (hourList[i].AHH_Value == 0.0 && (i == 0 || hourList[i - 1].AHH_Value > 0.0))
                    {
                        l = i;
                    }
                    //获取缺失终点
                    else if (hourList[i].AHH_Value > 0.0 && i > 0 && hourList[i - 1].AHH_Value == 0.0)
                    {
                        r = i;
                    }
                    if (l != -1 && r != -1 || (l != -1 && i == hourList.Count - 1))
                    {
                        if (r == -1) r = hourList.Count - 1;
                        InterpolationState IpS = new InterpolationState();
                        IpS.AnalogNo = hourList[l].AHH_AnalogNo;
                        IpS.StartTime = hourList[l].AHH_HTime;
                        IpS.EndTime = hourList[r].AHH_HTime;
                        IpS.Status = 0;
                        IpS.Lvalue = -100.0;//若边界缺失，置为负数，此处置为-100.0
                        IpS.Rvalue = -100.0;
                        if (l != 0)
                        {
                            IpS.Lvalue = hourList[l - 1].AHH_Value;
                            IpS.StartTime = hourList[l - 1].AHH_HTime;
                        }
                        if (r != hourList.Count - 1)
                        {
                            IpS.Rvalue = hourList[r + 1].AHH_Value;
                            IpS.EndTime = hourList[r + 1].AHH_HTime;
                        }
                        gapList.Add(IpS);
                        l = -1;
                        r = -1;
                    }
                }
                for (int i = 0; i < gapList.Count; i++)
                {
                    IList<InterpolationState> ipsList = iplStateRepos.queryIntersect(gapList[i]);
                    /*Console.WriteLine("AnalogNo:" + gapList[i].AnalogNo+" gap:" + gapList[i].StartTime + " " + gapList[i].EndTime);
                    for(int j = 0; j < ipsList.Count; j++)
                    {
                        Console.WriteLine("interSect Gap:" + ipsList[j].StartTime + " " + ipsList[j].EndTime);
                    }*/
                    //如果没有与当前缺失区间相交的区间，那么直接插入数据库
                    if (ipsList.Count == 0)
                    {
                        //iplStateRepos.insert(gapList[i]);
                    }
                    else
                    {
                        //将所有缺失区间合并成一个
                        for (int j = 0; j < ipsList.Count; j++)
                        {
                            if(gapList[i].StartTime > ipsList[j].StartTime)
                            {
                                gapList[i].StartTime = ipsList[j].StartTime;
                                gapList[i].Lvalue = ipsList[j].Lvalue;
                            }
                            if(gapList[i].EndTime < ipsList[j].EndTime)
                            {
                                gapList[i].EndTime = ipsList[j].EndTime;
                                gapList[i].Rvalue = ipsList[j].Rvalue;
                            }
                            //将已有缺失区间作废
                            ipsList[j].Status = -1;
                        }
                        //为节省存储，将已存在的第一个缺失区间回收
                        ipsList[0].StartTime = gapList[i].StartTime;
                        ipsList[0].EndTime = gapList[i].EndTime;
                        ipsList[0].Lvalue = gapList[i].Lvalue;
                        ipsList[0].Rvalue = gapList[i].Rvalue;
                        ipsList[0].Status = gapList[i].Status;
                        //iplStateRepos.modifyList(ipsList);
                    }
                }
            }
            catch (Exception ex)
            {
                log.write("Func:scanMissingData:" + ex.StackTrace);
            }
        }

        IList<AnalogHistoryHour> getHourData(DateTime startTime, DateTime endTime, int analogNo)
        {
            try
            {
                //获取该测点每小时读数
                IList<AnalogHistoryHour> historyList = analogHistoryRepos.getHourEnergyHistoryByAnalogNo(analogNo, startTime, endTime);
                IList<AnalogHistoryHour> hourList = new List<AnalogHistoryHour>();
                int count = 0;
                //填补缺失时间点读数，将值设为0.0
                for (DateTime tmp = startTime; tmp < endTime; tmp = tmp.AddHours(1))
                {
                    if (count >= historyList.Count || historyList[count].AHH_HTime != tmp)
                    {
                        AnalogHistoryHour ahh = new AnalogHistoryHour();
                        ahh.AHH_AnalogNo = analogNo;
                        ahh.AHH_HTime = tmp;
                        ahh.AHH_Value = 0.0;
                        hourList.Add(ahh);
                    }
                    else
                    {
                        hourList.Add(historyList[count]);
                        count++;
                    }
                }
                return hourList;
            }
            catch (Exception ex)
            {
                log.write("Func:getHourData:" + ex.StackTrace);
                return null;
            }
        }
    }
}
