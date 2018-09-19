using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Services;
using System.Data;
using System.Web.UI.HtmlControls;
using Aircall.Common;

namespace Aircall.admin
{
    public partial class EmployeeLeave_List : System.Web.UI.Page
    {
        IEmployeeLeaveService objEmployeeLeaveService;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["msg"] != null)
                {
                    if (Session["msg"].ToString() == "edit")
                    {
                        dvMessage.InnerHtml = "<strong>Employee Leave updated successfully.</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-success");
                        dvMessage.Visible = true;
                    }
                    else if (Session["msg"].ToString() == "add")
                    {
                        dvMessage.InnerHtml = "<strong>Employee Leave added successfully.</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-success");
                        dvMessage.Visible = true;
                    } 
                    Session["msg"] = null;
                }
                BindEmployeeLeave();
            }
        }

        protected void Page_PreRender(object sender, System.EventArgs e)
        {
            lnkDelete.Attributes.Add("onclick", "javascript:return checkDelete('Are you sure want to delete selected record?','Please select atleast one record')");
        }

        private void BindEmployeeLeave()
        {
            objEmployeeLeaveService = ServiceFactory.EmployeeLeaveService;
            DataTable dtEmpLeave = new DataTable();
            string EmpName = string.Empty;
            string StartDate = string.Empty;
            string EndDate = string.Empty;

            if (!string.IsNullOrEmpty(Request.QueryString["EmpName"]))
            {
                EmpName = Request.QueryString["EmpName"].ToString();
                txtEmployee.Text = EmpName;
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
            objEmployeeLeaveService.GetAllEmployeeLeave(EmpName, StartDate, EndDate, ref dtEmpLeave);
            if (dtEmpLeave.Rows.Count > 0)
                lstEmpLeave.DataSource = dtEmpLeave;
            lstEmpLeave.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string Param = string.Empty;

            if (!string.IsNullOrEmpty(txtEmployee.Text.Trim()))
                Param = "?EmpName=" + txtEmployee.Text.Trim();
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

            Response.Redirect(Application["SiteAddress"] + "admin/EmployeeLeave_List.aspx" + Param);
        }

        protected void lnkDelete_Click(object sender, EventArgs e)
        {
            try
            {
                bool Delete = false;
                dvMessage.InnerHtml = "";
                dvMessage.Visible = false;

                LoginModel objLoginModel = new LoginModel();
                objLoginModel = Session["LoginSession"] as LoginModel;

                objEmployeeLeaveService = ServiceFactory.EmployeeLeaveService;

                for (int i = 0; i <= lstEmpLeave.Items.Count - 1; i++)
                {
                    HtmlInputCheckBox chkLeave = (HtmlInputCheckBox)lstEmpLeave.Items[i].FindControl("chkcheck");
                    if (chkLeave.Checked)
                    {
                        HiddenField hdnLeaveId = (HiddenField)lstEmpLeave.Items[i].FindControl("hdnLeaveId");
                        HiddenField hdnAllowDelete = (HiddenField)lstEmpLeave.Items[i].FindControl("hdnAllowDelete");

                        if (hdnAllowDelete.Value == "0")
                        {
                            dvMessage.InnerHtml = "<strong>Only future leave can deleted.</strong>";
                            dvMessage.Attributes.Add("class", "alert alert-error");
                            dvMessage.Visible = true;
                            return;
                        }
                        if (!string.IsNullOrEmpty(hdnLeaveId.Value))
                        {
                            BizObjects.EmployeeLeave objEmployeeLeave = new BizObjects.EmployeeLeave();
                            objEmployeeLeave.Id = Convert.ToInt32(hdnLeaveId.Value);
                            objEmployeeLeave.DeletedBy = objLoginModel.Id;
                            objEmployeeLeave.DeletedDate = DateTime.UtcNow;

                            objEmployeeLeaveService.DeleteEmployeeLeave(ref objEmployeeLeave);
                            Delete = true;
                        }
                    }
                }
                if (Delete)
                {
                    dvMessage.InnerHtml = "<strong>Employee Leave deleted successfully.</strong>";
                    dvMessage.Attributes.Add("class", "alert alert-success");
                    dvMessage.Visible = true;
                }
                BindEmployeeLeave();
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