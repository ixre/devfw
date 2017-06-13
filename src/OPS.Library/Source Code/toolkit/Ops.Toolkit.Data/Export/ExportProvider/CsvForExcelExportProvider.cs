using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace Ops.Toolkit.Data.Export.ExportProvider
{
    public class ExcelExportProvider : IDataExportProvider
    {
        private const string split = "\t";

        public byte[] Export(DataTable dt, IDictionary<string, string> columns)
        {
            string strLine = "";
            StreamWriter sw;
            MemoryStream ms = new MemoryStream();

            bool isReColumn = !(columns == null || columns.Count == 0);
            int tmpInt = 0;
            sw = new StreamWriter(ms, Encoding.UTF8);

            //表头

            if (isReColumn)
            {
                foreach (string columnName in columns.Keys)
                {
                    if (tmpInt++ > 0)
                        strLine += split;
                    strLine += columns[columnName];
                }
            }
            else
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    if (i > 0)
                        strLine += split;
                    strLine += dt.Columns[i].ColumnName;
                }
            }
            strLine.Remove(strLine.Length - 1);

            sw.WriteLine(strLine);


            //表的内容
            strLine = "";
            for (int j = 0; j < dt.Rows.Count; j++)
            {
                strLine = "";

                if (isReColumn)
                {
                    tmpInt = 0;
                    foreach (string columnName in columns.Keys)
                    {
                        if (tmpInt++ != 0)
                            strLine += split;
                        if (dt.Rows[j][columnName] != null)
                        {
                            string cell = dt.Rows[j][columnName].ToString().Trim();
                            //防止里面含有特殊符号
                            cell = cell.Replace("\"", "\"\"");
                            cell = "\"" + cell + "\"";
                            strLine += cell;
                        }
                    }
                }
                else
                {
                    int colCount = dt.Columns.Count;
                    for (int k = 0; k < colCount; k++)
                    {
                        if (k > 0 && k < colCount)
                            strLine += split;
                        if (dt.Rows[j][k] == null)
                            strLine += "";
                        else
                        {
                            string cell = dt.Rows[j][k].ToString().Trim();
                            //防止里面含有特殊符号
                            cell = cell.Replace("\"", "\"\"");
                            cell = "\"" + cell + "\"";
                            strLine += cell;
                        }
                    }
                }
                sw.WriteLine(strLine);
            }

            sw.Close();
            byte[] bytes = ms.ToArray();

            ms.Dispose();

            return bytes;
        }
    }
}