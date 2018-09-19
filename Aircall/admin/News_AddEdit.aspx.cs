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
    public partial class News_AddEdit : System.Web.UI.Page
    {
        INewsService objNewsService;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["NewsId"]))
                {
                    BindNewsById();
                }
            }
        }

        private void BindNewsById()
        {
            btnSave.Text = "Update";
            //btnUpdate.Visible = true;

            int NewsId = Convert.ToInt32(Request.QueryString["NewsId"].ToString());
            objNewsService = ServiceFactory.NewsService;
            DataTable dtNews = new DataTable();
            objNewsService.GetNewsById(NewsId, ref dtNews);
            if (dtNews.Rows.Count > 0)
            {
                txtHeading.Text = dtNews.Rows[0]["NewsHeading"].ToString();
                txtURL.Text = dtNews.Rows[0]["NewsUrl"].ToString();
                txtShortDescription.Value = dtNews.Rows[0]["ShortDescription"].ToString();
                txtContent.Value = dtNews.Rows[0]["Description"].ToString();
                txtPublishDate.Text = (dtNews.Rows[0]["PublishDate"].ToString());
                chkActive.Checked = Convert.ToBoolean(dtNews.Rows[0]["IsActive"].ToString());
                txtMTitle.Text = dtNews.Rows[0]["MetaTitle"].ToString();
                txtMKeywords.Text = dtNews.Rows[0]["MetaKeywords"].ToString();
                txtMDes.Text = dtNews.Rows[0]["MetaDescription"].ToString();
                txtAMeta.Text = dtNews.Rows[0]["AdditionalMeta"].ToString();
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

                        objNewsService = ServiceFactory.NewsService;
                       
                        BizObjects.News objNews = new BizObjects.News();
                        objNews.NewsTitle = txtHeading.Text;
                        objNews.NewsUrl = txtURL.Text;
                        objNews.NewsHeading = txtHeading.Text;
                        objNews.ShortDescription = txtShortDescription.Value;
                        objNews.Description = txtContent.Value;
                        objNews.MetaTitle = txtMTitle.Text.ToString().Trim();
                        objNews.MetaKeywords = txtMKeywords.Text.ToString().Trim();
                        objNews.MetaDescription = txtMDes.Text.ToString().Trim();
                        objNews.AdditionalMeta = txtAMeta.Text.ToString().Trim();
                        objNews.PublishDate = Convert.ToDateTime(txtPublishDate.Text);
                        objNews.IsActive = chkActive.Checked;
                        objNews.AddedBy = objLoginModel.Id;
                        objNews.AddedByType = objLoginModel.RoleId;
                        objNews.AddedDate = DateTime.UtcNow;

                        if (!string.IsNullOrEmpty(Request.QueryString["NewsId"]))
                        {
                            int NewsId = Convert.ToInt32(Request.QueryString["NewsId"].ToString());
                            objNews.Id = NewsId;
                            objNews.UpdatedBy = objLoginModel.Id;
                            objNews.UpdatedByType = objLoginModel.RoleId;
                            objNews.UpdatedDate = DateTime.UtcNow;
                            objNewsService.UpdateNews(ref objNews);
                            Session["msg"] = "edit";
                            Response.Redirect(Application["SiteAddress"] + "admin/News_List.aspx");
                        }
                        else
                        {
                            Session["msg"] = "add";
                            objNewsService.AddNews(ref objNews);
                            Response.Redirect(Application["SiteAddress"] + "admin/News_List.aspx");
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

        protected void btnUpdate_Click(object sender, EventArgs e)
        {

        }
    }
}