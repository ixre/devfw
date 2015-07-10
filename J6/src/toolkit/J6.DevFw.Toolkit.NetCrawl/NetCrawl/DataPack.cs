//
//  Copyright 2011 @ S1N1.COM.all rights reseved.
//
//  Project : Untitled
//  File Name : DataPack.cs
//  Date : 2011/8/25
//  Author : 
//
//

using System;
using System.Collections;
using System.Collections.Generic;

namespace J6.DevFw.Toolkit.NetCrawl
{
    /// <summary>
    /// 采集的数据包
    /// </summary>
    public class DataPack : ICloneable, IEnumerable<KeyValuePair<string, string>>
    {
        private PropertyRule property;

        /// <summary>
        /// 数据集合
        /// </summary>
        private IDictionary<string, string> dict = new Dictionary<string, string>();

        public DataPack(PropertyRule property, string referenceUrl)
        {
            this.property = property;
            this.ReferenceUrl = referenceUrl;

            foreach (string key in property)
            {
                dict.Add(key, key);
            }
        }

        /// <summary>
        /// 采集来源地址
        /// </summary>
        public string ReferenceUrl { get; private set; }

        //获取数据
        public string this[string key]
        {
            get { return dict.Keys.Contains(key) ? dict[key] : null; }
            set
            {
                if (dict.Keys.Contains(key)) dict[key] = value;
                else dict.Add(key, value);
            }
        }

        /// <summary>
        /// 克隆一个新的DataPack对象，并返回
        /// </summary>
        /// <returns></returns>
        object ICloneable.Clone()
        {
            DataPack pack = new DataPack(property, this.ReferenceUrl);
            return pack;
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            foreach (KeyValuePair<string, string> pair in dict) yield return pair;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}