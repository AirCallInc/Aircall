using Aircall.Common;
using Services;
using Stripe;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Aircall.client
{
    public partial class OtherPayment : System.Web.UI.Page
    {
        IClientAddressService objClientAddressService;
        IStateService objStateService;
        ICitiesService objCitiesService;
        IClientService objClientService = ServiceFactory.ClientService;
        DataTable dtClient = new DataTable();
        IClientPaymentMethodService objClientPaymentMethodService = ServiceFactory.ClientPaymentMethodService;
        IStripeErrorLogService objStripeErrorLogService = ServiceFactory.StripeErrorLogService;

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
                BindMonthYearDropdown();

                objClientAddressService = ServiceFactory.ClientAddressService;
                DataTable dtAddress = new DataTable();
                objClientAddressService.GetClientAddressesByClientId(ClientId, true, ref dtAddress);
                txtFirstName.Text = dtClient.Rows[0]["FirstName"].ToString();
                txtLastName.Text = dtClient.Rows[0]["LastName"].ToString();

                DataRow[] rows = dtAddress.Select(" IsDefaultAddress = true ");
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
                BindClientPaymentMethods();
                //DataTable dtClientPaymentMethod = new DataTable();
                //objClientPaymentMethodService.GetClientPaymentMethodByClientId(ClientId, ref dtClientPaymentMethod);
                //if (dtClientPaymentMethod.Rows.Count > 0)
                //{
                //    DataTable dt = dtClientPaymentMethod.Clone();

                //    var row1 = dtClientPaymentMethod.Select(" IsDefaultPayment='true' ").FirstOrDefault();

                //    if (row1 != null)
                //    {
                //        txtCardNumber.Text = row1["CardNumber"].ToString();
                //        txtName.Text = row1["NameOnCard"].ToString();
                //        txtMonth.Text = row1["ExpiryMonth"].ToString();
                //        txtYear.Text = row1["ExpiryYear"].ToString();
                //        hdnCardId.Value = row1["Id"].ToString();
                //    }
                //    lstPaymentMethod.DataSource = dtClientPaymentMethod;
                //    lstPaymentMethod.DataBind();
                //}
            }
        }
        private void BindClientPaymentMethods()
        {
            DataTable dtClientPaymentMethod = new DataTable();
            objClientPaymentMethodService.GetClientPaymentMethodByClientId(ClientId, ref dtClientPaymentMethod);
            if (dtClientPaymentMethod.Rows.Count > 0)
            {
                DataTable dt = dtClientPaymentMethod.Clone();

                var row = dtClientPaymentMethod.Select(" IsDefaultPayment='true' ").FirstOrDefault();

                if (row != null)
                {
                    hdnCardId.Value = row["Id"].ToString();
                    if (row["CardType"].ToString() == "Visa")
                    {
                        rblVisa.Checked = true;
                        rblMaster.Checked = false;
                        rblDiscover.Checked = false;
                        rblAmex.Checked = false;
                    }
                    else if (row["CardType"].ToString() == "MasterCard")
                    {
                        rblMaster.Checked = true;
                        rblVisa.Checked = false;
                        rblDiscover.Checked = false;
                        rblAmex.Checked = false;
                    }
                    else if (row["CardType"].ToString() == "Discover")
                    {
                        rblDiscover.Checked = true;
                        rblMaster.Checked = false;
                        rblVisa.Checked = false;
                        rblAmex.Checked = false;
                    }
                    else
                    {
                        rblDiscover.Checked = false;
                        rblMaster.Checked = false;
                        rblVisa.Checked = false;
                        rblAmex.Checked = true;
                    }
                    txtCardNumber.Text = row["CardNumber"].ToString();
                    txtCardNumber.Enabled = false;
                    txtName.Text = row["NameOnCard"].ToString();
                    drpMonth.SelectedValue = row["ExpiryMonth"].ToString();
                    drpYear.SelectedValue = row["ExpiryYear"].ToString();

                    drpMonth.Enabled = false;
                    drpYear.Enabled = false;
                    rblVisa.Disabled = true;
                    rblMaster.Disabled = true;
                    rblDiscover.Disabled = true;
                    rblAmex.Disabled = true;
                }
                lstPaymentMethod.DataSource = dtClientPaymentMethod;
                lstPaymentMethod.DataBind();
            }
        }
        private void BindMonthYearDropdown()
        {

            drpMonth.Enabled = true;
            drpYear.Enabled = true;
            rblVisa.Disabled = false;
            rblMaster.Disabled = false;
            rblDiscover.Disabled = false;
            rblAmex.Disabled = false;

            DataTable dtMonth = new DataTable();
            dtMonth.Columns.Add("Month");
            for (int i = 01; i <= 12; i++)
            {
                dtMonth.Rows.Add(dtMonth.NewRow());
                dtMonth.Rows[dtMonth.Rows.Count - 1]["Month"] = i.ToString("00");
            }
            drpMonth.DataSource = dtMonth;
            drpMonth.DataTextField = "Month";
            drpMonth.DataValueField = "Month";
            drpMonth.DataBind();
            drpMonth.Items.Insert(0, new ListItem("Month", "0"));

            DataTable dtYear = new DataTable();
            dtYear.Columns.Add("Year");
            for (int i = DateTime.Now.Year; i < DateTime.Now.Year + 20; i++)
            {
                dtYear.Rows.Add(dtYear.NewRow());
                dtYear.Rows[dtYear.Rows.Count - 1]["Year"] = i.ToString();
            }
            drpYear.DataSource = dtYear;
            drpYear.DataTextField = "Year";
            drpYear.DataValueField = "Year";
            drpYear.DataBind();
            drpYear.Items.Insert(0, new ListItem("Year", "0"));
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
            if (!chkTerms.Checked)
            {
                dvMessage.InnerHtml = "Please Accept Terms & Condition.";
                dvMessage.Attributes.Add("class", "error");
                dvMessage.Visible = true;
                return;
            }
            string StripeCardId = "";
            int ClientId = (Session["ClientLoginCookie"] as LoginModel).Id;

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

            DataTable dtClient = new DataTable();
            objClientService.GetClientById(ClientId, ref dtClient);
            DataTable dtCard = new DataTable();
            dtCard.Columns.Add("CardNumber");
            dtCard.Columns.Add("NameOnCard");
            dtCard.Columns.Add("ExpiryMonth");
            dtCard.Columns.Add("ExpiryYear");
            dtCard.Columns.Add("CardType");
            dtCard.Rows.Add(dtCard.NewRow());

            if (txtCardNumber.Text.Contains("*"))
            {
                DataTable dtClientPaymentMethod = new DataTable();
                objClientPaymentMethodService.GetClientPaymentMethodByClientId(ClientId, ref dtClientPaymentMethod);
                foreach (DataRow row in dtClientPaymentMethod.Rows)
                {
                    if (row["Id"].ToString() == (hdnCardId.Value))
                    {
                        dtCard.Rows[0]["CardNumber"] = row["CardNumber"].ToString();
                        dtCard.Rows[0]["NameOnCard"] = row["NameOnCard"].ToString();
                        dtCard.Rows[0]["ExpiryMonth"] = row["ExpiryMonth"].ToString();
                        dtCard.Rows[0]["ExpiryYear"] = row["ExpiryYear"].ToString();
                        dtCard.Rows[0]["CardType"] = row["CardType"].ToString();
                        Session.Add("cardDetail", dtCard);
                        StripeCardId = row["StripeCardId"].ToString();
                        break;
                    }
                }
                if (string.IsNullOrWhiteSpace(StripeCardId))
                {
                    dvMessage.InnerText = "Invalid Card Detail";
                    dvMessage.Attributes.Add("class", "error");
                    dvMessage.Visible = true;
                }
                else
                {
                    Session.Add("stripeCard", StripeCardId);
                    //Response.Redirect("Receipt.aspx");
                }
            }
            else
            {
                DateTime dt = new DateTime(int.Parse(drpYear.SelectedValue), int.Parse(drpMonth.SelectedValue), 1);
                var ts = dt.Subtract(DateTime.UtcNow);
                if (ts.Ticks <= 0)
                {
                    dvMessage.InnerHtml = "Invalid Exp Month and Exp Year.";
                    dvMessage.Attributes.Add("class", "error");
                    dvMessage.Visible = true;
                    return;
                }
                BizObjects.ClientPaymentMethod objClientPaymentMethod = new BizObjects.ClientPaymentMethod();
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
                objClientPaymentMethod.NameOnCard = txtName.Text.Trim();
                objClientPaymentMethod.CardNumber = cardStr.PadLeft(16, '*');
                objClientPaymentMethod.ExpiryMonth = Convert.ToInt16(drpMonth.SelectedValue);
                objClientPaymentMethod.ExpiryYear = Convert.ToInt32(drpYear.SelectedValue);
                if (rblVisa.Checked)
                    objClientPaymentMethod.CardType = "Visa";
                else if (rblMaster.Checked)
                    objClientPaymentMethod.CardType = "MasterCard";
                else if (rblDiscover.Checked)
                    objClientPaymentMethod.CardType = "Discover";
                else
                    objClientPaymentMethod.CardType = "AmericanExpress";
                objClientPaymentMethod.IsDefaultPayment = false;
                objClientPaymentMethod.AddedBy = ClientId;
                objClientPaymentMethod.AddedByType = General.UserRoles.Client.GetEnumValue();
                objClientPaymentMethod.AddedDate = DateTime.UtcNow;
                dtCard.Rows[0]["CardNumber"] = objClientPaymentMethod.CardNumber;
                dtCard.Rows[0]["NameOnCard"] = objClientPaymentMethod.NameOnCard;
                dtCard.Rows[0]["ExpiryMonth"] = objClientPaymentMethod.ExpiryMonth;
                dtCard.Rows[0]["ExpiryYear"] = objClientPaymentMethod.ExpiryYear;
                dtCard.Rows[0]["CardType"] = objClientPaymentMethod.CardType;
                Session.Add("cardDetail", dtCard);
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
                        ExpirationYear = drpYear.SelectedValue,
                        ExpirationMonth = drpMonth.SelectedValue,
                        Name = txtName.Text.Trim(),
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
                    Session.Add("stripeCard", stripeCard.Id);
                    //objClientPaymentMethod.StripeCardId = stripeCard.Id;
                    objClientPaymentMethodService.AddClientPaymentMethod(ref objClientPaymentMethod);
                }
                catch (StripeException stex)
                {
                    BizObjects.StripeErrorLog objStripeErrorLog = new BizObjects.StripeErrorLog();
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
                    objStripeErrorLog.DateAdded = DateTime.Now;

                    objStripeErrorLogService.AddStripeErrorLog(ref objStripeErrorLog);

                    dvMessage.InnerText = stex.StripeError.Message.ToString();
                    dvMessage.Attributes.Add("class", "error");
                    dvMessage.Visible = true;
                    return;
                }
            }

            if (!string.IsNullOrWhiteSpace(txtAddress.Text) && !string.IsNullOrWhiteSpace(txtZip.Text) && !string.IsNullOrWhiteSpace(txtMobile.Text))
            {
                dtadd.Rows.Add(dtadd.NewRow());

                dtadd.Rows[0]["FirstName"] = txtFirstName.Text;
                dtadd.Rows[0]["LastName"] = txtLastName.Text;
                dtadd.Rows[0]["Company"] = txtCompany.Text;
                dtadd.Rows[0]["Address"] = txtAddress.Text;
                dtadd.Rows[0]["ZipCode"] = txtZip.Text;
                dtadd.Rows[0]["State"] = drpState.SelectedValue;
                dtadd.Rows[0]["StateName"] = drpState.SelectedItem.Text;
                dtadd.Rows[0]["CityName"] = drpCity.SelectedItem.Text;
                dtadd.Rows[0]["City"] = drpCity.SelectedValue;
                dtadd.Rows[0]["PhoneNumber"] = txtPhone.Text;
                dtadd.Rows[0]["MobileNumber"] = txtMobile.Text;
                Session.Add("billingAdd", dtadd);

                if (Session["uid"] == null)
                {
                    Response.Redirect("OtherReceipt.aspx");
                }
                else
                {
                    Response.Redirect("PlanRenewReceipt.aspx?uid=" + Session["uid"].ToString());
                }
            }
            else
            {
                dvErr.Style["display"] = "block";
                dvErr.InnerText = "Please Provide Proper Details.";
            }
        }
        protected void lstPaymentMethod_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            if (e.CommandName == "SelectCard" && e.CommandArgument.ToString() != "0")
            {

                ViewState["CardId"] = e.CommandArgument.ToString();

                int ClientPaymentId = Convert.ToInt32(e.CommandArgument.ToString());
                hdnCardId.Value = ClientPaymentId.ToString();
                DataTable dtCardInfo = new DataTable();
                objClientPaymentMethodService.GetClientPaymentMethodById(ClientPaymentId, ref dtCardInfo);
                if (dtCardInfo.Rows.Count > 0)
                {
                    if (dtCardInfo.Rows[0]["CardType"].ToString() == "Visa")
                        rblVisa.Checked = true;
                    else if (dtCardInfo.Rows[0]["CardType"].ToString() == "MasterCard")
                        rblMaster.Checked = true;
                    else if (dtCardInfo.Rows[0]["CardType"].ToString() == "Discover")
                        rblDiscover.Checked = true;
                    else
                        rblAmex.Checked = true;

                    txtName.Text = dtCardInfo.Rows[0]["NameOnCard"].ToString();
                    txtCardNumber.Text = dtCardInfo.Rows[0]["CardNumber"].ToString();
                    //txtMonth.Text = dtCardInfo.Rows[0]["ExpiryMonth"].ToString();
                    //txtYear.Text = dtCardInfo.Rows[0]["ExpiryYear"].ToString();
                    drpMonth.SelectedValue = dtCardInfo.Rows[0]["ExpiryMonth"].ToString();
                    drpYear.SelectedValue = dtCardInfo.Rows[0]["ExpiryYear"].ToString();
                }
                hdnCardMode.Value = "1";
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