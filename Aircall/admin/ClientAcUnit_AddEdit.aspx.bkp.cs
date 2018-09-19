﻿using Aircall.Common;
using Newtonsoft.Json;
using Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Stripe;

namespace Aircall.admin
{
    public partial class ClientAcUnit_AddEdit : System.Web.UI.Page
    {
        IClientService objClientService;
        IClientUnitService objClientUnitService;
        IClientAddressService objClientAddressService;
        IStateService objStateService;
        ICitiesService objCitiesService;
        IPlanService objPlanService;
        IPartsService objPartsService;
        IClientUnitPartService objClientUnitPartService;
        IClientUnitPicturesService objClientUnitPicturesService;
        IClientUnitManualsService objClientUnitManualsService;
        IUnitExtraInfoService objUnitExtraInfoService;
        IClientUnitServiceCountService objClientUnitServiceCountService;
        IUnitsService objUnitsService;
        IClientPaymentMethodService objClientPaymentMethodService;
        IStripeErrorLogService objStripeErrorLogService;
        IOrderService objOrderService;
        IBillingHistoryService objBillingHistoryService;
        IZipCodeService objZipCodeService;
        IServiceReportService objServiceReportService;
        public bool SubscriptionCreated;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                dvServicereport.Visible = false;
                BindUnitTypeDropdown();
                BindPlanTypeDropdown();

                BindElectricalAndBreakerDropdown();

                drpAddress.Items.Insert(0, new ListItem("Select Address", "0"));
                drpAddress.Items.Insert(1, new ListItem("Enter New Address", "-1"));

                drpCard.Items.Insert(0, new ListItem("Select Card", "0"));
                drpCard.Items.Insert(1, new ListItem("Enter New Card", "-1"));

                FillAllDropdowns();

                VisibleDivs();

