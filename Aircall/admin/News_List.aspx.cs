using Aircall.Common;
using Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Aircall.admin
{
    public partial class News_List : System.Web.UI.Page
    {
        INewsService objNewsService;
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                if (Session["msg"] != null)
                {
                    if (Session["msg"].ToString() == "edit")
                    {
                        dvMessage.InnerHtml = "<strong>News updated successfully.</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-success");
                        dvMessage.Visible = true;
                    }
                    else if (Session["msg"].ToString() == "add")
                    {
                        dvMessage.InnerHtml = "<strong>News added successfully.</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-success");
                        dvMessage.Visible = true;
                    }
                    Session["msg"] = null;
                }
                BindNews();
            }
        }
        protected void Page_PreRender(object sender, System.EventArgs e)
        {
            lnkActive.Attributes.Add("onclick", "javascript:return checkActive('Are you sure want to activate selected record?','Please select atleast one record')");
            lnkInactive.Attributes.Add("onclick", "javascript:return checkInactive('Are you sure want to inactivate selected record?','Please select atleast one record')");
            lnkDelete.Attributes.Add("onclick", "javascript:return checkDelete('Are you sure want to delete selected record?','Please select atleast one record')");
        }
        private void BindNews()
        {
            objNewsService = ServiceFactory.NewsService;
            DataTable dtNews = new DataTable();
            objNewsService.GetAllNews(false,ref dtNews);
            if (dtNews.Rows.Count > 0)
            {
                lstNews.DataSource = dtNews;
            }
            lstNews.DataBind();
        }
        protected void lnkActive_Click(object sender, EventArgs e)
        {
            bool Active = false;
            dvMessage.InnerHtml = "";
            dvMessage.Visible = false;
            objNewsService = ServiceFactory.NewsService;
            for (int i = 0; i <= lstNews.Items.Count - 1; i++)
            {
                HtmlInputCheckBox chkUsers = (HtmlInputCheckBox)lstNews.Items[i].FindControl("chkcheck");
                if (chkUsers.Checked)
                {
                    HiddenField hdNewsId = (HiddenField)lstNews.Items[i].FindControl("hdNewsId");
                    if (!string.IsNullOrEmpty(hdNewsId.Value))
                    {
                        objNewsService.SetStatus(true, Convert.ToInt32(hdNewsId.Value));
                        Active = true;
                    }
                }
            }
            if (Active)
            {
                dvMessage.InnerHtml = "<strong>Record updated successfully.</strong>";
                dvMessage.Attributes.Add("class", "alert alert-success");
                dvMessage.Visible = true;
            }
            BindNews();
        }

        protected void lnkInactive_Click(object sender, EventArgs e)
        {
            bool InActive = false;
            dvMessage.InnerHtml = "";
            dvMessage.Visible = false;
            objNewsService = ServiceFactory.NewsService;
            for (int i = 0; i <= lstNews.Items.Count - 1; i++)
            {
                HtmlInputCheckBox chkUsers = (HtmlInputCheckBox)lstNews.Items[i].FindControl("chkcheck");
                if (chkUsers.Checked)
                {
                    HiddenField hdnCityId = (HiddenField)lstNews.Items[i].FindControl("hdNewsId");
                    if (!string.IsNullOrEmpty(hdnCityId.Value))
                    {
                        objNewsService.SetStatus(false, Convert.ToInt32(hdnCityId.Value));
                        InActive = true;
                    }
                }
            }
            if (InActive)
            {
                dvMessage.InnerHtml = "<strong>Record updated successfully.</strong>";
                dvMessage.Attributes.Add("class", "alert alert-success");
                dvMessage.Visible = true;
            }
            BindNews();
        }

        protected void lnkDelete_Click(object sender, EventArgs e)
        {
            bool Delete = false;
            dvMessage.InnerHtml = "";
            dvMessage.Visible = false;

            LoginModel objLoginModel = new LoginModel();
            objLoginModel = Session["LoginSession"] as LoginModel;

            int UserId = objLoginModel.Id;
            int RoleId = objLoginModel.RoleId;
            BizObjects.News objNews = new BizObjects.News();
            objNewsService = ServiceFactory.NewsService;

            for (int i = 0; i <= lstNews.Items.Count - 1; i++)
            {
                HtmlInputCheckBox chkUsers = (HtmlInputCheckBox)lstNews.Items[i].FindControl("chkcheck");
                if (chkUsers.Checked)
                {
                    HiddenField hdNewsId = (HiddenField)lstNews.Items[i].FindControl("hdNewsId");
                    if (!string.IsNullOrEmpty(hdNewsId.Value))
                    {
                        objNews.Id = Convert.ToInt32(hdNewsId.Value);
                        objNews.DeletedBy = UserId;
                        objNews.DeletedByType = RoleId;
                        objNewsService.DeleteNews(ref objNews);
                        Delete = true;
                    }
                }
            }
            if (Delete)
            {
                dvMessage.InnerHtml = "<strong>Record deleted successfully.</strong>";
                dvMessage.Attributes.Add("class", "alert alert-success");
                dvMessage.Visible = true;
            }
            BindNews();
        }
    }
}