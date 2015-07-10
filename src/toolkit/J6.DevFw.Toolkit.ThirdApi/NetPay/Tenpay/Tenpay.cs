
//
// Copyright (C) 2007-2012 S1N1.COM,All rights reseved.
// 
// Project: OPS.Plugin
// FileName : Tenpay.cs
// Author : PC-CWLIU (new.min@msn.com)
// Create : 2013/05/01 17:58:32
// Description :
//
// Get infromation of this software,please visit our site http://www.ops.cc
//
//

using System;
using System.Collections;
using System.Web;
using J6.DevFw.Toolkit.ThirdApi.NetPay.Tenpay.Code;

namespace J6.DevFw.Toolkit.ThirdApi.NetPay.Tenpay
{
    /// <summary>
    /// 财付通
    /// </summary>
    internal class Tenpay : IPay
    {
        public string GetPayRequest(Hashtable hash)
        {
            /*接口需要的参数如下：
                       * ----------------------------------
                       * @product_fee : 产品价格
                       * @express_fee : 快递费用
                       * @orderno        : 订单号
                       * @desc            : 订单描述
                       * -----------------------------------
                       */

            //商户号
            string partner = hash["tenpay_key"] as string;
            //密钥
            string key = hash["tenpay_secret"] as string;


            TenpayUtil.bargainor_id = partner;
            TenpayUtil.tenpay_key = key;
            TenpayUtil.tenpay_return = PayUtil.GetReturnUrl(PayMethods.Tenpay, PayApiType.Direct);
            TenpayUtil.tenpay_notify = PayUtil.GetNotifyUrl(PayMethods.Tenpay, PayApiType.Direct);

            int product_fee = (int)(((decimal)hash["order_fee"]) * 100);                                             //商品费用
            decimal transport_fee = decimal.Parse(hash["order_exp_fee"].ToString());     //物流费用

            string bankCode = hash.Contains("bank") ? hash["bank"].ToString() : "DEFAULT";

            string order_desc = hash["order_desc"] as string;//HttpUtility.UrlEncode( ht["order_desc"] as string);

            //订单号，此处用时间和随机数生成，商户根据自己调整，保证唯一
            string out_trade_no = hash["order_no"] as string;
            string host = hash["usr_host"] as string;


            //当前时间 yyyyMMdd
            string date = DateTime.Now.ToString("yyyyMMdd");


            //创建RequestHandler实例
            RequestHandler reqHandler = new RequestHandler(HttpContext.Current);
            //初始化
            reqHandler.init();
            //设置密钥
            reqHandler.setKey(TenpayUtil.tenpay_key);
            reqHandler.setGateUrl("https://gw.tenpay.com/gateway/pay.htm");


            //-----------------------------
            //设置支付参数
            //-----------------------------
            reqHandler.setParameter("partner", TenpayUtil.bargainor_id);		        //商户号
            reqHandler.setParameter("out_trade_no", out_trade_no);	 //商家订单号
            reqHandler.setParameter("total_fee", product_fee.ToString());                 //商品金额,以分为单位
            reqHandler.setParameter("return_url", TenpayUtil.tenpay_return);		    //交易完成后跳转的URL
            reqHandler.setParameter("notify_url", TenpayUtil.tenpay_notify);		    //接收财付通通知的URL
            reqHandler.setParameter("body", order_desc);	                    //商品描述
            reqHandler.setParameter("bank_type", bankCode);		    //银行类型(中介担保时此参数无效)
            reqHandler.setParameter("spbill_create_ip", host);   //用户的公网ip，不是商户服务器IP
            reqHandler.setParameter("fee_type", "1");                    //币种，1人民币
            reqHandler.setParameter("subject", out_trade_no);              //商品名称(中介交易时必填)


            //系统可选参数
            reqHandler.setParameter("sign_type", "MD5");
            reqHandler.setParameter("service_version", "1.0");
            reqHandler.setParameter("input_charset", "UTF-8");
            reqHandler.setParameter("sign_key_index", "1");

            //业务可选参数

            reqHandler.setParameter("attach", "");                                  //附加数据，原样返回
            reqHandler.setParameter("product_fee", "0");                 //商品费用，必须保证transport_fee + product_fee=total_fee
            reqHandler.setParameter("transport_fee", "0");               //物流费用，必须保证transport_fee + product_fee=total_fee
            reqHandler.setParameter("time_start", DateTime.Now.ToString("yyyyMMddHHmmss"));            //订单生成时间，格式为yyyymmddhhmmss
            reqHandler.setParameter("time_expire", "");                 //订单失效时间，格式为yyyymmddhhmmss
            reqHandler.setParameter("buyer_id", "");                    //买方财付通账号
            reqHandler.setParameter("goods_tag", "");                   //商品标记
            reqHandler.setParameter("trade_mode", "1");                 //交易模式，1即时到账(默认)，2中介担保，3后台选择（买家进支付中心列表选择）
            reqHandler.setParameter("transport_desc", "");              //物流说明
            reqHandler.setParameter("trans_type", "1");                  //交易类型，1实物交易，2虚拟交易
            reqHandler.setParameter("agentid", "");                     //平台ID
            reqHandler.setParameter("agent_type", "");                  //代理模式，0无代理(默认)，1表示卡易售模式，2表示网店模式
            reqHandler.setParameter("seller_id", "");                   //卖家商户号，为空则等同于partner






            //获取请求带参数的url
            string requestUrl = reqHandler.getRequestURL();

            //Response.Redirect(requestUrl);
            return requestUrl;
            //Get的实现方式
            // string a_link = "<a target=\"_blank\" href=\"" + requestUrl + "\">" + "财付通支付" + "</a>";
            //  Response.Write(a_link);
            // return a_link;


            //post实现方式

            /* Response.Write("<form method=\"post\" action=\""+ reqHandler.getGateUrl() + "\" >\n");
             Hashtable ht = reqHandler.getAllParameters();
             foreach(DictionaryEntry de in ht) 
             {
                 Response.Write("<input type=\"hidden\" name=\"" + de.Key + "\" value=\"" + de.Value + "\" >\n");
             }
             Response.Write("<input type=\"submit\" value=\"财付通支付\" >\n</form>\n");*/


            //获取debug信息,建议把请求和debug信息写入日志，方便定位问题
            // string debuginfo = reqHandler.getDebugInfo();
            // Response.Write("<br/>requestUrl:" + requestUrl + "<br/>");
            // Response.Write("<br/>debuginfo:" + debuginfo + "<br/>");
        }

