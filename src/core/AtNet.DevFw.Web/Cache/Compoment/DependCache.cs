using System;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Caching;
using AtNet.DevFw.Framework.IO;
using Ops.Cms.Cache;

namespace AtNet.DevFw.Web.Cache.Compoment
{
    /// <summary>
    /// 注入缓存
    /// </summary>
    public class DependCache : CacheBase
    {
        private static readonly string cacheDependFile;

        internal DependCache() { }
        static DependCache()
        {
            cacheDependFile = Variables.PhysicPath + "config/cache.pid";
        }

        public override void Insert(string key, object value, DateTime absoluteExpireTime)
        {
            HttpRuntime.Cache.Insert(key, value, new CacheDependency(cacheDependFile), absoluteExpireTime, TimeSpan.Zero);
        }

        public override void Insert(string key, object value)
        {
            HttpRuntime.Cache.Insert(key, value, new CacheDependency(cacheDependFile),
                System.Web.Caching.Cache.NoAbsoluteExpiration,
                System.Web.Caching.Cache.NoSlidingExpiration);
        }

        /// <summary>
        /// 重建缓存
        /// </summary>
        public override string Rebuilt()
        {
            //初始化config文件夹
            if (!Directory.Exists(String.Concat(Variables.PhysicPath, "config/")))
            {
                Directory.CreateDirectory(String.Concat(Variables.PhysicPath, "config/")).Create();
            }

            using (FileStream fs = new FileStream(cacheDependFile, FileMode.OpenOrCreate, FileAccess.Write))
            {
                byte[] pid = Encoding.UTF8.GetBytes(new Random().Next(1000, 5000).ToString());
                fs.Seek(0, SeekOrigin.Begin);
                fs.Write(pid, 0, pid.Length);
                fs.Flush();
            }

            return  IoUtil.GetFileSHA1(cacheDependFile);
            
            //FileInfo file = new FileInfo(cacheDependFile);
            //file.LastWriteTimeUtc = DateTime.UtcNow;
        }

    }
}