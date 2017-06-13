//
//
//  Copyright 2011 (C) OPSoft INC,All rights reseved.
//
//  Project : tagsplugin
//  File Name : Tags.cs
//  Date : 8/27/2011
//  Author : Newmin
//  ---------------------------------------------
//  2011-09-13 newmin[+]:添加增加替换单个标签ReplaceSingleTag()
//                   [!]:修改可配置链接目标
//
//
namespace Ops.Plugin.Tags
{
    using System;

    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml;
    using System.IO;


    public class TagsManager
    {
        //配置文件路径
        private string configFilePath;

        private string[] nameArray;

        private TagsCollection tags=new TagsCollection();


        public class TagsCollection:IEnumerable<Tag>
        {
            internal IDictionary<string, Tag> dict = new Dictionary<string, Tag>();

            /// <summary>
            /// 标签数量
            /// </summary>
            public int Count
            {
                get { return dict.Count; }
            }

            public IEnumerator<Tag> GetEnumerator()
            {
                
                foreach (KeyValuePair<string,Tag> pair in dict)
                {
                    yield return pair.Value;
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }


        public TagsManager(string configFilePath)
        {
            this.configFilePath = configFilePath;
            if (!File.Exists(this.configFilePath))
            {
                //如果文件不存在,则创建并初始化内容
                File.Create(this.configFilePath).Dispose();
                const string initData = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<tags>\r\n\t<config/>\r\n\t<list/>\r\n</tags>";
                byte[] data = Encoding.UTF8.GetBytes(initData);
                FileStream fs = new FileStream(this.configFilePath, FileMode.Create, FileAccess.Write, FileShare.Read);
                fs.Write(data, 0, data.Length);
                fs.Flush();
                fs.Dispose();
            }
           this.LoadAllTags();
        }

        /// <summary>
        /// 标签集合
        /// </summary>
        public TagsCollection Tags { get { return tags; } }


        /// <summary>
        /// 加载所有Tags
        /// </summary>
        /// <returns></returns>
        private void LoadAllTags()
        {

            XmlDocument xd = new XmlDocument();

            //加载配置文件
            xd.Load(configFilePath);

            //所有标签节点
            XmlNodeList tagNodes = xd.SelectNodes("/tags/list/tag");

            //清空数据
            tags.dict.Clear();

            //创建关键词数组
            nameArray = new String[tagNodes.Count];

            //添加关键词到词典，并为关键词数组赋值
            for (int i = 0; i < tagNodes.Count; i++)
            {
                nameArray[i] = tagNodes[i].InnerText;

                tags.dict.Add(nameArray[i],
                    new Tag
                    {
                         Indent=int.Parse(tagNodes[i].Attributes["indent"].Value),
                        Name = tagNodes[i].InnerText,
                        Description = tagNodes[i].Attributes["description"].Value,
                        LinkUri = tagNodes[i].Attributes["linkuri"].Value
                    });
            }

            Array.Sort(nameArray, (a, b) => { return b.Length-a.Length; });

        }

        /// <summary>
        /// 添加标签
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public bool Add(Tag tag)
        {
            XmlDocument xd = new XmlDocument();
            xd.Load(configFilePath);

            //标签名称是否重复
            if (xd.SelectSingleNode(String.Format("/tags/list/tag[@name=\"{0}\"]", tag.Name)) != null) return false;

            //获取最后一个节点的序号
            XmlNode node = xd.SelectSingleNode("/tags/list/tag[last()]");
            if (node != null)
            {
                int tagIndex;
                int.TryParse(node.Attributes["indent"].Value, out tagIndex);
                tag.Indent = tagIndex + 1;
            }
            else
            {
                tag.Indent=1;
            }
            

            //所有标签节点
            XmlNode root = xd.SelectSingleNode("/tags/list");

            XmlNode tagNode = xd.CreateElement("tag");

            XmlAttribute xn =xd.CreateAttribute("indent");
            xn.Value = tag.Indent.ToString();
            tagNode.Attributes.Append(xn);

            //xn = xd.CreateAttribute("name");
            //xn.Value = tag.Name;
            //tagNode.Attributes.Append(xn);

            xn = xd.CreateAttribute("description");
            xn.Value = tag.Description;
            tagNode.Attributes.Append(xn);

            xn = xd.CreateAttribute("linkuri");
            xn.Value = tag.LinkUri;
            tagNode.Attributes.Append(xn);

            tagNode.InnerText = tag.Name;

            root.AppendChild(tagNode);

            //保存
            xd.Save(configFilePath);

            //清除数据并重新加载
            LoadAllTags();

            return true;

        }

        /// <summary>
        /// 获取设置
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetConfig(string key)
        {
            XmlDocument xd = new XmlDocument();
            xd.Load(configFilePath);
            XmlNode keyNode = xd.SelectSingleNode(String.Format("/tags/config/add[@key=\"{0}\"]",key));
            if (keyNode != null)
            {
                return keyNode.Attributes["value"].Value;
            }
            return null;
        }

        /// <summary>
        /// 修改设置
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetConfig(string key, string value)
        {

            XmlDocument xd = new XmlDocument();
            xd.Load(configFilePath);
            XmlNode keyNode = xd.SelectSingleNode(String.Format("/tags/config/add[@key=\"{0}\"]", key));
            if (keyNode != null)
            {
                keyNode.Attributes["value"].Value = value;
            }
            else
            {
                XmlNode rootNode = xd.SelectSingleNode("/tags/config");
                keyNode = xd.CreateElement("add");

                XmlAttribute xa=xd.CreateAttribute("key");
                xa.Value=key;
                keyNode.Attributes.Append(xa);

                xa=xd.CreateAttribute("value");
                xa.Value=value;
                keyNode.Attributes.Append(xa);

                rootNode.AppendChild(keyNode);
            }

            xd.Save(configFilePath);
        }

        /// <summary>
        /// 更新标签
        /// </summary>
        /// <param name="tag"></param>
        public void Update(Tag tag)
        {

            XmlDocument xd = new XmlDocument();
            xd.Load(configFilePath);

            XmlNode tagNode = xd.SelectSingleNode(String.Format("/tags/list/tag[@indent=\"{0}\"]", tag.Indent.ToString()));

            tagNode.InnerText = tag.Name;
            tagNode.Attributes["description"].Value = tag.Description;
            tagNode.Attributes["linkuri"].Value = tag.LinkUri;

            //保存
            xd.Save(configFilePath);

            //清除数据并重新加载
            LoadAllTags();

        }

        /// <summary>
        /// 删除标签
        /// </summary>
        /// <param name="id"></param>
        public void Delete(string id)
        {
            XmlDocument xd = new XmlDocument();
            xd.Load(configFilePath);

            XmlNode root = xd.SelectSingleNode("/tags/list");
            XmlNode tagNode = xd.SelectSingleNode(String.Format("/tags/list/tag[@indent=\"{0}\"]", id));

            if (tagNode != null)
            {
                root.RemoveChild(tagNode);
                xd.Save(configFilePath);

                //清除数据并重新加载
                LoadAllTags();
            }
        }

        /// <summary>
        /// 根据名称获取标签对象
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private Tag FindByName(string name)
        {
            if (tags.dict.Keys.Contains(name)) return tags.dict[name];
            return null;
        }

        private string Replace(string content,bool openInBlank)
        {
            //if (!defaultTagLinkFormat.Contains("{0}")) throw new ArgumentException("参数用{0}表示，并会用标签ID代替");

            Tag tag;
            Regex reg;

            //标签链接格式
            string tagLinkFormat =openInBlank?
                "<a href=\"{0}\" title=\"{1}\" class=\"auto-tag\" target=\"_blank\">{2}</a>" :
                "<a href=\"{0}\" title=\"{1}\" class=\"auto-tag\">{2}</a>";


            foreach (string key in nameArray)
            {
                tag=FindByName(key);
                if(tag==null)continue;

                reg = new Regex(String.Format("<a[^>]+>(?<key>{0})</a>|(?!<a[^>]*)(?<key>{0})(?![^<]*</a>)", Regex.Escape(key)), RegexOptions.IgnoreCase);


                content = reg.Replace(content, match =>
                {
                    return String.Format(tagLinkFormat,
                        String.IsNullOrEmpty(tag.LinkUri)?"javascript:;":tag.LinkUri,
                        tag.Description,
                        key);
                });
            }

            return content;
        }


        /// <summary>
        /// 替换链接
        /// </summary>
        /// <param name="content"></param>
        /// <param name="openInBlank"></param>
        /// <param name="singleMode"></param>
        /// <returns></returns>
        public string Replace(string content, bool openInBlank,bool singleMode)
        {
            //如果全部替换
            if (!singleMode) return Replace(content, openInBlank);

            Tag tag;
            Regex reg;

            //迭代变量
            int _index=0;

            foreach(string key in nameArray)
            {
                tag = FindByName(key);
                if (tag == null) continue;

                reg = new Regex(String.Format("<a[^>]+>(?<key>{0})</a>|(?!<a[^>]*)(?<key>{0})(?![^<]*</a>)", 
                    Regex.Escape(key)), RegexOptions.IgnoreCase);


                content = reg.Replace(content, match =>
                {
                   
                    if (++_index == 1)
                    {
                        return String.Format(

                            //标签链接格式
                            openInBlank ?"<a href=\"{0}\" title=\"{1}\" class=\"auto-tag\" target=\"_blank\">{2}</a>" :
                            "<a href=\"{0}\" title=\"{1}\" class=\"auto-tag\">{2}</a>",


                            String.IsNullOrEmpty(tag.LinkUri) ?"javascript:;":tag.LinkUri,
                            tag.Description,
                            key);
                    }
                    else
                    {
                        return match.Groups["key"].Value;
                    }
                });

                _index = 0;
            }
            return content;
        }

        /// <summary>
        /// 将单个标签替换成链接
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public string ReplaceSingleTag(string content)
        {
            return Replace(content, true, true);
        }

        /// <summary>
        /// 移除不存在的Tag
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public string RemoveAutoTags(string content)
        {
            string linkText;
            return Regex.Replace(content, "<a(.+?)class=\"auto-tag\"[^>]+>(.+?)</a>", match =>
            {
                linkText = match.Groups[2].Value;
                if (!tags.dict.Keys.Contains(linkText))
                {
                    return linkText;
                }
                return match.Value;
            },RegexOptions.Multiline);
        }


        /// <summary>
        /// 替换内容为标签
        /// </summary>
        /// <param name="defaultTagLinkFormat"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        private string Replace2(string content)
        {
            return null;



            Regex reg = new Regex(@"(?i)(?:^|(?<!<a\b[^<>]*)>)[^<>]*(?:<|$)");

#if DEBUG
            //System.Web.HttpContext.Current.Response.Write(String.Format("<br ><br />匹配{0}<br /><br />",reg.IsMatch(content)));




#endif
            //int length = 0;
            string temp;
            Tag tag;

            //已经添加的栈,确保只替换一次
            Stack<int> stack = new Stack<int>();

            return reg.Replace(content, m =>
            {
                temp =Regex.Replace(m.Value,"<|>|\\/",String.Empty);


               // System.Web.HttpContext.Current.Response.Write(String.Format("<br />({0})<br />", m.Value));

                //length = temp.Length;

                //遍历关键此数组,且保证关键词序列不在栈中
                for (int i = nameArray.Length - 1; i >= 0 && !stack.Contains(i); i--)
                {
                    tag = this.FindByName(nameArray[i]);

                    if (tag != null)
                    {
                        temp = Regex.Replace(temp, String.Format(@"(?is)^((?:(?:(?!{0}|</?a\b).)*<a\b(?:(?!</?a\b).)*</a>)*(?:(?!{0}|</?a\b).)*)(?<tag>{0})", Regex.Escape(tag.Name))
                            , String.Format("$1<a href=\"{0}\" target=\"_blank\" title=\"{1}\">{2}</a>",
                            String.IsNullOrEmpty(tag.LinkUri)?"javascript:;":tag.LinkUri,       //如果未设置链接，则使用默认格式
                            tag.Description, "${tag}"));
                    }

                    //保证文中只被替换一次
                    stack.Push(i);

                    // if (length != temp.Length)
                    //{
                    //   stack.Push(i);

                    //  }
                    //length = temp.Length;
                }

                return temp;
            });
        }

        /// <summary>
        /// 给关键字加链接，同一关键字只加一次
        /// </summary>
        /// <param name="content">源字符串</param>
        /// <param name="keys">关键字泛型</param>
        /// <returns>替换后结果</returns>
        private string keyAddUrl(string content)
        {
            return null;
            /*
            Regex reg = new Regex(@"(?i)(?:^|(?<!<a\b[^<>]*)>)[^<>]*(?:<|$)");
            int length = 0;
            string temp;
            Tag tag;

            return reg.Replace(content, m =>
            {
                temp = m.Value;
                length = temp.Length;
                for (int i = tags.Count - 1; i >= 0; i--)
                {
                    tag = this.FindByName(nameArray[i]);
                    temp = Regex.Replace(temp, String.Format(@"(?is)^((?:(?:(?!{0}|</?a\b).)*<a\b(?:(?!</?a\b).)*</a>)*(?:(?!{0}|</?a\b).)*)(?<tag>{0})", Regex.Escape(tag.Name))
                        , String.Format("$1<a href=\"{0}\" target=\"_blank\" title=\"{1}\">${tag}</a>",tag.LinkUri,tag.Description));
                    
                    if (length != temp.Length)
                    {
                       //tags.nameDict.Remove(tags.nameDict[i]);
                     }
                    length = temp.Length;
                }
                return temp;
            });*/
        }
    }
}
