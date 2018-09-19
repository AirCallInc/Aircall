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
    public partial class ClientSendNotification : System.Web.UI.Page
    {
        IClientService objClientService;
        IUserNotificationService objUserNotificationService;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["msg"] == "sent")
                {
                    dvMessage.InnerHtml = "<strong>Notification Sent Successfully.</strong>";
                    dvMessage.Attributes.Add("class", "alert alert-success");
                    dvMessage.Visible = true;
                }
                BindClientList();
            }
        }

        private void BindClientList()
        {
            DataTable dtClients = new DataTable();
            objClientService = ServiceFactory.ClientService;
            objClientService.GetAllClients(true, ref dtClients);
            if (dtClients.Rows.Count > 0)
            {
                chkClients.DataSource = dtClients;
                chkClients.DataTextField = dtClients.Columns["ClientName"].ToString();
                chkClients.DataValueField = dtClients.Columns["Id"].ToString();
                chkClients.DataBind();
                //chkClients.Items.Insert(0, new ListItem("Select All", "0"));
            }
        }

        protected void lnkClientSearch_Click(object sender, EventArgs e)
        {
            chkAll.Checked = false;
            chkClients.DataSource = "";
            chkClients.DataBind();
            DataTable dtClients = new DataTable();
            objClientService = ServiceFactory.ClientService;
            objClientService.GetClientByName(txtClientName.Text.Trim(), ref dtClients);
            if (dtClients.Rows.Count>0)
            {
                chkClients.DataSource = dtClients;
                chkClients.DataTextField = dtClients.Columns["ClientName"].ToString();
                chkClients.DataValueField = dtClients.Columns["Id"].ToString();
                chkClients.DataBind();
                //chkClients.Items.Insert(0, new ListItem("Select All", "0"));
            }
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            if (chkClients.SelectedIndex == -1)
            {
                dvMessage.InnerHtml = "<strong>Please Select Client.</strong>";
                dvMessage.Attributes.Add("class", "alert alert-error");
                dvMessage.Visible = true;
                return;
            }

            bool IsAll = false;
            bool IsSent = false;
            DataTable dtClient= new DataTable();
            DataTable dtBadgeCount = new DataTable();
            objClientService = ServiceFactory.ClientService;
            objUserNotificationService = ServiceFactory.UserNotificationService;

            //if (chkClients.SelectedValue.ToString() == "0")
            //    IsAll = true;

            foreach (ListItem item in chkClients.Items)
            {
                if (item.Selected)
                {
                    objClientService.GetClientById(Convert.ToInt32(item.Value), ref dtClient);
                    if (dtClient.Rows.Count>0)
                    {
                        long NotificationId = 0;
                        int BadgeCount = 0;
                        int ClientId = Convert.ToInt32(dtClient.Rows[0]["Id"].ToString());
                        BizObjects.UserNotification objUserNotification = new BizObjects.UserNotification();
                        string message = txtMessage.Text.Trim();
                        objUserNotificationService = ServiceFactory.UserNotificationService;
                        objUserNotification.UserId = ClientId;
                        objUserNotification.UserTypeId = General.UserRoles.Client.GetEnumValue();
                        objUserNotification.Message = message;
                        objUserNotification.Status = General.NotificationStatus.UnRead.GetEnumDescription();
                        objUserNotification.MessageType = General.NotificationType.FriendlyReminder.GetEnumDescription();
                        objUserNotification.AddedDate = DateTime.UtcNow;

                        NotificationId = objUserNotificationService.AddUserNotification(ref objUserNotification);

                        dtBadgeCount.Clear();
                        objUserNotificationService.GetBadgeCount(ClientId, General.UserRoles.Client.GetEnumValue(), ref dtBadgeCount);
                        BadgeCount = dtBadgeCount.Rows.Count;
                        Notifications objNotifications = new Notifications { NId = NotificationId, NType = General.NotificationType.FriendlyReminder.GetEnumValue() };
                        List<NotificationModel> notify = new List<NotificationModel>();
                        notify.Add(new NotificationModel { Key = "NId", Value = new object[] { objNotifications.NId } });
                        notify.Add(new NotificationModel { Key = "NType", Value = new object[] { objNotifications.NType } });
                        
                        if (!string.IsNullOrEmpty(dtClient.Rows[0]["DeviceType"].ToString()) &&
                            !string.IsNullOrEmpty(dtClient.Rows[0]["DeviceToken"].ToString()) &&
                             dtClient.Rows[0]["DeviceToken"].ToString().ToLower() != "no device token")
                        {
                            if (dtClient.Rows[0]["DeviceType"].ToString().ToLower() == "android")
                            {
                                string CustomData = "&data.NId=" + objNotifications.NId + "&data.NType=" + objNotifications.NType;
                                SendNotifications.SendAndroidNotification(dtClient.Rows[0]["DeviceToken"].ToString(), message, CustomData, "client");
                            }
                            else if (dtClient.Rows[0]["DeviceType"].ToString().ToLower() == "iphone")
                            {
                                SendNotifications.SendIphoneNotification(BadgeCount, dtClient.Rows[0]["DeviceToken"].ToString(), message, notify, "client");
                            }
                        }
                        IsSent = true;
                    }
                }
            }
            if (IsSent)
            {
                Response.Redirect(Application["SiteAddress"] + "admin/ClientSendNotification.aspx?msg=sent");
                return;
            }
            Response.Redirect(Application["SiteAddress"] + "admin/ClientSendNotification.aspx");
        }

        protected void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < chkClients.Items.Count; i++)
            {
                chkClients.Items[i].Selected = chkAll.Checked;
            }
        }

        protected void chkClients_SelectedIndexChanged(object sender, EventArgs e)
        {
            chkAll.Checked = false;
        }
    }
}