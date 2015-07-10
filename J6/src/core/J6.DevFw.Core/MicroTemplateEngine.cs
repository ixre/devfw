/*
* Copyright(C) 2010-2012 S1N1.COM
* 
* File Name	: SimpleTemplateEngine
* Author	: Administrator
* Create	: 2012/10/26 23:49:52
* Description	:
*
*/

using System;
using System.Reflection;
using System.Text.RegularExpressions;

namespace J6.DevFw
{
    /// <summary>
    /// 微型模板引擎
    /// </summary>
    public sealed class MicroTemplateEngine
    {
        /// <summary>
        /// 包含方法的类型实例
        /// </summary>
        private readonly object _classInstance;

        public MicroTemplateEngine(object classInstance)
        {
            this._classInstance = classInstance;
        }

        /// <summary>
        /// 数据列正则
        /// </summary>
        private static Regex fieldRegex = new Regex("{([A-Za-z\\[\\]0-9_\u4e00-\u9fa5]+)}");

        /// <summary>
        /// 执行解析模板内容
        /// </summary>
        /// <param name="instance">包含标签方法的类的实例</param>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string Execute(object instance, string html)
        {
            string resultTxt = html; //返回结果

            const string tagPattern = "\\$([A-Za-z_0-9\u4e00-\u9fa5]+)\\(([^)]*)\\)";
            const string paramPattern = "\\s*'([^']+)',*|\\s*(?!=')([^,]+),*";

            Regex tagRegex = new Regex(tagPattern); //方法正则
            Regex paramRegex = new Regex(paramPattern); //参数正则

            Type type = instance.GetType();
            MethodInfo method;
            string tagName;
            object[] parameters;
            Type[] parameterTypes; //参数类型数组
            MatchCollection paramMcs;

            resultTxt = tagRegex.Replace(resultTxt, m =>
            {
                tagName = m.Groups[1].Value;
                //获得参数
                paramMcs = paramRegex.Matches(m.Groups[2].Value);
                parameters = new object[paramMcs.Count];

                //查找是否存在方法(方法参数均为string类型)
                parameterTypes = new Type[parameters.Length];
                for (int i = 0; i < parameterTypes.Length; i++)
                {
                    parameterTypes[i] = typeof (String);
                }
                method = type.GetMethod(
                    tagName,
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.IgnoreCase,
                    null,
                    parameterTypes,
                    null);

                //如果方法存在则执行返回结果，否则返回原始值
                if (method == null)
                {
                    return m.Value;
                }
                else
                {
                    //数字参数
                    string intParamValue;
                    //则给参数数组赋值
                    for (int i = 0; i < paramMcs.Count; i++)
                    {
                        intParamValue = paramMcs[i].Groups[2].Value;
                        if (intParamValue != String.Empty)
                        {
                            parameters[i] = intParamValue;
                        }
                        else
                        {
                            parameters[i] = paramMcs[i].Groups[1].Value;
                        }
                    }

                    //执行方法并返回结果
                    return method.Invoke(instance, parameters).ToString();
                }
            });
            return resultTxt;
        }

        /// <summary>
        /// 执行解析模板内容
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public string Execute(string html)
        {
            return Execute(this._classInstance, html);
        }

        /// <summary>
        /// 替换列中的模板字符
        /// </summary>
        /// <param name="format"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public string FieldTemplate(string format, Func<string, string> func)
        {
            return fieldRegex.Replace(format, a => { return func(a.Groups[1].Value); });
        }
    }
}