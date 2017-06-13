
//
// Copyright (C) 2007-2012 OPSoft INC,All rights reseved.
// 
// Project: OPS.Plugin
// FileName : PayHandler.cs
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
    using System.Web;

    /// <summary>
    /// 支付处理类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class PaymentHandler<T> where T : class
    {
        
        /// <summary>
        /// 提交支付请求(收货信息仅支付宝担保时传递)
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="pt"></param>
        /// <param name="partner_id"></param>
        /// <param name="partner_secret"></param>
        /// <param name="seller_account"></param>
        /// <param name="orderNo"></param>
        /// <param name="order_fee"></param>
        /// <param name="order_exp_fee"></param>
        /// <param name="order_desc"></param>
        /// <param name="order_show_url"></param>
        /// <param name="receive_name"></param>
        /// <param name="receive_address"></param>
        /// <param name="receive_zip"></param>
        /// <param name="receive_phone"></param>
        /// <param name="receive_mobile"></param>
        /// <param name="return_url"></param>
        /// <param name="notify_url"></param>
        /// <returns></returns>
        public string SubmitRequest(
            PayMethods pm,
            PayApiType pt,
            string partner_id,
            string partner_secret,
            string seller_account,
            BankSign bank, 
            string orderNo,
            float order_fee,
            float order_exp_fee,
            string order_desc,
            string order_show_url,
            string subject,
            string receive_name,
            string receive_address,
            string receive_zip,
            string receive_phone,
            string receive_mobile,
            string return_url,
            string notify_url
            )
        {
            Hashtable ht = new Hashtable();

            // OrderInfo order;

            // order = new Order(orderNo);


            // if (order == null)
            // {
            //    p.PayUtil.SetLogMessage("订单不存在！");
            //     return Result((int)p.PaidHandleResult.Fail);
            // }


            // orderPM = this.LoadOrderPM(order);
            if (String.IsNullOrEmpty(return_url))
            {
                return_url = PayUtil.GetReturnUrl(pm, pt);
            }
            if (String.IsNullOrEmpty(notify_url))
            {
                notify_url = PayUtil.GetNotifyUrl(pm, pt);
            }

            ht.Add("order_subject",subject??"支付订单"+orderNo);
            ht.Add("order_no", orderNo);                                             //订单编号
            ht.Add("order_fee", order_fee);
            ht.Add("order_exp_fee", order_exp_fee);                                 //订单快递金额
            try{
            ht.Add("usr_host", HttpContext.Current.Request.UserHostAddress);        //用户IP地址
            }catch{}
            ht.Add("order_showurl", order_show_url);                                //订单显示地址
            ht.Add("return_url", return_url);                                       //返回地址
            ht.Add("notify_url", notify_url);                                       //通知地址
            ht.Add("order_desc", order_desc);                                       //订单描述
            ht.Add("seller_account", seller_account);                               //帐号

            //收货信息
            ht.Add("receive_name", receive_name);
            ht.Add("receive_address", receive_address);
            ht.Add("receive_mobile", receive_mobile);
            ht.Add("receive_zip", receive_zip);
            ht.Add("receive_phone", receive_phone);
            

            if(bank!=BankSign.Default)
            {
            	ht.Add("bank",bank.ToString());
            }
            

            if (pm != PayMethods.Unknown)
            {

                string payHtml = String.Empty;

                switch (pm)
                {
                    //财付通
                    case PayMethods.Tenpay:

                        ht.Add("tenpay_key", partner_id);
                        ht.Add("tenpay_secret", partner_secret);
                        ht.Add("trade_mode", -((int)pt-4));  //1:即时到帐,2:中介担保,3:自己选择

                        payHtml = String.Format("<script>location.replace('{0}')</script>",
                            PayUtil.GetGatewayStr(PayMethods.Tenpay, pt, ht));

                        break;

                    //支付宝
                    case PayMethods.Alipay:
                        ht.Add("alipay_key", partner_id);
                        ht.Add("alipay_secret", partner_secret);

                        payHtml = PayUtil.GetGatewayStr(PayMethods.Alipay, pt, ht);

                        break;

                    //在线支付
                    default:
                    case PayMethods.OnlinePay:
                        //
                        //UNDONE:
                        //
                        throw new NotImplementedException("暂不支持银行支付!");
                }


                //准备支付
                // orderPM.RepairPay();


                //跳转到支付页面
                return payHtml;
            }

            else
            {

               return "不支持的支付方式!";
            }

        }


        /// <summary>
        /// 页面回调
        /// </summary>
        /// <param name="pmid"></param>
        /// <param name="ptid">接口类型ID</param>
        /// <returns></returns>
        public PaidHandleResult Return(PayMethods pm, PayApiType pt, PayMointor<T> proc)
        {
            PaidHandleResult result;
            //PayMointor<string> proc = new PayMointor<string>(String.Empty);
            //proc.OnInit += orderNo =>
            //{
            //    proc.Instance=
            //};

            this.AttachEvent(proc);

           // PayMethods pm = (PayMethods)pmid;                //支付方式
            //PayApiType pt = (PayApiType)ptid;                //接口类型

            result = PayUtil.PayReturn(pm, pt, proc);
            if(result== PaidHandleResult.Success)
            {
            	proc.PaidSuccess();
            }
            else if(result== PaidHandleResult.Fail)
            {
            	proc.PaidFail();
            }
            return result;
        }

        /// <summary>
        /// 主动通知
        /// </summary>
        /// <param name="ppid"></param>
        /// <returns></returns>
        public string Notify(PayMethods pm, PayApiType pt, PayMointor<T> proc)
        {

            // PaidMointor<T> proc = new PaidMointor<T>(null);

            // proc.OnInit += orderNo =>
            // {
            // var itm = this.LoadOrderPM(orderNo.ToString());
            //proc.Instance = itm.Instance;
            // this.AttachEvent(proc);
            // };

            this.AttachEvent(proc);
            
            return PayUtil.PayNotify(pm, pt, proc);
        }


        private void AttachEvent(PayMointor<T> proc)
        {
            if (proc == null)
            {
                throw new ArgumentNullException("参数proc不能为空!");
            }
            else
            {
                //添加事件
                proc.OnFailed += OnOrderPaidFail;
                proc.OnSuccessed += OnOrderPaidSuccess;
                proc.OnRepairPay += OnOrderRepairPay;
            }
        }

        /// <summary>
        /// 支付失败时操作
        /// </summary>
        /// <param name="orderNo"></param>
        public abstract void OnOrderPaidFail(T t);

        /// <summary>
        /// 支付成功时操作
        /// </summary>
        /// <param name="orderNo"></param>
        public abstract void OnOrderPaidSuccess(T t);

        /// <summary>
        /// 准备支付时操作
        /// </summary>
        /// <param name="orderNo"></param>
        private void OnOrderRepairPay(T t)
        {

        }
    }
}
