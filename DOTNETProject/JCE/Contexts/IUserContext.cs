﻿using System;
using System.Collections.Generic;
using System.Text;

namespace JCE.Contexts
{
    /// <summary>
    /// 用户上下文
    /// </summary>
    public interface IUserContext
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        string UserId { get; }

        /// <summary>
        /// 用户名
        /// </summary>
        string UserName { get; }
    }
}
