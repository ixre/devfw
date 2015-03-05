/*
 * PagedLinksBuilder 分页链接构造器
 * Copyright 2010 OPS,All right reseved!
 * Newmin(ops.cc)  @  2010/11/18
 * 
 */

//
// --------------------------------------------------
// Copyright 2011 (C) OPSoft INC.All rights reseved.
// Project : OPSoft.Web.UI
// Name : PagedLinkBuilder.cs
// Author : newmin
// Create : 2010/11/18
// --------------------------------------------------
// 2011-09-06 [+] newmin: 添加第一页格式及内置风格样式
//
//
namespace Ops.Web.UI
{
    using System;
    using System.Text;
    using System.Text.RegularExpressions;

    public enum PageLinkStyle
    {
        /// <summary>
        /// 自定义风格
        /// </summary>
        Custom=-1,

        /// <summary>
        /// 默认
        /// </summary>
        Default=0
    }



    /// <summary>
    /// 分页链接创建器(url:www.ops.cc)
    /// </summary>
    
    [Obsolete("Instanct of UrlPager")]
    public class PagedLinkBuilder:IFormattable
    {
        /// <summary>
        /// 风格包
        /// </summary>
        private static readonly String[] stylePack = new string[1];

        static PagedLinkBuilder()
        {
            //默认风格
            stylePack[0] = @"
div.pagewrap{line-height:30px;position:relative;}
div.pagewrap span.disabled{padding:2px 5px;border:solid 1px #999;color:#888;margin:0 2px;}
div.pagewrap span.current{padding:2px 5px;margin:0 2px;border:solid 1px #4196cf;}
div.pagewrap input{width:30px;line-height:15px;border:solid 1px #4196cf;}
div.pagewrap a{line-height:25px;padding:2px 5px;background:#f5f5f5;margin:0 2px;border:solid 1px #4196cf;}
div.pagewrap span.pageinfo{position:absolute;right:10px;top:5px;color:#4196cf}  
            ";
        }

        public PagedLinkBuilder() { }

        public PagedLinkBuilder(int currentPageIndex, int pageCount)
        {
            CurrentPageIndex = currentPageIndex;
            PageCount = pageCount;
        }

        /// <summary>
        /// 当前页面索引（从1开始）
        /// </summary>
        public int CurrentPageIndex { get; set; }

        /// <summary>
        /// 页面总数
        /// </summary>
        public int PageCount { get; set; }

        /// <summary>
        /// 链接长度,创建多少个跳页链接
        /// </summary>
        public int? LinkCount { get; set; }

        /// <summary>
        /// 记录条数
        /// </summary>
        public int RecordCount { get; set; }

        /// <summary>
        /// 分页风格
        /// </summary>
        public PageLinkStyle Style { get; set; }

        /// <summary>
        /// 上一页文字
        /// </summary>
        public string PreviousPageText { get; set; }

        /// <summary>
        /// 下一页文字
        /// </summary>
        public string NextPageText { get; set; }


        /// <summary>
        /// 调页框按钮文本
        /// </summary>
        public string InputButtonText { get; set; }

        /// <summary>
        /// 分页链接格式
        /// </summary>
        public string LinkFormat { get; set; }

        /// <summary>
        /// 第一页链接格式
        /// </summary>
        public string FirstPageLinkFormat { get; set; }

        /// <summary>
        /// 页码文本格式
        /// </summary>
        public string PageTextFormat { get; set; }

        /// <summary>
        /// 是否允许输入页码调页
        /// </summary>
        public bool EnableInput { get; set; }

        /// <summary>
        /// 是否显示分页详细记录
        /// </summary>
        public bool DisplayInfo { get; set; }


        /// <summary>
        /// 输入分页链接HTML代码
        /// </summary>
        /// <param name="format">例如:?domain=ops.cc&page={0},{0}将会被解析成页码</param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        public string ToString(string format,IFormatProvider formatProvider)
        {           

            //计算上一页和下一页
            int pindex=CurrentPageIndex-1;
            int nindex=CurrentPageIndex+1;

            StringBuilder sb = new StringBuilder();

            //Div Wrap
            sb.Append("<div class=\"pagewrap\">");

            //输出上一页
            if (pindex < 1)
            {
                sb.Append("<span class=\"disabled\">").Append(PreviousPageText)
                    .Append("</span>");
            }
            else
            {
                sb.Append("<span class=\"previous\"><a href=\"")
                    .Append(String.Format(pindex == 1 ? FirstPageLinkFormat : LinkFormat, pindex))
                    .Append("\">").Append(PreviousPageText).Append("</a></span>");
            }


            //起始页:CurrentPageIndex / 10 * 10+1
            //结束页:(CurrentPageIndex%10==0?CurrentPageIndex-1: CurrentPageIndex) / 10 * 10
            //当前页数能整除10的时候需要减去10页，否则不能选中


            //链接页码数量(默认10)
            int c = LinkCount ?? 10;

            int startPage =(CurrentPageIndex-1) / c * c + 1;
            for (int i = 1, j = startPage; i<=c&&j<=PageCount; 
                    i++, j =(CurrentPageIndex%c==0?CurrentPageIndex-1: CurrentPageIndex) /c * c+ i)
            {
                if (j == CurrentPageIndex)
                {
                    if (String.IsNullOrEmpty(PageTextFormat))
                    {
                        sb.Append("<span class=\"current\">").Append(j.ToString()).Append("</span>");
                    }
                    else
                    {
                        sb.Append(String.Format(PageTextFormat,
                                  String.Format("<span class=\"current\">{0}</span>",j.ToString())));
                    }
                }
                else
                {
                    //如果为第一页，用第一页格式
                    if (String.IsNullOrEmpty(PageTextFormat))
                    {
                        sb.Append("<a class=\"page\" href=\"").Append(String.Format(j == 1 ? FirstPageLinkFormat : LinkFormat, j))
                            .Append("\">").Append(j.ToString()).Append("</a>");
                    }
                    else
                    {
                        sb.Append(String.Format(PageTextFormat,
                                  String.Format("<a class=\"page\" href=\"{0}\">{1}</a>",
                                  String.Format(j == 1 ? FirstPageLinkFormat : LinkFormat, j),
                                  j.ToString())));
                    }
                }
            }


            //显示输入页码框
            if (EnableInput)sb.Append("<input type=\"text\" size=\"2\"/><a href=\"#\" class=\"go\" onclick=\"gotoPage(this)\">").Append(InputButtonText??"跳页").Append("</a>");


            //输出下一页链接
            if (PageCount <= CurrentPageIndex)
            {
                sb.Append("<span class=\"disabled\">").Append(NextPageText).Append("</span>");
            }
            else
            {
                sb.Append("<span class=\"next\"><a href=\"").Append(String.Format(LinkFormat,CurrentPageIndex+1))
                    .Append("\">").Append(NextPageText).Append("</a></span>");
            }

            if (DisplayInfo)
            {
                sb.Append("&nbsp;<span class=\"pageinfo\">当前显示第").Append(CurrentPageIndex).Append("/").Append(PageCount)
                    .Append("页，共").Append(RecordCount).Append("条。</span>");
            }


            //Wrap Close
            sb.Append("</div>");


            //如果使用页码输入框，则输出JS
            if(EnableInput)
            {
                sb.Append(@"<script type=""text/javascript"">");
                sb.Append(String.Format("var __p={0},__f1='{1}',__f2='{2}';", PageCount, FirstPageLinkFormat, LinkFormat));
                sb.Append(@"
                            function gotoPage(t){
                                var page=t.previousSibling.value;
                                if(!/^\d+$/.test(page))page=1;
                                else if(page>__p)page=__p;
                                location.href=(page==1?__f1:__f2).replace('{0}',page);
                            }
                            </script>");
            }

            //风格
            if (Style != PageLinkStyle.Custom)
            {
                sb.Append("<style type=\"text/css\">")
                    .Append(stylePack[(int)Style]).Append("</style>");
            }



            return sb.ToString();
        }

        /// <summary>
        /// 返回分页Html
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToString(null, null);
        }


        #region 扩展
        /// <summary>
        /// 创建分页信息
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="currentPageIndex"></param>
        /// <param name="recordCount"></param>
        /// <param name="pageCount"></param>
        /// <returns></returns>
        public static string BuildPagerInfo(string format,ref int currentPageIndex, int recordCount, int pageCount)
        {
            StringBuilder sb = new StringBuilder();
            if (currentPageIndex < 1) currentPageIndex = 1;
            if (currentPageIndex > pageCount) currentPageIndex = pageCount;
            PagedLinkBuilder p = new PagedLinkBuilder(currentPageIndex, pageCount);
           // p.PreviousPageText = "<<";
            // p.NextPageText = ">>";
            p.Style = PageLinkStyle.Custom;
            p.PreviousPageText = "&lt;&lt;上一页";
             p.NextPageText = "&gt;&gt;下一页";
            p.EnableInput = true;
            p.InputButtonText = "跳页";
            p.FirstPageLinkFormat = format;
            p.DisplayInfo = true;
            p.RecordCount = recordCount;

            p.LinkFormat = format;

            return p.ToString();
        }

        /// <summary>
        /// 创建分页信息
        /// </summary>
        public static string BuildPagerInfo(string firstFormat,string format,int currentPageIndex, int recordCount, int pageCount)
        {
            StringBuilder sb = new StringBuilder();

            PagedLinkBuilder p = new PagedLinkBuilder(currentPageIndex, pageCount);
            p.RecordCount = recordCount;
            p.Style = PageLinkStyle.Default;

            p.PreviousPageText = "&lt;&lt;上一页";
            p.NextPageText = "&gt;&gt;下一页";
            p.InputButtonText = "跳页";

            p.FirstPageLinkFormat =firstFormat;
            p.LinkFormat = format;
            p.EnableInput = true;
            p.DisplayInfo = true;

            return p.ToString();
        }
        
        /// <summary>
        /// 创建分页信息
        /// </summary>
        public static string BuildPagerInfo(string format, int currentPageIndex, int recordCount, int pageCount)
        {
            StringBuilder sb = new StringBuilder();

            PagedLinkBuilder p = new PagedLinkBuilder(currentPageIndex, pageCount);
            p.RecordCount = recordCount;
            p.Style = PageLinkStyle.Default;

            p.PreviousPageText = "&lt;&lt;上一页";
            p.NextPageText = "&gt;&gt;下一页";
            p.InputButtonText = "跳页";

            p.FirstPageLinkFormat =format;
            p.LinkFormat = format;
            p.EnableInput = true;
            p.DisplayInfo = true;

            return p.ToString();
        }

        /// <summary>
        /// 创建分页信息
        /// </summary>
        public static string BuildPagerInfo(PagedLinkBuilder builder,string format,int currentPageIndex, int recordCount, int pageCount)
        {
            builder.FirstPageLinkFormat = format;
            builder.LinkFormat = format;
            builder.CurrentPageIndex = currentPageIndex;
            builder.RecordCount = recordCount;
            builder.PageCount = pageCount;
            return builder.ToString();
        }



        #endregion
    }
}