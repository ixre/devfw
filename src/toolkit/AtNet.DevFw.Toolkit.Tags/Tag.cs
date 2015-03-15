//
//
//  Copyright 2011 (C) OPSoft INC,All rights reseved.
//
//  Project : tagsplugin
//  File Name : Tag.cs
//  Date : 8/27/2011
//  Author : 
//
//


namespace AtNet.DevFw.Toolkit.Tags
{

    /// <summary>
    /// 标签
    /// </summary>
    public class Tag
    {
        /// <summary>
        /// 标识ID
        /// </summary>
        public int Indent { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 链接地址
        /// </summary>
        public string LinkUri { get; set; }
    }

}
