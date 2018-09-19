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
    public partial class sales_report : System.Web.UI.Page
    {
        IAdminService objAdminService = ServiceFactory.AdminService;
        IOrderService objOrderService = ServiceFactory.OrderService;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindYear();

                if (Request.QueryString["ClientName"] != null)
                {
                    this.txtClientName.Text = Request.QueryString["ClientName"];
                }
                if (Request.QueryString["Month"] != null)
                {
                    this.drpMonth.SelectedValue = Request.QueryString["Month"];
                }
                else
                {
                    var strcurrentmonth = DateTime.Now.Month.ToString();
                    this.drpMonth.SelectedValue = strcurrentmonth;
                }
                if (Request.QueryString["Year"] != null)
                {
                    this.ddlYear.SelectedValue = Request.QueryString["Year"];
                }
                else
                {
                    var strcurrentYear = DateTime.Now.Year.ToString();
                    this.ddlYear.SelectedValue = strcurrentYear;
                }

                BindSales();
            }
        }

        private void BindSales()
        {
            string ClientName = this.txtClientName.Text;
            int strmonth = Convert.ToInt32(this.drpMonth.SelectedValue);
            int stryear = Convert.ToInt32(this.ddlYear.SelectedValue);
            int PlanType = 0;
            DataTable dtMonthlySale = new DataTable();
            IAdminService objAdminService = ServiceFactory.AdminService;
            objAdminService.GetMonthlySaleReportChart(ClientName, strmonth, stryear, PlanType, ref dtMonthlySale);
            var list = new List<SaleItem>();
            foreach (DataRow dr in dtMonthlySale.Rows)
            {
                var amount = Convert.ToDecimal(dr["MonthlySale"]);
                var day = Convert.ToString(dr["Months"]);
                day = stryear.ToString() + "-" + day;
                if (amount > 0)
                {
                    list.Add(new SaleItem { SaleDay = day, SaleAmount = amount });
                }
            }
            this.lstSales.DataSource = list;
            this.lstSales.DataBind();
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

        protected void Button1_Click(object sender, EventArgs e)
        {
            var pa = "?Search=Y";
            pa += "&ClientName=" + this.txtClientName.Text;
            pa += "&Month=" + this.drpMonth.SelectedValue;
            pa += "&Year=" + this.ddlYear.SelectedValue;
            Response.Redirect("sales_report.aspx" + pa);
        }
    }

    public class SaleItem
    {
        public string SaleDay { get; set; }
        public decimal SaleAmount { get; set; }
    }
}