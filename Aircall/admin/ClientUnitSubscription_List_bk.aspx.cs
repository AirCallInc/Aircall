using Aircall.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Services;
using System.Data;
using System.Web.UI.HtmlControls;

namespace Aircall.admin
{
    public partial class ClientUnitSubscription_List_bk : System.Web.UI.Page
    {
        IClientUnitService objClientUnitService;
        IClientUnitSubscriptionService objClientUnitSubscriptionService;
       
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                objClientUnitService = ServiceFactory.ClientUnitService;
                if (Session["msg"] != null)
                {
                    if (Session["msg"].ToString() == "edit")
                    {
                        dvMessage.InnerHtml = "<strong>Unit Subscription updated successfully.</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-success");
                        dvMessage.Visible = true;
                    }
                    else if (Session["msg"].ToString() == "PChange")
                    {
                        dvMessage.InnerHtml = "<strong>Payment method changed successfully.</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-success");
                        dvMessage.Visible = true;
                    }
                    Session["msg"] = null;
                }
                BindSubscriptions();
               
            }
            
           
        }

        private void BindSubscriptions()
        {
            FillStatusDropdown();
            FillPaymentMethodDropdown();

            if (Session["SubscriptionSearch"] != null)
            {
                Session["SubscriptionSearch"] = null;
                Session.Remove("SubscriptionSearch");
            }

            objClientUnitSubscriptionService = ServiceFactory.ClientUnitSubscriptionService;
            DataTable dtSubscription = new DataTable();
            string ClientName = string.Empty;
            string StartDate = string.Empty;
            string EndDate = string.Empty;
            string Status = string.Empty;
            string PMethod = string.Empty;

            if (!string.IsNullOrEmpty(Request.QueryString["CName"]))
            {
                ClientName = Request.QueryString["CName"].ToString();
                txtClient.Text = ClientName;
            }
            if (!string.IsNullOrEmpty(Request.QueryString["SDate"]))
            {
                StartDate = Request.QueryString["SDate"].ToString();
                txtStart.Value = StartDate;
            }
            if (!string.IsNullOrEmpty(Request.QueryString["EDate"]))
            {
                EndDate = Request.QueryString["EDate"].ToString();
                txtEnd.Value = EndDate;
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Status"]))
            {
                Status = Request.QueryString["Status"].ToString();
                drpStatus.SelectedValue = Status;
            }
            if (!string.IsNullOrEmpty(Request.QueryString["PMethod"]))
            {
                PMethod = Request.QueryString["PMethod"].ToString();
                drpPaymentMethod.SelectedValue = PMethod;
            }

            SubscriptionSearch objSubscriptionSearch = new SubscriptionSearch();

            objSubscriptionSearch.ClientName = txtClient.Text.Trim();
            if (!string.IsNullOrEmpty(txtStart.Value.Trim()))
                objSubscriptionSearch.StartDate = Convert.ToDateTime(txtStart.Value.Trim());
            if (!string.IsNullOrEmpty(txtEnd.Value.Trim()))
                objSubscriptionSearch.EndDate = Convert.ToDateTime(txtEnd.Value.Trim());
            objSubscriptionSearch.Status = drpStatus.SelectedValue.ToString();
            objSubscriptionSearch.PaymentMethod = drpPaymentMethod.SelectedValue.ToString();

            Session["SubscriptionSearch"] = objSubscriptionSearch;


            if (PMethod == "0")
            {
                objClientUnitSubscriptionService.GetClientUnitSubscription(ClientName, StartDate, EndDate, PMethod, Status, ref dtSubscription);
                if (dtSubscription.Rows.Count > 0)
                {
                    lstUnitSubscription.DataSource = dtSubscription;
                    lstUnitSubscription.DataBind();
                }
                DataTable dtSubCard = new DataTable();
                objClientUnitSubscriptionService.GetClientUnitSubscription(ClientName, StartDate, EndDate, General.PaymentMethod.CC.GetEnumDescription(), Status, ref dtSubCard);
                if (dtSubCard.Rows.Count > 0)
                {
                    dvCard.Visible = true;
                    lstUnitSubscriptionCard.DataSource = dtSubCard;
                    lstUnitSubscriptionCard.DataBind();
                }
            }
            else
            {
                if (PMethod.ToString() == General.PaymentMethod.CC.GetEnumDescription())
                    lnkPaidUnpaid.Visible = false;
                else
                    lnkPaidUnpaid.Visible = true;
                objClientUnitSubscriptionService.GetClientUnitSubscription(ClientName, StartDate, EndDate, PMethod, Status, ref dtSubscription);
                if (dtSubscription.Rows.Count > 0)
                {
                    if (PMethod == General.PaymentMethod.CC.GetEnumDescription())
                    {
                        dvCheck.Visible = false;
                        dvCard.Visible = true;
                        lstUnitSubscriptionCard.DataSource = dtSubscription;
                        lstUnitSubscriptionCard.DataBind();
                    }
                    else
                    {
                        dvCheck.Visible = true;
                        dvCard.Visible = false;
                        lstUnitSubscription.DataSource = dtSubscription;
                        lstUnitSubscription.DataBind();
                    }
                }
            }
        }

        protected void Page_PreRender(object sender, System.EventArgs e)
        {
            lnkPaidUnpaid.Attributes.Add("onclick", "javascript:return checkPaid('Are you sure want to mark subscription as Paid?','Please select atleast one record')");
        }

        private void FillStatusDropdown()
        {
            var values = DurationExtensions.GetValues<General.UnitSubscriptionStatus>();
            List<string> data = new List<string>();
            foreach (var item in values)
            {
                General.UnitSubscriptionStatus p = (General.UnitSubscriptionStatus)item;
                data.Add(p.GetEnumDescription());
            }
            drpStatus.DataSource = data;
            drpStatus.DataBind();
            drpStatus.Items.Insert(0, new ListItem("Select", "0"));
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
            drpPaymentMethod.DataSource = data;
            drpPaymentMethod.DataBind();
            drpPaymentMethod.Items.Insert(0, new ListItem("Select", "0"));
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if (Session["SubscriptionSearch"] != null)
            {
                Session["SubscriptionSearch"] = null;
                Session.Remove("SubscriptionSearch");
            }

            SubscriptionSearch objSubscriptionSearch = new SubscriptionSearch();

            objSubscriptionSearch.ClientName = txtClient.Text.Trim();
            if (!string.IsNullOrEmpty(txtStart.Value.Trim()))
                objSubscriptionSearch.StartDate = Convert.ToDateTime(txtStart.Value.Trim());
            if (!string.IsNullOrEmpty(txtEnd.Value.Trim()))
                objSubscriptionSearch.EndDate = Convert.ToDateTime(txtEnd.Value.Trim());
            objSubscriptionSearch.Status = drpStatus.SelectedValue.ToString();
            objSubscriptionSearch.PaymentMethod = drpPaymentMethod.SelectedValue.ToString();

            Session["SubscriptionSearch"] = objSubscriptionSearch;

            string Param = string.Empty;
            if (!string.IsNullOrEmpty(txtClient.Text.Trim()))
                Param = "?CName=" + txtClient.Text.Trim();
            if (!string.IsNullOrEmpty(txtStart.Value.Trim()))
            {
                //var Date = Convert.ToDateTime(txtStart.Value.Trim());
                //var temp = Date.AddHours(7);

               // var test = Date.ToUniversalTime();
                //DateTime date1 = new DateTime(2006, 3, 21, 2, 0, 0);

                //Console.WriteLine(date1.ToUniversalTime());
                //Console.WriteLine(TimeZoneInfo.ConvertTimeToUtc(date1));

                //TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
                //Console.WriteLine(TimeZoneInfo.ConvertTimeToUtc(date1, tz));

                if (!string.IsNullOrEmpty(Param))
                    Param = Param + "&SDate=" + txtStart.Value.Trim();
                else
                    Param = "?SDate=" + txtStart.Value.Trim();
            }
            if (!string.IsNullOrEmpty(txtEnd.Value.Trim()))
            {
                if (!string.IsNullOrEmpty(Param))
                    Param = Param + "&EDate=" + txtEnd.Value.Trim();
                else
                    Param = "?EDate=" + txtEnd.Value.Trim();
            }

            if (!string.IsNullOrEmpty(Param))
                Param = Param + "&Status=" + drpStatus.SelectedValue.ToString();
            else
                Param = "?Status=" + drpStatus.SelectedValue.ToString();

            if (!string.IsNullOrEmpty(Param))
                Param = Param + "&PMethod=" + drpPaymentMethod.SelectedValue.ToString();
            else
                Param = "?PMethod=" + drpPaymentMethod.SelectedValue.ToString();

            Response.Redirect(Application["SiteAddress"] + "admin/ClientUnitSubscription_List.aspx" + Param);
        }

        protected void lnkPaidUnpaid_Click(object sender, EventArgs e)
        {
           
            dvMessage.InnerHtml = "";
            dvMessage.Visible = false;

            LoginModel objLoginModel = new LoginModel();
            objLoginModel = Session["LoginSession"] as LoginModel;

            int UserId = objLoginModel.Id;
            int RoleId = objLoginModel.RoleId;

            for (int i = 0; i <= lstUnitSubscription.Items.Count - 1; i++)
            {
                HtmlInputCheckBox chkUsers = (HtmlInputCheckBox)lstUnitSubscription.Items[i].FindControl("chkcheck");
                if (chkUsers.Checked)
                {
                    HiddenField hdnId = (HiddenField)lstUnitSubscription.Items[i].FindControl("hdnId");
                    HiddenField hdnUnitName = (HiddenField)lstUnitSubscription.Items[i].FindControl("hdnUnitName");
                    HiddenField ClientId = (HiddenField)lstUnitSubscription.Items[i].FindControl("ClientId");
                    HiddenField DeviceToken = (HiddenField)lstUnitSubscription.Items[i].FindControl("DeviceToken");
                    HiddenField DeviceType = (HiddenField)lstUnitSubscription.Items[i].FindControl("DeviceType");
                    HiddenField PaymentDueDate = (HiddenField)lstUnitSubscription.Items[i].FindControl("PaymentDueDate");

                    TextBox txtPONumber = (TextBox)lstUnitSubscription.Items[i].FindControl("txtPONumber");
                    TextBox txtCheckNumber = (TextBox)lstUnitSubscription.Items[i].FindControl("txtCheckNumber");

                    //Code Added on 07-07-2017
                    if (txtCheckNumber.Text.Trim() == "")
                    {
                        dvMessage.InnerHtml = "<strong>Please insert check number!</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-error");
                        dvMessage.Visible = true;
                       // BindSubscriptions();
                        return;
                    }

                    if (!string.IsNullOrEmpty(hdnId.Value))
                    {
                        BizObjects.ClientUnitSubscription objClientUnitSubscription = new BizObjects.ClientUnitSubscription();
                        objClientUnitSubscriptionService = ServiceFactory.ClientUnitSubscriptionService;
                        objClientUnitSubscription.Id = Convert.ToInt32(hdnId.Value);
                        objClientUnitSubscription.CardId = 0;
                        if (drpPaymentMethod.SelectedValue.ToString() == General.PaymentMethod.PO.GetEnumDescription())
                        {
                            objClientUnitSubscription.PONumber = txtPONumber.Text.Trim();
                            objClientUnitSubscription.CheckNumbers = txtCheckNumber.Text.Trim();
                        }
                        if (drpPaymentMethod.SelectedValue.ToString() == General.PaymentMethod.Check.GetEnumDescription())
                        {
                            objClientUnitSubscription.CheckNumbers = txtCheckNumber.Text.Trim();
                        }
                        //Code Added on 07-07-2017
                        if (drpPaymentMethod.SelectedValue.ToString() == "0" && !string.IsNullOrEmpty(txtCheckNumber.Text))
                        {
                            objClientUnitSubscription.CheckNumbers = txtCheckNumber.Text.Trim();
                        }
                        //Code end
                        objClientUnitSubscription.FrontImage = string.Empty;
                        objClientUnitSubscription.BackImage = string.Empty;
                        objClientUnitSubscription.AccountingNotes = string.Empty;
                        objClientUnitSubscription.Amount = 0;
                        objClientUnitSubscription.Status = General.UnitSubscriptionStatus.Paid.GetEnumDescription();
                        objClientUnitSubscription.AddedBy = UserId;
                        objClientUnitSubscription.AddedByType = RoleId;
                        objClientUnitSubscription.AddedDate = DateTime.UtcNow;

                        var BillingId = objClientUnitSubscriptionService.UpdateClientUnitSubscriptionService(ref objClientUnitSubscription, "", "");


                        //pp pending work
                        //DataTable dtUnit = new DataTable();
                        //objClientUnitService.GetClientUnitById(int.Parse(row["UnitId"].ToString()), ref dtUnit);
                        //if (dtUnit.Rows.Count > 0)
                        //{
                        //    string StripeCardId = dtUnit.Rows[0]["StripeCardId"].ToString();
                        //    int AddressId = Convert.ToInt32(dtUnit.Rows[0]["AddressId"].ToString());

                        //    BizObjects.Orders objOrders = new BizObjects.Orders();
                        //    IOrderService objOrderService = ServiceFactory.OrderService;
                        //    int OrderId = 0;
                        //    objOrders.OrderType = "Charge";
                        //    objOrders.ClientId = int.Parse(row["ClientId"].ToString());
                        //    objOrders.OrderAmount = decimal.Parse(row["PurchasedAmount"].ToString()); ;
                        //    objOrders.ChargeBy = General.PaymentMethod.CC.GetEnumDescription();
                        //    objOrders.AddedBy = 1;
                        //    objOrders.AddedByType = 1;
                        //    objOrders.AddedDate = DateTime.UtcNow;
                        //    OrderId = objOrderService.AddClientUnitOrderForSchedular(ref objOrders, StripeCardId, AddressId);
                        //}


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
                        objUserNotification.UserId = int.Parse(ClientId.Value);
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

                        objUserNotificationService.GetBadgeCount(int.Parse(ClientId.Value), General.UserRoles.Client.GetEnumValue(), ref dtBadgeCount);
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
            }
            dvMessage.InnerHtml = "<strong>Subscription Paid Successfully.</strong>";
            dvMessage.Attributes.Add("class", "alert alert-success");
            dvMessage.Visible = true;
            BindSubscriptions();
        }

        //protected void SortByServiceCase_Click(object sender, EventArgs e)
        //{

        //}

       

        //protected SortDirection ListViewSortDirection
        //{
        //    get
        //    {
        //        if (ViewState["sortDirection"] == null)
        //            ViewState["sortDirection"] = SortDirection.Ascending;
        //        return (SortDirection)ViewState["sortDirection"];
        //    }
        //    set { ViewState["sortDirection"] = value; }
           
        //}

        //protected string ListViewSortExpression
        //{
        //    get
        //    {
        //        if (ViewState["SortExpression"] == null)
        //            ViewState["SortExpression"] = "DueDate";
        //        //if (ViewState["SortExpression"] != null)
        //        //    if (ViewState["SortExpression"] == "")
        //        //        ViewState["SortExpression"] = "DueDate";
        //        return (string)ViewState["SortExpression"];
        //    }
        //    set { ViewState["SortExpression"] = value; }
        //}



        //protected void lstUnitSubscriptionCard_Sorting(object sender, ListViewSortEventArgs e)
        //{
        //    LinkButton lb = lstUnitSubscriptionCard.FindControl(e.SortExpression) as LinkButton;
        //    HtmlTableCell th = lb.Parent as HtmlTableCell;
        //    HtmlTableRow tr = th.Parent as HtmlTableRow;
        //    List<HtmlTableCell> ths = new List<HtmlTableCell>();

        //    if (tr != null)
        //    {
        //        foreach (HtmlTableCell item in tr.Controls)
        //        {
        //            try
        //            {
        //                if (item.ID.Contains("th"))
        //                {
        //                    item.Attributes["class"] = "sorting";
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //            }
        //        }
        //    }


        //    ListViewSortExpression = e.SortExpression;
        //    if (ListViewSortDirection == SortDirection.Ascending)
        //    {
        //        ListViewSortDirection = SortDirection.Descending;
        //        th.Attributes["class"] = "sorting_desc";
        //    }
        //    else
        //    {
        //        ListViewSortDirection = SortDirection.Ascending;
        //        th.Attributes["class"] = "sorting_asc";
        //    }
        //}

        //protected void SortByServiceCase_Click2(object sender, EventArgs e)
        //{

        //}

        //protected SortDirection ListViewSortDirectionGrid1
        //{
        //    get
        //    {
        //        if (ViewState["sortDirection"] == null)
        //            ViewState["sortDirection"] = SortDirection.Ascending;
        //        return (SortDirection)ViewState["sortDirection"];
        //    }
        //    set { ViewState["sortDirection"] = value; }
        //}

        //protected string ListViewSortExpressionGrid1
        //{
        //    get
        //    {
        //        if (ViewState["SortExpression"] == null)
        //            ViewState["SortExpression"] = "DueDate";
        //        //if (ViewState["SortExpression"] != null)
        //        //    if (ViewState["SortExpression"] == "")
        //        //        ViewState["SortExpression"] = "DueDate";
        //        return (string)ViewState["SortExpression"];
        //    }
        //    set { ViewState["SortExpression"] = value; }
        //}


        //protected void lstUnitSubscription_Sorting(object sender, ListViewSortEventArgs e)
        //{
        //    LinkButton lb = lstUnitSubscription.FindControl(e.SortExpression) as LinkButton;
        //    HtmlTableCell th = lb.Parent as HtmlTableCell;
        //    HtmlTableRow tr = th.Parent as HtmlTableRow;
        //    List<HtmlTableCell> ths = new List<HtmlTableCell>();

        //    if (tr != null)
        //    {
        //        foreach (HtmlTableCell item in tr.Controls)
        //        {
        //            try
        //            {
        //                if (item.ID.Contains("th"))
        //                {
        //                    item.Attributes["class"] = "sorting";
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //            }
        //        }
        //    }


        //    ListViewSortExpression = e.SortExpression;
        //    if (ListViewSortDirection == SortDirection.Ascending)
        //    {
        //        ListViewSortDirection = SortDirection.Descending;
        //        th.Attributes["class"] = "sorting_desc";
        //    }
        //    else
        //    {
        //        ListViewSortDirection = SortDirection.Ascending;
        //        th.Attributes["class"] = "sorting_asc";
        //    }
        //}

    }
}
