using Aircall.Common;
using BizObjects;
using Services;
using Stripe;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Aircall
{
    public partial class PaymentDueReminderScheduler : System.Web.UI.Page
    {
        IBillingHistoryService objBillingHistoryService = ServiceFactory.BillingHistoryService;
        IClientUnitSubscriptionService objClientUnitSubscriptionService = ServiceFactory.ClientUnitSubscriptionService;
        IUserNotificationService objUserNotificationService;
        protected void Page_Load(object sender, EventArgs e)
        {

            DateTime today = DateTime.UtcNow.ToLocalTime().Date;
            DataTable dtSubscriptionInvoices = new DataTable();
            objClientUnitSubscriptionService.GetClientPaymentDueSubscriptionForScheduler(ref dtSubscriptionInvoices);
            foreach (DataRow row in dtSubscriptionInvoices.Rows)
            {
                int ClientId = Convert.ToInt32(row["ClientId"].ToString());
                string Message = General.GetNotificationMessage("PaymentDueNotification");
                DateTime PaymentDueDate = DateTime.Parse(row["PaymentDueDate"].ToString());
                Message = Message.Replace("{{DueDate}}", PaymentDueDate.ToString("MMMM") + " " + PaymentDueDate.Day.Ordinal());

                long NotificationId = 0;
                int BadgeCount = 0;
                string MessageType = string.Empty;
                int NotificationType;
                MessageType = General.NotificationType.SubscriptionInvoicePaymentReminder.GetEnumDescription();
                NotificationType = General.NotificationType.SubscriptionInvoicePaymentReminder.GetEnumValue();
                BizObjects.UserNotification objUserNotification = new BizObjects.UserNotification();

                objUserNotificationService = ServiceFactory.UserNotificationService;
                objUserNotification.UserId = ClientId;
                objUserNotification.UserTypeId = General.UserRoles.Client.GetEnumValue();
                objUserNotification.Message = Message;
                objUserNotification.Status = General.NotificationStatus.UnRead.GetEnumDescription();
                objUserNotification.CommonId = 0;
                objUserNotification.MessageType = MessageType;
                objUserNotification.AddedDate = DateTime.UtcNow;

                NotificationId = objUserNotificationService.AddUserNotification(ref objUserNotification);

                DataTable dtBadgeCount = new DataTable();
                dtBadgeCount.Clear();
                objUserNotificationService.GetBadgeCount(ClientId, General.UserRoles.Client.GetEnumValue(), ref dtBadgeCount);
                BadgeCount = dtBadgeCount.Rows.Count;

                Notifications objNotifications = new Notifications { NId = NotificationId, NType = NotificationType };
                List<NotificationModel> notify = new List<NotificationModel>();
                notify.Add(new NotificationModel { Key = "NId", Value = new object[] { objNotifications.NId } });
                notify.Add(new NotificationModel { Key = "NType", Value = new object[] { objNotifications.NType } });

                if (!string.IsNullOrEmpty(row["DeviceType"].ToString()) &&
                    !string.IsNullOrEmpty(row["DeviceToken"].ToString()) &&
                    row["DeviceToken"].ToString().ToLower() != "no device token")
                {
                    if (row["DeviceType"].ToString().ToLower() == "android")
                    {
                        string CustomData = "&data.NId=" + objNotifications.NId + "&data.NType=" + objNotifications.NType;
                        SendNotifications.SendAndroidNotification(row["DeviceToken"].ToString(), Message, CustomData, "client");
                    }
                    else if (row["DeviceType"].ToString().ToLower() == "iphone")
                    {
                        SendNotifications.SendIphoneNotification(BadgeCount, row["DeviceToken"].ToString(), Message, notify, "client");
                    }
                }

                DataTable dtEmailtemplate = new DataTable();
                IEmailTemplateService objEmailTemplateService = ServiceFactory.EmailTemplateService;
                objEmailTemplateService.GetByName("PaymentDueReminderClient", ref dtEmailtemplate);
                if (dtEmailtemplate.Rows.Count > 0)
                {
                    string Emailbody = dtEmailtemplate.Rows[0]["EmailBody"].ToString();
                    string CCEmail = dtEmailtemplate.Rows[0]["CCEmails"].ToString();
                    Emailbody = Emailbody.Replace("{{DueDate}}", PaymentDueDate.ToString("MMMM") + " " + PaymentDueDate.Day.Ordinal());
                    Emailbody = Emailbody.Replace("{{Name}}", row["ClientName"].ToString());
                    string Subject = dtEmailtemplate.Rows[0]["EmailTemplateSubject"].ToString();
                    Email.SendEmail(Subject, row["Email"].ToString(), CCEmail, "", Emailbody);
                }
            }
        }
    }
}