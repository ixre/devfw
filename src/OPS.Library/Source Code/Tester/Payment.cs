/*
 * Created by SharpDevelop.
 * User: newmin
 * Date: 2013/12/12
 * Time: 21:23
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Ops.Plugin.NetPay;
using Ops;
using System.Collections;

namespace Test
{
	/// <summary>
	/// Description of Paymetn.
	/// </summary>
	public class Payment:PaymentHandler<string>
	{
		private static string apiUser,apiSecret,apiAccount;
		
		static Payment()
		{
			apiAccount="clientinfo@roadchina.com.cn";
			apiUser="2088201937033268";
			apiSecret="htdf7tjk1c5cdu6qi7rigeqpbanisrbt";
		}
		
		public string GetPaymentSubmit(bool isMobile,string orderNo)
		{
			
			return base.SubmitRequest(PayMethods.Alipay,
			                          isMobile?PayApiType.Mobile:PayApiType.Direct,
			                          apiUser,
			                          apiSecret,
			                          apiAccount,
			                          BankSign.Default,
			                          orderNo,
			                          30,
			                          0f,
			                          String.Format("订单{0}",orderNo),
			                          "",
			                          "春运保障产品",
			                          "",
			                          "",
			                          "",
			                          "",
			                          "",
			                         "http://127.0.0.1/return",
			                          "http://127.0.0.1/notify"
			                         );
		}
		
		public string GetReturn()
		{
			return "";
		}
		
		public string GetNotify()
		{
			return "";
		}
		
		public override void OnOrderPaidFail(string t)
		{
		}
		
		public override void OnOrderPaidSuccess(string order)
		{
			
		}
		
	}
}
