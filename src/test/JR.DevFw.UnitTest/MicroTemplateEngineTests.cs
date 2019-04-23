using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace JR.DevFw.Tests
{
    class TemplateClass : ReflectTemplateClass
    {
        public String hello(String s)
        {
            s = this.GetEngine().FieldTemplate(s, (k) =>
            {
                if (k.StartsWith("time_fmt["))
                {
                    String format = k.Substring(9, k.Length - 10);
                    return String.Format("{0:"+format+"}",DateTime.Now);
                }
                if (k == "name") return "jarrysix";
                return "{" + k + "}";
            });
            return "hello " + s;
        }
    }
    [TestClass()]
    public class MicroTemplateEngineTests
    {
        [TestMethod()]
        public void MicroTemplateEngineTest()
        {
            MicroTemplateEngine engine = new MicroTemplateEngine(new TemplateClass());
            String s = engine.Execute("$hello('world, now is {time_fmt[yyyy-MM]} - Dear {name}{name1}')");
            Console.WriteLine(s);
        }

        [TestMethod()]
        public void ExecuteTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ExecuteTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void FieldTemplateTest()
        {
            Assert.Fail();
        }
    }
}