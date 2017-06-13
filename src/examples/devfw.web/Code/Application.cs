
/*
 * 由SharpDevelop创建。
 * 用户： Administrator
 * 日期: 2013/12/10
 * 时间: 13:57
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */

using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using JR.DevFw.PluginKernel;
using JR.DevFw.Utils;

namespace JR.DevFw.Web.Example.Code
{
    /// <summary>
    /// Description of HttpApplicaiton.
    /// </summary>
    public partial class Application : HttpApplication
    {
        static Application()
        {
            //解决依赖
            FwCtx.ResolveAssemblies();
        }


        /// <summary>
        /// 初始化
        /// </summary>
        public static void ApplicationInit()
        {
            //设置可写权限
            IoUtil.SetDirCanWrite(FwCtx.Variables.AssetsPath);
            IoUtil.SetDirCanWrite("templates/");
            IoUtil.SetDirCanWrite(PluginConfig.PLUGIN_DIRECTORY);
            IoUtil.SetDirCanWrite(FwCtx.Variables.TempPath);
            IoUtil.SetDirCanWrite(FwCtx.Variables.TempPath + "update");
            IoUtil.SetDirHidden("config");
            IoUtil.SetDirHidden("bin");

            //加载插件
            WebCtx.Current.Plugin.Connect();
        }

        protected virtual void RegisterRoutes(RouteCollection routes)
        {

            // ------------------------------------------------
            //  注册首页路由，可以自由修改首页位置
            //  routes.MapRoute("HomeIndex", ""
            //  , new { controller = "Cms", action = "Index" });
            // ------------------------------------------------


            // ------------------------------------------------
            //     自定义路由放在这里
            //  -----------------------------------------------

            routes.MapRoute("HomeIndex", "", 
                new { controller = "Home", action = "Index" });

            routes.MapRoute("Default_Router",
                "{controller}/{action}/{id}", 
                new { controller = "Home", action = "Index" });
        }

        protected virtual void Application_Start()
        {
            //初始化
            ApplicationInit();

            //注册路由;
            RouteCollection routes = RouteTable.Routes;
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //注册自定义路由
            RegisterRoutes(routes);
        }

        protected virtual void Application_BeginRequest(object o, EventArgs e)
        {
            //mono 下编码可能会有问题
            if (FwCtx.Mono())
            {
                Response.Charset = "utf-8";
            }
        }

        protected virtual void Application_Error(object o, EventArgs e)
        {
        }
    }
}
