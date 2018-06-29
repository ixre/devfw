using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JR.DevFw.Framework.Api;
using JR.DevFw.Framework.Net;

namespace JR.DevFw.UnitTest
{
    /// <summary>
    /// ApiTest 的摘要说明
    /// </summary>
    [TestClass]
    public class ApiTest
    {
        public static string OwnerCode = "8deecad7d5720169j96de0ps07d69010";
        public static string Url = "http://localhost:1419";
        public static String ApiKey = "6002097906324308";
        public static String ApiSecret = "306c4fkbsad1a124da7d7ee7ac632a33";
        public static String SignType = "sha1";

        public ApiTest()
        {
            //
            //TODO:  在此处添加构造函数逻辑
            //
        }


        [TestMethod]
        public void TestMethod1()
        {
            IDictionary<String, String> dataMap = new Dictionary<String, String>();
            dataMap["user"] = ApiKey;
            dataMap["sys"] = "zunxin-sys-001";
            dataMap["sign_type"] = SignType;
            String sign= ApiUtils.Sign(SignType, dataMap, ApiSecret);
            dataMap["sign"] = sign;
            String query = HttpClient.ParseQuery(dataMap);
            String url = String.Format("{0}/own/login_gw?{1}", Url, query);
            Console.WriteLine("快速登录链接为：" + url);
        }
    }
}
