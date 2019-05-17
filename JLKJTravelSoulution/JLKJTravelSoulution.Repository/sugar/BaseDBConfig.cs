﻿
using System;
using System.IO;
using System.Linq;

namespace JLKJTravelSoulution.Repository
{
    public class BaseDBConfig
    {
        public static string ConnectionString = File.Exists(@"D:\my-file\dbCountPsw1.txt") ? File.ReadAllText(@"D:\my-file\dbCountPsw1.txt").Trim() : "server=192.168.10.168;uid=sa;pwd=123;database=WMBlogDB";

        //正常格式是

        //public static string ConnectionString = "server=.;uid=sa;pwd=sa;database=BlogDB"; 

        //原谅我用配置文件的形式，因为我直接调用的是我的服务器账号和密码，安全起见

    }
}
