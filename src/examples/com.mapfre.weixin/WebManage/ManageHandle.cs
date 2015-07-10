using System;
using System.Web;
using AtNet.DevFw.Framework.Extensions;
using AtNet.DevFw.PluginKernel;
using AtNet.DevFw.Template;
using AtNet.DevFw.Web.Plugins;
using Com.Plugin.Core;
using Com.Plugin.Entity;
using Com.Plugin.Weixin;
using Newtonsoft.Json;
using Senparc.Weixin.MP;

namespace Com.Plugin.WebManage
{
    internal class ManageHandle
    {
        private IPlugin _plugin;
        private readonly BaseHandle _baseHandle;
        private IExtendApp _app;

        public ManageHandle(IExtendApp app,IPlugin plugin)
        {
            this._app = app;
            this._plugin = plugin;
            this._baseHandle = new BaseHandle(app,plugin);
        }

        #region base

        public string UploadCgi_post(HttpContext context)
        {
            return this._baseHandle.Upload_post(context);
        }

        public string Export_Setup(HttpContext context)
        {
            return this._baseHandle.Export_Setup(context);
        }

        public string Export_GetExportData_post(HttpContext context)
        {
            return this._baseHandle.Export_GetExportData_post(context);
        }

        public void Export_ProcessExport(HttpContext context)
        {
            this._baseHandle.Export_ProcessExport(context);
        }

        #endregion

        public string Test(HttpContext context)
        {
            return "it's working!";
        }

        public string BasicSetting(HttpContext context)
        {
            PluginPackAttribute attr = this._plugin.GetAttribute();
            TemplatePage page =this._app.GetPage(this._plugin,"mg/basic_setting.html");

            string url = String.Format("http://{0}/{1}.sh.aspx/serve",
                context.Request.Url.Host,attr.WorkIndent);
            string token = attr.Settings["Weixin_Token"] ??
                           String.Empty.RandomLetters(6);
            string appId = attr.Settings["Weixin_AppId"] ?? "";
            string appSecret = attr.Settings["Weixin_AppSecret"] ?? "";
            string appEncodeString =attr.Settings["Weixin_AppEncodeString"] ?? "";
            string wlc = attr.Settings["Weixin_WelcomeMessage"]??"";
            string enterMessage = attr.Settings["Weixin_EnterMessage"]??"";
            string defaultRspMesssage=  attr.Settings["Weixin_DefaultResponseMessage"]??"";

            page.AddVariable("data", new
            {
                url = url,
                token = token,
                appId = appId,
                appSecret = appSecret,
                aes = appEncodeString,
                welcomeMessage = wlc,
                enterMessage = enterMessage,
                defaultRspMessage = defaultRspMesssage,
            });
            return page.ToString();
        }

        public string BasicSetting_post(HttpContext context)
        {
            PluginPackAttribute attr = this._plugin.GetAttribute();
            var form = context.Request.Form;
            attr.Settings["Weixin_Token"] = form["Weixin_Token"];
            attr.Settings["Weixin_AppId"] = form["Weixin_AppId"];
            attr.Settings["Weixin_AppSecret"] = form["Weixin_AppSecret"];
            attr.Settings["Weixin_AppEncodeString"] = form["Weixin_AppEncodeString"];
            attr.Settings["Weixin_WelcomeMessage"] = form["Weixin_WelcomeMessage"];
            attr.Settings["Weixin_EnterMessage"] = form["Weixin_EnterMessage"];
            attr.Settings["Weixin_DefaultResponseMessage"] = form["Weixin_DefaultResponseMessage"];

            attr.Settings.Flush();
            Config.InitWeixin(attr.Settings);

            return "{result:true,message:'修改成功'}";
        }

        public string Menu(HttpContext context)
        {
            TemplatePage page =this._app.GetPage(this._plugin,"mg/menu.html");
            String menuJson = JsonConvert.SerializeObject(WeixinHelper.GetMenuTreeNode());
            page.AddVariable("menu", menuJson);
            return page.ToString();
        }

        public string EditMenu(HttpContext context)
        {
            int pi = 0;
            int si = -1;
            string mi = context.Request["mi"];
            if (mi.Contains("-"))
            {
                string[] arr = mi.Split('-');
                pi = int.Parse(arr[0]) - 1;
                si = int.Parse(arr[1]) - 1;
            }
            else
            {
                pi = int.Parse(mi) - 1;
            }

            TemplatePage page =this._app.GetPage(this._plugin,"mg/menu_edit.html");
            String entityJson = JsonConvert.SerializeObject(WeixinHelper.GetMenuNode(pi, si));
            page.AddVariable("entity", entityJson);
            page.AddVariable("pi", pi);
            page.AddVariable("si", si);
            return page.ToString();
        }

