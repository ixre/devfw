using System;
using System.Web;

namespace JR.DevFw.PluginKernel.Web
{
    /// <summary>
    /// B/S插件宿主
    /// </summary>
    public interface IWebPluginHost : IPluginHost
    {
        /// <summary>
        /// 注册扩展处理程序
        /// </summary>
        /// <param name="plugin"></param>
        /// <param name="getReqHandler">委托PluginHandler<HttpContext,string>的实例</param>
        /// <param name="postReqHandler">委托PluginHandler<HttpContext,string>的实例</param>
        /// <returns></returns>
        bool Register(IPlugin plugin, PluginHandler<HttpContext> getReqHandler,
            PluginHandler<HttpContext> postReqHandler);

        #region 调用插件

        /// <summary>
        /// 扩展模块GET请求,返回false则应立即截断请求
        /// </summary>
        /// <param name="pluginName"></param>
        /// <param name="result"></param>
        /// <param name="context"></param>
        void HttpPluginRequest(HttpContext context, string pluginName, ref bool result);

        /// <summary>
        /// 扩展模块POST请求,返回false则应立即截断请求
        /// </summary>
        /// <param name="context"></param>
        /// <param name="pluginName"></param>
        /// <param name="result"></param>
        void HttpPluginPost(HttpContext context, string pluginName, ref bool result);

        #endregion

        /// <summary>
        /// 获取请求路径
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        string GetRequestPath(HttpContext context);

        /// <summary>
        /// 扩展插件Action请求，为插件提供Action功能
        /// </summary>
        /// <param name="t"></param>
        /// <param name="context"></param>
        /// <param name="isPostRequest"></param>
        bool HandleRequestUse<T>(T t, HttpContext context, bool isPostRequest);


        /// <summary>
        /// 为自定义请求提供Action功能
        /// </summary>
        /// <param name="t"></param>
        /// <param name="context"></param>
        /// <param name="isPostRequest"></param>
        bool HandleCustomRequestUse<T>(T t, HttpContext context, String path,bool isPostRequest);
    }
}