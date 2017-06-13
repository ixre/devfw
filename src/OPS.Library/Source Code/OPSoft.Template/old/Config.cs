//
// Copyright 2011 @ OPS Inc,All right reseved.
// Name:ITemplate.cs
// Author:newmin
// Create:2011/06/05
//

namespace Ops.Template
{
    public delegate void TemplatePageHandler(TemplatePage page);

    public class Config
    {
        private static readonly string _version;

        public static string Version
        {
            get { return "2.1"; }
        }

        /// <summary>
        /// 是否启用Html压缩
        /// </summary>
        public static bool EnabledCompress = true;

        /// <summary>
        /// 是否将模板缓存
        /// </summary>
        public static bool EnabledCache = true;

        /// <summary>
        /// 是否共享URL参数值
        /// </summary>
        public static bool UrlQueryShared = true;

        /// <summary>
        /// 共享HttpItem传递的数据
        /// </summary>
        public static bool HttpItemShared = true;
    }
}