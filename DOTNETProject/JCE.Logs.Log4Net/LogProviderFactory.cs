﻿using System;
using System.Collections.Generic;
using System.Text;
using JCE.Logs.Abstractions;

namespace JCE.Logs.Log4Net
{
    /// <summary>
    /// Log4Net日志提供程序工厂
    /// </summary>
    public class LogProviderFactory:ILogProviderFactory
    {
        /// <summary>
        /// 创建日志提供程序
        /// </summary>
        /// <param name="logName">日志名称</param>
        /// <param name="format">日志格式化器</param>
        /// <returns></returns>
        public ILogProvider Create(string logName, ILogFormat format = null)
        {
            return new Log4NetProvider(logName,format);
        }
    }
}
