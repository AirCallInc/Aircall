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
    public partial class HistoryService_View : System.Web.UI.Page
    {
        IServicesService objServicesService;
        IClientAddressService objClientAddressService;
        IServiceReportService objServiceReportService;
        IServiceRatingReviewService objServiceRatingReviewService;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindUserDetailById();
                //if (!string.IsNullOrEmpty(Request.QueryString["ServiceId"]))
                //{
                //    long ServiceId = Convert.ToInt64(Request.QueryString["ServiceId"].ToString());
                //    DataTable dtService = new DataTable();
                //    objServicesService = ServiceFactory.ServicesService;
                //    objServicesService.GetServiceById(ServiceId, ref dtService);
                //    if (dtService.Rows.Count>0)
                //    {
                //        ltrServiceCase.Text = dtService.Rows[0]["ServiceCaseNumber"].ToString();
                //        int ClientId =Convert.ToInt32(dtService.Rows[0]["ClientId"].ToString());
                //        ltrClientName.Text = dtService.Rows[0]["ClientName"].ToString();
                //        int AddressId = Convert.ToInt32(dtService.Rows[0]["AddressID"].ToString());

                //        DataTable dtAddress = new DataTable();
                //        objClientAddressService = ServiceFactory.ClientAddressService;
                //        objClientAddressService.GetAddressById(AddressId, ref dtAddress);
                //        if (dtAddress.Rows.Count>0)
                //        {
                //            ltrAddress.Text = dtAddress.Rows[0]["Address"].ToString() + "," +
                //                dtAddress.Rows[0]["CityName"].ToString() + "," +
                //                dtAddress.Rows[0]["StateName"].ToString() + "," +
                //                dtAddress.Rows[0]["ZipCode"].ToString();
                //        }
                //        ltrMobile.Text = dtService.Rows[0]["MobileNumber"].ToString();
                //        ltrHome.Text = dtService.Rows[0]["HomeNumber"].ToString();
                //        ltrOffice.Text = dtService.Rows[0]["OfficeNumber"].ToString();
                //        ltrServiceReqOn.Text = "";
                //        ltrServiceDate.Text = dtService.Rows[0]["ScheduleDate"].ToString();
                //        ltrEmployee.Text = dtService.Rows[0]["EmployeeName"].ToString();
                //        ltrPurpose.Text = dtService.Rows[0]["PurposeOfVisit"].ToString();
                //        ltrServiceTime.Text = dtService.Rows[0]["ScheduleStartTime"].ToString() + " - " +
                //            dtService.Rows[0]["ScheduleEndTime"].ToString();
                //        ltrServiceReport.Text = "";

                //        objServiceReportService = ServiceFactory.ServiceReportService;
                //        DataTable dtServiceReport = new DataTable();
                //        if (dtServiceReport.Rows.Count>0)
                //        {
                //            ltrWork.Text = dtServiceReport.Rows[0]["WorkPerformed"].ToString();
                //            ltrRecommend.Text = dtServiceReport.Rows[0]["Recommendationsforcustomer"].ToString();
                //        }

                //        objServiceRatingReviewService = ServiceFactory.ServiceRatingReviewService;
                //        DataTable dtRatings = new DataTable();
                //        objServiceRatingReviewService.GetServiceRatingsByServiceId(ServiceId, ref dtRatings);
                //        if (dtRatings.Rows.Count>0)
                //        {
                //            ltrRating.Text = dtRatings.Rows[0]["Rate"].ToString();
                //            ltrReview.Text = dtRatings.Rows[0]["Review"].ToString();
                //            ltrNotes.Text = dtRatings.Rows[0]["EmployeNotes"].ToString();
                //        }
                //    }
                //}
            }
        }

        private void BindUserDetailById()
        {
            long ServiceId = Convert.ToInt64(Request.QueryString["ServiceId"].ToString());
            objServicesService = ServiceFactory.ServicesService;
            DataTable dtService = new DataTable();
            objServicesService.GetCompletedServiceById(ServiceId, ref dtService);
            if (dtService.Rows.Count > 0)
            {
                lblServiceCaseNo.Text = dtService.Rows[0]["ServiceCaseNumber"].ToString();
                lblClientName.Text = dtService.Rows[0]["ClientName"].ToString();
                lblAddress.Text = dtService.Rows[0]["ClientAddress"].ToString();
                lblMobile.Text = dtService.Rows[0]["MobileNumber"].ToString();
                lblHome.Text = dtService.Rows[0]["HomeNumber"].ToString();
                lblOffice.Text = dtService.Rows[0]["OfficeNumber"].ToString();
                lblUnitServiced.Text = dtService.Rows[0]["ServiceUnits"].ToString();
                //lblPackageName.Text = dtUser.Rows[0]["PackageName"].ToString();
                lblServiceReq.Text = Convert.ToDateTime(dtService.Rows[0]["ServiceRequestedDate"].ToString()).ToString("MM/dd/yyyy");
                lblServiceDate.Text = Convert.ToDateTime(dtService.Rows[0]["ScheduleDate"].ToString()).ToString("MM/dd/yyyy");
                lblTechnician.Text = dtService.Rows[0]["EmployeeName"].ToString();
                lblPurposeofVisit.Text = dtService.Rows[0]["PurposeOfVisit"].ToString();
                ltrAssignedTotalTime.Text = dtService.Rows[0]["AssignedTotalTime"].ToString();
                ltrAssignedStart.Text = dtService.Rows[0]["ScheduleStartTime"].ToString();
                ltrAssignEnd.Text = dtService.Rows[0]["ScheduleEndTime"].ToString();
                lblserviceSTime.Text = dtService.Rows[0]["StartTime"].ToString();
                lblServiceETime.Text = dtService.Rows[0]["EndTime"].ToString();
                ltrExtra.Text = dtService.Rows[0]["ExtraTime"].ToString() == "0" ? "" : dtService.Rows[0]["ExtraTime"].ToString() + " Minutes";

                objServiceReportService = ServiceFactory.ServiceReportService;
                DataTable dtServiceReport = new DataTable();
                objServiceReportService.GetServiceReportOfUnitsByServiceId(ServiceId, ref dtServiceReport);
                if (dtServiceReport.Rows.Count > 0)
                {
                    lstServicereport.DataSource = dtServiceReport;
                    lstServicereport.DataBind();
                }

                //lblServiceReport.Text = dtService.Rows[0]["ServiceReportNumber"].ToString();
                lblWorkPerformed.Text = dtService.Rows[0]["WorkPerformed"].ToString();
                lblRecommen.Text = dtService.Rows[0]["Recommendationsforcustomer"].ToString();
                dvRating.Attributes.Add("data-rate", dtService.Rows[0]["Rating"].ToString());
                lblReview.Text = dtService.Rows[0]["Review"].ToString();
                lblEmployeeNotes.Text = dtService.Rows[0]["EmployeNotes"].ToString();
            }
        }
    }
}