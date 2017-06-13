
//
// Copyright (C) 2007-2012 OPSoft INC,All rights reseved.
// 
// Project: OPS.Plugin
// FileName : PaidHandler.cs
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
    /// 支付时处理方法
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="t"></param>
    public delegate void PaidHandler<T>(T t) where T : class;

    public delegate void PaidAsyncHandler<T>(T t,PaymentAsyncState state) where T : class;

    /// <summary>
    /// 支付同步状态
    /// </summary>
    public enum PaymentAsyncState
    {
        WAIT_BUYER_PAY,
        WAIT_SELLER_SEND_GOODS
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class PayMointor<T> where T : class
    {
        /// <summary>
        /// 当初始化时发生,通常用于根据订单号获取订单
        /// </summary>
        public event PaidHandler<string> OnInit;

        /// <summary>
        /// 支付成功
        /// </summary>
        public event PaidHandler<T> OnSuccessed;

        /// <summary>
        /// 同步状态
        /// </summary>
        public event PaidAsyncHandler<T> OnAsync;

        /// <summary>
        /// 准备支付
        /// </summary>
        public event PaidHandler<T> OnRepairPay;

        /// <summary>
        /// 当支付失败时
        /// </summary>
        public event PaidHandler<T> OnFailed;

        private T instance;

        public T Instance { get { return instance; } set { instance = value; } }

        public PayMointor(T t)
        {
            this.instance = t;
        }

        /// <summary>
        /// 支付成功
        /// </summary>
        public void PaidSuccess()
        {
            if (this.OnSuccessed != null)
            {
                this.OnSuccessed(this.instance);
            }
        }

        /// <summary>
        /// 支付失败
        /// </summary>
        public void PaidFail()
        {
            if (this.OnFailed != null)
            {
                this.OnFailed(this.instance);
            }
        }

        /// <summary>
        /// 准备支付
        /// </summary>
        public void RepairPay()
        {
            if (this.OnRepairPay != null)
            {
                this.OnRepairPay(this.instance);
            }
        }

        /// <summary>
        /// 同步状态
        /// </summary>
        /// <param name="state"></param>
        public void AsyncState(PaymentAsyncState state)
        {
            if (OnAsync != null)
            {
                OnAsync(this.instance,state);
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="orderNo"></param>
        public void Init(string orderNo)
        {
            if (this.OnInit != null)
            {
                this.OnInit(orderNo);
            }
        }

    }

}
