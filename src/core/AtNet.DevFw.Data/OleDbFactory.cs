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

using System.Data.Common;
using System.Data.OleDb;

namespace AtNet.DevFw.Data
{
    public class OleDbFactory : DataBaseFactory
    {
        public OleDbFactory(string connectionString)
            : base(connectionString)
        {
        }

        public override DbConnection GetConnection()
        {
            return new OleDbConnection(base.connectionString);
        }

        public override DbParameter CreateParameter(string name, object value)
        {
            return new OleDbParameter(name, value);
        }

        public override DbCommand CreateCommand(string sql)
        {
            return new OleDbCommand(sql);
        }

        public override DbDataAdapter CreateDataAdapter(DbConnection connection, string sql)
        {
            return new OleDbDataAdapter(sql, (OleDbConnection) connection);
        }

        public override int ExecuteScript(DbConnection conn, string sql, string delimiter)
        {
            throw new System.NotImplementedException();
        }
    }
}