
//
// Copyright (C) 2007-2012 S1N1.COM,All rights reseved.
// 
// Project: OPS.Plugin
// FileName : PayApiType.cs
// Author : PC-CWLIU (new.min@msn.com)
// Create : 2013/05/01 17:58:32
// Description :
//
// Get infromation of this software,please visit our site http://www.ops.cc
//
//

namespace JR.DevFw.Toolkit.ThirdApi.NetPay
{
    /// <summary>
    /// 支付接口
    /// </summary>
    public enum PayApiType
    {
        /// <summary>
        /// 标准双接口
        /// </summary>
        Normal = 1,

        /// <summary>
        /// 担保交易
        /// </summary>
        Guarantee = 2,

        /// <summary>
        /// 即时到帐
        /// </summary>
        Direct = 3,

        /// <summary>
        /// 手机网站支付
        /// </summary>
        Mobile=4
    }
}
