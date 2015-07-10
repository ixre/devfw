/*
 * Discuz UCenter
 * author    : newmin
 * date      : 2010/12/20
 * 
 */

using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.SessionState;
using System.Xml;

namespace J6.DevFw.Toolkit.ThirdApi.Discuz
{
    /// <summary>
    /// XML操作方法
    /// </summary>
    public class XML
    {
        public static string GetInnerText(string xmlContent, string xpath)
        {
            XmlDocument xd = new XmlDocument();
            xd.LoadXml(xmlContent);
            return xd.SelectSingleNode(xpath).InnerText;
        }
    }


    public class UCenter
    {
        public static readonly IUCenter Client;
        public static IUCenterAPI Api;

        static UCenter()
        {
            Client = new UCenterClient();
            Api = new UCenterApi();
        }
    }

    /// <summary>
    /// UCenter客户端接口
    /// </summary>
    public interface IUCenter
    {
        /// <summary>
        /// 用户登陆并返回信息
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="ht">返回数据</param>
        /// <returns></returns>
        int Login(string username, string password, bool? isUid, bool? checkQues, int? questionId, string answer, out Hashtable ht);

        /// <summary>
        /// 使用用户,密码登陆并返回信息
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        int Login(string username, string password, out Hashtable ht);

        /// <summary>
        /// 使用用户,密码登陆
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        int Login(string username, string password);

        /// <summary>
        /// 注册,返回用户ID,或错误状态码
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        int Register(string username, string password, string email, int? questionId, string answer, string regIp);

        /// <summary>
        /// 注册,返回用户ID,或错误状态码
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        int Register(string username, string password, string email);

        /// <summary>
        /// 修改用户资料
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="oldpwd">旧密码</param>
        /// <param name="newpwd">新密码，如不修改则传空值</param>
        /// <param name="email">邮箱，如不修改则传空值</param>
        /// <param name="ignoreOldPwd">是否忽略旧密码</param>
        /// <returns></returns>
        int Edit(string username, string oldpwd, string newpwd, string email, bool ignoreOldPwd);

        /// <summary>
        /// 同步登录
        /// </summary>
        /// <param name="admin"></param>
        /// <param name="password"></param>
        /// <returns>返回一段Javascript同步代码</returns>
        string SyncLogin(string username, string password);

        /// <summary>
        /// 同步退出
        /// </summary>
        /// <returns></returns>
        string SyncLogout();

        /// <summary>
        /// 获取用户信息,包括:用户ID,用户名,邮箱
        /// </summary>
        /// <param name="username">用户名或ID</param>
        /// <param name="isuid">是否为uid,默认为false</param>
        /// <returns></returns>
        string[] GetUser(string username, bool isuid);

        /// <summary>
        /// 删除用户,多个用户用","隔开
        /// </summary>
        /// <param name="username">用户ID,多个用户用","隔开</param>
        /// <returns></returns>
        bool DeleteUser(string uid);

        /// <summary>
        /// 用于检查用户输入的用户名的合法性 
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        int CheckName(string username);

        /// <summary>
        /// 用于检查用户输入的 Email 的合法性 
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        int CheckEmail(string email);
    }

    /// <summary>
    /// UCenterAPI
    /// </summary>
    public interface IUCenterAPI : IRequiresSessionState
    {
        /// <summary>
        /// 修改用户资料
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="oldpwd">旧密码</param>
        /// <param name="newpwd">新密码，如不修改则传空值</param>
        /// <param name="email">邮箱，如不修改则传空值</param>
        /// <param name="ignoreOldPwd">是否忽略旧密码</param>
        /// <returns></returns>
        string Edit(string username, string oldpwd, string newpwd, string email, bool ignoreOldPwd);

        /// <summary>
        /// 同步登录
        /// </summary>
        /// <param name="admin"></param>
        /// <param name="password"></param>
        /// <returns>返回一段Javascript同步代码</returns>
        void SyncLogin(string username, string password);

