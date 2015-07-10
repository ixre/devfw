using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using U1City.Infrastructure.DataExport;
using U1City.Infrastructure.Inject;
using U1City.UPos.Server.Contract.Common;
using U1City.UPos.Server.IQuery;

namespace U1City.UPos.Server.Services.Common
{
    [Injectable]
    public class DataQueryService:IDataQueryServiceContract
    {
        [Injected] private ICommonDataQuery _commonDataQuery;

        public DataTable GetQueryView(string sqlItemName, ExportParams exportParams, int pageSize, int pageIndex)
        {
            return _commonDataQuery.GetQuery(sqlItemName, exportParams,pageSize,pageIndex);

        }
    }
}