                if (!string.IsNullOrEmpty(Request.QueryString["CUnitId"]))
                {
                    btnSave.Text = "Update";
                    lnkSearch.Visible = false;
                    dvCardLeft.Visible = false;
                    dvServicereport.Visible = true;

                    objClientUnitService = ServiceFactory.ClientUnitService;
                    DataTable dtClientUnit = new DataTable();
                    int CUnitId = Convert.ToInt32(Request.QueryString["CUnitId"]);

                    //Bind Address and Client Unit Info
                    objClientUnitService.GetClientUnitById(CUnitId, ref dtClientUnit);
                    if (dtClientUnit.Rows.Count > 0)
                    {
                        txtClientName.Text = dtClientUnit.Rows[0]["ClientName"].ToString();
                        SearchClientByClientName(txtClientName.Text.Trim());
                        rblClient.SelectedValue = dtClientUnit.Rows[0]["ClientId"].ToString();
                        rblClient.Enabled = false;
                        txtUnitName.Text = dtClientUnit.Rows[0]["UnitName"].ToString();
                        txtNotes.Text = dtClientUnit.Rows[0]["Notes"].ToString();
                        drpPlanType.SelectedValue = dtClientUnit.Rows[0]["PlanTypeId"].ToString();
                        drpPlanType.Enabled = false;
                        int ClientId = Convert.ToInt32(dtClientUnit.Rows[0]["ClientId"].ToString());
                        hdnClient.Value = ClientId.ToString();
                        drpStatus.SelectedValue = dtClientUnit.Rows[0]["Status"].ToString();
                        BindAddressByClientId(ClientId);
                        drpAddress.SelectedValue = dtClientUnit.Rows[0]["AddressId"].ToString();
                        drpUnitType.SelectedValue = dtClientUnit.Rows[0]["UnitTypeId"].ToString();

                        if (dtClientUnit.Rows[0]["UnitTypeId"].ToString() == "2")
                            dvHeating.Visible = true;
                    }

                    //Bind Client Unit Part
                    objClientUnitPartService = ServiceFactory.ClientUnitPartService;
                    DataTable dtUnitPart = new DataTable();
                    objClientUnitPartService.GetUnitPartByUnitId(CUnitId, ref dtUnitPart);
                    if (dtUnitPart.Rows.Count > 0)
                    {
                        DataTable dtCooling = new DataTable();
                        DataTable dtHeating = new DataTable();
                        dtCooling = dtUnitPart.Clone();
                        dtHeating = dtUnitPart.Clone();
                        if (dtClientUnit.Rows[0]["UnitTypeId"].ToString() == "2")
                        {
                            string filter = "SplitType='Cooling'";
                            DataView dvCool = new DataView(dtUnitPart, filter, "", DataViewRowState.CurrentRows);
                            dtCooling = dvCool.ToTable();

                            filter = "SplitType='Heating'";
                            DataView dvHeat = new DataView(dtUnitPart, filter, "", DataViewRowState.CurrentRows);
                            dtHeating = dvHeat.ToTable();
                        }
                        else
                            dtCooling = dtUnitPart;

                        long ClientUnitPartId = Convert.ToInt64(dtCooling.Rows[0]["Id"].ToString());
                        txtModelNoCool.Text = dtCooling.Rows[0]["ModelNumber"].ToString();
                        txtSerialCool.Text = dtCooling.Rows[0]["SerialNumber"].ToString();
                        txtMfgBrandCool.Text = dtCooling.Rows[0]["ManufactureBrand"].ToString();
                        txtMfgDateCool.Value = dtCooling.Rows[0]["MFGMonthYear"].ToString();
                        txtUnitTonCool.Text = dtCooling.Rows[0]["UnitTon"].ToString();
                        drpBoosterCool.SelectedValue = dtCooling.Rows[0]["Booster"].ToString();
                        drpRefTypeCool.SelectedValue = dtCooling.Rows[0]["RefrigerantType"].ToString();
                        drpElecServiceCool.SelectedValue = dtCooling.Rows[0]["ElectricalService"].ToString();
                        drpMaxBreakerCool.SelectedValue = dtCooling.Rows[0]["MaxBreaker"].ToString();
                        drpBreakerCool.SelectedValue = dtCooling.Rows[0]["Breaker"].ToString();
                        drpCompressorCool.SelectedValue = dtCooling.Rows[0]["Compressor"].ToString();
                        drpCapacitorCool.SelectedValue = dtCooling.Rows[0]["Capacitor"].ToString();
                        drpContactorCool.SelectedValue = dtCooling.Rows[0]["Contactor"].ToString();
                        drpFilterdryerCool.SelectedValue = dtCooling.Rows[0]["Filterdryer"].ToString();
                        drpDefrostboardCool.SelectedValue = dtCooling.Rows[0]["Defrostboard"].ToString();
                        drpRelayCool.SelectedValue = dtCooling.Rows[0]["Relay"].ToString();
                        drpTXVValveCool.SelectedValue = dtCooling.Rows[0]["TXVValve"].ToString();
                        drpReversingValveCool.SelectedValue = dtCooling.Rows[0]["ReversingValve"].ToString();
                        drpBlowerMotorCool.SelectedValue = dtCooling.Rows[0]["BlowerMotor"].ToString();
                        drpCondensingMotorCool.SelectedValue = dtCooling.Rows[0]["Condensingfanmotor"].ToString();
                        drpInducerCool.SelectedValue = dtCooling.Rows[0]["Inducerdraftmotor"].ToString();
                        drpTransformerCool.SelectedValue = dtCooling.Rows[0]["Transformer"].ToString();
                        drpControlboardCool.SelectedValue = dtCooling.Rows[0]["Controlboard"].ToString();
                        drpLimitSwitchCool.SelectedValue = dtCooling.Rows[0]["Limitswitch"].ToString();
                        drpIgnitorCool.SelectedValue = dtCooling.Rows[0]["Ignitor"].ToString();
                        drpGasCool.SelectedValue = dtCooling.Rows[0]["Gasvalve"].ToString();
                        drpPressureswitchCool.SelectedValue = dtCooling.Rows[0]["Pressureswitch"].ToString();
                        drpFlamesensorCool.SelectedValue = dtCooling.Rows[0]["Flamesensor"].ToString();
                        drpRolloutsensorCool.SelectedValue = dtCooling.Rows[0]["Rolloutsensor"].ToString();
                        drpDoorswitchCool.SelectedValue = dtCooling.Rows[0]["Doorswitch"].ToString();
                        drpIgControlBoardCool.SelectedValue = dtCooling.Rows[0]["Ignitioncontrolboard"].ToString();
                        drpCoilCleanerCool.SelectedValue = dtCooling.Rows[0]["CoilCleaner"].ToString();
                        drpMiscCool.SelectedValue = dtCooling.Rows[0]["Misc"].ToString();

                        //Bind Extra Info
                        objUnitExtraInfoService = ServiceFactory.UnitExtraInfoService;
                        DataTable dtUnitInfo = new DataTable();
                        objUnitExtraInfoService.GetByClientUnitId(CUnitId, ClientUnitPartId, Aircall.Common.General.UnitExtraInfoType.Filter.ToString(), ref dtUnitInfo);
                        if (dtUnitInfo.Rows.Count > 0)
                        {
                            drpFilterQtyCool.SelectedValue = dtUnitInfo.Rows.Count.ToString();
                            FilterCoolDivVisible(Convert.ToInt32(drpFilterQtyCool.SelectedValue));
                            for (int i = 1; i <= dtUnitInfo.Rows.Count; i++)
                            {
                                DropDownList drpFilterSize = (DropDownList)PNLParts.FindControl("drpFilterSizeCool" + i);
                                DropDownList drpFilterLocation = (DropDownList)PNLParts.FindControl("drpFilterLocationCool" + i);
                                drpFilterSize.SelectedValue = dtUnitInfo.Rows[0]["PartId"].ToString();
                                if (Convert.ToBoolean(dtUnitInfo.Rows[0]["LocationOfPart"].ToString()))
                                    drpFilterLocation.SelectedValue = "1";
                                else
                                    drpFilterLocation.SelectedValue = "0";
                            }
                        }
                        objUnitExtraInfoService.GetByClientUnitId(CUnitId, ClientUnitPartId, Aircall.Common.General.UnitExtraInfoType.Fuses.ToString(), ref dtUnitInfo);
                        if (dtUnitInfo.Rows.Count > 0)
                        {
                            drpFuseQtyCool.SelectedValue = dtUnitInfo.Rows.Count.ToString();
                            FuseCoolDivVisible(Convert.ToInt32(drpFuseQtyCool.SelectedValue));
                            for (int i = 1; i <= dtUnitInfo.Rows.Count; i++)
                            {
                                DropDownList drpFuseType = (DropDownList)PNLParts.FindControl("drpFuseTypeCool" + i);
                                drpFuseType.SelectedValue = dtUnitInfo.Rows[0]["PartId"].ToString();
                            }
                        }

                        ClientUnitPartId = 0;
                        if (dtClientUnit.Rows[0]["UnitTypeId"].ToString() == "2")// && dtUnitPart.Rows.Count == 2)
                        {
                            ClientUnitPartId = Convert.ToInt32(dtHeating.Rows[0]["Id"].ToString());
                            txtModelNoHeat.Text = dtHeating.Rows[0]["ModelNumber"].ToString();
                            txtSerialHeat.Text = dtHeating.Rows[0]["SerialNumber"].ToString();
                            txtMfgBrandHeat.Text = dtHeating.Rows[0]["ManufactureBrand"].ToString();
                            txtMfgDateHeat.Value = dtHeating.Rows[0]["MFGMonthYear"].ToString();
                            txtUnitTonHeat.Text = dtHeating.Rows[0]["UnitTon"].ToString();
                            drpBoosterHeat.SelectedValue = dtHeating.Rows[0]["Booster"].ToString();
                            drpRefTypeHeat.SelectedValue = dtHeating.Rows[0]["RefrigerantType"].ToString();
                            drpElecServiceHeat.SelectedValue = dtHeating.Rows[0]["ElectricalService"].ToString();
                            drpMaxBreakerHeat.SelectedValue = dtHeating.Rows[0]["MaxBreaker"].ToString();
                            drpBreakerHeat.SelectedValue = dtHeating.Rows[0]["Breaker"].ToString();
                            drpCompressorHeat.SelectedValue = dtHeating.Rows[0]["Compressor"].ToString();
                            drpCapacitorHeat.SelectedValue = dtHeating.Rows[0]["Capacitor"].ToString();
                            drpContactorHeat.SelectedValue = dtHeating.Rows[0]["Contactor"].ToString();
                            drpFilterdryerHeat.SelectedValue = dtHeating.Rows[0]["Filterdryer"].ToString();
                            drpDefrostboardHeat.SelectedValue = dtHeating.Rows[0]["Defrostboard"].ToString();
                            drpRelayHeat.SelectedValue = dtHeating.Rows[0]["Relay"].ToString();
                            drpTXVValveHeat.SelectedValue = dtHeating.Rows[0]["TXVValve"].ToString();
                            drpReversingValveHeat.SelectedValue = dtHeating.Rows[0]["ReversingValve"].ToString();
                            drpBlowerMotorHeat.SelectedValue = dtHeating.Rows[0]["BlowerMotor"].ToString();
                            drpCondensingMotorHeat.SelectedValue = dtHeating.Rows[0]["Condensingfanmotor"].ToString();
                            drpInducerHeat.SelectedValue = dtHeating.Rows[0]["Inducerdraftmotor"].ToString();
                            drpTransformerHeat.SelectedValue = dtHeating.Rows[0]["Transformer"].ToString();
                            drpControlboardHeat.SelectedValue = dtHeating.Rows[0]["Controlboard"].ToString();
                            drpLimitSwitchHeat.SelectedValue = dtHeating.Rows[0]["Limitswitch"].ToString();
                            drpIgnitorHeat.SelectedValue = dtHeating.Rows[0]["Ignitor"].ToString();
                            drpGasHeat.SelectedValue = dtHeating.Rows[0]["Gasvalve"].ToString();
                            drpPressureswitchHeat.SelectedValue = dtHeating.Rows[0]["Pressureswitch"].ToString();
                            drpFlamesensorHeat.SelectedValue = dtHeating.Rows[0]["Flamesensor"].ToString();
                            drpRolloutsensorHeat.SelectedValue = dtHeating.Rows[0]["Rolloutsensor"].ToString();
                            drpDoorswitchHeat.SelectedValue = dtHeating.Rows[0]["Doorswitch"].ToString();
                            drpIgControlBoardHeat.SelectedValue = dtHeating.Rows[0]["Ignitioncontrolboard"].ToString();
                            drpCoilCleanerHeat.SelectedValue = dtHeating.Rows[0]["CoilCleaner"].ToString();
                            drpMiscHeat.SelectedValue = dtHeating.Rows[0]["Misc"].ToString();

                            //Bind Extra Info
                            objUnitExtraInfoService = ServiceFactory.UnitExtraInfoService;
                            objUnitExtraInfoService.GetByClientUnitId(CUnitId, ClientUnitPartId, Aircall.Common.General.UnitExtraInfoType.Filter.ToString(), ref dtUnitInfo);
                            if (dtUnitInfo.Rows.Count > 0)
                            {
                                drpFilterQtyHeat.SelectedValue = dtUnitInfo.Rows.Count.ToString();
                                FilterHeatDivVisible(Convert.ToInt32(drpFilterQtyHeat.SelectedValue));
                                for (int i = 1; i <= dtUnitInfo.Rows.Count; i++)
                                {
                                    DropDownList drpFilterSize = (DropDownList)PNLParts.FindControl("drpFilterSizeHeat" + i);
                                    DropDownList drpFilterLocation = (DropDownList)PNLParts.FindControl("drpFilterLocationHeat" + i);
                                    drpFilterSize.SelectedValue = dtUnitInfo.Rows[0]["PartId"].ToString();
                                    if (Convert.ToBoolean(dtUnitInfo.Rows[0]["LocationOfPart"].ToString()))
                                        drpFilterLocation.SelectedValue = "1";
                                    else
                                        drpFilterLocation.SelectedValue = "0";
                                }
                            }
                            objUnitExtraInfoService.GetByClientUnitId(CUnitId, ClientUnitPartId, Aircall.Common.General.UnitExtraInfoType.Fuses.ToString(), ref dtUnitInfo);
                            if (dtUnitInfo.Rows.Count > 0)
                            {
                                drpFuseQtyHeat.SelectedValue = dtUnitInfo.Rows.Count.ToString();
                                FuseHeatDivVisible(Convert.ToInt32(drpFuseQtyHeat.SelectedValue));
                                for (int i = 1; i <= dtUnitInfo.Rows.Count; i++)
                                {
                                    DropDownList drpFuseType = (DropDownList)PNLParts.FindControl("drpFuseTypeHeat" + i);
                                    drpFuseType.SelectedValue = dtUnitInfo.Rows[0]["PartId"].ToString();
                                }
                            }
                        }
                    }

                    //Bind Client Unit Images & Manuals
                    if (dtClientUnit.Rows[0]["UnitTypeId"].ToString() != "2")
                    {
                        objClientUnitPicturesService = ServiceFactory.ClientUnitPicturesService;
                        DataTable dtUnitPicture = new DataTable();
                        objClientUnitPicturesService.GetUnitPicturesByUnitId(CUnitId, drpUnitType.SelectedItem.Text, ref dtUnitPicture);
                        if (dtUnitPicture.Rows.Count > 0)
                        {
                            lstImageCool.DataSource = dtUnitPicture;
                            lstImageCool.DataBind();
                        }

                        objClientUnitManualsService = ServiceFactory.ClientUnitManualsService;
                        DataTable dtUnitManuals = new DataTable();
                        objClientUnitManualsService.GetManualsByUnitId(CUnitId, drpUnitType.SelectedItem.Text, ref dtUnitManuals);
                        if (dtUnitManuals.Rows.Count > 0)
                        {
                            lstManualCool.DataSource = dtUnitManuals;
                            lstManualCool.DataBind();
                        }
                    }
                    else
                    {
                        objClientUnitPicturesService = ServiceFactory.ClientUnitPicturesService;
                        DataTable dtUnitPictureCool = new DataTable();
                        objClientUnitPicturesService.GetUnitPicturesByUnitId(CUnitId, Aircall.Common.General.SplitType.Cooling.GetEnumDescription(), ref dtUnitPictureCool);
                        if (dtUnitPictureCool.Rows.Count > 0)
                        {
                            lstImageCool.DataSource = dtUnitPictureCool;
                            lstImageCool.DataBind();
                        }

                        DataTable dtUnitPictureHeat = new DataTable();
                        objClientUnitPicturesService.GetUnitPicturesByUnitId(CUnitId, Aircall.Common.General.SplitType.Heating.GetEnumDescription(), ref dtUnitPictureHeat);
                        if (dtUnitPictureHeat.Rows.Count > 0)
                        {
                            lstImageHeat.DataSource = dtUnitPictureHeat;
                            lstImageHeat.DataBind();
                        }

                        objClientUnitManualsService = ServiceFactory.ClientUnitManualsService;
                        DataTable dtUnitManualsCool = new DataTable();
                        objClientUnitManualsService.GetManualsByUnitId(CUnitId, Aircall.Common.General.SplitType.Cooling.GetEnumDescription(), ref dtUnitManualsCool);
                        if (dtUnitManualsCool.Rows.Count > 0)
                        {
                            lstManualCool.DataSource = dtUnitManualsCool;
                            lstManualCool.DataBind();
                        }

                        objClientUnitManualsService = ServiceFactory.ClientUnitManualsService;
                        DataTable dtUnitManualsHeat = new DataTable();
                        objClientUnitManualsService.GetManualsByUnitId(CUnitId, Aircall.Common.General.SplitType.Heating.GetEnumDescription(), ref dtUnitManualsHeat);
                        if (dtUnitManualsHeat.Rows.Count > 0)
                        {
                            lstManualHeat.DataSource = dtUnitManualsHeat;
                            lstManualHeat.DataBind();
                        }
                    }


                    //Bind Service Reports
                    objServiceReportService = ServiceFactory.ServiceReportService;
                    DataTable dtServiceReport = new DataTable();
                    objServiceReportService.GetServiceReportsByUnitId(CUnitId, ref dtServiceReport);
                    if (dtServiceReport.Rows.Count > 0)
                    {
                        lstServicereport.DataSource = dtServiceReport;
                        lstServicereport.DataBind();
                    }
                }
            }
        }

        private void BindElectricalAndBreakerDropdown()
        {
            string ElectricalService = General.GetSitesettingsValue("ElectricalServices");
            if (!string.IsNullOrEmpty(ElectricalService))
            {
                string[] ElecService = ElectricalService.Split(',');
                foreach (var item in ElecService)
                {
                    drpElecServiceCool.Items.Add(item.Trim());
                    drpElecServiceHeat.Items.Add(item.Trim());
                }
            }

            string MaxBreaker = General.GetSitesettingsValue("MaxBreaker");
            if (!string.IsNullOrEmpty(MaxBreaker))
            {
                string[] MaxBreakerArr = MaxBreaker.Split(',');
                foreach (var item in MaxBreakerArr)
                {
                    drpMaxBreakerCool.Items.Add(item.Trim());
                    drpMaxBreakerHeat.Items.Add(item.Trim());
                }
            }
        }

        private void VisibleDivs()
        {
            dvFilterCool1.Visible = false;
            dvFilterCool2.Visible = false;
            dvFilterCool3.Visible = false;
            dvFilterCool4.Visible = false;
            dvFilterCool5.Visible = false;
            dvFilterCool6.Visible = false;
            dvFuseCool1.Visible = false;
            dvFuseCool2.Visible = false;
            dvFuseCool3.Visible = false;
            dvFuseCool4.Visible = false;
            dvFuseCool5.Visible = false;
            dvFuseCool6.Visible = false;

            dvFilterHeat1.Visible = false;
            dvFilterHeat2.Visible = false;
            dvFilterHeat3.Visible = false;
            dvFilterHeat4.Visible = false;
            dvFilterHeat5.Visible = false;
            dvFilterHeat6.Visible = false;
            dvFuseHeat1.Visible = false;
            dvFuseHeat2.Visible = false;
            dvFuseHeat3.Visible = false;
            dvFuseHeat4.Visible = false;
            dvFuseHeat5.Visible = false;
            dvFuseHeat6.Visible = false;

            dvHeating.Visible = false;
        }

        private void FillAllDropdowns()
        {
            string PartTypeName = "Filter";
            DropDownList drpFilterCool = (DropDownList)PNLParts.FindControl("drpFilterSizeCool1");
            DropDownList drpFilterHeat = (DropDownList)PNLParts.FindControl("drpFilterSizeHeat1");
            FillPartsDropdown(PartTypeName, drpFilterCool, drpFilterHeat);

            PartTypeName = "Filter";
            drpFilterCool = (DropDownList)PNLParts.FindControl("drpFilterSizeCool2");
            drpFilterHeat = (DropDownList)PNLParts.FindControl("drpFilterSizeHeat2");
            FillPartsDropdown(PartTypeName, drpFilterCool, drpFilterHeat);

            PartTypeName = "Filter";
            drpFilterCool = (DropDownList)PNLParts.FindControl("drpFilterSizeCool3");
            drpFilterHeat = (DropDownList)PNLParts.FindControl("drpFilterSizeHeat3");
            FillPartsDropdown(PartTypeName, drpFilterCool, drpFilterHeat);

            PartTypeName = "Filter";
            drpFilterCool = (DropDownList)PNLParts.FindControl("drpFilterSizeCool4");
            drpFilterHeat = (DropDownList)PNLParts.FindControl("drpFilterSizeHeat4");
            FillPartsDropdown(PartTypeName, drpFilterCool, drpFilterHeat);

            PartTypeName = "Filter";
            drpFilterCool = (DropDownList)PNLParts.FindControl("drpFilterSizeCool5");
            drpFilterHeat = (DropDownList)PNLParts.FindControl("drpFilterSizeHeat5");
            FillPartsDropdown(PartTypeName, drpFilterCool, drpFilterHeat);

            PartTypeName = "Filter";
            drpFilterCool = (DropDownList)PNLParts.FindControl("drpFilterSizeCool6");
            drpFilterHeat = (DropDownList)PNLParts.FindControl("drpFilterSizeHeat6");
            FillPartsDropdown(PartTypeName, drpFilterCool, drpFilterHeat);

            PartTypeName = "Fuse";
            drpFilterCool = (DropDownList)PNLParts.FindControl("drpFuseTypeCool1");
            drpFilterHeat = (DropDownList)PNLParts.FindControl("drpFuseTypeHeat1");
            FillPartsDropdown(PartTypeName, drpFilterCool, drpFilterHeat);

            PartTypeName = "Fuse";
            drpFilterCool = (DropDownList)PNLParts.FindControl("drpFuseTypeCool2");
            drpFilterHeat = (DropDownList)PNLParts.FindControl("drpFuseTypeHeat2");
            FillPartsDropdown(PartTypeName, drpFilterCool, drpFilterHeat);

            PartTypeName = "Fuse";
            drpFilterCool = (DropDownList)PNLParts.FindControl("drpFuseTypeCool3");
            drpFilterHeat = (DropDownList)PNLParts.FindControl("drpFuseTypeHeat3");
            FillPartsDropdown(PartTypeName, drpFilterCool, drpFilterHeat);

            PartTypeName = "Fuse";
            drpFilterCool = (DropDownList)PNLParts.FindControl("drpFuseTypeCool4");
            drpFilterHeat = (DropDownList)PNLParts.FindControl("drpFuseTypeHeat4");
            FillPartsDropdown(PartTypeName, drpFilterCool, drpFilterHeat);

            PartTypeName = "Fuse";
            drpFilterCool = (DropDownList)PNLParts.FindControl("drpFuseTypeCool5");
            drpFilterHeat = (DropDownList)PNLParts.FindControl("drpFuseTypeHeat5");
            FillPartsDropdown(PartTypeName, drpFilterCool, drpFilterHeat);

            PartTypeName = "Fuse";
            drpFilterCool = (DropDownList)PNLParts.FindControl("drpFuseTypeCool6");
            drpFilterHeat = (DropDownList)PNLParts.FindControl("drpFuseTypeHeat6");
            FillPartsDropdown(PartTypeName, drpFilterCool, drpFilterHeat);

            PartTypeName = "Booster";
            drpFilterCool = (DropDownList)PNLParts.FindControl("drpBoosterCool");
            drpFilterHeat = (DropDownList)PNLParts.FindControl("drpBoosterHeat");
            FillPartsDropdown(PartTypeName, drpFilterCool, drpFilterHeat);

            PartTypeName = "Refrigerant";
            drpFilterCool = (DropDownList)PNLParts.FindControl("drpRefTypeCool");
            drpFilterHeat = (DropDownList)PNLParts.FindControl("drpRefTypeHeat");
            FillPartsDropdown(PartTypeName, drpFilterCool, drpFilterHeat);

            PartTypeName = "Breaker";
            drpFilterCool = (DropDownList)PNLParts.FindControl("drpBreakerCool");
            drpFilterHeat = (DropDownList)PNLParts.FindControl("drpBreakerHeat");
            FillPartsDropdown(PartTypeName, drpFilterCool, drpFilterHeat);

            PartTypeName = "Compressor";
            drpFilterCool = (DropDownList)PNLParts.FindControl("drpCompressorCool");
            drpFilterHeat = (DropDownList)PNLParts.FindControl("drpCompressorHeat");
            FillPartsDropdown(PartTypeName, drpFilterCool, drpFilterHeat);

            PartTypeName = "Capacitor";
            drpFilterCool = (DropDownList)PNLParts.FindControl("drpCapacitorCool");
            drpFilterHeat = (DropDownList)PNLParts.FindControl("drpCapacitorHeat");
            FillPartsDropdown(PartTypeName, drpFilterCool, drpFilterHeat);

            PartTypeName = "Contactor";
            drpFilterCool = (DropDownList)PNLParts.FindControl("drpContactorCool");
            drpFilterHeat = (DropDownList)PNLParts.FindControl("drpContactorHeat");
            FillPartsDropdown(PartTypeName, drpFilterCool, drpFilterHeat);

            PartTypeName = "Filter dryer";
            drpFilterCool = (DropDownList)PNLParts.FindControl("drpFilterdryerCool");
            drpFilterHeat = (DropDownList)PNLParts.FindControl("drpFilterdryerHeat");
            FillPartsDropdown(PartTypeName, drpFilterCool, drpFilterHeat);

            PartTypeName = "Defrost board";
            drpFilterCool = (DropDownList)PNLParts.FindControl("drpDefrostboardCool");
            drpFilterHeat = (DropDownList)PNLParts.FindControl("drpDefrostboardHeat");
            FillPartsDropdown(PartTypeName, drpFilterCool, drpFilterHeat);

            PartTypeName = "Relay";
            drpFilterCool = (DropDownList)PNLParts.FindControl("drpRelayCool");
            drpFilterHeat = (DropDownList)PNLParts.FindControl("drpRelayHeat");
            FillPartsDropdown(PartTypeName, drpFilterCool, drpFilterHeat);

            PartTypeName = "TXV Valve";
            drpFilterCool = (DropDownList)PNLParts.FindControl("drpTXVValveCool");
            drpFilterHeat = (DropDownList)PNLParts.FindControl("drpTXVValveHeat");
            FillPartsDropdown(PartTypeName, drpFilterCool, drpFilterHeat);

            PartTypeName = "Reversing Valve";
            drpFilterCool = (DropDownList)PNLParts.FindControl("drpReversingValveCool");
            drpFilterHeat = (DropDownList)PNLParts.FindControl("drpReversingValveHeat");
            FillPartsDropdown(PartTypeName, drpFilterCool, drpFilterHeat);

            PartTypeName = "Blower Motor";
            drpFilterCool = (DropDownList)PNLParts.FindControl("drpBlowerMotorCool");
            drpFilterHeat = (DropDownList)PNLParts.FindControl("drpBlowerMotorHeat");
            FillPartsDropdown(PartTypeName, drpFilterCool, drpFilterHeat);

            PartTypeName = "Condensing fan motor";
            drpFilterCool = (DropDownList)PNLParts.FindControl("drpCondensingMotorCool");
            drpFilterHeat = (DropDownList)PNLParts.FindControl("drpCondensingMotorHeat");
            FillPartsDropdown(PartTypeName, drpFilterCool, drpFilterHeat);

            PartTypeName = "Inducer draft motor/ flu vent motor";
            drpFilterCool = (DropDownList)PNLParts.FindControl("drpInducerCool");
            drpFilterHeat = (DropDownList)PNLParts.FindControl("drpInducerHeat");
            FillPartsDropdown(PartTypeName, drpFilterCool, drpFilterHeat);

            PartTypeName = "Transformer";
            drpFilterCool = (DropDownList)PNLParts.FindControl("drpTransformerCool");
            drpFilterHeat = (DropDownList)PNLParts.FindControl("drpTransformerHeat");
            FillPartsDropdown(PartTypeName, drpFilterCool, drpFilterHeat);

            PartTypeName = "Control board";
            drpFilterCool = (DropDownList)PNLParts.FindControl("drpControlboardCool");
            drpFilterHeat = (DropDownList)PNLParts.FindControl("drpControlboardHeat");
            FillPartsDropdown(PartTypeName, drpFilterCool, drpFilterHeat);

            PartTypeName = "Limit switch";
            drpFilterCool = (DropDownList)PNLParts.FindControl("drpLimitSwitchCool");
            drpFilterHeat = (DropDownList)PNLParts.FindControl("drpLimitSwitchHeat");
            FillPartsDropdown(PartTypeName, drpFilterCool, drpFilterHeat);

            PartTypeName = "Ignitor";
            drpFilterCool = (DropDownList)PNLParts.FindControl("drpIgnitorCool");
            drpFilterHeat = (DropDownList)PNLParts.FindControl("drpIgnitorHeat");
            FillPartsDropdown(PartTypeName, drpFilterCool, drpFilterHeat);

            PartTypeName = "Gas valve";
            drpFilterCool = (DropDownList)PNLParts.FindControl("drpGasCool");
            drpFilterHeat = (DropDownList)PNLParts.FindControl("drpGasHeat");
            FillPartsDropdown(PartTypeName, drpFilterCool, drpFilterHeat);

            PartTypeName = "Pressure switch";
            drpFilterCool = (DropDownList)PNLParts.FindControl("drpPressureswitchCool");
            drpFilterHeat = (DropDownList)PNLParts.FindControl("drpPressureswitchHeat");
            FillPartsDropdown(PartTypeName, drpFilterCool, drpFilterHeat);

            PartTypeName = "Flame sensor";
            drpFilterCool = (DropDownList)PNLParts.FindControl("drpFlamesensorCool");
            drpFilterHeat = (DropDownList)PNLParts.FindControl("drpFlamesensorHeat");
            FillPartsDropdown(PartTypeName, drpFilterCool, drpFilterHeat);

            PartTypeName = "Roll out sensor";
            drpFilterCool = (DropDownList)PNLParts.FindControl("drpRolloutsensorCool");
            drpFilterHeat = (DropDownList)PNLParts.FindControl("drpRolloutsensorHeat");
            FillPartsDropdown(PartTypeName, drpFilterCool, drpFilterHeat);

            PartTypeName = "Door switch";
            drpFilterCool = (DropDownList)PNLParts.FindControl("drpDoorswitchCool");
            drpFilterHeat = (DropDownList)PNLParts.FindControl("drpDoorswitchHeat");
            FillPartsDropdown(PartTypeName, drpFilterCool, drpFilterHeat);

            PartTypeName = "Ignition control board";
            drpFilterCool = (DropDownList)PNLParts.FindControl("drpIgControlBoardCool");
            drpFilterHeat = (DropDownList)PNLParts.FindControl("drpIgControlBoardHeat");
            FillPartsDropdown(PartTypeName, drpFilterCool, drpFilterHeat);

            PartTypeName = "Coil Cleaner";
            drpFilterCool = (DropDownList)PNLParts.FindControl("drpCoilCleanerCool");
            drpFilterHeat = (DropDownList)PNLParts.FindControl("drpCoilCleanerHeat");
            FillPartsDropdown(PartTypeName, drpFilterCool, drpFilterHeat);

            PartTypeName = "Misc";
            drpFilterCool = (DropDownList)PNLParts.FindControl("drpMiscCool");
            drpFilterHeat = (DropDownList)PNLParts.FindControl("drpMiscHeat");
            FillPartsDropdown(PartTypeName, drpFilterCool, drpFilterHeat);
        }

        private void FillPartsDropdown(string PartTypeName, DropDownList CoolControl, DropDownList HeatControl)
        {
            DataTable dtFilter = new DataTable();
            dtFilter = FindParts(PartTypeName);
            if (dtFilter.Rows.Count > 0)
            {
                if (CoolControl != null)
                {
                    CoolControl.DataSource = dtFilter;
                    //CoolControl.DataTextField = dtFilter.Columns["Name"].ToString();
                    CoolControl.DataTextField = dtFilter.Columns["PartSize"].ToString();
                    CoolControl.DataValueField = dtFilter.Columns["Id"].ToString();
                }
                if (HeatControl != null)
                {
                    HeatControl.DataSource = dtFilter;
                    //HeatControl.DataTextField = dtFilter.Columns["Name"].ToString();
                    HeatControl.DataTextField = dtFilter.Columns["PartSize"].ToString();
                    HeatControl.DataValueField = dtFilter.Columns["Id"].ToString();
                }
            }
            if (CoolControl != null)
            {
                CoolControl.DataBind();
                CoolControl.Items.Insert(0, new ListItem("Select " + PartTypeName, "0"));
            }
            if (HeatControl != null)
            {
                HeatControl.DataBind();
                HeatControl.Items.Insert(0, new ListItem("Select " + PartTypeName, "0"));
            }
        }

        private DataTable FindParts(string PartTypeName)
        {
            objPartsService = ServiceFactory.PartsService;
            DataTable dtParts = new DataTable();
            objPartsService.GetAllPartsByPartType(PartTypeName, ref dtParts);
            return dtParts;
        }

        private void BindPlanTypeDropdown()
        {
            objPlanService = ServiceFactory.PlanService;
            DataTable dtPlanType = new DataTable();
            objPlanService.GetAllPlanType(ref dtPlanType);
            if (dtPlanType.Rows.Count > 0)
            {
                drpPlanType.DataSource = dtPlanType;
                drpPlanType.DataTextField = dtPlanType.Columns["Name"].ToString();
                drpPlanType.DataValueField = dtPlanType.Columns["Id"].ToString();
                drpPlanType.DataBind();
            }
        }

        private void BindUnitTypeDropdown()
        {
            objClientUnitService = ServiceFactory.ClientUnitService;
            DataTable dtUnitTypes = new DataTable();
            objClientUnitService.GetUnitTypes(ref dtUnitTypes);
            if (dtUnitTypes.Rows.Count > 0)
            {
                drpUnitType.DataSource = dtUnitTypes;
                drpUnitType.DataValueField = dtUnitTypes.Columns["Id"].ToString();
                drpUnitType.DataTextField = dtUnitTypes.Columns["UnitTypeName"].ToString();
                drpUnitType.DataBind();
            }
        }

        protected void lnkSearch_Click(object sender, EventArgs e)
        {
            //if (!string.IsNullOrEmpty(txtClientName.Text.Trim()))
            //{
                SearchClientByClientName(txtClientName.Text.Trim());
            //}
        }

        private void SearchClientByClientName(string ClientName)
        {
            objClientService = ServiceFactory.ClientService;
            DataTable dtClients = new DataTable();
            objClientService.GetClientByName(ClientName, ref dtClients);
            if (dtClients.Rows.Count > 0)
            {
                rblClient.DataSource = dtClients;
                rblClient.DataValueField = dtClients.Columns["Id"].ToString();
                rblClient.DataTextField = dtClients.Columns["ClientName"].ToString();
                rblClient.DataBind();
            }
            else
            {
                rblClient.DataSource = "";
                rblClient.DataBind();
            }
        }

        protected void rblClient_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindAddressByClientId(Convert.ToInt32(rblClient.SelectedValue.ToString()));
            BindPaymentMethodByClientId(Convert.ToInt32(rblClient.SelectedValue.ToString()));
        }

        private void BindPaymentMethodByClientId(int ClientId)
        {
            objClientPaymentMethodService = ServiceFactory.ClientPaymentMethodService;
            DataTable dtClientPayment = new DataTable();
            objClientPaymentMethodService.GetClientPaymentMethodByClientId(ClientId, ref dtClientPayment);
            if (dtClientPayment.Rows.Count > 0)
            {
                drpCard.DataSource = dtClientPayment;
                drpCard.DataTextField = dtClientPayment.Columns["CardNumber"].ToString();
                drpCard.DataValueField = dtClientPayment.Columns["StripeCardId"].ToString();
                drpCard.DataBind();
                drpCard.Items.Insert(0, new ListItem("Select Card", "0"));
                drpCard.Items.Insert(dtClientPayment.Rows.Count + 1, new ListItem("Enter New Card", "-1"));
            }
            else
            {
                drpCard.DataSource = "";
                drpCard.DataBind();
                drpCard.Items.Insert(0, new ListItem("Select Card", "0"));
                drpCard.Items.Insert(dtClientPayment.Rows.Count + 1, new ListItem("Enter New Card", "-1"));
            }
        }

        private void BindAddressByClientId(int ClientId)
        {
            objClientAddressService = ServiceFactory.ClientAddressService;
            DataTable dtAddress = new DataTable();
            if (!string.IsNullOrEmpty(Request.QueryString["CUnitId"]))
                objClientAddressService.GetClientAddressesByClientId(ClientId,true ,ref dtAddress);
            else
                objClientAddressService.GetClientAddressesByClientId(ClientId,false ,ref dtAddress);

            if (dtAddress.Rows.Count > 0)
            {
                drpAddress.DataSource = dtAddress;
                drpAddress.DataTextField = dtAddress.Columns["ClientAddress"].ToString();
                drpAddress.DataValueField = dtAddress.Columns["Id"].ToString();
                drpAddress.DataBind();
                drpAddress.Items.Insert(0, new ListItem("Select Address", "0"));
                drpAddress.Items.Insert(dtAddress.Rows.Count + 1, new ListItem("Enter New Address", "-1"));
            }
            else
            {
                drpAddress.DataSource = "";
                drpAddress.DataBind();
                drpAddress.Items.Insert(0, new ListItem("Select Address", "0"));
                drpAddress.Items.Insert(dtAddress.Rows.Count + 1, new ListItem("Enter New Address", "-1"));
            }
        }

        private void FillStateDropdown()
        {
            objStateService = ServiceFactory.StateService;
            DataTable dtStates = new DataTable();
            if (!string.IsNullOrEmpty(Request.QueryString["CUnitId"]))
                objStateService.GetAllStates(true, true,ref dtStates);
            else
                objStateService.GetAllStates(true, false,ref dtStates);
            if (dtStates.Rows.Count > 0)
            {
                drpState.DataSource = dtStates;
                drpState.DataTextField = dtStates.Columns["Name"].ToString();
                drpState.DataValueField = dtStates.Columns["Id"].ToString();
            }
            drpState.DataBind();
            drpState.Items.Insert(0, new ListItem("Select State", "0"));
        }

        private void BindCityFromState(int StateId)
        {
            objCitiesService = ServiceFactory.CitiesService;
            DataTable dtCities = new DataTable();
            
            if (!string.IsNullOrEmpty(Request.QueryString["CUnitId"]))
                objCitiesService.GetAllCityByStateId(StateId, true,ref dtCities);
            else
                objCitiesService.GetAllCityByStateId(StateId, false,ref dtCities);

            if (dtCities.Rows.Count > 0)
            {
                drpCity.DataSource = dtCities;
                drpCity.DataValueField = dtCities.Columns["Id"].ToString();
                drpCity.DataTextField = dtCities.Columns["Name"].ToString();
            }
            else
                drpCity.DataSource = "";

            drpCity.DataBind();
            drpCity.Items.Insert(0, new ListItem("Select City", "0"));
        }

        protected void drpState_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (drpState.SelectedValue != "0")
            {
                BindCityFromState(Convert.ToInt32(drpState.SelectedValue.ToString()));
            }
            else
            {
                drpCity.DataSource = "";
                drpCity.DataBind();
                drpCity.Items.Insert(0, new ListItem("Select City", "0"));
            }
        }

        protected void drpAddress_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (drpAddress.SelectedValue.ToString() == "-1")
            {
                dvAddress.Visible = true;
                FillStateDropdown();
            }
            else
                dvAddress.Visible = false;
        }

        protected void drpUnitType_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 1. Packaged 2. Split 3. Heating
            if (drpUnitType.SelectedValue == "2")
            {
                dvHeating.Visible = true;
            }
            else
            {
                dvHeating.Visible = false;
            }

        }

        protected void  btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    if (Session["LoginSession"] != null)
                    {
                        LoginModel objLoginModel = new LoginModel();
                        objLoginModel = Session["LoginSession"] as LoginModel;

                        //Validation of Client Selected or not Start
                        if (string.IsNullOrEmpty(Request.QueryString["CUnitId"]))
                        {
                            if (rblClient.Items.Count == 0 || rblClient.SelectedIndex == -1)
                            {
                                dvMessage.InnerHtml = "<strong>Please Select Client</strong>";
                                dvMessage.Attributes.Add("class", "alert alert-error");
                                dvMessage.Visible = true;
                                return;
                            }
                        }
                        //Validation of Client Selected or not End

                        //Validation of Model & Serial No Start
                        if (drpUnitType.SelectedValue == "2")
                        {
                            if (!string.IsNullOrEmpty(txtModelNoCool.Text.Trim()) || !string.IsNullOrEmpty(txtModelNoHeat.Text.Trim()))
                            {
                                if (txtModelNoCool.Text.Trim() == txtModelNoHeat.Text.Trim())
                                {
                                    //dvMessage.InnerHtml = "<strong>The combination of Model Number and Serial Number must be different for Split type of Unit.</strong>";
                                    dvMessage.InnerHtml = "<strong>Model Number must be different for Split type of Unit.</strong>";
                                    dvMessage.Attributes.Add("class", "alert alert-error");
                                    dvMessage.Visible = true;
                                    return;
                                }
                            }
                        }

                        //Validation of Model & Serial No End

                        //Validation of Zipcode Start
                        if (drpAddress.SelectedValue == "-1")
                        {
                            objZipCodeService = ServiceFactory.ZipCodeService;
                            DataTable dtZipCode = new DataTable();
                            objZipCodeService.CheckValidZipCode(Convert.ToInt32(drpState.SelectedValue), Convert.ToInt32(drpCity.SelectedValue), txtZip.Text.Trim(), ref dtZipCode);
                            if (dtZipCode.Rows.Count == 0)
                            {
                                dvMessage.InnerHtml = "<strong>Please enter a valid Zip Code.</strong>";
                                dvMessage.Attributes.Add("class", "alert alert-error");
                                dvMessage.Visible = true;
                                return;
                            }
                        }
                        //Validation of Zipcode End

                        //Validation of Image Start
                        string CoolUnitImages = string.Empty;
                        string HeatUnitImages = string.Empty;

                        if (fpUnitPicCool.HasFiles)
                        {
                            string[] AllowedFileExtensions = new string[] { ".jpg", ".gif", ".png", ".jpeg" };
                            foreach (var item in fpUnitPicCool.PostedFiles)
                            {
                                if (!AllowedFileExtensions.Contains(item.FileName.Substring(item.FileName.LastIndexOf('.'))))
                                {
                                    dvMessage.InnerHtml = "<strong>Please select file of type: " + string.Join(", ", AllowedFileExtensions) + "</strong>";
                                    dvMessage.Attributes.Add("class", "alert alert-error");
                                    dvMessage.Visible = true;
                                    return;
                                }
                            }
                            foreach (var item in fpUnitPicCool.PostedFiles)
                            {
                                string filePath = Path.Combine(Server.MapPath("~/uploads/unitImages/"), item.FileName.Replace(' ', '_'));
                                item.SaveAs(filePath);
                                if (string.IsNullOrEmpty(CoolUnitImages))
                                    CoolUnitImages = item.FileName.Replace(' ', '_');
                                else
                                    CoolUnitImages = CoolUnitImages + '|' + item.FileName.Replace(' ', '_');
                            }
                        }
                        if (drpUnitType.SelectedValue == "2")
                        {
                            if (fpUnitPicHeat.HasFiles)
                            {
                                string[] AllowedFileExtensions = new string[] { ".jpg", ".gif", ".png", ".jpeg" };
                                foreach (var item in fpUnitPicHeat.PostedFiles)
                                {
                                    if (!AllowedFileExtensions.Contains(item.FileName.Substring(item.FileName.LastIndexOf('.'))))
                                    {
                                        dvMessage.InnerHtml = "<strong>Please select file of type: " + string.Join(", ", AllowedFileExtensions) + "</strong>";
                                        dvMessage.Attributes.Add("class", "alert alert-error");
                                        dvMessage.Visible = true;
                                        return;
                                    }
                                }
                                foreach (var item in fpUnitPicHeat.PostedFiles)
                                {
                                    string filePath = Path.Combine(Server.MapPath("~/uploads/unitImages/"), item.FileName.Replace(' ', '_'));
                                    item.SaveAs(filePath);
                                    if (string.IsNullOrEmpty(HeatUnitImages))
                                        HeatUnitImages = item.FileName.Replace(' ', '_');
                                    else
                                        HeatUnitImages = HeatUnitImages + '|' + item.FileName.Replace(' ', '_');
                                }
                            }
                        }
                        //Validation of Image End

                        //Validation of Manuals Start
                        string CoolUnitManuals = string.Empty;
                        string HeatUnitManuals = string.Empty;

                        if (fpManualCool.HasFiles)
                        {
                            string ManualFIleFormat = ".pdf";
                            foreach (var item in fpManualCool.PostedFiles)
                            {
                                if (!ManualFIleFormat.Contains(item.FileName.Substring(item.FileName.LastIndexOf('.'))))
                                {
                                    dvMessage.InnerHtml = "<strong>Please select file of type: " + ManualFIleFormat + "</strong>";
                                    dvMessage.Attributes.Add("class", "alert alert-error");
                                    dvMessage.Visible = true;
                                    return;
                                }
                            }
                            foreach (var item in fpManualCool.PostedFiles)
                            {
                                string filenameOriginal = DateTime.UtcNow.Ticks.ToString().Trim() + System.IO.Path.GetExtension(item.FileName);

                                string filePath = Path.Combine(Server.MapPath("~/uploads/unitManuals/"), filenameOriginal);
                                item.SaveAs(filePath);

                                //string filePath = Path.Combine(Server.MapPath("~/uploads/unitManuals/"), item.FileName.Replace(' ', '_'));
                                //item.SaveAs(filePath);
                                if (string.IsNullOrEmpty(CoolUnitManuals))
                                    CoolUnitManuals = filenameOriginal;//item.FileName.Replace(' ', '_');
                                else
                                    CoolUnitManuals = CoolUnitManuals + '|' + filenameOriginal;//item.FileName.Replace(' ', '_');
                            }
                        }
                        if (drpUnitType.SelectedValue == "2")
                        {
                            if (fpManualsHeat.HasFiles)
                            {
                                string ManualFIleFormat = ".pdf";
                                foreach (var item in fpManualsHeat.PostedFiles)
                                {
                                    if (!ManualFIleFormat.Contains(item.FileName.Substring(item.FileName.LastIndexOf('.'))))
                                    {
                                        dvMessage.InnerHtml = "<strong>Please select file of type: " + ManualFIleFormat + "</strong>";
                                        dvMessage.Attributes.Add("class", "alert alert-error");
                                        dvMessage.Visible = true;
                                        return;
                                    }
                                }
                                foreach (var item in fpManualsHeat.PostedFiles)
                                {
                                    //string filePath = Path.Combine(Server.MapPath("~/uploads/unitManuals/"), item.FileName.Replace(' ', '_'));
                                    //item.SaveAs(filePath);
                                    string filenameOriginal = DateTime.UtcNow.Ticks.ToString().Trim() + System.IO.Path.GetExtension(item.FileName);

                                    string filePath = Path.Combine(Server.MapPath("~/uploads/unitManuals/"), filenameOriginal);
                                    item.SaveAs(filePath);

                                    if (string.IsNullOrEmpty(HeatUnitManuals))
                                        HeatUnitManuals = filenameOriginal;//item.FileName.Replace(' ', '_');
                                    else
                                        HeatUnitManuals = HeatUnitManuals + '|' + filenameOriginal;//item.FileName.Replace(' ', '_');
                                }
                            }
                        }
                        //Validation of Manuals End

                        //Add New Client Card Start
                        string StripeCardId = string.Empty;
                        objClientPaymentMethodService = ServiceFactory.ClientPaymentMethodService;
                        if (string.IsNullOrEmpty(Request.QueryString["CUnitId"]))
                        {
                            if (drpCard.SelectedValue == "-1")
                            {
                                BizObjects.ClientPaymentMethod objClientPaymentMethod = new BizObjects.ClientPaymentMethod();
                                int ClientId = Convert.ToInt32(rblClient.SelectedValue.ToString());
                                DataTable dtClient = new DataTable();
                                objClientService = ServiceFactory.ClientService;
                                objClientService.GetClientById(ClientId, ref dtClient);

                                objClientPaymentMethod.ClientId = ClientId;
                                if (rblVisa.Checked)
                                    objClientPaymentMethod.CardType = "Visa";
                                else if (rblMaster.Checked)
                                    objClientPaymentMethod.CardType = "MasterCard";
                                else if (rblDiscover.Checked)
                                    objClientPaymentMethod.CardType = "Discover";
                                else
                                    objClientPaymentMethod.CardType = "AmericanExpress";

                                string cardStr = txtCardNumber.Text.Substring(txtCardNumber.Text.Trim().Length - 4);
                                objClientPaymentMethod.NameOnCard = txtCardName.Text.Trim();
                                objClientPaymentMethod.CardNumber = cardStr.PadLeft(16, '*');
                                objClientPaymentMethod.ExpiryMonth = Convert.ToInt16(txtMonth.Text.Trim());
                                objClientPaymentMethod.ExpiryYear = Convert.ToInt32(txtYear.Text.Trim());
                                objClientPaymentMethod.IsDefaultPayment = false;
                                objClientPaymentMethod.AddedBy = ClientId;
                                objClientPaymentMethod.AddedByType = General.UserRoles.Client.GetEnumValue();
                                objClientPaymentMethod.AddedDate = DateTime.UtcNow;

                                try
                                {
                                    var customerService = new StripeCustomerService();
                                    var myCustomer = customerService.Get(dtClient.Rows[0]["StripeCustomerId"].ToString());

                                    // setting up the card
                                    var myCard = new StripeCardCreateOptions();

                                    // setting up the card
                                    myCard.SourceCard = new SourceCard()
                                    {
                                        Number = txtCardNumber.Text.Trim(),
                                        ExpirationYear = txtYear.Text.Trim(),
                                        ExpirationMonth = txtMonth.Text.Trim(),
                                        Name = txtCardName.Text.Trim(),
                                        Cvc = txtCVV.Text.Trim()
                                    };

                                    var cardService = new StripeCardService();
                                    StripeCard stripeCard = cardService.Create(dtClient.Rows[0]["StripeCustomerId"].ToString(), myCard);
                                    if (string.IsNullOrEmpty(stripeCard.Id))
                                    {
                                        dvMessage.InnerHtml = "Invalid card.";
                                        dvMessage.Attributes.Add("class", "error");
                                        dvMessage.Visible = true;
                                        return;
                                    }
                                    objClientPaymentMethod.StripeCardId = stripeCard.Id;
                                    StripeCardId = stripeCard.Id;
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
                                    objStripeErrorLog.Userid = ClientId;
                                    objStripeErrorLog.UnitId = 0;
                                    objStripeErrorLog.DateAdded = DateTime.UtcNow;

                                    objStripeErrorLogService.AddStripeErrorLog(ref objStripeErrorLog);

                                    dvMessage.InnerHtml = stex.StripeError.Message.ToString();
                                    dvMessage.Attributes.Add("class", "error");
                                    dvMessage.Visible = true;
                                    return;
                                }
                                objClientPaymentMethodService.AddClientPaymentMethod(ref objClientPaymentMethod);
                            }
                            else
                            {
                                StripeCardId = drpCard.SelectedValue.ToString();
                            }
                        }
                        //Add New Client Card End

                        //Add New Client Address Start
                        int AddressId = 0;
                        decimal Latitude = 0;
                        decimal Longitude = 0;
                        if (drpAddress.SelectedValue == "-1")
                        {
                            string address = txtAddress.Text.ToString().Replace(" ", "+") + ",+" + drpCity.SelectedItem.Text + ",+" + drpState.SelectedItem.Text;

                            string url = string.Format("https://maps.googleapis.com/maps/api/geocode/json?address={0}", address + "&key=" + ConfigurationManager.AppSettings["GeoCodeKey"]);
                            WebRequest request = HttpWebRequest.Create(url);
                            using (WebResponse response = request.GetResponse())
                            {
                                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                                {
                                    string urlText = reader.ReadToEnd();
                                    var obj = JsonConvert.DeserializeObject<Example>(urlText);
                                    var geo = obj.results.FirstOrDefault();
                                    Latitude = (geo != null ? decimal.Parse(geo.geometry.location.lat.ToString()) : 0m);
                                    Longitude = (geo != null ? decimal.Parse(geo.geometry.location.lng.ToString()) : 0m);
                                }
                            }

                            //HttpClient client = new HttpClient();

                            //var dd = await client.GetAsync("https://maps.googleapis.com/maps/api/geocode/json?address=" + txtAddress.ToString().Replace(" ", "+") + ",+" + drpCity.SelectedItem.Text + ",+" + drpState.SelectedItem.Text + "&key=" + ConfigurationManager.AppSettings["GeoCodeKey"]);
                            //var data = await dd.Content.ReadAsAsync<Example>();
                            //var geo = data.results.FirstOrDefault();


                            BizObjects.ClientAddress objAddress = new BizObjects.ClientAddress();
                            objAddress.ClientId = Convert.ToInt32(rblClient.SelectedValue.ToString());
                            objAddress.Address = txtAddress.Text.Trim();
                            objAddress.State = Convert.ToInt32(drpState.SelectedValue.ToString());
                            objAddress.City = Convert.ToInt32(drpCity.SelectedValue.ToString());
                            objAddress.ZipCode = txtZip.Text.Trim();
                            objAddress.Latitude=Latitude;
                            objAddress.Longitude=Longitude;
                            objAddress.AddedBy = objLoginModel.Id;
                            objAddress.AddedByType = objLoginModel.RoleId;
                            objAddress.AddedDate = DateTime.UtcNow;

                            objClientAddressService = ServiceFactory.ClientAddressService;
                            AddressId = objClientAddressService.AddClientAddress(ref objAddress);
                        }
                        else
                        {
                            AddressId = Convert.ToInt32(drpAddress.SelectedValue.ToString());
                        }
                        //Add New Client Address End


                        BizObjects.ClientUnit objClientUnit = new BizObjects.ClientUnit();
                        int ClientUnitId = 0;

                        //Add Client Unit Start
                        if (!string.IsNullOrEmpty(Request.QueryString["CUnitId"]))
                            objClientUnit.ClientId = Convert.ToInt32(hdnClient.Value);
                        else
                            objClientUnit.ClientId = Convert.ToInt32(rblClient.SelectedValue.ToString());

                        objClientUnit.PlanTypeId = Convert.ToInt32(drpPlanType.SelectedValue.ToString());
                        objClientUnit.UnitName = txtUnitName.Text.Trim();
                        if (!string.IsNullOrEmpty(txtMfgDateCool.Value))
                        {
                            string[] CoolDate = txtMfgDateCool.Value.Trim().Split('/');
                            string strCoolDate = CoolDate[0].ToString() + "/01/" + CoolDate[1].ToString();
                            objClientUnit.ManufactureDate = Convert.ToDateTime(strCoolDate);
                        }
                        objClientUnit.AddressId = AddressId;
                        objClientUnit.UnitTypeId = Convert.ToInt32(drpUnitType.SelectedValue.ToString());
                        objClientUnit.AutoRenewal = false;
                        objClientUnit.Status = Convert.ToInt32(drpStatus.SelectedValue.ToString()); //(int)Aircall.Common.General.UnitStatus.ServiceSoon;
                        objClientUnit.Notes = txtNotes.Text.Trim();
                        objClientUnit.AddedBy = objLoginModel.Id;
                        objClientUnit.AddedByType = objLoginModel.RoleId;
                        objClientUnit.AddedDate = DateTime.UtcNow;

                        if (!string.IsNullOrEmpty(Request.QueryString["CUnitId"]))
                        {
                            int CUnitId = Convert.ToInt32(Request.QueryString["CUnitId"]);
                            objClientUnit.Id = CUnitId;
                            objClientUnit.UpdatedBy = objLoginModel.Id;
                            objClientUnit.UpdatedByType = objLoginModel.RoleId;
                            objClientUnit.UpdatedDate = DateTime.UtcNow;

                            objClientUnitService = ServiceFactory.ClientUnitService;
                            objClientUnitService.UpdateClientUnit(ref objClientUnit);
                            ClientUnitId = CUnitId;
                        }
                        else
                        {
                            objClientUnitService = ServiceFactory.ClientUnitService;
                            ClientUnitId = objClientUnitService.AddClientUnit(ref objClientUnit);
                        }
                        //Add Client Unit End

                        objClientUnitPartService = ServiceFactory.ClientUnitPartService;
                        string SubscriptionId = string.Empty;
                        DateTime StripeNextPaymentDate = DateTime.UtcNow;
                        decimal PlanPrice = 0;
                        if (ClientUnitId != 0)
                        {
                            //Start Unit Subscription
                            if (string.IsNullOrEmpty(Request.QueryString["CUnitId"]))
                            {
                                string StripeCustomerId = string.Empty;

                                string PaymentStatus = string.Empty;
                                DataTable dtClient = new DataTable();
                                objClientService = ServiceFactory.ClientService;
                                objClientUnitService = ServiceFactory.ClientUnitService;
                                objClientService.GetClientById(Convert.ToInt32(rblClient.SelectedValue.ToString()), ref dtClient);
                                if (dtClient.Rows.Count > 0)
                                {
                                    try
                                    {
                                        StripeCustomerId = dtClient.Rows[0]["StripeCustomerId"].ToString();
                                        var StripeCustomerService = new StripeCustomerService();
                                        StripeCustomerService.Update(StripeCustomerId, new StripeCustomerUpdateOptions() { DefaultSource = StripeCardId });
                                        var SubscriptionService = new StripeSubscriptionService();
                                        DataTable dtPlanType = new DataTable();
                                        objPlanService = ServiceFactory.PlanService;
                                        objPlanService.GetPlanByPlanType(Convert.ToInt32(drpPlanType.SelectedValue.ToString()), ref dtPlanType);
                                        if (dtPlanType.Rows.Count > 0)
                                        {
                                            StripeSubscription objStripeSubscription = SubscriptionService.Create(StripeCustomerId, dtPlanType.Rows[0]["StripePlanId"].ToString());
                                            SubscriptionId = objStripeSubscription.Id;
                                            PlanPrice = Convert.ToDecimal(dtPlanType.Rows[0]["PricePerMonth"].ToString());
                                            PaymentStatus = General.UnitPaymentTypes.Received.GetEnumDescription();

                                            StripeInvoiceService inv = new StripeInvoiceService();
                                            var inv1 = inv.Upcoming(StripeCustomerId, new StripeUpcomingInvoiceOptions() { SubscriptionId = SubscriptionId });
                                            StripeNextPaymentDate = inv1.PeriodEnd.Date;

                                            objClientUnitService.SetPaymentStatusByUnitId(ClientUnitId, PaymentStatus, SubscriptionId);
                                            SubscriptionCreated = true;
                                        }
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
                                        objStripeErrorLog.Userid = Convert.ToInt32(rblClient.SelectedValue.ToString());
                                        objStripeErrorLog.UnitId = ClientUnitId;
                                        objStripeErrorLog.DateAdded = DateTime.UtcNow;

                                        objStripeErrorLogService.AddStripeErrorLog(ref objStripeErrorLog);

                                        SubscriptionId = "";
                                        PaymentStatus = General.UnitPaymentTypes.NotReceived.GetEnumDescription();

                                        objClientUnitService.SetPaymentStatusByUnitId(ClientUnitId, PaymentStatus, SubscriptionId);
                                        SubscriptionCreated = false;
                                        objClientUnitService.HardDeleteClientUnit(ClientUnitId);
                                        dvMessage.InnerHtml = "<strong> " + stex.Message.ToString().Trim() + "</strong>";
                                        dvMessage.Attributes.Add("class", "alert alert-error");
                                        dvMessage.Visible = true;
                                        return;
                                    }
                                }
                            }


                            //Add Order & Billing History Start
                            if (string.IsNullOrEmpty(Request.QueryString["CUnitId"]))
                            {
                                BizObjects.Orders objOrders = new BizObjects.Orders();
                                objOrderService = ServiceFactory.OrderService;
                                int OrderId = 0;
                                objOrders.OrderType = "Charge";
                                objOrders.ClientId = Convert.ToInt32(rblClient.SelectedValue.ToString());
                                objOrders.OrderAmount = PlanPrice;
                                objOrders.ChargeBy = "CC";
                                objOrders.AddedBy = objLoginModel.Id;
                                objOrders.AddedByType = objLoginModel.RoleId;
                                objOrders.AddedDate = DateTime.UtcNow;
                                OrderId = objOrderService.AddClientUnitOrder(ref objOrders, StripeCardId, AddressId);
                                if (OrderId > 0)
                                {
                                    BizObjects.BillingHistory objBillingHistory = new BizObjects.BillingHistory();
                                    objBillingHistoryService = ServiceFactory.BillingHistoryService;
                                    objBillingHistory.ClientId = Convert.ToInt32(rblClient.SelectedValue);
                                    objBillingHistory.UnitId = ClientUnitId;
                                    objBillingHistory.OrderId = OrderId;
                                    objBillingHistory.PackageName = drpPlanType.SelectedItem.Text;
                                    objBillingHistory.BillingType = General.BillingTypes.Recurringpurchase.GetEnumDescription();
                                    objBillingHistory.OriginalAmount = PlanPrice;
                                    objBillingHistory.PurchasedAmount = PlanPrice;
                                    objBillingHistory.IsSpecialOffer = false;
                                    objBillingHistory.IsPaid = true;
                                    objBillingHistory.TransactionId = SubscriptionId;
                                    objBillingHistory.TransactionDate = DateTime.UtcNow;
                                    objBillingHistory.AddedBy = objLoginModel.Id;
                                    objBillingHistory.AddedDate = DateTime.UtcNow;
                                    objBillingHistory.StripeNextPaymentDate = StripeNextPaymentDate;

                                    objBillingHistoryService.AddClientUnitBillingHistory(ref objBillingHistory);
                                }
                            }
                            //Add Order & Billing History End

                            //Add Client Unit Service Count Start
                            if (string.IsNullOrEmpty(Request.QueryString["CUnitId"]))
                            {
                                AddClientUnitServiceCount(ClientUnitId);
                            }
                            //Add Client Unit Service Count End

                            //Add Client Unit Part Start
                            AddUnitPart(ClientUnitId);
                            //Add Client Unit Part End

                            //Add Client Unit Pictures Start
                            if (!string.IsNullOrEmpty(CoolUnitImages))
                            {
                                objClientUnitPicturesService = ServiceFactory.ClientUnitPicturesService;
                                BizObjects.ClientUnitPictures objClientUnitPictures = new BizObjects.ClientUnitPictures();

                                foreach (var item in CoolUnitImages.Split('|'))
                                {
                                    objClientUnitPictures.ClientUnitId = ClientUnitId;
                                    objClientUnitPictures.UnitImage = item;
                                    if (drpUnitType.SelectedValue == "2")
                                        objClientUnitPictures.SplitType = Aircall.Common.General.SplitType.Cooling.GetEnumDescription();
                                    else
                                        objClientUnitPictures.SplitType = drpUnitType.SelectedItem.Text;
                                    objClientUnitPictures.AddedBy = objLoginModel.Id;
                                    objClientUnitPictures.AddedByType = objLoginModel.RoleId;
                                    objClientUnitPictures.AddedDate = DateTime.UtcNow;

                                    objClientUnitPicturesService.AddClientUnitPictures(ref objClientUnitPictures);
                                }
                            }
                            if (!string.IsNullOrEmpty(HeatUnitImages))
                            {
                                objClientUnitPicturesService = ServiceFactory.ClientUnitPicturesService;
                                BizObjects.ClientUnitPictures objClientUnitPictures = new BizObjects.ClientUnitPictures();

                                foreach (var item in HeatUnitImages.Split('|'))
                                {
                                    objClientUnitPictures.ClientUnitId = ClientUnitId;
                                    objClientUnitPictures.UnitImage = item;
                                    objClientUnitPictures.SplitType = Aircall.Common.General.SplitType.Heating.GetEnumDescription();
                                    objClientUnitPictures.AddedBy = objLoginModel.Id;
                                    objClientUnitPictures.AddedByType = objLoginModel.RoleId;
                                    objClientUnitPictures.AddedDate = DateTime.UtcNow;

                                    objClientUnitPicturesService.AddClientUnitPictures(ref objClientUnitPictures);
                                }
                            }
                            //Add Client Unit Pictures End

                            //Add Client Unit Manuals Start
                            if (!string.IsNullOrEmpty(CoolUnitManuals))
                            {
                                objClientUnitManualsService = ServiceFactory.ClientUnitManualsService;
                                BizObjects.ClientUnitManuals objClientUnitManuals = new BizObjects.ClientUnitManuals();

                                foreach (var item in CoolUnitManuals.Split('|'))
                                {
                                    objClientUnitManuals.ClientUnitId = ClientUnitId;
                                    objClientUnitManuals.ManualName = item;
                                    if (drpUnitType.SelectedValue == "2")
                                        objClientUnitManuals.SplitType = Aircall.Common.General.SplitType.Cooling.GetEnumDescription();
                                    else
                                        objClientUnitManuals.SplitType = drpUnitType.SelectedItem.Text;
                                    objClientUnitManuals.AddedBy = objLoginModel.Id;
                                    objClientUnitManuals.AddedByType = objLoginModel.RoleId;
                                    objClientUnitManuals.AddedDate = DateTime.UtcNow;

                                    objClientUnitManualsService.AddClientUnitManuals(ref objClientUnitManuals);
                                }
                            }
                            if (!string.IsNullOrEmpty(HeatUnitManuals))
                            {
                                objClientUnitManualsService = ServiceFactory.ClientUnitManualsService;
                                BizObjects.ClientUnitManuals objClientUnitManuals = new BizObjects.ClientUnitManuals();

                                foreach (var item in HeatUnitManuals.Split('|'))
                                {
                                    objClientUnitManuals.ClientUnitId = ClientUnitId;
                                    objClientUnitManuals.ManualName = item;
                                    objClientUnitManuals.SplitType = Aircall.Common.General.SplitType.Heating.GetEnumDescription();
                                    objClientUnitManuals.AddedBy = objLoginModel.Id;
                                    objClientUnitManuals.AddedByType = objLoginModel.RoleId;
                                    objClientUnitManuals.AddedDate = DateTime.UtcNow;

                                    objClientUnitManualsService.AddClientUnitManuals(ref objClientUnitManuals);
                                }
                            }
                            //Add Client Unit Manuals End


                            if (!string.IsNullOrEmpty(txtModelNoCool.Text.Trim()) && !string.IsNullOrEmpty(txtSerialCool.Text.Trim()))
                            {
                                objUnitsService = ServiceFactory.UnitsService;
                                DataTable dtCoolUnit = new DataTable();
                                objUnitsService.FindUnitsByModelNumber(txtModelNoCool.Text.Trim(), ref dtCoolUnit);
                                if (dtCoolUnit.Rows.Count > 0)
                                {
                                    objClientUnitManualsService = ServiceFactory.ClientUnitManualsService;
                                    DataTable dtManual = new DataTable();
                                    int UnitId = Convert.ToInt32(dtCoolUnit.Rows[0]["Id"].ToString());
                                    int AddedBy = objLoginModel.Id;
                                    int AddedByType = objLoginModel.RoleId;
                                    DateTime AddedDate = DateTime.UtcNow;
                                    //objClientUnitManualsService.DeleteClientUnitManualByUnitId(ClientUnitId);
                                    if (drpUnitType.SelectedValue == "2")
                                        objClientUnitManualsService.AddClientUnitManualsFromUnitManuals(UnitId, ClientUnitId, General.SplitType.Cooling.GetEnumDescription(),AddedBy, AddedByType, AddedDate, ref dtManual);
                                    else
                                        objClientUnitManualsService.AddClientUnitManualsFromUnitManuals(UnitId, ClientUnitId, drpUnitType.SelectedItem.Text, AddedBy, AddedByType, AddedDate, ref dtManual);
                                    
                                    if (dtManual.Rows.Count > 0)
                                    {
                                        for (int i = 0; i < dtManual.Rows.Count; i++)
                                        {
                                            string filePath = Path.Combine(Server.MapPath("~/uploads/unitPartManuals/"), dtManual.Rows[i]["ManualFileName"].ToString());
                                            string fileDestPath = Path.Combine(Server.MapPath("~/uploads/unitManuals/"), dtManual.Rows[i]["ManualFileName"].ToString());
                                            FileInfo file = new FileInfo(filePath);
                                            if (file.Exists)
                                            {
                                                file.CopyTo(fileDestPath,true);
                                            }
                                        }
                                    }
                                }
                            }

                            if (drpUnitType.SelectedValue == "2")
                            {
                                if (!string.IsNullOrEmpty(txtModelNoHeat.Text.Trim()) && !string.IsNullOrEmpty(txtSerialHeat.Text.Trim()))
                                {
                                    objUnitsService = ServiceFactory.UnitsService;
                                    DataTable dtHeatUnit = new DataTable();
                                    objUnitsService.FindUnitsByModelNumber(txtModelNoHeat.Text.Trim(), ref dtHeatUnit);
                                    if (dtHeatUnit.Rows.Count > 0)
                                    {
                                        objClientUnitManualsService = ServiceFactory.ClientUnitManualsService;
                                        DataTable dtManual = new DataTable();
                                        int UnitId = Convert.ToInt32(dtHeatUnit.Rows[0]["Id"].ToString());
                                        int AddedBy = objLoginModel.Id;
                                        int AddedByType = objLoginModel.RoleId;
                                        DateTime AddedDate = DateTime.UtcNow;
                                        objClientUnitManualsService.AddClientUnitManualsFromUnitManuals(UnitId, ClientUnitId, General.SplitType.Heating.GetEnumDescription(),AddedBy, AddedByType, AddedDate, ref dtManual);
                                        if (dtManual.Rows.Count > 0)
                                        {
                                            for (int i = 0; i < dtManual.Rows.Count; i++)
                                            {
                                                string filePath = Path.Combine(Server.MapPath("~/uploads/unitPartManuals/"), dtManual.Rows[i]["ManualFileName"].ToString());
                                                string fileDestPath = Path.Combine(Server.MapPath("~/uploads/unitManuals/"), dtManual.Rows[i]["ManualFileName"].ToString());
                                                FileInfo file = new FileInfo(filePath);
                                                if (file.Exists)
                                                {
                                                    file.CopyTo(fileDestPath,true);
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            if (!string.IsNullOrEmpty(Request.QueryString["CUnitId"]))
                                Response.Redirect(Application["SiteAddress"] + "admin/ClientAcUnit_List.aspx?msg=edit");
                            else
                                Response.Redirect(Application["SiteAddress"] + "admin/ClientAcUnit_List.aspx?msg=add");
                        }
                    }
                    else
                    {
                        Response.Redirect(Application["SiteAddress"] + "admin/Login.aspx");
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

        private void AddUnitPart(int ClientUnitId)
        {
            if (!string.IsNullOrEmpty(Request.QueryString["CUnitId"]))
            {
                int CUnitId = Convert.ToInt32(Request.QueryString["CUnitId"]);
                objClientUnitPartService = ServiceFactory.ClientUnitPartService;
                objClientUnitPartService.DeleteClientUnitPartByUnitId(CUnitId);
            }

            BizObjects.ClientUnitParts objClientUnitParts = new BizObjects.ClientUnitParts();
            objClientUnitParts.UnitId = ClientUnitId;
            if (drpUnitType.SelectedValue == "2")
                objClientUnitParts.SplitType = Aircall.Common.General.SplitType.Cooling.ToString();
            else
                objClientUnitParts.SplitType = drpUnitType.SelectedItem.Text;

            objClientUnitParts.ModelNumber = txtModelNoCool.Text.Trim();
            objClientUnitParts.SerialNumber = txtSerialCool.Text.Trim();
            objClientUnitParts.ManufactureBrand = txtMfgBrandCool.Text.Trim();

            if (!string.IsNullOrEmpty(txtMfgDateCool.Value.Trim()))
            {
                string[] CoolDate = txtMfgDateCool.Value.Trim().Split('/');
                string strCoolDate = CoolDate[0].ToString() + "/01/" + CoolDate[1].ToString();
                objClientUnitParts.ManufactureDate = Convert.ToDateTime(strCoolDate);
            }
                
            objClientUnitParts.UnitTon = txtUnitTonCool.Text.Trim();
            objClientUnitParts.Booster = Convert.ToInt32(drpBoosterCool.SelectedValue.ToString());
            objClientUnitParts.RefrigerantType = Convert.ToInt32(drpRefTypeCool.SelectedValue.ToString());
            objClientUnitParts.ElectricalService = drpElecServiceCool.SelectedValue.ToString();
            objClientUnitParts.MaxBreaker = drpMaxBreakerCool.SelectedValue.ToString();
            objClientUnitParts.Breaker = Convert.ToInt32(drpBreakerCool.SelectedValue.ToString());
            objClientUnitParts.Compressor = Convert.ToInt32(drpCompressorCool.SelectedValue.ToString());
            objClientUnitParts.Capacitor = Convert.ToInt32(drpCapacitorCool.SelectedValue.ToString());
            objClientUnitParts.Contactor = Convert.ToInt32(drpContactorCool.SelectedValue.ToString());
            objClientUnitParts.Filterdryer = Convert.ToInt32(drpFilterdryerCool.SelectedValue.ToString());
            objClientUnitParts.Defrostboard = Convert.ToInt32(drpDefrostboardCool.SelectedValue.ToString());
            objClientUnitParts.Relay = Convert.ToInt32(drpRelayCool.SelectedValue.ToString());
            objClientUnitParts.TXVValve = Convert.ToInt32(drpTXVValveCool.SelectedValue.ToString());
            objClientUnitParts.ReversingValve = Convert.ToInt32(drpReversingValveCool.SelectedValue.ToString());
            objClientUnitParts.BlowerMotor = Convert.ToInt32(drpBlowerMotorCool.SelectedValue.ToString());
            objClientUnitParts.Condensingfanmotor = Convert.ToInt32(drpCondensingMotorCool.SelectedValue.ToString());
            objClientUnitParts.Inducerdraftmotor = Convert.ToInt32(drpInducerCool.SelectedValue.ToString());
            objClientUnitParts.Transformer = Convert.ToInt32(drpTransformerCool.SelectedValue.ToString());
            objClientUnitParts.Controlboard = Convert.ToInt32(drpControlboardCool.SelectedValue.ToString());
            objClientUnitParts.Limitswitch = Convert.ToInt32(drpLimitSwitchCool.SelectedValue.ToString());
            objClientUnitParts.Ignitor = Convert.ToInt32(drpIgnitorCool.SelectedValue.ToString());
            objClientUnitParts.Gasvalve = Convert.ToInt32(drpGasCool.SelectedValue.ToString());
            objClientUnitParts.Pressureswitch = Convert.ToInt32(drpPressureswitchCool.SelectedValue.ToString());
            objClientUnitParts.Flamesensor = Convert.ToInt32(drpFlamesensorCool.SelectedValue.ToString());
            objClientUnitParts.Rolloutsensor = Convert.ToInt32(drpRolloutsensorCool.SelectedValue.ToString());
            objClientUnitParts.Doorswitch = Convert.ToInt32(drpDoorswitchCool.SelectedValue.ToString());
            objClientUnitParts.Ignitioncontrolboard = Convert.ToInt32(drpIgControlBoardCool.SelectedValue.ToString());
            objClientUnitParts.CoilCleaner = Convert.ToInt32(drpCoilCleanerCool.SelectedValue.ToString());
            objClientUnitParts.Misc = Convert.ToInt32(drpMiscCool.SelectedValue.ToString());

            long ClientUnitPartId = 0;
            ClientUnitPartId = objClientUnitPartService.AddClientUnitPart(ref objClientUnitParts);
            //Add Extra Unit Info Start
            if (ClientUnitPartId != 0)
            {
                objUnitExtraInfoService = ServiceFactory.UnitExtraInfoService;
                if (drpFilterQtyCool.SelectedValue != "0")
                {
                    BizObjects.UnitExtraInfo objUnitExtraInfoFilter = new BizObjects.UnitExtraInfo();
                    objUnitExtraInfoFilter.UnitId = ClientUnitId;
                    objUnitExtraInfoFilter.ExtraInfoType = Aircall.Common.General.UnitExtraInfoType.Filter.ToString();
                    objUnitExtraInfoFilter.ClientUnitPartId = ClientUnitPartId;
                    int QtyCnt = Convert.ToInt32(drpFilterQtyCool.SelectedValue);
                    for (int i = 1; i <= QtyCnt; i++)
                    {
                        DropDownList drpFilterSize = (DropDownList)PNLParts.FindControl("drpFilterSizeCool" + i);
                        DropDownList drpFilterLocation = (DropDownList)PNLParts.FindControl("drpFilterLocationCool" + i);
                        objUnitExtraInfoFilter.PartId = Convert.ToInt32(drpFilterSize.SelectedValue.ToString());
                        //1=Inside Equipment 0=Inside Space
                        if (drpFilterLocation.SelectedValue == "0")
                            objUnitExtraInfoFilter.LocationOfPart = true;
                        else
                            objUnitExtraInfoFilter.LocationOfPart = false;

                        objUnitExtraInfoService.AddUnitExtraInfo(ref objUnitExtraInfoFilter);
                    }
                }
                if (drpFuseQtyCool.SelectedValue != "0")
                {
                    BizObjects.UnitExtraInfo objUnitExtraInfoFuse = new BizObjects.UnitExtraInfo();
                    objUnitExtraInfoFuse.UnitId = ClientUnitId;
                    objUnitExtraInfoFuse.ExtraInfoType = Aircall.Common.General.UnitExtraInfoType.Fuses.ToString();
                    objUnitExtraInfoFuse.ClientUnitPartId = ClientUnitPartId;
                    int QtyCnt = Convert.ToInt32(drpFuseQtyCool.SelectedValue);
                    for (int i = 1; i <= QtyCnt; i++)
                    {
                        DropDownList drpFuseType = (DropDownList)PNLParts.FindControl("drpFuseTypeCool" + i);
                        objUnitExtraInfoFuse.PartId = Convert.ToInt32(drpFuseType.SelectedValue.ToString());
                        objUnitExtraInfoService.AddUnitExtraInfo(ref objUnitExtraInfoFuse);
                    }
                }
            }
            //Add Extra Unit Info End

            ClientUnitPartId = 0;
            if (drpUnitType.SelectedValue == "2")
            {
                objClientUnitParts.UnitId = ClientUnitId;
                if (drpUnitType.SelectedValue == "2")
                    objClientUnitParts.SplitType = Aircall.Common.General.SplitType.Heating.ToString();

                objClientUnitParts.ModelNumber = txtModelNoHeat.Text.Trim();
                objClientUnitParts.SerialNumber = txtSerialHeat.Text.Trim();
                objClientUnitParts.ManufactureBrand = txtMfgBrandHeat.Text.Trim();
                if (!string.IsNullOrEmpty(txtMfgDateHeat.Value.Trim()))
                {
                    string[] HeatDate = txtMfgDateHeat.Value.Trim().Split('/');
                    string strHeatDate = HeatDate[0].ToString() + "/01/" + HeatDate[1].ToString();
                    objClientUnitParts.ManufactureDate = Convert.ToDateTime(strHeatDate);
                }
                    
                objClientUnitParts.UnitTon = txtUnitTonHeat.Text.Trim();
                objClientUnitParts.Booster = Convert.ToInt32(drpBoosterHeat.SelectedValue.ToString());
                objClientUnitParts.RefrigerantType = Convert.ToInt32(drpRefTypeHeat.SelectedValue.ToString());
                objClientUnitParts.ElectricalService = drpElecServiceHeat.SelectedValue.ToString();
                objClientUnitParts.MaxBreaker = drpMaxBreakerHeat.SelectedValue.ToString();
                objClientUnitParts.Breaker = Convert.ToInt32(drpBreakerHeat.SelectedValue.ToString());
                objClientUnitParts.Compressor = Convert.ToInt32(drpCompressorHeat.SelectedValue.ToString());
                objClientUnitParts.Capacitor = Convert.ToInt32(drpCapacitorHeat.SelectedValue.ToString());
                objClientUnitParts.Contactor = Convert.ToInt32(drpContactorHeat.SelectedValue.ToString());
                objClientUnitParts.Filterdryer = Convert.ToInt32(drpFilterdryerHeat.SelectedValue.ToString());
                objClientUnitParts.Defrostboard = Convert.ToInt32(drpDefrostboardHeat.SelectedValue.ToString());
                objClientUnitParts.Relay = Convert.ToInt32(drpRelayHeat.SelectedValue.ToString());
                objClientUnitParts.TXVValve = Convert.ToInt32(drpTXVValveHeat.SelectedValue.ToString());
                objClientUnitParts.ReversingValve = Convert.ToInt32(drpReversingValveHeat.SelectedValue.ToString());
                objClientUnitParts.BlowerMotor = Convert.ToInt32(drpBlowerMotorHeat.SelectedValue.ToString());
                objClientUnitParts.Condensingfanmotor = Convert.ToInt32(drpCondensingMotorHeat.SelectedValue.ToString());
                objClientUnitParts.Inducerdraftmotor = Convert.ToInt32(drpInducerHeat.SelectedValue.ToString());
                objClientUnitParts.Transformer = Convert.ToInt32(drpTransformerHeat.SelectedValue.ToString());
                objClientUnitParts.Controlboard = Convert.ToInt32(drpControlboardHeat.SelectedValue.ToString());
                objClientUnitParts.Limitswitch = Convert.ToInt32(drpLimitSwitchHeat.SelectedValue.ToString());
                objClientUnitParts.Ignitor = Convert.ToInt32(drpIgnitorHeat.SelectedValue.ToString());
                objClientUnitParts.Gasvalve = Convert.ToInt32(drpGasHeat.SelectedValue.ToString());
                objClientUnitParts.Pressureswitch = Convert.ToInt32(drpPressureswitchHeat.SelectedValue.ToString());
                objClientUnitParts.Flamesensor = Convert.ToInt32(drpFlamesensorHeat.SelectedValue.ToString());
                objClientUnitParts.Rolloutsensor = Convert.ToInt32(drpRolloutsensorHeat.SelectedValue.ToString());
                objClientUnitParts.Doorswitch = Convert.ToInt32(drpDoorswitchHeat.SelectedValue.ToString());
                objClientUnitParts.Ignitioncontrolboard = Convert.ToInt32(drpIgControlBoardHeat.SelectedValue.ToString());
                objClientUnitParts.CoilCleaner = Convert.ToInt32(drpCoilCleanerHeat.SelectedValue.ToString());
                objClientUnitParts.Misc = Convert.ToInt32(drpMiscHeat.SelectedValue.ToString());

                ClientUnitPartId = objClientUnitPartService.AddClientUnitPart(ref objClientUnitParts);

                //Add Extra Unit Info Start
                if (ClientUnitPartId != 0)
                {
                    if (drpFilterQtyHeat.SelectedValue != "0")
                    {
                        BizObjects.UnitExtraInfo objUnitExtraInfoFilter = new BizObjects.UnitExtraInfo();
                        objUnitExtraInfoFilter.UnitId = ClientUnitId;
                        objUnitExtraInfoFilter.ExtraInfoType = Aircall.Common.General.UnitExtraInfoType.Filter.ToString();
                        objUnitExtraInfoFilter.ClientUnitPartId = ClientUnitPartId;
                        int QtyCnt = Convert.ToInt32(drpFilterQtyHeat.SelectedValue);
                        for (int i = 1; i <= QtyCnt; i++)
                        {
                            DropDownList drpFilterSize = (DropDownList)PNLParts.FindControl("drpFilterSizeHeat" + i);
                            DropDownList drpFilterLocation = (DropDownList)PNLParts.FindControl("drpFilterLocationHeat" + i);
                            objUnitExtraInfoFilter.PartId = Convert.ToInt32(drpFilterSize.SelectedValue.ToString());
                            //1=Inside Equipment 0=Inside Space
                            if (drpFilterLocation.SelectedValue == "0")
                                objUnitExtraInfoFilter.LocationOfPart = true;
                            else
                                objUnitExtraInfoFilter.LocationOfPart = false;

                            objUnitExtraInfoService.AddUnitExtraInfo(ref objUnitExtraInfoFilter);
                        }
                    }
                    if (drpFuseQtyHeat.SelectedValue != "0")
                    {
                        BizObjects.UnitExtraInfo objUnitExtraInfoFuse = new BizObjects.UnitExtraInfo();
                        objUnitExtraInfoFuse.UnitId = ClientUnitId;
                        objUnitExtraInfoFuse.ExtraInfoType = Aircall.Common.General.UnitExtraInfoType.Fuses.ToString();
                        objUnitExtraInfoFuse.ClientUnitPartId = ClientUnitPartId;
                        int QtyCnt = Convert.ToInt32(drpFuseQtyHeat.SelectedValue);
                        for (int i = 1; i <= QtyCnt; i++)
                        {
                            DropDownList drpFuseType = (DropDownList)PNLParts.FindControl("drpFuseTypeHeat" + i);
                            objUnitExtraInfoFuse.PartId = Convert.ToInt32(drpFuseType.SelectedValue.ToString());
                            objUnitExtraInfoService.AddUnitExtraInfo(ref objUnitExtraInfoFuse);
                        }
                    }
                }
                //Add Extra Unit Info End
            }
        }

        private void AddClientUnitServiceCount(int ClientUnitId)
        {
            LoginModel objLoginModel = new LoginModel();
            objLoginModel = Session["LoginSession"] as LoginModel;

            BizObjects.ClientUnitServiceCount objUnitServiceCount = new BizObjects.ClientUnitServiceCount();
            objClientUnitServiceCountService = ServiceFactory.ClientUnitServiceCountService;
            objUnitServiceCount.ClientId = Convert.ToInt32(rblClient.SelectedValue.ToString());
            objUnitServiceCount.UnitId = ClientUnitId;
            objUnitServiceCount.PlanType = Convert.ToInt32(drpPlanType.SelectedValue.ToString());
            objUnitServiceCount.TotalDonePlanService = 0;
            objUnitServiceCount.TotalRequestService = 0;
            objUnitServiceCount.TotalDoneRequestService = 0;
            objUnitServiceCount.TotalBillsGenerated = 1;
            objUnitServiceCount.StripeUnitSubscriptionCount = SubscriptionCreated ? 1 : 0;
            objUnitServiceCount.AddedBy = objLoginModel.Id;
            objUnitServiceCount.AddedByType = objLoginModel.RoleId;
            objUnitServiceCount.AddedDate = DateTime.UtcNow;

            objClientUnitServiceCountService.AddClientUnitServiceCount(ref objUnitServiceCount);
        }

        protected void drpFilterQtyCool_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterCoolDivVisible(Convert.ToInt32(drpFilterQtyCool.SelectedValue));
        }

        private void FilterCoolDivVisible(int Cnt)
        {
            if (Cnt == 0)
            {
                for (int i = 1; i <= 6; i++)
                {
                    Control dvControl = (Control)PNLParts.FindControl("dvFilterCool" + i);
                    dvControl.Visible = false;
                }
            }
            for (int i = 1; i <= 6; i++)
            {
                Control dvControl = (Control)PNLParts.FindControl("dvFilterCool" + i);
                if (i <= Cnt)
                    dvControl.Visible = true;
                else
                    dvControl.Visible = false;
            }
        }

        protected void drpFuseQtyCool_SelectedIndexChanged(object sender, EventArgs e)
        {
            FuseCoolDivVisible(Convert.ToInt32(drpFuseQtyCool.SelectedValue));
        }

        private void FuseCoolDivVisible(int Cnt)
        {
            if (Cnt == 0)
            {
                for (int i = 1; i <= 6; i++)
                {
                    Control dvControl = (Control)PNLParts.FindControl("dvFuseCool" + i);
                    dvControl.Visible = false;
                }
            }
            for (int i = 1; i <= 6; i++)
            {
                Control dvControl = (Control)PNLParts.FindControl("dvFuseCool" + i);
                if (i <= Cnt)
                    dvControl.Visible = true;
                else
                    dvControl.Visible = false;
            }
        }

        protected void drpFilterQtyHeat_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterHeatDivVisible(Convert.ToInt32(drpFilterQtyHeat.SelectedValue));
        }

        private void FilterHeatDivVisible(int Cnt)
        {
            if (Cnt == 0)
            {
                for (int i = 1; i <= 6; i++)
                {
                    Control dvControl = (Control)PNLParts.FindControl("dvFilterHeat" + i);
                    dvControl.Visible = false;
                }
            }
            for (int i = 1; i <= 6; i++)
            {
                Control dvControl = (Control)PNLParts.FindControl("dvFilterHeat" + i);
                if (i <= Cnt)
                    dvControl.Visible = true;
                else
                    dvControl.Visible = false;
            }
        }

        protected void drpFuseQtyHeat_SelectedIndexChanged(object sender, EventArgs e)
        {
            FuseHeatDivVisible(Convert.ToInt32(drpFuseQtyHeat.SelectedValue));
        }

        private void FuseHeatDivVisible(int Cnt)
        {
            if (Cnt == 0)
            {
                for (int i = 1; i <= 6; i++)
                {
                    Control dvControl = (Control)PNLParts.FindControl("dvFuseHeat" + i);
                    dvControl.Visible = false;
                }
            }
            for (int i = 1; i <= 6; i++)
            {
                Control dvControl = (Control)PNLParts.FindControl("dvFuseHeat" + i);
                if (i <= Cnt)
                    dvControl.Visible = true;
                else
                    dvControl.Visible = false;
            }
        }

        protected void lnkFindUnitTypesCool_Click(object sender, EventArgs e)
        {
            dvMessage.InnerHtml = "";
            dvMessage.Visible = false;

            if (!string.IsNullOrEmpty(txtModelNoCool.Text.Trim()))
            {
                objUnitsService = ServiceFactory.UnitsService;
                DataTable dtCoolUnit = new DataTable();
                objUnitsService.FindUnitsByModelNumber(txtModelNoCool.Text.Trim(), ref dtCoolUnit);
                if (dtCoolUnit.Rows.Count > 0)
                {
                    txtMfgDateCool.Value = dtCoolUnit.Rows[0]["MFGMonthYear"].ToString();
                    txtMfgBrandCool.Text = dtCoolUnit.Rows[0]["ManufactureBrand"].ToString();
                    txtUnitTonCool.Text = dtCoolUnit.Rows[0]["UnitTon"].ToString();
                    drpBoosterCool.SelectedValue = dtCoolUnit.Rows[0]["Booster"].ToString();
                    drpRefTypeCool.SelectedValue = dtCoolUnit.Rows[0]["Refrigerant"].ToString();
                    drpElecServiceCool.SelectedValue = dtCoolUnit.Rows[0]["ElectricalService"].ToString();
                    if (!string.IsNullOrEmpty(dtCoolUnit.Rows[0]["MaxBreaker"].ToString()))
                        drpMaxBreakerCool.SelectedValue = dtCoolUnit.Rows[0]["MaxBreaker"].ToString();
                    drpBreakerCool.SelectedValue = dtCoolUnit.Rows[0]["Breaker"].ToString();
                    drpCompressorCool.SelectedValue = dtCoolUnit.Rows[0]["Compressor"].ToString();
                    drpCapacitorCool.SelectedValue = dtCoolUnit.Rows[0]["Capacitor"].ToString();
                    drpContactorCool.SelectedValue = dtCoolUnit.Rows[0]["Contactor"].ToString();
                    drpFilterdryerCool.SelectedValue = dtCoolUnit.Rows[0]["Filterdryer"].ToString();
                    drpDefrostboardCool.SelectedValue = dtCoolUnit.Rows[0]["Defrostboard"].ToString();
                    drpRelayCool.SelectedValue = dtCoolUnit.Rows[0]["Relay"].ToString();
                    drpTXVValveCool.SelectedValue = dtCoolUnit.Rows[0]["TXVValve"].ToString();
                    drpReversingValveCool.SelectedValue = dtCoolUnit.Rows[0]["ReversingValve"].ToString();
                    drpBlowerMotorCool.SelectedValue = dtCoolUnit.Rows[0]["BlowerMotor"].ToString();
                    drpCondensingMotorCool.SelectedValue = dtCoolUnit.Rows[0]["Condensingfanmotor"].ToString();
                    drpInducerCool.SelectedValue = dtCoolUnit.Rows[0]["Inducerdraftmotor"].ToString();
                    drpTransformerCool.SelectedValue = dtCoolUnit.Rows[0]["Transformer"].ToString();
                    drpControlboardCool.SelectedValue = dtCoolUnit.Rows[0]["Controlboard"].ToString();
                    drpLimitSwitchCool.SelectedValue = dtCoolUnit.Rows[0]["Limitswitch"].ToString();
                    drpIgnitorCool.SelectedValue = dtCoolUnit.Rows[0]["Ignitor"].ToString();
                    drpGasCool.SelectedValue = dtCoolUnit.Rows[0]["Gasvalve"].ToString();
                    drpPressureswitchCool.SelectedValue = dtCoolUnit.Rows[0]["Pressureswitch"].ToString();
                    drpFlamesensorCool.SelectedValue = dtCoolUnit.Rows[0]["Flamesensor"].ToString();
                    drpRolloutsensorCool.SelectedValue = dtCoolUnit.Rows[0]["Rolloutsensor"].ToString();
                    drpDoorswitchCool.SelectedValue = dtCoolUnit.Rows[0]["Doorswitch"].ToString();
                    drpIgControlBoardCool.SelectedValue = dtCoolUnit.Rows[0]["Ignitioncontrolboard"].ToString();
                    drpCoilCleanerCool.SelectedValue = dtCoolUnit.Rows[0]["CoilCleaner"].ToString();
                    drpMiscCool.SelectedValue = dtCoolUnit.Rows[0]["Misc"].ToString();

                    dvMessage.InnerHtml = "<strong>Unit Part found for provided Model Number.</strong>";
                    dvMessage.Attributes.Add("class", "alert alert-success");
                    dvMessage.Visible = true;
                    //btnSave_Click(btnSave, EventArgs.Empty);
                }
                else
                {
                    dvMessage.InnerHtml = "<strong>Unit Part not found for provided Model Number.</strong>";
                    dvMessage.Attributes.Add("class", "alert alert-error");
                    dvMessage.Visible = true;
                }
            }
        }

        protected void lnkFindUnitTypesHeat_Click(object sender, EventArgs e)
        {
            dvMessage.InnerHtml = "";
            dvMessage.Visible = false;

            if (!string.IsNullOrEmpty(txtModelNoHeat.Text.Trim()) && !string.IsNullOrEmpty(txtSerialHeat.Text.Trim()))
            {
                objUnitsService = ServiceFactory.UnitsService;
                DataTable dtHeatUnit = new DataTable();
                objUnitsService.FindUnitsByModelNumber(txtModelNoHeat.Text.Trim(), ref dtHeatUnit);
                if (dtHeatUnit.Rows.Count > 0)
                {
                    txtMfgDateHeat.Value = dtHeatUnit.Rows[0]["MFGMonthYear"].ToString();
                    txtMfgBrandHeat.Text = dtHeatUnit.Rows[0]["ManufactureBrand"].ToString();
                    txtUnitTonHeat.Text = dtHeatUnit.Rows[0]["UnitTon"].ToString();
                    drpBoosterHeat.SelectedValue = dtHeatUnit.Rows[0]["Booster"].ToString();
                    drpRefTypeHeat.SelectedValue = dtHeatUnit.Rows[0]["Refrigerant"].ToString();
                    drpElecServiceHeat.SelectedValue = dtHeatUnit.Rows[0]["ElectricalService"].ToString();
                    if (!string.IsNullOrEmpty(dtHeatUnit.Rows[0]["MaxBreaker"].ToString()))
                        drpMaxBreakerHeat.SelectedValue = dtHeatUnit.Rows[0]["MaxBreaker"].ToString();
                    drpBreakerHeat.SelectedValue = dtHeatUnit.Rows[0]["Breaker"].ToString();
                    drpCompressorHeat.SelectedValue = dtHeatUnit.Rows[0]["Compressor"].ToString();
                    drpCapacitorHeat.SelectedValue = dtHeatUnit.Rows[0]["Capacitor"].ToString();
                    drpContactorHeat.SelectedValue = dtHeatUnit.Rows[0]["Contactor"].ToString();
                    drpFilterdryerHeat.SelectedValue = dtHeatUnit.Rows[0]["Filterdryer"].ToString();
                    drpDefrostboardHeat.SelectedValue = dtHeatUnit.Rows[0]["Defrostboard"].ToString();
                    drpRelayHeat.SelectedValue = dtHeatUnit.Rows[0]["Relay"].ToString();
                    drpTXVValveHeat.SelectedValue = dtHeatUnit.Rows[0]["TXVValve"].ToString();
                    drpReversingValveHeat.SelectedValue = dtHeatUnit.Rows[0]["ReversingValve"].ToString();
                    drpBlowerMotorHeat.SelectedValue = dtHeatUnit.Rows[0]["BlowerMotor"].ToString();
                    drpCondensingMotorHeat.SelectedValue = dtHeatUnit.Rows[0]["Condensingfanmotor"].ToString();
                    drpInducerHeat.SelectedValue = dtHeatUnit.Rows[0]["Inducerdraftmotor"].ToString();
                    drpTransformerHeat.SelectedValue = dtHeatUnit.Rows[0]["Transformer"].ToString();
                    drpControlboardHeat.SelectedValue = dtHeatUnit.Rows[0]["Controlboard"].ToString();
                    drpLimitSwitchHeat.SelectedValue = dtHeatUnit.Rows[0]["Limitswitch"].ToString();
                    drpIgnitorHeat.SelectedValue = dtHeatUnit.Rows[0]["Ignitor"].ToString();
                    drpGasHeat.SelectedValue = dtHeatUnit.Rows[0]["Gasvalve"].ToString();
                    drpPressureswitchHeat.SelectedValue = dtHeatUnit.Rows[0]["Pressureswitch"].ToString();
                    drpFlamesensorHeat.SelectedValue = dtHeatUnit.Rows[0]["Flamesensor"].ToString();
                    drpRolloutsensorHeat.SelectedValue = dtHeatUnit.Rows[0]["Rolloutsensor"].ToString();
                    drpDoorswitchHeat.SelectedValue = dtHeatUnit.Rows[0]["Doorswitch"].ToString();
                    drpIgControlBoardHeat.SelectedValue = dtHeatUnit.Rows[0]["Ignitioncontrolboard"].ToString();
                    drpCoilCleanerHeat.SelectedValue = dtHeatUnit.Rows[0]["CoilCleaner"].ToString();
                    drpMiscHeat.SelectedValue = dtHeatUnit.Rows[0]["Misc"].ToString();

                    dvMessage.InnerHtml = "<strong>Unit Part found for provided Model Number.</strong>";
                    dvMessage.Attributes.Add("class", "alert alert-success");
                    dvMessage.Visible = true;
                    //btnSave_Click(btnSave, EventArgs.Empty);
                }
                else
                {
                    dvMessage.InnerHtml = "<strong>Unit Part not found for provided Model Number.</strong>";
                    dvMessage.Attributes.Add("class", "alert alert-error");
                    dvMessage.Visible = true;
                }
            }
        }

        protected void drpCard_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (drpCard.SelectedValue.ToString() == "-1")
            {
                dvCard.Visible = true;
                dvCVV.Visible = false;
                rblVisa.Checked = true;
            }
            else if (drpCard.SelectedValue.ToString() == "0")
            {
                dvCard.Visible = false;
                dvCVV.Visible = false;
            }
            else
            {
                dvCard.Visible = false;
                dvCVV.Visible = true;
            }

        }

        protected void lstImageCool_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            if (e.CommandName == "RemoveCoolImage" && e.CommandArgument.ToString() != "0")
            {
                objClientUnitPicturesService = ServiceFactory.ClientUnitPicturesService;
                int ImageId = Convert.ToInt32(e.CommandArgument.ToString());
                DataTable dtImage = new DataTable();
                objClientUnitPicturesService.DeleteClientUnitPictures(ImageId, ref dtImage);
                if (dtImage.Rows.Count > 0)
                {
                    string ImageName = dtImage.Rows[0]["ImageName"].ToString();
                    string imageFilePath = Server.MapPath(@"~/uploads/unitImages/" + ImageName);
                    FileInfo file = new FileInfo(imageFilePath);
                    if (file.Exists)
                    {
                        file.Delete();
                    }
                }

                Response.Redirect(Application["SiteAddress"] + "admin/ClientAcUnit_AddEdit.aspx?CUnitId=" + Request.QueryString["CUnitId"].ToString());
            }
        }

        protected void lstManualCool_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            if (e.CommandName == "RemoveManualCool" && e.CommandArgument.ToString() != "0")
            {
                objClientUnitManualsService = ServiceFactory.ClientUnitManualsService;
                int ManualId = Convert.ToInt32(e.CommandArgument.ToString());
                DataTable dtManual = new DataTable();
                objClientUnitManualsService.DeleteClientUnitManuals(ManualId, ref dtManual);
                if (dtManual.Rows.Count > 0)
                {
                    string ManualName = dtManual.Rows[0]["ManualName"].ToString();
                    string manualFilePath = Server.MapPath(@"~/uploads/unitManuals/" + ManualName);
                    FileInfo file = new FileInfo(manualFilePath);
                    if (file.Exists)
                    {
                        file.Delete();
                    }
                }

                Response.Redirect(Application["SiteAddress"] + "admin/ClientAcUnit_AddEdit.aspx?CUnitId=" + Request.QueryString["CUnitId"].ToString());
            }
        }

        protected void lstImageHeat_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            if (e.CommandName == "RemoveHeatImage" && e.CommandArgument.ToString() != "0")
            {
                objClientUnitPicturesService = ServiceFactory.ClientUnitPicturesService;
                int ImageId = Convert.ToInt32(e.CommandArgument.ToString());
                DataTable dtImage = new DataTable();
                objClientUnitPicturesService.DeleteClientUnitPictures(ImageId, ref dtImage);
                if (dtImage.Rows.Count > 0)
                {
                    string ImageName = dtImage.Rows[0]["ImageName"].ToString();
                    string imageFilePath = Server.MapPath(@"~/uploads/unitImages/" + ImageName);
                    FileInfo file = new FileInfo(imageFilePath);
                    if (file.Exists)
                    {
                        file.Delete();
                    }
                }

                Response.Redirect(Application["SiteAddress"] + "admin/ClientAcUnit_AddEdit.aspx?CUnitId=" + Request.QueryString["CUnitId"].ToString());
            }
        }

        protected void lstManualHeat_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            if (e.CommandName == "RemoveManualHeat" && e.CommandArgument.ToString() != "0")
            {
                objClientUnitManualsService = ServiceFactory.ClientUnitManualsService;
                int ManualId = Convert.ToInt32(e.CommandArgument.ToString());
                DataTable dtManual = new DataTable();
                objClientUnitManualsService.DeleteClientUnitManuals(ManualId, ref dtManual);
                if (dtManual.Rows.Count > 0)
                {
                    string ManualName = dtManual.Rows[0]["ManualName"].ToString();
                    string manualFilePath = Server.MapPath(@"~/uploads/unitManuals/" + ManualName);
                    FileInfo file = new FileInfo(manualFilePath);
                    if (file.Exists)
                    {
                        file.Delete();
                    }
                }

                Response.Redirect(Application["SiteAddress"] + "admin/ClientAcUnit_AddEdit.aspx?CUnitId=" + Request.QueryString["CUnitId"].ToString());
            }
        }
    }
}