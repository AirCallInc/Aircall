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
    public partial class ScheduleRequestedService : System.Web.UI.Page
    {
        IRequestServicesService objRequestServicesService;
        IClientService objClientService;
        IEmployeeService objEmployeeService;
        IUserNotificationService objUserNotificationService;
        IServicesService objServicesService;
        IEmailTemplateService objEmailTemplateService;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                objRequestServicesService = ServiceFactory.RequestServicesService;
                objClientService = ServiceFactory.ClientService;
                objUserNotificationService = ServiceFactory.UserNotificationService;
                objServicesService = ServiceFactory.ServicesService;
                DataTable dtRequestedService = new DataTable();
                DataTable dtService = new DataTable();
                DataTable dtClient = new DataTable();
                DataTable dtBadgeCount = new DataTable();
                long RequestedServiceId = 0;
                int ClientId = 0;
                objRequestServicesService.GetAllRequestedServiceForSchedule(ref dtRequestedService);
                if (dtRequestedService.Rows.Count > 0)
                {
                    for (int i = 0; i < dtRequestedService.Rows.Count; i++)
                    {
                        dtService.Clear();
                        RequestedServiceId = Convert.ToInt64(dtRequestedService.Rows[i]["Id"].ToString());
                        ClientId = Convert.ToInt32(dtRequestedService.Rows[i]["ClientId"].ToString());
                        int AddressId = Convert.ToInt32(dtRequestedService.Rows[i]["AddressId"].ToString());
                        string PurposeOfVisit = dtRequestedService.Rows[i]["PurposeOfVisit"].ToString();
                        DateTime RequestedServiceDate = Convert.ToDateTime(dtRequestedService.Rows[i]["ServiceRequestedOn"].ToString());
                        string RequestedServiceTime = dtRequestedService.Rows[i]["ServiceRequestedTime"].ToString();
                        objRequestServicesService.ScheduleRequestedService(0,RequestedServiceId, ClientId, AddressId, PurposeOfVisit, RequestedServiceDate, RequestedServiceTime, ref dtService);
                        if (dtService.Rows.Count > 0)
                        {
                            long ServiceId = Convert.ToInt64(dtService.Rows[0]["ServiceId"].ToString());
                            int EmployeeId = Convert.ToInt32(dtService.Rows[0]["EmployeeId"].ToString());
                            if (ServiceId != 0 && EmployeeId != 0)
                            {
                                DateTime ScheduleDate = Convert.ToDateTime(dtService.Rows[0]["ScheduleDate"].ToString());

                                objClientService.GetClientById(ClientId, ref dtClient);
                                if (dtClient.Rows.Count > 0)
                                {
                                    long NotificationId = 0;
                                    int BadgeCount = 0;
                                    BizObjects.UserNotification objUserNotification = new BizObjects.UserNotification();
                                    string message;
                                    int NotificationType;
                                    string MessageType;

                                    //if (ScheduleDate != RequestedServiceDate)
                                    //{
                                    //    message = General.GetNotificationMessage("RescheduleServiceOffDateSendToClient"); //"None of employee were available on " + RequestedServiceDate.ToString("MMMM dd, yyyy") + ".So service has been scheduled on " + ScheduleDate.ToString("MMMM dd, yyyy") + ".";
                                    //    message = message.Replace("{{RescheduleDate}}",RequestedServiceDate.ToString("MMMM dd, yyyy"));
                                    //    message = message.Replace("{{NewServiceDate}}", ScheduleDate.ToString("MMMM dd, yyyy"));
                                    //}
                                    //else
                                    //{
                                    //    message = General.GetNotificationMessage("RequestedServiceScheduleSendToClient"); //"Your Requested Service is Scheduled on " + ScheduleDate.ToString("MMMM dd, yyyy") + ".";
                                    //    message = message.Replace("{{ScheduleDate}}", ScheduleDate.ToString("MMMM dd, yyyy"));
                                    //}

                                    if (PurposeOfVisit == General.PurposeOfVisit.Emergency.GetEnumDescription() ||
                                        PurposeOfVisit == General.PurposeOfVisit.ContinuingPreviousWork.GetEnumDescription()||
                                        PurposeOfVisit == General.PurposeOfVisit.Repairing.GetEnumDescription())
                                    {
                                        if (ScheduleDate != RequestedServiceDate)
                                        {
                                            message = General.GetNotificationMessage("RescheduleServiceOffDateSendToClient"); //"None of employee were available on " + RequestedServiceDate.ToLocalTime().ToString("MMMM dd, yyyy") + ".So service has been scheduled on " + ScheduleDate.ToLocalTime().ToString("MMMM dd, yyyy") + ".";
                                            message = message.Replace("{{RescheduleDate}}", RequestedServiceDate.ToString("MMMM dd, yyyy"));
                                            message = message.Replace("{{NewServiceDate}}", ScheduleDate.ToString("MMMM dd, yyyy"));
                                        }
                                        else
                                        {
                                            message = General.GetNotificationMessage("RequestedServiceScheduleSendToClient"); //"Your Requested Service is Scheduled on " + ScheduleDate.ToLocalTime().ToString("MMMM dd, yyyy") + ".";
                                            message = message.Replace("{{ScheduleDate}}", ScheduleDate.ToString("MMMM dd, yyyy"));
                                        }
                                        MessageType = General.NotificationType.ServiceScheduled.GetEnumDescription();
                                        NotificationType = General.NotificationType.ServiceScheduled.GetEnumValue();
                                    }
                                    else
                                    {
                                        message = General.GetNotificationMessage("RequestedServiceApprovelSendToClient");
                                        MessageType = General.NotificationType.ServiceApproval.GetEnumDescription();
                                        NotificationType = General.NotificationType.ServiceApproval.GetEnumValue();
                                    }

                                    objUserNotificationService = ServiceFactory.UserNotificationService;
                                    //NotificationType = General.NotificationType.ServiceScheduled.GetEnumValue();
                                    objUserNotification.MessageType = MessageType;//General.NotificationType.ServiceScheduled.GetEnumDescription();
                                    objUserNotification.UserId = ClientId;
                                    objUserNotification.UserTypeId = General.UserRoles.Client.GetEnumValue();
                                    objUserNotification.Message = message;
                                    objUserNotification.Status = General.NotificationStatus.UnRead.GetEnumDescription();
                                    objUserNotification.CommonId = ServiceId;
                                    objUserNotification.AddedDate = DateTime.UtcNow;

                                    NotificationId = objUserNotificationService.AddUserNotification(ref objUserNotification);

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

                                        //DataTable dtApprovalEmail = new DataTable();
                                        //objServicesService.SetServiceApprovalUrl(ServiceId,ref dtApprovalEmail);
                                        //if (dtApprovalEmail.Rows.Count > 0)
                                        //{
                                        //    string ApprovalUrl = Application["SiteAddress"] + "ServiceApproval.aspx?Url=" + dtApprovalEmail.Rows[0]["ApprovalEmailUrl"].ToString();
                                        //    string ClientEmail = dtApprovalEmail.Rows[0]["Email"].ToString();
                                        //    Email.SendEmail("Service Approval", ClientEmail, "", "", ApprovalUrl);
                                        //}
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
                                if (PurposeOfVisit == General.PurposeOfVisit.Emergency.GetEnumDescription() ||
                                        PurposeOfVisit == General.PurposeOfVisit.ContinuingPreviousWork.GetEnumDescription()||
                                    PurposeOfVisit == General.PurposeOfVisit.Repairing.GetEnumDescription())
                                {
                                    objEmployeeService = ServiceFactory.EmployeeService;
                                    DataTable dtEmployee = new DataTable();
                                    BizObjects.UserNotification objUserNotification1 = new BizObjects.UserNotification();
                                    objEmployeeService.GetEmployeeById(EmployeeId, ref dtEmployee);
                                    if (dtEmployee.Rows.Count > 0)
                                    {
                                        objUserNotification1.UserId = EmployeeId;
                                        objUserNotification1.UserTypeId = General.UserRoles.Employee.GetEnumValue();
                                        string Empmessage = General.GetNotificationMessage("EmployeeSchedule");
                                        Empmessage = Empmessage.Replace("{{ScheduleDate}}", ScheduleDate.ToString("MMMM dd, yyyy"));
                                        objUserNotification1.Message = Empmessage;//"System has scheduled a service for you on " + ScheduleDate.ToString("MMMM dd, yyyy") + ".";
                                        objUserNotification1.Status = General.NotificationStatus.UnRead.GetEnumDescription();
                                        objUserNotification1.CommonId = ServiceId;
                                        objUserNotification1.MessageType = General.NotificationType.ServiceScheduled.GetEnumDescription();
                                        objUserNotification1.AddedDate = DateTime.UtcNow;

                                        long NotificationId = objUserNotificationService.AddUserNotification(ref objUserNotification1);

                                        dtBadgeCount.Clear();
                                        objUserNotificationService.GetBadgeCount(EmployeeId, General.UserRoles.Employee.GetEnumValue(), ref dtBadgeCount);
                                        int BadgeCount = dtBadgeCount.Rows.Count;

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
                                                SendNotifications.SendIphoneNotification(BadgeCount, dtEmployee.Rows[0]["DeviceToken"].ToString(), objUserNotification1.Message, notify1, "employee");
                                            }
                                        }
                                    }
                                }
                                ltrMessage.Text = "Requested Services Schedule Successfully.";
                            }
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                ltrMessage.Text = Ex.Message.ToString().Trim();
            }
        }
    }
}