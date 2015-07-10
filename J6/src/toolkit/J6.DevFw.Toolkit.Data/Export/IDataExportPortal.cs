using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace J6.DevFw.Toolkit.Data.Export
{
    public interface IDataExportPortal
    {
        /// <summary>
        /// 入口键
        /// </summary>
        string PortalKey { get; set; }

        /// <summary>
        /// 导出的列名(比如：数据表是因为列，这里我需要列出中文列)
        /// </summary>
        DataColumnMapping[] ColumnNames { get; }


        /// <summary>
        /// 获取要导出的数据及表结构
        /// </summary>
        /// <returns></returns>
        DataTable GetShemalAndData(Hashtable parameters, out int totalCount);

        /// <summary>
        /// 获取统计数据
        /// </summary>
        /// <returns></returns>
        DataRow GetTotalView(Hashtable parameters);

        /// <summary>
        /// 根据参数获取导出列名及导出名称
        /// </summary>
        /// <returns></returns>
        IDictionary<string, String> GetExportFieldAndName(String[] exportFields);
    }
}