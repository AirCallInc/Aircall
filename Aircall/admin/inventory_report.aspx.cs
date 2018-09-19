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
    public partial class inventory_report : System.Web.UI.Page
    {
        public string strstartdt1;
        public string strenddt1;
        public string opens;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                strstartdt1 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString("MM/dd/yyyy");
                strenddt1 = DateTime.Now.ToString("MM/dd/yyyy");
                txtStartDate.Value = strstartdt1 + " - " + strenddt1;
                binddata();
            }
            if (Request.Browser.IsMobileDevice)
            {
                opens = "left";
            }
            else
            {
                opens = "right";
            }
        }
        [System.Web.Services.WebMethod]
        public static string BindChart(string EmpName, string strmonth, string stryear, string PartName)
        {            
            IEmployeePartRequestMasterService objEmployeePartRequestMasterService = ServiceFactory.EmployeePartRequestMasterService;
            DataTable dtParts = new DataTable();
            DateTime dtFrom = DateTime.Parse(strmonth);
            DateTime dtTo = DateTime.Parse(stryear);
            DataTable dtResult = new DataTable();
            objEmployeePartRequestMasterService.GetMissingInventoryReportChart(dtFrom, dtTo,PartName, EmpName, ref dtResult);
            string str = string.Empty;

            if (true)
            {
                if (dtResult.Rows.Count > 0)
                {
                    for (int i = 0; i < dtResult.Rows.Count; i++)
                    {
                        if (string.IsNullOrEmpty(str))
                            str = dtResult.Rows[i]["EmployeeName"].ToString() + "|" + dtResult.Rows[i]["cnt"].ToString();
                        else
                            str = str + "##" + dtResult.Rows[i]["EmployeeName"].ToString() + "|" + dtResult.Rows[i]["cnt"].ToString();
                    }
                }
            }
            return str;
        }        

        protected void Button2_Click(object sender, EventArgs e)
        {
            binddata();
        }
        void binddata()
        {            
            IEmployeePartRequestMasterService objEmployeePartRequestMasterService = ServiceFactory.EmployeePartRequestMasterService;
            DataTable dtParts = new DataTable();
            DateTime dtFrom = DateTime.Parse(txtStartDate.Value.Split(("-").ToArray())[0].Trim());
            DateTime dtTo = DateTime.Parse(txtStartDate.Value.Split(("-").ToArray())[1].Trim());
            DataTable dtResult = new DataTable();
            objEmployeePartRequestMasterService.GetMissingInventoryReportTable(dtFrom, dtTo, txtPartName.Text, txtManufactureName.Text, ref dtResult);

            lstRating.DataSource = dtResult;
            lstRating.DataBind();
        }
    }
}