using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Services;
using System.Data;
using Aircall.Common;

namespace Aircall
{
    public partial class CardExpiration : System.Web.UI.Page
    {
        IClientPaymentMethodService objClientPaymentMethodService;
        IUserNotificationService objUserNotificationService;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                DataTable dtCardExpiration = new DataTable();
                DataTable dtBadgeCount = new DataTable();
                objClientPaymentMethodService = ServiceFactory.ClientPaymentMethodService;
                objClientPaymentMethodService.CardExpirationScheduler(ref dtCardExpiration);
                if (dtCardExpiration.Rows.Count > 0)
                {
                    for (int i = 0; i < dtCardExpiration.Rows.Count; i++)
                    {
                        long NotificationId = 0;
                        int BadgeCount = 0;
                        int ClientId = Convert.ToInt32(dtCardExpiration.Rows[i]["ClientId"].ToString());
                        int CardId = Convert.ToInt32(dtCardExpiration.Rows[i]["Id"].ToString());
                        BizObjects.UserNotification objUserNotification = new BizObjects.UserNotification();
                        string message = General.GetNotificationMessage("CardExpirationSendToClient"); //"Your Card " + dtCardExpiration.Rows[i]["CardNumber"].ToString() + " is about to Expire. Update your card information before it expires.";
                        message = message.Replace("{{CardNumber}}", dtCardExpiration.Rows[i]["CardNumber"].ToString());
                        objUserNotificationService = ServiceFactory.UserNotificationService;
                        objUserNotification.UserId = ClientId;
                        objUserNotification.UserTypeId = General.UserRoles.Client.GetEnumValue();
                        objUserNotification.Message = message;
                        objUserNotification.Status = General.NotificationStatus.UnRead.GetEnumDescription();
                        objUserNotification.CommonId = CardId;
                        objUserNotification.MessageType = General.NotificationType.CreditCardExpiration.GetEnumDescription();
                        objUserNotification.AddedDate = DateTime.UtcNow;

                        NotificationId = objUserNotificationService.AddUserNotification(ref objUserNotification);

                        dtBadgeCount.Clear();
                        objUserNotificationService.GetBadgeCount(ClientId, General.UserRoles.Client.GetEnumValue(), ref dtBadgeCount);
                        BadgeCount = dtBadgeCount.Rows.Count;

                        Notifications objNotifications = new Notifications { NId = NotificationId, NType = General.NotificationType.CreditCardExpiration.GetEnumValue(), CommonId = CardId };
                        List<NotificationModel> notify = new List<NotificationModel>();
                        notify.Add(new NotificationModel { Key = "NId", Value = new object[] { objNotifications.NId } });
                        notify.Add(new NotificationModel { Key = "NType", Value = new object[] { objNotifications.NType } });
                        notify.Add(new NotificationModel { Key = "CommonId", Value = new object[] { objNotifications.CommonId } });

                        if (!string.IsNullOrEmpty(dtCardExpiration.Rows[i]["DeviceType"].ToString()) &&
                            !string.IsNullOrEmpty(dtCardExpiration.Rows[i]["DeviceToken"].ToString()) &&
                             dtCardExpiration.Rows[i]["DeviceToken"].ToString().ToLower() != "no device token")
                        {
                            if (dtCardExpiration.Rows[i]["DeviceType"].ToString().ToLower() == "android")
                            {
                                string CustomData = "&data.NId=" + objNotifications.NId + "&data.NType=" + objNotifications.NType + "&data.CommonId=" + objNotifications.CommonId;
                                SendNotifications.SendAndroidNotification(dtCardExpiration.Rows[i]["DeviceToken"].ToString(), message, CustomData, "client");
                            }
                            else if (dtCardExpiration.Rows[i]["DeviceType"].ToString().ToLower() == "iphone")
                            {
                                SendNotifications.SendIphoneNotification(BadgeCount, dtCardExpiration.Rows[i]["DeviceToken"].ToString(), message, notify, "client");
                            }
                        }
                        objClientPaymentMethodService.UpdateIsExpireationFlagByCardId(CardId);
                    }
                }
                ltrMessage.Text = "Card Expiration Scheduler Run Successfully.";
            }
            catch (Exception Ex)
            {
                ltrMessage.Text = Ex.Message.ToString().Trim();
            }
        }
    }
}