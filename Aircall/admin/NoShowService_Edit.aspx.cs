using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Services;
using Aircall.Common;
using System.Data;

namespace Aircall.admin
{
    public partial class NoShowService_Edit : System.Web.UI.Page
    {
        IServicesService objServicesService;
        IServiceUnitService objServiceUnitService;
        IClientService objClientService;
        IAreasService objAreasService;
        IEmployeeService objEmployeeService;
        IClientAddressService objClientAddressService;
        IClientUnitService objClientUnitService;
        IEmployeeWorkAreaService objEmployeeWorkAreaService;
        IServiceReportService objServiceReportService;
        ISiteSettingService objSiteSettingService;
        IUserNotificationService objUserNotificationService;
        IServiceNoShowService objServiceNoShowService;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                FillPurposeOfVisitDropdown();
                FillServiceStatusDropdown();
                if (!string.IsNullOrEmpty(Request.QueryString["ServiceId"]))
                {
                    BindServiceInformationByServiceId();
                }
            }
        }

        private void BindServiceInformationByServiceId()
        {
            lnkAreaSearch.Visible = false;
            lnkEmpSearch.Visible = false;

            long ServiceId = Convert.ToInt64(Request.QueryString["ServiceId"]);
            objServicesService = ServiceFactory.ServicesService;
            objServiceUnitService = ServiceFactory.ServiceUnitService;
            objClientService = ServiceFactory.ClientService;
            objAreasService = ServiceFactory.AreasService;
            objEmployeeService = ServiceFactory.EmployeeService;
            objServiceNoShowService = ServiceFactory.ServiceNoShowService;
            DataTable dtNoShow = new DataTable();
            objServiceNoShowService.GetAllByServiceId(ServiceId, ref dtNoShow);
            if (dtNoShow.Rows.Count > 0)
            {
                btnMakePayment.Visible = false;
                btnNotifyAndSchedule.Visible = false;
            }


            DataTable dtService = new DataTable();
            objServicesService.GetServiceById(ServiceId, ref dtService);
            if (dtService.Rows.Count > 0)
            {
                int ClientId = Convert.ToInt32(dtService.Rows[0]["ClientId"].ToString());
                int AddressId = Convert.ToInt32(dtService.Rows[0]["AddressID"].ToString());
                string Status = dtService.Rows[0]["Status"].ToString();
                hdnStatus.Value = dtService.Rows[0]["Status"].ToString();
                //txtClient.Text = dtService.Rows[0]["ClientName"].ToString();
                lblClient.Text = dtService.Rows[0]["ClientName"].ToString();

                if (Status==General.ServiceTypes.LateCancelled.GetEnumDescription())
                {
                    btnMakePayment.Text = "Notify For Late Cancelled Payment";
                    btnNotifyAndSchedule.Text = "Notify and Cancel Service";
                }

                BindAddressByClientId(ClientId);
                rblAddress.SelectedValue = AddressId.ToString();
                rblAddress.Enabled = false;

                ltrMobile.Text = dtService.Rows[0]["MobileNumber"].ToString();
                ltrHome.Text = dtService.Rows[0]["HomeNumber"].ToString();
                ltrOffice.Text = dtService.Rows[0]["OfficeNumber"].ToString();
                BindUnitsByClientAndAddressId(ClientId, AddressId);

                DataTable dtServiceUnit = new DataTable();
                objServiceUnitService.GetServiceUnitByServiceId(ServiceId, ref dtServiceUnit);
                if (dtServiceUnit.Rows.Count > 0)
                {
                    for (int i = 0; i < dtServiceUnit.Rows.Count; i++)
                    {
                        ListItem item = chkUnits.Items.FindByValue(dtServiceUnit.Rows[i]["UnitId"].ToString());
                        item.Selected = true;
                    }
                    chkUnits.Enabled = false;
                }
                drpPurpose.SelectedValue = dtService.Rows[0]["PurposeOfVisit"].ToString();

                int ReportCount = 0;

                ReportCount = Convert.ToInt32(dtService.Rows[0]["ReportCount"].ToString());
                hdnReportCnt.Value = ReportCount.ToString();

                if (ReportCount > 0)
                {
                    dvLateReschedule.Visible = true;

                    txtWorkArea.Text = dtService.Rows[0]["AreaName"].ToString();
                    int WorkAreaId = Convert.ToInt32(dtService.Rows[0]["WorkAreaId"].ToString());
                    DataTable dtWorkAreas = new DataTable();
                    objAreasService.GetAreaById(WorkAreaId, true, ref dtWorkAreas);
                    if (dtWorkAreas.Rows.Count > 0)
                    {
                        rblWorkArea.DataSource = dtWorkAreas;
                        rblWorkArea.DataValueField = dtWorkAreas.Columns["Id"].ToString();
                        rblWorkArea.DataTextField = dtWorkAreas.Columns["Name"].ToString();
                        rblWorkArea.DataBind();
                    }
                    rblWorkArea.SelectedValue = WorkAreaId.ToString();
                    rblWorkArea.Enabled = false;

                    txtEmployee.Text = dtService.Rows[0]["EmployeeName"].ToString();
                    int EmployeeId = Convert.ToInt32(dtService.Rows[0]["EmployeeId"].ToString());
                    DataTable dtEmployee = new DataTable();
                    objEmployeeService.GetEmployeeById(EmployeeId, ref dtEmployee);
                    if (dtEmployee.Rows.Count > 0)
                    {
                        rblEmployee.DataSource = dtEmployee;
                        rblEmployee.DataTextField = dtEmployee.Columns["EmployeeName"].ToString();
                        rblEmployee.DataValueField = dtEmployee.Columns["Id"].ToString();
                        rblEmployee.DataBind();
                    }

                    rblEmployee.SelectedValue = EmployeeId.ToString();
                    rblEmployee.Enabled = false;
                    txtScheduleOn.Text = Convert.ToDateTime(dtService.Rows[0]["ScheduleDate"].ToString()).ToString("MM/dd/yyyy");
                    hdnScheduleOn.Value = dtService.Rows[0]["ScheduleDate"].ToString();
                    //txtStart.Value = dtService.Rows[0]["ScheduleStartTime"].ToString();
                    //txtEnd.Value = dtService.Rows[0]["ScheduleEndTime"].ToString();
                    ltrStart.Text = dtService.Rows[0]["ScheduleStartTime"].ToString();
                    ltrEnd.Text = dtService.Rows[0]["ScheduleEndTime"].ToString();
                    drpServiceStatus.SelectedValue = dtService.Rows[0]["Status"].ToString();

                    objServiceReportService = ServiceFactory.ServiceReportService;
                    DataTable dtServiceReport = new DataTable();
                    objServiceReportService.GetServiceReportByServiceId(ServiceId, ref dtServiceReport);
                    if (dtServiceReport.Rows.Count > 0)
                        txtNoShowEmpReason.Text = dtServiceReport.Rows[0]["WorkPerformed"].ToString();
                }
                else
                {
                    dvLateReschedule.Visible = false;
                }


                objSiteSettingService = ServiceFactory.SiteSettingService;
                DataTable dtSiteSetting = new DataTable();
                objSiteSettingService.GetSiteSettingByName("NoShowFeeInUSD", ref dtSiteSetting);
                if (dtSiteSetting.Rows.Count > 0)
                    txtNoShowAmount.Text = dtSiteSetting.Rows[0]["Value"].ToString();

                if (dtNoShow.Rows.Count > 0)
                    txtNoShowAmount.Text = dtNoShow.Rows[0]["NoShowAmount"].ToString();
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

        private void FillServiceStatusDropdown()
        {
            var values = DurationExtensions.GetValues<General.ServiceTypes>();
            List<string> data = new List<string>();
            foreach (var item in values)
            {
                General.ServiceTypes p = (General.ServiceTypes)item;
                data.Add(p.GetEnumDescription());
            }
            drpServiceStatus.DataSource = data;
            drpServiceStatus.DataBind();
        }

        private void BindAddressByClientId(int ClientId)
        {
            objClientAddressService = ServiceFactory.ClientAddressService;
            DataTable dtAddress = new DataTable();
            objClientAddressService.GetClientAddressesByClientId(ClientId, true, ref dtAddress);
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
            }
            else
                chkUnits.DataSource = "";
            chkUnits.DataBind();
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

        protected void btnMakePayment_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(Request.QueryString["ServiceId"]) &&
                        Session["LoginSession"] != null)
                {
                    LoginModel objLoginModel = new LoginModel();
                    objLoginModel = Session["LoginSession"] as LoginModel;

                    decimal NoShowAmount = string.IsNullOrEmpty(txtNoShowAmount.Text.Trim()) ? 0 : Convert.ToDecimal(txtNoShowAmount.Text.Trim());

                    if (NoShowAmount == 0)
                    {
                        dvMessage.InnerHtml = "<strong>Invalid No Show Amount.</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-error");
                        dvMessage.Visible = true;
                        return;
                    }


                    long ServiceId = Convert.ToInt64(Request.QueryString["ServiceId"]);
                    objServicesService = ServiceFactory.ServicesService;
                    objClientService = ServiceFactory.ClientService;
                    BizObjects.ServiceNoShow objServiceNoShow = new BizObjects.ServiceNoShow();
                    objServiceNoShow.ServiceId = ServiceId;
                    objServiceNoShow.NoShowAmount = Convert.ToDecimal(txtNoShowAmount.Text);
                    if (Convert.ToInt32(hdnReportCnt.Value) == 0)
                        objServiceNoShow.IsNoShow = false;
                    else
                        objServiceNoShow.IsNoShow = true;
                    objServiceNoShow.IsPaymentReceived = false;
                    objServiceNoShow.AddedBy = objLoginModel.Id;
                    objServiceNoShow.AddedByType = objLoginModel.RoleId;
                    objServiceNoShow.AddedDate = DateTime.UtcNow;
                    objServiceNoShowService = ServiceFactory.ServiceNoShowService;

                    objServiceNoShowService.AddServiceNoShow(ref objServiceNoShow);

                    DataTable dtService = new DataTable();
                    DataTable dtClient = new DataTable();
                    DataTable dtBadgeCount = new DataTable();
                    objUserNotificationService = ServiceFactory.UserNotificationService;
                    objServicesService.GetServiceById(ServiceId, ref dtService);
                    if (dtService.Rows.Count > 0)
                    {
                        int ClientId = Convert.ToInt32(dtService.Rows[0]["ClientId"].ToString());
                        objClientService.GetClientById(ClientId, ref dtClient);
                        if (dtClient.Rows.Count > 0)
                        {

                            long NotificationId = 0;
                            int BadgeCount = 0;
                            string MessageType = string.Empty;
                            int NType = 0;
                            if (hdnStatus.Value==General.ServiceTypes.LateCancelled.GetEnumDescription())
                            {
                                MessageType=General.NotificationType.LateCancelled.GetEnumDescription();
                                NType=General.NotificationType.LateCancelled.GetEnumValue();
                            }
                            else
                            {
                                MessageType=General.NotificationType.NoShow.GetEnumDescription();
                                NType=General.NotificationType.NoShow.GetEnumValue();
                            }

                            BizObjects.UserNotification objUserNotification = new BizObjects.UserNotification();
                            string message = string.Empty;
                            message = Convert.ToInt32(hdnReportCnt.Value) == 0 ? hdnStatus.Value == General.ServiceTypes.LateCancelled.GetEnumDescription() ? General.GetNotificationMessage("LateCancelledPaymentSendToClient") : General.GetNotificationMessage("LateReschedulePaymentSendToClient") : General.GetNotificationMessage("NoShowPaymentSendToClient");
                            message = message.Replace("{{Amount}}", txtNoShowAmount.Text);
                            objUserNotificationService = ServiceFactory.UserNotificationService;
                            objUserNotification.UserId = ClientId;
                            objUserNotification.UserTypeId = General.UserRoles.Client.GetEnumValue();
                            objUserNotification.Message = message;
                            objUserNotification.Status = General.NotificationStatus.UnRead.GetEnumDescription();
                            objUserNotification.CommonId = ServiceId;
                            objUserNotification.MessageType = MessageType;
                            objUserNotification.AddedDate = DateTime.UtcNow;

                            NotificationId = objUserNotificationService.AddUserNotification(ref objUserNotification);

                            dtBadgeCount.Clear();
                            objUserNotificationService.GetBadgeCount(ClientId, General.UserRoles.Client.GetEnumValue(), ref dtBadgeCount);
                            BadgeCount = dtBadgeCount.Rows.Count;

                            Notifications objNotifications = new Notifications { NId = NotificationId, NType = NType, CommonId = ServiceId };
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
                    }
                    Session["msg"] = "notifyPayment";
                    Response.Redirect(Application["SiteAddress"] + "admin/NoShowService_List.aspx", false);
                }
            }
            catch (Exception Ex)
            {
                dvMessage.InnerHtml = "<strong>Error! </strong>" + Ex.Message.ToString().Trim();
                dvMessage.Attributes.Add("class", "alert alert-error");
                dvMessage.Visible = true;
            }
        }

        protected void btnNotifyAndSchedule_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(Request.QueryString["ServiceId"]) && Session["LoginSession"] != null)
                {
                    if (Convert.ToInt32(hdnReportCnt.Value) > 0)
                    {
                        if (Convert.ToDateTime(hdnScheduleOn.Value) == Convert.ToDateTime(txtScheduleOn.Text))
                        {
                            dvMessage.InnerHtml = "<strong>Please Change Schedule Date for New Service.</strong>";
                            dvMessage.Attributes.Add("class", "alert alert-error");
                            dvMessage.Visible = true;
                            return;
                        }
                    }

                    LoginModel objLoginModel = new LoginModel();
                    objLoginModel = Session["LoginSession"] as LoginModel;

                    long ServiceId = Convert.ToInt64(Request.QueryString["ServiceId"]);
                    DataTable dtService = new DataTable();
                    DataTable dtClient = new DataTable();
                    DataTable dtBadgeCount = new DataTable();
                    objServicesService = ServiceFactory.ServicesService;
                    objClientService = ServiceFactory.ClientService;

                    BizObjects.ServiceNoShow objServiceNoShow = new BizObjects.ServiceNoShow();
                    objServiceNoShow.ServiceId = ServiceId;
                    objServiceNoShow.NoShowAmount = 0;
                    objServiceNoShow.IsPaymentReceived = false;
                    if (Convert.ToInt32(hdnReportCnt.Value) == 0)
                        objServiceNoShow.IsNoShow = false;
                    else
                        objServiceNoShow.IsNoShow = true;
                    objServiceNoShow.AddedBy = objLoginModel.Id;
                    objServiceNoShow.AddedByType = objLoginModel.RoleId;
                    objServiceNoShow.AddedDate = DateTime.UtcNow;
                    objServiceNoShowService = ServiceFactory.ServiceNoShowService;

                    objServiceNoShowService.AddServiceNoShow(ref objServiceNoShow);

                    objServicesService.GetServiceById(ServiceId, ref dtService);
                    if (dtService.Rows.Count > 0)
                    {
                        int ClientId = Convert.ToInt32(dtService.Rows[0]["ClientId"].ToString());
                        objClientService.GetClientById(ClientId, ref dtClient);
                        if (dtClient.Rows.Count > 0)
                        {

                            long NotificationId = 0;
                            int BadgeCount = 0;
                            BizObjects.UserNotification objUserNotification = new BizObjects.UserNotification();
                            string message = string.Empty;
                            message = Convert.ToInt32(hdnReportCnt.Value) == 0 ? General.GetNotificationMessage("LateRescheduleWithoutPaymentSendToClient") : General.GetNotificationMessage("NoShowWithoutPaymentSendToClient");

                            objUserNotificationService = ServiceFactory.UserNotificationService;
                            objUserNotification.UserId = ClientId;
                            objUserNotification.UserTypeId = General.UserRoles.Client.GetEnumValue();
                            objUserNotification.Message = message;
                            objUserNotification.Status = General.NotificationStatus.UnRead.GetEnumDescription();
                            objUserNotification.CommonId = ServiceId;
                            objUserNotification.MessageType = General.NotificationType.NoShow.GetEnumDescription();
                            objUserNotification.AddedDate = DateTime.UtcNow;

                            NotificationId = objUserNotificationService.AddUserNotification(ref objUserNotification);

                            dtBadgeCount.Clear();
                            objUserNotificationService.GetBadgeCount(ClientId, General.UserRoles.Client.GetEnumValue(), ref dtBadgeCount);
                            BadgeCount = dtBadgeCount.Rows.Count;

                            Notifications objNotifications = new Notifications { NId = NotificationId, NType = General.NotificationType.NoShow.GetEnumValue(), CommonId = ServiceId };
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

                        if (hdnStatus.Value!=General.ServiceTypes.LateCancelled.GetEnumDescription())
                        {
                            BizObjects.Services objServices = new BizObjects.Services();
                            objServices.Id = ServiceId;
                            objServices.UpdatedBy = objLoginModel.Id;
                            objServices.UpdatedByType = objLoginModel.RoleId;
                            objServices.UpdatedDate = DateTime.UtcNow;

                            objServicesService.UpdateServiceOfNoShow(ref objServices);
                        }
                        

                        //ClientId = Convert.ToInt32(dtService.Rows[0]["ClientId"].ToString());
                        //objClientService.GetClientById(ClientId, ref dtClient);
                        //if (dtClient.Rows.Count > 0)
                        //{

                        //    long NotificationId = 0;
                        //    int BadgeCount = 0;
                        //    BizObjects.UserNotification objUserNotification = new BizObjects.UserNotification();
                        //    string message = "Your Service is Scheduled.Please Approve or Reschedule.";
                        //    objUserNotificationService = ServiceFactory.UserNotificationService;
                        //    objUserNotification.UserId = ClientId;
                        //    objUserNotification.UserTypeId = General.UserRoles.Client.GetEnumValue();
                        //    objUserNotification.Message = message;
                        //    objUserNotification.Status = General.NotificationStatus.UnRead.GetEnumDescription();
                        //    objUserNotification.CommonId = ServiceId;
                        //    objUserNotification.MessageType = General.NotificationType.ServiceApproval.GetEnumDescription();
                        //    objUserNotification.AddedDate = DateTime.UtcNow;

                        //    NotificationId = objUserNotificationService.AddUserNotification(ref objUserNotification);

                        //    dtBadgeCount.Clear();
                        //    objUserNotificationService.GetBadgeCount(ClientId, General.UserRoles.Client.GetEnumValue(), ref dtBadgeCount);
                        //    BadgeCount = dtBadgeCount.Rows.Count;

                        //    Notifications objNotifications = new Notifications { NId = NotificationId, NType = General.NotificationType.ServiceApproval.GetEnumValue(), CommonId = ServiceId };
                        //    List<NotificationModel> notify = new List<NotificationModel>();
                        //    notify.Add(new NotificationModel { Key = "NId", Value = new object[] { objNotifications.NId } });
                        //    notify.Add(new NotificationModel { Key = "NType", Value = new object[] { objNotifications.NType } });
                        //    notify.Add(new NotificationModel { Key = "CommonId", Value = new object[] { objNotifications.CommonId } });

                        //    if (!string.IsNullOrEmpty(dtClient.Rows[0]["DeviceType"].ToString()) &&
                        //    !string.IsNullOrEmpty(dtClient.Rows[0]["DeviceToken"].ToString()) &&
                        //     dtClient.Rows[0]["DeviceToken"].ToString().ToLower() != "no device token")
                        //    {
                        //        if (dtClient.Rows[0]["DeviceType"].ToString().ToLower() == "android")
                        //        {
                        //            string CustomData = "&data.NId=" + objNotifications.NId + "&data.NType=" + objNotifications.NType + "&data.CommonId=" + objNotifications.CommonId;
                        //            SendNotifications.SendAndroidNotification(dtClient.Rows[0]["DeviceToken"].ToString(), message, CustomData, "client");
                        //        }
                        //        else if (dtClient.Rows[0]["DeviceType"].ToString().ToLower() == "iphone")
                        //        {
                        //            SendNotifications.SendIphoneNotification(BadgeCount, dtClient.Rows[0]["DeviceToken"].ToString(), message, notify, "client");
                        //        }
                        //    }
                        //}
                    }
                    if (hdnStatus.Value != General.ServiceTypes.LateCancelled.GetEnumDescription())
                    {
                        Session["msg"] = "schedule";
                        Response.Redirect(Application["SiteAddress"] + "admin/NoShowService_List.aspx", false);
                    }
                    else
                    {
                        Session["msg"] = "cancelled";
                        Response.Redirect(Application["SiteAddress"] + "admin/NoShowService_List.aspx", false);
                    }
                }
            }
            catch (Exception Ex)
            {
                dvMessage.InnerHtml = "<strong>Error! </strong>" + Ex.Message.ToString().Trim();
                dvMessage.Attributes.Add("class", "alert alert-error");
                dvMessage.Visible = true;
            }
        }

        protected void btnScheduleWithOutNotify_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(Request.QueryString["ServiceId"]) && Session["LoginSession"] != null)
                {
                    if (Convert.ToInt32(hdnReportCnt.Value) > 0)
                    {
                        if (Convert.ToDateTime(hdnScheduleOn.Value) == Convert.ToDateTime(txtScheduleOn.Text))
                        {
                            dvMessage.InnerHtml = "<strong>Please Change Schedule Date for New Service.</strong>";
                            dvMessage.Attributes.Add("class", "alert alert-error");
                            dvMessage.Visible = true;
                            return;
                        }
                    }

                    LoginModel objLoginModel = new LoginModel();
                    objLoginModel = Session["LoginSession"] as LoginModel;

                    long ServiceId = Convert.ToInt64(Request.QueryString["ServiceId"]);
                    DataTable dtService = new DataTable();
                    DataTable dtClient = new DataTable();
                    DataTable dtBadgeCount = new DataTable();
                    objServicesService = ServiceFactory.ServicesService;
                    objClientService = ServiceFactory.ClientService;

                    BizObjects.ServiceNoShow objServiceNoShow = new BizObjects.ServiceNoShow();
                    objServiceNoShow.ServiceId = ServiceId;
                    objServiceNoShow.NoShowAmount = 0;
                    objServiceNoShow.IsPaymentReceived = false;
                    if (Convert.ToInt32(hdnReportCnt.Value) == 0)
                        objServiceNoShow.IsNoShow = false;
                    else
                        objServiceNoShow.IsNoShow = true;
                    objServiceNoShow.AddedBy = objLoginModel.Id;
                    objServiceNoShow.AddedByType = objLoginModel.RoleId;
                    objServiceNoShow.AddedDate = DateTime.UtcNow;
                    objServiceNoShowService = ServiceFactory.ServiceNoShowService;

                    objServiceNoShowService.AddServiceNoShow(ref objServiceNoShow);

                    objServicesService.GetServiceById(ServiceId, ref dtService);
                    if (dtService.Rows.Count > 0)
                    {
                        int ClientId = Convert.ToInt32(dtService.Rows[0]["ClientId"].ToString());
                        objClientService.GetClientById(ClientId, ref dtClient);
                        
                        if (hdnStatus.Value != General.ServiceTypes.LateCancelled.GetEnumDescription())
                        {
                            BizObjects.Services objServices = new BizObjects.Services();
                            objServices.Id = ServiceId;
                            objServices.UpdatedBy = objLoginModel.Id;
                            objServices.UpdatedByType = objLoginModel.RoleId;
                            objServices.UpdatedDate = DateTime.UtcNow;
                            objServicesService.UpdateServiceOfNoShow(ref objServices);
                        }
                    }
                    if (hdnStatus.Value != General.ServiceTypes.LateCancelled.GetEnumDescription())
                    {
                        Session["msg"] = "schedule";
                        Response.Redirect(Application["SiteAddress"] + "admin/NoShowService_List.aspx", false);
                    }
                    else
                    {
                        Session["msg"] = "cancelled";
                        Response.Redirect(Application["SiteAddress"] + "admin/NoShowService_List.aspx", false);
                    }
                }
            }
            catch (Exception Ex)
            {
                dvMessage.InnerHtml = "<strong>Error! </strong>" + Ex.Message.ToString().Trim();
                dvMessage.Attributes.Add("class", "alert alert-error");
                dvMessage.Visible = true;
            }
        }
    }
}