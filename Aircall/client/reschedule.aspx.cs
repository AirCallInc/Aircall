using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Services;
using System.Data;
using Aircall.Common;
using System.IO;
using System.Configuration;

namespace Aircall.client
{
    public partial class reschedule : System.Web.UI.Page
    {
        #region "Declaration"
        IServicesService objServicesService = ServiceFactory.ServicesService;
        IServiceUnitService objServiceUnitService = ServiceFactory.ServiceUnitService;
        IPlanService objPlanService = ServiceFactory.PlanService;
        IRescheduleServicesService objRescheduleServicesService = ServiceFactory.RescheduleServicesService;
        IUserNotificationService objUserNotificationService = ServiceFactory.UserNotificationService;
        IEmployeeService objEmployeeService = ServiceFactory.EmployeeService;
        IClientService objClientService = ServiceFactory.ClientService;
        IRequestServicesService objRequestServicesService = ServiceFactory.RequestServicesService;
        #endregion

        #region "Page Events"
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["ClientLoginCookie"] != null)
            {

                if (Request.QueryString["Id"] != null)
                {
                    int ServiceId;
                    if (!int.TryParse(Request.QueryString["Id"], out ServiceId))
                    {
                        Response.Redirect("schedule.aspx", false);
                    }
                    if (!IsPostBack)
                    {
                        dvCancel.InnerHtml = "";
                        dvCancel.Visible = false;
                        //BindTimeSlot();
                        BindServiceInfo();

                    }
                }
                else
                {
                    Response.Redirect("dashboard.aspx", false);
                }
                if (Request.QueryString["cc"] != null)
                {
                    ltrReson.Text = "Reason For Cancel";
                    ltrHeading.Text = "Request For Cancel";
                }
                else
                {
                    ltrHeading.Text = "Reschedule Service";
                    ltrReson.Text = "Reason For Reschedule";
                }

            }
            else
                Response.Redirect(Application["SiteAddress"] + "sign-in.aspx", false);
        }
        #endregion

        #region "Functions"
        //private void BindTimeSlot()
        //{
        //    DataTable dtTimeSlot = new DataTable();
        //    objPlanService.GetTimeSlot(0, ref dtTimeSlot);
        //    if (dtTimeSlot.Rows.Count > 0)
        //    {
        //        ltrSlot1.Text = dtTimeSlot.Rows[0]["ServiceSlot1"].ToString();
        //        ltrSlot2.Text = dtTimeSlot.Rows[0]["ServiceSlot2"].ToString();
        //        hdnTimeSlot.Value = dtTimeSlot.Rows[0]["ServiceSlot1"].ToString() + "|" + dtTimeSlot.Rows[0]["ServiceSlot2"].ToString();
        //    }
        //}

        private void BindServiceInfo()
        {
            long ServiceId = Convert.ToInt64(Request.QueryString["Id"].ToString());
            DataTable dtService = new DataTable();
            objServicesService.GetServiceById(ServiceId, (Session["ClientLoginCookie"] as LoginModel).Id, ref dtService);
            if (dtService.Rows.Count > 0)
            {
                ltrCaseNo.Text = dtService.Rows[0]["ServiceCaseNumber"].ToString();
                drpPurposeOfVisit.Value = ltrServiceType.Text = dtService.Rows[0]["PurposeOfVisit"].ToString();

                if (string.IsNullOrEmpty(dtService.Rows[0]["EmployeeName"].ToString()) ||
                    string.IsNullOrEmpty(dtService.Rows[0]["ScheduleDate"].ToString()) ||
                    string.IsNullOrEmpty(dtService.Rows[0]["ScheduleStartTime"].ToString()))
                {
                    Response.Redirect(Application["SiteAddress"] + "client/dashboard.aspx", false);
                    return;
                }
                
                ltrEmployee.Text = dtService.Rows[0]["EmployeeName"].ToString();

                hdnHourDiff.Value = dtService.Rows[0]["HourDifference"].ToString();
                int LateRescheduleHours = int.Parse(General.GetSitesettingsValue("LateRescheduleHours"));
                if (int.Parse(hdnHourDiff.Value) <= LateRescheduleHours)
                {
                    dvR.Visible = false;
                    dvRSD.Visible = false;
                    dvST.Visible = false;
                    dvSTE.Visible = false;
                }
                string ImgPath = string.Empty;
                if (string.IsNullOrEmpty(dtService.Rows[0]["EmployeeImage"].ToString()))
                    ImgPath = Application["SiteAddress"] + "uploads/profile/employee/defultimage.jpg";
                else
                    ImgPath = Application["SiteAddress"] + "uploads/profile/employee/" + dtService.Rows[0]["EmployeeImage"].ToString();

                var path = ConfigurationManager.AppSettings["EMPProfilePath"].ToString() + dtService.Rows[0]["EmployeeImage"].ToString();
                path = Server.MapPath(path);
                if (File.Exists(path))
                    ImgPath = Application["SiteAddress"] + "uploads/profile/employee/" + dtService.Rows[0]["EmployeeImage"].ToString();
                else
                    ImgPath = Application["SiteAddress"] + "uploads/profile/employee/defultimage.jpg";

                imgEmployee.Src = ImgPath;
                hdnEmployeeId.Value = dtService.Rows[0]["EmployeeId"].ToString();
                ltrCustomerComplaint.Text = dtService.Rows[0]["CustomerComplaints"].ToString();
                ltrServiceDate.Text = DateTime.Parse(dtService.Rows[0]["ScheduleDate"].ToString()).ToString("MM/dd/yyyy") + " " + dtService.Rows[0]["ScheduleStartTime"].ToString();

                DataTable dtUnits = new DataTable();
                objServiceUnitService.GetServiceUnitsForPortal(ServiceId, ref dtUnits);
                ltrUnits.Text = dtUnits.Rows[0]["ServiceUnits"].ToString();
                hdnUnitCnt.Value = ltrUnits.Text.Split((",").ToArray()).Length.ToString();
                BindTimeSlotAndUnitByPlanId(Convert.ToInt32(dtService.Rows[0]["AddressID"].ToString()), dtUnits.Rows[0]["PlanType"].ToString());
                if (drpPurposeOfVisit.Value == General.PurposeOfVisit.Emergency.GetEnumDescription())
                {
                    dvSTE.Attributes.Add("style", "display:block");
                    dvST.Attributes.Add("style", "display:none");
                }
                else
                {
                    dvST.Attributes.Add("style", "display:block");
                    dvSTE.Attributes.Add("style", "display:none");
                }
                if (Request.QueryString["cc"] != null)
                {
                    btnReschedule.Text = "Submit";
                }
                else
                {
                    btnReschedule.Text = "Reschedule";
                }
            }
            else
            {
                Response.Redirect(Application["SiteAddress"] + "/client/schedule.aspx", false);
            }
        }

        private void BindTimeSlotAndUnitByPlanId(int AddressId, string PlanId)
        {
            var EmergencyServiceSlot1 = General.GetSitesettingsValue("EmergencyServiceSlot1");
            var EmergencyServiceSlot2 = General.GetSitesettingsValue("EmergencyServiceSlot2");

            DataTable dtPlan = new DataTable();
            objPlanService = ServiceFactory.PlanService;
            objPlanService.GetPlanByAddressIdForRecheduled(AddressId, ref dtPlan);
            if (dtPlan.Rows.Count > 0)
            {
                var rows = dtPlan.Select(" Id = " + PlanId.ToString() + " ");
                if (rows.Length > 0)
                {
                    ltrSlot1.Text = rows[0]["ServiceSlot1"].ToString();
                    ltrSlot2.Text = rows[0]["ServiceSlot2"].ToString();

                    hdnTimeSlot.Value = rows[0]["ServiceSlot1"].ToString() + "|" + rows[0]["ServiceSlot2"].ToString();

                    ltrSlot1E.Text = EmergencyServiceSlot1;
                    ltrSlot2E.Text = EmergencyServiceSlot2;

                    hdnTimeSlotE.Value = EmergencyServiceSlot1 + "|" + EmergencyServiceSlot2;

                    List<string> slot1 = ltrSlot1.Text.Split(("-").ToArray()).ToList();
                    List<string> slot2 = ltrSlot2.Text.Split(("-").ToArray()).ToList();
                    Plan p = new Plan();
                    p.ServiceTimeForFirstUnit = int.Parse(rows[0]["ServiceTimeForFirstUnit"].ToString());
                    p.ServiceTimeForAdditionalUnits = int.Parse(rows[0]["ServiceTimeForAdditionalUnits"].ToString());
                    var lunchtime = General.GetSitesettingsValue("EmployeeLunchTime");
                    var Slot1cnt = General.Slottime1(slot1.ToArray(), slot2.ToArray(), p, lunchtime);
                    var Slot2cnt = General.Slottime1(new List<string>().ToArray(), slot2.ToArray(), p, lunchtime);
                    firstslotunits.Value = Slot1cnt.ToString();
                    secondslotunits.Value = Slot2cnt.ToString();
                }
            }
        }
        #endregion

        #region "Events"
        protected void btnReschedule_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    bool IsLateReschedule = false;
                    string ClientDeviceType = string.Empty;
                    string ClientDeviceToken = string.Empty;
                    string EmpDeviceType = string.Empty;
                    string EmpDeviceToken = string.Empty;

                    if (Request.QueryString["Id"] != null)
                    {
                        string[] Slot = hdnTimeSlot.Value.Split(("|").ToArray());
                        string[] SlotE = hdnTimeSlotE.Value.Split(("|").ToArray());
                        string selectedSlot = "";

                        BizObjects.RescheduleService objRescheduleService = new BizObjects.RescheduleService();
                        long ServiceId = Convert.ToInt64(Request.QueryString["Id"].ToString());
                        DataTable dtService = new DataTable();
                        objServicesService.GetServiceById(ServiceId, (Session["ClientLoginCookie"] as LoginModel).Id, ref dtService);

                        objUserNotificationService.DeleNotification(ServiceId);
                        objRescheduleService.ServiceId = ServiceId;
                        var LateRescheduleHours = Convert.ToInt32(Aircall.Common.General.GetSitesettingsValue("LateRescheduleHours"));
                        var HourDiff = Convert.ToInt32(hdnHourDiff.Value);
                        if (HourDiff <= LateRescheduleHours)
                        {
                            IsLateReschedule = true;
                        }
                        objRescheduleService.RescheduleDate = (string.IsNullOrWhiteSpace(txtReschedule.Value.Trim()) ? DateTime.Now : Convert.ToDateTime(txtReschedule.Value.Trim()));
                        DateTime d = objRescheduleService.RescheduleDate;
                        if (drpPurposeOfVisit.Value != General.PurposeOfVisit.Emergency.GetEnumDescription())
                        {
                            if (rdslot1.Checked)
                            {
                                selectedSlot = Slot[0];
                            }
                            else
                            {
                                selectedSlot = Slot[1];
                            }
                        }
                        else
                        {
                            if (d.DayOfWeek == DayOfWeek.Saturday || d.DayOfWeek == DayOfWeek.Sunday)
                            {
                                if (rdslot1.Checked)
                                {
                                    selectedSlot = Slot[0];
                                }
                                else
                                {
                                    selectedSlot = Slot[1];
                                }
                            }
                            else
                            {
                                if (rdslot1E.Checked)
                                {
                                    selectedSlot = SlotE[0];
                                }
                                else
                                {
                                    selectedSlot = SlotE[1];
                                }
                            }

                        }

                        if (d == DateTime.Now.Date && IsLateReschedule == false)
                        {
                            if (!General.CheckTimeValidation(selectedSlot, 1))
                            {
                                dvMessage.InnerHtml = "<strong>Please Select Future time.</strong>";
                                dvMessage.Attributes.Add("class", "error");
                                dvMessage.Visible = true;
                                return;
                            }
                        }
                        objRescheduleService.Rescheduletime = selectedSlot;

                        objRescheduleService.Reason = txtReason.Text.Trim();
                        objRescheduleService.AddedBy = (Session["ClientLoginCookie"] as LoginModel).Id;
                        objRescheduleService.AddedByType = General.UserRoles.Client.GetEnumValue();
                        objRescheduleService.AddedDate = DateTime.UtcNow;
                        if (Request.QueryString["cc"] != null)
                        {
                            objRescheduleServicesService.AddRescheduleService(ref objRescheduleService, General.ServiceTypes.Cancelled.GetEnumDescription());
                        }
                        else
                        {
                            objRescheduleServicesService.AddRescheduleService(ref objRescheduleService, General.ServiceTypes.Rescheduled.GetEnumDescription());
                        }

                        string EmpName = "";
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


                                EmpDeviceType = dtEmployee.Rows[0]["DeviceType"].ToString();
                                EmpDeviceToken = dtEmployee.Rows[0]["DeviceToken"].ToString();
                                EmpName = dtEmployee.Rows[0]["EmployeeName"].ToString();
                                string message = General.GetNotificationMessage("RescheduleServiceSendToEmployee");
                                LoginModel login = Session["ClientLoginCookie"] as LoginModel;
                                message = message.Replace("{{ClientName}}", login.FullName);
                                message = message.Replace("{{ScheduleDate}}", Convert.ToDateTime(dtService.Rows[0]["ScheduleDate"].ToString()).ToString("MMMM dd, yyyy"));

                                NotifyUser(EmployeeId, "employee", General.NotificationType.FriendlyReminder.GetEnumDescription(),
                                        General.NotificationType.FriendlyReminder.GetEnumValue(), ServiceId, message, EmpDeviceType, EmpDeviceToken);
                            }
                        }
                        //DataTable dtClient = new DataTable();
                        //objClientService.GetClientById((Session["ClientLoginCookie"] as LoginModel).Id, ref dtClient);
                        //if (dtClient.Rows.Count > 0)
                        //{
                        //    ClientDeviceType = dtClient.Rows[0]["DeviceType"].ToString();
                        //    ClientDeviceToken = dtClient.Rows[0]["DeviceToken"].ToString();
                        //    BizObjects.UserNotification objUserNotification = new BizObjects.UserNotification();
                        //    string message = General.GetNotificationMessage("RescheduleServiceSendToClient"); //"Your Service at " + ClientName + "’s address on " + Convert.ToDateTime(txtScheduleOn.Text.Trim()).ToString("MMMM dd, yyyy") + " has been rescheduled.";
                        //    message = message.Replace("{{EmpName}}", EmpName);
                        //    message = message.Replace("{{ScheduleDate}}", Convert.ToDateTime(dtService.Rows[0]["ScheduleDate"].ToString()).ToString("MMMM dd, yyyy"));

                        //    NotifyUser((Session["ClientLoginCookie"] as LoginModel).Id, "client", General.NotificationType.FriendlyReminder.GetEnumDescription(),
                        //        General.NotificationType.FriendlyReminder.GetEnumValue(), ServiceId, message, ClientDeviceType, ClientDeviceToken);
                        //}
                        //If Emergency Service then directly reschedule service
                        if (drpPurposeOfVisit.Value == General.PurposeOfVisit.Emergency.GetEnumDescription() && !IsLateReschedule)
                        {
                            string RescheduleTime = selectedSlot;

                            DataTable dtService1 = new DataTable();
                            string message = string.Empty;

                            objRequestServicesService.ScheduleRequestedService(ServiceId, 0, (Session["ClientLoginCookie"] as LoginModel).Id, Convert.ToInt32(dtService.Rows[0]["AddressID"].ToString()), drpPurposeOfVisit.Value, Convert.ToDateTime(txtReschedule.Value.Trim()), RescheduleTime, ref dtService);
                            if (dtService1.Rows.Count > 0)
                            {
                                if (Convert.ToInt32(dtService1.Rows[0]["EmployeeId"].ToString()) != 0
                            && Convert.ToInt32(dtService1.Rows[0]["ServiceId"].ToString()) != 0)
                                {
                                    int EId = Convert.ToInt32(dtService1.Rows[0]["EmployeeId"].ToString());
                                    DateTime ScheduleDate = Convert.ToDateTime(dtService1.Rows[0]["ScheduleDate"].ToString());

                                    //send notification to client
                                    message = General.GetNotificationMessage("RequestedServiceScheduleSendToClient");
                                    message = message.Replace("{{ScheduleDate}}", ScheduleDate.ToString("MMMM dd, yyyy"));

                                    NotifyUser((Session["ClientLoginCookie"] as LoginModel).Id, "client", General.NotificationType.ServiceScheduled.GetEnumDescription(),
                                        General.NotificationType.ServiceScheduled.GetEnumValue(), ServiceId, message, ClientDeviceType, ClientDeviceToken);

                                    //send notification to employee
                                    message = string.Empty;
                                    General.GetNotificationMessage("EmployeeSchedule");
                                    message = message.Replace("{{ScheduleDate}}", ScheduleDate.ToString("MMMM dd, yyyy"));

                                    NotifyUser(EId, "employee", General.NotificationType.ServiceScheduled.GetEnumDescription(),
                                        General.NotificationType.ServiceScheduled.GetEnumValue(), ServiceId, message, EmpDeviceType, EmpDeviceToken);
                                }
                            }
                        }
                        Session["success"] = 1;
                        Response.Redirect(Application["SiteAddress"] + "client/schedule.aspx", false);
                    }
                    else
                        Response.Redirect(Application["SiteAddress"] + "client/schedule.aspx", false);
                }
                catch (Exception Ex)
                {
                    dvMessage.InnerHtml = Ex.Message.ToString().Trim();
                    dvMessage.Attributes.Add("class", "error");
                    dvMessage.Visible = true;
                }
            }
        }
        #endregion
        private void NotifyUser(int UserId, string Role, string MessageType, int NoType, long ServiceId, string message, string DeviceType, string DeviceToken)
        {
            long NotificationId = 0;
            DataTable dtBadgeCount = new DataTable();
            int BadgeCount = 0;
            BizObjects.UserNotification objUserNotification = new BizObjects.UserNotification();
            objUserNotificationService = ServiceFactory.UserNotificationService;
            objUserNotification.UserId = UserId;
            if (Role.ToLower() == "client")
            {
                objUserNotification.UserTypeId = General.UserRoles.Client.GetEnumValue();
            }
            else
            {
                objUserNotification.UserTypeId = General.UserRoles.Employee.GetEnumValue();
            }
            objUserNotification.MessageType = MessageType;

            objUserNotification.Message = message;
            if (ServiceId != 0)
                objUserNotification.CommonId = ServiceId;

            objUserNotification.Status = General.NotificationStatus.UnRead.GetEnumDescription();

            objUserNotification.AddedDate = DateTime.UtcNow;

            NotificationId = objUserNotificationService.AddUserNotification(ref objUserNotification);

            objUserNotificationService.GetBadgeCount(UserId, objUserNotification.UserTypeId, ref dtBadgeCount);
            BadgeCount = dtBadgeCount.Rows.Count;

            Notifications objNotifications = new Notifications { NId = NotificationId, NType = NoType, CommonId = objUserNotification.CommonId };
            List<NotificationModel> notify = new List<NotificationModel>();
            notify.Add(new NotificationModel { Key = "NId", Value = new object[] { objNotifications.NId } });
            notify.Add(new NotificationModel { Key = "NType", Value = new object[] { objNotifications.NType } });
            notify.Add(new NotificationModel { Key = "CommonId", Value = new object[] { objNotifications.CommonId } });

            if (Role.ToLower() == "client")
            {
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
            else
            {
                if (DeviceType.ToLower() == "iphone")
                {
                    SendNotifications.SendIphoneNotification(BadgeCount, DeviceToken, message, notify, "employee");
                }
            }
        }
        
    }
}