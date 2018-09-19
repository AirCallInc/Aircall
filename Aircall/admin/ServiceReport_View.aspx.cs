using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Services;
using System.Data;
using System.Net;
using System.Text;
using System.IO;
using Aircall.Common;

namespace Aircall.admin
{
    public partial class ServiceReport_View : System.Web.UI.Page
    {
        IServiceReportService objServiceReportService;
        IServiceReportImagesService objServiceReportImagesService;
        IServiceReportUnitsService objServiceReportUnitsService;
        IServicePartListService objServicePartListService;
        IEmployeePartRequestService objEmployeePartRequestService;
        public long ServiceId;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["ReportId"]))
                {
                    BindReports();
                }
            }
        }

        private void BindReports()
        {
            long ReportId = Convert.ToInt64(Request.QueryString["ReportId"].ToString());
            objServiceReportService = ServiceFactory.ServiceReportService;
            objServiceReportUnitsService = ServiceFactory.ServiceReportUnitsService;
            DataTable dtServiceReportUnits = new DataTable();
            DataTable dtServiceReport = new DataTable();
            objServiceReportService.GetServiceReportById(ReportId, ref dtServiceReport);
            if (dtServiceReport.Rows.Count > 0)
            {
                ServiceId = Convert.ToInt64(dtServiceReport.Rows[0]["ServiceId"].ToString());
                lblServiceReport.Text = dtServiceReport.Rows[0]["ServiceReportNumber"].ToString();
                lblContactName.Text = dtServiceReport.Rows[0]["ClientName"].ToString();
                ltrCompany.Text = dtServiceReport.Rows[0]["Company"].ToString()??"-";
                lblAddress.Text = dtServiceReport.Rows[0]["ClientAddress"].ToString();
                lblPurpose.Text = dtServiceReport.Rows[0]["PurposeOfVisit"].ToString();
                //lblBillingT.Text = dtServiceReport.Rows[0]["BillingType"].ToString();
                lblDate.Text = Convert.ToDateTime(dtServiceReport.Rows[0]["ScheduleDate"].ToString()).ToString("MM/dd/yyyy");
                ltrBillingType.Text = dtServiceReport.Rows[0]["BillingType"].ToString();
                lblTechnician.Text = dtServiceReport.Rows[0]["EmployeeName"].ToString();
                ltrAssignedTotalTime.Text = dtServiceReport.Rows[0]["AssignedTotalTime"].ToString() + " Minutes";
                ltrAssignedStart.Text = dtServiceReport.Rows[0]["ScheduleStartTime"].ToString();
                ltrAssignEnd.Text = dtServiceReport.Rows[0]["ScheduleEndTime"].ToString();
                lblTimeS.Text = dtServiceReport.Rows[0]["WorkStartedTime"].ToString();
                lblTimeC.Text = dtServiceReport.Rows[0]["WorkCompletedTime"].ToString();
                ltrExtra.Text = dtServiceReport.Rows[0]["ExtraTime"].ToString() == "0" ? "" : dtServiceReport.Rows[0]["ExtraTime"].ToString() + " Minutes";
                lblUnitServiced.Text = dtServiceReport.Rows[0]["ReportUnits"].ToString();
                lblWorkPerformed.Text = dtServiceReport.Rows[0]["WorkPerformed"].ToString();
                objServiceReportUnitsService.GetServiceReportUnitsByReportId(ReportId, ref dtServiceReportUnits);
                if (dtServiceReportUnits.Rows.Count > 0)
                {
                    lstServiceUnitsUsed.DataSource = dtServiceReportUnits;
                    lstServiceUnitsNotUsed.DataSource = dtServiceReportUnits;
                }
                lstServiceUnitsUsed.DataBind();
                lstServiceUnitsNotUsed.DataBind();

                //lblMaterialU.Text = dtServiceReport.Rows[0]["MaterialUsed"].ToString();
                //lblMaterialNU.Text = dtServiceReport.Rows[0]["MaterialNotUsed"].ToString();
                //lblPartReq.Text = dtServiceReport.Rows[0]["PartName"].ToString();
                objEmployeePartRequestService = ServiceFactory.EmployeePartRequestService;
                DataTable dtUnits = new DataTable();
                objEmployeePartRequestService.GetPartRequestUnitsByReportId(ReportId, ref dtUnits);
                if (dtUnits.Rows.Count > 0)
                {
                    lstRequestPartUnits.DataSource = dtUnits;
                    lstRequestPartUnits.DataBind();
                }

                lblRecommen.Text = dtServiceReport.Rows[0]["Recommendationsforcustomer"].ToString();
                lblEmailToC.Text = dtServiceReport.Rows[0]["Email"].ToString();
                hdnClientEmail.Value = dtServiceReport.Rows[0]["Email"].ToString();
                objServiceReportImagesService = ServiceFactory.ServiceReportImagesService;
                DataTable dtImage = new DataTable();
                objServiceReportImagesService.GetServiceReportImagesByReportId(ReportId, ref dtImage);
                if (dtImage.Rows.Count > 0)
                {
                    lstimage.DataSource = dtImage;
                    lstimage.DataBind();
                }
                ClientSig.ImageUrl = Application["SiteAddress"] + "/uploads/clientSignature/" + dtServiceReport.Rows[0]["ClientSignature"].ToString();
                ltrEmployeeNote.Text = dtServiceReport.Rows[0]["EmployeeNotes"].ToString();
            }
        }

        protected void btnrespond_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hdnClientEmail.Value)
                && !string.IsNullOrEmpty(Request.QueryString["ReportId"].ToString()))
            {
                long ReportId = Convert.ToInt64(Request.QueryString["ReportId"].ToString());
                var request = (HttpWebRequest)WebRequest.Create(Application["APIURL"] + "api/v1/employee/ResendReport1?" + "ReportId=" + ReportId + "&CCEmail=" + txtCCEmail.Text.Trim());

                var postData = "ReportId=" + ReportId + "&CCEmail=" + txtCCEmail.Text.Trim();
                var data = Encoding.ASCII.GetBytes(postData);

                request.Method = "GET";
                request.ContentType = "application/json";

                //using (var stream = request.GetRequestStream())
                //{
                //    stream.Write(data, 0, data.Length);
                //}

                var response = (HttpWebResponse)request.GetResponse();

                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                if (responseString.Contains("200"))
                {
                    dvMessage.InnerHtml = "<strong>Report Sent Successfully.</strong>";
                    dvMessage.Attributes.Add("class", "alert alert-Success");
                    dvMessage.Visible = true;
                    BindReports();
                    return;
                }
            }
        }

        protected void lstServiceUnitsUsed_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                if (ServiceId > 0)
                {
                    ListViewDataItem currentItem = (ListViewDataItem)e.Item;
                    HiddenField hdnUnitIdUsed = (HiddenField)currentItem.FindControl("hdnUnitIdUsed");
                    objServicePartListService = ServiceFactory.ServicePartListService;
                    DataTable dtServiceParts = new DataTable();
                    objServicePartListService.GetServicePartlistByServiceId(ServiceId, ref dtServiceParts);
                    DataTable dtServiceUsedParts = new DataTable();
                    if (dtServiceParts.Rows.Count > 0)
                    {
                        string filter = "UnitId=" + hdnUnitIdUsed.Value + " AND IsUsed=1";
                        DataView dv = new DataView(dtServiceParts, filter, "", DataViewRowState.CurrentRows);
                        dtServiceUsedParts = dv.ToTable();
                        if (dtServiceUsedParts.Rows.Count > 0)
                        {
                            ListView lstMaterialUsed = (ListView)currentItem.FindControl("lstMaterialUsed");
                            lstMaterialUsed.DataSource = dtServiceUsedParts;
                            lstMaterialUsed.DataBind();
                        }
                    }
                }
            }
        }

        protected void lstServiceUnitsNotUsed_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                if (ServiceId > 0)
                {
                    ListViewDataItem currentItem = (ListViewDataItem)e.Item;
                    HiddenField hdnUnitIdNotUsed = (HiddenField)currentItem.FindControl("hdnUnitIdNotUsed");
                    objServicePartListService = ServiceFactory.ServicePartListService;
                    DataTable dtServiceParts = new DataTable();
                    objServicePartListService.GetServicePartlistByServiceId(ServiceId, ref dtServiceParts);
                    DataTable dtServiceNotUsedParts = new DataTable();
                    if (dtServiceParts.Rows.Count > 0)
                    {
                        string filter = "UnitId=" + hdnUnitIdNotUsed.Value + " AND IsUsed=0";
                        DataView dv = new DataView(dtServiceParts, filter, "", DataViewRowState.CurrentRows);
                        dtServiceNotUsedParts = dv.ToTable();
                        if (dtServiceNotUsedParts.Rows.Count > 0)
                        {
                            ListView lstMaterialNotUsed = (ListView)currentItem.FindControl("lstMaterialNotUsed");
                            lstMaterialNotUsed.DataSource = dtServiceNotUsedParts;
                            lstMaterialNotUsed.DataBind();
                        }
                    }
                }
            }
        }

        protected void lstRequestPartUnits_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                ListViewDataItem currentitem = (ListViewDataItem)e.Item;
                HiddenField hdnUnitId = (HiddenField)currentitem.FindControl("hdnUnitId");
                if (!string.IsNullOrEmpty(Request.QueryString["ReportId"]))
                {
                    objEmployeePartRequestService=ServiceFactory.EmployeePartRequestService;
                    DataTable dtParts= new DataTable();
                    objEmployeePartRequestService.GetPartsByReportAndUnitId(Convert.ToInt64(Request.QueryString["ReportId"].ToString()), Convert.ToInt32(hdnUnitId.Value), ref dtParts);
                    if (dtParts.Rows.Count>0)
                    {
                        ListView lstRequestedPart = (ListView)currentitem.FindControl("lstRequestedPart");
                        lstRequestedPart.DataSource = dtParts;
                        lstRequestedPart.DataBind();
                    }
                }
            }
        }
    }
}