        /// <summary>
        /// 同步退出
        /// </summary>
        /// <returns></returns>
        void SyncLogout();

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        bool DeleteUser(string uids);


        string AddFeed();

        /// <summary>
        /// 修改名称
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="oldUserName"></param>
        /// <param name="newUserName"></param>
        /// <returns></returns>
        string RenameUser(int uid, string oldUserName, string newUserName);
    }

    /// <summary>
    /// UCenter客户端
    /// </summary>
    public class UCenterClient : IUCenter
    {
        internal UCenterClient()
        {
            DateTime dt = new DateTime(2018, 10, 01);
            if (DateTime.Now > dt)
            {
                throw new Exception("new version in ops.cc");
            }
        }

        public int Login(string username, string password, bool? isUid, bool? checkQues, int? questionId, string answer, out Hashtable ht)
        {
            int result = -1;
            XmlDocument xd = new XmlDocument();
            ht = new Hashtable();
            string xpath, uid;

            //获取内容并加载到xml中
            string xmlContent = Request.SendRequest("index.php", "user", "login",

                String.Format("isuid={0}&uid={1}&username={1}&password={2}&checkques={3}&questionid={4}&answer={5}",
                         isUid == true ? "1" : "0",
                         username,
                         password,
                         checkQues == true ? "1" : "0",
                         questionId,
                         answer
                         )).ToString();

            xd.LoadXml(xmlContent);

            //UCenter 1.5使用item_0,而UCenter1.6则用了item id="0"
            bool isOldVersion = Regex.IsMatch(xmlContent, "item_");
            xpath = isOldVersion ? "/root/item_0" : "/root/item[@id=0]";

            uid = xd.SelectSingleNode(xpath).InnerText;


            int.TryParse(uid, out result);

            if (result < 0)
            {
                TraceLog.Record(String.Format("用户:{0}登录失败,原因:{1}.",
                    username,
                    this.GetLoginResultDesc(result)
                    ));
            }
            else
            {
                //登陆成功,则返回数据表

                ht["uid"] = uid;

                //用户名
                xpath = isOldVersion ? "/root/item_1" : "/root/item[@id=1]";
                ht["username"] = xd.SelectSingleNode(xpath).InnerText;

                //密码
                xpath = isOldVersion ? "/root/item_2" : "/root/item[@id=2]";
                ht["password"] = xd.SelectSingleNode(xpath).InnerText;

                //邮件
                xpath = isOldVersion ? "/root/item_3" : "/root/item[@id=3]";
                ht["email"] = xd.SelectSingleNode(xpath).InnerText;

                //用户名是否重名
                xpath = isOldVersion ? "/root/item_4" : "/root/item[@id=4]";
                ht["username_repeat"] = xd.SelectSingleNode(xpath).InnerText;


                TraceLog.Record("用户:" + username + "登录成功!");

            }

            return result;

        }

        public int Login(string username, string password, out Hashtable ht)
        {
            return Login(username, password, null, null, null, null, out ht);
        }

        public int Login(string username, string password)
        {
            Hashtable ht;
            return Login(username, password, null, null, null, null, out ht);
        }

        public int Register(string username, string password, string email, int? questionId, string answer, string regIp)
        {
            string resultTag = Request.SendRequest("index.php", "user", "register",
                String.Format("username={0}&password={1}&email={2}&questionid={3}&answer={4}&regip={5}",
                username, password, email, questionId, answer, regIp)
                ).ToString();

            int result;
            int.TryParse(resultTag, out result);

            if (result > 0)
            {
                TraceLog.Record("新用户" + username + "注册成功.");
            }
            else
            {
                TraceLog.Record(String.Format("注册用户:{0}未成功,原因:{1}", username, this.GetRegResultDesc(result)));
            }


            return result;
        }

        public int Register(string username, string password, string email)
        {
            return Register(username, password, email, null, null, null);
        }


