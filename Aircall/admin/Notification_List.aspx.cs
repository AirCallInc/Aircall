using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Services;
using System.Data;

namespace Aircall.admin
{
    public partial class Notification_List : System.Web.UI.Page
    {
        INotificationMasterService objNotificationMasterService;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["msg"] != null)
                {
                    if (Session["msg"].ToString() == "edit")
                    {
                        dvMessage.InnerHtml = "<strong>Notification updated successfully.</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-success");
                        dvMessage.Visible = true;
                    }
                    Session["msg"] = null;
                }
                BindNotification();
            }
        }

        private void BindNotification()
        {
            DataTable dtNotification = new DataTable();
            objNotificationMasterService = ServiceFactory.NotificationMasterService;
            objNotificationMasterService.GetAllNotifications(ref dtNotification);
            if (dtNotification.Rows.Count > 0)
            {
                lstNotification.DataSource = dtNotification;
            }
            lstNotification.DataBind();
        }
    }
}