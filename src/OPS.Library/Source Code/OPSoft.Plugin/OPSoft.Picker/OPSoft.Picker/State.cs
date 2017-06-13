//
//
//  Copyright 2011 @ OPSoft Inc.all rights reseved.
//
//  Project : Untitled
//  File Name : State.cs
//  Date : 2011/8/25
//  Author : 
//
//


namespace OPSoft.Plugin.NetCrawl
{
    /// <summary>
    /// 采集状态
    /// </summary>
    public class State
    {
        /// <summary>
        /// 总数
        /// </summary>
        public int TotalCount { get; internal set; }

        /// <summary>
        /// 失败数
        /// </summary>
        public int FailCount { get; internal set; }

        /// <summary>
        /// 成功数
        /// </summary>
        public int SuccessCount { get; internal set; }
    }
}