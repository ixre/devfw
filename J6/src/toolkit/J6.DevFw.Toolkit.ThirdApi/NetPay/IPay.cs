
//
// Copyright (C) 2007-2012 S1N1.COM,All rights reseved.
// 
// Project: OPS.Plugin
// FileName : IPay.cs
// Author : PC-CWLIU (new.min@msn.com)
// Create : 2013/05/01 17:58:32
// Description :
//
// Get infromation of this software,please visit our site http://www.ops.cc
//
//

using System.Collections;

namespace J6.DevFw.Toolkit.ThirdApi.NetPay
{
    public interface IPay
    {
        /// <summary>
        /// 获取支付请求,可返回链接或表单
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        string GetPayRequest(Hashtable hash);

        /// <summary>
        /// 返回信息
        /// </summary>
        /// <param name="proc"></param>
        /// <returns></returns>
        PaidHandleResult Return<T>(PayMointor<T> proc) where T : class;

        /// <summary>
        /// 主动通知
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="proc"></param>
        /// <returns></returns>
        string Notify<T>(PayMointor<T> proc) where T : class;
    }
}
