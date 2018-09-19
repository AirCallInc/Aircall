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
    public partial class ServiceReport_List : System.Web.UI.Page
    {
        IServiceReportService objServiceReport;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["msg"] == "sent")
                {
                    dvMessage.InnerHtml = "<strong>Report updated and email sent sent to client successfully.</strong>";
                    dvMessage.Attributes.Add("class", "alert alert-success");
                    dvMessage.Visible = true;
                }
                else if (Request.QueryString["msg"] == "edit")
                {
                    dvMessage.InnerHtml = "<strong>Report updated successfully.</strong>";
                    dvMessage.Attributes.Add("class", "alert alert-success");
                    dvMessage.Visible = true;
                }
                GetAllCompletedStatus();
            }
        }

        private void GetAllCompletedStatus()
        {
            objServiceReport = ServiceFactory.ServiceReportService;
            DataTable dtReport = new DataTable();
            string ServiceReportNo = string.Empty;
            string Client = string.Empty;
            string EmpName = string.Empty;
            string StartDate = string.Empty;
            string EndDate = string.Empty;

            if (!string.IsNullOrEmpty(Request.QueryString["Client"]))
            {
                Client = Request.QueryString["Client"].ToString();
                txtClient.Text = Client;
            }
            if (!string.IsNullOrEmpty(Request.QueryString["RNo"]))
            {
                ServiceReportNo = Request.QueryString["RNo"].ToString();
                txtReportNo.Text = ServiceReportNo;
            }
            if (!string.IsNullOrEmpty(Request.QueryString["EmpName"]))
            {
                EmpName = Request.QueryString["EmpName"].ToString();
                txtEmployee.Text = EmpName;
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

            objServiceReport.GetAllServiceReport(ServiceReportNo, Client, EmpName, StartDate, EndDate, ListViewSortExpression, ListViewSortDirection.ToString(), ref dtReport);
            if (dtReport.Rows.Count > 0)
            {
                lstReport.DataSource = dtReport;
            }
            lstReport.DataBind();
        }

        protected SortDirection ListViewSortDirection
        {
            get
            {
                if (ViewState["sortDirection"] == null)
                    ViewState["sortDirection"] = SortDirection.Descending;
                return (SortDirection)ViewState["sortDirection"];
            }
            set { ViewState["sortDirection"] = value; }
        }

        protected string ListViewSortExpression
        {
            get
            {
                if (ViewState["SortExpression"] == null)
                    ViewState["SortExpression"] = "ScheduledDate";
                if (ViewState["SortExpression"] != null)
                    if (ViewState["SortExpression"].ToString() == "")
                        ViewState["SortExpression"] = "ScheduledDate";

                return (string)ViewState["SortExpression"];
            }
            set { ViewState["SortExpression"] = value; }
        }

        protected void dataPagerReport_PreRender(object sender, EventArgs e)
        {
            dataPagerReport.PageSize = Convert.ToInt32(Application["PageSize"].ToString());
            GetAllCompletedStatus();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string Param = string.Empty;

            if (!string.IsNullOrEmpty(txtClient.Text.Trim()))
                Param = "?Client=" + txtClient.Text.Trim();
            if (!string.IsNullOrEmpty(txtEmployee.Text.Trim()))
            {
                if (!string.IsNullOrEmpty(Param))
                    Param = Param + "&EmpName=" + txtEmployee.Text.Trim();
                else
                    Param = "?EmpName=" + txtEmployee.Text.Trim();
            }
            if (!string.IsNullOrEmpty(txtReportNo.Text.Trim()))
            {
                if (!string.IsNullOrEmpty(Param))
                    Param = Param + "&RNo=" + txtReportNo.Text.Trim();
                else
                    Param = "?RNo=" + txtReportNo.Text.Trim();
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

            Response.Redirect(Application["SiteAddress"] + "admin/ServiceReport_List.aspx" + Param);
        }

        protected void lstReport_Sorting(object sender, ListViewSortEventArgs e)
        {
            LinkButton lb = lstReport.FindControl(e.SortExpression) as LinkButton;
            HtmlTableCell th = lb.Parent as HtmlTableCell;
            HtmlTableRow tr = th.Parent as HtmlTableRow;
            List<HtmlTableCell> ths = new List<HtmlTableCell>();
            foreach (HtmlTableCell item in tr.Controls)
            {
                try
                {
                    if (item.ID != null)
                    {
                        if (item.ID.Contains("th"))
                        {
                            item.Attributes["class"] = "sorting";
                        }
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