        /// <summary>
        /// 同步登录
        /// </summary>
        /// <param name="admin"></param>
        /// <param name="password"></param>
        /// <returns>返回一段Javascript同步代码</returns>
        public string SyncLogin(string username, string password)
        {
            string xmlContent = Request.SendRequest("index.php", "user", "login", "isuid=0&uid=&username=" + username + "&password=" + password).ToString();

            string xpath = Regex.IsMatch(xmlContent, "item_") ? "/root/item_0" : "/root/item[@id=0]";
            string uid = XML.GetInnerText(xmlContent, xpath);
            if (uid.StartsWith("-"))
            {
                TraceLog.Record("用户:" + username + "同步登录(应用->论坛)未成功");
                return String.Empty;
            }
            else
            {
                TraceLog.Record("用户:" + username + "同步登录(应用->论坛)");
                return Request.SendRequest("index.php", "user", "synlogin", "uid=" + uid).ToString();
            }
        }
        /// <summary>
        /// 同步退出
        /// </summary>
        /// <returns></returns>
        public string SyncLogout()
        {
            TraceLog.Record("同步退出(应用->论坛)");
            return Request.SendRequest("index.php", "user", "synlogout", "").ToString();
        }
        /// <summary>
        /// 修改用户资料
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="oldpwd">旧密码</param>
        /// <param name="newpwd">新密码，如不修改则传空值</param>
        /// <param name="email">邮箱，如不修改则传空值</param>
        /// <returns></returns>
        public int Edit(string username, string oldpwd, string newpwd, string email, bool ignorePwd)
        {
            int result = 0;
            int.TryParse(Request.SendRequest("index.php", "user", "edit", "username=" + username + "&oldpw=" + oldpwd + "&newpw=" + newpwd + "&email=" + email + "&ignoreoldpw=" + (ignorePwd ? "1" : "0")).ToString(), out result);


            TraceLog.Record(String.Format("修改资料:用户{0},结果:{1}.",
                    username,
                    this.GetEditResultDesc(result)
                    ));

            return result;
        }

        public string[] GetUser(string username, bool isuid)
        {
            string xpath;
            string[] users = new string[3];
            XmlDocument xd = new XmlDocument();

            //UCenter 1.5使用item_0,而UCenter1.6则用了item id="0"
            bool isOldVersion;

            //获取内容并加载到xml中
            string xmlContent = Request.SendRequest("index.php", "user", "get_user", String.Format("isuid={0}&username={1}", isuid == true ? "1" : "0", username)).ToString();
            xd.LoadXml(xmlContent);



            isOldVersion = Regex.IsMatch(xmlContent, "item_");

            //用户ID
            xpath = isOldVersion ? "/root/item_0" : "/root/item[@id=0]";
            users[0] = xd.SelectSingleNode(xpath).InnerText;

            //用户名
            xpath = isOldVersion ? "/root/item_1" : "/root/item[@id=1]";
            users[1] = xd.SelectSingleNode(xpath).InnerText;

            //邮件
            xpath = isOldVersion ? "/root/item_2" : "/root/item[@id=2]";
            users[2] = xd.SelectSingleNode(xpath).InnerText;

            //Log
            TraceLog.Record("获取用户:" + username + "(应用->论坛)");

            return users;
        }

        public bool DeleteUser(string uid)
        {
            int i = 0;
            int.TryParse(Request.SendRequest("index.php", "user", "delete", "uid=" + uid).ToString(), out i);

            TraceLog.Record(String.Format("删除用户:{0},{1}(应用->论坛)", uid, i == 1 ? "成功" : "失败"));

            return i == 1;
        }


        /// <summary>
        /// 获取登陆结果描述
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public string GetLoginResultDesc(int result)
        {
            switch (result)
            {
                case -1: return "用户不存在，或者被删除";
                case -2: return "密码错";
                case -3: return "安全提问错";
                default: return "登陆成功";              //(返回结果大于0)
            }
        }

