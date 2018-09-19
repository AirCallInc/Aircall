using Aircall.Common;
using Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Aircall.admin
{
    public partial class ContactRequest_SendEmail : System.Web.UI.Page
    {
        IContactRequestService objContactRequestService;
        IEmailTemplateService objEmailTemplateService;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["RequestId"]))
                {
                    BindRequestById();
                }
            }
        }

        private void BindRequestById()
        {
            int RequestId = Convert.ToInt32(Request.QueryString["RequestId"].ToString());
            objContactRequestService = ServiceFactory.ContactRequestService;
            DataTable dtRequest = new DataTable();
            objContactRequestService.GetContactRequestById(RequestId, ref dtRequest);
            if (dtRequest.Rows.Count > 0)
            {
                ltrName.Text = dtRequest.Rows[0]["Name"].ToString();
                ltrEmail.Text = dtRequest.Rows[0]["Email"].ToString();
                ltrMessage.Text = dtRequest.Rows[0]["Message"].ToString();
                ltrReqDate.Text = dtRequest.Rows[0]["RequestDate"].ToString();
                txtSubject.Text = dtRequest.Rows[0]["ResponseEmailSubject"].ToString();
                txtBody.Value = dtRequest.Rows[0]["ResponseBody"].ToString();
            }
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    if (Session["LoginSession"] != null && !string.IsNullOrEmpty(Request.QueryString["RequestId"]))
                    {
                        LoginModel objLoginModel = new LoginModel();
                        objLoginModel = Session["LoginSession"] as LoginModel;

                        int UserId = objLoginModel.Id;
                        int RequestId=Convert.ToInt32(Request.QueryString["RequestId"].ToString());

                        objContactRequestService = ServiceFactory.ContactRequestService;
                        BizObjects.ContactRequest objContactRequest = new BizObjects.ContactRequest();
                        
                        objContactRequest.Id=RequestId;
                        objContactRequest.ResponseEmailSubject = txtSubject.Text.Trim();
                        objContactRequest.ResponseBody = txtBody.Value;
                        objContactRequest.ResponseBy = UserId;
                        objContactRequest.ResponseDate = DateTime.UtcNow;

                        DataTable dtRequest = new DataTable();

                        objContactRequestService.GetContactRequestById(RequestId, ref dtRequest);
                        if (dtRequest.Rows.Count > 0)
                        {
                            string EmailTo=dtRequest.Rows[0]["Email"].ToString();
                            Email.SendEmail(txtSubject.Text.Trim(), EmailTo, "", "", txtBody.Value);
                            //Email.Send("testlocalcoding@gmail.com", "this.admin", txtSubject.Text.Trim(), EmailTo, txtBody.Value, "smtp.gmail.com", 587, true);
                        }
                        
                        objContactRequestService.UpdateContactRequest(ref objContactRequest);

                        Session["msg"] = "sent";
                        Response.Redirect(Application["SiteAddress"] + "/admin/ContactRequest_List.aspx");
                    }
                    else
                    {
                        Response.Redirect(Application["SiteAddress"] + "/admin/Login.aspx");
                    }
                }
                catch (Exception Ex)
                {
                    dvMessage.InnerHtml = "<strong>Error!</strong> " + Ex.Message.Trim();
                    dvMessage.Attributes.Add("class", "alert alert-error");
                    dvMessage.Visible = true;
                }
            }
        }

        protected void chkUseTemplate_CheckedChanged(object sender, EventArgs e)
        {
            if (chkUseTemplate.Checked)
            {
                objEmailTemplateService = ServiceFactory.EmailTemplateService;
                DataTable dtEmailTemplate = new DataTable();
                objEmailTemplateService.GetByName("ContactUsReply", ref dtEmailTemplate);
                if (dtEmailTemplate.Rows.Count > 0)
                {
                    txtSubject.Text = dtEmailTemplate.Rows[0]["EmailTemplateSubject"].ToString();
                    txtBody.Value=dtEmailTemplate.Rows[0]["EmailBody"].ToString();
                }
            }
            else
            {
                txtSubject.Text = "";
                txtBody.Value = "";
            }
                
        }
    }
}