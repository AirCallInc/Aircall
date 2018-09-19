using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Services;
using Aircall.Common;
using System.Data;
using System.Web.UI.HtmlControls;

namespace Aircall.admin
{
    public partial class NoShowService_List : System.Web.UI.Page
    {
        IServicesService objServicesService;
        IUserNotificationService objUserNotificationService;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["msg"] != null)
                {
                    if (Session["msg"].ToString() == "notifyPayment")
                    {
                        dvMessage.InnerHtml = "<strong>No Show payment notification sent to client successfully.</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-success");
                        dvMessage.Visible = true;
                    }
                    else if (Session["msg"].ToString() == "schedule")
                    {
                        dvMessage.InnerHtml = "<strong>No Show without payment service scheduled.</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-success");
                        dvMessage.Visible = true;
                    }
                    else if (Session["msg"].ToString() == "cancelled")
                    {
                        dvMessage.InnerHtml = "<strong>Service cancelled successfully.</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-success");
                        dvMessage.Visible = true;
                    }
                    Session["msg"] = null;
                }
                BindNoShowServices();
            }
        }

        private void BindNoShowServices()
        {
            DataTable dtServices = new DataTable();
            string ServiceCaseNo = string.Empty;
            string EmpName = string.Empty;
            string ClientName = string.Empty;
            string WorkArea = string.Empty;
            string StartDate = string.Empty;
            string EndDate = string.Empty;

            if (!string.IsNullOrEmpty(Request.QueryString["EmpName"]))
            {
                EmpName = Request.QueryString["EmpName"].ToString();
                txtEmployee.Text = EmpName;
            }
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

            objServicesService = ServiceFactory.ServicesService;
            string Status = General.ServiceTypes.NoShow.GetEnumDescription();
            objServicesService.GetNoShowServicesByStatus(Status, ServiceCaseNo, EmpName, ClientName, WorkArea, StartDate, EndDate, ListViewSortExpression, ListViewSortDirection.ToString(), "", ref dtServices);
            if (dtServices.Rows.Count > 0)
            {
                lstNoShowServices.DataSource = dtServices;
            }
            lstNoShowServices.DataBind();
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

            Response.Redirect(Application["SiteAddress"] + "admin/NoShowService_List.aspx" + Param);
        }
        protected void dataPagerNoShowService_PreRender(object sender, EventArgs e)
        {
            BindNoShowServices();
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
                    ViewState["SortExpression"] = "IsNotify";
                return (string)ViewState["SortExpression"];
            }
            set { ViewState["SortExpression"] = value; }
        }
        protected void lstNoShowServices_Sorting(object sender, ListViewSortEventArgs e)
        {
            LinkButton lb = lstNoShowServices.FindControl(e.SortExpression) as LinkButton;
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

        protected void ServiceCase_Click(object sender, EventArgs e)
        {

        }

        protected void lstNoShowServices_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                DataRow row = (e.Item.DataItem as DataRowView).Row;

                HtmlInputCheckBox chkcheck = e.Item.FindControl("chkcheck") as HtmlInputCheckBox;
                HtmlAnchor lnkEdit = e.Item.FindControl("lnkEdit") as HtmlAnchor;

                if (Convert.ToBoolean(row["IsRequestedService"].ToString()) == false)
                {
                    chkcheck.Visible = false;
                }
                if (row["IsNotify"].ToString() != "0")
                {
                    chkcheck.Visible = false;
                    lnkEdit.Visible = false;
                }
                lnkEdit.HRef = Application["SiteAddress"] + "admin/NoShowService_Edit.aspx?ServiceId=" + row["Id"].ToString();
            }
        }

        protected void Page_PreRender(object sender, System.EventArgs e)
        {
            lnkDelete.Attributes.Add("onclick", "javascript:return checkDelete('Are you sure want to delete selected Services?','Please select atleast one Service')");
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
                for (int i = 0; i <= lstNoShowServices.Items.Count - 1; i++)
                {
                    HtmlInputCheckBox chkService = (HtmlInputCheckBox)lstNoShowServices.Items[i].FindControl("chkcheck");
                    if (chkService.Checked)
                    {
                        long ServiceId = 0;
                        HiddenField hdnServiceId = (HiddenField)lstNoShowServices.Items[i].FindControl("hdnServiceId");
                        if (!string.IsNullOrEmpty(hdnServiceId.Value))
                        {
                            ServiceId = Convert.ToInt64(hdnServiceId.Value);
                            objServicesService.DeleteService(ServiceId, UserId, RoleId, DateTime.UtcNow, ref dtServices);
                            if (dtServices.Rows.Count > 0)
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
                                if (EmployeeId != 0 && !string.IsNullOrEmpty(ScheduleDate))
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
                BindNoShowServices();
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