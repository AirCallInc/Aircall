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
    public partial class rating_report : System.Web.UI.Page
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
                if (Request.Browser.IsMobileDevice)
                {
                    opens = "left";
                }
                else
                {
                    opens = "right";
                }
                binddata();
            }
        }

        [System.Web.Services.WebMethod]
        public static string BindChart(string EmpName, string strmonth, string stryear)
        {
            DataTable dtMonthlySale = new DataTable();
            IAdminService objAdminService = ServiceFactory.AdminService;

            objAdminService.GetMonthlyRateReportChart(EmpName.Trim(), strmonth, stryear, ref dtMonthlySale);
            string str = string.Empty;

            if (dtMonthlySale.Rows.Count > 0)
            {
                for (int i = 0; i < dtMonthlySale.Rows.Count; i++)
                {
                    if (string.IsNullOrEmpty(str))
                        str = dtMonthlySale.Rows[i]["Rate"].ToString() + "|" + dtMonthlySale.Rows[i]["EmpCnt"].ToString();
                    else
                        str = str + "##" + dtMonthlySale.Rows[i]["Rate"].ToString() + "|" + dtMonthlySale.Rows[i]["EmpCnt"].ToString();
                }
            }
            return str;

        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            binddata();
        }
        protected SortDirection ListViewSortDirection
        {
            get
            {
                if (ViewState["sortDirection"] == null)
                    ViewState["sortDirection"] = SortDirection.Ascending;
                return (SortDirection)ViewState["sortDirection"];
            }
            set { ViewState["sortDirection"] = value; }
        }

        protected string ListViewSortExpression
        {
            get
            {
                if (ViewState["SortExpression"] == null)
                    ViewState["SortExpression"] = "";
                return (string)ViewState["SortExpression"];
            }
            set { ViewState["SortExpression"] = value; }
        }
        void binddata()
        {
            DataTable dtService = new DataTable();
            string start = txtStartDate.Value.Split(("-").ToArray())[0].Trim();
            string end = txtStartDate.Value.Split(("-").ToArray())[1].Trim();
            objServicesService.GetAllCompletedServices("","", txtEmpName.Text.Trim(), start, end, ListViewSortExpression, ListViewSortDirection.ToString(), ref dtService);
            lstRating.DataSource = dtService;
            lstRating.DataBind();
        }
    }
}