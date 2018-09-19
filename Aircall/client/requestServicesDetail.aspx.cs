using Aircall.Common;
using Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Aircall.client
{
    public partial class requestServicesDetail : System.Web.UI.Page
    {
        #region "Declaration"
        IClientAddressService objClientAddressService;
        IClientUnitService objClientUnitService;
        IRequestServicesService objRequestServicesService;
        IRequestServiceUnitsService objRequestServiceUnitsService;
        IPlanService objPlanService;
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["ClientLoginCookie"] != null)
            {
                if (Request.QueryString["rid"] != null)
                {
                    int ClientId = (Session["ClientLoginCookie"] as LoginModel).Id;

                    objRequestServicesService = ServiceFactory.RequestServicesService;
                    objRequestServiceUnitsService = ServiceFactory.RequestServiceUnitsService;

                    DataTable dtRequest = new DataTable();
                    int RequestId;// = int.Parse(Request.QueryString["rid"].ToString());                    
                    if (!int.TryParse(Request.QueryString["rid"], out RequestId))
                    {
                        Response.Redirect("request-service-list.aspx", false);
                    }
                    objRequestServicesService.GetRequestedServiceDetailsById(RequestId, (Session["ClientLoginCookie"] as LoginModel).Id, ref dtRequest);
                    if (dtRequest.Rows.Count > 0)
                    {
                        ltrServiceNo.Text = dtRequest.Rows[0]["ServiceCaseNumber"].ToString();
                        ltrAddress.Text = dtRequest.Rows[0]["Address"].ToString();

                        txtDate.Text = DateTime.Parse(dtRequest.Rows[0]["ServiceRequestedOn"].ToString()).Date.ToString("MM/dd/yyyy");
                        ltrTimeSlot.Text = dtRequest.Rows[0]["ServiceRequestedTime"].ToString();
                        ltrPurposeOfVisit.Text = dtRequest.Rows[0]["PurposeOfVisit"].ToString();
                        ltrPlan.Text = dtRequest.Rows[0]["PlanName"].ToString();
                        ltrUnits.Text = dtRequest.Rows[0]["Units"].ToString();
                        txtNotes.Text = dtRequest.Rows[0]["Notes"].ToString();
                    }
                    else
                    {
                        Response.Redirect(Application["SiteAddress"] + "/client/request-service-list.aspx", false);
                    }
                }
                else
                {
                    Response.Redirect(Application["SiteAddress"] + "/client/request-service-list.aspx", false);
                }
            }
            else
                Response.Redirect(Application["SiteAddress"] + "sign-in.aspx", false);
        }
    }
}