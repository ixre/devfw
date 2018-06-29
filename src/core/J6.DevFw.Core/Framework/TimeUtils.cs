using System;

namespace JR.DevFw.Framework
{
    /// <summary>
    /// 时间工具类
    /// </summary>
    public static class TimeUtils
    {
        /// <summary>
        /// 小时->秒
        /// </summary>
        public static int Hour = 3600;
        /// <summary>
        /// 小时->秒
        /// </summary>
        public static int Day = 24 * Hour;

        private static DateTime unixVar = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        /// <summary>  
        /// 获取时间戳  
        /// </summary>  
        /// <returns></returns>  
        public static string Unix(DateTime d)
        {
            TimeSpan ts = d - unixVar;
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        /// <summary>
        /// 获取时间戳(微秒)
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static string MilliUnix(DateTime d)
        {
            TimeSpan ts = d - unixVar;
            return Convert.ToInt64(ts.Milliseconds).ToString();
        }

        /// <summary>
        /// 获取日期00:00:00的时间戳
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static String DateUnix(DateTime d)
        {
            return Unix(d.Date);
        }
    }
}
