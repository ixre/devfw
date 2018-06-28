//
//
//  Copryright 2011 @ S1N1.COM.All rights reseved.
//
//  Project : OPS.Data
//  File Name : SQLiteFactory.cs
//  Date : 8/19/2011
//  Author : ¡ı√˙
//
//

using System.Data.Common;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace JR.DevFw.Data
{
    public class MySqlFactory : IDbDialect
    {
        private string connectionString;

        public MySqlFactory(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public  DbConnection GetConnection()
        {
            return new MySqlConnection(this.connectionString);
        }

        public  DbParameter CreateParameter(string name, object value)
        {
            return new MySqlParameter(name, value);
        }

        public  DbCommand CreateCommand(string sql)
        {
            return new MySqlCommand(sql);
        }

        public  DbDataAdapter CreateDataAdapter(DbConnection connection, string sql)
        {
            return new MySqlDataAdapter(sql, (MySqlConnection) connection);
        }


        public  int ExecuteScript(DbConnection conn, RowAffer r, string sql, string delimiter)
        {
            MySqlScript script = new MySqlScript((MySqlConnection) conn, sql);

            if (!string.IsNullOrEmpty(delimiter))
            {
                script.Delimiter = delimiter;
            }

            return script.Execute();
        }

        public string GetConnectionString()
        {
            return this.connectionString;
        }

        public DbParameter[] ParseParameters(IDictionary<string, object> paramMap)
        {
            return DataUtil.ParameterMapToArray(this, paramMap);
        }
    }
}