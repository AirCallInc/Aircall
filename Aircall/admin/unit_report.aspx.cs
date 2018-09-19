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
    public partial class unit_report : System.Web.UI.Page
    {
        IServicesService objServicesService = ServiceFactory.ServicesService;
        IAdminService objAdminService = ServiceFactory.AdminService;
        IOrderService objOrderService = ServiceFactory.OrderService;
        public string strstartdt;
        public string strenddt;
        public string opens;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                strstartdt = DateTime.Now.ToString("MM/dd/yyyy");
                strenddt = DateTime.Now.ToString("MM/dd/yyyy");
                txtStartDate.Value = strstartdt + " - " + strenddt;
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
        public static string BindChart(string ManufactureName, string strmonth, string stryear, bool IsPackaged)
        {
            DataTable dtMonthlySale = new DataTable();
            IAdminService objAdminService = ServiceFactory.AdminService;

            objAdminService.GetUnitReportChart(ManufactureName, strmonth, stryear, IsPackaged, ref dtMonthlySale);
            string str = string.Empty;

            if (dtMonthlySale.Rows.Count > 0)
            {
                for (int i = 0; i < dtMonthlySale.Rows.Count; i++)
                {
                    if (string.IsNullOrEmpty(str))
                        str = dtMonthlySale.Rows[i]["ManufactureBrand"].ToString() + "|" + dtMonthlySale.Rows[i]["UnitServiced"].ToString();
                    else
                        str = str + "##" + dtMonthlySale.Rows[i]["ManufactureBrand"].ToString() + "|" + dtMonthlySale.Rows[i]["UnitServiced"].ToString();
                }
            }
            return str;
        }
        [System.Web.Services.WebMethod]
        public static string BindChartAge()
        {
            DataTable dtMonthlySale = new DataTable();
            IAdminService objAdminService = ServiceFactory.AdminService;

            objAdminService.GetUnitAgeWiseCount(ref dtMonthlySale);
            string str = string.Empty;

            if (dtMonthlySale.Rows.Count > 0)
            {
                for (int i = 0; i < dtMonthlySale.Rows.Count; i++)
                {
                    if (string.IsNullOrEmpty(str))
                        str = dtMonthlySale.Rows[i]["UnitAge"].ToString() + "|" + dtMonthlySale.Rows[i]["Cnt"].ToString();
                    else
                        str = str + "##" + dtMonthlySale.Rows[i]["UnitAge"].ToString() + "|" + dtMonthlySale.Rows[i]["Cnt"].ToString();
                }
            }
            return str;
        }
    }
}