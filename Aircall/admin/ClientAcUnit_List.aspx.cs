using Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Aircall.admin
{
    public partial class ClientAcUnit_List : System.Web.UI.Page
    {
        IClientUnitService objClientUnitService;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["msg"] != null)
                {
                    if (Session["msg"].ToString() == "edit")
                    {
                        dvMessage.InnerHtml = "<strong>Client unit updated successfully.</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-success");
                        dvMessage.Visible = true;
                    }
                    else if (Session["msg"].ToString() == "add")
                    {
                        dvMessage.InnerHtml = "<strong>Client unit added successfully.</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-success");
                        dvMessage.Visible = true;
                    }
                    Session["msg"] = null;
                }
                BindClientUnits();

                if (Request.QueryString["ShowAddSubscription"] == "Y")
                {
                    this.btnAddSubscription.Visible = true;
                }
                else
                {
                    this.btnAddSubscription.Visible = false;
                }
            }
        }

        private void BindClientUnits()
        {
            objClientUnitService = ServiceFactory.ClientUnitService;
            DataTable dtUnits = new DataTable();
            string ClientName = string.Empty;
            int Status = 0;

            if (!string.IsNullOrEmpty(Request.QueryString["ClientName"]))
            {
                ClientName = Request.QueryString["ClientName"].ToString();
                txtClient.Text = ClientName;
            }

            if (!string.IsNullOrEmpty(Request.QueryString["Status"]))
            {
                Status = Convert.ToInt32(Request.QueryString["Status"].ToString());
                drpStatus.SelectedValue = Status.ToString();
            }

            if (!string.IsNullOrEmpty(Request.QueryString["ClientId"]))
            {
                dvFilter.Visible = false;
                int ClientId = Convert.ToInt32(Request.QueryString["ClientId"].ToString());
                objClientUnitService.GetClientUnitsByClientIdADMIN(ClientId, ListViewSortExpression, ListViewSortDirection.ToString(), 0, ref dtUnits);
                //if (dtUnits.Rows.Count > 0)
                //{
                //    string filter = "PaymentStatus='Received'";
                //    DataView dv = new DataView(dtUnits, filter, "Id", DataViewRowState.CurrentRows);
                //    dtUnits = dv.ToTable();
                //    lstUnits.DataSource = dtUnits;
                //}
                lstUnits.DataSource = dtUnits;
                lstUnits.DataBind();
            }
            else
            {
                objClientUnitService.GetClientUnitsADMIN(ClientName, Status, ListViewSortExpression, ListViewSortDirection.ToString(), ref dtUnits);
                if (dtUnits.Rows.Count > 0)
                {
                    lstUnits.DataSource = dtUnits;
                }
                lstUnits.DataBind();
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string Param = string.Empty;

            if (!string.IsNullOrEmpty(txtClient.Text.Trim()))
                Param = "?ClientName=" + txtClient.Text.Trim();
            if (drpStatus.SelectedValue != "0")
            {
                if (!string.IsNullOrEmpty(Param))
                    Param = Param + "&Status=" + drpStatus.SelectedValue.ToString();
                else
                    Param = "?Status=" + drpStatus.SelectedValue.ToString();
            }

            Response.Redirect(Application["SiteAddress"] + "admin/ClientAcUnit_List.aspx" + Param);
        }

        protected void dataPagerClientUnit_PreRender(object sender, EventArgs e)
        {
            dataPagerClientUnit.PageSize = Convert.ToInt32(Application["PageSize"].ToString());
            BindClientUnits();
        }

        protected void SortByServiceCase_Click(object sender, EventArgs e)
        {

        }

        protected void lstUnits_Sorting(object sender, ListViewSortEventArgs e)
        {
            LinkButton lb = lstUnits.FindControl(e.SortExpression) as LinkButton;
            if (lb != null)
            {
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

        protected void btnAddSubscription_Click(object sender, EventArgs e)
        {
            var ClientId = Request.QueryString["ClientId"];
            Response.Redirect(Application["SiteAddress"] + "admin/ClientUnitSubscription_Edit_ByClient.aspx?ClientId=" + ClientId);
        }
    }
}