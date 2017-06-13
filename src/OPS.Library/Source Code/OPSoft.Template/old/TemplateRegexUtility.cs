//
// Copyright 2011 @ OPS Inc,All right reseved.
// Name:RegexUtility.cs
// Author:newmin
// Create:2011/06/05
//

using System.Text.RegularExpressions;

namespace Ops.Template
{
    public sealed class TemplateRegexUtility
    {
        private static readonly Regex tagRegex = new Regex("\\${([a-zA-Z\u4e00-\u9fa5\\._]+)}");

        //部分页匹配模式
        internal static Regex partialRegex = new Regex("\\${partial:\"(.+?)\"}");

        public static string Replace(string templateID, string tagKey, string tagValue)
        {
            return null;
        }

        /// <summary>
        /// 替换模板数据
        /// </summary>
        /// <param name="templateID"></param>
        /// <param name="eval"></param>
        /// <returns></returns>
        internal static string ReplaceTemplate(string templateID, MatchEvaluator eval)
        {
            string html = TemplateUtility.Read(templateID);

            return TemplateRegexUtility.Replace(html, eval);
        }

        /// <summary>
        /// 替换标签
        /// </summary>
        /// <param name="html"></param>
        /// <param name="eval"></param>
        /// <returns></returns>
        public static string Replace(string html, MatchEvaluator eval)
        {
            //如果包含部分视图，则替换成部分视图的内容
            //ReplacePartial(html);

            //替换匹配
            return tagRegex.Replace(html, eval);
        }

        internal static string ReplaceHtml(string html, string tagKey, string tagValue)
        {
            return Regex.Replace(html, "\\${" + tagKey + "}", tagValue, RegexOptions.IgnoreCase);
        }


        /// <summary>
        /// 替换模板的部分视图
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        internal static string ReplacePartial(string html)
        {
            //匹配的部分视图编号
            string matchValue;

            //如果包含部分视图，则替换成部分视图的内容
            if (partialRegex.IsMatch(html))
            {
                //替换模板里的部分视图，并将内容添加到模板内容中
                html = Regex.Replace(html, "\\${partial:\"(.+?)\"}", match =>
                {
                    matchValue = match.Groups[1].Value;
                    return Regex.IsMatch(matchValue, "^[a-z0-9]+$", RegexOptions.IgnoreCase)
                        ? TemplateUtility.Read(match.Groups[1].Value)
                        : match.Value;
                });
            }

            //返回替换部分视图后的内容
            return html;
        }
    }
}