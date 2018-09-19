using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Services;
using System.Data;

namespace Aircall.admin
{
    public partial class CMSPagesMobile_List : System.Web.UI.Page
    {
        IMobileScreensServices objMobileScreensServices;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["msg"] != null)
                {
                    if (Session["msg"].ToString() == "edit")
                    {
                        dvMessage.InnerHtml = "<strong>Mobile CMS Page has been updated successfully.</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-success");
                        dvMessage.Visible = true;
                    }
                    Session["msg"] = null;
                }
                BindMobileScreens();
            }
        }

        private void BindMobileScreens()
        {
            objMobileScreensServices = ServiceFactory.MobileScreensServices;
            DataTable dtMobileScreen = new DataTable();
            objMobileScreensServices.GetAllMobileScreens(ref dtMobileScreen);
            if (dtMobileScreen.Rows.Count > 0)
            {
                lstMobileCMSPages.DataSource = dtMobileScreen;
            }
            lstMobileCMSPages.DataBind();
        }
    }
}