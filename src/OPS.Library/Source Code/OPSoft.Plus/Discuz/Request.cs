/*
 * name      : Request
 * author    : newmin
 * date      : 2010/12/20
 * 
 */
namespace Ops.Plugin.Discuz
{
    using System;
    using System.Text;
    using System.Configuration;
    using System.Collections;
    using System.IO;
    using System.Net;
    using System.Web;
    using System.Text.RegularExpressions;
    using System.Collections.Specialized;

    public class Request
    {
        #region 对asp.net无用的
        /*
        /// <summary>
        /// 连接 UCenter 的方式
        /// mysql:MySQL 方式;空:远程方式
        /// </summary>
        public const string UC_CONNECT="mysql";
        /// <summary>
        ///UCenter 数据库主机
        /// </summary>
        public const string UC_DBHOST="192.168.1.82";
        /// <summary>
        ///UCenter 数据库用户名
        /// </summary>
        public const string UC_DBUSER="root";
        /// <summary>
        /// UCenter 数据库密码
        /// </summary>
        public const string UC_DBPW="";
        /// <summary>
        /// UCenter 数据库名称
        /// </summary>
        public const string UC_DBNAME="";
        /// <summary>
        /// UCenter 数据库字符集
        /// </summary>
        public const string UC_DBCHARSET="";
        /// <summary>
        /// UCenter 数据库表前缀
        /// </summary>
        public const string UC_DBTABLEPRE="";
        /// <summary>
        /// UCenter 数据库持久连接 0=关闭, 1=打开
        /// </summary>
        public const string UC_DBCONNECT="";
        */
        #endregion


        /// <summary>
        /// 与 UCenter 的通信密钥, 要与 UCenter 保持一致
        /// </summary>
        public static string UC_KEY;
        /// <summary>
        /// UCenter 服务端的 URL 地址
        /// </summary>
        public static string UC_API;
        /// <summary>
        /// UCenter 的字符集
        /// </summary>
        public static string UC_CHARSET;
        /// <summary>
        /// 当前应用的 ID
        /// </summary>
        public static string UC_APPID;
        public static bool UC_RecordLog;
        //匹配host的模式
        private static readonly string hostPattern;
        static Request()
        {
            UC_APPID = ConfigurationManager.AppSettings["uc_appid"];
            UC_KEY = ConfigurationManager.AppSettings["uc_key"];
            UC_API = ConfigurationManager.AppSettings["uc_api"];
            UC_CHARSET = ConfigurationManager.AppSettings["uc_charset"] ?? "gbk";
            UC_RecordLog = ConfigurationManager.AppSettings["uc_log"] == "True";

            hostPattern = "^(?=" + UC_API.Remove(UC_API.IndexOf("/", 7)).Replace("/", "\\/") + "|http:\\/\\/" + HttpContext.Current.Request.Url.Host + ")";
        }

        internal static object SendRequest(string path, string module, string action, string param)
        {
            //获取时间参数
            string time = ((DateTime.UtcNow.Ticks - new DateTime(0x7b2, 1, 1).Ticks) / 0x989680L).ToString();
            //获取agent参数
            string agent = Authcode.MD5(HttpContext.Current.Request.UserAgent);
            StringBuilder sb = new StringBuilder();
            sb.Append("agent=").Append(agent).Append("&time=")
                .Append(time).Append("&").Append(param);
            //需要先解码
            string input = HttpContext.Current.Server.UrlEncode(Authcode.DiscuzAuthcodeEncode(sb.ToString(), UC_KEY, 0));

            sb.Remove(0, sb.Length);
            sb.Append("m=").Append(module).Append("&a=").Append(action).Append("&inajax=2&appid=")
                .Append(UC_APPID).Append("&input=").Append(input);


