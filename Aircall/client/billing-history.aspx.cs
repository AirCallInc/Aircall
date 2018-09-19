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
    public partial class billing_history : System.Web.UI.Page
    {
        IBillingHistoryService objBillingHistoryService;
        protected void Page_Load(object sender, EventArgs e)
        {
            objBillingHistoryService = ServiceFactory.BillingHistoryService;
            if (Session["ClientLoginCookie"] != null)
            {
                if (!IsPostBack)
                {
                    BindBillingHistory();
                }
            }
            else
                Response.Redirect(Application["SiteAddress"] + "sign-in.aspx", false);
        }

        private void BindBillingHistory()
        {
            DataTable dt = new DataTable();
            int ClientId = (Session["ClientLoginCookie"] as LoginModel).Id;
            objBillingHistoryService.GetAllBillingHistoryByClientId(ClientId, ref dt);
            lstBilling.DataSource = dt;
            lstBilling.DataBind();

        }

        protected void dataPagerBilling_PreRender(object sender, EventArgs e)
        {
            dataPagerBilling.PageSize = int.Parse(General.GetSitesettingsValue("ClientPortalPageSize"));
            BindBillingHistory();
        }
    }
}