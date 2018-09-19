using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Stripe;
using Services;
using System.Data;
using Aircall.Common;

namespace Aircall.admin
{
    public partial class RenewCancel_UnitSubscription : System.Web.UI.Page
    {
        IClientUnitService objClientUnitService;
        IClientUnitInvoiceService objClientUnitInvoiceService;
        IServicesService objServicesService;
        IStripeErrorLogService objStripeErrorLogService;
        IUserNotificationService objUserNotificationService;
        IClientUnitSubscriptionService objClientUnitSubscriptionService;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["UnitId"]))
                {
                    int UnitId = Convert.ToInt32(Request.QueryString["UnitId"].ToString());
                    objClientUnitService = ServiceFactory.ClientUnitService;
                    DataTable dtUnit = new DataTable();
                    objClientUnitService.GetClientUnitById(UnitId, ref dtUnit);
                    if (dtUnit.Rows.Count > 0)
                    {
                        if (Convert.ToBoolean(dtUnit.Rows[0]["AutoRenewal"].ToString()))
                            btnRenew.Visible = false;
                    }
                }
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(Request.QueryString["UnitId"]) &&
                        Session["LoginSession"] != null)
                {
                    int UnitId = Convert.ToInt32(Request.QueryString["UnitId"].ToString());
                    objServicesService = ServiceFactory.ServicesService;
                    DataTable dtService = new DataTable();
                    objServicesService.CheckServiceScheduleByUnitAndStatus(UnitId, General.ServiceTypes.Scheduled.GetEnumDescription(), ref dtService);
                    if (dtService.Rows.Count > 0)
                    {
                        dvMessage.InnerHtml = "<strong>You can not Cancel Subscription for this Unit because of an active scheduled service.</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-error");
                        dvMessage.Visible = true;
                        return;
                    }

                    LoginModel objLoginModel = new LoginModel();
                    objLoginModel = Session["LoginSession"] as LoginModel;

                    int UserId = objLoginModel.Id;
                    int RoleId = objLoginModel.RoleId;

                    objClientUnitService = ServiceFactory.ClientUnitService;
                    DataTable dtUnit = new DataTable();
                    objClientUnitService.GetClientUnitById(UnitId, ref dtUnit);
                    if (dtUnit.Rows.Count > 0)
                    {
                        int ClientId = Convert.ToInt32(dtUnit.Rows[0]["ClientId"].ToString());
                        string StripeCustomerId = dtUnit.Rows[0]["StripeCustomerId"].ToString();
                        string StripeSubscriptionId = dtUnit.Rows[0]["StripeSubscriptionId"].ToString();
                        bool IsSpecialApplied = Convert.ToBoolean(dtUnit.Rows[0]["IsSpecialApplied"].ToString());
                        //if (!string.IsNullOrEmpty(StripeSubscriptionId) && IsSpecialApplied==false)
                        //{
                            try
                            {
                                //var SubscriptionService = new StripeSubscriptionService();
                                //StripeSubscription UnitSubscription = SubscriptionService.Cancel(StripeCustomerId, StripeSubscriptionId, true);
                                objClientUnitService.CancelUnitSubscription(UnitId, UserId, RoleId, DateTime.UtcNow);

                            }
                            catch (Exception ex)
                            {
                                //BizObjects.StripeErrorLog objStripeErrorLog = new BizObjects.StripeErrorLog();
                                //objStripeErrorLogService = ServiceFactory.StripeErrorLogService;
                                //objStripeErrorLog.ChargeId = stex.StripeError.ChargeId;
                                //objStripeErrorLog.Code = stex.StripeError.Code;
                                //objStripeErrorLog.DeclineCode = stex.StripeError.DeclineCode;
                                //objStripeErrorLog.ErrorType = stex.StripeError.ErrorType;
                                //objStripeErrorLog.Error = stex.StripeError.Error;
                                //objStripeErrorLog.ErrorSubscription = stex.StripeError.ErrorSubscription;
                                //objStripeErrorLog.Message = stex.StripeError.Message;
                                //objStripeErrorLog.Parameter = stex.StripeError.Parameter;
                                //objStripeErrorLog.Userid = ClientId;
                                //objStripeErrorLog.UnitId = UnitId;
                                //objStripeErrorLog.DateAdded = DateTime.UtcNow;

                                //objStripeErrorLogService.AddStripeErrorLog(ref objStripeErrorLog);

                                dvMessage.InnerHtml = ex.Message.ToString();
                                dvMessage.Attributes.Add("class", "error");
                                dvMessage.Visible = true;
                                return;
                            }
                        //}
                        //else
                        //    objClientUnitService.CancelUnitSubscription(UnitId, UserId, RoleId, DateTime.UtcNow);

                        BizObjects.ClientUnitInvoice objClientUnitInvoice = new BizObjects.ClientUnitInvoice();
                        objClientUnitInvoice.ClientId = Convert.ToInt32(dtUnit.Rows[0]["ClientId"].ToString());
                        objClientUnitInvoice.UnitId = UnitId;
                        objClientUnitInvoice.Amount = 0;
                        objClientUnitInvoice.RenewCancelReason = txtCancelReason.Text;
                        objClientUnitInvoice.AddedBy = objLoginModel.Id;
                        objClientUnitInvoice.AddedByType = objLoginModel.RoleId;
                        objClientUnitInvoice.AddedDate = DateTime.UtcNow;

                        objClientUnitInvoiceService = ServiceFactory.ClientUnitInvoiceService;
                        objClientUnitInvoiceService.AddClientUnitInvoice(ref objClientUnitInvoice);

                        //Send Notification

                        long NotificationId = 0;
                        int BadgeCount = 0;
                        DataTable dtBadgeCount = new DataTable();
                        BizObjects.UserNotification objUserNotification = new BizObjects.UserNotification();
                        string message = General.GetNotificationMessage("UnitPlanCancelSendToClient"); //"Unit " + dtUnit.Rows[0]["UnitName"] + " plan is successfully cancelled.";
                        message = message.Replace("{{UnitName}}", dtUnit.Rows[0]["UnitName"].ToString());
                        objUserNotificationService = ServiceFactory.UserNotificationService;
                        objUserNotification.UserId = ClientId;
                        objUserNotification.UserTypeId = General.UserRoles.Client.GetEnumValue();
                        objUserNotification.Message = message;
                        objUserNotification.Status = General.NotificationStatus.UnRead.GetEnumDescription();
                        objUserNotification.MessageType = General.NotificationType.UnitPlanCancelled.GetEnumDescription();
                        objUserNotification.AddedDate = DateTime.UtcNow;

                        NotificationId = objUserNotificationService.AddUserNotification(ref objUserNotification);

                        dtBadgeCount.Clear();
                        objUserNotificationService.GetBadgeCount(ClientId, General.UserRoles.Client.GetEnumValue(), ref dtBadgeCount);
                        BadgeCount = dtBadgeCount.Rows.Count;

                        Notifications objNotifications = new Notifications { NId = NotificationId, NType = General.NotificationType.UnitPlanCancelled.GetEnumValue() };
                        List<NotificationModel> notify = new List<NotificationModel>();
                        notify.Add(new NotificationModel { Key = "NId", Value = new object[] { objNotifications.NId } });
                        notify.Add(new NotificationModel { Key = "NType", Value = new object[] { objNotifications.NType } });

                        if (!string.IsNullOrEmpty(dtUnit.Rows[0]["DeviceType"].ToString()) &&
                        !string.IsNullOrEmpty(dtUnit.Rows[0]["DeviceToken"].ToString()) &&
                         dtUnit.Rows[0]["DeviceToken"].ToString().ToLower() != "no device token")
                        {
                            if (dtUnit.Rows[0]["DeviceType"].ToString().ToLower() == "android")
                            {
                                string CustomData = "&data.NId=" + objNotifications.NId + "&data.NType=" + objNotifications.NType + "&data.CommonId=" + objNotifications.CommonId;
                                SendNotifications.SendAndroidNotification(dtUnit.Rows[0]["DeviceToken"].ToString(), message, CustomData, "client");
                            }
                            else if (dtUnit.Rows[0]["DeviceType"].ToString().ToLower() == "iphone")
                            {
                                SendNotifications.SendIphoneNotification(BadgeCount, dtUnit.Rows[0]["DeviceToken"].ToString(), message, notify, "client");
                            }
                        }
                        Session["msg"] = "cancel";
                        Response.Redirect(Application["SiteAddress"] + "admin/RenewCancel_List.aspx");
                    }
                }
            }
            catch (Exception Ex)
            {
                dvMessage.InnerHtml = "<strong>Error!</strong> " + Ex.Message.ToString().Trim();
                dvMessage.Attributes.Add("class", "alert alert-error");
                dvMessage.Visible = true;
            }
        }

        protected void btnRenew_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(Request.QueryString["UnitId"]) &&
                                Session["LoginSession"] != null)
                {
                    LoginModel objLoginModel = new LoginModel();
                    objLoginModel = Session["LoginSession"] as LoginModel;

                    int UnitId = Convert.ToInt32(Request.QueryString["UnitId"].ToString());
                    objClientUnitService = ServiceFactory.ClientUnitService;
                    DataTable dtUnit = new DataTable();
                    objClientUnitService.CheckForRenewUnitSubscription(UnitId, ref dtUnit);
                    if (dtUnit.Rows.Count>0)
                    {
                        int CompletedServiceCount = Convert.ToInt32(dtUnit.Rows[0]["CompletedServiceCount"].ToString());
                        int NumberOfService = Convert.ToInt32(dtUnit.Rows[0]["NumberOfService"].ToString());
                        if (CompletedServiceCount<NumberOfService)
                        {
                            dvMessage.InnerHtml = "<strong>You can not renew plan for this unit because some of the plan service are pending for this unit.</strong>";
                            dvMessage.Attributes.Add("class", "alert alert-error");
                            dvMessage.Visible = true;
                            return;
                        }
                    }

                    dtUnit.Rows.Clear();
                    objClientUnitService.GetClientUnitById(UnitId, ref dtUnit);
                    if (dtUnit.Rows.Count > 0)
                    {
                        int ClientId = Convert.ToInt32(dtUnit.Rows[0]["ClientId"].ToString());
                        int AddressId = Convert.ToInt32(dtUnit.Rows[0]["AddressId"].ToString());
                        string PlanName1 = dtUnit.Rows[0]["PlanName1"].ToString();
                        string StripeCardId = dtUnit.Rows[0]["StripeCardId"].ToString();
                        string StripeCustomerId = dtUnit.Rows[0]["StripeCustomerId"].ToString();
                        bool AutoRenewal = Convert.ToBoolean(dtUnit.Rows[0]["AutoRenewal"].ToString());
                        bool IsSpecialApplied = Convert.ToBoolean(dtUnit.Rows[0]["IsSpecialApplied"].ToString());
                        decimal Amount = Convert.ToDecimal(dtUnit.Rows[0]["PricePerMonth"].ToString());
                        //decimal DiscountPrice = Convert.ToDecimal(dtUnit.Rows[0]["DiscountPrice"].ToString());
                        string CurrentPaymentMethod = dtUnit.Rows[0]["CurrentPaymentMethod"].ToString();
                        int CardId = Convert.ToInt32(dtUnit.Rows[0]["CardId"].ToString());
                        string StripePlanId = dtUnit.Rows[0]["StripePlanId"].ToString();
                        string SubscriptionId = string.Empty;
                        int AddedBy = objLoginModel.Id;
                        int AddedByType = objLoginModel.RoleId;

                        //Insert Unit(Renew)
                        int NewUnitId = objClientUnitService.RenewUnitSubscription(UnitId, AddedBy, AddedByType, DateTime.UtcNow);
                        //DateTime StripeNextPaymentDate = DateTime.UtcNow;
                        //bool SubscriptionCreated=false;
                        //Generate Subscription in stripe
                        if (NewUnitId != 0)
                        {
                            string PaymentStatus = string.Empty;
                            try
                            {
                                //var SubscriptionService = new StripeSubscriptionService();
                                //StripeSubscription objStripeSubscription = SubscriptionService.Create(StripeCustomerId, StripePlanId);
                                //SubscriptionId = objStripeSubscription.Id;
                                if (CurrentPaymentMethod==General.PaymentMethod.CC.GetEnumDescription())
                                {
                                    var StripeResponse = new Aircall.Common.StripeResponse();
                                    string desc = "Charge For " + PlanName1;
                                    StripeResponse = General.StripeCharge(true, "", StripeCustomerId, StripeCardId, Convert.ToInt32(Amount * 100), desc, "");
                                    if (StripeResponse.PaymentStatus != "Failed" && StripeResponse.ex == null)
                                        PaymentStatus = General.UnitPaymentTypes.Received.GetEnumDescription();
                                }
                                else
                                {
                                    PaymentStatus = General.UnitPaymentTypes.Received.GetEnumDescription();
                                    StripeCardId = string.Empty;
                                }

                                //objClientUnitService.SetPaymentStatusByUnitId(NewUnitId, PaymentStatus, SubscriptionId);
                                objClientUnitService.SetPaymentStatusByUnitId(NewUnitId, PaymentStatus, StripeCardId);

                                //Insert into Invoice
                                BizObjects.ClientUnitInvoice objClientUnitInvoice = new BizObjects.ClientUnitInvoice();
                                objClientUnitInvoice.ClientId = Convert.ToInt32(dtUnit.Rows[0]["ClientId"].ToString());
                                objClientUnitInvoice.UnitId = NewUnitId;
                                objClientUnitInvoice.Amount = Amount;
                                objClientUnitInvoice.RenewCancelReason = txtCancelReason.Text;
                                objClientUnitInvoice.AddedBy = objLoginModel.Id;
                                objClientUnitInvoice.AddedByType = objLoginModel.RoleId;
                                objClientUnitInvoice.AddedDate = DateTime.UtcNow;

                                objClientUnitInvoiceService = ServiceFactory.ClientUnitInvoiceService;
                                objClientUnitInvoiceService.AddClientUnitInvoice(ref objClientUnitInvoice);
                                
                                //StripeInvoiceService inv = new StripeInvoiceService();
                                //var inv1 = inv.Upcoming(StripeCustomerId, new StripeUpcomingInvoiceOptions() { SubscriptionId = SubscriptionId });
                                //StripeNextPaymentDate = inv1.PeriodEnd.Date;
                                //SubscriptionCreated = true;

                                //Insert into Billing History
                                BizObjects.Orders objOrders = new BizObjects.Orders();
                                IOrderService objOrderService = ServiceFactory.OrderService;
                                int OrderId = 0;
                                objOrders.OrderType = "Charge";
                                objOrders.ClientId = ClientId;
                                objOrders.OrderAmount = Amount;
                                objOrders.ChargeBy = CurrentPaymentMethod;
                                objOrders.AddedBy = objLoginModel.Id;
                                objOrders.AddedByType = objLoginModel.RoleId;
                                objOrders.AddedDate = DateTime.UtcNow;
                                OrderId = objOrderService.AddClientUnitOrder(ref objOrders, StripeCardId, AddressId);
                                if (OrderId > 0)
                                {
                                    BizObjects.BillingHistory objBillingHistory = new BizObjects.BillingHistory();
                                    IBillingHistoryService objBillingHistoryService = ServiceFactory.BillingHistoryService;
                                    objBillingHistory.ClientId = ClientId;
                                    //objBillingHistory.UnitId = NewUnitId;
                                    objBillingHistory.OrderId = OrderId;
                                    objBillingHistory.PackageName = PlanName1;
                                    objBillingHistory.BillingType = General.BillingTypes.Recurringpurchase.GetEnumDescription();
                                    objBillingHistory.OriginalAmount = Amount;
                                    objBillingHistory.PurchasedAmount = Amount;
                                    objBillingHistory.IsSpecialOffer = IsSpecialApplied;
                                    objBillingHistory.IsPaid = true;
                                    objBillingHistory.TransactionId = "";//SubscriptionId;
                                    objBillingHistory.TransactionDate = DateTime.UtcNow;
                                    objBillingHistory.AddedBy = objLoginModel.Id;
                                    objBillingHistory.AddedDate = DateTime.UtcNow;
                                    //objBillingHistory.StripeNextPaymentDate = StripeNextPaymentDate;

                                    objBillingHistoryService.AddClientUnitBillingHistory(ref objBillingHistory);
                                }

                                //Add Client Unit Subscription Start
                                BizObjects.ClientUnitSubscription objClientUnitSubscription = new BizObjects.ClientUnitSubscription();
                                objClientUnitSubscriptionService = ServiceFactory.ClientUnitSubscriptionService;
                                objClientUnitSubscription.ClientId = ClientId;
                                //objClientUnitSubscription.ClientUnitIds = NewUnitId;
                                objClientUnitSubscription.ClientUnitIds = "";
                                objClientUnitSubscription.OrderId = OrderId;
                                objClientUnitSubscription.PaymentMethod = CurrentPaymentMethod;
                                objClientUnitSubscription.CardId = CardId;
                                objClientUnitSubscription.PONumber = string.Empty;
                                objClientUnitSubscription.CheckNumbers = string.Empty;
                                objClientUnitSubscription.FrontImage = string.Empty;
                                objClientUnitSubscription.BackImage = string.Empty;
                                objClientUnitSubscription.AccountingNotes = string.Empty;
                                objClientUnitSubscription.Amount = Amount;
                                objClientUnitSubscription.AddedBy = objLoginModel.Id;
                                objClientUnitSubscription.AddedByType = objLoginModel.RoleId;
                                // objClientUnitSubscription.AddedDate = DateTime.UtcNow; Code Commented on 19-07-2017
                                objClientUnitSubscription.AddedDate = DateTime.UtcNow.ToLocalTime();
                                objClientUnitSubscriptionService.AddClientUnitSubscriptionService(ref objClientUnitSubscription, false);

                                //Add Client Unit Subscription End

                                //Send Notification

                                long NotificationId = 0;
                                int BadgeCount = 0;
                                DataTable dtBadgeCount = new DataTable();
                                BizObjects.UserNotification objUserNotification = new BizObjects.UserNotification();
                                string message = General.GetNotificationMessage("UnitPlanRenewSendToClient"); //"Plan for Unit " + dtUnit.Rows[0]["UnitName"] + " has been  is successfully renewed.";
                                message = message.Replace("{{UnitName}}", dtUnit.Rows[0]["UnitName"].ToString());
                                objUserNotificationService = ServiceFactory.UserNotificationService;
                                objUserNotification.UserId = ClientId;
                                objUserNotification.UserTypeId = General.UserRoles.Client.GetEnumValue();
                                objUserNotification.Message = message;
                                objUserNotification.Status = General.NotificationStatus.UnRead.GetEnumDescription();
                                objUserNotification.MessageType = General.NotificationType.UnitPlanRenew.GetEnumDescription();
                                objUserNotification.AddedDate = DateTime.UtcNow;

                                NotificationId = objUserNotificationService.AddUserNotification(ref objUserNotification);

                                dtBadgeCount.Clear();
                                objUserNotificationService.GetBadgeCount(ClientId, General.UserRoles.Client.GetEnumValue(), ref dtBadgeCount);
                                BadgeCount = dtBadgeCount.Rows.Count;

                                Notifications objNotifications = new Notifications { NId = NotificationId, NType = General.NotificationType.UnitPlanRenew.GetEnumValue() };
                                List<NotificationModel> notify = new List<NotificationModel>();
                                notify.Add(new NotificationModel { Key = "NId", Value = new object[] { objNotifications.NId } });
                                notify.Add(new NotificationModel { Key = "NType", Value = new object[] { objNotifications.NType } });


                                if (!string.IsNullOrEmpty(dtUnit.Rows[0]["DeviceType"].ToString()) &&
                                !string.IsNullOrEmpty(dtUnit.Rows[0]["DeviceToken"].ToString()) &&
                                 dtUnit.Rows[0]["DeviceToken"].ToString().ToLower() != "no device token")
                                {
                                    if (dtUnit.Rows[0]["DeviceType"].ToString().ToLower() == "android")
                                    {
                                        string CustomData = "&data.NId=" + objNotifications.NId + "&data.NType=" + objNotifications.NType + "&data.CommonId=" + objNotifications.CommonId;
                                        SendNotifications.SendAndroidNotification(dtUnit.Rows[0]["DeviceToken"].ToString(), message, CustomData, "client");
                                    }
                                    else if (dtUnit.Rows[0]["DeviceType"].ToString().ToLower() == "iphone")
                                    {
                                        SendNotifications.SendIphoneNotification(BadgeCount, dtUnit.Rows[0]["DeviceToken"].ToString(), message, notify, "client");
                                    }
                                }
                                Session["msg"] = "renew";
                                Response.Redirect(Application["SiteAddress"] + "admin/RenewCancel_List.aspx");
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
                                objStripeErrorLog.UnitId = UnitId;
                                objStripeErrorLog.DateAdded = DateTime.UtcNow;

                                objStripeErrorLogService.AddStripeErrorLog(ref objStripeErrorLog);

                                dvMessage.InnerHtml = stex.StripeError.Message.ToString();
                                dvMessage.Attributes.Add("class", "error");
                                dvMessage.Visible = true;
                                return;
                            }
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                dvMessage.InnerHtml = Ex.Message.ToString();
                dvMessage.Attributes.Add("class", "alert alert-error");
                dvMessage.Visible = true;
            }
        }
    }
}