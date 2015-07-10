using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using U1City.Infrastructure.DataExport;
using U1City.Infrastructure.DataExport.ExportProvider;
using U1City.UPos.Server.UI.QueryAndExport.Order;
using U1City.UPos.Server.UI.QueryAndExport.ToolKit;

namespace U1City.UPos.Server.UI.Controllers
{
    public class ExportController :BaseController
    {
        public ActionResult Setup(string portal)
        {
            ViewBag.SetupHtml= WebExportOptionUIBuilder.BuildColumnCheckHtml(portal);
            return View();
        }

        [AcceptVerbs("POST")]
        public JsonResult GetExportData()
        {
            IDataExportPortal portal = ExportHelper.GetPortal(this.Request["portal"], this.Request["params"], null);
            return new U1Json(portal.GetShemalAndData());
        }

        public void ProcessExport(string portal, string columns)
        {
            IList<string> columnNames;
            IDataExportProvider provider;
            string extension;

            //获取导出提供者
            switch (this.Request["exportFormat"] ?? "excel")
            {
                default:
                case "excel":
                    provider = new ExcelExportProvider();
                    extension = "xls";
                    break;
                case "csv":
                    provider = new CsvExportProvider();
                    extension = "csv";
                    break;
                case "txt":
                    provider = new TextExportProvider();
                    extension = "txt";
                    break;
            }


            //获取列名
            Regex reg = new Regex("^columnNames\\[\\d+\\]$", RegexOptions.IgnoreCase);
            columnNames = new List<string>();

            foreach (string key in this.Request.QueryString.Keys)
            {
                if (reg.IsMatch(key))
                {
                    columnNames.Add(this.Request.QueryString[key]);
                }
            }



            IDataExportPortal _portal = ExportHelper.GetPortal(portal, this.Request["params"], columnNames.ToArray());
                


            byte[] bytes = DataExportDirector.Export(_portal, provider);

            this.Response.BinaryWrite(bytes);
            this.Response.ContentType = "application/octet-stream";
            this.Response.AppendHeader("Content-Disposition", String.Format("attachment;filename=\"{0:yyyyMMdd-hhssfff}.{1}\"",
                DateTime.Now,
                extension));

        }

        [AcceptVerbs("POST")]
        public JsonResult GetTotalView()
        {
            return new U1Json(new { OrderNum = 1, SaleGoodsNum =2});
            IDataExportPortal portal = ExportHelper.GetPortal(this.Request["portal"], this.Request["params"], null);
            return new U1Json(portal.GetTotalView());
        }
         
    }
}
