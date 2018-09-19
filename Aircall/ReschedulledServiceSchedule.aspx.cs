using Aircall.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Services;

namespace Aircall
{
    public partial class ReschedulledServiceSchedule : System.Web.UI.Page
    {
        IServicesService objServicesService;
        IClientService objClientService;
        IEmployeeService objEmployeeService;
        IRequestServicesService objRequestServicesService;
        IUserNotificationService objUserNotificationService;
        IEmailTemplateService objEmailTemplateService;
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
                    objEmployeeService = ServiceFactory.EmployeeService;
                    DataTable dtReschedulledService = new DataTable();
                    objServicesService.GetCancelledAndReschedulledServices(ref dtReschedulledService);
                    if (dtReschedulledService.Rows.Count > 0)
                    {
                        DataTable dtService = new DataTable();
                        for (int i = 0; i < dtReschedulledService.Rows.Count; i++)
                        {
                            long ServiceId = Convert.ToInt64(dtReschedulledService.Rows[i]["Id"].ToString());
                            int ClientId = Convert.ToInt32(dtReschedulledService.Rows[i]["ClientId"].ToString());
                            int AddressId = Convert.ToInt32(dtReschedulledService.Rows[i]["AddressID"].ToString());
                            string PurposeOfVisit = dtReschedulledService.Rows[i]["PurposeOfVisit"].ToString();
                            DateTime RescheduleDate = Convert.ToDateTime(dtReschedulledService.Rows[i]["RescheduleDate"].ToString());
                            string Rescheduletime = dtReschedulledService.Rows[i]["Rescheduletime"].ToString();

                            dtService.Clear();
                            objRequestServicesService = ServiceFactory.RequestServicesService;
                            //objServicesService.RescheduledServiceSchedule(ServiceId, ClientId, AddressId, ref dtService);
                            objRequestServicesService.ScheduleRequestedService(ServiceId, 0, ClientId, AddressId, PurposeOfVisit, RescheduleDate, Rescheduletime, ref dtService);
                            if (dtService.Rows.Count > 0)
                            {
                                if (Convert.ToInt32(dtService.Rows[0]["EmployeeId"].ToString()) != 0
                                    && Convert.ToInt32(dtService.Rows[0]["ServiceId"].ToString()) != 0)
                                {
                                    int EmployeeId = Convert.ToInt32(dtService.Rows[0]["EmployeeId"].ToString());
                                    DateTime ScheduleDate = Convert.ToDateTime(dtService.Rows[0]["ScheduleDate"].ToString());
                                    //send notification to client
                                    DataTable dtClient = new DataTable();
                                    objClientService.GetClientById(ClientId, ref dtClient);
                                    if (dtClient.Rows.Count > 0)
                                    {
                                        long NotificationId = 0;
                                        int BadgeCount = 0;
                                        BizObjects.UserNotification objUserNotification = new BizObjects.UserNotification();
                                        string message = string.Empty;
                                        string MessageType = string.Empty;
                                        int NotificationType;

                                        if (PurposeOfVisit == General.PurposeOfVisit.Emergency.GetEnumDescription() ||
                                            PurposeOfVisit == General.PurposeOfVisit.ContinuingPreviousWork.GetEnumDescription() ||
                                            PurposeOfVisit == General.PurposeOfVisit.Repairing.GetEnumDescription())
                                        {

                                            message = General.GetNotificationMessage("RequestedServiceScheduleSendToClient"); //"Your Requested Service is Scheduled on " + ScheduleDate.ToLocalTime().ToString("MMMM dd, yyyy") + ".";
                                            message = message.Replace("{{ScheduleDate}}", ScheduleDate.ToString("MMMM dd, yyyy"));
                                            MessageType = General.NotificationType.ServiceScheduled.GetEnumDescription();
                                            NotificationType = General.NotificationType.ServiceScheduled.GetEnumValue();
                                        }
                                        else
                                        {
                                            message = General.GetNotificationMessage("ServiceApprovelSendToClient");
                                            MessageType = General.NotificationType.ServiceApproval.GetEnumDescription();
                                            NotificationType = General.NotificationType.ServiceApproval.GetEnumValue();
                                        }

                                        objUserNotificationService = ServiceFactory.UserNotificationService;
                                        objUserNotification.UserId = ClientId;
                                        objUserNotification.UserTypeId = General.UserRoles.Client.GetEnumValue();
                                        objUserNotification.Message = message;
                                        objUserNotification.Status = General.NotificationStatus.UnRead.GetEnumDescription();
                                        objUserNotification.CommonId = ServiceId;
                                        objUserNotification.MessageType = MessageType;//General.NotificationType.ServiceApproval.GetEnumDescription(); //General.NotificationType.ServiceScheduled.GetEnumDescription();
                                        objUserNotification.AddedDate = DateTime.UtcNow;

                                        NotificationId = objUserNotificationService.AddUserNotification(ref objUserNotification);

                                        DataTable dtBadgeCount = new DataTable();
                                        dtBadgeCount.Clear();
                                        objUserNotificationService.GetBadgeCount(ClientId, General.UserRoles.Client.GetEnumValue(), ref dtBadgeCount);
                                        BadgeCount = dtBadgeCount.Rows.Count;

                                        //Notifications objNotifications = new Notifications { NId = NotificationId, NType = General.NotificationType.ServiceScheduled.GetEnumValue(), CommonId = ServiceId };
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

                                    //send notification to employee
                                    if (PurposeOfVisit == General.PurposeOfVisit.Emergency.GetEnumDescription() ||
                                            PurposeOfVisit == General.PurposeOfVisit.ContinuingPreviousWork.GetEnumDescription() ||
                                        PurposeOfVisit==General.PurposeOfVisit.Repairing.GetEnumDescription())
                                    {
                                        DataTable dtEmployee = new DataTable();
                                        objEmployeeService.GetEmployeeById(EmployeeId, ref dtEmployee);
                                        if (dtEmployee.Rows.Count > 0)
                                        {
                                            long NotificationId = 0;
                                            int BadgeCount = 0;
                                            BizObjects.UserNotification objUserNotification = new BizObjects.UserNotification();
                                            objUserNotificationService = ServiceFactory.UserNotificationService;
                                            string message = General.GetNotificationMessage("EmployeeSchedule"); //"System has scheduled a service for you on " + Convert.ToDateTime(dtService.Rows[0]["ScheduleDate"].ToString()).ToString("MMMM dd, yyyy") + ".";
                                            message = message.Replace("{{ScheduleDate}}", ScheduleDate.ToString("MMMM dd, yyyy"));
                                            objUserNotification.UserId = EmployeeId;
                                            objUserNotification.UserTypeId = General.UserRoles.Employee.GetEnumValue();
                                            objUserNotification.Message = message;
                                            objUserNotification.CommonId = ServiceId;
                                            objUserNotification.Status = General.NotificationStatus.UnRead.GetEnumDescription();
                                            objUserNotification.MessageType = General.NotificationType.ServiceScheduled.GetEnumDescription();
                                            objUserNotification.AddedDate = DateTime.UtcNow;

                                            NotificationId = objUserNotificationService.AddUserNotification(ref objUserNotification);

                                            DataTable dtBadgeCount = new DataTable();
                                            objUserNotificationService.GetBadgeCount(EmployeeId, General.UserRoles.Employee.GetEnumValue(), ref dtBadgeCount);
                                            BadgeCount = dtBadgeCount.Rows.Count;

                                            Notifications objNotifications = new Notifications { NId = NotificationId, NType = General.NotificationType.ServiceScheduled.GetEnumValue(), CommonId = ServiceId };
                                            List<NotificationModel> notify = new List<NotificationModel>();
                                            notify.Add(new NotificationModel { Key = "NId", Value = new object[] { objNotifications.NId } });
                                            notify.Add(new NotificationModel { Key = "NType", Value = new object[] { objNotifications.NType } });
                                            notify.Add(new NotificationModel { Key = "CommonId", Value = new object[] { objNotifications.CommonId } });

                                            if (!string.IsNullOrEmpty(dtEmployee.Rows[0]["DeviceType"].ToString()) &&
                                                    !string.IsNullOrEmpty(dtEmployee.Rows[0]["DeviceToken"].ToString()) &&
                                                     dtEmployee.Rows[0]["DeviceToken"].ToString().ToLower() != "no device token")
                                            {
                                                if (dtEmployee.Rows[0]["DeviceType"].ToString().ToLower() == "iphone")
                                                {
                                                    SendNotifications.SendIphoneNotification(BadgeCount, dtEmployee.Rows[0]["DeviceToken"].ToString(), message, notify, "employee");
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    ltrMessage.Text = "Automatic Schedule is Turned Off. You have to Schedule Service manually";
                }
            }
            catch (Exception Ex)
            {
                ltrMessage.Text = Ex.Message.ToString().Trim();
            }
        }
    }
}