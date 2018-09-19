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
using System.Configuration;

namespace Aircall.partner
{
    public partial class Login : System.Web.UI.Page
    {
        IPartnerService objPartnerService;
        IEmailTemplateService objEmailTemplateService;

        protected void Page_Load(object sender, EventArgs e)
        {
            dvMsg.Visible = false;
            dvSuccessMsg.Visible = false;
            if (!IsPostBack)
            {
                if (Session["PartnerLoginCookie"] != null)
                {
                    Response.Redirect(Application["SiteAddress"].ToString() + "partner/dashboard.aspx", false);
                }
                if (Request.Cookies["PartnerRememberCookie"] != null)
                {
                    if (Request.Cookies["PartnerRememberCookie"]["Username"] != null && Request.Cookies["PartnerRememberCookie"]["Password"] != null)
                    {
                        txtUsername.Text = Request.Cookies["PartnerRememberCookie"]["Username"].ToString();
                        txtPassword.Attributes.Add("Value", General.Decrypt(Request.Cookies["PartnerRememberCookie"]["Password"].ToString()));
                        chkRemember.Checked = true;
                    }
                }
            }
        }

        protected void loginbtn_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtUsername.Text.Trim()) && !string.IsNullOrEmpty(txtPassword.Text.Trim()) && (txtUsername.Text.Trim() != "Partnername" && txtPassword.Text.Trim() != "Password"))
            {
                objPartnerService = ServiceFactory.PartnerService;
                DataTable dtPartner = new DataTable();
                string password;
                using (MD5 md5Hash = MD5.Create())
                {
                    password = Md5Encrypt.GetMd5Hash(md5Hash, txtPassword.Text.Trim());
                }

                objPartnerService.CheckPartnerLogin(txtUsername.Text.Trim(), password, ref dtPartner);
                if (dtPartner.Rows.Count > 0)
                {
                    LoginModel login = new LoginModel();
                    login.Id = int.Parse(dtPartner.Rows[0]["Id"].ToString());
                    login.FullName = dtPartner.Rows[0]["Firstname"].ToString() + " " + dtPartner.Rows[0]["Lastname"].ToString();
                    login.RoleId = int.Parse(dtPartner.Rows[0]["RoleId"].ToString());
                    login.Username = dtPartner.Rows[0]["UserName"].ToString();
                    login.AffiliateId = dtPartner.Rows[0]["AssignedAffiliateId"].ToString();

                    if (string.IsNullOrEmpty(dtPartner.Rows[0]["Image"].ToString()))
                        login.Image = Application["SiteAddress"] + "uploads/profile/partner/defultimage.jpg";
                    else
                        login.Image = Application["SiteAddress"] + "uploads/profile/partner/" + dtPartner.Rows[0]["Image"].ToString();                    

                    Session.Add("PartnerLoginCookie", login);

                    if (chkRemember.Checked)
                    {
                        HttpCookie PartnerRememberCookie = new HttpCookie("PartnerRememberCookie");
                        PartnerRememberCookie.Values["Username"] = txtUsername.Text.Trim();
                        PartnerRememberCookie.Values["Password"] = General.EncryptVerification(txtPassword.Text.Trim());
                        PartnerRememberCookie.Expires = DateTime.Now.AddMonths(1);
                        Response.Cookies.Add(PartnerRememberCookie);
                    }

                    Response.Redirect("dashboard.aspx", true);
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
                    objPartnerService = ServiceFactory.PartnerService;
                    DataTable dtPartner = new DataTable();
                    objPartnerService.GetPartnerInfoByEmail(txtEmail.Text.Trim(), ref dtPartner);
                    if (dtPartner.Rows.Count > 0)
                    {
                        string Name = dtPartner.Rows[0]["FirstName"].ToString() + " " + dtPartner.Rows[0]["LastName"].ToString();
                        string randomString = Guid.NewGuid().ToString().Substring(0, RandomStringLength);
                        objPartnerService.SetForgotPasswordLink(txtEmail.Text.Trim(), randomString);

                        DataTable dtEmailTemplate = new DataTable();
                        objEmailTemplateService = ServiceFactory.EmailTemplateService;
                        objEmailTemplateService.GetByName("ResetPasswordPartner", ref dtEmailTemplate);
                        if (dtEmailTemplate.Rows.Count > 0)
                        {
                            string EmailBody = dtEmailTemplate.Rows[0]["EmailBody"].ToString();
                            string link = Application["SiteAddress"].ToString().Trim() + "partner/ResetPassword.aspx?Url=" + randomString;
                            EmailBody = EmailBody.Replace("{{Link}}", link);
                            EmailBody = EmailBody.Replace("{{Name}}", Name);
                            Email.SendEmail(dtEmailTemplate.Rows[0]["EmailTemplateSubject"].ToString(), txtEmail.Text.ToString(), "", "", EmailBody);
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