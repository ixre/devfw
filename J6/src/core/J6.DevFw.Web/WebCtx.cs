using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Web;
using J6.DevFw.Framework.Web.Cache;
using J6.DevFw.PluginKernel.Web;
using J6.DevFw.Web.Cache;
using J6.DevFw.Web.Cache.Compoment;
using J6.DevFw.Web.Plugin;

namespace J6.DevFw.Web
{
    /// <summary>
    /// 应用程序上下文
    /// </summary>
    public class WebCtx
    {

        /// <summary>
        /// 错误日志文件
        /// </summary>
        public static string ErrorFilePath;

        private const string KeyCtxDomain = "_ctx_domain";


        /// <summary>
        /// 是否作为虚拟目录运行
        /// </summary>
        public bool IsVirtualDirectoryRunning = false;

        public static global::System.Web.HttpContext Context
        {
            get { return global::System.Web.HttpContext.Current; }
        }


        /// <summary>
        /// 上下文来源
        /// </summary>
        public object Source { get; set; }

        public HttpResponse Response { get { return Context.Response; } }
        public HttpRequest Request { get { return Context.Request; } }


        static WebCtx()
        {
            ErrorFilePath = AppDomain.CurrentDomain.BaseDirectory + "tmp/logs/error.log";
        }

        /// <summary>
        /// 站点域名
        /// </summary>
        private string _siteDomain;
        private PageDataItems _dataItems;
        private string _staticDomain;
        private string _resouceDomain;
        private string _host;
        private string _siteAppPath;
        private static IPluginApp _extends;

        /// <summary>
        /// 运行时发生
        /// </summary>
        public static event FwHandler OnBeginRequest;


        /// <summary>
        /// 当前的Host,包含端口，如：www.ops.cc:8080
        /// </summary>
        public string Host
        {
            get
            {
                var context = Context;
                if (this._host == null)
                {
                    this._host = String.Format("{0}{1}", context.Request.Url.Host,
                        context.Request.Url.Port != 80 ? ":" + context.Request.Url.Port.ToString() : "");
                }
                return this._host;
            }
        }

        /// <summary>
        /// 系统应用程序目录
        /// </summary>
        public string ApplicationPath
        {
            get { return Context.Request.ApplicationPath; }
        }

        /// <summary>
        /// 插件扩展
        /// </summary>
        public static IPluginApp Plugin
        {
            get
            {
                if (_extends == null)
                {
                    var handler = new WebPluginHandleProxy<System.Web.HttpContext>();
                    _extends = new WebPluginApp(handler);
                }
                return _extends;
            }
        }


        public static ICache Cache
        {
            get { return CacheFactory.Sington; }
        }

//        /// <summary>
//        /// 域名
//        /// </summary>
//        public string SiteDomain
//        {
//            get
//            {
//                if (_siteDomain == null)
//                {
//                    string host = String.Format("{0}{1}", context.Request.Url.Host,
//                        context.Request.Url.Port != 80 ? ":" + context.Request.Url.Port.ToString() : "");
//
//                    _siteDomain= String.Format("http://{0}{1}{2}",
//                        host,
//                        this.ApplicationPath=="/"?"":this.ApplicationPath,
//                        this.SiteAppPath=="/"?"/":this.SiteAppPath+"/"
//                        );
//
////                    this._siteDomain = this.CurrentSite.FullDomain;
////
////                    if (this._siteDomain.IndexOf("#") != -1)
////                    {
////                        this._siteDomain = this._siteDomain.Replace(
////                             "#", host);
////                    }
//                }
//                return _siteDomain;
//            }
//        }

//        /// <summary>
//        /// 资源域
//        /// </summary>
//        public string ResourceDomain
//        {
//            get
//            {
//                if (this._resouceDomain == null)
//                {
//                    //RES DOMAIN
//                    if (this.IsVirtualDirectoryRunning)
//                    {
//                        this._resouceDomain = String.Empty;
//                    }
//                }
//                return this._resouceDomain ?? (this._resouceDomain = this.SiteDomain);
//            }
//        }
//
//        /// <summary>
//        /// 静态资源域
//        /// </summary>
//        public string StaticDomain
//        {
//            get
//            {
//                if (this._staticDomain == null)
//                {
//                    if (Settings.SERVER_STATIC_ENABLED && Settings.SERVER_STATIC.Length != 0)
//                    {
//                        this._staticDomain = String.Concat("http://", Settings.SERVER_STATIC, "/");
//                    }
//                    else
//                    {
//                        this._staticDomain = this.ResourceDomain == String.Empty ? "/" : this.ResourceDomain;
//                    }
//                }
//
//                return this._staticDomain;
//            }
//        }
        /// <summary>
        /// 数据项
        /// </summary>
        public PageDataItems Items
        {
            get
            {
                if (_dataItems == null)
                {
                    _dataItems = new PageDataItems();
                }
                return _dataItems;
            }
        }
        public static string Domain
        {
            get
            {
                HttpContext context = Context;
                String domain = context.Items[KeyCtxDomain] as String;
                if (domain == null)
                {
                    HttpRequest request = Context.Request;
                    String applicationPath = request.ApplicationPath;
                    domain = String.Format("{0}://{1}{2}{3}",
                        request.Url.Scheme,
                        request.Url.Host,
                        context.Request.Url.Port != 80 ? ":" + request.Url.Port.ToString() : "",
                        applicationPath == "/" ? "" : applicationPath
                        );
                    context.Items[KeyCtxDomain] = domain;
                }

                return domain;
            }
            set { Context.Items[KeyCtxDomain] = value; }
        }


