//
//
//  Copyright 2011 @ S1N1.COM.all rights reseved.
//
//  Project : Untitled
//  File Name : Director.cs
//  Date : 2011/8/25
//  Author : 
//
//

using System;
using System.IO;
using System.Text;
using System.Xml;

namespace AtNet.DevFw.Toolkit.NetCrawl
{
    public class Collector
    {
        /// <summary>
        /// 配置文件路径
        /// </summary>
        private string configFilePath;


        internal Collector(string configFile)
        {
            this.configFilePath = configFile;
        }

        /// <summary>
        /// 创建新的采集执行对象
        /// </summary>
        /// <param name="configFile"></param>
        /// <returns></returns>
        public static Collector Create(string configFile)
        {
            Collector dirc = new Collector(configFile);
            dirc.init();
            return dirc;
        }


        /// <summary>
        /// 所有项目
        /// </summary>
        private static Project[] projects;

        private void init()
        {
            if (this.configFilePath == "") throw new ArgumentException("配置文件不能为空！");
            if (!File.Exists(configFilePath))
            {
                //如果文件不存在,则创建并初始化内容
                File.Create(configFilePath).Dispose();
                const string initData = @"<?xml version=""1.0"" encoding=""utf-8""?>
<config>
  <projects>
    <project id=""ifengmainland"" name=""【示例】凤凰网大陆新闻"" encoding=""utf-8"">
        <listUriRule><![CDATA[http://news.ifeng.com/mainland/rt-channel/rtlist_20110825/{0}.shtml]]></listUriRule>
        <listBlockRule><![CDATA[<div\s+class=""newsList"">\s+([\s\S]+?)\s+</div>]]></listBlockRule>
        <pageUriRule><![CDATA[http://news.ifeng.com/mainland/detail_\d+_\d+/\d+/\d+_\d+.shtml]]></pageUriRule>
        <filterWordsRule><![CDATA[]]></filterWordsRule>
        <propertyRules>
        <add name=""title""><![CDATA[<h1\s+id=""artical_topic"">\s*([\s\S]+?)\s*</h1>]]></add>
        <add name=""content""><![CDATA[<div\s+id=""artical_real"">\s*([\s\S]+?)\s*<span class=""ifengLogo"">]]></add>
        </propertyRules>
    </project>
  </projects>
</config>";

                byte[] data = Encoding.UTF8.GetBytes(initData);
                FileStream fs = new FileStream(configFilePath, FileMode.Create, FileAccess.Write, FileShare.Read);
                fs.Write(data, 0, data.Length);
                fs.Flush();
                fs.Dispose();
            }
        }

        public Project GetProject(string id)
        {
            foreach (Project pro in this.GetProjects())
            {
                if (pro.Id == id) return pro;
            }
            return null;
        }

        /// <summary>
        /// 创建项目
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public bool CreateProject(Project project)
        {
            XmlNode newPrj,
                propertyNode;

            XmlDocument xd = new XmlDocument();
            xd.Load(configFilePath);

            XmlNodeList xnodelist = xd.SelectNodes(String.Format("/config/projects/project[@id=\"{0}\"]", project.Id));
            if (xnodelist.Count != 0) return false;

            XmlNode projectsNode = xd.SelectSingleNode("/config/projects");
            XmlAttribute xa;

            newPrj = xd.CreateElement("project");

            xa = xd.CreateAttribute("id");
            xa.Value = project.Id;
            newPrj.Attributes.Append(xa);

            xa = xd.CreateAttribute("name");
            xa.Value = project.Name;
            newPrj.Attributes.Append(xa);

            xa = xd.CreateAttribute("encoding");
            xa.Value = project.RequestEncoding;
            newPrj.Attributes.Append(xa);


            //列表地址规则
            propertyNode = xd.CreateElement("listUriRule");
            propertyNode.AppendChild(xd.CreateCDataSection(project.ListUriRule));
            newPrj.AppendChild(propertyNode);

            //列表块规则
            propertyNode = xd.CreateElement("listBlockRule");
            propertyNode.AppendChild(xd.CreateCDataSection(project.ListBlockRule));
            newPrj.AppendChild(propertyNode);

            //页面规则
            propertyNode = xd.CreateElement("pageUriRule");
            propertyNode.AppendChild(xd.CreateCDataSection(project.PageUriRule));
            newPrj.AppendChild(propertyNode);

            //过滤词规则
            propertyNode = xd.CreateElement("filterWordsRule");
            propertyNode.AppendChild(xd.CreateCDataSection(project.FilterWordsRule));
            newPrj.AppendChild(propertyNode);

            //内容属性规则
            propertyNode = xd.CreateElement("propertyRules");
            XmlNode pn;
            foreach (string pname in project.Rules)
            {
                pn = xd.CreateElement("add");

                //添加名称属性
                xa = xd.CreateAttribute("name");
                xa.Value = pname;
                pn.Attributes.Append(xa);
                pn.AppendChild(xd.CreateCDataSection(project.Rules[pname]));

                propertyNode.AppendChild(pn);
            }
            newPrj.AppendChild(propertyNode);

            projectsNode.AppendChild(newPrj);
            xd.Save(configFilePath);
            return true;
        }

        /// <summary>
        /// 保存项目
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="project"></param>
        /// <returns></returns>
        public bool SaveProject(string projectId, Project project)
        {
            XmlDocument xd = new XmlDocument();
            xd.Load(configFilePath);

            XmlNode projectNode,
                propertyNode;

            //获取项目节点
            projectNode =
                xd.SelectSingleNode(String.Format(String.Intern("/config/projects/project[@id=\"{0}\"]"), projectId));

            //节点为空，则返回false
            if (projectNode == null) return false;

            //新的编号已经存在则返回
            if (projectId != project.Id &&
                xd.SelectSingleNode(String.Format(String.Intern("/config/projects/project[@id=\"{0}\"]"), project.Id)) !=
                null) return false;

            projectNode.Attributes["id"].Value = project.Id;
            projectNode.Attributes["name"].Value = project.Name;
            projectNode.Attributes["encoding"].Value = project.RequestEncoding;

            //移除节点
            projectNode.RemoveChild(projectNode["listUriRule"]);
            projectNode.RemoveChild(projectNode["listBlockRule"]);
            projectNode.RemoveChild(projectNode["pageUriRule"]);
            projectNode.RemoveChild(projectNode["filterWordsRule"]);
            projectNode.RemoveChild(projectNode["propertyRules"]);

            /*********** 重新添加节点 ***************/

            //列表地址规则
            propertyNode = xd.CreateElement("listUriRule");
            propertyNode.AppendChild(xd.CreateCDataSection(project.ListUriRule));
            projectNode.AppendChild(propertyNode);

            //列表块规则
            propertyNode = xd.CreateElement("listBlockRule");
            propertyNode.AppendChild(xd.CreateCDataSection(project.ListBlockRule));
            projectNode.AppendChild(propertyNode);

            //页面规则
            propertyNode = xd.CreateElement("pageUriRule");
            propertyNode.AppendChild(xd.CreateCDataSection(project.PageUriRule));
            projectNode.AppendChild(propertyNode);

            //过滤词规则
            propertyNode = xd.CreateElement("filterWordsRule");
            propertyNode.AppendChild(xd.CreateCDataSection(project.FilterWordsRule));
            projectNode.AppendChild(propertyNode);

            //内容属性规则
            propertyNode = xd.CreateElement("propertyRules");
            XmlNode pn;
            XmlAttribute xa;
            foreach (string pname in project.Rules)
            {
                pn = xd.CreateElement("add");

                //添加名称属性
                xa = xd.CreateAttribute("name");
                xa.Value = pname;
                pn.Attributes.Append(xa);
                pn.AppendChild(xd.CreateCDataSection(project.Rules[pname]));

                propertyNode.AppendChild(pn);
            }
            projectNode.AppendChild(propertyNode);


            //保存
            xd.Save(configFilePath);

            return true;
        }

        /// <summary>
        /// 移除项目
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public bool RemoveProject(Project project)
        {
            XmlDocument xd = new XmlDocument();
            xd.Load(configFilePath);

            XmlNode prjsNode = xd.SelectSingleNode("/config/projects");
            XmlNode prjNode = xd.SelectSingleNode(String.Format("/config/projects/project[@id=\"{0}\"]", project.Id));
            if (prjNode == null) return false;

            prjsNode.RemoveChild(prjNode);

            xd.Save(configFilePath);
            return true;
        }

        public Project[] GetProjects()
        {
            if (projects == null)
            {
                Project pro;
                XmlNodeList propertyNodes;

                //加载配置文件
                XmlDocument xd = new XmlDocument();
                xd.Load(configFilePath);

                //获取项目配置列表
                XmlNodeList projectList = xd.SelectNodes("/config/projects/project");

                projects = new Project[projectList.Count];

                int i = 0;

                foreach (XmlNode node in projectList)
                {
                    //创建项目并为项目绑定属性规则
                    pro = new Project();
                    pro.Rules = new PropertyRule();

                    pro.Id = node.Attributes["id"].Value;
                    pro.Name = node.Attributes["name"].Value;
                    pro.RequestEncoding = node.Attributes["encoding"].Value;
                    pro.ListUriRule = node["listUriRule"].InnerText;
                    pro.ListBlockRule = node["listBlockRule"].InnerText;
                    pro.PageUriRule = node["pageUriRule"].InnerText;
                    pro.FilterWordsRule = node["filterWordsRule"].InnerText;

                    propertyNodes =
                        xd.SelectNodes(String.Format("/config/projects/project[@id=\"{0}\"]/propertyRules/add", pro.Id));
                    foreach (XmlNode pnode in propertyNodes)
                    {
                        pro.Rules.Add(pnode.Attributes["name"].Value, pnode.InnerText);
                    }
                    projects[i] = pro;
                    ++i;
                }
            }
            return projects;
        }


        /// <summary>
        /// 清除项目
        /// </summary>
        public void ClearProjects()
        {
            projects = null;
        }
    }
}