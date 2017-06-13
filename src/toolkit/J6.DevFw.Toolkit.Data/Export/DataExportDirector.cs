using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace JR.DevFw.Toolkit.Data.Export
{
    public static class DataExportDirector
    {
        public static byte[] Export(IDataExportPortal portal, ExportParams parameters, IDataExportProvider provider)
        {
            int totalCount;
            IDictionary<string, String> dict = null;

            //获取参数的索引及名称
            if (parameters != null && parameters.ExportFields != null)
            {
                dict = portal.GetExportFieldAndName(parameters.ExportFields);
            }
            return
                provider.Export(
                    portal.GetShemalAndData(parameters == null ? null : parameters.Parameters, out totalCount), dict);
        }

        public static ExportParams GetExportParams(string paramMappings, string[] columnNames)
        {
            //string[] splitArr;

            //object[,] data;
            //string[] paramsArr = paramMappings.Split(';');

            //if (paramMappings == "")
            //{
            //    data = new object[0, 2];
            //}
            //else
            //{
            //    data = new object[paramsArr.Length, 2];
            //    //添加传入的参数
            //    for (int i = 0; i < paramsArr.Length; i++)
            //    {
            //        splitArr = paramsArr[i].Split(':');
            //        data[i, 0] = splitArr[0];
            //        data[i, 1] = paramsArr[i].Substring(splitArr[0].Length + 1);
            //    }
            //}
            Hashtable hash = new Hashtable();
            Regex regex = new Regex("\\s*([^:]+):([^;]*);*\\s*");
            MatchCollection mcs = regex.Matches(paramMappings);
            foreach (Match m in mcs)
            {
                hash.Add(m.Groups[1].Value, m.Groups[2].Value);
            }

            return new ExportParams(hash, columnNames);
        }

        ///// <summary>
        ///// 根据类名创建导出
        ///// </summary>
        ///// <param name="exportPortalClassFullName"></param>
        ///// <param name="provider"></param>
        ///// <param name="parameters"></param>
        ///// <returns></returns>
        //public static byte[] Export(string exportPortalClassFullName,
        //    IDataExportProvider provider,
        //    ExportParams parameters)
        //{

        //    IDataExportPortal portal = ExportUtil.GetNotNullPortal(exportPortalClassFullName);


        //    portal.Parameters = parameters;
        //    return Export(portal, provider);
        //}
    }
}