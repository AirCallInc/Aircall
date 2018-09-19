using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Aircall.admin
{
    public partial class Logout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            HttpCookie LoginCookie = new HttpCookie("LoginCookie");
            LoginCookie.Expires = DateTime.Now.AddHours(-1);
            Response.Cookies.Add(LoginCookie);
            Response.Redirect(Application["SiteAddress"] + "admin/Login.aspx");
        }
    }
}