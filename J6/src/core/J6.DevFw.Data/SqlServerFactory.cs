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

using System.Data.Common;
using System.Data.SqlClient;

namespace J6.DevFw.Data
{
    public class SqlServerFactory : DataBaseFactory
    {
        public SqlServerFactory(string connectionString)
            : base(connectionString)
        {
        }

        public override DbConnection GetConnection()
        {
            return new SqlConnection(base.connectionString);
        }

        public override DbParameter CreateParameter(string name, object value)
        {
            return new SqlParameter(name, value);
        }

        public override DbCommand CreateCommand(string sql)
        {
            return new SqlCommand(sql);
        }

        public override DbDataAdapter CreateDataAdapter(DbConnection connection, string sql)
        {
            return new SqlDataAdapter(sql, (SqlConnection) connection);
        }

        public override int ExecuteScript(DbConnection conn, string sql, string delimiter)
        {
            throw new System.NotImplementedException();
        }
    }
}