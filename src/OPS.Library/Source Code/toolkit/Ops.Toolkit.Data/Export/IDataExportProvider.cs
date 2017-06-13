using System.Collections.Generic;
using System.Data;

namespace Ops.Toolkit.Data.Export
{
    public interface IDataExportProvider
    {
        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <param name="columns">要导出的列名及对应导出的列名,可为空</param>
        /// <returns></returns>
        byte[] Export(DataTable dt, IDictionary<string, string> columns);
    }
}