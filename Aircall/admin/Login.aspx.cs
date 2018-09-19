using Aircall.Common;
using Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Aircall.Admin
{
    public partial class Login : System.Web.UI.Page
    {
        IUsersService objUsersService;
        IEmailTemplateService objEmailTemplateService;

        protected void Page_Load(object sender, EventArgs e)
        {
            dvMsg.Visible = false;
            dvSuccessMsg.Visible = false;

            if (!IsPostBack)
            {
                if (Request.Cookies["AdminRememberCookie"] != null)
                {
                    if (Request.Cookies["AdminRememberCookie"]["Username"] != null && Request.Cookies["AdminRememberCookie"]["Password"] != null)
                    {
                        txtUsername.Text = Request.Cookies["AdminRememberCookie"]["Username"].ToString();
                        txtPassword.Attributes.Add("Value", General.Decrypt(Request.Cookies["AdminRememberCookie"]["Password"].ToString()));
                    }
                }
                if (Request.QueryString["msg"] != null)
                {
                    if (Request.QueryString["msg"].ToString() != "")
                    {
                        if (Request.QueryString["msg"].ToString() == "resetsuccess")
                        {
                            lblSuccessMsg.Text = "Your password reset successfully.";
                            dvSuccessMsg.Visible = true;
                        }
                        else
                            dvSuccessMsg.Visible = false;
                    }
                    else
                        dvSuccessMsg.Visible = false;
                }
                else dvSuccessMsg.Visible = false;
            }

        }

        protected void loginbtn_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtUsername.Text.Trim()) && !string.IsNullOrEmpty(txtPassword.Text.Trim()) && (txtUsername.Text.Trim() != "Username" && txtPassword.Text.Trim() != "Password"))
            {
                objUsersService = ServiceFactory.UsersService;
                DataTable dtUser = new DataTable();
                string password;
                using (MD5 md5Hash = MD5.Create())
                {
                    password = Md5Encrypt.GetMd5Hash(md5Hash, txtPassword.Text.Trim());
                }

                objUsersService.CheckAdminLogin(txtUsername.Text.Trim(), password, ref dtUser);
                if (dtUser.Rows.Count > 0)
                {
                    LoginModel objLoginModel = new LoginModel();
                    objLoginModel.Id = Convert.ToInt32(dtUser.Rows[0]["Id"].ToString());
                    objLoginModel.RoleId = Convert.ToInt32(dtUser.Rows[0]["RoleId"].ToString());
                    objLoginModel.Username = dtUser.Rows[0]["UserName"].ToString();
                    objLoginModel.FullName = dtUser.Rows[0]["Firstname"].ToString() + " " + dtUser.Rows[0]["Lastname"].ToString();

                    if (string.IsNullOrEmpty(dtUser.Rows[0]["Image"].ToString()))
                        objLoginModel.Image = Application["SiteAddress"] + "uploads/profile/defultimage.jpg";
                    else
                        objLoginModel.Image = Application["SiteAddress"] + "uploads/profile/" + dtUser.Rows[0]["Image"].ToString();

                    Session["LoginSession"] = objLoginModel;

                    //HttpCookie LoginCookie = new HttpCookie("LoginCookie");
                    //LoginCookie.Values["UserId"] = dtUser.Rows[0]["Id"].ToString();
                    //LoginCookie.Values["RoleId"] = dtUser.Rows[0]["RoleId"].ToString();
                    //LoginCookie.Values["Username"] = dtUser.Rows[0]["UserName"].ToString();
                    //LoginCookie.Values["FullName"] = dtUser.Rows[0]["Firstname"].ToString() + " " + dtUser.Rows[0]["Lastname"].ToString();

                    //if (string.IsNullOrEmpty(dtUser.Rows[0]["Image"].ToString()))
                    //    LoginCookie.Values["Image"] = Application["SiteAddress"] + "admin/img/avatar1_small.jpg";
                    //else
                    //    LoginCookie.Values["Image"] = Application["SiteAddress"] + "uploads/profile/" + dtUser.Rows[0]["Image"].ToString();

                    //LoginCookie.Expires = DateTime.Now.AddHours(1);
                    //Response.Cookies.Add(LoginCookie);

                    if (chkRemember.Checked)
                    {
                        HttpCookie AdminRememberCookie = new HttpCookie("AdminRememberCookie");
                        AdminRememberCookie.Values["Username"] = txtUsername.Text.Trim();
                        AdminRememberCookie.Values["Password"] = General.EncryptVerification(txtPassword.Text.Trim());
                        AdminRememberCookie.Expires = DateTime.Now.AddMonths(1);
                        Response.Cookies.Add(AdminRememberCookie);
                    }

                    Response.Redirect(Application["SiteAddress"] + "admin/dashboard.aspx", true);
                }
                else
                {
                    lblMsg.Text = "Incorrect Username or Password.";
                    dvMsg.Visible = true;
                }
            }
        }

        protected void btnForgot_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtEmail.Text.Trim()) && txtEmail.Text.Trim() != "Email")
            {
                try
                {
                    int RandomStringLength = Convert.ToInt32(ConfigurationManager.AppSettings["RandomStringLength"].ToString().Trim());
                    objUsersService = ServiceFactory.UsersService;

                    DataTable dtUser = new DataTable();
                    objUsersService.GetUserInfoByEmail(txtEmail.Text.Trim(), 0, ref dtUser);
                    if (dtUser.Rows.Count > 0)
                    {
                        string Name = dtUser.Rows[0]["FirstName"].ToString() + " " + dtUser.Rows[0]["LastName"].ToString();
                        string randomString = Guid.NewGuid().ToString().Substring(0, RandomStringLength);
                        objUsersService.SetForgotPasswordLink(txtEmail.Text.Trim(), randomString);

                        DataTable dtEmailtemplate = new DataTable();
                        objEmailTemplateService = ServiceFactory.EmailTemplateService;
                        objEmailTemplateService.GetByName("ResetPasswordAdmin", ref dtEmailtemplate);
                        if (dtEmailtemplate.Rows.Count > 0)
                        {
                            string Emailbody = dtEmailtemplate.Rows[0]["EmailBody"].ToString();
                            string CCEmail = dtEmailtemplate.Rows[0]["CCEmails"].ToString();
                            string ResetPasswordLink = ConfigurationManager.AppSettings["SiteAddress"].ToString().Trim() + "ResetPassword.aspx?Url=" + randomString;
                            Emailbody = Emailbody.Replace("{{Link}}", ResetPasswordLink);
                            Emailbody = Emailbody.Replace("{{Name}}", Name);
                            string Subject = dtEmailtemplate.Rows[0]["EmailTemplateSubject"].ToString();
                            Email.SendEmail(Subject, txtEmail.Text.Trim(), CCEmail, "", Emailbody);
                            lblSuccessMsg.Text = "Reset password link sent to registered email.";
                            dvSuccessMsg.Visible = true;
                        }
                    }
                    else
                    {
                        lblSuccessMsg.Text = "Entered Email address is not registered.";
                        dvSuccessMsg.Visible = true;
                    }
                }
                catch (Exception Ex)
                {
                    lblMsg.Text = Ex.Message;
                    dvMsg.Visible = true;
                }
            }
        }
    }
}