using Aircall.Common;
using AuthorizeNet.Api.Contracts.V1;
using AuthorizeNetLib;
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
    public partial class payment_method : System.Web.UI.Page
    {
        #region "Declaration"
        IClientService objClientService = ServiceFactory.ClientService;
        IClientPaymentMethodService objClientPaymentMethodService = ServiceFactory.ClientPaymentMethodService;
        IStripeErrorLogService objStripeErrorLogService = ServiceFactory.StripeErrorLogService;
        #endregion

        #region "Page Events"
        protected void Page_Load(object sender, EventArgs e)
        {
            pnsCC.DefaultButton = btnSave.ID;
            if (!IsPostBack)
            {
                BindMonthYearDropdown();
                hdnCardMode.Value = "0";
                ltrCardText.Text = "Add New Card";
                if (Session["success"] != null)
                {
                    if (Session["success"].ToString() == "1")
                    {
                        dvMessage.InnerHtml = "Card saved Successfully.";
                        dvMessage.Attributes.Add("class", "success");
                        dvMessage.Visible = true;
                        Session["success"] = null;
                    }
                }
                if (Session["ClientLoginCookie"] != null)
                {
                    BindClientPaymentMethods();
                }
                else
                    Response.Redirect(Application["SiteAddress"] + "sign-in.aspx");
            }
        }

        private void BindMonthYearDropdown()
        {
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
        #endregion

        #region "Functions"
        private void BindClientPaymentMethods()
        {
            int ClientId = (Session["ClientLoginCookie"] as LoginModel).Id;
            DataTable dtClientPaymentMethod = new DataTable();
            objClientPaymentMethodService.GetClientPaymentMethodByClientId(ClientId, ref dtClientPaymentMethod);
            if (dtClientPaymentMethod.Rows.Count > 0)
                lstPaymentMethod.DataSource = dtClientPaymentMethod;
            lstPaymentMethod.DataBind();
        }
        #endregion

        #region "Events"
        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                DateTime dt2 = new DateTime(int.Parse(drpYear.SelectedValue), int.Parse(drpMonth.SelectedValue), 1);
                DateTime dt = dt2.AddMonths(1).AddDays(-1);
                var ts = dt.Subtract(DateTime.UtcNow);

                if (ts.Ticks <= 0)
                {
                    dvMessage.InnerHtml = "Invalid Exp Month and Exp Year.";
                    dvMessage.Attributes.Add("class", "error");
                    dvMessage.Visible = true;
                    return;
                }
                try
                {
                    BizObjects.ClientPaymentMethod objClientPaymentMethod = new BizObjects.ClientPaymentMethod();
                    DataTable oldCC = new DataTable();
                    int ClientId = (Session["ClientLoginCookie"] as LoginModel).Id;
                    string cardStr = txtCardNumber.Text.Substring(txtCardNumber.Text.Trim().Length - 4);

                    objClientPaymentMethodService.GetClientPaymentMethodByClientId(ClientId, ref oldCC);

                    var rows = oldCC.Select(" CardNumber ='" + cardStr.PadLeft(16, '*') + "'");
                    if (rows.Length > 0)
                    {
                        dvMessage.InnerHtml = "CC Already Exists!";
                        dvMessage.Attributes.Add("class", "error");
                        dvMessage.Visible = true;
                        hdnCardMode.Value = "1";
                        return;
                    }

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

                    dvMessage.InnerHtml = "Card saved Successfully.";
                    dvMessage.Attributes.Add("class", "success");
                    dvMessage.Visible = true;

                    BindClientPaymentMethods();
                    ClearControl();
                }
                catch (Exception Ex)
                {
                    dvMessage.InnerHtml = Ex.Message.ToString().Trim();
                    dvMessage.Attributes.Add("class", "error");
                    dvMessage.Visible = true;
                }
            }
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
            objClientPaymentMethod.IsDefaultPayment = this.chkIsDefault.Checked;
            objClientPaymentMethod.AddedBy = clientId;
            objClientPaymentMethod.AddedByType = General.UserRoles.Client.GetEnumValue();
            objClientPaymentMethod.AddedDate = DateTime.UtcNow;
            objClientPaymentMethod.CustomerPaymentProfileId = customerPaymentProfileId;

            var cardId = objClientPaymentMethodService.AddClientPaymentMethod(ref objClientPaymentMethod);

            if (chkIsDefault.Checked)
            {
                var instance = new DBUtility.SQLDBHelper();
                var sql = string.Format("update ClientPaymentMethod set IsDefaultPayment = '{0}' where Id <> {1} and ClientId = {2}", "0", cardId, clientId);
                instance.ExecuteSQL(sql, null);
                instance.Close();
            }

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

            var helper = new AuthorizeNetHelper();

            helper.CreateCustomerPaymentProfile(customerProfileId, echeck, billTo, ref isSuccess, ref customerPaymentProfileId, ref errCode, ref errText);

            return customerPaymentProfileId;
        }
        protected void lstPaymentMethod_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            if (e.CommandName == "EditCard" && e.CommandArgument.ToString() != "0")
            {
                ViewState["CardId"] = e.CommandArgument.ToString();
                ltrCardText.Text = "Edit Card";
                int ClientPaymentId = Convert.ToInt32(e.CommandArgument.ToString());
                DataTable dtCardInfo = new DataTable();
                objClientPaymentMethodService.GetClientPaymentMethodById(ClientPaymentId, ref dtCardInfo);
                if (dtCardInfo.Rows.Count > 0)
                {
                    if (dtCardInfo.Rows[0]["CardType"].ToString() == "Visa")
                    {
                        rblVisa.Checked = true;
                        rblMaster.Checked = false;
                        rblDiscover.Checked = false;
                        rblAmex.Checked = false;
                    }
                    else if (dtCardInfo.Rows[0]["CardType"].ToString() == "MasterCard")
                    {
                        rblMaster.Checked = true;
                        rblVisa.Checked = false;
                        rblDiscover.Checked = false;
                        rblAmex.Checked = false;
                    }
                    else if (dtCardInfo.Rows[0]["CardType"].ToString() == "Discover")
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

                    txtName.Text = dtCardInfo.Rows[0]["NameOnCard"].ToString();
                    txtCardNumber.Text = dtCardInfo.Rows[0]["CardNumber"].ToString();
                    //txtMonth.Text = dtCardInfo.Rows[0]["ExpiryMonth"].ToString();
                    //txtYear.Text = dtCardInfo.Rows[0]["ExpiryYear"].ToString();
                    drpMonth.SelectedValue = dtCardInfo.Rows[0]["ExpiryMonth"].ToString();
                    drpYear.SelectedValue = dtCardInfo.Rows[0]["ExpiryYear"].ToString();
                    chkIsDefault.Checked = Convert.ToBoolean(dtCardInfo.Rows[0]["IsDefaultPayment"].ToString());
                    hdnChkIsDefault.Checked = Convert.ToBoolean(dtCardInfo.Rows[0]["IsDefaultPayment"].ToString());
                    btnSave.Visible = false;
                    btnUpdate.Visible = true;
                    txtCardNumber.Enabled = false;
                    regExpCard.Enabled = false;
                    pnsCC.DefaultButton = btnUpdate.ID;
                }
                hdnCardMode.Value = "1";
            }
        }
        public void ClearControl()
        {
            rblVisa.Checked = true;
            rblMaster.Checked = false;
            rblDiscover.Checked = false;
            rblAmex.Checked = false;
            txtCardNumber.Text = txtName.Text = txtCVV.Text = string.Empty;
            txtCardNumber.Enabled = true;
            drpMonth.SelectedValue = drpYear.SelectedValue = "0";
            btnUpdate.Visible = false;
            btnSave.Visible = true;
            ltrCardText.Text = "Add New Card";
            chkIsDefault.Checked = false;
            pnsCC.DefaultButton = btnSave.ID;
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            var cardId = Convert.ToInt32(ViewState["CardId"].ToString());
            var sql = string.Format("update ClientPaymentMethod set IsDefaultPayment = '{0}' where Id = {1}", chkIsDefault.Checked ? "1" : "0", cardId);
            var instance = new DBUtility.SQLDBHelper();
            instance.ExecuteSQL(sql, null);
            if (chkIsDefault.Checked)
            {
                int clientId = (Session["ClientLoginCookie"] as LoginModel).Id;
                sql = string.Format("update ClientPaymentMethod set IsDefaultPayment = '{0}' where Id <> {1} and ClientId = {2}", "0", cardId, clientId);
                instance.ExecuteSQL(sql, null);
            }
            instance.Close();
            //BindClientPaymentMethods();
            //ClearControl();
            Session["success"] = 2;
            Response.Redirect(Application["SiteAddress"] + "client/payment-method.aspx");
        }

        protected void btnUpdate_Click_old(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    if (hdnChkIsDefault.Checked == true && chkIsDefault.Checked == false)
                    {
                        dvMessage1.InnerHtml = "Please select another card as Default first.";
                        dvMessage1.Attributes.Add("class", "error");
                        dvMessage1.Visible = true;
                        return;
                    }
                    if (ViewState["CardId"] != null)
                    {
                        BizObjects.ClientPaymentMethod objClientPaymentMethod = new BizObjects.ClientPaymentMethod();
                        int ClientId = (Session["ClientLoginCookie"] as LoginModel).Id;
                        DataTable dtClient = new DataTable();
                        objClientService.GetClientById(ClientId, ref dtClient);

                        DataTable dtCardInfo = new DataTable();
                        objClientPaymentMethodService.GetClientPaymentMethodById(Convert.ToInt32(ViewState["CardId"].ToString()), ref dtCardInfo);

                        objClientPaymentMethod.Id = Convert.ToInt32(ViewState["CardId"].ToString());
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
                        //objClientPaymentMethod.CardNumber = cardStr.PadLeft(16, '*');
                        objClientPaymentMethod.ExpiryMonth = Convert.ToInt16(drpMonth.SelectedValue);
                        objClientPaymentMethod.ExpiryYear = Convert.ToInt32(drpYear.SelectedValue);
                        objClientPaymentMethod.IsDefaultPayment = chkIsDefault.Checked;
                        objClientPaymentMethod.UpdatedBy = ClientId;
                        objClientPaymentMethod.UpdatedByType = General.UserRoles.Client.GetEnumValue();
                        objClientPaymentMethod.UpdatedDate = DateTime.UtcNow;
                        try
                        {
                            if (chkIsDefault.Checked)
                            {
                                StripeCustomerUpdateOptions up = new StripeCustomerUpdateOptions();
                                up.DefaultSource = dtCardInfo.Rows[0]["StripeCardId"].ToString();
                                new StripeCustomerService().Update(dtClient.Rows[0]["StripeCustomerId"].ToString(), up);
                            }
                            //stripe card update
                            var myCard = new StripeCardUpdateOptions();

                            myCard.Name = txtName.Text.Trim();
                            myCard.ExpirationYear = drpYear.SelectedValue;
                            myCard.ExpirationMonth = drpMonth.SelectedValue;

                            var cardService = new StripeCardService();
                            StripeCard stripeCard = cardService.Update(dtClient.Rows[0]["StripeCustomerId"].ToString(), dtCardInfo.Rows[0]["StripeCardId"].ToString(), myCard);
                            if (string.IsNullOrEmpty(stripeCard.Id))
                            {
                                dvMessage.InnerHtml = "Invalid card.";
                                dvMessage.Attributes.Add("class", "error");
                                dvMessage.Visible = true;
                                return;
                            }
                            //objClientPaymentMethod.StripeCardId = stripeCard.Id;
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

                            dvMessage.InnerHtml = stex.StripeError.Message.ToString();
                            dvMessage.Attributes.Add("class", "error");
                            dvMessage.Visible = true;
                            return;
                        }

                        objClientPaymentMethodService.UpdateClientPaymentMethod(ref objClientPaymentMethod);
                        BindClientPaymentMethods();
                        ClearControl();
                        Session["success"] = 2;
                        Response.Redirect(Application["SiteAddress"] + "client/payment-method.aspx", false);
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

        protected void lnkAdd_Click(object sender, ImageClickEventArgs e)
        {
            ClearControl();
            hdnCardMode.Value = "1";
        }
    }
}