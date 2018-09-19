using Aircall.Common;
using Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Aircall.client
{
    public partial class change_password : System.Web.UI.Page
    {
        #region "Declaration"
        IClientService objClientService = ServiceFactory.ClientService;
        #endregion

        #region "Page Events"
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        #endregion

        #region "Events"
        protected void btnChangePassword_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    int ClientId = (Session["ClientLoginCookie"] as LoginModel).Id;
                    string OldPassword = string.Empty;
                    string NewPassword = string.Empty;
                    using (MD5 md5Hash = MD5.Create())
                    {
                        OldPassword = Md5Encrypt.GetMd5Hash(md5Hash, txtOldPassword.Text.Trim());
                        NewPassword = Md5Encrypt.GetMd5Hash(md5Hash, txtNewPassword.Text.Trim());
                    }
                    DataTable dtClient = new DataTable();
                    objClientService.ChangePassword(OldPassword, NewPassword, ClientId, ref dtClient);
                    if (dtClient.Rows[0]["ClientId"].ToString() == "-1")
                    {
                        dvMessage.InnerHtml = "<strong>Old password not match.</strong>";
                        dvMessage.Attributes.Add("class", "error");
                        dvMessage.Visible = true;
                        return;
                    }
                    else
                    {
                        dvMessage.InnerHtml = "<strong>Password Changed Successfully.</strong>";
                        dvMessage.Attributes.Add("class", "success");
                        dvMessage.Visible = true;
                    }
                }
                catch (Exception Ex)
                {
                    dvMessage.InnerHtml = Ex.Message.Trim();
                    dvMessage.Attributes.Add("class", "error");
                    dvMessage.Visible = true;
                }
            }
        }
        #endregion
    }
}