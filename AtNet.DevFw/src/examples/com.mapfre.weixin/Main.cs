/**
 * Copyright (C) 2007-2015 OPSoft INC,All rights reseved.
 * Get more infromation of this software,please visit site http://cms.ops.cc
 * 
 * name : Main.cs
 * author : newmin (new.min@msn.com)
 * date : 2012/12/01 23:00:00
 * description : 
 * history : 
 */
using System;
using AtNet.DevFw.PluginKernel;
using AtNet.DevFw.Web.Plugins;
using Senparc.Weixin.MP.CommonAPIs;

namespace Com.Plugin
{
	/// <summary>
	/// Description of Main.
	/// </summary>
	public class Main:IPlugin
	{
        private PluginPackAttribute _attr;
		public PluginConnectionResult Connect(IPluginHost app)
		{
			IExtendApp _app = app as IExtendApp;
			if(_app!=null)
            {
                Config.Init(this);

				RequestProxry req=new RequestProxry(_app,this);
				_app.Register(this,req.HandleGet,req.HandlePost);
                _app.MapExtendPluginRoute(this);
                _app.MapExtendPluginRoute(this,"wxm");
                this.init();
			}


			return PluginConnectionResult.Success;
		}

        private void init()
        {
            //注册appid和appsecret
            if (!AccessTokenContainer.CheckRegistered(Variables.AppId))
            {
                AccessTokenContainer.Register(Variables.AppId, Variables.AppSecret);
            }
          
        }
		
		public bool Install()
		{
			return true;
		}
		
		public bool Uninstall()
		{
			return true;
		}
		
		public void Run()
		{
			
		}
		
		public void Pause()
		{
		}
		
		public string GetMessage()
		{
			return "";
		}
		
		public object Call(string method, params object[] parameters)
		{
			throw new NotImplementedException();
		}


        public void Logln(string line)
        {
            Logger.Println("[WeiXin]:"+line);
        }


        public PluginPackAttribute GetAttribute()
        {

            if (this._attr == null)
            {
                this._attr = PluginUtil.GetAttribute(this);
            }
            return this._attr;
        }
    }
}
