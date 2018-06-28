//
//
//  Copryright 2011 @ S1N1.COM.All rights reseved.
//
//  Project : OPS.Data
//  File Name : OLEDBFactory.cs
//  Date : 8/19/2011
//  Author : ¡ı√˙
//
//

using System.Collections.Generic;
using System.Data.Common;
using System.Data.OleDb;

namespace JR.DevFw.Data
{
    public class OleDbFactory : IDbDialect
    {
        private string connectionString;

        public OleDbFactory(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public  DbConnection GetConnection()
        {
            return new OleDbConnection(this.connectionString);
        }

        public  DbParameter CreateParameter(string name, object value)
        {
            return new OleDbParameter(name, value);
        }

        public  DbCommand CreateCommand(string sql)
        {
            return new OleDbCommand(sql);
        }

        public  DbDataAdapter CreateDataAdapter(DbConnection connection, string sql)
        {
            return new OleDbDataAdapter(sql, (OleDbConnection) connection);
        }


        public  int ExecuteScript( DbConnection conn, RowAffer r, string sql, string delimiter)
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