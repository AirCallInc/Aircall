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
    public partial class Notification_Edit : System.Web.UI.Page
    {
        INotificationMasterService objNotificationMasterService;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["Id"]))
                {
                    BindNotificationById();
                }
            }
        }

        private void BindNotificationById()
        {
            int Id = Convert.ToInt32(Request.QueryString["Id"].ToString());
            DataTable dtNotification = new DataTable();
            objNotificationMasterService = ServiceFactory.NotificationMasterService;
            objNotificationMasterService.GetById(Id, ref dtNotification);
            if (dtNotification.Rows.Count > 0)
            {
                txtName.Text = dtNotification.Rows[0]["Name"].ToString();
                txtMsg.Text = dtNotification.Rows[0]["Message"].ToString();
                ltrTags.Text = dtNotification.Rows[0]["AvailableTags"].ToString();
                txtName.Enabled = false;
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["LoginSession"] != null)
                {
                    LoginModel objLoginModel = new LoginModel();
                    objLoginModel = Session["LoginSession"] as LoginModel;

                        if (txtMsg.Text.Trim().Length>1000)
                        {
                            dvMessage.InnerHtml = "<strong>Message length must not exceed to 1000 charactors.</strong>";
                            dvMessage.Attributes.Add("class", "alert alert-error");
                            dvMessage.Visible = true;
                            return;
                        }

                    BizObjects.NotificationMaster objNotificationMaster = new BizObjects.NotificationMaster();
                    if (!string.IsNullOrEmpty(Request.QueryString["Id"]))
                    {
                        int Id = Convert.ToInt32(Request.QueryString["Id"].ToString());
                        objNotificationMaster.Id = Id;
                        objNotificationMaster.Message = txtMsg.Text.Trim();
                        objNotificationMaster.UpdatedBy = objLoginModel.Id;
                        objNotificationMaster.UpdatedByType = objLoginModel.RoleId;
                        objNotificationMaster.UpdatedDate = DateTime.UtcNow;

                        objNotificationMasterService = ServiceFactory.NotificationMasterService;
                        objNotificationMasterService.UpdateNotification(ref objNotificationMaster);
                        Session["msg"] = "edit";
                        Response.Redirect(Application["SiteAddress"] + "admin/Notification_List.aspx");
                    }
                }
            }
            catch (Exception Ex)
            {
                dvMessage.InnerHtml = "<strong>Error ! </strong>" + Ex.Message.ToString().Trim();
                dvMessage.Attributes.Add("class", "alert alert-error");
                dvMessage.Visible = true;
            }
        }
    }
}