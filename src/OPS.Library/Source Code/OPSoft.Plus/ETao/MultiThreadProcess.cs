using System;
using System.Collections.Generic;
using System.Threading;

/*******************************************
* 文 件 名：MultiThreadProcess.cs
* 文件说明：
* 创 建 人：刘成文
* 创建日期：2012-11-9 13:56:56
********************************************/

namespace Ops.Plugin.ETao
{

    /// <summary>
    /// 多线程处理事件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="t"></param>
    public delegate void ThreadProcessHandler<T>(T t);

    /// <summary>
    /// 多线程处理
    /// </summary>
    public class MultiThreadProcess
    {
        /// <summary>
        /// 线程数量
        /// </summary>
        private int threadCount;

        /// <summary>
        ///  线程堆栈，用于轮询线程
        /// </summary>
        private Stack<int> threadStack;

        /// <summary>
        /// 线程锁
        /// </summary>
        private object threadLocker = new object();

        /// <summary>
        /// 线程集合
        /// </summary>
        private Thread[] threads;

        private bool isAlive=true;

        /// <summary>
        /// 是否活动中
        /// </summary>
        public bool IsAlive { get { return isAlive; } }

        public MultiThreadProcess(int threads,int processTimes)
        {
            this.threadCount = threads;
            this.threads = new Thread[threads];

            //初始化线程堆栈
            this.threadStack = new Stack<int>(processTimes);
            for (int i = processTimes; i > 0; i--)
            {
                this.threadStack.Push(i);
            }
        }

        public void Start<T>(ThreadProcessHandler<T> handler, T t)
        {
            while (this.isAlive)
            {
                for (int i = 0; i < threadCount && this.threadStack.Count > 0; i++)
                {
                    if (threads[i] == null || threads[i].ThreadState == ThreadState.Stopped)
                    {
                        threads[i] = new Thread(() =>
                        {
                            lock (this.threadStack)
                            {
                                if (this.threadStack.Count > 0)
                                {
                                    this.threadStack.Pop();
                                    handler(t);
                                }
                            }
                        });

                        threads[i].Name = String.Format("thread{0}", i.ToString());
                        threads[i].Start();
                    }
                }

                if (this.threadStack.Count == 0)
                {
                    //终止线程
                    //for (int i = 0; i < threadCount; i++)
                    //{
                    //    if (this.threads[i] != null)
                    //    {
                    //        this.threads[i].Abort();
                    //    }
                    //}
                    do
                    {
                        bool hasThreadRunning = false;
                        for (int i = 0; i < threadCount; i++)
                        {
                            if (this.threads[i] != null && this.threads[i].ThreadState == ThreadState.Running)
                                hasThreadRunning = true;
                        }
                        if (!hasThreadRunning)
                        {
                            this.isAlive = false;
                        }

                    } while (this.isAlive);

                    //设置线程任务完成
                    this.isAlive = false;
                }
            }
        }

    }
}
