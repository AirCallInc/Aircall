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
    public partial class BillingHistory_List : System.Web.UI.Page
    {
        IBillingHistoryService objBillingHistory;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GetAllBillingHistory();

                if (Request.QueryString["msg"] == "success")
                {
                    var msg = "Pay successfully.";
                    DisplaySuccessMessage(msg);
                }
            }
        }

        private void DisplaySuccessMessage(string msg)
        {
            dvMessage.InnerHtml = "<strong>" + msg + "</strong>";
            dvMessage.Attributes.Add("class", "alert alert-success");
            dvMessage.Visible = true;
        }

        private void GetAllBillingHistory()
        {
            objBillingHistory = ServiceFactory.BillingHistoryService;
            DataTable dtBills = new DataTable();
            string ClientName = string.Empty;
            string StartDate = string.Empty;
            string EndDate = string.Empty;
            string Status = string.Empty;

            if (!string.IsNullOrEmpty(Request.QueryString["ClientName"]))
            {
                ClientName = Request.QueryString["ClientName"].ToString();
                txtClient.Text = ClientName;
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
            if (!string.IsNullOrEmpty(Request.QueryString["Status"]))
            {
                Status = Request.QueryString["Status"].ToString();
                drpStatus.SelectedValue = Status;
            }
            else
            {
                Status = "All";
                drpStatus.SelectedValue = Status;
            }
            objBillingHistory.GetAllBillingHistory(ClientName, StartDate, EndDate, Status, ref dtBills);
            if (dtBills.Rows.Count > 0)
            {
                lstBilling.DataSource = dtBills;
            }
            lstBilling.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string Param = string.Empty;

            if (!string.IsNullOrEmpty(txtClient.Text.Trim()))
                Param = "?ClientName=" + txtClient.Text.Trim();
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
            if (!string.IsNullOrEmpty(drpStatus.SelectedValue))
            {
                if (!string.IsNullOrEmpty(Param))
                    Param = Param + "&Status=" + drpStatus.SelectedValue;
                else
                    Param = "?Status=" + drpStatus.SelectedValue;
            }
            Response.Redirect(Application["SiteAddress"] + "admin/BillingHistory_List.aspx" + Param);

        }

        protected void dataPagerBilling_PreRender(object sender, EventArgs e)
        {
            dataPagerBilling.PageSize = Convert.ToInt32(Application["PageSize"].ToString());
            GetAllBillingHistory();
        }

        protected void lstBilling_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                HtmlTableRow row = new HtmlTableRow();
                row = e.Item.FindControl("trTd") as HtmlTableRow;
                DataRow row1 = (e.Item.DataItem as DataRowView).Row;
                Button btnPay = e.Item.FindControl("btnPay") as Button;
                if (row1["IsPaid"].ToString() == "True")
                {
                    btnPay.Visible = false;
                }
                else
                {
                    row.Attributes["class"] = row.Attributes["class"] + " failed-billing";
                }
            }
        }

        protected void lstBilling_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            if (e.CommandName == "PayInvoice")
            {
                string URL = Application["SiteAddress"] + "admin/Repay.aspx?bid=" + e.CommandArgument.ToString();
                Response.Redirect(URL);
            }
        }
    }
}