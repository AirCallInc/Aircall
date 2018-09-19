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
    public partial class OtherReceipt : System.Web.UI.Page
    {
        IClientUnitService objClientUnitService = ServiceFactory.ClientUnitService;
        IClientService objClientService = ServiceFactory.ClientService;
        IUserNotificationService objUserNotificationService = ServiceFactory.UserNotificationService;
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
            ClientId = (Session["ClientLoginCookie"] as LoginModel).Id;

            objClientService.GetClientById(ClientId, ref dtClient);
            litName.Text = dtClient.Rows[0]["ClientName"].ToString();
            litEmail.Text = dtClient.Rows[0]["Email"].ToString();
            StripeCustomerId = dtClient.Rows[0]["StripeCustomerId"].ToString();
            objClientUnitService.GetClientUnitsByClientId(ClientId, ref dtClient);
            DataTable dt = new DataTable();

            dt = Session["PaymentDetail"] as DataTable;

            lstSummary.DataSource = dt;
            lstSummary.DataBind();
        }
        protected void lstSummary_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                ClientId = (Session["ClientLoginCookie"] as LoginModel).Id;
                Literal litClientName = e.Item.FindControl("litClientName") as Literal;
                Literal litService = e.Item.FindControl("litService") as Literal;
                Literal litReason = e.Item.FindControl("litReason") as Literal;
                Literal litDesc = e.Item.FindControl("litDesc") as Literal;
                Literal litAmount = e.Item.FindControl("litAmount") as Literal;

                DataRow row = (e.Item.DataItem as DataRowView).Row;
                //var desc = row["Description"].ToString();
                litClientName.Text = row["ClientName"].ToString();
                litService.Text = row["ServiceCaseNumber"].ToString();
                var amt1 = 0m;

                amt1 = decimal.Parse(row["NoShowAmount"].ToString().Replace("$", ""));
                var IsSpecial = true;
                var StripeCardId = Session["stripeCard"].ToString();
                int amt = int.Parse((amt1 * 100).ToString("0"));
                var desc = "Charge For No Show Service: " + litService.Text;
                var StripeResponse = General.StripeCharge(IsSpecial, "", StripeCustomerId, StripeCardId, amt, desc, litService.Text);
                string successImg = "<figure><img src=\"images/right_icon.png\" /></figure>";
                string failImg = "<figure><img src=\"images/fail_icon.png\" /></figure>";
                litAmount.Text = "$" + amt1.ToString("0.00");
                total = total + amt1;
                if (StripeResponse.ex == null)
                {
                    long ServiceId = Convert.ToInt64(row["ServiceId"].ToString());
                    objUserNotificationService.DeleteNotificationByCommonIdType(ServiceId, General.NotificationType.NoShow.GetEnumDescription());
                    objUserNotificationService.DeleteNotificationByCommonIdType(ServiceId, General.NotificationType.LateCancelled.GetEnumDescription());
                    //var rtn = objClientUnitService.UpdateClientUnitPortal(int.Parse(row["Id"].ToString()), StripeResponse.PaymentStatus, ClientId, General.UserRoles.Client.GetEnumValue(), DateTime.UtcNow);
                    DataTable dtadd = new DataTable();
                    dtadd = Session["billingAdd"] as DataTable;

                    ClientId = (Session["ClientLoginCookie"] as LoginModel).Id;

                    BillingHistory bh = new BillingHistory();
                    bh.BillingAddress = dtadd.Rows[0]["Address"].ToString();
                    bh.BillingState = int.Parse(dtadd.Rows[0]["State"].ToString());
                    bh.BillingFirstName = dtadd.Rows[0]["FirstName"].ToString();
                    bh.BillingLastName = dtadd.Rows[0]["LastName"].ToString();
                    bh.Company = dtadd.Rows[0]["Company"].ToString();

                    bh.AddedBy = bh.ClientId = ClientId;
                    // bh.AddedDate = DateTime.UtcNow; Code Commented on 19-07-2017
                    bh.AddedDate = DateTime.UtcNow.ToLocalTime();
                    bh.BillingCity = int.Parse(dtadd.Rows[0]["City"].ToString());
                    bh.BillingZipcode = dtadd.Rows[0]["ZipCode"].ToString();
                    //bh.TransactionDate = DateTime.UtcNow;  Code Commented on 19-07-2017
                     bh.TransactionDate = DateTime.UtcNow.ToLocalTime();

                    bh.ServiceCaseNumber = row["ServiceCaseNumber"].ToString();
                    bh.TransactionId = StripeResponse.TransactionId;
                    bh.BillingMobileNumber = dtadd.Rows[0]["PhoneNumber"].ToString();
                    bh.BillingPhoneNumber = dtadd.Rows[0]["MobileNumber"].ToString();
                    bh.PackageName = "No Show Payment Service: " + litService.Text;
                    bh.BillingType = General.BillingTypes.FixedCost.GetEnumDescription();
                    bh.IsSpecialOffer = false;
                    bh.OriginalAmount = amt1;
                    bh.PurchasedAmount = amt1;
                    bh.IsPaid = true;
                    bh.failcode = "";
                    bh.faildesc = "Payment Success!";

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


            ClientId = (Session["ClientLoginCookie"] as LoginModel).Id;

            objClientService.GetClientById(ClientId, ref dtClient);

            if (StripeErr.Trim() != "")
            {
                dvMessage.InnerHtml = "<ul>" + StripeErr + "</ul>";
                dvMessage.Attributes.Add("class", "error");
                dvMessage.Visible = true;
            }
            else
            {
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
                var ordernumber = dtClient.Rows[0]["AccountNumber"].ToString() + "-" + dtadd.Rows[0]["ZipCode"].ToString() + "-O" + (orderCount + 1).ToString();
                cUnitOrder.OrderNumber = ordernumber;
                OrderService.AddClientOrder(ref cUnitOrder);
                IBillingHistoryService objBillingHistory = ServiceFactory.BillingHistoryService;
                foreach (var item in bhs)
                {
                    BillingHistory bh = item;
                    bh.OrderId = cUnitOrder.Id;
                    
                    objBillingHistory.AddBillingHistory(ref bh);
                }

                DataTable dtService = new DataTable();
                DataTable dtBadgeCount = new DataTable();
                IServicesService objServicesService = ServiceFactory.ServicesService;
                objClientService = ServiceFactory.ClientService;
                DataTable dt = new DataTable();
                dt = Session["PaymentDetail"] as DataTable;
                if (dt.Rows[0]["ServiceType"].ToString() != General.ServiceTypes.LateCancelled.GetEnumDescription())
                {
                    BizObjects.Services objServices = new BizObjects.Services();
                    objServices.Id = int.Parse(dt.Rows[0]["ServiceId"].ToString());
                    objServices.UpdatedBy = ClientId;
                    objServices.UpdatedByType = General.UserRoles.Client.GetEnumValue();
                    objServices.UpdatedDate = DateTime.UtcNow;

                    objServicesService.UpdateServiceOfNoShow(ref objServices);
                }
                else
                {
                    objServicesService.UpdateServiceStatus(int.Parse(dt.Rows[0]["ServiceId"].ToString()), General.ServiceTypes.Cancelled.GetEnumDescription());
                }

                Session["cardDetail"] = null;
                Session["stripeCard"] = null;
            }
        }
    }
}