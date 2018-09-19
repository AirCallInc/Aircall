using Aircall.Common;
using Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Aircall.client
{
    public partial class ServiceApprove : System.Web.UI.Page
    {
        IServicesService objServicesService;
        IServiceUnitService objServiceUnitService = ServiceFactory.ServiceUnitService;
        IUserNotificationService objUserNotificationService = ServiceFactory.UserNotificationService;
        IEmployeeService objEmployeeService = ServiceFactory.EmployeeService;
        protected void Page_Load(object sender, EventArgs e)
        {
            objServicesService = ServiceFactory.ServicesService;
            if (Session["ClientLoginCookie"] != null)
            {
                if (Request.QueryString["Id"] != null)
                {
                    var LateRescheduleHours = int.Parse(General.GetSitesettingsValue("LateRescheduleHours"));
                    LateRescheduleDisplayMessage.Value = General.GetSitesettingsValue("LateRescheduleDisplayMessage");
                    LateCancelDisplayMessage.Value = General.GetSitesettingsValue("LateCancelDisplayMessage");

                    var ClientId = (Session["ClientLoginCookie"] as LoginModel).Id;
                    int ServiceId;//= int.Parse(Request.QueryString["Id"].ToString());
                    if (!int.TryParse(Request.QueryString["Id"], out ServiceId))
                    {
                        Response.Redirect("dashboard.aspx", false);
                    }
                    DataTable dt = new DataTable();
                    objServicesService.GetServiceForClientById(ServiceId, (Session["ClientLoginCookie"] as LoginModel).Id, ref dt);

                    if (dt.Rows.Count > 0)
                    {
                        objUserNotificationService.UpdateStatusByClientIdNotificationIdMessageType(ClientId, ServiceId, General.NotificationType.ServiceApproval.GetEnumDescription());
                        DataRow row;
                        row = dt.Rows[0];
                        if (row["Status"].ToString() != "Waiting Approval")
                        {
                            Response.Redirect(Application["SiteAddress"] + "client/dashboard.aspx", false);
                            return;
                        }
                        if (dt.Rows[0]["IsRequestedService"].ToString().ToLower() == "false")
                        {
                            btnCancel.Visible = false;
                        }
                        else
                        {
                            btnCancel.Visible = true;
                        }
                        ltrAddress.Text = row["Address"].ToString();
                        ltrServiceCase.Text = row["ServiceCaseNumber"].ToString();
                        ltrEmployee.Text = row["EmployeeName"].ToString();
                        hdnEmployeeId.Value = row["EmployeeId"].ToString();
                        hdnDate.Value = Convert.ToDateTime(row["ScheduleDate"].ToString()).ToString("MM/dd/yyyy") + " " + row["ScheduleStartTime"].ToString();
                        var diff24Hourdate = DateTime.Parse(hdnDate.Value) - DateTime.Now;
                        is24Hrs.Value = (diff24Hourdate.TotalHours <= LateRescheduleHours).ToString();

                        ltrDate.Text = DateTime.Parse(row["ScheduleDate"].ToString()).ToString("MMMM dd, yyyy");
                        ltrTime.Text = row["ScheduleStartTime"].ToString() + " - " + DateTime.Parse(row["ScheduleEndTime"].ToString()).AddHours(1).ToString("hh:mm tt");
                        DataTable dtU = new DataTable();
                        objServiceUnitService.GetServiceUnitsForPortal(ServiceId, ref dtU);
                        ltrUnit.Text = dtU.Rows[0]["ServiceUnits"].ToString();
                        ltrComplaint.Text = (string.IsNullOrWhiteSpace(row["CustomerComplaints"].ToString()) ? "No Complaint" : row["CustomerComplaints"].ToString());
                    }
                    else
                    {
                        Response.Redirect("dashboard.aspx", false);
                    }
                }
                else
                {
                    Response.Redirect("dashboard.aspx", false);
                }
            }
            else
                Response.Redirect(Application["SiteAddress"] + "sign-in.aspx", false);
        }

        protected void btnApprove_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["nid"] != null)
            {
                try
                {
                    int NotificationId = Convert.ToInt16(Request.QueryString["nid"].ToString());
                    objUserNotificationService.DeleteNotificationById(NotificationId);
                }
                catch (Exception ex)
                {
                }
            }
            var ClientId = (Session["ClientLoginCookie"] as LoginModel).Id;
            var ServiceId = int.Parse(Request.QueryString["Id"].ToString());
            BizObjects.Services objServices = new BizObjects.Services();
            objServices.Id = ServiceId;
            objServices.Status = General.ServiceTypes.Scheduled.GetEnumDescription();
            objServices.UpdatedBy = ClientId;
            objServices.UpdatedByType = General.UserRoles.Client.GetEnumValue();
            objServices.UpdatedDate = DateTime.Now;

            objServicesService.UpdateServiceStatus(ref objServices);

            if (!string.IsNullOrEmpty(hdnEmployeeId.Value))
            {
                int EmployeeId = Convert.ToInt32(hdnEmployeeId.Value);
                objEmployeeService = ServiceFactory.EmployeeService;
                DataTable dtEmployee = new DataTable();
                objEmployeeService.GetEmployeeById(EmployeeId, ref dtEmployee);
                if (dtEmployee.Rows.Count > 0)
                {
                    long NotificationId = 0;
                    int BadgeCount = 0;
                    BizObjects.UserNotification objUserNotification = new BizObjects.UserNotification();
                    string message = General.GetNotificationMessage("EmployeeSchedule");

                    objUserNotificationService.DeleteNotificationByCommonIdType(ServiceId, General.NotificationType.ServiceApproval.GetEnumDescription());

                    DataTable dt = new DataTable();
                    objServicesService.GetServiceForClientById(ServiceId, (Session["ClientLoginCookie"] as LoginModel).Id, ref dt);
                    if (dt.Rows.Count > 0)
                    {
                        DataRow row;
                        row = dt.Rows[0];

                        message = message.Replace("{{ScheduleDate}}", DateTime.Parse(row["ScheduleDate"].ToString()).ToString("MMMM dd, yyyy"));
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
                    else
                    {
                        Response.Redirect(Application["SiteAddress"] + "client/dashboard.aspx", false);
                    }
                }
            }

            Response.Redirect(Application["SiteAddress"] + "client/dashboard.aspx", false);
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            var objRescheduleServicesService = ServiceFactory.RescheduleServicesService;
            if (!string.IsNullOrEmpty(Request.QueryString["Id"]))
            {
                long ServiceId = Convert.ToInt64(Request.QueryString["Id"].ToString());
                int EmployeeId = 0;
                int ClientId = 0;
                string ScheduleDate = string.Empty;
                string ClientName = string.Empty;
                objServicesService = ServiceFactory.ServicesService;
                DataTable dtServices = new DataTable();
                objServicesService.GetServiceById(ServiceId, ref dtServices);

                if (dtServices.Rows.Count > 0)
                {
                    EmployeeId = Convert.ToInt32(dtServices.Rows[0]["EmployeeId"].ToString());
                    ClientId = Convert.ToInt32(dtServices.Rows[0]["ClientId"].ToString());
                    ClientName = dtServices.Rows[0]["ClientName"].ToString();
                    ScheduleDate = dtServices.Rows[0]["ScheduleDate"].ToString();
                }
                //objServicesService.UpdateServiceStatus(ServiceId, General.ServiceTypes.Cancelled.GetEnumDescription());

                LoginModel objLoginModel = new LoginModel();
                objLoginModel = Session["ClientLoginCookie"] as LoginModel;

                BizObjects.RescheduleService objRescheduleService = new BizObjects.RescheduleService();
                objRescheduleServicesService = ServiceFactory.RescheduleServicesService;

                objRescheduleService.ServiceId = ServiceId;
                objRescheduleService.RescheduleDate = DateTime.UtcNow;
                objRescheduleService.Rescheduletime = string.Empty;

                objRescheduleService.Reason = "";
                objRescheduleService.AddedBy = objLoginModel.Id;
                objRescheduleService.AddedByType = objLoginModel.RoleId;
                objRescheduleService.AddedDate = DateTime.UtcNow;

                var added = objRescheduleServicesService.AddRescheduleService(ref objRescheduleService, General.ServiceTypes.Cancelled.GetEnumDescription());

                //Delete Notification
                objUserNotificationService = ServiceFactory.UserNotificationService;
                objUserNotificationService.DeleNotification(ServiceId);

                //Send Notification to Employee
                if (EmployeeId != 0)
                {
                    objEmployeeService = ServiceFactory.EmployeeService;
                    DataTable dtEmployee = new DataTable();
                    objEmployeeService.GetEmployeeById(EmployeeId, ref dtEmployee);
                    if (dtEmployee.Rows.Count > 0)
                    {
                        long NotificationId = 0;
                        int BadgeCount = 0;
                        BizObjects.UserNotification objUserNotification = new BizObjects.UserNotification();
                        string message = General.GetNotificationMessage("CancelledServiceSendToEmployee"); //"Your Service at " + ClientName + "’s address on " + Convert.ToDateTime(txtScheduleOn.Text.Trim()).ToString("MMMM dd, yyyy") + " has been rescheduled.";
                        message = message.Replace("{{ClientName}}", ClientName);
                        message = message.Replace("{{ScheduleDate}}", Convert.ToDateTime(ScheduleDate).ToString("MMMM dd, yyyy"));
                        objUserNotification.UserId = EmployeeId;
                        objUserNotification.UserTypeId = General.UserRoles.Employee.GetEnumValue();
                        objUserNotification.Message = message;
                        objUserNotification.CommonId = ServiceId;
                        objUserNotification.Status = General.NotificationStatus.UnRead.GetEnumDescription();
                        objUserNotification.MessageType = General.NotificationType.FriendlyReminder.GetEnumDescription();
                        objUserNotification.AddedDate = DateTime.UtcNow;

                        NotificationId = objUserNotificationService.AddUserNotification(ref objUserNotification);

                        DataTable dtBadgeCount = new DataTable();
                        objUserNotificationService.GetBadgeCount(EmployeeId, General.UserRoles.Employee.GetEnumValue(), ref dtBadgeCount);
                        BadgeCount = dtBadgeCount.Rows.Count;

                        Notifications objNotifications = new Notifications { NId = NotificationId, NType = General.NotificationType.FriendlyReminder.GetEnumValue(), CommonId = ServiceId };
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
                if (added == 0)
                {
                    dvMessage.InnerHtml = "<strong>Service has been Cancelled successfully.</strong>";
                    dvMessage.Attributes.Add("class", "error");
                    dvMessage.Visible = true;
                    btnReschedule.Visible = btnCancel.Visible = btnApprove.Visible = false;
                }
            }
        }

        protected void btnReschedule_Click(object sender, EventArgs e)
        {
            var ServiceId = int.Parse(Request.QueryString["Id"].ToString());
            Response.Redirect(Application["SiteAddress"] + "client/reschedule.aspx?Id=" + Request.QueryString["Id"].ToString(), false);
        }
    }
}