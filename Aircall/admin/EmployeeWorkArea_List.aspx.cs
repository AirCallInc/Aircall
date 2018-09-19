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
    public partial class EmployeeWorkArea_List : System.Web.UI.Page
    {
        IEmployeeWorkAreaService objEmployeeWorkAreaService;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["msg"] != null)
                {
                    if (Session["msg"].ToString() == "edit")
                    {
                        dvMessage.InnerHtml = "<strong>Employee Work Area updated successfully.</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-success");
                        dvMessage.Visible = true;
                    }
                    else if (Session["msg"].ToString() == "add")
                    {
                        dvMessage.InnerHtml = "<strong>Employee Work Area added successfully.</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-success");
                        dvMessage.Visible = true;
                    }
                    Session["msg"] = null;
                }
                BindEmployeeWorkAreas();
            }
        }

        private void BindEmployeeWorkAreas()
        {
            DataTable dtEmployee = new DataTable();
            string EmployeeName = string.Empty;
            string AreaName = string.Empty;

            if (!string.IsNullOrEmpty(Request.QueryString["EmpName"]))
            {
                EmployeeName = Request.QueryString["EmpName"].ToString();
                txtEmployee.Text = EmployeeName;
            }
            if (!string.IsNullOrEmpty(Request.QueryString["AreaName"]))
            {
                AreaName = Request.QueryString["AreaName"].ToString();
                txtArea.Text = AreaName;
            }
            objEmployeeWorkAreaService = ServiceFactory.EmployeeWorkAreaService;
            objEmployeeWorkAreaService.Get1stPriorityGroupEmployee(EmployeeName, AreaName, ref dtEmployee);
            if (dtEmployee.Rows.Count > 0)
            {
                lstEmpWorkArea.DataSource = dtEmployee;
            }
            lstEmpWorkArea.DataBind();
        }

        protected void Page_PreRender(object sender, System.EventArgs e)
        {
            lnkDelete.Attributes.Add("onclick", "javascript:return checkDelete('Scheduled Service for selected employees will be moved to Pending. Are you sure want to delete selected record?','Please select atleast one record')");
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string Param = string.Empty;

            if (!string.IsNullOrEmpty(txtEmployee.Text.Trim()))
                Param = "?EmpName=" + txtEmployee.Text.Trim();
            if (!string.IsNullOrEmpty(txtArea.Text.Trim()))
            {
                if (!string.IsNullOrEmpty(Param))
                    Param = Param + "&AreaName=" + txtArea.Text.Trim();
                else
                    Param = "?AreaName=" + txtArea.Text.Trim();
            }
            Response.Redirect(Application["SiteAddress"] + "admin/EmployeeWorkArea_List.aspx" + Param);
        }

        protected void dataPagerEmpArea_PreRender(object sender, EventArgs e)
        {
            dataPagerEmpArea.PageSize = Convert.ToInt32(Application["PageSize"].ToString());
            BindEmployeeWorkAreas();
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

            objEmployeeWorkAreaService = ServiceFactory.EmployeeWorkAreaService;
            for (int i = 0; i <= lstEmpWorkArea.Items.Count - 1; i++)
            {
                HtmlInputCheckBox chkUsers = (HtmlInputCheckBox)lstEmpWorkArea.Items[i].FindControl("chkcheck");
                if (chkUsers.Checked)
                {
                    HiddenField hdnWorkAreaId = (HiddenField)lstEmpWorkArea.Items[i].FindControl("hdnWorkAreaId");
                    if (!string.IsNullOrEmpty(hdnWorkAreaId.Value))
                    {
                        objEmployeeWorkAreaService.DeleteEmployeeWorkArea(Convert.ToInt32(hdnWorkAreaId.Value),UserId,RoleId);
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
            BindEmployeeWorkAreas();
        }
    }
}