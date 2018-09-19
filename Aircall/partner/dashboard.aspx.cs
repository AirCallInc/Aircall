using Aircall.Common;
using Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Aircall.partner
{
    public partial class dashboard : System.Web.UI.Page
    {
        public string strcurrentmonth;
        public string strcurrentYear;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["PartnerLoginCookie"] == null)
            {
                Response.Redirect(Application["SiteAddress"] + "partner/Login.aspx");
            }
            strcurrentmonth = DateTime.Now.Month.ToString();
            strcurrentYear = DateTime.Now.Year.ToString();
        }
        [System.Web.Services.WebMethod]
        //public static string BindChart(int ClientId, int Month, DateTime Startdate, DateTime EndDate)
        public static string BindChart(int ClientId, int strmonth, int stryear)
        {
            LoginModel login = HttpContext.Current.Session["PartnerLoginCookie"] as LoginModel;
            DataTable dtPartnerprofit = new DataTable();
            IPartnerService Objpartnerclientprofit = ServiceFactory.PartnerService;
            Objpartnerclientprofit.GetPartnerSaleReportChart(login.Id, strmonth, stryear, ref dtPartnerprofit);
            string str = string.Empty;

            if (dtPartnerprofit.Rows.Count > 0)
            {
                for (int i = 0; i < dtPartnerprofit.Rows.Count; i++)
                {
                    if (string.IsNullOrEmpty(str))
                        str = dtPartnerprofit.Rows[i]["days"].ToString() + "|" + dtPartnerprofit.Rows[i]["MonthlySale"].ToString();
                    else
                        str = str + "##" + dtPartnerprofit.Rows[i]["days"].ToString() + "|" + dtPartnerprofit.Rows[i]["MonthlySale"].ToString();
                }
            }
            return str;

        }
    }
}