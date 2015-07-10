/**
 * Copyright (C) 2007-2015 OPSoft INC,All rights reseved.
 * Get more infromation of this software,please visit site http://cms.ops.cc
 * 
 * name : IDataLogic.cs
 * author : newmin (new.min@msn.com)
 * date : 2012/12/01 23:00:00
 * description : 
 * history : 
 */

using System.Collections;
using System.Data;

namespace Com.Plugin.Core.ILogic
{
    /// <summary>
    /// Description of ICustomer.
    /// </summary>
    public interface IDataLogic
    {
        DataTable GetQueryView(string queryName, Hashtable hash, int pageSize, int currentPageIndex, out int totalCount);

        DataRow GetTotalView(string queryName, Hashtable hash);

        string GetColumnMappingString(string queryName);

    }
}
