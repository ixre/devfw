/**
 * Copyright (C) 2007-2015 S1N1.COM,All rights reseved.
 * Get more infromation of this software,please visit site http://cms.ops.cc
 * 
 * name : RequestHandle.cs
 * author : newmin (new.min@msn.com)
 * date : 2012/12/01 23:00:00
 * description : 
 * history : 
 */

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using JR.DevFw;
using JR.DevFw.Framework.Extensions;
using JR.DevFw.Framework.Graphic;
using JR.DevFw.Framework.Web.UI;
using JR.DevFw.PluginKernel;
using Com.Plugin.Core.Utils;
using Com.Plugin.Weixin;
using Senparc.Weixin;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;
using Variable = Com.Plugin.Variables;

namespace Com.Plugin.WebManage
{
    /// <summary>
    ///     Description of MobileActions.
    /// </summary>
    internal class RequestHandle
    {
        private readonly IPlugin _plugin;
        private long compress = 50L;
        private int imgHeight;
        private int imgWidth;
        private long quality = 90L;
        private string waterPath;
        private string workSpace;

        internal RequestHandle(IPlugin plugin)
        {
            _plugin = plugin;

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
        ///     首页
        /// </summary>
        /// <param name="context"></param>
        public string Default(HttpContext context)
        {
            return "Hello world! it's call from a plugin.";
        }

        public void Download(HttpContext context)
        {
            if (!RequestProxry.VerifyLogin(context)) return;

            var url = context.Request["url"];
            var filePath = AppDomain.CurrentDomain.BaseDirectory + url;
            if (!File.Exists(filePath))
            {
                context.Response.Write("资源不存在");
                return;
            }

            var fileName = Regex.Match(url, "(\\\\|/)(([^\\\\/]+)\\.(.+))$").Groups[2].Value;
            context.Response.AppendHeader("Content-Type", "");
            context.Response.AppendHeader("Content-Disposition", "attachment;filename=" + fileName);

            const int bufferSize = 100;
            var buffer = new byte[bufferSize];
            var readSize = -1;

            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                while (readSize != 0)
                {
                    readSize = fs.Read(buffer, 0, bufferSize);
                    context.Response.BinaryWrite(buffer);
                }
            }
        }

        /// <summary>
        ///     上传文件
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string Upload_post(HttpContext context)
        {
            var uploadfor = context.Request["for"];
            var id = context.Request["upload.id"];
            var dt = DateTime.Now;
            var dir = string.Format("/images/{0:yyyyMMdd}/", dt);
            var name = String.Format("{0}{1:HHss}{2}",
                String.IsNullOrEmpty(uploadfor) ? "" : uploadfor + "_",
                dt, String.Empty.RandomLetters(4));

            var file = new FileUpload(dir, name).Upload();
            if (uploadfor == "image")
            {
                var rootPath = FwCtx.PhysicalPath;


                var img = new Bitmap(rootPath + file);
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

                var data = GraphicsHelper.DrawBySize(img, ImageSizeMode.CustomSize, width, height, ImageFormat.Png, 100L,
                    80L, null);
                img.Dispose();
                var ms1 = new MemoryStream(data);
                img = new Bitmap(ms1);

                Image water = new Bitmap(waterPath);

                data = GraphicsHelper.MakeWatermarkImage(
                    img,
                    water,
                    WatermarkPosition.Middle
                    );

                ms1.Dispose();
                img.Dispose();

                var fs = File.OpenWrite(rootPath + file);
                var w = new BinaryWriter(fs);
                w.Write(data);
                w.Flush();
                fs.Dispose();
            }

            return "{" + String.Format("url:'{0}'", file) + "}";
        }

        /// <summary>
        ///     验证码
        /// </summary>
        /// <param name="context"></param>
        public void VerifyCode(HttpContext context)
        {
            string word = null;
            var v = new VerifyCode();
            var font = v.GetDefaultFont();
            try
            {
                font = new Font(font.FontFamily, 16);
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

        /************ 微信  *********************/

        public string Serve(HttpContext context)
        {
            var signature = context.Request["signature"];
            var timestamp = context.Request["timestamp"];
            var nonce = context.Request["nonce"];
            var echostr = context.Request["echostr"];

            //get method - 仅在微信后台填写URL验证时触发
            if (CheckSignature.Check(signature, timestamp, nonce, Variable.Token))
            {
                return echostr; //返回随机字符串则表示验证通过
            }

            return "ok:" + signature + "," + CheckSignature.GetSignature(timestamp, nonce, Variable.Token) + "。" +
                   "如果你在浏览器中看到这句话，说明此地址可以被作为微信公众账号后台的Url，请注意保持Token一致。";
        }

        public string Serve_POST(HttpContext context)
        {
            var signature = context.Request["signature"];
            var timestamp = context.Request["timestamp"];
            var nonce = context.Request["nonce"];
            var echostr = context.Request["echostr"];

           
            //post method - 当有用户想公众账号发送消息时触发
            if ( !CheckSignature.Check(signature, timestamp, nonce, Variable.Token))
            {
                return "参数错误！";
            }

            try
            {
                //自定义MessageHandler，对微信请求的详细判断操作都在这里面。
                var messageHandler = new CustomMessageHandler(context.Request.InputStream, null);
                //执行微信处理过程
                messageHandler.Execute();
                //输出结果
                XDocument doc = messageHandler.ResponseDocument;
                if (doc != null)
                {
                    return doc.ToString();
                }
                return messageHandler.ResponseDocument.ToString();
            }
            catch (Exception exc)
            {
                this._plugin.Logln("[Exception]:"+exc.Message+"\n"+exc.StackTrace);
            }
            return "";
        }

        public string ServePost(HttpContext context)
        {
            return Serve_POST(context);
        }

        public string WxApi(HttpContext context)
        {
            return Serve(context);
        }

        public string WxApi_POST(HttpContext context)
        {
            return Serve_POST(context);
        }

        public string Auth(HttpContext context)
        {
            string returnUrl = context.Request["return_url"];
            if (returnUrl == "")
            {
                return "mission parmeter return_url";
            }

            string authProcUrl = WeixinHelper.GetAuthProcUrl(returnUrl);
            string authUrl = OAuth.GetAuthorizeUrl(Variables.AppId, authProcUrl,
                "OK", OAuthScope.snsapi_base, "code");

            //authUrl = authProcUrl;
            context.Response.Redirect(authUrl, true);
            return "";
        }

        public string AuthProc(HttpContext context)
        {
            string returnUrl = context.Request["return_url"];
            string code = context.Request["code"];

            
            //string redirectUrl1 =returnUrl+ (returnUrl.Contains("?") ? "&" : "?") + "open_id=12344";
            //context.Response.Redirect(redirectUrl1);
            //return "";

           OAuthAccessTokenResult result = OAuth.GetAccessToken(Variables.AppId, 
               Variables.AppSecret, code);
            if (result.errcode != ReturnCode.请求成功)
            {
                return "错误"+result.errmsg;
            }

            string redirectUrl =returnUrl+ (returnUrl.Contains("?") ? "&" : "?") +
                "open_id=" + result.openid;

            context.Response.Redirect(redirectUrl);
            return "";
        }

        public string TestId(HttpContext context)
        {
            return "open_id:"+context.Request["open_id"];
        }
    }
}