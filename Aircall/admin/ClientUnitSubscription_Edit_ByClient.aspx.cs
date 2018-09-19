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
    public partial class ClientUnitSubscription_Edit_ByClient : System.Web.UI.Page
    {
        IClientService objClientService;
        IClientUnitService objClientUnitService;
        IClientUnitSubscriptionService objClientUnitSubscriptionService;
        IClientPaymentMethodService objClientPaymentMethodService;
        IStripeErrorLogService objStripeErrorLogService;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                objClientUnitService = ServiceFactory.ClientUnitService;
                BindMonthYearDropdown();
                ShowHideDiv();
                FillPaymentMethodDropdown();

                if (!string.IsNullOrEmpty(Request.QueryString["ClientId"]))
                {
                    objClientUnitSubscriptionService = ServiceFactory.ClientUnitSubscriptionService;
                    DataTable dtSubscription = new DataTable();
                    int ClientId = Convert.ToInt16(Request.QueryString["ClientId"].ToString());
                    objClientUnitSubscriptionService.GetClientUnitUnPaidSubscription(ClientId, ref dtSubscription);

                    if (dtSubscription.Rows.Count > 0)
                    {
                        FillSubscription(dtSubscription);
                    }

                    FillChecks(ClientId);
                }
            }
        }

        private void FillSubscription(DataTable dt)
        {
            var dr = dt.Rows[0];
            ltrClientName.Text = dr["ClientName"].ToString();
            ltlCompanyName.Text = dr["Company"].ToString();
            hdnClientId.Value = dr["ClientId"].ToString();
            ltrTotalUnits.Text = dr["TotalUnitCount"].ToString();
            ltrPricePerMonth.Text = "$" + dr["PricePerMonth"].ToString();
            ltrTotalAmount.Text = "$" + dr["TotalAmount"].ToString();
            this.hdfPricePerMonth.Value = dr["PricePerMonth"].ToString();
            this.txtAdjustPricePerMonth.Text = dr["PricePerMonth"].ToString();
            this.hdfTotalAmount.Value = dr["TotalAmount"].ToString();
            var unitIds = dr["UnPaidClientUnitIds"].ToString();
            if (unitIds.StartsWith(","))
            {
                unitIds = unitIds.Substring(1);
            }
            this.hdfUnitIds.Value = unitIds;
            this.drpPaidMonths_SelectedIndexChanged(null, null);
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

            //decimal amount = Convert.ToDecimal(this.hdfPricePerMonth.Value);
            decimal amount = Convert.ToDecimal(this.txtAdjustPricePerMonth.Text);

            helper.CreateSubscriptionFromCustomerProfile(customerProfileId, customerPaymentProfileId, customerAddressId, schedule, amount, ref isSuccess, ref subscriptionId, ref errCode, ref errText);

            return isSuccess;
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

        private void FillChecks(int ClientId)
        {
            List<CheckItem> list = new List<CheckItem>();
            for (var i = 1; i <= 5; i++)
            {
                list.Add(new CheckItem { Sr = i, CheckNumber = "", Amount = null });
            }
            this.lstChecks.DataSource = list;
            this.lstChecks.DataBind();
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

        private void FillClientPaymentCard(int ClientId)
        {
            drpCard.DataSource = "";
            drpCard.DataBind();

            drpCard.Items.Insert(0, new ListItem("Select Card", "0"));
            drpCard.Items.Insert(1, new ListItem("Enter New Card", "-1"));

            objClientPaymentMethodService = ServiceFactory.ClientPaymentMethodService;
            DataTable dtPaymentMethods = new DataTable();
            objClientPaymentMethodService.GetClientPaymentMethodByClientId(ClientId, ref dtPaymentMethods);
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

        private void FillStatusDropdown(string Status)
        {
            var values = DurationExtensions.GetValues<General.UnitSubscriptionStatus>();
            List<string> data = new List<string>();
            foreach (var item in values)
            {
                if (item.ToString().ToLower() != "fail") //Code Added on 05-07-2017 for payment related issue 
                {
                    General.UnitSubscriptionStatus p = (General.UnitSubscriptionStatus)item;
                    data.Add(p.GetEnumDescription());
                }
            }
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
                    drpPaidMonths_SelectedIndexChanged(null, null);
                    break;
                case General.PaymentMethod.CC:
                    dvCard.Visible = true;
                    dvPO.Visible = false;
                    dvCheck.Visible = false;
                    if (!string.IsNullOrEmpty(hdnClientId.Value))
                    {
                        FillClientPaymentCard(Convert.ToInt32(hdnClientId.Value));
                    }
                    break;
                case General.PaymentMethod.PO:
                    dvCard.Visible = false;
                    dvPO.Visible = true;
                    dvCheck.Visible = true;
                    drpPaidMonths_SelectedIndexChanged(null, null);
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

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    LoginModel objLoginModel = new LoginModel();
                    objLoginModel = Session["LoginSession"] as LoginModel;
                    int CardId = 0;

                    if (!string.IsNullOrEmpty(Request.QueryString["ClientId"]))
                    {
                        BizObjects.ClientUnitSubscription objClientUnitSubscription = new BizObjects.ClientUnitSubscription();
                        objClientUnitSubscriptionService = ServiceFactory.ClientUnitSubscriptionService;

                        Int64 Id = Convert.ToInt64(Request.QueryString["ClientId"].ToString());

                        string[] AllowedFileExtensions = new string[] { ".jpg", ".gif", ".png", ".jpeg" };
                        string frontImageName = string.Empty;
                        string backImageName = string.Empty;

                        if (drpPayment.SelectedValue.ToString() != General.PaymentMethod.CC.GetEnumDescription())
                        {
                            if (fpdFront.HasFile)
                            {
                                if (fpdBack.HasFile)
                                {
                                    if (!AllowedFileExtensions.Contains(fpdFront.FileName.Substring(fpdFront.FileName.LastIndexOf('.')))
                                && !AllowedFileExtensions.Contains(fpdBack.FileName.Substring(fpdBack.FileName.LastIndexOf('.'))))
                                    {
                                        dvMessage.InnerHtml = "<strong>Please select file of type: " + string.Join(", ", AllowedFileExtensions) + "</strong>";
                                        dvMessage.Attributes.Add("class", "alert alert-error");
                                        dvMessage.Visible = true;
                                        return;
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
                        else
                        {
                            //Add New Client Card Start
                            objClientPaymentMethodService = ServiceFactory.ClientPaymentMethodService;
                            objClientService = ServiceFactory.ClientService;
                            if (drpCard.SelectedValue == "-1")
                            {
                                DataTable dtClient = new DataTable();
                                string StripeCustomerId = string.Empty;

                                objClientService.GetClientById(Convert.ToInt32(hdnClientId.Value), ref dtClient);
                                if (dtClient.Rows.Count > 0)
                                    StripeCustomerId = dtClient.Rows[0]["StripeCustomerId"].ToString();

                                BizObjects.ClientPaymentMethod objClientPaymentMethod = new BizObjects.ClientPaymentMethod();

                                objClientPaymentMethod.ClientId = Convert.ToInt32(hdnClientId.Value);
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
                                objClientPaymentMethod.ExpiryMonth = Convert.ToInt16(drpMonth.SelectedValue.ToString()); //Convert.ToInt16(txtMonth.Text.Trim());
                                objClientPaymentMethod.ExpiryYear = Convert.ToInt32(drpYear.SelectedValue.ToString()); //Convert.ToInt32(txtYear.Text.Trim());
                                objClientPaymentMethod.IsDefaultPayment = false;
                                objClientPaymentMethod.AddedBy = objLoginModel.Id;
                                objClientPaymentMethod.AddedByType = objLoginModel.RoleId;
                                objClientPaymentMethod.AddedDate = DateTime.UtcNow;

                                try
                                {
                                    var customerService = new StripeCustomerService();
                                    var myCustomer = customerService.Get(StripeCustomerId);

                                    // setting up the card
                                    var myCard = new StripeCardCreateOptions();

                                    myCard.SourceCard = new SourceCard()
                                    {
                                        Number = txtCardNumber.Text.Trim(),
                                        ExpirationYear = drpYear.SelectedValue.ToString(), //txtYear.Text.Trim(),
                                        ExpirationMonth = drpMonth.SelectedValue.ToString(), //txtMonth.Text.Trim(),
                                        Name = txtCardName.Text.Trim(),
                                        Cvc = txtCVV.Text.Trim()
                                    };

                                    var cardService = new StripeCardService();
                                    StripeCard stripeCard = cardService.Create(StripeCustomerId, myCard);
                                    if (string.IsNullOrEmpty(stripeCard.Id))
                                    {
                                        dvMessage.InnerHtml = "Invalid card.";
                                        dvMessage.Attributes.Add("class", "alert alert-error");
                                        dvMessage.Visible = true;
                                        return;
                                    }
                                    //objClientPaymentMethod.StripeCardId = stripeCard.Id;
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
                                    objStripeErrorLog.Userid = Convert.ToInt32(hdnClientId.Value);
                                    objStripeErrorLog.UnitId = 0;
                                    objStripeErrorLog.DateAdded = DateTime.UtcNow;

                                    objStripeErrorLogService.AddStripeErrorLog(ref objStripeErrorLog);

                                    dvMessage.InnerHtml = stex.StripeError.Message.ToString();
                                    dvMessage.Attributes.Add("class", "alert alert-error");
                                    dvMessage.Visible = true;
                                    return;
                                }
                                CardId = objClientPaymentMethodService.AddClientPaymentMethod(ref objClientPaymentMethod);
                            }
                            else
                            {
                                CardId = Convert.ToInt32(drpCard.SelectedValue.ToString());
                            }
                        }


                        objClientUnitSubscription.Id = Id;
                        if (drpPayment.SelectedValue.ToString() == General.PaymentMethod.CC.GetEnumDescription())
                            objClientUnitSubscription.CardId = CardId;//Convert.ToInt32(drpCard.SelectedValue.ToString());
                        else
                        {
                            objClientUnitSubscription.PONumber = txtPoNo.Text.Trim();
                            objClientUnitSubscription.FrontImage = frontImageName;
                            objClientUnitSubscription.BackImage = backImageName;
                        }
                        objClientUnitSubscription.AccountingNotes = txtAccountNotes.Text;
                        objClientUnitSubscription.Status = "Paid";
                        if (drpPayment.SelectedValue == General.PaymentMethod.CC.GetEnumDescription())
                        {
                            objClientUnitSubscription.Status = "Paid";
                        }
                        else
                        {
                            objClientUnitSubscription.Status = "Paid";
                        }

                        var BillingId = objClientUnitSubscriptionService.UpdateClientUnitSubscriptionServiceById(ref objClientUnitSubscription);

                        var status = "Paid";

                        if (status == General.UnitSubscriptionStatus.Paid.GetEnumDescription())
                        {
                            long NotificationId = 0;
                            int BadgeCount = 0;
                            string message = string.Empty;
                            int NotificationType;
                            BizObjects.UserNotification objUserNotification = new BizObjects.UserNotification();
                            IUserNotificationService objUserNotificationService;

                            message = General.GetNotificationMessage("PaymentSuccessForSubscriptionInvoice");

                            message = message.Replace("{{UnitName}}", hdnUnitName.Value);
                            message = message.Replace("{{MonthYear}}", PaymentDueDate.Value);

                            objUserNotificationService = ServiceFactory.UserNotificationService;
                            objUserNotification.UserId = int.Parse(hdnClientId.Value);
                            objUserNotification.UserTypeId = General.UserRoles.Client.GetEnumValue();
                            objUserNotification.Message = message;
                            objUserNotification.Status = General.NotificationStatus.UnRead.GetEnumDescription();


                            NotificationType = General.NotificationType.SubscriptionInvoicePaymentFailed.GetEnumValue();
                            objUserNotification.MessageType = General.NotificationType.SubscriptionInvoicePaymentFailed.GetEnumDescription();
                            objUserNotification.CommonId = BillingId;

                            objUserNotification.AddedDate = DateTime.UtcNow;
                            NotificationId = objUserNotificationService.AddUserNotification(ref objUserNotification);

                            DataTable dtBadgeCount = new DataTable();
                            dtBadgeCount.Clear();

                            objUserNotificationService.GetBadgeCount(int.Parse(hdnClientId.Value), General.UserRoles.Client.GetEnumValue(), ref dtBadgeCount);
                            BadgeCount = dtBadgeCount.Rows.Count;

                            Notifications objNotifications = new Notifications { NId = NotificationId, NType = NotificationType, CommonId = objUserNotification.CommonId };
                            List<NotificationModel> notify = new List<NotificationModel>();
                            notify.Add(new NotificationModel { Key = "NId", Value = new object[] { objNotifications.NId } });
                            notify.Add(new NotificationModel { Key = "NType", Value = new object[] { objNotifications.NType } });
                            notify.Add(new NotificationModel { Key = "CommonId", Value = new object[] { objNotifications.CommonId } });

                            if (!string.IsNullOrEmpty(DeviceType.Value) &&
                                !string.IsNullOrEmpty(DeviceToken.Value) &&
                                DeviceToken.Value.ToLower() != "no device token")
                            {
                                if (DeviceType.Value.ToLower() == "android")
                                {
                                    string CustomData = "&data.NId=" + objNotifications.NId + "&data.NType=" + objNotifications.NType + "&data.CommonId=" + objNotifications.CommonId;
                                    SendNotifications.SendAndroidNotification(DeviceToken.Value, message, CustomData, "client");
                                }
                                else if (DeviceType.Value.ToLower() == "iphone")
                                {
                                    SendNotifications.SendIphoneNotification(BadgeCount, DeviceToken.Value, message, notify, "client");
                                }
                            }
                        }
                    }

                    Session["msg"] = "edit";
                    Response.Redirect(Application["SiteAddress"] + "admin/ClientUnitSubscription_List.aspx");
                }
                catch (Exception Ex)
                {
                    dvMessage.InnerHtml = "<strong>" + Ex.Message.ToString().Trim() + "</strong>";
                    dvMessage.Attributes.Add("class", "alert alert-error");
                    dvMessage.Visible = true;
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Session["LoginSession"] != null)
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
        }

        private void PayByCC()
        {
            int clientId = Convert.ToInt16(Request.QueryString["ClientId"].ToString());
            string customerPaymentProfileId = "";
            string errMsg = "";
            var cardId = 0;

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
                objClientPaymentMethodService.GetClientPaymentMethodById(cardId, ref dtPaymentCard);
                if (dtPaymentCard.Rows.Count > 0)
                    customerPaymentProfileId = dtPaymentCard.Rows[0]["CustomerPaymentProfileId"].ToString();
            }

            if (!string.IsNullOrEmpty(errMsg))
            {
                DisplayErrorMessage(errMsg);
                return;
            }

            string authorizeNetSubscriptionId = "";
            string errCode = "";
            string errText = "";
            var isSuccess = AddSubscriptionToAuthorizeNet(clientId, customerPaymentProfileId, ref authorizeNetSubscriptionId, ref errCode, ref errText);

            if (!isSuccess)
            {
                errMsg = errCode + " " + errText;
                DisplayErrorMessage(errMsg);
                return;
            }

            LoginModel objLoginModel = new LoginModel();
            objLoginModel = Session["LoginSession"] as LoginModel;

            var objClientAddressService = ServiceFactory.ClientAddressService;
            DataTable dtAddress = new DataTable();
            objClientAddressService.GetClientAddressesByClientId(clientId, true, ref dtAddress);

            int AddressId = Convert.ToInt32(dtAddress.Rows[0]["Id"]);
            var totalAmount = Convert.ToDecimal(this.hdfTotalAmount.Value);
            //var pricePerMonth = Convert.ToDecimal(this.hdfPricePerMonth.Value);
            var pricePerMonth = Convert.ToDecimal(this.txtAdjustPricePerMonth.Text);

            var orderId = AddOrder(clientId, objLoginModel, customerPaymentProfileId, AddressId, totalAmount, "Charge", "CC");

            UpdatePricePerMonth(pricePerMonth, orderId);

            //Add Client Unit Subscription Start
            var arr = this.hdfUnitIds.Value.Split(',');
            var unitId = Convert.ToInt32(arr[0]);
            BizObjects.ClientUnitSubscription objClientUnitSubscription = new BizObjects.ClientUnitSubscription();
            objClientUnitSubscriptionService = ServiceFactory.ClientUnitSubscriptionService;
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
            objClientUnitSubscription.AccountingNotes = this.txtAccountNotes.Text;
            objClientUnitSubscription.PricePerMonth = pricePerMonth;
            objClientUnitSubscription.Amount = totalAmount;
            objClientUnitSubscription.AddedBy = objLoginModel.Id;
            objClientUnitSubscription.AddedByType = objLoginModel.RoleId;
            objClientUnitSubscription.AddedDate = DateTime.UtcNow.ToLocalTime();
            objClientUnitSubscription.TotalUnits = Convert.ToInt32(this.ltrTotalUnits.Text);

            var unitSubscriptionId = objClientUnitSubscriptionService.AddClientUnitSubscriptionService(ref objClientUnitSubscription, false);
            UpdateAuthorizeNetSubscriptionId(authorizeNetSubscriptionId, unitSubscriptionId);
            this.drpPaidMonths.SelectedValue = "1";
            //UpdatePaidUntilDate(unitSubscriptionId, 1);
            UpdateAdjustInfo(unitSubscriptionId);
            //var billingHistoryId = AddBillingHistory(clientId, orderId, unitId, pricePerMonth * 1, 1, General.BillingTypes.Recurringpurchase.GetEnumDescription(), objLoginModel, unitSubscriptionId, cardId, string.Empty, string.Empty);
            UpdateClientUnitSubscriptionId(clientId, unitSubscriptionId);
            UpdateSubmittedFlag(clientId);

            Session["msg"] = "add";
            Response.Redirect("ClientUnitSubscription_List_UnSubmitted.aspx");
        }

        private void PayByCheck()
        {
            int clientId = Convert.ToInt16(Request.QueryString["ClientId"].ToString());
            string customerPaymentProfileId = "";
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

            LoginModel objLoginModel = new LoginModel();
            objLoginModel = Session["LoginSession"] as LoginModel;

            var objClientAddressService = ServiceFactory.ClientAddressService;
            DataTable dtAddress = new DataTable();
            objClientAddressService.GetClientAddressesByClientId(clientId, true, ref dtAddress);

            int AddressId = Convert.ToInt32(dtAddress.Rows[0]["Id"]);
            var totalAmount = Convert.ToDecimal(this.hdfTotalAmount.Value);
            //var pricePerMonth = Convert.ToDecimal(this.hdfPricePerMonth.Value);
            var pricePerMonth = Convert.ToDecimal(this.txtAdjustPricePerMonth.Text);

            var orderId = AddOrder(clientId, objLoginModel, customerPaymentProfileId, AddressId, totalAmount, "Charge", this.drpPayment.SelectedValue.ToString());

            UpdatePricePerMonth(pricePerMonth, orderId);

            //Add Client Unit Subscription Start
            var arr = this.hdfUnitIds.Value.Split(',');
            var unitId = Convert.ToInt32(arr[0]);
            BizObjects.ClientUnitSubscription objClientUnitSubscription = new BizObjects.ClientUnitSubscription();
            objClientUnitSubscriptionService = ServiceFactory.ClientUnitSubscriptionService;
            objClientUnitSubscription.ClientId = clientId;
            objClientUnitSubscription.UnitId = unitId;
            objClientUnitSubscription.ClientUnitIds = this.hdfUnitIds.Value;
            objClientUnitSubscription.OrderId = orderId;
            objClientUnitSubscription.PaymentMethod = this.drpPayment.SelectedValue.ToString();
            objClientUnitSubscription.CardId = 0;
            objClientUnitSubscription.PONumber = this.drpPayment.SelectedValue.ToString() == "PO" ? this.txtPoNo.Text : string.Empty;
            objClientUnitSubscription.CheckNumbers = checkNumbers;
            objClientUnitSubscription.CheckAmounts = checkAmounts;
            objClientUnitSubscription.FrontImage = frontImageName;
            objClientUnitSubscription.BackImage = backImageName;
            objClientUnitSubscription.AccountingNotes = this.txtAccountNotes.Text;
            objClientUnitSubscription.PricePerMonth = pricePerMonth;
            objClientUnitSubscription.Amount = totalAmount;
            objClientUnitSubscription.AddedBy = objLoginModel.Id;
            objClientUnitSubscription.AddedByType = objLoginModel.RoleId;
            objClientUnitSubscription.AddedDate = DateTime.UtcNow.ToLocalTime();
            objClientUnitSubscription.TotalUnits = Convert.ToInt32(this.ltrTotalUnits.Text);

            var unitSubscriptionId = objClientUnitSubscriptionService.AddClientUnitSubscriptionService(ref objClientUnitSubscription, false);
            int paidMonths = Convert.ToInt32(this.drpPaidMonths.SelectedValue);
            //UpdatePaidUntilDate(unitSubscriptionId, paidMonths);
            UpdateAdjustInfo(unitSubscriptionId);
            var billingHistoryId = AddBillingHistory(clientId, orderId, unitId, pricePerMonth * paidMonths, paidMonths, this.drpPayment.SelectedValue.ToString(), objLoginModel, unitSubscriptionId, 0, checkNumbers, checkAmounts, this.txtPoNo.Text);
            //UpdateBillingHistoryPaidUntilDate(billingHistoryId);
            UpdateAdditionalInfo(frontImageName, backImageName, this.txtAccountNotes.Text, billingHistoryId);
            UpdateClientUnitSubscriptionId(clientId, unitSubscriptionId);
            UpdateSubmittedFlag(clientId);
            UpdatePaymentStatusToReceived(clientId, this.hdfUnitIds.Value);

            Session["msg"] = "add";
            Response.Redirect("ClientUnitSubscription_List_UnSubmitted.aspx");
        }

        private int AddBillingHistory(int clientId, int orderId, int unitId, decimal amount, int paidMonths, string billingType, LoginModel objLoginModel, int clientUnitSubscriptionId, int cardId, string checkNumbers, string checkAmounts, string PO)
        {
            BizObjects.BillingHistory objBillingHistory = new BizObjects.BillingHistory();
            var objBillingHistoryService = ServiceFactory.BillingHistoryService;

            objBillingHistory.ClientId = clientId;
            objBillingHistory.ClientUnitIds = this.hdfUnitIds.Value;
            objBillingHistory.ClientUnitSubscriptionId = clientUnitSubscriptionId;
            objBillingHistory.PaidMonths = paidMonths;
            objBillingHistory.OrderId = orderId;
            objBillingHistory.PackageName = "";
            objBillingHistory.BillingType = billingType;
            objBillingHistory.OriginalAmount = amount;
            objBillingHistory.PurchasedAmount = amount;
            objBillingHistory.IsSpecialOffer = false;
            objBillingHistory.IsPaid = true;
            objBillingHistory.TransactionId = "";
            objBillingHistory.TransactionDate = DateTime.UtcNow.ToLocalTime();
            objBillingHistory.AddedBy = objLoginModel.Id;
            objBillingHistory.AddedDate = DateTime.UtcNow.ToLocalTime();
            objBillingHistory.CardId = cardId;
            objBillingHistory.CheckNumbers = checkNumbers;
            objBillingHistory.CheckAmounts = checkAmounts;
            objBillingHistory.faildesc = "Payment Success!";
            objBillingHistory.PO = PO;
            objBillingHistory.TransactionStatus = "";
            objBillingHistory.ResponseReasonDescription = "";
            objBillingHistory.ResponseCode = 0;

            return objBillingHistoryService.AddClientUnitBillingHistory(ref objBillingHistory);
        }

        private void UpdateAdditionalInfo(string frontImageName, string backImageName, string accountingNotes, int newBillingId)
        {
            var sql = string.Format("update BillingHistory set FrontImage = '{0}', BackImage = '{1}', AccountingNotes = '{2}' where Id = {3}", frontImageName, backImageName, accountingNotes, newBillingId);
            var helper = new DBUtility.SQLDBHelper();
            helper.ExecuteSQL(sql, null);
            helper.Close();
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

        private void UpdateAdjustInfo(int unitSubscriptionId)
        {
            var sql = string.Format("update ClientUnitSubscription set IsAdjusted = @IsAdjusted, AdjustComment = @AdjustComment, OriginalPricePerMonth = @OriginalPricePerMonth where Id = {0}", unitSubscriptionId);
            SqlParameter[] paramArr = new SqlParameter[]
            {
                new SqlParameter("@IsAdjusted", this.chkAdjust.Checked),
                new SqlParameter("@AdjustComment", this.txtAdjustCommment.Text),
                new SqlParameter("@OriginalPricePerMonth", Convert.ToDecimal(this.hdfPricePerMonth.Value))
            };
            var instance = new SQLDBHelper();
            instance.ExecuteSQL(sql, paramArr);
            instance.Close();
        }

        private void UpdateSubmittedFlag(int clientId)
        {
            var sql = string.Format("update dbo.ClientUnit set IsSubmittedToSubscription = '1' where ClientId = {0} and Id in ({1})", clientId, this.hdfUnitIds.Value);
            var instance = new SQLDBHelper();
            instance.ExecuteSQL(sql, null);
            instance.Close();
        }

        private void UpdateClientUnitSubscriptionId(int clientId, int clientUnitSubscriptionId)
        {
            var sql = string.Format("update dbo.ClientUnit set ClientUnitSubscriptionId = {2} where ClientId = {0} and Id in ({1})", clientId, this.hdfUnitIds.Value, clientUnitSubscriptionId);
            var instance = new SQLDBHelper();
            instance.ExecuteSQL(sql, null);
            instance.Close();
        }

        private void UpdatePaymentStatusToReceived(int clientId, string unitIds)
        {
            var sql = string.Format("update dbo.ClientUnit set PaymentStatus = '{2}' where ClientId = {0} and Id in ({1})", clientId, unitIds, "Received");
            var instance = new SQLDBHelper();
            instance.ExecuteSQL(sql, null);
            instance.Close();
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

        private void NotifyClientForFailedPayment(int ClientId, string UnitName, int CardId, int BillingId, Aircall.Common.StripeResponse response)
        {
            objClientService = ServiceFactory.ClientService;
            DataTable dtClient = new DataTable();
            objClientService.GetClientById(ClientId, ref dtClient);
            if (dtClient.Rows.Count > 0)
            {
                long NotificationId = 0;
                int BadgeCount = 0;
                string message = string.Empty;
                int NotificationType;

                BizObjects.UserNotification objUserNotification = new BizObjects.UserNotification();
                IUserNotificationService objUserNotificationService;

                message = General.GetNotificationMessage("PaymentFailedForSubscriptionInvoice");

                message = message.Replace("{{UnitName}}", UnitName);
                message = message.Replace("{{MonthYear}}", DateTime.UtcNow.Date.ToString("MMMM yyyy"));
                message = message.Replace("{{Reason}}", response.ex.StripeError.Error);

                objUserNotificationService = ServiceFactory.UserNotificationService;
                objUserNotification.UserId = ClientId;
                objUserNotification.UserTypeId = General.UserRoles.Client.GetEnumValue();
                objUserNotification.Message = message;
                objUserNotification.Status = General.NotificationStatus.UnRead.GetEnumDescription();

                if (response.ex.StripeError.Code != "expired_card")
                {
                    NotificationType = General.NotificationType.SubscriptionInvoicePaymentFailed.GetEnumValue();
                    objUserNotification.MessageType = General.NotificationType.SubscriptionInvoicePaymentFailed.GetEnumDescription();
                    objUserNotification.CommonId = BillingId;
                }
                else
                {
                    NotificationType = General.NotificationType.CreditCardExpiration.GetEnumValue();
                    objUserNotification.CommonId = CardId;
                    objUserNotification.MessageType = General.NotificationType.CreditCardExpiration.GetEnumDescription();
                }

                objUserNotification.AddedDate = DateTime.UtcNow;
                NotificationId = objUserNotificationService.AddUserNotification(ref objUserNotification);

                DataTable dtBadgeCount = new DataTable();
                dtBadgeCount.Clear();

                objUserNotificationService.GetBadgeCount(ClientId, General.UserRoles.Client.GetEnumValue(), ref dtBadgeCount);
                BadgeCount = dtBadgeCount.Rows.Count;

                Notifications objNotifications = new Notifications { NId = NotificationId, NType = NotificationType, CommonId = objUserNotification.CommonId };
                List<NotificationModel> notify = new List<NotificationModel>();
                notify.Add(new NotificationModel { Key = "NId", Value = new object[] { objNotifications.NId } });
                notify.Add(new NotificationModel { Key = "NType", Value = new object[] { objNotifications.NType } });
                notify.Add(new NotificationModel { Key = "CommonId", Value = new object[] { objNotifications.CommonId } });

                if (!string.IsNullOrEmpty(dtClient.Rows[0]["DeviceType"].ToString()) &&
                                                    !string.IsNullOrEmpty(dtClient.Rows[0]["DeviceToken"].ToString()) &&
                                                    dtClient.Rows[0]["DeviceToken"].ToString().ToLower() != "no device token")
                {
                    if (dtClient.Rows[0]["DeviceType"].ToString().ToLower() == "android")
                    {
                        string CustomData = "&data.NId=" + objNotifications.NId + "&data.NType=" + objNotifications.NType + "&data.CommonId=" + objNotifications.CommonId;
                        SendNotifications.SendAndroidNotification(dtClient.Rows[0]["DeviceToken"].ToString(), message, CustomData, "client");
                    }
                    else if (dtClient.Rows[0]["DeviceType"].ToString().ToLower() == "iphone")
                    {
                        SendNotifications.SendIphoneNotification(BadgeCount, dtClient.Rows[0]["DeviceToken"].ToString(), message, notify, "client");
                    }
                }
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("ClientUnitSubscription_List_UnSubmitted.aspx");
        }

        private int AddOrder(int ClientId, LoginModel objLoginModel, string customerPaymentProfileId, int AddressId, decimal totalAmount, string OrderType, string ChargeBy)
        {
            BizObjects.Orders objOrders = new BizObjects.Orders();
            var objOrderService = ServiceFactory.OrderService;
            int OrderId = 0;
            objOrders.OrderType = OrderType;
            objOrders.ClientId = ClientId;
            objOrders.OrderAmount = totalAmount;
            objOrders.ChargeBy = ChargeBy;
            objOrders.AddedBy = objLoginModel.Id;
            objOrders.AddedByType = objLoginModel.RoleId;
            objOrders.AddedDate = DateTime.UtcNow;
            OrderId = objOrderService.AddClientUnitOrder(ref objOrders, customerPaymentProfileId, AddressId);

            return OrderId;
        }

        protected void drpPaidMonths_SelectedIndexChanged(object sender, EventArgs e)
        {
            //var pricePerMonth = Convert.ToDecimal(this.hdfPricePerMonth.Value);
            var pricePerMonth = Convert.ToDecimal(this.txtAdjustPricePerMonth.Text);
            var months = Convert.ToDecimal(this.drpPaidMonths.SelectedValue);
            var amount = pricePerMonth * months;
            this.ltrPayAmount.Text = "$" + amount.ToString("N2");
        }

        protected void chkAdjust_CheckedChanged(object sender, EventArgs e)
        {
            var visible = chkAdjust.Checked;
            this.divAdjust1.Visible = visible;
            this.divAdjust2.Visible = visible;
            if (!visible)
            {
                this.txtAdjustPricePerMonth.Text = this.hdfPricePerMonth.Value;
                drpPaidMonths_SelectedIndexChanged(null, null);
            }
        }

        protected void txtAdjustPricePerMonth_TextChanged(object sender, EventArgs e)
        {
            drpPaidMonths_SelectedIndexChanged(null, null);
        }
    }

    public class CheckItem
    {
        public int Sr { get; set; }
        public string CheckNumber { get; set; }
        public decimal? Amount { get; set; }
    }
}