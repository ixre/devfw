using Ops.Framework;
using Ops.Framework.Extensions;
using Ops.Template;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Ops.Template;
using Ops.Utils;


namespace Tester
{
    class Program
    {
        static void Main(string[] args)
        {
        	//Payment_Test();
           // SettingFile_Test();
            // OPSoft_Json_JsonAnalyzer_Test();

           // TestTemplate2();

            TestUrlParams();
            Console.ReadLine();
        }

        private static void TestUrlParams()
        {
            string paramMappings = "caseNo:案件编号; partnerCode:供应商代码;contractNo:合同号;createTime:案件发生时间; importTime:导入时间;state:状态";
            Hashtable hash = new Hashtable();
            Regex regex = new Regex("\\s*([^:]+):([^;]*);*\\s*");
            MatchCollection mcs = regex.Matches(paramMappings);
            foreach (Match m in mcs)
            {
                hash.Add(m.Groups[1].Value, m.Groups[2].Value);
                Console.WriteLine("{0}>{1}", m.Groups[1].Value, m.Groups[2].Value);
            }
        }

        private static void TestTemplate2()
        {
            TemplatePage page = new TemplatePage();
            page.TemplateContent = "$ada('fsdf')=========$ad('x')";
            Console.WriteLine(page.ToString());
        }

        private static void SettingFile_Test()
        {
            SettingFile file = new SettingFile(AppDomain.CurrentDomain.BaseDirectory + "json.xml");


            // file.Append("banner", "{uri:'#',img:'/images/banner1.jpg'}");

            //Console.WriteLine(file["banner"]);
            //[CDATA[{uri:'#',img:'/images/banner1.jpg'}
            //file["banner"] = "fadfa";


           
            Console.WriteLine(file["db_type"]);
            if (file.Contains("db_type"))
            {
                file["db_type"] = "mssql" + "".RandomLetters(5);
            }

            Console.WriteLine(file["db_conn"]);
            Console.WriteLine(file["db_type"]);

            string key;
            for (int i = 0; i < 100; i++)
            {
                 key = "".RandomLetters(2) + "_" + String.Empty.RandomLetters(5); 
                if (!file.Contains(key))
                {
                    file.Add(key,String.Empty.RandomLetters(5));
                    System.Threading.Thread.Sleep(500);
                }
            }

            foreach (KeyValuePair<string, string> k in file.SearchKey(""))
            {
                Console.WriteLine(k.Key + "->" + k.Value);
            }

            file.Flush();
        }

        private static void Payment_Test()
        {
        	Ops.Framework.LogFile file=new Ops.Framework.LogFile(AppDomain.CurrentDomain.BaseDirectory+"1.txt");
        	
        	
        	string result=new Test.Payment().GetPaymentSubmit(!false,"20133131313");
        	file.Truncate();
        	file.Println(result);
        	Console.WriteLine(result);
      
        }

        /// <summary>
        /// JSON分析器测试
        /// </summary>
        private static void OPSoft_Json_JsonAnalyzer_Test()
        {
            JsonAnalyzer json = new JsonAnalyzer("{id:123,uri:'#',alt:'我的家在中国,你的呢?',img:'/images/banner1.jpg'}");

            Console.WriteLine(json.GetValue("img"));
            Console.WriteLine(json.GetValue("id"));
            Console.WriteLine("alt=" + json.GetValue("alt"));


            Console.WriteLine(json.SetValue("uri", "http://www.ops.cc"));

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            IDictionary<string, string> dict = JsonAnalyzer.ConvertToDictionary("{id:123,uri:'#',alt:'我的家在中国,你的呢?',img:'/images/banner1.jpg'}");
            foreach (KeyValuePair<string, string> k in dict)
            {
                Console.WriteLine(k.Key + "->" + k.Value);
            }


        }



        internal class TPLObject
        {
            [TemplateTag]
            protected string Tpl()
            {
                return "tpl";
            }

            [TemplateTag]
            public string Tpl(string str)
            {
                return str;
            }
            [TemplateTag]
            private string Tpl(string num, string str)
            {
                return String.Format("{0}*{1}", num, str);
            }

            [TemplateMethod]
            private string EachTpl(string str,string num,string url,string format)
            {
                StringBuilder sb = new StringBuilder();
                string[] strArr = str.Split(',');
                foreach (string _str in strArr)
                {
                    sb.Append(SimpleTplEngine.FieldTemplate(format, a =>
                    {
                        if (a == num) return _str;
                        if (a == url) return "http://" + _str + ".com";
                        return "{" + a + "}";
                    }));
                }
                return sb.ToString();
            }
        }

        private static void TestTemplate()
        {
            const string templateStr = "$tpl(),$tpl('str'),$tpl(3),$tpl(3,'car')";
            const string templateStr2 = @"@eachtpl('1,2,3',num,url){
                                             {num}({url}) ->
                                            $tpl(3,'{str}')
                                        }@endeach";

            SimpleTplEngine tpl = new SimpleTplEngine(new TPLObject(),false);
            Console.WriteLine("template output:{0}", tpl.Execute(templateStr));

            Console.WriteLine("output:@eachtpl('1,2,3',num,url){{num} -> $tpl(3,'{str}')}");

            Console.WriteLine("template output:{0}", tpl.Execute(templateStr2));

            Console.WriteLine("==================================");



        }

    }
}
