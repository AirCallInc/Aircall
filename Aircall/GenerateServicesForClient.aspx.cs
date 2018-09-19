using System;
using System.Collections.Generic;
using System.Web;
using Services;
using System.Net;
using System.Text;
using System.IO;
using System.Data;
using System.Configuration;
using JdSoft.Apple.Apns.Notifications;
using Aircall.Common;
using System.Collections;
using System.Reflection;

namespace Aircall
{
    public partial class GenerateServicesForClient : System.Web.UI.Page
    {
        IServicesService objServicesService;
        IClientService objClientService;
        IEmployeeService objEmployeeService;
        IUserNotificationService objUserNotificationService;
        IEmailTemplateService objEmailTemplateService;
        IClientAddressService objClientAddressService;

        protected void Page_Load(object sender, EventArgs e)
        {
            //DataTable dtEmployee = new DataTable();
            //objEmployeeService = ServiceFactory.EmployeeService;
            //objEmployeeService.GetEmployeeById(4, ref dtEmployee);
            //if (dtEmployee.Rows.Count > 0)
            //{
            //    if (!string.IsNullOrEmpty(dtEmployee.Rows[0]["DeviceType"].ToString()) &&
            //        !string.IsNullOrEmpty(dtEmployee.Rows[0]["DeviceToken"].ToString()))
            //    {
            //        if (dtEmployee.Rows[0]["DeviceType"].ToString().ToLower() == "iphone")
            //        {
            //            SendIphoneNotification("4", dtEmployee.Rows[0]["DeviceToken"].ToString(), "hiii", "employee");
            //        }
            //    }
            //}
            bool AutomaticScheduleService = true;
            string AutomaticScheduleServiceStr = General.GetSitesettingsValue("AutomaticScheduleService");

            if (AutomaticScheduleServiceStr.ToLower() == "on")
                AutomaticScheduleService = true;
            else
                AutomaticScheduleService = false;

            if (AutomaticScheduleService)
            {
                DataTable dtServiceUnits = new DataTable();
                DataTable dtClient = new DataTable();
                DataTable dtServiceId = new DataTable();
                DataTable dtBadgeCount = new DataTable();

                string DeviceType = string.Empty;
                string DeviceToken = string.Empty;
                int ClientId;
                int AddressId;
                int FirstServiceWithinDays;
                int DurationInMonth;
                int NumberOfService;
                int PlanTypeId;
                objServicesService = ServiceFactory.ServicesService;
                objClientService = ServiceFactory.ClientService;
                objServicesService.GetServiceUnits(ref dtServiceUnits);
                if (dtServiceUnits.Rows.Count > 0)
                {
                    for (int i = 0; i < dtServiceUnits.Rows.Count; i++)
                    {
                        DataTable dtIserviceAdded = new DataTable();
                        bool GoForMergeUnit = false;
                        ClientId = Convert.ToInt32(dtServiceUnits.Rows[i]["ClientId"].ToString());
                        AddressId = Convert.ToInt32(dtServiceUnits.Rows[i]["AddressId"].ToString());
                        FirstServiceWithinDays = Convert.ToInt32(dtServiceUnits.Rows[i]["FirstServiceWithinDays"].ToString());
                        DurationInMonth = Convert.ToInt32(dtServiceUnits.Rows[i]["DurationInMonth"].ToString());
                        NumberOfService = Convert.ToInt32(dtServiceUnits.Rows[i]["NumberOfService"].ToString());
                        PlanTypeId = Convert.ToInt32(dtServiceUnits.Rows[i]["PlanTypeId"].ToString());
                        dtClient.Clear();
                        dtServiceId.Clear();
                        dtIserviceAdded.Clear();
                        objServicesService.CheckServiceAlreadyAddedForClientAddress(ClientId, AddressId, PlanTypeId, ref dtIserviceAdded);
                        if (Convert.ToBoolean(dtIserviceAdded.Rows[0]["ServiceAdded"].ToString()))
                            GoForMergeUnit = true;
                        else
                            GoForMergeUnit = false;

                        if (GoForMergeUnit)
                            objServicesService.ScheduleAllServiceForClientUnitWithMerge(ClientId, AddressId, FirstServiceWithinDays, DurationInMonth, NumberOfService, PlanTypeId, ref dtServiceId);
                        else
                            objServicesService.ScheduleAllServiceForClientUnit(ClientId, AddressId, FirstServiceWithinDays, DurationInMonth, NumberOfService, PlanTypeId, ref dtServiceId);

                        if (dtServiceId.Rows.Count > 0)
                        {
                            for (int j = 0; j < dtServiceId.Rows.Count; j++)
                            {
                                if (GoForMergeUnit == false)
                                {
                                    if (dtServiceId.Rows[j]["ServiceId"].ToString() != "0" &&
                                        dtServiceId.Rows[j]["EmployeeId"].ToString() != "0")
                                    {
                                        long ServiceId = Convert.ToInt64(dtServiceId.Rows[j]["ServiceId"].ToString());
                                        int EmployeeId = Convert.ToInt32(dtServiceId.Rows[j]["EmployeeId"].ToString());
                                        long NotificationId = 0;
                                        int BadgeCount = 0;

                                        objClientService.GetClientById(ClientId, ref dtClient);
                                        if (dtClient.Rows.Count > 0)
                                        {
                                            if (!string.IsNullOrEmpty(dtClient.Rows[0]["DeviceType"].ToString()) &&
                                                !string.IsNullOrEmpty(dtClient.Rows[0]["DeviceToken"].ToString()) &&
                                                dtClient.Rows[0]["DeviceToken"].ToString().ToLower() != "no device token")
                                            {
                                                BizObjects.UserNotification objUserNotification = new BizObjects.UserNotification();
                                                string message = General.GetNotificationMessage("ServiceApprovelSendToClient"); //"Your Service is Scheduled.Please Approve or Reschedule.";
                                                objUserNotificationService = ServiceFactory.UserNotificationService;
                                                objUserNotification.UserId = ClientId;
                                                objUserNotification.UserTypeId = General.UserRoles.Client.GetEnumValue();
                                                objUserNotification.Message = message;
                                                objUserNotification.Status = General.NotificationStatus.UnRead.GetEnumDescription();
                                                objUserNotification.CommonId = ServiceId;
                                                objUserNotification.MessageType = General.NotificationType.ServiceApproval.GetEnumDescription();
                                                objUserNotification.AddedDate = DateTime.UtcNow;

                                                NotificationId = objUserNotificationService.AddUserNotification(ref objUserNotification);

                                                dtBadgeCount.Clear();
                                                objUserNotificationService.GetBadgeCount(ClientId, General.UserRoles.Client.GetEnumValue(), ref dtBadgeCount);
                                                BadgeCount = dtBadgeCount.Rows.Count;

                                                Notifications objNotifications = new Notifications { NId = NotificationId, NType = 1, CommonId = ServiceId };
                                                List<NotificationModel> notify = new List<NotificationModel>();
                                                notify.Add(new NotificationModel { Key = "NId", Value = new object[] { objNotifications.NId } });
                                                notify.Add(new NotificationModel { Key = "NType", Value = new object[] { objNotifications.NType } });
                                                notify.Add(new NotificationModel { Key = "CommonId", Value = new object[] { objNotifications.CommonId } });

                                                if (dtClient.Rows[0]["DeviceType"].ToString().ToLower() == "android")
                                                {
                                                    string CustomData = "&data.NId=" + objNotifications.NId + "&data.NType=" + objNotifications.NType + "&data.CommonId=" + objNotifications.CommonId;
                                                    //SendNotifications.SendAndroidNotification(dtClient.Rows[0]["DeviceToken"].ToString(), message, CustomData, "client");
                                                }
                                                else if (dtClient.Rows[0]["DeviceType"].ToString().ToLower() == "iphone")
                                                {
                                                    //SendNotifications.SendIphoneNotification(BadgeCount, dtClient.Rows[0]["DeviceToken"].ToString(), message, notify, "client");
                                                }

                                                DataTable dtApprovalEmail = new DataTable();
                                                objServicesService.SetServiceApprovalUrl(ServiceId, ref dtApprovalEmail);
                                                if (dtApprovalEmail.Rows.Count > 0)
                                                {
                                                    DataTable dtEmailtemplate = new DataTable();
                                                    objEmailTemplateService = ServiceFactory.EmailTemplateService;
                                                    objEmailTemplateService.GetByName("ServiceApproval", ref dtEmailtemplate);
                                                    if (dtEmailtemplate.Rows.Count > 0)
                                                    {
                                                        string Emailbody = dtEmailtemplate.Rows[0]["EmailBody"].ToString();
                                                        string CCEmail = dtEmailtemplate.Rows[0]["CCEmails"].ToString();
                                                        string ApprovalUrl = Application["SiteAddress"] + "ServiceApproval.aspx?Url=" + dtApprovalEmail.Rows[0]["ApprovalEmailUrl"].ToString();
                                                        Emailbody = Emailbody.Replace("{{Link}}", ApprovalUrl);
                                                        Emailbody = Emailbody.Replace("{{UserName}}", dtClient.Rows[0]["ClientName"].ToString());
                                                        string Subject = dtEmailtemplate.Rows[0]["EmailTemplateSubject"].ToString();
                                                        //Email.SendEmail(Subject, dtClient.Rows[0]["Email"].ToString(), CCEmail, "", Emailbody);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (dtServiceId.Rows[j]["ServiceId"].ToString() != "0" &&
                                    dtServiceId.Rows[j]["EmployeeId"].ToString() != "0" &&
                                    Convert.ToBoolean(dtServiceId.Rows[j]["IsMovedToPending"].ToString()))
                                    {
                                        long ServiceId = Convert.ToInt64(dtServiceId.Rows[j]["ServiceId"].ToString());
                                        int EmployeeId = Convert.ToInt32(dtServiceId.Rows[j]["EmployeeId"].ToString());
                                        string ServiceCaseNo = dtServiceId.Rows[j]["ServiceCaseNumber"].ToString();

                                        //Send Notification to Client
                                        long NotificationId = 0;
                                        int BadgeCount = 0;
                                        string message = string.Empty;
                                        BizObjects.UserNotification objUserNotification = new BizObjects.UserNotification();
                                        objUserNotificationService = ServiceFactory.UserNotificationService;

                                        DataTable dtAddress = new DataTable();
                                        string strAddress = string.Empty;
                                        objClientAddressService = ServiceFactory.ClientAddressService;
                                        objClientAddressService.GetAddressById(AddressId, ref dtAddress);
                                        if (dtAddress.Rows.Count > 0)
                                            strAddress = dtAddress.Rows[0]["Address"].ToString();

                                        objClientService.GetClientById(ClientId, ref dtClient);
                                        if (dtClient.Rows.Count > 0)
                                        {
                                            message = General.GetNotificationMessage("MergeUnits");
                                            message = message.Replace("{{Address}}", strAddress.ToString());
                                            objUserNotification.UserId = ClientId;
                                            objUserNotification.UserTypeId = General.UserRoles.Client.GetEnumValue();
                                            objUserNotification.Message = message;
                                            objUserNotification.Status = General.NotificationStatus.UnRead.GetEnumDescription();
                                            objUserNotification.CommonId = ServiceId;
                                            objUserNotification.MessageType = General.NotificationType.FriendlyReminder.GetEnumDescription();
                                            objUserNotification.AddedDate = DateTime.UtcNow;

                                            NotificationId = objUserNotificationService.AddUserNotification(ref objUserNotification);

                                            dtBadgeCount.Clear();

                                            objUserNotificationService.GetBadgeCount(ClientId, General.UserRoles.Client.GetEnumValue(), ref dtBadgeCount);
                                            BadgeCount = dtBadgeCount.Rows.Count;

                                            Notifications objNotifications = new Notifications { NId = NotificationId, NType = General.NotificationType.FriendlyReminder.GetEnumValue(), CommonId = ServiceId };
                                            List<NotificationModel> notify = new List<NotificationModel>();
                                            notify.Add(new NotificationModel { Key = "NId", Value = new object[] { objNotifications.NId } });
                                            notify.Add(new NotificationModel { Key = "NType", Value = new object[] { objNotifications.NType } });
                                            notify.Add(new NotificationModel { Key = "CommonId", Value = new object[] { objNotifications.CommonId } });

                                            if (!string.IsNullOrEmpty(dtClient.Rows[0]["DeviceType"].ToString()) &&
                                                !string.IsNullOrEmpty(dtClient.Rows[0]["DeviceToken"].ToString()) &&
                                                dtClient.Rows[0]["DeviceToken"].ToString().ToLower() != "no device token")
                                            {
                                                if (dtClient.Rows[0]["DeviceType"].ToString().ToLower() == "android")
                                                {
                                                    string CustomData = "&data.NId=" + objNotifications.NId + "&data.NType=" + objNotifications.NType + "&data.CommonId=" + objNotifications.CommonId;
                                                    //SendNotifications.SendAndroidNotification(dtClient.Rows[0]["DeviceToken"].ToString(), message, CustomData, "client");
                                                }
                                                else if (dtClient.Rows[0]["DeviceType"].ToString().ToLower() == "iphone")
                                                {
                                                    //SendNotifications.SendIphoneNotification(BadgeCount, dtClient.Rows[0]["DeviceToken"].ToString(), message, notify, "client");
                                                }
                                            }
                                        }

                                        //Send Notification to Employee
                                        DataTable dtEmployee = new DataTable();
                                        dtBadgeCount.Clear();
                                        objEmployeeService = ServiceFactory.EmployeeService;
                                        objEmployeeService.GetEmployeeById(EmployeeId, ref dtEmployee);
                                        if (dtEmployee.Rows.Count > 0)
                                        {
                                            BizObjects.UserNotification objUserNotification1 = new BizObjects.UserNotification();
                                            objUserNotification1.UserId = EmployeeId;
                                            objUserNotification1.UserTypeId = General.UserRoles.Employee.GetEnumValue();
                                            message = General.GetNotificationMessage("MergeUnits");
                                            message = message.Replace("{{Address}}", strAddress.ToString());
                                            objUserNotification1.Message = message;

                                            objUserNotification1.Status = General.NotificationStatus.UnRead.GetEnumDescription();
                                            objUserNotification1.CommonId = ServiceId;
                                            objUserNotification1.MessageType = General.NotificationType.FriendlyReminder.GetEnumDescription();
                                            objUserNotification1.AddedDate = DateTime.UtcNow;

                                            NotificationId = objUserNotificationService.AddUserNotification(ref objUserNotification1);

                                            objUserNotificationService.GetBadgeCount(EmployeeId, General.UserRoles.Employee.GetEnumValue(), ref dtBadgeCount);
                                            int EmployeeBadgeCount = dtBadgeCount.Rows.Count;

                                            Notifications objNotifications1 = new Notifications { NId = NotificationId, NType = General.NotificationType.FriendlyReminder.GetEnumValue(), CommonId = ServiceId };
                                            List<NotificationModel> notify1 = new List<NotificationModel>();
                                            notify1.Add(new NotificationModel { Key = "NId", Value = new object[] { objNotifications1.NId } });
                                            notify1.Add(new NotificationModel { Key = "NType", Value = new object[] { objNotifications1.NType } });
                                            notify1.Add(new NotificationModel { Key = "CommonId", Value = new object[] { objNotifications1.CommonId } });

                                            if (!string.IsNullOrEmpty(dtEmployee.Rows[0]["DeviceType"].ToString()) &&
                                                !string.IsNullOrEmpty(dtEmployee.Rows[0]["DeviceToken"].ToString()) &&
                                                dtEmployee.Rows[0]["DeviceToken"].ToString().ToLower() != "no device token")
                                            {
                                                if (dtEmployee.Rows[0]["DeviceType"].ToString().ToLower() == "iphone")
                                                {
                                                    //SendNotifications.SendIphoneNotification(EmployeeBadgeCount, dtEmployee.Rows[0]["DeviceToken"].ToString(), objUserNotification1.Message, notify1, "employee");
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            //objServicesService.RunScheduler();
        }
    }
}