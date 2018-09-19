using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ImageResizer;
using Services;
using System.Data;
using Aircall.Common;
using Stripe;
using AuthorizeNetLib;

namespace Aircall.admin
{
    public partial class Client_AddEdit : System.Web.UI.Page
    {
        IClientService objClientService;
        IClientAddressService objClientAddressService;
        IClientPaymentMethodService objClientPaymentMethodService;
        IPartnerService objPartnerService;
        IStripeErrorLogService objStripeErrorLogService;
        IEmailTemplateService objEmailTemplateService;
        ISiteSettingService objSiteSettingService;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["ClientId"]))
                {
                    BindClientByClientId();

                    //Client Addresses
                    dvClientAddress.Visible = true;
                    BindClientAddress();

                    //Client Payment Method
                    dvClientPayment.Visible = true;
                    BindClientPaymentMethod();
                    BindMonthYearDropdown();
                }
            }
        }

        private void BindMonthYearDropdown()
        {
            drpMonth.Enabled = true;
            drpYear.Enabled = true;

            DataTable dtMonth = new DataTable();
            dtMonth.Columns.Add("Month");
            for (int i = 01; i <= 12; i++)
            {
                dtMonth.Rows.Add(dtMonth.NewRow());
                dtMonth.Rows[dtMonth.Rows.Count - 1]["Month"] = i.ToString("00");
            }
            drpMonth.DataSource = dtMonth;
            drpMonth.DataTextField = "Month";
            drpMonth.DataValueField = "Month";
            drpMonth.DataBind();

            DataTable dtYear = new DataTable();
            dtYear.Columns.Add("Year");
            for (int i = DateTime.Now.Year; i < DateTime.Now.Year + 20; i++)
            {
                dtYear.Rows.Add(dtYear.NewRow());
                dtYear.Rows[dtYear.Rows.Count - 1]["Year"] = i.ToString();
            }
            drpYear.DataSource = dtYear;
            drpYear.DataTextField = "Year";
            drpYear.DataValueField = "Year";
            drpYear.DataBind();
        }

        private void BindClientPaymentMethod()
        {
            int ClientId = Convert.ToInt32(Request.QueryString["ClientId"].ToString());
            objClientPaymentMethodService = ServiceFactory.ClientPaymentMethodService;
            DataTable dtPaymentMethod = new DataTable();
            objClientPaymentMethodService.GetClientPaymentMethodByClientId(ClientId, ref dtPaymentMethod);
            if (dtPaymentMethod.Rows.Count > 0)
                lstCPayment.DataSource = dtPaymentMethod;
            lstCPayment.DataBind();
        }

        private void BindClientAddress()
        {
            int ClientId = Convert.ToInt32(Request.QueryString["ClientId"].ToString());
            objClientAddressService = ServiceFactory.ClientAddressService;
            DataTable dtClientAddress = new DataTable();

            if (!string.IsNullOrEmpty(Request.QueryString["ClientId"]))
                objClientAddressService.GetClientAddressesByClientId(ClientId, true, ref dtClientAddress);
            else
                objClientAddressService.GetClientAddressesByClientId(ClientId, false, ref dtClientAddress);

            if (dtClientAddress.Rows.Count > 0)
                lstCAddress.DataSource = dtClientAddress;
            lstCAddress.DataBind();
        }

        private void BindClientByClientId()
        {
            btnAdd.Visible = false;
            btnUpdate.Visible = true;

            int ClientId = Convert.ToInt32(Request.QueryString["ClientId"].ToString());
            DataTable dtClient = new DataTable();
            objClientService = ServiceFactory.ClientService;
            objClientService.GetClientById(ClientId, ref dtClient);
            if (dtClient.Rows.Count > 0)
            {
                txtFName.Text = dtClient.Rows[0]["FirstName"].ToString();
                txtLName.Text = dtClient.Rows[0]["LastName"].ToString();
                txtCompany.Text = dtClient.Rows[0]["Company"].ToString();
                txtEmail.Text = dtClient.Rows[0]["Email"].ToString();
                rqfvPassword.Enabled = false;
                hdnPassword.Value = dtClient.Rows[0]["Password"].ToString();
                txtMobile.Text = dtClient.Rows[0]["MobileNumber"].ToString();
                txtOffice.Text = dtClient.Rows[0]["OfficeNumber"].ToString();
                txtHome.Text = dtClient.Rows[0]["HomeNumber"].ToString();
                if (!string.IsNullOrEmpty(dtClient.Rows[0]["Image"].ToString()))
                {
                    lnkImage.HRef = Application["SiteAddress"] + "uploads/profile/client/" + dtClient.Rows[0]["Image"].ToString();
                    lnkImage.Visible = true;
                }
                hdnImage.Value = dtClient.Rows[0]["Image"].ToString();

                chkActive.Checked = Convert.ToBoolean(dtClient.Rows[0]["IsActive"].ToString());
                if (!string.IsNullOrEmpty(dtClient.Rows[0]["AffiliateId"].ToString()))
                {
                    txtAffiliate.Visible = false;
                    lnkAffiliate.Visible = true;
                    lnkAffiliate.HRef = Application["SiteAddress"] + "admin/Partner_AddEdit.aspx?PartnerId=" + dtClient.Rows[0]["AffiliateId"].ToString();
                    lnkAffiliate.InnerHtml = dtClient.Rows[0]["AssignedAffiliateId"].ToString();
                }
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    if (Session["LoginSession"] != null)
                    {
                        var ClientId = 0;
                        var result = this.AddClient(ref ClientId);

                        if (result)
                        {
                            this.SendEmailToAdmin();
                            this.SendEmailToClient();
                            this.AddNotification(ClientId);

                            Response.Redirect(Application["SiteAddress"] + "admin/Client_List.aspx");
                        }
                    }
                    else
                    {
                        Response.Redirect(Application["SiteAddress"] + "admin/Login.aspx");
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

        private bool AddClient(ref int ClientId)
        {
            LoginModel objLoginModel = new LoginModel();
            objLoginModel = Session["LoginSession"] as LoginModel;

            BizObjects.Client objClient = new BizObjects.Client();
            objClientService = ServiceFactory.ClientService;

            if (string.IsNullOrEmpty(txtMobile.Text.Trim()) && string.IsNullOrEmpty(txtOffice.Text.Trim()) && string.IsNullOrEmpty(txtHome.Text.Trim()))
            {
                dvMessage.InnerHtml = "<strong>Please provide one of numbers from Mobile, Office or Home</strong>";
                dvMessage.Attributes.Add("class", "alert alert-error");
                dvMessage.Visible = true;
                return false;
            }

            if (fpImage.HasFile)
            {
                string[] AllowedFileExtensions = new string[] { ".jpg", ".gif", ".png", ".jpeg" };
                if (!AllowedFileExtensions.Contains(fpImage.FileName.Substring(fpImage.FileName.LastIndexOf('.'))))
                {
                    dvMessage.InnerHtml = "<strong>Please select file of type: " + string.Join(", ", AllowedFileExtensions) + "</strong>";
                    dvMessage.Attributes.Add("class", "alert alert-error");
                    dvMessage.Visible = true;
                    return false;
                }
                else
                {
                    string filenameOriginal = DateTime.UtcNow.Ticks.ToString().Trim();
                    string fileName = fpImage.FileName;
                    string extension = System.IO.Path.GetExtension(fileName);
                    string extensionwithoutdot = extension.Remove(0, 1);

                    Instructions rsiphnWxH = new Instructions();
                    rsiphnWxH.Width = 200;
                    rsiphnWxH.Height = 200;
                    rsiphnWxH.Mode = FitMode.Stretch;
                    rsiphnWxH.Format = extensionwithoutdot;

                    ImageJob imjob = new ImageJob(fpImage.PostedFile.InputStream, Server.MapPath("~/uploads/profile/client/" + filenameOriginal + extension), rsiphnWxH);
                    imjob.CreateParentDirectory = true;
                    imjob.Build();
                    objClient.Image = filenameOriginal + extension;
                }
            }

            DataTable dtClient = new DataTable();

            objClientService.GetClientByEmail(txtEmail.Text.Trim(), 0, ref dtClient);

            if (dtClient.Rows.Count > 0)
            {
                dvMessage.InnerHtml = "<strong>Email already exist.</strong>";
                dvMessage.Attributes.Add("class", "alert alert-error");
                dvMessage.Visible = true;
                return false;
            }

            objClient.RoleId = (int)General.UserRoles.Client;

            objClient.FirstName = txtFName.Text.Trim();
            objClient.LastName = txtLName.Text.Trim();
            objClient.Company = txtCompany.Text.Trim();
            objClient.Email = txtEmail.Text.Trim();

            using (MD5 md5Hash = MD5.Create())
            {
                objClient.Password = Md5Encrypt.GetMd5Hash(md5Hash, txtPassword.Text.Trim());
            }

            objClient.PhoneNumber = txtMobile.Text.Trim();
            objClient.MobileNumber = txtMobile.Text.Trim();
            objClient.OfficeNumber = txtOffice.Text.Trim();
            objClient.HomeNumber = txtHome.Text.Trim();

            if (!string.IsNullOrEmpty(txtAffiliate.Text.Trim()))
            {
                objPartnerService = ServiceFactory.PartnerService;
                DataTable dtAffiliate = new DataTable();
                objPartnerService.GetPartnerByAffiliateId(txtAffiliate.Text.Trim(), ref dtAffiliate);
                if (dtAffiliate.Rows.Count > 0)
                    objClient.AffiliateId = Convert.ToInt32(dtAffiliate.Rows[0]["Id"].ToString());
                else
                {
                    dvMessage.InnerHtml = "<strong>Affiliate ID not found.</strong>";
                    dvMessage.Attributes.Add("class", "alert alert-error");
                    dvMessage.Visible = true;
                    return false;
                }
            }

            objClient.IsActive = chkActive.Checked;
            objClient.AddedBy = objLoginModel.Id;
            objClient.AddedByType = objLoginModel.RoleId;
            objClient.AddedDate = DateTime.UtcNow;

            string customerProfileId = "";
            var errCode = "";
            var errText = "";

            if (!objClientService.AddClientToAuthorizeNet(objClient, ref customerProfileId, ref errCode, ref errText))
            {
                dvMessage.InnerHtml = string.Format("<strong>Add client to AuthroizeNet failed. {0} {1}</strong>", errCode, errText);
                dvMessage.Attributes.Add("class", "alert alert-error");
                dvMessage.Visible = true;
                return false;
            }

            objClient.CustomerProfileId = customerProfileId;

            ClientId = objClientService.AddClient(ref objClient);

            return true;
        }

        private void SendEmailToAdmin()
        {
            //send email to admin
            objEmailTemplateService = ServiceFactory.EmailTemplateService;
            objSiteSettingService = ServiceFactory.SiteSettingService;
            DataTable dtEmailTemplate = new DataTable();
            DataTable dtSiteSetting = new DataTable();
            objEmailTemplateService.GetByName("NewUserAdmin", ref dtEmailTemplate);

            if (dtEmailTemplate.Rows.Count > 0)
            {
                string EmailBody = dtEmailTemplate.Rows[0]["EmailBody"].ToString();
                string CCEmail = dtEmailTemplate.Rows[0]["CCEmails"].ToString();
                EmailBody = EmailBody.Replace("{{FirstName}}", txtFName.Text.Trim());
                EmailBody = EmailBody.Replace("{{LastName}}", txtLName.Text.Trim());
                EmailBody = EmailBody.Replace("{{Company}}", string.IsNullOrEmpty(txtCompany.Text.Trim()) ? "-" : txtCompany.Text.Trim());
                EmailBody = EmailBody.Replace("{{Email}}", txtEmail.Text.Trim());
                EmailBody = EmailBody.Replace("{{PhoneNumber}}", txtMobile.Text.Trim());
                EmailBody = EmailBody.Replace("{{RegisterDate}}", DateTime.UtcNow.ToString("MM/dd/yyyy"));

                string Subject = dtEmailTemplate.Rows[0]["EmailTemplateSubject"].ToString();
                objSiteSettingService.GetSiteSettingByName("AdminEmail", ref dtSiteSetting);
                if (dtSiteSetting.Rows.Count > 0)
                    Email.SendEmail(Subject, dtSiteSetting.Rows[0]["Value"].ToString(), CCEmail, "", EmailBody);
            }
        }

        private void SendEmailToClient()
        {
            //send email to client
            objEmailTemplateService = ServiceFactory.EmailTemplateService;
            objSiteSettingService = ServiceFactory.SiteSettingService;
            DataTable dtEmailTemplate = new DataTable();
            DataTable dtSiteSetting = new DataTable();
            dtEmailTemplate.Clear();
            objEmailTemplateService.GetByName("NewUserClient", ref dtEmailTemplate);

            if (dtEmailTemplate.Rows.Count > 0)
            {
                string EmailBody = dtEmailTemplate.Rows[0]["EmailBody"].ToString();
                string CCEmail = dtEmailTemplate.Rows[0]["CCEmails"].ToString();
                string LoginUrl = Application["SiteAddress"].ToString() + "sign-in.aspx";
                EmailBody = EmailBody.Replace("{{FirstName}}", txtFName.Text.Trim());
                EmailBody = EmailBody.Replace("{{LastName}}", txtLName.Text.Trim());
                EmailBody = EmailBody.Replace("{{Company}}", string.IsNullOrEmpty(txtCompany.Text.Trim()) ? "-" : txtCompany.Text.Trim());
                EmailBody = EmailBody.Replace("{{Email}}", txtEmail.Text.Trim());
                EmailBody = EmailBody.Replace("{{PhoneNumber}}", txtMobile.Text.Trim());
                EmailBody = EmailBody.Replace("{{RegisterDate}}", DateTime.UtcNow.ToLocalTime().ToString("MM/dd/yyyy"));
                EmailBody = EmailBody.Replace("{{LoginUrl}}", LoginUrl);

                string Subject = dtEmailTemplate.Rows[0]["EmailTemplateSubject"].ToString();
                Email.SendEmail(Subject, txtEmail.Text.Trim(), CCEmail, "", EmailBody);
            }
        }

        private void AddNotification(int clientId)
        {
            BizObjects.UserNotification objUserNotification = new BizObjects.UserNotification();
            IUserNotificationService objUserNotificationService;
            string message = string.Empty;
            message = General.GetNotificationMessage("WelcomeUserClient");
            objUserNotificationService = ServiceFactory.UserNotificationService;
            objUserNotification.UserId = clientId;
            objUserNotification.UserTypeId = General.UserRoles.Client.GetEnumValue();
            objUserNotification.Message = message;
            objUserNotification.Status = General.NotificationStatus.UnRead.GetEnumDescription();
            objUserNotification.MessageType = General.NotificationType.FriendlyReminder.GetEnumDescription();
            objUserNotification.CommonId = 0;

            objUserNotification.AddedDate = DateTime.UtcNow;
            objUserNotificationService.AddUserNotification(ref objUserNotification);
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    if (!string.IsNullOrEmpty(Request.QueryString["ClientId"]))
                    {
                        if (UpdateClient())
                        {
                            Session["msg"] = "edit";
                            Response.Redirect(Application["SiteAddress"] + "admin/Client_List.aspx");
                        }
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

        private bool UpdateClient()
        {
            LoginModel objLoginModel = new LoginModel();
            objLoginModel = Session["LoginSession"] as LoginModel;

            int ClientId = Convert.ToInt32(Request.QueryString["ClientId"].ToString());
            BizObjects.Client objClient = new BizObjects.Client();
            objClientService = ServiceFactory.ClientService;

            if (string.IsNullOrEmpty(txtMobile.Text.Trim()) && string.IsNullOrEmpty(txtOffice.Text.Trim()) && string.IsNullOrEmpty(txtHome.Text.Trim()))
            {
                dvMessage.InnerHtml = "<strong>Please provide one of numbers from Mobile, Office or Home</strong>";
                dvMessage.Attributes.Add("class", "alert alert-error");
                dvMessage.Visible = true;
                return false;
            }

            if (!string.IsNullOrEmpty(txtAffiliate.Text.Trim()))
            {
                objPartnerService = ServiceFactory.PartnerService;
                DataTable dtAffiliate = new DataTable();
                objPartnerService.GetPartnerByAffiliateId(txtAffiliate.Text.Trim(), ref dtAffiliate);
                if (dtAffiliate.Rows.Count > 0)
                    objClient.AffiliateId = Convert.ToInt32(dtAffiliate.Rows[0]["Id"].ToString());
                else
                {
                    dvMessage.InnerHtml = "<strong>Affiliate ID not found.</strong>";
                    dvMessage.Attributes.Add("class", "alert alert-error");
                    dvMessage.Visible = true;
                    return false;
                }
            }

            DataTable dtClients = new DataTable();

            objClientService.GetClientByEmail(txtEmail.Text.Trim(), ClientId, ref dtClients);
            if (dtClients.Rows.Count > 0)
            {
                dvMessage.InnerHtml = "<strong>Email already exist.</strong>";
                dvMessage.Attributes.Add("class", "alert alert-error");
                dvMessage.Visible = true;
                return false;
            }

            if (fpImage.HasFile)
            {
                string[] AllowedFileExtensions = new string[] { ".jpg", ".gif", ".png", ".jpeg" };
                if (!AllowedFileExtensions.Contains(fpImage.FileName.Substring(fpImage.FileName.LastIndexOf('.'))))
                {
                    dvMessage.InnerHtml = "<strong>Please select file of type: " + string.Join(", ", AllowedFileExtensions) + "</strong>";
                    dvMessage.Attributes.Add("class", "alert alert-error");
                    dvMessage.Visible = true;
                    return false;
                }
                else
                {
                    string filenameOriginal = DateTime.UtcNow.Ticks.ToString().Trim();
                    string fileName = fpImage.FileName;
                    string extension = System.IO.Path.GetExtension(fileName);
                    string extensionwithoutdot = extension.Remove(0, 1);

                    Instructions rsiphnWxH = new Instructions();
                    rsiphnWxH.Width = 200;
                    rsiphnWxH.Height = 200;
                    rsiphnWxH.Mode = FitMode.Stretch;
                    rsiphnWxH.Format = extensionwithoutdot;

                    ImageJob imjob = new ImageJob(fpImage.PostedFile.InputStream, Server.MapPath("~/uploads/profile/client/" + filenameOriginal + extension), rsiphnWxH);
                    imjob.CreateParentDirectory = true;
                    imjob.Build();
                    objClient.Image = filenameOriginal + extension;
                }
            }
            else
            {
                objClient.Image = hdnImage.Value;
            }

            objClient.Id = ClientId;

            objClient.FirstName = txtFName.Text.Trim();
            objClient.LastName = txtLName.Text.Trim();
            objClient.Company = txtCompany.Text.Trim();
            objClient.Email = txtEmail.Text.Trim();

            if (!string.IsNullOrEmpty(txtPassword.Text.Trim()))
            {
                using (MD5 md5Hash = MD5.Create())
                {
                    objClient.Password = Md5Encrypt.GetMd5Hash(md5Hash, txtPassword.Text.Trim());
                }
            }
            else
            {
                objClient.Password = hdnPassword.Value;
            }

            objClient.PhoneNumber = txtMobile.Text.Trim();
            objClient.MobileNumber = txtMobile.Text.Trim();
            objClient.OfficeNumber = txtOffice.Text.Trim();
            objClient.HomeNumber = txtHome.Text.Trim();

            objClient.IsActive = chkActive.Checked;
            objClient.UpdatedBy = objLoginModel.Id;
            objClient.UpdatedByType = objLoginModel.RoleId;
            objClient.UpdatedDate = DateTime.UtcNow;

            objClientService.UpdateClient(ref objClient);

            return true;
        }

        protected void lstCPayment_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            if (e.CommandName == "UpdateCard")
            {
                int CardId = int.Parse(e.CommandArgument.ToString());
                objClientPaymentMethodService = ServiceFactory.ClientPaymentMethodService;
                DataTable dtPaymentMethod = new DataTable();
                objClientPaymentMethodService.GetClientPaymentMethodById(CardId, ref dtPaymentMethod);
                txtCardNumber.Enabled = false;
                rblVisa.Disabled = rblAmex.Disabled = rblMaster.Disabled = rblDiscover.Disabled = true;
                DataRow row = dtPaymentMethod.Rows[0];

                txtCardName.Text = row["NameOnCard"].ToString();
                txtCardNumber.Text = row["CardNumber"].ToString();
                drpMonth.SelectedValue = row["ExpiryMonth"].ToString();
                drpYear.SelectedValue = row["ExpiryYear"].ToString();
                hdnStripeCardId.Value = row["StripeCardId"].ToString();
                hdnIsDefault.Value = row["IsDefaultPayment"].ToString();
                hdnCardId.Value = e.CommandArgument.ToString();

                switch (row["CardNumber"].ToString())
                {
                    case "Visa":
                        rblVisa.Checked = true;
                        break;
                    case "AMEX":
                        rblAmex.Checked = true;
                        break;
                    case "MasterCard":
                        rblMaster.Checked = true;
                        break;
                    default:
                        rblDiscover.Checked = true;
                        break;
                }
                dvCard.Visible = true;
            }
        }

        protected void btnCardSave_Click(object sender, EventArgs e)
        {
            if (hdnStripeCardId.Value.Trim() != "")
            {
                BizObjects.ClientPaymentMethod objClientPaymentMethod = new BizObjects.ClientPaymentMethod();
                int ClientId = int.Parse(Request.QueryString["ClientId"].ToString());
                objClientService = ServiceFactory.ClientService;
                DataTable dtClient = new DataTable();
                objClientService.GetClientById(ClientId, ref dtClient);
                objClientPaymentMethodService = ServiceFactory.ClientPaymentMethodService;
                DataTable dtCardInfo = new DataTable();
                objClientPaymentMethodService.GetClientPaymentMethodById(Convert.ToInt32(hdnCardId.Value), ref dtCardInfo);

                objClientPaymentMethod.Id = Convert.ToInt32(hdnCardId.Value.ToString());
                objClientPaymentMethod.ClientId = ClientId;
                if (rblVisa.Checked)
                    objClientPaymentMethod.CardType = "Visa";
                else if (rblMaster.Checked)
                    objClientPaymentMethod.CardType = "MasterCard";
                else if (rblDiscover.Checked)
                    objClientPaymentMethod.CardType = "Discover";
                else
                    objClientPaymentMethod.CardType = "AMEX";
                string cardStr = txtCardNumber.Text.Substring(txtCardNumber.Text.Trim().Length - 4);
                objClientPaymentMethod.NameOnCard = txtCardName.Text.Trim();
                objClientPaymentMethod.ExpiryMonth = Convert.ToInt16(drpMonth.SelectedValue);
                objClientPaymentMethod.ExpiryYear = Convert.ToInt32(drpYear.SelectedValue);
                objClientPaymentMethod.IsDefaultPayment = bool.Parse(hdnIsDefault.Value);
                objClientPaymentMethod.UpdatedBy = ClientId;
                objClientPaymentMethod.UpdatedByType = General.UserRoles.Client.GetEnumValue();
                objClientPaymentMethod.UpdatedDate = DateTime.UtcNow;
                try
                {
                    if (bool.Parse(hdnIsDefault.Value))
                    {
                        StripeCustomerUpdateOptions up = new StripeCustomerUpdateOptions();
                        up.DefaultSource = dtCardInfo.Rows[0]["StripeCardId"].ToString();
                        new StripeCustomerService().Update(dtClient.Rows[0]["StripeCustomerId"].ToString(), up);
                    }
                    //stripe card update
                    var myCard = new StripeCardUpdateOptions();

                    myCard.Name = txtCardName.Text.Trim();
                    myCard.ExpirationYear = drpYear.SelectedValue;
                    myCard.ExpirationMonth = drpMonth.SelectedValue;

                    var cardService = new StripeCardService();
                    StripeCard stripeCard = cardService.Update(dtClient.Rows[0]["StripeCustomerId"].ToString(), dtCardInfo.Rows[0]["StripeCardId"].ToString(), myCard);
                    if (string.IsNullOrEmpty(stripeCard.Id))
                    {
                        dvMessage1.InnerHtml = "Invalid card.";
                        dvMessage1.Attributes.Add("class", "error");
                        dvMessage1.Visible = true;
                        return;
                    }
                    //objClientPaymentMethod.StripeCardId = stripeCard.Id;
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
                    objStripeErrorLog.Userid = ClientId;
                    objStripeErrorLog.UnitId = 0;
                    objStripeErrorLog.DateAdded = DateTime.Now;
                    objStripeErrorLogService = ServiceFactory.StripeErrorLogService;
                    objStripeErrorLogService.AddStripeErrorLog(ref objStripeErrorLog);

                    dvMessage1.InnerHtml = stex.StripeError.Message.ToString();
                    dvMessage1.Attributes.Add("class", "error");
                    dvMessage1.Visible = true;
                    return;
                }
                objClientPaymentMethodService.UpdateClientPaymentMethod(ref objClientPaymentMethod);
            }
        }
    }
}