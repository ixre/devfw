//
// Copyright 2011 @ OPS Inc,All right reseved.
// Name:TemplateUtility.cs
// Author:newmin
// Create:2013/09/05
//

using System;
using System.Collections.Generic;
using System.Web;

namespace AtNet.DevFw.Template
{
    /// <summary>
    /// HTTP数据容器
    /// </summary>
    internal sealed class HttpDataContrainer : IDataContrainer
    {
        private IDictionary<string, object> varDict;

        public HttpDataContrainer()
        {
            object obj = HttpContext.Current.Items["__tpl_var_define__"];
            if (obj != null)
            {
                varDict = obj as IDictionary<string, object>;
            }
            else
            {
                varDict = new Dictionary<string, object>();
                HttpContext.Current.Items["__tpl_var_define__"] = varDict;
            }
        }

        public string GetTemplatePageCacheContent(string templateID)
        {
            if (TemplateCache.templateDictionary.ContainsKey(templateID))
            {
                return HttpRuntime.Cache["tpl_cache_" + templateID] as string;
            }
            return null;
        }

        public void SetTemplatePageCacheContent(string templateID, string content, string dependFileName)
        {
            HttpRuntime.Cache.Insert("tpl_cache_" + templateID, content,
                new System.Web.Caching.CacheDependency(dependFileName), DateTime.Now.AddDays(30), TimeSpan.Zero);
        }

        public void DefineVariable<T>(string key, T value)
        {
            if (value == null) return; //防止非法参数

            /*
            IDictionary<string, object> varDict;
            object obj = HttpContext.Current.Items["__tpl_var_define__"];
            if (obj != null)
            {
                varDict = obj as IDictionary<string, object>;
                if (varDict.Keys.Contains(key))
                {
                    throw new ArgumentException("模板变量已定义。", key);
                }
            }
            else
            {
                varDict = new Dictionary<string, object>();
                HttpContext.Current.Items["__tpl_var_define__"] = varDict;
            }*/

            if (varDict.Keys.Contains(key))
            {
                throw new ArgumentException("模板变量已定义。", key);
            }

            //如果不是基元类型，则保存类型
            Type t = typeof (T);
            if (t == typeof (String) || t.IsPrimitive)
            {
                varDict.Add(key, value);
            }
            else
            {
                varDict.Add(key, new Variable {Key = key, Value = value, Type = t});
            }

            HttpContext.Current.Items["__tpl_var_define__"] = varDict;
        }

        public void DefineVariable(string key, Variable variable)
        {
            if (varDict.Keys.Contains(key))
            {
                throw new ArgumentException("模板变量已定义。", key);
            }
            variable.Key = key;
            varDict.Add(key, variable);
            HttpContext.Current.Items["__tpl_var_define__"] = varDict;
        }

        public object GetVariable(string key)
        {
            if (this.varDict.Keys.Contains(key))
            {
                return varDict[key];
            }
            return null;
        }

        public IDictionary<string, object> GetDefineVariable()
        {
            /*
            object obj = HttpContext.Current.Items["__tpl_var_define__"];
            if (obj != null)
            {
                return obj as IDictionary<string, object>;
            }
            return null;
            */

            return this.varDict;
        }
    }
}