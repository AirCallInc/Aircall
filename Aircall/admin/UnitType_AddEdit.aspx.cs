using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Services;
using System.Data;
using System.IO;
using Aircall.Common;

namespace Aircall.admin
{
    public partial class UnitType_AddEdit : System.Web.UI.Page
    {
        IPartsService objPartsService;
        IUnitsService objUnitsService;
        IUnitManualsService objUnitManualsService;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindElectricalAndMaxBreakerDropdown();
                FillAllPartsDropdown();
                if (!string.IsNullOrEmpty(Request.QueryString["UnitId"]))
                {
                    BindUnitsByUnitId();
                }
            }
        }

        private void BindElectricalAndMaxBreakerDropdown()
        {
            string ElectricalService = General.GetSitesettingsValue("ElectricalServices");
            if (!string.IsNullOrEmpty(ElectricalService))
            {
                string[] ElecService = ElectricalService.Split(',');
                foreach (var item in ElecService)
                {
                    drpElecService.Items.Add(item.Trim());
                }
            }
            string MaxBreaker = General.GetSitesettingsValue("MaxBreaker");
            if (!string.IsNullOrEmpty(MaxBreaker))
            {
                string[] MaxBreakerArr = MaxBreaker.Split(',');
                foreach (var item in MaxBreakerArr)
                {
                    drpMaxBreaker.Items.Add(item.Trim());
                }
            }
        }

        private void BindUnitsByUnitId()
        {
            btnAdd.Text = "Update";

            int UnitId = Convert.ToInt32(Request.QueryString["UnitId"]);
            DataTable dtUnit = new DataTable();
            objUnitsService = ServiceFactory.UnitsService;
            objUnitsService.GetByUnitId(UnitId, ref dtUnit);
            if (dtUnit.Rows.Count > 0)
            {
                txtModelNo.Text = dtUnit.Rows[0]["ModelNumber"].ToString();
                txtSerial.Text = dtUnit.Rows[0]["SerialNumber"].ToString();
                txtMfgDate.Value = dtUnit.Rows[0]["MFGMonthYear"].ToString();
                txtMfgBrand.Text = dtUnit.Rows[0]["ManufactureBrand"].ToString();
                txtTon.Text = dtUnit.Rows[0]["UnitTon"].ToString();
                //drpBooster.SelectedValue = dtUnit.Rows[0]["Booster"].ToString();
                drpRefType.SelectedValue = dtUnit.Rows[0]["Refrigerant"].ToString();
                drpElecService.SelectedValue = dtUnit.Rows[0]["ElectricalService"].ToString();
                drpMaxBreaker.SelectedValue = dtUnit.Rows[0]["MaxBreaker"].ToString();
                drpBreaker.SelectedValue = dtUnit.Rows[0]["Breaker"].ToString();
                drpCompressor.SelectedValue = dtUnit.Rows[0]["Compressor"].ToString();
                drpCapacitor.SelectedValue = dtUnit.Rows[0]["Capacitor"].ToString();
                drpContactor.SelectedValue = dtUnit.Rows[0]["Contactor"].ToString();
                drpFilterdryer.SelectedValue = dtUnit.Rows[0]["Filterdryer"].ToString();
                drpDefrostboard.SelectedValue = dtUnit.Rows[0]["Defrostboard"].ToString();
                drpRelay.SelectedValue = dtUnit.Rows[0]["Relay"].ToString();
                drpTXVValve.SelectedValue = dtUnit.Rows[0]["TXVValve"].ToString();
                drpReversingValve.SelectedValue = dtUnit.Rows[0]["ReversingValve"].ToString();
                drpBlowerMotor.SelectedValue = dtUnit.Rows[0]["BlowerMotor"].ToString();
                drpCondensingMotor.SelectedValue = dtUnit.Rows[0]["Condensingfanmotor"].ToString();
                drpInducer.SelectedValue = dtUnit.Rows[0]["Inducerdraftmotor"].ToString();
                drpTransformer.SelectedValue = dtUnit.Rows[0]["Transformer"].ToString();
                drpControlboard.SelectedValue = dtUnit.Rows[0]["Controlboard"].ToString();
                drpLimitSwitch.SelectedValue = dtUnit.Rows[0]["Limitswitch"].ToString();
                drpIgnitor.SelectedValue = dtUnit.Rows[0]["Ignitor"].ToString();
                drpGas.SelectedValue = dtUnit.Rows[0]["Gasvalve"].ToString();
                drpPressureswitch.SelectedValue = dtUnit.Rows[0]["Pressureswitch"].ToString();
                drpFlamesensor.SelectedValue = dtUnit.Rows[0]["Flamesensor"].ToString();
                drpRolloutsensor.SelectedValue = dtUnit.Rows[0]["Rolloutsensor"].ToString();
                drpDoorswitch.SelectedValue = dtUnit.Rows[0]["Doorswitch"].ToString();
                drpIgControlBoard.SelectedValue = dtUnit.Rows[0]["Ignitioncontrolboard"].ToString();
                drpCoilCleaner.SelectedValue = dtUnit.Rows[0]["Coil"].ToString();
                drpMisc.SelectedValue = dtUnit.Rows[0]["Misc"].ToString();
                chkActive.Checked = Convert.ToBoolean(dtUnit.Rows[0]["Status"].ToString());

                DataTable dtUnitManuals = new DataTable();
                objUnitManualsService = ServiceFactory.UnitManualsService;
                objUnitManualsService.GetUnitManualsByUnitId(UnitId, ref dtUnitManuals);
                if (dtUnitManuals.Rows.Count>0)
                {
                    lstManulas.DataSource = dtUnitManuals;
                }
                lstManulas.DataBind();
            }
        }

        private void FillAllPartsDropdown()
        {
            //string PartTypeName = "Booster";
            //DropDownList drpControls = (DropDownList)PNLParts.FindControl("drpBooster");
            //FillPartsDropdown(PartTypeName, drpControls);

            string PartTypeName = "Refrigerant";
            DropDownList drpControls = (DropDownList)PNLParts.FindControl("drpRefType");
            FillPartsDropdown(PartTypeName, drpControls);

            PartTypeName = "Breaker";
            drpControls = (DropDownList)PNLParts.FindControl("drpBreaker");
            FillPartsDropdown(PartTypeName, drpControls);

            PartTypeName = "Compressor";
            drpControls = (DropDownList)PNLParts.FindControl("drpCompressor");
            FillPartsDropdown(PartTypeName, drpControls);

            PartTypeName = "Capacitor";
            drpControls = (DropDownList)PNLParts.FindControl("drpCapacitor");
            FillPartsDropdown(PartTypeName, drpControls);

            PartTypeName = "Contactor";
            drpControls = (DropDownList)PNLParts.FindControl("drpContactor");
            FillPartsDropdown(PartTypeName, drpControls);

            PartTypeName = "Filter dryer";
            drpControls = (DropDownList)PNLParts.FindControl("drpFilterdryer");
            FillPartsDropdown(PartTypeName, drpControls);

            PartTypeName = "Defrost board";
            drpControls = (DropDownList)PNLParts.FindControl("drpDefrostboard");
            FillPartsDropdown(PartTypeName, drpControls);

            PartTypeName = "Relay";
            drpControls = (DropDownList)PNLParts.FindControl("drpRelay");
            FillPartsDropdown(PartTypeName, drpControls);

            PartTypeName = "TXV Valve";
            drpControls = (DropDownList)PNLParts.FindControl("drpTXVValve");
            FillPartsDropdown(PartTypeName, drpControls);

            PartTypeName = "Reversing Valve";
            drpControls = (DropDownList)PNLParts.FindControl("drpReversingValve");
            FillPartsDropdown(PartTypeName, drpControls);

            PartTypeName = "Blower Motor";
            drpControls = (DropDownList)PNLParts.FindControl("drpBlowerMotor");
            FillPartsDropdown(PartTypeName, drpControls);

            PartTypeName = "Condensing fan motor";
            drpControls = (DropDownList)PNLParts.FindControl("drpCondensingMotor");
            FillPartsDropdown(PartTypeName, drpControls);

            PartTypeName = "Inducer draft motor/ flu vent motor";
            drpControls = (DropDownList)PNLParts.FindControl("drpInducer");
            FillPartsDropdown(PartTypeName, drpControls);

            PartTypeName = "Transformer";
            drpControls = (DropDownList)PNLParts.FindControl("drpTransformer");
            FillPartsDropdown(PartTypeName, drpControls);

            PartTypeName = "Control board";
            drpControls = (DropDownList)PNLParts.FindControl("drpControlboard");
            FillPartsDropdown(PartTypeName, drpControls);

            PartTypeName = "Limit switch";
            drpControls = (DropDownList)PNLParts.FindControl("drpLimitSwitch");
            FillPartsDropdown(PartTypeName, drpControls);

            PartTypeName = "Ignitor";
            drpControls = (DropDownList)PNLParts.FindControl("drpIgnitor");
            FillPartsDropdown(PartTypeName, drpControls);

            PartTypeName = "Gas valve";
            drpControls = (DropDownList)PNLParts.FindControl("drpGas");
            FillPartsDropdown(PartTypeName, drpControls);

            PartTypeName = "Pressure switch";
            drpControls = (DropDownList)PNLParts.FindControl("drpPressureswitch");
            FillPartsDropdown(PartTypeName, drpControls);

            PartTypeName = "Flame sensor";
            drpControls = (DropDownList)PNLParts.FindControl("drpFlamesensor");
            FillPartsDropdown(PartTypeName, drpControls);

            PartTypeName = "Roll out sensor";
            drpControls = (DropDownList)PNLParts.FindControl("drpRolloutsensor");
            FillPartsDropdown(PartTypeName, drpControls);

            PartTypeName = "Door switch";
            drpControls = (DropDownList)PNLParts.FindControl("drpDoorswitch");
            FillPartsDropdown(PartTypeName, drpControls);

            PartTypeName = "Ignition control board";
            drpControls = (DropDownList)PNLParts.FindControl("drpIgControlBoard");
            FillPartsDropdown(PartTypeName, drpControls);

            PartTypeName = "Coil";
            drpControls = (DropDownList)PNLParts.FindControl("drpCoilCleaner");
            FillPartsDropdown(PartTypeName, drpControls);

            PartTypeName = "Misc";
            drpControls = (DropDownList)PNLParts.FindControl("drpMisc");
            FillPartsDropdown(PartTypeName, drpControls);
        }

        private void FillPartsDropdown(string PartTypeName, DropDownList drpControls)
        {
            DataTable dtFilter = new DataTable();
            dtFilter = FindParts(PartTypeName);
            if (dtFilter.Rows.Count > 0)
            {
                if (drpControls != null)
                {
                    drpControls.DataSource = dtFilter;
                    drpControls.DataTextField = dtFilter.Columns["PartSize"].ToString();
                    drpControls.DataValueField = dtFilter.Columns["Id"].ToString();
                }
            }
            if (drpControls != null)
            {
                drpControls.DataBind();
                drpControls.Items.Insert(0, new ListItem("Select " + PartTypeName, "0"));
            }
        }

        private DataTable FindParts(string PartTypeName)
        {
            objPartsService = ServiceFactory.PartsService;
            DataTable dtParts = new DataTable();
            objPartsService.GetAllPartsByPartType(PartTypeName, ref dtParts);
            return dtParts;
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    if (Session["LoginSession"] != null)
                    {
                        LoginModel objLoginModel = new LoginModel();
                        objLoginModel = Session["LoginSession"] as LoginModel;

                        //Validation of Manuals Start
                        string UnitManuals = string.Empty;
                        if (fupdUnitManual.HasFiles)
                        {
                            string ManualFIleFormat = ".pdf";
                            foreach (var item in fupdUnitManual.PostedFiles)
                            {
                                if (!ManualFIleFormat.Contains(item.FileName.Substring(item.FileName.LastIndexOf('.'))))
                                {
                                    dvMessage.InnerHtml = "<strong>Please select file of type: " + ManualFIleFormat + "</strong>";
                                    dvMessage.Attributes.Add("class", "alert alert-error");
                                    dvMessage.Visible = true;
                                    return;
                                }
                            }
                            foreach (var item in fupdUnitManual.PostedFiles)
                            {
                                string filePath = Path.Combine(Server.MapPath("~/uploads/unitPartManuals/"), item.FileName.Replace(' ', '_'));
                                item.SaveAs(filePath);
                                if (string.IsNullOrEmpty(UnitManuals))
                                    UnitManuals = item.FileName.Replace(' ', '_');
                                else
                                    UnitManuals = UnitManuals + '|' + item.FileName.Replace(' ', '_');
                            }
                        }
                        //Validation of Manuals End


                        BizObjects.Units objUnits = new BizObjects.Units();
                        objUnitsService = ServiceFactory.UnitsService;

                        objUnits.ModelNumber = txtModelNo.Text.Trim();
                        objUnits.SerialNumber = txtSerial.Text.Trim();
                        string []date = txtMfgDate.Value.Trim().Split('/');
                        string strDate = date[0].ToString() + "/01/" + date[1].ToString();
                        objUnits.ManufactureDate = Convert.ToDateTime(strDate);
                        objUnits.ManufactureBrand = txtMfgBrand.Text.Trim();
                        objUnits.UnitTon = txtTon.Text.Trim();
                        //objUnits.Booster = Convert.ToInt32(drpBooster.SelectedValue.ToString());
                        objUnits.Refrigerant = Convert.ToInt32(drpRefType.SelectedValue.ToString());
                        objUnits.ElectricalService = drpElecService.SelectedValue.ToString();
                        objUnits.MaxBreaker = drpMaxBreaker.SelectedValue.Trim();
                        objUnits.Breaker = Convert.ToInt32(drpBreaker.SelectedValue.ToString());
                        objUnits.Compressor = Convert.ToInt32(drpCompressor.SelectedValue.ToString());
                        objUnits.Capacitor = Convert.ToInt32(drpCapacitor.SelectedValue.ToString());
                        objUnits.Contactor = Convert.ToInt32(drpContactor.SelectedValue.ToString());
                        objUnits.Filterdryer = Convert.ToInt32(drpFilterdryer.SelectedValue.ToString());
                        objUnits.Defrostboard = Convert.ToInt32(drpDefrostboard.SelectedValue.ToString());
                        objUnits.Relay = Convert.ToInt32(drpRelay.SelectedValue.ToString());
                        objUnits.TXVValve = Convert.ToInt32(drpTXVValve.SelectedValue.ToString());
                        objUnits.ReversingValve = Convert.ToInt32(drpReversingValve.SelectedValue.ToString());
                        objUnits.BlowerMotor = Convert.ToInt32(drpBlowerMotor.SelectedValue.ToString());
                        objUnits.Condensingfanmotor = Convert.ToInt32(drpCondensingMotor.SelectedValue.ToString());
                        objUnits.Inducerdraftmotor = Convert.ToInt32(drpInducer.SelectedValue.ToString());
                        objUnits.Transformer = Convert.ToInt32(drpTransformer.SelectedValue.ToString());
                        objUnits.Controlboard = Convert.ToInt32(drpControlboard.SelectedValue.ToString());
                        objUnits.Limitswitch = Convert.ToInt32(drpLimitSwitch.SelectedValue.ToString());
                        objUnits.Ignitor = Convert.ToInt32(drpIgnitor.SelectedValue.ToString());
                        objUnits.Gasvalve = Convert.ToInt32(drpGas.SelectedValue.ToString());
                        objUnits.Pressureswitch = Convert.ToInt32(drpPressureswitch.SelectedValue.ToString());
                        objUnits.Flamesensor = Convert.ToInt32(drpFlamesensor.SelectedValue.ToString());
                        objUnits.Rolloutsensor = Convert.ToInt32(drpRolloutsensor.SelectedValue.ToString());
                        objUnits.Doorswitch = Convert.ToInt32(drpDoorswitch.SelectedValue.ToString());
                        objUnits.Ignitioncontrolboard = Convert.ToInt32(drpIgControlBoard.SelectedValue.ToString());
                        objUnits.Coil = Convert.ToInt32(drpCoilCleaner.SelectedValue.ToString());
                        objUnits.Misc = Convert.ToInt32(drpMisc.SelectedValue.ToString());
                        objUnits.Status = chkActive.Checked;
                        objUnits.AddedBy = objLoginModel.Id;
                        objUnits.AddedByType = objLoginModel.RoleId;
                        objUnits.AddedDate = DateTime.UtcNow;
                        int UnitId = 0;

                        if (!string.IsNullOrEmpty(Request.QueryString["UnitId"]))
                        {
                            UnitId = Convert.ToInt32(Request.QueryString["UnitId"].ToString());
                            objUnits.Id = UnitId;
                            objUnits.UpdatedBy = objLoginModel.Id;
                            objUnits.UpdatedByType = objLoginModel.RoleId;
                            objUnits.UpdatedDate = DateTime.UtcNow;

                            objUnitsService.UpdateUnit(ref objUnits);
                        }
                        else
                        {
                            UnitId = objUnitsService.AddUnit(ref objUnits);
                        }
                        if (!string.IsNullOrEmpty(UnitManuals))
                        {
                            if (UnitId != 0)
                            {
                                objUnitManualsService = ServiceFactory.UnitManualsService;
                                BizObjects.UnitManuals objUnitManuals = new BizObjects.UnitManuals();

                                foreach (var item in UnitManuals.Split('|'))
                                {
                                    objUnitManuals.UnitsId = UnitId;
                                    objUnitManuals.ManualFileName = item;
                                    objUnitManualsService.AddUnitManuals(ref objUnitManuals);
                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(Request.QueryString["UnitId"]))
                        {
                            Session["msg"] = "edit";
                            Response.Redirect(Application["SiteAddress"] + "admin/UnitType_List.aspx");
                        }
                        else
                        {
                            Session["msg"] = "add";
                            Response.Redirect(Application["SiteAddress"] + "admin/UnitType_List.aspx");
                        }
                    }
                    else
                        Response.Redirect(Application["SiteAddress"] + "admin/Login.aspx");
                }
                catch (Exception Ex)
                {
                    dvMessage.InnerHtml = "<strong>Error!</strong> " + Ex.Message.ToString().Trim();
                    dvMessage.Attributes.Add("class", "alert alert-error");
                    dvMessage.Visible = true;
                }
            }
        }

        protected void lstManulas_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            if (e.CommandName=="RemoveManual" && e.CommandArgument.ToString() !="0")
            {
                objUnitManualsService = ServiceFactory.UnitManualsService;
                int ManualId = Convert.ToInt32(e.CommandArgument.ToString());
                DataTable dtManual = new DataTable();
                objUnitManualsService.DeleteUnitManual(ManualId, ref dtManual);
                if (dtManual.Rows.Count>0)
                {
                    string ManualName = dtManual.Rows[0]["ManualName"].ToString();
                    string imageFilePath = Server.MapPath(@"~/uploads/unitPartManuals/" + ManualName);
                    FileInfo file = new FileInfo(imageFilePath);
                    if (file.Exists)
                    {
                        file.Delete();
                    }
                }
                Response.Redirect(Application["SiteAddress"] + "admin/UnitType_AddEdit.aspx?UnitId=" + Request.QueryString["UnitId"].ToString());
            }
        }
    }
}