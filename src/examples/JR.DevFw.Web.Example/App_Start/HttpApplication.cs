using System;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using JR.DevFw.Utils;
using JR.DevFw.PluginKernel;

namespace JR.DevFw.Web.Example
{
    public class HttpApplication :System.Web.HttpApplication
    {
        static HttpApplication()
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


        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }


        protected virtual void Application_BeginRequest(object o, EventArgs e)
        {
            //mono 下编码可能会有问题
            if (FwCtx.Mono())
            {
                Response.Charset = "utf-8";
            }
        }

    }
}