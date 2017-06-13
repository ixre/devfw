
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
    using Com.Alipay;


    /// <summary>
    /// 支付宝即时到帐
    /// </summary>
    internal class AlipayRedirect : IPay
    {
        public string GetPayRequest(Hashtable hash)
        {
            //商户号
            string partner = hash["alipay_key"].ToString();
            //密钥
            string key = hash["alipay_secret"].ToString();


            Config.Partner = partner;
            Config.Key = key;

            
            string bankCode = hash.Contains("bank") ? hash["bank"].ToString() : "DEFAULT";

            float product_fee = (float)hash["order_fee"];                                             //商品费用
            float transport_fee = float.Parse(hash["order_exp_fee"].ToString());     //物流费用

            string order_desc = (hash["order_desc"]??"").ToString();;

            //订单号，此处用时间和随机数生成，商户根据自己调整，保证唯一
            string out_trade_no = hash["order_no"].ToString();
            string host = hash["usr_host"] as string;

            string showUrl = hash["order_showurl"] as string;

            //卖家支付宝帐户
            string seller_email = hash["seller_account"] as string;

            //订单描述、订单详细、订单备注，显示在支付宝收银台里的“商品描述”里
            string subject = hash["order_subject"].ToString(); //parma.desc ?? "支付订单";

            //当前时间 yyyyMMdd
            string date = DateTime.Now.ToString("yyyyMMdd");
            //todo
            string body = order_desc;

            //订单总金额，显示在支付宝收银台里的“应付总额”里
            string total_fee = Convert.ToString(product_fee + transport_fee);



            ////////////////////////////////////////////请求参数////////////////////////////////////////////

            //支付类型
            string payment_type = "1";
            //必填，不能修改
            //服务器异步通知页面路径


            string notify_url =hash.Contains("notify_url")?hash["notify_url"].ToString():PayUtil.GetNotifyUrl(PayMethods.Alipay, PayApiType.Direct);

            //需http://格式的完整路径，不能加?id=123这类自定义参数

            //页面跳转同步通知页面路径
            string return_url =hash.Contains("return_url")?hash["return_url"].ToString():PayUtil.GetReturnUrl(PayMethods.Alipay, PayApiType.Direct);
            //需http://格式的完整路径，不能加?id=123这类自定义参数，不能写成http://localhost/


            //防钓鱼时间戳
            string anti_phishing_key = "";
            //若要使用请调用类文件submit中的query_timestamp函数

            //客户端的IP地址
            string exter_invoke_ip = "";
            //非局域网的外网IP地址，如：221.0.0.1


            ////////////////////////////////////////////////////////////////////////////////////////////////

            //把请求参数打包成数组
            SortedDictionary<string, string> sParaTemp = new SortedDictionary<string, string>();
            sParaTemp.Add("partner", Config.Partner);
            sParaTemp.Add("_input_charset", Config.Input_charset.ToLower());
            sParaTemp.Add("service", "create_direct_pay_by_user");
            sParaTemp.Add("payment_type", payment_type);
            sParaTemp.Add("notify_url", notify_url);
            sParaTemp.Add("return_url", return_url);
            sParaTemp.Add("seller_email", seller_email);
            sParaTemp.Add("out_trade_no", out_trade_no);
            sParaTemp.Add("subject", subject);
            sParaTemp.Add("total_fee", total_fee);
            sParaTemp.Add("body", body);
            sParaTemp.Add("show_url", showUrl);
            sParaTemp.Add("anti_phishing_key", anti_phishing_key);
            sParaTemp.Add("exter_invoke_ip", exter_invoke_ip);

            
            if (bankCode != "DEFAULT")
            {
                sParaTemp.Add("defaultbank", bankCode);
                sParaTemp.Add("payment", "bankPay");
            }

            //建立请求
            string sHtmlText = Submit.BuildRequest(sParaTemp, "get", "确认");


            return sHtmlText;
        }

        public PaidHandleResult Return<T>(PayMointor<T> proc) where T : class
        {
        	
        	// http://www.jin-ec.com/mapfre/pay/notify?body=%E8%AE%A2%E5%8D%952013122345312
        	// &buyer_email=newmin.net%40gmail.com&buyer_id=2088302384317810&exterface=create_direct_pay_by_user
            // &is_success=T&notify_id=RqPnCoPT3K9%252Fvwbh3I75KL02sthKJHtG2dh1Mg5RF5qgJKDY8jd2nu0ChZQAfPMX38xu
        	// &notify_time=2013-12-23+14%3A18%3A53&notify_type=trade_status_sync&out_trade_no=2013122345312
        	//&payment_type=1&seller_email=clientinfo%40roadchina.com.cn&seller_id=2088201937033268
        	//&subject=%E6%98%A5%E8%BF%90%E4%BF%9D%E9%9A%9C%E4%BA%A7%E5%93%81&total_fee=0.01
        	// &trade_no=2013122303460581&trade_status=TRADE_SUCCESS&sign=c8c01b5ac095540f0a35d4f7f5831956&sign_type=MD5
        	//
            var request = HttpContext.Current.Request;
            SortedDictionary<string, string> sPara = PayUtil.GetRequestGet();

            if (sPara.Count > 0)//判断是否有带返回参数
            {
                Notify aliNotify = new Notify();
                bool verifyResult = aliNotify.Verify(sPara, request.QueryString["notify_id"], request.QueryString["sign"]);
                if (verifyResult)//验证成功
                {
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    //请在这里加上商户的业务逻辑程序代码

                    //——请根据您的业务逻辑来编写程序（以下代码仅作参考）——
                    //获取支付宝的通知返回参数，可参考技术文档中页面跳转同步通知参数列表
                    string trade_no = request.QueryString["trade_no"];              //支付宝交易号
                    string order_no = request.QueryString["out_trade_no"];	        //获取订单号
                    string total_fee = request.QueryString["total_fee"];            //获取总金额
                    string subject = request.QueryString["subject"];                //商品名称、订单名称
                    string body = request.QueryString["body"];                      //商品描述、订单备注、描述
                    string buyer_email = request.QueryString["buyer_email"];        //买家支付宝账号
                    string trade_status = request.QueryString["trade_status"];      //交易状态

                    proc.Init(order_no);


                    if (request.QueryString["trade_status"] == "TRADE_FINISHED" || request.QueryString["trade_status"] == "TRADE_SUCCESS")
                    {
                        return PaidHandleResult.Success;
                    }

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

            SortedDictionary<string, string> sPara = PayUtil.GetRequestPost();
            if (sPara.Count > 0)//判断是否有带返回参数
            {
                Notify aliNotify = new Notify();
                bool verifyResult = aliNotify.Verify(sPara, request.Form["notify_id"], request.Form["sign"]);

                if (verifyResult)//验证成功
                {
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    //请在这里加上商户的业务逻辑程序代码

                    //——请根据您的业务逻辑来编写程序（以下代码仅作参考）——
                    //获取支付宝的通知返回参数，可参考技术文档中服务器异步通知参数列表
                    string trade_no = request.Form["trade_no"];         //支付宝交易号
                    string order_no = request.Form["out_trade_no"];     //获取订单号
                    string total_fee = request.Form["total_fee"];       //获取总金额
                    string subject = request.Form["subject"];           //商品名称、订单名称
                    string body = request.Form["body"];                 //商品描述、订单备注、描述
                    string buyer_email = request.Form["buyer_email"];   //买家支付宝账号
                    string trade_status = request.Form["trade_status"]; //交易状态


                    proc.Init(order_no);

                    if (request.Form["trade_status"] == "TRADE_FINISHED")
                    {
                        //判断该笔订单是否在商户网站中已经做过处理
                        //如果没有做过处理，根据订单号（out_trade_no）在商户网站的订单系统中查到该笔订单的详细，并执行商户的业务程序
                        //如果有做过处理，不执行商户的业务程序

                        //注意：
                        //该种交易状态只在两种情况下出现
                        //1、开通了普通即时到账，买家付款成功后。
                        //2、开通了高级即时到账，从该笔交易成功时间算起，过了签约时的可退款时限（如：三个月以内可退款、一年以内可退款等）后。



                    }
                    else if (request.Form["trade_status"] == "TRADE_SUCCESS")
                    {
                        //判断该笔订单是否在商户网站中已经做过处理
                        //如果没有做过处理，根据订单号（out_trade_no）在商户网站的订单系统中查到该笔订单的详细，并执行商户的业务程序
                        //如果有做过处理，不执行商户的业务程序

                        //注意：
                        //该种交易状态只在一种情况下出现——开通了高级即时到账，买家付款成功后。



                    }
                    else
                    {
                    }

                    //——请根据您的业务逻辑来编写程序（以上代码仅作参考）——

                    proc.PaidSuccess();

                    //log.Append("success:" + request.Form["trade_status"]+"\r\n"+order_no+"\r\n");
                    return "success";  //请不要修改或删除

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                }
                else//验证失败
                {

                    proc.PaidFail();

                    //log.Append("fail:" + request.Form["trade_status"] + "\r\n");

                    return "fail";
                }
            }
            else
            {
                //log.Append("无通知参数:" + request.Form["trade_status"] + "\r\n");
                return "无通知参数";
            }
        }
    }


}
