using System.Text.RegularExpressions;
using AtNet.DevFw.Web;
using Com.Plugin.Core;
using Com.Plugin.Entity;
using Ops.Cms;
using Senparc.Weixin.MP.Entities;

namespace Com.Plugin.Weixin.Content
{
    public class WeixinRender
    {
        private static readonly WeiXinGenerator generator = new WeiXinGenerator();

        public static IResponseMessageBase Render(CustomMessageHandler handler, string eventKey)
        {
            //IResponseMessageBase rsp  = null;

            if (eventKey.StartsWith("res:"))
            {
                return generator.GetResponseByResKey(handler,eventKey.Substring(eventKey.IndexOf(":") + 1));
            }

            switch (eventKey)
            {
                case "product_promotion":
                    return generator.GetPromotion(handler);
                case "co_intro":
                    return generator.GetCoIntro(handler);
                case "clalm_profile":
                    return generator.GetClalmProfile(handler);
                case "co_contact":
                    return generator.GetCoContact(handler);
            }

            if (Variables.DebugMode)
            {
                var strongResponseMessage = handler.CreateResponseMessage<ResponseMessageText>();
                strongResponseMessage.Content = "您点击了按钮，EventKey：" + eventKey;
                return strongResponseMessage;
            }
            return null;
        }
    }

    public class WeiXinGenerator
    {
        public IResponseMessageBase GetPromotion(CustomMessageHandler handler)
        {
            var rsp = handler.CreateResponseMessage<ResponseMessageNews>();
            rsp.Articles.Add(new Article()
            {
                Title = "您点击了子菜单图文按钮",
                Description = "您点击了子菜单图文按钮，这是一条图文信息。",
                PicUrl = "http://weixin.senparc.com/Images/qrcode.jpg",
                Url = "http://weixin.senparc.com"
            });
            return rsp;
        }

        public IResponseMessageBase GetCoIntro(CustomMessageHandler handler)
        {
            var rsp = handler.CreateResponseMessage<ResponseMessageNews>();
            rsp.Articles.Add(new Article()
            {
                Title = "公司介绍",
                Description = "中国人民人寿保险股份有限公司和路华救援（北京）有限公司合作进行旅行保险的服务...",
                PicUrl = "http://www.epicc.com.cn/ymdblm/db_gyrb/201205/W020120710544945374543.jpg",
                Url = "http://travel.e-picclife.com/m/aboutus.aspx"
            });
            return rsp;


            //
        }

        internal IResponseMessageBase GetCoContact(CustomMessageHandler handler)
        {
            var rsp = handler.CreateResponseMessage<ResponseMessageNews>();
            rsp.Articles.Add(new Article()
            {
                Title = "公司介绍",
                Description = "如果您有任何的疑问和意见，欢迎和我们联系!",
                PicUrl = "http://travel.e-picclife.com/m/_imgs_v2/paris/banner_contactus.jpg",
                Url = "http://travel.e-picclife.com/m/contactus.aspx"
            });
            return rsp;
        }

        public IResponseMessageBase GetClalmProfile(CustomMessageHandler handler)
        {
            var rsp = handler.CreateResponseMessage<ResponseMessageNews>();
            rsp.Articles.Add(new Article()
            {
                Title = "理赔资料",
                Description = "如果您有任何的疑问和意见，欢迎和我们联系!",
                PicUrl = "http://travel.e-picclife.com/m/_imgs_v2/paris/banner_contactus.jpg",
                Url = "http://travel.e-picclife.com/m/contactus.aspx"
            });
            rsp.Articles.Add(new Article()
            {
                Title = "理赔资料-理赔资料A",
                Description = "如果您有任何的疑问和意见，欢迎和我们联系!",
                PicUrl = "http://travel.e-picclife.com/m/_imgs_v2/paris/banner_contactus.jpg",
                Url = "http://travel.e-picclife.com/m/contactus.aspx"
            });
            rsp.Articles.Add(new Article()
            {
                Title = "理赔资料-理赔资料B",
                Description = "如果您有任何的疑问和意见，欢迎和我们联系!",
                PicUrl = "http://travel.e-picclife.com/m/_imgs_v2/paris/banner_contactus.jpg",
                Url = "http://travel.e-picclife.com/m/contactus.aspx"
            });
            rsp.Articles.Add(new Article()
            {
                Title = "理赔资料-理赔资料C",
                Description = "如果您有任何的疑问和意见，欢迎和我们联系!",
                PicUrl = "http://travel.e-picclife.com/m/_imgs_v2/paris/banner_contactus.jpg",
                Url = "http://travel.e-picclife.com/m/contactus.aspx"
            });
            rsp.Articles.Add(new Article()
            {
                Title = "理赔资料-理赔资料D",
                Description = "如果您有任何的疑问和意见，欢迎和我们联系!",
                PicUrl = "http://travel.e-picclife.com/m/_imgs_v2/paris/banner_contactus.jpg",
                Url = "http://travel.e-picclife.com/m/contactus.aspx"
            });
            return rsp;
        }

        public IResponseMessageBase GetResponseByResKey(CustomMessageHandler handler,string resKey)
        {
            IWxRes res = IocObject.WeixinRes.GetResByKey(resKey);
            if (res == null)
            {
                Config.Logln("素材"+resKey+"不存在");
            }
            TextRes trs;
            if ((trs = res as TextRes) != null)
            {
                var strongResponseMessage = handler.CreateResponseMessage<ResponseMessageText>();
                strongResponseMessage.Content =trs.Content;

                Config.Logln("素材" + resKey + "/文本");
                return strongResponseMessage;
            }
            else
            {
                var rsp = handler.CreateResponseMessage<ResponseMessageNews>();
                ArticleRes ares = res as ArticleRes;
                var items = ares.Items;

                string domain = WebCtx.Domain;

                foreach (var item in items)
                {
                    if (item.Enabled)
                    {
                        rsp.Articles.Add(new Article()
                        {
                            Title = item.Title,
                            Description = item.Description??"",
                            PicUrl = domain + "/"+item.Pic,
                            Url =item.Url.StartsWith("http://")?item.Url:domain+ item.Url
                        });
                    }
                }

                Config.Logln("素材"+resKey+"/图文");
                return rsp;
            }
        }
    }

}
