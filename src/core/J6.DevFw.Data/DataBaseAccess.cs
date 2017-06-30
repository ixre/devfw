//
//
//  Copryright 2011 @ S1N1.COM.All rights reseved.
//
//  Project : OPS.Data
//  File Name : DataBaseAccess.cs
//  Date : 8/19/2011
//  Author : 刘铭
//
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text.RegularExpressions;
using JR.DevFw.Data.Extensions;
using System.Threading;

namespace JR.DevFw.Data
{
    public class DataBaseAccess
    {
        private static readonly Object locker = new object();
        private readonly IDataBase dbFactory;
        private static readonly Regex procedureRegex = new Regex("\\s");
        private IList<String> _totalSqls;
        private bool _totalOpen;
        private int _commandTimeout = 30000;

        /// <summary>
        /// 实例化数据库访问对象
        /// </summary>
        /// <param name="type"></param>
        /// <param name="connectionString"></param>
        public DataBaseAccess(DataBaseType type, string connectionString)
        {
            if (connectionString.IndexOf("$ROOT$") != -1)
            {
                connectionString = connectionString.Replace("$ROOT$", AppDomain.CurrentDomain.BaseDirectory);
            }

            this.DbType = type;

            switch (type)
            {
                case DataBaseType.OLEDB:
                    dbFactory = new OleDbFactory(connectionString);
                    break;
                case DataBaseType.SQLite:
                    dbFactory = new SQLiteFactory(connectionString);
                    break;
                case DataBaseType.MonoSQLite:
                    dbFactory = new MonoSQLiteFactory(connectionString);
                    break;
                case DataBaseType.SQLServer:
                    dbFactory = new SqlServerFactory(connectionString);
                    break;
                case DataBaseType.MySQL:
                    dbFactory = new MySqlFactory(connectionString);
                    break;
            }
        }

        /// <summary>
        /// 执行命令超时时间，默认为30000(30秒)
        /// </summary>
        public int CommandTimeout
        {
            get { return this._commandTimeout; }
            set
            {
                if (value <= 2000)
                {
                    throw new ArgumentException("无效数值");
                }
                this._commandTimeout = value;
            }
        }

        private DbConnection CreateOpenedConnection()
        {
            DbConnection connection = dbFactory.GetConnection();
            connection.Open();
            return connection;
        }

        /// <summary>
        /// 重设统计
        /// </summary>
        public void StartNewTotal()
        {
            if (!_totalOpen)
                _totalOpen = true;
            if (_totalSqls == null)
                _totalSqls = new List<String>();
            else
                _totalSqls.Clear();
        }

        public IList<string> GetTotalSqls()
        {
            if (this._totalSqls == null)
                throw new Exception("请使用StartNewTotal()开始统计");
            return this._totalSqls;
        }

        private void AddTotalSql(string sql)
        {
            _totalSqls.Add(sql);
        }


        /// <summary>
        /// 数据库类型
        /// </summary>
        public DataBaseType DbType { get; private set; }

        /// <summary>
        /// 数据库适配器
        /// </summary>
        public IDataBase DataBaseAdapter
        {
            get { return this.dbFactory; }
        }

        /// <summary>
        /// 返回一个参数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public DbParameter NewParameter(string name, object value)
        {
            return dbFactory.CreateParameter(name, value);
        }

        private DbCommand CreateCommand(string sql)
        {
            DbCommand cmd = this.dbFactory.CreateCommand(sql);
            cmd.CommandTimeout = this._commandTimeout;
            return cmd;
        }

