using Aircall.Common;
using Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Aircall
{
    public partial class Contact : System.Web.UI.Page
    {
        IContactRequestService ObjContactrequest;
        IEmailTemplateService objEmailTemplateService;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

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
                    objContact.RequestDate = DateTime.Now;

                    ObjContactrequest.AddNewContact(ref objContact);
                    DataTable dtEmailTemplate = new DataTable();
                    objEmailTemplateService = ServiceFactory.EmailTemplateService;
                    objEmailTemplateService.GetByName("ContactUs", ref dtEmailTemplate);
                    if (dtEmailTemplate.Rows.Count > 0)
                    {
                        string EmailBody = dtEmailTemplate.Rows[0]["EmailBody"].ToString();
                        Email.SendEmail(dtEmailTemplate.Rows[0]["EmailTemplateSubject"].ToString(), txtEmail.Text.ToString(), "", "", EmailBody);
                    }
                    txtYourName.Text = "";
                    txtEmail.Text  = "";
                    txtphone.Text = "";
                    txtmsg.Text = "";
                   
                }
                catch (Exception ex)
                {

                }
            }
        }

    }
}