using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Services;
using System.Data;
using Aircall.Common;

namespace Aircall.admin
{
    public partial class EmailTemplate_Edit : System.Web.UI.Page
    {
        IEmailTemplateService objEmailTemplateService;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["Id"]))
                {
                    BindEmailTemplateById();
                }
            }
        }

        private void BindEmailTemplateById()
        {
            int Id = Convert.ToInt32(Request.QueryString["Id"].ToString());
            DataTable dtEmailTemplate = new DataTable();
            objEmailTemplateService = ServiceFactory.EmailTemplateService;
            objEmailTemplateService.GetById(Id, ref dtEmailTemplate);
            if (dtEmailTemplate.Rows.Count > 0)
            {
                txtTemplateName.Text = dtEmailTemplate.Rows[0]["Name"].ToString();
                txtSubject.Text = dtEmailTemplate.Rows[0]["EmailTemplateSubject"].ToString();
                txtEmail.Text = dtEmailTemplate.Rows[0]["FromEmail"].ToString();
                txtCCEmails.Text = dtEmailTemplate.Rows[0]["CCEmails"].ToString();
                CKEditor1.Value = dtEmailTemplate.Rows[0]["EmailBody"].ToString();
                ltrAvailableTag.Text = dtEmailTemplate.Rows[0]["AvailableTags"].ToString();
                txtTemplateName.Enabled = false;
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    if (Session["LoginSession"] != null)
                    {
                        LoginModel objLoginModel = new LoginModel();
                        objLoginModel = Session["LoginSession"] as LoginModel;
                        BizObjects.EmailTemplate objEmailTemplate = new BizObjects.EmailTemplate();
                        if (!string.IsNullOrEmpty(Request.QueryString["Id"]))
                        {
                            if ((string.IsNullOrEmpty(CKEditor1.Value.ToString()) && CKEditor1.Value.ToString().Trim() != "<br />" && CKEditor1.Value.ToString().Trim() != " "))
                            {
                                dvMessage.InnerHtml = "<strong>Email Body is required.</strong>";
                                dvMessage.Attributes.Add("class", "alert alert-error");
                                dvMessage.Visible = true;
                                return;
                            }

                            int Id = Convert.ToInt32(Request.QueryString["Id"].ToString());
                            objEmailTemplate.Id = Id;
                            objEmailTemplate.EmailTemlateSubject = txtSubject.Text.Trim();
                            objEmailTemplate.FromEmail = txtEmail.Text.Trim();
                            objEmailTemplate.CCEmails = txtCCEmails.Text.Trim();
                            objEmailTemplate.EmailBody = CKEditor1.Value.ToString().Trim();
                            objEmailTemplate.UpdatedBy = objLoginModel.Id;
                            objEmailTemplate.UpdatedByType = objLoginModel.RoleId;
                            objEmailTemplate.UpdatedDate = DateTime.UtcNow;
                            objEmailTemplateService = ServiceFactory.EmailTemplateService;
                            objEmailTemplateService.UpdateEmailTemplate(ref objEmailTemplate);

                            Session["msg"] = "edit";
                            Response.Redirect(Application["SiteAddress"] + "admin/EmailTemplate_List.aspx");
                        }
                    }
                }
                catch (Exception Ex)
                {
                    dvMessage.InnerHtml = "<strong>Error ! </strong>" + Ex.Message.ToString().Trim();
                    dvMessage.Attributes.Add("class", "alert alert-error");
                    dvMessage.Visible = true;
                }
            }
        }
    }
}