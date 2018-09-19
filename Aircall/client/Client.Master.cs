using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Aircall.client
{
    public partial class Client : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            Response.Cache.SetNoStore();
            if (Session["ClientLoginCookie"] == null)
            {
                Response.Redirect(Application["SiteAddress"] + "sign-in.aspx");
            }
            if (!Request.Url.ToString().ToLower().Contains("my_units.aspx") && !Request.Url.ToString().ToLower().Contains("unit_details.aspx"))
            {
                Session["AddressExpression"] = null;
            }
        }
    }
}