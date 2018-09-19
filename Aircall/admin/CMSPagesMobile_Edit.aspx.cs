using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Services;
using System.Data;
using Aircall.Common;

namespace Aircall.admin
{
    public partial class CMSPagesMobile_Edit : System.Web.UI.Page
    {
        IMobileScreensServices objMobileScreensServices;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["CMSPageId"]))
                {
                    BindScreenById();
                }
            }
        }

        private void BindScreenById()
        {
            int ScreenId = Convert.ToInt32(Request.QueryString["CMSPageId"].ToString());
            DataTable dtMobileScreen = new DataTable();
            objMobileScreensServices = ServiceFactory.MobileScreensServices;
            objMobileScreensServices.GetMobileScreenById(ScreenId, ref dtMobileScreen);
            if (dtMobileScreen.Rows.Count > 0)
            {
                txtPageTitle.Text = dtMobileScreen.Rows[0]["PageTitle"].ToString();
                CKEditor.Value = dtMobileScreen.Rows[0]["Description"].ToString();
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    objMobileScreensServices = ServiceFactory.MobileScreensServices;
                    BizObjects.MobileScreens objMobileScreens = new BizObjects.MobileScreens();
                    if (!string.IsNullOrEmpty(Request.QueryString["CMSPageId"]) && Session["LoginSession"] != null)
                    {
                        LoginModel objLoginModel = new LoginModel();
                        objLoginModel = Session["LoginSession"] as LoginModel;

                        int ScreenId = Convert.ToInt32(Request.QueryString["CMSPageId"].ToString());
                        objMobileScreens.Id = ScreenId;
                        objMobileScreens.PageTitle = txtPageTitle.Text;
                        objMobileScreens.Description = CKEditor.Value;
                        objMobileScreens.UpdatedBy = objLoginModel.Id;
                        objMobileScreens.UpdatedByType = objLoginModel.RoleId;
                        objMobileScreens.UpdatedDate = DateTime.UtcNow;

                        objMobileScreensServices.UpdateMobileScreen(ref objMobileScreens);
                        Session["msg"] = "edit";
                        Response.Redirect(Application["SiteAddress"] + "admin/CMSPagesMobile_List.aspx");
                    }
                }
                catch (Exception Ex)
                {
                    dvMessage.InnerHtml = "<strong>Error!</strong> " + Ex.Message.Trim();
                    dvMessage.Attributes.Add("class", "alert alert-error");
                    dvMessage.Visible = true;
                }
            }
        }
    }
}