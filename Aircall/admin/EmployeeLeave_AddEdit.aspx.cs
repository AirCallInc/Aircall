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
    public partial class EmployeeLeave_AddEdit : System.Web.UI.Page
    {
        IEmployeeService objEmployeeService;
        IEmployeeLeaveService objEmployeeLeaveService;
        IEmployeeScheduleService objEmployeeScheduleService;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["LeaveId"]))
                {
                    BindEmployeeLeaveById();
                }
            }
        }

        private void BindEmployeeLeaveById()
        {
            int LeaveId = Convert.ToInt32(Request.QueryString["LeaveId"].ToString());
            btnAdd.Text = "Update Leave";
            lnkEmpSearch.Visible = false;
            objEmployeeLeaveService = ServiceFactory.EmployeeLeaveService;
            objEmployeeService = ServiceFactory.EmployeeService;
            DataTable dtLeave = new DataTable();
            DataTable dtEmployee = new DataTable();
            objEmployeeLeaveService.GetEmployeeLeaveById(LeaveId, ref dtLeave);
            if (dtLeave.Rows.Count > 0)
            {
                txtEmployee.Text = dtLeave.Rows[0]["EmployeeName"].ToString();
                int EmployeeId = Convert.ToInt32(dtLeave.Rows[0]["EmployeeId"].ToString());
                objEmployeeService.GetEmployeeById(EmployeeId, ref dtEmployee);
                if (dtEmployee.Rows.Count > 0)
                {
                    rblEmployee.DataSource = dtEmployee;
                    rblEmployee.DataTextField = dtEmployee.Columns["EmployeeName"].ToString();
                    rblEmployee.DataValueField = dtEmployee.Columns["Id"].ToString();
                    rblEmployee.DataBind();
                    rblEmployee.SelectedValue = EmployeeId.ToString();
                }
                txtStart.Text = Convert.ToDateTime(dtLeave.Rows[0]["StartDate"].ToString()).ToString("MM/dd/yyyy");
                txtEnd.Text = Convert.ToDateTime(dtLeave.Rows[0]["EndDate"].ToString()).ToString("MM/dd/yyyy");
                chkAvailable.Checked = Convert.ToBoolean(dtLeave.Rows[0]["AvailableOnHoliday"].ToString());
                txtReason.Text = dtLeave.Rows[0]["Reason"].ToString();
            }
        }

        protected void lnkEmpSearch_Click(object sender, EventArgs e)
        {
            objEmployeeService = ServiceFactory.EmployeeService;
            DataTable dtEmployees = new DataTable();
            objEmployeeService.GetEmployeeByName(txtEmployee.Text.Trim(), true, ref dtEmployees);
            if (dtEmployees.Rows.Count > 0)
            {
                rblEmployee.DataSource = dtEmployees;
                rblEmployee.DataTextField = dtEmployees.Columns["EmployeeName"].ToString();
                rblEmployee.DataValueField = dtEmployees.Columns["Id"].ToString();
                rblEmployee.DataBind();
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                if (Session["LoginSession"] != null)
                {
                    try
                    {
                        LoginModel objLoginModel = new LoginModel();
                        objLoginModel = Session["LoginSession"] as LoginModel;

                        if (rblEmployee.Items.Count == 0 || rblEmployee.SelectedIndex == -1)
                        {
                            dvMessage.InnerHtml = "<strong>Please Select Employee.</strong>";
                            dvMessage.Attributes.Add("class", "alert alert-error");
                            dvMessage.Visible = true;
                            return;
                        }

                        DateTime StartDate = Convert.ToDateTime(txtStart.Text.Trim());
                        DateTime EndDate = Convert.ToDateTime(txtEnd.Text.Trim());
                        if (StartDate > EndDate)
                        {
                            dvMessage.InnerHtml = "<strong>To date should be greater than From date.</strong>";
                            dvMessage.Attributes.Add("class", "alert alert-error");
                            dvMessage.Visible = true;
                            return;
                        }

                        BizObjects.EmployeeLeave objEmployeeLeave = new BizObjects.EmployeeLeave();
                        objEmployeeLeaveService = ServiceFactory.EmployeeLeaveService;
                        objEmployeeScheduleService = ServiceFactory.EmployeeScheduleService;
                        int LeaveId = 0;
                        objEmployeeLeave.EmployeeId = Convert.ToInt32(rblEmployee.SelectedValue);
                        objEmployeeLeave.StartDate = Convert.ToDateTime(txtStart.Text.Trim());
                        objEmployeeLeave.EndDate = Convert.ToDateTime(txtEnd.Text.Trim());
                        objEmployeeLeave.Reason = txtReason.Text.Trim();
                        objEmployeeLeave.AvailableOnHoliday = chkAvailable.Checked;
                        objEmployeeLeave.AddedBy = objLoginModel.Id;
                        objEmployeeLeave.AddedDate = DateTime.UtcNow;

                        if (!string.IsNullOrEmpty(Request.QueryString["LeaveId"]))
                        {
                            LeaveId = Convert.ToInt32(Request.QueryString["LeaveId"].ToString());
                            objEmployeeLeave.Id = LeaveId;
                            objEmployeeLeave.UpdatedBy = objLoginModel.Id;
                            objEmployeeLeave.UpdatedDate = DateTime.UtcNow;
                            objEmployeeLeaveService.UpdateEmployeeLeave(ref objEmployeeLeave);

                            objEmployeeScheduleService.DeleteEmployeeScheduleByLeaveId(LeaveId);
                        }
                        else
                            LeaveId = objEmployeeLeaveService.AddEmployeeLeave(ref objEmployeeLeave);


                        if (LeaveId != 0)
                            objEmployeeScheduleService.AddEmployeeLeaveSchedule(Convert.ToInt32(rblEmployee.SelectedValue), StartDate, EndDate, LeaveId);

                        if (!string.IsNullOrEmpty(Request.QueryString["LeaveId"]))
                        {
                            Session["msg"] = "edit";
                            Response.Redirect(Application["SiteAddress"] + "admin/EmployeeLeave_List.aspx");
                        }
                        else
                        {
                            Session["msg"] = "add";
                            Response.Redirect(Application["SiteAddress"] + "admin/EmployeeLeave_List.aspx");
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

        protected void txtStart_TextChanged(object sender, EventArgs e)
        {
            DateTime StartDate = CheckDate(Convert.ToDateTime(txtStart.Text.Trim()));
            txtStart.Text = Convert.ToDateTime(StartDate.ToString()).ToString("MM/dd/yyyy");
        }

        protected DateTime CheckDate(DateTime dt)
        {
            DateTime NewDate;
            if (dt.DayOfWeek.ToString() == "Sunday")
                NewDate = dt.AddDays(1);
            else if (dt.DayOfWeek.ToString() == "Saturday")
                NewDate = dt.AddDays(2);
            else
                NewDate = dt;
            return NewDate;
        }
    }
}