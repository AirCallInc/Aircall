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

namespace Aircall.admin
{
    public partial class ScheduledService_List : System.Web.UI.Page
    {
        IServicesService objServicesService;
        IUserNotificationService objUserNotificationService;

        protected void Page_Load(object sender, EventArgs e)
        {
            dataPagerScheduledService.PageSize = Convert.ToInt32(Application["PageSize"].ToString());
            if (!IsPostBack)
            {
                FillPurposeOfVisitDropdown();
                if (Request.QueryString["msg"] == "edit")
                {
                    dvMessage.InnerHtml = "<strong>Client Service has been updated successfully.</strong>";
                    dvMessage.Attributes.Add("class", "alert alert-success");
                    dvMessage.Visible = true;
                }
                else if (Request.QueryString["msg"] == "add")
                {
                    dvMessage.InnerHtml = "<strong>Client Service has been added successfully.</strong>";
                    dvMessage.Attributes.Add("class", "alert alert-success");
                    dvMessage.Visible = true;
                }
                else if (Request.QueryString["msg"] == "reschedule")
                {
                    dvMessage.InnerHtml = "<strong>Client Service has been rescheduled successfully.</strong>";
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
                    dvMessage.InnerHtml = "<strong>Some of the parts are not in stock.</strong>";
                    dvMessage.Attributes.Add("class", "alert alert-error");
                    dvMessage.Visible = true;
                }
                else if (Request.QueryString["msg"] == "-7")
                {
                    dvMessage.InnerHtml = "<strong>Parts Not Found.</strong>";
                    dvMessage.Attributes.Add("class", "alert alert-error");
                    dvMessage.Visible = true;
                }
                BindServices();
            }
        }

        protected void Page_PreRender(object sender, System.EventArgs e)
        {
            lnkDelete.Attributes.Add("onclick", "javascript:return checkDelete('Are you sure want to delete selected Services?','Please select atleast one Service')");
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
            drpRequestType.DataSource = data;
            drpRequestType.DataBind();
            drpRequestType.Items.Insert(0, new ListItem("Select Request Type", "0"));
        }

