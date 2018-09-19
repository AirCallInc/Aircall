using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Aircall.partner.controls
{
    public partial class sidebar1 : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void lnkLogout_Click(object sender, EventArgs e)
        {
            Session["PartnerLoginCookie"] = null;
            //Session.Abandon();
            Response.Redirect(Application["SiteAddress"] + "partner/login.aspx");
        }
    }
}