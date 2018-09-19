using Aircall.Common;
using Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Aircall.client
{
    public partial class unit_details : System.Web.UI.Page
    {
        IClientUnitService objClientUnitService = ServiceFactory.ClientUnitService;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["ClientLoginCookie"] == null)
            {
                Response.Redirect(Application["SiteAddress"] + "sign-in.aspx");
            }
            if (Request.QueryString["uid"] != null)
            {
                DataTable dtClient = new DataTable();
                int UnitId;
                if (!int.TryParse(Request.QueryString["uid"], out UnitId))
                {
                    Response.Redirect("my_units.aspx");
                }
                //var UnitId = int.Parse(Request.QueryString["uid"]);
                objClientUnitService.GetClientUnitForPortalById(UnitId, (Session["ClientLoginCookie"] as LoginModel).Id, ref dtClient);
                if (dtClient.Rows.Count <= 0)
                {
                    Response.Redirect("dashboard.aspx");
                }
                DataRow row = dtClient.Rows[0];
                litUnitName.Text = row["UnitName"].ToString();
                ltrUnitTon.Text = row["UnitTon"].ToString();
                ltrUnitPlan.Text = row["PlanTypeName"].ToString();
                ltrUnitStatus.Text = ((General.UnitStatus)int.Parse(row["Status"].ToString())).GetEnumDescription();
                if (!string.IsNullOrWhiteSpace(row["ManufactureDate"].ToString()))
                {
                    DateTime ManufactureDate = DateTime.Parse(row["ManufactureDate"].ToString());
                    ltrAge.Text = (DateTime.UtcNow.Year - ManufactureDate.Year) > 0 ? DateTime.UtcNow.Year - ManufactureDate.Year + " Year" : (DateTime.UtcNow.TotalMonths(ManufactureDate) > 0 ? DateTime.UtcNow.TotalMonths(ManufactureDate) : 0) + " Month";
                    ltrMfgDate.Text = ManufactureDate.ToString("MM/YYYY");
                }
                else
                {
                    ltrMfgDate.Text = "N/A";
                    ltrAge.Text = "1 Year";
                }

                if (!string.IsNullOrEmpty(row["CompletedServiceDate"].ToString()))
                    ltrLastService.Text = Convert.ToDateTime(row["CompletedServiceDate"].ToString()).ToString("MM/dd/yyyy") + " At " + row["CompletedServiceTime"].ToString();
                else
                    ltrLastService.Text = "N/A";

                if (!string.IsNullOrEmpty(row["UpcomingServiceDate"].ToString()))
                    ltrUpCommingService.Text = Convert.ToDateTime(row["UpcomingServiceDate"].ToString()).ToString("MM/dd/yyyy") + " At " + row["UpcomingServiceTime"].ToString();
                else
                    ltrUpCommingService.Text = "N/A";

                //ltrTotalService.Text = row["NumberOfService"].ToString();
                //ltrRemainingService.Text = (int.Parse(row["NumberOfService"].ToString()) - int.Parse(row["CompletedServiceCount"].ToString())).ToString();
                ltrVisitPerYear.Text = row["VisitPerYear"].ToString();
                ltrEmp.Text = row["EmployeeName"].ToString();
            }
        }

        [System.Web.Services.WebMethod]
        public static string UpdateUnitName(string unitname, int uid)
        {
            var finalstring = string.Empty;
            IClientUnitService objClientService = ServiceFactory.ClientUnitService;
            DataTable dtClientServiceDetails = new DataTable();
            int ClientId = (HttpContext.Current.Session["ClientLoginCookie"] as LoginModel).Id;
            //int UnitId = int.Parse(HttpContext.Current.Request.QueryString["uid"].ToString());
            DataTable dtUnits = new DataTable();
            objClientService = ServiceFactory.ClientUnitService;
            objClientService.GetClientUnitsByClientIdUnitName(ClientId, unitname, ref dtUnits);
            var unitExists = dtUnits.Rows.Count;

            if (unitExists > 0)
            {
                if (dtUnits.Rows[0]["Id"].ToString() == (uid).ToString())
                {
                    objClientService.UpdateClientUnitNamePortal(uid, unitname, ClientId, General.UserRoles.Client.GetEnumValue(), DateTime.UtcNow);
                    return "Unit Name Updated.";
                }
                else
                {
                    return "Unit Name Already Exists.";
                }
            }
            else
            {
                objClientService.UpdateClientUnitNamePortal(uid, unitname, ClientId, General.UserRoles.Client.GetEnumValue(), DateTime.UtcNow);
                return "Unit Name Updated.";
            }
        }
    }
}