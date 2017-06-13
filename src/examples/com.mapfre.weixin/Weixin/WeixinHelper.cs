using System;
using System.Collections.Generic;
using System.Net.Configuration;
using System.Text.RegularExpressions;
using System.Web;
using JR.DevFw.Framework;
using JR.DevFw.Framework.Net;
using JR.DevFw.PluginKernel;
using JR.DevFw.Web;
using Newtonsoft.Json;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.CommonAPIs;

namespace Com.Plugin.Weixin
{
    public static class WeixinHelper
    {
        private static MenuFull_ButtonGroup _buttonGroup;

        private static string _authUrlPrefix;
        private static string _hostPath;

        private static MenuFull_ButtonGroup GetButtonGroup()
        {
            if (_buttonGroup == null)
            {
                String menus = Variables.MenuButtons;
                _buttonGroup = JsonConvert.DeserializeObject<MenuFull_ButtonGroup>(menus);
            }
            return _buttonGroup;
        }
        public static TreeNode GetMenuTreeNode()
        {
            MenuFull_ButtonGroup menu = GetButtonGroup();
            TreeNode root =new TreeNode("微信菜单","0","",true,"");
            TreeNode tree = new TreeNode("菜单根目录", "0", "", true, "");
            root.childs.Add(tree);

            int i = 1, j = 1;
            foreach (var btn in menu.button)
            {
                TreeNode tn = new TreeNode(btn.name,i.ToString(),"",true,"");
              
                if (btn.sub_button!=null)
                {
                    foreach (var btn2 in btn.sub_button)
                    {
                        TreeNode tn2 = new TreeNode(btn2.name,i.ToString()+"-"+j.ToString(),"",true,"");
                        tn.childs.Add(tn2);
                        j++;
                    }
                    j = 1;
                }
                tree.childs.Add(tn);
                i++;
            }
            return root;
        }

        internal static MenuFull_RootButton GetMenuNode(int pi, int si)
        {
            MenuFull_ButtonGroup menu = GetButtonGroup();
            MenuFull_RootButton parent =  menu.button[pi];
            if (si == -1) return parent;
            return parent.sub_button[si];
        }

        internal static void SaveMenuNode(int pi, int si, MenuFull_RootButton button)
        {
            MenuFull_ButtonGroup menu = GetButtonGroup();
            if (si == -1)
            {
                menu.button[pi] = button;
            }
            else
            {
                menu.button[pi].sub_button[si] = button;
            }

            ReflushSetting(menu);
        }

        public static void CreateMenuNode(int pi, MenuFull_RootButton button)
        {
            MenuFull_ButtonGroup menu = GetButtonGroup();
            if (pi == -1)
            {
                if (menu.button.Count == 3)
                {
                    throw new Exception("菜单(一级)超出最大数量3个！");
                }
                menu.button.Add(button);
            }
            else
            {
                var list = menu.button[pi].sub_button;
                if (list == null)
                {
                    list = new List<MenuFull_RootButton>();
                }


                if (menu.button.Count == 5)
                {
                    throw new Exception("菜单(二级)超出最大数量5个！");
                }

                list.Add(button);
                menu.button[pi].sub_button = list;
            }

            ReflushSetting(menu);
        }
      
        public static void DelMenu(int pi, int si)
        {
            MenuFull_ButtonGroup group = GetButtonGroup();
            MenuFull_RootButton menu;
            if (si == -1)
            {
                group.button.Remove(group.button[pi]);
            }
            else
            {
                group.button[pi].sub_button.Remove(group.button[pi].sub_button[si]);
            }

            ReflushSetting(group);
        }

        private static void ReflushSetting(MenuFull_ButtonGroup menu)
        {
            String json = JsonConvert.SerializeObject(menu);
            SettingFile st = Config.PluginAttr.Settings;

            json = json.Replace(",\"sub_button\":null", "")
                .Replace(",\"url\":null", "")
                .Replace(",\"key\":null", "")
                .Replace("\"type\":null,","");

            st["Weixin_MenuButtons"] = json;
            st.Flush();

            Variables.MenuButtons = json;
            _buttonGroup = menu;
        }

        public static string PostApplyMenu()
        {
            var result = AccessTokenContainer.GetTokenResult(Variables.AppId);
            // GetMenuResult menuResult = CommonApi.GetMenu(result.access_token);

            CommonApi.DeleteMenu(result.access_token);
            var buttons = Variables.MenuButtons;

            Regex regex = new Regex("\\{(local|auth)\\}([^\"]*)");
            buttons = regex.Replace(buttons, a =>
            {
                if (a.Groups[1].Value == "local")
                {
                    return WeixinHelper.GetLocalPath()+a.Groups[2].Value;
                }
                else
                {
                    return WeixinHelper.GetAuthUrl(a.Groups[2].Value);
                }
            });

            var resultHtml = HttpClient.Post("https://api.weixin.qq.com/cgi-bin/menu/create?access_token="
                                             + result.access_token, buttons, null);
            return Regex.Match(resultHtml, "\"errmsg\":\"([^\"]+)\"").Groups[1].Value;
        }

        private static string GetLocalPath()
        {
            if (_hostPath == null)
            {
                _hostPath =WebCtx.Current.Domain;
            }
            return _hostPath;;
        }

        /// <summary>
        /// 获取验证处理地址
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        public static string GetAuthProcUrl(string returnUrl)
        {
            return String.Format("{0}AuthProc?return_url={1}",
                GetAuthUrlPrefix(), 
                HttpUtility.UrlEncode(returnUrl));
        }

        private static string GetAuthUrlPrefix()
        {
            if (_authUrlPrefix == null)
            {
                String overrideIndent =  Config.PluginAttr.Settings[PluginSettingKeys.OverrideUrlIndent];
                if (overrideIndent == null || overrideIndent.Trim() == "")
                {
                    _authUrlPrefix = WebCtx.Current.Domain + "/" + Config.PluginAttr.WorkIndent + ".sh.aspx/";
                }
                else
                {
                    _authUrlPrefix = WebCtx.Current.Domain + "/" + overrideIndent + "/";
                }
            }
            return _authUrlPrefix;
        }

        public static string GetAuthUrl(string returnUrl)
        {
            return String.Format("{0}Auth?return_url={1}",
                GetAuthUrlPrefix(),
                HttpUtility.UrlEncode(returnUrl));
        }
    }
}
