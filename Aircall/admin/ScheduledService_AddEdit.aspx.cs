using Aircall.Common;
using Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Aircall.admin
{
    public partial class ScheduledService_AddEdit : System.Web.UI.Page
    {
        IClientService objClientService;
        IClientAddressService objClientAddressService;
        IClientUnitService objClientUnitService;
        IAreasService objAreasService;
        IEmployeeWorkAreaService objEmployeeWorkAreaService;
        IEmployeeService objEmployeeService;
        IServicesService objServicesService;
        IServiceUnitService objServiceUnitService;
        IRescheduleServicesService objRescheduleServicesService;
        IServiceReportService objServiceReportService;
        IUserNotificationService objUserNotificationService;
        IRequestServicesService objRequestServicesService;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                FillPurposeOfVisitDropdown();
                //FillServiceStatusDropdown();
                hdnMaintenanceServicesWithinDays.Value = General.GetSitesettingsValue("MaintenanceServicesWithinDays");
                hdnEmergencyAndOtherServiceWithinDays.Value = General.GetSitesettingsValue("EmergencyAndOtherServiceWithinDays");

                if (!string.IsNullOrEmpty(Request.QueryString["ServiceId"]))
                {
                    BindServiceInformationByServiceId();
                    BindRescheduleAttemptLog();
                }
                else
                {
                    txtStart.Value = "08:00 AM";
                    dvReschedule.Visible = false;
                    dvRescheduleAttempt.Visible = false;
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
                dvRescheduleAttempt.Visible = true;
                lstReschedule.DataSource = dtRescheduleService;
                lstReschedule.DataBind();
            }
            else
                dvRescheduleAttempt.Visible = false;
        }

        private void BindServiceInformationByServiceId()
        {
            lnkSearchClient.Visible = false;
            btnSchedule.Text = "Update Schedule Service";
            btnAssignEmployee.Visible = true;

            long ServiceId = Convert.ToInt64(Request.QueryString["ServiceId"]);
            objServicesService = ServiceFactory.ServicesService;
            objServiceUnitService = ServiceFactory.ServiceUnitService;
            objClientService = ServiceFactory.ClientService;
            objAreasService = ServiceFactory.AreasService;
            objEmployeeService = ServiceFactory.EmployeeService;
            objEmployeeWorkAreaService = ServiceFactory.EmployeeWorkAreaService;

            DataTable dtService = new DataTable();
            objServicesService.GetServiceById(ServiceId, ref dtService);
            if (dtService.Rows.Count > 0)
            {
                if (!Convert.ToBoolean(dtService.Rows[0]["IsRequestedService"].ToString()))
                    btnCancel.Visible = false;

                int ClientId = Convert.ToInt32(dtService.Rows[0]["ClientId"].ToString());
                int AddressId = Convert.ToInt32(dtService.Rows[0]["AddressID"].ToString());
                int PlanTypeId = Convert.ToInt32(dtService.Rows[0]["PlanTypeId"].ToString());
                txtClient.Text = dtService.Rows[0]["ClientName"].ToString();
                txtClient.Visible = false;
                ltrClient.Text = dtService.Rows[0]["ClientName"].ToString();
                hdnClientName.Value = dtService.Rows[0]["ClientName"].ToString();
                DataTable dtClients = new DataTable();
                objClientService.GetClientById(ClientId, ref dtClients);
                if (dtClients.Rows.Count > 0)
                {
                    rblClient.DataSource = dtClients;
                    rblClient.DataTextField = dtClients.Columns["ClientName"].ToString();
                    rblClient.DataValueField = dtClients.Columns["Id"].ToString();
                    rblClient.DataBind();
                }
                rblClient.SelectedValue = ClientId.ToString();
                rblClient.Enabled = false;

                BindAddressByClientId(ClientId);
                rblAddress.SelectedValue = AddressId.ToString();
                rblAddress.Enabled = false;

                hdnDrivetime.Value = dtService.Rows[0]["Drivetime"].ToString();
                hdnServiceTimeForFirstUnit.Value = dtService.Rows[0]["ServiceTimeForFirstUnit"].ToString();
                hdnServiceTimeForAdditionalUnits.Value = dtService.Rows[0]["ServiceTimeForAdditionalUnits"].ToString();


                ltrMobile.Text = dtService.Rows[0]["MobileNumber"].ToString();
                ltrHome.Text = dtService.Rows[0]["HomeNumber"].ToString();
                ltrOffice.Text = dtService.Rows[0]["OfficeNumber"].ToString();
                //BindUnitsByClientAndAddressId(ClientId, AddressId);
                DataTable dtUnits = new DataTable();
                objClientUnitService = ServiceFactory.ClientUnitService;
                objClientUnitService.GetUnitsByClientAddressAndPlanId(ClientId, AddressId, PlanTypeId, ref dtUnits);
                if (dtUnits.Rows.Count > 0)
                {
                    chkUnits.DataSource = dtUnits;
                    chkUnits.DataTextField = dtUnits.Columns["UnitName"].ToString();
                    chkUnits.DataValueField = dtUnits.Columns["Id"].ToString();
                    chkUnits.DataBind();
                }
                BindTimeSlotByClientAddressIdAndPlanTypeId(ClientId, AddressId, PlanTypeId);
                DataTable dtServiceUnit = new DataTable();
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
                hdnUnitCnt.Value = dtServiceUnit.Rows.Count.ToString();
                drpPurpose.SelectedValue = dtService.Rows[0]["PurposeOfVisit"].ToString();

                hdnPurposeOfVisit.Value = dtService.Rows[0]["PurposeOfVisit"].ToString();
                drpPurpose.Enabled = false;

                //txtWorkArea.Text = dtService.Rows[0]["AreaName"].ToString();
                //txtWorkArea.Enabled = false;

                int WorkAreaId = Convert.ToInt32(dtService.Rows[0]["WorkAreaId"].ToString());

                BindWorkAreaByAddressId(AddressId);

                rblWorkArea.SelectedValue = WorkAreaId.ToString();
                hdnWorkAreaId.Value = WorkAreaId.ToString();
                lnkAreaSearch.Visible = false;

                //txtEmployee.Text = dtService.Rows[0]["EmployeeName"].ToString();
                //txtEmployee.Enabled = false;

                int EmployeeId = Convert.ToInt32(dtService.Rows[0]["EmployeeId"].ToString());
                DataTable dtEmployee = new DataTable();

                //objEmployeeService.GetEmployeeById(EmployeeId, ref dtEmployee);
                objEmployeeWorkAreaService.GetAllEmployeeByAreaId(WorkAreaId, "", false, ref dtEmployee);
                if (dtEmployee.Rows.Count > 0)
                {
                    rblEmployee.DataSource = dtEmployee;
                    rblEmployee.DataTextField = dtEmployee.Columns["EmployeeName"].ToString();
                    rblEmployee.DataValueField = dtEmployee.Columns["EmployeeId"].ToString();
                    rblEmployee.DataBind();
                }
                rblEmployee.SelectedValue = EmployeeId.ToString();
                hdnEmployeeId.Value = dtService.Rows[0]["EmployeeId"].ToString();

                lnkEmpSearch.Visible = false;

                txtScheduleOn.Text = Convert.ToDateTime(dtService.Rows[0]["ScheduleDate"].ToString()).ToString("MM/dd/yyyy");
                txtScheduleOn.Enabled = false;
                ltrScheduleDate.Text = Convert.ToDateTime(dtService.Rows[0]["ScheduleDate"].ToString()).ToString("MM/dd/yyyy");
                txtReschedule.Text = Convert.ToDateTime(dtService.Rows[0]["ScheduleDate"].ToString()).ToString("MM/dd/yyyy");

                hdnScheduleOn.Value = Convert.ToDateTime(dtService.Rows[0]["ScheduleDate"].ToString()).ToString("MM/dd/yyyy");

                txtStart.Value = dtService.Rows[0]["ScheduleStartTime"].ToString();
                txtStart.Disabled = true;
                txtEnd.Value = dtService.Rows[0]["ScheduleEndTime"].ToString();
                txtEnd.Disabled = true;
                ltrScheduleTime.Text = dtService.Rows[0]["ScheduleStartTime"].ToString() + " - " + dtService.Rows[0]["ScheduleEndTime"].ToString();

                hdnStart.Value = dtService.Rows[0]["ScheduleStartTime"].ToString();
                hdnEnd.Value = dtService.Rows[0]["ScheduleEndTime"].ToString();

                //drpServiceStatus.SelectedValue = dtService.Rows[0]["Status"].ToString();
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

        //private void FillServiceStatusDropdown()
        //{
        //    var values = DurationExtensions.GetValues<General.ServiceTypes>();
        //    List<string> data = new List<string>();
        //    foreach (var item in values)
        //    {
        //        General.ServiceTypes p = (General.ServiceTypes)item;
        //        data.Add(p.GetEnumDescription());
        //    }
        //    drpServiceStatus.DataSource = data;
        //    drpServiceStatus.DataBind();
        //}

        protected void lnkSearchClient_Click(object sender, EventArgs e)
        {
            dvMessage.InnerHtml = "";
            dvMessage.Visible = false;

            //if (!string.IsNullOrEmpty(txtClient.Text.Trim()))
            //{
                BindClientByClientName(txtClient.Text.Trim());

                rblAddress.DataSource = "";
                rblAddress.DataBind();
                chkUnits.DataSource = "";
                chkUnits.DataBind();
            //}
        }

        private void BindClientByClientName(string ClientName)
        {
            DataTable dtClients = new DataTable();
            objClientService = ServiceFactory.ClientService;
            objClientService.GetClientByName(ClientName, ref dtClients);
            if (dtClients.Rows.Count > 0)
            {
                rblClient.DataSource = dtClients;
                rblClient.DataTextField = dtClients.Columns["ClientName"].ToString();
                rblClient.DataValueField = dtClients.Columns["Id"].ToString();
            }
            else
                rblClient.DataSource = "";
            rblClient.DataBind();
        }

        protected void rblAddress_SelectedIndexChanged(object sender, EventArgs e)
        {
            dvMessage.InnerHtml = "";
            dvMessage.Visible = false;

            BindUnitsByClientAndAddressId(Convert.ToInt32(rblClient.SelectedValue), Convert.ToInt32(rblAddress.SelectedValue));
            BindWorkAreaByAddressId(Convert.ToInt32(rblAddress.SelectedValue.ToString()));

        }

        private void BindWorkAreaByAddressId(int AddressId)
        {
            DataTable dtWorkAreas = new DataTable();
            objAreasService = ServiceFactory.AreasService;
            objAreasService.GetAllAreasByClientAddressId(AddressId, ref dtWorkAreas);
            if (dtWorkAreas.Rows.Count > 0)
            {
                rblWorkArea.DataSource = dtWorkAreas;
                rblWorkArea.DataValueField = dtWorkAreas.Columns["Id"].ToString();
                rblWorkArea.DataTextField = dtWorkAreas.Columns["Name"].ToString();
                rblWorkArea.DataBind();
            }
        }

        private void BindUnitsByClientAndAddressId(int ClientId, int AddressId)
        {
            objClientUnitService = ServiceFactory.ClientUnitService;
            DataTable dtUnits = new DataTable();
            objClientUnitService.GetClientUnitByClientAndAddressId(ClientId, AddressId, ref dtUnits);
            if (dtUnits.Rows.Count > 0)
            {
                chkUnits.DataSource = dtUnits;
                chkUnits.DataTextField = dtUnits.Columns["UnitName"].ToString();
                chkUnits.DataValueField = dtUnits.Columns["Id"].ToString();

                BindTimeSlotByClientAddressIdAndPlanTypeId(ClientId, AddressId, Convert.ToInt32(dtUnits.Rows[0]["PlanTypeId"].ToString()));
            }
            else
                chkUnits.DataSource = "";
            chkUnits.DataBind();
        }

        protected void rblClient_SelectedIndexChanged(object sender, EventArgs e)
        {
            dvMessage.InnerHtml = "";
            dvMessage.Visible = false;

            BindAddressByClientId(Convert.ToInt32(rblClient.SelectedValue));
            BindContactInfoByClientId(Convert.ToInt32(rblClient.SelectedValue));
            rblWorkArea.DataSource = "";
            rblWorkArea.DataBind();

            rblEmployee.DataSource = "";
            rblEmployee.DataBind();
        }

        private void BindAddressByClientId(int ClientId)
        {
            objClientAddressService = ServiceFactory.ClientAddressService;
            DataTable dtAddress = new DataTable();
            if (!string.IsNullOrEmpty(Request.QueryString["ServiceId"]))
                objClientAddressService.GetClientAddressesByClientId(ClientId, true, ref dtAddress);
            else
                objClientAddressService.GetClientAddressesByClientId(ClientId, false, ref dtAddress);

            if (dtAddress.Rows.Count > 0)
            {
                rblAddress.DataSource = dtAddress;
                rblAddress.DataTextField = dtAddress.Columns["ClientAddress"].ToString();
                rblAddress.DataValueField = dtAddress.Columns["Id"].ToString();
            }
            else
                rblAddress.DataSource = "";
            rblAddress.DataBind();
        }

        private void BindContactInfoByClientId(int ClientId)
        {
            objClientService = ServiceFactory.ClientService;
            DataTable dtClient = new DataTable();
            objClientService.GetClientById(ClientId, ref dtClient);
            if (dtClient.Rows.Count > 0)
            {
                ltrMobile.Text = dtClient.Rows[0]["MobileNumber"].ToString();
                ltrHome.Text = dtClient.Rows[0]["HomeNumber"].ToString();
                ltrOffice.Text = dtClient.Rows[0]["OfficeNumber"].ToString();
            }
        }

        protected void lnkAreaSearch_Click(object sender, EventArgs e)
        {
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
            rblEmployee.DataSource = "";
            rblEmployee.DataBind();
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
                rblEmployee.DataTextField = dtEmployees.Columns["EmployeeName"].ToString();
                rblEmployee.DataValueField = dtEmployees.Columns["EmployeeId"].ToString();
                rblEmployee.DataBind();
            }
        }

        protected void btnSchedule_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    if (Session["LoginSession"] != null)
                    {
                        if (rblClient.Items.Count == 0 || rblClient.SelectedIndex == -1)
                        {
                            dvMessage.InnerHtml = "<strong>Please Select Client</strong>";
                            dvMessage.Attributes.Add("class", "alert alert-error");
                            dvMessage.Visible = true;
                            return;
                        }
                        if (rblAddress.Items.Count == 0 || rblAddress.SelectedIndex == -1)
                        {
                            dvMessage.InnerHtml = "<strong>Please Select Address</strong>";
                            dvMessage.Attributes.Add("class", "alert alert-error");
                            dvMessage.Visible = true;
                            return;
                        }
                        if (rblWorkArea.Items.Count == 0 || rblWorkArea.SelectedIndex == -1)
                        {
                            dvMessage.InnerHtml = "<strong>Please Select Work Area.</strong>";
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

                        TimeSpan StartTime = new TimeSpan();
                        StartTime = DateTime.ParseExact(txtStart.Value.Trim(), "hh:mm tt", CultureInfo.CurrentCulture).TimeOfDay;
                        TimeSpan EndTime = new TimeSpan();
                        EndTime = DateTime.ParseExact(txtEnd.Value.Trim(), "hh:mm tt", CultureInfo.CurrentCulture).TimeOfDay;

                        if (EndTime.Ticks<StartTime.Ticks)
                        {
                            dvMessage.InnerHtml = "<strong>Schedule End Time must be Greater than Schedule Start Time.</strong>";
                            dvMessage.Attributes.Add("class", "alert alert-error");
                            dvMessage.Visible = true;
                            return;
                        }

                        if (StartTime.Ticks==EndTime.Ticks)
                        {
                            dvMessage.InnerHtml = "<strong>Service Start Time is same as End Time.</strong>";
                            dvMessage.Attributes.Add("class", "alert alert-error");
                            dvMessage.Visible = true;
                            return;
                        }


                        LoginModel objLoginModel = new LoginModel();
                        objLoginModel = Session["LoginSession"] as LoginModel;

                        long ServiceId = 0;
                        BizObjects.Services objServices = new BizObjects.Services();
                        BizObjects.ServiceUnits objServiceUnits = new BizObjects.ServiceUnits();
                        objServicesService = ServiceFactory.ServicesService;
                        objServiceUnitService = ServiceFactory.ServiceUnitService;
                        DataTable dtService = new DataTable();

                        objServices.ClientId = Convert.ToInt32(rblClient.SelectedValue.ToString());
                        objServices.AddressID = Convert.ToInt32(rblAddress.SelectedValue.ToString());
                        objServices.PurposeOfVisit = drpPurpose.SelectedValue;
                        objServices.CustomerComplaints = txtCustomerNote.Text.Trim();
                        objServices.DispatcherNotes = txtDispatcherNote.Text.Trim();
                        objServices.TechnicianNotes = txtEmpNote.Text.Trim();
                        objServices.Status = General.ServiceTypes.Scheduled.GetEnumDescription();//drpServiceStatus.SelectedValue.ToString();//
                        objServices.ScheduledBy = objLoginModel.Id;
                        objServices.AddedBy = objLoginModel.Id;
                        objServices.AddedByType = objLoginModel.RoleId;
                        objServices.AddedDate = DateTime.UtcNow;

                        if (!string.IsNullOrEmpty(Request.QueryString["ServiceId"]))
                        {
                            ServiceId = Convert.ToInt64(Request.QueryString["ServiceId"].ToString());
                            objServices.Id = ServiceId;
                            objServices.UpdatedBy = objLoginModel.Id;
                            objServices.UpdatedByType = objLoginModel.RoleId;
                            objServices.UpdatedDate = DateTime.UtcNow;

                            //Below function also delete records from ServiceUnits
                            objServicesService.UpdateService(ref objServices);
                        }
                        else
                        {
                            objServices.WorkAreaId = Convert.ToInt32(rblWorkArea.SelectedValue);
                            objServices.EmployeeId = Convert.ToInt32(rblEmployee.SelectedValue);
                            objServices.ScheduleDate = Convert.ToDateTime(txtScheduleOn.Text.Trim());
                            objServices.ScheduleStartTime = txtStart.Value.Trim();
                            objServices.ScheduleEndTime = txtEnd.Value.Trim();
                            ServiceId = objServicesService.InsertWaitingApprovalService(ref objServices, 0);
                        }

                        if (ServiceId > 0)
                        {
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

                            if (string.IsNullOrEmpty(Request.QueryString["ServiceId"]))
                            {
                                bool IsPartInStock = false;
                                objServicesService.ScheduleService(ServiceId, ref dtService);

                                if (dtService.Rows.Count > 0)
                                {
                                    long RtnService = Convert.ToInt64(dtService.Rows[0]["ServiceId"].ToString());
                                    IsPartInStock = Convert.ToBoolean(dtService.Rows[0]["IsPartInStock"].ToString());

                                    switch (RtnService)
                                    {
                                        case -1:
                                            Response.Redirect(Application["SiteAddress"] + "admin/ScheduledService_List.aspx?msg=-1");
                                            break;
                                        case -2:
                                            Response.Redirect(Application["SiteAddress"] + "admin/ScheduledService_List.aspx?msg=-2");
                                            break;
                                        case -3:
                                            Response.Redirect(Application["SiteAddress"] + "admin/ScheduledService_List.aspx?msg=-3");
                                            break;
                                        case -4:
                                            Response.Redirect(Application["SiteAddress"] + "admin/ScheduledService_List.aspx?msg=-4");
                                            break;
                                        case -5:
                                            Response.Redirect(Application["SiteAddress"] + "admin/ScheduledService_List.aspx?msg=-5");
                                            break;
                                        case -6:
                                            Response.Redirect(Application["SiteAddress"] + "admin/ScheduledService_List.aspx?msg=-6");
                                            break;
                                        case -7:
                                            Response.Redirect(Application["SiteAddress"] + "admin/ScheduledService_List.aspx?msg=-7");
                                            break;
                                        default:
                                            break;
                                    }
                                }


                                if (IsPartInStock)
                                {
                                    objEmployeeService = ServiceFactory.EmployeeService;
                                    DataTable dtEmployee = new DataTable();
                                    objEmployeeService.GetEmployeeById(Convert.ToInt32(rblEmployee.SelectedValue), ref dtEmployee);
                                    if (dtEmployee.Rows.Count > 0)
                                    {
                                        long NotificationId = 0;
                                        int BadgeCount = 0;
                                        int EmployeeId = Convert.ToInt32(rblEmployee.SelectedValue);
                                        BizObjects.UserNotification objUserNotification = new BizObjects.UserNotification();
                                        objUserNotificationService = ServiceFactory.UserNotificationService;
                                        string message = General.GetNotificationMessage("EmployeeSchedule");//"System has scheduled a service for you on " + Convert.ToDateTime(txtScheduleOn.Text.Trim()).ToString("MMMM dd, yyyy") + ".";
                                        message = message.Replace("{{ScheduleDate}}", Convert.ToDateTime(txtScheduleOn.Text.Trim()).ToString("MMMM dd, yyyy"));
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


                        //if (drpServiceStatus.SelectedValue==General.ServiceTypes.Completed.GetEnumDescription())
                        //{
                        //    objClientUnitService = ServiceFactory.ClientUnitService;
                        //    objClientUnitService.SetStatusByServiceId(General.UnitStatus.Serviced.GetEnumValue(), ServiceId);    
                        //}

                        //if (drpServiceStatus.SelectedValue == General.ServiceTypes.WaitingApproval.GetEnumDescription() || drpServiceStatus.SelectedValue == General.ServiceTypes.Pending.GetEnumDescription()
                        //    || drpServiceStatus.SelectedValue == General.ServiceTypes.Scheduled.GetEnumDescription())
                        //{
                        //    objClientUnitService = ServiceFactory.ClientUnitService;
                        //    objClientUnitService.SetStatusByServiceId(General.UnitStatus.ServiceSoon.GetEnumValue(), ServiceId);
                        //}

                        if (!string.IsNullOrEmpty(Request.QueryString["ServiceId"]))
                            Response.Redirect(Application["SiteAddress"] + "admin/ScheduledService_List.aspx?msg=edit");
                        else
                            Response.Redirect(Application["SiteAddress"] + "admin/ScheduledService_List.aspx?msg=add");
                    }
                    else
                        Response.Redirect(Application["SiteAddress"] + "/admin/Login.aspx");
                }
                catch (Exception Ex)
                {
                    dvMessage.InnerHtml = "<strong>Error!</strong> " + Ex.Message.ToString().Trim();
                    dvMessage.Attributes.Add("class", "alert alert-error");
                    dvMessage.Visible = true;
                }
            }
        }

        //protected void btnReschedule_Click(object sender, EventArgs e)
        //{
        //    if (Page.IsValid)
        //    {
        //        try
        //        {
        //            if (Request.Cookies["LoginCookie"] != null)
        //            {
        //                if (rblClient.Items.Count == 0 || rblClient.SelectedIndex == -1)
        //                {
        //                    dvMessage.InnerHtml = "<strong>Please Select Client</strong>";
        //                    dvMessage.Attributes.Add("class", "alert alert-error");
        //                    dvMessage.Visible = true;
        //                    return;
        //                }
        //                if (rblAddress.Items.Count == 0 || rblAddress.SelectedIndex == -1)
        //                {
        //                    dvMessage.InnerHtml = "<strong>Please Select Address</strong>";
        //                    dvMessage.Attributes.Add("class", "alert alert-error");
        //                    dvMessage.Visible = true;
        //                    return;
        //                }
        //                if (rblWorkArea.Items.Count == 0 || rblWorkArea.SelectedIndex == -1)
        //                {
        //                    dvMessage.InnerHtml = "<strong>Please Select Work Area.</strong>";
        //                    dvMessage.Attributes.Add("class", "alert alert-error");
        //                    dvMessage.Visible = true;
        //                    return;
        //                }
        //                if (rblEmployee.Items.Count == 0 || rblEmployee.SelectedIndex == -1)
        //                {
        //                    dvMessage.InnerHtml = "<strong>Please Select Employee.</strong>";
        //                    dvMessage.Attributes.Add("class", "alert alert-error");
        //                    dvMessage.Visible = true;
        //                    return;
        //                }
        //                if (chkUnits.SelectedIndex == -1)
        //                {
        //                    dvMessage.InnerHtml = "<strong>Please Select Service Units.</strong>";
        //                    dvMessage.Attributes.Add("class", "alert alert-error");
        //                    dvMessage.Visible = true;
        //                    return;
        //                }

        //                long ServiceId = 0;
        //                BizObjects.RescheduleService objRescheduleService = new BizObjects.RescheduleService();
        //                objRescheduleServicesService = ServiceFactory.RescheduleServicesService;
        //                if (!string.IsNullOrEmpty(Request.QueryString["ServiceId"]))
        //                {
        //                    ServiceId = Convert.ToInt64(Request.QueryString["ServiceId"].ToString());
        //                    objRescheduleService.ServiceId = ServiceId;
        //                    objRescheduleService.RescheduleDate = Convert.ToDateTime(txtScheduleOn.Text.Trim());
        //                    objRescheduleService.Rescheduletime = txtStart.Value.Trim() + " - " + txtEnd.Value.Trim();
        //                    objRescheduleService.Reason = txtRescheduleReason.Text;
        //                    objRescheduleService.AddedBy = Convert.ToInt32(Request.Cookies["LoginCookie"]["UserId"].ToString());
        //                    objRescheduleService.AddedByType = Convert.ToInt32(Request.Cookies["LoginCookie"]["RoleId"].ToString());
        //                    objRescheduleService.AddedDate = DateTime.UtcNow;

        //                    objRescheduleServicesService.AddRescheduleService(ref objRescheduleService);

        //                    Response.Redirect(Application["SiteAddress"] + "admin/ScheduledService_List.aspx?msg=reschedule");
        //                }
        //            }
        //            else
        //                Response.Redirect(Application["SiteAddress"] + "/admin/Login.aspx");
        //        }
        //        catch (Exception Ex)
        //        {
        //            dvMessage.InnerHtml = "<strong>Error!</strong> " + Ex.Message.ToString().Trim();
        //            dvMessage.Attributes.Add("class", "alert alert-error");
        //            dvMessage.Visible = true;
        //        }
        //    }
        //}

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

        private void BindTimeSlotByClientAddressIdAndPlanTypeId(int ClientId, int AddressId, int PlanTypeId)
        {
            objClientUnitService = ServiceFactory.ClientUnitService;
            DataTable dtUnits = new DataTable();
            objClientUnitService.GetUnitsByClientAddressAndPlanId(ClientId, AddressId, PlanTypeId, ref dtUnits);
            if (dtUnits.Rows.Count > 0)
            {
                hdnServiceTime.Value = dtUnits.Rows[0]["ServiceSlot1"].ToString() + "|" + dtUnits.Rows[0]["ServiceSlot2"].ToString();
                List<string> slot1 = dtUnits.Rows[0]["ServiceSlot1"].ToString().Split(("-").ToArray()).ToList();
                List<string> slot2 = dtUnits.Rows[0]["ServiceSlot2"].ToString().Split(("-").ToArray()).ToList();
                Plan p = new Plan();
                p.ServiceTimeForFirstUnit = int.Parse(dtUnits.Rows[0]["ServiceTimeForFirstUnit"].ToString());
                p.ServiceTimeForAdditionalUnits = int.Parse(dtUnits.Rows[0]["ServiceTimeForAdditionalUnits"].ToString());
                var lunchtime = General.GetSitesettingsValue("EmployeeLunchTime");
                var Slot1cnt = General.Slottime1(slot1.ToArray(), slot2.ToArray(), p, lunchtime);
                var Slot2cnt = General.Slottime1(new List<string>().ToArray(), slot2.ToArray(), p, lunchtime);
                firstslotunits.Value = Slot1cnt.ToString();
                secondslotunits.Value = Slot2cnt.ToString();
                hdnDrivetime.Value = dtUnits.Rows[0]["Drivetime"].ToString();
                hdnServiceTimeForFirstUnit.Value = dtUnits.Rows[0]["ServiceTimeForFirstUnit"].ToString();
                hdnServiceTimeForAdditionalUnits.Value = dtUnits.Rows[0]["ServiceTimeForAdditionalUnits"].ToString();
            }
            else
                hdnServiceTime.Value = "";
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
                        string ClientDeviceType = string.Empty;
                        string ClientDeviceToken = string.Empty;
                        string EmpDeviceType = string.Empty;
                        string EmpDeviceToken = string.Empty;
                        objLoginModel = Session["LoginSession"] as LoginModel;
                        if (!string.IsNullOrEmpty(Request.QueryString["ServiceId"]))
                        {
                            long ServiceId = Convert.ToInt64(Request.QueryString["ServiceId"].ToString());
                            DataTable dtService = new DataTable();
                            objServicesService = ServiceFactory.ServicesService;
                            objServicesService.GetServiceById(ServiceId, ref dtService);
                            int ClientId=0;
                            if (dtService.Rows.Count > 0)
                                ClientId = Convert.ToInt32(dtService.Rows[0]["ClientId"].ToString());

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
                            if (dtClient.Rows.Count > 0)
                            {
                                ClientDeviceType = dtClient.Rows[0]["DeviceType"].ToString();
                                ClientDeviceToken = dtClient.Rows[0]["DeviceToken"].ToString();

                                //BizObjects.UserNotification objUserNotification = new BizObjects.UserNotification();
                                string message = General.GetNotificationMessage("RescheduleServiceSendToClient"); //"Your Service at " + ClientName + "’s address on " + Convert.ToDateTime(txtScheduleOn.Text.Trim()).ToString("MMMM dd, yyyy") + " has been rescheduled.";
                                message = message.Replace("{{EmpName}}", objLoginModel.Username);
                                message = message.Replace("{{ScheduleDate}}", Convert.ToDateTime(txtScheduleOn.Text.Trim()).ToString("MMMM dd, yyyy"));

                                NotifyUser(ClientId, "client", General.NotificationType.FriendlyReminder.GetEnumDescription(),
                                        General.NotificationType.FriendlyReminder.GetEnumValue(), ServiceId, message, ClientDeviceType, ClientDeviceToken);

                                //objUserNotification.UserId = ClientId;
                                //objUserNotification.UserTypeId = General.UserRoles.Client.GetEnumValue();
                                //objUserNotification.Message = message;
                                //objUserNotification.CommonId = ServiceId;
                                //objUserNotification.Status = General.NotificationStatus.UnRead.GetEnumDescription();
                                //objUserNotification.MessageType = General.NotificationType.FriendlyReminder.GetEnumDescription();
                                //objUserNotification.AddedDate = DateTime.UtcNow;

                                //NotificationId = objUserNotificationService.AddUserNotification(ref objUserNotification);

                                //DataTable dtBadgeCount = new DataTable();
                                //objUserNotificationService.GetBadgeCount(ClientId, General.UserRoles.Client.GetEnumValue(), ref dtBadgeCount);
                                //BadgeCount = dtBadgeCount.Rows.Count;

                                //Notifications objNotifications = new Notifications { NId = NotificationId, NType = General.NotificationType.FriendlyReminder.GetEnumValue(), CommonId = ServiceId };
                                //List<NotificationModel> notify = new List<NotificationModel>();
                                //notify.Add(new NotificationModel { Key = "NId", Value = new object[] { objNotifications.NId } });
                                //notify.Add(new NotificationModel { Key = "NType", Value = new object[] { objNotifications.NType } });
                                //notify.Add(new NotificationModel { Key = "CommonId", Value = new object[] { objNotifications.CommonId } });

                                //if (!string.IsNullOrEmpty(dtClient.Rows[0]["DeviceType"].ToString()) &&
                                //                !string.IsNullOrEmpty(dtClient.Rows[0]["DeviceToken"].ToString()) &&
                                //                dtClient.Rows[0]["DeviceToken"].ToString().ToLower() != "no device token")
                                //{
                                //    if (dtClient.Rows[0]["DeviceType"].ToString().ToLower() == "android")
                                //    {
                                //        string CustomData = "&data.NId=" + objNotifications.NId + "&data.NType=" + objNotifications.NType + "&data.CommonId=" + objNotifications.CommonId;
                                //        SendNotifications.SendAndroidNotification(dtClient.Rows[0]["DeviceToken"].ToString(), message, CustomData, "client");
                                //    }
                                //    else if (dtClient.Rows[0]["DeviceType"].ToString().ToLower() == "iphone")
                                //    {
                                //        SendNotifications.SendIphoneNotification(BadgeCount, dtClient.Rows[0]["DeviceToken"].ToString(), message, notify, "client");
                                //    }
                                //}
                            }

                            //Send Notification to Employee
                            if (!string.IsNullOrEmpty(hdnEmployeeId.Value) && !string.IsNullOrEmpty(hdnClientName.Value))
                            {
                                string ClientName = hdnClientName.Value;
                                int EmployeeId = Convert.ToInt32(hdnEmployeeId.Value);
                                objUserNotificationService = ServiceFactory.UserNotificationService;
                                objEmployeeService = ServiceFactory.EmployeeService;
                                DataTable dtEmployee = new DataTable();
                                objEmployeeService.GetEmployeeById(EmployeeId, ref dtEmployee);
                                if (dtEmployee.Rows.Count > 0)
                                {
                                    EmpDeviceType = dtEmployee.Rows[0]["DeviceType"].ToString();
                                    EmpDeviceToken = dtEmployee.Rows[0]["DeviceToken"].ToString();

                                    //long NotificationId = 0;
                                    //int BadgeCount = 0;
                                    //BizObjects.UserNotification objUserNotification = new BizObjects.UserNotification();
                                    string message = General.GetNotificationMessage("RescheduleServiceSendToEmployee"); //"Your Service at " + ClientName + "’s address on " + Convert.ToDateTime(txtScheduleOn.Text.Trim()).ToString("MMMM dd, yyyy") + " has been rescheduled.";
                                    message = message.Replace("{{ClientName}}", ClientName);
                                    message = message.Replace("{{ScheduleDate}}", Convert.ToDateTime(txtScheduleOn.Text.Trim()).ToString("MMMM dd, yyyy"));

                                    NotifyUser(EmployeeId, "employee", General.NotificationType.FriendlyReminder.GetEnumDescription(),
                                        General.NotificationType.FriendlyReminder.GetEnumValue(), ServiceId, message, EmpDeviceType, EmpDeviceToken);

                                    //objUserNotification.UserId = EmployeeId;
                                    //objUserNotification.UserTypeId = General.UserRoles.Employee.GetEnumValue();
                                    //objUserNotification.Message = message;
                                    //objUserNotification.CommonId = ServiceId;
                                    //objUserNotification.Status = General.NotificationStatus.UnRead.GetEnumDescription();
                                    //objUserNotification.MessageType = General.NotificationType.FriendlyReminder.GetEnumDescription();
                                    //objUserNotification.AddedDate = DateTime.UtcNow;

                                    //NotificationId = objUserNotificationService.AddUserNotification(ref objUserNotification);

                                    //DataTable dtBadgeCount = new DataTable();
                                    //objUserNotificationService.GetBadgeCount(EmployeeId, General.UserRoles.Employee.GetEnumValue(), ref dtBadgeCount);
                                    //BadgeCount = dtBadgeCount.Rows.Count;

                                    //Notifications objNotifications = new Notifications { NId = NotificationId, NType = General.NotificationType.FriendlyReminder.GetEnumValue(), CommonId = ServiceId };
                                    //List<NotificationModel> notify = new List<NotificationModel>();
                                    //notify.Add(new NotificationModel { Key = "NId", Value = new object[] { objNotifications.NId } });
                                    //notify.Add(new NotificationModel { Key = "NType", Value = new object[] { objNotifications.NType } });
                                    //notify.Add(new NotificationModel { Key = "CommonId", Value = new object[] { objNotifications.CommonId } });

                                    //if (!string.IsNullOrEmpty(dtEmployee.Rows[0]["DeviceType"].ToString()) &&
                                    //        !string.IsNullOrEmpty(dtEmployee.Rows[0]["DeviceToken"].ToString()) &&
                                    //         dtEmployee.Rows[0]["DeviceToken"].ToString().ToLower() != "no device token")
                                    //{
                                    //    if (dtEmployee.Rows[0]["DeviceType"].ToString().ToLower() == "iphone")
                                    //    {
                                    //        SendNotifications.SendIphoneNotification(BadgeCount, dtEmployee.Rows[0]["DeviceToken"].ToString(), message, notify, "employee");
                                    //    }
                                    //}
                                }
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
                                DataTable dtServices = new DataTable();
                                string message = string.Empty;
                                objRequestServicesService = ServiceFactory.RequestServicesService;
                                objRequestServicesService.ScheduleRequestedService(ServiceId, 0, ClientId, Convert.ToInt32(rblAddress.SelectedValue.ToString()), drpPurpose.SelectedValue, Convert.ToDateTime(txtReschedule.Text.Trim()), RescheduleTime, ref dtServices);
                                if (dtServices.Rows.Count > 0)
                                {
                                    if (Convert.ToInt32(dtServices.Rows[0]["EmployeeId"].ToString()) != 0
                                && Convert.ToInt32(dtServices.Rows[0]["ServiceId"].ToString()) != 0)
                                    {
                                        int EId = Convert.ToInt32(dtServices.Rows[0]["EmployeeId"].ToString());
                                        DateTime ScheduleDate = Convert.ToDateTime(dtServices.Rows[0]["ScheduleDate"].ToString());

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

        protected void btnAssignEmployee_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    dvMessage.InnerHtml = "";
                    dvMessage.Visible = false;

                    if (rblWorkArea.Items.Count == 0 || rblWorkArea.SelectedIndex == -1)
                    {
                        dvMessage.InnerHtml = "<strong>Please Select Work Area.</strong>";
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

                    TimeSpan StartTime = new TimeSpan();
                    StartTime = DateTime.ParseExact(txtStart.Value.Trim(), "hh:mm tt", CultureInfo.CurrentCulture).TimeOfDay;
                    TimeSpan EndTime = new TimeSpan();
                    EndTime = DateTime.ParseExact(txtEnd.Value.Trim(), "hh:mm tt", CultureInfo.CurrentCulture).TimeOfDay;

                    if (EndTime.Ticks < StartTime.Ticks)
                    {
                        dvMessage.InnerHtml = "<strong>Schedule End Time must be Greater than Schedule Start Time.</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-error");
                        dvMessage.Visible = true;
                        return;
                    }

                    if (StartTime.Ticks == EndTime.Ticks)
                    {
                        dvMessage.InnerHtml = "<strong>Service Start Time is same as End Time.</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-error");
                        dvMessage.Visible = true;
                        return;
                    }

                    if (!string.IsNullOrEmpty(Request.QueryString["ServiceId"]))
                    {
                        long ServiceId = Convert.ToInt64(Request.QueryString["ServiceId"].ToString());

                        if (hdnWorkAreaId.Value != rblWorkArea.SelectedValue ||
                            hdnEmployeeId.Value != rblEmployee.SelectedValue ||
                            hdnScheduleOn.Value != txtScheduleOn.Text ||
                            hdnStart.Value != txtStart.Value ||
                            hdnEnd.Value != txtEnd.Value)
                        {
                            objServicesService = ServiceFactory.ServicesService;
                            DataTable dtServices = new DataTable();
                            objServicesService.AssignEmployeeFromAdmin(ServiceId, Convert.ToInt32(rblWorkArea.SelectedValue.ToString()),
                                Convert.ToInt32(rblEmployee.SelectedValue.ToString()), Convert.ToDateTime(txtScheduleOn.Text.Trim()),
                                txtStart.Value, txtEnd.Value, ref dtServices);
                            if (dtServices.Rows.Count > 0)
                            {
                                int CommonId = Convert.ToInt32(dtServices.Rows[0]["CommonId"].ToString());
                                switch (CommonId)
                                {
                                    case -1:
                                        dvMessage.InnerHtml = "<strong>Selected Units has multiple plan type.</strong>";
                                        dvMessage.Attributes.Add("class", "alert alert-error");
                                        dvMessage.Visible = true;
                                        break;
                                    case -2:
                                        dvMessage.InnerHtml = "<strong>Selected Employee Workarea did not match with client's zip code.</strong>";
                                        dvMessage.Attributes.Add("class", "alert alert-error");
                                        dvMessage.Visible = true;
                                        break;
                                    case -3:
                                        dvMessage.InnerHtml = "<strong>Selected Employee Working time did not match.</strong>";
                                        dvMessage.Attributes.Add("class", "alert alert-error");
                                        dvMessage.Visible = true;
                                        break;
                                    case -4:
                                        dvMessage.InnerHtml = "<strong>Selected Employee is on leave.</strong>";
                                        dvMessage.Attributes.Add("class", "alert alert-error");
                                        dvMessage.Visible = true;
                                        break;
                                    case -5:
                                        dvMessage.InnerHtml = "<strong>Employee is already assigned to another service for selected timings.</strong>";
                                        dvMessage.Attributes.Add("class", "alert alert-error");
                                        dvMessage.Visible = true;
                                        break;
                                    case -6:
                                        string PartsNotFound = dtServices.Rows[0]["Description"].ToString();
                                        dvMessage.InnerHtml = "<strong>Some of the parts like " + PartsNotFound + " not in stock.</strong>";
                                        dvMessage.Attributes.Add("class", "alert alert-error");
                                        dvMessage.Visible = true;
                                        break;
                                    case -7:
                                        dvMessage.InnerHtml = "<strong>Parts Not Found.</strong>";
                                        dvMessage.Attributes.Add("class", "alert alert-error");
                                        dvMessage.Visible = true;
                                        break;
                                    default:
                                        SendNotificationForServiceSchedule(ServiceId, Convert.ToInt32(rblClient.SelectedValue.ToString()), Convert.ToInt32(rblEmployee.SelectedValue.ToString()), Convert.ToDateTime(txtScheduleOn.Text.Trim()));
                                        Response.Redirect(Application["SiteAddress"] + "admin/ScheduledService_List.aspx?msg=edit");
                                        break;
                                }
                            }
                        }
                        else
                        {
                            dvMessage.InnerHtml = "<strong>Please Change anyone from Workarea, Employee, Service Date, Start Time & End Time.</strong>";
                            dvMessage.Attributes.Add("class", "alert alert-error");
                            dvMessage.Visible = true;
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

        protected void rblWorkArea_SelectedIndexChanged(object sender, EventArgs e)
        {
            rblEmployee.DataSource = "";
            rblEmployee.DataBind();

            DataTable dtEmployee = new DataTable();
            objEmployeeWorkAreaService = ServiceFactory.EmployeeWorkAreaService;

            objEmployeeWorkAreaService.GetAllEmployeeByAreaId(Convert.ToInt32(rblWorkArea.SelectedValue.ToString()), "", false, ref dtEmployee);
            if (dtEmployee.Rows.Count > 0)
            {
                rblEmployee.DataSource = dtEmployee;
                rblEmployee.DataTextField = dtEmployee.Columns["EmployeeName"].ToString();
                rblEmployee.DataValueField = dtEmployee.Columns["EmployeeId"].ToString();
                rblEmployee.DataBind();
            }
        }

        private void SendNotificationForServiceSchedule(long ServiceId, int ClientId, int EmployeeId, DateTime ScheduleDate)
        {
            string message = string.Empty;
            objClientService = ServiceFactory.ClientService;
            string ClientDeviceType = string.Empty;
            string ClientDeviceToken = string.Empty;
            string EmpDeviceType = string.Empty;
            string EmpDeviceToken = string.Empty;
            DataTable dtClient = new DataTable();
            objEmployeeService = ServiceFactory.EmployeeService;
            DataTable dtEmployee = new DataTable();

            objClientService.GetClientById(ClientId, ref dtClient);
            if (dtClient.Rows.Count > 0)
            {
                ClientDeviceType = dtClient.Rows[0]["DeviceType"].ToString();
                ClientDeviceToken = dtClient.Rows[0]["DeviceToken"].ToString();
            }

            objEmployeeService.GetEmployeeById(EmployeeId, ref dtEmployee);
            if (dtEmployee.Rows.Count > 0)
            {
                EmpDeviceType = dtEmployee.Rows[0]["DeviceType"].ToString();
                EmpDeviceToken = dtEmployee.Rows[0]["DeviceToken"].ToString();
            }


            message = General.GetNotificationMessage("PendingServiceScheduleSendToClient");
            message = message.Replace("{{ScheduleDate}}", ScheduleDate.ToString("MMMM dd, yyyy"));

            NotifyUser(ClientId, "client", General.NotificationType.PeriodicServiceReminder.GetEnumDescription(), General.NotificationType.PeriodicServiceReminder.GetEnumValue()
                , ServiceId, message, ClientDeviceType, ClientDeviceToken);

            //send notification to employee

            message = General.GetNotificationMessage("EmployeeSchedule");
            message = message.Replace("{{ScheduleDate}}", ScheduleDate.ToString("MMMM dd, yyyy"));

            NotifyUser(EmployeeId, "employee", General.NotificationType.ServiceScheduled.GetEnumDescription(), General.NotificationType.ServiceScheduled.GetEnumValue()
                , ServiceId, message, EmpDeviceType, EmpDeviceToken);

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

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(Request.QueryString["ServiceId"]))
                {
                    long ServiceId = Convert.ToInt64(Request.QueryString["ServiceId"].ToString());
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

                    LoginModel objLoginModel = new LoginModel();
                    objLoginModel = Session["LoginSession"] as LoginModel;

                    BizObjects.RescheduleService objRescheduleService = new BizObjects.RescheduleService();
                    objRescheduleServicesService = ServiceFactory.RescheduleServicesService;

                    objRescheduleService.ServiceId = ServiceId;
                    objRescheduleService.RescheduleDate = Convert.ToDateTime(txtReschedule.Text.Trim());
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

    }
}