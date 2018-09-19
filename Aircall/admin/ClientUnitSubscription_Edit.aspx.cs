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

namespace Aircall.admin
{
    public partial class ClientUnitSubscription_Edit : System.Web.UI.Page
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

                if (!string.IsNullOrEmpty(Request.QueryString["SubId"]))
                {
                    objClientUnitSubscriptionService = ServiceFactory.ClientUnitSubscriptionService;
                    DataTable dtSubscription = new DataTable();
                    Int64 Id = Convert.ToInt64(Request.QueryString["SubId"].ToString());
                    objClientUnitSubscriptionService.GetClientUnitSubscriptionById(Id, ref dtSubscription);
                    if (dtSubscription.Rows.Count > 0)
                    {
                        ltrClientName.Text = dtSubscription.Rows[0]["ClientName"].ToString();
                        hdnClientId.Value = dtSubscription.Rows[0]["Id"].ToString();
                        hdnUnitName.Value = ltrUnit.Text = dtSubscription.Rows[0]["UnitName"].ToString();
                        ltrDueDate.Text = Convert.ToDateTime(dtSubscription.Rows[0]["PaymentDueDate"].ToString()).ToString("MM/dd/yyyy");
                        PaymentDueDate.Value = Convert.ToDateTime(dtSubscription.Rows[0]["PaymentDueDate"].ToString()).ToString("MMMM yyyy");
                        drpPayment.SelectedValue = dtSubscription.Rows[0]["PaymentMethod"].ToString();
                        hdnPayment.Value = dtSubscription.Rows[0]["PaymentMethod"].ToString();
                        DeviceToken.Value = dtSubscription.Rows[0]["DeviceToken"].ToString();
                        DeviceType.Value = dtSubscription.Rows[0]["DeviceType"].ToString();
                        ShowHideDiv();
                        drpPayment.Enabled = false;
                        lnkChangePaymentMethod.HRef = Application["SiteAddress"] + "admin/ChangePaymentMethod.aspx?SubId=" + Id.ToString();

                        if (dtSubscription.Rows[0]["PaymentMethod"].ToString() == General.PaymentMethod.CC.GetEnumDescription())
                        {
                            FillClientPaymentCard(Convert.ToInt32(dtSubscription.Rows[0]["Id"].ToString()));
                            drpCard.SelectedValue = dtSubscription.Rows[0]["CardId"].ToString();
                            if (dtSubscription.Rows[0]["Status"].ToString() == General.UnitSubscriptionStatus.Paid.GetEnumDescription())
                                rqfvCVV.Enabled = false;
                        }
                        else
                        {
                            btnSave.Text = "Save";
                            txtPoNo.Text = dtSubscription.Rows[0]["PONumber"].ToString();
                            txtCheck.Text = dtSubscription.Rows[0]["CheckNumber"].ToString();
                            if (!string.IsNullOrEmpty(dtSubscription.Rows[0]["FrontImage"].ToString()))
                            {
                                hdnFront.Value = dtSubscription.Rows[0]["FrontImage"].ToString();
                                lnkFront.HRef = Application["SiteAddress"] + "uploads/checkImages/" + dtSubscription.Rows[0]["FrontImage"].ToString();
                                lnkFront.Visible = true;
                            }
                            if (!string.IsNullOrEmpty(dtSubscription.Rows[0]["BackImage"].ToString()))
                            {
                                hdnBack.Value = dtSubscription.Rows[0]["BackImage"].ToString();
                                lnkBack.HRef = Application["SiteAddress"] + "uploads/checkImages/" + dtSubscription.Rows[0]["BackImage"].ToString();
                                lnkBack.Visible = true;
                            }
                        }
                        FillStatusDropdown(dtSubscription.Rows[0]["Status"].ToString());
                        txtAccountNotes.Text = dtSubscription.Rows[0]["AccountingNotes"].ToString();
                        drpStatus.SelectedValue = dtSubscription.Rows[0]["Status"].ToString();
                        hdnStatus.Value = dtSubscription.Rows[0]["Status"].ToString();

                        //Code Added on 07-07-2017
                        if(drpStatus.SelectedValue == General.UnitSubscriptionStatus.Paid.GetEnumDescription())
                        {
                            //drpStatus.Enabled = false;
                        }
                        //Code end
                        if (dtSubscription.Rows[0]["Status"].ToString() == General.UnitSubscriptionStatus.Paid.GetEnumDescription())
                        {
                            btnSave.Visible = false;
                            btnUpdate.Visible = true;
                        }
                    }
                }
                else
                {
                    FillStatusDropdown("");
                }
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
            drpStatus.DataSource = data;
            drpStatus.DataBind();
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
                    if (!string.IsNullOrEmpty(hdnClientId.Value))
                    {
                        FillClientPaymentCard(Convert.ToInt32(hdnClientId.Value));
                    }
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

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    LoginModel objLoginModel = new LoginModel();
                    objLoginModel = Session["LoginSession"] as LoginModel;
                    int CardId = 0;

