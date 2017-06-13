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
    public class SqlQuery
    {
        private object[,] _parameters;


        public SqlQuery(string sql)
        {
            this.Sql = sql;
        }

        public SqlQuery(string sql, object[,] parameters)
        {
            if (parameters != null)
            {
                if (parameters.GetLength(0) != 0 && parameters.GetLength(1) != 2)
                {
                    throw new ArgumentOutOfRangeException("Parameters", "多纬数组的二维长度必须为2");
                }
            }
            this._parameters = parameters;

            this.Sql = sql;
        }

        /// <summary>
        /// SQL语句
        /// </summary>
        public String Sql { get; set; }

        /// <summary>
        /// 参数
        /// 
        ///    this.Parameters = new object[,]{
        ///         {"@age","age"},
        ///        {"@name","name"}
        ///    };
        /// </summary>
        public object[,] Parameters
        {
            get { return this._parameters; }
        }


        /// <summary>
        /// 转换为参数
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public DbParameter[] ToParams(IDataBase db)
        {
            DbParameter[] parameters = new DbParameter[this.Parameters.GetLength(0)];

            for (int i = 0; i < parameters.Length; i++)
            {
                parameters[i] = db.CreateParameter(this.Parameters[i, 0].ToString(), this.Parameters[i, 1]);
            }

            return parameters;
        }
    }
}