using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpolation.RedisTool
{
    static class RedisManager
    {
        public static PooledRedisClientManager prcm;
        public static IRedisClient redis;

        static RedisManager()
        {
            prcm = new PooledRedisClientManager("127.0.0.1:6379");
            redis = prcm.GetClient();
        }

        class test
        {
            private int a;
            private int b;
            public void setA(int a)
            {
                this.a = a;
            }
            public int getA()
            {
                return this.a;
            }

            public void setB(int b)
            {
                this.b = b;
            }
            public int getB()
            {
                return this.b;
            }
        }

        static void main(string[] args)
        {
            test t = new test();
            t.setA(12);
            t.setB(13);
            RedisManager.redis.Set("test", t);
            test tt = RedisManager.redis.Get<test>("test");
            Console.WriteLine(t);
        }
    }
}
