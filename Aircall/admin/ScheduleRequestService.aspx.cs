using Aircall.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Services;
using System.Data;
using System.Globalization;

namespace Aircall.admin
{
    public partial class ScheduleRequestService : System.Web.UI.Page
    {
        IRequestServicesService objRequestServicesService;
        IClientAddressService objClientAddressService;
        IClientUnitService objClientUnitService;
        IRequestServiceUnitsService objRequestServiceUnitsService;
        IServicesService objServicesService;
        IServiceUnitService objServiceUnitService;
        IAreasService objAreasService;
        IEmployeeWorkAreaService objEmployeeWorkAreaService;
        IEmployeeService objEmployeeService;
        IUserNotificationService objUserNotificationService;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                FillPurposeOfVisitDropdown();
                if (!string.IsNullOrEmpty(Request.QueryString["ServiceId"]))
                {
                    txtStart.Value = "08:00 AM";
                    long ReqServiceId = Convert.ToInt64(Request.QueryString["ServiceId"].ToString());
                    DataTable dtReqService = new DataTable();
                    objRequestServicesService = ServiceFactory.RequestServicesService;
                    objRequestServicesService.GetRequestedServiceById(ReqServiceId, ref dtReqService);
                    if (dtReqService.Rows.Count > 0)
                    {
                        int ClientId = Convert.ToInt32(dtReqService.Rows[0]["ClientId"].ToString());
                        int PlanTypeId = Convert.ToInt32(dtReqService.Rows[0]["PlanTypeId"].ToString());
                        hdnClientId.Value = ClientId.ToString();
                        int AddressId = Convert.ToInt32(dtReqService.Rows[0]["AddressID"].ToString());
                        txtClient.Text = dtReqService.Rows[0]["ClientName"].ToString();
                        objClientAddressService = ServiceFactory.ClientAddressService;
                        DataTable dtAddress = new DataTable();
                        objClientAddressService.GetClientAddressesByClientId(ClientId, true, ref dtAddress);
                        if (dtAddress.Rows.Count > 0)
                        {
                            rblAddress.DataSource = dtAddress;
                            rblAddress.DataTextField = dtAddress.Columns["Address"].ToString();
                            rblAddress.DataValueField = dtAddress.Columns["Id"].ToString();
                            rblAddress.DataBind();
                            rblAddress.SelectedValue = AddressId.ToString();
                            rblAddress.Enabled = false;
                        }

                        hdnDrivetime.Value = dtReqService.Rows[0]["Drivetime"].ToString();
                        hdnServiceTimeForFirstUnit.Value = dtReqService.Rows[0]["ServiceTimeForFirstUnit"].ToString();
                        hdnServiceTimeForAdditionalUnits.Value = dtReqService.Rows[0]["ServiceTimeForAdditionalUnits"].ToString();

                        ltrMobile.Text = dtReqService.Rows[0]["MobileNumber"].ToString();
                        ltrHome.Text = dtReqService.Rows[0]["HomeNumber"].ToString();
                        ltrOffice.Text = dtReqService.Rows[0]["OfficeNumber"].ToString();
                        drpPurpose.SelectedValue = dtReqService.Rows[0]["PurposeOfVisit"].ToString();

                        ltrRequestedDate.Text = Convert.ToDateTime(dtReqService.Rows[0]["ServiceRequestedOn"].ToString()).ToString("MM/dd/yyyy");
                        txtScheduleOn.Text = Convert.ToDateTime(dtReqService.Rows[0]["ServiceRequestedOn"].ToString()).ToString("MM/dd/yyyy");
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
                            objRequestServiceUnitsService = ServiceFactory.RequestServiceUnitsService;
                            objRequestServiceUnitsService.GetRequestServiceUnitByRequestServiceId(ReqServiceId, ref dtServiceUnit);
                            if (dtServiceUnit.Rows.Count > 0)
                            {
                                for (int i = 0; i < dtServiceUnit.Rows.Count; i++)
                                {
                                    ListItem item = chkUnits.Items.FindByValue(dtServiceUnit.Rows[i]["UnitId"].ToString());
                                    item.Selected = true;
                                }
                            }
                        }
                        BindWorkAreaByAddressId(AddressId);
                        ltrRequestedTime.Text = dtReqService.Rows[0]["ServiceRequestedTime"].ToString();
                        txtCustomerNote.Text = dtReqService.Rows[0]["Notes"].ToString();
                    }
                }
                if (!string.IsNullOrEmpty(Request.QueryString["Date"]))
                    txtScheduleOn.Text = Convert.ToDateTime(Request.QueryString["Date"].ToString()).ToString("MM/dd/yyyy");
                if (!string.IsNullOrEmpty(Request.QueryString["StartTime"]))
                    txtStart.Value = Convert.ToDateTime(Request.QueryString["StartTime"]).ToString("hh:mm tt");
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

