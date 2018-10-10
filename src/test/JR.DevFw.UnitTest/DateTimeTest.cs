using System;
using JR.DevFw.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JR.DevFw.UnitTest
{
    [TestClass]
    public class DateTimeTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            long unix = 1538062009;
            var unixVar = new DateTime(1970, 1, 1, 0, 0, 0, 0);

            TimeSpan ts = new TimeSpan(unix*10000*1000);
            DateTime dt =  TimeZone.CurrentTimeZone.ToLocalTime(unixVar).Add(ts);
            Console.WriteLine(dt.ToString());
            Console.WriteLine(TimeUtils.UnixTime(1538062009).ToString());


        }
    }
}
