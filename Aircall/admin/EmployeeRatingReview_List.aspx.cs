using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Services;
using System.Data;
using System.Web.UI.HtmlControls;

namespace Aircall.admin
{
    public partial class EmployeeRatingReview_List : System.Web.UI.Page
    {
        IServicesService objCompleted;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GetAllCompletedStatus();
            }
        }

        private void GetAllCompletedStatus()
        {
            objCompleted = ServiceFactory.ServicesService;
            DataTable dtCompleted = new DataTable();
            string ServiceCaseNo = string.Empty;
            string EmpName = string.Empty;
            string ClientName = string.Empty;
            string StartDate = string.Empty;
            string EndDate = string.Empty;

            if (!string.IsNullOrEmpty(Request.QueryString["ClientName"]))
            {
                ClientName = Request.QueryString["ClientName"].ToString();
                txtClient.Text = ClientName;
            }
            if (!string.IsNullOrEmpty(Request.QueryString["EmpName"]))
            {
                EmpName = Request.QueryString["EmpName"].ToString();
                txtemployee.Text = EmpName;
            }
            if (!string.IsNullOrEmpty(Request.QueryString["StartDate"]))
            {
                StartDate = Request.QueryString["StartDate"].ToString();
                txtStart.Value = StartDate;
            }
            if (!string.IsNullOrEmpty(Request.QueryString["EndDate"]))
            {
                EndDate = Request.QueryString["EndDate"].ToString();
                txtEnd.Value = EndDate;
            }

            objCompleted.GetAllCompletedServices(ServiceCaseNo,ClientName, EmpName, StartDate, EndDate, ListViewSortExpression, ListViewSortDirection.ToString(), ref dtCompleted);
            if (dtCompleted.Rows.Count > 0)
            {
                lstEmpRatings.DataSource = dtCompleted;

            }
            lstEmpRatings.DataBind();
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

        protected void lstEmpRatings_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                DataRow dr = (e.Item.DataItem as DataRowView).Row;
                HtmlContainerControl dvRating = e.Item.FindControl("dvRating") as HtmlContainerControl;
                HtmlContainerControl dvEmpRating = e.Item.FindControl("dvEmpRating") as HtmlContainerControl;
                dvRating.Attributes.Add("data-rate", dr["Ratings"].ToString());
                dvEmpRating.Attributes.Add("data-rate", dr["EmpAverageRating"].ToString());
            }
        }

        protected void dataPagerEmpRatings_PreRender(object sender, EventArgs e)
        {
            dataPagerEmpRatings.PageSize = Convert.ToInt32(Application["PageSize"].ToString());
            GetAllCompletedStatus();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string Param = string.Empty;

            if (!string.IsNullOrEmpty(txtClient.Text.Trim()))
                Param = "?ClientName=" + txtClient.Text.Trim();
            if (!string.IsNullOrEmpty(txtemployee.Text.Trim()))
            {
                if (!string.IsNullOrEmpty(Param))
                    Param = Param + "&EmpName=" + txtemployee.Text.Trim();
                else
                    Param = "?EmpName=" + txtemployee.Text.Trim();
            }

            if (!string.IsNullOrEmpty(txtStart.Value.Trim()))
            {
                if (!string.IsNullOrEmpty(Param))
                    Param = Param + "&StartDate=" + txtStart.Value.Trim();
                else
                    Param = "?StartDate=" + txtStart.Value.Trim();
            }
            if (!string.IsNullOrEmpty(txtEnd.Value.Trim()))
            {
                if (!string.IsNullOrEmpty(Param))
                    Param = Param + "&EndDate=" + txtEnd.Value.Trim();
                else
                    Param = "?EndDate=" + txtEnd.Value.Trim();
            }

            Response.Redirect(Application["SiteAddress"] + "admin/EmployeeRatingReview_List.aspx" + Param);
        }

        protected void lstEmpRatings_Sorting(object sender, ListViewSortEventArgs e)
        {
            LinkButton lb = lstEmpRatings.FindControl(e.SortExpression) as LinkButton;
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

        protected void SortByServiceCase_Click(object sender, EventArgs e)
        {

        }
    }
}