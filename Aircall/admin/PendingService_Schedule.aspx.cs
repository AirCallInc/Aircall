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
    public partial class PendingService_Schedule : System.Web.UI.Page
    {
        IServiceAttemptCountService objServiceAttemptCountService;
        IRescheduleServicesService objRescheduleServicesService;
        IServicesService objServicesService;
        IClientAddressService objClientAddressService;
        IClientUnitService objClientUnitService;
        IServiceUnitService objServiceUnitService;
        IAreasService objAreasService;
        IEmployeeWorkAreaService objEmployeeWorkAreaService;
        IEmployeeService objEmployeeService;
        IUserNotificationService objUserNotificationService;
        IClientService objClientService;
        IEmailTemplateService objEmailTemplateService;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                FillPurposeOfVisitDropdown();
                if (!string.IsNullOrEmpty(Request.QueryString["ServiceId"]))
                {
                    BindServiceInformation();
                    BindAttemptFailReasons();
                    BindRescheduleAttemptLog();
                    txtStart.Value = "08:00 AM";
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


        private void BindServiceInformation()
        {
            long ServiceId = Convert.ToInt64(Request.QueryString["ServiceId"].ToString());
            objServicesService = ServiceFactory.ServicesService;
            DataTable dtService = new DataTable();
            objServicesService.GetServiceById(ServiceId, ref dtService);
            if (dtService.Rows.Count > 0)
            {
                int ClientId = Convert.ToInt32(dtService.Rows[0]["ClientId"].ToString());
                int PlanTypeId = Convert.ToInt32(dtService.Rows[0]["PlanTypeId"].ToString());
                hdnClient.Value = dtService.Rows[0]["ClientId"].ToString();
                hdnServiceDayGap.Value = dtService.Rows[0]["ServiceDayGap"].ToString();
                int AddressId = Convert.ToInt32(dtService.Rows[0]["AddressID"].ToString());
                txtClient.Text = dtService.Rows[0]["ClientName"].ToString();
                objClientAddressService = ServiceFactory.ClientAddressService;
                DataTable dtAddress = new DataTable();
                objClientAddressService.GetClientAddressesByClientId(ClientId, true,ref dtAddress);
                if (dtAddress.Rows.Count > 0)
                {
                    rblAddress.DataSource = dtAddress;
                    rblAddress.DataTextField = dtAddress.Columns["ClientAddress"].ToString();
                    rblAddress.DataValueField = dtAddress.Columns["Id"].ToString();
                    rblAddress.DataBind();
                    rblAddress.SelectedValue = AddressId.ToString();
                    rblAddress.Enabled = false;
                }
                hdnDrivetime.Value = dtService.Rows[0]["Drivetime"].ToString();
                hdnServiceTimeForFirstUnit.Value = dtService.Rows[0]["ServiceTimeForFirstUnit"].ToString();
                hdnServiceTimeForAdditionalUnits.Value = dtService.Rows[0]["ServiceTimeForAdditionalUnits"].ToString();

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
                            {
                                item.Selected = true;
                            }
                        }
                    }
                }

                txtServiceRequested.Text = Convert.ToDateTime(dtService.Rows[0]["AddedDate"].ToString()).ToString("MM/dd/yyyy");
                txtServiceRequested.Enabled = false;
                drpPurpose.SelectedValue = dtService.Rows[0]["PurposeOfVisit"].ToString();
                txtWorkArea.Text = dtService.Rows[0]["AreaName"].ToString();
                //DataTable dtArea = new DataTable();
                //objAreasService = ServiceFactory.AreasService;
                //int AreaId = 0;
                //if (!string.IsNullOrEmpty(dtService.Rows[0]["WorkAreaId"].ToString()))
                //{
                //    AreaId = Convert.ToInt32(dtService.Rows[0]["WorkAreaId"].ToString());
                //    objAreasService.GetAreaById(AreaId, true, ref dtArea);
                //    if (dtArea.Rows.Count > 0)
                //    {
                //        rblWorkArea.DataSource = dtArea;
                //        rblWorkArea.DataValueField = dtArea.Columns["Id"].ToString();
                //        rblWorkArea.DataTextField = dtArea.Columns["Name"].ToString();
                //        rblWorkArea.DataBind();
                //        rblWorkArea.SelectedValue = AreaId.ToString();
                //    }
                //}
                BindWorkAreaByAddressId(AddressId);
                //txtEmployee.Text = dtService.Rows[0]["EmployeeName"].ToString();

                //objEmployeeWorkAreaService = ServiceFactory.EmployeeWorkAreaService;
                //DataTable dtEmployee = new DataTable();
                //objEmployeeWorkAreaService.GetAllEmployeeByAreaId(AreaId, "", false, ref dtEmployee);
                //if (dtEmployee.Rows.Count > 0)
                //{
                //    rblEmployee.DataSource = dtEmployee;
                //    rblEmployee.DataTextField = dtEmployee.Columns["EmpName"].ToString();
                //    rblEmployee.DataValueField = dtEmployee.Columns["EmployeeId"].ToString();
                //    rblEmployee.DataBind();
                //    rblEmployee.SelectedValue = dtService.Rows[0]["EmployeeId"].ToString();
                //}
                //if (!string.IsNullOrEmpty(dtService.Rows[0]["ScheduleDate"].ToString()))
                //    txtScheduleOn.Text = Convert.ToDateTime(dtService.Rows[0]["ScheduleDate"].ToString()).ToString("MM/dd/yyyy");

                //txtStart.Value = dtService.Rows[0]["ScheduleStartTime"].ToString();
                //txtEnd.Value = dtService.Rows[0]["ScheduleEndTime"].ToString();
                txtCustomerNote.Text = dtService.Rows[0]["CustomerComplaints"].ToString();
                txtDispatcherNote.Text = dtService.Rows[0]["DispatcherNotes"].ToString();
                txtEmpNote.Text = dtService.Rows[0]["TechnicianNotes"].ToString();
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

        protected void btnSchedule_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                bool Redirect = false;
                try
                {
                    if (Session["LoginSession"] != null)
                    {
                        LoginModel objLoginModel = new LoginModel();
                        objLoginModel = Session["LoginSession"] as LoginModel;

                        dvMessage.InnerHtml = "";
                        dvMessage.Visible = false;
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

                            if (drpPurpose.SelectedValue!=General.PurposeOfVisit.Emergency.GetEnumDescription())
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

                            if (Convert.ToDateTime(txtScheduleOn.Text).Date==DateTime.Now.Date)
                            {
                                TimeSpan CurrentTimeSpan = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                                if (STime.Ticks < CurrentTimeSpan.Ticks)
                                {
                                    dvMessage.InnerHtml = "<strong>Schedule Start Time must be Greater than Current time.</strong>";
                                    dvMessage.Attributes.Add("class", "alert alert-error");
                                    dvMessage.Visible = true;
                                    return;
                                }
                            }

                            if (StartTime == EndTime)
                            {
                                dvMessage.InnerHtml = "<strong>Service Start Time is same as End Time.</strong>";
                                dvMessage.Attributes.Add("class", "alert alert-error");
                                dvMessage.Visible = true;
                                return;
                            }

                            //if ((txtStart.Value.Trim().Contains("PM") && txtEnd.Value.Trim().Contains("PM")) ||
                            //    (txtStart.Value.Trim().Contains("AM") && txtEnd.Value.Trim().Contains("AM")))
                            //{
                            //    if (StartTime > EndTime)
                            //    {
                            //        dvMessage.InnerHtml = "<strong>Service End Time must be greater than Start Time.</strong>";
                            //        dvMessage.Attributes.Add("class", "alert alert-error");
                            //        dvMessage.Visible = true;
                            //        return;
                            //    }
                            //}

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

                            BizObjects.Services objServices = new BizObjects.Services();
                            BizObjects.ServiceUnits objServiceUnits = new BizObjects.ServiceUnits();

                            objServices.Id = ServiceId;
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
                            objServices.ScheduledBy = objLoginModel.Id;
                            objServices.UpdatedBy = objLoginModel.Id;
                            objServices.UpdatedByType = objLoginModel.RoleId;
                            objServices.UpdatedDate = DateTime.UtcNow;

                            string ServiceUnits = string.Empty;
                            foreach (ListItem item in chkUnits.Items)
                            {
                                if (item.Selected)
                                {
                                    if (string.IsNullOrEmpty(ServiceUnits))
                                        ServiceUnits = item.Value;
                                    else
                                        ServiceUnits = ServiceUnits + ',' + item.Value;
                                }
                            }
                             int EmployeeId = 0;
                            objServicesService = ServiceFactory.ServicesService;
                            DataTable dtService = new DataTable();
                            objServicesService.SchedulePendingServiceFromAdmin(ref objServices, ServiceUnits, ref dtService);
                            if (dtService.Rows.Count > 0)
                            {
                                for (int i = 0; i < dtService.Rows.Count; i++)
                                {
                                    EmployeeId = Convert.ToInt32(dtService.Rows[i]["EmployeeId"].ToString());
                                    BindAttemptFailReasons();
                                    switch (EmployeeId)
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
                                            dvMessage.InnerHtml = "<strong>None of the Employee are free for Client workarea.</strong>";
                                            dvMessage.Attributes.Add("class", "alert alert-error");
                                            dvMessage.Visible = true;
                                            break;
                                        default:
                                            long SId = Convert.ToInt64(dtService.Rows[i]["ServiceId"].ToString());
                                            int CId = Convert.ToInt32(dtService.Rows[i]["ClientId"].ToString());
                                            DateTime ScheduleDt = Convert.ToDateTime(dtService.Rows[i]["ScheduleDate"].ToString());

                                            objClientUnitService = ServiceFactory.ClientUnitService;
                                            objClientUnitService.SetStatusByServiceId(General.UnitStatus.ServiceSoon.GetEnumValue(), SId);

                                            SendNotificationForServiceSchedule(SId, CId, EmployeeId, ScheduleDt, drpPurpose.SelectedItem.Text);
                                            Redirect = true;
                                            break;
                                    }
                                }
                            }
                            if (Redirect)
                                Response.Redirect(Application["SiteAddress"] + "admin/PendingService_List.aspx?msg=edit");
                            else if (Redirect==false && dtService.Rows.Count==0)
                                Response.Redirect(Application["SiteAddress"] + "admin/PendingService_List.aspx?msg=pendingPart");


                        }
                    }
                }
                catch (Exception Ex)
                {
                    dvMessage.InnerHtml = "<strong>Error! </strong>" + Ex.Message.ToString();
                    dvMessage.Attributes.Add("class", "alert alert-error");
                    dvMessage.Visible = true;
                }
            }
        }

        private void SendNotificationForServiceSchedule(long ServiceId, int ClientId, int EmployeeId, DateTime ScheduleDate, string PurposeOfVisit)
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
            dvMessage.InnerHtml = "";
            dvMessage.Visible = false;
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
            dvMessage.InnerHtml = "";
            dvMessage.Visible = false;
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