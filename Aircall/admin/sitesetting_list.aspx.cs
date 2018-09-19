using Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Aircall.admin
{
    public partial class sitesetting_list : System.Web.UI.Page
    {
        ISiteSettingService objSiteSettingService = ServiceFactory.SiteSettingService;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["upd"] != null)
            {
                dvMessage.InnerText = "Record Updated.";
                dvMessage.Visible = true;
                dvMessage.Attributes.Add("class", "alert alert-success");
                Session["upd"] = null;
            }
            DataTable dtResult = new DataTable();
            objSiteSettingService.GetAllSiteSetting(ref dtResult);

            lstSiteSettings.DataSource = dtResult;
            lstSiteSettings.DataBind();
        }
    }
}