using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Services;
using System.Data;
using System.Web.UI.HtmlControls;

namespace Aircall.admin
{
    public partial class Plan_List : System.Web.UI.Page
    {
        IPlanService objPlanService;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindPlans();
            }
        }

        private void BindPlans()
        {
            objPlanService = ServiceFactory.PlanService;
            DataTable dtPlans = new DataTable();
            objPlanService.GetAllPlan(ref dtPlans);
            if (dtPlans.Rows.Count > 0)
            {
                lstPlan.DataSource = dtPlans;
            }
            lstPlan.DataBind();
        }

        protected void Page_PreRender(object sender, System.EventArgs e)
        {
            
        }
    }
}