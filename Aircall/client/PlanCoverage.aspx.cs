using Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Aircall.client
{
    public partial class PlanCoverage : System.Web.UI.Page
    {
        IPlanService objPlanService;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["ClientLoginCookie"] == null)
            {
                Response.Redirect(Application["SiteAddress"] + "sign-in.aspx");
            }

            DataTable dt = new DataTable();
            objPlanService = ServiceFactory.PlanService;

            objPlanService.GetAllPlanForFrontEnd(ref dt);

            lstSubscriptionPlans.DataSource = dt;
            lstSubscriptionPlans.DataBind();
        }

        protected void lstPlans_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                DataRow row = (e.Item.DataItem as DataRowView).Row;
                Image imgPlan = e.Item.FindControl("imgPlan") as Image;
                HtmlAnchor aPlan = e.Item.FindControl("aPlan") as HtmlAnchor;
                Literal ltrPlan = e.Item.FindControl("ltrPlan") as Literal;
                aPlan.HRef = "plan-coverage-detail.aspx?id=" + row["PlanTypeId"].ToString();
                imgPlan.ImageUrl = ConfigurationManager.AppSettings["PlanImageURL"].ToString() + row["Image"].ToString();
                ltrPlan.Text = row["Name"].ToString();

                aPlan.Style.Add("background-color", row["BackGroundColorHGS"].ToString());


            }
        }
    }
}