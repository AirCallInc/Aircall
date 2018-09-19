using Aircall.Common;
using Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;

namespace Aircall.client
{
    public partial class past_services : System.Web.UI.Page
    {
        IServiceUnitService objServiceUnitService = ServiceFactory.ServiceUnitService;
        IServicesService objServicesService = ServiceFactory.ServicesService;
        DataTable dtResult = new DataTable();
        int ClientId = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["ClientLoginCookie"] != null)
            {
                ClientId = (Session["ClientLoginCookie"] as LoginModel).Id;
                objServicesService.GetServiceForClientByStatus(ClientId, General.ServiceTypes.Completed.GetEnumDescription(), ref dtResult);
                DataTable dtFinal = new DataTable();
                dtFinal = dtResult.Copy();

                DataTable dt = new DataTable();
                objServicesService.GetServiceForClientByStatus(ClientId, General.ServiceTypes.NoShow.GetEnumDescription(), ref dt);

                dtFinal.Merge(dt);

                lstSummary.DataSource = dtFinal;
                lstSummary.DataBind();
            }
            else
                Response.Redirect(Application["SiteAddress"] + "sign-in.aspx", false);

        }

        protected void lstSummary_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                Literal ltrServiceNo = e.Item.FindControl("ltrServiceNo") as Literal;
                Literal ltrUnitName = e.Item.FindControl("ltrUnitName") as Literal;
                //Literal ltrPlan = e.Item.FindControl("ltrPlan") as Literal;
                Literal ltrServiceDate = e.Item.FindControl("ltrServiceDate") as Literal;
                Literal ltrEmpName = e.Item.FindControl("ltrEmpName") as Literal;
                HtmlContainerControl dvRating = e.Item.FindControl("dvRating") as HtmlContainerControl;
                Image imgTechPer = e.Item.FindControl("imgTechPer") as Image;
                DataRow row = (e.Item.DataItem as DataRowView).Row;
                int ServiceId = int.Parse(row["Id"].ToString());
                ltrServiceNo.Text = row["ServiceCaseNumber"].ToString();
                ltrServiceDate.Text = row["ScheduleDate"].ToString().Replace(" ", "-");
                //ltrPlan.Text = row["PlanTypeName"].ToString() + " - " + row["PackageDisplayName"].ToString();
                //ltrUnitName.Text = row["UnitName"].ToString();
                ltrEmpName.Text = row["EmployeeName"].ToString();
                if (!string.IsNullOrWhiteSpace(row["Image"].ToString()))
                {
                    imgTechPer.ImageUrl = ConfigurationManager.AppSettings["EMPProfileImageURL"].ToString() + row["Image"].ToString();
                }
                else
                {
                    imgTechPer.ImageUrl = "images/place-holder-img.png";
                }
                dvRating.Attributes.Add("title", row["Rating"].ToString());

                DataTable dt = new DataTable();
                objServiceUnitService.GetServiceUnitsForPortal(ServiceId, ref dt);
                if (Convert.ToBoolean(row["IsNoShow"].ToString()))
                {
                    ltrUnitName.Text = General.ServiceTypes.NoShow.GetEnumDescription();
                }
                else
                {
                    ltrUnitName.Text = "&nbsp;";
                }
            }
        }
    }
}