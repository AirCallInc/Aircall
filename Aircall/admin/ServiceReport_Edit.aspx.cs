using Aircall.Common;
using ImageResizer;
using Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Aircall.admin
{
    public partial class ServiceReport_Edit : System.Web.UI.Page
    {
        DataTable dtUnitServiced = new DataTable();
        IServiceReportService objServiceReportService;
        IClientAddressService objClientAddressService;
        IServiceUnitService objServiceUnitService;
        IServiceReportUnitsService objServiceReportUnitsService;
        IServicePartListService objServicePartListService;
        IPartsService objPartList;
        IServiceReportImagesService objServiceReportImagesService;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //FillPurposeOfVisitDropdown();
                FillBillingTypeDropdown();
                if (!string.IsNullOrEmpty(Request.QueryString["ReportId"]))
                {
                    BindServiceReportById();
                    FillPartsDropdown();
                }
            }
        }

        //private void FillPurposeOfVisitDropdown()
        //{
        //    var values = DurationExtensions.GetValues<General.PurposeOfVisit>();
        //    List<string> data = new List<string>();
        //    foreach (var item in values)
        //    {
        //        General.PurposeOfVisit p = (General.PurposeOfVisit)item;
        //        data.Add(p.GetEnumDescription());
        //    }
        //    ddlPurposeofVisit.DataSource = data;
        //    ddlPurposeofVisit.DataBind();
        //}
        private void BindServiceReportById()
        {
            int ReportId = Convert.ToInt32(Request.QueryString["ReportId"]);
            objServiceReportService = ServiceFactory.ServiceReportService;
            objServiceReportUnitsService = ServiceFactory.ServiceReportUnitsService;
            DataTable dtServiceReport = new DataTable();


            objServiceReportService.GetServiceReportById(ReportId, ref dtServiceReport);
            if (dtServiceReport.Rows.Count > 0)
            {
                ltrReportNo.Text = dtServiceReport.Rows[0]["ServiceReportNumber"].ToString();
                lblContactName.Text = dtServiceReport.Rows[0]["ClientName"].ToString();
                ltrCompany.Text = dtServiceReport.Rows[0]["Company"].ToString()??"-";
                hdnClientId.Value = dtServiceReport.Rows[0]["ClientId"].ToString();
                int ClientId = Convert.ToInt32(dtServiceReport.Rows[0]["ClientId"].ToString());

                lblTechnician.Text = dtServiceReport.Rows[0]["EmployeeName"].ToString();

                //GetClientAddressesByClientId(ClientId);
                //rblAddress.SelectedValue = dtServiceReport.Rows[0]["AddressID"].ToString();
                ltrAddress.Text = dtServiceReport.Rows[0]["ClientAddress"].ToString();

                //ddlPurposeofVisit.SelectedValue = dtServiceReport.Rows[0]["PurposeOfVisit"].ToString().Trim();
                ltrPurposeofVisit.Text = dtServiceReport.Rows[0]["PurposeOfVisit"].ToString().Trim();

                drpBilling.SelectedValue = dtServiceReport.Rows[0]["BillingType"].ToString().Trim();

                //txtScheduleOn.Text = Convert.ToDateTime(dtServiceReport.Rows[0]["ScheduleDate"].ToString()).ToString("MM/dd/yyyy");
                ltrScheduleOn.Text = Convert.ToDateTime(dtServiceReport.Rows[0]["ScheduleDate"].ToString()).ToString("MM/dd/yyyy");
                txtStart.Value = dtServiceReport.Rows[0]["WorkStartedTime"].ToString();
                txtEnd.Value = dtServiceReport.Rows[0]["WorkCompletedTime"].ToString();

                BindServiceUnits(Convert.ToInt64(dtServiceReport.Rows[0]["ServiceId"].ToString()));

                DataTable dtCompletedServiceUnits = new DataTable();
                objServiceReportUnitsService.GetCompletedServiceUnitsByReportId(ReportId, ref dtCompletedServiceUnits);

                for (int j = 0; j < dtCompletedServiceUnits.Rows.Count; j++)
                {
                    for (int i = 0; i < cblUnit.Items.Count; i++)
                    {
                        if (cblUnit.Items[i].Value.ToString() == dtCompletedServiceUnits.Rows[j]["UnitId"].ToString())
                            cblUnit.Items[i].Selected = true;
                    }
                }
                txtWorkedP.Text = dtServiceReport.Rows[0]["WorkPerformed"].ToString();


                objServicePartListService = ServiceFactory.ServicePartListService;
                DataTable dtServiceParts = new DataTable();
                objServicePartListService.GetServicePartlistByServiceId(Convert.ToInt64(dtServiceReport.Rows[0]["ServiceId"].ToString()), ref dtServiceParts);
                if (dtServiceParts.Rows.Count > 0)
                {
                    cblMaterialU.DataSource = dtServiceParts;
                    cblMaterialU.DataTextField = "Name";
                    cblMaterialU.DataValueField = "ServicePartListId";
                }
                cblMaterialU.DataBind();

                string filter = "IsUsed=True";
                DataView dv = new DataView(dtServiceParts, filter, "", DataViewRowState.CurrentRows);
                DataTable dtMaterialUsed = dv.ToTable();

                for (int j = 0; j < dtMaterialUsed.Rows.Count; j++)
                {
                    ListItem item = cblMaterialU.Items.FindByValue(dtMaterialUsed.Rows[j]["ServicePartListId"].ToString());
                    item.Selected = true;
                }

                cblMaterialU.Enabled = false;

                objServiceReportImagesService = ServiceFactory.ServiceReportImagesService;
                DataTable dtPictures = new DataTable();
                objServiceReportImagesService.GetServiceReportImagesByReportId(ReportId, ref dtPictures);
                if (dtPictures.Rows.Count>0)
                {
                    lstPicture.DataSource = dtPictures;
                }
                lstPicture.DataBind();
                hdnPictureCount.Value = dtPictures.Rows.Count.ToString();

                txtRecomm.Text = dtServiceReport.Rows[0]["Recommendationsforcustomer"].ToString();

                if (!string.IsNullOrEmpty(dtServiceReport.Rows[0]["ClientSignature"].ToString()))
                {
                    lnkclientsig.HRef = Application["SiteAddress"] + "uploads/clientSignature/" + dtServiceReport.Rows[0]["ClientSignature"].ToString();
                    lnkclientsig.Visible = true;
                }
                hdnclientsig.Value = dtServiceReport.Rows[0]["ClientSignature"].ToString();

                txtEmployeeNot.Text = dtServiceReport.Rows[0]["EmployeeNotes"].ToString();
                txtEmailC.Text = dtServiceReport.Rows[0]["Email"].ToString();
                txtEmailC.Enabled = false;
                txtCCEmail.Text = dtServiceReport.Rows[0]["CCEmail"].ToString();
            }
        }

        private void BindServiceUnits(long ServiceId)
        {
            objServiceUnitService = ServiceFactory.ServiceUnitService;
            objServiceUnitService.GetServiceUnitByServiceId(ServiceId, ref dtUnitServiced);
            cblUnit.DataSource = dtUnitServiced;
            cblUnit.DataTextField = "UnitName";
            cblUnit.DataValueField = "UnitId";
            cblUnit.DataBind();
        }

        private void FillBillingTypeDropdown()
        {
            var Biling = DurationExtensions.GetValues<General.ReportBilling>();
            List<string> BillingList = new List<string>();
            foreach (var item in Biling)
            {
                General.ReportBilling p = (General.ReportBilling)item;
                BillingList.Add(p.GetEnumDescription());
            }
            drpBilling.DataSource = BillingList;
            drpBilling.DataBind();
        }

        //private void GetClientAddressesByClientId(int ClientId)
        //{
        //    DataTable dtClientAddress = new DataTable();
        //    objClientAddressService = ServiceFactory.ClientAddressService;
        //    objClientAddressService.GetClientAddressesByClientId(ClientId, true,ref dtClientAddress);
        //    if (dtClientAddress.Rows.Count > 0)
        //    {
        //        rblAddress.DataSource = dtClientAddress;
        //        rblAddress.DataTextField = "ClientAddress";
        //        rblAddress.DataValueField = "Id";
        //    }
        //    rblAddress.DataBind();
        //}

        protected void btncancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("ServiceReport_List.aspx");
        }

        private void FillPartsDropdown()
        {
            objPartList = ServiceFactory.PartsService;
            DataTable dtpartList = new DataTable();
            objPartList.GetAllParts(true, ref dtpartList);
            if (dtpartList.Rows.Count > 0)
            {
                drpParts.DataSource = dtpartList;
                drpParts.DataValueField = dtpartList.Columns["Id"].ToString();
                drpParts.DataTextField = dtpartList.Columns["Name"].ToString();
            }
            drpParts.DataBind();
        }

        protected void btnrespond_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    if (Session["LoginSession"] != null &&
                        !string.IsNullOrEmpty(Request.QueryString["ReportId"]))
                    {
                        LoginModel objLoginModel = new LoginModel();
                        objLoginModel = Session["LoginSession"] as LoginModel;
                        string[] AllowedFileExtensions = new string[] { ".jpg", ".gif", ".png", ".jpeg" };

                        BizObjects.ServiceReport objServiceReport = new BizObjects.ServiceReport();

                        TimeSpan StartTime = new TimeSpan();
                        StartTime = DateTime.ParseExact(txtStart.Value.Trim(), "hh:mm tt", CultureInfo.CurrentCulture).TimeOfDay;
                        TimeSpan EndTime = new TimeSpan();
                        EndTime = DateTime.ParseExact(txtEnd.Value.Trim(), "hh:mm tt", CultureInfo.CurrentCulture).TimeOfDay;

                        if (EndTime.Ticks < StartTime.Ticks)
                        {
                            dvMessage.InnerHtml = "<strong>Time completed work must be Greater than Time started work.</strong>";
                            dvMessage.Attributes.Add("class", "alert alert-error");
                            dvMessage.Visible = true;
                            return;
                        }

                        if (fpclientsig.HasFile)
                        {
                            
                            if (!AllowedFileExtensions.Contains(fpclientsig.FileName.Substring(fpclientsig.FileName.LastIndexOf('.'))))
                            {
                                dvMessage.InnerHtml = "<strong>Please select file of type: " + string.Join(", ", AllowedFileExtensions) + "</strong>";
                                dvMessage.Attributes.Add("class", "alert alert-error");
                                dvMessage.Visible = true;
                                return;
                            }
                            else
                            {
                                string filenameOriginal = DateTime.Now.Ticks.ToString().Trim() + "ClientSignature";
                                string fileName = fpclientsig.FileName;
                                string extension = System.IO.Path.GetExtension(fileName);
                                string extensionwithoutdot = extension.Remove(0, 1);

                                Instructions rsiphnWxH = new Instructions();
                                rsiphnWxH.Width = 175;
                                //rsiphnWxH.Height = 29;
                                rsiphnWxH.Mode = FitMode.Stretch;
                                rsiphnWxH.Format = extensionwithoutdot;

                                ImageJob imjob = new ImageJob(fpclientsig.PostedFile.InputStream, Server.MapPath("~/uploads/clientSignature/" + filenameOriginal + extension), rsiphnWxH);
                                imjob.CreateParentDirectory = true;
                                imjob.Build();
                                objServiceReport.ClientSignature = filenameOriginal + extension;
                            }
                        }
                        else
                        {
                            objServiceReport.ClientSignature = hdnclientsig.Value;
                        }

                        string ServicePictures = string.Empty;
                        if (fpdPicture.HasFiles)
                        {
                            if(fpdPicture.PostedFiles.Count + Convert.ToInt32(hdnPictureCount.Value.ToString()) > 16)
                            {
                                dvMessage.InnerHtml = "<strong>You can upload max 15 Picture of service report.</strong>";
                                dvMessage.Attributes.Add("class", "alert alert-error");
                                dvMessage.Visible = true;
                                return;
                            }
                            foreach (var item in fpdPicture.PostedFiles)
                            {
                                if (!AllowedFileExtensions.Contains(item.FileName.Substring(item.FileName.LastIndexOf('.'))))
                                {
                                    dvMessage.InnerHtml = "<strong>Please select file of type: " + string.Join(", ", AllowedFileExtensions) + "</strong>";
                                    dvMessage.Attributes.Add("class", "alert alert-error");
                                    dvMessage.Visible = true;
                                    return;
                                }

                            }
                            foreach (var item in fpdPicture.PostedFiles)
                            {
                                string filenameOriginal = DateTime.Now.Ticks.ToString().Trim() + "Images";
                                string fileName = item.FileName;
                                string extension = System.IO.Path.GetExtension(fileName);
                                string extensionwithoutdot = extension.Remove(0, 1);
                                string filePath = Path.Combine(Server.MapPath("~/uploads/reportimage/"), filenameOriginal + extension);
                                
                                item.SaveAs(filePath);
                                
                                filePath = Path.Combine(Server.MapPath("~/uploads/reportimage/Email/"), filenameOriginal + extension);
                                
                                Instructions rsiphnWxH = new Instructions();
                                rsiphnWxH.Width = 175;
                                rsiphnWxH.Mode = FitMode.Stretch;
                                rsiphnWxH.Format = extensionwithoutdot;

                                ImageJob imjob = new ImageJob(item.InputStream, Server.MapPath("~/uploads/reportimage/Email/" + filenameOriginal + extension), rsiphnWxH);
                                imjob.CreateParentDirectory = true;
                                imjob.Build();

                                if (string.IsNullOrEmpty(ServicePictures))
                                    ServicePictures = filenameOriginal + extension;
                                else
                                    ServicePictures = ServicePictures + '|' + filenameOriginal + extension;
                            }
                        }

                        /////// servicereport 
                        objServiceReportService = ServiceFactory.ServiceReportService;
                        long ReportId = Convert.ToInt64(Request.QueryString["ReportId"].ToString());

                        objServiceReport.Id = ReportId;
                        objServiceReport.BillingType = drpBilling.SelectedValue.ToString();
                        objServiceReport.WorkStartedTime = Convert.ToString(txtStart.Value);
                        objServiceReport.WorkCompletedTime = Convert.ToString(txtEnd.Value);
                        objServiceReport.WorkPerformed = txtWorkedP.Text.ToString().Trim();
                        objServiceReport.IsEmailToClient = chksendEmail.Checked;
                        objServiceReport.Recommendationsforcustomer = txtRecomm.Text.ToString().Trim();
                        objServiceReport.EmployeeNotes = txtEmployeeNot.Text.ToString().Trim();
                        objServiceReport.CCEmail = txtCCEmail.Text.ToString().Trim();
                        objServiceReport.UpdatedBy = objLoginModel.Id;
                        objServiceReport.UpdatedByType = objLoginModel.RoleId;
                        objServiceReport.UpdatedDate = DateTime.UtcNow;


                        objServiceReportService.UpdateServiceReport(ref objServiceReport);

                        /// service 
                        //BizObjects.Services objServices = new BizObjects.Services();
                        //objServices.AddressID = Convert.ToInt32(rblAddress.SelectedValue);
                        //objServices.Id = Convert.ToInt32(Request.QueryString["ServiceId"]);
                        //objServices.ScheduleDate = Convert.ToDateTime(txtScheduleOn.Text.ToString());
                        //objServices.PurposeOfVisit = Convert.ToString(ddlPurposeofVisit.SelectedItem);

                        objServiceReportUnitsService = ServiceFactory.ServiceReportUnitsService;
                        for (int i = 0; i < cblUnit.Items.Count; i++)
                        {
                            BizObjects.ServiceReportUnits objServiceReportUnits = new BizObjects.ServiceReportUnits();
                            int UnitId = Convert.ToInt32(cblUnit.Items[i].Value);
                            objServiceReportUnits.ServiceReportId = ReportId;
                            objServiceReportUnits.IsCompleted = cblUnit.Items[i].Selected;
                            objServiceReportUnits.UnitId = UnitId;
                            objServiceReportUnitsService.UpdateServiceReportIsCompletedByUnitAndReportId(ref objServiceReportUnits);
                        }

                        objServicePartListService = ServiceFactory.ServicePartListService;
                        for (int i = 0; i < cblMaterialU.Items.Count; i++)
                        {
                            int ServicePartListId = Convert.ToInt32(cblMaterialU.Items[i].Value);
                            objServicePartListService.UpdateIsUsedById(ServicePartListId, cblMaterialU.Items[i].Selected);
                        }

                        if (!string.IsNullOrEmpty(ServicePictures))
                        {
                            objServiceReportImagesService = ServiceFactory.ServiceReportImagesService;
                            BizObjects.ServiceReportImages objServiceReportImages = new BizObjects.ServiceReportImages();

                            foreach (var item in ServicePictures.Split('|'))
                            {
                                objServiceReportImages.ServiceReportId = ReportId;
                                objServiceReportImages.ServiceImage = item;
                                objServiceReportImagesService.AddServiceReportImages(ref objServiceReportImages);
                            }
                        }

                        if (chksendEmail.Checked)
                        {
                            //Write code of send email of service report to client
                            var request = (HttpWebRequest)WebRequest.Create(Application["APIURL"] + "api/v1/employee/ResendReport1?" + "ReportId=" + ReportId + "&CCEmail=" + txtCCEmail.Text.Trim());

                            request.Method = "GET";
                            request.ContentType = "application/json";
                            var response = (HttpWebResponse)request.GetResponse();

                            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                        }

                        if (chksendEmail.Checked)
                            Response.Redirect("ServiceReport_List.aspx?msg=sent");
                        else
                            Response.Redirect("ServiceReport_List.aspx?msg=edit");
                    }
                }
                catch (Exception Ex)
                {
                    dvMessage.InnerHtml = "<strong>Error!</strong> " + Ex.Message.ToString().Trim();
                    dvMessage.Attributes.Add("class", "alert alert-error");
                    dvMessage.Visible = true;
                }
            }
        }
    }
}