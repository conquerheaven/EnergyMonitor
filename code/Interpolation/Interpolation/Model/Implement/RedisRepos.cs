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

namespace Interpolation.Model.Implement
{
    class RedisRepos : IRedisRepos
    {
        private IRedisClient redis;
        private Log log;
        private ObjectSerializer ser;

        public RedisRepos()
        {
            redis = RedisManager.redis;
            log = new Log();
            ser = new ObjectSerializer();
        }

        public IList<AnalogHistoryHour> getByAnalogNoAndTimeGap(int AnalogNo, DateTime startTime, DateTime endTime)
        {
            IList<AnalogHistoryHour> ipdList = new List<AnalogHistoryHour>();
            try
            {
                IList<AnalogHistoryHour> ipdRedis = ser.Deserialize(redis.Get<byte[]>(AnalogNo.ToString())) as IList<AnalogHistoryHour>;
                for(int i = 0; i < ipdRedis.Count; i++)
                {
                    if(startTime <= ipdRedis[i].AHH_HTime && ipdRedis[i].AHH_HTime <= endTime)
                    {
                        ipdList.Add(ipdRedis[i]);
                    }
                }
            }catch(Exception ex)
            {
                log.write("Func:getByAnalogNoAndTimeGap:" + ex.Message);
            }
            return ipdList;
        }
    }
}
