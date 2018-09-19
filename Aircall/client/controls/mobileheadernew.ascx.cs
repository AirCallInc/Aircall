using Aircall.Common;
using System;
using System.Web;

namespace Aircall.client.controls
{
    public partial class mobileheadernew : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["ClientLoginCookie"] != null)
                lnkUsername.InnerText = (Session["ClientLoginCookie"] as LoginModel).FullName;
            else
                Response.Redirect(Application["SiteAddress"] + "sign-in.aspx");
        }

        protected void lnkLogout_Click(object sender, EventArgs e)
        {
            Session["ClientLoginCookie"] = null;
            //Session.Abandon();
            Response.Redirect(Application["SiteAddress"] + "sign-in.aspx");
        }
    }
}