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
    public partial class sales_report_bk : System.Web.UI.Page
    {
        IPlanService objPlanService;
        IAdminService objAdminService = ServiceFactory.AdminService;
        IOrderService objOrderService = ServiceFactory.OrderService;
        public string strcurrentmonth;
        public string strcurrentYear;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindPlanTypeDropdown();
                BindYear();
            }
            strcurrentmonth = DateTime.Now.Month.ToString();
            strcurrentYear = DateTime.Now.Year.ToString();
        }
        private void BindPlanTypeDropdown()
        {
            objPlanService = ServiceFactory.PlanService;
            DataTable dtPlanType = new DataTable();
            objPlanService.GetAllPlanType(ref dtPlanType);
            if (dtPlanType.Rows.Count > 0)
            {
                drpPlanType.DataSource = dtPlanType;
                drpPlanType.DataTextField = dtPlanType.Columns["Name"].ToString();
                drpPlanType.DataValueField = dtPlanType.Columns["Id"].ToString();
                drpPlanType.DataBind();
            }
            drpPlanType.Items.Insert(0, new ListItem("All Plan Type", "0"));
        }
        private void BindYear()
        {
            ddlYear.Items.Clear();

            ddlYear.Items.Add(new ListItem("select Year", "0"));
            for (int i = 2016; i <= DateTime.Now.Year; i++)
            {
                ddlYear.Items.Add(new ListItem(i.ToString(), i.ToString()));
            }
        }
        [System.Web.Services.WebMethod]
        public static string BindChart(string ClientName, int strmonth, int stryear, int PlanType)
        {
            DataTable dtMonthlySale = new DataTable();
            IAdminService objAdminService = ServiceFactory.AdminService;
            objAdminService.GetMonthlySaleReportChart(ClientName, strmonth, stryear, PlanType, ref dtMonthlySale);
            string str = string.Empty;
            decimal total = 0m;
            if (dtMonthlySale.Rows.Count > 0)
            {
                for (int i = 0; i < dtMonthlySale.Rows.Count; i++)
                {
                    if (strmonth > 0)
                    {
                        if (string.IsNullOrEmpty(str))
                            str = dtMonthlySale.Rows[i]["days"].ToString() + "|" + dtMonthlySale.Rows[i]["MonthlySale"].ToString();
                        else
                            str = str + "##" + dtMonthlySale.Rows[i]["days"].ToString() + "|" + dtMonthlySale.Rows[i]["MonthlySale"].ToString();
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(str))
                            str = dtMonthlySale.Rows[i]["Months"].ToString() + "|" + dtMonthlySale.Rows[i]["MonthlySale"].ToString();
                        else
                            str = str + "##" + dtMonthlySale.Rows[i]["Months"].ToString() + "|" + dtMonthlySale.Rows[i]["MonthlySale"].ToString();
                    }
                    total = total + decimal.Parse(dtMonthlySale.Rows[i]["MonthlySale"].ToString());
                }
            }
            return str + "^^" + total.ToString();
        }
    }
}