using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using U1City.Infrastructure.DataExport;
using U1City.UPos.Server.UI.Supports.Security;

namespace U1City.UPos.Server.UI.QueryAndExport.ToolKit
{
    public static class ExportHelper
    {
        public static IDataExportPortal GetPortal(string className,string _params,string[] columnNames)
        {
            IDataExportPortal portal = ExportUtil.GetNotNullPortal(className);
             string[] splitArr;

            object[,] data;
            string[] paramsArr = _params.Split(';');
            
            data = new object[paramsArr.Length + 1, 2];

            //添加渠道编号
            data[0, 0] = "merchantId";
            data[0, 1] = new LoginUserManager().CurrentUserInfo.MerchantID;

            //添加传入的参数
            for (int i = 0; i < paramsArr.Length; i++)
            {
                splitArr = paramsArr[i].Split(':');
                data[i + 1, 0] = splitArr[0];
                data[i + 1, 1] = paramsArr[i].Substring(splitArr[0].Length+1);
            }

            portal.Parameters = new ExportParams(data, columnNames);

            return portal;
        }

        public static IDataExportPortal GetPortal(Type type, string _params)
        {
            return GetPortal(type.FullName, _params, null);
        }
    }
}