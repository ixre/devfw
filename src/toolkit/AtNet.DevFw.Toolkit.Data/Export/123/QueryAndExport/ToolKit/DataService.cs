using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using U1city.ClientCommon;
using U1City.UPos.Server.Contract.Common;

namespace U1City.UPos.Server.UI.QueryAndExport
{
    public class DataService
    {
        public static IDataQueryServiceContract Instance
        {
            get { return new Client<IDataQueryServiceContract>().GetInstance(); }
        }

    }
}
