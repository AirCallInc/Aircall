using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Services;
using System.Data;
using System.Web.UI.HtmlControls;
using Aircall.Common;

namespace Aircall.admin
{
    public partial class Partner_List : System.Web.UI.Page
    {
        IPartnerService objPartnerService;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["msg"] != null)
                {
                    if (Session["msg"].ToString() == "edit")
                    {
                        dvMessage.InnerHtml = "<strong>Partner updated successfully.</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-success");
                        dvMessage.Visible = true;
                    }
                    else if (Session["msg"].ToString() == "add")
                    {
                        dvMessage.InnerHtml = "<strong>Partner added successfully.</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-success");
                        dvMessage.Visible = true;
                    }
                    Session["msg"] = null;
                }
                BindPartners();
            }
        }

        private void BindPartners()
        {
            objPartnerService = ServiceFactory.PartnerService;
            DataTable dtPartners = new DataTable();
            string PartnerName = string.Empty;
            if (!string.IsNullOrEmpty(Request.QueryString["PartnerName"]))
            {
                PartnerName = Request.QueryString["PartnerName"].ToString();
                txtPartner.Text = PartnerName;
            }
            objPartnerService.GetAllPartners(PartnerName, ListViewSortExpression, ListViewSortDirection.ToString(), false, ref dtPartners);
            if (dtPartners.Rows.Count > 0)
            {
                lstPartners.DataSource = dtPartners;
            }
            lstPartners.DataBind();
        }

        protected void Page_PreRender(object sender, System.EventArgs e)
        {
            lnkActive.Attributes.Add("onclick", "javascript:return checkActive('Are you sure want to activate selected record?','Please select atleast one record')");
            lnkInactive.Attributes.Add("onclick", "javascript:return checkInactive('Are you sure want to inactivate selected record?','Please select atleast one record')");
            lnkDelete.Attributes.Add("onclick", "javascript:return checkDelete('Are you sure want to delete selected record?','Please select atleast one record')");
        }

        protected void lnkActive_Click(object sender, EventArgs e)
        {
            if (Session["LoginSession"] != null)
            {
                LoginModel objLoginModel = new LoginModel();
                objLoginModel = Session["LoginSession"] as LoginModel;

                int UserId = objLoginModel.Id;
                int RoleId = objLoginModel.RoleId;

                bool Active = false;
                dvMessage.InnerHtml = "";
                dvMessage.Visible = false;
                objPartnerService = ServiceFactory.PartnerService;
                for (int i = 0; i <= lstPartners.Items.Count - 1; i++)
                {
                    HtmlInputCheckBox chkUsers = (HtmlInputCheckBox)lstPartners.Items[i].FindControl("chkcheck");
                    if (chkUsers.Checked)
                    {
                        HiddenField hdnPartnerId = (HiddenField)lstPartners.Items[i].FindControl("hdnPartnerId");
                        if (!string.IsNullOrEmpty(hdnPartnerId.Value))
                        {
                            objPartnerService.SetStatus(true, Convert.ToInt32(hdnPartnerId.Value),UserId,RoleId,DateTime.UtcNow);
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
                BindPartners();
            }
            else
            {
                Response.Redirect(Application["SiteAddress"] + "admin/Login.aspx");
            }
        }

        protected void lnkInactive_Click(object sender, EventArgs e)
        {
            if (Session["LoginSession"] != null)
            {
                LoginModel objLoginModel = new LoginModel();
                objLoginModel = Session["LoginSession"] as LoginModel;

                int UserId = objLoginModel.Id;
                int RoleId = objLoginModel.RoleId;

                bool InActive = false;
                dvMessage.InnerHtml = "";
                dvMessage.Visible = false;
                objPartnerService = ServiceFactory.PartnerService;
                for (int i = 0; i <= lstPartners.Items.Count - 1; i++)
                {
                    HtmlInputCheckBox chkUsers = (HtmlInputCheckBox)lstPartners.Items[i].FindControl("chkcheck");
                    if (chkUsers.Checked)
                    {
                        HiddenField hdnPartnerId = (HiddenField)lstPartners.Items[i].FindControl("hdnPartnerId");
                        if (!string.IsNullOrEmpty(hdnPartnerId.Value))
                        {
                            objPartnerService.SetStatus(false, Convert.ToInt32(hdnPartnerId.Value), UserId, RoleId, DateTime.UtcNow);
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
                BindPartners();
            }
            else
            {
                Response.Redirect(Application["SiteAddress"] + "admin/Login.aspx");
            }
        }

        protected void lnkDelete_Click(object sender, EventArgs e)
        {

        }

        protected void dataPagerPartner_PreRender(object sender, EventArgs e)
        {
            dataPagerPartner.PageSize = Convert.ToInt32(Application["PageSize"].ToString());
            BindPartners();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string Param = string.Empty;

            if (!string.IsNullOrEmpty(txtPartner.Text.Trim()))
                Param = "?PartnerName=" + txtPartner.Text.Trim();

            Response.Redirect(Application["SiteAddress"] + "admin/Partner_List.aspx" + Param);
        }

        protected SortDirection ListViewSortDirection
        {
            get
            {
                if (ViewState["sortDirection"] == null)
                    ViewState["sortDirection"] = SortDirection.Ascending;
                return (SortDirection)ViewState["sortDirection"];
            }
            set { ViewState["sortDirection"] = value; }
        }

        protected string ListViewSortExpression
        {
            get
            {
                if (ViewState["SortExpression"] == null)
                    ViewState["SortExpression"] = "Id";
                return (string)ViewState["SortExpression"];
            }
            set { ViewState["SortExpression"] = value; }
        }

        protected void lstPartners_Sorting(object sender, ListViewSortEventArgs e)
        {
            LinkButton lb = lstPartners.FindControl(e.SortExpression) as LinkButton;
            HtmlTableCell th = lb.Parent as HtmlTableCell;
            HtmlTableRow tr = th.Parent as HtmlTableRow;
            List<HtmlTableCell> ths = new List<HtmlTableCell>();
            foreach (HtmlTableCell item in tr.Controls)
            {
                try
                {
                    if (item.ID.Contains("th"))
                    {
                        item.Attributes["class"] = "sorting";
                    }
                }
                catch (Exception ex)
                {
                }
            }

            ListViewSortExpression = e.SortExpression;
            if (ListViewSortDirection == SortDirection.Ascending)
            {
                ListViewSortDirection = SortDirection.Descending;
                th.Attributes["class"] = "sorting_desc";
            }
            else
            {
                ListViewSortDirection = SortDirection.Ascending;
                th.Attributes["class"] = "sorting_asc";
            }
        }

        protected void AffiliateCount_Click(object sender, EventArgs e)
        {

        }
    }
}