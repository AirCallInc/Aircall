using Aircall.Common;
using Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Aircall
{
    public partial class ClientContact_Us : System.Web.UI.Page
    {
        IBlocksService objBlocksService = ServiceFactory.BlocksService;
        ICMSPagesService objCMSService = ServiceFactory.CMSPagesService;
        IContactRequestService ObjContactrequest;
        IEmailTemplateService objEmailTemplateService;
        IClientService objClientService = ServiceFactory.ClientService;
        DataTable dtCMSService = new DataTable();
        DataTable dtCMSSectionService = new DataTable();
        ISiteSettingService objSiteSettingService = ServiceFactory.SiteSettingService;
         
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string url = Page.RouteData.Values["Contact"] as string;
                string str = "";
                if (url != null)
                {
                    url = url.Replace("/", "");
                    str = url.Split('.')[0].ToString();
                }
                else
                {
                    str = "contact-us";
                }
                InnerHTML(str);
                if (Session["ClientLoginCookie"] != null)
                {
                    LoginModel login = Session["ClientLoginCookie"] as LoginModel;
                    DataTable dt = new DataTable();
                    objClientService.GetClientById(login.Id, ref dt);

                    DataRow row = dt.Rows[0];
                    txtYourName.Text = row["ClientName"].ToString();
                    txtEmail.Text = row["Email"].ToString();
                    txtphone.Text = row["MobileNumber"].ToString();
                }
            }
        }
        private void InnerHTML(string url)
        {
            string iBanner = String.Empty;

            string iMBanner = String.Empty;
            string iFooter = String.Empty;
            string iContent = String.Empty;
            DataTable dtSection = new DataTable();
            DataTable dtMSection = new DataTable();
            DataTable dtFSection = new DataTable();
            objCMSService.GetPageContent(url, ref dtCMSService);

            if (dtCMSService.Rows.Count > 0)
            {
                foreach (DataRow item in dtCMSService.Rows)
                {
                    iContent = iContent + item["Description"].ToString();

                }
                if (!string.IsNullOrWhiteSpace(dtCMSService.Rows[0]["BannerImage"].ToString()))
                {
                    BanngerImg.Attributes.Add("style", "background-image:url('" + Application["SiteAddress"] + "uploads/BannerImg/" + dtCMSService.Rows[0]["BannerImage"].ToString() + "')");
                }
                else
                {
                    BanngerImg.Visible = false;
                }
                ltBannerText.Text = dtCMSService.Rows[0]["PageTitle"].ToString();
            }

            if (dtCMSService.Rows.Count > 0)
            {
                Page.Title = dtCMSService.Rows[0]["MetaTitle"].ToString();
                Page.MetaDescription = dtCMSService.Rows[0]["MetaDescription"].ToString();
                Page.MetaKeywords = dtCMSService.Rows[0]["MetaKeywords"].ToString();

                objBlocksService.GetBlockByCMSId(Convert.ToInt32(dtCMSService.Rows[0]["Id"].ToString()), ref dtSection);
                if (dtSection.Rows.Count > 0)
                {
                    string filter = "Position='Top'";
                    DataView dv = new DataView(dtSection, filter, "Order", DataViewRowState.CurrentRows);
                    dtSection = dv.ToTable();
                    if (dtSection.Rows.Count > 0)
                    {
                        foreach (DataRow item in dtSection.Rows)
                        {
                            iBanner = iBanner + item["Description"].ToString();
                        }
                    }
                }
                objBlocksService.GetBlockByCMSId(Convert.ToInt32(dtCMSService.Rows[0]["Id"].ToString()), ref dtMSection);
                if (dtMSection.Rows.Count > 0)
                {
                    string filter = "Position='Middle'";
                    DataView dv = new DataView(dtMSection, filter, "Order", DataViewRowState.CurrentRows);
                    dtMSection = dv.ToTable();
                    if (dtMSection.Rows.Count > 0)
                    {
                        foreach (DataRow item in dtMSection.Rows)
                        {
                            iMBanner = iMBanner + item["Description"].ToString();
                        }
                    }
                }
                objBlocksService.GetBlockByCMSId(Convert.ToInt32(dtCMSService.Rows[0]["Id"].ToString()), ref dtFSection);
                if (dtFSection.Rows.Count > 0)
                {
                    string filter = "Position='Bottom'";
                    DataView dv = new DataView(dtFSection, filter, "Order", DataViewRowState.CurrentRows);
                    dtFSection = dv.ToTable();
                    if (dtFSection.Rows.Count > 0)
                    {
                        foreach (DataRow item in dtFSection.Rows)
                        {
                            iFooter = iFooter + item["Description"].ToString();
                        }
                    }
                }
                //ltBanner.Text = iBanner;
                ltMiddle.Text = iMBanner;
                ltContent.Text = iContent;
                ltBottom.Text = iFooter;
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    BizObjects.ContactRequest objContact = new BizObjects.ContactRequest();
                    ObjContactrequest = ServiceFactory.ContactRequestService;

                    objContact.Name = txtYourName.Text.ToString().Trim();
                    objContact.Email = txtEmail.Text.ToString().Trim();
                    objContact.PhoneNumber = txtphone.Text.ToString();
                    objContact.Message = txtmsg.Text.ToString().Trim();
                    objContact.RequestDate = DateTime.UtcNow;

                    ObjContactrequest.AddNewContact(ref objContact);
                    DataTable dtEmailTemplate = new DataTable();
                    objEmailTemplateService = ServiceFactory.EmailTemplateService;
                    objEmailTemplateService.GetByName("ContactUs", ref dtEmailTemplate);
                    if (dtEmailTemplate.Rows.Count > 0)
                    {
                        string EmailBody = dtEmailTemplate.Rows[0]["EmailBody"].ToString();
                        string CCEmail = dtEmailTemplate.Rows[0]["CCEmails"].ToString();
                        EmailBody= EmailBody.Replace("{{FirstName}}", txtYourName.Text.Trim());
                        Email.SendEmail(dtEmailTemplate.Rows[0]["EmailTemplateSubject"].ToString(), txtEmail.Text.ToString(), CCEmail, "", EmailBody);
                    }
                    dtEmailTemplate.Clear();
                    objEmailTemplateService.GetByName("ContactUsAdmin", ref dtEmailTemplate);
                    if (dtEmailTemplate.Rows.Count > 0)
                    {
                        string EmailBody = dtEmailTemplate.Rows[0]["EmailBody"].ToString();
                        string CCEmail = dtEmailTemplate.Rows[0]["CCEmails"].ToString();
                        EmailBody=EmailBody.Replace("{{Name}}", txtYourName.Text.Trim());
                        EmailBody = EmailBody.Replace("{{Email}}", txtEmail.Text.Trim());
                        EmailBody = EmailBody.Replace("{{PhoneNumber}}", txtphone.Text.Trim());
                        EmailBody = EmailBody.Replace("{{Message}}", txtmsg.Text.Trim());
                        DataTable dtSiteSetting= new DataTable();
                        objSiteSettingService.GetSiteSettingByName("AdminEmail", ref dtSiteSetting);
                        Email.SendEmail(dtEmailTemplate.Rows[0]["EmailTemplateSubject"].ToString(), dtSiteSetting.Rows[0]["Value"].ToString(), CCEmail, "", EmailBody);
                    }

                    if (Session["ClientLoginCookie"] != null)
                    {
                        LoginModel login = Session["ClientLoginCookie"] as LoginModel;
                        DataTable dt = new DataTable();
                        objClientService.GetClientById(login.Id, ref dt);

                        DataRow row = dt.Rows[0];
                        txtYourName.Text = row["ClientName"].ToString();
                        txtEmail.Text = row["Email"].ToString();
                        txtphone.Text = row["MobileNumber"].ToString();
                    }
                    else
                    {
                        txtYourName.Text = "";
                        txtEmail.Text = "";
                        txtphone.Text = "";
                    }
                    txtmsg.Text = "";
                    dvMessage.InnerText = "Your contact request has been submitted successfully!";
                    dvMessage.Attributes.Add("class", "success");
                    dvMessage.Visible = true;

                }
                catch (Exception ex)
                {

                }
            }
        }
    }
}