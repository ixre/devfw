using System.Collections.Generic;

namespace AtNet.DevFw.Toolkit.ThirdApi.ETao
{

    /// <summary>
    /// 商品项
    /// </summary>
    public class Item
    {
        /// <summary>
        /// 给合作商家创建的淘宝会员账号
        /// </summary>
        public string seller_id { get { return Config.Seller; } }

        /// <summary>
        /// 商家自定义的商品
        /// </summary>
        public string outer_id { get; set; }

        /// <summary>
        /// 商品标题，不超过60个字节-
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 商品类型，一口价（fixed 默认）、团购（group）
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 商品是否有货，默认为0，表示无货，1标示有货，必填
        /// </summary>
        public string available { get; set; }

        /// <summary>
        /// 商品价格，格式：5.00；单位：元；精确到：分；取值范围：0-100000000
        /// </summary>
        public string price { get; set; }

        /// <summary>
        /// 优惠信息
        /// </summary>
        public IDictionary<string, object> discount { get; set; }

        /// <summary>
        /// 商品简描述, 不超过1000个字节
        /// </summary>
        public string desc { get; set; }
        
	    /// <summary>
        /// 商品品牌，不超过30个字符
	    /// </summary>
        public string brand { get; set; }

        /// <summary>
        /// 商品Tag标签，有助于搜索，不超过5个标签(如：阿迪达斯\Adidas)
        /// </summary>
        public string tags { get; set; }

        /// <summary>
        /// 商品图片的地址，类型：jpg、jpeg、png，不支持gif；最大：500k
        /// </summary>
        public string image { get; set; }

        /// <summary>
        /// 商品更多辅助图片的地址，类型：jpg、jpeg、png，不支持gif；最大：500k
        /// </summary>
        public IDictionary<string,object> more_images { get; set; }

        /// <summary>
        /// 商户自定义类目ID，一个商品可以属于多个类目，半角逗号分隔
        /// </summary>
        public string scids { get; set; }

        /// <summary>
        /// 邮费价格，精确到2位小数；单位：元；卖家承担运费邮费即为0
        /// </summary>
        public float post_fee { get; set; }

        /// <summary>
        /// SKU属性项、属性值，不同属性项间以分号分隔，不同属性值间以逗号分隔
        /// </summary>
        public string props { get; set; }

        /// <summary>
        /// 不填默认不推荐，1/0
        /// </summary>
        public string showcase { get; set; }

        /// <summary>
        /// 商品链接绝对地址
        /// </summary>
        public string href { get; set; }
    }
}
