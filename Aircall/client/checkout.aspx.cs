using Aircall.Common;
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
    public partial class checkout : System.Web.UI.Page
    {
        IClientAddressService objClientAddressService;
        IStateService objStateService;
        ICitiesService objCitiesService;
        IClientService objClientService = ServiceFactory.ClientService;
        DataTable dtClient = new DataTable();

        int ClientId = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["ClientLoginCookie"] == null)
            {
                Response.Redirect(Application["SiteAddress"] + "sign-in.aspx");
            }
            if (!IsPostBack)
            {

                ClientId = (Session["ClientLoginCookie"] as LoginModel).Id;
                objClientService.GetClientById(ClientId, ref dtClient);
                FillStateDropdown();
                objClientAddressService = ServiceFactory.ClientAddressService;
                DataTable dtAddress = new DataTable();
                objClientAddressService.GetClientAddressesByClientId(ClientId, true, ref dtAddress);
                //if (dtAddress.Rows.Count <= 0)
                //{
                //    Response.Redirect(Application["SiteAddress"]+"client/dashboard.aspx");
                //}
                DataRow[] rows = dtAddress.Select(" IsDefaultAddress = true ");
                txtFirstName.Text = dtClient.Rows[0]["FirstName"].ToString();
                txtLastName.Text = dtClient.Rows[0]["LastName"].ToString();
                if (rows.Length > 0)
                {
                    DataRow row = rows[0];
                    txtAddress.Text = row["Address"].ToString();
                    txtZip.Text = row["ZipCode"].ToString();
                    drpState.SelectedValue = row["State"].ToString();
                    drpState_SelectedIndexChanged(drpState, EventArgs.Empty);
                    drpCity.SelectedValue = row["City"].ToString();
                }
                txtPhone.Text = dtClient.Rows[0]["HomeNumber"].ToString();
                txtMobile.Text = dtClient.Rows[0]["MobileNumber"].ToString();
                txtCompany.Text = dtClient.Rows[0]["Company"].ToString();
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
            }
            drpState.DataBind();
            drpState.Items.Insert(0, new ListItem("Select State", "0"));
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
            drpCity.Items.Insert(0, new ListItem("Select City", "0"));
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            DataTable dtadd = new DataTable();
            dtadd.Columns.Add("FirstName");
            dtadd.Columns.Add("LastName");
            dtadd.Columns.Add("Address");
            dtadd.Columns.Add("ZipCode");
            dtadd.Columns.Add("State");
            dtadd.Columns.Add("StateName");
            dtadd.Columns.Add("City");
            dtadd.Columns.Add("CityName");
            dtadd.Columns.Add("Company");
            dtadd.Columns.Add("PhoneNumber");
            dtadd.Columns.Add("MobileNumber");
            

            if (!string.IsNullOrWhiteSpace(txtAddress.Text) && !string.IsNullOrWhiteSpace(txtZip.Text) && !string.IsNullOrWhiteSpace(txtMobile.Text))
            {
                objClientAddressService = ServiceFactory.ClientAddressService;
                int rtn = objClientAddressService.ValidateZipcode(Convert.ToInt32(drpState.SelectedValue.ToString()), Convert.ToInt32(drpCity.SelectedValue.ToString()), txtZip.Text.Trim());
                if (rtn > 0)
                {
                    dtadd.Rows.Add(dtadd.NewRow());

                    dtadd.Rows[0]["FirstName"] = txtFirstName.Text;
                    dtadd.Rows[0]["LastName"] = txtLastName.Text;
                    dtadd.Rows[0]["Address"] = txtAddress.Text;
                    dtadd.Rows[0]["Company"] = txtCompany.Text;
                    dtadd.Rows[0]["ZipCode"] = txtZip.Text;
                    dtadd.Rows[0]["State"] = drpState.SelectedValue;
                    dtadd.Rows[0]["City"] = drpCity.SelectedValue;
                    dtadd.Rows[0]["StateName"] = drpState.SelectedItem.Text;
                    dtadd.Rows[0]["CityName"] = drpCity.SelectedItem.Text;
                    dtadd.Rows[0]["PhoneNumber"] = txtPhone.Text;
                    dtadd.Rows[0]["MobileNumber"] = txtMobile.Text;
                    Session.Add("billingAdd", dtadd);
                    Response.Redirect("PaymentMethod.aspx");
                }
                else if (rtn == -1)
                {
                    dvErr.InnerHtml = General.GetSitesettingsValue("ServiceInactiveZipCodeMessage"); //"<strong>Service in this zip code is inactivate.</strong>";
                    dvErr.Attributes.Add("class", "error");
                    dvErr.Visible = true;
                    return;
                }
                else if (rtn == -2)
                {
                    dvErr.InnerHtml = "<strong>Please enter a valid Zip Code.</strong>";
                    dvErr.Attributes.Add("class", "error");
                    dvErr.Visible = true;
                    return;
                }
            }
            else
            {
                dvErr.Style["display"] = "block";
                dvErr.InnerText = "Please Provide Proper Details.";
            }
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
    }
}