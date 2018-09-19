using System;
using System.Web.UI;
using Services;
using System.Data;
using System.Security.Cryptography;
using Aircall.Common;

namespace Aircall
{
    public partial class reset_password : System.Web.UI.Page
    {
        #region "Declaration"
        DataTable dtClient = new DataTable();
        IClientService objClientService = ServiceFactory.ClientService;
        #endregion

        #region "Page Events"
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString.Count > 0)
                {
                    if (Request.QueryString["Url"] != null)
                    {
                        dtClient = new DataTable();
                        objClientService.CheckResetPasswordLinkExpiration(Request.QueryString["Url"].ToString(), ref dtClient);
                        if (dtClient.Rows.Count > 0)
                        {
                            if (Convert.ToInt32(dtClient.Rows[0]["Id"]) > 0)
                            {
                                lblEmail.InnerText = dtClient.Rows[0]["Email"].ToString();
                                dvResetPassword.Visible = true;
                                dvLinkExpired.Visible = false;
                                dvResetPasswordSuccess.Visible = false;
                                dvResetError.Visible = false;
                            }
                            else
                            {
                                dvResetPassword.Visible = false;
                                dvLinkExpired.Visible = true;
                                dvResetPasswordSuccess.Visible = false;
                                dvResetError.Visible = false;
                            }
                        }
                        else
                        {
                            dvResetPassword.Visible = false;
                            dvLinkExpired.Visible = true;
                            dvResetPasswordSuccess.Visible = false;
                            dvResetError.Visible = false;
                        }
                    }
                    else if (Request.QueryString["EmployeeUrl"] != null)
                    {
                        dtClient = new DataTable();
                        objClientService.CheckResetPasswordLinkExpirationEmployee(Request.QueryString["EmployeeUrl"].ToString(), ref dtClient);
                        if (dtClient.Rows.Count > 0)
                        {
                            if (Convert.ToInt32(dtClient.Rows[0]["Id"]) > 0)
                            {
                                lblEmail.InnerText = dtClient.Rows[0]["Email"].ToString();
                                dvResetPassword.Visible = true;
                                dvLinkExpired.Visible = false;
                                dvResetPasswordSuccess.Visible = false;
                                dvResetError.Visible = false;
                            }
                            else
                            {
                                dvResetPassword.Visible = false;
                                dvLinkExpired.Visible = true;
                                dvResetPasswordSuccess.Visible = false;
                                dvResetError.Visible = false;
                            }
                        }
                        else
                        {
                            dvResetPassword.Visible = false;
                            dvLinkExpired.Visible = true;
                            dvResetPasswordSuccess.Visible = false;
                            dvResetError.Visible = false;
                        }
                    }
                    else
                    {
                        dvResetPassword.Visible = false;
                        dvLinkExpired.Visible = true;
                        dvResetPasswordSuccess.Visible = false;
                        dvResetError.Visible = false;
                    }
                }
                else
                {
                    dvResetPassword.Visible = false;
                    dvLinkExpired.Visible = true;
                    dvResetPasswordSuccess.Visible = false;
                    dvResetError.Visible = false;
                }
            }
        }
        #endregion

        #region "Events"
        protected void btnResetPassword_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                string password;
                using (MD5 md5Hash = MD5.Create())
                {
                    password = Md5Encrypt.GetMd5Hash(md5Hash, txtPassword.Text.Trim());
                }
                if (Request.QueryString["Url"] != null)
                {
                    objClientService.ResetPassword(lblEmail.InnerText.ToString(), password, ref dtClient);
                }
                else
                {
                    objClientService.ResetPasswordEmployee(lblEmail.InnerText.ToString(), password, ref dtClient);
                }
                if (dtClient.Rows.Count > 0)
                {
                    if (Convert.ToInt32(dtClient.Rows[0]["Id"]) > 0)
                    {
                        dvResetPasswordSuccess.Visible = true;
                        dvResetPassword.Visible = false;
                        dvLinkExpired.Visible = false;
                    }
                    else if (Convert.ToInt32(dtClient.Rows[0]["Id"]) == -2)
                    {
                        dvError.InnerHtml = "New password should not be same as old password.";
                        dvError.Visible = true;
                        dvResetPassword.Visible = true;
                        dvLinkExpired.Visible = false;
                        dvResetPasswordSuccess.Visible = false;
                        dvResetError.Visible = false;
                    }
                    else
                    {
                        dvResetError.Visible = true;
                        dvResetPassword.Visible = false;
                        dvLinkExpired.Visible = false;
                        dvResetPasswordSuccess.Visible = false;
                    }
                }
                else
                {
                    dvResetError.Visible = true;
                    dvResetPassword.Visible = false;
                    dvLinkExpired.Visible = false;
                    dvResetPasswordSuccess.Visible = false;
                }
            }
        }
        #endregion
    }
}