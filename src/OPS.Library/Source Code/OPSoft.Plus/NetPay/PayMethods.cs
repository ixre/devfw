
//
// Copyright (C) 2007-2012 OPSoft INC,All rights reseved.
// 
// Project: OPS.Plugin
// FileName : PayMethods.cs
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
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// 支付方式
    /// </summary>
    public enum PayMethods
    {

        /// <summary>
        /// 未指定
        /// </summary>
        Unknown = -1,

        /// <summary>
        /// 支付宝
        /// </summary>
        Alipay = 1,

        /// <summary>
        /// 腾讯财付通
        /// </summary>
        Tenpay = 2,


        /// <summary>
        /// 银联支付
        /// </summary>
        ChinaPay = 3,

        /// <summary>
        /// 在线支付
        /// </summary>
        OnlinePay = 4
    }
}
