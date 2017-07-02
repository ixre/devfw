using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JR.DevFw.Data;
using System.Collections.Generic;

namespace JR.DevFw.UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        private class Word
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public static string ID2 { get; set; }
        }

        [TestMethod]
        public void TestMethod1()
        {
            var db = repareDB();
            Test_SQLite(db);

           // Test_OLEDB();

            //Test_SQLServer();
        }

        static DataBaseAccess repareDB()
        {
            const string dbfile = "../../test.db";
           return new DataBaseAccess(DataBaseType.SQLite, String.Format("Data Source={0}", dbfile));
        }

        static void Test_SQLite(DataBaseAccess db)
        {
            db.Use((a, b, c, d) =>
            {
                if (d == null)
                {
                    Console.WriteLine("执行操作成功：" + a + "; SQL=" + b + "; Data=" +DataUtil.ParamsToString(c));
                    return true;
                }

                Console.WriteLine("执行操作失败：" + a + "; SQL=" + b + "; Data=" + " Exception:" + d.Message);
                return false;
            });

            db.ExecuteScript("DROP TABLE IF EXISTS [test_table];CREATE TABLE [test_table](id int,name nvarchar(100))",";");
            db.ExecuteNonQuery("INSERT INTO [test_table] values(1,'刘铭')");

            Random rd = new Random();

            //插入10条数据
            int i = 0;
            do
            {
                int id = rd.Next(1, 30) * rd.Next(1, 10);
                string guid = Guid.NewGuid().ToString();
                db.ExecuteNonQuery(String.Format("INSERT INTO [test_table] values({0},'{1}')",
                    id, guid));
                String guid2 = db.ExecuteScalar("SELECT max(id) FROM [test_table]").ToString();
                db.ExecuteNonQuery(String.Format("UPDATE [test_table] SET [name] = '{0}' WHERE id= '{1}'", id,guid2+"-"+ guid.Substring(0, 6)));
                i++;

            } while (i < 5);


            SqlQuery[] sql = new SqlQuery[5];

            for (int j = 0; j < sql.Length; j++)
            {
                sql[j] = new SqlQuery("INSERT INTO [test_table] ([id],[name]) values(@id,@name)", new object[,]{
                    {"@id",j.ToString()},
                    {"@name","user"+j.ToString()}
                });
            }

            db.ExecuteNonQuery(sql);


            //读取第一条数据
            object obj = db.ExecuteScalar("SELECT  [name] from test_table limit 1,2");
            Console.WriteLine(String.Format("第二条数据的值是:{0}", obj));

            //返回实体读取
            Word w = db.ToEntity<Word>("SELECT [id],[name] from test_table where id=1");
            Console.WriteLine("实体的值为:{0}", w.Name);

            //读取出所有数据
            Console.WriteLine("读取前5条数据");
            db.ExecuteReader("SELECT [id],[name] from test order by id asc limit 0,5", (reader) =>
            {
                while (reader.Read())
                {
                    Console.WriteLine(String.Format("{0}->{1}", reader.GetInt32(0), reader.GetString(1)));
                }
            });

            //读取所有数据
            Console.WriteLine("读取所有数据按ID从小到大排序");

            IList<Word> list = db.ToEntityList<Word>("SELECT [ID],[Name] from test order by id asc");
            foreach (Word word in list)
            {
                Console.WriteLine(String.Format("{0}->{1}", word.ID, word.Name));
            }


            //删除数据库
            // System.IO.File.Delete(dbfile);
        }

        static void Test_SQLServer()
        {
            Console.WriteLine("\r\n测试MSSqlserver..........");

            DataBaseAccess db = new DataBaseAccess(DataBaseType.SQLServer, "server=(local);database=fang;uid=sa;pwd=123000");
            db.ExecuteReader("SELECT * FROM ADS", (reader) =>
            {
                while (reader.Read())
                {
                    Console.WriteLine(reader["Aduser"] + "->" + reader["adtext"]);
                }
            });
        }

        static void Test_OLEDB()
        {
            DataBaseAccess db = new DataBaseAccess(DataBaseType.OLEDB, "Provider=Microsoft.JET.OLEDB.4.0;Data Source=test.mdb");

            db.ExecuteReader("SELECT * FROM test", (reader) =>
            {
                while (reader.Read())
                {
                    Console.WriteLine(reader["id"] + "->" + reader["name"]);
                }
            });
        }
    }
}
