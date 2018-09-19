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

    public partial class CMSPages_List : System.Web.UI.Page
    {
        ICMSPagesService objCMSList;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindListOfCMSPages();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void BindListOfCMSPages()
        {

            objCMSList = ServiceFactory.CMSPagesService;
            DataTable dtCMSPages = new DataTable();

            objCMSList.GetAllCMSPages(PageTitleExpression, MenuTitleExpression, ListViewSortExpression, ListViewSortDirection.ToString(), ref dtCMSPages);
            if (dtCMSPages.Rows.Count > 0)
            {
                lstCMSPages.DataSource = dtCMSPages;
            }
            lstCMSPages.DataBind();
        }
        protected void Page_PreRender(object sender, System.EventArgs e)
        {
            lnkActive.Attributes.Add("onclick", "javascript:return checkActive('Are you sure want to activate selected record?','Please select atleast one record')");
            lnkInactive.Attributes.Add("onclick", "javascript:return checkInactive('Are you sure want to inactivate selected record?','Please select atleast one record')");
            lnkDelete.Attributes.Add("onclick", "javascript:return checkDelete('Are you sure want to delete selected record?','Please select atleast one record')");
        }



        protected void lnkActive_Click(object sender, EventArgs e)
        {
            try
            {
                bool Active = false;
                dvMessage.InnerHtml = "";
                dvMessage.Visible = false;
                objCMSList = ServiceFactory.CMSPagesService;
                for (int i = 0; i <= lstCMSPages.Items.Count - 1; i++)
                {
                    HtmlInputCheckBox chkCMS = (HtmlInputCheckBox)lstCMSPages.Items[i].FindControl("chkcheck");
                    if (chkCMS.Checked)
                    {
                        HiddenField hdnCMSId = (HiddenField)lstCMSPages.Items[i].FindControl("hdnCMSPageId");
                        if (!string.IsNullOrEmpty(hdnCMSId.Value))
                        {
                            objCMSList.SetStatus(true, Convert.ToInt32(hdnCMSId.Value));
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
                BindListOfCMSPages();
            }
            catch (Exception Ex)
            {
                dvMessage.InnerHtml = "<strong>Error!</strong> " + Ex.Message.Trim();
                dvMessage.Attributes.Add("class", "alert alert-error");
                dvMessage.Visible = true;
            }
        }

        protected void lnkInactive_Click(object sender, EventArgs e)
        {
            try
            {
                bool InActive = false;
                dvMessage.InnerHtml = "";
                dvMessage.Visible = false;
                objCMSList = ServiceFactory.CMSPagesService;
                for (int i = 0; i <= lstCMSPages.Items.Count - 1; i++)
                {
                    HtmlInputCheckBox chkCMS = (HtmlInputCheckBox)lstCMSPages.Items[i].FindControl("chkcheck");

                    if (chkCMS.Checked)
                    {
                        HiddenField hdnCMSId = (HiddenField)lstCMSPages.Items[i].FindControl("hdnCMSPageId");
                        if (!string.IsNullOrEmpty(hdnCMSId.Value))
                        {
                            objCMSList.SetStatus(false, Convert.ToInt32(hdnCMSId.Value));
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
                BindListOfCMSPages();
            }
            catch (Exception Ex)
            {
                dvMessage.InnerHtml = "<strong>Error!</strong> " + Ex.Message.Trim();
                dvMessage.Attributes.Add("class", "alert alert-error");
                dvMessage.Visible = true;
            }
        }

        protected void lnkDelete_Click(object sender, EventArgs e)
        {
            try
            {
                bool Deleted = false;
                dvMessage.InnerHtml = "";
                dvMessage.Visible = false;
                objCMSList = ServiceFactory.CMSPagesService;
                for (int i = 0; i <= lstCMSPages.Items.Count - 1; i++)
                {
                    HtmlInputCheckBox chkCMS = (HtmlInputCheckBox)lstCMSPages.Items[i].FindControl("chkcheck");
                    if (chkCMS.Checked)
                    {
                        HiddenField hdnCMSId = (HiddenField)lstCMSPages.Items[i].FindControl("hdnCMSPageId");
                        if (!string.IsNullOrEmpty(hdnCMSId.Value))
                        {
                            objCMSList.DeleteCMSPages(Convert.ToInt32(hdnCMSId.Value));
                            Deleted = true;
                        }
                    }
                }
                if (Deleted)
                {
                    dvMessage.InnerHtml = "<strong>Record deleted successfully.</strong>";
                    dvMessage.Attributes.Add("class", "alert alert-success");
                    dvMessage.Visible = true;
                }
                BindListOfCMSPages();
            }
            catch (Exception Ex)
            {
                dvMessage.InnerHtml = "<strong>Error!</strong> " + Ex.Message.Trim();
                dvMessage.Attributes.Add("class", "alert alert-error");
                dvMessage.Visible = true;
            }
        }

        protected void lstCMSPages_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                DataRow row = (e.Item.DataItem as DataRowView).Row;

                HtmlInputCheckBox chkcheck = e.Item.FindControl("chkcheck") as HtmlInputCheckBox;
                string[] ids = { "5", "10", "7", "6", "17", "22", "23", "24" };
                if (ids.Contains(row["Id"].ToString()))
                {
                    chkcheck.Visible = false;
                }

            }
        }

        protected void dataPagerCMS_PreRender(object sender, EventArgs e)
        {
            dataPagerCMS.PageSize = Convert.ToInt32(Application["PageSize"].ToString());
            BindListOfCMSPages();
        }

        protected void SortByServiceCase_Click(object sender, EventArgs e)
        {

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
                    ViewState["SortExpression"] = "";
                return (string)ViewState["SortExpression"];
            }
            set { ViewState["SortExpression"] = value; }
        }

        protected string PageTitleExpression
        {
            get
            {
                if (ViewState["PageTitleExpression"] == null)
                    ViewState["PageTitleExpression"] = "";
                return (string)ViewState["PageTitleExpression"];
            }
            set { ViewState["PageTitleExpression"] = value; }
        }

        protected string MenuTitleExpression
        {
            get
            {
                if (ViewState["MenuTitleExpression"] == null)
                    ViewState["MenuTitleExpression"] = "";
                return (string)ViewState["MenuTitleExpression"];
            }
            set { ViewState["MenuTitleExpression"] = value; }
        }

        protected void lstCMSPages_Sorting(object sender, ListViewSortEventArgs e)
        {
            LinkButton lb = lstCMSPages.FindControl(e.SortExpression) as LinkButton;
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

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtMenuTitle.Text))
            {
                MenuTitleExpression = txtMenuTitle.Text;
            }

            if (!string.IsNullOrWhiteSpace(txtPageTitle.Text))
            {
                PageTitleExpression = txtPageTitle.Text;
            }
        }
    }
}
