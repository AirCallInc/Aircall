using Aircall.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Services;
using System.Security.Cryptography;

namespace Aircall
{
    public partial class ResetPassword : System.Web.UI.Page
    {
        IUsersService objUsersService;

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                dvInactive.Visible = false;
                dvSuccessMsg.Visible = false;
                if(Request.QueryString["Url"]!=null)
                {
                    CheckPasswordLink();
                }
                else
                {
                    //Error Page 404
                }
            }
        }

        private void CheckPasswordLink()
        {
            string Url = Request.QueryString["Url"].ToString();
            objUsersService = ServiceFactory.UsersService;
            DataTable dtResult = new DataTable();
            objUsersService.CheckResetPasswordLink(Url, ref dtResult);
            if (dtResult.Rows.Count > 0)
            {
                //bool IsLinkActive = Convert.ToBoolean(dtResult.Rows[0]["IsLinkActive"].ToString());
                //if (!IsLinkActive)
                //{
                //    resetPasswordform.Visible = false;
                //    dvInactive.Visible = true;
                //}
            }
            else
            {
                resetPasswordform.Visible = false;
                dvInactive.Visible = true;
            }
        }

        protected void resetPass_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["Url"] != null)
            {
                try
                {
                    string Url = Request.QueryString["Url"].ToString();

                    objUsersService = ServiceFactory.UsersService;
                    string password;
                    using (MD5 md5Hash = MD5.Create())
                    {
                        password = Md5Encrypt.GetMd5Hash(md5Hash, txtPassword.Text.Trim());
                    }
                    objUsersService.ResetPassword(Url, password);

                    lblSuccessMsg.Text = "Your password reset successfully.";
                    dvSuccessMsg.Visible = true;
                    Response.Redirect(Application["SiteAddress"] + "admin/Login.aspx?msg=resetsuccess");
                }
                catch (Exception Ex)
                {

                    throw;
                }

            }
        }
    }
}