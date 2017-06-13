using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using Com.Alipay;
using Ops.Plugin.NetPay;

namespace Ops.Plugin.NetPay
{
    internal class AlipayComfireApi:IPay
    {
        public string GetPayFormHtml(Hashtable ht)
        {
            //商户号
            string partner = ht["alipay_key"] as string;
            //密钥
            string key = ht["alipay_secret"] as string;


            Config.Partner = partner;// +"5";
            Config.Key = key;


            decimal product_fee = (decimal)ht["order_fee"];                                             //商品费用
            decimal transport_fee = decimal.Parse(ht["order_exp_fee"].ToString());     //物流费用

            string order_desc = ht["order_desc"] as string;

            //订单号，此处用时间和随机数生成，商户根据自己调整，保证唯一
            string out_trade_no = ht["order_no"] as string;
            string host = ht["usr_host"] as string;

            string showUrl = ht["order_surl"] as string;
            
             string notify_url =ht.Contains("notify_url")?ht["notify_url"].ToString():PayUtil.GetNotifyUrl(PayMethods.Alipay, PayApiType.Guarantee);

            //需http://格式的完整路径，不能加?id=123这类自定义参数

            //页面跳转同步通知页面路径
            string return_url =ht.Contains("return_url")?ht["return_url"].ToString():PayUtil.GetReturnUrl(PayMethods.Alipay, PayApiType.Guarantee);
            //需http://格式的完整路径，不能加?id=123这类自定义参数，不能写成http://localhost/


            //卖家支付宝帐户
            string seller_email = ht["seller_account"] as string;

            //订单描述、订单详细、订单备注，显示在支付宝收银台里的“商品描述”里
            string subject =  ht["order_subject"].ToString(); //parma.desc ?? "支付订单";

            //当前时间 yyyyMMdd
            string date = DateTime.Now.ToString("yyyyMMdd");
            //todo
            string body = order_desc;

            //订单总金额，显示在支付宝收银台里的“应付总额”里
            string total_fee = Convert.ToString(product_fee + transport_fee);


            ReceiveInfo receive=default(ReceiveInfo);
            if (ht.ContainsKey("receive"))
            {
                receive =(ReceiveInfo) ht["receive"];
            }



            ////////////////////////////////////////////请求参数////////////////////////////////////////////

            //支付类型
            string payment_type = "1";

            //必填

            //商品数量
            string quantity = "1";
            //必填，建议默认为1，不改变值，把一次交易看成是一次下订单而非购买一件商品
            //物流费用
            string logistics_fee = "0.00";
            //必填，即运费
            //物流类型
            string logistics_type = "EXPRESS";
            //必填，三个值可选：EXPRESS（快递）、POST（平邮）、EMS（EMS）
            //物流支付方式
            string logistics_payment = "SELLER_PAY";
            //必填，两个值可选：SELLER_PAY（卖家承担运费）、BUYER_PAY（买家承担运费）
            //订单描述




            ////////////////////////////////////////////////////////////////////////////////////////////////

            //把请求参数打包成数组
            SortedDictionary<string, string> sParaTemp = new SortedDictionary<string, string>();
            sParaTemp.Add("partner", Config.Partner);
            sParaTemp.Add("_input_charset", Config.Input_charset.ToLower());
            sParaTemp.Add("service", "create_partner_trade_by_buyer");
            sParaTemp.Add("payment_type", payment_type);
            sParaTemp.Add("notify_url", notify_url);
            sParaTemp.Add("return_url", return_url);
            sParaTemp.Add("seller_email", seller_email);
            sParaTemp.Add("out_trade_no", out_trade_no);
            sParaTemp.Add("subject", subject);
            sParaTemp.Add("price", total_fee);
            sParaTemp.Add("quantity", quantity);
            sParaTemp.Add("logistics_fee", logistics_fee);
            sParaTemp.Add("logistics_type", logistics_type);
            sParaTemp.Add("logistics_payment", logistics_payment);
            sParaTemp.Add("body", body);
            sParaTemp.Add("show_url", showUrl);
            sParaTemp.Add("receive_name", receive.Name);
            sParaTemp.Add("receive_address", receive.Address);
            sParaTemp.Add("receive_zip", receive.Zip);
            sParaTemp.Add("receive_phone", receive.Phone);
            sParaTemp.Add("receive_mobile",receive.Mobile);


            //建立请求
           return Submit.BuildRequest(sParaTemp, "get", "确认");

        }