        /// <summary>
        /// 返回一个参数,可指定参数的类型为输出参数或者输入参数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public DbParameter NewParameter(string name, object value, ParameterDirection direction)
        {
            DbParameter parameter = dbFactory.CreateParameter(name, value);
            parameter.Direction = direction;
            return parameter;
        }

        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(string commandText)
        {
            if (this._totalOpen) this.AddTotalSql(commandText);
            DbParameter[] parameters = null;
            return this.ExecuteNonQuery(commandText, parameters);
        }

        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(string commandText, params DbParameter[] parameters)
        {
            if (this._totalOpen) this.AddTotalSql(commandText);
            int result = 0;
            using (DbConnection conn = this.CreateOpenedConnection())
            {
                DbCommand cmd = this.CreateCommand(commandText);
                cmd.Connection = conn;
                //自动判断是T-SQL还是存储过程
                cmd.CommandType = procedureRegex.IsMatch(commandText) ? CommandType.Text : CommandType.StoredProcedure;
                if (parameters != null) cmd.Parameters.AddRange(parameters);
                // SQLite不支持并发的写入
                if (this.DbType == DataBaseType.SQLite)
                {
                    Monitor.Enter(locker);
                    result = cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    Monitor.Exit(locker);
                    return result;
                }
                result = cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
            return result;
        }

        /// <summary>
        /// 返回查询的第一行第一列值
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public object ExecuteScalar(string commandText)
        {
            if (this._totalOpen) this.AddTotalSql(commandText);
            DbParameter[] parameters = null;
            return ExecuteScalar(commandText, parameters);
        }

        /// <summary>
        /// 返回查询的第一行第一列值
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public object ExecuteScalar(string commandText, params DbParameter[] parameters)
        {
            if (this._totalOpen) this.AddTotalSql(commandText);
            object obj;
            using (DbConnection conn = this.CreateOpenedConnection())
            {
                DbCommand cmd = this.CreateCommand(commandText);
                cmd.Connection = conn;
                //自动判断是T-SQL还是存储过程
                cmd.CommandType = procedureRegex.IsMatch(commandText) ? CommandType.Text : CommandType.StoredProcedure;
                if (parameters != null) cmd.Parameters.AddRange(parameters);
                obj = cmd.ExecuteScalar();
                cmd.Dispose();
            }
            return obj;
        }

        /// <summary>
        /// 读取DataReader中的数据
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        private DbDataReader ExecuteReader(string commandText, params DbParameter[] parameters)
        {
            if (this._totalOpen) this.AddTotalSql(commandText);
            using (DbConnection conn = this.CreateOpenedConnection())
            {
                DbCommand cmd = this.CreateCommand(commandText);
                cmd.Connection = conn;
                //自动判断是T-SQL还是存储过程
                cmd.CommandType = procedureRegex.IsMatch(commandText) ? CommandType.Text : CommandType.StoredProcedure;
                if (parameters != null) cmd.Parameters.AddRange(parameters);
                DbDataReader rd = cmd.ExecuteReader();
                cmd.Dispose();
                return rd;
            }
        }

        /// <summary>
        /// 读取DataReader中的数据
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="func"></param>
        public void ExecuteReader(string commandText, DataReaderFunc func)
        {
            if (this._totalOpen) this.AddTotalSql(commandText);
            DbParameter[] parameters = null;
            ExecuteReader(commandText, func, parameters);
        }

        /// <summary>
        /// 读取DataReader中的数据
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="func"></param>
        /// <param name="parameters"></param>
        public void ExecuteReader(string commandText, DataReaderFunc func, params DbParameter[] parameters)
        {
            if (this._totalOpen) this.AddTotalSql(commandText);
            using (DbConnection conn = this.CreateOpenedConnection())
            {
                DbCommand cmd = this.CreateCommand(commandText);
                cmd.Connection = conn;
                //自动判断是T-SQL还是存储过程
                cmd.CommandType = procedureRegex.IsMatch(commandText)
                    ? CommandType.Text
                    : CommandType.StoredProcedure;
                if (parameters != null) cmd.Parameters.AddRange(parameters);
                DbDataReader rd = cmd.ExecuteReader();
                func(rd);
                cmd.Dispose();
            }
        }

        /// <summary>
        /// 从数据库中读取数据并保存在内存中
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public DataSet GetDataSet(string commandText)
        {
            if (this._totalOpen) this.AddTotalSql(commandText);
            DbParameter[] parameters = null;
            return GetDataSet(commandText, parameters);
        }

        /// <summary>
        /// 从数据库中读取数据并保存在内存中
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public DataSet GetDataSet(string commandText, params DbParameter[] parameters)
        {
            if (this._totalOpen) this.AddTotalSql(commandText);

            DataSet ds = new DataSet();
            using (DbConnection conn = this.CreateOpenedConnection())
            {
                DbDataAdapter adapter = dbFactory.CreateDataAdapter(conn, commandText);
                if (parameters != null)
                {
                    adapter.SelectCommand.Parameters.AddRange(parameters);
                    //自动判断是T-SQL还是存储过程
                    adapter.SelectCommand.CommandType = procedureRegex.IsMatch(commandText)
                        ? CommandType.Text
                        : CommandType.StoredProcedure;
                }
                adapter.Fill(ds);
            }
            return ds;
        }

        public T ToEntity<T>(string commandText) where T : new()
        {
            return ToEntity<T>(commandText, null);
        }


        /// <summary>
        /// 将查询结果转换为实体对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public T ToEntity<T>(string commandText, params DbParameter[] parameters) where T : new()
        {
            if (this._totalOpen) this.AddTotalSql(commandText);

            T t = default(T);
            ExecuteReader(commandText, (reader) =>
            {
                if (reader.HasRows)
                {
                    t = reader.ToEntity<T>();
                }
            }, parameters);
            return t;
        }

        /// <summary>
        /// 以DataReader返回数据并转换成实体类集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public IList<T> ToEntityList<T>(string commandText) where T : new()
        {
            return ToEntityList<T>(commandText, null);
        }

        /// <summary>
        /// 以DataReader返回数据并转换成实体类集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public IList<T> ToEntityList<T>(string commandText, params DbParameter[] parameters) where T : new()
        {
            if (this._totalOpen) this.AddTotalSql(commandText);

            IList<T> list = null;

            ExecuteReader(commandText, (reader) =>
            {
                if (reader.HasRows)
                {
                    list = reader.ToEntityList<T>();
                }
            }, parameters);

            return list ?? new List<T>();
        }

        /// <summary>
        /// 执行脚本(仅Mysql)
        /// </summary>
        /// <param name="sql">sql脚本</param>
        /// <param name="delimiter">分割符，可传递空</param>
        /// <returns></returns>
        public int ExecuteScript(string sql, string delimiter)
        {
            int result = -1;
            using (DbConnection conn = this.CreateOpenedConnection())
            {
                result = dbFactory.ExecuteScript(conn, sql, delimiter);
            }

            return result;
        }

        #region  新的连结方式

        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="sqls"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(params SqlQuery[] sqls)
        {
            if (this._totalOpen)
                foreach (SqlQuery sql in sqls)
                    this.AddTotalSql(sql.Sql);

            if (sqls.Length == 0) throw new ArgumentOutOfRangeException("sqls", "SQLEntity至少应指定一个!");

            DbTransaction trans = null;
            DbCommand cmd;
            int result = 0;

            DbConnection conn = this.CreateOpenedConnection();

            //打开连接并设置事务
            trans = conn.BeginTransaction();


            SqlEntityHandler sh = s =>
            {
                //创建Command,并设置连接
                cmd = this.CreateCommand(s.Sql);
                cmd.Connection = conn;
                //自动判断是T-SQL还是存储过程
                cmd.CommandType = procedureRegex.IsMatch(s.Sql)
                    ? CommandType.Text
                    : CommandType.StoredProcedure;

                //添加参数
                if (s.Parameters != null) cmd.Parameters.AddRange(s.ToParams(dbFactory));
                //使用事务
                cmd.Transaction = trans;
                //SQLite不支持并发写入
                if (this.DbType == DataBaseType.SQLite)
                {
                    Monitor.Enter(locker);
                    result += cmd.ExecuteNonQuery();
                    Monitor.Exit(locker);
                    cmd.Dispose();
                    return;
                }
                result += cmd.ExecuteNonQuery();
                cmd.Dispose();
            };

            try
            {
                foreach (SqlQuery sql in sqls)
                {
                    sh(sql);
                }
                //提交事务

                trans.Commit();
            }
            catch (DbException ex)
            {
                //如果用事务执行,则回滚
                trans.Rollback();

                //重新抛出异常
                throw ex;
            }
            finally
            {
                //关闭连接
                conn.Close();
            }


            return result;
        }

        /// <summary>
        /// 读取DataReader中的数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="func"></param>
        public void ExecuteReader(SqlQuery sql, DataReaderFunc func)
        {
            if (this._totalOpen) this.AddTotalSql(sql.Sql);
            using (DbConnection conn = this.CreateOpenedConnection())
            {
                DbCommand cmd = this.CreateCommand(sql.Sql);
                cmd.Connection = conn;

                //自动判断是T-SQL还是存储过程
                cmd.CommandType = procedureRegex.IsMatch(sql.Sql) ? CommandType.Text : CommandType.StoredProcedure;

                if (sql.Parameters != null)
                    cmd.Parameters.AddRange(sql.ToParams(dbFactory));
                func(cmd.ExecuteReader());
                cmd.Dispose();
            }
        }


        /// <summary>
        /// 从数据库中读取数据并保存在内存中
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataSet GetDataSet(SqlQuery sql)
        {
            if (this._totalOpen) this.AddTotalSql(sql.Sql);
            DataSet ds = new DataSet();
            using (DbConnection conn = this.CreateOpenedConnection())
            {
                DbDataAdapter adapter = dbFactory.CreateDataAdapter(conn, sql.Sql);

                if (sql.Parameters != null)
                {
                    adapter.SelectCommand.Parameters.AddRange(sql.ToParams(dbFactory));
                    //自动判断是T-SQL还是存储过程
                    adapter.SelectCommand.CommandType = procedureRegex.IsMatch(sql.Sql)
                        ? CommandType.Text
                        : CommandType.StoredProcedure;
                }
                adapter.Fill(ds);
            }

            return ds;
        }


        /// <summary>
        /// 返回查询的第一行第一列值
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public object ExecuteScalar(SqlQuery sql)
        {
            if (this._totalOpen) this.AddTotalSql(sql.Sql);

            object obj;
            using (DbConnection conn = this.CreateOpenedConnection())
            {
                DbCommand cmd = this.CreateCommand(sql.Sql);
                cmd.Connection = conn;

                //自动判断是T-SQL还是存储过程
                cmd.CommandType = procedureRegex.IsMatch(sql.Sql)
                    ? CommandType.Text
                    : CommandType.StoredProcedure;

                if (sql.Parameters != null) cmd.Parameters.AddRange(sql.ToParams(dbFactory));
                obj = cmd.ExecuteScalar();
                cmd.Dispose();
            }

            return obj;
        }

        /// <summary>
        /// 将查询结果转换为实体对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public T ToEntity<T>(SqlQuery sql) where T : new()
        {
            if (this._totalOpen) this.AddTotalSql(sql.Sql);

            T t = default(T);
            this.ExecuteReader(sql, (reader) =>
            {
                if (reader.HasRows)
                {
                    t = reader.ToEntity<T>();
                }
            });


            return t;
        }

        /// <summary>
        /// 以DataReader返回数据并转换成实体类集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public IList<T> ToEntityList<T>(SqlQuery sql) where T : new()
        {
            if (this._totalOpen) this.AddTotalSql(sql.Sql);

            IList<T> list = null;

            this.ExecuteReader(sql, (reader) =>
            {
                if (reader.HasRows)
                {
                    list = reader.ToEntityList<T>();
                }
            });

            return list ?? new List<T>();
        }

        #endregion

        #region Hashtable获取数据

        public int ExecuteNonQuery(string commandText, Hashtable data)
        {
            var parameters = GetParametersFromHashTable(data);
            return this.ExecuteNonQuery(commandText, parameters);
        }

        public object ExecuteScalar(string commandText, Hashtable data)
        {
            return this.ExecuteScalar(commandText, this.GetParametersFromHashTable(data));
        }

        public void ExecuteReader(string commandText, Hashtable data, DataReaderFunc func)
        {
            this.ExecuteReader(commandText, func, this.GetParametersFromHashTable(data));
        }

        public DataSet GetDataSet(string commandText, Hashtable data)
        {
            return this.GetDataSet(commandText, this.GetParametersFromHashTable(data));
        }

        private DbParameter[] GetParametersFromHashTable(Hashtable data)
        {
            DbParameter[] parameters = new DbParameter[data.Keys.Count];

            int i = 0;
            foreach (DictionaryEntry d in data)
            {
                parameters[i++] = this.NewParameter("@" + d.Key, d.Value);
            }
            return parameters;
        }

        #endregion

        /*
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (conn!=null && conn.State != ConnectionState.Closed)
            {
                conn.Dispose();
            }
        }

        ~DataBaseAccess()
        {
            this.Dispose();
        }
		 */
    }
}