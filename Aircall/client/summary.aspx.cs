using Aircall.Common;
using Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Aircall.client
{
    public partial class summary : System.Web.UI.Page
    {
        IClientUnitService objClientUnitService = ServiceFactory.ClientUnitService;
        IClientService objClientService = ServiceFactory.ClientService;
        DataTable dtClient = new DataTable();
        decimal total = 0m;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                int ClientId = 0;
                if (Session["ClientLoginCookie"] == null)
                {
                    Response.Redirect(Application["SiteAddress"] + "sign-in.aspx");
                }
                ClientId = (Session["ClientLoginCookie"] as LoginModel).Id;

                BindSummary(ClientId);
            }
        }

        private void BindSummary(int ClientId)
        {
            objClientService.GetClientById(ClientId, ref dtClient);
            litName.Text = dtClient.Rows[0]["ClientName"].ToString();
            litEmail.Text = dtClient.Rows[0]["Email"].ToString();

            //objClientUnitService.GetClientUnitsByClientId(ClientId, ref dtClient);
            objClientUnitService.GetUnPaidClientUnitsByClientId(ClientId, ref dtClient);
            DataTable dt = dtClient.Clone();

            //if (dtClient.Select(" ValidUnit = 0 ").Length > 0)
            if (false)
            {
                lnkcheckout.Enabled = false;
                //lnkcheckout.HRef = "#";
                dvMessage.InnerText = "Some of your unit plan is InActive / Changed Please remove Unit and Add again.";
                dvMessage.Attributes.Add("class", "error");
                dvMessage.Visible = true;
            }
            else
            {
                lnkcheckout.Enabled = true;
                //lnkcheckout.HRef = "checkout.aspx";
                dvMessage.InnerText = "";
                dvMessage.Attributes.Remove("class");
                dvMessage.Visible = false;
            }

            //var rows = dtClient.Select(" PaymentStatus='NotReceived' OR PaymentStatus='Failed' AND AddedBy=" + ClientId.ToString() + " AND AddedByType=" + General.UserRoles.Client.GetEnumValue().ToString() + "");
            var rows = dtClient.Rows;
            foreach (DataRow row in rows)
            {
                dt.Rows.Add(row.ItemArray);
            }

            if (dt.Rows.Count >= 5)
            {
                lnkAddunit.Visible = false;
            }
            else
            {
                lnkAddunit.Visible = true;
            }

            if (dt.Rows.Count > 0)
            {
                lstSummary.DataSource = dt;
                lstSummary.DataBind();
            }
            else
                Response.Redirect(Application["SiteAddress"] + "client/add-ac-unit.aspx");

        }

        protected void lstSummary_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                Literal litUnitName = e.Item.FindControl("litUnitName") as Literal;
                Literal litPlan = e.Item.FindControl("litPlan") as Literal;
                //Literal litPlanType = e.Item.FindControl("litPlanType") as Literal;
                Literal litDesc = e.Item.FindControl("litDesc") as Literal;
                Literal litAmount = e.Item.FindControl("litAmount") as Literal;
                HtmlTableRow tr = e.Item.FindControl("tr") as HtmlTableRow;

                DataRow row = (e.Item.DataItem as DataRowView).Row;
                //var desc = Regex.Replace(row["ShortDescription"].ToString(), "<.*?>", String.Empty).Replace(System.Environment.NewLine, string.Empty);
                //var desc = row["Description"].ToString();
                litUnitName.Text = row["UnitName"].ToString();
                litPlan.Text = row["PlanTypeName"].ToString();
                //litDesc.Text = desc.Substring(0, (desc.Length > 256 ? 256 : desc.Length));
                var amt = 0m;

                //if (row["ValidUnit"].ToString() == "0")
                //{
                //    tr.Attributes.Add("class", "removeunit");
                //}
                //if (Convert.ToBoolean(row["IsSpecialApplied"].ToString()))
                if (false)
                {
                    litAmount.Text = "$ " + row["DiscountPrice"].ToString();
                    //litPlanType.Text = "Special Offer";
                    amt = decimal.Parse(row["DiscountPrice"].ToString());
                }
                else
                {
                    litAmount.Text = "$ " + row["PricePerMonth"].ToString();
                    //litPlanType.Text = "Recurring";
                    amt = decimal.Parse(row["PricePerMonth"].ToString());
                }
                total = total + amt;
            }
        }

        protected void lstSummary_DataBound(object sender, EventArgs e)
        {
            Literal litTotal = (Literal)lstSummary.FindControl("litTotal");
            if (litTotal != null)
                litTotal.Text = "$ " + total.ToString("0.00");

            //(lstSummary.FindControl("litTotal") as Literal).Text = total.ToString("0.00");
        }

        protected void lstSummary_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            if (e.CommandName.ToString() == "RemoveUnit" && e.CommandArgument.ToString() != "0")
            {
                try
                {
                    if (Session["ClientLoginCookie"] == null)
                    {
                        Response.Redirect(Application["SiteAddress"] + "sign-in.aspx");
                    }

                    IUserNotificationService objUserNotificationService = ServiceFactory.UserNotificationService;

                    int ClientId = (Session["ClientLoginCookie"] as LoginModel).Id;
                    objUserNotificationService.DeleteNotificationByUserIdType(ClientId, General.UserRoles.Client.GetEnumValue(), General.NotificationType.PaymentFailed.GetEnumDescription(),0);
                    int UnitId = Convert.ToInt32(e.CommandArgument.ToString());
                    objClientUnitService = ServiceFactory.ClientUnitService;
                    objClientUnitService.DeleteClientUnit(UnitId);
                    BindSummary(ClientId);
                    return;
                }
                catch (Exception Ex)
                {
                    dvMessage.InnerText = Ex.Message.ToString();
                    dvMessage.Attributes.Add("class", "error");
                    dvMessage.Visible = true;
                }
            }
        }

        protected void lnkAddunit_Click(object sender, EventArgs e)
        {
            Response.Redirect(Application["SiteAddress"] + "client/add-ac-unit.aspx");
        }
    }
}