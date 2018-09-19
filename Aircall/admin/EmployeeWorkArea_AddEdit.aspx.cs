using Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Aircall.admin
{
    public partial class EmployeeWorkArea_AddEdit : System.Web.UI.Page
    {
        IAreasService objAreasService;
        IEmployeeService objEmployeeService;
        IEmployeeWorkAreaService objEmployeeWorkAreaService;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                FillPriorityGroups();

                if (!string.IsNullOrEmpty(Request.QueryString["EmployeeId"]))
                {
                    BindEmployeeWorkAreaByEmployee();
                }
            }
        }

        private void BindEmployeeWorkAreaByEmployee()
        {
            btnSave.Text = "Update";
            lnkSearch.Visible = false;
            Panel1.DefaultButton = "";
            int EmployeeId = Convert.ToInt32(Request.QueryString["EmployeeId"].ToString());
            DataTable dtPriority1 = new DataTable();
            BindEmployee(EmployeeId);
            objEmployeeWorkAreaService = ServiceFactory.EmployeeWorkAreaService;
            objEmployeeWorkAreaService.GetWorkAreaByPriority((int)Aircall.Common.General.PriorityArea.Priority1, EmployeeId, ref dtPriority1);
            if (dtPriority1.Rows.Count > 0)
            {
                for (int i = 0; i < dtPriority1.Rows.Count; i++)
                {
                    ListItem item = chkPriority1.Items.FindByValue(dtPriority1.Rows[i]["AreaId"].ToString());
                    item.Selected = true;
                }
            }
            DataTable dtPriority2 = new DataTable();
            objEmployeeWorkAreaService = ServiceFactory.EmployeeWorkAreaService;
            objEmployeeWorkAreaService.GetWorkAreaByPriority((int)Aircall.Common.General.PriorityArea.Priority2, EmployeeId, ref dtPriority2);
            if (dtPriority2.Rows.Count > 0)
            {
                for (int i = 0; i < dtPriority2.Rows.Count; i++)
                {
                    ListItem item = chkPriority2.Items.FindByValue(dtPriority2.Rows[i]["AreaId"].ToString());
                    item.Selected = true;
                }
            }
        }

        private void BindEmployee(int EmployeeId)
        {
            objEmployeeService = ServiceFactory.EmployeeService;
            DataTable dtEmployees = new DataTable();
            objEmployeeService.GetEmployeeById(EmployeeId, ref dtEmployees);
            if (dtEmployees.Rows.Count > 0)
            {
                rblEmployee.DataSource = dtEmployees;
                rblEmployee.DataTextField = dtEmployees.Columns["EmployeeName"].ToString();
                rblEmployee.DataValueField = dtEmployees.Columns["Id"].ToString();
                rblEmployee.DataBind();
                rblEmployee.SelectedValue = EmployeeId.ToString();
                rblEmployee.Enabled = false;
                txtEmpName.Text = dtEmployees.Rows[0]["EmployeeName"].ToString();
                txtEmpName.Enabled = false;
            }
        }

        private void FillPriorityGroups()
        {
            DataTable dtGroupAreas = new DataTable();
            objAreasService = ServiceFactory.AreasService;
            objAreasService.GetAllAreas(true, ref dtGroupAreas);
            if (dtGroupAreas.Rows.Count > 0)
            {
                chkPriority1.DataSource = dtGroupAreas;
                chkPriority1.DataTextField = dtGroupAreas.Columns["Name"].ToString();
                chkPriority1.DataValueField = dtGroupAreas.Columns["Id"].ToString();
                chkPriority2.DataSource = dtGroupAreas;
                chkPriority2.DataTextField = dtGroupAreas.Columns["Name"].ToString();
                chkPriority2.DataValueField = dtGroupAreas.Columns["Id"].ToString();
            }
            chkPriority1.DataBind();
            chkPriority2.DataBind();
        }

        protected void lnkSearch_Click(object sender, EventArgs e)
        {
            //if (!string.IsNullOrEmpty(txtEmpName.Text.Trim()))
            //{
            objEmployeeService = ServiceFactory.EmployeeService;
            DataTable dtEmployee = new DataTable();
            objEmployeeService.GetEmployeeByName(txtEmpName.Text.Trim(), false, ref dtEmployee);
            if (dtEmployee.Rows.Count > 0)
            {
                rblEmployee.DataSource = dtEmployee;
                rblEmployee.DataTextField = dtEmployee.Columns["EmployeeName"].ToString();
                rblEmployee.DataValueField = dtEmployee.Columns["Id"].ToString();
                rblEmployee.DataBind();
            }
            else
            {
                rblEmployee.DataSource = "";
                rblEmployee.DataBind();
            }
            //}
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    if (rblEmployee.Items.Count == 0 || rblEmployee.SelectedIndex == -1)
                    {
                        dvMessage.InnerHtml = "<strong>Please Select Employee.</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-error");
                        dvMessage.Visible = true;
                        return;
                    }

                    //if (chkPriority1.Items.Count == 0 || chkPriority1.SelectedIndex == -1)
                    //{
                    //    dvMessage.InnerHtml = "<strong>Please Select 1st Priority.</strong>";
                    //    dvMessage.Attributes.Add("class", "alert alert-error");
                    //    dvMessage.Visible = true;
                    //    return;
                    //}

                    if ((chkPriority1.Items.Count == 0 || chkPriority1.SelectedIndex == -1) && (chkPriority2.Items.Count == 0 || chkPriority2.SelectedIndex == -1))
                    {
                        dvMessage.InnerHtml = "<strong>Please Select 1st Priority or 2nd Priority.</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-error");
                        dvMessage.Visible = true;
                        return;
                    }

                    //if (chkPriority2.Items.Count == 0 || chkPriority2.SelectedIndex == -1)
                    //{
                    //    dvMessage.InnerHtml = "<strong>Please Select 2nd Priority.</strong>";
                    //    dvMessage.Attributes.Add("class", "alert alert-error");
                    //    dvMessage.Visible = true;
                    //    return;
                    //}

                    if (!string.IsNullOrEmpty(Request.QueryString["EmployeeId"]))
                    {
                        int EmployeeId = Convert.ToInt32(Request.QueryString["EmployeeId"].ToString());
                        objEmployeeWorkAreaService = ServiceFactory.EmployeeWorkAreaService;
                        objEmployeeWorkAreaService.DeleteEmployeeWorkAreaByEmployeeId(EmployeeId);
                    }
                    else
                    {
                        DataTable dtEmployeeWorkArea = new DataTable();
                        objEmployeeWorkAreaService = ServiceFactory.EmployeeWorkAreaService;
                        objEmployeeWorkAreaService.GetEmployeeWorkAreaByEmployeeId(Convert.ToInt32(rblEmployee.SelectedValue.ToString()), ref dtEmployeeWorkArea);
                        if (dtEmployeeWorkArea.Rows.Count > 0)
                        {
                            dvMessage.InnerHtml = "<strong>Selected Employee Workarea already added into the system.</strong>";
                            dvMessage.Attributes.Add("class", "alert alert-error");
                            dvMessage.Visible = true;
                            return;
                        }
                    }

                    BizObjects.EmployeeWorkArea objEmployeeWorkArea = new BizObjects.EmployeeWorkArea();
                    objEmployeeWorkAreaService = ServiceFactory.EmployeeWorkAreaService;

                    objEmployeeWorkArea.EmployeeId = Convert.ToInt32(rblEmployee.SelectedValue.ToString());
                    foreach (ListItem item in chkPriority1.Items)
                    {
                        if (item.Selected)
                        {
                            objEmployeeWorkArea.PriorityArea = (int)Aircall.Common.General.PriorityArea.Priority1;
                            objEmployeeWorkArea.AreaId = Convert.ToInt32(item.Value);
                            objEmployeeWorkAreaService.AddEmployeeWorkArea(ref objEmployeeWorkArea);
                        }
                    }

                    foreach (ListItem item in chkPriority2.Items)
                    {
                        if (item.Selected)
                        {
                            objEmployeeWorkArea.PriorityArea = (int)Aircall.Common.General.PriorityArea.Priority2;
                            objEmployeeWorkArea.AreaId = Convert.ToInt32(item.Value);
                            objEmployeeWorkAreaService.AddEmployeeWorkArea(ref objEmployeeWorkArea);
                        }
                    }
                    if (!string.IsNullOrEmpty(Request.QueryString["EmployeeId"]))
                    {
                        Session["msg"] = "edit";
                        Response.Redirect(Application["SiteAddress"] + "admin/EmployeeWorkArea_List.aspx");
                    }
                    else
                    {
                        Session["msg"] = "add";
                        Response.Redirect(Application["SiteAddress"] + "admin/EmployeeWorkArea_List.aspx");
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

        protected void chkPriority1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string SelectedP2 = string.Empty;
            foreach (ListItem item in chkPriority2.Items)
            {
                if (item.Selected)
                {
                    SelectedP2 += item.Text + ',';
                }
            };

            DataTable dtGroupAreas = new DataTable();
            objAreasService = ServiceFactory.AreasService;
            objAreasService.GetAllAreas(true, ref dtGroupAreas);
            if (dtGroupAreas.Rows.Count > 0)
            {
                chkPriority2.DataSource = dtGroupAreas;
                chkPriority2.DataTextField = dtGroupAreas.Columns["Name"].ToString();
                chkPriority2.DataValueField = dtGroupAreas.Columns["Id"].ToString();
            }
            chkPriority2.DataBind();

            foreach (ListItem item in chkPriority1.Items)
            {
                if (item.Selected)
                {
                    chkPriority2.Items.Remove(item);
                }
            }

            foreach (var item in SelectedP2.Split(','))
            {
                foreach (ListItem item1 in chkPriority2.Items)
                {
                    if (item1.Text == item)
                    {
                        item1.Selected = true;
                    }
                }
            }
        }

        protected void chkPriority2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string SelectedP1 = string.Empty;
            foreach (ListItem item in chkPriority1.Items)
            {
                if (item.Selected)
                {
                    SelectedP1 += item.Text + ',';
                }
            };

            DataTable dtGroupAreas = new DataTable();
            objAreasService = ServiceFactory.AreasService;
            objAreasService.GetAllAreas(true, ref dtGroupAreas);
            if (dtGroupAreas.Rows.Count > 0)
            {
                chkPriority1.DataSource = dtGroupAreas;
                chkPriority1.DataTextField = dtGroupAreas.Columns["Name"].ToString();
                chkPriority1.DataValueField = dtGroupAreas.Columns["Id"].ToString();
            }
            chkPriority1.DataBind();

            foreach (ListItem item in chkPriority2.Items)
            {
                if (item.Selected)
                {
                    chkPriority1.Items.Remove(item);
                }
            }

            foreach (var item in SelectedP1.Split(','))
            {
                foreach (ListItem item1 in chkPriority1.Items)
                {
                    if (item1.Text == item)
                    {
                        item1.Selected = true;
                    }
                }
            }
        }
    }
}