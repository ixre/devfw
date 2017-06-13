
//
// Copyright (C) 2007-2012 OPSoft INC,All rights reseved.
// 
// Project: OPS.Plugin
// FileName : AlipayRedirect.cs
// Author : PC-CWLIU (new.min@msn.com)
// Create : 2013/05/01 17:58:32
// Description :
//
// Get infromation of this software,please visit our site http://www.ops.cc
//
//

namespace Ops.Plugin.NetPay
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Web;
    using Com.Alipay.Mobile;
    using System.Collections.Specialized;
    using System.Xml;


    /// <summary>
    /// 支付宝即时到帐
    /// </summary>
    internal class AlipayMobile : IPay
    {
        public string GetPayRequest(Hashtable hash)
        {

            //==================================================================================
            //支付宝网关地址
            string GATEWAY_NEW = "http://wappaygw.alipay.com/service/rest.htm?";

            ////////////////////////////////////////////调用授权接口alipay.wap.trade.create.direct获取授权码token////////////////////////////////////////////

            //返回格式
            string format = "xml";
            //必填，不需要修改

            //返回格式
            string v = "2.0";
            //必填，不需要修改

            //请求号
            string req_id = DateTime.Now.ToString("yyyyMMddHHmmss");
            //必填，须保证每次请求都是唯一




            //商户号
            string partner = hash["alipay_key"].ToString();
            //密钥
            string key = hash["alipay_secret"].ToString();


            Config.Partner = partner;
            Config.Key = key;

            float product_fee = (float)hash["order_fee"];                                             //商品费用
            float transport_fee = float.Parse(hash["order_exp_fee"].ToString());     //物流费用


            //当前时间 yyyyMMdd
            string date = DateTime.Now.ToString("yyyyMMdd");





            //req_data详细信息

            //服务器异步通知页面路径
            //需http://格式的完整路径，不允许加?id=123这类自定义参数


            //页面跳转同步通知页面路径
            string notify_url = hash.Contains("notify_url") ? hash["notify_url"].ToString() : PayUtil.GetNotifyUrl(PayMethods.Alipay, PayApiType.Direct);

            //需http://格式的完整路径，不能加?id=123这类自定义参数

            //页面跳转同步通知页面路径
            string return_url = hash.Contains("return_url") ? hash["return_url"].ToString() : PayUtil.GetReturnUrl(PayMethods.Alipay, PayApiType.Direct);

            //操作中断返回地址
            string merchant_url = return_url;
            //用户付款中途退出返回商户的地址。需http://格式的完整路径，不允许加?id=123这类自定义参数

            //卖家支付宝帐户
            string seller_email = hash["seller_account"] as string;
            //必填

            //订单号，此处用时间和随机数生成，商户根据自己调整，保证唯一
            string out_trade_no = hash["order_no"].ToString();
            string host = hash["usr_host"] as string;

            string showUrl = hash["order_showurl"] as string;
            //商户网站订单系统中唯一订单号，必填

            //订单描述、订单详细、订单备注，显示在支付宝收银台里的“商品描述”里
            string subject = hash["order_subject"].ToString(); //parma.desc ?? "支付订单";
            //必填

            //订单总金额，显示在支付宝收银台里的“应付总额”里
            string total_fee = Convert.ToString(product_fee + transport_fee);
            //必填

            //请求业务参数详细
            string req_dataToken = "<direct_trade_create_req><notify_url>" + notify_url + "</notify_url><call_back_url>" + return_url + "</call_back_url><seller_account_name>" + seller_email + "</seller_account_name><out_trade_no>" + out_trade_no + "</out_trade_no><subject>" + subject + "</subject><total_fee>" + total_fee + "</total_fee><merchant_url>" + merchant_url + "</merchant_url></direct_trade_create_req>";
            //必填

            //把请求参数打包成数组
            Dictionary<string, string> sParaTempToken = new Dictionary<string, string>();
            sParaTempToken.Add("partner", Config.Partner);
            sParaTempToken.Add("_input_charset", Config.Input_charset.ToLower());
            sParaTempToken.Add("sec_id", Config.Sign_type.ToUpper());
            sParaTempToken.Add("service", "alipay.wap.trade.create.direct");
            sParaTempToken.Add("format", format);
            sParaTempToken.Add("v", v);
            sParaTempToken.Add("req_id", req_id);
            sParaTempToken.Add("req_data", req_dataToken);

            //建立请求
            string sHtmlTextToken = Submit.BuildRequest(GATEWAY_NEW, sParaTempToken);
            //URLDECODE返回的信息
            System.Text.Encoding code = System.Text.Encoding.GetEncoding(Config.Input_charset);
            sHtmlTextToken = HttpUtility.UrlDecode(sHtmlTextToken, code);

            //解析远程模拟提交后返回的信息
            Dictionary<string, string> dicHtmlTextToken = Submit.ParseResponse(sHtmlTextToken);

            //获取token
            string request_token = dicHtmlTextToken["request_token"];

            ////////////////////////////////////////////根据授权码token调用交易接口alipay.wap.auth.authAndExecute////////////////////////////////////////////


            //业务详细
            string req_data = "<auth_and_execute_req><request_token>" + request_token + "</request_token></auth_and_execute_req>";
            //必填

            //把请求参数打包成数组
            Dictionary<string, string> sParaTemp = new Dictionary<string, string>();
            sParaTemp.Add("partner", Config.Partner);
            sParaTemp.Add("_input_charset", Config.Input_charset.ToLower());
            sParaTemp.Add("sec_id", Config.Sign_type.ToUpper());
            sParaTemp.Add("service", "alipay.wap.auth.authAndExecute");
            sParaTemp.Add("format", format);
            sParaTemp.Add("v", v);
            sParaTemp.Add("req_data", req_data);

            //建立请求
            string sHtmlText = Submit.BuildRequest(GATEWAY_NEW, sParaTemp, "get", "确认");


            return sHtmlText;
        }

        /// <summary>
        /// 获取支付宝GET过来通知消息，并以“参数名=参数值”的形式组成数组
        /// </summary>
        /// <returns>request回来的信息组成的数组</returns>
        public Dictionary<string, string> GetRequestGet()
        {
            int i = 0;
            Dictionary<string, string> sArray = new Dictionary<string, string>();
            NameValueCollection coll;
            //Load Form variables into NameValueCollection variable.
            coll = HttpContext.Current.Request.QueryString;

            // Get names of all forms into a string array.
            String[] requestItem = coll.AllKeys;

            for (i = 0; i < requestItem.Length; i++)
            {
                sArray.Add(requestItem[i], HttpContext.Current.Request.QueryString[requestItem[i]]);
            }

            return sArray;
        }
        /// <summary>
        /// 获取支付宝POST过来通知消息，并以“参数名=参数值”的形式组成数组
        /// </summary>
        /// <returns>request回来的信息组成的数组</returns>
        public Dictionary<string, string> GetRequestPost()
        {
            int i = 0;
            Dictionary<string, string> sArray = new Dictionary<string, string>();
            NameValueCollection coll;
            //Load Form variables into NameValueCollection variable.
            coll = HttpContext.Current.Request.Form;

            // Get names of all forms into a string array.
            String[] requestItem = coll.AllKeys;

            for (i = 0; i < requestItem.Length; i++)
            {
                sArray.Add(requestItem[i], HttpContext.Current.Request.Form[requestItem[i]]);
            }

            return sArray;
        }

        public PaidHandleResult Return<T>(PayMointor<T> proc) where T : class
        {
            var request = HttpContext.Current.Request;
            Dictionary<string, string> sPara = this.GetRequestGet();

            if (sPara.Count > 0)//判断是否有带返回参数
            {
                Notify aliNotify = new Notify();
                bool verifyResult = aliNotify.VerifyReturn(sPara,request.QueryString["sign"]);
                if (verifyResult)//验证成功
                {
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    //请在这里加上商户的业务逻辑程序代码

                    //——请根据您的业务逻辑来编写程序（以下代码仅作参考）——
                    //获取支付宝的通知返回参数，可参考技术文档中页面跳转同步通知参数列表
                    string trade_no = request.QueryString["trade_no"];              //支付宝交易号
                    string order_no = request.QueryString["out_trade_no"];	        //获取订单号
                   // string total_fee = request.QueryString["total_fee"];            //获取总金额
                    //string subject = request.QueryString["subject"];                //商品名称、订单名称
                   // string body = request.QueryString["body"];                      //商品描述、订单备注、描述
                   // string buyer_email = request.QueryString["buyer_email"];        //买家支付宝账号
                    //string trade_status = request.QueryString["trade_status"];      //交易状态

                    //交易状态
                    string result = request.QueryString["result"];

                    proc.Init(order_no);

                    return PaidHandleResult.Success;

                    //打印页面
                    // Response.Write("验证成功<br />");
                    // Response.Write("trade_no=" + trade_no);

                    //——请根据您的业务逻辑来编写程序（以上代码仅作参考）——

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////

                }
            }

            return PaidHandleResult.Fail;
        }

        public string Notify<T>(PayMointor<T> proc) where T : class
        {
            /*
             * http://localhost:8080/pay/notify_1_3.html?body=%d6%a7%b8%b6%b6%a9%b5%a5&buyer_email=newmin.net@gmail.com&buyer_id=2088302384317810&exterface=create_direct_pay_by_user&is_success=T&notify_id=RqPnCoPT3K9%2Fvwbh3I73%2FBJ%2FCypvvG4k72c8QSXT5yE44%2FMSUK0zqoTBaXxNf4BPOKZI&notify_time=2013-03-21+11:42:53&notify_type=trade_status_sync&out_trade_no=20130125033249408&payment_type=1&seller_email=chiaus_im@163.com&seller_id=2088801968591818&subject=%E6%94%AF%E4%BB%98%E8%AE%A2%E5%8D%95&total_fee=0.01&trade_no=2013032154249581&trade_status=TRADE_SUCCESS&sign=af01b9aa0bd11df2a723eb5ef52e9298&sign_type=MD5
            */
            var request = HttpContext.Current.Request;
            //商户号
            string partner = Config.Partner;
            //密钥
            string key = Config.Key;

            Dictionary<string, string> sPara = this.GetRequestPost();
            if (sPara.Count > 0)//判断是否有带返回参数
            {
                Notify aliNotify = new Notify();
                bool verifyResult = aliNotify.VerifyNotify(sPara, request.Form["sign"]);

                if (verifyResult)//验证成功
                { //解密（如果是RSA签名需要解密，如果是MD5签名则下面一行清注释掉）
                   // sPara = aliNotify.Decrypt(sPara);

                    //XML解析notify_data数据
                    try
                    {
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(sPara["notify_data"]);
                        //商户订单号
                        string out_trade_no = xmlDoc.SelectSingleNode("/notify/out_trade_no").InnerText;
                        //支付宝交易号
                        string trade_no = xmlDoc.SelectSingleNode("/notify/trade_no").InnerText;
                        //交易状态
                        string trade_status = xmlDoc.SelectSingleNode("/notify/trade_status").InnerText;


                        proc.Init(out_trade_no);

                        if (trade_status == "TRADE_FINISHED")
                        {
                            //判断该笔订单是否在商户网站中已经做过处理
                            //如果没有做过处理，根据订单号（out_trade_no）在商户网站的订单系统中查到该笔订单的详细，并执行商户的业务程序
                            //如果有做过处理，不执行商户的业务程序

                            //注意：
                            //该种交易状态只在两种情况下出现
                            //1、开通了普通即时到账，买家付款成功后。
                            //2、开通了高级即时到账，从该笔交易成功时间算起，过了签约时的可退款时限（如：三个月以内可退款、一年以内可退款等）后。


                            proc.PaidSuccess();
                            return "success";  //请不要修改或删除
                        }
                        else if (trade_status == "TRADE_SUCCESS")
                        {
                            //判断该笔订单是否在商户网站中已经做过处理
                            //如果没有做过处理，根据订单号（out_trade_no）在商户网站的订单系统中查到该笔订单的详细，并执行商户的业务程序
                            //如果有做过处理，不执行商户的业务程序

                            //注意：
                            //该种交易状态只在一种情况下出现——开通了高级即时到账，买家付款成功后。

                            proc.PaidSuccess();

                           return "success";  //请不要修改或删除
                        }
                        else
                        {
                           return trade_status;
                        }

                    }
                    catch (Exception exc)
                    {
                        return exc.Message;
                    }


                    proc.PaidSuccess();

                    //log.Append("success:" + request.Form["trade_status"]+"\r\n"+order_no+"\r\n");
                    return "success";  //请不要修改或删除
                }
            }


            proc.PaidFail();

            //log.Append("fail:" + request.Form["trade_status"] + "\r\n");

            return "fail";

        }
    }


}
