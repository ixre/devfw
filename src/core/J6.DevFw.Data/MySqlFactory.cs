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

namespace JR.DevFw.Data
{
    public class MySqlFactory : DataBaseFactory
    {
        public MySqlFactory(string connectionString)
            : base(connectionString)
        {
        }

        public override DbConnection GetConnection()
        {
            return new MySqlConnection(base.connectionString);
        }

        public override DbParameter CreateParameter(string name, object value)
        {
            return new MySqlParameter(name, value);
        }

        public override DbCommand CreateCommand(string sql)
        {
            return new MySqlCommand(sql);
        }

        public override DbDataAdapter CreateDataAdapter(DbConnection connection, string sql)
        {
            return new MySqlDataAdapter(sql, (MySqlConnection) connection);
        }


        public override int ExecuteScript(DbConnection conn, string sql, string delimiter)
        {
            MySqlScript script = new MySqlScript((MySqlConnection) conn, sql);

            if (!string.IsNullOrEmpty(delimiter))
            {
                script.Delimiter = delimiter;
            }

            return script.Execute();
        }
    }
}