                    if (!string.IsNullOrEmpty(Request.QueryString["SubId"]))
                    {
                        BizObjects.ClientUnitSubscription objClientUnitSubscription = new BizObjects.ClientUnitSubscription();
                        objClientUnitSubscriptionService = ServiceFactory.ClientUnitSubscriptionService;

                        Int64 Id = Convert.ToInt64(Request.QueryString["SubId"].ToString());

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

                            if (txtCheck.Text.Trim() == "")
                            {
                                dvMessage.InnerHtml = "<strong>Please insert check number!</strong>";
                                dvMessage.Attributes.Add("class", "alert alert-error");
                                dvMessage.Visible = true;
                                return;
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
                            objClientUnitSubscription.CheckNumbers = txtCheck.Text.Trim();
                            objClientUnitSubscription.FrontImage = frontImageName;
                            objClientUnitSubscription.BackImage = backImageName;
                        }
                        objClientUnitSubscription.AccountingNotes = txtAccountNotes.Text;
                        if (drpPayment.SelectedValue == General.PaymentMethod.CC.GetEnumDescription())
                        {
                            objClientUnitSubscription.Status = (hdnStatus.Value == General.UnitSubscriptionStatus.Paid.GetEnumDescription() ? hdnStatus.Value : drpStatus.SelectedValue.ToString());
                        }
                        else
                        {
                            objClientUnitSubscription.Status = drpStatus.SelectedValue.ToString();
                        }

                        var BillingId = objClientUnitSubscriptionService.UpdateClientUnitSubscriptionServiceById(ref objClientUnitSubscription);

                        if (drpStatus.SelectedValue == General.UnitSubscriptionStatus.Paid.GetEnumDescription())
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

                    //Redirect("edit");
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
            if (Page.IsValid)
            {
                try
                {
                    bool insFlag = false;

                    //Code Added on 06-07-2017
                    if (txtCheck.Text.Trim() == "" && drpPayment.SelectedValue.ToString() == General.PaymentMethod.PO.GetEnumDescription())
                    {
                        dvMessage.InnerHtml = "<strong>Please insert check number!</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-error");
                        dvMessage.Visible = true;
                        return;
                    }
                    if (!string.IsNullOrEmpty(Request.QueryString["SubId"]))
                    {
                        LoginModel objLoginModel = new LoginModel();
                        objLoginModel = Session["LoginSession"] as LoginModel;

                        BizObjects.ClientUnitSubscription objClientUnitSubscription = new BizObjects.ClientUnitSubscription();
                        objClientUnitSubscriptionService = ServiceFactory.ClientUnitSubscriptionService;

                        Int64 Id = Convert.ToInt64(Request.QueryString["SubId"].ToString());

                        string[] AllowedFileExtensions = new string[] { ".jpg", ".gif", ".png", ".jpeg" };
                        string frontImageName = string.Empty;
                        string backImageName = string.Empty;
                        string StripeCardId = string.Empty;
                        string StripeCustomerId = string.Empty;
                        string FailedCode = string.Empty;
                        string FailedDesc = string.Empty;
                        int CardId = 0;
                        decimal Amount = 0;
                        string Status = string.Empty;

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
                            int ClientId = 0;

                            ClientId = Convert.ToInt32(hdnClientId.Value);
                            DataTable dtClient = new DataTable();
                            objClientService = ServiceFactory.ClientService;
                            objClientService.GetClientById(ClientId, ref dtClient);
                            if (dtClient.Rows.Count > 0)
                                StripeCustomerId = dtClient.Rows[0]["StripeCustomerId"].ToString();

                            //Add New Client Card Start
                            objClientPaymentMethodService = ServiceFactory.ClientPaymentMethodService;
                            if (drpCard.SelectedValue == "-1")
                            {
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
                                objClientPaymentMethod.NameOnCard = txtCardName.Text.Trim();
                                objClientPaymentMethod.CardNumber = cardStr.PadLeft(16, '*');
                                objClientPaymentMethod.ExpiryMonth = Convert.ToInt16(drpMonth.SelectedValue.ToString()); //Convert.ToInt16(txtMonth.Text.Trim());
                                objClientPaymentMethod.ExpiryYear = Convert.ToInt32(drpYear.SelectedValue.ToString()); //Convert.ToInt32(txtYear.Text.Trim());
                                objClientPaymentMethod.IsDefaultPayment = false;
                                objClientPaymentMethod.AddedBy = ClientId;
                                objClientPaymentMethod.AddedByType = General.UserRoles.Client.GetEnumValue();
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
                                    dvMessage.Attributes.Add("class", "alert alert-error");
                                    dvMessage.Visible = true;
                                    return;
                                }
                                CardId = objClientPaymentMethodService.AddClientPaymentMethod(ref objClientPaymentMethod);
                            }
                            else
                            {
                                CardId = Convert.ToInt32(drpCard.SelectedValue.ToString());
                                DataTable dtPaymentCard = new DataTable();
                                objClientPaymentMethodService.GetClientPaymentMethodById(CardId, ref dtPaymentCard);
                                if (dtPaymentCard.Rows.Count > 0)
                                    StripeCardId = dtPaymentCard.Rows[0]["StripeCardId"].ToString();
                            }
                            //Add New Client Card End

                            //UnPaid Condition Added on Code 06-07-2017
                            #region Paid Status
                            if (drpStatus.SelectedValue.ToString() == General.UnitSubscriptionStatus.Paid.GetEnumDescription() || drpStatus.SelectedValue.ToString() == General.UnitSubscriptionStatus.UnPaid.GetEnumDescription())
                            {
                                //Take Payment From Card Start
                                try
                                {
                                    DataTable dtSubscription = new DataTable();
                                    objClientUnitSubscriptionService.GetClientUnitSubscriptionById(Id, ref dtSubscription);


                                    if (dtSubscription.Rows.Count > 0)
                                    {
                                        string InvoiceNumber = dtSubscription.Rows[0]["InvoiceNumber"].ToString();
                                        string PlanName = dtSubscription.Rows[0]["Name"].ToString();
                                        string UnitName = dtSubscription.Rows[0]["UnitName"].ToString();
                                        Amount = Convert.ToDecimal(dtSubscription.Rows[0]["PlanAmount"].ToString());

                                        string desc = "Payment Received For Invoice No: " + InvoiceNumber + " of Plan " + PlanName;
                                        var StripeResponse = new Aircall.Common.StripeResponse();

                                        StripeResponse = General.StripeCharge(true, "", StripeCustomerId, StripeCardId, Convert.ToInt32(Amount * 100), desc, "");
                                        #region Stripe Error
                                        if (StripeResponse.ex != null)
                                        {
                                            var stex = StripeResponse.ex;
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
                                            objStripeErrorLog.DateAdded = DateTime.UtcNow;

                                            objStripeErrorLogService.AddStripeErrorLog(ref objStripeErrorLog);
                                            FailedCode = stex.StripeError.Code;
                                            FailedDesc = "Payment Failed!";//stex.StripeError.Error;
                                            Status = General.UnitSubscriptionStatus.Fail.GetEnumDescription();

                                            objClientUnitSubscription.Id = Id;
                                            objClientUnitSubscription.CardId = CardId;
                                            objClientUnitSubscription.PONumber = string.Empty;
                                            objClientUnitSubscription.CheckNumbers = string.Empty;
                                            objClientUnitSubscription.FrontImage = string.Empty;
                                            objClientUnitSubscription.BackImage = string.Empty;
                                            objClientUnitSubscription.AccountingNotes = txtAccountNotes.Text;
                                            objClientUnitSubscription.Amount = Amount;
                                            objClientUnitSubscription.Status = Status;
                                            objClientUnitSubscription.AddedBy = objLoginModel.Id;
                                            objClientUnitSubscription.AddedByType = objLoginModel.RoleId;
                                            objClientUnitSubscription.AddedDate = DateTime.UtcNow;

                                            int BillingId = objClientUnitSubscriptionService.UpdateClientUnitSubscriptionService(ref objClientUnitSubscription, FailedCode, FailedDesc);



                                            NotifyClientForFailedPayment(ClientId, UnitName, CardId, BillingId, StripeResponse);
                                            Session["msg"] = "edit";
                                            Response.Redirect(Application["SiteAddress"] + "admin/ClientUnitSubscription_List.aspx");
                                            return;
                                        }
                                        #endregion
                                        else
                                        {

                                            Status = General.UnitSubscriptionStatus.Paid.GetEnumDescription();
                                            //pp
                                            BizObjects.Orders objOrders = new BizObjects.Orders();
                                            IOrderService objOrderService = ServiceFactory.OrderService;
                                            int OrderId = 0;
                                            objOrders.OrderType = "Charge";
                                            //Code Commented on 01/07/2017
                                            //objOrders.ClientId = int.Parse(dtSubscription.Rows[0]["SubId"].ToString()); 
                                            //objOrders.OrderAmount = decimal.Parse(dtSubscription.Rows[0]["PurchasedAmount"].ToString()); 

                                            objOrders.ClientId = int.Parse(dtSubscription.Rows[0]["Id"].ToString());
                                            objOrders.OrderAmount = decimal.Parse(dtSubscription.Rows[0]["PlanAmount"].ToString());

                                            objOrders.ChargeBy = General.PaymentMethod.CC.GetEnumDescription();
                                            objOrders.AddedBy = 1;
                                            objOrders.AddedByType = 1;
                                            objOrders.AddedDate = DateTime.UtcNow;
                                            OrderId = objOrderService.AddClientUnitOrderForSchedular(ref objOrders, StripeCardId, Convert.ToInt32(dtSubscription.Rows[0]["AddressId"].ToString()));
                                            insFlag = true;
                                        }
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
                                    objStripeErrorLog.Userid = ClientId;
                                    objStripeErrorLog.DateAdded = DateTime.UtcNow;

                                    objStripeErrorLogService.AddStripeErrorLog(ref objStripeErrorLog);
                                    FailedCode = stex.StripeError.Code;
                                    FailedDesc = "Payment Failed!"; //stex.StripeError.Error;
                                    Status = General.UnitSubscriptionStatus.Fail.GetEnumDescription();
                                }
                                //Take Payment From Card End
                            }
                            #endregion

                          

                        }

                        objClientUnitSubscription.Id = Id;
                        if (drpPayment.SelectedValue.ToString() == General.PaymentMethod.CC.GetEnumDescription())
                            objClientUnitSubscription.CardId = CardId;
                        else
                        {
                            //Status = drpStatus.SelectedValue.ToString(); // Code Commented on 10/07/2017
                            Status = General.UnitSubscriptionStatus.Paid.GetEnumDescription(); // // Code Added on 10/07/2017 to make payment either it is paid or unpaid
                            objClientUnitSubscription.PONumber = txtPoNo.Text.Trim();
                            objClientUnitSubscription.CheckNumbers = txtCheck.Text.Trim();
                            objClientUnitSubscription.FrontImage = frontImageName;
                            objClientUnitSubscription.BackImage = backImageName;
                        }

                       
                        objClientUnitSubscription.AccountingNotes = txtAccountNotes.Text;
                        objClientUnitSubscription.Amount = Amount;
                        objClientUnitSubscription.Status = Status;
                        objClientUnitSubscription.AddedBy = objLoginModel.Id;
                        objClientUnitSubscription.AddedByType = objLoginModel.RoleId;
                        objClientUnitSubscription.AddedDate = DateTime.UtcNow;

                        var BillingId1 = objClientUnitSubscriptionService.UpdateClientUnitSubscriptionService(ref objClientUnitSubscription, FailedCode, FailedDesc);

                        //UnPaid Condition Added on Code 06-07-2017
                        if (Status == General.UnitSubscriptionStatus.Paid.GetEnumDescription() || Status == General.UnitSubscriptionStatus.UnPaid.GetEnumDescription())
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
                            objUserNotification.CommonId = BillingId1;

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
                        Session["msg"] = "edit";
                        Redirect("edit");
                        //Response.Redirect(Application["SiteAddress"] + "admin/ClientUnitSubscription_List.aspx?msg=edit");
                    }
                }
                catch (Exception Ex)
                {
                    dvMessage.InnerHtml = "<strong>" + Ex.Message.ToString().Trim() + "</strong>";
                    dvMessage.Attributes.Add("class", "alert alert-error");
                    dvMessage.Visible = true;
                }
            }
        }

        public void Redirect(string msg)
        {
            if (Session["SubscriptionSearch"] != null)
            {
                SubscriptionSearch objSubscriptionSearch = new SubscriptionSearch();
                objSubscriptionSearch = Session["SubscriptionSearch"] as SubscriptionSearch;

                string Param = string.Empty;
                if (!string.IsNullOrEmpty(objSubscriptionSearch.ClientName))
                    Param = "&CName=" + objSubscriptionSearch.ClientName;

                if (!string.IsNullOrEmpty(objSubscriptionSearch.StartDate.ToString()))
                {
                    if (!string.IsNullOrEmpty(Param))
                        Param = Param + "&SDate=" + objSubscriptionSearch.StartDate.ToString("MM/dd/yyyy");
                    else
                        Param = "&SDate=" + objSubscriptionSearch.StartDate.ToString("MM/dd/yyyy");
                }
                if (!string.IsNullOrEmpty(objSubscriptionSearch.EndDate.ToString()))
                {
                    if (!string.IsNullOrEmpty(Param))
                        Param = Param + "&EDate=" + objSubscriptionSearch.EndDate.ToString("MM/dd/yyyy");
                    else
                        Param = "&EDate=" + objSubscriptionSearch.EndDate.ToString("MM/dd/yyyy");
                }

                if (!string.IsNullOrEmpty(Param))
                    Param = Param + "&Status=" + objSubscriptionSearch.Status.ToString();
                else
                    Param = "&Status=" + objSubscriptionSearch.Status.ToString();

                if (!string.IsNullOrEmpty(Param))
                    Param = Param + "&PMethod=" + objSubscriptionSearch.PaymentMethod.ToString();
                else
                    Param = "&PMethod=" + objSubscriptionSearch.PaymentMethod.ToString();


                Response.Redirect(Application["SiteAddress"] + "admin/ClientUnitSubscription_List.aspx?msg=" + msg + Param);
            }
            else
                Response.Redirect(Application["SiteAddress"] + "admin/ClientUnitSubscription_List.aspx?msg=" + msg);
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
            Redirect("cancel");
        }
    }
}