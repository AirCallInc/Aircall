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
    public partial class partner_report : System.Web.UI.Page
    {
        public string strcurrentmonth;
        public string strcurrentYear;

        protected void Page_Load(object sender, EventArgs e)
        {
            BindYear();
            strcurrentmonth = DateTime.Now.Month.ToString();
            strcurrentYear = DateTime.Now.Year.ToString();
            drpMonth.SelectedValue = DateTime.Now.Month.ToString();
            ddlYear.SelectedValue = DateTime.Now.Year.ToString();
            BindClientByAffiliateId();
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
        public static string BindChart(string partner, int strmonth, int stryear)
        {
            DataTable dtMonthlySale = new DataTable();
            IAdminService objAdminService = ServiceFactory.AdminService;
            objAdminService.GetPartnerSaleReportChart(partner, strmonth, stryear, ref dtMonthlySale);
            string str = string.Empty;

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
                }
            }
            return str;

        }

        private void BindClientByAffiliateId()
        {
            IAdminService objAdminService = ServiceFactory.AdminService;
            DataTable dtclient = new DataTable();
            objAdminService.GetPartnerSaleReportTable(txtPartnerName.Text, int.Parse(drpMonth.SelectedItem.Value), int.Parse(ddlYear.SelectedItem.Value), ref dtclient);
            if (dtclient.Rows.Count > 0)
            {
                lstPartnerClient.DataSource = dtclient;
            }
            lstPartnerClient.DataBind();
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            BindClientByAffiliateId();
        }
    }
}