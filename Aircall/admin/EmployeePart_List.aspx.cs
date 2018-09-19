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
    public partial class EmployeePart_List : System.Web.UI.Page
    {
        IServicesService objServicesService;
        IServicePartListService objServicePartListService;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindEmployeePartList();
            }
            //imgPlusCountry.Attributes.Add("onClick", "javascript:OnClickView();return false;");
            //imgMinusCountry.Attributes.Add("onClick", "javascript:OnClickHide();return false;");
            //imgPlusCountry1.Attributes.Add("onClick", "javascript:OnClickView1();return false;");
            //imgMinusCountry1.Attributes.Add("onClick", "javascript:OnClickHide1();return false;");
        }

        private void BindEmployeePartList()
        {
            objServicesService = ServiceFactory.ServicesService;
            DataTable dtServices = new DataTable();
            string ServiceCaseNumber = string.Empty;
            string EmpName = string.Empty;
            string StartDate = string.Empty;
            string EndDate = string.Empty;

            if (!string.IsNullOrEmpty(Request.QueryString["EmpName"]))
            {
                EmpName = Request.QueryString["EmpName"].ToString();
                txtEmployee.Text = EmpName;
            }
            if (!string.IsNullOrEmpty(Request.QueryString["CaseNo"]))
            {
                ServiceCaseNumber = Request.QueryString["CaseNo"].ToString();
                txtCaseNo.Text = ServiceCaseNumber;
            }
            if (!string.IsNullOrEmpty(Request.QueryString["StartDate"]))
            {
                StartDate = Request.QueryString["StartDate"].ToString();
                txtStart.Value = StartDate;
            }
            else
            {
                StartDate = DateTime.Now.ToString("MM/dd/yyyy");
                txtStart.Value = StartDate;
            }
                
            if (!string.IsNullOrEmpty(Request.QueryString["EndDate"]))
            {
                EndDate = Request.QueryString["EndDate"].ToString();
                txtEnd.Value = EndDate;
            }
            else
            {
                EndDate = DateTime.Now.AddDays(1).ToString("MM/dd/yyyy");
                txtEnd.Value = EndDate;
            }
                
            objServicesService.GetServiceForEmployeePartList(ServiceCaseNumber, EmpName, StartDate, EndDate, ref dtServices);
            if (dtServices.Rows.Count > 0)
            {
                lstEmployee.DataSource = dtServices;
            }
            lstEmployee.DataBind();
        }

        protected void lstEmployee_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType==ListViewItemType.DataItem)
            {
                DataRow dr = (e.Item.DataItem as DataRowView).Row;
                DataTable dtEmpParts = new DataTable();
                ListView lstEmpParts = (ListView)e.Item.FindControl("lstEmpParts");
                objServicePartListService = ServiceFactory.ServicePartListService;
                objServicePartListService.GetServicePartlistByServiceIdForEmployeePartList(Convert.ToInt64(dr["Id"].ToString()), ref dtEmpParts);
                if (dtEmpParts.Rows.Count>0)
                {
                    lstEmpParts.DataSource=dtEmpParts;
                }
                lstEmpParts.DataBind();
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string Param = string.Empty;

            if (!string.IsNullOrEmpty(txtEmployee.Text.Trim()))
                Param = "?EmpName=" + txtEmployee.Text.Trim();
            if (!string.IsNullOrEmpty(txtCaseNo.Text.Trim()))
            {
                if (!string.IsNullOrEmpty(Param))
                    Param = Param + "&CaseNo=" + txtCaseNo.Text.Trim();
                else
                    Param = "?CaseNo=" + txtCaseNo.Text.Trim();
            }
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

            Response.Redirect(Application["SiteAddress"] + "admin/EmployeePart_List.aspx" + Param);
        }
    }
}