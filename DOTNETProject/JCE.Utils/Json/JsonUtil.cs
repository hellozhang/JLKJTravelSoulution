﻿using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace JCE.Utils.Json
{
    /// <summary>
    /// Json操作辅助类
    /// </summary>
    public static class JsonUtil
    {
        /// <summary>
        /// 对象锁
        /// </summary>
        private static object lockObj=new object();

        #region JsonDateTimeFormat(Json时间格式化)

        /// <summary>
        /// Json时间格式化
        /// </summary>
        /// <param name="json">json</param>
        /// <returns></returns>
        public static string JsonDateTimeFormat(string json)
        {
            json = Regex.Replace(json, @"\\/Date\((\d+)\)\\/", match =>
            {
                DateTime dt = new DateTime(1970, 1, 1);
                dt = dt.AddMilliseconds(long.Parse(match.Groups[1].Value));
                return dt.ToString("yyyy-MM-dd HH:mm:ss.fff");
            });
            return json;
        }

        #endregion

        #region ToObject(将Json字符串转换为对象)
        /// <summary>
        /// 将Json字符串转换为对象
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="json">Json字符串</param>
        /// <returns></returns>
        public static T ToObject<T>(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return default(T);
            }
            return JsonConvert.DeserializeObject<T>(json);
        }
        #endregion

        #region ToJson(将对象转换为Json字符串)

        /// <summary>
        /// 将对象转换为Json字符串
        /// </summary>
        /// <param name="target">目标对象</param>
        /// <param name="isConvertToSingleQuotes">是否将双引号转换成单引号</param>
        /// <param name="camelCase">是否驼峰式命名</param>
        /// <param name="indented">是否缩进</param>
        /// <returns></returns>
        public static string ToJson(object target, bool isConvertToSingleQuotes = false, bool camelCase = false, bool indented = false)
        {
            if (target == null)
            {
                return "{}";
            }
            var options = new JsonSerializerSettings();
            if (camelCase)
            {
                options.ContractResolver = new CamelCasePropertyNamesContractResolver();
            }
            if (indented)
            {
                options.Formatting = Formatting.Indented;
            }
            var result = JsonConvert.SerializeObject(target, options);
            if (isConvertToSingleQuotes)
            {
                result = result.Replace("\"", "'");
            }
            return result;
        }
        #endregion

        #region SerializableToFile(将对象序列化到Json文件)

        /// <summary>
        /// 将对象序列化到Json文件
        /// </summary>
        /// <param name="fileName">文件名，绝对路径</param>
        /// <param name="obj">对象</param>
        public static void SerializableToFile(string fileName, object obj)
        {
            lock (obj)
            {
                using (FileStream fs=new FileStream(fileName,FileMode.OpenOrCreate))
                {
                    using (StreamWriter sw=new StreamWriter(fs,Encoding.UTF8))
                    {
                        sw.Write(ToJson(obj,false,false,true));
                    }
                }
            }
        }

        #endregion

        #region DeserializeFromFile(从Json文件反序列成对象)

        /// <summary>
        /// 从Json文件反序列成对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="fileName">文件名，绝对路径</param>
        /// <returns></returns>
        public static T DeserializeFromFile<T>(string fileName)
        {
            try
            {
                using (FileStream fs=new FileStream(fileName,FileMode.OpenOrCreate))
                {
                    using (StreamReader sr=new StreamReader(fs,Encoding.UTF8))
                    {
                        return ToObject<T>(sr.ReadToEnd());
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
    }
}
