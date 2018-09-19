using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Services;
using System.Data;
using Aircall.Common;
using System.Web.UI.HtmlControls;
using System.Configuration;

namespace Aircall.admin
{
    public partial class PendingService_List : System.Web.UI.Page
    {
        public string workareaEmployee;
        public string employeeSchedule;
        IServicesService objServicesService;
        IClientService objClientService;
        IUserNotificationService objUserNotificationService;
        IRequestServicesService objRequestServicesService;
        IEmployeeService objEmployeeService;
        IEmailTemplateService objEmailTemplateService;
        IClientUnitService objClientUnitService;
        ICalendarService objCalendarService;
        IClientAddressService objClientAddressService;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["LoginSession"] == null)
            {
                Response.Redirect(Application["SiteAddress"] + "admin/Login.aspx");
            }
            //dataPagerPendingService.PageSize = Convert.ToInt32(Application["PageSize"].ToString());
            if (!IsPostBack)
            {
                string jsonstr = string.Empty;
                jsonstr = CalendarData.GetElementJsonString(0, 0, 0);
                workareaEmployee = jsonstr;
                jsonstr = string.Empty;

                jsonstr = CalendarData.GetEmployeeSchedule(0, 0);
                employeeSchedule = jsonstr;

                if (Request.QueryString["msg"] == "scheduled")
                {
                    dvMessage.InnerHtml = "<strong>New Requested Service scheduled successfully.</strong>";
                    dvMessage.Attributes.Add("class", "alert alert-success");
                    dvMessage.Visible = true;
                }
                else if (Request.QueryString["msg"] == "edit")
                {
                    dvMessage.InnerHtml = "<strong>Service scheduled successfully.</strong>";
                    dvMessage.Attributes.Add("class", "alert alert-success");
                    dvMessage.Visible = true;
                }
                else if (Request.QueryString["msg"] == "-1")
                {
                    dvMessage.InnerHtml = "<strong>Selected Units has multiple plan type.</strong>";
                    dvMessage.Attributes.Add("class", "alert alert-error");
                    dvMessage.Visible = true;
                }
                else if (Request.QueryString["msg"] == "-2")
                {
                    dvMessage.InnerHtml = "<strong>Selected Employee Workarea did not match with client's zip code.</strong>";
                    dvMessage.Attributes.Add("class", "alert alert-error");
                    dvMessage.Visible = true;
                }
                else if (Request.QueryString["msg"] == "-3")
                {
                    dvMessage.InnerHtml = "<strong>Selected Employee Working time did not match.</strong>";
                    dvMessage.Attributes.Add("class", "alert alert-error");
                    dvMessage.Visible = true;
                }
                else if (Request.QueryString["msg"] == "-4")
                {
                    dvMessage.InnerHtml = "<strong>Selected Employee is on leave.</strong>";
                    dvMessage.Attributes.Add("class", "alert alert-error");
                    dvMessage.Visible = true;
                }
                else if (Request.QueryString["msg"] == "-5")
                {
                    dvMessage.InnerHtml = "<strong>Employee is already assigned to another service for selected timings.</strong>";
                    dvMessage.Attributes.Add("class", "alert alert-error");
                    dvMessage.Visible = true;
                }
                else if (Request.QueryString["msg"] == "-6")
                {
                    dvMessage.InnerHtml = "<strong>Parts Not Found.</strong>";
                    dvMessage.Attributes.Add("class", "alert alert-error");
                    dvMessage.Visible = true;
                }
                else if (Request.QueryString["msg"] == "-7")
                {
                    dvMessage.InnerHtml = "<strong>Some of the parts are not in stock.</strong>";
                    dvMessage.Attributes.Add("class", "alert alert-error");
                    dvMessage.Visible = true;
                }
                else if (Request.QueryString["msg"] == "unit")
                {
                    dvMessage.InnerHtml = "<strong>Services scheduled successfully.</strong>";
                    dvMessage.Attributes.Add("class", "alert alert-success");
                    dvMessage.Visible = true;
                }
                else if (Request.QueryString["msg"] == "pending")
                {
                    dvMessage.InnerHtml = "<strong>Pending Services scheduled successfully.</strong>";
                    dvMessage.Attributes.Add("class", "alert alert-success");
                    dvMessage.Visible = true;
                }
                else if (Request.QueryString["msg"] == "requested")
                {
                    dvMessage.InnerHtml = "<strong>Requested Services scheduled successfully.</strong>";
                    dvMessage.Attributes.Add("class", "alert alert-success");
                    dvMessage.Visible = true;
                }
                else if (Request.QueryString["msg"] == "reschedule")
                {
                    dvMessage.InnerHtml = "<strong>Rescheduled Services scheduled successfully.</strong>";
                    dvMessage.Attributes.Add("class", "alert alert-success");
                    dvMessage.Visible = true;
                }
                else if (Request.QueryString["msg"] == "pendingPart")
                {
                    dvMessage.InnerHtml = "<strong>Some of the parts are not in stock.Service are in pending state.</strong>";
                    dvMessage.Attributes.Add("class", "alert alert-error");
                    dvMessage.Visible = true;
                }
                else if (Request.QueryString["msg"] == "del")
                {
                    dvMessage.InnerHtml = "<strong>Service deleted successfully.</strong>";
                    dvMessage.Attributes.Add("class", "alert alert-success");
                    dvMessage.Visible = true;
                }
                BindEmployeeDropdown();
                BindCityDropdown();
                BindPendingAndRequestedService();
            }
        }

        private void BindEmployeeDropdown()
        {
            objCalendarService = ServiceFactory.CalendarService;
            DataTable dtEmployees = new DataTable();
            objCalendarService.GetEmployeeSchedule(0, ref dtEmployees);
            if (dtEmployees.Rows.Count > 0)
            {
                drpEmployee.DataSource = dtEmployees;
                drpEmployee.DataTextField = dtEmployees.Columns["EmpName"].ToString();
                drpEmployee.DataValueField = dtEmployees.Columns["EmployeeId"].ToString();
            }
            drpEmployee.DataBind();
            drpEmployee.Items.Insert(0, new ListItem("Select Employee", "0"));
        }
        private void BindEmployeeDropdownCityWize(int CityId)
        {
            drpEmployee.Items.Clear();
            objCalendarService = ServiceFactory.CalendarService;
            DataTable dtEmployees = new DataTable();
            objCalendarService.GetEmployeeScheduleCityWise(0, CityId, ref dtEmployees);
            if (dtEmployees.Rows.Count > 0)
            {
                drpEmployee.DataSource = dtEmployees;
                drpEmployee.DataTextField = dtEmployees.Columns["EmpName"].ToString();
                drpEmployee.DataValueField = dtEmployees.Columns["EmployeeId"].ToString();
            }
            drpEmployee.DataBind();
            drpEmployee.Items.Insert(0, new ListItem("Select Employee", "0"));
        }
        private void BindCityDropdown()
        {
            var objCitiesService = ServiceFactory.CitiesService;
            DataTable dtCities = new DataTable();
            objCitiesService.GetAllCityByStateId(1, true, ref dtCities);
            if (dtCities.Rows.Count > 0)
            {
                drpCity.DataSource = dtCities;
                drpCity.DataValueField = dtCities.Columns["Id"].ToString();
                drpCity.DataTextField = dtCities.Columns["Name"].ToString();
            }
            else
                drpCity.DataSource = "";

            drpCity.DataBind();
            drpCity.Items.Insert(0, new ListItem("Select City", "0"));
        }
        private void BindPendingAndRequestedService()
        {
            objServicesService = ServiceFactory.ServicesService;
            DataTable dtServices = new DataTable();
            string ServiceCaseNo = string.Empty;
            string ClientName = string.Empty;
            string StartDate = string.Empty;
            string EndDate = string.Empty;

            if (!string.IsNullOrEmpty(Request.QueryString["SNo"]))
            {
                ServiceCaseNo = Request.QueryString["SNo"].ToString();
                txtCaseNo.Text = ServiceCaseNo;
            }
            if (!string.IsNullOrEmpty(Request.QueryString["ClientName"]))
            {
                ClientName = Request.QueryString["ClientName"].ToString();
                txtClient.Text = ClientName;
            }
            if (!string.IsNullOrEmpty(Request.QueryString["StartDate"]))
            {
                StartDate = Request.QueryString["StartDate"].ToString();
                txtStart.Value = StartDate;
            }
            if (!string.IsNullOrEmpty(Request.QueryString["EndDate"]))
            {
                EndDate = Request.QueryString["EndDate"].ToString();
                txtEnd.Value = EndDate;
            }

            objServicesService.GetPendingAndRequestedService(ServiceCaseNo, ClientName, StartDate, EndDate, ListViewSortExpression, ListViewSortDirection.ToString(), ref dtServices);
            if (dtServices.Rows.Count > 0)
            {
                lstPendingService.DataSource = dtServices;
                lstPendingService.DataBind();
            }
            lstPendingService.DataBind();
        }

        protected void lnkSchedule_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dtServiceUnits = new DataTable();
                DataTable dtClient = new DataTable();
                DataTable dtServiceId = new DataTable();
                DataTable dtBadgeCount = new DataTable();
                DataTable dtEmployee = new DataTable();

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
                BizObjects.UserNotification objUserNotification = new BizObjects.UserNotification();
                string message = string.Empty;

                bool AutomaticScheduleService = true;
                string AutomaticScheduleServiceStr = General.GetSitesettingsValue("AutomaticScheduleService");

                if (AutomaticScheduleServiceStr.ToLower() == "on")
                    AutomaticScheduleService = true;
                else
                    AutomaticScheduleService = false;

                if (AutomaticScheduleService)
                {
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
                                                message = General.GetNotificationMessage("ServiceApprovelSendToClient"); //"Your Service is Scheduled.Please Approve or Reschedule.";
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

                                                Notifications objNotifications = new Notifications { NId = NotificationId, NType = General.NotificationType.ServiceApproval.GetEnumValue(), CommonId = ServiceId };
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

                                            DataTable dtAddress = new DataTable();
                                            string strAddress = string.Empty;
                                            objClientAddressService = ServiceFactory.ClientAddressService;
                                            objClientAddressService.GetAddressById(AddressId, ref dtAddress);
                                            if (dtAddress.Rows.Count > 0)
                                                strAddress = dtAddress.Rows[0]["Address"].ToString();

                                            //Send Notification to Client
                                            long NotificationId = 0;
                                            int BadgeCount = 0;
                                            objUserNotificationService = ServiceFactory.UserNotificationService;

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
                                                        SendNotifications.SendAndroidNotification(dtClient.Rows[0]["DeviceToken"].ToString(), message, CustomData, "client");
                                                    }
                                                    else if (dtClient.Rows[0]["DeviceType"].ToString().ToLower() == "iphone")
                                                    {
                                                        SendNotifications.SendIphoneNotification(BadgeCount, dtClient.Rows[0]["DeviceToken"].ToString(), message, notify, "client");
                                                    }
                                                }
                                            }

                                            //Send Notification to Employee
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
                                                        SendNotifications.SendIphoneNotification(EmployeeBadgeCount, dtEmployee.Rows[0]["DeviceToken"].ToString(), objUserNotification1.Message, notify1, "employee");
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    Response.Redirect(Application["SiteAddress"] + "admin/PendingService_List.aspx?msg=unit");

                    //dvMessage.InnerHtml = "<strong>Services scheduled successfully.</strong>";
                    //dvMessage.Attributes.Add("class", "alert alert-success");
                    //dvMessage.Visible = true;
                }
                else
                {
                    dvMessage.InnerHtml = "<strong>Automatic Schedule is Turned Off. You have to Schedule Service manually.</strong>";
                    dvMessage.Attributes.Add("class", "alert alert-error");
                    dvMessage.Visible = true;
                }
                BindPendingAndRequestedService();
            }
            catch (Exception Ex)
            {
                dvMessage.InnerHtml = Ex.Message.ToString().Trim();
                dvMessage.Attributes.Add("class", "alert alert-error");
                dvMessage.Visible = true;
            }
        }

        protected void lnkSchedulePending_Click(object sender, EventArgs e)
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
                                        PurposeOfVisit == General.PurposeOfVisit.ContinuingPreviousWork.GetEnumDescription() ||
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
                                            PurposeOfVisit == General.PurposeOfVisit.ContinuingPreviousWork.GetEnumDescription() ||
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
                    //dvMessage.InnerHtml = "<strong>Pending Services scheduled successfully.</strong>";
                    //dvMessage.Attributes.Add("class", "alert alert-success");
                    //dvMessage.Visible = true;
                    Response.Redirect(Application["SiteAddress"] + "admin/PendingService_List.aspx?msg=pending");
                }
                else
                {
                    dvMessage.InnerHtml = "<strong>Automatic Schedule is Turned Off. You have to Schedule Service manually.</strong>";
                    dvMessage.Attributes.Add("class", "alert alert-error");
                    dvMessage.Visible = true;
                }
                BindPendingAndRequestedService();
            }
            catch (Exception Ex)
            {
                dvMessage.InnerHtml = Ex.Message.ToString().Trim();
                dvMessage.Attributes.Add("class", "alert alert-error");
                dvMessage.Visible = true;
            }
        }

        protected void dataPagerPendingService_PreRender(object sender, EventArgs e)
        {
            dataPagerPendingService.PageSize = 10;//Convert.ToInt32(Application["PageSize"].ToString());
            BindPendingAndRequestedService();
        }

        protected void lnkScheduleRequested_Click(object sender, EventArgs e)
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
                    objRequestServicesService = ServiceFactory.RequestServicesService;
                    objClientService = ServiceFactory.ClientService;
                    objUserNotificationService = ServiceFactory.UserNotificationService;
                    objServicesService = ServiceFactory.ServicesService;
                    DataTable dtRequestedService = new DataTable();
                    DataTable dtService = new DataTable();
                    DataTable dtClient = new DataTable();
                    DataTable dtClientBadgeCount = new DataTable();
                    DataTable dtEmployeeBadgeCount = new DataTable();
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
                            objRequestServicesService.ScheduleRequestedService(0, RequestedServiceId, ClientId, AddressId, PurposeOfVisit, RequestedServiceDate, RequestedServiceTime, ref dtService);
                            BizObjects.UserNotification objUserNotification = new BizObjects.UserNotification();
                            if (dtService.Rows.Count > 0)
                            {
                                for (int j = 0; j < dtService.Rows.Count; j++)
                                {
                                    long ServiceId = Convert.ToInt64(dtService.Rows[j]["ServiceId"].ToString());
                                    int EmployeeId = Convert.ToInt32(dtService.Rows[j]["EmployeeId"].ToString());
                                    if (ServiceId != 0 && EmployeeId != 0)
                                    {
                                        DateTime ScheduleDate = Convert.ToDateTime(dtService.Rows[j]["ScheduleDate"].ToString());

                                        objClientService.GetClientById(ClientId, ref dtClient);
                                        if (dtClient.Rows.Count > 0)
                                        {
                                            long NotificationId = 0;
                                            int ClientBadgeCount = 0;
                                            string message;
                                            int NotificationType;
                                            string MessageType;

                                            if (PurposeOfVisit == General.PurposeOfVisit.Emergency.GetEnumDescription() ||
                                                PurposeOfVisit == General.PurposeOfVisit.ContinuingPreviousWork.GetEnumDescription() ||
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

                                            objUserNotification.MessageType = MessageType;
                                            objUserNotification.UserId = ClientId;
                                            objUserNotification.UserTypeId = General.UserRoles.Client.GetEnumValue();
                                            objUserNotification.Message = message;
                                            objUserNotification.Status = General.NotificationStatus.UnRead.GetEnumDescription();
                                            objUserNotification.CommonId = ServiceId;
                                            objUserNotification.AddedDate = DateTime.UtcNow;

                                            NotificationId = objUserNotificationService.AddUserNotification(ref objUserNotification);

                                            objUserNotificationService.GetBadgeCount(ClientId, General.UserRoles.Client.GetEnumValue(), ref dtClientBadgeCount);
                                            ClientBadgeCount = dtClientBadgeCount.Rows.Count;

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
                                                    SendNotifications.SendIphoneNotification(ClientBadgeCount, dtClient.Rows[0]["DeviceToken"].ToString(), message, notify, "client");
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
                                        //Update Unit Status
                                        objClientUnitService = ServiceFactory.ClientUnitService;
                                        objClientUnitService.SetStatusByServiceId(General.UnitStatus.ServiceSoon.GetEnumValue(), ServiceId);

                                        if (PurposeOfVisit == General.PurposeOfVisit.Emergency.GetEnumDescription() ||
                                                PurposeOfVisit == General.PurposeOfVisit.ContinuingPreviousWork.GetEnumDescription() ||
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
                                        //dvMessage.InnerHtml = "<strong>Requested Services scheduled successfully.</strong>";
                                        //dvMessage.Attributes.Add("class", "alert alert-success");
                                        //dvMessage.Visible = true;
                                        //BindPendingAndRequestedService();
                                    }
                                }
                            }
                        }
                    }
                    Response.Redirect(Application["SiteAddress"] + "admin/PendingService_List.aspx?msg=requested");
                }
                else
                {
                    dvMessage.InnerHtml = "<strong>Automatic Schedule is Turned Off. You have to Schedule Service manually.</strong>";
                    dvMessage.Attributes.Add("class", "alert alert-error");
                    dvMessage.Visible = true;
                }
            }
            catch (Exception Ex)
            {
                dvMessage.InnerHtml = Ex.Message.ToString().Trim();
                dvMessage.Attributes.Add("class", "alert alert-error");
                dvMessage.Visible = true;
            }
        }

        protected void lnkReschedulled_Click(object sender, EventArgs e)
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
                                //    if (Convert.ToBoolean(dtService.Rows[0]["IsEmployeeAvailable"].ToString()) &&
                                //          Convert.ToBoolean(dtService.Rows[0]["IsPartInStock"].ToString()) &&
                                //          Convert.ToInt32(dtService.Rows[0]["EmployeeId"].ToString()) > 0)
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
                                        //if (Convert.ToDateTime(dtService.Rows[0]["ScheduleDate"].ToString()) != Convert.ToDateTime(dtService.Rows[0]["RescheduleDate"].ToString()))
                                        //if (Convert.ToDateTime(dtService.Rows[0]["ScheduleDate"].ToString()) != RescheduleDate)
                                        //{
                                        //    message = General.GetNotificationMessage("RescheduleServiceOffDateSendToClient"); //"None of employee were available on " + Convert.ToDateTime(dtService.Rows[0]["RescheduleDate"].ToString()).ToLocalTime().ToString("MMMM dd, yyyy") + ".So service has been scheduled on " + Convert.ToDateTime(dtService.Rows[0]["ScheduleDate"].ToString()).ToLocalTime().ToString("MMMM dd, yyyy") + ".";
                                        //    //message = message.Replace("{{RescheduleDate}}", Convert.ToDateTime(dtService.Rows[0]["RescheduleDate"].ToString()).ToLocalTime().ToString("MMMM dd, yyyy"));
                                        //    message = message.Replace("{{RescheduleDate}}", Convert.ToDateTime(dtService.Rows[0]["ScheduleDate"].ToString()).ToLocalTime().ToString("MMMM dd, yyyy"));
                                        //    message = message.Replace("{{NewServiceDate}}", Convert.ToDateTime(dtService.Rows[0]["ScheduleDate"].ToString()).ToLocalTime().ToString("MMMM dd, yyyy"));
                                        //}
                                        //else
                                        //{
                                        //    message = General.GetNotificationMessage("RescheduleServiceOnDateSendToClient");//"Your Rescheduled Service is Scheduled on " + Convert.ToDateTime(dtService.Rows[0]["ScheduleDate"].ToString()).ToLocalTime().ToString("MMMM dd, yyyy") + ".";
                                        //    message = message.Replace("{{ScheduleDate}}", Convert.ToDateTime(dtService.Rows[0]["ScheduleDate"].ToString()).ToLocalTime().ToString("MMMM dd, yyyy"));
                                        //}
                                        if (PurposeOfVisit == General.PurposeOfVisit.Emergency.GetEnumDescription() ||
                                            PurposeOfVisit == General.PurposeOfVisit.ContinuingPreviousWork.GetEnumDescription() ||
                                            PurposeOfVisit == General.PurposeOfVisit.Repairing.GetEnumDescription())
                                        {
                                            //if (ScheduleDate != RequestedServiceDate)
                                            //{
                                            //    message = General.GetNotificationMessage("RescheduleServiceOffDateSendToClient"); //"None of employee were available on " + RequestedServiceDate.ToLocalTime().ToString("MMMM dd, yyyy") + ".So service has been scheduled on " + ScheduleDate.ToLocalTime().ToString("MMMM dd, yyyy") + ".";
                                            //    message = message.Replace("{{RescheduleDate}}", RequestedServiceDate.ToLocalTime().ToString("MMMM dd, yyyy"));
                                            //    message = message.Replace("{{NewServiceDate}}", ScheduleDate.ToLocalTime().ToString("MMMM dd, yyyy"));
                                            //}
                                            //else
                                            //{
                                            message = General.GetNotificationMessage("RequestedServiceScheduleSendToClient"); //"Your Requested Service is Scheduled on " + ScheduleDate.ToLocalTime().ToString("MMMM dd, yyyy") + ".";
                                            message = message.Replace("{{ScheduleDate}}", ScheduleDate.ToString("MMMM dd, yyyy"));
                                            //}
                                            MessageType = General.NotificationType.ServiceScheduled.GetEnumDescription();
                                            NotificationType = General.NotificationType.ServiceScheduled.GetEnumValue();
                                        }
                                        else
                                        {
                                            message = General.GetNotificationMessage("ServiceApprovelSendToClient");
                                            MessageType = General.NotificationType.ServiceApproval.GetEnumDescription();
                                            NotificationType = General.NotificationType.ServiceApproval.GetEnumValue();
                                        }
                                        //message = General.GetNotificationMessage("ServiceApprovelSendToClient");

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
                                        PurposeOfVisit == General.PurposeOfVisit.Repairing.GetEnumDescription())
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

                                    //dvMessage.InnerHtml = "<strong>Reschedulled Services scheduled successfully.</strong>";
                                    //dvMessage.Attributes.Add("class", "alert alert-success");
                                    //dvMessage.Visible = true;
                                    //BindPendingAndRequestedService();
                                }
                            }
                        }
                    }
                    Response.Redirect(Application["SiteAddress"] + "admin/PendingService_List.aspx?msg=reschedule");
                }
                else
                {
                    dvMessage.InnerHtml = "<strong>Automatic Schedule is Turned Off. You have to Schedule Service manually.</strong>";
                    dvMessage.Attributes.Add("class", "alert alert-error");
                    dvMessage.Visible = true;
                }
            }
            catch (Exception Ex)
            {
                dvMessage.InnerHtml = Ex.Message.ToString().Trim();
                dvMessage.Attributes.Add("class", "alert alert-error");
                dvMessage.Visible = true;
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string Param = string.Empty;

            if (!string.IsNullOrEmpty(txtClient.Text.Trim()))
                Param = "?ClientName=" + txtClient.Text.Trim();
            if (!string.IsNullOrEmpty(txtCaseNo.Text.Trim()))
            {
                if (!string.IsNullOrEmpty(Param))
                    Param = Param + "&SNo=" + txtCaseNo.Text.Trim();
                else
                    Param = "?SNo=" + txtCaseNo.Text.Trim();
            }
            if (!string.IsNullOrEmpty(txtStart.Value.Trim()))
            {
                if (!string.IsNullOrEmpty(Param))
                    Param = Param + "&StartDate=" + txtStart.Value.Trim();
                else
                    Param = "?StartDate=" + txtStart.Value.Trim();
            }
            if (!string.IsNullOrEmpty(txtEnd.Value.Trim()))
            {
                if (!string.IsNullOrEmpty(Param))
                    Param = Param + "&EndDate=" + txtEnd.Value.Trim();
                else
                    Param = "?EndDate=" + txtEnd.Value.Trim();
            }

            Response.Redirect(Application["SiteAddress"] + "admin/PendingService_List.aspx" + Param);
        }

        protected SortDirection ListViewSortDirection
        {
            get
            {
                if (ViewState["sortDirection"] == null)
                    ViewState["sortDirection"] = SortDirection.Descending;
                return (SortDirection)ViewState["sortDirection"];
            }
            set { ViewState["sortDirection"] = value; }
        }

        protected string ListViewSortExpression
        {
            get
            {
                if (ViewState["SortExpression"] == null)
                    ViewState["SortExpression"] = "ScheduleAttempt";
                return (string)ViewState["SortExpression"];
            }
            set { ViewState["SortExpression"] = value; }
        }

        protected void lstPendingService_Sorting(object sender, ListViewSortEventArgs e)
        {
            LinkButton lb = lstPendingService.FindControl(e.SortExpression) as LinkButton;
            HtmlTableCell th = lb.Parent as HtmlTableCell;
            HtmlTableRow tr = th.Parent as HtmlTableRow;
            List<HtmlTableCell> ths = new List<HtmlTableCell>();
            foreach (HtmlTableCell item in tr.Controls)
            {
                try
                {
                    if (item.ID.Contains("th"))
                    {
                        item.Attributes["class"] = "sorting";
                    }
                }
                catch (Exception ex)
                {
                }
            }

            ListViewSortExpression = e.SortExpression;
            if (ListViewSortDirection == SortDirection.Ascending)
            {
                ListViewSortDirection = SortDirection.Descending;
                th.Attributes["class"] = "sorting_desc";
            }
            else
            {
                ListViewSortDirection = SortDirection.Ascending;
                th.Attributes["class"] = "sorting_asc";
            }
        }

        protected void SortByServiceCase_Click(object sender, EventArgs e)
        {

        }

        [System.Web.Services.WebMethod]
        public static string SetEmployeeForService(string ServiceDate, string StartTime, string EndTime, string EmployeeId, string ServiceId)
        {
            string url = string.Empty;
            string SDay = Convert.ToDateTime(ServiceDate).DayOfWeek.ToString();
            IServicesService objServicesService = ServiceFactory.ServicesService;
            IRequestServicesService objRequestServicesService = ServiceFactory.RequestServicesService;
            string PurposeOfVisit = string.Empty;
            DataTable dtService = new DataTable();

            if (ServiceId.Contains("S-"))
            {
                ServiceId = ServiceId.Replace("S-", "");
                url = ConfigurationManager.AppSettings["SiteAddress"].ToString() + "admin/PendingService_Schedule.aspx?ServiceId=" + ServiceId + "&Date=" + ServiceDate + "&StartTime=" + StartTime;
                if (SDay == "Sunday" || SDay == "Saturday")
                {
                    objServicesService.GetServiceById(Convert.ToInt64(ServiceId), ref dtService);
                    if (dtService.Rows.Count > 0)
                    {
                        PurposeOfVisit = dtService.Rows[0]["PurposeOfVisit"].ToString();
                        if (PurposeOfVisit != General.PurposeOfVisit.Emergency.GetEnumDescription())
                        {
                            url = "not allowed";
                        }
                    }
                }
            }
            else
            {
                ServiceId = ServiceId.Replace("R-", "");
                url = ConfigurationManager.AppSettings["SiteAddress"].ToString() + "admin/ScheduleRequestService.aspx?ServiceId=" + ServiceId + "&Date=" + ServiceDate + "&StartTime=" + StartTime;
                if (SDay == "Sunday" || SDay == "Saturday")
                {
                    objRequestServicesService.GetRequestedServiceById(Convert.ToInt64(ServiceId), ref dtService);
                    if (dtService.Rows.Count > 0)
                    {
                        PurposeOfVisit = dtService.Rows[0]["PurposeOfVisit"].ToString();
                        if (PurposeOfVisit != General.PurposeOfVisit.Emergency.GetEnumDescription())
                        {
                            url = "not allowed";
                        }
                    }
                }
            }

            return url;
            //LoginModel objLoginModel = new LoginModel();
            //ICalendarService objCalendarService = ServiceFactory.CalendarService;
            //IClientUnitService objClientUnitService = ServiceFactory.ClientUnitService;
            //string message = string.Empty;
            //objLoginModel = HttpContext.Current.Session["LoginSession"] as LoginModel;
            //if (objLoginModel != null)
            //{
            //    long SId = Convert.ToInt64(ServiceId.ToString());
            //    EmployeeId = EmployeeId.Replace("E-", "");
            //    int EId = Convert.ToInt32(EmployeeId);
            //    DateTime ScDate = Convert.ToDateTime(ServiceDate);

            //    DataTable dtService = new DataTable();
            //    objCalendarService.AssignEmployeeFromCalendar(SId, EId, ScDate, StartTime, EndTime, objLoginModel.Id, objLoginModel.RoleId, ref dtService);

            //    if (dtService.Rows.Count > 0)
            //    {
            //        int EmpId = 0;
            //        for (int i = 0; i < dtService.Rows.Count; i++)
            //        {
            //            EmpId = Convert.ToInt32(dtService.Rows[i]["EmployeeId"].ToString());
            //            switch (EmpId)
            //            {
            //                case -1:
            //                    message = "Selected Units has multiple plan type.";
            //                    break;
            //                case -2:
            //                    message = "Selected Employee Workarea did not match with client's zip code.";
            //                    break;
            //                case -3:
            //                    message = "Selected Employee Working time did not match.";
            //                    break;
            //                case -4:
            //                    message = "Selected Employee is on leave.";
            //                    break;
            //                case -5:
            //                    message = "Employee is already assigned to another service for selected timings.";
            //                    break;
            //                default:
            //                    message = "Service Scheduled.";
            //                    long NewServiceId = Convert.ToInt64(dtService.Rows[i]["ServiceId"].ToString());
            //                    int CId = Convert.ToInt32(dtService.Rows[i]["ClientId"].ToString());
            //                    DateTime ScheduleDt = Convert.ToDateTime(dtService.Rows[i]["ScheduleDate"].ToString());
            //                    string PurposeOfVisit = dtService.Rows[i]["PurposeOfVisit"].ToString();
            //                    objClientUnitService.SetStatusByServiceId(General.UnitStatus.ServiceSoon.GetEnumValue(), NewServiceId);
            //                    PendingService_List obj = new PendingService_List();
            //                    obj.SendNotification(NewServiceId, CId, EId, PurposeOfVisit, ScheduleDt);
            //                    break;
            //            }
            //        }

            //int EmpId = Convert.ToInt32(dtService.Rows[0]["EmployeeId"].ToString());
            //string PurposeOfVisit = dtService.Rows[0]["PurposeOfVisit"].ToString();
            //int ClientId = Convert.ToInt32(dtService.Rows[0]["ClientId"].ToString());
            //switch (EmpId)
            //{
            //    case -1:
            //        message = "Service Unit has multiple plan type of units.";
            //        break;
            //    case -2:
            //        message = "Selected Employee Workarea not matched with client zipcode.";
            //        break;
            //    case -3:
            //        message = "Employee Working hour is not matched with selected timings.";
            //        break;
            //    case -4:
            //        message = "Employee is on leave for selected date.";
            //        break;
            //    case -5:
            //        message = "Employee is already assigned at selected timings.";
            //        break;
            //    case -6:
            //        message = "Some of the Parts are not in stock.";
            //        break;
            //    case -7:
            //        message = "Parts Not Found.";
            //        break;
            //    case -8:
            //        message = "You had assigned less timings for service.";
            //        break;
            //    default:
            //        message = "Service Scheduled.";
            //        PendingService_List obj = new PendingService_List();
            //        obj.SendNotification(SId, ClientId, EId, PurposeOfVisit, ScDate);
            //        break;
            //}
            //    }
            //}

            //return message;
        }

        public void SendNotification(long ServiceId, int ClientId, int EmployeeId, string PurposeOfVisit, DateTime ScheduleDate)
        {
            DataTable dtClient = new DataTable();
            objClientService = ServiceFactory.ClientService;
            objServicesService = ServiceFactory.ServicesService;
            objClientService.GetClientById(ClientId, ref dtClient);
            if (dtClient.Rows.Count > 0)
            {
                string message = string.Empty;
                string MessageType = string.Empty;
                int NotificationType;

                if (PurposeOfVisit == General.PurposeOfVisit.Emergency.GetEnumDescription() ||
                PurposeOfVisit == General.PurposeOfVisit.ContinuingPreviousWork.GetEnumDescription() ||
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
                            string ApprovalUrl = HttpContext.Current.Application["SiteAddress"] + "ServiceApproval.aspx?Url=" + dtApprovalEmail.Rows[0]["ApprovalEmailUrl"].ToString();
                            Emailbody = Emailbody.Replace("{{Link}}", ApprovalUrl);
                            Emailbody = Emailbody.Replace("{{UserName}}", dtClient.Rows[0]["ClientName"].ToString());
                            string Subject = dtEmailtemplate.Rows[0]["EmailTemplateSubject"].ToString();
                            Email.SendEmail(Subject, dtClient.Rows[0]["Email"].ToString(), CCEmail, "", Emailbody);
                        }
                    }
                }
            }
            //Update Unit Status
            objClientUnitService = ServiceFactory.ClientUnitService;
            objClientUnitService.SetStatusByServiceId(General.UnitStatus.ServiceSoon.GetEnumValue(), ServiceId);

            if (PurposeOfVisit == General.PurposeOfVisit.Emergency.GetEnumDescription() ||
                    PurposeOfVisit == General.PurposeOfVisit.ContinuingPreviousWork.GetEnumDescription() ||
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
        }

        [System.Web.Services.WebMethod]
        public static List<MarkerData> GetEmployeeMapData(int EmployeeId)
        {
            List<MarkerData> Marker = new List<MarkerData>();
            ICalendarService objCalendarService = ServiceFactory.CalendarService;
            DataTable dtMarker = new DataTable();
            objCalendarService.GetMapDataByEmployeeId(EmployeeId, ref dtMarker);
            if (dtMarker.Rows.Count > 0)
            {
                Marker = (from dr in dtMarker.AsEnumerable()
                          select new MarkerData
                          {
                              Lat = dr.Field<decimal>("Latitude"),
                              Lng = dr.Field<decimal>("Longitude"),
                              ServiceCaseNumber = dr.Field<string>("ServiceCaseNumber"),
                              EmpLat = dr.Field<decimal>("EMPLatitude"),
                              EmpLng = dr.Field<decimal>("EMPLongitude"),
                              EmpName = dr.Field<string>("EmpName")
                          }).ToList();
            }
            return Marker;
        }

        [System.Web.Services.WebMethod]
        public static ReInitializeTimeline ReInitializeTimeline(int EmployeeId, int CityId)
        {
            ReInitializeTimeline obj = new ReInitializeTimeline();
            obj.workareaEmployee = CalendarData.GetElementJsonString(0, EmployeeId, CityId);
            obj.employeeSchedule = CalendarData.GetEmployeeSchedule(EmployeeId, CityId);
            return obj;
        }

        protected void drpEmployee_SelectedIndexChanged(object sender, EventArgs e)
        {
            workareaEmployee = CalendarData.GetElementJsonString(0, Convert.ToInt32(drpEmployee.SelectedValue.ToString()), Convert.ToInt32(drpCity.SelectedValue.ToString()));
            if (drpEmployee.SelectedValue.ToString() == "0")
                employeeSchedule = CalendarData.GetEmployeeSchedule(Convert.ToInt32(drpEmployee.SelectedValue.ToString()), Convert.ToInt32(drpCity.SelectedValue.ToString()));
            else
                employeeSchedule = CalendarData.GetEmployeeScheduleFilter(Convert.ToInt32(drpEmployee.SelectedValue.ToString()), Convert.ToInt32(drpCity.SelectedValue.ToString()));
        }
        protected void drpCity_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindEmployeeDropdownCityWize(Convert.ToInt32(drpCity.SelectedValue.ToString()));
            workareaEmployee = CalendarData.GetElementJsonString(0, Convert.ToInt32(drpEmployee.SelectedValue.ToString()), Convert.ToInt32(drpCity.SelectedValue.ToString()));
            if (drpEmployee.SelectedValue.ToString() == "0")
                employeeSchedule = CalendarData.GetEmployeeSchedule(Convert.ToInt32(drpEmployee.SelectedValue.ToString()), Convert.ToInt32(drpCity.SelectedValue.ToString()));
            else
                employeeSchedule = CalendarData.GetEmployeeScheduleFilter(Convert.ToInt32(drpEmployee.SelectedValue.ToString()), Convert.ToInt32(drpCity.SelectedValue.ToString()));
        }
        protected void lstPendingService_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            if (Session["LoginSession"] != null)
            {
                LoginModel objLoginModel = new LoginModel();
                objLoginModel = Session["LoginSession"] as LoginModel;

                if (e.CommandName == "DeleteService" && e.CommandArgument.ToString() != "0")
                {
                    string[] SId = e.CommandArgument.ToString().Split('-');
                    long ServiceId = Convert.ToInt64(SId[1].ToString());
                    objServicesService = ServiceFactory.ServicesService;
                    DataTable dtService = new DataTable();
                    if (SId[0].ToString() == "S")
                    {
                        objServicesService.DeleteService(ServiceId, objLoginModel.Id, objLoginModel.RoleId, DateTime.UtcNow, ref dtService);
                        if (dtService.Rows.Count > 0)
                        {
                            int EmployeeId = Convert.ToInt32(dtService.Rows[0]["EmployeeId"].ToString());
                            int ClientId = Convert.ToInt32(dtService.Rows[0]["ClientId"].ToString());
                            string ClientName = dtService.Rows[0]["ClientName"].ToString();
                            string ScheduleDate = dtService.Rows[0]["ScheduleDate"].ToString();
                            string ClientDeviceType = dtService.Rows[0]["ClientDeviceType"].ToString();
                            string ClientDeviceToken = dtService.Rows[0]["ClientDeviceToken"].ToString();
                            string EmployeeDeviceType = dtService.Rows[0]["EmployeeDeviceType"].ToString();
                            string EmployeeDeviceToken = dtService.Rows[0]["EmployeeDeviceToken"].ToString();
                            string Address = dtService.Rows[0]["ClientAddress"].ToString();
                            string message = string.Empty;

                            //Send Notification to Client
                            message = General.GetNotificationMessage("DeleteServiceSendToClient");
                            message = message.Replace("{{Address}}", Address);
                            NotifyUser(ClientId, "client", General.NotificationType.FriendlyReminder.GetEnumDescription(), General.NotificationType.FriendlyReminder.GetEnumValue(),
                                ServiceId, message, ClientDeviceType, ClientDeviceToken);
                        }
                    }
                    else
                    {
                        objRequestServicesService = ServiceFactory.RequestServicesService;
                        objRequestServicesService.DeleteRequestedService(ServiceId, objLoginModel.Id, objLoginModel.RoleId, DateTime.UtcNow);
                    }
                    Response.Redirect(Application["SiteAddress"] + "admin/PendingService_List.aspx?msg=del");
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