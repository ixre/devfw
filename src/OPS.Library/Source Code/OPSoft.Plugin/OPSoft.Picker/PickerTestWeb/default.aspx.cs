using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;

namespace Collection.Web
{
    public partial class _default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            
            HttpContext.Current.Server.Execute("~/1.aspx");
            Page.Visible = false;

            return;
            Regex reg = new Regex("http://www.01wed.com/news/(\\d+).html");
            bool result = reg.IsMatch("http://www.01wed.com/news/25206.html");

            Response.Write(result.ToString());
        }
    }
}