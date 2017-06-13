
//
// Copyright (C) 2007-2012 OPSoft INC,All rights reseved.
// 
// Project: OPS.Plugin
// FileName : PaidHandleResult.cs
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
    /// 支付处理结果
    /// </summary>
    public enum PaidHandleResult
    {

        /// <summary>
        /// 支付成功
        /// </summary>
        Success,

        /// <summary>
        /// 支付失败
        /// </summary>
        Fail,
        /// <summary>
        /// 准备支付
        /// </summary>
        Repair
    }
}
