using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;

namespace Ops.Toolkit.Data.Export
{
    /// <summary>
    /// 数据导出入口基础类
    /// </summary>
    [Serializable]
    public abstract class BaseDataExportPortal : IDataExportPortal
    {
        private readonly DataColumnMapping[] _columnNames;

        protected BaseDataExportPortal(DataColumnMapping[] columnNames)
        {
            this._columnNames = columnNames;
        }

        protected BaseDataExportPortal(string configPath)
        {
            ExportUtil.ReadExportConfigFormFile(configPath, out this._columnNames);
        }

        public abstract string PortalKey { get; set; }

        public DataColumnMapping[] ColumnNames
        {
            get { return this._columnNames; }
        }

        public abstract DataTable GetShemalAndData(Hashtable parameters, out int totalCount);

        public abstract DataRow GetTotalView(Hashtable parameters);

        public virtual IDictionary<string, string> GetExportFieldAndName(String[] exportFields)
        {
            if (this._columnNames == null || this._columnNames.Length == 0) return null;

            IDictionary<string, string> dictionary = new Dictionary<string, string>();

            foreach (string columnField in exportFields)
            {
                DataColumnMapping map = this._columnNames.SingleOrDefault(
                    a => String.Compare(a.Field, columnField, true,
                        CultureInfo.InvariantCulture) == 0);

                if (map.Field != null)
                {
                    dictionary.Add(map.Field, map.Name);
                }
            }
            return dictionary;
        }
    }
}