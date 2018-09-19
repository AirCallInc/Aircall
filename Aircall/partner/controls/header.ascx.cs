using Aircall.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Aircall.partner.controls
{
    public partial class header1 : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["PartnerLoginCookie"] != null)
            {
                LoginModel login = Session["PartnerLoginCookie"] as LoginModel;
                int PartnerId = login.Id;
                string Username = login.Username;//Request.Cookies["PartnerLoginCookie"]["Username"].ToString();
                ltrFullname.Text = login.FullName;//Request.Cookies["PartnerLoginCookie"]["FullName"].ToString();
                imgUser.Src = login.Image;//Request.Cookies["PartnerLoginCookie"]["Image"].ToString();
            }
            else
            {
                Response.Redirect(Application["SiteAddress"] + "partner/Login.aspx");
            }
        }

        protected void lnkLogout_Click(object sender, EventArgs e)
        {
            Session["PartnerLoginCookie"] = null;
            //Session.Abandon();
            Response.Redirect(Application["SiteAddress"] + "partner/login.aspx");
        }
    }
}