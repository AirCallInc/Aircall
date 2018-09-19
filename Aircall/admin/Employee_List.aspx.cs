using Aircall.Common;
using Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Aircall.admin
{
    public partial class Employee_List : System.Web.UI.Page
    {
        IEmployeeService objEmployeeService;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["msg"] != null)
                {
                    if (Session["msg"].ToString() == "edit")
                    {
                        dvMessage.InnerHtml = "<strong>Employee updated successfully.</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-success");
                        dvMessage.Visible = true;
                    }
                    else if (Session["msg"].ToString() == "add")
                    {
                        dvMessage.InnerHtml = "<strong>Employee added successfully.</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-success");
                        dvMessage.Visible = true;
                    }
                    Session["msg"] = null;
                }
                BindEmployees();
            }
        }

        private void BindEmployees()
        {
            string EmpName = string.Empty;
            if (!string.IsNullOrEmpty(Request.QueryString["EmpName"]))
            {
                EmpName = Request.QueryString["EmpName"].ToString();
                txtEmployee.Text = EmpName;
            }

            DataTable dtEmployees = new DataTable();
            objEmployeeService = ServiceFactory.EmployeeService;
            objEmployeeService.GetAllEmployeesByName(EmpName, ListViewSortExpression, ListViewSortDirection.ToString(), ref dtEmployees);
            if (dtEmployees.Rows.Count > 0)
            {
                lstEmployee.DataSource = dtEmployees;
            }
            lstEmployee.DataBind();
        }

        protected void Page_PreRender(object sender, System.EventArgs e)
        {
            lnkActive.Attributes.Add("onclick", "javascript:return checkActive('Are you sure want to activate selected record?','Please select atleast one record')");
            lnkInactive.Attributes.Add("onclick", "javascript:return checkInactive('Scheduled Service for selected employees will be moved to Pending. Are you sure want to inactivate selected record?','Please select atleast one record')");
            lnkDelete.Attributes.Add("onclick", "javascript:return checkDelete('Scheduled Service for selected employees will be moved to Pending. Are you sure want to delete selected record?','Please select atleast one record')");
        }

        protected void lnkActive_Click(object sender, EventArgs e)
        {
            bool Active = false;
            dvMessage.InnerHtml = "";
            dvMessage.Visible = false;
            objEmployeeService = ServiceFactory.EmployeeService;
            for (int i = 0; i <= lstEmployee.Items.Count - 1; i++)
            {
                HtmlInputCheckBox chkUsers = (HtmlInputCheckBox)lstEmployee.Items[i].FindControl("chkcheck");
                if (chkUsers.Checked)
                {
                    HiddenField hdnEmployeeId = (HiddenField)lstEmployee.Items[i].FindControl("hdnEmployeeId");
                    if (!string.IsNullOrEmpty(hdnEmployeeId.Value))
                    {
                        objEmployeeService.SetStatus(true, Convert.ToInt32(hdnEmployeeId.Value));
                        Active = true;
                    }
                }
            }
            if (Active)
            {
                dvMessage.InnerHtml = "<strong>Record updated successfully.</strong>";
                dvMessage.Attributes.Add("class", "alert alert-success");
                dvMessage.Visible = true;
            }
            BindEmployees();
        }

        protected void lnkInactive_Click(object sender, EventArgs e)
        {
            bool InActive = false;
            dvMessage.InnerHtml = "";
            dvMessage.Visible = false;
            objEmployeeService = ServiceFactory.EmployeeService;
            for (int i = 0; i <= lstEmployee.Items.Count - 1; i++)
            {
                HtmlInputCheckBox chkUsers = (HtmlInputCheckBox)lstEmployee.Items[i].FindControl("chkcheck");
                if (chkUsers.Checked)
                {
                    HiddenField hdnEmployeeId = (HiddenField)lstEmployee.Items[i].FindControl("hdnEmployeeId");
                    if (!string.IsNullOrEmpty(hdnEmployeeId.Value))
                    {
                        objEmployeeService.SetStatus(false, Convert.ToInt32(hdnEmployeeId.Value));
                        InActive = true;
                    }
                }
            }
            if (InActive)
            {
                dvMessage.InnerHtml = "<strong>Record updated successfully.</strong>";
                dvMessage.Attributes.Add("class", "alert alert-success");
                dvMessage.Visible = true;
            }
            BindEmployees();
        }

        protected void lnkDelete_Click(object sender, EventArgs e)
        {
            bool Delete = false;
            dvMessage.InnerHtml = "";
            dvMessage.Visible = false;

            LoginModel objLoginModel = new LoginModel();
            objLoginModel = Session["LoginSession"] as LoginModel;

            int UserId = objLoginModel.Id;
            int RoleId = objLoginModel.RoleId;

            BizObjects.Employee objEmployee = new BizObjects.Employee();
            objEmployeeService = ServiceFactory.EmployeeService;
            for (int i = 0; i <= lstEmployee.Items.Count - 1; i++)
            {
                HtmlInputCheckBox chkUsers = (HtmlInputCheckBox)lstEmployee.Items[i].FindControl("chkcheck");
                if (chkUsers.Checked)
                {
                    HiddenField hdnEmployeeId = (HiddenField)lstEmployee.Items[i].FindControl("hdnEmployeeId");
                    if (!string.IsNullOrEmpty(hdnEmployeeId.Value))
                    {
                        objEmployee.Id = Convert.ToInt32(hdnEmployeeId.Value);
                        objEmployee.DeletedBy = UserId;
                        objEmployee.DeletedByType = RoleId;
                        objEmployee.DeletedDate = DateTime.UtcNow;
                        objEmployeeService.DeleteEmployee(ref objEmployee);
                        Delete = true;
                    }
                }
            }
            if (Delete)
            {
                dvMessage.InnerHtml = "<strong>Record deleted successfully.</strong>";
                dvMessage.Attributes.Add("class", "alert alert-success");
                dvMessage.Visible = true;
            }
            BindEmployees();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string Param = string.Empty;

            if (!string.IsNullOrEmpty(txtEmployee.Text.Trim()))
                Param = "?EmpName=" + txtEmployee.Text.Trim();
            Response.Redirect(Application["SiteAddress"] + "admin/Employee_List.aspx" + Param);
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
                    ViewState["SortExpression"] = "";
                return (string)ViewState["SortExpression"];
            }
            set { ViewState["SortExpression"] = value; }
        }

        protected void dataPagerWaitingService_PreRender(object sender, EventArgs e)
        {
            dataPagerWaitingService.PageSize = Convert.ToInt32(Application["PageSize"].ToString());
            BindEmployees();
        }

        protected void SortByServiceCase_Click(object sender, EventArgs e)
        {

        }

        protected void lstEmployee_Sorting(object sender, ListViewSortEventArgs e)
        {
            LinkButton lb = lstEmployee.FindControl(e.SortExpression) as LinkButton;
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
    }
}