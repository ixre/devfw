using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using Ops.Data;

namespace Ops.Data.Test
{
    class Program
    {
        private class Word
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public static string ID2 { get; set; }
        }

        static void Main(string[] args)
        {
            

            Test_SQLite();

            Test_OLEDB();

            //Test_SQLServer();

           
            Console.ReadLine();
        }

        static void Test_SQLite()
        {
            const string dbfile = "test.db";

            DataBaseAccess db = new DataBaseAccess(DataBaseType.SQLite, String.Format("Data Source={0}", dbfile));


            // db.ToEntityList<Word>("SELECT * FROM test");

            //return;

            db.ExecuteNonQuery("CREATE TABLE [test](id int,name nvarchar(100))");
            db.ExecuteNonQuery("INSERT INTO [test] values(1,'刘铭')");
           
            Random rd = new Random();

            //插入10条数据
            int i = 0;
            do
            {
                try
                {
                    db.ExecuteNonQuery(String.Format("INSERT INTO [test] values({0},'{1}')",
                        rd.Next(1, 30) * rd.Next(1, 10),
                        Guid.NewGuid().ToString()));
                }
                catch
                {
                }

                i++;

            } while (i < 5);


           SqlQuery[] sql=new SqlQuery[5];

            for (int j = 0; j < sql.Length; j++)
            {
                sql[j] = new SqlQuery("INSERT INTO [test] values(@id,@name)", new object[,]{
                    {"@id",j.ToString()},
                    {"@name","user"+j.ToString()}
                });
            }

            db.ExecuteNonQuery(sql);


            //读取第一条数据
            object obj = db.ExecuteScalar("SELECT  [name] from test limit 1,2");
            Console.WriteLine(String.Format("第二条数据的值是:{0}", obj));

            //返回实体读取
            Word w = db.ToEntity<Word>("SELECT [id],[name] from test where id=1");
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
