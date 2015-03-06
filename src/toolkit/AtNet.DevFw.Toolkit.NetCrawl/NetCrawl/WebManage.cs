using System;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace AtNet.DevFw.Toolkit.NetCrawl
{
    public class WebManage : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {
        private static string subtitle = "-采集管理插件 Power by OPSoft";
        private static string ct_css = "<link rel=\"StyleSheet\" type=\"text/css\" href=\"?action=css\"/>";
        private static string navigator = WebManageResource.partial_navigator;

        protected HttpRequest request;
        protected HttpResponse response;
        private Collector director;

        public WebManage(Collector director)
        {
            this.director = director;
        }

        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            Begin_Request();

            string action = context.Request["action"];
            string httpMethod = context.Request.HttpMethod;

            //未指定动作，则显示欢迎页面
            if (String.IsNullOrEmpty(action))
            {
                Show_WelcomePage();
                return;
            }

            request = context.Request;
            response = context.Response;

            Type type = this.GetType();
            MethodInfo method = type.GetMethod(String.Format("{0}_{1}", action, httpMethod),
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (method == null)
            {
                context.Response.Write("执行出错");
                return;
            }
            context.Response.Write(method.Invoke(this, null) as string);
        }

        public virtual void Begin_Request()
        {
        }

        /// <summary>
        /// 加载样式表
        /// </summary>
        protected string Css_Get()
        {
            return WebManageResource.webmanagestyle;
        }

        protected string Show_WelcomePage()
        {
            return WebManageResource.welcome_page.Replace("${ct_css}", ct_css)
                .Replace("${navigator}", WebManageResource.partial_navigator);
        }

        protected string Help_Get()
        {
            return "文档整理中，请上我们官网<a href=\"http://www.ops.cc\">www.ops.cc</a>查询相关信息！";
        }

        /// <summary>
        /// 项目列表
        /// </summary>
        protected string List_Get()
        {
            string prjsListHtml,
                //项目列表Html
                propertyHtml;


            StringBuilder sb = new StringBuilder();
            StringBuilder propertyStr = new StringBuilder();


            Project[] projects = this.director.GetProjects();
            if (projects == null)
            {
                sb.Append("<li>暂无采集项目![<a href=\"?action=create\">点击添加</a>]</li>");
            }
            else
            {
                foreach (Project prj in projects)
                {
                    //项目属性
                    foreach (string key in prj.Rules)
                    {
                        propertyStr.Append("[").Append(key).Append("],");
                    }
                    if (propertyStr.Length == 0)
                    {
                        propertyHtml = "无";
                    }
                    else
                    {
                        propertyHtml = propertyStr.Remove(propertyStr.Length - 1, 1).ToString();
                        propertyStr.Remove(0, propertyStr.Length);
                    }

                    //项目基本信息
                    sb.Append("<div class=\"project\"><h2><strong>项目名称：")
                        .Append(prj.Name)
                        .Append("</strong>(编号：")
                        .Append(prj.Id)
                        .Append(")</h2><p class=\"details\"> 编码方式：").Append(prj.RequestEncoding).Append("<br />列表规则：")
                        .Append(prj.ListUriRule).Append("<br />页面规则：")
                        .Append(prj.PageUriRule).Append("<br />采集属性：").Append(propertyHtml)
                        .Append("</p><span class=\"project_operate\">[&nbsp;<a href=\"?action=invoke&projectId=")
                        .Append(prj.Id).Append("\">开始采集</a>&nbsp;]&nbsp;&nbsp;[&nbsp;<a href=\"?action=edit&projectId=")
                        .Append(prj.Id).Append("\">修改</a>&nbsp;]&nbsp;&nbsp;[&nbsp;<a href=\"?action=delete&projectId=")
                        .Append(prj.Id).Append("\">删除</a>&nbsp;]</span></div>");
                }
            }

            prjsListHtml = sb.ToString();

            return WebManageResource.project_list
                .Replace("${ct_css}", ct_css)
                .Replace("${subtitle}", subtitle)
                .Replace("${navigator}", WebManageResource.partial_navigator)
                .Replace("${listHtml}", prjsListHtml);
        }

        #region 创建项目

        protected string CreateProject_Get()
        {
            return WebManageResource.create_project.Replace("${ct_css}", ct_css)
                .Replace("${subtitle}", subtitle)
                .Replace("${navigator}", WebManageResource.partial_navigator);
        }

        protected string CreateProject_Post()
        {
            bool result; //新建项目是否成功

            Project project = new Project();
            project.Rules = new PropertyRule();

            string id = request.Form["id"],
                name = request.Form["name"],
                encoding = request.Form["encoding"],
                listRule = request.Form["listRule"],
                blockRule = request.Form["listBlockRule"],
                pageRule = request.Form["pageRule"],
                filterRule = request.Form["filterWordsRule"];


#if DEBUG
            response.Write(HttpContext.Current.Server.HtmlEncode(String.Format("ID:{0}<br />Name:{1}\r\nListRule:{2}\r\nListBlockRule:{3}\r\nPageRule:{4}\r\nFilterRule:{5}\r\nencoding:{6}",
                id, name, listRule, blockRule, pageRule, filterRule,encoding)));

            response.Write("<br />");
#endif

            if (String.IsNullOrEmpty(id) || String.IsNullOrEmpty(name))
            {
                return "<script>window.parent.tip('编号或名称不能为空！');</script>";
            }


            project.Id = id;
            project.Name = name;
            project.RequestEncoding = encoding;
            project.ListUriRule = listRule;
            project.ListBlockRule = blockRule;
            project.PageUriRule = pageRule;
            project.FilterWordsRule = filterRule;


            //添加属性并赋值
            //客户端属性与规则匹配：p1 <-> r1
            Regex propertyNameRegex = new Regex("^p(\\d+)$");
            string propertyIndex; //属性编号

            foreach (string key in request.Form)
            {
                if (propertyNameRegex.IsMatch(key))
                {
                    propertyIndex = propertyNameRegex.Match(key).Groups[1].Value;

                    //如果值不为空，则添加属性
                    if (request.Form[key] != String.Empty)
                    {
                        project.Rules.Add(request.Form[key], request.Form["r" + propertyIndex]);
                    }
                }
            }

            /*
            //输出添加到的属性
            foreach (string key in project.Rules)
            {
                response.Write(HttpContext.Current.Server.HtmlEncode(key + "->" + project.Rules[key]+"<br />"));
            }
             */

            result = this.director.CreateProject(project);

            //清除项目缓存
            this.director.ClearProjects();

            return result
                ? "<script>window.parent.tip('添加成功!');</script>"
                : "<script>window.parent.tip('项目已存在!');</script>";
        }

        #endregion

        #region 更新项目

        protected string Edit_Get()
        {
            string projectId = request.QueryString["projectId"];
            Project prj = this.director.GetProject(projectId);
            if (prj == null)
            {
                return WebManageResource.error.Replace("${ct_css}", ct_css)
                    .Replace("${subtitle}", subtitle)
                    .Replace("${navigator}", WebManageResource.partial_navigator)
                    .Replace("${msg}", "项目不存在");
            }

            StringBuilder sb = new StringBuilder();
            int i = 0;

            foreach (string key in prj.Rules)
            {
                ++i;
                sb.Append("<p><input type=\"text\" name=\"p").Append(i.ToString()).Append("\" value=\"")
                    .Append(key).Append("\"/><textarea name=\"r").Append(i.ToString()).Append("\" class=\"rulebox2\">")
                    .Append(prj.Rules[key]).Append("</textarea>")
                    .Append(
                        "<span class=\"removeProperty\">[&nbsp;<a href=\"javascript:;\" onclick=\"removeProperty(this)\">删除</a>&nbsp;]</span></p>");
            }

            return WebManageResource.update_project
                .Replace("${ct_css}", ct_css)
                .Replace("${subtitle}", subtitle)
                .Replace("${navigator}", WebManageResource.partial_navigator)
                .Replace("${id}", projectId)
                .Replace("${name}", prj.Name)
                .Replace("${encoding}", prj.RequestEncoding)
                .Replace("${listUriRule}", prj.ListUriRule)
                .Replace("${listBlockRule}", prj.ListBlockRule)
                .Replace("${pageUriRule}", prj.PageUriRule)
                .Replace("${filterWordsRule}", prj.FilterWordsRule)
                .Replace("${propertiesHtml}", sb.ToString());
        }

        protected string Edit_Post()
        {
            string projectId = request.QueryString["projectId"];


            bool result; //编辑项目是否成功

            Project project = new Project();
            project.Rules = new PropertyRule();

            string id = request.Form["id"],
                name = request.Form["name"],
                encoding = request.Form["encoding"],
                listRule = request.Form["listRule"],
                blockRule = request.Form["listBlockRule"],
                pageRule = request.Form["pageRule"],
                filterRule = request.Form["filterWordsRule"];


#if DEBUG
            response.Write(HttpContext.Current.Server.HtmlEncode(String.Format("ID:{0}<br />Name:{1}\r\nListRule:{2}\r\nListBlockRule:{3}\r\nPageRule:{4}\r\nFilterRule:{5}\r\nencoding:{6}",
                id, name, listRule, blockRule, pageRule, filterRule, encoding)));

            response.Write("<br />");
#endif

            if (String.IsNullOrEmpty(id) || String.IsNullOrEmpty(name))
            {
                return "<script>alert('编号或名称不能为空！');</script>";
            }


            project.Id = id;
            project.Name = name;
            project.RequestEncoding = encoding;
            project.ListUriRule = listRule;
            project.ListBlockRule = blockRule;
            project.PageUriRule = pageRule;
            project.FilterWordsRule = filterRule;


            //添加属性并赋值
            //客户端属性与规则匹配：p1 <-> r1
            Regex propertyNameRegex = new Regex("^p(\\d+)$");
            string propertyIndex; //属性编号

            foreach (string key in request.Form)
            {
                if (propertyNameRegex.IsMatch(key))
                {
                    propertyIndex = propertyNameRegex.Match(key).Groups[1].Value;

                    //如果值不为空，则添加属性
                    if (request.Form[key] != String.Empty)
                    {
                        project.Rules.Add(request.Form[key], request.Form["r" + propertyIndex]);
                    }
                }
            }

            /*
            //输出添加到的属性
            foreach (string key in project.Rules)
            {
                response.Write(HttpContext.Current.Server.HtmlEncode(key + "->" + project.Rules[key]+"<br />"));
            }
             */

            result = this.director.SaveProject(projectId, project);

            //清除项目缓存
            this.director.ClearProjects();

            return result
                ? "<script>window.parent.tip('修改成功!');</script>"
                : "<script>window.parent.tip('项目编号已存在!');</script>";
        }

        #endregion

        #region 删除项目

        protected string Delete_Get()
        {
            string projectId = request.QueryString["projectId"];
            string confirm = request["confirm"];

            string msg; //返回信息


            Project prj = this.director.GetProject(projectId);
            if (prj == null)
            {
                msg = "<span style=\"color:red\">项目不存在!\"></span>";
            }


            if (String.IsNullOrEmpty(confirm))
            {
                msg = String.Format(
                    "您确定删除项目：<span style=\"color:red\">{0}</span>&nbsp;吗？<br /><a href=\"?action=delete&confirm=ok&projectid={1}\">确定</a>&nbsp;<a href=\"?action=list\">取消</a>"
                    , prj.Name, projectId);
            }
            else
            {
                msg = "项目删除成功！";

                this.director.RemoveProject(prj);

                //更新项目缓存
                this.director.ClearProjects();
            }


            return WebManageResource.delete_project
                .Replace("${ct_css}", ct_css)
                .Replace("${subtitle}", subtitle)
                .Replace("${navigator}", WebManageResource.partial_navigator)
                .Replace("${msg}", msg);
        }

        #endregion

        /********************************************************
         * 
         *  采集说明:
         *  
         *  1.开始采集时读取继承类返回的Html代码，供重写Invoke方法读取某些参数！
         *  
         *  2.客户端通过设置HiddenField  [typeid]的值，来识别采集单页或是列表
         *    [typeid]:1:采集单页,2:传递列表页参数采集,3:输入列表页参数采集
         *    
         *  3.通过识别采集方式，来调用继承类的采集处理代码。执行完毕，想客户端传送
         *    采集完成指令!
         * 
         */

        /// <summary>
        /// 开始执行采集
        /// </summary>
        protected string Invoke_Get()
        {
            string projectId = request.QueryString["projectId"];

            Project prj = this.director.GetProject(projectId);
            if (prj == null)
            {
                return "<script>window.parent.tip('项目不存在!');</script>";
            }

            return WebManageResource.invoke_collect
                .Replace("${ct_css}", ct_css)
                .Replace("${subtitle}", subtitle)
                .Replace("${navigator}", WebManageResource.partial_navigator)
                .Replace("${customHtml}", Return_InvokePageHtml())
                .Replace("${pageUriRule}", HttpContext.Current.Server.HtmlEncode(prj.PageUriRule))
                .Replace("${listUriRule}", HttpContext.Current.Server.HtmlEncode(prj.ListUriRule)
                    .Replace("{0}", "<span style=\"color:red\">{0}</span>"))
                .Replace("${listBlockRule}", HttpContext.Current.Server.HtmlEncode(prj.ListBlockRule));
        }

        protected string Invoke_Post()
        {
            string typeID = request.Form["ct_typeid"];
            string projectId = request.QueryString["projectId"];

            //执行采集返回的数据
            string invoke_returnData = String.Empty;

            Project prj = this.director.GetProject(projectId);
            if (prj == null)
            {
                return "<script>window.parent.tip('项目不存在!');</script>";
            }

            switch (typeID)
            {
                case "1":
                    invoke_returnData = Invoke_SinglePage(prj, request.Form["singlePageUri"]);
                    break;
                case "2":
                    invoke_returnData = Invoke_ListPage(prj, (object) request.Form["listPageParameter"]);
                    break;
                case "3":
                    invoke_returnData = Invoke_ListPage(prj, request.Form["listPageUri"]);
                    break;
            }

            return "<script>window.parent.invoke_compelete('" + invoke_returnData + "')</script>";
        }

        /// <summary>
        /// 返回一段Html代码，并呈现在采集页面上
        /// 如：返回一段分类的标签，并在采集中读取选中的分类，从而实现将采集的内容发布到指定分类中
        /// </summary>
        public virtual string Return_InvokePageHtml()
        {
            return "您好，欢迎使用采集系统!<br />";
        }


        /// <summary>
        /// 采集单页,并返回提示数据
        /// </summary>
        /// <param name="project"></param>
        /// <param name="pageUri"></param>
        public virtual string Invoke_SinglePage(Project project, string pageUri)
        {
            project.InvokeSingle(pageUri, dp =>
            {
#if DEBUG
                saveLog("\r\n----------------------------------------\r\n标题："+dp["title"] + "<br />\r\n内容：" + dp["content"]+"\r\n");
#endif
            });

            //重置计数
            project.ResetState();

            return null;
        }

        /// <summary>
        ///根据列表页的地址采集页面,并返回提示数据
        /// </summary>
        /// <param name="project"></param>
        /// <param name="listPageUri"></param>
        /// <returns></returns>
        public virtual string Invoke_ListPage(Project project, string listPageUri)
        {
            string returnData;

            int i = 0;
            object obj = String.Empty;

            project.UseMultiThread = true;

            project.InvokeList(listPageUri, dp =>
            {
                lock (obj)
                {
                    ++i;
#if DEBUG
                    saveLog(String.Format("采集到第{0}条->{1}", i, dp["title"]));
#endif
                }
            });

            returnData = String.Format("任务总数:{0},成功：{1},失败:{2}", project.State.TotalCount, project.State.SuccessCount,
                project.State.FailCount);

            //重置计数
            project.ResetState();

            return returnData;
        }

        /// <summary>
        /// 向列表页规则传递参数，并返回提示数据
        /// </summary>
        /// <param name="project"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public virtual string Invoke_ListPage(Project project, object parameter)
        {
            string returnData;

            int i = 0;
            object obj = String.Empty;

            project.UseMultiThread = true;

            project.InvokeList(parameter, dp =>
            {
                lock (obj)
                {
                    ++i;
#if DEBUG
                    saveLog(String.Format("采集到第{0}条->{1}", i, dp["title"]));
#endif
                }
            });

            returnData = String.Format("任务总数:{0},成功：{1},失败:{2}", project.State.TotalCount, project.State.SuccessCount,
                project.State.FailCount);

            //重置计数
            project.ResetState();

            return returnData;
        }

        protected void saveLog(string str)
        {
            using (
                System.IO.StreamWriter sr =
                    new System.IO.StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "collection.log", true))
            {
                sr.WriteLine(str);
                sr.Flush();
                sr.Dispose();
            }
        }

        /// <summary>
        /// 向客户端发送提示信息
        /// </summary>
        /// <param name="msg"></param>
        protected void SendTipMessage(string msg)
        {
            response.Write(String.Format("<script type=\"text/javascript\">window.parent.tip('{0}');</script>", msg));
            response.End();
        }
    }
}