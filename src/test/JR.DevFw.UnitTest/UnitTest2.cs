using System;
using JR.DevFw.Data;
using JR.DevFw.Data.Extensions;
using JR.DevFw.Data.Orm;
using JR.DevFw.Data.Orm.Mapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JR.DevFw.Framework;
using System.Data;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        private static String orderNo = "YH-00011";

        [TestMethod]
        public void TestInsert()
        {
            var db = this.GetConn();
            IEntityManager<OrdOrderEntity> im = new EntityManager<OrdOrderEntity>(db);
            OrdOrderEntity ord = new OrdOrderEntity
            {
                AssignId = 1,
                AdjustAmount = 100,
                BuyerId = 1,
                DeductAmount = 0,
                FinalFee = 100,
                IsPaid = 0,
                ID = 0,
                OrderKind = 1,
                OrderNo = orderNo,
                PaidTime = 0,
                Subject = "商标订单"
            };
            im.Insert(ord);
        }

        private DataBaseAccess GetConn()
        {
            string connString = "server=cn-s1.ns.to2.net;database=yh01;uid=yh01;pwd=zyh123456;port=3316;SslMode = none;";
            DataBaseAccess db = new DataBaseAccess(DataBaseType.MySQL, connString);
            return db;
        }

        [TestMethod]
        public void TestUpdate()
        {
            var db = this.GetConn();
            OrdOrderEntity e = null;
            db.ExecuteReader("SELECT * FROM ord_order where order_no=@orderNo", rd =>
                {
                    e = rd.ToEntity<OrdOrderEntity>();
                },
                new[] {db.CreateParameter("@orderNo", orderNo)});
            if (e != null)
            {
                Console.WriteLine(JsonSerializer.SerializerObject(e));
            }

        }
    }

    /// <summary>
    /// 
    /// </summary>
    [DataTable("ord_order")]
    public class OrdOrderEntity
    {


        /// <summary>
        /// 
        /// </summary>
        [Column("id", IsPrimaryKey = true, AutoGeneried = true)]
        [Alias("id")]
        public int ID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Alias("order_no")]
        public string OrderNo { get; set; }
        /// <summary>
        /// 订单类型,1:普通，2:专利，3：资质，4：商标，5：注册公司
        /// </summary>
        [Alias("order_kind")]
        public int OrderKind { get; set; }
        /// <summary>
        /// 订单标题,格式：公司-业务
        /// </summary>
        [Alias("subject")]
        public string Subject { get; set; }
        /// <summary>
        /// 卖家(业务员编号)
        /// </summary>
        [Alias("seller_id")]
        public int SellerId { get; set; }
        /// <summary>
        /// 买家(公司编号)
        /// </summary>
        [Alias("buyer_id")]
        public int BuyerId { get; set; }
        /// <summary>
        /// 分配用户编号
        /// </summary>
        [Alias("assign_Id")]
        public int AssignId { get; set; }
        /// <summary>
        /// 总金额
        /// </summary>
        [Alias("total_amount")]
        public int TotalAmount { get; set; }
        /// <summary>
        /// 抵扣金额，分期付款已支付的金额
        /// </summary>
        [Alias("deduct_amount")]
        public int DeductAmount { get; set; }
        /// <summary>
        /// 调整金额
        /// </summary>
        [Alias("adjust_amount")]
        public int AdjustAmount { get; set; }


        /// <summary>
        /// 最终金额
        /// </summary>
        [Alias("final_fee")]
        public int FinalFee { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        [Alias("state")]
        public int State { get; set; }
        /// <summary>
        /// 是否支付
        /// </summary>
        [Alias("is_paid")]
        public int IsPaid { get; set; }
        /// <summary>
        /// 支付时间
        /// </summary>
        [Alias("paid_time")]
        public int PaidTime { get; set; }
        /// <summary>
        /// 提交时间
        /// </summary>
        [Alias("submit_time")]
        public int SubmitTime { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        [Alias("update_time")]
        public int UpdateTime { get; set; }
    }
}
