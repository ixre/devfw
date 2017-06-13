using System;
using System.Data;
using System.IO;
using System.Web;
using JR.DevFw.Data;
using JR.DevFw.Framework.Extensions;
using JR.DevFw.Template;
using Com.Plugin.Core.Utils;
using JR.DevFw.Framework.Web.UI;
using JR.DevFw.PluginKernel;
using JR.DevFw.Web;
using JR.DevFw.Web.Plugin;

namespace Com.Plugin.Core
{
    internal class BaseHandle
    {
        private IPluginApp _app;
        private IPlugin _plugin;

        public BaseHandle(IPluginApp app, IPlugin plugin)
        {
            this._app = app;
            this._plugin = plugin;
        }


        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string Upload_post(HttpContext context)
        {
            string uploadfor = context.Request["for"];
            DateTime dt = DateTime.Now;
            string dir = string.Format("{0}{1}/uploads/{2:yyyyMMdd}/", 
                PluginConfig.PLUGIN_DIRECTORY,
                Config.PluginAttr.WorkIndent , dt);
            string name = String.Format("{0}{1:HHss}{2}",
                String.IsNullOrEmpty(uploadfor) ? "" : uploadfor + "_",
                dt, String.Empty.RandomLetters(4));

            string file = new FileUpload(dir, name).Upload();
            return "{" + String.Format("url:'{0}'", file) + "}";
        }


        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string Export_Setup(HttpContext context)
        {
            if (!RequestProxry.VerifyLogin(context)) return null;

            TemplatePage page = this._app.GetPage(this._plugin,"mg/export_setup.html");
            page.AddVariable("page", new TemplatePageVariable());
            page.AddVariable("export", new { setup = ExportHandle.Setup(context.Request["portal"]) });
            return page.ToString();
        }

        /// <summary>
        /// 导出数据源
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string Export_GetExportData_post(HttpContext context)
        {
            return ExportHandle.GetExportData(context);
        }

        public void Export_ProcessExport(HttpContext context)
        {
            if (!RequestProxry.VerifyLogin(context)) return;
            ExportHandle.ProcessExport(context);
        }

        public string Export_Import(HttpContext context)
        {
            if (!RequestProxry.VerifyLogin(context)) return null;

            TemplatePage page = this._app.GetPage(this._plugin, "admin/export_import.html");
            page.AddVariable("page", new TemplatePageVariable());
            page.AddVariable("case", new { json = new object() });
            return page.ToString();
        }

        public string Export_Import_post(HttpContext context)
        {
            // try
            // {
            FileInfo file = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + context.Request["file"]);
            DataTable dt = NPOIHelper.ImportFromExcel(file.FullName).Tables[0];
            //DataView dv = dt.DefaultView;
            //dv.Sort = "财务编号 ASC";
            SqlQuery[] querys = new SqlQuery[dt.Rows.Count];
            int i = 0;
            DateTime importTime = DateTime.Now;
            foreach (DataRow dr in dt.Rows)
            {
                const string insertSql = @" ";
            }

            int rows = IocObject.GetDao().ExecuteNonQuery(querys);
            if (rows < 0) rows = 0;
            return "{result:true,message:'导入完成,共导入" + rows.ToString() + "条！'}";
            /* }
             catch (Exception exc)
             {
                 return "{result:false,message:'"+exc.Message+"！'}";
             }*/
        }
    }
}
