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

using System;
using AtNet.DevFw.Framework;
using AtNet.DevFw.PluginKernel;
using Senparc.Weixin.MP.TenPayLib;
using Senparc.Weixin.MP.TenPayLibV3;

namespace Com.Plugin
{
    /// <summary>
    /// Description of Config.
    /// </summary>
    public class Config
    {
        /// <summary>
        /// 是否为开发模式
        /// </summary>
        public static bool DebugMode = !true;

        private static IPlugin app;

        internal static PluginPackAttribute PluginAttr;
        public static string DB_PREFIX="";

        public static void Init(IPlugin plugin)
        {
            app = plugin;
            PluginAttr = plugin.GetAttribute();
            initCfg(PluginAttr);
            InitWeixin(PluginAttr.Settings);

            //提供微信支付信息
            var weixinPay_PartnerId = PluginAttr.Settings["WeixinPay_PartnerId"];
            var weixinPay_Key = PluginAttr.Settings["WeixinPay_Key"];
            var weixinPay_AppId = PluginAttr.Settings["WeixinPay_AppId"];
            var weixinPay_AppKey = PluginAttr.Settings["WeixinPay_AppKey"];
            var weixinPay_TenpayNotify = PluginAttr.Settings["WeixinPay_TenpayNotify"];
            var tenPayV3_MchId = PluginAttr.Settings["TenPayV3_MchId"];
            var tenPayV3_Key = PluginAttr.Settings["TenPayV3_Key"];
            var tenPayV3_AppId = PluginAttr.Settings["TenPayV3_AppId"];
            var tenPayV3_AppSecret = PluginAttr.Settings["TenPayV3_AppSecret"];
            var tenPayV3_TenpayNotify = PluginAttr.Settings["TenPayV3_TenpayNotify"];
            var weixinPayInfo = new TenPayInfo(weixinPay_PartnerId, weixinPay_Key, weixinPay_AppId, weixinPay_AppKey, weixinPay_TenpayNotify);
            TenPayInfoCollection.Register(weixinPayInfo);
            var tenPayV3Info = new TenPayV3Info(tenPayV3_AppId, tenPayV3_AppSecret, tenPayV3_MchId, tenPayV3_Key,
            tenPayV3_TenpayNotify);
            TenPayV3InfoCollection.Register(tenPayV3Info);



        }

        public static void InitWeixin(SettingFile set)
        {
            Variables.Token = set["Weixin_Token"];
            Variables.AppId = set["Weixin_AppId"];
            Variables.AppSecret = set["Weixin_AppSecret"];
            Variables.AppEncodeString = set["Weixin_AppEncodeString"];
            Variables.ApiDomain = set["Weixin_ApiDomain"];
            Variables.MenuButtons = set["Weixin_MenuButtons"];
            Variables.WxWelcomeMessage = set["Weixin_WelcomeMessage"] ?? "";
            Variables.WxEnterMessage = set["Weixin_EnterMessage"] ?? "";
            Variables.WxDefaultResponseMessage = set["Weixin_DefaultResponseMessage"]??"";
        }

        private static void initCfg(PluginPackAttribute attr)
        {

            //初始化配置
            bool isChanged = false;
            if (!attr.Settings.Contains("WeixinPay_PartnerId"))
            {
                attr.Settings.Add("WeixinPay_PartnerId", "");
                isChanged = true;
            }
            if (!attr.Settings.Contains("WeixinPay_Key"))
            {
                attr.Settings.Add("WeixinPay_Key", "");
                isChanged = true;
            }
            if (!attr.Settings.Contains("WeixinPay_AppId"))
            {
                attr.Settings.Add("WeixinPay_AppId", "");
                isChanged = true;
            }
            if (!attr.Settings.Contains("WeixinPay_AppKey"))
            {
                attr.Settings.Add("WeixinPay_AppKey", "");
                isChanged = true;
            }
            if (!attr.Settings.Contains("WeixinPay_TenpayNotify"))
            {
                attr.Settings.Add("WeixinPay_TenpayNotify", "");
                isChanged = true;
            }
            if (!attr.Settings.Contains("TenPayV3_MchId"))
            {
                attr.Settings.Add("TenPayV3_MchId", "");
                isChanged = true;
            }

            if (!attr.Settings.Contains("TenPayV3_Key"))
            {
                attr.Settings.Add("TenPayV3_Key", "");
                isChanged = true;
            }

            if (!attr.Settings.Contains("TenPayV3_AppId"))
            {
                attr.Settings.Add("TenPayV3_AppId", "");
                isChanged = true;
            }
            if (!attr.Settings.Contains("TenPayV3_AppSecret"))
            {
                attr.Settings.Add("TenPayV3_AppSecret", "");
                isChanged = true;
            }
            if (!attr.Settings.Contains("TenPayV3_TenpayNotify"))
            {
                attr.Settings.Add("TenPayV3_TenpayNotify", "");
                isChanged = true;
            }

            if (!attr.Settings.Contains("Weixin_AppId"))
            {
                attr.Settings.Add("Weixin_AppId", "填写微信AppId");
                isChanged = true;
            }

            if (!attr.Settings.Contains("Weixin_AppSecret"))
            {
                attr.Settings.Add("Weixin_AppSecret", "填写微信AppSecret");
                isChanged = true;
            }

            if (!attr.Settings.Contains("Weixin_AppEncodeString"))
            {
                attr.Settings.Add("Weixin_AppEncodeString", "填写微信App解密字符串");
                isChanged = true;
            }

            if (!attr.Settings.Contains("Weixin_Token"))
            {
                attr.Settings.Add("Weixin_Token", "填写微信token");
                isChanged = true;
            }

            if (!attr.Settings.Contains("Weixin_ApiDomain"))
            {
                attr.Settings.Add("Weixin_ApiDomain", "填写用于对接微信的域名：如http://www.ops.cc/weixin/。");
                isChanged = true;
            }

            if (!attr.Settings.Contains("Weixin_WelcomeMessage"))
            {
                attr.Settings.Add("Weixin_WelcomeMessage", "欢迎来到微信！");
                isChanged = true;
            }

            if (!attr.Settings.Contains("Weixin_EnterMessage"))
            {
                attr.Settings.Add("Weixin_EnterMessage", "");
                isChanged = true;
            }


            if (!attr.Settings.Contains("Weixin_MenuButtons"))
            {
                attr.Settings.Add("Weixin_MenuButtons", "填写微信自定义按钮");
                isChanged = true;
            }

            if (isChanged) attr.Settings.Flush();
        }

        public static void Logln(String line)
        {
            app.Logln(line);
        }
    }
}