        public PaidHandleResult Return<T>(PayMointor<T> proc) where T : class
        {
            //商户号
            string partner = TenpayUtil.bargainor_id;// p.PayApiKey;
            //密钥
            string key = TenpayUtil.tenpay_key;// p.PayApiSecret;


            //创建ResponseHandler实例
            ResponseHandler resHandler = new ResponseHandler(System.Web.HttpContext.Current);
            resHandler.setKey(key);

            //判断签名
            if (true || resHandler.isTenpaySign())
            {

                ///通知id
                string notify_id = resHandler.getParameter("notify_id");
                //商户订单号
                string out_trade_no = resHandler.getParameter("out_trade_no");
                //财付通订单号
                string transaction_id = resHandler.getParameter("transaction_id");
                //金额,以分为单位
                string total_fee = resHandler.getParameter("total_fee");
                //如果有使用折扣券，discount有值，total_fee+discount=原请求的total_fee
                string discount = resHandler.getParameter("discount");
                //支付结果
                string trade_state = resHandler.getParameter("trade_state");
                //交易模式，1即时到账，2中介担保
                string trade_mode = resHandler.getParameter("trade_mode");


                proc.Init(out_trade_no);


                if ("1".Equals(trade_mode))
                {

                    //即时到账 
                    if ("0".Equals(trade_state))
                    {
                        //------------------------------
                        //即时到账处理业务开始
                        //------------------------------

                        //处理数据库逻辑
                        //注意交易单不要重复处理
                        //注意判断返回金额






                        //------------------------------
                        //即时到账处理业务完毕
                        //------------------------------


                        //SetLogMessage("即时到帐付款成功");
                        return PaidHandleResult.Success;



                        //给财付通系统发送成功信息，财付通系统收到此结果后不再进行后续通知

                    }
                    else
                    {
                        //SetLogMessage("即时到账支付失败");
                        return PaidHandleResult.Fail;
                    }

                }
                else if ("2".Equals(trade_mode))
                {    //中介担保
                    if ("0".Equals(trade_state))
                    {
                        //------------------------------
                        //中介担保处理业务开始
                        //------------------------------

                        //处理数据库逻辑
                        //注意交易单不要重复处理
                        //注意判断返回金额

                        //------------------------------
                        //中介担保处理业务完毕
                        //------------------------------


                        //Response.Write("中介担保付款成功");
                        //给财付通系统发送成功信息，财付通系统收到此结果后不再进行后续通知

                        //SetLogMessage("中介担保付款成功");
                        return PaidHandleResult.Success;
                    }
                    else
                    {
                        PayUtil.SetLogMessage("trade_state=" + trade_state);
                        return PaidHandleResult.Fail;
                    }
                }
            }
            else
            {
                //Response.Write("认证签名失败");
                PayUtil.SetLogMessage("认证签名失败");
                return PaidHandleResult.Fail;
            }

            return PaidHandleResult.Fail;

        }

