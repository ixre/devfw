
//
// Copyright (C) 2007-2012 S1N1.COM,All rights reseved.
// 
// Project: OPS.Plugin
// FileName : PayApiType.cs
// Author : PC-CWLIU (new.min@msn.com)
// Create : 2013/05/01 17:58:32
// Description :
//
// Get infromation of this software,please visit our site http://www.ops.cc
//
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using J6.DevFw.Toolkit.ThirdApi.NetPay.Alipay;
using J6.DevFw.Toolkit.ThirdApi.NetPay.ChinaPay;

namespace J6.DevFw.Toolkit.ThirdApi.NetPay
{
    public static class PayUtil
    {
        /// <summary>
        /// 获取支付宝GET过来通知消息，并以“参数名=参数值”的形式组成数组
        /// </summary>
        /// <returns>request回来的信息组成的数组</returns>
        internal static SortedDictionary<string, string> GetRequestGet()
        {
            int i = 0;
            SortedDictionary<string, string> sPara = new SortedDictionary<string, string>();
            NameValueCollection coll;
            //Load Form variables into NameValueCollection variable.

            var request = HttpContext.Current.Request;
            coll = request.QueryString;

            // Get names of all forms into a string array.
            String[] requestItem = coll.AllKeys;

            for (i = 0; i < requestItem.Length; i++)
            {
                sPara.Add(requestItem[i], request.QueryString[requestItem[i]]);
            }

            return sPara;
        }
        /// <summary>
        /// 获取支付宝POST过来通知消息，并以“参数名=参数值”的形式组成数组
        /// </summary>
        /// <returns>request回来的信息组成的数组</returns>
        internal static SortedDictionary<string, string> GetRequestPost()
        {
            int i = 0;
            SortedDictionary<string, string> sPara = new SortedDictionary<string, string>();
            NameValueCollection coll;
            //Load Form variables into NameValueCollection variable.
            var request = HttpContext.Current.Request;
            coll = request.Form;
            //coll = request.QueryString;

            // Get names of all forms into a string array.
            String[] requestItem = coll.AllKeys;

            for (i = 0; i < requestItem.Length; i++)
            {
                sPara.Add(requestItem[i], request.Form[requestItem[i]]);
            }

            return sPara;
        }


        /// <summary>
        /// 设置支付日志记录
        /// </summary>
        /// <param name="msg"></param>
        public static void SetLogMessage(string msg)
        {
            global::System.Web.HttpContext.Current.Session["$shop.orderpay_message"] = msg;
        }

        /// <summary>
        /// 获取支付日志记录
        /// </summary>
        /// <returns></returns>
        public static string GetLogMessage()
        {
            return global::System.Web.HttpContext.Current.Session["$shop.orderpay_message"] as string;
        }

        /// <summary>
        /// 获取提示的地址
        /// </summary>
        /// <param name="ap">支付方式编号</param>
        /// <param name="at">接口类型,默认为1</param>
        /// <returns></returns>
        internal static string GetNotifyUrl(PayMethods ap, PayApiType at)
        {
            global::System.Web.HttpRequest req = global::System.Web.HttpContext.Current.Request;

            return String.Format("http://{0}{1}/netpay/notify_{2}_{3}.html",
                req.Url.Host, req.Url.Port == 80 ? "" : ":" + req.Url.Port, ((int)ap).ToString(), ((int)at).ToString());
        }

        /// <summary>
        /// 获取返回的地址
        /// </summary>
        /// <param name="ppid">支付方式编号</param>
        /// <param name="atid">接口类型,默认为1</param>
        /// <returns></returns>
        internal static string GetReturnUrl(PayMethods ap, PayApiType at)
        {
            global::System.Web.HttpRequest req = global::System.Web.HttpContext.Current.Request;


            return String.Format("http://{0}{1}/netpay/return_{2}_{3}.html",
                req.Url.Host, req.Url.Port == 80 ? "" : ":" + req.Url.Port, ((int)ap).ToString(), ((int)at).ToString());

            /*
            return String.Format("http://{0}{1}/pay/return_{2}_{3}.html",
               "www.lmgdto.com","", ((int)ap).ToString(), ((int)at).ToString());
            */
        }


