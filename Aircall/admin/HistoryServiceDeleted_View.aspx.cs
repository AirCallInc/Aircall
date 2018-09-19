using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Services;
using System.Data;

namespace Aircall.admin
{
    public partial class HistoryServiceDeleted_View : System.Web.UI.Page
    {
        IRequestServicesService objRequestServicesService;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["ServiceId"]))
                {
                    long ReqServiceId = Convert.ToInt64(Request.QueryString["ServiceId"].ToString());
                    objRequestServicesService = ServiceFactory.RequestServicesService;
                    DataTable dtReqServices = new DataTable();
                    objRequestServicesService.GetRequestedServiceById(ReqServiceId, ref dtReqServices);
                    if (dtReqServices.Rows.Count>0)
                    {
                        lblServiceCaseNo.Text = dtReqServices.Rows[0]["ServiceCaseNumber"].ToString();
                        lblClientName.Text = dtReqServices.Rows[0]["ClientName"].ToString();
                        lblAddress.Text = dtReqServices.Rows[0]["ClientAddress"].ToString();
                        lblMobile.Text = dtReqServices.Rows[0]["MobileNumber"].ToString();
                        lblHome.Text = dtReqServices.Rows[0]["HomeNumber"].ToString();
                        lblOffice.Text = dtReqServices.Rows[0]["OfficeNumber"].ToString();
                        lblUnitRequested.Text = dtReqServices.Rows[0]["RequestedUnits"].ToString();//
                        lblServiceReq.Text = Convert.ToDateTime(dtReqServices.Rows[0]["ServiceRequestedOn"].ToString()).ToString("MM/dd/yyyy");
                        lblServiceReqTime.Text = dtReqServices.Rows[0]["ServiceRequestedTime"].ToString();
                        lblPurposeofVisit.Text = dtReqServices.Rows[0]["PurposeOfVisit"].ToString();
                        lblNotes.Text = dtReqServices.Rows[0]["Notes"].ToString();
                        lblDeleted.Text = dtReqServices.Rows[0]["DeletedByClient"].ToString();
                    }
                }
            }
        }
    }
}