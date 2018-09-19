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
    public partial class SchedulePendingService : System.Web.UI.Page
    {
        IServicesService objServicesService;
        IClientService objClientService;
        IUserNotificationService objUserNotificationService;
        IEmailTemplateService objEmailTemplateService;
        IClientUnitService objClientUnitService;
        IEmployeeService objEmployeeService;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                bool AutomaticScheduleService = true;
                string AutomaticScheduleServiceStr = General.GetSitesettingsValue("AutomaticScheduleService");

                if (AutomaticScheduleServiceStr.ToLower() == "on")
                    AutomaticScheduleService = true;
                else
                    AutomaticScheduleService = false;

                if (AutomaticScheduleService)
                {
                    objServicesService = ServiceFactory.ServicesService;
                    objClientService = ServiceFactory.ClientService;
                    DataTable dtServices = new DataTable();
                    DataTable dtClient = new DataTable();
                    objServicesService.GetPendingServiceForScheduler(ref dtServices);
                    if (dtServices.Rows.Count > 0)
                    {
                        for (int i = 0; i < dtServices.Rows.Count; i++)
                        {
                            long ServiceId = Convert.ToInt64(dtServices.Rows[i]["Id"].ToString());
                            int ClientId = Convert.ToInt32(dtServices.Rows[i]["ClientId"].ToString());
                            int AddressID = Convert.ToInt32(dtServices.Rows[i]["AddressID"].ToString());
                            DateTime ExpectedStartDate = Convert.ToDateTime(dtServices.Rows[i]["ExpectedStartDate"].ToString());
                            DateTime ExpectedEndDate = Convert.ToDateTime(dtServices.Rows[i]["ExpectedEndDate"].ToString());
                            DataTable dtService = new DataTable();
                            dtService.Clear();
                            dtClient.Clear();
                            objServicesService.SchedulePendingService(ServiceId, ClientId, AddressID, ExpectedStartDate, ExpectedEndDate, ref dtService);
                            if (dtService.Rows.Count > 0)
                            {
                                for (int j = 0; j < dtService.Rows.Count; j++)
                                {
                                    //if (Convert.ToBoolean(dtService.Rows[0]["IsEmployeeAvailable"].ToString()) &&
                                    //Convert.ToBoolean(dtService.Rows[0]["IsPartInStock"].ToString()) &&
                                    //Convert.ToInt32(dtService.Rows[0]["EmployeeId"].ToString()) > 0)
                                    //{
                                    //Send Notification to Client
                                    string PurposeOfVisit = dtService.Rows[j]["PurposeOfVisit"].ToString();

                                    DateTime ScheduleDate = Convert.ToDateTime(dtService.Rows[j]["ScheduleDate"].ToString());
                                    ServiceId = Convert.ToInt64(dtService.Rows[j]["ServiceId"].ToString());
                                    int EmployeeId = Convert.ToInt32(dtService.Rows[j]["EmployeeId"].ToString());
                                    objClientService.GetClientById(ClientId, ref dtClient);
                                    if (dtClient.Rows.Count > 0)
                                    {
                                        string message = string.Empty;
                                        string MessageType = string.Empty;
                                        int NotificationType;

                                        if (PurposeOfVisit == General.PurposeOfVisit.Emergency.GetEnumDescription() ||
                                        PurposeOfVisit == General.PurposeOfVisit.ContinuingPreviousWork.GetEnumDescription()||
                                        PurposeOfVisit == General.PurposeOfVisit.Repairing.GetEnumDescription())
                                        {

                                            message = General.GetNotificationMessage("PendingServiceScheduleSendToClient");
                                            message = message.Replace("{{ScheduleDate}}", ScheduleDate.ToString("MMMM dd, yyyy"));
                                            MessageType = General.NotificationType.PeriodicServiceReminder.GetEnumDescription();
                                            NotificationType = General.NotificationType.PeriodicServiceReminder.GetEnumValue();
                                        }
                                        else
                                        {
                                            message = General.GetNotificationMessage("ServiceApprovelSendToClient");//"Your Service is Scheduled.Please Approve or Reschedule.";
                                            MessageType = General.NotificationType.ServiceApproval.GetEnumDescription();
                                            NotificationType = General.NotificationType.ServiceApproval.GetEnumValue();
                                        }
                                        long NotificationId = 0;
                                        int BadgeCount = 0;
                                        BizObjects.UserNotification objUserNotification = new BizObjects.UserNotification();

                                        objUserNotificationService = ServiceFactory.UserNotificationService;
                                        objUserNotification.UserId = ClientId;
                                        objUserNotification.UserTypeId = General.UserRoles.Client.GetEnumValue();
                                        objUserNotification.Message = message;
                                        objUserNotification.Status = General.NotificationStatus.UnRead.GetEnumDescription();
                                        objUserNotification.CommonId = ServiceId;
                                        objUserNotification.MessageType = MessageType;
                                        objUserNotification.AddedDate = DateTime.UtcNow;

                                        NotificationId = objUserNotificationService.AddUserNotification(ref objUserNotification);

                                        DataTable dtBadgeCount = new DataTable();
                                        dtBadgeCount.Clear();
                                        objUserNotificationService.GetBadgeCount(ClientId, General.UserRoles.Client.GetEnumValue(), ref dtBadgeCount);
                                        BadgeCount = dtBadgeCount.Rows.Count;

                                        Notifications objNotifications = new Notifications { NId = NotificationId, NType = NotificationType, CommonId = ServiceId };
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
                                                SendNotifications.SendAndroidNotification(dtClient.Rows[0]["DeviceToken"].ToString(), message, CustomData, "client");
                                            }
                                            else if (dtClient.Rows[0]["DeviceType"].ToString().ToLower() == "iphone")
                                            {
                                                SendNotifications.SendIphoneNotification(BadgeCount, dtClient.Rows[0]["DeviceToken"].ToString(), message, notify, "client");
                                            }
                                        }

                                        if (PurposeOfVisit == General.PurposeOfVisit.Maintenance.GetEnumDescription())
                                        {
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
                                                    Email.SendEmail(Subject, dtClient.Rows[0]["Email"].ToString(), CCEmail, "", Emailbody);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            DataTable dtEmailtemplate = new DataTable();
                                            DataTable dtEmailService = new DataTable();
                                            objEmailTemplateService = ServiceFactory.EmailTemplateService;
                                            objEmailTemplateService.GetByName("RequestedServiceScheduleClient", ref dtEmailtemplate);
                                            if (dtEmailtemplate.Rows.Count > 0)
                                            {
                                                objServicesService = ServiceFactory.ServicesService;
                                                objServicesService.GetServiceById(ServiceId, ref dtEmailService);
                                                string Emailbody = dtEmailtemplate.Rows[0]["EmailBody"].ToString();
                                                string CCEmail = dtEmailtemplate.Rows[0]["CCEmails"].ToString();
                                                Emailbody = Emailbody.Replace("{{ServiceCaseNumber}}", dtEmailService.Rows[0]["ServiceCaseNumber"].ToString());
                                                Emailbody = Emailbody.Replace("{{Address}}", dtEmailService.Rows[0]["ClientAddress"].ToString());
                                                Emailbody = Emailbody.Replace("{{PurposeOfVisit}}", PurposeOfVisit);
                                                Emailbody = Emailbody.Replace("{{ScheduleDate}}", Convert.ToDateTime(dtEmailService.Rows[0]["ScheduleDate"].ToString()).ToString("MMMM dd, yyyy"));
                                                Emailbody = Emailbody.Replace("{{ScheduleTime}}", dtEmailService.Rows[0]["ScheduleStartTime"].ToString() + " - " + dtEmailService.Rows[0]["ScheduleEndTime"].ToString());
                                                Emailbody = Emailbody.Replace("{{Technician}}", dtEmailService.Rows[0]["EmployeeName"].ToString());
                                                string Subject = dtEmailtemplate.Rows[0]["EmailTemplateSubject"].ToString();
                                                Subject = Subject.Replace("{{PurposeOfVisit}}", PurposeOfVisit);
                                                Email.SendEmail(Subject, dtClient.Rows[0]["Email"].ToString(), CCEmail, "", Emailbody);
                                            }
                                        }
                                    }
                                    //Update Unit Status
                                    objClientUnitService = ServiceFactory.ClientUnitService;
                                    objClientUnitService.SetStatusByServiceId(General.UnitStatus.ServiceSoon.GetEnumValue(), ServiceId);

                                    if (PurposeOfVisit == General.PurposeOfVisit.Emergency.GetEnumDescription() ||
                                            PurposeOfVisit == General.PurposeOfVisit.ContinuingPreviousWork.GetEnumDescription()||
                                        PurposeOfVisit == General.PurposeOfVisit.Repairing.GetEnumDescription())
                                    {
                                        objEmployeeService = ServiceFactory.EmployeeService;
                                        DataTable dtEmployee = new DataTable();
                                        objEmployeeService.GetEmployeeById(EmployeeId, ref dtEmployee);
                                        if (dtEmployee.Rows.Count > 0)
                                        {
                                            BizObjects.UserNotification objUserNotification1 = new BizObjects.UserNotification();
                                            objUserNotification1.UserId = EmployeeId;
                                            objUserNotification1.UserTypeId = General.UserRoles.Employee.GetEnumValue();
                                            string message = General.GetNotificationMessage("EmployeeSchedule");
                                            message = message.Replace("{{ScheduleDate}}", ScheduleDate.ToString("MMMM dd, yyyy"));
                                            objUserNotification1.Message = message;//"System has scheduled a service for you on " + ScheduleDate.ToString("MMMM dd, yyyy") + ".";

                                            objUserNotification1.Status = General.NotificationStatus.UnRead.GetEnumDescription();
                                            objUserNotification1.CommonId = ServiceId;
                                            objUserNotification1.MessageType = General.NotificationType.ServiceScheduled.GetEnumDescription();
                                            objUserNotification1.AddedDate = DateTime.UtcNow;

                                            long NotificationId = objUserNotificationService.AddUserNotification(ref objUserNotification1);

                                            DataTable dtEmployeeBadgeCount = new DataTable();
                                            objUserNotificationService.GetBadgeCount(EmployeeId, General.UserRoles.Employee.GetEnumValue(), ref dtEmployeeBadgeCount);
                                            int EmployeeBadgeCount = dtEmployeeBadgeCount.Rows.Count;

                                            Notifications objNotifications1 = new Notifications { NId = NotificationId, NType = General.NotificationType.ServiceScheduled.GetEnumValue(), CommonId = ServiceId };
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
                                                    SendNotifications.SendIphoneNotification(EmployeeBadgeCount, dtEmployee.Rows[0]["DeviceToken"].ToString(), objUserNotification1.Message, notify1, "employee");
                                                }
                                            }
                                        }
                                    }
                                    //}
                                }

                            }
                        }
                    }
                    ltrMessage.Text = "Pending Services has been scheduled successfully.";
                }
            }
            catch (Exception Ex)
            {
                ltrMessage.Text = Ex.Message.ToString().Trim();
            }
        }
    }
}