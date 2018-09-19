using Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Aircall.Common;

namespace Aircall.admin
{
    public partial class dashboard : System.Web.UI.Page
    {
        IAdminService objAdminService = ServiceFactory.AdminService;
        IOrderService objOrderService = ServiceFactory.OrderService;
        public string strcurrentmonth;
        public string strcurrentYear;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["LoginSession"] != null)
            {
                LoginModel objLoginModel = new LoginModel();
                objLoginModel = Session["LoginSession"] as LoginModel;

                if (objLoginModel.RoleId == General.UserRoles.WarehouseUser.GetEnumValue())
                {
                    ltrRoleName.Text = "Aircall Warehouse User";
                    lstDashboardrw.DataSource = "";
                    lstDashboardrw.DataBind();
                }
                else
                {
                    ltrRoleName.Text="Aircall Admin";
                    DataTable dtDashboardCnt = new DataTable();
                    objAdminService.GetDashboard(ref dtDashboardCnt);

                    DataView view = new DataView(dtDashboardCnt);
                    DataTable distinctValues = view.ToTable(true, "rw");
                    lstDashboardrw.DataSource = distinctValues;
                    lstDashboardrw.DataBind();
                }
                strcurrentmonth = DateTime.Now.Month.ToString();
                strcurrentYear = DateTime.Now.Year.ToString();

                DataTable dtMonthlySale = new DataTable();
                objOrderService.GetAllOrders("", "", "", "", ref dtMonthlySale);
                DataTable dtclone = dtMonthlySale.Clone();
                var datarows = dtMonthlySale.Rows.Cast<DataRow>().Take(5).ToArray();
                foreach (var item in datarows)
                {
                    dtclone.Rows.Add(item.ItemArray);
                }
                lstResentOrders.DataSource = dtclone;
                lstResentOrders.DataBind();

                int RoleId = objLoginModel.RoleId;
                if (General.UserRoles.WarehouseUser.GetEnumValue() == RoleId)
                {
                    dvOrder.Visible = false;
                    dvSales.Visible = false;
                }
            }
        }
        [System.Web.Services.WebMethod]
        public static string BindChart(int ClientId, int strmonth, int stryear)
        {

            DataTable dtMonthlySale = new DataTable();
            IAdminService objAdminService = ServiceFactory.AdminService;
            objAdminService.GetMonthlySaleReportChart("", strmonth, stryear, 0, ref dtMonthlySale);
            string str = string.Empty;
            decimal total = 0m;
            if (dtMonthlySale.Rows.Count > 0)
            {
                for (int i = 0; i < dtMonthlySale.Rows.Count; i++)
                {
                    if (string.IsNullOrEmpty(str))
                        str = dtMonthlySale.Rows[i]["days"].ToString() + "|" + dtMonthlySale.Rows[i]["MonthlySale"].ToString();
                    else
                        str = str + "##" + dtMonthlySale.Rows[i]["days"].ToString() + "|" + dtMonthlySale.Rows[i]["MonthlySale"].ToString();
                    total = total + decimal.Parse(dtMonthlySale.Rows[i]["MonthlySale"].ToString());
                }
            }
            return str + "^^" + total.ToString();
        }
        protected void lstDashboardrw_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                DataRow row = (e.Item.DataItem as DataRowView).Row;
                DataTable dtDashboardCnt = new DataTable();
                objAdminService.GetDashboard(ref dtDashboardCnt);
                DataTable dtclone = dtDashboardCnt.Clone();
                DataRow[] rows = dtDashboardCnt.Select(" rw = " + row["rw"].ToString());
                foreach (var item in rows)
                {
                    dtclone.Rows.Add(item.ItemArray);
                }
                ListView lstDashboard = e.Item.FindControl("lstDashboard") as ListView;

                lstDashboard.DataSource = dtclone;
                lstDashboard.DataBind();

            }
        }
    }
}