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
using AtNet.DevFw.Web.Plugins;

namespace Com.Plugin
{
    /// <summary>
    /// Description of HandleRequest.
    /// </summary>
    public class RequestProxry
    {
        private readonly IExtendApp _app;
        private readonly RequestHandle _handler;
        public RequestProxry(IExtendApp app)
        {
            this._app = app;
            this._handler = new RequestHandle(app);
        }

        public static bool VerifyLogin(HttpContext context)
        {
            bool result = UserState.Administrator.HasLogin;
            if (!result)
            {
                context.Response.Write("<script>window.parent.location.replace('/admin?return_url=')</script>");
            }
            return result;
        }

        public void HandleGet(HttpContext context, ref bool handled)
        {
            if (this._app.HanleRequestUse(_handler, context, false))
            {
                handled = true;
            }
        }

        public void HandlePost(HttpContext context, ref bool handled)
        {
            if (this._app.HanleRequestUse(_handler, context, true))
            {
                handled = true;
            }
        }

    }
}
