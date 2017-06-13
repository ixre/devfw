using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OPSoft.Plugin.NetCrawl
{
    internal class RuleFormat
    {
        public static string Format(string rule)
        {
            return rule.Replace("$$", "\\s*([\\s\\S]+?)\\s*");
        }
    }
}