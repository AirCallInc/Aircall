using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Aircall.partner
{
    public partial class Logout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            HttpCookie PartnerLoginCookie = new HttpCookie("PartnerLoginCookie");
            PartnerLoginCookie.Expires = DateTime.Now.AddHours(-1);
            Response.Cookies.Add(PartnerLoginCookie);
            Response.Redirect(Application["SiteAddress"] + "partner/Login.aspx");
        }
    }
}