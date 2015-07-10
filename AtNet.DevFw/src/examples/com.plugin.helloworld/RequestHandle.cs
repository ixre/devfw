/**
 * Copyright (C) 2007-2015 OPSoft INC,All rights reseved.
 * Get more infromation of this software,please visit site http://cms.ops.cc
 * 
 * name : RequestHandle.cs
 * author : newmin (new.min@msn.com)
 * date : 2012/12/01 23:00:00
 * description : 
 * history : 
 */
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using AtNet.DevFw;
using AtNet.DevFw.Data;
using AtNet.DevFw.Framework.Extensions;
using AtNet.DevFw.Framework.Graphic;
using AtNet.DevFw.Framework.Web.UI;
using AtNet.DevFw.Template;
using AtNet.DevFw.Web.Plugins;
using Com.Plugin.Core;
using Com.Plugin.Core.Utils;

namespace Com.Plugin
{
    /// <summary>
    /// Description of MobileActions.
    /// </summary>
    public partial class RequestHandle
    {

        private string workSpace;
        private int imgWidth;
        private int imgHeight;
        private string waterPath;
        private long quality = 90L;
        private long compress = 50L;
        private IExtendApp _app;

        internal RequestHandle(IExtendApp app)
        {
            this._app = app;
            //            workSpace = Config.PluginAttrs.WorkSpace;
            //            log = new LogFile(workSpace + "log.txt");
            //
            //            imgWidth = int.Parse(Config.PluginAttrs.Settings["img.width"]);
            //            imgHeight = int.Parse(Config.PluginAttrs.Settings["img.height"]);
            //
            //            if (Config.PluginAttrs.Settings.Contains("quality"))
            //            {
            //                quality = long.Parse(Config.PluginAttrs.Settings["quality"]);
            //            }
            //
            //            if (Config.PluginAttrs.Settings.Contains("compress"))
            //            {
            //                compress = long.Parse(Config.PluginAttrs.Settings["compress"]);
            //            }
            //
            //
            //            float waterMarkPercent = float.Parse(Config.PluginAttrs.Settings["img.water.percent"]);
            //            if (waterMarkPercent == 0F) waterMarkPercent = 1;
            //
            //            waterPath = workSpace + "watermark_resize.png";
            //            Image srcImg = new Bitmap(workSpace + "watermark.png");
            //            byte[] data = GraphicsHelper.DrawBySize(srcImg,
            //                ImageSizeMode.SuitWidth,
            //                (int)(srcImg.Width * waterMarkPercent),
            //                (int)(srcImg.Height * waterMarkPercent),
            //                ImageFormat.Png,
            //                100L,
            //                100L,
            //                null);
            //
            //            using (FileStream fs = new FileStream(waterPath, FileMode.OpenOrCreate, FileAccess.Write))
            //            {
            //                fs.Write(data, 0, data.Length);
            //                fs.Flush();
            //                fs.Dispose();
            //            }
            //
            //            srcImg.Dispose();
        }


        /// <summary>
        /// 首页
        /// </summary>
        /// <param name="context"></param>
        public string Default(HttpContext context)
        {
            return "Hello world! it's call from a plugin.";
        }

        public void Download(HttpContext context)
        {
            if (!RequestProxry.VerifyLogin(context)) return;

            string url = context.Request["url"];
            string filePath = AppDomain.CurrentDomain.BaseDirectory + url;
            if (!File.Exists(filePath))
            {
                context.Response.Write("资源不存在");
                return;
            }

            string fileName = Regex.Match(url, "(\\\\|/)(([^\\\\/]+)\\.(.+))$").Groups[2].Value;
            context.Response.AppendHeader("Content-Type", "");
            context.Response.AppendHeader("Content-Disposition", "attachment;filename=" + fileName);

            const int bufferSize = 100;
            byte[] buffer = new byte[bufferSize];
            int readSize = -1;

            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                while (readSize != 0)
                {
                    readSize = fs.Read(buffer, 0, bufferSize);
                    context.Response.BinaryWrite(buffer);
                }
            }
        }


        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string Upload_post(HttpContext context)
        {
            string uploadfor = context.Request["for"];
            string id = context.Request["upload.id"];
            DateTime dt = DateTime.Now;
            string dir = string.Format("/images/{0:yyyyMMdd}/", dt);
            string name = String.Format("{0}{1:HHss}{2}",
                String.IsNullOrEmpty(uploadfor) ? "" : uploadfor + "_",
                dt, String.Empty.RandomLetters(4));

            string file = new FileUpload(dir, name).Upload();
            if (uploadfor == "image")
            {
                string rootPath = FwCtx.PhysicalPath;


                Bitmap img = new Bitmap(rootPath + file);
                int width, height;
                if (img.Width > img.Height)
                {
                    width = imgWidth;
                    height = imgHeight;
                }
                else
                {
                    width = imgHeight;
                    height = imgWidth;
                }

                byte[] data = GraphicsHelper.DrawBySize(img, ImageSizeMode.CustomSize, width, height, ImageFormat.Png, 100L, 80L, null);
                img.Dispose();
                MemoryStream ms1 = new MemoryStream(data);
                img = new Bitmap(ms1);

                Image water = new Bitmap(waterPath);

                data = GraphicsHelper.MakeWatermarkImage(
                    img,
                    water,
                     WatermarkPosition.Middle
                     );

                ms1.Dispose();
                img.Dispose();

                FileStream fs = File.OpenWrite(rootPath + file);
                BinaryWriter w = new BinaryWriter(fs);
                w.Write(data);
                w.Flush();
                fs.Dispose();
            }

            return "{" + String.Format("url:'{0}'", file) + "}";
        }

        /// <summary>
        /// 验证码
        /// </summary>
        /// <param name="context"></param>
        public void VerifyCode(HttpContext context)
        {
            string word = null;
            VerifyCode v = new VerifyCode();
            var font = v.GetDefaultFont();
            try
            {
                font = new System.Drawing.Font(font.FontFamily, 16);
                v.AllowRepeat = false;
                context.Response.BinaryWrite(v.GraphicDrawImage(4,
                    VerifyWordOptions.Number,
                    !true,
                    font,
                    30,
                    out word));
            }
            catch
            {
                if (font != null)
                {
                    font.Dispose();
                }
            }
            context.Response.ContentType = "Image/Jpeg";
            VerifyCodeManager.AddWord(word);
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string Export_Setup(HttpContext context)
        {
            if (!RequestProxry.VerifyLogin(context)) return null;

            TemplatePage page =this._app.GetPage<Main>("admin/export_setup.html");
            //page.AddVariable("page", new PageVariable());
            page.AddVariable("export", new { setup = ExportHandle.Setup(context.Request["portal"]) });
            return page.ToString();
        }

        /// <summary>
        /// 导出数据源
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string Export_GetExportData_Post(HttpContext context)
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

            TemplatePage page = Cms.Plugins.GetPage<Main>("admin/export_import.html");
            page.AddVariable("page", new PageVariable());
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
