//
//
//  Copyright 2011 (C) S1N1.COM,All rights reseved.
//  -----------------------------
//  Project : OPSoft.Plugin.NetCrawl
//  File Name : Project.cs
//  Date : 2011/8/25
//  Author : Newmin
//  -----------------------------
//  2011-09-06 [+] newmin:添加检测绝对路径的功能，采集列表中包含"/news/12.html"类似文章
//
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using J6.DevFw.Utils;

namespace J6.DevFw.Toolkit.NetCrawl
{
    public delegate void DataPackFunc(DataPack datapack);

    public class Project
    {
        private State state = new State();

        public bool SaveResouce;

        public string SaveResourceExtension;

        public string ResouceSavePath;

        /// <summary>
        /// 相对路径的根路径
        /// </summary>
        private string basePath;

        /// <summary>
        /// 相对地址匹配表达式
        /// </summary>
        private static Regex absoluteUriRegex = new Regex("^(?!http)", RegexOptions.IgnoreCase);

        /// <summary>
        /// 项目编号
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 默认30秒超时
        /// </summary>
        public int RequestTimeOut = 30000;

        private string _formatedPageUriRule;

        /// <summary>
        /// 请求编码
        /// </summary>
        public string RequestEncoding { get; set; }

        /// <summary>
        /// 是否使用多线程，默认多线程
        /// </summary>
        public bool UseMultiThread { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        private Encoding Encode
        {
            get
            {
                int codepage;
                switch (RequestEncoding.ToLower())
                {
                    default:
                        codepage = 65001;
                        break;
                    case "default":
                        return Encoding.Default;
                    case "gb2312":
                        codepage = 936;
                        break;
                    case "big5":
                        codepage = 950;
                        break;
                }
                return Encoding.GetEncoding(codepage);
            }
        }

        /// <summary>
        /// 列表页URI规则
        /// </summary>
        public string ListUriRule { get; set; }

        /// <summary>
        /// 列表块规则,用于获取页面的URI列表
        /// </summary>
        public string ListBlockRule { get; set; }

        /// <summary>
        /// 页面URI规则
        /// </summary>
        public string PageUriRule { get; set; }

        /// <summary>
        /// 格式化的
        /// </summary>
        public string FormatedPageUriRule
        {
            get
            {
                return this._formatedPageUriRule ??
                       (this._formatedPageUriRule = RuleFormat.Format(this.PageUriRule));
            }
        }

        /// <summary>
        /// 要过滤的词语规则,将会被替换成{filter:过滤的词组}
        /// </summary>
        public string FilterWordsRule { get; set; }

        /// <summary>
        /// 数据属性规则
        /// </summary>
        public PropertyRule Rules { get; set; }

        /// <summary>
        /// 采集状态
        /// </summary>
        public State State
        {
            get { return state; }
        }

        /// <summary>
        /// 重置计数状态
        /// </summary>
        public void ResetState()
        {
            state.TotalCount = 0;
            state.SuccessCount = 0;
            state.FailCount = 0;
        }


        /// <summary>
        /// 采集列表页并返回数据集合
        /// </summary>
        /// <param name="listUriParameter"></param>
        /// <returns></returns>
        public IList<DataPack> Collect(object listUriParameter)
        {
            //ReadPage(parameter);
            string uri = String.Format(this.ListUriRule, listUriParameter);
            IList<DataPack> packs = new List<DataPack>();

            AnalysisListPage(uri, dp => { packs.Add(dp); });


#if DEBUG
    // 读取单页数据
    // int i=0;
    // DataPack pack= GetPageData("http://news.163.com/11/0824/10/7C7DG91H00011229.html", ref i);
#endif

            return packs;
        }

        /// <summary>
        /// 采集列表页，并对采集的结果执行操作
        /// </summary>
        /// <param name="listUriParameter">列表URI规则中的参数"{0}"的值</param>
        /// <param name="func"></param>
        public void InvokeList(object listUriParameter, DataPackFunc func)
        {
            string uri = String.Format(this.ListUriRule, listUriParameter);
            AnalysisListPage(uri, func);
        }

        /// <summary>
        /// 采集列表页，并对采集的结果执行操作
        /// </summary>
        /// <param name="listUri">列表页地址</param>
        /// <param name="func"></param>
        public void InvokeList(string listUri, DataPackFunc func)
        {
            AnalysisListPage(listUri, func);
        }

        /// <summary>
        /// 采集单篇文章
        /// </summary>
        /// <param name="pageUri"></param>
        /// <param name="func"></param>
        public void InvokeSingle(string pageUri, DataPackFunc func)
        {
            int i = 0;
            this.State.TotalCount = 1;
            GetPageData(pageUri, ref i, func);
        }


        /// <summary>
        /// 分析列表页面,并对结果执行回执操作
        /// </summary>
        /// <param name="parameter"></param>
        private void AnalysisListPage(string pageUri, DataPackFunc func)
        {
            int taskCount = 0,
                //任务数
                taskNumbers = 0; //一个计数用于判定任务是否完成

            string html; //下载的列表页面Html

            int bufferLength = 1;
            byte[] buffer = new byte[bufferLength]; //下载的数据缓冲区
            StringBuilder sb = new StringBuilder(); //构造返回的结果
            MatchCollection listMatches; //列表块匹配及页面地址匹配


#if DEBUG
            Console.WriteLine("开始从:{0}下载数据...", pageUri);
#endif


            //下载列表页内容
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(pageUri);
            request.Timeout = this.RequestTimeOut;

            Stream stream = request.GetResponse().GetResponseStream();


            using (StreamReader sr = new StreamReader(stream, this.Encode))
            {
                html = sr.ReadToEnd();
            }

#if DEBUG
            Console.WriteLine("返回的数据为:{0}", html);
#endif


            //分析列表页代码
            listMatches = Regex.Matches(html, RuleFormat.Format(this.ListBlockRule));


            //没有找到匹配
            if (listMatches.Count == 0)
            {
#if DEBUG
                Console.WriteLine("没找到匹配!");
#endif
                return;
            }


            //分析匹配数据

#if DEBUGS
            Console.WriteLine("\r\n------------------------------\r\n得到匹配的列表数据为:\r\n");
#endif

            Regex pageUriRegex = new Regex(this.FormatedPageUriRule);

            //创建词典
            IList<string> pageUrls = new List<string>();

            foreach (Match m in listMatches)
            {
#if DEBUG
                Console.WriteLine("\r\n------------------------------------------------\r\n{0}", m.Value);
#endif
                foreach (Match pm in pageUriRegex.Matches(m.Value))
                {
#if DEBUG
                    Console.WriteLine(pm.Value);
#endif
                    pageUrls.Add(pm.Value);


                    //获取页面数据，并添加已执行任务数

                    //多线程获取
                    //if (!UseSingleThread)
                    //{
                    //    new Thread(() =>
                    //    {
                    //        //调用回执方法
                    //        GetPageData(pm.Value, ref taskNumbers, func);
                    //    }
                    //    ).Start();
                    //}
                    //else   //单线程调用
                    //{
                    //    //调用回执方法
                    //    GetPageData(pm.Value, ref taskNumbers, func);
                    //}
                }
            }

            //增加任务数
            taskCount = pageUrls.Count;

            if (!this.UseMultiThread) //单线程
            {
                foreach (string pageUrl in pageUrls)
                {
                    //调用回执方法
                    GetPageData(pageUrl, ref taskNumbers, func);
                }
            }
            else
            {
                MultiThreadProcess mp = new MultiThreadProcess(5, taskCount);
                mp.Start<IList<string>>(urls =>
                {
                    lock (urls)
                    {
                        //调用回执方法
                        GetPageData(urls[0], ref taskNumbers, func);
                        pageUrls.Remove(urls[0]);
                    }
                }, pageUrls);
            }

            //设置任务总数
            state.TotalCount = taskCount;

            //直到线程均执行完毕，则返回
            do
            {
            } while (taskNumbers != taskCount);


#if DEBUG
            Console.WriteLine("任务完成....!共采集到{0}条", taskCount);
#endif
        }

        /// <summary>
        /// 获取一个页面的数据并返回
        /// </summary>
        /// <param name="pageUri">页面地址</param>
        /// <param name="number">维护一个计数,判断任务是否完成</param>
        /// <returns></returns>
        private DataPack GetPageData(string pageUri, ref int number, DataPackFunc func)
        {
            DataPack dp;
            int bufferLength = 10;
            byte[] buffer = new byte[bufferLength]; //下载的数据缓冲区
            StringBuilder sb = new StringBuilder(); //构造返回的结果
            Match match; //属性匹配


            //页面地址跟页面地址规则不匹配！

            if (!Regex.IsMatch(pageUri, this.FormatedPageUriRule))
            {
                ++number;
                state.FailCount++;
                return null;

                //throw new ArgumentException("页面地址跟页面地址规则不匹配！", pageUri);
            }

            //如果页面地址为相对路径，则加上域名
            if (absoluteUriRegex.IsMatch(pageUri)) pageUri = GetBasePath(pageUri) + pageUri;


            //返回页面的HTML
            string html = String.Empty;

            try
            {
                HttpWebRequest req = (HttpWebRequest) WebRequest.Create(pageUri);
                req.Timeout = this.RequestTimeOut;


                Stream stream = req.GetResponse().GetResponseStream();

                html = sb.ToString();
                using (StreamReader sr = new StreamReader(stream, this.Encode))
                {
                    html = sr.ReadToEnd();
                }
            }
            catch (Exception exc)
            {
                state.FailCount++;
                return null;
            }

            //输出返回的数据
#if DEBUG
            Console.WriteLine("\r\n------------------------------\r\n得到匹配的列表数据为:{0}",html);
#endif
            dp = new DataPack(Rules, pageUri);


            foreach (string propertyName in this.Rules)
            {
                match = Regex.Match(html, this.Rules[propertyName]);
                if (match != null)
                {
                    dp[propertyName] = match.Groups[1].Value;
                }
            }

#if DEBUG
            Console.WriteLine("\r\n-------------------------\r\n");
            foreach (KeyValuePair<string, string> pair in dp)
            {
                Console.WriteLine("{0}->{1}\r\n", pair.Key, pair.Value);
            }
#endif


            //更新计数
            ++number;


#if DEBUG
            Console.WriteLine("flish");
#endif
            //执行回执参数
            if (func != null) func(dp);


            //添加一个成功的计数
            state.SuccessCount++;

            return dp;
        }

        private string GetBasePath(string pageUri)
        {
            //如果已经计算出路径，则直接返回
            if (basePath != null) return basePath;

            //如果绝对路径以"/"开头
            if (pageUri.StartsWith("/"))
            {
                Regex reg = new Regex("^(http://[^/]+/)", RegexOptions.IgnoreCase);
                if (reg.IsMatch(ListUriRule))
                {
                    basePath = reg.Match(ListUriRule).Groups[1].Value;
                }
            }
            else
            {
                Regex reg = new Regex("([^/]+)$", RegexOptions.IgnoreCase);
                if (reg.IsMatch(ListUriRule))
                {
                    string filePath = reg.Match(ListUriRule).Value;
                    basePath = ListUriRule.Replace(filePath, String.Empty);
                }
            }
            return basePath;
        }
    }
}