        public PaidHandleResult Return<T>(PayMointor<T> proc) where T : class
        {
            
            SortedDictionary<string, string> sPara = PayUtil.GetRequestGet();
            var request = HttpContext.Current.Request;
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

                    //商户订单号

                    string out_trade_no = request.QueryString["out_trade_no"];

                    //支付宝交易号

                    string trade_no = request.QueryString["trade_no"];

                    //交易状态
                    string trade_status = request.QueryString["trade_status"];

                    proc.Init(out_trade_no);


                    if (request.QueryString["trade_status"] == "WAIT_SELLER_SEND_GOODS")
                    {
                        //判断该笔订单是否在商户网站中已经做过处理
                        //如果没有做过处理，根据订单号（out_trade_no）在商户网站的订单系统中查到该笔订单的详细，并执行商户的业务程序
                        //如果有做过处理，不执行商户的业务程序

                        return PaidHandleResult.Success;
                    }
                    else
                    {
                        return PaidHandleResult.Fail;
                    }


                }
                else//验证失败
                {
                    return PaidHandleResult.Fail;
                }
            }
            else
            {
                return PaidHandleResult.Fail;
            }
        }

        public string AlipayNotify<T>(PayMointor<T> proc)where T:class
        {
            SortedDictionary<string, string> sPara =PayUtil.GetRequestPost();
            var request = HttpContext.Current.Request;

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

                    //商户订单号

                    string out_trade_no = request.Form["out_trade_no"];

                    //支付宝交易号

                    string trade_no = request.Form["trade_no"];

                    //交易状态
                    string trade_status = request.Form["trade_status"];


                    proc.Init(out_trade_no);


                    if (request.Form["trade_status"] == "WAIT_BUYER_PAY")
                    {
                        proc.AsyncState(PaymentAsyncState.WAIT_BUYER_PAY);
                    }
                    else if (request.Form["trade_status"] == "WAIT_SELLER_SEND_GOODS")
                    {

                        proc.PaidSuccess();
                        
                        //该判断示买家已在支付宝交易管理中产生了交易记录且付款成功，但卖家没有发货

                        //判断该笔订单是否在商户网站中已经做过处理
                        //如果没有做过处理，根据订单号（out_trade_no）在商户网站的订单系统中查到该笔订单的详细，并执行商户的业务程序
                        //如果有做过处理，不执行商户的业务程序

                    }
                    else if (request.Form["trade_status"] == "WAIT_BUYER_CONFIRM_GOODS")
                    {//该判断表示卖家已经发了货，但买家还没有做确认收货的操作

                        //判断该笔订单是否在商户网站中已经做过处理
                        //如果没有做过处理，根据订单号（out_trade_no）在商户网站的订单系统中查到该笔订单的详细，并执行商户的业务程序
                        //如果有做过处理，不执行商户的业务程序

                    }
                    else if (request.Form["trade_status"] == "TRADE_FINISHED")
                    {//该判断表示买家已经确认收货，这笔交易完成

                        //判断该笔订单是否在商户网站中已经做过处理
                        //如果没有做过处理，根据订单号（out_trade_no）在商户网站的订单系统中查到该笔订单的详细，并执行商户的业务程序
                        //如果有做过处理，不执行商户的业务程序

                    }
                    else
                    {
                    }

                    return "success";

                    //——请根据您的业务逻辑来编写程序（以上代码仅作参考）——

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
                return "无通知参数";
            }
        }


        public string GetPayRequest(Hashtable hash)
        {
            return this.GetPayFormHtml(hash);
        }


        public string Notify<T>(PayMointor<T> proc) where T : class
        {
            return this.AlipayNotify(proc);
        }
    }
}
