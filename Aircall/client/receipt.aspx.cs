using Aircall.Common;
using BizObjects;
using Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Aircall.client
{
    public partial class receipt : System.Web.UI.Page
    {
        IClientUnitService objClientUnitService = ServiceFactory.ClientUnitService;
        IClientService objClientService = ServiceFactory.ClientService;
        IPartnerService objPartnerService = ServiceFactory.PartnerService;
        IEmailTemplateService objEmailTemplateService;
        IClientUnitSubscriptionService objClientUnitSubscriptionService;
        IClientPaymentMethodService objClientPaymentMethodService;
        DataTable dtClient = new DataTable();
        decimal total = 0m;
        int ClientId = 0;
        string CustomerProfileId = "";
        string StripeErr = "";
        List<BillingHistory> bhs = new List<BillingHistory>();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["ClientLoginCookie"] == null)
                {
                    Response.Redirect(Application["SiteAddress"] + "sign-in.aspx");
                }
                ClientId = (Session["ClientLoginCookie"] as LoginModel).Id;

                objClientService.GetClientById(ClientId, ref dtClient);
                litName.Text = dtClient.Rows[0]["ClientName"].ToString();
                litEmail.Text = dtClient.Rows[0]["Email"].ToString();
                CustomerProfileId = dtClient.Rows[0]["CustomerProfileId"].ToString();
                //objClientUnitService.GetClientUnitsByClientId(ClientId, ref dtClient);
                objClientUnitService.GetUnPaidClientUnitsByClientId(ClientId, ref dtClient);
                DataTable dt = dtClient.Clone();

                //var rows = dtClient.Select(" PaymentStatus='NotReceived' OR PaymentStatus='Failed' ");
                var rows = dtClient.Rows;
                foreach (DataRow row in rows)
                {
                    dt.Rows.Add(row.ItemArray);
                }

                if (dt.Rows.Count > 0)
                {
                    lstSummary.DataSource = dt;
                    lstSummary.DataBind();
                }
            }
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
                litPlan.Text = row["Name"].ToString();
                var amt1 = 0m;
                if (row["IsSpecialApplied"].ToString().ToLower() == "true")
                {
                    amt1 = decimal.Parse(row["DiscountPrice"].ToString());
                    litPlanType.Text = "Special Offer";
                }
                else
                {
                    amt1 = decimal.Parse(row["PricePerMonth"].ToString());
                    litPlanType.Text = "Recurring";
                }
                var IsSpecial = Convert.ToBoolean(row["IsSpecialApplied"].ToString());
                var StripeCardId = Session["stripeCard"].ToString();
                int amt = int.Parse((amt1 * 100).ToString("0"));
                var desc = "Charge For Plan " + litPlan.Text;
                var StripeResponse = General.StripeCharge(true, "", CustomerProfileId, StripeCardId, amt, desc, litPlan.Text);
                string successImg = "<figure><img src=\"images/right_icon.png\" /></figure>";
                string failImg = "<figure><img src=\"images/fail_icon.png\" /></figure>";
                litAmount.Text = "$ " + amt1.ToString("0.00");
                total = total + amt1;
                if (StripeResponse.ex == null)
                {

                    var rtn = objClientUnitService.UpdateClientUnitPortal(int.Parse(row["Id"].ToString()), StripeResponse.PaymentStatus, StripeCardId, ClientId, General.UserRoles.Client.GetEnumValue(), DateTime.UtcNow);


                    DataTable dtadd = new DataTable();
                    dtadd = Session["billingAdd"] as DataTable;

                    ClientId = (Session["ClientLoginCookie"] as LoginModel).Id;
                    DataTable dtclient1 = new DataTable();
                    objClientService.GetClientById(ClientId, ref dtclient1);

                    float SalesCommission = float.Parse(dtclient1.Rows[0]["SalesCommission"].ToString());

                    //objPartnerService
                    AddClientUnitServiceCount(int.Parse(row["Id"].ToString()), int.Parse(row["PlanTypeId"].ToString()), IsSpecial);
                    BillingHistory bh = new BillingHistory();
                    bh.BillingAddress = dtadd.Rows[0]["Address"].ToString();
                    bh.BillingState = int.Parse(dtadd.Rows[0]["State"].ToString());
                    bh.AddedBy = ClientId;
                    bh.ClientId = ClientId;
                    // bh.AddedDate = DateTime.UtcNow; Code Commented on 19-07-2017
                    bh.AddedDate = DateTime.UtcNow.ToLocalTime();
                    bh.BillingFirstName = dtadd.Rows[0]["FirstName"].ToString();
                    bh.BillingLastName = dtadd.Rows[0]["LastName"].ToString();
                    bh.Company = dtadd.Rows[0]["Company"].ToString();
                    bh.BillingCity = int.Parse(dtadd.Rows[0]["City"].ToString());
                    bh.BillingZipcode = dtadd.Rows[0]["ZipCode"].ToString();
                    //bh.TransactionDate = DateTime.UtcNow; Code Commented on 19-07-2017
                    bh.TransactionDate = DateTime.UtcNow.ToLocalTime();
                    bh.TransactionId = StripeResponse.TransactionId;
                    bh.BillingMobileNumber = dtadd.Rows[0]["PhoneNumber"].ToString();
                    bh.BillingPhoneNumber = dtadd.Rows[0]["MobileNumber"].ToString();
                    bh.PackageName = litPlan.Text;
                    //bh.UnitId = int.Parse(row["Id"].ToString());
                    bh.BillingType = (IsSpecial == false ? General.BillingTypes.Recurringpurchase.GetEnumDescription() : General.BillingTypes.FixedCost.GetEnumDescription());
                    bh.IsSpecialOffer = IsSpecial;
                    bh.OriginalAmount = amt1;
                    bh.PurchasedAmount = amt1;
                    bh.IsPaid = true;
                    bh.failcode = "";
                    bh.faildesc = "Payment Success!";
                    //bh.StripeNextPaymentDate = (IsSpecial == false ? StripeResponse.StripeNextPaymentDate : DateTime.Now);
                    if (SalesCommission > 0)
                    {
                        bh.PartnerSalesCommisionAmount = (bh.PurchasedAmount * (decimal.Parse(SalesCommission.ToString()) / 100));
                    }
                    bhs.Add(bh);

                    litAmount.Text = successImg + litAmount.Text;
                }
                else
                {
                    litAmount.Text = failImg + litAmount.Text;
                    StripeErr = StripeErr + "<li>" + StripeResponse.ex.StripeError.Message + "</li>";
                    btnRetry.Visible = true;
                    //dvMessage.InnerHtml = StripeResponse.ex.StripeError.Message;
                    //dvMessage.Attributes.Add("class", "error");
                    //dvMessage.Visible = true;
                }
            }
        }
        private void AddClientUnitServiceCount(int ClientUnitId, int PlanTypeId, bool IsSpecial)
        {
            LoginModel objLoginModel = new LoginModel();
            objLoginModel = Session["ClientLoginCookie"] as LoginModel;

            BizObjects.ClientUnitServiceCount objUnitServiceCount = new BizObjects.ClientUnitServiceCount();
            IClientUnitServiceCountService objClientUnitServiceCountService = ServiceFactory.ClientUnitServiceCountService;
            objUnitServiceCount.ClientId = objLoginModel.Id;
            objUnitServiceCount.UnitId = ClientUnitId;
            objUnitServiceCount.PlanType = PlanTypeId;
            objUnitServiceCount.TotalDonePlanService = 0;
            objUnitServiceCount.TotalRequestService = 0;
            objUnitServiceCount.TotalDoneRequestService = 0;
            objUnitServiceCount.TotalBillsGenerated = 1;
            objUnitServiceCount.StripeUnitSubscriptionCount = (IsSpecial ? 0 : 1);
            objUnitServiceCount.AddedBy = objLoginModel.Id;
            objUnitServiceCount.AddedByType = objLoginModel.RoleId;
            objUnitServiceCount.AddedDate = DateTime.UtcNow;

            objClientUnitServiceCountService.AddClientUnitServiceCount(ref objUnitServiceCount);
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
            cUnitOrder.ChargeBy = General.PaymentMethod.CC.GetEnumDescription();
            cUnitOrder.AddedBy = ClientId;
            cUnitOrder.AddedByType = (int)General.UserRoles.Client;
            cUnitOrder.AddedDate = DateTime.UtcNow;
            cUnitOrder.IsDeleted = false;
            cUnitOrder.OrderType = "Charge";
            cUnitOrder.OrderAmount = total;
            var orderCount = dtorder.Rows.Count;
            //var orderCount = db.Orders.Where(x => x.ClientId == ClientId && x.IsDeleted == false).Count();
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
                objClientUnitSubscriptionService.AddClientUnitSubscriptionService(ref objClientUnitSubscription, item.IsSpecialOffer);

                //Add Client Unit Subscription End
            }


            objEmailTemplateService = ServiceFactory.EmailTemplateService;
            DataTable dtEmailTemplate = new DataTable();
            objEmailTemplateService.GetByName("UnitOrderClient", ref dtEmailTemplate);
            if (dtEmailTemplate.Rows.Count > 0)
            {
                string EmailBody = dtEmailTemplate.Rows[0]["EmailBody"].ToString();
                var strclient = EmailBody;

                string CCEmail = dtEmailTemplate.Rows[0]["CCEmails"].ToString();


                strclient = strclient.Replace("{{ClientName}}", (Session["ClientLoginCookie"] as LoginModel).FullName);
                strclient = strclient.Replace("{{Address}}", dtadd.Rows[0]["Address"].ToString() + ",<br/>" + dtadd.Rows[0]["CityName"].ToString() + ", " + dtadd.Rows[0]["StateName"].ToString() + ",<br/>" + dtadd.Rows[0]["ZipCode"].ToString());
                strclient = strclient.Replace("{{PurchasedDate}}", cUnitOrder.AddedDate.ToString("MM/dd/yyyy"));
                StringBuilder sb = new StringBuilder();

                sb.Append("<table border='1' style='border-collapse: collapse;'>");
                sb.Append("<tr>");
                sb.Append("<th>");
                sb.Append("Unit Name");
                sb.Append("</th>");
                sb.Append("<th>");
                sb.Append("Unit Location Address");
                sb.Append("</th>");
                sb.Append("<th>");
                sb.Append("Plan");
                sb.Append("</th>");
                sb.Append("<th>");
                sb.Append("Payment Type");
                sb.Append("</th>");
                sb.Append("<th>");
                sb.Append("Rate");
                sb.Append("</th>");

                sb.Append("</tr>");
                foreach (var item in bhs)
                {
                    IClientAddressService objClientAddress = ServiceFactory.ClientAddressService;
                    DataTable dtAddress = new DataTable();
                    DataTable dtUnit = new DataTable();
                    //objClientUnitService.GetClientUnitById(item.UnitId, ref dtUnit);
                    objClientUnitService.GetClientUnitById(0, ref dtUnit);
                    //var clientunit = db.ClientUnits.Where(x => x.Id == item.UnitId).FirstOrDefault();

                    objClientAddress.GetAddressById(int.Parse(dtUnit.Rows[0]["AddressId"].ToString()), ref dtAddress);

                    sb.Append("<tr>");
                    sb.Append("<td>");
                    sb.Append(dtUnit.Rows[0]["UnitName"].ToString());
                    sb.Append("</td>");
                    sb.Append("<td>");
                    sb.Append(dtAddress.Rows[0]["Address"].ToString() + ",<br/>" + dtAddress.Rows[0]["CityName"].ToString() + ", " + dtAddress.Rows[0]["StateName"].ToString() + ",<br/>" + dtAddress.Rows[0]["ZipCode"].ToString());
                    sb.Append("</td>");
                    sb.Append("<td>");
                    sb.Append(dtUnit.Rows[0]["PlanName"].ToString());
                    sb.Append("</td>");
                    sb.Append("<td>");
                    sb.Append((item.IsSpecialOffer ? "Special Offer" : "Recurring"));
                    sb.Append("</td>");
                    sb.Append("<td> $");
                    sb.Append(item.PurchasedAmount);
                    sb.Append("</td>");
                    sb.Append("</tr>");
                }
                sb.Append("<tr>");
                sb.Append("<td colspan='5'>");
                sb.Append("Total: $");
                sb.Append(total.ToString("0.00"));
                sb.Append("</td>");
                sb.Append("</tr>");
                sb.Append("</table>");
                //strclient = strclient.Replace("{{Amount}}", total.ToString("0.00"));
                strclient = strclient.Replace("{{UnitsPurchased}}", sb.ToString());

                string Subject = dtEmailTemplate.Rows[0]["EmailTemplateSubject"].ToString();
                var AdminEmail = General.GetSitesettingsValue("AdminEmail");
                Email.SendEmail(Subject, UserInfo.Rows[0]["Email"].ToString(), CCEmail, "", strclient);

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