using Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Aircall.Common;

namespace Aircall.admin
{
    public partial class WaitingService_Edit : System.Web.UI.Page
    {
        IClientAddressService objClientAddressService;
        IClientUnitService objClientUnitService;
        IServiceUnitService objServiceUnitService;
        IAreasService objAreasService;
        IEmployeeWorkAreaService objEmployeeWorkAreaService;
        IServicesService objServicesService;
        IServiceAttemptCountService objServiceAttemptCountService;
        IUserNotificationService objUserNotificationService;
        IClientService objClientService;
        IEmployeeService objEmployeeService;
        IServiceReportService objServiceReportService;
        IRescheduleServicesService objRescheduleServicesService;
        IRequestServicesService objRequestServicesService;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                hdnMaintenanceServicesWithinDays.Value = General.GetSitesettingsValue("MaintenanceServicesWithinDays");
                hdnEmergencyAndOtherServiceWithinDays.Value = General.GetSitesettingsValue("EmergencyAndOtherServiceWithinDays");
                hdnWeekTimeSlot.Value = General.GetSitesettingsValue("EmergencyServiceSlot1") + "|" + General.GetSitesettingsValue("EmergencyServiceSlot2");
                FillPurposeOfVisitDropdown();

                if (!string.IsNullOrEmpty(Request.QueryString["ServiceId"]))
                {
                    BindServiceInformation();
                    BindAttemptFailReasons();
                    BindRescheduleAttemptLog();
                }
            }
        }

        private void BindRescheduleAttemptLog()
        {
            long ServiceId = Convert.ToInt64(Request.QueryString["ServiceId"].ToString());
            DataTable dtRescheduleService = new DataTable();
            objRescheduleServicesService = ServiceFactory.RescheduleServicesService;
            objRescheduleServicesService.GetAllByServiceId(ServiceId, ref dtRescheduleService);
            if (dtRescheduleService.Rows.Count > 0)
            {
                dvReschedule.Visible = true;
                lstReschedule.DataSource = dtRescheduleService;
                lstReschedule.DataBind();
            }
            else
                dvReschedule.Visible = false;
        }

        private void BindAttemptFailReasons()
        {
            long ServiceId = Convert.ToInt64(Request.QueryString["ServiceId"].ToString());
            DataTable dtAttempt = new DataTable();
            objServiceAttemptCountService = ServiceFactory.ServiceAttemptCountService;
            objServiceAttemptCountService.GetAttemptCountsByServiceId(ServiceId, ref dtAttempt);
            if (dtAttempt.Rows.Count > 0)
            {
                dvAttempt.Visible = true;
                lstAttempt.DataSource = dtAttempt;
                lstAttempt.DataBind();
            }
            else
                dvAttempt.Visible = false;
        }

        private void BindServiceInformation()
        {
            long ServiceId = Convert.ToInt64(Request.QueryString["ServiceId"].ToString());
            objServicesService = ServiceFactory.ServicesService;
            DataTable dtService = new DataTable();
            objServicesService.GetServiceById(ServiceId, ref dtService);
            if (dtService.Rows.Count > 0)
            {
                if (Convert.ToBoolean(dtService.Rows[0]["IsNoShow"].ToString()))
                {
                    btnApprove.Visible = false;
                    btnCancel.Visible = false;
                }

                int ClientId = Convert.ToInt32(dtService.Rows[0]["ClientId"].ToString());
                int PlanTypeId = Convert.ToInt32(dtService.Rows[0]["PlanTypeId"].ToString());
                hdnClient.Value = dtService.Rows[0]["ClientId"].ToString();
                hdnServiceDayGap.Value = dtService.Rows[0]["ServiceDayGap"].ToString();
                //hdnServiceCaseNo.Value = dtService.Rows[0]["ServiceCaseNumber"].ToString();
                int AddressId = Convert.ToInt32(dtService.Rows[0]["AddressID"].ToString());
                //txtClient.Text = dtService.Rows[0]["ClientName"].ToString();
                ltrClient.Text = dtService.Rows[0]["ClientName"].ToString();
                objClientAddressService = ServiceFactory.ClientAddressService;
                DataTable dtAddress = new DataTable();
                objClientAddressService.GetClientAddressesByClientId(ClientId, true, ref dtAddress);
                if (dtAddress.Rows.Count > 0)
                {
                    rblAddress.DataSource = dtAddress;
                    rblAddress.DataTextField = dtAddress.Columns["ClientAddress"].ToString();
                    rblAddress.DataValueField = dtAddress.Columns["Id"].ToString();
                    rblAddress.DataBind();
                    rblAddress.SelectedValue = AddressId.ToString();
                    rblAddress.Enabled = false;
                }
                ltrMobile.Text = dtService.Rows[0]["MobileNumber"].ToString();
                ltrHome.Text = dtService.Rows[0]["HomeNumber"].ToString();
                ltrOffice.Text = dtService.Rows[0]["OfficeNumber"].ToString();
                DataTable dtUnits = new DataTable();
                objClientUnitService = ServiceFactory.ClientUnitService;
                //objClientUnitService.GetClientUnitByClientAndAddressId(ClientId, AddressId, ref dtUnits);
                objClientUnitService.GetUnitsByClientAddressAndPlanId(ClientId, AddressId, PlanTypeId, ref dtUnits);
                if (dtUnits.Rows.Count > 0)
                {
                    chkUnits.DataSource = dtUnits;
                    chkUnits.DataTextField = dtUnits.Columns["UnitName"].ToString();
                    chkUnits.DataValueField = dtUnits.Columns["Id"].ToString();
                    chkUnits.DataBind();

                    DataTable dtServiceUnit = new DataTable();
                    objServiceUnitService = ServiceFactory.ServiceUnitService;
                    objServiceUnitService.GetServiceUnitByServiceId(ServiceId, ref dtServiceUnit);
                    if (dtServiceUnit.Rows.Count > 0)
                    {
                        for (int i = 0; i < dtServiceUnit.Rows.Count; i++)
                        {
                            ListItem item = chkUnits.Items.FindByValue(dtServiceUnit.Rows[i]["UnitId"].ToString());
                            if (item != null)
                                item.Selected = true;

                        }
                        lstUnits.DataSource = dtServiceUnit;
                        lstUnits.DataBind();
                    }

                    //BindTimeSlotByClientAddressIdAndPlanTypeId(ClientId, AddressId, Convert.ToInt32(dtUnits.Rows[0]["PlanTypeId"].ToString()));
                    BindTimeSlotByClientAddressIdAndPlanTypeId(ClientId, AddressId, PlanTypeId, dtService.Rows[0]["PurposeOfVisit"].ToString(), dtService.Rows[0]["ScheduleDate"].ToString());
                }

                txtServiceRequested.Text = Convert.ToDateTime(dtService.Rows[0]["AddedDate"].ToString()).ToString("MM/dd/yyyy");
                txtServiceRequested.Enabled = false;
                drpPurpose.SelectedValue = dtService.Rows[0]["PurposeOfVisit"].ToString();
                hdnPurposeOfVisit.Value = dtService.Rows[0]["PurposeOfVisit"].ToString();
                txtWorkArea.Text = dtService.Rows[0]["AreaName"].ToString();
                DataTable dtArea = new DataTable();
                objAreasService = ServiceFactory.AreasService;
                int AreaId = 0;
                if (!string.IsNullOrEmpty(dtService.Rows[0]["WorkAreaId"].ToString()))
                {
                    AreaId = Convert.ToInt32(dtService.Rows[0]["WorkAreaId"].ToString());
                    objAreasService.GetAreaById(AreaId, true, ref dtArea);
                    if (dtArea.Rows.Count > 0)
                    {
                        rblWorkArea.DataSource = dtArea;
                        rblWorkArea.DataValueField = dtArea.Columns["Id"].ToString();
                        rblWorkArea.DataTextField = dtArea.Columns["Name"].ToString();
                        rblWorkArea.DataBind();
                        rblWorkArea.SelectedValue = AreaId.ToString();
                    }
                }
                lnkSearch.Visible = false;
                rblWorkArea.Enabled = false;
                txtEmployee.Text = dtService.Rows[0]["EmployeeName"].ToString();

                objEmployeeWorkAreaService = ServiceFactory.EmployeeWorkAreaService;
                DataTable dtEmployee = new DataTable();
                objEmployeeWorkAreaService.GetAllEmployeeByAreaId(AreaId, "", false, ref dtEmployee);
                if (dtEmployee.Rows.Count > 0)
                {
                    rblEmployee.DataSource = dtEmployee;
                    rblEmployee.DataTextField = dtEmployee.Columns["EmployeeName"].ToString();
                    rblEmployee.DataValueField = dtEmployee.Columns["EmployeeId"].ToString();
                    rblEmployee.DataBind();
                    rblEmployee.SelectedValue = dtService.Rows[0]["EmployeeId"].ToString();
                    //hdnEmployeeId.Value = dtService.Rows[0]["EmployeeId"].ToString();
                }
                rblEmployee.Enabled = false;
                lnkEmpSearch.Visible = false;
                if (!string.IsNullOrEmpty(dtService.Rows[0]["ScheduleDate"].ToString()))
                {
                    txtScheduleOn.Text = Convert.ToDateTime(dtService.Rows[0]["ScheduleDate"].ToString()).ToString("MM/dd/yyyy");
                    ltrScheduleDate.Text = Convert.ToDateTime(dtService.Rows[0]["ScheduleDate"].ToString()).ToString("MM/dd/yyyy");
                    txtReschedule.Text = Convert.ToDateTime(dtService.Rows[0]["ScheduleDate"].ToString()).ToString("MM/dd/yyyy");
                }

                txtScheduleOn.Enabled = false;

                txtStart.Value = dtService.Rows[0]["ScheduleStartTime"].ToString();
                txtEnd.Value = dtService.Rows[0]["ScheduleEndTime"].ToString();
                ltrScheduleTime.Text = dtService.Rows[0]["ScheduleStartTime"].ToString() + " - " + dtService.Rows[0]["ScheduleEndTime"].ToString();
                txtStart.Disabled = true;
                txtEnd.Disabled = true;

                txtCustomerNote.Text = dtService.Rows[0]["CustomerComplaints"].ToString();
                txtDispatcherNote.Text = dtService.Rows[0]["DispatcherNotes"].ToString();
                txtEmpNote.Text = dtService.Rows[0]["TechnicianNotes"].ToString();
                objServiceReportService = ServiceFactory.ServiceReportService;
                DataTable dtServiceReport = new DataTable();
                objServiceReportService.GetServiceReportOfUnitsByServiceId(ServiceId, ref dtServiceReport);
                if (dtServiceReport.Rows.Count > 0)
                {
                    lstServicereport.DataSource = dtServiceReport;
                    lstServicereport.DataBind();
                }
                if (!Convert.ToBoolean(dtService.Rows[0]["IsRequestedService"].ToString()))
                    btnCancel.Visible = false;
            }
        }

        private void BindTimeSlotByClientAddressIdAndPlanTypeId(int ClientId, int AddressId, int PlanTypeId, string PurposeOfVisit, string ScheduleDate)
        {
            objClientUnitService = ServiceFactory.ClientUnitService;
            DataTable dtUnits = new DataTable();
            DateTime dt = Convert.ToDateTime(ScheduleDate);
            objClientUnitService.GetUnitsByClientAddressAndPlanId(ClientId, AddressId, PlanTypeId, ref dtUnits);
            if (dtUnits.Rows.Count > 0)
            {
                hdnServiceTime.Value = dtUnits.Rows[0]["ServiceSlot1"].ToString() + "|" + dtUnits.Rows[0]["ServiceSlot2"].ToString();
                hdnWeekEndTimeSlot.Value = dtUnits.Rows[0]["ServiceSlot1"].ToString() + "|" + dtUnits.Rows[0]["ServiceSlot2"].ToString();
            }
            else
                hdnServiceTime.Value = "";

            if (PurposeOfVisit == General.PurposeOfVisit.Emergency.GetEnumDescription())
            {
                if (dt.DayOfWeek == DayOfWeek.Saturday || dt.DayOfWeek == DayOfWeek.Sunday)
                    hdnServiceTime.Value = hdnWeekEndTimeSlot.Value;
                else
                    hdnServiceTime.Value = hdnWeekTimeSlot.Value;
            }
        }

        private void FillPurposeOfVisitDropdown()
        {
            var values = DurationExtensions.GetValues<General.PurposeOfVisit>();
            List<string> data = new List<string>();
            foreach (var item in values)
            {
                General.PurposeOfVisit p = (General.PurposeOfVisit)item;
                data.Add(p.GetEnumDescription());
            }
            drpPurpose.DataSource = data;
            drpPurpose.DataBind();
        }

        protected void lnkSearch_Click(object sender, EventArgs e)
        {
            rblWorkArea.DataSource = "";
            objAreasService = ServiceFactory.AreasService;
            DataTable dtAreas = new DataTable();
            objAreasService.SearchByAreaName(txtWorkArea.Text.Trim(), ref dtAreas);
            if (dtAreas.Rows.Count > 0)
            {
                rblWorkArea.DataSource = dtAreas;
                rblWorkArea.DataValueField = dtAreas.Columns["Id"].ToString();
                rblWorkArea.DataTextField = dtAreas.Columns["Name"].ToString();
            }
            rblWorkArea.DataBind();
            txtEmployee.Text = "";
            rblEmployee.DataSource = "";
            rblEmployee.DataBind();
        }

        protected void lnkEmpSearch_Click(object sender, EventArgs e)
        {
            if (rblWorkArea.Items.Count == 0 || rblWorkArea.SelectedIndex == -1)
            {
                dvMessage.InnerHtml = "<strong>Please Select Work Area</strong>";
                dvMessage.Attributes.Add("class", "alert alert-error");
                dvMessage.Visible = true;
                return;
            }
            objEmployeeWorkAreaService = ServiceFactory.EmployeeWorkAreaService;
            DataTable dtEmployees = new DataTable();
            objEmployeeWorkAreaService.GetAllEmployeeByAreaId(Convert.ToInt32(rblWorkArea.SelectedValue.ToString()), txtEmployee.Text.Trim(), false, ref dtEmployees);
            if (dtEmployees.Rows.Count > 0)
            {
                rblEmployee.DataSource = dtEmployees;
                rblEmployee.DataTextField = dtEmployees.Columns["EmpName"].ToString();
                rblEmployee.DataValueField = dtEmployees.Columns["EmployeeId"].ToString();
                rblEmployee.DataBind();
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(Request.QueryString["ServiceId"]))
                {
                    long ServiceId = Convert.ToInt64(Request.QueryString["ServiceId"].ToString());
                    int EmployeeId = 0;
                    int ClientId = 0;
                    string ScheduleDate=string.Empty;
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
                    objLoginModel = Session["LoginSession"] as LoginModel;

                    BizObjects.RescheduleService objRescheduleService = new BizObjects.RescheduleService();
                    objRescheduleServicesService = ServiceFactory.RescheduleServicesService;

                    objRescheduleService.ServiceId = ServiceId;
                    objRescheduleService.RescheduleDate = Convert.ToDateTime(txtReschedule.Text.Trim());
                    //string[] TimeSlot = hdnServiceTime.Value.Split('|');
                    //if (chkRequestedTime.Checked)
                    //    objRescheduleService.Rescheduletime = TimeSlot[0];
                    //else
                    objRescheduleService.Rescheduletime = string.Empty;

                    objRescheduleService.Reason = txtReason.Text.Trim();
                    objRescheduleService.AddedBy = objLoginModel.Id;
                    objRescheduleService.AddedByType = objLoginModel.RoleId;
                    objRescheduleService.AddedDate = DateTime.UtcNow;

                    objRescheduleServicesService.AddRescheduleService(ref objRescheduleService, General.ServiceTypes.Cancelled.GetEnumDescription());


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
                    Response.Redirect(Application["SiteAddress"] + "admin/WaitingService_List.aspx?msg=cen");
                }
            }
            catch (Exception Ex)
            {
                dvMessage.InnerHtml = "<strong>Error!</strong> " + Ex.Message.ToString().Trim();
                dvMessage.Attributes.Add("class", "alert alert-error");
                dvMessage.Visible = true;
            }
        }

        protected void btnApprove_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    if (!string.IsNullOrEmpty(Request.QueryString["ServiceId"]))
                    {
                        long ServiceId = Convert.ToInt64(Request.QueryString["ServiceId"].ToString());
                        if (rblWorkArea.Items.Count == 0 || rblWorkArea.SelectedIndex == -1)
                        {
                            dvMessage.InnerHtml = "<strong>Please Select Work Area</strong>";
                            dvMessage.Attributes.Add("class", "alert alert-error");
                            dvMessage.Visible = true;
                            return;
                        }
                        if (rblEmployee.Items.Count == 0 || rblEmployee.SelectedIndex == -1)
                        {
                            dvMessage.InnerHtml = "<strong>Please Select Employee.</strong>";
                            dvMessage.Attributes.Add("class", "alert alert-error");
                            dvMessage.Visible = true;
                            return;
                        }
                        if (chkUnits.SelectedIndex == -1)
                        {
                            dvMessage.InnerHtml = "<strong>Please Select Service Units.</strong>";
                            dvMessage.Attributes.Add("class", "alert alert-error");
                            dvMessage.Visible = true;
                            return;
                        }
                        if (chkUnits.Items.Count == 0 || chkUnits.SelectedIndex == -1)
                        {
                            dvMessage.InnerHtml = "<strong>Please Select Units.</strong>";
                            dvMessage.Attributes.Add("class", "alert alert-error");
                            dvMessage.Visible = true;
                            return;
                        }

                        //if (!string.IsNullOrEmpty(hdnServiceDayGap.Value))
                        //{
                        //    int DayCount = Convert.ToInt32(hdnServiceDayGap.Value);
                        //    DateTime dt = Convert.ToDateTime(txtScheduleOn.Text.Trim());
                        //    if (dt.Date < DateTime.UtcNow.AddDays(DayCount).Date)
                        //    {
                        //        dvMessage.InnerHtml = "<strong>Please Enter a Date " + DayCount.ToString() + " days from now.</strong>";
                        //        dvMessage.Attributes.Add("class", "alert alert-error");
                        //        dvMessage.Visible = true;
                        //        return;
                        //    }
                        //}
                        //else
                        //    return;

                        string ClientUnitIds = string.Empty;
                        foreach (ListItem item in chkUnits.Items)
                        {
                            if (item.Selected)
                            {
                                if (string.IsNullOrEmpty(ClientUnitIds))
                                    ClientUnitIds = item.Value;
                                else
                                    ClientUnitIds = ClientUnitIds + ',' + item.Value;
                            }
                        }
                        objClientUnitService = ServiceFactory.ClientUnitService;
                        DataTable dtClientUnit = new DataTable();
                        objClientUnitService.CheckPlanTypeByClientUnitIds(ClientUnitIds, ref dtClientUnit);
                        if (dtClientUnit.Rows.Count > 1)
                        {
                            dvMessage.InnerHtml = "<strong>Please select unit of same plan type.</strong>";
                            dvMessage.Attributes.Add("class", "alert alert-error");
                            dvMessage.Visible = true;
                            return;
                        }

                        BizObjects.Services objServices = new BizObjects.Services();
                        BizObjects.ServiceUnits objServiceUnits = new BizObjects.ServiceUnits();
                        objServices.Id = ServiceId;
                        objServices.AddedDate = Convert.ToDateTime(txtServiceRequested.Text.Trim());
                        objServices.PurposeOfVisit = drpPurpose.SelectedValue;
                        objServices.WorkAreaId = Convert.ToInt32(rblWorkArea.SelectedValue);
                        objServices.EmployeeId = Convert.ToInt32(rblEmployee.SelectedValue);
                        objServices.ScheduleDate = Convert.ToDateTime(txtScheduleOn.Text.Trim());
                        objServices.ScheduleStartTime = txtStart.Value.Trim();
                        objServices.ScheduleEndTime = txtEnd.Value.Trim();
                        objServices.CustomerComplaints = txtCustomerNote.Text.Trim();
                        objServices.DispatcherNotes = txtDispatcherNote.Text.Trim();
                        objServices.TechnicianNotes = txtEmpNote.Text.Trim();
                        objServices.Status = General.ServiceTypes.Scheduled.GetEnumDescription();

                        objServicesService = ServiceFactory.ServicesService;
                        objServiceUnitService = ServiceFactory.ServiceUnitService;

                        objServicesService.UpdateWaitingApprovalService(ref objServices);

                        foreach (ListItem item in chkUnits.Items)
                        {
                            if (item.Selected)
                            {
                                objServiceUnits.ServiceId = ServiceId;
                                objServiceUnits.UnitId = Convert.ToInt32(item.Value);
                                objServiceUnitService.AddServiceUnit(ref objServiceUnits);
                            }
                        }

                        objClientUnitService = ServiceFactory.ClientUnitService;
                        objClientUnitService.SetStatusByServiceId(General.UnitStatus.ServiceSoon.GetEnumValue(), ServiceId);

                        //send notification to client and employee for service
                        DataTable dtBadgeCount = new DataTable();
                        objClientService = ServiceFactory.ClientService;
                        DataTable dtClient = new DataTable();
                        //if (!string.IsNullOrEmpty(hdnClient.Value))
                        //{
                        //    objClientService.GetClientById(Convert.ToInt32(hdnClient.Value), ref dtClient);
                        //    if (dtClient.Rows.Count > 0)
                        //    {
                        //        if (!string.IsNullOrEmpty(dtClient.Rows[0]["DeviceType"].ToString()) &&
                        //            !string.IsNullOrEmpty(dtClient.Rows[0]["DeviceToken"].ToString()) &&
                        //             dtClient.Rows[0]["DeviceToken"].ToString().ToLower() != "no device token")
                        //        {
                        //            long NotificationId = 0;
                        //            int BadgeCount = 0;
                        //            int ClientId = Convert.ToInt32(hdnClient.Value);
                        //            BizObjects.UserNotification objUserNotification = new BizObjects.UserNotification();
                        //            string message = "Your unit Service is Scheduled.";
                        //            objUserNotificationService = ServiceFactory.UserNotificationService;
                        //            objUserNotification.UserId = ClientId;
                        //            objUserNotification.UserTypeId = General.UserRoles.Client.GetEnumValue();
                        //            objUserNotification.Message = message;
                        //            objUserNotification.Status = General.NotificationStatus.UnRead.GetEnumDescription();
                        //            objUserNotification.MessageType = General.NotificationType.FriendlyReminder.GetEnumDescription();
                        //            objUserNotification.AddedDate = DateTime.UtcNow;

                        //            NotificationId = objUserNotificationService.AddUserNotification(ref objUserNotification);

                        //            dtBadgeCount.Clear();
                        //            objUserNotificationService.GetBadgeCount(ClientId, General.UserRoles.Client.GetEnumValue(), ref dtBadgeCount);
                        //            BadgeCount = dtBadgeCount.Rows.Count;

                        //            Notifications objNotifications = new Notifications { NId = NotificationId, NType = General.NotificationType.CreditCardExpiration.GetEnumValue() };
                        //            List<NotificationModel> notify = new List<NotificationModel>();
                        //            notify.Add(new NotificationModel { Key = "NId", Value = new object[] { objNotifications.NId } });
                        //            notify.Add(new NotificationModel { Key = "NType", Value = new object[] { objNotifications.NType } });
                        //            //notify.Add(new NotificationModel { Key = "CommonId", Value = new object[] { objNotifications.CommonId } });

                        //            if (dtClient.Rows[0]["DeviceType"].ToString().ToLower() == "android")
                        //            {
                        //                string CustomData = "&data.NId=" + objNotifications.NId + "&data.NType=" + objNotifications.NType;
                        //                SendNotifications.SendAndroidNotification(dtClient.Rows[0]["DeviceToken"].ToString(), message, CustomData, "client");
                        //            }
                        //            else if (dtClient.Rows[0]["DeviceType"].ToString().ToLower() == "iphone")
                        //            {
                        //                SendNotifications.SendIphoneNotification(BadgeCount, dtClient.Rows[0]["DeviceToken"].ToString(), message, notify, "client");
                        //            }
                        //        }
                        //    }
                        //}
                        objUserNotificationService = ServiceFactory.UserNotificationService;
                        objEmployeeService = ServiceFactory.EmployeeService;
                        DataTable dtEmployee = new DataTable();
                        objEmployeeService.GetEmployeeById(Convert.ToInt32(rblEmployee.SelectedValue), ref dtEmployee);
                        if (dtEmployee.Rows.Count > 0)
                        {
                            long NotificationId = 0;
                            int BadgeCount = 0;
                            int EmployeeId = Convert.ToInt32(rblEmployee.SelectedValue);
                            BizObjects.UserNotification objUserNotification = new BizObjects.UserNotification();

                            string message = General.GetNotificationMessage("EmployeeSchedule"); //"System has scheduled a service for you on " + Convert.ToDateTime(txtScheduleOn.Text.Trim()).ToString("MMMM dd, yyyy") + ".";
                            message = message.Replace("{{ScheduleDate}}", Convert.ToDateTime(txtScheduleOn.Text.Trim()).ToString("MMMM dd, yyyy"));
                            objUserNotification.UserId = EmployeeId;
                            objUserNotification.UserTypeId = General.UserRoles.Employee.GetEnumValue();
                            objUserNotification.Message = message;
                            objUserNotification.CommonId = ServiceId;
                            objUserNotification.Status = General.NotificationStatus.UnRead.GetEnumDescription();
                            objUserNotification.MessageType = General.NotificationType.ServiceScheduled.GetEnumDescription();
                            objUserNotification.AddedDate = DateTime.UtcNow;

                            NotificationId = objUserNotificationService.AddUserNotification(ref objUserNotification);

                            dtBadgeCount.Clear();
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
                                    //SendNotifications.SendIphoneNotification(BadgeCount, dtEmployee.Rows[0]["DeviceToken"].ToString(), message, notify, "employee");
                                }
                            }
                        }

                        //Delete Notification
                        objUserNotificationService.DeleteNotificationByCommonIdType(ServiceId, General.NotificationType.ServiceApproval.GetEnumDescription());
                        //objUserNotificationService.DeleNotification(ServiceId);

                        Response.Redirect(Application["SiteAddress"] + "admin/WaitingService_List.aspx?msg=edit");
                    }
                }
                catch (Exception Ex)
                {
                    dvMessage.InnerHtml = "<strong>Error!</strong> " + Ex.Message.ToString().Trim();
                    dvMessage.Attributes.Add("class", "alert alert-error");
                    dvMessage.Visible = true;
                }
            }
        }

        protected void lstUnits_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["ServiceId"]))
                {
                    long ServiceId = Convert.ToInt64(Request.QueryString["ServiceId"].ToString());
                    objServicesService = ServiceFactory.ServicesService;
                    DataTable dtMaterialList = new DataTable();

                    ListViewDataItem currentItem = (ListViewDataItem)e.Item;
                    HiddenField hdnUnitId = (HiddenField)currentItem.FindControl("hdnUnitId");

                    objServicesService.GetMaterialListByServiceAndUnitId(ServiceId, Convert.ToInt32(hdnUnitId.Value), ref dtMaterialList);
                    if (dtMaterialList.Rows.Count > 0)
                    {
                        ListView lstUnitParts = (ListView)currentItem.FindControl("lstUnitParts");
                        lstUnitParts.DataSource = dtMaterialList;
                        lstUnitParts.DataBind();
                    }
                }
            }
        }

        protected void btnReschedule_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    if (!string.IsNullOrEmpty(Request.QueryString["ServiceId"]))
                    {
                        LoginModel objLoginModel = new LoginModel();
                        objLoginModel = Session["LoginSession"] as LoginModel;
                        if (!string.IsNullOrEmpty(Request.QueryString["ServiceId"]))
                        {
                            long ServiceId = Convert.ToInt64(Request.QueryString["ServiceId"].ToString());
                            objServicesService = ServiceFactory.ServicesService;
                            DataTable dtServices = new DataTable();
                            objServicesService.GetServiceById(ServiceId, ref dtServices);
                            string ClientDeviceType = string.Empty;
                            string ClientDeviceToken = string.Empty;
                            string EmpDeviceType = string.Empty;
                            string EmpDeviceToken = string.Empty;

                            if (dtServices.Rows.Count > 0)
                            {
                                int EmployeeId = Convert.ToInt32(dtServices.Rows[0]["EmployeeId"].ToString());
                                string ClientName = dtServices.Rows[0]["ClientName"].ToString();
                                int ClientId = Convert.ToInt32(dtServices.Rows[0]["ClientId"].ToString());

                                BizObjects.RescheduleService objRescheduleService = new BizObjects.RescheduleService();
                                objRescheduleServicesService = ServiceFactory.RescheduleServicesService;

                                objRescheduleService.ServiceId = ServiceId;
                                objRescheduleService.RescheduleDate = Convert.ToDateTime(txtReschedule.Text.Trim());
                                string[] TimeSlot = hdnServiceTime.Value.Split('|');
                                if (chkRequestedTime.Checked)
                                    objRescheduleService.Rescheduletime = TimeSlot[0];
                                else
                                    objRescheduleService.Rescheduletime = TimeSlot[1];

                                objRescheduleService.Reason = txtReason.Text.Trim();
                                objRescheduleService.AddedBy = objLoginModel.Id;
                                objRescheduleService.AddedByType = objLoginModel.RoleId;
                                objRescheduleService.AddedDate = DateTime.UtcNow;

                                objRescheduleServicesService.AddRescheduleService(ref objRescheduleService, General.ServiceTypes.Rescheduled.GetEnumDescription());

                                objUserNotificationService = ServiceFactory.UserNotificationService;
                                objUserNotificationService.DeleteNotificationByCommonIdType(ServiceId, General.NotificationType.ServiceApproval.GetEnumDescription());
                                objUserNotificationService.DeleteNotificationByCommonIdType(ServiceId, General.NotificationType.ServiceScheduled.GetEnumDescription());
                                objUserNotificationService.DeleteNotificationByCommonIdType(ServiceId, General.NotificationType.PeriodicServiceReminder.GetEnumDescription());


                                //Send Notification to Client
                                objClientService = ServiceFactory.ClientService;
                                DataTable dtClient = new DataTable();
                                objClientService.GetClientById(ClientId, ref dtClient);
                                if (dtClient.Rows.Count>0)
                                {
                                    ClientDeviceType = dtClient.Rows[0]["DeviceType"].ToString();
                                    ClientDeviceToken = dtClient.Rows[0]["DeviceToken"].ToString();
                                    BizObjects.UserNotification objUserNotification = new BizObjects.UserNotification();
                                    string message = General.GetNotificationMessage("RescheduleServiceSendToClient"); //"Your Service at " + ClientName + "’s address on " + Convert.ToDateTime(txtScheduleOn.Text.Trim()).ToString("MMMM dd, yyyy") + " has been rescheduled.";
                                    message = message.Replace("{{EmpName}}", objLoginModel.Username);
                                    message = message.Replace("{{ScheduleDate}}", Convert.ToDateTime(txtScheduleOn.Text.Trim()).ToString("MMMM dd, yyyy"));

                                    NotifyUser(ClientId, "client", General.NotificationType.FriendlyReminder.GetEnumDescription(), 
                                        General.NotificationType.FriendlyReminder.GetEnumValue(), ServiceId, message, ClientDeviceType, ClientDeviceToken);
                                }

                                //Send Notification to Employee
                                objEmployeeService = ServiceFactory.EmployeeService;
                                DataTable dtEmployee = new DataTable();
                                objEmployeeService.GetEmployeeById(EmployeeId, ref dtEmployee);
                                if (dtEmployee.Rows.Count > 0)
                                {
                                    EmpDeviceType=dtEmployee.Rows[0]["DeviceType"].ToString();
                                    EmpDeviceToken = dtEmployee.Rows[0]["DeviceToken"].ToString();
                                    BizObjects.UserNotification objUserNotification = new BizObjects.UserNotification();
                                    string message = General.GetNotificationMessage("RescheduleServiceSendToEmployee"); //"Your Service at " + ClientName + "’s address on " + Convert.ToDateTime(txtScheduleOn.Text.Trim()).ToString("MMMM dd, yyyy") + " has been rescheduled.";
                                    message = message.Replace("{{ClientName}}", ClientName);
                                    message = message.Replace("{{ScheduleDate}}", Convert.ToDateTime(txtScheduleOn.Text.Trim()).ToString("MMMM dd, yyyy"));

                                    NotifyUser(EmployeeId, "employee", General.NotificationType.FriendlyReminder.GetEnumDescription(), 
                                        General.NotificationType.FriendlyReminder.GetEnumValue(), ServiceId, message, EmpDeviceType, EmpDeviceToken);
                                }

                                //If Emergency Service then directly reschedule service
                                if (drpPurpose.SelectedValue == General.PurposeOfVisit.Emergency.GetEnumDescription())
                                {
                                    string[] Slot = hdnServiceTime.Value.Split('|');
                                    string RescheduleTime = string.Empty;
                                    if (chkRequestedTime.Checked)
                                        RescheduleTime = Slot[0];
                                    else
                                        RescheduleTime = Slot[1];
                                    DataTable dtService = new DataTable();
                                    string message = string.Empty;

                                    objRequestServicesService.ScheduleRequestedService(ServiceId, 0, ClientId, Convert.ToInt32(rblAddress.SelectedValue.ToString()), drpPurpose.SelectedValue, Convert.ToDateTime(txtReschedule.Text.Trim()), RescheduleTime, ref dtService);
                                    if (dtService.Rows.Count > 0)
                                    {
                                        if (Convert.ToInt32(dtService.Rows[0]["EmployeeId"].ToString()) != 0
                                    && Convert.ToInt32(dtService.Rows[0]["ServiceId"].ToString()) != 0)
                                        {
                                            int EId = Convert.ToInt32(dtService.Rows[0]["EmployeeId"].ToString());
                                            DateTime ScheduleDate = Convert.ToDateTime(dtService.Rows[0]["ScheduleDate"].ToString());

                                            //send notification to client
                                            message = General.GetNotificationMessage("RequestedServiceScheduleSendToClient");
                                            message = message.Replace("{{ScheduleDate}}", ScheduleDate.ToString("MMMM dd, yyyy"));

                                            NotifyUser(ClientId, "client", General.NotificationType.ServiceScheduled.GetEnumDescription(), 
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
                            }

                            Response.Redirect(Application["SiteAddress"] + "admin/WaitingService_List.aspx?msg=reschedule");
                        }
                    }
                }
                catch (Exception Ex)
                {
                    dvMessage.InnerHtml = "<strong>Error!</strong> " + Ex.Message.ToString().Trim();
                    dvMessage.Attributes.Add("class", "alert alert-error");
                    dvMessage.Visible = true;
                }
            }
        }

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