            HttpWebRequest request = WebRequest.Create(UC_API + path) as HttpWebRequest;
            if (request == null) return "连接UCenter服务端失败!";
            request.Accept = "*/*";
            request.Method = "POST";
            request.UserAgent = HttpContext.Current.Request.UserAgent;
            request.ContentType = "application/x-www-form-urlencoded";
            request.Timeout = 30000;//30秒超时

            //发送请求
            byte[] data = Encoding.GetEncoding("GBK").GetBytes(sb.ToString());
            request.ContentLength = data.Length;
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(data, 0, data.Length);
            requestStream.Dispose();

            //获取响应
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    return sr.ReadToEnd();
                }
            }
            else
            {
                return "Error:" + response.StatusCode.ToString();
            }
        }
        /// <summary>
        /// 调用UCenter API
        /// </summary>
        /// <param name="api"></param>
        /// <returns></returns>
        public static object CallUCenterAPI(HttpContext context, IUCenterAPI api)
        {
            //判断请求的主机
            string referer = context.Request.Headers["referer"];
            if (!Regex.IsMatch(referer, hostPattern)) return "404";

            //因为包含+传递过来为" ",则需先转码再解码
            string query = context.Server.UrlDecode(context.Server.UrlEncode(context.Request["code"]));
            //等同于以下这句
            //string query = context.Server.UrlEncode(context.Request["code"]).Replace("%2b","+").Replace("%2f","/");

            System.Collections.Specialized.NameValueCollection parameters = HttpUtility.ParseQueryString
                (Authcode.DiscuzAuthcodeDecode(query, UC_KEY));

            #region 记录参数
            /*
            StringBuilder sb = new StringBuilder();
            sb.Append(query).Append("\r\n");
            sb.Append(Authcode.DiscuzAuthcodeDecode(query, UC_KEY));
            foreach (string s in parameters.Keys)
            {
                sb.Append("\r\n").Append(s).Append(":" + parameters[s]);
            }
            TraceLog.Record(sb.ToString());
            */
            #endregion
            #region 记录日志
            /*
            StringBuilder sb = new StringBuilder();
            
            using (Stream stream = context.Request.InputStream)
            {
                byte[] buffer = new byte[100];
                int count;
                if (stream.Length != 0)
                {
                    do
                    {
                        count = stream.Read(buffer, 0, buffer.Length);
                        sb.Append(Encoding.Default.GetString(buffer));
                    } while (count != 0);
                }
            }
            string streamContent = sb.ToString();

            sb.Remove(0, sb.Length);

            NameValueCollection _params = context.Request.Params;
           
            foreach (string a in _params.Keys)
            {
                sb.Append("&").Append(a).Append("=").Append(_params[a]);
            }

            TraceLog.Record(Authcode.DiscuzAuthcodeDecode(query, UC_KEY)+"\r\nMethod:"+context.Request.HttpMethod+
                       "\r\nParams:"+sb.ToString()+"\r\nStream:"+streamContent+context.Request["action"]);

            */
            #endregion

            switch (parameters["action"])
            {
                case "test":
                    TraceLog.Record("与UCenter建立通信成功!");
                    return "1";
                case "addfeed":
                    return api.AddFeed();
                case "deleteuser":
                    return api.DeleteUser(parameters["ids"]);

                case "synlogin":
                    api.SyncLogin(parameters["username"], parameters["password"]);return "";

                case "synlogout":
                     api.SyncLogout();return"";

                case "updatepw":
                    return api.Edit(parameters["username"], parameters["oldpw"], parameters["newpw"], parameters["email"], true);

                case "getcreditsettings":
                    break;

                case "gettag":
                    break;

                case "renameuser":
                    return api.RenameUser(int.Parse(parameters["uid"]), parameters["oldusername"], parameters["newusername"]);
             


                case "updateapps":
                    break;

                case "updatebadwords":
                    break;

                case "updateclient":
                    break;

                case "updatecredit":
                    break;

                case "updatecreditsettings":
                    break;

                case "updatehosts":
                    break;
            }
            return "";
        }

    }
}