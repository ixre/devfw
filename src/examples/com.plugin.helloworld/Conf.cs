/**
 * Copyright (C) 2007-2015 OPSoft INC,All rights reseved.
 * Get more infromation of this software,please visit site http://cms.ops.cc
 * 
 * name : Config.cs
 * author : newmin (new.min@msn.com)
 * date : 2012/12/01 23:00:00
 * description : 
 * history : 
 */

using AtNet.DevFw.PluginKernel;
using Com.PluginKernel;

namespace Com.Plugin
{
	/// <summary>
	/// Description of Config.
	/// </summary>
	public class Conf
	{
		/// <summary>
		/// 是否为开发模式
		/// </summary>
		public static bool DebugMode=!true;
		
        internal static PluginPackAttribute PluginAttrs;
	    public static string DB_PREFIX="";

	    static Conf()
		{
            PluginAttrs = PluginUtil.GetAttribute<Main>();

            //初始化配置


			//bool isChanged=false;
            //if(!PackAttr.Settings.Contains("notify.workerindent"))
            //{
            //    PackAttr.Settings.Append("notify.workerindent","");
            //    isChanged=true;
            //}
			
            //if(!PackAttr.Settings.Contains("alipay.account"))
            //{
            //    PackAttr.Settings.Append("alipay.account","");
            //    isChanged=true;
            //}
			
            //if(!PackAttr.Settings.Contains("alipay.userkey"))
            //{
            //    PackAttr.Settings.Append("alipay.userkey","");
            //    isChanged=true;
            //}
			
            //if(!PackAttr.Settings.Contains("alipay.secret"))
            //{
            //    PackAttr.Settings.Append("alipay.secret","");
            //    isChanged=true;
            //}

           // if (isChanged) PluginAttrs.Settings.Flush();
				
		}


	}
}