        public string CreateMenu(HttpContext context)
        {
            int pi = int.Parse(context.Request["pi"]) - 1;
            TemplatePage page = this._app.GetPage(this._plugin,"mg/menu_create.html");
            String menuJson = JsonConvert.SerializeObject(WeixinHelper.GetMenuTreeNode());
            page.AddVariable("menu", menuJson);
            page.AddVariable("pi", pi);
            return page.ToString();
        }

        public string EditMenu_post(HttpContext context)
        {
            int pi = int.Parse(context.Request["pi"]);
            int si = int.Parse(context.Request["si"]);
            MenuFull_RootButton button = WeixinHelper.GetMenuNode(pi, si);
            button.name = context.Request["name"];
            button.type = context.Request["type"];
            button.key = context.Request["key"];
            button.url = context.Request["url"];
            WeixinHelper.SaveMenuNode(pi, si, button);
            return "{result:true,message:'修改成功'}";
        }

        public string CreateMenu_post(HttpContext context)
        {
            int pi = int.Parse(context.Request["pi"]);
            MenuFull_RootButton button = new MenuFull_RootButton();
            button.name = context.Request["name"];
            button.type = context.Request["type"];
            button.key = context.Request["key"];
            button.url = context.Request["url"];
            WeixinHelper.CreateMenuNode(pi, button);
            return "{result:true,message:'修改成功'}";
        }

        public string DelMenu_post(HttpContext context)
        {
            int pi = 0;
            int si = -1;
            string mi = context.Request["mi"];
            if (mi.Contains("-"))
            {
                string[] arr = mi.Split('-');
                pi = int.Parse(arr[0]) - 1;
                si = int.Parse(arr[1]) - 1;
            }
            else
            {
                pi = int.Parse(mi) - 1;
            }
            WeixinHelper.DelMenu(pi, si);
            return "{result:true,message:'删除成功'}";
        }


        public string ApplyMenu_post(HttpContext context)
        {
            try
            {
                String msg = WeixinHelper.PostApplyMenu();
                if (msg != "ok")
                {
                    throw new Exception(msg);
                }
            }
            catch (Exception exc)
            {
                return "{result:false,message:'" + exc.Message + "'}";
            }
            return "{result:true,message:'操作成功'}";
        }


        /// <summary>
        /// 素材管理
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string ResourceList(HttpContext context)
        {
            TemplatePage page = this._app.GetPage(this._plugin,"mg/resource_list.html");
            return page.ToString();
        }

        public string CheckResKey_post(HttpContext context)
        {
            int id = int.Parse(context.Request["id"]);
            string resKey = context.Request["resKey"];

            bool result = IocObject.WeixinRes.CheckResKey(id, resKey);

            return result ? "{result:true}" : "{result:false}";

        }

        #region TextRes

        public string EditTextRes(HttpContext context)
        {
            int id = int.Parse(context.Request["id"]);
            TemplatePage page = this._app.GetPage(this._plugin, "mg/text_res.html");
            TextRes res = IocObject.WeixinRes.GetResById(id) as TextRes;
            if (res == null)
            {
                page.AddVariable("content", "");
                page.AddVariable("entity", "{}");
            }
            else
            {

                page.AddVariable("content", res.Content == null ? "" : res.Content);
                page.AddVariable("entity", JsonConvert.SerializeObject(res));
            }

            return page.ToString();
        }

        public string CreateTextRes(HttpContext context)
        {
            TemplatePage page = this._app.GetPage(this._plugin, "mg/text_res.html");

            page.AddVariable("entity", "{Id:0}");
            page.AddVariable("content", "");

            return page.ToString();
        }

        public string SaveTextRes_post(HttpContext context)
        {
            bool isCreate = false;
            IWxRes res;
            try
            {
                TextRes tres = context.Request.Form.ConvertToEntity<TextRes>();
                res = tres;

                if (tres.Id > 0)
                {
                    TextRes ores = IocObject.WeixinRes.GetResById(tres.Id) as TextRes;
                    tres.TypeId = ores.TypeId;
                    tres.TypeName = ores.TypeName;
                    tres.CreateTime = ores.CreateTime;
                }
                else
                {
                    isCreate = true;
                    tres.CreateTime = DateTime.Now;
                    tres.TypeName = "文本";
                    tres.TypeId = 1;
                }
                tres.UpdateTime = DateTime.Now;
                tres.Content = context.Request["Content"];
                int id = tres.Save();
            }
            catch (Exception exc)
            {
                return "{result:false,message:'" + exc.Message + "'}";
            }
            return "{result:true,message:'" + (isCreate ? "添加成功" : "保存成功") + "'}";
        }

