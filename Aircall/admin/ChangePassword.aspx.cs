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

namespace Aircall.admin
{
    public partial class ChangePassword : System.Web.UI.Page
    {
        IUsersService objUsersService;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    LoginModel objLoginModel= new LoginModel();
                    objLoginModel = Session["LoginSession"] as LoginModel;
                    string Username = objLoginModel.Username;
                    DataTable dtResult = new DataTable();
                    objUsersService = ServiceFactory.UsersService;
                    string Password = string.Empty;
                    string newPassword = string.Empty;
                    using (MD5 md5Hash = MD5.Create())
                    {
                        Password = Md5Encrypt.GetMd5Hash(md5Hash, txtOldPassword.Text.Trim());
                        newPassword = Md5Encrypt.GetMd5Hash(md5Hash, txtNewPassword.Text.Trim());
                    }

                    objUsersService.CheckAdminLogin(Username, Password, ref dtResult);
                    if (dtResult.Rows.Count > 0)
                    {
                        objUsersService.ChangePassword(Username, newPassword);
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