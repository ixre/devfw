using System;

namespace JR.DevFw.Toolkit.Data.Export
{
    /// <summary>
    /// 列映射
    /// </summary>
    [Serializable]
    public struct DataColumnMapping
    {
        private string _field;
        private string _name;

        public DataColumnMapping(string field, string name)
        {
            this._field = field;
            this._name = name;
        }

        public DataColumnMapping(string field) : this(field, field)
        {
        }

        /// <summary>
        /// 列的字段 (通常是列在数据源的名称)
        /// </summary>
        public string Field
        {
            get { return this._field; }
        }

        /// <summary>
        /// 列的名称
        /// </summary>
        public string Name
        {
            get { return this._name; }
        }
    }
}