namespace J6.DevFw.Toolkit.Data.Export
{
    /// <summary>
    /// 导入导出项目配置
    /// </summary>
    public struct ExportItemConfig
    {
        private readonly string _columnMappingString;
        private readonly string _query;
        private readonly string _total;
        private readonly string _import;

        public ExportItemConfig(string columnMappingString, string query, string total, string import)
        {
            this._columnMappingString = columnMappingString;
            this._query = query;
            this._total = total;
            this._import = import;
        }


        public string ColumnMappingString
        {
            get { return this._columnMappingString; }
        }

        public string Query
        {
            get { return this._query; }
        }

        public string Total
        {
            get { return this._total; }
        }

        public string Import
        {
            get { return this._import; }
        }
    }
}