using Aircall.Common;
using BizObjects;
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
    public partial class PlanRenewReceipt : System.Web.UI.Page
    {
        IClientUnitService objClientUnitService = ServiceFactory.ClientUnitService;
        IClientService objClientService = ServiceFactory.ClientService;
        IPlanService objPlanService;
        IClientUnitSubscriptionService objClientUnitSubscriptionService;
        IClientPaymentMethodService objClientPaymentMethodService;

        DataTable dtClient = new DataTable();
        decimal total = 0m;
        int ClientId = 0;
        string StripeCustomerId = "";
        string StripeErr = "";
        List<BillingHistory> bhs = new List<BillingHistory>();
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
            ClientId = (Session["ClientLoginCookie"] as LoginModel).Id;

            objClientService.GetClientById(ClientId, ref dtClient);
            litName.Text = dtClient.Rows[0]["ClientName"].ToString();
            litEmail.Text = dtClient.Rows[0]["Email"].ToString();
            StripeCustomerId = dtClient.Rows[0]["StripeCustomerId"].ToString();
            objClientUnitService.GetClientUnitsByClientId(ClientId, ref dtClient);
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
                row["StripePlanId"] = dtplan.Rows[0]["StripePlanId"].ToString();
                dt.Rows.Add(row.ItemArray);
            }

            lstSummary.DataSource = dt;
            lstSummary.DataBind();
        }
        protected void lstSummary_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                ClientId = (Session["ClientLoginCookie"] as LoginModel).Id;
                Literal litUnitName = e.Item.FindControl("litUnitName") as Literal;
                Literal litPlan = e.Item.FindControl("litPlan") as Literal;
                Literal litPlanType = e.Item.FindControl("litPlanType") as Literal;
                Literal litDesc = e.Item.FindControl("litDesc") as Literal;
                Literal litAmount = e.Item.FindControl("litAmount") as Literal;

                DataRow row = (e.Item.DataItem as DataRowView).Row;
                string StripePlanId = row["StripePlanId"].ToString();
                //var desc = row["Description"].ToString();
                litUnitName.Text = row["UnitName"].ToString();
                litPlan.Text = row["PlanTypeName"].ToString();
                var amt1 = 0m;

                amt1 = decimal.Parse(row["PricePerMonth"].ToString());
                litPlanType.Text = "Recurring";

                var IsSpecial = false;
                var StripeCardId = Session["stripeCard"].ToString();
                int amt = int.Parse((amt1 * 100).ToString("0"));
                var desc = "Charge For Plan " + litPlan.Text;
                var StripeResponse = General.StripeCharge(true, "", StripeCustomerId, StripeCardId, amt, desc, litPlan.Text);
                string successImg = "<figure><img src=\"images/right_icon.png\" /></figure>";
                string failImg = "<figure><img src=\"images/fail_icon.png\" /></figure>";
                litAmount.Text = "$ " + amt1.ToString("0.00");
                total = total + amt1;
                if (StripeResponse.ex == null)
                {
                    var rtn = objClientUnitService.RenewUnitSubscription(int.Parse(row["Id"].ToString()), ClientId, General.UserRoles.Client.GetEnumValue(), DateTime.UtcNow);
                    //var rtn = objClientUnitService.UpdateClientUnitPortal(int.Parse(row["Id"].ToString()), StripeResponse.PaymentStatus, ClientId, General.UserRoles.Client.GetEnumValue(), DateTime.UtcNow);
                    var rtn1 = objClientUnitService.UpdateClientUnitPortal(rtn, StripeResponse.PaymentStatus, StripeCardId, ClientId, General.UserRoles.Client.GetEnumValue(), DateTime.UtcNow);

                    DataTable dtadd = new DataTable();
                    dtadd = Session["billingAdd"] as DataTable;

                    ClientId = (Session["ClientLoginCookie"] as LoginModel).Id;

                    BillingHistory bh = new BillingHistory();
                    bh.BillingAddress = dtadd.Rows[0]["Address"].ToString();
                    bh.BillingState = int.Parse(dtadd.Rows[0]["State"].ToString());
                    bh.AddedBy = bh.ClientId = ClientId;
                    //bh.AddedDate = DateTime.UtcNow; Code Commented on 19-07-2017

                    bh.AddedDate = DateTime.UtcNow.ToLocalTime();

                    bh.BillingCity = int.Parse(dtadd.Rows[0]["City"].ToString());
                    bh.BillingZipcode = dtadd.Rows[0]["ZipCode"].ToString();
                    // bh.TransactionDate = DateTime.UtcNow; Code Commented on 19-07-2017
                    bh.TransactionDate = DateTime.UtcNow.ToLocalTime();

                    bh.TransactionId = StripeResponse.TransactionId;
                    bh.BillingMobileNumber = dtadd.Rows[0]["PhoneNumber"].ToString();
                    bh.BillingPhoneNumber = dtadd.Rows[0]["MobileNumber"].ToString();
                    bh.PackageName = litPlan.Text;
                    //bh.UnitId = rtn;
                    bh.BillingType = (IsSpecial == false ? General.BillingTypes.Recurringpurchase.GetEnumDescription() : General.BillingTypes.FixedCost.GetEnumDescription());
                    bh.IsSpecialOffer = IsSpecial;
                    bh.OriginalAmount = amt1;
                    bh.PurchasedAmount = amt1;
                    bh.IsPaid = true;
                    bh.failcode = "";
                    bh.faildesc = "Payment Success!";
                    //bh.StripeNextPaymentDate = StripeResponse.StripeNextPaymentDate;
                    bhs.Add(bh);

                    litAmount.Text = successImg + litAmount.Text;
                }
                else
                {
                    litAmount.Text = failImg + litAmount.Text;
                    StripeErr = StripeErr + "<li>" + StripeResponse.ex.StripeError.Message + "</li>";
                    //dvMessage.InnerHtml = StripeResponse.ex.StripeError.Message;
                    //dvMessage.Attributes.Add("class", "error");
                    //dvMessage.Visible = true;
                }
            }
        }

        protected void lstSummary_DataBound(object sender, EventArgs e)
        {
            (lstSummary.FindControl("litTotal") as Literal).Text = "$ " + total.ToString("0.00");
            ClientId = (Session["ClientLoginCookie"] as LoginModel).Id;
            DataTable UserInfo = new DataTable();
            objClientService.GetClientById(ClientId, ref UserInfo);
            DataTable dtadd = new DataTable();
            dtadd = Session["billingAdd"] as DataTable;
            DataTable card = new DataTable();
            card = Session["cardDetail"] as DataTable;

            IOrderService OrderService = ServiceFactory.OrderService;

            DataTable dtorder = new DataTable();
            OrderService.GetOrderByClientId(ClientId, ref dtorder);

            Orders cUnitOrder = new Orders();

            cUnitOrder.CardNumber = card.Rows[0]["CardNumber"].ToString();
            cUnitOrder.NameOnCard = card.Rows[0]["NameOnCard"].ToString();
            cUnitOrder.ExpirationMonth = int.Parse(card.Rows[0]["ExpiryMonth"].ToString());
            cUnitOrder.ExpirationYear = int.Parse(card.Rows[0]["ExpiryYear"].ToString());
            cUnitOrder.CardType = card.Rows[0]["CardType"].ToString();
            cUnitOrder.CCEmail = UserInfo.Rows[0]["Email"].ToString();
            cUnitOrder.ClientId = ClientId;
            //cUnitOrder.BankName = "";
            cUnitOrder.ChequeDate = DateTime.Parse("1900-01-01");
            cUnitOrder.ChequeNo = "";
            cUnitOrder.ChargeBy = "New CC";
            cUnitOrder.AddedBy = ClientId;
            cUnitOrder.AddedByType = (int)General.UserRoles.Client;
            cUnitOrder.AddedDate = DateTime.UtcNow;
            cUnitOrder.IsDeleted = false;
            cUnitOrder.OrderType = "Charge";
            cUnitOrder.OrderAmount = total;
            var orderCount = dtorder.Rows.Count;
            var ordernumber = UserInfo.Rows[0]["AccountNumber"].ToString() + "-" + dtadd.Rows[0]["ZipCode"].ToString() + "-O" + (orderCount + 1).ToString();
            cUnitOrder.OrderNumber = ordernumber;
            OrderService.AddClientOrder(ref cUnitOrder);
            IBillingHistoryService objBillingHistory = ServiceFactory.BillingHistoryService;
            var StripeCardId = Session["stripeCard"].ToString();
            foreach (var item in bhs)
            {
                objClientPaymentMethodService = ServiceFactory.ClientPaymentMethodService;
                DataTable dtCard = new DataTable();
                objClientPaymentMethodService.GetClientPaymentMethodByStripeCardId(StripeCardId, ref dtCard);
                int CardId = 0;
                if (dtCard.Rows.Count > 0)
                    CardId = Convert.ToInt32(dtCard.Rows[0]["Id"].ToString());

                BillingHistory bh = item;
                bh.OrderId = cUnitOrder.Id;
                objBillingHistory.AddBillingHistory(ref bh);

                //Add Client Unit Subscription Start
                BizObjects.ClientUnitSubscription objClientUnitSubscription = new BizObjects.ClientUnitSubscription();
                objClientUnitSubscriptionService = ServiceFactory.ClientUnitSubscriptionService;
                objClientUnitSubscription.ClientId = ClientId;
                //objClientUnitSubscription.ClientUnitIds = bh.UnitId;
                objClientUnitSubscription.ClientUnitIds = "";
                objClientUnitSubscription.OrderId = bh.OrderId;
                objClientUnitSubscription.PaymentMethod = General.PaymentMethod.CC.GetEnumDescription();
                objClientUnitSubscription.CardId = CardId;
                objClientUnitSubscription.PONumber = string.Empty;
                objClientUnitSubscription.CheckNumbers = string.Empty;
                objClientUnitSubscription.FrontImage = string.Empty;
                objClientUnitSubscription.BackImage = string.Empty;
                objClientUnitSubscription.AccountingNotes = string.Empty;
                objClientUnitSubscription.Amount = bh.PurchasedAmount;
                objClientUnitSubscription.AddedBy = bh.ClientId;
                objClientUnitSubscription.AddedByType = General.UserRoles.Client.GetEnumValue();
                // objClientUnitSubscription.AddedDate = DateTime.UtcNow; Code Commented on 19-07-2017
                objClientUnitSubscription.AddedDate = DateTime.UtcNow.ToLocalTime();

                objClientUnitSubscriptionService.AddClientUnitSubscriptionService(ref objClientUnitSubscription, false);

                //Add Client Unit Subscription End
            }

            if (StripeErr.Trim() != "")
            {
                dvMessage.InnerHtml = "<ul>" + StripeErr + "</ul>";
                dvMessage.Attributes.Add("class", "error");
                dvMessage.Visible = true;
            }
        }
    }
}