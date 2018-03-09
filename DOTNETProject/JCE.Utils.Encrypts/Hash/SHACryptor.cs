﻿using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using JCE.Utils.Extensions;

namespace JCE.Utils.Encrypts.Hash
{
    /// <summary>
    /// Hash 加密
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class SHACryptor: HashCryptorBase
    {
        /// <summary>
        /// 获取字符串的 SHA1 哈希值，默认编码为<see cref="Encoding.UTF8"/>
        /// </summary>
        /// <param name="value">待加密的值</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static string Sha1(string value,Encoding encoding=null)
        {
            return Encrypt<SHA1Managed>(value, encoding);
        }

        /// <summary>
        /// 获取字符串的 SHA256 哈希值，默认编码为<see cref="Encoding.UTF8"/>
        /// </summary>
        /// <param name="value">待加密的值</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static string Sha256(string value, Encoding encoding = null)
        {
            return Encrypt<SHA256Managed>(value, encoding);
        }

        /// <summary>
        /// 获取字符串的 SHA384 哈希值，默认编码为<see cref="Encoding.UTF8"/>
        /// </summary>
        /// <param name="value">待加密的值</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static string Sha384(string value, Encoding encoding = null)
        {
            return Encrypt<SHA384Managed>(value, encoding);
        }

        /// <summary>
        /// 获取字符串的SHA512哈希值，默认编码为<see cref="Encoding.UTF8"/>
        /// </summary>
        /// <param name="value">待加密的值</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static string Sha512(string value, Encoding encoding = null)
        {
            return Encrypt<SHA512Managed>(value, encoding);
        }
        
    }
}
