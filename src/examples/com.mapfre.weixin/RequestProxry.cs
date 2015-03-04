/**
 * Copyright (C) 2007-2015 OPSoft INC,All rights reseved.
 * Get more infromation of this software,please visit site http://cms.ops.cc
 * 
 * name : RequestProxry.cs
 * author : newmin (new.min@msn.com)
 * date : 2012/12/01 23:00:00
 * description : 
 * history : 
 */

using System.Web;
using AtNet.DevFw.PluginKernel;
using AtNet.DevFw.Web.Plugins;
using Com.Plugin.Core;
using Com.Plugin.WebManage;

namespace Com.Plugin
{
    /// <summary>
    /// Description of HandleRequest.
    /// </summary>
    public class RequestProxry
    {
        private readonly IExtendApp _app;
        private readonly RequestHandle _handler;

        private readonly ManageHandle _mgHandler;

        public RequestProxry(IExtendApp app, IPlugin plugin)
        {
            this._app = app;
            this._handler = new RequestHandle(plugin);
            this._mgHandler = new ManageHandle(app,plugin);
        }

        public static bool VerifyLogin(HttpContext context)
        {
            return true;
            //            bool result = UserState.Administrator.HasLogin;
            //            if (!result)
            //            {
            //                context.Response.Write("<script>window.parent.location.replace('/admin?return_url=')</script>");
            //            }
            //            return result;
        }

        public void HandleGet(HttpContext context, ref bool handled)
        {
            if (context.Request.Path.Contains("/wxm"))
            {
                if (this._app.HandleRequestUse(this._mgHandler, context, false))
                {
                    handled = true;
                }
                return;
            }
            if (this._app.HandleRequestUse(this._handler, context, false))
            {
                handled = true;
            }
        }

        public void HandlePost(HttpContext context, ref bool handled)
        {
            if (context.Request.Path.Contains("/wxm"))
            {
                if (this._app.HandleRequestUse(this._mgHandler, context, true))
                {
                    handled = true;
                }
                return;
            }
            if (this._app.HandleRequestUse(this._handler, context, true))
            {
                handled = true;
            }
        }

    }
}
