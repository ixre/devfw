using System;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;

namespace J6.DevFw.Toolkit.Data.Export
{
    public static class ExportUtil
    {
        /// <summary>
        /// 通过类名来获取导出入口
        /// </summary>
        /// <param name="exportPortalClassFullName"></param>
        /// <returns></returns>
        public static IDataExportPortal GetPortal(string exportPortalClassFullName)
        {
            IDataExportPortal portal = null;

            Assembly[] asses = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly ass in asses)
            {
                Type type = ass.GetType(exportPortalClassFullName);
                if (type != null)
                {
                    portal = Activator.CreateInstance(type) as IDataExportPortal;
                    if (portal != null) break;
                }
            }

            return portal;
        }

        /// <summary>
        /// 通过类名来获取存在的导出入口
        /// </summary>
        /// <param name="exportPortalClassFullName"></param>
        /// <returns></returns>
        public static IDataExportPortal GetNotNullPortal(string exportPortalClassFullName)
        {
            IDataExportPortal portal = GetPortal(exportPortalClassFullName);
            if (portal == null)
            {
                throw new ArgumentNullException(String.Format("导出类型不存在或未实现接口：IDataExportPortal!类名：{0}",
                    exportPortalClassFullName));
            }
            return portal;
        }

        /// <summary>
        /// 从文件中读取导出配置
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="columns"></param>
        /// <param name="queryName"></param>
        /// <returns></returns>
        public static bool ReadExportConfigFormFile(
            string filePath,
            out DataColumnMapping[] columns)
        {
            return ReadExportConfigFormXml(
                System.IO.File.ReadAllText(filePath),
                null,
                out columns);
        }

        public static bool ReadExportConfigFormXml(
            string xml,
            string xpath,
            out DataColumnMapping[] columns)
        {
            XmlDocument xd = new XmlDocument();
            xd.LoadXml(xml);
            if (String.IsNullOrEmpty(xpath))
            {
                xpath = "/item";
            }
            XmlNode rootNode = xd.SelectSingleNode(xpath);
            XmlNode node;

            if (rootNode == null)
            {
                throw new ArgumentNullException("xpath");
            }

            #region 获取列配置

            node = rootNode.SelectSingleNode("column");
            if (node == null)
            {
                columns = null;
            }
            else
            {
                string columnMappingString = node.Attributes["mapping"].Value;
                columns = GetColumnMappings(columnMappingString);
            }

            #endregion

            return true;
        }

        /// <summary>
        /// 获取列映射数组
        /// </summary>
        /// <param name="columnMappingString"></param>
        /// <returns></returns>
        public static DataColumnMapping[] GetColumnMappings(string columnMappingString)
        {
            DataColumnMapping[] columns;
            Regex regex = new Regex("\\s*([^:]+):([^;]*);*\\s*");
            MatchCollection mcs = regex.Matches(columnMappingString);
            columns = new DataColumnMapping[mcs.Count];
            int i = 0;
            foreach (Match m in mcs)
            {
                columns[i++] = new DataColumnMapping(m.Groups[1].Value, m.Groups[2].Value);
            }
            return columns;
        }


        public static ExportItemConfig GetExportItemFormXml(string xml, string xpath)
        {
            string mappingString;
            string query;
            string import;
            string total;
            XmlDocument xd = new XmlDocument();
            xd.LoadXml(xml);
            if (String.IsNullOrEmpty(xpath))
            {
                xpath = "/item";
            }
            XmlNode rootNode = xd.SelectSingleNode(xpath);
            XmlNode node;

            if (rootNode == null)
            {
                throw new ArgumentNullException("xpath");
            }

            node = rootNode.SelectSingleNode("column");
            mappingString = node == null ? null : node.Attributes["mapping"].Value;

            node = rootNode.SelectSingleNode("query");
            query = node == null ? null : node.InnerText;


            node = rootNode.SelectSingleNode("import");
            import = node == null ? null : node.InnerText;


            node = rootNode.SelectSingleNode("total");
            total = node == null ? null : node.InnerText;

            return new ExportItemConfig(mappingString, query, total, import);
        }
    }
}