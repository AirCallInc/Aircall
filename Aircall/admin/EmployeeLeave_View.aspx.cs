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
    public partial class EmployeeLeave_View : System.Web.UI.Page
    {
        IEmployeeLeaveService objEmployeeLeaveService;
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["LeaveId"]))
                {
                    int LeaveId = Convert.ToInt32(Request.QueryString["LeaveId"].ToString());
                    objEmployeeLeaveService = ServiceFactory.EmployeeLeaveService;
                    DataTable dtLeave = new DataTable();
                    objEmployeeLeaveService.GetEmployeeLeaveById(LeaveId, ref dtLeave);
                    if (dtLeave.Rows.Count>0)
                    {
                        ltrEmpName.Text = dtLeave.Rows[0]["EmployeeName"].ToString();
                        ltrStart.Text = Convert.ToDateTime(dtLeave.Rows[0]["StartDate"].ToString()).ToString("MM/dd/yyyy");
                        ltrEnd.Text = Convert.ToDateTime(dtLeave.Rows[0]["EndDate"].ToString()).ToString("MM/dd/yyyy");
                        ltrReason.Text = dtLeave.Rows[0]["Reason"].ToString();
                        ltrApprovedBy.Text = dtLeave.Rows[0]["UserName"].ToString();
                        ltrAddedDate.Text = Convert.ToDateTime(dtLeave.Rows[0]["AddedDate"].ToString()).ToString("MM/dd/yyyy");
                    }
                }
            }
        }
    }
}