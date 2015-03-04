//
//
//  Copryright 2011 @ OPSoft INC.All rights reseved.
//
//  Project : OPS.Data
//  File Name : SQLiteFactory.cs
//  Date : 8/19/2011
//  Author : ¡ı√˙
//
//

using System.Data.Common;
using System.Data.SQLite;

namespace AtNet.DevFw.Data
{
    public class SQLiteFactory : DataBaseFactory
    {
        public SQLiteFactory(string connectionString)
            : base(connectionString)
        {
        }

        public override DbConnection GetConnection()
        {
            return new SQLiteConnection(base.connectionString);
        }

        public override DbParameter CreateParameter(string name, object value)
        {
            return new SQLiteParameter(name, value);
        }

        public override DbCommand CreateCommand(string sql)
        {
            return new SQLiteCommand(sql);
        }

        public override DbDataAdapter CreateDataAdapter(DbConnection connection, string sql)
        {
            return new SQLiteDataAdapter(sql, (SQLiteConnection) connection);
        }


        public override int ExecuteScript(DbConnection conn, string sql, string delimiter)
        {
            throw new System.NotImplementedException();
        }
    }
}