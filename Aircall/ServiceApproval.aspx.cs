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
    public partial class ServiceApproval : System.Web.UI.Page
    {
        IServicesService objServicesService;
        IRescheduleServicesService objRescheduleServicesService;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString.Count > 0)
                {
                    if (!string.IsNullOrEmpty(Request.QueryString["Url"]))
                    {
                        string Url = Request.QueryString["Url"].ToString();
                        DataTable dtService = new DataTable();
                        objServicesService = ServiceFactory.ServicesService;
                        objServicesService.CheckServiceApprovalUrl(Url, ref dtService);
                        if (dtService.Rows.Count > 0)
                        {
                            if (dtService.Rows[0]["Id"].ToString() != "-1")
                            {
                                var LateRescheduleHours = int.Parse(General.GetSitesettingsValue("LateRescheduleHours"));
                                LateRescheduleDisplayMessage.Value = General.GetSitesettingsValue("LateRescheduleDisplayMessage");
                                LateCancelDisplayMessage.Value = General.GetSitesettingsValue("LateCancelDisplayMessage");

                                Session["ServiceId"] = dtService.Rows[0]["Id"].ToString();
                                ltrServiceNo.Text = dtService.Rows[0]["ServiceCaseNumber"].ToString();
                                ltrAddress.Text = dtService.Rows[0]["Address"].ToString();
                                hdnDate.Value = Convert.ToDateTime(dtService.Rows[0]["ScheduleDate"].ToString()).ToString("MM/dd/yyyy") + " " + dtService.Rows[0]["ScheduleStartTime"].ToString();
                                var diff24Hourdate = DateTime.Parse(hdnDate.Value) - DateTime.Now;
                                is24Hrs.Value = (diff24Hourdate.TotalHours <= LateRescheduleHours).ToString();

                                ltrDate.Text = Convert.ToDateTime(dtService.Rows[0]["ScheduleDate"].ToString()).ToString("MMMM dd, yyyy");
                                ltrTime.Text = dtService.Rows[0]["ScheduleStartTime"].ToString() + " - " + DateTime.Parse(dtService.Rows[0]["ScheduleEndTime"].ToString()).AddHours(1).ToString("hh:mm tt");// dtService.Rows[0]["ScheduleEndTime"].ToString();
                                ltrUnits.Text = dtService.Rows[0]["ServiceUnits"].ToString();
                                hdnUnitCnt.Value = ltrUnits.Text.Split((",").ToArray()).Length.ToString();
                                drpPurposeOfVisit.Value = dtService.Rows[0]["PurposeOfVisit"].ToString();
                                ltrEmployee.Text = dtService.Rows[0]["EmployeeName"].ToString();
                                string[] TimeSlot = dtService.Rows[0]["TimeSlot"].ToString().Split('|');

                                hdnEmployeeId.Value = dtService.Rows[0]["EmployeeId"].ToString();
                                if (dtService.Rows[0]["IsRequestedService"].ToString().ToLower() == "false")
                                {
                                    btnCancel.Visible = false;
                                }
                                else
                                {
                                    btnCancel.Visible = true;
                                }
                                ltrSlot1.Text = TimeSlot[0];
                                ltrSlot2.Text = TimeSlot[1];
                                hdnTimeSlot.Value = dtService.Rows[0]["TimeSlot"].ToString();
                                List<string> slot1 = ltrSlot1.Text.Split(("-").ToArray()).ToList();
                                List<string> slot2 = ltrSlot2.Text.Split(("-").ToArray()).ToList();
                                Plan p = new Plan();
                                p.ServiceTimeForFirstUnit = int.Parse(dtService.Rows[0]["ServiceTimeForFirstUnit"].ToString());
                                p.ServiceTimeForAdditionalUnits = int.Parse(dtService.Rows[0]["ServiceTimeForAdditionalUnits"].ToString());
                                var lunchtime = General.GetSitesettingsValue("EmployeeLunchTime");
                                var Slot1cnt = General.Slottime1(slot1.ToArray(), slot2.ToArray(), p, lunchtime);
                                var Slot2cnt = General.Slottime1(new List<string>().ToArray(), slot2.ToArray(), p, lunchtime);
                                firstslotunits.Value = Slot1cnt.ToString();
                                secondslotunits.Value = Slot2cnt.ToString();
                                if (!string.IsNullOrEmpty(dtService.Rows[0]["Image"].ToString()))
                                    imgEmp.ImageUrl = Application["SiteAddress"] + "uploads/profile/employee/" + dtService.Rows[0]["Image"].ToString();
                                else
                                    imgEmp.ImageUrl = Application["SiteAddress"] + "uploads/profile/employee/defultimage.jpg";
                            }
                            else
                            {
                                dvService.Visible = false;
                                dvExpire.Visible = true;
                            }
                        }
                    }
                    else
                    {
                        dvService.Visible = false;
                        dvExpire.Visible = true;
                    }
                }
                else
                {
                    dvService.Visible = false;
                    dvExpire.Visible = true;
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Request.QueryString.Count > 0)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["Url"]))
                {
                    string Url = Request.QueryString["Url"].ToString();
                    objServicesService = ServiceFactory.ServicesService;

                    DataTable dtService = new DataTable();
                    objServicesService.CheckServiceApprovalUrl(Url, ref dtService);
                    if (dtService.Rows.Count > 0)
                    {
                        if (dtService.Rows[0]["Id"].ToString() == "-1")
                        {
                            dvService.Visible = false;
                            dvExpire.Visible = true;
                            return;
                        }
                    }
                    objServicesService.ApproveCancelServiceByUrl(Url, General.ServiceTypes.Scheduled.GetEnumDescription());

                    if (!string.IsNullOrEmpty(hdnEmployeeId.Value))
                    {
                        int EmployeeId = Convert.ToInt32(hdnEmployeeId.Value);
                        IEmployeeService objEmployeeService = ServiceFactory.EmployeeService;
                        IUserNotificationService objUserNotificationService = ServiceFactory.UserNotificationService;

                        DataTable dtEmployee = new DataTable();
                        objEmployeeService.GetEmployeeById(EmployeeId, ref dtEmployee);
                        if (dtEmployee.Rows.Count > 0)
                        {
                            long NotificationId = 0;
                            int BadgeCount = 0;
                            BizObjects.UserNotification objUserNotification = new BizObjects.UserNotification();
                            string message = General.GetNotificationMessage("EmployeeSchedule");

                            objUserNotificationService.DeleteNotificationByCommonIdType(Convert.ToInt16(Session["ServiceId"]), General.NotificationType.ServiceApproval.GetEnumDescription());

                            DataTable dt = new DataTable();
                            objServicesService.GetServiceById(Convert.ToInt16(Session["ServiceId"]), ref dt);
                            if (dt.Rows.Count > 0)
                            {
                                DataRow row;
                                row = dt.Rows[0];

                                message = message.Replace("{{ScheduleDate}}", DateTime.Parse(row["ScheduleDate"].ToString()).ToString("MMMM dd, yyyy"));
                                objUserNotification.UserId = EmployeeId;
                                objUserNotification.UserTypeId = General.UserRoles.Employee.GetEnumValue();
                                objUserNotification.Message = message;
                                objUserNotification.CommonId = Convert.ToInt64(Session["ServiceId"]);
                                objUserNotification.Status = General.NotificationStatus.UnRead.GetEnumDescription();
                                objUserNotification.MessageType = General.NotificationType.ServiceScheduled.GetEnumDescription();
                                objUserNotification.AddedDate = DateTime.UtcNow;

                                NotificationId = objUserNotificationService.AddUserNotification(ref objUserNotification);

                                DataTable dtBadgeCount = new DataTable();
                                objUserNotificationService.GetBadgeCount(EmployeeId, General.UserRoles.Employee.GetEnumValue(), ref dtBadgeCount);
                                BadgeCount = dtBadgeCount.Rows.Count;

                                Notifications objNotifications = new Notifications { NId = NotificationId, NType = General.NotificationType.ServiceScheduled.GetEnumValue(), CommonId = Convert.ToInt64(Session["ServiceId"]) };
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
                                Response.Redirect(Application["SiteAddress"].ToString(), false);
                            }
                        }
                    }

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "AlertMessage", "alert('Service Approved Successfully');window.location.href='https://aircallservices.com/';", true);
                    //ScriptManager.RegisterStartupScript(this, this.GetType(), "AlertMessage", "alert('Service Approved Successfully');window.location.href='" + Application["SiteAddress"].ToString() + "';", true);
                    //Response.Redirect(Application["SiteAddress"].ToString(), false);
                }
            }

        }

        protected void btnReschedule_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                if (Session["ServiceId"] != null &&
                    !string.IsNullOrEmpty(Request.QueryString["Url"]))
                {
                    string Url = Request.QueryString["Url"].ToString();
                    objServicesService = ServiceFactory.ServicesService;
                    DataTable dtService = new DataTable();
                    objServicesService.CheckServiceApprovalUrl(Url, ref dtService);
                    if (dtService.Rows.Count > 0)
                    {
                        if (dtService.Rows[0]["Id"].ToString() != "-1")
                        {
                            dvService.Visible = false;
                            dvExpire.Visible = true;
                            return;
                        }
                    }
                    BizObjects.RescheduleService objReschedule = new BizObjects.RescheduleService();
                    objReschedule.ServiceId = Convert.ToInt64(Session["ServiceId"].ToString());
                    objReschedule.RescheduleDate = Convert.ToDateTime(txtReschedule.Text.Trim());
                    string[] TimeSlot = hdnTimeSlot.Value.Split('|');
                    if (rdslot1.Checked)
                        objReschedule.Rescheduletime = TimeSlot[0].ToString();
                    else
                        objReschedule.Rescheduletime = TimeSlot[1].ToString();
                    objReschedule.Reason = txtReason.Text.Trim();
                    objReschedule.AddedByType = General.UserRoles.Client.GetEnumValue();
                    objReschedule.AddedDate = DateTime.UtcNow;

                    objRescheduleServicesService = ServiceFactory.RescheduleServicesService;
                    objRescheduleServicesService.AddRescheduleService(ref objReschedule, General.ServiceTypes.Rescheduled.GetEnumDescription());


                    objServicesService.ApproveCancelServiceByUrl(Url, General.ServiceTypes.Rescheduled.GetEnumDescription());

                    Session.Remove("ServiceId");

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "AlertMessage", "alert('Service Rescheduled Successfully');window.location.href='" + Application["SiteAddress"].ToString() + "';", true);
                    //Response.Redirect(Application["SiteAddress"].ToString(), false);
                }
            }
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

                //LoginModel objLoginModel = new LoginModel();
                //objLoginModel = Session["LoginSession"] as LoginModel;

                BizObjects.RescheduleService objRescheduleService = new BizObjects.RescheduleService();
                objRescheduleServicesService = ServiceFactory.RescheduleServicesService;

                objRescheduleService.ServiceId = ServiceId;
                objRescheduleService.RescheduleDate = DateTime.UtcNow;
                objRescheduleService.Rescheduletime = string.Empty;

                objRescheduleService.Reason = "";
                objRescheduleService.AddedBy = ClientId;
                objRescheduleService.AddedByType = General.UserRoles.Client.GetEnumValue();
                objRescheduleService.AddedDate = DateTime.UtcNow;

                var added = objRescheduleServicesService.AddRescheduleService(ref objRescheduleService, General.ServiceTypes.Cancelled.GetEnumDescription());

                //Delete Notification
                var objUserNotificationService = ServiceFactory.UserNotificationService;
                objUserNotificationService.DeleNotification(ServiceId);

                //Send Notification to Employee
                if (EmployeeId != 0)
                {
                    var objEmployeeService = ServiceFactory.EmployeeService;
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
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "AlertMessage", "alert('Service has been Cancelled successfully');window.location.href='" + Application["SiteAddress"].ToString() + "';", true);
                }
            }
        }
    }
}