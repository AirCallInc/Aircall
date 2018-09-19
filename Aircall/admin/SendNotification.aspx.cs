using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Services;
using System.Data;
using Aircall.Common;

namespace Aircall.admin
{
    public partial class SendNotification : System.Web.UI.Page
    {
        IAreasService objAreasService;
        IEmployeeWorkAreaService objEmployeeWorkAreaService;
        IEmployeeService objEmployeeService;
        IUserNotificationService objUserNotificationService;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void lnkAreaSearch_Click(object sender, EventArgs e)
        {
            chkAll.Checked = false;
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
            chkEmployee.DataSource = "";
            chkEmployee.DataBind();
        }

        protected void lnkEmpSearch_Click(object sender, EventArgs e)
        {
            //if (rblWorkArea.Items.Count == 0 || rblWorkArea.SelectedIndex == -1)
            //{
            //    dvMessage.InnerHtml = "<strong>Please Select Work Area</strong>";
            //    dvMessage.Attributes.Add("class", "alert alert-error");
            //    dvMessage.Visible = true;
            //    return;
            //}
            chkAll.Checked = false;
            objEmployeeWorkAreaService = ServiceFactory.EmployeeWorkAreaService;
            objEmployeeService = ServiceFactory.EmployeeService;
            DataTable dtEmployees = new DataTable();
            if (rblWorkArea.Items.Count != 0 || rblWorkArea.SelectedIndex != -1)
            {
                int AreaId = 0;
                if (rblWorkArea.SelectedIndex == -1)
                    AreaId = 0;
                else
                    AreaId = Convert.ToInt32(rblWorkArea.SelectedValue.ToString());
                objEmployeeWorkAreaService.GetAllEmployeeByAreaId(AreaId, txtEmployee.Text.Trim(), true, ref dtEmployees);
                if (dtEmployees.Rows.Count > 0)
                {
                    chkEmployee.DataSource = dtEmployees;
                    chkEmployee.DataTextField = dtEmployees.Columns["EmpName"].ToString();
                    chkEmployee.DataValueField = dtEmployees.Columns["EmployeeId"].ToString();
                    chkEmployee.DataBind();
                    //chkEmployee.Items.Insert(0, new ListItem("Select All", "0"));
                }
            }
            else
            {
                objEmployeeService.GetEmployeeByName(txtEmployee.Text.Trim(), true, ref dtEmployees);
                if (dtEmployees.Rows.Count > 0)
                {
                    chkEmployee.DataSource = dtEmployees;
                    chkEmployee.DataTextField = dtEmployees.Columns["EmployeeName"].ToString();
                    chkEmployee.DataValueField = dtEmployees.Columns["Id"].ToString();
                    chkEmployee.DataBind();
                    //chkEmployee.Items.Insert(0, new ListItem("Select All", "0"));
                }
            }
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            //if (rblWorkArea.Items.Count == 0 || rblWorkArea.SelectedIndex == -1)
            //{
            //    dvMessage.InnerHtml = "<strong>Please Select Work Area.</strong>";
            //    dvMessage.Attributes.Add("class", "alert alert-error");
            //    dvMessage.Visible = true;
            //    return;
            //}
            if (chkEmployee.SelectedIndex == -1)
            {
                dvMessage.InnerHtml = "<strong>Please Select Employee.</strong>";
                dvMessage.Attributes.Add("class", "alert alert-error");
                dvMessage.Visible = true;
                return;
            }
            bool IsAll = false;
            bool IsSent = false;
            objEmployeeService = ServiceFactory.EmployeeService;
            DataTable dtEmployee = new DataTable();
            DataTable dtBadgeCount = new DataTable();
            //if (chkEmployee.SelectedValue.ToString() == "0")
            //    IsAll = true;

            foreach (ListItem item in chkEmployee.Items)
            {
                if (item.Selected)
                {
                    objEmployeeService.GetEmployeeById(Convert.ToInt32(item.Value), ref dtEmployee);
                    if (dtEmployee.Rows.Count > 0)
                    {
                        long NotificationId = 0;
                        int BadgeCount = 0;
                        int EmployeeId = Convert.ToInt32(dtEmployee.Rows[0]["Id"].ToString());
                        BizObjects.UserNotification objUserNotification = new BizObjects.UserNotification();
                        string message = txtMessage.Text.Trim();
                        objUserNotificationService = ServiceFactory.UserNotificationService;
                        objUserNotification.UserId = EmployeeId;
                        objUserNotification.UserTypeId = General.UserRoles.Employee.GetEnumValue();
                        objUserNotification.Message = message;
                        objUserNotification.Status = General.NotificationStatus.UnRead.GetEnumDescription();
                        objUserNotification.MessageType = General.NotificationType.FriendlyReminder.GetEnumDescription();
                        objUserNotification.AddedDate = DateTime.UtcNow;

                        NotificationId = objUserNotificationService.AddUserNotification(ref objUserNotification);

                        dtBadgeCount.Clear();
                        objUserNotificationService.GetBadgeCount(EmployeeId, General.UserRoles.Employee.GetEnumValue(), ref dtBadgeCount);
                        BadgeCount = dtBadgeCount.Rows.Count;
                        Notifications objNotifications = new Notifications { NId = NotificationId, NType = General.NotificationType.FriendlyReminder.GetEnumValue() };
                        List<NotificationModel> notify = new List<NotificationModel>();
                        notify.Add(new NotificationModel { Key = "NId", Value = new object[] { objNotifications.NId } });
                        notify.Add(new NotificationModel { Key = "NType", Value = new object[] { objNotifications.NType } });

                        if (!string.IsNullOrEmpty(dtEmployee.Rows[0]["DeviceType"].ToString()) &&
                    !string.IsNullOrEmpty(dtEmployee.Rows[0]["DeviceToken"].ToString()) &&
                     dtEmployee.Rows[0]["DeviceToken"].ToString().ToLower() != "no device token")
                        {
                            SendNotifications.SendIphoneNotification(BadgeCount, dtEmployee.Rows[0]["DeviceToken"].ToString(), message, notify, "employee");
                            IsSent = true;
                        }
                    }
                }
            }
            if (IsSent)
            {
                dvMessage.InnerHtml = "<strong>Notification Sent Successfully.</strong>";
                dvMessage.Attributes.Add("class", "alert alert-success");
                dvMessage.Visible = true;
            }
            //clear controls
            txtWorkArea.Text = "";
            rblWorkArea.DataSource = "";
            rblWorkArea.DataBind();
            txtEmployee.Text = "";
            chkEmployee.DataSource = "";
            chkEmployee.DataBind();
            txtMessage.Text = "";
            //Response.Redirect(Application["SiteAddress"] + "admin/SendNotification.aspx");
        }

        protected void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < chkEmployee.Items.Count; i++)
            {
                chkEmployee.Items[i].Selected = chkAll.Checked;
            }
        }

        protected void chkEmployee_SelectedIndexChanged(object sender, EventArgs e)
        {
            chkAll.Checked = false;
        }
    }
}