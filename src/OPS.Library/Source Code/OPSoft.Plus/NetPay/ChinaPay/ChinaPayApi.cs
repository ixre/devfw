using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Web;
using Ops.Framework.Extensions;

namespace Ops.Plugin.NetPay
{
    public class ChinaPayApi:IPay
    {

        public string GetPayRequest(Hashtable hash)
        {

            string[] keys = (hash["chinapay_secret"] as string).Split('|');

            ChinaPaySignData.MerchantID = hash["chinapay_key"] as string;
            ChinaPaySignData.PrivateKeyPath =String.Format("{0}{1}",AppDomain.CurrentDomain.BaseDirectory, keys[0]);
            ChinaPaySignData.PublicKeyPath =String.Format("{0}{1}",AppDomain.CurrentDomain.BaseDirectory, keys[1]);

             //test url   //http://localhost:8080/pay/notify_3_1.html?merid=808080201301103&orderno=2013052309145966&transdate=20130523&amount=000000000001&currencycode=156&transtype=0001&status=1001&checkvalue=872E476C3C0B471CF6C408F2B311E8A89FB78D5166676B60FB06F45797F24FFAD092FDE5CE85BA084CCDEFF4097D31DA68DDCCB5A59B719E0EB1718EB2CC77FA72F0F0039AE03684CC59EE6204A2EC104B792F33321D4EC66D7A90F9C3BD7531A50FA47199FE084D4715DEC312117C8E62A673947B293472BD8B5685C67106B8&GateId=8607&Priv1=beizhu
            const string gatewayHTML = @"<!--银联支付 --><div style='display:none'>
<form id='myform'  action='https://payment.chinapay.com/pay/TransGet'   method='post' >
    <table>
        <tr>
            <td>商户号:</td>
            <td><input type='text' name='MerId' id='MerId' value='%1%' /></td>
        </tr>
         <tr>
            <td>支付版本号:</td>
            <td><input type='text' name='Version'  id='Version' value='%2%'  /></td>
        </tr>
        <tr>
            <td>订单号:</td>
            <td><input type='text' name='OrdId'  id='OrdId'  value='%3%' /></td>
        </tr>
        <tr>
            <td>订单金额:</td>
            <td><input type='text' name='TransAmt'  id='TransAmt' value='%4%' /></td>
        </tr>
        <tr>
            <td>货币代码:</td>
            <td><input type='text' name='CuryId' id='CuryId' value='%5%'  /></td>
        </tr>
        <tr>
            <td>订单日期:</td>
            <td><input type='text' name='TransDate'  id='TransDate' value='%6%'  /></td>
        </tr>
        <tr>
            <td>交易类型:</td>
            <td><input type='text' name='TransType'  id='TransType' value='%7%' /></td>
        </tr>
        <tr>
            <td>后台返回地址:</td>
            <td><input type='text' name='BgRetUrl' id='BgRetUrl' value='%8%'  /></td>
        </tr><tr>
            <td>页面返回地址:</td>
            <td><input type='text' name='PageRetUrl' id='PageRetUrl' value='%9%' /></td>
        </tr>
        <tr>
            <td>网关号:</td>
            <td><input type='text' name='GateId' id='GateId' value='%10%' /></td>
        </tr>
        <tr>
            <td>备注:</td>
            <td><input type='text' name='Priv1' id='Priv1' value='%11%' /></td>
        </tr>
        <tr>
            <td>签名:</td>
            <td><input type='text' name='ChkValue' id='ChkValue' value='%12%' /></td>
        </tr>
        
       
    </table>
     <script type='text/javascript'>
        document.getElementById('myform').submit();
    </script>
   </form></div>";


            decimal product_fee = (decimal)hash["order_fee"];                            //商品费用
            decimal transport_fee = decimal.Parse(hash["order_exp_fee"].ToString());     //物流费用

            string MerId = ChinaPaySignData.MerchantID;//商户号
            string OrdId = hash["order_no"] as string;//订单号
            string TransAmt = String.Format("{0:D12}",(int)((product_fee + transport_fee)*100));//订单金额
            string CuryId = "156";//货币代码 (人民币)
            string TransDate = DateTime.Now.ToString("yyyyMMdd");//订单日期
            string TransType = "0001";                           //交易类型
            string Priv1 = hash["order_desc"] as string;//备注


            //准备签名的数据
            string plain = MerId + OrdId + TransAmt + CuryId + TransDate + TransType + Priv1;

            string ChkValue = null;
            //Response.Write("MerId:" + MerId);
            ChkValue = ChinaPaySignData.sign(MerId, plain);
            //Response.Write("ChkValue:" + ChkValue);

            //签名长度256位
            if (ChkValue == null || ChkValue.Length != 256)
            {
                return "签名不正确,无法支付!";
            }

            return gatewayHTML.Template(
                MerId,
                "20070129",
                OrdId,
                TransAmt,
                CuryId,
                TransDate,
                TransType,
                PayUtil.GetNotifyUrl(PayMethods.ChinaPay, PayApiType.Normal),
                PayUtil.GetReturnUrl(PayMethods.ChinaPay, PayApiType.Normal),
                "", //网关留空,为银联支付
                Priv1,
                ChkValue);
        }

        public PaidHandleResult Return<T>(PayMointor<T> proc) where T:class
        {
            var request = HttpContext.Current.Request;

            string MerId = request["MerId"];//商户号
            string OrdId = request["OrderNo"];//订单号
            string TransAmt = request["Amount"];//订单金额
            string CuryId = request["CurrencyCode"];//货币代码
            string TransDate = request["TransDate"];//订单日期
            string TransType = request["TransType"];//交易类型
            string Priv1 = request["Priv1"];//备注
            string GateId = request["GateId"];//网关
            string status = request["status"];

            proc.Init(OrdId);

            string CheckValue = request["checkvalue"];//签名数据   
            bool res = ChinaPaySignData.check(MerId, OrdId, TransAmt, CuryId, TransDate, TransType, status, CheckValue);
            if (res)
            {
                return PaidHandleResult.Fail;
            }
            else
            {
                return PaidHandleResult.Success;
            }

        }

        public string Notify<T>(PayMointor<T> proc) where T : class
        {

            var request = HttpContext.Current.Request;

            string MerId = request["MerId"];//商户号
            string OrdId = request["OrderNo"];//订单号
            string TransAmt = request["Amount"];//订单金额
            string CuryId = request["CurrencyCode"];//货币代码
            string TransDate = request["TransDate"];//订单日期
            string TransType = request["TransType"];//交易类型
            string Priv1 = request["Priv1"];//备注
            string GateId = request["GateId"];//网关
            string status = request["status"];

            proc.Init(OrdId);

            string CheckValue = request["checkvalue"];//签名数据   
            bool res = ChinaPaySignData.check(MerId, OrdId, TransAmt, CuryId, TransDate, TransType, status, CheckValue);
            if (res)
            {
                proc.PaidFail();
                return "0";
            }
            else
            {
                proc.PaidSuccess();
                return "1";

            }
        }
    }
}
