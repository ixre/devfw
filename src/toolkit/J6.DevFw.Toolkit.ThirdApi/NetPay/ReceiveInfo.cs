/*******************************************
* 文 件 名: ReceiveInfo
* 作    者: 刘成文
* 创建时间: 2013-4-28 10:23:56
* 修改说明：
********************************************/

namespace JR.DevFw.Toolkit.ThirdApi.NetPay
{
    /// <summary>
    /// 收货信息
    /// </summary>
    public struct ReceiveInfo
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
    }
}
