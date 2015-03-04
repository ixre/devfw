using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.PluginKernel;
using System.IO;
using Com.PluginKernel.Demo;

namespace test
{


    public class Class1
    {
        static void Main()
        {
            string phyPath = AppDomain.CurrentDomain.BaseDirectory;
            //File.Copy(phyPath + "../../../test1/binDebug/test1.dll", phyPath + "plugins/test1.exe", true);

            DemoPluginApp app = new DemoPluginApp();

            //连接
            app.Connect();
            
            PluginPackAttribute attr;
            IPlugin p= PluginUtil.GetPlugin("com.test",out attr);
            

            //运行所有
            //app.Run();

            app.Print("这是一条消息!");

            Console.ReadKey();
        }
    }
}
