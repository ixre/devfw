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

namespace Ops.Data
{
    using System.Data.Common;
    using Mono.Data.Sqlite;

    public class MonoSQLiteFactory : DataBaseFactory
    {
        public MonoSQLiteFactory(string connectionString)
            : base(connectionString)
        {
        }

        public override DbConnection GetConnection()
        {
            return new SqliteConnection(base.connectionString);
        }

        public override DbParameter CreateParameter(string name, object value)
        {
            return new SqliteParameter(name, value);
        }

        public override DbCommand CreateCommand(string sql)
        {
            return new SqliteCommand(sql);
        }

        public override DbDataAdapter CreateDataAdapter(DbConnection connection, string sql)
        {
            return new SqliteDataAdapter(sql, (SqliteConnection) connection);
        }

        public override int ExecuteScript(DbConnection conn, string sql, string delimiter)
        {
            throw new System.NotImplementedException();
        }
    }
}