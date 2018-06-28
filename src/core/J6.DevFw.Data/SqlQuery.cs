//
//
//  Copryright 2013 @ S1N1.COM.All rights reseved.
//
//  Project : OPS.Data
//  File Name : SqlEntity.cs
//  Date : 05/27/2013
//  Author : 刘铭
//
//

using System;
using System.Data.Common;

namespace JR.DevFw.Data
{
    /// <summary>
    /// SQL查询实体
    /// </summary>
    public struct SqlQuery
    {
        private DbParameter[] parameters;
        private Object[,] dataArray;

        public SqlQuery(string sql)
        {
            this.Sql = sql;
            this.parameters = new DbParameter[0] { };
            this.dataArray = null;
        }

        public SqlQuery(string sql,Object[,] data)
        {
            this.Sql = sql;
            this.parameters = null;
            this.dataArray = data;
        }

        public SqlQuery(string sql, DbParameter[] parameters)
        {
            this.Sql = sql;
            this.parameters = parameters;
            this.dataArray = null;
        }

        /// <summary>
        /// SQL语句
        /// </summary>
        public String Sql { get; set; }

        /// <summary>
        /// 参数
        /// </summary>
        public DbParameter[] Parameters
        {
            get { return this.parameters; }
        }


        /// <summary>
        /// 转换参数
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public bool Parse(IDbDialect db)
        {
            if(this.parameters == null && this.dataArray != null)
            {
                this.parameters = DataUtil.ToParams(db, this.dataArray);
                return true;
            }
            return false;
        }
    }
}