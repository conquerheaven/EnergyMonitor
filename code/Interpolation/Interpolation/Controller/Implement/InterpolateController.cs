using Interpolation.Controller.Interface;
using Interpolation.Model.Entities;
using Interpolation.Model.Implement;
using Interpolation.RedisTool;
using Interpolation.Tools;
using ServiceStack.Redis;
using ServiceStack.Redis.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpolation.Controller.Implement
{
    class InterpolateController : IInterpolateController
    {
        private IRedisClient redis;
        private Log log;
        private ObjectSerializer ser;
        private InterpolationStateRepos interpolationStateRepos;

        public InterpolateController()
        {
            redis = RedisManager.redis;
            log = new Log();
            ser = new ObjectSerializer();
            interpolationStateRepos = new InterpolationStateRepos();
        }

        public IList<InterpolationData> Strategy(InterpolationState ips)
        {
            IList<InterpolationData> ipsList = new List<InterpolationData>();
            if(ips.Lvalue < 0.0 || ips.Rvalue < 0.0)
            {
                return null;
            }
            if(ips.Status == 1)
            {
                //query from InterpolationData;
            }
            try
            {
                PointRelation pointRel = ser.Deserialize(redis.Get<byte[]>(ips.AnalogNo.ToString())) as PointRelation;
                IList<int> sonList = pointRel.SonList;
                if(sonList == null || sonList.Count == 0)
                {
                    //ipsList = Interpolate(ips);
                    return ipsList;
                }
                for(int i = 0; i < sonList.Count; i++)
                {
                    IList<InterpolationState> sonIpsList = interpolationStateRepos.queryIntersectWithParent(ips, sonList[i]);
                    IList<InterpolationData> sonIpdList = new List<InterpolationData>();
                    for(int j = 0; j < sonIpsList.Count; j++)
                    {
                        IList<InterpolationData> ipdList = Strategy(sonIpsList[i]);
                        if(ipsList == null)
                        {
                            return null;
                        }
                        for(int k = 0;  k < ipdList.Count; k++)
                        {
                            sonIpdList.Add(ipdList[k]);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                log.write("Func:Strategy:"+ex.Message);
            }
            return ipsList;
        }
    }
}
