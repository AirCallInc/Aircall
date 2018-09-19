using ImageResizer;
using Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Stripe;
using Aircall.Common;
using System.Globalization;

namespace Aircall.admin
{
    public partial class Plan_AddEdit : System.Web.UI.Page
    {
        IPlanService objPlanService;
        IStripeErrorLogService objStripeErrorLogService;
        IStateService objStateService;
        ICitiesService objCitiesService;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtSortDesc.Attributes.Add("maxlength", txtSortDesc.MaxLength.ToString());
                FillStateDropdown();
                drpCountry.Enabled = false;
                drpState.Enabled = false;
                drpCity.Enabled = false;
                BindPlanTypeDropdown();
                if (!string.IsNullOrEmpty(Request.QueryString["PlanTypeId"]))
                {
                    BindPlans();
                }
            }
        }

        private void FillStateDropdown()
        {
            objStateService = ServiceFactory.StateService;
            DataTable dtStates = new DataTable();
            objStateService.GetAllStates(true, true,ref dtStates);
            if (dtStates.Rows.Count > 0)
            {
                drpState.DataSource = dtStates;
                drpState.DataTextField = dtStates.Columns["Name"].ToString();
                drpState.DataValueField = dtStates.Columns["Id"].ToString();
                drpState.DataBind();
                string filter = "Name='California'";
                DataView dv = new DataView(dtStates, filter, "", DataViewRowState.CurrentRows);
                DataTable dtStateRow = new DataTable();
                dtStateRow = dv.ToTable();
                if (dtStateRow.Rows.Count>0)
                {
                    drpState.SelectedValue = dtStateRow.Rows[0]["Id"].ToString();
                    BindCityFromState(Convert.ToInt32(dtStateRow.Rows[0]["Id"].ToString()));
                }
                    
            }
            drpState.DataBind();
        }

        private void BindCityFromState(int StateId)
        {
            objCitiesService = ServiceFactory.CitiesService;
            DataTable dtCities = new DataTable();
            objCitiesService.GetAllCityByStateId(StateId, true,ref dtCities);
            if (dtCities.Rows.Count > 0)
            {
                drpCity.DataSource = dtCities;
                drpCity.DataValueField = dtCities.Columns["Id"].ToString();
                drpCity.DataTextField = dtCities.Columns["Name"].ToString();
            }
            else
                drpCity.DataSource = "";
            drpCity.DataBind();
        }

        private void BindPlans()
        {
            btnSave.Visible = false;
            btnUpdate.Visible = true;
            drpPlanType.Enabled = false;

            int PlanTypeId = Convert.ToInt32(Request.QueryString["PlanTypeId"].ToString());
            objPlanService = ServiceFactory.PlanService;
            DataTable dtPlans = new DataTable();
            objPlanService.GetPlanByPlanType(PlanTypeId, ref dtPlans);
            if (dtPlans.Rows.Count > 0)
            {
                txtPlanName.Text = dtPlans.Rows[0]["Name"].ToString();
                drpPlanType.SelectedValue = dtPlans.Rows[0]["PlanTypeId"].ToString();
                txtSortDesc.Text = dtPlans.Rows[0]["ShortDescription"].ToString();
                txtPackageA.Value = dtPlans.Rows[0]["Description"].ToString();
                hdnPackageA.Value = dtPlans.Rows[0]["Id"].ToString();
                txtPackageADispName.Text = dtPlans.Rows[0]["PackageDisplayName"].ToString();
                txtPackageB.Value = dtPlans.Rows[1]["Description"].ToString();
                hdnPackageB.Value = dtPlans.Rows[1]["Id"].ToString();
                txtPackageBDispName.Text = dtPlans.Rows[1]["PackageDisplayName"].ToString();
                txtPrice.Text = dtPlans.Rows[0]["PricePerMonth"].ToString();
                hdnPrice.Value = dtPlans.Rows[0]["PricePerMonth"].ToString();
                txtDuration.Text = dtPlans.Rows[0]["DurationInMonth"].ToString();
                hdnDuration.Value = dtPlans.Rows[0]["DurationInMonth"].ToString();
                txtNoOfService.Text = dtPlans.Rows[0]["NumberOfService"].ToString();
                hdnService.Value = dtPlans.Rows[0]["NumberOfService"].ToString();
                txtFirstService.Text = dtPlans.Rows[0]["FirstServiceWithinDays"].ToString();
                txtOtherServiceGap.Text = dtPlans.Rows[0]["OtherServiceScheduleDaysGap"].ToString();
                txtDriveTime.Text = dtPlans.Rows[0]["Drivetime"].ToString();
                txtServiceTimeForFirstUnit.Text = dtPlans.Rows[0]["ServiceTimeForFirstUnit"].ToString();
                txtServiceTimeForAdditionalUnits.Text = dtPlans.Rows[0]["ServiceTimeForAdditionalUnits"].ToString();
                chkSpecial.Checked = Convert.ToBoolean(dtPlans.Rows[0]["ShowSpecialPrice"].ToString());
                chkAutorenewal.Checked = Convert.ToBoolean(dtPlans.Rows[0]["ShowAutoRenewal"].ToString());
                txtDiscount.Text = dtPlans.Rows[0]["DiscountPrice"].ToString();
                txtColor.Text = dtPlans.Rows[0]["BackGroundColorHGS"].ToString();
                if (!string.IsNullOrEmpty(dtPlans.Rows[0]["Image"].ToString()))
                {
                    lnkImage.HRef = Application["SiteAddress"] + "uploads/plan/" + dtPlans.Rows[0]["Image"].ToString();
                    lnkImage.Visible = true;
                }
                hdnImage.Value = dtPlans.Rows[0]["Image"].ToString();
                string[] TimeSlot1 = dtPlans.Rows[0]["ServiceSlot1"].ToString().Split('-');
                txtSlot1Start.Value = TimeSlot1[0].Trim();
                txtSlot1End.Value = TimeSlot1[1].Trim();
                string[] TimeSlot2 = dtPlans.Rows[0]["ServiceSlot2"].ToString().Split('-');
                txtSlot2Start.Value = TimeSlot2[0].Trim();
                txtSlot2End.Value = TimeSlot2[1].Trim();
                chkActive.Checked = Convert.ToBoolean(dtPlans.Rows[0]["Status"].ToString());
            }
        }

        private void BindPlanTypeDropdown()
        {
            objPlanService = ServiceFactory.PlanService;
            DataTable dtPlanTypes = new DataTable();
            //objPlanService.GetAllPlanType(ref dtPlanTypes);
            objPlanService.GetAllPlanTypeForPlan(ref dtPlanTypes);
            if (dtPlanTypes.Rows.Count > 0)
            {
                drpPlanType.DataSource = dtPlanTypes;
                drpPlanType.DataTextField = dtPlanTypes.Columns["Name"].ToString();
                drpPlanType.DataValueField = dtPlanTypes.Columns["Id"].ToString();
            }
            drpPlanType.DataBind();
            drpPlanType.Items.Insert(0, new ListItem("Select Plan Type", "0"));
        }

        public string GenerateRgb(string backgroundColor)
        {
            Color color = ColorTranslator.FromHtml(backgroundColor);
            int r = Convert.ToInt16(color.R);
            int g = Convert.ToInt16(color.G);
            int b = Convert.ToInt16(color.B);
            return string.Format("rgb({0}, {1}, {2});", r, g, b);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    if (Session["LoginSession"] != null)
                    {
                        if (string.IsNullOrEmpty(txtPackageA.Value) || string.IsNullOrEmpty(txtPackageB.Value))
                        {
                            dvMessage.InnerHtml = "<strong>Package A and Package B Decription is required.</strong>";
                            dvMessage.Attributes.Add("class", "alert alert-error");
                            dvMessage.Visible = true;
                            return;
                        }

                        TimeSpan duration = DateTime.Parse(txtSlot2End.Value.Trim()).Subtract(DateTime.Parse(txtSlot1Start.Value.Trim()));

                        TimeSpan Slot1StartTime = new TimeSpan();
                        Slot1StartTime = DateTime.ParseExact(txtSlot1Start.Value.Trim(), "hh:mm tt", CultureInfo.CurrentCulture).TimeOfDay;

                        TimeSpan Slot1EndTime = new TimeSpan();
                        Slot1EndTime = DateTime.ParseExact(txtSlot1End.Value.Trim(), "hh:mm tt", CultureInfo.CurrentCulture).TimeOfDay;

                        if (Slot1StartTime.Ticks >= Slot1EndTime.Ticks)
                        {
                            dvMessage.InnerHtml = "Service Slot1 End time must greater than Service Slot1 Start time.";
                            dvMessage.Attributes.Add("class", "alert alert-error");
                            dvMessage.Visible = true;
                            return;
                        }

                        TimeSpan Slot2StartTime = new TimeSpan();
                        Slot2StartTime = DateTime.ParseExact(txtSlot2Start.Value.Trim(), "hh:mm tt", CultureInfo.CurrentCulture).TimeOfDay;

                        TimeSpan Slot2EndTime = new TimeSpan();
                        Slot2EndTime = DateTime.ParseExact(txtSlot2End.Value.Trim(), "hh:mm tt", CultureInfo.CurrentCulture).TimeOfDay;

                        if (Slot2StartTime.Ticks >= Slot2EndTime.Ticks)
                        {
                            dvMessage.InnerHtml = "Service Slot2 End time must greater than Service Slot2 Start time.";
                            dvMessage.Attributes.Add("class", "alert alert-error");
                            dvMessage.Visible = true;
                            return;
                        }

                        if ((Convert.ToInt32(txtDriveTime.Text.Trim()) + Convert.ToInt32(txtServiceTimeForFirstUnit.Text.Trim()) + 60) > duration.TotalMinutes)
                        {
                            dvMessage.InnerHtml = "Drive time and Service Time For First Unit must be less than total time of Plan.";
                            dvMessage.Attributes.Add("class", "alert alert-error");
                            dvMessage.Visible = true;
                            return;
                        }

                        if (txtSortDesc.Text.Trim().Length > 256)
                        {
                            dvMessage.InnerHtml = "Sort Description is too long. Allow only 256 charactors.";
                            dvMessage.Attributes.Add("class", "alert alert-error");
                            dvMessage.Visible = true;
                            return;
                        }


                        LoginModel objLoginModel = new LoginModel();
                        objLoginModel = Session["LoginSession"] as LoginModel;

                        DataTable dtPlans = new DataTable();
                        objPlanService = ServiceFactory.PlanService;
                        objPlanService.GetPlanByPlanType(Convert.ToInt32(drpPlanType.SelectedValue.ToString()), ref dtPlans);
                        if (dtPlans.Rows.Count == 0)
                        {
                            BizObjects.Plans objPlans = new BizObjects.Plans();

                            if (fpPlanImage.HasFile)
                            {
                                string[] AllowedFileExtensions = new string[] { ".jpg", ".gif", ".png", ".jpeg" };
                                if (!AllowedFileExtensions.Contains(fpPlanImage.FileName.Substring(fpPlanImage.FileName.LastIndexOf('.'))))
                                {
                                    dvMessage.InnerHtml = "<strong>Please select file of type: " + string.Join(", ", AllowedFileExtensions) + "</strong>";
                                    dvMessage.Attributes.Add("class", "alert alert-error");
                                    dvMessage.Visible = true;
                                    return;
                                }
                                else
                                {
                                    string filenameOriginal = DateTime.UtcNow.Ticks.ToString().Trim();
                                    string fileName = fpPlanImage.FileName;
                                    string extension = System.IO.Path.GetExtension(fileName);
                                    string extensionwithoutdot = extension.Remove(0, 1);

                                    Instructions rsiphnWxH = new Instructions();
                                    rsiphnWxH.Width = 200;
                                    rsiphnWxH.Height = 200;
                                    rsiphnWxH.Mode = FitMode.Stretch;
                                    rsiphnWxH.Format = extensionwithoutdot;

                                    ImageJob imjob = new ImageJob(fpPlanImage.PostedFile.InputStream, Server.MapPath("~/uploads/plan/" + filenameOriginal + extension), rsiphnWxH);
                                    imjob.CreateParentDirectory = true;
                                    imjob.Build();
                                    objPlans.Image = filenameOriginal + extension;
                                }
                            }
                            //Create Stripe Plan
                            try
                            {
                                CreatePlanInStripe();
                            }
                            catch (StripeException stex)
                            {
                                BizObjects.StripeErrorLog objStripeErrorLog = new BizObjects.StripeErrorLog();
                                objStripeErrorLogService = ServiceFactory.StripeErrorLogService;
                                objStripeErrorLog.ChargeId = stex.StripeError.ChargeId;
                                objStripeErrorLog.Code = stex.StripeError.Code;
                                objStripeErrorLog.DeclineCode = stex.StripeError.DeclineCode;
                                objStripeErrorLog.ErrorType = stex.StripeError.ErrorType;
                                objStripeErrorLog.Error = stex.StripeError.Error;
                                objStripeErrorLog.ErrorSubscription = stex.StripeError.ErrorSubscription;
                                objStripeErrorLog.Message = stex.StripeError.Message;
                                objStripeErrorLog.Parameter = stex.StripeError.Parameter;
                                objStripeErrorLog.Userid = 0;
                                objStripeErrorLog.UnitId = 0;
                                objStripeErrorLog.DateAdded = DateTime.UtcNow;

                                objStripeErrorLogService.AddStripeErrorLog(ref objStripeErrorLog);

                                dvMessage.InnerHtml = stex.StripeError.Message.ToString();
                                dvMessage.Attributes.Add("class", "error");
                                dvMessage.Visible = true;
                                return;
                            }

                            //Package Type 1=PackageA 0=PackageB
                            bool PackageA = true;

                            for (int i = 1; i <= 2; i++)
                            {
                                objPlans.Name = txtPlanName.Text.Trim();
                                objPlans.PackageType = PackageA;
                                objPlans.ShortDescription = txtSortDesc.Text.Trim();
                                objPlans.PlanTypeId = Convert.ToInt32(drpPlanType.SelectedValue.ToString());

                                if (PackageA)
                                {
                                    objPlans.Description = txtPackageA.Value;
                                    objPlans.PackageDisplayName = txtPackageADispName.Text.Trim();
                                }
                                else
                                {
                                    objPlans.Description = txtPackageB.Value;
                                    objPlans.PackageDisplayName = txtPackageBDispName.Text.Trim();
                                }
                                    

                                objPlans.PricePerMonth = Convert.ToDecimal(txtPrice.Text.Trim());
                                objPlans.DurationInMonth = Convert.ToInt16(txtDuration.Text.Trim());
                                objPlans.NumberOfService = Convert.ToInt16(txtNoOfService.Text.Trim());
                                objPlans.FirstServiceWithinDays = Convert.ToInt16(txtFirstService.Text.Trim());
                                objPlans.OtherServiceScheduleDaysGap = Convert.ToInt32(txtOtherServiceGap.Text.Trim());
                                objPlans.Drivetime =Convert.ToInt32(txtDriveTime.Text.Trim());
                                objPlans.ServiceTimeForFirstUnit = Convert.ToInt32(txtServiceTimeForFirstUnit.Text.Trim());
                                objPlans.ServiceTimeForAdditionalUnits = Convert.ToInt32(txtServiceTimeForAdditionalUnits.Text.Trim());
                                objPlans.ShowSpecialPrice = chkSpecial.Checked;
                                objPlans.ShowAutoRenewal = chkAutorenewal.Checked;
                                objPlans.StripePlanId = txtPlanName.Text.Trim().Replace(' ', '-').ToLower() + '-' + Convert.ToDecimal(txtPrice.Text.Trim()).ToString() + '-' + txtDuration.Text.Trim();
                                if (!string.IsNullOrEmpty(txtDiscount.Text.Trim()))
                                    objPlans.DiscountPrice = Convert.ToDecimal(txtDiscount.Text.Trim());

                                objPlans.BackGroundColorHGS = txtColor.Text.Trim();
                                objPlans.BackGroundColorRGB = GenerateRgb(txtColor.Text.Trim());
                                objPlans.Status = chkActive.Checked;
                                objPlans.AddedBy = objLoginModel.Id;
                                objPlans.AddedByType = objLoginModel.RoleId;
                                objPlans.AddedDate = DateTime.UtcNow;
                                
                                objPlanService.AddPlan(ref objPlans);
                                PackageA = false;
                            }
                            string TimeSlot1=txtSlot1Start.Value.Trim() + " - " + txtSlot1End.Value.Trim();
                            string TimeSlot2=txtSlot2Start.Value.Trim() + " - " + txtSlot2End.Value.Trim();
                            objPlanService.UpdatePlanTypeTimeSlot(Convert.ToInt32(drpPlanType.SelectedValue.ToString()), TimeSlot1, TimeSlot2);
                            Session["msg"] = "add";
                            Response.Redirect(Application["SiteAddress"] + "admin/Plan_List.aspx");
                        }
                        else
                        {
                            dvMessage.InnerHtml = "<strong>Selected plan type already exists.</strong>";
                            dvMessage.Attributes.Add("class", "alert alert-error");
                            dvMessage.Visible = true;
                            return;
                        }
                    }
                    else
                    {
                        Response.Redirect(Application["SiteAddress"] + "/admin/Login.aspx");
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

        private void CreatePlanInStripe()
        {
            var PlanService = new StripePlanService();
            try
            {
                StripePlan Plan = PlanService.Get(txtPlanName.Text.Trim().Replace(' ', '-').ToLower() + '-' + Convert.ToDecimal(txtPrice.Text.Trim()).ToString() + '-' + txtDuration.Text.Trim());
            }
            catch (StripeException stex)
            {
                StripePlanCreateOptions objStripePlan = new StripePlanCreateOptions();
                objStripePlan.Amount = (int)(Convert.ToDecimal(txtPrice.Text) * 100);
                objStripePlan.Currency = "usd";
                objStripePlan.Id = txtPlanName.Text.Trim().Replace(' ', '-').ToLower() + '-' + Convert.ToDecimal(txtPrice.Text.Trim()).ToString() + '-' + txtDuration.Text.Trim();
                objStripePlan.Interval = "month";
                objStripePlan.Name = txtPlanName.Text.Trim().Replace(' ', '-').ToLower() + '-' + Convert.ToDecimal(txtPrice.Text.Trim()).ToString() + '-' + txtDuration.Text.Trim();
                PlanService.Create(objStripePlan);
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Request.QueryString["PlanTypeId"]))
            {
                int PlanTypeId = Convert.ToInt32(Request.QueryString["PlanTypeId"].ToString());

                if (string.IsNullOrEmpty(txtPackageA.Value) || string.IsNullOrEmpty(txtPackageB.Value))
                {
                    dvMessage.InnerHtml = "<strong>Package A and Package B Decription is required.</strong>";
                    dvMessage.Attributes.Add("class", "alert alert-error");
                    dvMessage.Visible = true;
                    return;
                }

                TimeSpan duration = DateTime.Parse(txtSlot2End.Value.Trim()).Subtract(DateTime.Parse(txtSlot1Start.Value.Trim()));

                TimeSpan Slot1StartTime = new TimeSpan();
                Slot1StartTime = DateTime.ParseExact(txtSlot1Start.Value.Trim(), "hh:mm tt", CultureInfo.CurrentCulture).TimeOfDay;

                TimeSpan Slot1EndTime = new TimeSpan();
                Slot1EndTime = DateTime.ParseExact(txtSlot1End.Value.Trim(), "hh:mm tt", CultureInfo.CurrentCulture).TimeOfDay;

                if (Slot1StartTime.Ticks >= Slot1EndTime.Ticks)
                {
                    dvMessage.InnerHtml = "Service Slot1 End time must greater than Service Slot1 Start time.";
                    dvMessage.Attributes.Add("class", "alert alert-error");
                    dvMessage.Visible = true;
                    return;
                }

                TimeSpan Slot2StartTime = new TimeSpan();
                Slot2StartTime = DateTime.ParseExact(txtSlot2Start.Value.Trim(), "hh:mm tt", CultureInfo.CurrentCulture).TimeOfDay;

                TimeSpan Slot2EndTime = new TimeSpan();
                Slot2EndTime = DateTime.ParseExact(txtSlot2End.Value.Trim(), "hh:mm tt", CultureInfo.CurrentCulture).TimeOfDay;

                if (Slot2StartTime.Ticks >= Slot2EndTime.Ticks)
                {
                    dvMessage.InnerHtml = "Service Slot2 End time must greater than Service Slot2 Start time.";
                    dvMessage.Attributes.Add("class", "alert alert-error");
                    dvMessage.Visible = true;
                    return;
                }

                if ((Convert.ToInt32(txtDriveTime.Text.Trim()) + Convert.ToInt32(txtServiceTimeForFirstUnit.Text.Trim()) + 60) > duration.TotalMinutes)
                {
                    dvMessage.InnerHtml = "Drive time and Service Time For First Unit must be less than total time of Plan.";
                    dvMessage.Attributes.Add("class", "alert alert-error");
                    dvMessage.Visible = true;
                    return;
                }

                if (txtSortDesc.Text.Trim().Length > 256)
                {
                    dvMessage.InnerHtml = "Sort Description is too long. Allow only 256 charactors.";
                    dvMessage.Attributes.Add("class", "alert alert-error");
                    dvMessage.Visible = true;
                    return;
                }

                LoginModel objLoginModel = new LoginModel();
                objLoginModel = Session["LoginSession"] as LoginModel;

                BizObjects.Plans objPlans = new BizObjects.Plans();

                if (fpPlanImage.HasFile)
                {
                    string[] AllowedFileExtensions = new string[] { ".jpg", ".gif", ".png", ".jpeg" };
                    if (!AllowedFileExtensions.Contains(fpPlanImage.FileName.Substring(fpPlanImage.FileName.LastIndexOf('.'))))
                    {
                        dvMessage.InnerHtml = "<strong>Please select file of type: " + string.Join(", ", AllowedFileExtensions) + "</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-error");
                        dvMessage.Visible = true;
                        return;
                    }
                    else
                    {
                        string filenameOriginal = DateTime.UtcNow.Ticks.ToString().Trim();
                        string fileName = fpPlanImage.FileName;
                        string extension = System.IO.Path.GetExtension(fileName);
                        string extensionwithoutdot = extension.Remove(0, 1);

                        Instructions rsiphnWxH = new Instructions();
                        rsiphnWxH.Width = 200;
                        rsiphnWxH.Height = 200;
                        rsiphnWxH.Mode = FitMode.Stretch;
                        rsiphnWxH.Format = extensionwithoutdot;

                        ImageJob imjob = new ImageJob(fpPlanImage.PostedFile.InputStream, Server.MapPath("~/uploads/plan/" + filenameOriginal + extension), rsiphnWxH);
                        imjob.CreateParentDirectory = true;
                        imjob.Build();
                        objPlans.Image = filenameOriginal + extension;
                    }
                }
                else
                {
                    objPlans.Image = hdnImage.Value;
                }

                //Stripe Plan Create If Price, Duration & No Of Service Changes
                if (Convert.ToDecimal(hdnPrice.Value)!=Convert.ToDecimal(txtPrice.Text) ||
                    Convert.ToInt16(hdnDuration.Value)!=Convert.ToInt16(txtDuration.Text) ||
                    Convert.ToInt16(hdnService.Value) != Convert.ToInt16(txtNoOfService.Text))
                {
                    try
                    {
                        CreatePlanInStripe();
                    }
                    catch (StripeException stex)
                    {
                        BizObjects.StripeErrorLog objStripeErrorLog = new BizObjects.StripeErrorLog();
                        objStripeErrorLogService = ServiceFactory.StripeErrorLogService;
                        objStripeErrorLog.ChargeId = stex.StripeError.ChargeId;
                        objStripeErrorLog.Code = stex.StripeError.Code;
                        objStripeErrorLog.DeclineCode = stex.StripeError.DeclineCode;
                        objStripeErrorLog.ErrorType = stex.StripeError.ErrorType;
                        objStripeErrorLog.Error = stex.StripeError.Error;
                        objStripeErrorLog.ErrorSubscription = stex.StripeError.ErrorSubscription;
                        objStripeErrorLog.Message = stex.StripeError.Message;
                        objStripeErrorLog.Parameter = stex.StripeError.Parameter;
                        objStripeErrorLog.Userid = 0;
                        objStripeErrorLog.UnitId = 0;
                        objStripeErrorLog.DateAdded = DateTime.UtcNow;

                        objStripeErrorLogService.AddStripeErrorLog(ref objStripeErrorLog);

                        dvMessage.InnerHtml = stex.StripeError.Message.ToString();
                        dvMessage.Attributes.Add("class", "error");
                        dvMessage.Visible = true;
                        return;
                    }
                }


                //Package Type 1=PackageA 0=PackageB
                bool PackageA = true;

                objPlanService = ServiceFactory.PlanService;

                for (int i = 1; i <= 2; i++)
                {
                    objPlans.Name = txtPlanName.Text.Trim();
                    objPlans.PackageType = PackageA;
                    objPlans.ShortDescription = txtSortDesc.Text.Trim();
                    objPlans.PlanTypeId = Convert.ToInt32(drpPlanType.SelectedValue.ToString());

                    if (PackageA)
                    {
                        objPlans.Description = txtPackageA.Value;
                        objPlans.Id = Convert.ToInt32(hdnPackageA.Value);
                        objPlans.PackageDisplayName = txtPackageADispName.Text.Trim();
                    }
                    else
                    {
                        objPlans.Description = txtPackageB.Value;
                        objPlans.Id = Convert.ToInt32(hdnPackageB.Value);
                        objPlans.PackageDisplayName = txtPackageBDispName.Text.Trim();
                    }
                    objPlans.PricePerMonth = Convert.ToDecimal(txtPrice.Text.Trim());
                    objPlans.DurationInMonth = Convert.ToInt16(txtDuration.Text.Trim());
                    objPlans.NumberOfService = Convert.ToInt16(txtNoOfService.Text.Trim());
                    objPlans.FirstServiceWithinDays = Convert.ToInt16(txtFirstService.Text.Trim());
                    objPlans.OtherServiceScheduleDaysGap = Convert.ToInt32(txtOtherServiceGap.Text.Trim());
                    objPlans.Drivetime = Convert.ToInt32(txtDriveTime.Text.Trim());
                    objPlans.ServiceTimeForFirstUnit = Convert.ToInt32(txtServiceTimeForFirstUnit.Text.Trim());
                    objPlans.ServiceTimeForAdditionalUnits = Convert.ToInt32(txtServiceTimeForAdditionalUnits.Text.Trim());
                    objPlans.ShowSpecialPrice = chkSpecial.Checked;
                    objPlans.ShowAutoRenewal = chkAutorenewal.Checked;

                    if (!string.IsNullOrEmpty(txtDiscount.Text.Trim()))
                        objPlans.DiscountPrice = Convert.ToDecimal(txtDiscount.Text.Trim());

                    objPlans.StripePlanId = txtPlanName.Text.Trim().Replace(' ', '-').ToLower() + '-' + Convert.ToDecimal(txtPrice.Text.Trim()).ToString() + '-' + txtDuration.Text.Trim();
                    objPlans.BackGroundColorHGS = txtColor.Text.Trim();
                    objPlans.BackGroundColorRGB = GenerateRgb(txtColor.Text.Trim());
                    objPlans.Status = chkActive.Checked;
                    objPlans.UpdatedBy = objLoginModel.Id;
                    objPlans.UpdatedByType= objLoginModel.RoleId;
                    objPlans.UpdatedDate= DateTime.UtcNow;

                    objPlanService.UpdatePlan(ref objPlans);
                    PackageA = false;
                }
                string TimeSlot1 = txtSlot1Start.Value.Trim() + " - " + txtSlot1End.Value.Trim();
                string TimeSlot2 = txtSlot2Start.Value.Trim() + " - " + txtSlot2End.Value.Trim();
                objPlanService.UpdatePlanTypeTimeSlot(Convert.ToInt32(drpPlanType.SelectedValue.ToString()), TimeSlot1, TimeSlot2);
                Session["msg"] = "edit";
                Response.Redirect(Application["SiteAddress"] + "admin/Plan_List.aspx");

            }
        }
    }
}