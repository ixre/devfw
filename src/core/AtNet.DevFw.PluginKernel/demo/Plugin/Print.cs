using Com.PluginKernel;
using System;
using Com.PluginKernel.Demo;

namespace Com.Plugin
{
    [PluginInfo("测试插件","刘铭","","")]
    public class Print :IPlugin
    {
        private PluginPackAttribute _attr;
        public void Run()
        {
            Console.WriteLine("运行：" + this.GetType().Name);
        }

        public PluginConnectionResult Connect(IPluginHost app)
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
    	
    	
		public object Call(string method, params object[] parameters)
		{
			throw new NotImplementedException();
		}


        public void Logln(string line)
        {
            throw new NotImplementedException();
        }


        public PluginPackAttribute GetAttribute()
        {
            if (this._attr == null)
            {
                this._attr = PluginUtil.GetAttribute(this);
            }
            return this._attr;
        }
    }
}
