using CSRedis;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace RedisWatch
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Redis控制台查看程序";
            do
            {
                try
                {
                    ConsoleWrite();
                    string key = Console.ReadLine();
                    if (string.IsNullOrEmpty(key))
                    {
                        continue;
                    }
                    try
                    {
                        if (RedisHelper.Exists(key))
                        {
                            object obj = RedisHelper.Get(key);
                            try
                            {

                                var timeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine(JsonConvert.SerializeObject(obj, timeConverter));
                            }
                            catch (Exception e)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine(obj.ToString());
                            }
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("不存在KEY");
                        }

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("{0}", e);
                        Console.WriteLine("无效的命令参数. 请选择 1, 2, 3，4,5 or 6.");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("{0}", e);
                }
            } while (true);
        }
        static void ConsoleWrite()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("请输入KEY");
        }

        static class RedisHelper
        {
            static RedisHelper()
            {
                CSRedisHelper.Initialization(
                    csredis: new CSRedisClient("116.62.196.145:6379,defaultDatabase=0,poolsize=50,ssl=false,writeBuffer=10240,prefix="),
                    serialize: value => Newtonsoft.Json.JsonConvert.SerializeObject(value),
                    deserialize: (data, type) => Newtonsoft.Json.JsonConvert.DeserializeObject(data, type));
            }




            /// 这里的 MergeKey 用来拼接 Key 的前缀，具体不同的业务模块使用不同的前缀。
            /// </summary>
            /// <param name="key"></param>
            /// <returns></returns>
            private static string MergeKey(string key)
            {
                //BaseSystemInfo.SystemCode +
                return key;
            }
            /// <summary>
            /// 根据key获取缓存对象
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="key"></param>
            /// <returns></returns>
            public static T Get<T>(string key)
            {

                return Deserialize<T>(CSRedisHelper.GetBytes(key));
            }
            /// <summary>
            /// 根据key获取缓存对象
            /// </summary>
            /// <param name="key"></param>
            /// <returns></returns>
            public static object Get(string key)
            {

                return Deserialize<object>(CSRedisHelper.GetBytes(key));
            }

            /// <summary>
            /// 设置缓存
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            public static void Set(string key, object value)
            {

                CSRedisHelper.SetBytes(key, Serialize(value));
            }

            /// <summary>
            /// 设置缓存
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            /// <param name="expiry"></param>
            public static void Set(string key, object value, DateTime expireTime)
            {

                TimeSpan span = DateTime.Now.AddMinutes(30) - DateTime.Now;//默认为30分钟
                if (expireTime > DateTime.Now)
                {
                    span = expireTime - DateTime.Now;
                }
                TimeSpan sec = TimeSpan.Parse(span.Days + "." + span.Hours + ":" + span.Minutes + ":" + span.Seconds);
                CSRedisHelper.SetBytes(key, Serialize(value), sec.Seconds);
            }
            /// <summary>
            /// 判断在缓存中是否存在该key的缓存数据
            /// </summary>
            /// <param name="key"></param>
            /// <returns></returns>
            public static bool Exists(string key)
            {

                return CSRedisHelper.Exists(key);  //可直接调用
            }

            /// <summary>
            /// 移除指定key的缓存
            /// </summary>
            /// <param name="key"></param>
            /// <returns></returns>
            public static long Remove(string key)
            {

                return CSRedisHelper.Remove(new string[] { key });
            }


            /// <summary>
            /// 序列化对象
            /// </summary>
            /// <param name="o"></param>
            /// <returns></returns>
            static byte[] Serialize(object o)
            {
                if (o == null)
                {
                    return null;
                }
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    binaryFormatter.Serialize(memoryStream, o);
                    byte[] objectDataAsStream = memoryStream.ToArray();
                    return objectDataAsStream;
                }
            }

            /// <summary>
            /// 反序列化对象
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="stream"></param>
            /// <returns></returns>
            static T Deserialize<T>(byte[] stream)
            {
                if (stream == null)
                {
                    return default(T);
                }
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                using (MemoryStream memoryStream = new MemoryStream(stream))
                {
                    T result = (T)binaryFormatter.Deserialize(memoryStream);
                    return result;
                }
            }




        }

    }
}