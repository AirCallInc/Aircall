using Aircall.Common;
using Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Aircall.controls
{
    public partial class AfterLogin : System.Web.UI.UserControl
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["ClientLoginCookie"] != null)
            {
                LoginModel login = Session["ClientLoginCookie"] as LoginModel;
                lnkUsername.InnerText = login.FullName;
                
            }
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