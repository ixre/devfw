using System.Data;
using U1city.ClientCommon;
using U1City.Infrastructure.DataExport;
using U1city.UiHelp.Mvc;
using U1City.UPos.Server.Contract.Common;
using U1City.UPos.Server.UI.Supports.Security;

namespace U1City.UPos.Server.UI.QueryAndExport.Order
{
    public class OrderExport:BaseDataExportPortal
    {
        private const string queryName = "Query_MerchantOfflineShopSalesOrder";

        private static string[] columns = new[] { "订单号", "门店名称", "销售日期", "销售时间", "销售数量" };


        public override string[] ColumnNames
        {
            get { return columns; }
        }


        public override DataTable GetShemalAndData()
        {
            return DataService.Instance.GetQueryView(
                queryName,
                this.Parameters,
                10000,
                1);
        }

        public override DataRow GetTotalView()
        {
            throw new System.NotImplementedException();
        }
    }
}