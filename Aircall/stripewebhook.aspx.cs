using Aircall.Common;
using BizObjects;
using Services;
using Stripe;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Aircall
{
    public partial class stripewebhook : System.Web.UI.Page
    {
        IBillingHistoryService objBillingHistoryService = ServiceFactory.BillingHistoryService;

        protected void Page_Load(object sender, EventArgs e)
        {
            var json = "";
            IClientService objClientService = ServiceFactory.ClientService;
            using (var inputStream = new System.IO.StreamReader(Request.InputStream))
            {
                json = inputStream.ReadToEnd();
                //json = @"{
                //          ""id"": ""evt_19Q94KCEvTkHNrWheMtNzceC"",
                //          ""object"": ""event"",
                //          ""api_version"": ""2016-03-07"",
                //          ""created"": 1481607796,
                //          ""data"": {
                //                            ""object"": {
                //                                ""id"": ""in_19Q94KCEvTkHNrWhLdnkaelX"",
                //              ""object"": ""invoice"",
                //              ""amount_due"": 2999,
                //              ""application_fee"": null,
                //              ""attempt_count"": 1,
                //              ""attempted"": true,
                //              ""charge"": ""ch_19Q94KCEvTkHNrWh4tClGFLH"",
                //              ""closed"": true,
                //              ""currency"": ""usd"",
                //              ""customer"": ""cus_9BXqUVFGk85VQR"",
                //              ""date"": 1481607796,
                //              ""description"": null,
                //              ""discount"": null,
                //              ""ending_balance"": 0,
                //              ""forgiven"": false,
                //              ""lines"": {
                //                                    ""object"": ""list"",
                //                ""data"": [
                //                  {
                //                    ""id"": ""sub_9jcI2KgSOAeoZK"",
                //                    ""object"": ""line_item"",
                //                    ""amount"": 2999,
                //                    ""currency"": ""usd"",
                //                    ""description"": null,
                //                    ""discountable"": true,
                //                    ""livemode"": false,
                //                    ""metadata"": {},
                //                    ""period"": {
                //                      ""start"": 1481607796,
                //                      ""end"": 1484286196
                //                    },
                //                    ""plan"": {
                //                      ""id"": ""residential-plan-29.99-36"",
                //                      ""object"": ""plan"",
                //                      ""amount"": 2999,
                //                      ""created"": 1477471379,
                //                      ""currency"": ""usd"",
                //                      ""interval"": ""month"",
                //                      ""interval_count"": 1,
                //                      ""livemode"": false,
                //                      ""metadata"": {},
                //                      ""name"": ""Residential Plan"",
                //                      ""statement_descriptor"": null,
                //                      ""trial_period_days"": null
                //                    },
                //                    ""proration"": false,
                //                    ""quantity"": 1,
                //                    ""subscription"": null,
                //                    ""type"": ""subscription""
                //                  }
                //                ],
                //                ""has_more"": false,
                //                ""total_count"": 1,
                //                ""url"": ""/v1/invoices/in_19Q94KCEvTkHNrWhLdnkaelX/lines""
                //              },
                //              ""livemode"": false,
                //              ""metadata"": {},
                //              ""next_payment_attempt"": null,
                //              ""paid"": true,
                //              ""period_end"": 1481607796,
                //              ""period_start"": 1479790816,
                //              ""receipt_number"": null,
                //              ""starting_balance"": 0,
                //              ""statement_descriptor"": null,
                //              ""subscription"": ""sub_9jcI2KgSOAeoZK"",
                //              ""subtotal"": 2999,
                //              ""tax"": null,
                //              ""tax_percent"": null,
                //              ""total"": 2999,
                //              ""webhooks_delivered_at"": null
                //            }
                //          },
                //          ""livemode"": false,
                //          ""pending_webhooks"": 1,
                //          ""request"": ""req_9jcILJoroVXmTv"",
                //          ""type"": ""invoice.payment_succeeded""
                //        }";
                //objClient.insertstriptlog(json);
                StripeEvent stripeEvent = null;
                try
                {
                    // as in header, you need https://github.com/jaymedavis/stripe.net
                    // it's a great library that should have been offered by Stripe directly
                    stripeEvent = StripeEventUtility.ParseEvent(json);

                    //Response.Write("ok");
                    //return;
                }
                catch (Exception ex)
                {

                }
                if (stripeEvent != null)
                {
                    objClientService = ServiceFactory.ClientService;

                    BizObjects.UserNotification objUserNotification = new BizObjects.UserNotification();
                    string message = string.Empty;
                    //bh.
                    switch (stripeEvent.Type)
                    {
                        case "invoice.payment_succeeded":
                            //stripeEvent.
                            {
                                var cust = stripeEvent.Data.Object.customer.Value;
                                var subscription = stripeEvent.Data.Object.subscription.Value;
                                var charge = stripeEvent.Data.Object.charge.Value;
                                var invId = stripeEvent.Data.Object.id.Value;

                                var total = decimal.Parse(stripeEvent.Data.Object.total.Value.ToString()) / 100;

                                BillingHistory bh = new BillingHistory();

                                DataTable dtClient = new DataTable();
                                StripeInvoice inv = new StripeInvoiceService().Get(invId);
                                StripeCharge ch = new StripeChargeService().Get(charge);
                                StripeSubscription sub = new StripeSubscriptionService().Get(cust, subscription);

                                IClientUnitService objClientUnit = ServiceFactory.ClientUnitService;
                                objClientUnit.GetClientUnitBySubscriptionId(subscription, ref dtClient);


                                DateTime StripeNextPaymentDate = DateTime.Parse(dtClient.Rows[0]["StripeNextPaymentDate"].ToString());

                                int ClientId = int.Parse(dtClient.Rows[0]["ClientId"].ToString());
                                float SalesCommission = float.Parse(dtClient.Rows[0]["SalesCommission"].ToString());
                                bh.ClientId = ClientId;
                                bh.AddedBy = ClientId;
                                bh.AddedDate = DateTime.Now;
                                //bh.UnitId = int.Parse(dtClient.Rows[0]["Id"].ToString());
                                bh.BillingAddress = dtClient.Rows[0]["Address"].ToString();
                                bh.BillingCity = int.Parse(dtClient.Rows[0]["CityId"].ToString());
                                bh.BillingMobileNumber = dtClient.Rows[0]["MobileNumber"].ToString();
                                bh.BillingPhoneNumber = dtClient.Rows[0]["PhoneNumber"].ToString();
                                bh.BillingState = int.Parse(dtClient.Rows[0]["StateId"].ToString());
                                bh.BillingType = General.BillingTypes.Recurringpurchase.GetEnumDescription();
                                bh.BillingZipcode = dtClient.Rows[0]["ZipCode"].ToString();
                                bh.IsPaid = true;
                                bh.OriginalAmount = inv.AmountDue / 100m;
                                bh.PackageName = dtClient.Rows[0]["PlanName"].ToString();
                                bh.PartnerSalesCommisionAmount = 0m;
                                bh.PurchasedAmount = inv.AmountDue / 100m;
                                bh.ServiceCaseNumber = "";
                                bh.TransactionDate = inv.Date.Value;
                                //if (int.Parse(dtClient.Rows[0]["TotalBill"].ToString()) > 1)
                                if (StripeNextPaymentDate < inv.PeriodEnd)
                                {
                                    bh.TransactionId = invId;
                                }
                                else
                                {
                                    bh.TransactionId = subscription;
                                }

                                bh.IsDeleted = false;
                                bh.IsSpecialOffer = false;
                                bh.OrderId = int.Parse(dtClient.Rows[0]["OrderId"].ToString());
                                bh.failcode = "";
                                bh.faildesc = "Payment Success!";
                                //bh.StripeNextPaymentDate = inv.PeriodEnd;
                                if (SalesCommission > 0)
                                {
                                    bh.PartnerSalesCommisionAmount = (bh.PurchasedAmount * (decimal.Parse(SalesCommission.ToString()) / 100));
                                }
                                int rtn = objBillingHistoryService.AddBillingHistory(ref bh);
                                if (int.Parse(dtClient.Rows[0]["LastBillingID"].ToString()) < rtn)
                                {
                                    IClientUnitServiceCountService objClientUnitServiceCountService = ServiceFactory.ClientUnitServiceCountService;
                                    ClientUnitServiceCount objUnitServiceCount = new ClientUnitServiceCount();
                                    objUnitServiceCount.ClientId = bh.ClientId;
                                    //objUnitServiceCount.UnitId = bh.UnitId;
                                    objUnitServiceCount.TotalDonePlanService = 0;
                                    objUnitServiceCount.TotalRequestService = 0;
                                    objUnitServiceCount.TotalDoneRequestService = 0;
                                    objUnitServiceCount.TotalBillsGenerated = 1;
                                    objUnitServiceCount.StripeUnitSubscriptionCount = 1;
                                    objUnitServiceCount.UpdatedBy = 1;
                                    objUnitServiceCount.UpdatedByType = 1;
                                    objUnitServiceCount.UpdatedDate = DateTime.UtcNow;

                                    objClientUnitServiceCountService.UpdateClientUnitServiceCount(ref objUnitServiceCount);
                                }                                
                            }
                            // do work
                            break;
                        case "invoice.payment_failed":
                            // do work

                            {
                                var cust = stripeEvent.Data.Object.customer.Value;
                                var subscription = stripeEvent.Data.Object.subscription.Value;
                                var charge = stripeEvent.Data.Object.charge.Value;
                                var invId = stripeEvent.Data.Object.id.Value;

                                var total = decimal.Parse(stripeEvent.Data.Object.total.Value.ToString()) / 100;

                                BillingHistory bh = new BillingHistory();

                                DataTable dtClient = new DataTable();
                                StripeInvoice inv = new StripeInvoiceService().Get(invId);
                                StripeCharge ch = new StripeChargeService().Get(charge);


                                IClientUnitService objClientUnit = ServiceFactory.ClientUnitService;
                                objClientUnit.GetClientUnitBySubscriptionId(subscription, ref dtClient);

                                int ClientId = int.Parse(dtClient.Rows[0]["ClientId"].ToString());

                                bh.ClientId = ClientId;
                                bh.AddedBy = ClientId;
                                bh.AddedDate = DateTime.Now;
                                //bh.UnitId = int.Parse(dtClient.Rows[0]["Id"].ToString());
                                bh.BillingAddress = dtClient.Rows[0]["Address"].ToString();
                                bh.BillingCity = int.Parse(dtClient.Rows[0]["CityId"].ToString());
                                bh.BillingMobileNumber = dtClient.Rows[0]["MobileNumber"].ToString();
                                bh.BillingPhoneNumber = dtClient.Rows[0]["PhoneNumber"].ToString();
                                bh.BillingState = int.Parse(dtClient.Rows[0]["StateId"].ToString());
                                bh.BillingType = General.BillingTypes.Recurringpurchase.GetEnumDescription();
                                bh.BillingZipcode = dtClient.Rows[0]["ZipCode"].ToString();
                                bh.IsPaid = false;
                                bh.OriginalAmount = inv.AmountDue / 100m;
                                bh.PackageName = dtClient.Rows[0]["PlanName"].ToString();
                                bh.PartnerSalesCommisionAmount = 0m;
                                bh.PurchasedAmount = inv.AmountDue / 100m;
                                bh.ServiceCaseNumber = "";
                                bh.TransactionDate = inv.Date.Value;
                                bh.TransactionId = invId;
                                bh.IsDeleted = false;
                                bh.failcode = ch.FailureCode;
                                bh.faildesc = ch.FailureMessage;
                                bh.OrderId = int.Parse(dtClient.Rows[0]["OrderId"].ToString());
                                //bh.StripeNextPaymentDate = inv.PeriodEnd;

                                var BillingId = objBillingHistoryService.AddBillingHistory(ref bh);


                                DataTable ClientDetail = new DataTable();
                                objClientService.GetClientById(ClientId, ref ClientDetail);
                                long NotificationId = 0;
                                int BadgeCount = 0;

                                if (ClientDetail.Rows.Count > 0)
                                {
                                    IUserNotificationService objUserNotificationService;

                                    message = General.GetNotificationMessage("PaymentFailedForSubscriptionInvoice");

                                    message = message.Replace("{{UnitName}}", dtClient.Rows[0]["UnitName"].ToString());
                                    message = message.Replace("{{MonthYear}}", inv.Date.Value.ToString("MMMM yyyy"));
                                    message = message.Replace("{{Reason}}", ch.FailureMessage);

                                    objUserNotificationService = ServiceFactory.UserNotificationService;
                                    objUserNotification.UserId = ClientId;
                                    objUserNotification.UserTypeId = General.UserRoles.Client.GetEnumValue();
                                    objUserNotification.Message = message;
                                    objUserNotification.Status = General.NotificationStatus.UnRead.GetEnumDescription();

                                    if (bh.failcode != "expired_card")
                                    {
                                        objUserNotification.MessageType = General.NotificationType.SubscriptionInvoicePaymentFailed.GetEnumDescription();
                                        objUserNotification.CommonId = BillingId;
                                    }
                                    else
                                    {
                                        IClientPaymentMethodService objCPMS = ServiceFactory.ClientPaymentMethodService;
                                        DataTable dtCPM = new DataTable();
                                        objCPMS.GetClientPaymentMethodByOrderId(bh.OrderId, ref dtCPM);

                                        objUserNotification.CommonId = int.Parse(dtCPM.Rows[0]["Id"].ToString());
                                        objUserNotification.MessageType = General.NotificationType.CreditCardExpiration.GetEnumDescription();
                                    }
                                    objUserNotification.AddedDate = DateTime.UtcNow;
                                    NotificationId = objUserNotificationService.AddUserNotification(ref objUserNotification);

                                    DataTable dtBadgeCount = new DataTable();
                                    dtBadgeCount.Clear();

                                    objUserNotificationService.GetBadgeCount(ClientId, General.UserRoles.Client.GetEnumValue(), ref dtBadgeCount);
                                    BadgeCount = dtBadgeCount.Rows.Count;

                                    Notifications objNotifications = new Notifications { NId = NotificationId, NType = General.NotificationType.SubscriptionInvoicePaymentFailed.GetEnumValue(), CommonId = BillingId };
                                    List<NotificationModel> notify = new List<NotificationModel>();
                                    notify.Add(new NotificationModel { Key = "NId", Value = new object[] { objNotifications.NId } });
                                    notify.Add(new NotificationModel { Key = "NType", Value = new object[] { objNotifications.NType } });
                                    notify.Add(new NotificationModel { Key = "CommonId", Value = new object[] { objNotifications.CommonId } });

                                    if (!string.IsNullOrEmpty(ClientDetail.Rows[0]["DeviceType"].ToString()) &&
                                        !string.IsNullOrEmpty(ClientDetail.Rows[0]["DeviceToken"].ToString()) &&
                                        ClientDetail.Rows[0]["DeviceToken"].ToString().ToLower() != "no device token")
                                    {
                                        if (ClientDetail.Rows[0]["DeviceType"].ToString().ToLower() == "android")
                                        {
                                            string CustomData = "&data.NId=" + objNotifications.NId + "&data.NType=" + objNotifications.NType + "&data.CommonId=" + objNotifications.CommonId;
                                            SendNotifications.SendAndroidNotification(ClientDetail.Rows[0]["DeviceToken"].ToString(), message, CustomData, "client");
                                        }
                                        else if (ClientDetail.Rows[0]["DeviceType"].ToString().ToLower() == "iphone")
                                        {
                                            SendNotifications.SendIphoneNotification(BadgeCount, ClientDetail.Rows[0]["DeviceToken"].ToString(), message, notify, "client");
                                        }
                                    }
                                }

                            }
                            break;
                        case "customer.subscription.updated":
                        case "customer.subscription.deleted":
                        case "customer.subscription.created":
                            // do work
                            break;
                    }
                }

            }
            Response.Output.Flush();
            Response.Output.Write("OK");
        }
    }
}