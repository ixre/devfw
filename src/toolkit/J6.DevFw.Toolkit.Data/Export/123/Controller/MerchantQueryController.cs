using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using U1city.ClientCommon;
using U1city.Common;
using U1City.Infrastructure.DataExport;
using U1City.Infrastructure.Extensions;
using U1City.UPos.Server.Contract.Order;
using U1City.UPos.Server.Contract.Shop;
using U1City.UPos.Server.DataTransferObject.Order;
using U1City.UPos.Server.UI.QueryAndExport.Order;
using U1City.UPos.Server.UI.QueryAndExport.ToolKit;

namespace U1City.UPos.Server.UI.Controllers
{
    public class MerchantQueryController : BaseController
    {
        //
        // GET: /MerchantQuery/

        public ActionResult OfflineShopSaleOrdersQuery()
        {
            PagePars pars = new PagePars();
            pars.Add("MerchantID", base.LoginUserManager.CurrentUserInfo.MerchantID);
            ViewBag.Shops = new Client<IOfflineShopContract>().GetInstance().GetShopsOfMerchant(pars);

            return View();
        }


        public ActionResult ShowOrderDetail(string orderNo)
        {
            long merchantId = base.LoginUserManager.CurrentUserInfo.MerchantID;
            OrderDTO order = new Client<IOrderContract>().GetInstance().GetOrderByOrderNo(merchantId, orderNo);

            ViewBag.order = order;

            ViewBag.Json = new U1Json();
            return View();
        }
    }
}
