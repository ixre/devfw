using System;
using System.Collections.Generic;
using System.Linq;

namespace Com.PluginKernel
{
    /// <summary>
    /// 表示插件与主程序连接的结果
    ///
    /// </summary>
    public enum PluginConnectionResult
    {
        /// <summary>
        /// 
        /// </summary>
        Failed,

        /// <summary>
        /// 与版本号不匹配
        /// </summary>
        NotMatch,

        /// <summary>
        /// 
        /// </summary>
        Success
    }
}