        private void BindServices()
        {
            DataTable dtServices = new DataTable();
            string ServiceCaseNumber = string.Empty;
            string EmpName = string.Empty;
            string ClientName = string.Empty;
            string WorkArea = string.Empty;
            string StartDate = string.Empty;
            string EndDate = string.Empty;
            string Req = string.Empty;

            if (!string.IsNullOrEmpty(Request.QueryString["SNo"]))
            {
                ServiceCaseNumber = Request.QueryString["SNo"].ToString();
                txtCaseNo.Text = ServiceCaseNumber;
            }
            if (!string.IsNullOrEmpty(Request.QueryString["EmpName"]))
            {
                EmpName = Request.QueryString["EmpName"].ToString();
                txtEmployee.Text = EmpName;
            }
            if (!string.IsNullOrEmpty(Request.QueryString["ClientName"]))
            {
                ClientName = Request.QueryString["ClientName"].ToString();
                txtClient.Text = ClientName;
            }
            if (!string.IsNullOrEmpty(Request.QueryString["WorkArea"]))
            {
                WorkArea = Request.QueryString["WorkArea"].ToString();
                txtWorkArea.Text = WorkArea;
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
            if (!string.IsNullOrEmpty(Request.QueryString["Req"]))
            {
                Req = Request.QueryString["Req"].ToString();
                drpRequestType.SelectedValue = Req;
            }
            objServicesService = ServiceFactory.ServicesService;
            string Status = General.ServiceTypes.Scheduled.GetEnumDescription();
            objServicesService.GetServicesByStatus(Status, ServiceCaseNumber, EmpName, ClientName, WorkArea, StartDate, EndDate, ListViewSortExpression, ListViewSortDirection.ToString(), Req, ref dtServices);
            if (dtServices.Rows.Count > 0)
            {
                lstScheduleServices.DataSource = dtServices;
            }
            lstScheduleServices.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string Param = string.Empty;

            if (!string.IsNullOrEmpty(txtEmployee.Text.Trim()))
                Param = "?EmpName=" + txtEmployee.Text.Trim();
            if (!string.IsNullOrEmpty(txtClient.Text.Trim()))
            {
                if (!string.IsNullOrEmpty(Param))
                    Param = Param + "&ClientName=" + txtClient.Text.Trim();
                else
                    Param = "?ClientName=" + txtClient.Text.Trim();
            }
            if (!string.IsNullOrEmpty(txtCaseNo.Text.Trim()))
            {
                if (!string.IsNullOrEmpty(Param))
                    Param = Param + "&SNo=" + txtCaseNo.Text.Trim();
                else
                    Param = "?SNo=" + txtCaseNo.Text.Trim();
            }
            if (!string.IsNullOrEmpty(txtWorkArea.Text.Trim()))
            {
                if (!string.IsNullOrEmpty(Param))
                    Param = Param + "&WorkArea=" + txtWorkArea.Text.Trim();
                else
                    Param = "?WorkArea=" + txtWorkArea.Text.Trim();
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
            if (drpRequestType.SelectedValue!="0")
            {
                if (!string.IsNullOrEmpty(Param))
                    Param = Param + "&Req=" + drpRequestType.SelectedValue.ToString().Trim();
                else
                    Param = "?Req=" + drpRequestType.SelectedValue.ToString().Trim();
            }
            Response.Redirect(Application["SiteAddress"] + "admin/ScheduledService_List.aspx" + Param);
        }

        protected void dataPagerScheduledService_PreRender(object sender, EventArgs e)
        {
            dataPagerScheduledService.PageSize = Convert.ToInt32(Application["PageSize"].ToString());
            BindServices();
        }

        protected void lstScheduleServices_ItemCommand(object sender, ListViewCommandEventArgs e)
        {

        }

        protected SortDirection ListViewSortDirection
        {
            get
            {
                if (ViewState["sortDirection"] == null)
                    ViewState["sortDirection"] = SortDirection.Ascending;
                return (SortDirection)ViewState["sortDirection"];
            }
            set { ViewState["sortDirection"] = value; }
        }

        protected string ListViewSortExpression
        {
            get
            {
                if (ViewState["SortExpression"] == null)
                    ViewState["SortExpression"] = "Id";
                return (string)ViewState["SortExpression"];
            }
            set { ViewState["SortExpression"] = value; }
        }

        protected void lstScheduleServices_Sorting(object sender, ListViewSortEventArgs e)
        {
            LinkButton lb = lstScheduleServices.FindControl(e.SortExpression) as LinkButton;
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

        protected void lstScheduleServices_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType==ListViewItemType.DataItem)
            {
                DataRow row = (e.Item.DataItem as DataRowView).Row;

                HtmlInputCheckBox chkcheck = e.Item.FindControl("chkcheck") as HtmlInputCheckBox;

                if (Convert.ToBoolean(row["IsRequestedService"].ToString())==false)
                {
                    chkcheck.Visible = false;
                }
            }
        }

        protected void lnkDelete_Click(object sender, EventArgs e)
        {
            try
            {
                dvMessage.InnerHtml = "";
                dvMessage.Visible = false;

                LoginModel objLoginModel = new LoginModel();
                objLoginModel = Session["LoginSession"] as LoginModel;
                objServicesService = ServiceFactory.ServicesService;

                int UserId = objLoginModel.Id;
                int RoleId = objLoginModel.RoleId;
                DataTable dtServices = new DataTable();
                for (int i = 0; i <= lstScheduleServices.Items.Count - 1; i++)
                {
                    HtmlInputCheckBox chkService = (HtmlInputCheckBox)lstScheduleServices.Items[i].FindControl("chkcheck");
                    if (chkService.Checked)
                    {
                        long ServiceId=0;
                        HiddenField hdnServiceId = (HiddenField)lstScheduleServices.Items[i].FindControl("hdnServiceId");
                        if (!string.IsNullOrEmpty(hdnServiceId.Value))
                        {
                            ServiceId=Convert.ToInt64(hdnServiceId.Value);
                            objServicesService.DeleteService(ServiceId, UserId, RoleId, DateTime.UtcNow, ref dtServices);
                            if (dtServices.Rows.Count>0)
                            {
                                int EmployeeId = Convert.ToInt32(dtServices.Rows[0]["EmployeeId"].ToString());
                                int ClientId = Convert.ToInt32(dtServices.Rows[0]["ClientId"].ToString());
                                string ClientName = dtServices.Rows[0]["ClientName"].ToString();
                                string ScheduleDate = dtServices.Rows[0]["ScheduleDate"].ToString();
                                string ClientDeviceType = dtServices.Rows[0]["ClientDeviceType"].ToString();
                                string ClientDeviceToken = dtServices.Rows[0]["ClientDeviceToken"].ToString();
                                string EmployeeDeviceType = dtServices.Rows[0]["EmployeeDeviceType"].ToString();
                                string EmployeeDeviceToken = dtServices.Rows[0]["EmployeeDeviceToken"].ToString();
                                string Address = dtServices.Rows[0]["ClientAddress"].ToString();
                                string message = string.Empty;
                                
                                //Send Notification to Client
                                message = General.GetNotificationMessage("DeleteServiceSendToClient");
                                message = message.Replace("{{Address}}", Address);
                                NotifyUser(ClientId, "client", General.NotificationType.FriendlyReminder.GetEnumDescription(), General.NotificationType.FriendlyReminder.GetEnumValue(),
                                    ServiceId, message, ClientDeviceType, ClientDeviceToken);

                                message = string.Empty;
                                //Send Notification to Employee
                                if (EmployeeId!=0 && !string.IsNullOrEmpty(ScheduleDate))
                                {
                                    message = General.GetNotificationMessage("DeleteServiceSendToEmployee");
                                    message = message.Replace("{{ClientName}}", ClientName);
                                    message = message.Replace("{{ScheduleDate}}", Convert.ToDateTime(ScheduleDate).ToString("MMMM dd, yyyy"));
                                    NotifyUser(EmployeeId, "employee", General.NotificationType.FriendlyReminder.GetEnumDescription(), General.NotificationType.FriendlyReminder.GetEnumValue(),
                                        ServiceId, message, EmployeeDeviceType, EmployeeDeviceToken);
                                }
                            }
                            dtServices.Rows.Clear();
                        }
                    }
                }
                dvMessage.InnerHtml = "<strong>Requested Service Deleted successfully.</strong>";
                dvMessage.Attributes.Add("class", "alert alert-success");
                dvMessage.Visible = true;
                BindServices();
            }
            catch (Exception Ex)
            {
                dvMessage.InnerHtml = "<strong>Error!</strong>" + Ex.Message.ToString().Trim();
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
    }
}