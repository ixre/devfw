//
//
//  Copryright 2011 @ S1N1.COM.All rights reseved.
//
//  Project : OPS.Data
//  File Name : SQLServerFactory.cs
//  Date : 8/19/2011
//  Author : ¡ı√˙
//
//

using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;

namespace JR.DevFw.Data
{
    public class SqlServerFactory : IDbDialect
    {
        private string connectionString;

        public SqlServerFactory(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public  DbConnection GetConnection()
        {
            return new SqlConnection(this.connectionString);
        }

        public  DbParameter CreateParameter(string name, object value)
        {
            return new SqlParameter(name, value);
        }

        public  DbCommand CreateCommand(string sql)
        {
            return new SqlCommand(sql);
        }

        public  DbDataAdapter CreateDataAdapter(DbConnection connection, string sql)
        {
            return new SqlDataAdapter(sql, (SqlConnection) connection);
        }
        public  int ExecuteScript(DbConnection conn, RowAffer r, string sql, string delimiter)
        {
            int result = 0;
            string[] array = sql.Split(';');
            foreach (string s in array)
            {
                result += r(s);
            }
            return result;
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