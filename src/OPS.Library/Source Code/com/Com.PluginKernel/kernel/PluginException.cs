using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.PluginKernel
{
    /// <summary>
    /// 插件异常
    /// </summary>
    public class PluginException : Exception
    {
        public PluginException(string message) : base(message)
        {
        }

        public override string HelpLink
        {
            get { return "http://www.ops.cc"; }
            set { throw new NotImplementedException(); }
        }

        public override string Message
        {
            get { return "插件异常:" + base.Message; }
        }
    }
}