using JR.DevFw.PluginKernel;
using JR.DevFw.PluginKernel.Web;
using JR.DevFw.Template;

namespace JR.DevFw.Web.Plugin
{
    /// <summary>
    /// 插件应用
    /// </summary>
    public interface IPluginApp : IWebPluginHost
    {
        TemplatePage GetPage<T>(string filePath) where T : IPlugin;
        TemplatePage GetPage(IPlugin plugin, string filePath);
        TemplatePage GetTemplatePage(string filePath, PluginPackAttribute attr);

       // void LoadSession(HttpContext context);

        /// <summary>
        /// 注册插件路由
        /// </summary>
        /// <param name="plugin"></param>
        /// <param name="aliasName"></param>
         void MapPluginRoute(IPlugin plugin, string aliasName);

        /// <summary>
        /// 注册插件路由,通常由插件APP自动调用
        /// </summary>
        /// <param name="plugin"></param>
        void MapPluginRoute(IPlugin plugin);
    }
}
