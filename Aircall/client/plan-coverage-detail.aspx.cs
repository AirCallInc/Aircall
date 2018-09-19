using Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Aircall.client
{
    public partial class plan_coverage_detail : System.Web.UI.Page
    {
        IPlanService objPlanService;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["ClientLoginCookie"] == null)
            {
                Response.Redirect(Application["SiteAddress"] + "sign-in.aspx");
            }
            if (Request.QueryString["id"] == null)
            {
                Response.Redirect(Application["SiteAddress"] + "client/dashboard.aspx");
            }
            int RequestId;// = int.Parse(Request.QueryString["rid"].ToString());                    
            if (!int.TryParse(Request.QueryString["id"], out RequestId))
            {
                Response.Redirect("dashboard.aspx", false);
            }
            var PlanTypeId = int.Parse(Request.QueryString["id"]);
            DataTable dt = new DataTable();
            objPlanService = ServiceFactory.PlanService;
            objPlanService.GetPlanByPlanType(PlanTypeId, ref dt);

            DataRow PackageA = dt.Select(" PackageType=1")[0];
            DataRow PackageB = dt.Select(" PackageType=0")[0];
            ltrPlanName.Text = PackageA["Name"].ToString();
            ltrPackageNameA.Text = PackageA["PackageDisplayName"].ToString();
            ltrPackageNameB.Text = PackageB["PackageDisplayName"].ToString();

            ltrRateA.Text = PackageA["PricePerMonth"].ToString();
            ltrRateB.Text = PackageB["PricePerMonth"].ToString();

            ltrDescPackA.Text = PackageA["Description"].ToString();
            ltrDescPackB.Text = PackageB["Description"].ToString();

            //dvA.Style.Add("background-color", GenerateRgb(PackageA["BackGroundColorHGS"].ToString(), "1"));
            //dvB.Style.Add("background-color", GenerateRgb(PackageA["BackGroundColorHGS"].ToString(), "1"));

            //dvA1.Style.Add("background-color", GenerateRgb(PackageA["BackGroundColorHGS"].ToString(), "0.15"));
            //dvB1.Style.Add("background-color", GenerateRgb(PackageA["BackGroundColorHGS"].ToString(), "0.15"));

            //bigA.Style.Add("color", GenerateRgb(PackageA["BackGroundColorHGS"].ToString(), "1"));
            //bigB.Style.Add("color", GenerateRgb(PackageA["BackGroundColorHGS"].ToString(), "1"));

            StringBuilder sb = new StringBuilder();
            sb.Append("<style>");
            sb.Append(".plan-coverage-block .plan-coverage-single a{background-color: " + GenerateRgb(PackageA["BackGroundColorHGS"].ToString(), "1") + ";}");
            sb.Append(".pricing-coverage-block .pricing-table-block .pricing-table-title {background-color: " + GenerateRgb(PackageA["BackGroundColorHGS"].ToString(), "1") + "; }");
            sb.Append(".pricing-coverage-block  .pricing-table-block .pricing-table-price {background-color: " + GenerateRgb(PackageA["BackGroundColorHGS"].ToString(), "0.15") + "; }");
            sb.Append(".pricing-coverage-block  .pricing-table-block h5 { color: " + GenerateRgb(PackageA["BackGroundColorHGS"].ToString(), "1") + "; }");
            sb.Append(".pricing-coverage-block  .pricing-table-block .pricing-table-price big { color: " + GenerateRgb(PackageA["BackGroundColorHGS"].ToString(), "1") + "; }");
            sb.Append(".pricing-coverage-block  .pricing-table-block .pricing-table-features { color: " + GenerateRgb(PackageA["BackGroundColorHGS"].ToString(), "1") + "; }");
            //sb.Append(".pricing-coverage-block  .pricing-table-block .pricing-table-features h5, { color: " + GenerateRgb(PackageA["BackGroundColorHGS"].ToString(), "1") + "; }");
            sb.Append("</style>");
            
            ltrCSS.Text = sb.ToString();
        }
        public string GenerateRgb(string backgroundColor, string part)
        {
            Color color = ColorTranslator.FromHtml(backgroundColor);
            int r = Convert.ToInt16(color.R);
            int g = Convert.ToInt16(color.G);
            int b = Convert.ToInt16(color.B);
            return string.Format("rgba({0}, {1}, {2},{3});", r, g, b, part);
        }
    }
}