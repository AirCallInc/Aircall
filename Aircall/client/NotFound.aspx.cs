using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Services;
using System.Data;

namespace Aircall.client
{
    public partial class NotFound : System.Web.UI.Page
    {
        ICMSPagesService objCMSPagesService;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindTermsConditionPage();
            }
        }

        private void BindTermsConditionPage()
        {
            DataTable dtPage = new DataTable();
            objCMSPagesService = ServiceFactory.CMSPagesService;
            objCMSPagesService.GetCMSPageById(22, ref dtPage);
            if (dtPage.Rows.Count > 0)
            {
                ltrCMSPage.Text = dtPage.Rows[0]["Description"].ToString();
            }
        }
    }
}