namespace Ops.Regions
{
    using System.Text.RegularExpressions;
    using ops.region;
    public static class Define
    {
        public static string PROVINCES =OptXML(RegionRes.provinces);
        public static string CITIES = OptXML(RegionRes.cities);
        public static string DISTRICTS=OptXML(RegionRes.districts);

        static string OptXML(string xmlContent)
        {
            xmlContent = Regex.Replace(xmlContent, "(\n|\r)\\s*", "");
            return xmlContent;
        }
    }
}