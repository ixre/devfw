//
// Copyright (C) 2007-2012 S1N1.COM,All rights reseved.
// 
// Project: OPS.Data.Extensions
// FileName : ObjectExtensions.cs
// Author : PC-CWLIU (new.min@msn.com)
// Create : 2012/05/24 17:58:32
// Description :
//
// Get infromation of this software,please visit our site http://www.ops.cc
//
//

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using J6.DevFw.Data.Extensions;

namespace J6.DevFw.Data
{
    /// <summary>
    /// 数据实用辅助类
    /// </summary>
    public static class DataUtility
    {
        /// <summary>
        /// 转换DataTable为实体列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <returns></returns>
        public static IList<T> ConvertToEntityList<T>(DataTable table) where T : new()
        {
            return table.ToEntityList<T>();
        }

        /// <summary>
        ///将DataReader中的行转换成实体集合(仅拷贝实体与数据表列名相同的数据)
        /// </summary>
        public static IList<T> ConvertToEntityList<T>(DbDataReader reader) where T : new()
        {
            return reader.ToEntityList<T>();
        }

        public static T ConvertToEntity<T>(DataRow row) where T : new()
        {
            return row.ToEntity<T>();
        }

        /// <summary>
        ///将DataReader转换成实体(仅拷贝实体与数据表列名相同的数据)
        /// </summary>
        public static T ConvertToEntity<T>(DbDataReader reader) where T : new()
        {
            return reader.ToEntity<T>();
        }

        /// <summary>
        /// 生成SQL语句参数对象数组
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="obj"></param>
        /// <param name="db">数据库类型</param>
        /// <param name="fields">字段，用空格隔开多个字段。参数名称需与字段名称一致！</param>
        /// <returns></returns>
        public static DbParameter[] GetDbParameter<T>(T obj, DataBaseType dbtype, String fields)
        {
            return obj.GetDbParameter(dbtype, fields);
        }
    }
}