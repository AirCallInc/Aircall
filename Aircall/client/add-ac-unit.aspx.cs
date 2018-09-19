using Aircall.Common;
using AuthorizeNet.Api.Contracts.V1;
using AuthorizeNetLib;
using Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Aircall.client
{
    public partial class add_ac_unit : System.Web.UI.Page
    {
        IClientUnitService objClientUnitService;
        IPlanService objPlanService;
        IPartsService objPartsService;
        IClientAddressService objClientAddressService;
        IStateService objStateService;
        ICitiesService objCitiesService;
        IClientUnitPartService objClientUnitPartService;
        IUnitExtraInfoService objUnitExtraInfoService;
        IZipCodeService objZipCodeService;

        int ClientId = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["ClientLoginCookie"] == null)
            {
                Response.Redirect(Application["SiteAddress"] + "sign-in.aspx");
            }
            if (!IsPostBack)
            {
                BindPlanTypeDropdown();
                BindUnitTypeDropdown();
                ClientId = (Session["ClientLoginCookie"] as LoginModel).Id;
                BindAddressByClientId(ClientId);
                FillStateDropdown();
                fillBooster();
                drpPlan_SelectedIndexChanged(null, null);
                //int Qty = BindSummary(ClientId);
                int Qty = 5;
                txtQty.Attributes.Add("max", Qty.ToString());
                rvQty.MaximumValue = Qty.ToString();
                rvQty.ErrorMessage = string.Format("Quantity must be between 1 to {0}", Qty);
            }
            ChangeAddressValidation(false);
        }

        private void BindPlanTypeDropdown()
        {
            objPlanService = ServiceFactory.PlanService;
            DataTable dtPlanType = new DataTable();
            objPlanService.GetAllPlanType(ref dtPlanType);
            if (dtPlanType.Rows.Count > 0)
            {
                drpPlan.DataSource = dtPlanType;
                drpPlan.DataTextField = dtPlanType.Columns["PlanName"].ToString();
                drpPlan.DataValueField = dtPlanType.Columns["Id"].ToString();
                drpPlan.DataBind();
            }

            drpPlan.SelectedIndex = 0;
        }

        private void BindAddressByClientId(int ClientId)
        {
            objClientAddressService = ServiceFactory.ClientAddressService;
            DataTable dtAddress = new DataTable();
            objClientAddressService.GetClientAddressesByClientId(ClientId, false, ref dtAddress);

            string filter = "ShowInList = True";
            DataView dv = new DataView(dtAddress, filter, "", DataViewRowState.CurrentRows);
            dtAddress = dv.ToTable();

            var rows = dtAddress.Select(" IsDefaultAddress = true ");

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
            if (dtAddress.Rows.Count > 0 && dtAddress.Rows.Count < 2)
            {
                if (rows.Length > 0)
                {
                    var row = rows.First();
                    drpAddress.SelectedValue = row["Id"].ToString();
                }
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
                drpUnitType.Items.Insert(0, new ListItem("Select Unit Type", "0"));
            }
        }

        private void FillStateDropdown()
        {
            objStateService = ServiceFactory.StateService;
            DataTable dtStates = new DataTable();
            objStateService.GetAllStates(true, false, ref dtStates);
            if (dtStates.Rows.Count > 0)
            {
                drpState.DataSource = dtStates;
                drpState.DataTextField = dtStates.Columns["Name"].ToString();
                drpState.DataValueField = dtStates.Columns["Id"].ToString();

            }
            drpState.DataBind();
            drpState.Items.Insert(0, new ListItem("Select State", "0"));
            var rows = dtStates.Select(" IsDefault = True ");
            if (rows.Length > 0)
            {
                drpState.SelectedValue = rows[0]["Id"].ToString();
                drpState_SelectedIndexChanged(drpState, EventArgs.Empty);
            }
            drpState_SelectedIndexChanged(drpState, EventArgs.Empty);
        }

        private void BindCityFromState(int StateId)
        {
            objCitiesService = ServiceFactory.CitiesService;
            DataTable dtCities = new DataTable();
            objCitiesService.GetAllCityByStateId(StateId, false, ref dtCities);
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

        private void FillAllDropdowns()
        {
            string PartTypeName = "Filter";
            DropDownList drpFilterSingle = (DropDownList)pnlSingle.FindControl("drpFilterSizeCool1");

            FillPartsDropdown(PartTypeName, drpFilterSingle, null);



            PartTypeName = "Fuse";
            DropDownList drpFuseSingle = (DropDownList)pnlSingle.FindControl("drpFuseTypeCool1");

            FillPartsDropdown(PartTypeName, drpFuseSingle, null);
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
                    CoolControl.DataTextField = dtFilter.Columns["PartSize"].ToString();
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
                dvAddress.Attributes["style"] = "display:block";
                FillStateDropdown();
                ChangeAddressValidation(true);
            }
            else
            {
                dvAddress.Attributes["style"] = "display:none";
                ChangeAddressValidation(false);
            }
        }
        public void ChangeAddressValidation(bool status)
        {
            rqfvtxtAddress.Enabled = status;
            rqfvState.Enabled = status;
            rqfvCity.Enabled = status;
            rqfvZip.Enabled = status;
        }
        protected void drpUnitType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (drpUnitType.SelectedValue == "2")
            {
                pnlSplit.Attributes["style"] = "display:block";
                pnlSingle.Attributes["style"] = "display:none";
            }
            else
            {
                if (drpUnitType.SelectedValue != "0")
                {
                    h6.InnerText = drpUnitType.SelectedItem.Text + " Information";
                    pnlSingle.Attributes["style"] = "display:block";
                    pnlSplit.Attributes["style"] = "display:none";
                }
                else
                {
                    pnlSingle.Attributes["style"] = "display:none";
                    pnlSplit.Attributes["style"] = "display:none";
                }

            }
        }
        public void fillBooster()
        {
            string PartTypeName = "Thermostat";
            FillPartsDropdown(PartTypeName, drpBoosterCool, null);
            FillPartsDropdown(PartTypeName, drpBoosterSingle, null);
            FillPartsDropdown(PartTypeName, drpBoosterHeat, null);
        }
        protected void drpFilterQtyCool_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("id");

            for (int i = 1; i <= int.Parse(drpFilterQtyCool.SelectedItem.Value); i++)
            {
                dt.Rows.Add(i);
            }
            rptCoolingFilter.DataSource = dt;
            rptCoolingFilter.DataBind();
        }

        protected void drpFilterQtySingle_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("id");

            for (int i = 1; i <= int.Parse(drpFilterQtySingle.SelectedItem.Value); i++)
            {
                dt.Rows.Add(i);
            }
            rptSingleFilter.DataSource = dt;
            rptSingleFilter.DataBind();
        }

        protected void drpFilterQtyHeat_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("id");

            for (int i = 1; i <= int.Parse(drpFilterQtyHeat.SelectedItem.Value); i++)
            {
                dt.Rows.Add(i);
            }
            rptHeatingFilter.DataSource = dt;
            rptHeatingFilter.DataBind();
        }

        protected void drpFuseQtyCool_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("id");

            for (int i = 1; i <= int.Parse(drpFuseQtyCool.SelectedItem.Value); i++)
            {
                dt.Rows.Add(i);
            }
            rptCoolingFuses.DataSource = dt;
            rptCoolingFuses.DataBind();
        }

        protected void drpFuseQtyHeat_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("id");

            for (int i = 1; i <= int.Parse(drpFuseQtyHeat.SelectedItem.Value); i++)
            {
                dt.Rows.Add(i);
            }
            rptHeatingFuses.DataSource = dt;
            rptHeatingFuses.DataBind();
        }

        protected void drpFuseQtySingle_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("id");

            for (int i = 1; i <= int.Parse(drpFuseQtySingle.SelectedItem.Value); i++)
            {
                dt.Rows.Add(i);
            }
            rptSingleFuses.DataSource = dt;
            rptSingleFuses.DataBind();
        }

        protected void rptSingleFilter_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item)
            {
                string PartTypeName = "Filter";
                DropDownList drpFilterSingle = (DropDownList)e.Item.FindControl("drpFilterSizeCool1");
                FillPartsDropdown(PartTypeName, drpFilterSingle, null);
            }
            if (e.Item.ItemType == ListItemType.AlternatingItem)
            {
                string PartTypeName = "Filter";
                DropDownList drpFilterSingle = (DropDownList)e.Item.FindControl("drpFilterSizeCool1");
                FillPartsDropdown(PartTypeName, drpFilterSingle, null);
            }
        }

        protected void rptCoolingFilter_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item)
            {
                string PartTypeName = "Filter";
                DropDownList drpFilterSizeCool1 = (DropDownList)e.Item.FindControl("drpFilterSizeCool1");
                FillPartsDropdown(PartTypeName, drpFilterSizeCool1, null);
            }
            if (e.Item.ItemType == ListItemType.AlternatingItem)
            {
                string PartTypeName = "Filter";
                DropDownList drpFilterSizeCool1 = (DropDownList)e.Item.FindControl("drpFilterSizeCool1");
                FillPartsDropdown(PartTypeName, drpFilterSizeCool1, null);
            }
        }

        protected void rptHeatingFilter_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item)
            {
                string PartTypeName = "Filter";
                DropDownList drpFilterSizeHeat = (DropDownList)e.Item.FindControl("drpFilterSizeHeat");
                FillPartsDropdown(PartTypeName, drpFilterSizeHeat, null);
            }
            if (e.Item.ItemType == ListItemType.AlternatingItem)
            {
                string PartTypeName = "Filter";
                DropDownList drpFilterSizeHeat = (DropDownList)e.Item.FindControl("drpFilterSizeHeat");
                FillPartsDropdown(PartTypeName, drpFilterSizeHeat, null);
            }
        }

        protected void rptSingleFuses_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item)
            {
                string PartTypeName = "Fuse";
                DropDownList drpFilterSingle = (DropDownList)e.Item.FindControl("drpFuseTypeSingle");
                FillPartsDropdown(PartTypeName, drpFilterSingle, null);
            }
            if (e.Item.ItemType == ListItemType.AlternatingItem)
            {
                string PartTypeName = "Fuse";
                DropDownList drpFilterSingle = (DropDownList)e.Item.FindControl("drpFuseTypeSingle");
                FillPartsDropdown(PartTypeName, drpFilterSingle, null);
            }
        }

        protected void rptCoolingFuses_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item)
            {
                string PartTypeName = "Fuse";
                DropDownList drpFuseTypeCool = (DropDownList)e.Item.FindControl("drpFuseTypeCool");
                FillPartsDropdown(PartTypeName, drpFuseTypeCool, null);
            }
            if (e.Item.ItemType == ListItemType.AlternatingItem)
            {
                string PartTypeName = "Fuse";
                DropDownList drpFuseTypeCool = (DropDownList)e.Item.FindControl("drpFuseTypeCool");
                FillPartsDropdown(PartTypeName, drpFuseTypeCool, null);
            }
        }

        protected void rptHeatingFuses_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item)
            {
                string PartTypeName = "Fuse";
                DropDownList drpFuseTypeHeat = (DropDownList)e.Item.FindControl("drpFuseTypeHeat");
                FillPartsDropdown(PartTypeName, drpFuseTypeHeat, null);
            }
            if (e.Item.ItemType == ListItemType.AlternatingItem)
            {
                string PartTypeName = "Fuse";
                DropDownList drpFuseTypeHeat = (DropDownList)e.Item.FindControl("drpFuseTypeHeat");
                FillPartsDropdown(PartTypeName, drpFuseTypeHeat, null);
            }
        }

        protected void chkProvideInfo_CheckedChanged(object sender, EventArgs e)
        {
            if (chkProvideInfo.Checked)
            {
                dvUnitTypes.Attributes["style"] = "display:block";
            }
            else
            {
                dvUnitTypes.Attributes["style"] = "display:none";
            }
        }
        private string generateUnitName(int ClientId, int cnt = 0)
        {
            ClientId = (Session["ClientLoginCookie"] as LoginModel).Id;
            var dtClientUnitCnt = new DataTable();
            objClientUnitService = ServiceFactory.ClientUnitService;
            objClientUnitService.GetClientUnitsByClientId(ClientId, ref dtClientUnitCnt);
            var unitCount = (cnt == 0 ? dtClientUnitCnt.Rows.Count : cnt) + 1;
            string name = "AC" + unitCount;
            DataTable dtUnits = new DataTable();
            objClientUnitService.GetClientUnitsByClientIdUnitName(ClientId, name, ref dtUnits);
            var unitExists = dtUnits.Rows.Count;
            if (unitExists > 0)
            {
                name = generateUnitName(ClientId, unitCount);
            }
            return name;
        }
        private int BindSummary(int ClientId)
        {

            DataTable dtClient = new DataTable();
            objClientUnitService.GetClientUnitsByClientId(ClientId, ref dtClient);

            var rows = dtClient.Select(" PaymentStatus='NotReceived' OR PaymentStatus='Failed' AND AddedBy=" + ClientId.ToString() + " AND AddedByType=" + General.UserRoles.Client.GetEnumValue().ToString() + "");
            if (rows.Length >= 5)
            {
                return 0;
            }
            else
            {
                return 5 - rows.Length;
            }
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(drpPlan.SelectedValue.ToString()) <= 0)
            {
                dvMessage.InnerText = "Please Select Plan.";
                dvMessage.Attributes.Add("class", "error");
                dvMessage.Visible = true;
                return;
            }

            //Validation of Zipcode Start
            if (drpAddress.SelectedValue == "-1")
            {
                if (drpState.SelectedValue == "0")
                {
                    dvMessage.InnerText = "Please Select State.";
                    dvMessage.Attributes.Add("class", "error");
                    dvMessage.Visible = true;
                    return;
                }
                if (drpCity.SelectedValue == "0")
                {
                    dvMessage.InnerText = "Please Select City.";
                    dvMessage.Attributes.Add("class", "error");
                    dvMessage.Visible = true;
                    return;
                }
                objZipCodeService = ServiceFactory.ZipCodeService;
                DataTable dtZipCode = new DataTable();
                objClientAddressService = ServiceFactory.ClientAddressService;
                int rtn = objClientAddressService.ValidateZipcode(Convert.ToInt32(drpState.SelectedValue), Convert.ToInt32(drpCity.SelectedValue), txtZip.Text.Trim());
                if (rtn == -1)
                {
                    dvMessage.InnerHtml = General.GetSitesettingsValue("ServiceInactiveZipCodeMessage"); //"<strong>Service in this zip code is inactivate.</strong>";
                    dvMessage.Attributes.Add("class", "error");
                    dvMessage.Visible = true;
                    return;
                }
                else if (rtn == -2)
                {
                    dvMessage.InnerHtml = "<strong>Please enter a valid Zip Code.</strong>";
                    dvMessage.Attributes.Add("class", "error");
                    dvMessage.Visible = true;
                    return;
                }
            }
            ClientId = (Session["ClientLoginCookie"] as LoginModel).Id;
            if (!string.IsNullOrWhiteSpace(txtUnitName.Text))
            {
                DataTable dtUnits = new DataTable();
                objClientUnitService = ServiceFactory.ClientUnitService;
                objClientUnitService.GetClientUnitsByClientIdUnitName(ClientId, txtUnitName.Text.Trim(), ref dtUnits);
                var unitExists = dtUnits.Rows.Count;
                if (unitExists > 0)
                {
                    dvMessage.InnerHtml = "<strong>Unit Name Already Exists.</strong>";
                    dvMessage.Attributes.Add("class", "error");
                    dvMessage.Visible = true;
                    return;
                }
            }
            //Add New Client Address Start
            objPlanService = ServiceFactory.PlanService;
            int AddressId = 0;

            var dtClientUnitCnt = new DataTable();
            objClientUnitService = ServiceFactory.ClientUnitService;
            objClientUnitService.GetClientUnitsByClientId(ClientId, ref dtClientUnitCnt);

            if (drpAddress.SelectedValue == "-1")
            {
                string customerAddressId = "";
                var result = this.AddClientAddressToAuthorizeNet(ClientId, ref customerAddressId);

                if (!result)
                {
                    return;
                }

                BizObjects.ClientAddress objAddress = new BizObjects.ClientAddress();
                objAddress.ClientId = ClientId;
                objAddress.Address = txtAddress.Text.Trim();
                objAddress.State = Convert.ToInt32(drpState.SelectedValue.ToString());
                objAddress.City = Convert.ToInt32(drpCity.SelectedValue.ToString());
                objAddress.ZipCode = txtZip.Text.Trim();
                objAddress.AddedBy = ClientId;
                objAddress.AddedByType = (int)Aircall.Common.General.UserRoles.Client;
                objAddress.AddedDate = DateTime.UtcNow;
                objAddress.CustomerAddressId = customerAddressId;

                objClientAddressService = ServiceFactory.ClientAddressService;
                AddressId = objClientAddressService.AddClientAddress(ref objAddress);
            }
            else
            {
                AddressId = Convert.ToInt32(drpAddress.SelectedValue.ToString());
            }
            //Add New Client Address End
            bool added = false;
            int Qty = int.Parse(txtQty.Text);
            for (int j = 0; j < Qty; j++)
            {
                BizObjects.ClientUnit objClientUnit = new BizObjects.ClientUnit();
                int ClientUnitId = 0;

                //Add Client Unit Start
                objClientUnit.ClientId = ClientId;
                //objClientUnit
                objClientUnit.PlanTypeId = Convert.ToInt32(drpPlan.SelectedValue.ToString());

                objClientUnit.UnitName = (txtUnitName.Text.Trim() == "" ? generateUnitName(ClientId, 0) : (Qty > 1 ? txtUnitName.Text.Trim() + (j + 1).ToString() : txtUnitName.Text.Trim()));
                objClientUnit.AddressId = AddressId;
                var Packaged = drpUnitType.Items.FindByText("Packaged");
                objClientUnit.UnitTypeId = (Convert.ToInt32(drpUnitType.SelectedValue.ToString()) == 0 ? int.Parse(Packaged.Value) : Convert.ToInt32(drpUnitType.SelectedValue.ToString()));
                if (chkAutoRenewal.Checked)
                {
                    objClientUnit.AutoRenewal = true;
                }
                else
                {
                    objClientUnit.AutoRenewal = false;
                }
                if (chkSpecialOffer.Checked)
                {
                    objClientUnit.IsSpecialApplied = true;
                }
                else
                {
                    objClientUnit.IsSpecialApplied = false;
                }

                objClientUnit.Status = (int)Aircall.Common.General.UnitStatus.ServiceSoon;
                objClientUnit.PaymentStatus = Aircall.Common.General.PaymentStatus.NotReceived.ToString();
                objClientUnit.AddedBy = ClientId;
                objClientUnit.AddedByType = (int)Aircall.Common.General.UserRoles.Client;
                objClientUnit.AddedDate = DateTime.UtcNow;

                ClientUnitId = objClientUnitService.AddClientUnit(ref objClientUnit);
                var planTypeId = this.drpPlan.SelectedValue.ToString();
                var visitPerYear = Convert.ToInt32(this.drpVisitsPerYear.SelectedValue);
                objClientUnitService.UpdateUnitPricePerMonth(ClientUnitId, planTypeId, visitPerYear);
                //Add Client Unit End

                objClientUnitPartService = ServiceFactory.ClientUnitPartService;

                if (ClientUnitId != 0)
                {
                    //if (chkProvideInfo.Checked)
                    //{
                    AddUnitPart(ClientUnitId);
                    //}
                    added = true;
                }
            }
            if (added)
            {
                Response.Redirect(Application["SiteAddress"] + "client/summary.aspx?clientId=" + ClientId.ToString());
            }

        }

        private decimal GetPricePerMonth()
        {
            string planTypeId = this.drpPlan.SelectedValue.ToString();
            int visitPerYear = Convert.ToInt32(this.drpVisitsPerYear.SelectedValue);
            objClientUnitService = ServiceFactory.ClientUnitService;
            var pricePerMonth = objClientUnitService.GetPricePerMonth(planTypeId, visitPerYear);
            return pricePerMonth;
        }

        private bool AddClientAddressToAuthorizeNet(int clientId, ref string customerAddressId)
        {
            var objClientService = ServiceFactory.ClientService;
            DataTable dtClient = null;
            objClientService.GetClientById(clientId, ref dtClient);
            DataRow dr = dtClient.Rows[0];
            var firstName = dr["FirstName"].ToString();
            var lastName = dr["LastName"].ToString();
            var customerProfileId = dr["CustomerProfileId"].ToString();
            var cityName = this.drpCity.SelectedItem.Text;
            var stateName = this.drpState.SelectedItem.Text;

            customerAddressType address = new customerAddressType();
            address.firstName = firstName;
            address.lastName = lastName;
            address.address = this.txtAddress.Text;
            address.city = cityName;
            address.zip = this.txtZip.Text;
            address.state = stateName;

            var helper = new AuthorizeNetHelper();
            string errCode = "", errText = "";
            bool isSuccess = false;
            helper.CreateCustomerAddress(customerProfileId, address, ref isSuccess, ref customerAddressId, ref errCode, ref errText);

            if (isSuccess)
            {
                return true;
            }
            else
            {
                var errMsg = "";
                if (!string.IsNullOrEmpty(errText))
                {
                    errMsg = errText;
                }
                else
                {
                    errMsg = "Add address to AuthorizeNet failed.";
                }
                dvMessage.InnerHtml = "<strong>" + errMsg + "</strong>";
                dvMessage.Attributes.Add("class", "alert alert-error");
                dvMessage.Visible = true;
                return false;
            }
        }

        private void AddUnitPart(int ClientUnitId)
        {

            BizObjects.ClientUnitParts objClientUnitParts = new BizObjects.ClientUnitParts();
            objClientUnitParts.UnitId = ClientUnitId;
            if (drpUnitType.SelectedValue == "0")
            {
                objClientUnitParts.SplitType = "Packaged";
                objClientUnitParts.ModelNumber = txtModelNoSingle.Text.Trim();
                objClientUnitParts.SerialNumber = txtSerialSingle.Text.Trim();
                objClientUnitParts.ManufactureBrand = txtMfgBrandSingle.Text.Trim();
                if (!string.IsNullOrEmpty(txtMfgDateSingle.Text.Trim()))
                {
                    string[] date = txtMfgDateSingle.Text.Trim().Split('/');
                    string strDate = date[0].ToString() + "/01/" + date[1].ToString();
                    objClientUnitParts.ManufactureDate = Convert.ToDateTime(strDate);
                }
                objClientUnitParts.UnitTon = txtUnitTonSingle.Text.Trim();
                if (drpBoosterSingle.SelectedValue != "0")
                    objClientUnitParts.Thermostat = Convert.ToInt32(drpBoosterSingle.SelectedValue.ToString());

                long ClientUnitPartId = 0;
                ClientUnitPartId = objClientUnitPartService.AddClientUnitPartPortal(ref objClientUnitParts);
                //Add Extra Unit Info Start
                if (ClientUnitPartId != 0)
                {
                    objUnitExtraInfoService = ServiceFactory.UnitExtraInfoService;
                    if (drpFilterQtySingle.SelectedValue != "0")
                    {
                        BizObjects.UnitExtraInfo objUnitExtraInfoFilter = new BizObjects.UnitExtraInfo();
                        objUnitExtraInfoFilter.UnitId = ClientUnitId;
                        objUnitExtraInfoFilter.ExtraInfoType = Aircall.Common.General.UnitExtraInfoType.Filter.ToString();
                        objUnitExtraInfoFilter.ClientUnitPartId = ClientUnitPartId;
                        int QtyCnt = Convert.ToInt32(drpFilterQtySingle.SelectedValue);

                        for (int i = 0; i < rptSingleFilter.Items.Count; i++)
                        {
                            var item = rptSingleFilter.Items[i];
                            DropDownList drpFilterSize = (DropDownList)item.FindControl("drpFilterSizeCool1");
                            DropDownList drpFilterLocation = (DropDownList)item.FindControl("drpFilterLocationCool1");
                            objUnitExtraInfoFilter.PartId = Convert.ToInt32(drpFilterSize.SelectedValue.ToString());
                            //0=Inside Equipment 1=Inside Space
                            if (drpFilterLocation.SelectedValue == "0")
                                objUnitExtraInfoFilter.LocationOfPart = false;
                            else
                                objUnitExtraInfoFilter.LocationOfPart = true;

                            objUnitExtraInfoService.AddUnitExtraInfo(ref objUnitExtraInfoFilter);
                        }
                    }
                    if (drpFuseQtySingle.SelectedValue != "0")
                    {
                        BizObjects.UnitExtraInfo objUnitExtraInfoFuse = new BizObjects.UnitExtraInfo();
                        objUnitExtraInfoFuse.UnitId = ClientUnitId;
                        objUnitExtraInfoFuse.ExtraInfoType = Aircall.Common.General.UnitExtraInfoType.Fuses.ToString();
                        objUnitExtraInfoFuse.ClientUnitPartId = ClientUnitPartId;
                        int QtyCnt = Convert.ToInt32(drpFuseQtySingle.SelectedValue);
                        for (int i = 0; i < rptSingleFuses.Items.Count; i++)
                        {
                            var item = rptSingleFuses.Items[i];
                            DropDownList drpFuseType = (DropDownList)item.FindControl("drpFuseTypeSingle");
                            objUnitExtraInfoFuse.PartId = Convert.ToInt32(drpFuseType.SelectedValue.ToString());
                            objUnitExtraInfoService.AddUnitExtraInfo(ref objUnitExtraInfoFuse);
                        }
                    }
                }
            }
            else if (drpUnitType.SelectedValue != "2" && drpUnitType.SelectedValue != "0")
            {
                objClientUnitParts.SplitType = drpUnitType.SelectedItem.Text;
                objClientUnitParts.ModelNumber = txtModelNoSingle.Text.Trim();
                objClientUnitParts.SerialNumber = txtSerialSingle.Text.Trim();
                objClientUnitParts.ManufactureBrand = txtMfgBrandSingle.Text.Trim();
                if (!string.IsNullOrEmpty(txtMfgDateSingle.Text.Trim()))
                {
                    string[] date = txtMfgDateSingle.Text.Trim().Split('/');
                    string strDate = date[0].ToString() + "/01/" + date[1].ToString();
                    objClientUnitParts.ManufactureDate = Convert.ToDateTime(strDate);
                }

                objClientUnitParts.UnitTon = txtUnitTonSingle.Text.Trim();
                if (drpBoosterSingle.SelectedValue != "0")
                    objClientUnitParts.Thermostat = Convert.ToInt32(drpBoosterSingle.SelectedValue.ToString());


                long ClientUnitPartId = 0;
                ClientUnitPartId = objClientUnitPartService.AddClientUnitPartPortal(ref objClientUnitParts);
                //Add Extra Unit Info Start
                if (ClientUnitPartId != 0)
                {
                    objUnitExtraInfoService = ServiceFactory.UnitExtraInfoService;
                    if (drpFilterQtySingle.SelectedValue != "0")
                    {
                        BizObjects.UnitExtraInfo objUnitExtraInfoFilter = new BizObjects.UnitExtraInfo();
                        objUnitExtraInfoFilter.UnitId = ClientUnitId;
                        objUnitExtraInfoFilter.ExtraInfoType = Aircall.Common.General.UnitExtraInfoType.Filter.ToString();
                        objUnitExtraInfoFilter.ClientUnitPartId = ClientUnitPartId;
                        int QtyCnt = Convert.ToInt32(drpFilterQtySingle.SelectedValue);

                        for (int i = 0; i < rptSingleFilter.Items.Count; i++)
                        {
                            var item = rptSingleFilter.Items[i];
                            DropDownList drpFilterSize = (DropDownList)item.FindControl("drpFilterSizeCool1");
                            DropDownList drpFilterLocation = (DropDownList)item.FindControl("drpFilterLocationCool1");
                            objUnitExtraInfoFilter.PartId = Convert.ToInt32(drpFilterSize.SelectedValue.ToString());
                            //0=Inside Equipment 1=Inside Space
                            if (drpFilterLocation.SelectedValue == "0")
                                objUnitExtraInfoFilter.LocationOfPart = false;
                            else
                                objUnitExtraInfoFilter.LocationOfPart = true;

                            objUnitExtraInfoService.AddUnitExtraInfo(ref objUnitExtraInfoFilter);
                        }
                    }
                    if (drpFuseQtySingle.SelectedValue != "0")
                    {
                        BizObjects.UnitExtraInfo objUnitExtraInfoFuse = new BizObjects.UnitExtraInfo();
                        objUnitExtraInfoFuse.UnitId = ClientUnitId;
                        objUnitExtraInfoFuse.ExtraInfoType = Aircall.Common.General.UnitExtraInfoType.Fuses.ToString();
                        objUnitExtraInfoFuse.ClientUnitPartId = ClientUnitPartId;
                        int QtyCnt = Convert.ToInt32(drpFuseQtySingle.SelectedValue);
                        for (int i = 0; i < rptSingleFuses.Items.Count; i++)
                        {
                            var item = rptSingleFuses.Items[i];
                            DropDownList drpFuseType = (DropDownList)item.FindControl("drpFuseTypeSingle");
                            objUnitExtraInfoFuse.PartId = Convert.ToInt32(drpFuseType.SelectedValue.ToString());
                            objUnitExtraInfoService.AddUnitExtraInfo(ref objUnitExtraInfoFuse);
                        }
                    }
                }
            }
            else if (drpUnitType.SelectedValue == "2")
            {
                //cooling
                objClientUnitParts.SplitType = Aircall.Common.General.SplitType.Cooling.ToString();
                objClientUnitParts.ModelNumber = txtModelNoCool.Text.Trim();
                objClientUnitParts.SerialNumber = txtSerialCool.Text.Trim();
                objClientUnitParts.ManufactureBrand = txtMfgBrandCool.Text.Trim();
                if (!string.IsNullOrEmpty(txtMfgDateSingle.Text.Trim()))
                {
                    string[] date = txtMfgDateSingle.Text.Trim().Split('/');
                    string strDate = date[0].ToString() + "/01/" + date[1].ToString();
                    objClientUnitParts.ManufactureDate = Convert.ToDateTime(strDate);
                }

                objClientUnitParts.UnitTon = txtUnitTonSingle.Text.Trim();
                objClientUnitParts.Thermostat = Convert.ToInt32(drpBoosterCool.SelectedValue.ToString());


                long ClientUnitPartId = 0;
                ClientUnitPartId = objClientUnitPartService.AddClientUnitPartPortal(ref objClientUnitParts);
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

                        for (int i = 0; i < rptCoolingFilter.Items.Count; i++)
                        {
                            var item = rptCoolingFilter.Items[i];
                            DropDownList drpFilterSize = (DropDownList)item.FindControl("drpFilterSizeCool1");
                            DropDownList drpFilterLocation = (DropDownList)item.FindControl("drpFilterLocationCool");
                            objUnitExtraInfoFilter.PartId = Convert.ToInt32(drpFilterSize.SelectedValue.ToString());
                            //0=Inside Equipment 1=Inside Space
                            if (drpFilterLocation.SelectedValue == "0")
                                objUnitExtraInfoFilter.LocationOfPart = false;
                            else
                                objUnitExtraInfoFilter.LocationOfPart = true;

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
                        for (int i = 0; i < rptCoolingFuses.Items.Count; i++)
                        {
                            var item = rptCoolingFuses.Items[i];
                            DropDownList drpFuseType = (DropDownList)item.FindControl("drpFuseTypeCool");
                            objUnitExtraInfoFuse.PartId = Convert.ToInt32(drpFuseType.SelectedValue.ToString());
                            objUnitExtraInfoService.AddUnitExtraInfo(ref objUnitExtraInfoFuse);
                        }
                    }
                }

                //Heating
                objClientUnitParts.SplitType = Aircall.Common.General.SplitType.Heating.ToString();
                objClientUnitParts.ModelNumber = txtModelNoHeat.Text.Trim();
                objClientUnitParts.SerialNumber = txtSerialHeat.Text.Trim();
                objClientUnitParts.ManufactureBrand = txtMfgBrandHeat.Text.Trim();
                if (!string.IsNullOrEmpty(txtMfgDateSingle.Text.Trim()))
                {
                    string[] date = txtMfgDateSingle.Text.Trim().Split('/');
                    string strDate = date[0].ToString() + "/01/" + date[1].ToString();
                    objClientUnitParts.ManufactureDate = Convert.ToDateTime(strDate);
                }

                objClientUnitParts.UnitTon = txtUnitTonSingle.Text.Trim();
                objClientUnitParts.Thermostat = Convert.ToInt32(drpBoosterHeat.SelectedValue.ToString());

                ClientUnitPartId = 0;
                ClientUnitPartId = objClientUnitPartService.AddClientUnitPartPortal(ref objClientUnitParts);
                //Add Extra Unit Info Start
                if (ClientUnitPartId != 0)
                {
                    objUnitExtraInfoService = ServiceFactory.UnitExtraInfoService;
                    if (drpFilterQtyHeat.SelectedValue != "0")
                    {
                        BizObjects.UnitExtraInfo objUnitExtraInfoFilter = new BizObjects.UnitExtraInfo();
                        objUnitExtraInfoFilter.UnitId = ClientUnitId;
                        objUnitExtraInfoFilter.ExtraInfoType = Aircall.Common.General.UnitExtraInfoType.Filter.ToString();
                        objUnitExtraInfoFilter.ClientUnitPartId = ClientUnitPartId;
                        int QtyCnt = Convert.ToInt32(drpFilterQtyCool.SelectedValue);

                        for (int i = 0; i < rptHeatingFilter.Items.Count; i++)
                        {
                            var item = rptHeatingFilter.Items[i];
                            DropDownList drpFilterSize = (DropDownList)item.FindControl("drpFilterSizeHeat");
                            DropDownList drpFilterLocation = (DropDownList)item.FindControl("drpFilterLocationHeat");
                            objUnitExtraInfoFilter.PartId = Convert.ToInt32(drpFilterSize.SelectedValue.ToString());
                            //0=Inside Equipment 1=Inside Space
                            if (drpFilterLocation.SelectedValue == "0")
                                objUnitExtraInfoFilter.LocationOfPart = false;
                            else
                                objUnitExtraInfoFilter.LocationOfPart = true;

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
                        for (int i = 0; i < rptHeatingFuses.Items.Count; i++)
                        {
                            var item = rptHeatingFuses.Items[i];
                            DropDownList drpFuseType = (DropDownList)item.FindControl("drpFuseTypeHeat");
                            objUnitExtraInfoFuse.PartId = Convert.ToInt32(drpFuseType.SelectedValue.ToString());
                            objUnitExtraInfoService.AddUnitExtraInfo(ref objUnitExtraInfoFuse);
                        }
                    }
                }
            }
            //Add Extra Unit Info End            
        }

        protected void drpPlan_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                UpdatePanel2.Update();
                dvSubscription.Visible = true;
                dvSpecial.Visible = true;
                dvPM.Visible = true;
                //dvTotal.Visible = true;

                objPlanService = ServiceFactory.PlanService;
                DataTable dtPlanType = new DataTable();
                objPlanService.GetAllPlanType(ref dtPlanType);
                var row = dtPlanType.Select(" Id = " + drpPlan.SelectedValue)[0];
                decimal DurationInMonth = decimal.Parse(row["DurationInMonth"].ToString());
                decimal PricePerMonth = decimal.Parse(row["PricePerMonth"].ToString());
                decimal DiscountPrice = decimal.Parse(row["DiscountPrice"].ToString());
                hdnDurationInMonth.Value = row["DurationInMonth"].ToString();
                hdnPricePerMonth.Value = row["PricePerMonth"].ToString();
                hdnDiscountPrice.Value = row["DiscountPrice"].ToString();


                lblPM.Text = "$" +  ( Convert.ToInt32(txtQty.Text.Trim()) * PricePerMonth).ToString("0.00");
                lblTotalMonth.Text = "For " + DurationInMonth.ToString() + " Months";
                lblTotalPrice.Text = "$" + (DurationInMonth * PricePerMonth).ToString("0.00");
                if (row["ShowAutoRenewal"].ToString().ToLower() == "true")
                {
                    AutoRenewalEnable.Value = "true";
                    //chkAutoRenewal.Visible = true;
                    dvSubscription.Visible = true;
                    dvSpecialLeft.Visible = false;
                }
                else
                {
                    AutoRenewalEnable.Value = "false";
                    dvSpecialLeft.Visible = true;
                    dvSubscription.Visible = false;
                }
                if (row["ShowSpecialPrice"].ToString().ToLower() == "true")
                {
                    SpecialEnabled.Value = "true";
                    //chkSpecialOffer.Visible = true;
                    dvSpecial.Visible = true;
                    chkSpecialOffer.Text = "Special Offer Save $" + (Convert.ToInt32(txtQty.Text.Trim())*((PricePerMonth * DurationInMonth) - DiscountPrice)).ToString() + " & pay $" + (Convert.ToInt32(txtQty.Text.Trim()) * DiscountPrice).ToString() + " now";
                }
                else
                {
                    SpecialEnabled.Value = "false";
                    dvSpecial.Visible = false;
                }

            }
            catch (Exception ex)
            {
                dvSubscription.Visible = false;
                dvSpecial.Visible = false;
                //chkSpecialOffer.Visible = false;
                //chkAutoRenewal.Visible = false;
                dvPM.Visible = false;
                dvTotal.Visible = false;
				lblPM.Text = "";
                lblTotalMonth.Text = "";
                lblTotalPrice.Text = "";
            }
            chkAutoRenewal_CheckedChanged(chkAutoRenewal, EventArgs.Empty);
        }

        protected void chkSpecialOffer_CheckedChanged(object sender, EventArgs e)
        {
            UpdatePanel2.Update();
            if (chkSpecialOffer.Checked)
            {
                chkAutoRenewal.Checked = false;
                dvSubscription.Visible = false;
                dvSpecialLeft.Visible = true;
            }
            else
            {
                if (Convert.ToBoolean(AutoRenewalEnable.Value))
                {
                    dvSpecialLeft.Visible = false;
                }
                dvSubscription.Visible = Convert.ToBoolean(AutoRenewalEnable.Value);
                dvSpecial.Visible = Convert.ToBoolean(SpecialEnabled.Value);
            }
        }

        protected void chkAutoRenewal_CheckedChanged(object sender, EventArgs e)
        {
            UpdatePanel2.Update();
            if (chkAutoRenewal.Checked)
            {
                chkSpecialOffer.Checked = false;
                if (Convert.ToBoolean(AutoRenewalEnable.Value))
                {
                    dvSpecial.Visible = false;
                }
            }
            else
            {
                dvSubscription.Visible = Convert.ToBoolean(AutoRenewalEnable.Value);
                dvSpecial.Visible = Convert.ToBoolean(SpecialEnabled.Value);
            }
        }

        protected void drpVisitsPerYear_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}