        public string Notify<T>(PayMointor<T> proc) where T : class
        {
            //商户号
            string partner = TenpayUtil.bargainor_id;
            //密钥
            string key = TenpayUtil.tenpay_key;


            //创建ResponseHandler实例
            ResponseHandler resHandler = new ResponseHandler(System.Web.HttpContext.Current);
            resHandler.setKey(key);

            //判断签名
            if (resHandler.isTenpaySign())
            {

                ///通知id
                string notify_id = resHandler.getParameter("notify_id");


                //通过通知ID查询，确保通知来至财付通
                //创建查询请求
                RequestHandler queryReq = new RequestHandler(System.Web.HttpContext.Current);
                queryReq.init();
                queryReq.setKey(key);
                queryReq.setGateUrl("https://gw.tenpay.com/gateway/verifynotifyid.xml");
                queryReq.setParameter("partner", partner);
                queryReq.setParameter("notify_id", notify_id);

                //通信对象
                TenpayHttpClient httpClient = new TenpayHttpClient();
                httpClient.setTimeOut(5);
                //设置请求内容
                httpClient.setReqContent(queryReq.getRequestURL());

                //后台调用
                if (httpClient.call())
                {
                    //设置结果参数
                    ClientResponseHandler queryRes = new ClientResponseHandler();
                    queryRes.setContent(httpClient.getResContent());
                    queryRes.setKey(key);

                    //判断签名及结果
                    //只有签名正确,retcode为0，trade_state为0才是支付成功
                    if (queryRes.isTenpaySign() && queryRes.getParameter("retcode") == "0" && queryRes.getParameter("trade_state") == "0" && queryRes.getParameter("trade_mode") == "1")
                    {
                        //取结果参数做业务处理
                        string out_trade_no = queryRes.getParameter("out_trade_no");
                        //财付通订单号
                        string transaction_id = queryRes.getParameter("transaction_id");
                        //金额,以分为单位
                        string total_fee = queryRes.getParameter("total_fee");
                        //如果有使用折扣券，discount有值，total_fee+discount=原请求的total_fee
                        string discount = queryRes.getParameter("discount");
                        //支付结果
                        string trade_state = resHandler.getParameter("trade_state");
                        //交易模式，1即时到帐 2中介担保
                        string trade_mode = resHandler.getParameter("trade_mode");

                        proc.Init(out_trade_no);


                        //判断签名及结果
                        if (queryRes.isTenpaySign() && "0".Equals(queryRes.getParameter("retcode")))
                        {
                            // Response.Write("id验证成功");

                            if ("1".Equals(trade_mode))
                            {       //即时到账 
                                if ("0".Equals(trade_state))
                                {

                                    proc.PaidSuccess();

                                    return "Success";

                                }
                                else
                                {
                                    proc.PaidFail();
                                    //SetLogMessage("即时到账支付失败");
                                    return "即时到账支付失败";

                                }
                            }
                            else if ("2".Equals(trade_mode))
                            { //中介担保
                                //------------------------------
                                //中介担保处理业务开始
                                //------------------------------
                                //处理数据库逻辑
                                //注意交易单不要重复处理
                                //注意判断返回金额

                                int iStatus = 0;
                                switch (iStatus)
                                {
                                    case 0:		//付款成功

                                        break;
                                    case 1:		//交易创建

                                        break;
                                    case 2:		//收获地址填写完毕

                                        break;
                                    case 4:		//卖家发货成功

                                        break;
                                    case 5:		//买家收货确认，交易成功

                                        break;
                                    case 6:		//交易关闭，未完成超时关闭

                                        break;
                                    case 7:		//修改交易价格成功

                                        break;
                                    case 8:		//买家发起退款

                                        break;
                                    case 9:		//退款成功

                                        break;
                                    case 10:	//退款关闭

                                        break;

                                }


                                //------------------------------
                                //中介担保处理业务开始
                                //------------------------------


                                //给财付通系统发送成功信息，财付通系统收到此结果后不再进行后续通知

                                proc.PaidSuccess();
                                return "Success";

                            }
                        }
                        else
                        {
                            //错误时，返回结果可能没有签名，写日志trade_state、retcode、retmsg看失败详情。
                            //通知财付通处理失败，需要重新通知\



                            // SetLogMessage("查询验证签名失败或id验证失败\r\nretcode:" + queryRes.getParameter("retcode"));

                            proc.PaidFail();
                            return "查询验证签名失败或id验证失败retcode:" + queryRes.getParameter("retcode"); ;

                        }

                    }
                    else
                    {
                        //通知财付通处理失败，需要重新通知

                        //SetLogMessage("后台调用通信失败\r\ncall err:" + httpClient.getErrInfo() + "<br>" + httpClient.getResponseCode() + "<br>");

                        proc.PaidFail();

                        return "后台调用通信失败call err:" + httpClient.getErrInfo() + "<br>" + httpClient.getResponseCode() + "<br>";

                    }


                }
                else
                {
                    proc.PaidFail();
                    return "通知签名验证失败";
                }
            }
            return "";
        }
    }

}
