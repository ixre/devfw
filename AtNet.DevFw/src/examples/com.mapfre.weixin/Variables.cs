
namespace Com.Plugin
{
    internal class Variables
    {
        public static string Token;

        public static string AppId;

        public static string AppSecret;

        /// <summary>
        /// 解密字符串
        /// </summary>
        public static string AppEncodeString;
        public static string ApiDomain;         //
        public static string MenuButtons;       //自定义菜单
        public static string WxWelcomeMessage;    //微信欢迎文字
        public static string WxEnterMessage; //微信进入文字
        public static string WxDefaultResponseMessage;  //微信默认自动回复消息
        public static bool DebugMode = false;

        #region 按钮模板

        /*
         {
     "button":[
     {    
          "type":"click",
          "name":"今日歌曲",
          "key":"V1001_TODAY_MUSIC"
      },
      {
           "name":"菜单",
           "sub_button":[
           {    
               "type":"view",
               "name":"搜索",
               "url":"http://www.soso.com/"
            }
           ]
       }]
 }
         */

        #endregion
    }
}
