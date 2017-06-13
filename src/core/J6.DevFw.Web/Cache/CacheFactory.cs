using JR.DevFw.Web.Cache.Compoment;

namespace JR.DevFw.Web.Cache
{
    /// <summary>
    /// 
    /// </summary>
    public static class CacheFactory
    {
        private static ICache _cacheInstance;

        /// <summary>
        /// 
        /// </summary>
        public static ICache Sington
        {
            get
            {
                return _cacheInstance ?? (_cacheInstance = new BasicCache(new DependCache()));
               // return cacheInstance ?? (cacheInstance = new CmsCache(new LevelDbCacheProvider()));
            }
        }
    }
}
