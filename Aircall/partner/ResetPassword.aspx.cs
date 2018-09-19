using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Services;
using System.Data;
using System.Security.Cryptography;
using Aircall.Common;

namespace Aircall.partner
{
    public partial class ResetPassword : System.Web.UI.Page
    {
        IPartnerService objPartnerService;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["Url"] != null)
                {
                    CheckResetPasswordLink();
                }
            }
        }

        private void CheckResetPasswordLink()
        {
            string Url = Request.QueryString["Url"].ToString();
            objPartnerService = ServiceFactory.PartnerService;
            DataTable dtResult = new DataTable();
            objPartnerService.CheckPartnerResetPasswordLink(Url, ref dtResult);
            if (dtResult.Rows.Count > 0)
            {

            }
            else
            {
                resetPasswordform.Visible = false;
                dvInactive.Visible = true;
                dvLogin.Visible = true;
            }
        }

        protected void resetPass_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                if (Request.QueryString["Url"] != null)
                {
                    try
                    {
                        string Url = Request.QueryString["Url"].ToString();
                        objPartnerService = ServiceFactory.PartnerService;
                        string password;
                        using (MD5 md5Hash = MD5.Create())
                        {
                            password = Md5Encrypt.GetMd5Hash(md5Hash, txtPassword.Text.Trim());
                        }
                        objPartnerService.PartnerResetPassword(Url, password);

                        lblSuccessMsg.Text = "Your password reset successfully.";
                        dvSuccessMsg.Visible = true;
                        dvLogin.Visible = true;
                    }
                    catch (Exception Ex)
                    {
                    }
                }
            }
        }
    }
}