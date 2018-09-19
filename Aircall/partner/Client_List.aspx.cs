using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using Services;
using System.Web.UI.WebControls;
using System.Data;
using Aircall.Common;

namespace Aircall.partner
{
    public partial class Client_List : System.Web.UI.Page
    {
        IPartnerService objPartnerService; 
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["PartnerLoginCookie"] != null)
                {
                    BindClientByAffiliateId();
                }
                else
                {
                    Response.Redirect(Application["SiteAddress"] + "partner/Login.aspx");
                }   
            }

        }

        private void BindClientByAffiliateId()
        {
            objPartnerService = ServiceFactory.PartnerService;
            DataTable dtclient = new DataTable();
            int PartnerId = (Session["PartnerLoginCookie"] as LoginModel).Id;
            objPartnerService.GetClientListByPartnerId(PartnerId, ref dtclient);
            if (dtclient.Rows.Count > 0)
            {
                lstPartnerClient.DataSource = dtclient;
            }
            lstPartnerClient.DataBind();
        }
    }
}