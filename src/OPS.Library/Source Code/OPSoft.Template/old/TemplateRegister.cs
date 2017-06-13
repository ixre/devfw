//
// Copyright 2011 @ OPS Inc,All right reseved.
// Name:TemplateRegister.cs
// Author:newmin
// Create:2011/06/28
//

using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Ops.Template
{
    /// <summary>
    /// 模板注册
    /// </summary>
    public class TemplateRegister
    {
        private DirectoryInfo directory;
        private TemplateNames nametype;

        /// <summary>
        /// 注册模板时发生
        /// </summary>
        public event TemplateBehavior OnRegister;

        public TemplateRegister(string directoryPath, TemplateNames nametype)
        {
            this.nametype = nametype;
            this.directory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + directoryPath);
            if (!this.directory.Exists) throw new DirectoryNotFoundException("模版文件夹不存在!");
        }

        public TemplateRegister(string directoryPath)
        {
            this.directory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + directoryPath);
            if (!this.directory.Exists) throw new DirectoryNotFoundException("模版文件夹不存在!");
        }

        public TemplateRegister(DirectoryInfo templateDirectory, TemplateNames nametype)
        {
            this.nametype = nametype;
            this.directory = templateDirectory;
            if (!this.directory.Exists) throw new DirectoryNotFoundException("模版文件夹不存在!");
        }

        /// <summary>
        /// 注册模板
        /// </summary>
        public void Register()
        {
            //注册模板
            RegisterTemplates(directory, this.nametype);

            //触发注册模板事件
            if (OnRegister != null) OnRegister();
        }

        //递归方式注册模板
        private static void RegisterTemplates(DirectoryInfo dir, TemplateNames nametype)
        {
            Regex allowExt = new Regex("(.html|.phtml)$", RegexOptions.IgnoreCase);
            foreach (FileInfo file in dir.GetFiles())
            {
                if (allowExt.IsMatch(file.Extension))
                {
                    TemplateCache.RegisterTemplate(TemplateUtility.GetTemplateID(file.FullName, nametype), file.FullName);
                }
            }
            foreach (DirectoryInfo _dir in dir.GetDirectories())
            {
                //如果文件夹是可见的
                if ((_dir.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
                {
                    RegisterTemplates(_dir, nametype);
                }
            }
        }
    }
}