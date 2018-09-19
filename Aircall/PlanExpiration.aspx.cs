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
    public partial class PlanExpiration : System.Web.UI.Page
    {
        IClientUnitService objClientUnitService;
        IUserNotificationService objUserNotificationService;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                DataTable dtUnitPlanExpiration = new DataTable();
                DataTable dtBadgeCount = new DataTable();
                objClientUnitService = ServiceFactory.ClientUnitService;
                objUserNotificationService = ServiceFactory.UserNotificationService;
                objClientUnitService.ClientUnitPlanExpiration(ref dtUnitPlanExpiration);
                if (dtUnitPlanExpiration.Rows.Count > 0)
                {
                    for (int i = 0; i < dtUnitPlanExpiration.Rows.Count; i++)
                    {

                        int UnitId = Convert.ToInt32(dtUnitPlanExpiration.Rows[i]["Id"].ToString());
                        int ClientId = Convert.ToInt32(dtUnitPlanExpiration.Rows[i]["ClientId"].ToString());
                        string ClientEmail = dtUnitPlanExpiration.Rows[i]["Email"].ToString();
                        long NotificationId = 0;
                        int BadgeCount = 0;
                        BizObjects.UserNotification objUserNotification = new BizObjects.UserNotification();
                        string message;
                        string MessageType;
                        int MessageTypeVal;
                        if (Convert.ToBoolean(dtUnitPlanExpiration.Rows[i]["AutoRenewal"].ToString()))
                        {
                            if (!Convert.ToBoolean(dtUnitPlanExpiration.Rows[i]["ZipCodeStatus"].ToString()))
                            {
                                message = "Currently we are not providing service on your area. So your unit [UnitName] has not been renewed.";
                                MessageType = General.NotificationType.PlanRenewed.GetEnumDescription();
                                MessageTypeVal = General.NotificationType.PlanRenewed.GetEnumValue();
                            }
                            else
                            {
                                message = General.GetNotificationMessage("PlanExpirationAutomaticRenew");
                                message = message.Replace("{{UnitName}}", dtUnitPlanExpiration.Rows[i]["UnitName"].ToString());
                                message = message.Replace("{{Days}}", General.GetSitesettingsValue("PlanExpirationNotifyDays").ToString());
                                MessageType = General.NotificationType.PlanRenewed.GetEnumDescription();
                                MessageTypeVal = General.NotificationType.PlanRenewed.GetEnumValue();
                            }
                        }
                        else
                        {
                            message = General.GetNotificationMessage("PlanExpirationAutomaticRenew");
                            message = message.Replace("{{UnitName}}", dtUnitPlanExpiration.Rows[i]["UnitName"].ToString());
                            MessageType = General.NotificationType.PlanExpiration.GetEnumDescription();
                            MessageTypeVal = General.NotificationType.PlanExpiration.GetEnumValue();



                            objUserNotification.UserId = ClientId;
                            objUserNotification.UserTypeId = General.UserRoles.Client.GetEnumValue();
                            objUserNotification.Message = message;
                            objUserNotification.Status = General.NotificationStatus.UnRead.GetEnumDescription();
                            objUserNotification.CommonId = UnitId;
                            objUserNotification.MessageType = MessageType;
                            objUserNotification.AddedDate = DateTime.UtcNow;

                            NotificationId = objUserNotificationService.AddUserNotification(ref objUserNotification);

                            dtBadgeCount.Clear();
                            objUserNotificationService.GetBadgeCount(ClientId, General.UserRoles.Client.GetEnumValue(), ref dtBadgeCount);
                            BadgeCount = dtBadgeCount.Rows.Count;

                            Notifications objNotifications = new Notifications { NId = NotificationId, NType = MessageTypeVal, CommonId = UnitId };
                            List<NotificationModel> notify = new List<NotificationModel>();
                            notify.Add(new NotificationModel { Key = "NId", Value = new object[] { objNotifications.NId } });
                            notify.Add(new NotificationModel { Key = "NType", Value = new object[] { objNotifications.NType } });
                            notify.Add(new NotificationModel { Key = "CommonId", Value = new object[] { objNotifications.CommonId } });

                            if (!string.IsNullOrEmpty(dtUnitPlanExpiration.Rows[i]["DeviceType"].ToString()) &&
                            !string.IsNullOrEmpty(dtUnitPlanExpiration.Rows[i]["DeviceToken"].ToString()) &&
                             dtUnitPlanExpiration.Rows[i]["DeviceToken"].ToString().ToLower() != "no device token")
                            {
                                if (dtUnitPlanExpiration.Rows[i]["DeviceType"].ToString().ToLower() == "android")
                                {
                                    string CustomData = "&data.NId=" + objNotifications.NId + "&data.NType=" + objNotifications.NType + "&data.CommonId=" + objNotifications.CommonId;
                                    SendNotifications.SendAndroidNotification(dtUnitPlanExpiration.Rows[i]["DeviceToken"].ToString(), message, CustomData, "client");
                                }
                                else if (dtUnitPlanExpiration.Rows[i]["DeviceType"].ToString().ToLower() == "iphone")
                                {
                                    SendNotifications.SendIphoneNotification(BadgeCount, dtUnitPlanExpiration.Rows[i]["DeviceToken"].ToString(), message, notify, "client");
                                }
                                //string EmailBody = "Package Plan For unit: " + dtUnitPlanExpiration.Rows[i]["UnitName"].ToString() + "is going to expire.Please renew plan.";
                                //Email.SendEmail("Plan Expiration For unit:" + dtUnitPlanExpiration.Rows[i]["UnitName"].ToString(), ClientEmail, "", "", EmailBody);
                            }
                        }
                    }
                    ltrMessage.Text = "Plan Expiration Scheduler run successfully.";
                }
            }
            catch (Exception Ex)
            {
                ltrMessage.Text = Ex.Message.ToString().Trim();
            }
        }
    }
}