using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Services;
using Aircall.Common;
using System.Data;
using System.IO;
using Stripe;
using AuthorizeNet.Api.Contracts.V1;
using AuthorizeNetLib;
using DBUtility;
using System.Data.SqlClient;

namespace Aircall.admin
{
    public partial class Repay : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["bid"]))
                {
                    BindMonthYearDropdown();
                    FillPaymentMethodDropdown();

                    var billingId = Convert.ToInt32(Request.QueryString["bid"]);
                    this.hdfBillingId.Value = billingId.ToString();
                    var dr = GetBillingInfo(billingId);
                    var checkNumbers = dr["CheckNumbers"].ToString();
                    var checkAmounts = dr["CheckAmounts"].ToString();
                    var billingType = dr["BillingType"].ToString();

                    if (billingType == "Recurring Purchase" || billingType == "CC")
                    {
                        this.drpPayment.SelectedValue = "CC";
                    }
                    else if (billingType == "Check")
                    {
                        this.drpPayment.SelectedValue = "Check";
                    }
                    else if (billingType == "PO")
                    {
                        this.drpPayment.SelectedValue = "PO";
                    }

                    this.hdfClientId.Value = dr["ClientId"].ToString();
                    FillClientPaymentCard(Convert.ToInt32(hdfClientId.Value));
                    ShowHideDiv();
                    FillChecks(checkNumbers, checkAmounts);
                    this.txtPoNo.Text = dr["PO"].ToString();
                    //this.txtAccountNotes.Text = dr["AccountingNotes"].ToString();
                    //if (string.IsNullOrEmpty(this.txtAccountNotes.Text))
                    //{
                    //    this.txtAccountNotes.Text = dr["faildesc"].ToString();
                    //}

                    ltrTotalAmount.Text = "$" + dr["PurchasedAmount"].ToString();
                    this.hdfTotalAmount.Value = dr["PurchasedAmount"].ToString();
                    ltrClientName.Text = dr["ClientName"].ToString();
                    ltlCompanyName.Text = dr["Company"].ToString();

                    if (dr["CardId"].ToString() != "" && dr["CardId"].ToString() != "0")
                    {
                        this.drpCard.SelectedValue = dr["CardId"].ToString();
                    }
                }
            }
        }

        private DataRow GetBillingInfo(int billingId)
        {
            var sql = string.Format("select top 1 A.*, B.FirstName + ' ' + B.LastName as ClientName, B.Company from BillingHistory A inner join Client B on A.ClientId = B.Id where A.Id = {0}", billingId);
            var helper = new DBUtility.SQLDBHelper();
            var ds = helper.Query(sql, null);
            helper.Close();
            return ds.Tables[0].Rows[0];
        }

        private void FillChecks(string checkNumbers, string checkAmounts)
        {
            if (!string.IsNullOrEmpty(checkNumbers))
            {
                var arr1 = checkNumbers.Split(',');
                var arr2 = checkNumbers.Split(',');
                var list = new List<CheckItem>();
                for (int i = 0; i < arr1.Length; i++)
                {
                    var checkNumber = arr1[i];
                    var checkAmount = Convert.ToDecimal(arr2[i]);
                    list.Add(new CheckItem { Sr = i + 1, CheckNumber = checkNumber, Amount = checkAmount });
                }
                for (var i = arr1.Length; i < 5; i++)
                {
                    var checkNumber = "";
                    decimal? checkAmount = null;
                    list.Add(new CheckItem { Sr = i + 1, CheckNumber = checkNumber, Amount = checkAmount });
                }
                this.lstChecks.DataSource = list;
                this.lstChecks.DataBind();
            }
            else
            {
                var list = new List<CheckItem>();
                for (var i = 0; i < 5; i++)
                {
                    var checkNumber = "";
                    decimal? checkAmount = null;
                    list.Add(new CheckItem { Sr = i + 1, CheckNumber = checkNumber, Amount = checkAmount });
                }
                this.lstChecks.DataSource = list;
                this.lstChecks.DataBind();
            }
        }

        private void BindMonthYearDropdown()
        {
            drpMonth.Enabled = true;
            drpYear.Enabled = true;

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
        }

        private void FillClientPaymentCard(int clientId)
        {
            drpCard.DataSource = "";
            drpCard.DataBind();

            drpCard.Items.Insert(0, new ListItem("Select Card", "0"));
            drpCard.Items.Insert(1, new ListItem("Enter New Card", "-1"));

            var objClientPaymentMethodService = ServiceFactory.ClientPaymentMethodService;
            DataTable dtPaymentMethods = new DataTable();
            objClientPaymentMethodService.GetClientPaymentMethodByClientId(clientId, ref dtPaymentMethods);

            if (dtPaymentMethods.Rows.Count > 0)
            {
                drpCard.DataSource = dtPaymentMethods;
                drpCard.DataTextField = dtPaymentMethods.Columns["CardNumber"].ToString();
                drpCard.DataValueField = dtPaymentMethods.Columns["Id"].ToString();
                drpCard.DataBind();

                drpCard.Items.Insert(0, new ListItem("Select Card", "0"));
                drpCard.Items.Insert(dtPaymentMethods.Rows.Count + 1, new ListItem("Enter New Card", "-1"));
            }
        }

        private void FillPaymentMethodDropdown()
        {
            var values = DurationExtensions.GetValues<General.PaymentMethod>();
            List<string> data = new List<string>();
            foreach (var item in values)
            {
                General.PaymentMethod p = (General.PaymentMethod)item;
                data.Add(p.GetEnumDescription());
            }
            drpPayment.DataSource = data;
            drpPayment.DataBind();
            drpPayment.Items.Insert(0, new ListItem("Select", "0"));
        }

        protected void drpPayment_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowHideDiv();
        }

        public void ShowHideDiv()
        {

            General.PaymentMethod p = Enum.GetValues(typeof(General.PaymentMethod)).Cast<General.PaymentMethod>().FirstOrDefault(v => v.GetEnumDescription() == drpPayment.SelectedValue);
            switch (p)
            {
                case General.PaymentMethod.Check:
                    dvCard.Visible = false;
                    dvPO.Visible = false;
                    dvCheck.Visible = true;
                    break;
                case General.PaymentMethod.CC:
                    dvCard.Visible = true;
                    dvPO.Visible = false;
                    dvCheck.Visible = false;
                    break;
                case General.PaymentMethod.PO:
                    dvCard.Visible = false;
                    dvPO.Visible = true;
                    dvCheck.Visible = true;
                    break;
                default:
                    dvCard.Visible = false;
                    dvPO.Visible = false;
                    dvCheck.Visible = false;
                    break;
            }

        }

        protected void drpCard_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (drpCard.SelectedValue.ToString() == "-1")
                dvNewCard.Visible = true;
            else
                dvNewCard.Visible = false;
        }

        private void DisplayErrorMessage(string errMsg)
        {
            dvMessage.InnerHtml = "<strong>" + errMsg + "</strong>";
            dvMessage.Attributes.Add("class", "alert alert-error");
            dvMessage.Visible = true;
        }

        private void DisplaySuccessMessage(string errMsg)
        {
            dvMessage.InnerHtml = "<strong>" + errMsg + "</strong>";
            dvMessage.Attributes.Add("class", "alert alert-success");
            dvMessage.Visible = true;
        }

        private void HideMessage()
        {
            dvMessage.Visible = false;
        }

        private void ValidCheck(ref string errMsg, ref string checkNumbers, ref string checkAmounts)
        {
            for (var i = 0; i < this.lstChecks.Items.Count; i++)
            {
                var item = this.lstChecks.Items[i];
                TextBox txtCheckNumber = item.FindControl("txtCheckNumber") as TextBox;
                TextBox txtAmount = item.FindControl("txtAmount") as TextBox;
                if (!string.IsNullOrEmpty(txtCheckNumber.Text))
                {
                    decimal amount = 0;
                    if (!decimal.TryParse(txtAmount.Text, out amount))
                    {
                        errMsg = "Check amount needs to be numberic.";
                        return;
                    }
                    else
                    {
                        if (checkNumbers != "")
                        {
                            checkNumbers += ",";
                        }
                        checkNumbers += txtCheckNumber.Text;
                        if (checkAmounts != "")
                        {
                            checkAmounts += ",";
                        }
                        checkAmounts += txtAmount.Text;
                    }
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

        private string CreatePaymentProfile(int clientId, string cardNumber, string expirationDate, string cardCode, ref string customerPaymentProfileId, ref bool isSuccess, ref string errCode, ref string errText)
        {
            var objClientService = ServiceFactory.ClientService;
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

        private int AddNewCardToDB(int clientId, string customerPaymentProfileId)
        {
            var objClientPaymentMethodService = ServiceFactory.ClientPaymentMethodService;
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
            objClientPaymentMethod.NameOnCard = txtCardName.Text.Trim();
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

        private void UploadImages(ref string errMsg, ref string frontImageName, ref string backImageName)
        {
            string[] AllowedFileExtensions = new string[] { ".jpg", ".gif", ".png", ".jpeg" };

            if (fpdFront.HasFile)
            {
                if (fpdBack.HasFile)
                {
                    if (!AllowedFileExtensions.Contains(fpdFront.FileName.Substring(fpdFront.FileName.LastIndexOf('.')))
                && !AllowedFileExtensions.Contains(fpdBack.FileName.Substring(fpdBack.FileName.LastIndexOf('.'))))
                    {
                        errMsg = "Please select file of type: " + string.Join(", ", AllowedFileExtensions);
                    }
                    else
                    {
                        frontImageName = DateTime.UtcNow.Ticks.ToString().Trim() + "-front" + System.IO.Path.GetExtension(fpdFront.FileName);
                        fpdFront.PostedFile.SaveAs(Path.Combine(Server.MapPath("~/uploads/checkImages/"), frontImageName));

                        backImageName = DateTime.UtcNow.Ticks.ToString().Trim() + "-back" + System.IO.Path.GetExtension(fpdBack.FileName);
                        fpdBack.PostedFile.SaveAs(Path.Combine(Server.MapPath("~/uploads/checkImages/"), backImageName));
                    }
                }
            }
            else
            {
                frontImageName = hdnFront.Value;
                backImageName = hdnBack.Value;
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("BillingHistory_View.aspx");
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (drpPayment.SelectedValue.ToString() == General.PaymentMethod.CC.GetEnumDescription())
            {
                PayByCC();
            }
            else
            {
                PayByCheck();
            }
        }

        private void PayByCC()
        {
            int clientId = Convert.ToInt16(this.hdfClientId.Value);
            string customerPaymentProfileId = "";
            string errMsg = "";
            var cardId = 0;
            var customerProfileId = GetCustomerProfileId(clientId);

            if (drpCard.SelectedValue == "-1")
            {
                cardId = AddNewCard(clientId, ref customerPaymentProfileId, ref errMsg);

                if (!string.IsNullOrEmpty(errMsg))
                {
                    DisplayErrorMessage(errMsg);
                    return;
                }
            }
            else
            {
                cardId = Convert.ToInt32(drpCard.SelectedValue.ToString());
                DataTable dtPaymentCard = new DataTable();
                var objClientPaymentMethodService = ServiceFactory.ClientPaymentMethodService;
                objClientPaymentMethodService.GetClientPaymentMethodById(cardId, ref dtPaymentCard);
                if (dtPaymentCard.Rows.Count > 0)
                    customerPaymentProfileId = dtPaymentCard.Rows[0]["CustomerPaymentProfileId"].ToString();
            }

            if (!string.IsNullOrEmpty(errMsg))
            {
                DisplayErrorMessage(errMsg);
                return;
            }

            var billingId = Convert.ToInt32(this.hdfBillingId.Value);
            this.hdfBillingId.Value = billingId.ToString();
            var dr = GetBillingInfo(billingId);
            var objLoginModel = Session["LoginSession"] as LoginModel;
            BizObjects.BillingHistory objBillingHistory = new BizObjects.BillingHistory();
            var objBillingHistoryService = ServiceFactory.BillingHistoryService;
            var amount = Convert.ToDecimal(dr["PurchasedAmount"]);

            string errCode = "";
            string errText = "";
            string transactionId = "";
            string authCode = "";

            var temp = PayByAuthorizeNet(ref errCode, ref errText, customerProfileId, customerPaymentProfileId, amount, ref transactionId, ref authCode);

            if (!temp)
            {
                errMsg = errText;
                if (!string.IsNullOrEmpty(errText))
                {
                    errMsg = errText;
                    
                }
                else
                {
                    errMsg = "Pay by AuthorizeNet failed.";
                }

                DisplayErrorMessage(errMsg);
                FillClientPaymentCard(clientId);
                this.drpCard.SelectedValue = cardId.ToString();
                this.drpCard_SelectedIndexChanged(null, null);
                return;
            }

            objBillingHistory.ClientId = clientId;
            objBillingHistory.ClientUnitIds = dr["ClientUnitIds"].ToString();
            objBillingHistory.ClientUnitSubscriptionId = Convert.ToInt32(dr["ClientUnitSubscriptionId"]);
            objBillingHistory.PaidMonths = Convert.ToInt32(dr["PaidMonths"]);
            objBillingHistory.OrderId = Convert.ToInt32(dr["OrderId"]);
            objBillingHistory.PackageName = "";
            objBillingHistory.BillingType = this.drpPayment.SelectedValue;
            objBillingHistory.OriginalAmount = amount;
            objBillingHistory.PurchasedAmount = amount;
            objBillingHistory.IsSpecialOffer = false;
            objBillingHistory.IsPaid = true;
            objBillingHistory.TransactionId = transactionId;
            objBillingHistory.TransactionDate = DateTime.UtcNow.ToLocalTime();
            objBillingHistory.AddedBy = objLoginModel.Id;
            objBillingHistory.AddedDate = DateTime.UtcNow.ToLocalTime();
            objBillingHistory.CardId = cardId;
            objBillingHistory.CheckNumbers = "";
            objBillingHistory.CheckAmounts = "";
            objBillingHistory.faildesc = "Payment Success! Auth code is " + authCode;
            objBillingHistory.PO = "";
            objBillingHistory.TransactionStatus = "";
            objBillingHistory.ResponseReasonDescription = "";
            objBillingHistory.ResponseCode = 0;

            var newBillingId = objBillingHistoryService.AddClientUnitBillingHistory(ref objBillingHistory);
            UpdateRePayId(billingId, newBillingId);
            UpdateAdditionalInfo("", "", this.txtAccountNotes.Text, newBillingId);

            if (!string.IsNullOrEmpty(dr["ClientUnitIds"].ToString()) && !string.IsNullOrEmpty(dr["ClientUnitSubscriptionId"].ToString()))
            {
                UpdatePaymentStatusToReceived(clientId, dr["ClientUnitIds"].ToString());
            }

            Response.Redirect("BillingHistory_List.aspx?msg=success");
        }

        private void UpdatePaymentStatusToReceived(int clientId, string unitIds)
        {
            var sql = string.Format("update dbo.ClientUnit set PaymentStatus = '{2}' where ClientId = {0} and Id in ({1})", clientId, unitIds, "Received");
            var instance = new SQLDBHelper();
            instance.ExecuteSQL(sql, null);
            instance.Close();
        }

        private bool PayByAuthorizeNet(ref string errCode, ref string errText, string customerProfileId, string customerPaymentProfileId, decimal amount, ref string transId, ref string authCode)
        {
            bool isSuccess = false;
            var helper = new AuthorizeNetHelper();
            helper.ChargeCustomerProfile(customerProfileId, customerPaymentProfileId, amount, ref isSuccess, ref transId, ref authCode, ref errCode, ref errText);
            return isSuccess;
        }

        private string GetCustomerProfileId(int clientId)
        {
            var sql = string.Format("select top 1 A.CustomerProfileId from Client A where A.Id = {0}", clientId);
            var helper = new DBUtility.SQLDBHelper();
            var ds = helper.Query(sql, null);
            helper.Close();
            return ds.Tables[0].Rows[0]["CustomerProfileId"].ToString();
        }

        private void PayByCheck()
        {
            int clientId = Convert.ToInt16(this.hdfClientId.Value);
            string frontImageName = "";
            string backImageName = "";
            string errMsg = "";

            UploadImages(ref errMsg, ref frontImageName, ref backImageName);

            if (!string.IsNullOrEmpty(errMsg))
            {
                DisplayErrorMessage(errMsg);
                return;
            }

            string checkNumbers = "";
            string checkAmounts = "";
            ValidCheck(ref errMsg, ref checkNumbers, ref checkAmounts);

            if (!string.IsNullOrEmpty(errMsg))
            {
                DisplayErrorMessage(errMsg);
                return;
            }

            var billingId = Convert.ToInt32(this.hdfBillingId.Value);
            this.hdfBillingId.Value = billingId.ToString();
            var dr = GetBillingInfo(billingId);
            var objLoginModel = Session["LoginSession"] as LoginModel;
            BizObjects.BillingHistory objBillingHistory = new BizObjects.BillingHistory();
            var objBillingHistoryService = ServiceFactory.BillingHistoryService;
            var amount = Convert.ToDecimal(dr["PurchasedAmount"]);

            objBillingHistory.ClientId = clientId;
            objBillingHistory.ClientUnitIds = dr["ClientUnitIds"].ToString();
            objBillingHistory.ClientUnitSubscriptionId = Convert.ToInt32(dr["ClientUnitSubscriptionId"]);
            objBillingHistory.PaidMonths = Convert.ToInt32(dr["PaidMonths"]);
            objBillingHistory.OrderId = Convert.ToInt32(dr["OrderId"]);
            objBillingHistory.PackageName = "";
            objBillingHistory.BillingType = this.drpPayment.SelectedValue;
            objBillingHistory.OriginalAmount = amount;
            objBillingHistory.PurchasedAmount = amount;
            objBillingHistory.IsSpecialOffer = false;
            objBillingHistory.IsPaid = true;
            objBillingHistory.TransactionId = "";
            objBillingHistory.TransactionDate = DateTime.UtcNow.ToLocalTime();
            objBillingHistory.AddedBy = objLoginModel.Id;
            objBillingHistory.AddedDate = DateTime.UtcNow.ToLocalTime();
            objBillingHistory.CardId = 0;
            objBillingHistory.CheckNumbers = checkNumbers;
            objBillingHistory.CheckAmounts = checkAmounts;
            objBillingHistory.faildesc = "Payment Success!";
            objBillingHistory.PO = this.txtPoNo.Text;
            objBillingHistory.TransactionStatus = "";
            objBillingHistory.ResponseReasonDescription = "";
            objBillingHistory.ResponseCode = 0;

            var newBillingId = objBillingHistoryService.AddClientUnitBillingHistory(ref objBillingHistory);
            UpdateRePayId(billingId, newBillingId);
            UpdateAdditionalInfo(frontImageName, backImageName, this.txtAccountNotes.Text, newBillingId);

            Response.Redirect("BillingHistory_List.aspx?msg=success");
        }

        private void UpdateRePayId(int billingId, int newBillingId)
        {
            var sql = string.Format("update BillingHistory set RePayBillingId = {0} where Id = {1}", newBillingId, billingId);
            var helper = new DBUtility.SQLDBHelper();
            helper.ExecuteSQL(sql, null);
            helper.Close();
        }

        private void UpdateAdditionalInfo(string frontImageName, string backImageName, string accountingNotes, int newBillingId)
        {
            var sql = string.Format("update BillingHistory set FrontImage = '{0}', BackImage = '{1}', AccountingNotes = '{2}' where Id = {3}", frontImageName, backImageName, accountingNotes, newBillingId);
            var helper = new DBUtility.SQLDBHelper();
            helper.ExecuteSQL(sql, null);
            helper.Close();
        }
    }
}