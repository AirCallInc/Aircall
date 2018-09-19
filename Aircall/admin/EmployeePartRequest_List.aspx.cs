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
    public partial class EmployeePartRequest_List : System.Web.UI.Page
    {
        IDailyPartListMasterService objDailyPartListMasterService;
        IEmployeePartRequestMasterService objEmployeePartRequestMasterService;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                FillPartTypeDropdown();
                FillEmpPartRequestStatusDropdown();
                if (Session["msg"] != null)
                {
                    if (Session["msg"].ToString() == "edit")
                    {
                        dvMessage.InnerHtml = "<strong>Employee Part Request updated successfully.</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-success");
                        dvMessage.Visible = true;
                    }
                    else if (Session["msg"].ToString() == "add")
                    {
                        dvMessage.InnerHtml = "<strong>Employee Part Request added successfully.</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-success");
                        dvMessage.Visible = true;
                    }
                    Session["msg"] = null;
                }
                BindEmployeePartRequest();
            }
        }
        private void FillPartTypeDropdown()
        {
            DataTable dtParts = new DataTable();
            objDailyPartListMasterService = ServiceFactory.DailyPartListMasterService;
            objDailyPartListMasterService.GetAllDailyPartList(ref dtParts);
            if (dtParts.Rows.Count > 0)
            {
                drpPartType.DataSource = dtParts;
                drpPartType.DataTextField = dtParts.Columns["Name"].ToString();
                drpPartType.DataValueField = dtParts.Columns["Id"].ToString();
            }
            drpPartType.DataBind();
            drpPartType.Items.Insert(0, new ListItem("Select Part Type", "0"));
            drpPartType.Items.Insert(drpPartType.Items.Count, new ListItem("Other", "-1"));
        }

        private void FillEmpPartRequestStatusDropdown()
        {
            var values = DurationExtensions.GetValues<General.EmpPartRequestStatus>();
            List<object> data = new List<object>();
            foreach (var item in values)
            {
                General.EmpPartRequestStatus p = (General.EmpPartRequestStatus)item;
                data.Add(new { Id = p.GetEnumValue(), Name = p.GetEnumDescription() });
            }
            drpStatus.DataSource = data;
            drpStatus.DataValueField = "Id";
            drpStatus.DataTextField = "Name";
            drpStatus.DataBind();
            drpStatus.Items.Insert(0, new ListItem("Select Status", "0"));
        }
        private void BindEmployeePartRequest()
        {
            objEmployeePartRequestMasterService = ServiceFactory.EmployeePartRequestMasterService;
            DataTable dtEmpParts = new DataTable();

            string EmpName = string.Empty;
            string PartName = string.Empty;
            string PartType = "0";
            string Status = "0";

            if (!string.IsNullOrEmpty(Request.QueryString["PartType"]))
            {
                PartType = Request.QueryString["PartType"].ToString();
                drpPartType.SelectedValue = PartType;
            }
            if (!string.IsNullOrEmpty(Request.QueryString["EmpName"]))
            {
                EmpName = Request.QueryString["EmpName"].ToString();
                txtEmpname.Text= EmpName;
            }
            if (!string.IsNullOrEmpty(Request.QueryString["PName"]))
            {
                PartName = Request.QueryString["PName"].ToString();
                txtPart.Text = PartName;
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Status"]))
            {
                Status = Request.QueryString["Status"].ToString();
                drpStatus.SelectedValue = Status;
            }

            objEmployeePartRequestMasterService.GetAllEmployeePartRequestByPartTypeId(EmpName, PartName,int.Parse(PartType), int.Parse(Status),ref dtEmpParts);
            if (dtEmpParts.Rows.Count > 0)
            {
                lstEmpPartRequest.DataSource = dtEmpParts;
            }
            lstEmpPartRequest.DataBind();
        }

        protected void dataPagerEmpPartRequest_PreRender(object sender, EventArgs e)
        {
            dataPagerEmpPartRequest.PageSize = Convert.ToInt32(Application["PageSize"].ToString());
            BindEmployeePartRequest();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string Param = string.Empty;
            if (drpPartType.SelectedItem.Value != "0")
            {
                Param = "?PartType=" + drpPartType.SelectedItem.Value;
            }
            if (!string.IsNullOrEmpty(txtEmpname.Text.Trim()))
            {
                if (!string.IsNullOrEmpty(Param))
                    Param = Param + "&EmpName=" + txtEmpname.Text.Trim();
                else
                    Param = "?EmpName=" + txtEmpname.Text.Trim();
            }
            if (!string.IsNullOrEmpty(txtPart.Text.Trim()))
            {
                if (!string.IsNullOrEmpty(Param))
                    Param = Param + "&PName=" + txtPart.Text.Trim();
                else
                    Param = "?PName=" + txtPart.Text.Trim();
            }
            if (drpStatus.SelectedItem.Value != "0")
            {
                if (!string.IsNullOrEmpty(Param))
                    Param = Param + "&Status=" + drpStatus.SelectedValue.ToString();
                else
                    Param = "?Status=" + drpStatus.SelectedValue.ToString();
            }
            Response.Redirect(Application["SiteAddress"] + "admin/EmployeePartRequest_List.aspx" + Param);
        }
    }
}