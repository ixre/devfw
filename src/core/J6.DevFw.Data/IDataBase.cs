//
//
//  Generated by StarUML(tm) C# Add-In
//
//  @ Project : OPS.Data
//  @ File Name : IDataBase.cs
//  @ Date : 8/18/2011
//  @ Author : ????
//
//

using System;
using System.Data.Common;

namespace JR.DevFw.Data
{
    /// <summary>
    /// 中间件
    /// </summary>
    /// <param name="action"></param>
    /// <param name="sql"></param>
    /// <param name="sqlParams"></param>
    /// <param name="exc"></param>
    /// <returns></returns>
    public delegate bool Middleware(String action, String sql, DbParameter[] sqlParams, Exception exc);

    /// <summary>
    /// 影响行的SQL函数
    /// </summary>
    /// <param name="sql"></param>
    /// <returns></returns>
    public delegate int RowAffer(string sql);

    /// <summary>
    /// 数据读取器函数
    /// </summary>
    /// <param name="reader"></param>
    public delegate void DataReaderFunc(DbDataReader reader);

    public interface IDataBase
    {
        string ConnectionString { get; }
        DbConnection GetConnection();
        DbParameter CreateParameter(string name, object value);
        DbCommand CreateCommand(string sql);
        DbDataAdapter CreateDataAdapter(DbConnection connection, string sql);

        /// <summary>
        /// ??н??
        /// </summary>
        /// <param name="sql">Sql???</param>
        /// <param name="delimiter">??????????";"</param>
        /// <returns></returns>
        int ExecuteScript(DbConnection conn, RowAffer r, string sql, string delimiter);
    }

    public abstract class DataBaseFactory : IDataBase
    {
        protected string connectionString;

        public DataBaseFactory(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public string ConnectionString
        {
            get { return this.connectionString; }
        }

        public abstract DbConnection GetConnection();

        public abstract DbParameter CreateParameter(string name, object value);

        public abstract DbCommand CreateCommand(string commandText);

        public abstract DbDataAdapter CreateDataAdapter(DbConnection connection, string commandText);

        public abstract int ExecuteScript(DbConnection connection, RowAffer r, string sql, string delimiter);
    }
}