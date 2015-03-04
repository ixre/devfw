//
// Copyright 2011 @ OPS Inc,All right reseved.
// Name:TemplateCache.cs
// Author:newmin
// Create:2011/06/05
//

using System;
using System.Collections.Generic;

namespace AtNet.DevFw.Template
{
    /// <summary>
    /// 模板缓存
    /// </summary>
    public static class TemplateCache
    {
        /// <summary>
        /// 模板编号列表
        /// </summary>
        internal static IDictionary<string, Template> templateDictionary = new Dictionary<string, Template>();

        /// <summary>
        /// 标签词典
        /// </summary>
        public static readonly TagCollection Tags = new TagCollection();


        /// <summary>
        /// 标签
        /// </summary>
        public class TagCollection
        {
            private static IDictionary<string, string> tagDictionary = new Dictionary<string, string>();

            public string this[string key]
            {
                get
                {
                    if (!tagDictionary.ContainsKey(key)) return "${" + key + "}";
                    return tagDictionary[key];
                }
                set
                {
                    if (tagDictionary.ContainsKey(key)) tagDictionary[key] = value;
                    else tagDictionary.Add(key, value);
                }
            }

            public void Add(string key, string value)
            {
                if (tagDictionary.ContainsKey(key))
                    throw new ArgumentException("键:" + key + "已经存在!");
                else tagDictionary.Add(key, value);
            }
        }

        /// <summary>
        /// 注册模板
        /// </summary>
        /// <param name="templateID"></param>
        /// <param name="filePath"></param>
        internal static void RegisterTemplate(string templateID, string filePath)
        {
            templateID = templateID.ToLower();
            if (!templateDictionary.ContainsKey(templateID))
            {
                templateDictionary.Add(templateID, new Template
                {
                    ID = templateID,
                    FilePath = filePath
                });
            }
        }

        /// <summary>
        /// 如果模板字典包含改模板则获取缓存
        /// </summary>
        /// <param name="templateID"></param>
        /// <returns></returns>
        internal static string GetTemplateContent(string templateID)
        {
            string _templateID = templateID.ToLower();

            if (templateDictionary.ContainsKey(_templateID))
            {
                return templateDictionary[_templateID].Content;
            }
            //throw new ArgumentNullException("TemplateID", String.Format("模板{0}不存在,ID:", templateID));
            throw new ArgumentNullException("TemplateID", String.Format("模板{0}不存在。", templateID));
        }

        /// <summary>
        /// 清除所有模板缓存
        /// </summary>
        //internal static void Clear()
        //{
        //    templateDictionary = null;
        //    foreach (System.Collections.DictionaryEntry k in HttpRuntime.Cache)
        //    {
        //        if (k.Key.ToString().StartsWith("tpl_")) HttpRuntime.Cache.Remove(k.Key.ToString());
        //    }
        //}
    }
}