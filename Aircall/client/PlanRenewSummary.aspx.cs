using Aircall.Common;
using Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Aircall.client
{
    public partial class PlanRenewSummary : System.Web.UI.Page
    {
        IClientUnitService objClientUnitService = ServiceFactory.ClientUnitService;
        IClientService objClientService = ServiceFactory.ClientService;
        IPlanService objPlanService;

        DataTable dtClient = new DataTable();
        decimal total = 0m;
        int ClientId = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["ClientLoginCookie"] == null)
            {
                Response.Redirect(Application["SiteAddress"] + "sign-in.aspx");
            }
            if (Request.QueryString["uid"] == null)
            {
                Response.Redirect(Application["SiteAddress"] + "client/dashboard.aspx");
            }
            int RequestId;// = int.Parse(Request.QueryString["rid"].ToString());                    
            if (!int.TryParse(Request.QueryString["uid"], out RequestId))
            {
                Response.Redirect("dashboard.aspx", false);
            }
            Session["uid"] = Request.QueryString["uid"].ToString();
            ClientId = (Session["ClientLoginCookie"] as LoginModel).Id;

            objClientService.GetClientById(ClientId, ref dtClient);
            litName.Text = dtClient.Rows[0]["ClientName"].ToString();
            litEmail.Text = dtClient.Rows[0]["Email"].ToString();

            objClientUnitService.GetClientUnitsByClientId((Session["ClientLoginCookie"] as LoginModel).Id, ref dtClient);
            DataTable dt = dtClient.Clone();
            int UnitId = int.Parse(Request.QueryString["uid"].ToString());
            var rows = dtClient.Select(" Id=" + UnitId.ToString());
            objPlanService = ServiceFactory.PlanService;
            DataTable dtplan = new DataTable();
            objPlanService.GetPlanByPlanType(int.Parse(rows[0]["PlanTypeId"].ToString()), ref dtplan);
            foreach (DataRow row in rows)
            {
                row["PricePerMonth"] = dtplan.Rows[0]["PricePerMonth"].ToString();
                row["Description"] = dtplan.Rows[0]["Description"].ToString();
                //row["PricePerMonth"] = dtplan.Rows[0]["PricePerMonth"].ToString();
                dt.Rows.Add(row.ItemArray);
            }
            lstSummary.DataSource = dt;
            lstSummary.DataBind();
        }

        protected void lstSummary_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                Literal litUnitName = e.Item.FindControl("litUnitName") as Literal;
                Literal litPlan = e.Item.FindControl("litPlan") as Literal;
                Literal litPlanType = e.Item.FindControl("litPlanType") as Literal;
                Literal litDesc = e.Item.FindControl("litDesc") as Literal;
                Literal litAmount = e.Item.FindControl("litAmount") as Literal;

                DataRow row = (e.Item.DataItem as DataRowView).Row;
                var desc = Regex.Replace(row["Description"].ToString(), "<.*?>", String.Empty).Replace(System.Environment.NewLine, string.Empty);
                //var desc = row["Description"].ToString();
                litUnitName.Text = row["UnitName"].ToString();
                litPlan.Text = row["PlanTypeName"].ToString();
                litDesc.Text = desc.Substring(0, (desc.Length > 100 ? 100 : desc.Length));

                var amt = 0m;
                amt = decimal.Parse(row["PricePerMonth"].ToString());
                litAmount.Text = "$ " + amt;
                litPlanType.Text = "Recurring";

                total = total + amt;
            }
        }

        protected void lstSummary_DataBound(object sender, EventArgs e)
        {
            (lstSummary.FindControl("litTotal") as Literal).Text = "$ " + total.ToString("0.00");
        }
    }
}