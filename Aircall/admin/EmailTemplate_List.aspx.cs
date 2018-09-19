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
    public partial class EmailTemplate_List : System.Web.UI.Page
    {
        IEmailTemplateService objEmailTemplateService;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["msg"] != null)
                {
                    if (Session["msg"].ToString() == "edit")
                    {
                        dvMessage.InnerHtml = "<strong>Email Template updated successfully.</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-success");
                        dvMessage.Visible = true;
                    }
                    Session["msg"] = null;
                }
                BindEmailTemplates();
            }
        }

        private void BindEmailTemplates()
        {
            objEmailTemplateService = ServiceFactory.EmailTemplateService;
            DataTable dtEmailTemplates = new DataTable();
            string Name = string.Empty;
            string Subject = string.Empty;

            if (!string.IsNullOrEmpty(Request.QueryString["Name"]))
            {
                Name = Request.QueryString["Name"].ToString();
                txtName.Text = Name;
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Subject"]))
            {
                Subject = Request.QueryString["Subject"].ToString();
                txtSubject.Text = Subject;
            }

            objEmailTemplateService.GetAllEmailTemplate(Name,Subject, ref dtEmailTemplates);

            lstEmailTemplate.DataSource = dtEmailTemplates;
            lstEmailTemplate.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string Param = string.Empty;

            if (!string.IsNullOrEmpty(txtName.Text.Trim()))
                Param = "?Name=" + txtName.Text.Trim();

            if (!string.IsNullOrEmpty(txtSubject.Text.Trim()))
            {
                if (!string.IsNullOrEmpty(Param))
                    Param = Param + "&Subject=" + txtSubject.Text.Trim();
                else
                    Param = "?Subject=" + txtSubject.Text.Trim();
            }

            Response.Redirect(Application["SiteAddress"] + "admin/EmailTemplate_List.aspx" + Param);
        }

        protected void dataPagerEmailTemplate_PreRender(object sender, EventArgs e)
        {
            dataPagerEmailTemplate.PageSize = Convert.ToInt32(Application["PageSize"].ToString());
            BindEmailTemplates();
        }
    }
}