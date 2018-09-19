using Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using DBUtility;

namespace Aircall.admin
{
    public partial class SalesPerDay : System.Web.UI.Page
    {
        IAdminService objAdminService = ServiceFactory.AdminService;
        IOrderService objOrderService = ServiceFactory.OrderService;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindSales();
            }
        }

        private void BindSales()
        {
            var saleDay = Request.QueryString["SaleDay"];
            var arr = saleDay.Split('-');
            var year = Convert.ToInt32(arr[0]);
            var month = Convert.ToInt32(arr[1]);
            var day = Convert.ToInt32(arr[2]);
            var sql = @"select A.TransactionDate as SaleDay, A.PurchasedAmount as SaleAmount, B.FirstName + ' ' + B.LastName as ClientName, B.Company from BillingHistory A
                        inner join Client B on A.ClientId = B.Id
                        where day(A.TransactionDate) = @Day and month(A.TransactionDate) = @Month and year(A.TransactionDate) = @Year and isnull(A.IsPaid, '0') = '1'";
            SqlParameter[] paramArr = new SqlParameter[]
            {
                new SqlParameter("@Day", day),
                new SqlParameter("@Month", month),
                new SqlParameter("@Year", year),
            };
            var instance = new DBUtility.SQLDBHelper();
            var ds = instance.Query(sql, paramArr);
            this.lstSales.DataSource = ds.Tables[0];
            this.lstSales.DataBind();
        }
    }
}