using System;
using System.IO;

namespace AtNet.DevFw.Toolkit.ThirdApi.ETao
{
    public class Config
    {
        /// <summary>
        /// 是否开启ETao
        /// </summary>
        public static bool Opened = false;

        /// <summary>
        /// 保存路径
        /// </summary>
        public const string SavePath = "api/etao/";

        /// <summary>
        /// 销售账号
        /// </summary>
        public static string Seller = "etao_test";

        /// <summary>
        /// 域名
        /// </summary>
        public static string Domain = "http://xxx.com/";
        
        /// <summary>
        /// 接口版本
        /// </summary>
        public const string Version = "1.0";

        /// <summary>
        /// 最后生成时间
        /// </summary>
        public static DateTime LastBuildTime;

        /// <summary>
        /// 使用线程数
        /// </summary>
        public const int Threads = 5;

        public static string PhypicPath;

        static Config()
        {
            PhypicPath = AppDomain.CurrentDomain.BaseDirectory;
            LastBuildTime = DateTime.Now.AddDays(-2);
            DirectoryInfo dir = new DirectoryInfo(Config.PhypicPath+Config.SavePath+"items/");
            if (!dir.Exists)
            {
                Directory.CreateDirectory(dir.FullName).Create();
            }
      
        }

        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="seller"></param>
        /// <param name="opened"></param>
        public static void Set(string seller, bool opened)
        {
            Seller = seller;
            Opened = opened;
        }
    }
}
