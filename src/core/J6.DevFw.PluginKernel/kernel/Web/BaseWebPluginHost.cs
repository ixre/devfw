using System;
using System.Text.RegularExpressions;
using System.Web;

namespace JR.DevFw.PluginKernel.Web
{
    /// <summary>
    /// B/S插件宿主
    /// </summary>
    [PluginHost("B/S插件宿主", "使用{module}.sh/{action}访问自定义扩展")]
    public abstract class BaseWebPluginHost : BasePluginHost, IWebPluginHost
    {
        protected WebPluginHandleProxy<HttpContext> WebHandler;

        /// <summary>
        /// 
        /// </summary>
        protected BaseWebPluginHost()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_webHandler"></param>
        protected BaseWebPluginHost(WebPluginHandleProxy<HttpContext> _webHandler)
        {
            this.WebHandler = _webHandler;
        }

        /// <summary>
        /// 注册扩展处理程序
        /// </summary>
        /// <param name="plugin">扩展名称，而且也是访问地址的名称。如扩展名称为:ext,那么可使用/ext.sh访问该扩展插件</param>
        /// <param name="getReqHandler">委托PluginHandler<CmsContext,string>的实例</param>
        /// <param name="postReqHandler">委托PluginHandler<CmsContext,string>的实例</param>
        /// <returns></returns>
        public bool Register(IPlugin plugin, PluginHandler<HttpContext> getReqHandler,
            PluginHandler<HttpContext> postReqHandler)
        {
            return this.WebHandler.Register(plugin, getReqHandler, postReqHandler);
        }


        /// <summary>
        /// 扩展模块GET请求,返回false则应立即截断请求
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="page"></param>
        public virtual void HttpPluginRequest(HttpContext context, string extendName, ref bool result)
        {
            extendName = extendName.ToLower();
            this.WebHandler.HandleGetRequest(context, extendName, ref result);
        }


        /// <summary>
        /// 扩展模块POST请求,返回false则应立即截断请求
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="page"></param>
        public virtual void HttpPluginPost(HttpContext context, string extendName, ref bool result)
        {
            extendName = extendName.ToLower();
            this.WebHandler.HandlePostRequest(context, extendName, ref result);
        }

        /// <summary>
        /// 获取请求路径
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string GetRequestPath(HttpContext context)
        {
            Match match = Regex.Match(context.Request.Path, "/(.+?)/([^\\?]+)");
            return match == Match.Empty ? "" : match.Groups[2].Value;
        }

        /// <summary>
        /// 扩展插件Action请求，为插件提供Action功能
        /// </summary>
        /// <param name="t"></param>
        /// <param name="context"></param>
        /// <param name="isPostRequest"></param>
        public virtual bool HandleRequestUse<T>(T t, HttpContext context, bool isPostRequest)
        {
            string path = this.GetRequestPath(context);
            return this.HandleCustomRequestUse(t, context, path, isPostRequest);
        }


        public bool HandleCustomRequestUse<T>(T t, HttpContext context, string path, bool isPostRequest)
        {
            string action = null;

            if (path.Length != 0)
            {
                action = path.IndexOf('/') == -1 ? path : path.Substring(0, path.IndexOf('/'));
            }
            action = String.Concat(action ?? "default", isPostRequest ? "_post" : "");

            try
            {
                object obj = this.WebHandler.HandleRequestUse<T>(
                    t
                    , context
                    , action
                    , context.Request.RawUrl);


                if (obj != null)
                {
                    context.Response.Write(obj.ToString());
                }
                return true;
            }
            catch (PluginException exc)
            {
                Logger.Println("[ Request][ Error]" + exc.Message + ";url :" + context.Request.RawUrl);
                throw exc.InnerException ?? exc;
            }
            catch (Exception exc)
            {
                Logger.PrintException(context.Request.RawUrl, exc);
                throw;
            }
        }
    }
}