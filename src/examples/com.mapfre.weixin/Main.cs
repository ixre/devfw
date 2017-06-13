/**
 * Copyright (C) 2007-2015 S1N1.COM,All rights reseved.
 * Get more infromation of this software,please visit site http://cms.ops.cc
 * 
 * name : Main.cs
 * author : newmin (new.min@msn.com)
 * date : 2012/12/01 23:00:00
 * description : 
 * history : 
 */
using System;
using JR.DevFw.PluginKernel;
using JR.DevFw.Web.Plugin;
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
			IPluginApp _app = app as IPluginApp;
			if(_app!=null)
            {
                Config.Init(this);

				RequestProxry req=new RequestProxry(_app,this);
				_app.Register(this,req.HandleGet,req.HandlePost);
                _app.MapPluginRoute(this);
                _app.MapPluginRoute(this,"wxm");
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
