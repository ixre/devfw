
using JR.DevFw.Data;

namespace Com.Plugin.Core
{
   public  class DbGenerator
    {
       private string _connString;

       internal DbGenerator()
       {
       }

       /// <summary>
       /// 数据库访问对象
       /// </summary>
       public DataBaseAccess New()
       {
           if (_connString == null)
           {
               _connString = Config.PluginAttr.Settings["db_conn"];
               //if (_connString.Contains("$ROOT"))
               //{
               //    string dir = PluginUtil.GetAttribute<Main>().WorkSpace;
               //    _connString = _connString.Replace("$ROOT", dir.Substring(0, dir.Length - 1));
               //}
           }
           return new DataBaseAccess(
               DataBaseType.SQLServer,
               _connString
               );
       }
    }
}
