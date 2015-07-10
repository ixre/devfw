namespace AtNet.DevFw.Toolkit.NetCrawl
{
    internal class RuleFormat
    {
        public static string Format(string rule)
        {
            return rule.Replace("$$", "\\s*([\\s\\S]+?)\\s*");
        }
    }
}