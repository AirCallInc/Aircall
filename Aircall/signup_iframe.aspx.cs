using Aircall.Common;
using Services;
using Stripe;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Aircall
{
    public partial class signup_iframe : System.Web.UI.Page
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
            //if (Session["ClientLoginCookie"] != null)
            //{
            //    Response.Redirect(Application["SiteAddress"].ToString() + "client/dashboard.aspx", false);
            //}

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
            var a = int.Parse(hdnNum1.Value);
            var b = int.Parse(hdnNum2.Value);
            var c = int.Parse((string.IsNullOrWhiteSpace(txtCaptcha.Text) ? "0" : txtCaptcha.Text));
            if ((a + b) != c)
            {
                dvMessage.InnerHtml = "Please Enter Valid Captcha .";
                dvMessage.Attributes.Add("class", "error");
                dvMessage.Visible = true;
                return;
            }
            if (Page.IsValid)
            {
                try
                {
                    objClientService.GetClientByEmail(txtEmail.Text.Trim(), 0, ref dtClient);
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
                    objClient.AddedDate = DateTime.Now;

                    //create new customer in stripe
                    try
                    {
                        var myCustomer = new StripeCustomerCreateOptions();

                        myCustomer.Email = objClient.Email;
                        myCustomer.Description = objClient.FirstName + ' ' + objClient.LastName + " (" + objClient.Email + ")";
                        var customerService = new StripeCustomerService();
                        StripeCustomer stripeCustomer = customerService.Create(myCustomer);
                        //objClient.StripeCustomerId = stripeCustomer.Id;
                    }
                    catch (StripeException stex)
                    {
                        BizObjects.StripeErrorLog objStripeErrorLog = new BizObjects.StripeErrorLog();
                        objStripeErrorLog.ChargeId = stex.StripeError.ChargeId;
                        objStripeErrorLog.Code = stex.StripeError.Code;
                        objStripeErrorLog.DeclineCode = stex.StripeError.DeclineCode;
                        objStripeErrorLog.ErrorType = stex.StripeError.ErrorType;
                        objStripeErrorLog.Error = stex.StripeError.Error;
                        objStripeErrorLog.ErrorSubscription = stex.StripeError.ErrorSubscription;
                        objStripeErrorLog.Message = stex.StripeError.Message;
                        objStripeErrorLog.Parameter = stex.StripeError.Parameter;
                        objStripeErrorLog.Userid = 0;
                        objStripeErrorLog.UnitId = 0;
                        objStripeErrorLog.DateAdded = DateTime.Now;

                        objStripeErrorLogService.AddStripeErrorLog(ref objStripeErrorLog);

                        dvMessage.InnerHtml = stex.StripeError.Message.ToString();
                        dvMessage.Attributes.Add("class", "error");
                        dvMessage.Visible = true;
                    }

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
                        dvMessage.InnerHtml = "Client Successfully registered! Please wait while we are redirecting.";
                        dvMessage.Attributes.Add("class", "error");
                        dvMessage.Visible = true;
                        Response.Write("<script>window.open('" + Application["SiteAddress"].ToString() + "client/dashboard.aspx" + "','_parent');</script>");
                        //Response.Redirect(Application["SiteAddress"].ToString() + "client/dashboard.aspx", false);
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

        protected void btnPreReg_Click(object sender, EventArgs e)
        {
            var request = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["APIURL"].ToString().Trim() + "api/v1/profile/checkClientExists?" + "Email=" + Server.UrlEncode(txtPreEmail.Text));

            var postData = "Email=" + Server.UrlEncode(txtPreEmail.Text);
            var data = Encoding.ASCII.GetBytes(postData);

            request.Method = "GET";
            //request.ContentType = "application/json";

            //using (var stream = request.GetRequestStream())
            //{
            //    stream.Write(data, 0, data.Length);
            //}

            var response = (HttpWebResponse)request.GetResponse();

            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();


            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseModel>(responseString);
            if (obj.StatusCode == 200)
            {
                hdnExtra.Value = "40";
                pnlPreReg.Visible = false;
                pnlReg.Visible = true;
                txtEmail.Text = txtPreEmail.Text;
                dvMessage.Visible = false;
            }
            else
            {
                dvMessage.InnerText = obj.Message;
                dvMessage.Attributes.Add("class", "error");
                dvMessage.Visible = true;
            }
        }
    }
    public class ResponseModel
    {
        public int StatusCode { get; set; }
        public string Token { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
        public Nullable<DateTime> LastCallDateTime { get; set; }
    }
}