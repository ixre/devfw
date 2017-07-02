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

// https://system.data.sqlite.org/index.html/doc/trunk/www/downloads.wiki
// Œ»∂®∞Ê£∫
// http://system.data.sqlite.org/downloads/1.0.95.0/sqlite-netFx40-binary-bundle-Win32-2010-1.0.95.0.zip
// http://system.data.sqlite.org/downloads/1.0.95.0/sqlite-netFx40-binary-bundle-x64-2010-1.0.95.0.zip
using System.Data.Common;
using System.Data.SQLite;

namespace JR.DevFw.Data
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

        public override int ExecuteScript(DbConnection conn, RowAffer r, string sql, string delimiter)
        {
            int result = 0;
            string[] array = sql.Split(';');
            foreach (string s in array)
            {
                result += r(s);
            }
            return result;
        }
    }
}