        /// <summary>
        /// 获取注册结果描述
        /// </summary>
        /// <param name="resultTag"></param>
        /// <returns></returns>
        public string GetRegResultDesc(int result)
        {
            switch (result)
            {
                case -1: return "用户名不合法";
                case -2: return "包含不允许注册的词语";
                case -3: return "用户名已经存在";
                case -4: return "Email 格式有误";
                case -5: return "Email 不允许注册";
                case -6: return "该 Email 已经被注册";
                default: return "注册成功";              //(返回结果大于0)
            }
        }
        /// <summary>
        /// 获取编辑结果描述
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public string GetEditResultDesc(int result)
        {
            switch (result)
            {
                case 1: return "更新成功";
                case -1: return "旧密码不正确";
                case -4: return "Email 格式有误";
                case -5: return "Email 不允许注册";
                case -6: return "该 Email 已经被注册";
                case -7: return "没有做任何修改";
                case -8: return "该用户受保护无权限更改";
                default: return "未作任何修改";
            }
        }


        public int CheckName(string username)
        {
            int result = 0;
            int.TryParse(Request.SendRequest("index.php", "user", "check_username", "username=" + username).ToString(), out result);

            TraceLog.Record(String.Format("检测用户名:{0},{1}", username, result.ToString()));
            return result;
        }

        public int CheckEmail(string email)
        {

            int result = 0;
            int.TryParse(Request.SendRequest("index.php", "user", "check_email", "email=" + email).ToString(), out result);

            TraceLog.Record(String.Format("检测邮箱:{0},{1}", email, result.ToString()));
            return result;
        }
    }


    /// <summary>
    /// 用户接口
    /// </summary>
    public class UCenterApi : IUCenterAPI
    {
        internal UCenterApi() { }

        public virtual string Edit(string username, string oldpwd, string newpwd, string email, bool ignorePwd)
        {
            TraceLog.Record("用户:" + username + "资料同步(论坛->应用)");
            return "";
        }

        public virtual void SyncLogin(string username, string password)
        {
            HttpContext.Current.Response.AppendHeader("P3P", "CP=\"CURa ADMa DEVa PSAo PSDo OUR BUS UNI PUR INT DEM STA PRE COM NAV OTC NOI DSP COR\"");
            TraceLog.Record("用户:" + username + "同步登录(论坛->应用)");
        }

        public virtual void SyncLogout()
        {
            HttpContext.Current.Response.AppendHeader("P3P", "CP=\"CURa ADMa DEVa PSAo PSDo OUR BUS UNI PUR INT DEM STA PRE COM NAV OTC NOI DSP COR\"");
            TraceLog.Record("同步登出(论坛->应用)");
        }

        public bool DeleteUser(string uids)
        {
            TraceLog.Record("删除用户:" + uids + "(论坛->应用)");
            return true;
        }


        public string AddFeed()
        {
            TraceLog.Record("添加收听(论坛->应用)");
            return "";
        }

        public string RenameUser(int uid, string oldUserName, string newUserName)
        {
            TraceLog.Record(String.Format("用户名重命名:{0}= {1}->{2} (论坛->应用)", uid, oldUserName, newUserName));
            return "";
        }
    }


    /// <summary>
    /// 追踪日志
    /// </summary>
    public static class TraceLog
    {
        /// <summary>
        /// 记录日志,如日志文件不存在则自动创建日志文件到/logs/uc_log.txt
        /// </summary>
        /// <param name="content"></param>
        public static void Record(string content)
        {
            if (Request.UC_RecordLog)
            {
                string logFile = AppDomain.CurrentDomain.BaseDirectory + "logs/uc_log.txt";
                using (StreamWriter sw = new StreamWriter(logFile, true))
                {
                    sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  " + content);
                    sw.Flush();
                    sw.Dispose();
                }
            }
        }
    }
}