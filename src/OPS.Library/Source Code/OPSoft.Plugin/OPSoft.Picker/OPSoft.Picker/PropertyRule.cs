//
//
//  Copyright 2011 @ OPSoft Inc.all rights reseved.
//
//  Project : Untitled
//  File Name : PropertyRule.cs
//  Date : 2011/8/25
//  Author : 
//
//

using System.Collections;
using System.Collections.Generic;

namespace OPSoft.Plugin.NetCrawl
{
    /// <summary>
    /// 数据属性规则（正则表达式）
    /// </summary>
    public class PropertyRule : IEnumerable<string>
    {
        private IDictionary<string, string> dict = new Dictionary<string, string>();

        public string ID { get; set; }

        /// <summary>
        /// 添加属性规则
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(string key, string value)
        {
            dict.Add(key, value);
        }

        /// <summary>
        /// 获取属性规则
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string this[string key]
        {
            get { return dict.Keys.Contains(key) ? dict[key] : null; }
        }

        public IEnumerator<string> GetEnumerator()
        {
            foreach (string key in dict.Keys) yield return key;
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}