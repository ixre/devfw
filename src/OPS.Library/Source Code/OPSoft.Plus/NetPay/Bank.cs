using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace Ops.Plugin.NetPay
{
    public class BankItem
    {
        public BankItem(BankSign sign,string name,string netway)
        {
            this.Sign = sign;
            this.HansName = name;
            this.NetGateway = netway;
        }

        /// <summary>
        /// 标识
        /// </summary>
        public BankSign Sign { get; set; }

        /// <summary>
        /// 汉语名称
        /// </summary>
        public string HansName { get; set; }

        /// <summary>
        /// 网关
        /// </summary>
        public string NetGateway { get; set; }
    }

    public enum BankSign
    {
        Default,ICBC, ABC, CCB, BOC, CIB, CITIC, CMB, CMBC, CEB, COMM, GDB, SPAB, POSTGC, SPDB
    }


    /// <summary>
    /// 银行
    /// </summary>
    public class Bank
    {
        public static BankItem[] Alipay;
        public static BankItem[] Tenpay;

        static Bank()
        {
            Alipay = new BankItem[]{
               new BankItem(BankSign.ICBC,"中国工商银行","ICBCBTB"),
               new BankItem(BankSign.ABC,"中国农业银行","ABCBTB"),
               new BankItem(BankSign.CCB,"中国建设银行","CCBBTB"),
               new BankItem(BankSign.BOC,"中国银行","BOCB2C"),
               new BankItem(BankSign.CIB,"兴业银行","CIB"),
               new BankItem(BankSign.CITIC,"中信银行","CITIC"),
               new BankItem(BankSign.CMB,"招商银行","CMB"),
               new BankItem(BankSign.CMBC,"中国民生银行","CMBC"),
               new BankItem(BankSign.CEB,"中国光大银行","CEBBANK"),
               new BankItem(BankSign.COMM,"交通银行","COMM"),
               new BankItem(BankSign.GDB,"广发银行","GDB"),
               new BankItem(BankSign.SPAB,"平安银行","SPABANK"),
               new BankItem(BankSign.POSTGC,"中国邮政储蓄银行","POSTGC"),
               new BankItem(BankSign.SPDB,"浦发银行","SPDBB2B")
            };

            Tenpay = new BankItem[]{
               new BankItem(BankSign.ICBC,"中国工商银行","ICBC"),
               new BankItem(BankSign.ABC,"中国农业银行","ABC"),
               new BankItem(BankSign.CCB,"中国建设银行","CCB"),
               new BankItem(BankSign.BOC,"中国银行","BOC"),
               new BankItem(BankSign.CIB,"兴业银行","CIB"),
               new BankItem(BankSign.CITIC,"中信银行","CITIC"),
               new BankItem(BankSign.CMB,"招商银行","CMB"),
               new BankItem(BankSign.CMBC,"中国民生银行","CMBC"),
               new BankItem(BankSign.CEB,"中国光大银行","CEB"),
               new BankItem(BankSign.COMM,"交通银行","COMM"),
               new BankItem(BankSign.GDB,"广发银行","GDB"),
               new BankItem(BankSign.SPAB,"平安银行","PAB"),
               new BankItem(BankSign.POSTGC,"中国邮政储蓄银行","POSTGC"),
               new BankItem(BankSign.SPDB,"浦发银行","SPDB")
            };

        }

        /// <summary>
        /// 获取银行图标
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static byte[] ReadBankImage(string id)
        {

            System.Drawing.Bitmap img=null;
            BankSign sign =(BankSign)Enum.Parse(typeof(BankSign), id, true);
            switch (sign)
            {
                case BankSign.ICBC: img = BankRes.ICBC; break;
                case BankSign.ABC: img = BankRes.ABC; break;
                case BankSign.CCB: img = BankRes.CCB; break;
                case BankSign.BOC: img = BankRes.BOC; break;
                case BankSign.CIB: img = BankRes.CIB; break;
                case BankSign.CITIC: img = BankRes.CITIC; break;
                case BankSign.CMB: img = BankRes.CMB; break;
                case BankSign.CMBC: img = BankRes.CMBC; break;
                case BankSign.CEB: img = BankRes.CEB; break;
                case BankSign.COMM: img = BankRes.COMM; break;
                case BankSign.GDB: img = BankRes.GDB; break;
                case BankSign.SPAB: img = BankRes.SPAB; break;
                case BankSign.POSTGC: img = BankRes.POSTGC; break;
                case BankSign.SPDB: img = BankRes.SPDB; break;
            }
            if (img == null)
            {
                return new byte[0];
            }

            MemoryStream ms = new MemoryStream();
            img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);

            byte[] buffer = ms.ToArray();
            ms.Dispose();

            return buffer;
        }
    }
}
