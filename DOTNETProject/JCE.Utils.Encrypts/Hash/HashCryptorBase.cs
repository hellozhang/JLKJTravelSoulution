﻿using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using JCE.Utils.Extensions;

namespace JCE.Utils.Encrypts.Hash
{
    /// <summary>
    /// Hash 加密基类
    /// </summary>
    public class HashCryptorBase
    {
        /// <summary>
        /// ShaX 加密
        /// </summary>
        /// <typeparam name="T">哈希算法</typeparam>
        /// <param name="value">待加密的值</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        internal static string Encrypt<T>(string value, Encoding encoding) where T : HashAlgorithm, new()
        {
            value.CheckNotNullOrEmpty(nameof(value));
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }

            byte[] bytes = encoding.GetBytes(value);
            HashAlgorithm hash = new T();
            try
            {
                bytes = hash.ComputeHash(bytes);
            }
            finally
            {
                hash.Clear();
            }
            StringBuilder sb = new StringBuilder();
            foreach (var item in bytes)
            {
                sb.AppendFormat("{0:x2}", item);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Hmac 加密
        /// </summary>
        /// <typeparam name="T">密钥哈希算法</typeparam>
        /// <param name="value">待加密的值</param>
        /// <param name="key">密钥</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        internal static string Encrypt<T>(string value, string key, Encoding encoding)
            where T : KeyedHashAlgorithm, new()
        {
            value.CheckNotNullOrEmpty(nameof(value));
            key.CheckNotNullOrEmpty(nameof(key));
            if (encoding == null)
            {
                encoding=Encoding.UTF8;
            }
            byte[] keys = encoding.GetBytes(key);
            byte[] datas = encoding.GetBytes(value);
            using (T hash=new T())
            {
                hash.Key = keys;
                hash.Initialize();
                byte[] result = hash.ComputeHash(datas);
                //return Convert.ToBase64String(result);
                return BitConverter.ToString(result).Replace("-","").ToLower();
            }
        }
    }
}
