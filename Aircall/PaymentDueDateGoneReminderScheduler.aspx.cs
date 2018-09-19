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
    public partial class PaymentDueDateGoneReminderScheduler : System.Web.UI.Page
    {
        IBillingHistoryService objBillingHistoryService = ServiceFactory.BillingHistoryService;
        IClientUnitSubscriptionService objClientUnitSubscriptionService = ServiceFactory.ClientUnitSubscriptionService;
        protected void Page_Load(object sender, EventArgs e)
        {
            DataTable dtSubscriptionInvoices = new DataTable();
            objClientUnitSubscriptionService.GetPastDueRemindersForScheduler(ref dtSubscriptionInvoices);
            foreach (DataRow row in dtSubscriptionInvoices.Rows)
            {
                string Name, InvoiceNumber, UnitName, PlanName;
                decimal PaymentAmount = 0m;

                long NotificationId = 0;
                int BadgeCount = 0;
                string message = string.Empty;
                int NotificationType;
                int Reminder = int.Parse(row["Reminder"].ToString());
                BizObjects.UserNotification objUserNotification = new BizObjects.UserNotification();
                IUserNotificationService objUserNotificationService;
                DataTable dtEmailtemplate = new DataTable();
                IEmailTemplateService objEmailTemplateService = ServiceFactory.EmailTemplateService;
                bool sendEmail = true;
                switch (Reminder)
                {
                    case 1:
                        message = General.GetNotificationMessage("PastDue1stReminderClient");
                        objEmailTemplateService.GetByName("PastDue1stReminderClient", ref dtEmailtemplate);
                        if (row["PaymentMethod"].ToString() != General.PaymentMethod.CC.GetEnumDescription())
                        {
                            sendEmail = false;
                        }
                        break;
                    case 2:
                        message = General.GetNotificationMessage("PastDue2ndReminderClient");
                        objEmailTemplateService.GetByName("PastDue2ndReminderClient", ref dtEmailtemplate);
                        break;
                    case 3:
                        message = General.GetNotificationMessage("PastDue3rdReminderClient");
                        objEmailTemplateService.GetByName("PastDue3rdReminderClient", ref dtEmailtemplate);
                        break;
                    default:
                        break;
                }
                if (sendEmail)
                {
                    string Emailbody = dtEmailtemplate.Rows[0]["EmailBody"].ToString();
                    message = message.Replace("{{Name}}", row["ClientName"].ToString());
                    message = message.Replace("{{PaymentAmount}}", "$ " + row["PurchasedAmount"].ToString());
                    message = message.Replace("{{InvoiceNumber}}", row["InvoiceNumber"].ToString());
                    message = message.Replace("{{UnitName}}", row["UnitName"].ToString());
                    message = message.Replace("{{PlanName}}", row["PlanName"].ToString());

                    Emailbody = Emailbody.Replace("{{Name}}", row["ClientName"].ToString());
                    Emailbody = Emailbody.Replace("{{PaymentAmount}}", "$ " + row["PurchasedAmount"].ToString());
                    Emailbody = Emailbody.Replace("{{InvoiceNumber}}", row["InvoiceNumber"].ToString());
                    Emailbody = Emailbody.Replace("{{UnitName}}", row["UnitName"].ToString());
                    Emailbody = Emailbody.Replace("{{PlanName}}", row["PlanName"].ToString());


                    objUserNotificationService = ServiceFactory.UserNotificationService;
                    objUserNotification.UserId = int.Parse(row["ClientId"].ToString());
                    objUserNotification.UserTypeId = General.UserRoles.Client.GetEnumValue();
                    objUserNotification.Message = message;
                    objUserNotification.Status = General.NotificationStatus.UnRead.GetEnumDescription();

                    NotificationType = General.NotificationType.PastDueReminder.GetEnumValue();
                    objUserNotification.MessageType = General.NotificationType.PastDueReminder.GetEnumDescription();
                    objUserNotification.CommonId = 0;

                    objUserNotification.AddedDate = DateTime.UtcNow;
                    NotificationId = objUserNotificationService.AddUserNotification(ref objUserNotification);

                    DataTable dtBadgeCount = new DataTable();
                    dtBadgeCount.Clear();

                    objUserNotificationService.GetBadgeCount(int.Parse(row["ClientId"].ToString()), General.UserRoles.Client.GetEnumValue(), ref dtBadgeCount);
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
                            string CustomData = "&data.NId=" + objNotifications.NId + "&data.NType=" + objNotifications.NType + "&data.CommonId=" + objNotifications.CommonId;
                            SendNotifications.SendAndroidNotification(row["DeviceToken"].ToString(), message, CustomData, "client");
                        }
                        else if (row["DeviceType"].ToString().ToLower() == "iphone")
                        {
                            SendNotifications.SendIphoneNotification(BadgeCount, row["DeviceToken"].ToString(), message, notify, "client");
                        }
                    }
                    if (dtEmailtemplate.Rows.Count > 0)
                    {
                        string CCEmail = dtEmailtemplate.Rows[0]["CCEmails"].ToString();
                        string Subject = dtEmailtemplate.Rows[0]["EmailTemplateSubject"].ToString();
                        Email.SendEmail(Subject, row["Email"].ToString(), CCEmail, "", Emailbody);
                    } 
                }
            }
        }
    }
}