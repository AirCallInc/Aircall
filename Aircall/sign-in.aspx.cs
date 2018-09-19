using System;
using Services;
using System.Data;
using System.Security.Cryptography;
using Aircall.Common;
using System.Web;

namespace Aircall
{
    public partial class sign_in : System.Web.UI.Page
    {
        #region "Declaration"
        IClientService objClientService = ServiceFactory.ClientService;
        IEmailTemplateService objEmailTemplateService = ServiceFactory.EmailTemplateService;
        DataTable dtClient = new DataTable();
        #endregion

        #region "Page Events"
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["ClientLoginCookie"] != null)
                {
                    Response.Redirect(Application["SiteAddress"].ToString() + "client/dashboard.aspx", false);
                }

                if (Request.Cookies["ClientRememberCookie"] != null)
                {
                    if (Request.Cookies["ClientRememberCookie"]["Email"] != null && Request.Cookies["ClientRememberCookie"]["Password"] != null)
                    {
                        txtEmail.Text = Request.Cookies["ClientRememberCookie"]["Email"].ToString();
                        txtPassword.Attributes.Add("Value", General.Decrypt(Request.Cookies["ClientRememberCookie"]["Password"].ToString()));
                        chkRememberMe.Checked = true;
                    }
                }
            }
        }
        #endregion

        #region "Events"
        protected void btnForgotPassword_Click(object sender, EventArgs e)
        {
            dvError.Visible = false;
            dvError.InnerHtml = "";
            if (Page.IsValid)
            {
                dtClient = new DataTable();
                objClientService.CheckForForgotPassword(txtForgotEmail.Text.Trim(), ref dtClient);
                if (dtClient.Rows.Count > 0)
                {
                    if (Convert.ToInt32(dtClient.Rows[0]["Id"]) > 0)
                    {
                        string Name = dtClient.Rows[0]["FirstName"].ToString() + " " + dtClient.Rows[0]["LastName"].ToString();
                        DataTable dtEmailTemplate = new DataTable();
                        objEmailTemplateService.GetByName("ResetPasswordClient", ref dtEmailTemplate);
                        if (dtEmailTemplate.Rows.Count > 0)
                        {
                            string body = dtEmailTemplate.Rows[0]["EmailBody"].ToString();
                            string CCEmail = dtEmailTemplate.Rows[0]["CCEmails"].ToString();
                            string link = Application["SiteAddress"].ToString().Trim() + "reset-password.aspx?Url=" + dtClient.Rows[0]["PasswordUrl"].ToString();
                            body = body.Replace("{{Link}}", link.ToString());
                            body = body.Replace("{{Name}}", Name);
                            Email.SendEmail(dtEmailTemplate.Rows[0]["EmailTemplateSubject"].ToString(), txtForgotEmail.Text.Trim(), CCEmail, "", body);
                            dvForgotPasswordSuccess.Visible = true;
                            txtForgotEmail.Text = "";
                        }
                    }
                    else
                    {
                        dvError1.Visible = true;
                        dvError1.InnerHtml = "You are not registered with this email.";
                    }
                }
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            dvError.Visible = false;
            dvError.InnerHtml = "";
            if (Page.IsValid)
            {
                string password;
                using (MD5 md5Hash = MD5.Create())
                {
                    password = Md5Encrypt.GetMd5Hash(md5Hash, txtPassword.Text.Trim());
                }
                objClientService.Login(txtEmail.Text.Trim(), password, ref dtClient);
                if (dtClient.Rows.Count > 0)
                {
                    if (Convert.ToInt32(dtClient.Rows[0]["Id"]) > 0)
                    {
                        LoginModel login = new LoginModel();
                        login.Id = int.Parse(dtClient.Rows[0]["Id"].ToString());
                        login.FullName = dtClient.Rows[0]["FullName"].ToString();
                        login.RoleId = General.UserRoles.Client.GetEnumValue();
                        Session.Add("ClientLoginCookie", login);

                        //HttpCookie ClientLoginCookie = new HttpCookie("ClientLoginCookie");
                        //ClientLoginCookie.Values["ClientId"] = dtClient.Rows[0]["Id"].ToString();
                        //ClientLoginCookie.Values["ClientName"] = dtClient.Rows[0]["FullName"].ToString();
                        //ClientLoginCookie.Expires = DateTime.Now.AddHours(3);
                        //Response.Cookies.Add(ClientLoginCookie);

                        if (chkRememberMe.Checked)
                        {
                            HttpCookie ClientRememberCookie = new HttpCookie("ClientRememberCookie");
                            ClientRememberCookie.Values["Email"] = txtEmail.Text.Trim();
                            ClientRememberCookie.Values["Password"] = General.EncryptVerification(txtPassword.Text.Trim());
                            ClientRememberCookie.Expires = DateTime.Now.AddMonths(1);
                            Response.Cookies.Add(ClientRememberCookie);
                        }
                        else
                        {
                            HttpCookie ClientRememberCookie = new HttpCookie("ClientRememberCookie");
                            ClientRememberCookie.Expires = DateTime.Now.AddMonths(-1);
                            Response.Cookies.Add(ClientRememberCookie);
                        }

                        Response.Redirect(Application["SiteAddress"].ToString() + "client/dashboard.aspx", false);
                    }
                    else if (Convert.ToInt32(dtClient.Rows[0]["Id"]) == -1)
                    {
                        dvError.Visible = true;
                        dvError.InnerHtml = "Incorrect Email or Password";
                    }
                    else if (Convert.ToInt32(dtClient.Rows[0]["Id"]) == -2)
                    {
                        dvError.Visible = true;
                        dvError.InnerHtml = "Your account was deactivated by Admin.";
                    }
                    
                }
            }
        }
        #endregion
    }
}