        public static void SaveErrorLog(Exception exception)
        {
            lock (ErrorFilePath)
            {
                HttpRequest req = Context.Request;

                if (!File.Exists(ErrorFilePath))
                {
                    string dir = AppDomain.CurrentDomain.BaseDirectory + "tmp/logs";
                    if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                    File.Create(ErrorFilePath).Close();
                }
                Context.Response.Write((File.Exists(ErrorFilePath).ToString()));
                using (FileStream fs = new FileStream(ErrorFilePath, FileMode.Append, FileAccess.Write))
                {
                    StreamWriter sw = new StreamWriter(fs);
                    StringBuilder sb = new StringBuilder();

                    sb.Append("---------------------------------------------------------------------\r\n")
                        .Append("[错误]：IP:").Append(req.UserHostAddress)
                        .Append("\t时间：").Append(DateTime.Now.ToString())
                        .Append("\r\n[信息]：").Append(exception.Message)
                        .Append("\r\n[路径]：").Append(req.Url.PathAndQuery)
                        .Append("  -> 来源：").Append(req.Headers["referer"] ?? "无")
                        .Append("\r\n[堆栈]：").Append(exception.StackTrace)
                        .Append("\r\n\r\n");

                    sw.Write(sb.ToString());

                    sw.Flush();
                    sw.Dispose();
                    fs.Dispose();
                }
            }
        }


//        public bool CheckSiteState()
//        {
//            if (this.CurrentSite.State == SiteState.Normal)
//            {
//                return true;
//            }
//            else if (this.CurrentSite.State == SiteState.Closed)
//            {
//                this.RenderNotfound();
//            }
//            else if (this.CurrentSite.State == SiteState.Paused)
//            {
//                this.Render("<h1 style=\"color:red;text-align:center;font-size:16px;padding:20px\">网站维护中,暂停访问！</h1>");
//                //this.RenderNotfound("<h1 style=\"color:red\">网站维护中,暂停访问！</h1>");
//            }
//            return false;
//        }

        /// <summary>
        /// 检查或设置客户端缓存(后台启用缓存并设时间>0)
        /// </summary>
        /// <returns></returns>
        public bool CheckAndSetClientCache()
        {
            if (WebConf.Opti_ClientCache && WebConf.Opti_ClientCacheSeconds > 0)
            {
                if (CacheUtil.CheckClientCacheExpires(WebConf.Opti_ClientCacheSeconds))
                {
                    CacheUtil.SetClientCache(this.Response, WebConf.Opti_ClientCacheSeconds);
                }
                else
                {
                    return false;
                }
            }
            return true;
        }


        /// <summary>
        /// 检查或设置客户端缓存(自定义时间,单位：秒)
        /// </summary>
        /// <returns></returns>
        public bool CheckAndSetClientCache(int maxAge)
        {
            if (maxAge > 0)
            {
                if (CacheUtil.CheckClientCacheExpires(maxAge))
                {
                    CacheUtil.SetClientCache(this.Response, maxAge);
                }
                else
                {
                    return false;
                }
            }
            return true;
        }


        /// <summary>
        /// 呈现
        /// </summary>
        /// <param name="html"></param>
        public void Render(string html)
        {
            HttpResponse response = this.Response;

            response.Write(html);

            //GZip压缩
            if (WebConf.Opti_SupportGZip)
            {
                response.Filter = new GZipStream(response.Filter, CompressionMode.Compress);
                response.AddHeader("Content-Encoding", "gzip");
            }
            /*
            else
            {
            	response.Filter=new DeflateStream(response.Filter,CompressionMode.Compress);
                response.AddHeader("Content-Encoding", "deflate");
            }*/

            response.AddHeader("X-AspNet-Version", String.Format("J6.DevFw - v{0}", FwCtx.Version.GetVersion()));
            response.AddHeader("Support-URL", "github.com/J6/devfw");
        }

//
//        /// <summary>
//        /// 显示400页面
//        /// </summary>
//        /// <returns></returns>
//        public void RenderNotfound()
//        {
//            this.RenderNotfound("File not found!", null);
//        }
//
//        /// <summary>
//        /// 显示400页面
//        /// </summary>
//        /// <returns></returns>
//        public void RenderNotfound(string message, TemplatePageHandler handler)
//        {
//            Response.StatusCode = 404;
//
//            string html = null;
//            try
//            {
//                TemplatePage tpl = new TemplatePage(String.Format("/{0}/not_found", this.CurrentSite.Tpl));
//                if (handler != null)
//                {
//                    handler(tpl);
//                }
//                tpl.Render();
//                return;
//            }
//            catch
//            {
//                html = "File not found!";
//            }
//
//            Response.Write(html);
//        }
//
//        public string ComposeUrl(string url)
//        {
//            if (url.StartsWith("/"))
//                throw new ArgumentException("URL不能以\"/\"开头!");
//
//            return String.Concat(this.SiteDomain, url);
//        }
    }
}