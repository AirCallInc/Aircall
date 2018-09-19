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
    public partial class RequestService_AddEdit : System.Web.UI.Page
    {
        IClientService objClientService;
        IClientAddressService objClientAddressService;
        IClientUnitService objClientUnitService;
        IRequestServicesService objRequestServicesService;
        IRequestServiceUnitsService objRequestServiceUnitsService;
        IPlanService objPlanService;
        IClientUnitServiceCountService objClientUnitServiceCountService;
        IServicesService objServicesService;
        IUserNotificationService objUserNotificationService;
        IEmployeeService objEmployeeService;
        IEmailTemplateService objEmailTemplateService;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                FillPurposeOfVisitDropdown();
                hdnEmergencyAndOtherServiceWithinDays.Value = General.GetSitesettingsValue("EmergencyAndOtherServiceWithinDays");
                hdnMaintenanceServicesWithinDays.Value = General.GetSitesettingsValue("MaintenanceServicesWithinDays");
                hdnWeekTimeSlot.Value = General.GetSitesettingsValue("EmergencyServiceSlot1") + "|" + General.GetSitesettingsValue("EmergencyServiceSlot2");
                if (!string.IsNullOrEmpty(Request.QueryString["RequestServiceId"]))
                {
                    BindRequestServiceInfo();
                }
            }
        }

        private void BindRequestServiceInfo()
        {
            long RequestServiceId = Convert.ToInt64(Request.QueryString["RequestServiceId"].ToString());
            DataTable dtRequestService = new DataTable();
            objRequestServicesService = ServiceFactory.RequestServicesService;
            objClientService = ServiceFactory.ClientService;
            objRequestServiceUnitsService = ServiceFactory.RequestServiceUnitsService;

            objRequestServicesService.GetRequestedServiceById(RequestServiceId, ref dtRequestService);
            if (dtRequestService.Rows.Count > 0)
            {
                btnSave.Text = "Update";
                lnkSearch.Visible = false;

                txtClient.Text = dtRequestService.Rows[0]["ClientName"].ToString();

                DataTable dtClient = new DataTable();
                int ClientId = Convert.ToInt32(dtRequestService.Rows[0]["ClientId"].ToString());
                objClientService.GetClientById(ClientId, ref dtClient);
                if (dtClient.Rows.Count > 0)
                {
                    rblClient.DataSource = dtClient;
                    rblClient.DataTextField = dtClient.Columns["ClientName"].ToString();
                    rblClient.DataValueField = dtClient.Columns["Id"].ToString();
                    rblClient.DataBind();
                }
                rblClient.SelectedValue = ClientId.ToString();
                rblClient.Enabled = false;

                BindContactInfoByClientId(ClientId);
                BindAddressByClientId(ClientId);
                rblAddress.SelectedValue = dtRequestService.Rows[0]["AddressId"].ToString();
                rblAddress.Enabled = false;
                int AddressId = Convert.ToInt32(dtRequestService.Rows[0]["AddressId"].ToString());

                BindPlanTypeByAddressId(AddressId);
                int PlanTypeId = Convert.ToInt32(dtRequestService.Rows[0]["PlanTypeId"].ToString());
                drpPlanType.SelectedValue = PlanTypeId.ToString();
                drpPlanType.Enabled = false;
                BindUnitsByClientAddressIdAndPlanTypeId(ClientId, AddressId, PlanTypeId);
                DataTable dtReqUnits = new DataTable();
                objRequestServiceUnitsService.GetRequestServiceUnitByRequestServiceId(RequestServiceId, ref dtReqUnits);
                if (dtReqUnits.Rows.Count > 0)
                {
                    for (int i = 0; i < dtReqUnits.Rows.Count; i++)
                    {
                        ListItem item = chkUnits.Items.FindByValue(dtReqUnits.Rows[i]["UnitId"].ToString());
                        item.Selected = true;
                    }
                }


                //if (dtRequestService.Rows[0]["PurposeOfVisit"].ToString() == General.PurposeOfVisit.Emergency.GetEnumDescription())
                //{
                //    string[] TimeSlot = dtRequestService.Rows[0]["ServiceRequestedTime"].ToString().Split('-');
                //    txtStart.Value = TimeSlot[0].ToString();
                //}
                //else
                //{
                    string[] TimeSlot = dtRequestService.Rows[0]["TimeSlot"].ToString().Split('|');
                    if (dtRequestService.Rows[0]["ServiceRequestedTime"].ToString() == TimeSlot[0])
                        chkRequestedTime.Checked = true;
                    else
                        chkRequestedTime.Checked = false;
                //}


                txtServiceRequested.Text = dtRequestService.Rows[0]["ServiceRequestedOn"].ToString();
                //drpPurpose.SelectedValue = dtRequestService.Rows[0]["PurposeOfVisit"].ToString();
                General.PurposeOfVisit p = DurationExtensions.GetValueFromDescription<General.PurposeOfVisit>(dtRequestService.Rows[0]["PurposeOfVisit"].ToString());
                drpPurpose.SelectedValue = p.GetEnumValue().ToString();


                txtNotes.Text = dtRequestService.Rows[0]["Notes"].ToString();
            }
        }

        private void FillPurposeOfVisitDropdown()
        {
            var values = DurationExtensions.GetValues<General.PurposeOfVisit>();
            List<object> data = new List<object>();
            foreach (var item in values)
            {
                General.PurposeOfVisit p = (General.PurposeOfVisit)item;
                data.Add(new { Id = p.GetEnumValue(), Name = p.GetEnumDisplayName() });
            }
            drpPurpose.DataSource = data;
            drpPurpose.DataValueField = "Id";
            drpPurpose.DataTextField = "Name";
            drpPurpose.DataBind();
        }

        protected void lnkSearch_Click(object sender, EventArgs e)
        {
            dvMessage.InnerHtml = "";
            dvMessage.Visible = false;

            //if (!string.IsNullOrEmpty(txtClient.Text.Trim()))
            //{
            DataTable dtClients = new DataTable();
            objClientService = ServiceFactory.ClientService;
            objClientService.GetClientByName(txtClient.Text.Trim(), ref dtClients);
            if (dtClients.Rows.Count > 0)
            {
                rblClient.DataSource = dtClients;
                rblClient.DataTextField = dtClients.Columns["ClientName"].ToString();
                rblClient.DataValueField = dtClients.Columns["Id"].ToString();
            }
            else
                rblClient.DataSource = "";
            rblClient.DataBind();

            rblAddress.DataSource = "";
            rblAddress.DataBind();
            chkUnits.DataSource = "";
            chkUnits.DataBind();
            //}
        }

        protected void rblClient_SelectedIndexChanged(object sender, EventArgs e)
        {
            dvMessage.InnerHtml = "";
            dvMessage.Visible = false;

            BindAddressByClientId(Convert.ToInt32(rblClient.SelectedValue));
            BindContactInfoByClientId(Convert.ToInt32(rblClient.SelectedValue));
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

        private void BindAddressByClientId(int ClientId)
        {
            objClientAddressService = ServiceFactory.ClientAddressService;
            DataTable dtAddress = new DataTable();
            if (!string.IsNullOrEmpty(Request.QueryString["RequestServiceId"]))
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

        protected void rblAddress_SelectedIndexChanged(object sender, EventArgs e)
        {
            dvMessage.InnerHtml = "";
            dvMessage.Visible = false;
            chkUnits.DataSource = "";
            chkUnits.DataBind();
            hdnServiceTime.Value = "";
            drpPlanType.DataSource = "";
            drpPlanType.DataBind();

            BindPlanTypeByAddressId(Convert.ToInt32(rblAddress.SelectedValue));
            drpPlanType.Items.Insert(0, new ListItem("Select Plan", "0"));
        }

        private void BindPlanTypeByAddressId(int AddressId)
        {
            objPlanService = ServiceFactory.PlanService;
            DataTable dtPlanType = new DataTable();
            objPlanService.GetPlanByAddressId(AddressId, ref dtPlanType);
            if (dtPlanType.Rows.Count > 0)
            {
                drpPlanType.DataSource = dtPlanType;
                drpPlanType.DataTextField = dtPlanType.Columns["PlanName"].ToString();
                drpPlanType.DataValueField = dtPlanType.Columns["Id"].ToString();
            }
            else
                drpPlanType.DataSource = "";
            drpPlanType.DataBind();

        }

        private void BindUnitsByClientAddressIdAndPlanTypeId(int ClientId, int AddressId, int PlanTypeId)
        {
            objClientUnitService = ServiceFactory.ClientUnitService;
            DataTable dtUnits = new DataTable();
            objClientUnitService.GetUnitsByClientAddressAndPlanId(ClientId, AddressId, PlanTypeId, ref dtUnits);
            if (dtUnits.Rows.Count > 0)
            {
                chkUnits.DataSource = dtUnits;
                chkUnits.DataTextField = dtUnits.Columns["UnitName"].ToString();
                chkUnits.DataValueField = dtUnits.Columns["Id"].ToString();
                hdnServiceTime.Value = dtUnits.Rows[0]["ServiceSlot1"].ToString() + "|" + dtUnits.Rows[0]["ServiceSlot2"].ToString();
                hdnWeekEndTimeSlot.Value = dtUnits.Rows[0]["ServiceSlot1"].ToString() + "|" + dtUnits.Rows[0]["ServiceSlot2"].ToString();

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
            }
            else
            {
                chkUnits.DataSource = "";
                hdnServiceTime.Value = "";
            }
            chkUnits.DataBind();


        }

        protected void btnSave_Click(object sender, EventArgs e)
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
                    if (chkUnits.SelectedIndex == -1)
                    {
                        dvMessage.InnerHtml = "<strong>Please Select Service Units.</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-error");
                        dvMessage.Visible = true;
                        return;
                    }
                    General.PurposeOfVisit p = (General.PurposeOfVisit)Convert.ToInt32(drpPurpose.SelectedValue);

                    if (string.IsNullOrEmpty(Request.QueryString["RequestServiceId"]) && Convert.ToDateTime(txtServiceRequested.Text.Trim()).Date==DateTime.Now.Date)
                    {
                        //if (p.GetEnumDescription() == General.PurposeOfVisit.Emergency.GetEnumDescription() &&
                        //    Convert.ToDateTime(txtServiceRequested.Text.Trim()) == DateTime.Now.Date)
                        //{
                        //    if (!General.CheckTimeValidation(txtStart.Value + " - 11:55 PM", 0))
                        //    {
                        //        dvMessage.InnerHtml = "<strong>Please Select Future time.</strong>";
                        //        dvMessage.Attributes.Add("class", "alert alert-error");
                        //        dvMessage.Visible = true;
                        //        return;
                        //    }
                        //}
                        //else if (p.GetEnumDescription() != General.PurposeOfVisit.Emergency.GetEnumDescription() &&
                        //    Convert.ToDateTime(txtServiceRequested.Text.Trim()) == DateTime.Now.Date)
                        //{
                            string[] Slot = hdnServiceTime.Value.Split('|');
                            string SelectedSlot = string.Empty;
                            if (chkRequestedTime.Checked)
                                SelectedSlot = Slot[0];
                            else
                                SelectedSlot = Slot[1];

                            if (!General.CheckTimeValidation(SelectedSlot, 1))
                            {
                                dvMessage.InnerHtml = "<strong>Please Select Future time.</strong>";
                                dvMessage.Attributes.Add("class", "alert alert-error");
                                dvMessage.Visible = true;
                                return;
                            }
                        //}
                    }

                    LoginModel objLoginModel = new LoginModel();
                    objLoginModel = Session["LoginSession"] as LoginModel;

                    BizObjects.RequestService objRequestService = new BizObjects.RequestService();
                    BizObjects.RequestServiceUnits objRequestServiceUnits = new BizObjects.RequestServiceUnits();
                    objRequestServicesService = ServiceFactory.RequestServicesService;
                    objRequestServiceUnitsService = ServiceFactory.RequestServiceUnitsService;

                    objRequestService.ClientId = Convert.ToInt32(rblClient.SelectedValue);
                    objRequestService.AddressId = Convert.ToInt32(rblAddress.SelectedValue);
                    objRequestService.PurposeOfVisit = p.GetEnumDescription(); //drpPurpose.SelectedValue;
                    string[] TimeSlot = hdnServiceTime.Value.Split('|');
                    //if (p.GetEnumDescription() == General.PurposeOfVisit.Emergency.GetEnumDescription())
                    //{
                    //    objRequestService.ServiceRequestedTime = txtStart.Value + " - 11:55 PM";
                    //}
                    //else
                    //{
                        if (chkRequestedTime.Checked)
                            objRequestService.ServiceRequestedTime = TimeSlot[0];
                        else
                            objRequestService.ServiceRequestedTime = TimeSlot[1];
                    //}

                    objRequestService.ServiceRequestedOn = Convert.ToDateTime(txtServiceRequested.Text.Trim());
                    objRequestService.Notes = txtNotes.Text.Trim();
                    objRequestService.AddedBy = objLoginModel.Id;
                    objRequestService.AddedByType = objLoginModel.RoleId;
                    objRequestService.AddedDate = DateTime.UtcNow;

                    long RequestServiceId = 0;
                    if (!string.IsNullOrEmpty(Request.QueryString["RequestServiceId"]))
                    {
                        RequestServiceId = Convert.ToInt64(Request.QueryString["RequestServiceId"].ToString());
                        objRequestService.Id = RequestServiceId;
                        objRequestService.UpdatedBy = objLoginModel.Id;
                        objRequestService.UpdatedByType = objLoginModel.RoleId;
                        objRequestService.UpdatedDate = DateTime.UtcNow;
                        objRequestServicesService.UpdateRequestService(ref objRequestService);
                        objRequestServiceUnitsService.DeleteRequestServiceUnitsByReqServiceId(RequestServiceId);
                    }
                    else
                    {
                        RequestServiceId = objRequestServicesService.AddRequestService(ref objRequestService);
                    }

                    foreach (ListItem item in chkUnits.Items)
                    {
                        if (item.Selected)
                        {
                            objRequestServiceUnits.ServiceId = RequestServiceId;
                            objRequestServiceUnits.UnitId = Convert.ToInt32(item.Value);
                            objRequestServiceUnitsService.AddRequestServiceUnits(ref objRequestServiceUnits);
                        }
                    }

                    if (string.IsNullOrEmpty(Request.QueryString["RequestServiceId"]))
                    {
                        objClientUnitServiceCountService = ServiceFactory.ClientUnitServiceCountService;
                        if (RequestServiceId != 0)
                            objClientUnitServiceCountService.UpdateClientUnitServiceCountWithRequestedService(RequestServiceId);

                        if (p.GetEnumDescription() == General.PurposeOfVisit.Emergency.GetEnumDescription() && RequestServiceId != 0)
                        {
                            objServicesService = ServiceFactory.ServicesService;
                            DataTable dtServices = new DataTable();
                            objServicesService.EmergencyRequestedServiceSchedule(RequestServiceId, ref dtServices);
                            if (dtServices.Rows.Count > 0)
                            {
                                int EmployeeId = Convert.ToInt32(dtServices.Rows[0]["EmployeeId"].ToString());
                                if (EmployeeId > 0)
                                {
                                    DateTime ScheduleDate = Convert.ToDateTime(dtServices.Rows[0]["ScheduleDate"].ToString());
                                    long ServiceId = Convert.ToInt64(dtServices.Rows[0]["ServiceId"].ToString());


                                    string message = string.Empty;

                                    objClientService = ServiceFactory.ClientService;
                                    //Send Notification to Client
                                    message = General.GetNotificationMessage("RequestedServiceScheduleSendToClient");
                                    message = message.Replace("{{ScheduleDate}}", ScheduleDate.ToString("MMMM dd, yyyy"));
                                    DataTable dtClient = new DataTable();

                                    objClientService.GetClientById(Convert.ToInt32(rblClient.SelectedValue.ToString()), ref dtClient);
                                    if (dtClient.Rows.Count > 0)
                                    {
                                        if (!string.IsNullOrEmpty(dtClient.Rows[0]["DeviceType"].ToString()) &&
                                                !string.IsNullOrEmpty(dtClient.Rows[0]["DeviceToken"].ToString()) &&
                                                dtClient.Rows[0]["DeviceToken"].ToString().ToLower() != "no device token")
                                        {
                                            NotifyUser(Convert.ToInt32(rblClient.SelectedValue.ToString()), "client", General.NotificationType.ServiceScheduled.GetEnumDescription(), General.NotificationType.ServiceScheduled.GetEnumValue(), ServiceId, message, dtClient.Rows[0]["DeviceType"].ToString(), dtClient.Rows[0]["DeviceToken"].ToString().ToLower());
                                        }
                                    }

                                    //Send Notification to Employee
                                    message = string.Empty;

                                    message = General.GetNotificationMessage("EmployeeSchedule");
                                    message = message.Replace("{{ScheduleDate}}", ScheduleDate.ToString("MMMM dd, yyyy"));


                                    objEmployeeService = ServiceFactory.EmployeeService;
                                    DataTable dtEmployee = new DataTable();
                                    objEmployeeService.GetEmployeeById(EmployeeId, ref dtEmployee);
                                    if (dtEmployee.Rows.Count > 0)
                                    {
                                        if (!string.IsNullOrEmpty(dtEmployee.Rows[0]["DeviceType"].ToString()) &&
                                                !string.IsNullOrEmpty(dtEmployee.Rows[0]["DeviceToken"].ToString()) &&
                                                dtEmployee.Rows[0]["DeviceToken"].ToString().ToLower() != "no device token")
                                        {
                                            if (dtEmployee.Rows[0]["DeviceType"].ToString().ToLower() == "iphone")
                                            {
                                                NotifyUser(EmployeeId, "employee", General.NotificationType.ServiceScheduled.GetEnumDescription(), General.NotificationType.ServiceScheduled.GetEnumValue(), ServiceId, message, dtEmployee.Rows[0]["DeviceType"].ToString(), dtEmployee.Rows[0]["DeviceToken"].ToString().ToLower());
                                            }
                                        }
                                    }
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
                                        Emailbody = Emailbody.Replace("{{PurposeOfVisit}}", p.GetEnumDescription());
                                        Emailbody = Emailbody.Replace("{{ScheduleDate}}", Convert.ToDateTime(dtEmailService.Rows[0]["ScheduleDate"].ToString()).ToString("MMMM dd, yyyy"));
                                        Emailbody = Emailbody.Replace("{{ScheduleTime}}", dtEmailService.Rows[0]["ScheduleStartTime"].ToString() + " - " + dtEmailService.Rows[0]["ScheduleEndTime"].ToString());
                                        Emailbody = Emailbody.Replace("{{Technician}}", dtEmailService.Rows[0]["EmployeeName"].ToString());
                                        string Subject = dtEmailtemplate.Rows[0]["EmailTemplateSubject"].ToString();
                                        Subject = Subject.Replace("{{PurposeOfVisit}}", p.GetEnumDescription());
                                        Email.SendEmail(Subject, dtClient.Rows[0]["Email"].ToString(), CCEmail, "", Emailbody);
                                    }
                                    Response.Redirect(Application["SiteAddress"] + "admin/PendingService_List.aspx?msg=requested");
                                }
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(Request.QueryString["RequestServiceId"]))
                        Response.Redirect(Application["SiteAddress"] + "admin/PendingService_List.aspx?msg=edit");
                    else
                        Response.Redirect(Application["SiteAddress"] + "admin/PendingService_List.aspx?msg=add");
                }
                else
                {
                    Response.Redirect(Application["SiteAddress"] + "/admin/Login.aspx");
                }
            }
            catch (Exception Ex)
            {
                dvMessage.InnerHtml = "<strong>Error!</strong> " + Ex.Message.ToString().Trim();
                dvMessage.Attributes.Add("class", "alert alert-error");
                dvMessage.Visible = true;
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

        protected void drpPlanType_SelectedIndexChanged(object sender, EventArgs e)
        {
            chkUnits.DataSource = "";
            chkUnits.DataBind();

            BindUnitsByClientAddressIdAndPlanTypeId(Convert.ToInt32(rblClient.SelectedValue), Convert.ToInt32(rblAddress.SelectedValue), Convert.ToInt32(drpPlanType.SelectedValue));
        }
    }
}