using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Services;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using System.Collections;
using System.Text;
using Aircall.Common;

namespace Aircall.partner
{
    public partial class Commission_Report : System.Web.UI.Page
    {
        IPartnerService Objclient;
        public string strcurrentmonth;
        public string strcurrentYear;
        //public DateTime startOfMonth;
        //public DateTime endOfMonth;
        protected void Page_Load(object sender, EventArgs e)
        {
            strcurrentmonth = DateTime.Now.Month.ToString();
            strcurrentYear = DateTime.Now.Year.ToString();
            //DateTime today = DateTime.Today;
            // int daysInMonth = DateTime.DaysInMonth(today.Year, today.Month);
            // startOfMonth = new DateTime(today.Year, today.Month, 1);
            //  endOfMonth = new DateTime(today.Year, today.Month, daysInMonth);
            if (Session["PartnerLoginCookie"] == null)
            {
                Response.Redirect(Application["SiteAddress"] + "partner/Login.aspx");
            }
            if (!IsPostBack)
            {
                BindClientByPartner();
                BindYear();
            }
        }

        private void BindClientByPartner()
        {
            int PartnerId = (Session["PartnerLoginCookie"] as LoginModel).Id;// Convert.ToInt32(Request.Cookies["PartnerLoginCookie"]["PartnerId"].ToString());
            Objclient = ServiceFactory.PartnerService;
            DataTable dtclient = new DataTable();
            Objclient.GetAllClientByPartner(PartnerId, ref dtclient);
            if (dtclient.Rows.Count > 0)
            {
                ddlClient.DataSource = dtclient;
                ddlClient.DataValueField = "Id";
                ddlClient.DataTextField = "ClientName";
            }
            ddlClient.DataBind();
            ddlClient.Items.Insert(0, new ListItem("All Client", "0"));
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
        //public static string BindChart(int ClientId, int Month, DateTime Startdate, DateTime EndDate)
        public static string BindChart(int ClientId, int strmonth, int stryear)
        {
            DataTable dtPartnerprofit = new DataTable();
            LoginModel login = HttpContext.Current.Session["PartnerLoginCookie"] as LoginModel;
            IPartnerService Objpartnerclientprofit = ServiceFactory.PartnerService;
            Objpartnerclientprofit.GetProfiteByPartner(ClientId, login.Id, strmonth, stryear, ref dtPartnerprofit);
            string str = string.Empty;

            if (dtPartnerprofit.Rows.Count > 0)
            {
                for (int i = 0; i < dtPartnerprofit.Rows.Count; i++)
                {
                    if (strmonth > 0)
                    {
                        if (string.IsNullOrEmpty(str))
                            str = dtPartnerprofit.Rows[i]["days"].ToString() + "|" + dtPartnerprofit.Rows[i]["Commission"].ToString();
                        else
                            str = str + "##" + dtPartnerprofit.Rows[i]["days"].ToString() + "|" + dtPartnerprofit.Rows[i]["Commission"].ToString();
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(str))
                            str = dtPartnerprofit.Rows[i]["Months"].ToString() + "|" + dtPartnerprofit.Rows[i]["Commission"].ToString();
                        else
                            str = str + "##" + dtPartnerprofit.Rows[i]["Months"].ToString() + "|" + dtPartnerprofit.Rows[i]["Commission"].ToString();
                    }
                }
            }
            return str;
        }
    }
}