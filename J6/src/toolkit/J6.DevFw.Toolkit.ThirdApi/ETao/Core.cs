using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace J6.DevFw.Toolkit.ThirdApi.ETao
{
    /// <summary>
    /// 获取产品项
    /// </summary>
    /// <returns></returns>
    public delegate IList<Item> GetItemsHandler();

    /// <summary>
    /// 获取分类项
    /// </summary>
    /// <returns></returns>
    public delegate Cate[] GetCatsHandler();


    public class Core
    {
        /// <summary>
        /// 创建项目
        /// </summary>
        /// <param name="item"></param>
        private static void Generate(Item item)
        {
            string fileName = String.Format("{0}{1}items/{2}.xml", Config.PhypicPath, Config.SavePath, item.outer_id);

            StringBuilder sb = new StringBuilder();
            IDictionary<string,object> prolist;

            XmlDocument xd = new XmlDocument();

            xd.AppendChild(xd.CreateXmlDeclaration("1.0", null, null));

            XmlNode root = xd.CreateElement("item");

            Type type = typeof(Item);
            PropertyInfo[] pros = type.GetProperties();
            object proValue;


            foreach (PropertyInfo pro in pros)
            {
                XmlNode proNode = xd.CreateElement(pro.Name);
                proValue = pro.GetValue(item, null);
                if (proValue != null)
                {
                    if (!pro.PropertyType.IsValueType && proValue as IDictionary<string, object> != null)
                    {
                        prolist = pro.GetValue(item, null) as IDictionary<string, object>;
                        foreach (KeyValuePair<string, object> entity in prolist)
                        {
                            //重复键用:隔开
                            XmlNode childProNode = xd.CreateElement(entity.Key.Split(':')[0]);
                            childProNode.InnerText = entity.Value.ToString().Replace("$DOMAIN", Config.Domain);
                            proNode.AppendChild(childProNode);
                        }
                    }
                    else
                    {
                        proNode.InnerText = proValue.ToString().Replace("$DOMAIN", Config.Domain);
                    }
                }

                root.AppendChild(proNode);
            }
            xd.AppendChild(root);
            xd.Save(fileName);
        }

        public static void UploadItem(Item item)
        {
            Generate(item);
            string fileName = String.Format("{0}{1}IncrementIndex.xml", Config.PhypicPath, Config.SavePath);
            XmlDocument xd = new XmlDocument();
            if (!File.Exists(fileName))
            {

                XmlNode root = xd.CreateElement("root");

                Core.AppendNode(xd, root, "version", Config.Version);
                Core.AppendNode(xd, root, "modified", String.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now));
                Core.AppendNode(xd, root, "seller_id", Config.Seller);
                Core.AppendNode(xd, root, "cat_url", String.Format("{0}{1}SellerCats.xml", Config.Domain, Config.SavePath));
                Core.AppendNode(xd, root, "dir", String.Format("{0}{1}items/", Config.Domain, Config.SavePath));
                Core.AppendNode(xd,root,"item_ids",null);
                xd.AppendChild(root);
                xd.Save(fileName);
            }

            //重新加载
            xd.Load(fileName);
            XmlNode ids = xd.SelectSingleNode("/root/item_ids");

            XmlNode modify = xd.SelectSingleNode("/root/modified");
            modify.InnerText = String.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);

            bool isExist=false;
            foreach(XmlNode xn in ids.ChildNodes)
            {
                if (xn.NodeType == XmlNodeType.Element && xn.InnerText==item.outer_id.ToString())
                {
                    isExist = true;
                    break;
                }
            }

            if (!isExist)
            {
                XmlNode xn = xd.CreateElement("outer_id");
                
                XmlAttribute xat = xd.CreateAttribute("action");
                xat.Value = "upload";
                xn.Attributes.Append(xat);

                xn.InnerText = item.outer_id;
                ids.AppendChild(xn);
            }

            xd.Save(fileName);

        }

        public static void Generate(Cate[] rootCates)
        {
            string fileName = String.Format("{0}{1}SellerCats.xml", Config.PhypicPath, Config.SavePath);
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            StringBuilder sb = new StringBuilder();
            Cate[] cates;

            XmlDocument xd = new XmlDocument();
            XmlNode root = xd.CreateElement("root");

            //分类节点
            XmlNode cateNode;
            XmlNode catesNode;
            object proValue;

            Core.AppendNode(xd, root, "version", Config.Version);
            Core.AppendNode(xd, root, "modified",String.Format("{0:yyyy-MM-dd HH:mm:ss}",DateTime.Now.ToUniversalTime()));
            Core.AppendNode(xd, root, "seller_id", Config.Seller);

            catesNode = xd.CreateElement("seller_cats");



            Type type = typeof(Cate);
            PropertyInfo[] pros = type.GetProperties();

            foreach(Cate c in rootCates){

                cateNode =xd.CreateElement("cat");
                foreach (PropertyInfo pro in pros)
                {
                    proValue = pro.GetValue(c, null);
                    if (proValue as Cate[] != null)
                    {
                        cates = pro.GetValue(c, null) as Cate[];
                        if (cates.Length != 0)
                        {
                            XmlNode cateNode2 = xd.CreateElement("cats");
                            XmlNode cateNode3;
                            foreach (Cate c2 in cates)
                            {
                                cateNode3 = xd.CreateElement("cat");
                                foreach (PropertyInfo pro2 in pros)
                                {
                                    if (pro2.Name != "child")
                                    {
                                        Core.AppendNode(xd, cateNode3, pro2.Name, pro2.GetValue(c, null).ToString());
                                    }
                                }
                                cateNode2.AppendChild(cateNode3);
                            }

                            cateNode.AppendChild(cateNode2);
                        }
                    }
                    else
                    {
                        Core.AppendNode(xd, cateNode, pro.Name, proValue.ToString());
                    }
                }

                catesNode.AppendChild(cateNode);
            }
            root.AppendChild(catesNode);
            xd.AppendChild(root);
            xd.Save(fileName);
        }

        /// <summary>
        /// 创建完整索引
        /// </summary>
        /// <param name="items"></param>
        public static void GenerateFullIndex(IList<Item> items)
        {
            XmlDocument xd = new XmlDocument();
            FileInfo file = new FileInfo(String.Format("{0}{1}fullindex.xml", Config.PhypicPath, Config.SavePath));
            if (file.Exists)
            {
                file.Delete();
            }

            xd.AppendChild(xd.CreateXmlDeclaration("1.0", null, null));

            XmlNode root = xd.CreateElement("root");

            Core.AppendNode(xd, root, "version", Config.Version);
            Core.AppendNode(xd, root, "modified", String.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now));
            Core.AppendNode(xd, root, "seller_id", Config.Seller);
            Core.AppendNode(xd, root, "cat_url", String.Format("{0}{1}SellerCats.xml", Config.Domain, Config.SavePath));
            Core.AppendNode(xd, root, "dir", String.Format("{0}{1}items/", Config.Domain, Config.SavePath));

            XmlNode ids = xd.CreateElement("item_ids");




            MultiThreadProcess mp = new MultiThreadProcess(Config.Threads, items.Count);
            mp.Start<IList<Item>>(item =>
            {
                Item itm = item[0];

                XmlNode xn = xd.CreateElement("outer_id");

                XmlAttribute xat = xd.CreateAttribute("action");
                xat.Value = "upload";
                xn.Attributes.Append(xat);

                xn.InnerText = itm.outer_id;
                ids.AppendChild(xn);

                if (item.Count == 1)
                {
                    UploadItem(itm);
                }
                else
                {
                    Generate(itm);
                }

                item.Remove(itm);

            }, items);

            while (true)
            {
                if (!mp.IsAlive)
                {
                    root.AppendChild(ids);
                    xd.AppendChild(root);
                    xd.Save(file.FullName);
                    break;
                }
            }

        }

        /// <summary>
        /// 删除项目
        /// </summary>
        /// <param name="item"></param>
        public static void DelItem(Item item)
        {
           string fileName=String.Format("{0}{1}items/{2}.xml", Config.PhypicPath, Config.SavePath, item.outer_id);
           if (File.Exists(fileName))
           {
               File.Delete(fileName);
           }
           string fileName2 = String.Format("{0}{1}IncrementIndex.xml", Config.PhypicPath, Config.SavePath);

           XmlDocument xd = new XmlDocument();
           //重新加载
           xd.Load(fileName2);

           XmlNode modify = xd.SelectSingleNode("/root/modified");
           modify.InnerText = String.Format("{0:yyyy-MM-dd HH:mm:ss", DateTime.Now.ToUniversalTime());


           XmlNode ids = xd.SelectSingleNode("/root/item_ids");

           XmlNode xn=null;
           foreach (XmlNode _xn in ids.ChildNodes)
           {
               if (_xn.NodeType == XmlNodeType.Element && _xn.InnerText == item.outer_id.ToString())
               {
                   xn = _xn;
                   break;
               }
           }

           if (xn==null)
           {
               xn = xd.CreateElement("outer_id");

               XmlAttribute xat = xd.CreateAttribute("action");
               xat.Value = "delete";
               xn.Attributes.Append(xat);

               xn.InnerText = item.outer_id;
               ids.AppendChild(xn);
           }
           else
           {
               xn.Attributes["action"].Value = "delete";
           }

           xd.Save(fileName2);
        }

        private static XmlNode NewNode(XmlDocument xd, string nodeName, string nodeValue)
        {
            XmlNode xn = xd.CreateElement(nodeName);
            if (nodeValue != null)
            {
                xn.InnerText = nodeValue;
            }
            return xn;
        }

        private static void AppendNode(XmlDocument xd, XmlNode parentNode, string nodeName, string nodeValue)
        {
            parentNode.AppendChild(NewNode(xd, nodeName, nodeValue));
        }

        /// <summary>
        /// ETao监视器
        /// </summary>
        public static ETaoMoniter Moninter
        {
            get { return new ETaoMoniter(); }
        }


        /// <summary>
        /// 开启ETao服务 
        /// </summary>
        public static void StartService(string seller,GetItemsHandler hand,GetCatsHandler hand2)
        {
            //设置域名
            if (ETao.Config.Domain == "http://xxx.com")
            {
                ETao.Config.Domain = String.Format("http://{0}/", global::System.Web.HttpContext.Current.Request.Url.Host);
            }

            //设置账号
            ETao.Config.Seller = seller;

            //开启
            const int interval = 3600000;                 //时间间隔(1小时)


            new global::System.Threading.Thread(() =>
            {

                while (true)
                {
                    DateTime dt = DateTime.Now;
                    if ((dt - ETao.Config.LastBuildTime).Days >= 1)
                    {
                        ETao.Core.GenerateFullIndex(hand());
                        ETao.Core.Generate(hand2());

                        ETao.Config.LastBuildTime = dt;
                        global::System.Threading.Thread.Sleep(interval * 24);
                    }
                    else
                    {
                        global::System.Threading.Thread.Sleep(interval);
                    }
                }

            }).Start();

        }
    }
}
