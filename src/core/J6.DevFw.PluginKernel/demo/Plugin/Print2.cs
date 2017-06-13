using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.PluginKernel;

namespace Com.Plugin
{
    [PluginInfo("HTML打印插件","刘铭","","")]
    public class HtmlPrint :IPlugin
    {
        public void Run()
        {
            Console.WriteLine("运行：" + this.GetType().Name);
        }

        public PluginConnectionResult Connect(IPluginApp app)
        {
            DemoPluginApp demo = app as DemoPluginApp;
            demo.OnPrinting += (string str,ref bool result) =>
            {
               Console.WriteLine("{0}开始打印：{1}",this.GetType().Name,str);
            };

            return PluginConnectionResult.Success;
        }

        public bool Install()
        {
            throw new NotImplementedException();
        }

        public bool Uninstall()
        {
            throw new NotImplementedException();
        }

        public void Pause()
        {
            throw new NotImplementedException();
        }

        public string GetMessage()
        {
            return "";
        }
    	
		public object GetCalledObject()
		{
			return "return a object";
		}
    	
		public object Call(string method, params object[] parameters)
		{
			throw new NotImplementedException();
		}
    }
}