        /// <summary>
        /// 获取网关地址
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="pt"></param>
        /// <param name="ht"></param>
        /// <returns></returns>
        public static string GetGatewayStr(PayMethods pm, PayApiType pt, Hashtable ht)
        {
            IPay _pay = null;
            if (pm == PayMethods.Alipay)
            {
                if (ht.Contains("bank"))
                {
                    pt = PayApiType.Direct;
                }

                if (pt == PayApiType.Direct)
                {
                    _pay = new AlipayRedirect();
                }
                else if (pt == PayApiType.Normal)
                {
                    _pay = new AlipayNormal();
                }
                else if(pt== PayApiType.Mobile)
                {
                	
                	_pay=new AlipayMobile();
                }
                else
                {
                    _pay = new AlipayComfireApi();
                }

            }
            else if (pm == PayMethods.Tenpay)
            {
                //财付通默认支持所有
                _pay = new Tenpay.Tenpay();
            }
            else if (pm == PayMethods.ChinaPay)
            {
                _pay = new ChinaPayApi();
            }


            return _pay == null ? String.Empty : _pay.GetPayRequest(ht);


        }



        public static PaidHandleResult PayReturn<T>(PayMethods pm, PayApiType pt, PayMointor<T> proc) where T:class
        {
            IPay _pay = null;
            if (pm == PayMethods.Alipay)
            {
                if (pt == PayApiType.Direct)
                {
                	_pay = new AlipayRedirect();
                }
                else if (pt == PayApiType.Normal)
                {
                    _pay = new AlipayNormal();
                }
                else if(pt== PayApiType.Mobile)
                {
                	_pay=new AlipayMobile();
                }
                else
                {
                    _pay = new AlipayComfireApi();
                }

            }
            else if (pm == PayMethods.Tenpay)
            {
                //财付通默认支持所有
                _pay = new Tenpay.Tenpay();
            }
            else if (pm == PayMethods.ChinaPay)
            {
                _pay = new ChinaPayApi();
            }


            if (_pay == null)
            {
                SetLogMessage("不支持的支付方式");
                return PaidHandleResult.Fail;
            }
            else
            {
                return _pay.Return(proc);
            }
        }

        public static string PayNotify<T>(PayMethods pm, PayApiType pt, PayMointor<T> proc) where T : class
        {
            IPay _pay = null;
            if (pm == PayMethods.Alipay)
            {
                if (pt == PayApiType.Direct)
                {
                    _pay = new AlipayRedirect();
                }
                else if (pt == PayApiType.Normal)
                {
                    _pay = new AlipayNormal();
                }
                else if(pt== PayApiType.Mobile)
                {
                	_pay=new AlipayMobile();
                }
                else
                {
                    _pay = new AlipayComfireApi();
                }

            }
            else if (pm == PayMethods.Tenpay)
            {
                //财付通默认支持所有
                _pay = new Tenpay.Tenpay();
            }
            else if (pm == PayMethods.ChinaPay)
            {
                _pay = new ChinaPayApi();
            }


            return _pay == null ? "不支持的支付方式" : _pay.Notify(proc);

        }


        public static string GetGatewaySubmit(string url, SortedDictionary<string, string> dict)
        {
            const string format = "<form method='POST' action='%url%'>%fields%</form><script>document.forms[0].submit();</script>";
            if (dict == null || !dict.Keys.Contains("order_no"))
            {
                throw new ArgumentException("字典不能为空，且必须包括orderNo键值!");
            }

            StringBuilder sb = new StringBuilder();

            foreach (string key in dict.Keys)
            {
                sb.Append("<input type='hidden' name='").Append(key).Append("' value='")
                    .Append((dict[key] ?? "").Replace("'", "\\'")).Append("'/>");
            }

            return format.Replace("%url%", url).Replace("%fields%", sb.ToString());
        }
    }

}
