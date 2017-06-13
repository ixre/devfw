using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;


namespace Ops.Plugin.NetPay
{
    public class ChinaPaySignData
    {
       internal static string PrivateKeyPath = "";
        internal static string PublicKeyPath = "";
        internal static string MerchantID = "808080201301103";

        static ChinaPaySignData()
        {
            PrivateKeyPath = AppDomain.CurrentDomain.BaseDirectory + "uploads/resources/cpkey/prk.key";
            PublicKeyPath = AppDomain.CurrentDomain.BaseDirectory + "uploads/resources/cpkey/pk.key";
        }

        //签名
        public static string sign(string MerId, string plain)
        {
            global::NetPay netPay = new global::NetPay();
            Boolean flag = netPay.buildKey(MerId, 0, ChinaPaySignData.PrivateKeyPath);
            string sign = null;
            if (flag)
            {
                if (netPay.PrivateKeyFlag)
                {
                    sign = netPay.Sign(plain);
                }
            }
            return sign;

        }

        //验签
        public static bool check(string MerId, string OrdId, string TransAmt, string CuryId, string TransDate, string TransType, string status, string ChkValue)
        {

            global::NetPay netPay = new global::NetPay();
            Boolean flag = netPay.buildKey(ChinaPaySignData.MerchantID, 0, ChinaPaySignData.PublicKeyPath);
            if (flag)
            {
                if (netPay.PublicKeyFlag)
                {
                    flag = netPay.verifyTransResponse(MerId, OrdId, TransAmt, CuryId, TransDate, TransType, status, ChkValue);
                }
                else
                {
                    flag = false;
                }
            }
            else
            {
                flag = false;
            }

            return flag;

        }


        //得到交易日期
        public static string getTransDate()
        {
            return DateTime.Now.ToString("yyyyMMdd");
        }

        //得到订单号16位
        public static string getOrdId()
        {
            return DateTime.Now.ToString("yyyyMMHHmmffffff");
        }

    }
}