        protected void btnApprove_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    if (Session["LoginSession"] != null)
                    {
                        if (!string.IsNullOrEmpty(Request.QueryString["ServiceId"]))
                        {
                            long ReqServiceId = Convert.ToInt64(Request.QueryString["ServiceId"].ToString());
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

                            LoginModel objLoginModel = new LoginModel();
                            objLoginModel = Session["LoginSession"] as LoginModel;

                            DateTime StartTime = new DateTime();
                            DateTime EndTime = new DateTime();
                            string Start = string.Empty;
                            string End = string.Empty;

                            string LunchTime = General.GetSitesettingsValue("EmployeeLunchTime");
                            string[] LunchTimeArr = LunchTime.Split('-');
                            DateTime LunchStartTime = Convert.ToDateTime(DateTime.Now.Date.ToShortDateString() + " " + LunchTimeArr[0].ToString().Trim());
                            DateTime LunchEndTime = Convert.ToDateTime(DateTime.Now.Date.ToShortDateString() + " " + LunchTimeArr[1].ToString().Trim());

                            Start = DateTime.Now.Date.ToShortDateString() + " " + txtStart.Value.Trim();
                            StartTime = Convert.ToDateTime(Start);
                            End = DateTime.Now.Date.ToShortDateString() + " " + txtEnd.Value.Trim();
                            EndTime = Convert.ToDateTime(End);

                            if (drpPurpose.SelectedValue != General.PurposeOfVisit.Emergency.GetEnumDescription())
                            {
                                if (StartTime >= LunchStartTime && StartTime < LunchEndTime)
                                {
                                    dvMessage.InnerHtml = "<strong>Service are not assigned between " + LunchTime.ToString() + " (Lunch time).</strong>";
                                    dvMessage.Attributes.Add("class", "alert alert-error");
                                    dvMessage.Visible = true;
                                    return;
                                }
                            }

                            TimeSpan STime = new TimeSpan();
                            STime = DateTime.ParseExact(txtStart.Value.Trim(), "hh:mm tt", CultureInfo.CurrentCulture).TimeOfDay;
                            TimeSpan ETime = new TimeSpan();
                            ETime = DateTime.ParseExact(txtEnd.Value.Trim(), "hh:mm tt", CultureInfo.CurrentCulture).TimeOfDay;

                            if (ETime.Ticks < STime.Ticks)
                            {
                                dvMessage.InnerHtml = "<strong>Schedule End Time must be Greater than Schedule Start Time.</strong>";
                                dvMessage.Attributes.Add("class", "alert alert-error");
                                dvMessage.Visible = true;
                                return;
                            }

                            if (StartTime == EndTime)
                            {
                                dvMessage.InnerHtml = "<strong>Service Start Time is same as End Time.</strong>";
                                dvMessage.Attributes.Add("class", "alert alert-error");
                                dvMessage.Visible = true;
                                return;
                            }

                            BizObjects.Services objServices = new BizObjects.Services();
                            objServicesService = ServiceFactory.ServicesService;
                            DataTable dtService = new DataTable();
                            objServices.Id = ReqServiceId;
                            objServices.ClientId = Convert.ToInt32(hdnClientId.Value);
                            objServices.AddressID = Convert.ToInt32(rblAddress.SelectedValue);
                            objServices.PurposeOfVisit = drpPurpose.SelectedValue;
                            objServices.WorkAreaId = Convert.ToInt32(rblWorkArea.SelectedValue);
                            objServices.EmployeeId = Convert.ToInt32(rblEmployee.SelectedValue);
                            objServices.ScheduleDate = Convert.ToDateTime(txtScheduleOn.Text.Trim());
                            objServices.ScheduleStartTime = txtStart.Value;
                            objServices.ScheduleEndTime = txtEnd.Value;
                            objServices.CustomerComplaints = txtCustomerNote.Text.Trim();
                            objServices.DispatcherNotes = txtDispatcherNote.Text.Trim();
                            objServices.TechnicianNotes = txtEmpNote.Text.Trim();
                            objServices.ScheduledBy = objLoginModel.Id;
                            objServices.AddedBy = objLoginModel.Id;
                            objServices.AddedByType = objLoginModel.RoleId;
                            objServices.AddedDate = DateTime.UtcNow;

                            string UnitId = string.Empty;

                            foreach (ListItem item in chkUnits.Items)
                            {
                                if (item.Selected)
                                {
                                    if (string.IsNullOrEmpty(UnitId))
                                        UnitId = item.Value;
                                    else
                                        UnitId = UnitId + "," + item.Value;
                                }
                            }
                            long ServiceId = 0;
                            int EmployeeId = 0;
                            DateTime ScheduleDate;
                            objServicesService.ScheduleRequestedServiceFromAdmin(ref objServices, UnitId, ref dtService);
                            if (dtService.Rows.Count > 0)
                            {
                                for (int i = 0; i < dtService.Rows.Count; i++)
                                {
                                    ServiceId = Convert.ToInt64(dtService.Rows[i]["ServiceId"].ToString());
                                    EmployeeId = Convert.ToInt32(dtService.Rows[i]["EmployeeId"].ToString());
                                    ScheduleDate=Convert.ToDateTime(dtService.Rows[i]["ScheduleDate"].ToString());
                                    objClientUnitService = ServiceFactory.ClientUnitService;
                                    objClientUnitService.SetStatusByServiceId(General.UnitStatus.ServiceSoon.GetEnumValue(), ServiceId);
                                    SendNotificationForServiceSchedule(ServiceId, Convert.ToInt32(hdnClientId.Value), EmployeeId, ScheduleDate, drpPurpose.SelectedValue.ToString());
                                }
                            }
                            Response.Redirect(Application["SiteAddress"] + "admin/PendingService_List.aspx?msg=requested");
                        }
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


        private void SendNotificationForServiceSchedule(long ServiceId, int ClientId, int EmployeeId, DateTime ScheduleDate, string PurposeOfVisit)
        {
            string message = string.Empty;
            IClientService objClientService = ServiceFactory.ClientService;
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

            if (PurposeOfVisit == General.PurposeOfVisit.Emergency.GetEnumDescription() ||
                                       PurposeOfVisit == General.PurposeOfVisit.ContinuingPreviousWork.GetEnumDescription() ||
                                       PurposeOfVisit == General.PurposeOfVisit.Repairing.GetEnumDescription())
            {
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
            else
            {
                message = General.GetNotificationMessage("ServiceApprovelSendToClient");

                NotifyUser(ClientId, "client", General.NotificationType.ServiceApproval.GetEnumDescription(), General.NotificationType.ServiceApproval.GetEnumValue()
                    , ServiceId, message, ClientDeviceType, ClientDeviceToken);
            }

            if (PurposeOfVisit == General.PurposeOfVisit.Maintenance.GetEnumDescription())
            {
                DataTable dtApprovalEmail = new DataTable();
                objServicesService = ServiceFactory.ServicesService;
                objServicesService.SetServiceApprovalUrl(ServiceId, ref dtApprovalEmail);
                if (dtApprovalEmail.Rows.Count > 0)
                {
                    DataTable dtEmailtemplate = new DataTable();
                    IEmailTemplateService objEmailTemplateService = ServiceFactory.EmailTemplateService;
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

        protected void lnkSearch_Click(object sender, EventArgs e)
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
                rblEmployee.DataTextField = dtEmployees.Columns["EmpName"].ToString();
                rblEmployee.DataValueField = dtEmployees.Columns["EmployeeId"].ToString();
                rblEmployee.DataBind();
            }
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

        protected void rblWorkArea_SelectedIndexChanged(object sender, EventArgs e)
        {
            rblEmployee.DataSource = "";
            rblEmployee.DataBind();

            txtWorkArea.Text = rblWorkArea.SelectedItem.Text;
            objEmployeeWorkAreaService = ServiceFactory.EmployeeWorkAreaService;
            DataTable dtEmployees = new DataTable();
            objEmployeeWorkAreaService.GetAllEmployeeByAreaId(Convert.ToInt32(rblWorkArea.SelectedValue.ToString()), "", false, ref dtEmployees);
            if (dtEmployees.Rows.Count > 0)
            {
                rblEmployee.DataSource = dtEmployees;
                rblEmployee.DataTextField = dtEmployees.Columns["EmployeeName"].ToString();
                rblEmployee.DataValueField = dtEmployees.Columns["EmployeeId"].ToString();
                rblEmployee.DataBind();
            }
        }
    }
}