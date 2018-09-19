using Aircall.Common;
using Services;
using Stripe;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Aircall
{
    public partial class signup : System.Web.UI.Page
    {
        #region "Declaration"
        IClientService objClientService = ServiceFactory.ClientService;
        IEmailTemplateService objEmailTemplateService = ServiceFactory.EmailTemplateService;
        IPartnerService objPartnerService = ServiceFactory.PartnerService;
        IStripeErrorLogService objStripeErrorLogService = ServiceFactory.StripeErrorLogService;
        ISiteSettingService objSiteSettingService = ServiceFactory.SiteSettingService;
        ICMSPagesService objCMSPagesService = ServiceFactory.CMSPagesService;
        DataTable dtClient = new DataTable();
        #endregion

        #region "Page Events"
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["ClientLoginCookie"] != null)
            {
                Response.Redirect(Application["SiteAddress"].ToString() + "client/dashboard.aspx", false);
            }

        }
        #endregion

        #region "Events"
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (!chkTerms.Checked)
            {
                dvMessage.InnerHtml = "Please Accept Terms & Condition.";
                dvMessage.Attributes.Add("class", "error");
                dvMessage.Visible = true;
                return;
            }
            if (Page.IsValid)
            {
                try
                {
                    objClientService.GetClientByEmail(txtEmail.Text.Trim(), 0,ref dtClient);
                    if (dtClient.Rows.Count > 0)
                    {
                        dvMessage.InnerHtml = "<strong>Email already exists.</strong>";
                        dvMessage.Attributes.Add("class", "error");
                        dvMessage.Visible = true;
                        return;
                    }

                    BizObjects.Client objClient = new BizObjects.Client();
                    objClient.RoleId = General.UserRoles.Client.GetEnumValue();
                    //if (rbtnMore.Checked)
                    //    objClient.HVACUnit = "My HVAC Unit is more than 10 years old";
                    //else if (rbtnLess.Checked)
                    //    objClient.HVACUnit = "My HVAC Unit is less than 10 years old";
                    //else
                    //    objClient.HVACUnit = "I don't know how old my HVAC Unit is";

                    objClient.FirstName = txtFirstName.Text.Trim();
                    objClient.LastName = txtLastName.Text.Trim();
                    objClient.Company = txtCompany.Text.Trim();
                    objClient.Email = txtEmail.Text.Trim();
                    objClient.MobileNumber = txtPhone.Text.Trim();
                    using (MD5 md5Hash = MD5.Create())
                    {
                        objClient.Password = Md5Encrypt.GetMd5Hash(md5Hash, txtPassword.Text.Trim());
                    }
                    if (!string.IsNullOrEmpty(txtPartner.Text.Trim()))
                    {
                        objPartnerService = ServiceFactory.PartnerService;
                        DataTable dtAffiliate = new DataTable();
                        objPartnerService.GetPartnerByAffiliateId(txtPartner.Text.Trim(), ref dtAffiliate);
                        if (dtAffiliate.Rows.Count > 0)
                            objClient.AffiliateId = Convert.ToInt32(dtAffiliate.Rows[0]["Id"].ToString());
                        else
                        {
                            dvMessage.InnerHtml = "<strong>Affiliate ID not found.</strong>";
                            dvMessage.Attributes.Add("class", "error");
                            dvMessage.Visible = true;
                            return;
                        }
                    }
                    objClient.IsActive = true;
                    objClient.AddedByType = General.UserRoles.Client.GetEnumValue();
                    objClient.AddedDate = DateTime.UtcNow;

                    string customerProfileId = "";
                    var errCode = "";
                    var errText = "";

                    if (!objClientService.AddClientToAuthorizeNet(objClient, ref customerProfileId, ref errCode, ref errText))
                    {
                        dvMessage.InnerHtml = string.Format("<strong>Add client to AuthroizeNet failed. {0} {1}</strong>", errCode, errText);
                        dvMessage.Attributes.Add("class", "alert alert-error");
                        dvMessage.Visible = true;
                        return;
                    }

                    objClient.CustomerProfileId = customerProfileId;
                    int ClientId = 0;
                    ClientId = objClientService.AddClient(ref objClient);
                    if (ClientId != 0)
                    {
                        LoginModel login = new LoginModel();
                        login.Id = ClientId;
                        login.FullName = txtFirstName.Text.Trim() + " " + txtLastName.Text.Trim(); ;
                        login.RoleId = General.UserRoles.Client.GetEnumValue();

                        Session.Add("ClientLoginCookie", login);

                        //send email to admin
                        DataTable dtEmailTemplate = new DataTable();
                        DataTable dtSiteSetting = new DataTable();
                        objEmailTemplateService.GetByName("NewUserAdmin", ref dtEmailTemplate);
                        if (dtEmailTemplate.Rows.Count > 0)
                        {
                            string EmailBody = dtEmailTemplate.Rows[0]["EmailBody"].ToString();
                            string CCEmail = dtEmailTemplate.Rows[0]["CCEmails"].ToString();
                            EmailBody = EmailBody.Replace("{{FirstName}}", txtFirstName.Text.Trim());
                            EmailBody = EmailBody.Replace("{{LastName}}", txtLastName.Text.Trim());
                            EmailBody = EmailBody.Replace("{{Company}}", string.IsNullOrEmpty(txtCompany.Text.Trim()) ? "-" : txtCompany.Text.Trim());
                            EmailBody = EmailBody.Replace("{{Email}}", txtEmail.Text.Trim());
                            EmailBody = EmailBody.Replace("{{PhoneNumber}}", txtPhone.Text.Trim());
                            EmailBody = EmailBody.Replace("{{RegisterDate}}", DateTime.UtcNow.ToString("MM/dd/yyyy"));

                            string Subject = dtEmailTemplate.Rows[0]["EmailTemplateSubject"].ToString();
                            objSiteSettingService.GetSiteSettingByName("AdminEmail", ref dtSiteSetting);
                            if (dtSiteSetting.Rows.Count > 0)
                                Email.SendEmail(Subject, dtSiteSetting.Rows[0]["Value"].ToString(), CCEmail, "", EmailBody);
                        }

                        //send email to client
                        dtEmailTemplate.Clear();
                        objEmailTemplateService.GetByName("NewUserClient", ref dtEmailTemplate);
                        if (dtEmailTemplate.Rows.Count > 0)
                        {
                            string EmailBody = dtEmailTemplate.Rows[0]["EmailBody"].ToString();
                            string CCEmail = dtEmailTemplate.Rows[0]["CCEmails"].ToString();
                            string LoginUrl = Application["SiteAddress"].ToString() + "sign-in.aspx";
                            EmailBody = EmailBody.Replace("{{FirstName}}", txtFirstName.Text.Trim());
                            EmailBody = EmailBody.Replace("{{LastName}}", txtLastName.Text.Trim());
                            EmailBody = EmailBody.Replace("{{Company}}", string.IsNullOrEmpty(txtCompany.Text.Trim()) ? "-" : txtCompany.Text.Trim());
                            EmailBody = EmailBody.Replace("{{Email}}", txtEmail.Text.Trim());
                            EmailBody = EmailBody.Replace("{{PhoneNumber}}", txtPhone.Text.Trim());
                            EmailBody = EmailBody.Replace("{{RegisterDate}}", DateTime.UtcNow.ToLocalTime().ToString("MM/dd/yyyy"));
                            EmailBody = EmailBody.Replace("{{LoginUrl}}", LoginUrl);

                            string Subject = dtEmailTemplate.Rows[0]["EmailTemplateSubject"].ToString();
                            Email.SendEmail(Subject, txtEmail.Text.Trim(), CCEmail, "", EmailBody);
                        }

                        BizObjects.UserNotification objUserNotification = new BizObjects.UserNotification();
                        IUserNotificationService objUserNotificationService;
                        string message = string.Empty;
                        message = General.GetNotificationMessage("WelcomeUserClient");
                        objUserNotificationService = ServiceFactory.UserNotificationService;
                        objUserNotification.UserId = ClientId;
                        objUserNotification.UserTypeId = General.UserRoles.Client.GetEnumValue();
                        objUserNotification.Message = message;
                        objUserNotification.Status = General.NotificationStatus.UnRead.GetEnumDescription();
                        objUserNotification.MessageType = General.NotificationType.FriendlyReminder.GetEnumDescription();
                        objUserNotification.CommonId = 0;

                        objUserNotification.AddedDate = DateTime.UtcNow;

                        //Code Commented on 13-07-2017 to not dispaly notification in current version.. Remove comment in next version 
                        //objUserNotificationService.AddUserNotification(ref objUserNotification);

                        Response.Redirect(Application["SiteAddress"].ToString() + "client/dashboard.aspx", false);
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