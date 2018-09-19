using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Services;
using System.Data;
using Aircall.Common;

namespace Aircall.client
{
    public partial class address_addEdit : System.Web.UI.Page
    {
        #region "Declaration"
        IStateService objStateService = ServiceFactory.StateService;
        ICitiesService objCitiesService = ServiceFactory.CitiesService;
        IClientAddressService objClientAddressService = ServiceFactory.ClientAddressService;
        #endregion

        #region "Page Events"
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                FillStateDropdown();
                if (Session["AddressId"] != null)
                {
                    ViewState["AddressId"] = Session["AddressId"].ToString();
                    DataTable dtAddress = new DataTable();
                    objClientAddressService.GetAddressByIdClientId(Convert.ToInt32(ViewState["AddressId"].ToString()), (Session["ClientLoginCookie"] as LoginModel).Id, ref dtAddress);
                    if (dtAddress.Rows.Count > 0)
                    {
                        txtAddress.Text = dtAddress.Rows[0]["Address"].ToString();
                        drpState.SelectedValue = dtAddress.Rows[0]["State"].ToString();
                        BindCitiesFromStateId(Convert.ToInt32(dtAddress.Rows[0]["State"].ToString()));
                        drpCity.SelectedValue = dtAddress.Rows[0]["City"].ToString();
                        txtZip.Text = dtAddress.Rows[0]["ZipCode"].ToString();
                        chkIsDefault.Checked = Convert.ToBoolean(dtAddress.Rows[0]["IsDefaultAddress"].ToString());
                        hdnIsDefault.Value = dtAddress.Rows[0]["IsDefaultAddress"].ToString();
                        btnUpdate.Visible = true;
                        btnSubmit.Visible = false;
                        Session.Remove("AddressId");
                    }
                }
            }
        }
        #endregion

        #region "Functions"
        private void FillStateDropdown()
        {
            DataTable dtState = new DataTable();
            if (Session["AddressId"] != null)
                objStateService.GetAllStates(true, true,ref dtState);
            else
                objStateService.GetAllStates(true, false,ref dtState);
            if (dtState.Rows.Count > 0)
            {
                drpState.DataSource = dtState;
                drpState.DataTextField = dtState.Columns["Name"].ToString();
                drpState.DataValueField = dtState.Columns["Id"].ToString();
            }            
            drpState.DataBind();
            drpState.Items.Insert(0, new ListItem("Select State", "0"));
            var rows = dtState.Select(" IsDefault = True ");
            if (rows.Length > 0)
            {
                drpState.SelectedValue = rows[0]["Id"].ToString();
                drpState_SelectedIndexChanged(drpState, EventArgs.Empty);
            }
        }

        private void BindCitiesFromStateId(int StateId)
        {
            DataTable dtCity = new DataTable();
            if (Session["AddressId"] != null)
                objCitiesService.GetAllCityByStateId(StateId, true,ref dtCity);
            else
                objCitiesService.GetAllCityByStateId(StateId, false,ref dtCity);
            if (dtCity.Rows.Count > 0)
            {
                drpCity.DataSource = dtCity;
                drpCity.DataTextField = dtCity.Columns["Name"].ToString();
                drpCity.DataValueField = dtCity.Columns["Id"].ToString();
            }
            drpCity.DataBind();
            drpCity.Items.Insert(0, new ListItem("Select City", "0"));
        }
        #endregion

        #region "Events"
        protected void drpState_SelectedIndexChanged(object sender, EventArgs e)
        {
            drpCity.DataSource = "";
            drpCity.DataBind();
            if (drpState.SelectedValue != "0")
            {
                BindCitiesFromStateId(Convert.ToInt32(drpState.SelectedValue.ToString()));
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    int rtn = objClientAddressService.ValidateZipcode(Convert.ToInt32(drpState.SelectedValue.ToString()), Convert.ToInt32(drpCity.SelectedValue.ToString()), txtZip.Text.Trim());

                    if (rtn > 0)
                    {
                        int ClientId = (Session["ClientLoginCookie"] as LoginModel).Id;
                        BizObjects.ClientAddress objClientAddress = new BizObjects.ClientAddress();
                        objClientAddress.ClientId = ClientId;
                        objClientAddress.Address = txtAddress.Text.Trim();
                        objClientAddress.State = Convert.ToInt32(drpState.SelectedValue.ToString());
                        objClientAddress.City = Convert.ToInt32(drpCity.SelectedValue.ToString());
                        objClientAddress.ZipCode = txtZip.Text.Trim();
                        objClientAddress.IsDefaultAddress = chkIsDefault.Checked;
                        objClientAddress.AddedBy = ClientId;
                        objClientAddress.AddedByType = General.UserRoles.Client.GetEnumValue();
                        objClientAddress.AddedDate = DateTime.UtcNow;

                        objClientAddressService.AddClientAddress(ref objClientAddress);

                        Response.Redirect(Application["SiteAddress"] + "client/address-list.aspx", false);
                    }
                    else if (rtn == -1)
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
                catch (Exception Ex)
                {
                    dvMessage.InnerHtml = Ex.Message.ToString().Trim();
                    dvMessage.Attributes.Add("class", "error");
                    dvMessage.Visible = true;
                }
            }
        }


        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    if (chkIsDefault.Checked == false && hdnIsDefault.Value.ToLower() == "true")
                    {
                        dvMessage.InnerHtml = "Please select another address as Default first.";
                        dvMessage.Attributes.Add("class", "error");
                        dvMessage.Visible = true;

                        return;
                    }
                    if (ViewState["AddressId"] != null)
                    {
                        int rtn = objClientAddressService.ValidateZipcode(Convert.ToInt32(drpState.SelectedValue.ToString()), Convert.ToInt32(drpCity.SelectedValue.ToString()), txtZip.Text.Trim());

                        if (rtn > 0)
                        {
                            int ClientId = (Session["ClientLoginCookie"] as LoginModel).Id;
                            BizObjects.ClientAddress objClientAddress = new BizObjects.ClientAddress();
                            objClientAddress.Id = Convert.ToInt32(Convert.ToInt32(ViewState["AddressId"].ToString()));
                            objClientAddress.ClientId = ClientId;
                            objClientAddress.Address = txtAddress.Text.Trim();
                            objClientAddress.State = Convert.ToInt32(drpState.SelectedValue.ToString());
                            objClientAddress.City = Convert.ToInt32(drpCity.SelectedValue.ToString());
                            objClientAddress.ZipCode = txtZip.Text.Trim();
                            objClientAddress.IsDefaultAddress = chkIsDefault.Checked;
                            objClientAddress.UpdatedBy = ClientId;
                            objClientAddress.UpdatedByType = General.UserRoles.Client.GetEnumValue();
                            objClientAddress.UpdatedDate = DateTime.Now;

                            objClientAddressService.UpdateClientAddress(ref objClientAddress);

                            Response.Redirect(Application["SiteAddress"] + "client/address-list.aspx", false);
                        }
                        else if (rtn == -1)
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
                }
                catch (Exception Ex)
                {
                    dvMessage.InnerHtml = Ex.Message.ToString().Trim();
                    dvMessage.Attributes.Add("class", "error");
                    dvMessage.Visible = true;
                }
            }
        }
        #endregion
    }
}