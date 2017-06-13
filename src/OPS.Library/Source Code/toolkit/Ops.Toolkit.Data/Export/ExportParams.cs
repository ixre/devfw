using System;
using System.Collections;

namespace Ops.Toolkit.Data.Export
{
    /// <summary>
    /// 数据导出参数
    /// </summary>
    [Serializable]
    public class ExportParams : IEnumerable
    {
        private Hashtable _parameters;

        public ExportParams(Object[,] data, String[] exportColumnFields) : this(exportColumnFields)
        {
            if (data == null) return;

            if (data.GetLength(1) != 2)
            {
                throw new ArgumentException("参数维度应为2");
            }

            for (int i = 0; i < data.GetLength(0); i++)
            {
                this.Parameters.Add(data[i, 0], data[i, 1]);
            }
        }

        public ExportParams(Hashtable hash, String[] exportColumnFields)
            : this(exportColumnFields)
        {
            if (hash != null)
            {
                this._parameters = hash;
            }
        }

        public ExportParams(String[] exportColumnFields)
        {
            this.ExportFields = exportColumnFields;
        }

        /// <summary>
        /// 要到导出的列(对应IDataExportPortal的ColumnNames或DataTable的Shelma
        /// </summary>
        public String[] ExportFields { get; private set; }

        /// <summary>
        /// 参数
        /// </summary>
        public Hashtable Parameters
        {
            get { return this._parameters ?? (this._parameters = new Hashtable()); }
        }

        public IEnumerator GetEnumerator()
        {
            return this.Parameters.GetEnumerator();
        }
    }
}