using System.Text.RegularExpressions;

namespace J6.DevFw.Toolkit.Region
{
    public static class Define
    {
        public static string Provinces =OptXml(RegionRes.provinces);
        public static string Cities = OptXml(RegionRes.cities);
        public static string Districts=OptXml(RegionRes.districts);

        static string OptXml(string xmlContent)
        {
            xmlContent = Regex.Replace(xmlContent, "(\n|\r)\\s*", "");
            return xmlContent;
        }
    }
}