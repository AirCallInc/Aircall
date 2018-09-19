using Aircall.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Services;
using System.Data;
using BizObjects;

namespace Aircall.client
{
    public partial class request_services : System.Web.UI.Page
    {
        #region "Declaration"
        IClientAddressService objClientAddressService;
        IClientUnitService objClientUnitService;
        IRequestServicesService objRequestServicesService;
        IRequestServiceUnitsService objRequestServiceUnitsService;
        IPlanService objPlanService;
        IServicesService objServicesService;
        #endregion

        #region "Page Events"
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["ClientLoginCookie"] != null)
            {
                if (!IsPostBack)
                {
                    FillPurposeOfVisitDropdown();
                    BindClientAddressesByClientId();
                    if (Request.QueryString["rid"] != null)
                    {
                        objRequestServicesService = ServiceFactory.RequestServicesService;
                        objRequestServiceUnitsService = ServiceFactory.RequestServiceUnitsService;


                        DataTable dtRequest = new DataTable();
                        //int RequestId = int.Parse(Request.QueryString["rid"].ToString());
                        int RequestId;// = int.Parse(Request.QueryString["rid"].ToString());                    
                        if (!int.TryParse(Request.QueryString["rid"], out RequestId))
                        {
                            Response.Redirect("request-service-list.aspx", false);
                        }
                        objRequestServicesService.GetRequestedServiceById(RequestId, (Session["ClientLoginCookie"] as LoginModel).Id, ref dtRequest);
                        if (dtRequest.Rows.Count > 0)
                        {
                            var row = dtRequest.Rows[0];
                            General.PurposeOfVisit p = DurationExtensions.GetValueFromDescription<General.PurposeOfVisit>(row["PurposeOfVisit"].ToString());
                            drpPurposeOfVisit.SelectedValue = p.GetEnumValue().ToString();
                            drpClientAddress.SelectedValue = row["AddressId"].ToString();
                            BindPlanByAddressId(Convert.ToInt32(row["AddressId"].ToString()));

                            drpClientAddress.Enabled = false;
                            drpPurposeOfVisit.Enabled = false;

                            chkUnits.DataSource = "";
                            chkUnits.DataBind();
                            txtDate.Value = DateTime.Parse(dtRequest.Rows[0]["ServiceRequestedOn"].ToString()).Date.ToString("MM/dd/yyyy");
                            int ClientId = (Session["ClientLoginCookie"] as LoginModel).Id;
                            objClientUnitService = ServiceFactory.ClientUnitService;
                            DataTable dtUnits = new DataTable();
                            objRequestServiceUnitsService.GetRequestServiceUnitByRequestServiceId(RequestId, ref dtUnits);

                            if (dtUnits.Rows.Count > 0)
                            {
                                DataRow row1 = dtUnits.Rows[0];
                                rblPlan.SelectedValue = row1["PlanTypeId"].ToString();
                                rblPlan.Enabled = false;
                                BindTimeSlotAndUnitByPlanId(row1["PlanTypeId"].ToString());
                                chkUnits.Enabled = false;
                            }
                            string[] Slot = hdnTimeSlot.Value.Split(("|").ToArray());
                            string[] SlotE = hdnTimeSlotE.Value.Split(("|").ToArray());
                            if (p.GetEnumDescription() != General.PurposeOfVisit.Emergency.GetEnumDescription())
                            {
                                if (Slot[0] == dtRequest.Rows[0]["ServiceRequestedTime"].ToString())
                                {
                                    rdslot1.Checked = true;
                                    rdslot2.Checked = false;
                                }
                                else
                                {
                                    rdslot2.Checked = true;
                                    rdslot1.Checked = false;
                                }
                            }
                            else
                            {
                                DayOfWeek dow = DateTime.Parse(dtRequest.Rows[0]["ServiceRequestedOn"].ToString()).DayOfWeek;
                                if (dow == DayOfWeek.Sunday || dow == DayOfWeek.Saturday)
                                {
                                    if (SlotE[0] == dtRequest.Rows[0]["ServiceRequestedTime"].ToString())
                                    {
                                        rdslot1.Checked = true;
                                        rdslot2.Checked = false;
                                    }
                                    else
                                    {
                                        rdslot2.Checked = true;
                                        rdslot1.Checked = false;
                                    }
                                }
                                else
                                {
                                    if (SlotE[0] == dtRequest.Rows[0]["ServiceRequestedTime"].ToString())
                                    {
                                        rdslot1E.Checked = true;
                                        rdslot2E.Checked = false;
                                    }
                                    else
                                    {
                                        rdslot2E.Checked = true;
                                        rdslot1E.Checked = false;
                                    }
                                }
                            }
                            if (dtUnits.Rows.Count > 0)
                            {
                                foreach (DataRow row1 in dtUnits.Rows)
                                {
                                    foreach (ListItem item in chkUnits.Items)
                                    {
                                        if (item.Value == row1["UnitId"].ToString())
                                        {
                                            item.Selected = true;
                                        }
                                    }
                                }
                            }
                            txtNotes.Text = dtRequest.Rows[0]["Notes"].ToString();
                        }
                        else
                        {
                            Response.Redirect("request-service-list.aspx");
                        }
                    }
                }
            }
            else
                Response.Redirect(Application["SiteAddress"] + "sign-in.aspx", false);
        }
        #endregion

        #region "Functions"
        private void FillPurposeOfVisitDropdown()
        {
            var values = DurationExtensions.GetValues<General.PurposeOfVisit>();
            List<object> data = new List<object>();
            foreach (var item in values)
            {
                General.PurposeOfVisit p = (General.PurposeOfVisit)item;
                data.Add(new { Id = p.GetEnumValue(), Name = p.GetEnumDisplayName() });
            }
            drpPurposeOfVisit.DataSource = data;
            drpPurposeOfVisit.DataValueField = "Id";
            drpPurposeOfVisit.DataTextField = "Name";
            drpPurposeOfVisit.DataBind();
        }

        private void BindClientAddressesByClientId()
        {
            if (Session["ClientLoginCookie"] != null)
            {
                int ClientId = (Session["ClientLoginCookie"] as LoginModel).Id;
                objClientAddressService = ServiceFactory.ClientAddressService;
                DataTable dtAddress = new DataTable();
                if (Request.QueryString["rid"] != null)
                    objClientAddressService.GetClientAddressesByClientId(ClientId, false, ref dtAddress);
                else
                    objClientAddressService.GetClientAddressesByClientId(ClientId, false, ref dtAddress);

                string filter = "ShowInList = True";
                DataView dv = new DataView(dtAddress, filter, "", DataViewRowState.CurrentRows);
                dtAddress = dv.ToTable();

                if (dtAddress.Rows.Count > 0)
                {
                    drpClientAddress.DataSource = dtAddress;
                    drpClientAddress.DataTextField = dtAddress.Columns["ClientAddress"].ToString();
                    drpClientAddress.DataValueField = dtAddress.Columns["Id"].ToString();
                }
                drpClientAddress.DataBind();
                drpClientAddress.Items.Insert(0, new ListItem("Select Address", "0"));
            }
            else
                Response.Redirect(Application["SiteAddress"] + "sign-in.aspx");
        }
        #endregion

        #region "Events"
        protected void drpClientAddress_SelectedIndexChanged(object sender, EventArgs e)
        {
            rblPlan.DataSource = "";
            rblPlan.DataBind();
            chkUnits.DataSource = "";
            chkUnits.DataBind();
            if (drpClientAddress.SelectedValue != "0")
            {
                BindPlanByAddressId(int.Parse(drpClientAddress.SelectedValue));
            }
            else
            {
                dvRequestedTime.Visible = false;
            }
        }

        private void BindPlanByAddressId(int AddressId)
        {
            DataTable dtPlan = new DataTable();
            objPlanService = ServiceFactory.PlanService;
            objPlanService.GetPlanByAddressId(AddressId, ref dtPlan);

            if (dtPlan.Rows.Count > 0)
            {
                rblPlan.DataSource = dtPlan;
                rblPlan.DataValueField = "Id";
                rblPlan.DataTextField = "PlanName";
                rblPlan.DataBind();
            }
        }
        #endregion

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            objRequestServicesService = ServiceFactory.RequestServicesService;
            objRequestServiceUnitsService = ServiceFactory.RequestServiceUnitsService;

            if (rblPlan.Items.Count == 0 || rblPlan.SelectedIndex == -1)
            {
                dvMessage.InnerHtml = "<strong>Please Select Plan</strong>";
                dvMessage.Attributes.Add("class", "error");
                dvMessage.Visible = true;
                return;
            }
            if (chkUnits.SelectedIndex == -1)
            {
                dvMessage.InnerHtml = "<strong>Please Select Service Units.</strong>";
                dvMessage.Attributes.Add("class", "error");
                dvMessage.Visible = true;
                return;
            }

            DataTable dt = new DataTable();
            if (Session["ClientLoginCookie"] != null)
            {
                int ClientId = (Session["ClientLoginCookie"] as LoginModel).Id;
                objRequestServicesService.GetRequestedServiceByClientId(ClientId, ref dt);
                string[] Slot = hdnTimeSlot.Value.Split(("|").ToArray());
                string[] SlotE = hdnTimeSlotE.Value.Split(("|").ToArray());
                string selectedSlot = "";
                DateTime d = DateTime.Parse(txtDate.Value);
                var p = (General.PurposeOfVisit)Convert.ToInt32(drpPurposeOfVisit.SelectedValue);
                if (p.GetEnumDescription() != General.PurposeOfVisit.Emergency.GetEnumDescription())
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

                if (Convert.ToDateTime(txtDate.Value.Trim()) == DateTime.Now.Date)
                {
                    if (!General.CheckTimeValidation(selectedSlot, 1))
                    {
                        dvMessage.InnerHtml = "<strong>Please Select Future time.</strong>";
                        dvMessage.Attributes.Add("class", "error");
                        dvMessage.Visible = true;
                        return;
                    }
                }
                if (Request.QueryString["rid"] == null)
                {
                    var rows = dt.Select(" ServiceRequestedOn='" + txtDate.Value + "' ");
                    if (rows.Length > 0)
                    {
                        DataTable dt1 = new DataTable();

                        for (var i = 0; i < rows.Length; i++)
                        {
                            objRequestServiceUnitsService.GetRequestServiceUnitByRequestServiceId(int.Parse(rows[i]["Id"].ToString()), ref dt1);
                            bool added = false;
                            foreach (ListItem item in chkUnits.Items)
                            {
                                if (item.Selected)
                                {
                                    if (dt1.Select(" UnitId=" + item.Value).Length > 0)
                                    {
                                        added = true;
                                    }
                                }
                            }
                            if (added)
                            {
                                dvMessage.InnerText = "Some of the selected units were already requested for this date.";
                                dvMessage.Attributes.Add("class", "error");
                                dvMessage.Visible = true;
                                return;
                            }
                        }
                    }
                }

                //var p = (General.PurposeOfVisit)Convert.ToInt32(drpPurposeOfVisit.SelectedValue);
                RequestService rs = new RequestService();
                rs.ClientId = ClientId;
                rs.Notes = (string.IsNullOrWhiteSpace(txtNotes.Text) ? "No notes has been added" : txtNotes.Text);
                rs.PurposeOfVisit = p.GetEnumDescription();
                rs.ServiceRequestedTime = selectedSlot;
                rs.ServiceRequestedOn = d;
                rs.AddressId = Convert.ToInt32(drpClientAddress.SelectedValue);
                rs.AddedBy = ClientId;
                rs.AddedByType = General.UserRoles.Client.GetEnumValue();
                rs.AddedDate = DateTime.UtcNow;

                long rtn = 0;
                if (Request.QueryString["rid"] == null)
                {
                    rtn = objRequestServicesService.AddRequestService(ref rs);
                }
                else
                {
                    rtn = Convert.ToInt64(Request.QueryString["rid"].ToString());
                    rs.Id = rtn;
                    rs.UpdatedBy = ClientId;
                    rs.UpdatedByType = General.UserRoles.Client.GetEnumValue();
                    rs.UpdatedDate = DateTime.UtcNow;
                    objRequestServicesService.UpdateRequestService(ref rs);

                    objRequestServiceUnitsService.DeleteRequestServiceUnitsByReqServiceId(rtn);
                }

                if (rtn > 0)
                {
                    foreach (ListItem item in chkUnits.Items)
                    {
                        if (item.Selected)
                        {
                            RequestServiceUnits rsu = new RequestServiceUnits();
                            rsu.UnitId = Convert.ToInt16(item.Value);
                            rsu.ServiceId = rtn;
                            objRequestServiceUnitsService.AddRequestServiceUnits(ref rsu);
                        }
                    }
                    if (Request.QueryString["rid"] == null)
                    {
                        IClientUnitServiceCountService objClientUnitServiceCountService = ServiceFactory.ClientUnitServiceCountService;
                        objClientUnitServiceCountService.UpdateClientUnitServiceCountWithRequestedService(rtn);

                        if (rs.PurposeOfVisit == General.PurposeOfVisit.Emergency.GetEnumDescription())
                        {
                            DataTable dtServices = new DataTable();
                            objServicesService = ServiceFactory.ServicesService;
                            objServicesService.EmergencyRequestedServiceSchedule(rtn, ref dtServices);

                            if (dtServices.Rows.Count > 0)
                            {
                                int EmployeeId = Convert.ToInt32(dtServices.Rows[0]["EmployeeId"].ToString());
                                if (EmployeeId > 0)
                                {
                                    DateTime ScheduleDate = Convert.ToDateTime(dtServices.Rows[0]["ScheduleDate"].ToString());
                                    long ServiceId = Convert.ToInt64(dtServices.Rows[0]["ServiceId"].ToString());


                                    string message = string.Empty;

                                    IClientService objClientService = ServiceFactory.ClientService;
                                    //Send Notification to Client
                                    message = General.GetNotificationMessage("RequestedServiceScheduleSendToClient");
                                    message = message.Replace("{{ScheduleDate}}", ScheduleDate.ToString("MMMM dd, yyyy"));
                                    DataTable dtClient = new DataTable();

                                    objClientService.GetClientById(ClientId, ref dtClient);
                                    if (dtClient.Rows.Count > 0)
                                    {
                                        NotifyUser(ClientId, "client", General.NotificationType.ServiceScheduled.GetEnumDescription(), General.NotificationType.ServiceScheduled.GetEnumValue(), ServiceId, message, dtClient.Rows[0]["DeviceType"].ToString(), dtClient.Rows[0]["DeviceToken"].ToString().ToLower());

                                        DataTable dtEmailtemplate = new DataTable();
                                        DataTable dtEmailService = new DataTable();
                                        IEmailTemplateService objEmailTemplateService = ServiceFactory.EmailTemplateService;
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
                                    }
                                    //Send Notification to Employee
                                    message = string.Empty;

                                    message = General.GetNotificationMessage("EmployeeSchedule");
                                    message = message.Replace("{{ScheduleDate}}", ScheduleDate.ToString("MMMM dd, yyyy"));


                                    IEmployeeService objEmployeeService = ServiceFactory.EmployeeService;
                                    DataTable dtEmployee = new DataTable();
                                    objEmployeeService.GetEmployeeById(EmployeeId, ref dtEmployee);
                                    if (dtEmployee.Rows.Count > 0)
                                    {
                                        NotifyUser(EmployeeId, "employee", General.NotificationType.ServiceScheduled.GetEnumDescription(), General.NotificationType.ServiceScheduled.GetEnumValue(), ServiceId, message, dtEmployee.Rows[0]["DeviceType"].ToString(), dtEmployee.Rows[0]["DeviceToken"].ToString().ToLower());
                                    }
                                }
                            }
                        }
                    }
                }
                var MaintenanceServiceSubmitMessage = General.GetSitesettingsValue("MaintenanceServiceSubmitMessage");
                var RepairServiceSubmitMessage = General.GetSitesettingsValue("RepairServiceSubmitMessage");
                var ContinuingPreviousWorkServiceSubmitMessage = General.GetSitesettingsValue("ContinuingPreviousWorkServiceSubmitMessage");
                var EmergencyServiceSubmitMessage = General.GetSitesettingsValue("EmergencyServiceSubmitMessage");

                if (Request.QueryString["rid"] == null)
                    Session["success"] = 1;
                else
                    Session["success"] = 2;

                Response.Redirect("request-service-list.aspx");
            }
        }
        private void NotifyUser(int UserId, string Role, string MessageType, int NoType, long ServiceId, string message, string DeviceType, string DeviceToken)
        {
            long NotificationId = 0;
            DataTable dtBadgeCount = new DataTable();
            int BadgeCount = 0;
            BizObjects.UserNotification objUserNotification = new BizObjects.UserNotification();
            IUserNotificationService objUserNotificationService = ServiceFactory.UserNotificationService;
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
        protected void rblPlan_SelectedIndexChanged(object sender, EventArgs e)
        {
            chkUnits.DataSource = "";
            chkUnits.DataBind();

            BindTimeSlotAndUnitByPlanId(rblPlan.SelectedValue);
            dvRequestedTime.Visible = true;
        }

        private void BindTimeSlotAndUnitByPlanId(string PlanId)
        {
            var EmergencyServiceSlot1 = General.GetSitesettingsValue("EmergencyServiceSlot1");
            var EmergencyServiceSlot2 = General.GetSitesettingsValue("EmergencyServiceSlot2");
            int ClientId = (Session["ClientLoginCookie"] as LoginModel).Id;
            objClientUnitService = ServiceFactory.ClientUnitService;
            DataTable dtUnits = new DataTable();
            objClientUnitService.GetClientUnitByClientAndAddressIdPlanForPortal(ClientId, Convert.ToInt32(drpClientAddress.SelectedValue), Convert.ToInt32(rblPlan.SelectedValue), ref dtUnits);
            if (dtUnits.Rows.Count > 0)
            {
                chkUnits.DataSource = dtUnits;
                chkUnits.DataTextField = dtUnits.Columns["UnitName"].ToString();
                chkUnits.DataValueField = dtUnits.Columns["Id"].ToString();
                chkUnits.DataBind();
            }

            DataTable dtPlan = new DataTable();
            objPlanService = ServiceFactory.PlanService;
            int AddressId = int.Parse(drpClientAddress.SelectedValue);
            objPlanService.GetPlanByAddressId(AddressId, ref dtPlan);
            if (dtPlan.Rows.Count > 0)
            {
                var rows = dtPlan.Select(" Id = " + PlanId.ToString() + " ");
                if (rows.Length > 0)
                {
                    ltrSlot1.Text = rows[0]["ServiceSlot1"].ToString();
                    ltrSlot2.Text = rows[0]["ServiceSlot2"].ToString();

                    ltrSlot1E.Text = EmergencyServiceSlot1;
                    ltrSlot2E.Text = EmergencyServiceSlot2;
                    hdnTimeSlot.Value = rows[0]["ServiceSlot1"].ToString() + "|" + rows[0]["ServiceSlot2"].ToString();
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
    }
}