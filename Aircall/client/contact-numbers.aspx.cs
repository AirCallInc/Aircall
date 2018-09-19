using Aircall.Common;
using Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Aircall.client
{
    public partial class contact_numbers : System.Web.UI.Page
    {
        #region "Declaration"
        IClientService objClientService = ServiceFactory.ClientService;
        #endregion

        #region "Page Events"
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["ClientLoginCookie"] != null)
                {
                    BindClientContactInfo();
                }
                else
                    Response.Redirect(Application["SiteAddress"] + "sign-in.aspx");
            }
        }
        #endregion

        #region "Functions"
        private void BindClientContactInfo()
        {
            int ClientId = (Session["ClientLoginCookie"] as LoginModel).Id;
            DataTable dtClient = new DataTable();
            objClientService.GetClientById(ClientId, ref dtClient);
            if (dtClient.Rows.Count > 0)
            {
                txtMobile.Text = dtClient.Rows[0]["MobileNumber"].ToString();
                txtOffice.Text = dtClient.Rows[0]["OfficeNumber"].ToString();
                txtHome.Text = dtClient.Rows[0]["HomeNumber"].ToString();
            }
        }
        #endregion

        #region "Events"
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    int ClientId = (Session["ClientLoginCookie"]as LoginModel).Id;
                    if (string.IsNullOrEmpty(txtMobile.Text.Trim()) &&
                        string.IsNullOrEmpty(txtOffice.Text.Trim()) &&
                        string.IsNullOrEmpty(txtHome.Text.Trim()))
                    {
                        dvMessage.InnerHtml = "<strong>Please enter one of number.</strong>";
                        dvMessage.Attributes.Add("class", "error");
                        dvMessage.Visible = true;
                        return;
                    }
                    else
                    {
                        BizObjects.Client objClient = new BizObjects.Client();
                        objClient.Id = ClientId;
                        objClient.MobileNumber = txtMobile.Text.Trim();
                        objClient.OfficeNumber = txtOffice.Text.Trim();
                        objClient.HomeNumber = txtHome.Text.Trim();
                        objClient.UpdatedBy = ClientId;
                        objClient.UpdatedByType = General.UserRoles.Client.GetEnumValue();
                        objClient.UpdatedDate = DateTime.Now;

                        objClientService.UpdateClientContactInfo(ref objClient);

                        dvMessage.InnerHtml = "<strong>Contact info updated successfully.</strong>";
                        dvMessage.Attributes.Add("class", "success");
                        dvMessage.Visible = true;
                    }
                }
                catch (Exception Ex)
                {
                    dvMessage.InnerHtml = Ex.Message.ToString().Trim();
                    dvMessage.Attributes.Add("class", "error");
                    dvMessage.Visible = true;
                }
            }
        }
        #endregion
    }
}