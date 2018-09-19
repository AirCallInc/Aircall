using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
//using Stripe;
using Services;
using System.Data;

namespace Aircall.admin
{
    public partial class RenewCancel_List_bk : System.Web.UI.Page
    {
        IClientUnitService objClientUnitService;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["msg"] != null)
                {
                    if (Session["msg"].ToString() == "cancel")
                    {
                        dvMessage.InnerHtml = "<strong>Unit Subscription Successfully Cancelled.</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-success");
                        dvMessage.Visible = true;
                    }
                    if (Session["msg"].ToString() == "renew")
                    {
                        dvMessage.InnerHtml = "<strong>Unit Subscription Successfully Renewed.</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-success");
                        dvMessage.Visible = true;
                    }
                    Session["msg"] = null;
                }
                BindUnitSubscriptions();
            }

            //var Invoice = new StripeInvoice();
            //Invoice.CustomerId = "sub_8stHG073NHA2fp";
            //var InvoiceService = new StripeInvoiceService();
            //var InvoiceList=InvoiceService.List(new StripeInvoiceListOptions() { CustomerId = "cus_8sszJ307F4M4eM" });

        }

        private void BindUnitSubscriptions()
        {
            string ClientName = string.Empty;
            if (!string.IsNullOrEmpty(Request.QueryString["ClientName"]))
            {
                ClientName = Request.QueryString["ClientName"].ToString();
                txtClient.Text = ClientName;
            }

            objClientUnitService = ServiceFactory.ClientUnitService;
            DataTable dtClientUnit = new DataTable();
            objClientUnitService.GetAllCancelRenewPlanUnitsByClientName(ClientName, ref dtClientUnit);
            if (dtClientUnit.Rows.Count > 0)
            {
                lstSubscriptions.DataSource = dtClientUnit;
            }
            lstSubscriptions.DataBind();
        }

        protected void lstSubscriptions_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                DataRow dr = (e.Item.DataItem as DataRowView).Row;
                string StripeCustomerId = dr["StripeCustomerId"].ToString();
                string UnitSubscriptionId = dr["StripeSubscriptionId"].ToString();
                int DurationInMonth = Convert.ToInt32(dr["DurationInMonth"].ToString());
                int Count = Convert.ToInt32(dr["StripeUnitSubscriptionCount"].ToString()); ;

                if (Convert.ToBoolean(dr["IsSpecialApplied"].ToString()))
                {
                    Literal ltrSubscription = (Literal)e.Item.FindControl("ltrSubscription");
                    ltrSubscription.Text = "0";
                }
                else
                {
                    //var InvoiceService = new StripeInvoiceService();
                    //var InvoiceList = InvoiceService.List(new StripeInvoiceListOptions() { CustomerId = StripeCustomerId });
                    //Count = InvoiceList.Where(top => top.SubscriptionId == UnitSubscriptionId && top.Paid == true).Count();
                    Literal ltrSubscription = (Literal)e.Item.FindControl("ltrSubscription");
                    ltrSubscription.Text = (DurationInMonth - Count).ToString();
                }
            }
        }

        protected void dataPagerUnitSubscription_PreRender(object sender, EventArgs e)
        {
            dataPagerUnitSubscription.PageSize = Convert.ToInt32(Application["PageSize"].ToString());
            BindUnitSubscriptions();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string Param = string.Empty;

            if (!string.IsNullOrEmpty(txtClient.Text.Trim()))
                Param = "?ClientName=" + txtClient.Text.Trim();

            Response.Redirect(Application["SiteAddress"] + "admin/RenewCancel_List.aspx" + Param);
        }
    }
}