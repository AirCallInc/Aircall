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
    public partial class ServiceReminder : System.Web.UI.Page
    {
        IServicesService objServicesService;
        IUserNotificationService objUserNotificationService;
        IClientUnitService objClientUnitService;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {                
                //NotifyType : 1= 1st Reminder, 2=2nd Reminder, 3=3rd Reminder
                bool IsMorning = false;
                TimeSpan ts = new TimeSpan(12, 0, 0); //12 o'clock
                TimeSpan now = DateTime.UtcNow.TimeOfDay;

                if (now <= ts)
                    IsMorning = true;

                objServicesService = ServiceFactory.ServicesService;
                DataTable dtReminder = new DataTable();
                objServicesService.ScheduledServiceReminder(3, IsMorning, ref dtReminder);
                if (dtReminder.Rows.Count > 0)
                {
                    for (int i = 0; i < dtReminder.Rows.Count; i++)
                    {
                        long ServiceId = Convert.ToInt64(dtReminder.Rows[i]["Id"].ToString());
                        string ServiceCaseNo = dtReminder.Rows[i]["ServiceCaseNumber"].ToString();
                        string EmployeeName = dtReminder.Rows[i]["EmployeeName"].ToString();
                        int ClientId = Convert.ToInt32(dtReminder.Rows[i]["ClientId"].ToString());
                        string Duration = dtReminder.Rows[i]["Duration"].ToString();
                        string strAddress = dtReminder.Rows[i]["Address"].ToString();
                        string message = General.GetNotificationMessage("PeriodicServiceReminderSendToClient"); //"Service " + ServiceCaseNo + " will be done within next " + dtReminder.Rows[i]["Duration"].ToString() + ".";
                        message = message.Replace("{{Address}}", strAddress);
                        message = message.Replace("{{Duration}}", dtReminder.Rows[i]["Duration"].ToString());
                        string DeviceType = dtReminder.Rows[i]["DeviceType"].ToString().ToLower();
                        string DeviceToken = dtReminder.Rows[i]["DeviceToken"].ToString().ToLower();

                        objClientUnitService = ServiceFactory.ClientUnitService;
                        objClientUnitService.SetStatusByServiceId(General.UnitStatus.ServiceSoon.GetEnumValue(), ServiceId);

                        NotifyUser(ClientId, ServiceId, message, DeviceType, DeviceToken);
                    }
                }
                dtReminder.Clear();
                objServicesService.ScheduledServiceReminder(2, IsMorning, ref dtReminder);
                if (dtReminder.Rows.Count > 0)
                {
                    for (int i = 0; i < dtReminder.Rows.Count; i++)
                    {
                        long ServiceId = Convert.ToInt64(dtReminder.Rows[i]["Id"].ToString());
                        string ServiceCaseNo = dtReminder.Rows[i]["ServiceCaseNumber"].ToString();
                        int ClientId = Convert.ToInt32(dtReminder.Rows[i]["ClientId"].ToString());
                        string strAddress = dtReminder.Rows[i]["Address"].ToString();
                        string message = General.GetNotificationMessage("PeriodicServiceReminderSendToClient"); //"Service " + ServiceCaseNo + " will be done within next " + dtReminder.Rows[i]["Duration"].ToString() + ".";
                        message = message.Replace("{{Address}}", strAddress);
                        message = message.Replace("{{Duration}}", dtReminder.Rows[i]["Duration"].ToString());
                        string DeviceType = dtReminder.Rows[i]["DeviceType"].ToString().ToLower();
                        string DeviceToken = dtReminder.Rows[i]["DeviceToken"].ToString().ToLower();

                        objClientUnitService = ServiceFactory.ClientUnitService;
                        objClientUnitService.SetStatusByServiceId(General.UnitStatus.ServiceSoon.GetEnumValue(), ServiceId);

                        NotifyUser(ClientId, ServiceId, message, DeviceType, DeviceToken);
                    }
                }
                dtReminder.Clear();
                objServicesService.ScheduledServiceReminder(1, IsMorning, ref dtReminder);
                if (dtReminder.Rows.Count > 0)
                {
                    for (int i = 0; i < dtReminder.Rows.Count; i++)
                    {
                        long ServiceId = Convert.ToInt64(dtReminder.Rows[i]["Id"].ToString());
                        string ServiceCaseNo = dtReminder.Rows[i]["ServiceCaseNumber"].ToString();
                        int ClientId = Convert.ToInt32(dtReminder.Rows[i]["ClientId"].ToString());
                        string strAddress = dtReminder.Rows[i]["Address"].ToString();
                        string message = General.GetNotificationMessage("PeriodicServiceReminderSendToClient"); //"Service " + ServiceCaseNo + " will be done within next " + dtReminder.Rows[i]["Duration"].ToString() + ".";
                        message = message.Replace("{{Address}}", strAddress);
                        message = message.Replace("{{Duration}}", dtReminder.Rows[i]["Duration"].ToString());
                        string DeviceType = dtReminder.Rows[i]["DeviceType"].ToString().ToLower();
                        string DeviceToken = dtReminder.Rows[i]["DeviceToken"].ToString().ToLower();

                        objClientUnitService = ServiceFactory.ClientUnitService;
                        objClientUnitService.SetStatusByServiceId(General.UnitStatus.ServiceSoon.GetEnumValue(), ServiceId);

                        NotifyUser(ClientId, ServiceId, message, DeviceType, DeviceToken);
                    }
                }
                ltrMessage.Text = "Service Reminder Notification Sent Successfully.";
            }
            catch (Exception Ex)
            {
                ltrMessage.Text = Ex.Message.ToString().Trim();
            }
        }

        private void NotifyUser(int ClientId, long ServiceId, string message, string DeviceType, string DeviceToken)
        {
            long NotificationId = 0;
            DataTable dtBadgeCount = new DataTable();
            int BadgeCount = 0;
            
            objUserNotificationService = ServiceFactory.UserNotificationService;
            objUserNotificationService.DeleteNotificationByUserIdTypeScheduler(ClientId, General.UserRoles.Client.GetEnumValue(), General.NotificationType.PeriodicServiceReminder.GetEnumDescription(), ServiceId);

            BizObjects.UserNotification objUserNotification = new BizObjects.UserNotification();
            //objUserNotificationService = ServiceFactory.UserNotificationService;
            objUserNotification.UserId = ClientId;
            objUserNotification.UserTypeId = General.UserRoles.Client.GetEnumValue();
            objUserNotification.Message = message;
            objUserNotification.CommonId = ServiceId;
            objUserNotification.Status = General.NotificationStatus.UnRead.GetEnumDescription();
            objUserNotification.MessageType = General.NotificationType.PeriodicServiceReminder.GetEnumDescription();
            objUserNotification.AddedDate = DateTime.UtcNow;

            NotificationId = objUserNotificationService.AddUserNotification(ref objUserNotification);

            objUserNotificationService.GetBadgeCount(ClientId, General.UserRoles.Client.GetEnumValue(), ref dtBadgeCount);
            BadgeCount = dtBadgeCount.Rows.Count;

            Notifications objNotifications = new Notifications { NId = NotificationId, NType = General.NotificationType.PeriodicServiceReminder.GetEnumValue(), CommonId = ServiceId };
            List<NotificationModel> notify = new List<NotificationModel>();
            notify.Add(new NotificationModel { Key = "NId", Value = new object[] { objNotifications.NId } });
            notify.Add(new NotificationModel { Key = "NType", Value = new object[] { objNotifications.NType } });
            notify.Add(new NotificationModel { Key = "CommonId", Value = new object[] { objNotifications.CommonId } });

            if (DeviceType.ToLower() == "android")
            {
                string CustomData = "&data.NId=" + objNotifications.NId + "&data.NType=" + objNotifications.NType + "&data.CommonId=" + objNotifications.CommonId;
                SendNotifications.SendAndroidNotification(DeviceToken, message, CustomData, "client");
            }
            else if (DeviceType.ToLower() == "iphone")
            {
                SendNotifications.SendIphoneNotification(BadgeCount, DeviceToken, message, notify, "client");
            }
        }
    }
}