using System.Web;
using AtNet.DevFw.PluginKernel;
using AtNet.DevFw.PluginKernel.Web;
using AtNet.DevFw.Template;

namespace AtNet.DevFw.Web.Plugins
{
    /// <summary>
    /// 插件应用
    /// </summary>
    public interface IExtendApp : IExtendPluginHost
    {
        TemplatePage GetPage<T>(string filePath) where T : IPlugin;
        TemplatePage GetPage(IPlugin plugin, string filePath);

        TemplatePage GetTemplatePage(string filePath, PluginPackAttribute attr);

        void LoadSession(HttpContext context);

        /// <summary>
        /// 注册插件路由
        /// </summary>
        /// <param name="plugin"></param>
        /// <param name="aliasName"></param>
         void MapExtendPluginRoute(IPlugin plugin, string aliasName);

        /// <summary>
        /// 注册插件路由
        /// </summary>
        /// <param name="plugin"></param>
        void MapExtendPluginRoute(IPlugin plugin);
    }
}
