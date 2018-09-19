using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Services;
using System.Security.Cryptography;
using Aircall.Common;

namespace Aircall.partner
{
    public partial class ChangePassword : System.Web.UI.Page
    {
        IPartnerService objPartnerService;
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    LoginModel login = Session["PartnerLoginCookie"] as LoginModel;
                    string Username = login.Username;// Request.Cookies["PartnerLoginCookie"]["Username"].ToString();
                    DataTable dtResult = new DataTable();
                    objPartnerService = ServiceFactory.PartnerService;
                    string Password = string.Empty;
                    string newPassword = string.Empty;
                    using (MD5 md5Hash = MD5.Create())
                    {
                        Password = Md5Encrypt.GetMd5Hash(md5Hash, txtOldPassword.Text.Trim());
                        newPassword = Md5Encrypt.GetMd5Hash(md5Hash, txtNewPassword.Text.Trim());
                    }

                    objPartnerService.CheckPartnerLogin(Username, Password, ref dtResult);
                    if (dtResult.Rows.Count > 0)
                    {
                        objPartnerService.ChangePassword(Username, newPassword);
                        dvMessage.InnerHtml = "<strong>Your Password changed successfully.</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-success");
                        dvMessage.Visible = true;
                    }
                    else
                    {
                        dvMessage.InnerHtml = "<strong>Old Password is incorrect.</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-error");
                        dvMessage.Visible = true;
                    }
                }
                catch (Exception Ex)
                {
                    dvMessage.InnerHtml = "<strong>Error!</strong> " + Ex.Message.ToString().Trim();
                    dvMessage.Attributes.Add("class", "alert alert-error");
                    dvMessage.Visible = true;
                }
            }
        }
    }
}