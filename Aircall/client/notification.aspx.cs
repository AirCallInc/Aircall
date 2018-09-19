using Aircall.Common;
using Services;
using System;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;

namespace Aircall.client
{
    public partial class notification : System.Web.UI.Page
    {
        IUserNotificationService objUserNotificationService = ServiceFactory.UserNotificationService;
        DataTable dtuserNotification = new DataTable();
        int UserId = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["ClientLoginCookie"] != null)
            {
                if (!IsPostBack)
                {
                    UserId = (Session["ClientLoginCookie"] as LoginModel).Id;
                    BindNotificationData(UserId);
                    //objUserNotificationService.UpdateNotificationStatusByClientId(UserId);
                }
            }
            else
                Response.Redirect(Application["SiteAddress"] + "sign-in.aspx", false);
        }

        protected void dataPagerRequest_PreRender(object sender, EventArgs e)
        {
            it.PageSize = int.Parse(General.GetSitesettingsValue("ClientPortalPageSize"));
            UserId = (Session["ClientLoginCookie"] as LoginModel).Id;
            BindNotificationData(UserId);
        }

        private void BindNotificationData(int ClientId)
        {
            objUserNotificationService.GetAllNotificationByUserId(UserId, ref dtuserNotification);
            if (dtuserNotification.Rows.Count > 0)
            {
                lstNotification.DataSource = dtuserNotification;
            }
            lstNotification.DataBind();
        }

        protected void lstNotification_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {



                Literal ltDisplayDate = e.Item.FindControl("ltDisplayDate") as Literal;
                Literal ltMessage = e.Item.FindControl("ltMessage") as Literal;
                Image imgPersonSRC = e.Item.FindControl("imgPersonSRC") as Image;
                LinkButton lnkMsg = e.Item.FindControl("lnkMsg") as LinkButton;

                HiddenField hfCommonId = e.Item.FindControl("hfCommonId") as HiddenField;

                DataRow row = (e.Item.DataItem as DataRowView).Row;
                if (DateTime.UtcNow.Date == DateTime.Parse(row["AddedDate"].ToString()).Date)
                {
                    ltDisplayDate.Text = "Today " + DateTime.Parse(row["AddedDate"].ToString()).ToLocalTime().ToString("hh:mm tt");
                }
                ltDisplayDate.Text = DateTime.Parse(row["AddedDate"].ToString()).ToLocalTime().ToString("MMMM dd, yyyy hh:mm tt");
                //if (row["MessageType"].ToString() == Common.General.NotificationType.ServiceApproval.ToString().ToUpper())
                //{
                if (row["Status"].ToString() == "UnRead")
                {
                    lnkMsg.Font.Bold = true;
                }
                else
                {
                    lnkMsg.Font.Bold = false;
                }
                lnkMsg.Text = row["Message"].ToString();
                lnkMsg.CommandName = row["MessageType"].ToString();
                lnkMsg.CommandArgument = row["Id"].ToString();
                if (string.IsNullOrEmpty(row["CommonId"].ToString()))
                {
                    lnkMsg.Visible = false;
                    ltMessage.Text = row["Message"].ToString();
                }
                if (lnkMsg.CommandName == General.NotificationType.PaymentFailed.GetEnumDescription())
                {
                    lnkMsg.Visible = true;
                    ltMessage.Text = "";
                }
                if (lnkMsg.CommandName == General.NotificationType.PartPurchased.GetEnumDescription() || lnkMsg.CommandName == General.NotificationType.FriendlyReminder.GetEnumDescription() || lnkMsg.CommandName == General.NotificationType.NoShow.GetEnumDescription())
                {
                    imgPersonSRC.Visible = false;
                }

                if (string.IsNullOrEmpty(row["EmplyeePIC"].ToString()))
                {
                    imgPersonSRC.ImageUrl = Application["SiteAddress"] + "uploads/profile/employee/place-holder-img.png";
                }
                else
                {
                    imgPersonSRC.ImageUrl = ConfigurationManager.AppSettings["EMPProfileImageURL"].ToString() + row["EmplyeePIC"].ToString();
                }
                General.NotificationType MessageType = Enum.GetValues(typeof(General.NotificationType)).Cast<General.NotificationType>()
                                                               .FirstOrDefault(v => v.GetEnumDescription() == lnkMsg.CommandName);

                switch (MessageType)
                {
                    case General.NotificationType.FriendlyReminder:
                    case General.NotificationType.AdminNotification:
                    case General.NotificationType.PlanRenewed:
                    case General.NotificationType.PaymentFailed:
                    case General.NotificationType.UnitPlanRenew:
                    case General.NotificationType.UnitPlanCancelled:
                    case General.NotificationType.UnitPaymentFailed:
                    case General.NotificationType.SalesPersonVisit:
                    case General.NotificationType.SubscriptionInvoicePaymentReminder:
                    case General.NotificationType.PastDueReminder:
                        objUserNotificationService.UpdateStatusById(long.Parse(row["Id"].ToString()));
                        break;
                    default:
                        break;
                }                
            }
        }

        protected void lstNotification_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            General.NotificationType MessageType = Enum.GetValues(typeof(General.NotificationType)).Cast<General.NotificationType>()
                                                               .FirstOrDefault(v => v.GetEnumDescription() == e.CommandName);

            HiddenField hfCommonId = (HiddenField)e.Item.FindControl("hfCommonId");
            long ServiceId = 0;
            if (!string.IsNullOrEmpty(hfCommonId.Value))
                ServiceId = Convert.ToInt64((e.Item.FindControl("hfCommonId") as HiddenField).Value);

            switch (MessageType)
            {
                case General.NotificationType.ServiceApproval:

                    Response.Redirect(Application["SiteAddress"] + "client/ServiceApprove.aspx?Id=" + ServiceId.ToString() + "&nid=" + e.CommandArgument.ToString(), false);
                    break;
                case General.NotificationType.FriendlyReminder:
                    break;
                case General.NotificationType.LateCancelled:
                case General.NotificationType.NoShow:
                    Session["ServiceId"] = ServiceId;
                    Session["NotificationId"] = e.CommandArgument.ToString();
                    Response.Redirect(Application["SiteAddress"] + "client/NoShowDetail.aspx?nid=" + e.CommandArgument.ToString(), false);
                    break;
                case General.NotificationType.PartPurchased:
                    Session["ServiceId"] = ServiceId;
                    Session["NotificationId"] = e.CommandArgument.ToString();
                    Response.Redirect(Application["SiteAddress"] + "client/BillingDetails.aspx?bid=" + ServiceId.ToString(), false);
                    break;
                case General.NotificationType.SubscriptionInvoicePaymentFailed:
                    Session["ServiceId"] = ServiceId;
                    Session["NotificationId"] = e.CommandArgument.ToString();
                    Response.Redirect(Application["SiteAddress"] + "client/BillingDetails.aspx?bid=" + ServiceId.ToString(), false);
                    break;
                case General.NotificationType.RateService:
                    Session["ServiceId"] = ServiceId;
                    Session["NotificationId"] = e.CommandArgument.ToString();
                    Response.Redirect(Application["SiteAddress"] + "client/ServiceDetails.aspx?sid=" + ServiceId.ToString(), false);
                    break;
                case General.NotificationType.CreditCardExpiration:
                    Session["NotificationId"] = e.CommandArgument.ToString();
                    objUserNotificationService.UpdateStatusByClientIdNotificationIdMessageType((Session["ClientLoginCookie"] as LoginModel).Id, ServiceId, General.NotificationType.CreditCardExpiration.GetEnumDescription());
                    Response.Redirect(Application["SiteAddress"] + "client/update-payment-method.aspx?cid=" + ServiceId.ToString(), false);
                    break;
                case General.NotificationType.AdminNotification:
                    break;
                case General.NotificationType.PlanExpiration:
                    objUserNotificationService.UpdateStatusByClientIdNotificationIdMessageType((Session["ClientLoginCookie"] as LoginModel).Id, ServiceId, General.NotificationType.PlanExpiration.GetEnumDescription());
                    Session["NotificationId"] = e.CommandArgument.ToString();
                    Response.Redirect(Application["SiteAddress"] + "client/PlanRenewSummary.aspx?uid=" + ServiceId.ToString(), false);
                    break;
                case General.NotificationType.PlanRenewed:
                    break;
                case General.NotificationType.ServiceScheduled:
                    Session["ServiceId"] = ServiceId;
                    Session["NotificationId"] = e.CommandArgument.ToString();
                    objUserNotificationService.UpdateStatusByClientIdNotificationIdMessageType((Session["ClientLoginCookie"] as LoginModel).Id, ServiceId, General.NotificationType.ServiceScheduled.GetEnumDescription());
                    Response.Redirect(Application["SiteAddress"] + "client/ServiceScheduleDetail.aspx?Id=" + ServiceId.ToString(), false);
                    break;
                case General.NotificationType.PaymentFailed:
                    Response.Redirect(Application["SiteAddress"] + "client/summary.aspx?clientId");
                    break;
                case General.NotificationType.UnitPlanRenew:
                    break;
                case General.NotificationType.UnitPlanCancelled:
                    break;
                case General.NotificationType.UnitPaymentFailed:
                    break;
                case General.NotificationType.SalesPersonVisit:
                    break;
                case General.NotificationType.PeriodicServiceReminder:
                    Session["ServiceId"] = ServiceId;
                    Session["NotificationId"] = e.CommandArgument.ToString();
                    Response.Redirect(Application["SiteAddress"] + "client/ServiceScheduleDetail.aspx?Id=" + ServiceId.ToString(), false);
                    break;
                default:
                    break;
            }
        }
    }
}