        #endregion

        #region ArticleRes

        public string CreateArticleRes(HttpContext context)
        {
            TemplatePage page = this._app.GetPage(this._plugin, "mg/article_res.html");

            page.AddVariable("entity", "{Id:0}");
            page.AddVariable("data", new
            {
                content = "",
                description = ""
            });

            return page.ToString();
        }

        public string EditArticleRes(HttpContext context)
        {
            int id = int.Parse(context.Request["id"]);
            TemplatePage page = this._app.GetPage(this._plugin, "mg/article_res.html");
            ArticleRes res = IocObject.WeixinRes.GetResById(id) as ArticleRes;
            res.Items = null;
            page.AddVariable("entity", res != null ? JsonConvert.SerializeObject(res) : "{}");

            return page.ToString();
        }

        public string SaveArticleRes_post(HttpContext context)
        {
            bool isCreate = false;
            IWxRes res;
            try
            {
                ArticleRes tres = context.Request.Form.ConvertToEntity<ArticleRes>();
                res = tres;

                if (tres.Id > 0)
                {
                    ArticleRes ores = IocObject.WeixinRes.GetResById(tres.Id) as ArticleRes;
                    tres.TypeId = ores.TypeId;
                    tres.TypeName = ores.TypeName;
                    tres.CreateTime = ores.CreateTime;
                }
                else
                {
                    isCreate = true;
                    tres.CreateTime = DateTime.Now;
                    tres.TypeName = "图文";
                    tres.TypeId = 2;
                }
                tres.UpdateTime = DateTime.Now;
                int id = res.Save();
            }
            catch (Exception exc)
            {
                return "{result:false,message:'" + exc.Message + "'}";
            }
            return "{result:true,message:'" + (isCreate ? "添加成功" : "保存成功") + "'}";
        }

        public string ArticleItems(HttpContext context)
        {
            string resId = context.Request["res_id"];
            TemplatePage page = this._app.GetPage(this._plugin, "mg/article_itemlist.html");
            page.AddVariable("resId", resId);
            return page.ToCompressedString();
        }

        public string CreateArticleItem(HttpContext context)
        {
            string resId = context.Request["res_id"];
            TemplatePage page = this._app.GetPage(this._plugin, "mg/article_item.html");
            page.AddVariable("entity", "{Id:0,Enabled:true}");
            page.AddVariable("resId", resId);
            page.AddVariable("content", "");
            page.AddVariable("description", "");

            return page.ToString();
        }

        public string EditArticleItem(HttpContext context)
        {
            string resId = context.Request["res_id"];
            int id = int.Parse(context.Request["id"]);
            TemplatePage page = this._app.GetPage(this._plugin, "mg/article_item.html");
            ArticleResItem res = IocObject.WeixinRes.GetArticleItem(id);
            page.AddVariable("content", res.Content);
            page.AddVariable("description", res.Description);
            res.Content = null;
            res.Description = null;
            page.AddVariable("entity", res != null ? JsonConvert.SerializeObject(res) : "{}");
            page.AddVariable("resId", resId);
            return page.ToString();
        }


        public string SaveArticleItem_post(HttpContext context)
        {
            bool isCreate = false;
            try
            {
                ArticleResItem tres = context.Request.Form.ConvertToEntity<ArticleResItem>();
                isCreate = tres.Id <= 0;
                ArticleRes res = IocObject.WeixinRes.GetResById(int.Parse(context.Request.Form["ResId"])) as ArticleRes;
                tres.SetArticle(res);
                int id = tres.Save();
            }
            catch (Exception exc)
            {
                return "{result:false,message:'" + exc.Message + "'}";
            }
            return "{result:true,message:'" + (isCreate ? "添加成功" : "保存成功") + "'}";
        }

        public string DelRes_post(HttpContext context)
        {
            try
            {
                int resId = int.Parse(context.Request["id"]);
                IocObject.WeixinRes.DeleteRes(resId);
            }
            catch (Exception exc)
            {
                return "{result:false,message:'" + exc.Message + "'}";

            }
            return "{result:true,message:'删除成功'}";
        }

        #endregion
    }
}
