using Aircall.Common;
using AuthorizeNet.Api.Contracts.V1;
using AuthorizeNetLib;
using DBUtility;
using Services;
using Stripe;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Aircall.client
{
    public partial class PaymentMethod : System.Web.UI.Page
    {
        #region "Declaration"
        IClientService objClientService = ServiceFactory.ClientService;
        IClientUnitService objClientUnitService = ServiceFactory.ClientUnitService;
        IClientPaymentMethodService objClientPaymentMethodService = ServiceFactory.ClientPaymentMethodService;
        IStripeErrorLogService objStripeErrorLogService = ServiceFactory.StripeErrorLogService;
        #endregion

        private void BindClientPaymentMethods()
        {
            if (Session["ClientLoginCookie"] == null)
            {
                Response.Redirect(Application["SiteAddress"] + "sign-in.aspx");
            }
            int ClientId = (Session["ClientLoginCookie"] as LoginModel).Id;

            var objClientUnitSubscriptionService = ServiceFactory.ClientUnitSubscriptionService;
            DataTable dtSubscription = new DataTable();
            objClientUnitSubscriptionService.GetClientUnitUnPaidSubscription(ClientId, ref dtSubscription);

            if (dtSubscription.Rows.Count > 0)
            {
                FillSubscription(dtSubscription);
            }

            DataTable dtClientPaymentMethod = new DataTable();
            objClientPaymentMethodService.GetClientPaymentMethodByClientId(ClientId, ref dtClientPaymentMethod);
            if (dtClientPaymentMethod.Rows.Count > 0)
            {
                DataTable dt = dtClientPaymentMethod.Clone();

                var row = dtClientPaymentMethod.Select(" IsDefaultPayment='true' ").FirstOrDefault();

                if (row != null)
                {
                    UpdateMode = "1";
                    hdnCardId.Value = row["Id"].ToString();
                    if (row["CardType"].ToString() == "Visa")
                    {
                        rblVisa.Checked = true;
                        rblMaster.Checked = false;
                        rblDiscover.Checked = false;
                        rblAmex.Checked = false;
                        txtCVV.MaxLength = 3;
                    }
                    else if (row["CardType"].ToString() == "MasterCard")
                    {
                        rblMaster.Checked = true;
                        rblVisa.Checked = false;
                        rblDiscover.Checked = false;
                        rblAmex.Checked = false;
                        txtCVV.MaxLength = 3;
                    }
                    else if (row["CardType"].ToString() == "Discover")
                    {
                        rblDiscover.Checked = true;
                        rblMaster.Checked = false;
                        rblVisa.Checked = false;
                        rblAmex.Checked = false;
                        txtCVV.MaxLength = 3;
                    }
                    else
                    {
                        rblDiscover.Checked = false;
                        rblMaster.Checked = false;
                        rblVisa.Checked = false;
                        rblAmex.Checked = true;
                        txtCVV.MaxLength = 4;
                    }
                    txtCardNumber.Text = row["CardNumber"].ToString();

                    txtName.Text = row["NameOnCard"].ToString();
                    //txtMonth.Text = row["ExpiryMonth"].ToString();
                    //txtYear.Text = row["ExpiryYear"].ToString();
                    drpMonth.SelectedValue = row["ExpiryMonth"].ToString();
                    drpYear.SelectedValue = row["ExpiryYear"].ToString();

                    drpMonth.Enabled = false;
                    drpYear.Enabled = false;
                    rblVisa.Disabled = true;
                    rblMaster.Disabled = true;
                    rblDiscover.Disabled = true;
                    rblAmex.Disabled = true;
                }
                txtCardNumber.Enabled = false;
                lstPaymentMethod.DataSource = dtClientPaymentMethod;
                lstPaymentMethod.DataBind();
            }
        }

        private void FillSubscription(DataTable dt)
        {
            var dr = dt.Rows[0];
            var unitIds = dr["UnPaidClientUnitIds"].ToString();
            this.hdfTotalUnits.Value = dr["TotalUnitCount"].ToString();
            if (unitIds.StartsWith(","))
            {
                unitIds = unitIds.Substring(1);
            }
            this.hdfUnitIds.Value = unitIds;
            var amt = Convert.ToDecimal(dr["TotalAmount"]);
            ltrAmount.Text = amt.ToString("0.00");
            hdfAmount.Value = amt.ToString();
            this.hdfPricePerMonth.Value = dr["PricePerMonth"].ToString();
        }

        protected void lstPaymentMethod_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            if (e.CommandName == "SelectCard" && e.CommandArgument.ToString() != "0")
            {
                ViewState["CardId"] = e.CommandArgument.ToString();
                int ClientPaymentId = Convert.ToInt32(e.CommandArgument.ToString());
                DataTable dtCardInfo = new DataTable();
                objClientPaymentMethodService.GetClientPaymentMethodById(ClientPaymentId, ref dtCardInfo);
                hdnCardId.Value = ClientPaymentId.ToString();
                if (dtCardInfo.Rows.Count > 0)
                {
                    UpdateMode = "1";
                    if (dtCardInfo.Rows[0]["CardType"].ToString() == "Visa")
                    {
                        rblVisa.Checked = true;
                        rblMaster.Checked = false;
                        rblDiscover.Checked = false;
                        rblAmex.Checked = false;
                        txtCVV.MaxLength = 3;
                    }
                    else if (dtCardInfo.Rows[0]["CardType"].ToString() == "MasterCard")
                    {
                        rblMaster.Checked = true;
                        rblVisa.Checked = false;
                        rblDiscover.Checked = false;
                        rblAmex.Checked = false;
                        txtCVV.MaxLength = 3;
                    }
                    else if (dtCardInfo.Rows[0]["CardType"].ToString() == "Discover")
                    {
                        rblDiscover.Checked = true;
                        rblMaster.Checked = false;
                        rblVisa.Checked = false;
                        rblAmex.Checked = false;
                        txtCVV.MaxLength = 3;
                    }
                    else
                    {
                        rblDiscover.Checked = false;
                        rblMaster.Checked = false;
                        rblVisa.Checked = false;
                        rblAmex.Checked = true;
                        txtCVV.MaxLength = 4;
                    }
                    txtName.Text = dtCardInfo.Rows[0]["NameOnCard"].ToString();
                    txtCardNumber.Text = dtCardInfo.Rows[0]["CardNumber"].ToString();
                    //txtMonth.Text = dtCardInfo.Rows[0]["ExpiryMonth"].ToString();
                    //txtYear.Text = dtCardInfo.Rows[0]["ExpiryYear"].ToString();                                        
                    drpMonth.SelectedValue = dtCardInfo.Rows[0]["ExpiryMonth"].ToString();
                    drpYear.SelectedValue = dtCardInfo.Rows[0]["ExpiryYear"].ToString();

                    drpMonth.Enabled = false;
                    drpYear.Enabled = false;
                    rblVisa.Disabled = true;
                    rblMaster.Disabled = true;
                    rblDiscover.Disabled = true;
                    rblAmex.Disabled = true;
                }
                hdnCardMode.Value = "1";
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["ClientLoginCookie"] == null)
            {
                Response.Redirect(Application["SiteAddress"] + "sign-in.aspx");
            }
            var PDFUrl = General.GetSitesettingsValue("SalesAgreement");
            PDFUrl = (PDFUrl == "" ? "" : ConfigurationManager.AppSettings["PolicyURL"].ToString() + PDFUrl);
            hdnpdfurl.Value = PDFUrl;
            lnkPdf.NavigateUrl = "https://docs.google.com/gview?url=" + PDFUrl;
            int ClientId = (Session["ClientLoginCookie"] as LoginModel).Id;
            if (Session["billingAdd"] == null)
            {
                Response.Redirect(Application["SiteAddress"] + "client/summary.aspx?clientId=" + ClientId.ToString());
            }
            Response.Buffer = true;
            Response.ExpiresAbsolute = DateTime.Now.AddDays(-1d);
            Response.Expires = -100;
            Response.CacheControl = "no-cache";
            if (!IsPostBack)
            {
                UpdateMode = "0";
                BindMonthYearDropdown();
                BindClientPaymentMethods();
            }
        }

        private string UpdateMode
        {
            get
            {
                if (ViewState["UpdateMode"] == null)
                    ViewState["UpdateMode"] = "0";
                return (string)ViewState["UpdateMode"];
            }
            set { ViewState["UpdateMode"] = value; }
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

        protected void btnSave_Click(object sender, EventArgs e)
        {
            int ClientId = (Session["ClientLoginCookie"] as LoginModel).Id;
            if (!chkTerms.Checked)
            {
                dvMessage.InnerHtml = "Please Accept Terms & Condition.";
                dvMessage.Attributes.Add("class", "error");
                dvMessage.Visible = true;
                return;
            }
            if (Session["billingAdd"] == null)
            {
                Response.Redirect(Application["SiteAddress"] + "client/summary.aspx?clientId=" + ClientId.ToString());
            }
            if (UpdateMode != "1")
            {
                DataTable oldCC = new DataTable();
                string cardStr1 = txtCardNumber.Text.Substring(txtCardNumber.Text.Trim().Length - 4);

                objClientPaymentMethodService.GetClientPaymentMethodByClientId(ClientId, ref oldCC);

                var rows = oldCC.Select(" CardNumber ='" + cardStr1.PadLeft(16, '*') + "'");
                if (rows.Length > 0)
                {
                    dvMessage.InnerHtml = "CC Already Exists!";
                    dvMessage.Attributes.Add("class", "error");
                    dvMessage.Visible = true;
                    hdnCardMode.Value = "1";
                    return;
                }
            }

            string CustomerPaymentProfileId = "";

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
                        CustomerPaymentProfileId = row["CustomerPaymentProfileId"].ToString();
                        break;
                    }
                }
                if (string.IsNullOrWhiteSpace(CustomerPaymentProfileId))
                {
                    dvMessage.InnerText = "Invalid Card Detail";
                    dvMessage.Attributes.Add("class", "error");
                    dvMessage.Visible = true;
                }
                else
                {
                    Session.Add("stripeCard", CustomerPaymentProfileId);
                    AddSubscription(ClientId);
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
                    objClientPaymentMethod.CardType = "AMEX";
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
                    objClientPaymentMethod.CardType = "AMEX";
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
                    var customerPaymentProfileId = "";
                    int clientId = (Session["ClientLoginCookie"] as LoginModel).Id;
                    var errMsg = "";
                    AddNewCard(clientId, ref customerPaymentProfileId, ref errMsg);

                    if (!string.IsNullOrEmpty(errMsg))
                    {
                        dvMessage.InnerHtml = errMsg;
                        dvMessage.Attributes.Add("class", "error");
                        dvMessage.Visible = true;
                        return;
                    }

                    //Response.Redirect("Receipt.aspx");
                    AddSubscription(ClientId);
                }
                catch (Exception ex)
                {
                    dvMessage.InnerHtml = ex.Message;
                    dvMessage.Attributes.Add("class", "error");
                    dvMessage.Visible = true;
                    return;
                }
            }
        }

        private void AddSubscription(int clientId)
        {
            var objClientPaymentMethodService = ServiceFactory.ClientPaymentMethodService;
            var cardId = Convert.ToInt32(this.hdnCardId.Value);
            DataTable dtPaymentCard = new DataTable();
            objClientPaymentMethodService.GetClientPaymentMethodById(cardId, ref dtPaymentCard);
            var customerPaymentProfileId = "";

            if (dtPaymentCard.Rows.Count > 0)
            {
                customerPaymentProfileId = dtPaymentCard.Rows[0]["CustomerPaymentProfileId"].ToString();
            }

            var errMsg = "";
            string authorizeNetSubscriptionId = "";
            string errCode = "";
            string errText = "";
            var isSuccess = AddSubscriptionToAuthorizeNet(clientId, customerPaymentProfileId, ref authorizeNetSubscriptionId, ref errCode, ref errText);

            if (!isSuccess)
            {
                errMsg = errCode + " " + errText;
                dvMessage.InnerHtml = errMsg;
                dvMessage.Attributes.Add("class", "error");
                dvMessage.Visible = true;
                return;
            }

            var objClientAddressService = ServiceFactory.ClientAddressService;
            DataTable dtAddress = new DataTable();
            objClientAddressService.GetClientAddressesByClientId(clientId, true, ref dtAddress);

            int AddressId = Convert.ToInt32(dtAddress.Rows[0]["Id"]);
            var totalAmount = Convert.ToDecimal(this.hdfAmount.Value);
            var pricePerMonth = Convert.ToDecimal(this.hdfPricePerMonth.Value);

            var orderId = AddOrder(clientId, customerPaymentProfileId, AddressId, totalAmount, "Charge", "CC");

            UpdatePricePerMonth(pricePerMonth, orderId);

            var arr = this.hdfUnitIds.Value.Split(',');
            var unitId = Convert.ToInt32(arr[0]);
            BizObjects.ClientUnitSubscription objClientUnitSubscription = new BizObjects.ClientUnitSubscription();
            var objClientUnitSubscriptionService = ServiceFactory.ClientUnitSubscriptionService;
            objClientUnitSubscription.ClientId = clientId;
            objClientUnitSubscription.UnitId = unitId;
            objClientUnitSubscription.ClientUnitIds = this.hdfUnitIds.Value;
            objClientUnitSubscription.OrderId = orderId;
            objClientUnitSubscription.PaymentMethod = General.PaymentMethod.CC.GetEnumDescription();
            objClientUnitSubscription.CardId = cardId;
            objClientUnitSubscription.PONumber = string.Empty;
            objClientUnitSubscription.CheckNumbers = string.Empty;
            objClientUnitSubscription.CheckAmounts = string.Empty;
            objClientUnitSubscription.FrontImage = string.Empty;
            objClientUnitSubscription.BackImage = string.Empty;
            objClientUnitSubscription.AccountingNotes = "User pays in the front end website";
            objClientUnitSubscription.PricePerMonth = pricePerMonth;
            objClientUnitSubscription.Amount = totalAmount;
            //objClientUnitSubscription.AddedBy = objLoginModel.Id;
            //objClientUnitSubscription.AddedByType = objLoginModel.RoleId;
            objClientUnitSubscription.AddedDate = DateTime.UtcNow.ToLocalTime();
            objClientUnitSubscription.TotalUnits = Convert.ToInt32(this.hdfTotalUnits.Value);

            var unitSubscriptionId = objClientUnitSubscriptionService.AddClientUnitSubscriptionService(ref objClientUnitSubscription, false);

            UpdateAuthorizeNetSubscriptionId(authorizeNetSubscriptionId, unitSubscriptionId);
            UpdateClientUnitSubscriptionId(clientId, unitSubscriptionId);
            UpdateSubmittedFlag(clientId);

            Response.Redirect(Application["SiteAddress"] + "client/dashboard.aspx");
        }

        private int AddOrder(int ClientId, string customerPaymentProfileId, int AddressId, decimal totalAmount, string OrderType, string ChargeBy)
        {
            BizObjects.Orders objOrders = new BizObjects.Orders();
            var objOrderService = ServiceFactory.OrderService;
            int OrderId = 0;
            objOrders.OrderType = OrderType;
            objOrders.ClientId = ClientId;
            objOrders.OrderAmount = totalAmount;
            objOrders.ChargeBy = ChargeBy;
            //objOrders.AddedBy = objLoginModel.Id;
            //objOrders.AddedByType = objLoginModel.RoleId;
            objOrders.AddedDate = DateTime.UtcNow;
            OrderId = objOrderService.AddClientUnitOrder(ref objOrders, customerPaymentProfileId, AddressId);

            return OrderId;
        }

        private void UpdatePricePerMonth(decimal pricePerMonth, int orderId)
        {
            var sql = string.Format("update Orders set PricePerMonth = {0} where Id = {1}", pricePerMonth, orderId);
            var instance = new SQLDBHelper();
            instance.ExecuteSQL(sql, null);
            instance.Close();
        }

        private void UpdateAuthorizeNetSubscriptionId(string authorizeNetSubscriptionId, int unitSubscriptionId)
        {
            var sql = string.Format("update ClientUnitSubscription set AuthorizeNetSubscriptionId = '{0}' where Id = {1}", authorizeNetSubscriptionId, unitSubscriptionId);
            var instance = new SQLDBHelper();
            instance.ExecuteSQL(sql, null);
            instance.Close();
        }

        private void UpdateSubmittedFlag(int clientId)
        {
            var sql = string.Format("update dbo.ClientUnit set IsSubmittedToSubscription = '1' where ClientId = {0} and Id in ({1})", clientId, this.hdfUnitIds.Value);
            var instance = new SQLDBHelper();
            instance.ExecuteSQL(sql, null);
        }

        private void UpdateClientUnitSubscriptionId(int clientId, int clientUnitSubscriptionId)
        {
            var sql = string.Format("update dbo.ClientUnit set ClientUnitSubscriptionId = {2} where ClientId = {0} and Id in ({1})", clientId, this.hdfUnitIds.Value, clientUnitSubscriptionId);
            var instance = new SQLDBHelper();
            instance.ExecuteSQL(sql, null);
        }

        private bool AddSubscriptionToAuthorizeNet(int clientId, string customerPaymentProfileId, ref string subscriptionId, ref string errCode, ref string errText)
        {
            paymentScheduleTypeInterval interval = new paymentScheduleTypeInterval();

            interval.length = 1;
            interval.unit = ARBSubscriptionUnitEnum.months;

            paymentScheduleType schedule = new paymentScheduleType
            {
                interval = interval,
                startDate = DateTime.Now.AddMinutes(10),
                totalOccurrences = 9999,
            };

            objClientService = ServiceFactory.ClientService;
            DataTable dtClient = null;
            objClientService.GetClientById(clientId, ref dtClient);

            var objClientAddressService = ServiceFactory.ClientAddressService;
            DataTable dtAddress = new DataTable();
            objClientAddressService.GetClientAddressesByClientId(clientId, true, ref dtAddress);

            var helper = new AuthorizeNetHelper();

            string customerProfileId = dtClient.Rows[0]["CustomerProfileId"].ToString();
            string customerAddressId = dtAddress.Rows[0]["CustomerAddressId"].ToString();

            bool isSuccess = false;

            decimal amount = Convert.ToDecimal(this.hdfPricePerMonth.Value);

            helper.CreateSubscriptionFromCustomerProfile(customerProfileId, customerPaymentProfileId, customerAddressId, schedule, amount, ref isSuccess, ref subscriptionId, ref errCode, ref errText);

            return isSuccess;
        }

        private int AddNewCard(int clientId, ref string customerPaymentProfileId, ref string errMsg)
        {
            bool isSuccess = false;
            string errCode = "";
            string errText = "";
            string cardNumber = this.txtCardNumber.Text.Trim();
            string expirationDate = drpMonth.SelectedValue.ToString() + drpYear.SelectedValue.ToString().Substring(2, 2);
            string cardCode = this.txtCVV.Text.Trim();

            this.CreatePaymentProfile(clientId, cardNumber, expirationDate, cardCode, ref customerPaymentProfileId, ref isSuccess, ref errCode, ref errText);

            if (isSuccess)
            {
                var cardId = this.AddNewCardToDB(clientId, customerPaymentProfileId);
                this.hdnCardId.Value = cardId.ToString();
                errMsg = "";
                return cardId;
            }
            else
            {
                errMsg = errText;
                return 0;
            }
        }

        private int AddNewCardToDB(int clientId, string customerPaymentProfileId)
        {
            objClientPaymentMethodService = ServiceFactory.ClientPaymentMethodService;
            BizObjects.ClientPaymentMethod objClientPaymentMethod = new BizObjects.ClientPaymentMethod();

            objClientPaymentMethod.ClientId = clientId;
            if (rblVisa.Checked)
                objClientPaymentMethod.CardType = "Visa";
            else if (rblMaster.Checked)
                objClientPaymentMethod.CardType = "MasterCard";
            else if (rblDiscover.Checked)
                objClientPaymentMethod.CardType = "Discover";
            else
                objClientPaymentMethod.CardType = "AmericanExpress";

            string cardStr = txtCardNumber.Text.Substring(txtCardNumber.Text.Trim().Length - 4);
            objClientPaymentMethod.NameOnCard = this.txtName.Text;
            objClientPaymentMethod.CardNumber = cardStr.PadLeft(16, '*');
            objClientPaymentMethod.ExpiryMonth = Convert.ToInt16(drpMonth.SelectedValue.ToString());
            objClientPaymentMethod.ExpiryYear = Convert.ToInt32(drpYear.SelectedValue.ToString());
            objClientPaymentMethod.IsDefaultPayment = false;
            objClientPaymentMethod.AddedBy = clientId;
            objClientPaymentMethod.AddedByType = General.UserRoles.Client.GetEnumValue();
            objClientPaymentMethod.AddedDate = DateTime.UtcNow;
            objClientPaymentMethod.CustomerPaymentProfileId = customerPaymentProfileId;

            var cardId = objClientPaymentMethodService.AddClientPaymentMethod(ref objClientPaymentMethod);

            return cardId;
        }

        private string CreatePaymentProfile(int clientId, string cardNumber, string expirationDate, string cardCode, ref string customerPaymentProfileId, ref bool isSuccess, ref string errCode, ref string errText)
        {
            objClientService = ServiceFactory.ClientService;
            DataTable dtClient = null;
            objClientService.GetClientById(clientId, ref dtClient);
            DataRow dr = dtClient.Rows[0];
            var FirstName = dr["FirstName"].ToString();
            var LastName = dr["LastName"].ToString();
            var customerProfileId = dr["CustomerProfileId"].ToString();

            var creditCard = new creditCardType
            {
                cardNumber = cardNumber,
                expirationDate = expirationDate,
                cardCode = cardCode,
            };

            paymentType echeck = new paymentType { Item = creditCard };

            var billTo = new customerAddressType
            {
                firstName = FirstName,
                lastName = LastName
            };

            if (Session["billingAdd"] != null)
            {
                DataTable dtadd = Session["billingAdd"] as DataTable;
                var bdr = dtadd.Rows[0];
                var firstName = bdr["FirstName"].ToString();
                var lastName = bdr["LastName"].ToString();
                var address = bdr["Address"].ToString();
                var zipCode = bdr["ZipCode"].ToString();
                var state = bdr["StateName"].ToString();
                var city = bdr["CityName"].ToString();
                billTo = new customerAddressType
                {
                    firstName = FirstName,
                    lastName = LastName,
                    address = address,
                    state = state,
                    city = city,
                    zip = zipCode
                };
            }

            var helper = new AuthorizeNetHelper();

            helper.CreateCustomerPaymentProfile(customerProfileId, echeck, billTo, ref isSuccess, ref customerPaymentProfileId, ref errCode, ref errText);

            return customerPaymentProfileId;
        }

        protected void imgPdf_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                int ClientId = (Session["ClientLoginCookie"] as LoginModel).Id;
                DataTable dtClient = new DataTable();
                objClientService.GetClientById(ClientId, ref dtClient);
                var request = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["APIURL"].ToString().Trim() + "api/v1/common/SendSalesAgreement?" + "ClientId=" + ClientId.ToString());

                var postData = "ClientId=" + ClientId.ToString();
                var data = Encoding.ASCII.GetBytes(postData);

                request.Method = "GET";
                //request.ContentType = "application/json";

                //using (var stream = request.GetRequestStream())
                //{
                //    stream.Write(data, 0, data.Length);
                //}

                var response = (HttpWebResponse)request.GetResponse();

                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                if (responseString.Contains("200"))
                {
                    dvMessage.InnerHtml = "<strong>An email for Sales Agreement has been sent to your email address.</strong>";
                    dvMessage.Attributes.Add("class", "success");
                    dvMessage.Visible = true;
                    return;
                }
            }
            catch(Exception ex)
            {               
                btnSave_Click(sender, e);
            }
        }
    }
}