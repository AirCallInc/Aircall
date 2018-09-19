using Aircall.Common;
using Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Aircall.client
{
    public partial class BillingDetails : System.Web.UI.Page
    {
        IBillingHistoryService objBillingHistoryService;
        IOrderService objOrderService;
        IUserNotificationService objUserNotificationService;
        protected void Page_Load(object sender, EventArgs e)
        {
            objBillingHistoryService = ServiceFactory.BillingHistoryService;
            objUserNotificationService = ServiceFactory.UserNotificationService;
            if (Session["ClientLoginCookie"] != null)
            {
                if (!IsPostBack)
                {

                    if (Request.QueryString["bid"] != null)
                    {
                        int RequestId;// = int.Parse(Request.QueryString["rid"].ToString());
                        if (!int.TryParse(Request.QueryString["bid"], out RequestId))
                        {
                            Response.Redirect("billing-history.aspx", false);
                        }
                        int BillingId = int.Parse(Request.QueryString["bid"].ToString());
                        DataTable dt = new DataTable();
                        int ClientId = (Session["ClientLoginCookie"] as LoginModel).Id;
                        objUserNotificationService.UpdateStatusByClientIdNotificationIdMessageType(ClientId, BillingId, General.NotificationType.PartPurchased.GetEnumDescription());
                        objBillingHistoryService.GetBillingHistoryById(BillingId, ref dt);
                        if (dt.Rows.Count > 0)
                        {
                            DataRow row = dt.Rows[0];
                            ltrServiceNo.Text = row["ServiceCaseNumber"].ToString();
                            ltrOrderNo.Text = row["OrderNumber"].ToString();
                            ltrPlan.Text = row["PackageName"].ToString();
                            //ltrUnit.Text = row["UnitName"].ToString();
                            //ltrPaymentMethod.Text = row["PaymentMethod"].ToString();
                            var billingType = row["BillingType"].ToString();
                            if (billingType == "Recurring Purchase")
                            {
                                ltrPaymentMethod.Text = "Card Number";
                                ltrPaymentByNumber.Text = row["CardNumber"].ToString();
                            }
                            else
                            {
                                ltrPaymentMethod.Text = "Check Number";
                                ltrPaymentByNumber.Text = row["CheckNumbers"].ToString();
                            }
                            ltrTransactionId.Text = row["TransactionId"].ToString();
                            if (string.IsNullOrEmpty(ltrTransactionId.Text))
                            {
                                ltrTransactionId.Text = row["InvoiceNumber"].ToString();
                            }
                            ltrDate.Text = DateTime.Parse(row["TransactionDate"].ToString()).ToString("MMMM dd, yyyy");
                            ltrTime.Text = DateTime.Parse(row["TransactionDate"].ToString()).ToString("hh:mm tt");
                            ltrAmount.Text = "$ " + row["PurchasedAmount"].ToString();

                            objOrderService = ServiceFactory.OrderService;
                            DataTable dtOrderItem = new DataTable();

                            int OrderId = int.Parse(row["OrderId"].ToString());

                            objOrderService.GetOrderItemByOrderId(OrderId, ref dtOrderItem);

                            if (dtOrderItem.Rows.Count > 0)
                            {
                                lstParts.DataSource = dtOrderItem;
                                lstParts.DataBind();
                            }
                            if (row["PackageName"].ToString().Contains("Part Order"))
                            {
                                dvPart.Visible = true;
                                dvPart1.Visible = true;
                                dvPartGrid.Visible = true;
                                dvNoShow.Visible = false;
                                dvUnit.Visible = false;
                                dvUnit1.Visible = false;
                            }
                            else if (row["PackageName"].ToString().Contains("No Show"))
                            {
                                dvPart.Visible = false;
                                dvPart1.Visible = false;
                                dvPartGrid.Visible = false;
                                dvNoShow.Visible = true;
                                dvUnit.Visible = false;
                                dvUnit1.Visible = false;
                            }
                            else
                            {
                                dvPart.Visible = false;
                                dvPart1.Visible = false;
                                dvPartGrid.Visible = false;
                                dvNoShow.Visible = false;
                                dvUnit.Visible = true;
                                dvUnit1.Visible = true;
                            }
                        }
                    }
                    else
                    {
                        Response.Redirect("billing-history.aspx");
                    }
                }
            }
            else
                Response.Redirect(Application["SiteAddress"] + "sign-in.aspx", false);
        }
    }
}