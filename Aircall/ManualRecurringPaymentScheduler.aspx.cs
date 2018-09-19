using Aircall.Common;
using Services;
using System;
using System.Collections.Generic;
using System.Data;

namespace Aircall
{
    public partial class ManualRecurringPaymentScheduler : System.Web.UI.Page
    {
        IClientUnitService objClientUnitService;
        IBillingHistoryService objBillingHistoryService = ServiceFactory.BillingHistoryService;
        IClientUnitSubscriptionService objClientUnitSubscriptionService = ServiceFactory.ClientUnitSubscriptionService;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                objClientUnitService = ServiceFactory.ClientUnitService;

            DateTime today = DateTime.UtcNow.ToLocalTime().Date;
            DataTable dtSubscriptionInvoices = new DataTable();
            objClientUnitSubscriptionService.GetClientUnitSubscriptionForScheduler(today, General.PaymentMethod.CC.GetEnumDescription(), ref dtSubscriptionInvoices);
            foreach (DataRow row in dtSubscriptionInvoices.Rows)
            {
                var desc = "Payment Received For Invoice No: " + row["InvoiceNumber"].ToString() + " of Plan " + row["PlanName"].ToString();
                var StripeResponse = General.StripeCharge(true, "", row["StripeCustomerId"].ToString(), row["StripeCardId"].ToString(), Convert.ToInt32(Convert.ToDecimal(row["PurchasedAmount"].ToString()) * 100), desc, row["PlanName"].ToString());
                if (StripeResponse.ex == null)
                {
                    BizObjects.ClientUnitSubscription objClientUnitSubscription = new BizObjects.ClientUnitSubscription();
                    objClientUnitSubscriptionService = ServiceFactory.ClientUnitSubscriptionService;
                    objClientUnitSubscription.Id = Convert.ToInt32(row["Id"].ToString());
                    objClientUnitSubscription.CardId = Convert.ToInt32(row["CardId"].ToString());
                    objClientUnitSubscription.PONumber = string.Empty;
                    objClientUnitSubscription.CheckNumbers = string.Empty;
                    objClientUnitSubscription.FrontImage = string.Empty;
                    objClientUnitSubscription.BackImage = string.Empty;
                    objClientUnitSubscription.AccountingNotes = string.Empty;
                    objClientUnitSubscription.Amount = decimal.Parse(row["PurchasedAmount"].ToString());
                    objClientUnitSubscription.Status = General.UnitSubscriptionStatus.Paid.GetEnumDescription();
                    objClientUnitSubscription.AddedBy = 1;
                    objClientUnitSubscription.AddedByType = 1;
                    objClientUnitSubscription.AddedDate = DateTime.UtcNow;
                    var BillingId = objClientUnitSubscriptionService.UpdateClientUnitSubscriptionService(ref objClientUnitSubscription, "", "");

                    //pp
                    DataTable dtUnit = new DataTable();
                    objClientUnitService.GetClientUnitById(int.Parse(row["UnitId"].ToString()), ref dtUnit);
                    if (dtUnit.Rows.Count > 0)
                    {
                        string StripeCardId = dtUnit.Rows[0]["StripeCardId"].ToString();
                        int AddressId = Convert.ToInt32(dtUnit.Rows[0]["AddressId"].ToString());

                        BizObjects.Orders objOrders = new BizObjects.Orders();
                        IOrderService objOrderService = ServiceFactory.OrderService;
                        int OrderId = 0;
                        objOrders.OrderType = "Charge";
                        objOrders.ClientId = int.Parse(row["ClientId"].ToString());
                        objOrders.OrderAmount = decimal.Parse(row["PurchasedAmount"].ToString()); ;
                        objOrders.ChargeBy = General.PaymentMethod.CC.GetEnumDescription();
                        objOrders.AddedBy = 1;
                        objOrders.AddedByType = 1;
                        objOrders.AddedDate = DateTime.UtcNow;
                        OrderId = objOrderService.AddClientUnitOrderForSchedular(ref objOrders, StripeCardId, AddressId);
                    }



                    long NotificationId = 0;
                    int BadgeCount = 0;
                    string message = string.Empty;
                    int NotificationType;

                    BizObjects.UserNotification objUserNotification = new BizObjects.UserNotification();
                    IUserNotificationService objUserNotificationService;

                    message = General.GetNotificationMessage("PaymentSuccessForSubscriptionInvoice");

                    message = message.Replace("{{UnitName}}", row["UnitName"].ToString());
                    message = message.Replace("{{MonthYear}}", today.ToString("MMMM yyyy"));

                    objUserNotificationService = ServiceFactory.UserNotificationService;
                    objUserNotification.UserId = int.Parse(row["ClientId"].ToString());
                    objUserNotification.UserTypeId = General.UserRoles.Client.GetEnumValue();
                    objUserNotification.Message = message;
                    objUserNotification.Status = General.NotificationStatus.UnRead.GetEnumDescription();


                    NotificationType = General.NotificationType.SubscriptionInvoicePaymentFailed.GetEnumValue();
                    objUserNotification.MessageType = General.NotificationType.SubscriptionInvoicePaymentFailed.GetEnumDescription();
                    objUserNotification.CommonId = BillingId;

                    objUserNotification.AddedDate = DateTime.UtcNow;
                    NotificationId = objUserNotificationService.AddUserNotification(ref objUserNotification);

                    DataTable dtBadgeCount = new DataTable();
                    dtBadgeCount.Clear();

                    objUserNotificationService.GetBadgeCount(int.Parse(row["ClientId"].ToString()), General.UserRoles.Client.GetEnumValue(), ref dtBadgeCount);
                    BadgeCount = dtBadgeCount.Rows.Count;

                    Notifications objNotifications = new Notifications { NId = NotificationId, NType = NotificationType, CommonId = objUserNotification.CommonId };
                    List<NotificationModel> notify = new List<NotificationModel>();
                    notify.Add(new NotificationModel { Key = "NId", Value = new object[] { objNotifications.NId } });
                    notify.Add(new NotificationModel { Key = "NType", Value = new object[] { objNotifications.NType } });
                    notify.Add(new NotificationModel { Key = "CommonId", Value = new object[] { objNotifications.CommonId } });

                    if (!string.IsNullOrEmpty(row["DeviceType"].ToString()) && !string.IsNullOrEmpty(row["DeviceToken"].ToString()) &&
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
                }
                else
                {
                    BizObjects.StripeErrorLog objStripeErrorLog = new BizObjects.StripeErrorLog();
                    var stex = StripeResponse.ex;
                    IStripeErrorLogService objStripeErrorLogService = ServiceFactory.StripeErrorLogService;
                    objStripeErrorLog.ChargeId = stex.StripeError.ChargeId;
                    objStripeErrorLog.Code = stex.StripeError.Code;
                    objStripeErrorLog.DeclineCode = stex.StripeError.DeclineCode;
                    objStripeErrorLog.ErrorType = stex.StripeError.ErrorType;
                    objStripeErrorLog.Error = stex.StripeError.Error;
                    objStripeErrorLog.ErrorSubscription = stex.StripeError.ErrorSubscription;
                    objStripeErrorLog.Message = stex.StripeError.Message;
                    objStripeErrorLog.Parameter = stex.StripeError.Parameter;
                    objStripeErrorLog.Userid = int.Parse(row["ClientId"].ToString());
                    objStripeErrorLog.DateAdded = DateTime.UtcNow;

                    objStripeErrorLogService.AddStripeErrorLog(ref objStripeErrorLog);

                    BizObjects.ClientUnitSubscription objClientUnitSubscription = new BizObjects.ClientUnitSubscription();
                    objClientUnitSubscriptionService = ServiceFactory.ClientUnitSubscriptionService;
                    objClientUnitSubscription.Id = Convert.ToInt32(row["Id"].ToString());
                    objClientUnitSubscription.CardId = Convert.ToInt32(row["CardId"].ToString());

                    objClientUnitSubscription.PONumber = string.Empty;

                    objClientUnitSubscription.CheckNumbers = string.Empty;

                    objClientUnitSubscription.FrontImage = string.Empty;
                    objClientUnitSubscription.BackImage = string.Empty;
                    objClientUnitSubscription.AccountingNotes = StripeResponse.ex.StripeError.Message;
                    objClientUnitSubscription.Amount = decimal.Parse(row["PurchasedAmount"].ToString());
                    objClientUnitSubscription.Status = General.UnitSubscriptionStatus.Fail.GetEnumDescription();
                    objClientUnitSubscription.AddedBy = 1;
                    objClientUnitSubscription.AddedByType = 1;
                    objClientUnitSubscription.AddedDate = DateTime.UtcNow;
                    //Code Commented on 03-07-2017
                    //var BillingId = objClientUnitSubscriptionService.UpdateClientUnitSubscriptionService(ref objClientUnitSubscription, StripeResponse.ex.StripeError.Code, StripeResponse.ex.StripeError.Error);

                    var BillingId = objClientUnitSubscriptionService.UpdateClientUnitSubscriptionService(ref objClientUnitSubscription, StripeResponse.ex.StripeError.Code, "Payment Failed!");

                    long NotificationId = 0;
                    int BadgeCount = 0;
                    string message = string.Empty;
                    int NotificationType;


                    BizObjects.UserNotification objUserNotification = new BizObjects.UserNotification();
                    IUserNotificationService objUserNotificationService;

                    message = General.GetNotificationMessage("PaymentFailedForSubscriptionInvoice");

                    message = message.Replace("{{UnitName}}", row["UnitName"].ToString());
                    message = message.Replace("{{MonthYear}}", today.ToString("MMMM yyyy"));
                    message = message.Replace("{{Reason}}", StripeResponse.ex.StripeError.Error);

                    objUserNotificationService = ServiceFactory.UserNotificationService;
                    objUserNotification.UserId = int.Parse(row["ClientId"].ToString());
                    objUserNotification.UserTypeId = General.UserRoles.Client.GetEnumValue();
                    objUserNotification.Message = message;
                    objUserNotification.Status = General.NotificationStatus.UnRead.GetEnumDescription();

                    if (StripeResponse.ex.StripeError.Code != "expired_card")
                    {
                        NotificationType = General.NotificationType.SubscriptionInvoicePaymentFailed.GetEnumValue();
                        objUserNotification.MessageType = General.NotificationType.SubscriptionInvoicePaymentFailed.GetEnumDescription();
                        objUserNotification.CommonId = BillingId;
                    }
                    else
                    {
                        NotificationType = General.NotificationType.CreditCardExpiration.GetEnumValue();
                        objUserNotification.CommonId = Convert.ToInt32(row["CardId"].ToString());
                        objUserNotification.MessageType = General.NotificationType.CreditCardExpiration.GetEnumDescription();
                    }
                    objUserNotification.AddedDate = DateTime.UtcNow;
                    NotificationId = objUserNotificationService.AddUserNotification(ref objUserNotification);

                    DataTable dtBadgeCount = new DataTable();
                    dtBadgeCount.Clear();

                    objUserNotificationService.GetBadgeCount(int.Parse(row["ClientId"].ToString()), General.UserRoles.Client.GetEnumValue(), ref dtBadgeCount);
                    BadgeCount = dtBadgeCount.Rows.Count;

                    Notifications objNotifications = new Notifications { NId = NotificationId, NType = NotificationType, CommonId = objUserNotification.CommonId };
                    List<NotificationModel> notify = new List<NotificationModel>();
                    notify.Add(new NotificationModel { Key = "NId", Value = new object[] { objNotifications.NId } });
                    notify.Add(new NotificationModel { Key = "NType", Value = new object[] { objNotifications.NType } });
                    notify.Add(new NotificationModel { Key = "CommonId", Value = new object[] { objNotifications.CommonId } });

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

                }

               
            }
        }
    }
}