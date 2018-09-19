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
    public partial class SalesRequest_Detail : System.Web.UI.Page
    {
        ISalesVisitRequestService objSalesVisitRequestService;
        IEmployeeService objEmployeeService;
        IUserNotificationService objUserNotificationService;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["RequestId"]))
                {
                    BindSalesRequestInfo();
                }
            }
        }

        private void BindSalesRequestInfo()
        {
            int RequestId = Convert.ToInt32(Request.QueryString["RequestId"].ToString());
            objSalesVisitRequestService = ServiceFactory.SalesVisitRequestService;
            objEmployeeService = ServiceFactory.EmployeeService;
            DataTable dtSalesReq = new DataTable();
            objSalesVisitRequestService.GetSalesRequestById(RequestId, ref dtSalesReq);
            if (dtSalesReq.Rows.Count > 0)
            {
                ltrSubmittedBy.Text = dtSalesReq.Rows[0]["RequestedEmpName"].ToString();
                ltrClient.Text = dtSalesReq.Rows[0]["ClientName"].ToString();
                ltrAddress.Text = dtSalesReq.Rows[0]["ClientAddress"].ToString();
                ltrSubmittedOn.Text = dtSalesReq.Rows[0]["AddedDate"].ToString();
                txtNotes.Text = dtSalesReq.Rows[0]["Notes"].ToString();
                txtNotes.Enabled = false;
                if (!string.IsNullOrEmpty(dtSalesReq.Rows[0]["EmployeeId"].ToString()))
                {
                    int EmpId = Convert.ToInt32(dtSalesReq.Rows[0]["EmployeeId"].ToString());
                    DataTable dtEmployee = new DataTable();
                    objEmployeeService.GetEmployeeById(EmpId, ref dtEmployee);
                    if (dtEmployee.Rows.Count > 0)
                    {
                        txtSalesEmployee.Text = dtEmployee.Rows[0]["EmployeeName"].ToString();
                        rblEmployee.DataSource = dtEmployee;
                        rblEmployee.DataTextField = dtEmployee.Columns["EmployeeName"].ToString();
                        rblEmployee.DataValueField = dtEmployee.Columns["Id"].ToString();
                        rblEmployee.DataBind();
                        rblEmployee.SelectedValue = EmpId.ToString();
                        rblEmployee.Enabled = false;
                        btnNotify.Visible = false;
                    }
                    else
                    {
                        rblEmployee.DataSource = "";
                        rblEmployee.DataBind();
                    }
                }
            }
        }

        protected void lnkSalesEmpSearch_Click(object sender, EventArgs e)
        {
            rblEmployee.DataSource = "";
            rblEmployee.DataBind();
            DataTable dtEmployee = new DataTable();
            objEmployeeService = ServiceFactory.EmployeeService;
            objEmployeeService.GetAllSalesEmployeeByName(txtSalesEmployee.Text.Trim(), ref dtEmployee);
            if (dtEmployee.Rows.Count > 0)
            {
                rblEmployee.DataSource = dtEmployee;
                rblEmployee.DataTextField = dtEmployee.Columns["EmployeeName"].ToString();
                rblEmployee.DataValueField = dtEmployee.Columns["Id"].ToString();
                rblEmployee.DataBind();
            }
        }

        protected void btnNotify_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    if (!string.IsNullOrEmpty(Request.QueryString["RequestId"]) ||
                                Session["LoginSession"] != null)
                    {
                        if (rblEmployee.Items.Count == 0 || rblEmployee.SelectedIndex == -1)
                        {
                            dvMessage.InnerHtml = "<strong>Please Select Employee.</strong>";
                            dvMessage.Attributes.Add("class", "alert alert-error");
                            dvMessage.Visible = true;
                            return;
                        }

                        LoginModel objLoginModel = new LoginModel();
                        objLoginModel = Session["LoginSession"] as LoginModel;

                        int RequestId = Convert.ToInt32(Request.QueryString["RequestId"].ToString());
                        BizObjects.SalesVisitRequest objSalesVisitRequest = new BizObjects.SalesVisitRequest();
                        objSalesVisitRequestService = ServiceFactory.SalesVisitRequestService;
                        objEmployeeService = ServiceFactory.EmployeeService;
                        objSalesVisitRequest.Id = RequestId;
                        objSalesVisitRequest.EmployeeId = Convert.ToInt32(rblEmployee.SelectedValue);
                        objSalesVisitRequest.RepliedDate = DateTime.UtcNow;
                        objSalesVisitRequest.AssignedBy = objLoginModel.Id;
                        objSalesVisitRequest.AssignedByType = objLoginModel.RoleId;

                        objSalesVisitRequestService.AssignSalesEmployee(ref objSalesVisitRequest);

                        DataTable dtEmployee = new DataTable();
                        DataTable dtBadgeCount = new DataTable();
                        objEmployeeService.GetEmployeeById(Convert.ToInt32(rblEmployee.SelectedValue), ref dtEmployee);
                        if (dtEmployee.Rows.Count > 0)
                        {

                            long NotificationId = 0;
                            int BadgeCount = 0;
                            int EmployeeId = Convert.ToInt32(rblEmployee.SelectedValue);
                            BizObjects.UserNotification objUserNotification = new BizObjects.UserNotification();
                            string message = General.GetNotificationMessage("SalesVistSendToEmployee"); //"System has scheduled a sales visit service for you.";
                            objUserNotificationService = ServiceFactory.UserNotificationService;
                            objUserNotification.UserId = EmployeeId;
                            objUserNotification.UserTypeId = General.UserRoles.Employee.GetEnumValue();
                            objUserNotification.Message = message;
                            objUserNotification.CommonId = RequestId;
                            objUserNotification.Status = General.NotificationStatus.UnRead.GetEnumDescription();
                            objUserNotification.MessageType = General.NotificationType.SalesPersonVisit.GetEnumDescription();
                            objUserNotification.AddedDate = DateTime.UtcNow;

                            NotificationId = objUserNotificationService.AddUserNotification(ref objUserNotification);

                            objUserNotificationService.GetBadgeCount(EmployeeId, General.UserRoles.Employee.GetEnumValue(), ref dtBadgeCount);
                            BadgeCount = dtBadgeCount.Rows.Count;

                            Notifications objNotifications = new Notifications { NId = NotificationId, NType = General.NotificationType.SalesPersonVisit.GetEnumValue(), CommonId = RequestId };
                            List<NotificationModel> notify = new List<NotificationModel>();
                            notify.Add(new NotificationModel { Key = "NId", Value = new object[] { objNotifications.NId } });
                            notify.Add(new NotificationModel { Key = "NType", Value = new object[] { objNotifications.NType } });
                            notify.Add(new NotificationModel { Key = "CommonId", Value = new object[] { objNotifications.CommonId } });
                            
                            if (!string.IsNullOrEmpty(dtEmployee.Rows[0]["DeviceType"].ToString()) &&
                                    !string.IsNullOrEmpty(dtEmployee.Rows[0]["DeviceToken"].ToString()) &&
                                     dtEmployee.Rows[0]["DeviceToken"].ToString().ToLower() != "no device token")
                            {
                                if (dtEmployee.Rows[0]["DeviceType"].ToString().ToLower() == "iphone")
                                {
                                    SendNotifications.SendIphoneNotification(BadgeCount, dtEmployee.Rows[0]["DeviceToken"].ToString(), message, notify, "employee");
                                }
                            }
                        }
                        Session["msg"] = "edit";
                        Response.Redirect(Application["SiteAddress"] + "admin/SalesRequest_List.aspx");
                    }
                }
                catch (Exception Ex)
                {
                    dvMessage.InnerHtml = "<strong>Error!</strong> " + Ex.Message.ToString().Trim();
                    dvMessage.Attributes.Add("class", "alert alert-error");
                    dvMessage.Visible = true;
                }
            }
        }
    }
}