using api.ActionFilters;
using api.App_Start;
using api.Models;
using api.Repository;
using api.ViewModel;
using AutoMapper;
using System.IdentityModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Script.Serialization;
using System.Net.Http.Headers;
using System.IO;
using System.Text.RegularExpressions;
using System.Net.Mail;
using Stripe;
using Nito.AspNetBackgroundTasks;
using Newtonsoft.Json;
using System.Threading;
using api.Common;

namespace api.Controllers
{
    [RoutePrefix("v1/profile")]
    public class NewProfileController : BaseClientApiController
    {
        Aircall_DBEntities1 db;

        [AuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("GetExpiredCreditCardById")]
        public async Task<IHttpActionResult> GetExpiredCreditCardById([FromBody]NotificationRequest request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();

            try
            {
                var UserInfo = db.Clients.Where(x => x.Id == request.ClientId && x.IsDeleted == false).FirstOrDefault();
                if (!UserInfo.IsActive)
                {
                    res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                    res.Message = "Your account was deactivated by Admin";
                    res.Data = null;
                }
                else
                {
                    var ccs = db.ClientPaymentMethods.Where(x => x.ClientId == request.ClientId && x.Id == request.CardId).ToList();
                    var data = AutoMapper.Mapper.Map<List<ClientPaymentModel>>(ccs);
                    var notificaiton = db.UserNotifications.Where(x => x.Id == request.NotificationId).FirstOrDefault();
                    if (notificaiton != null)
                    {
                        notificaiton.Status = Utilities.NotificationStatus.Read.GetEnumDescription();
                        notificaiton.UpdatedBy = request.ClientId;
                        notificaiton.UpdatedDate = DateTime.UtcNow;
                        notificaiton.UserTypeId = Utilities.UserRoles.Client.GetEnumValue();
                        db.SaveChanges();
                    }
                    if (data.Count > 0)
                    {
                        res.StatusCode = (int)HttpStatusCode.OK;
                        res.Message = "Record Found";
                        res.Data = data.First();
                    }
                    else
                    {
                        res.StatusCode = HttpStatusCode.NotFound.GetEnumValue();
                        res.Message = "No record found";
                        res.Data = null;
                    }
                }

            }
            catch (Exception ex)
            {
                res.StatusCode = (int)HttpStatusCode.BadRequest;
                res.Message = "Invalid Request";
                res.Data = null;
            }
            if (updatetoken)
            {
                res.Token = accessToken;
            }
            else
            {
                res.Token = "";
            }
            db.Dispose();
            return Ok(res);
        }

        [AuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("GetExpiredPlanUnitById")]
        public async Task<IHttpActionResult> GetExpiredPlanUnitById([FromBody]NotificationRequest request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();

            var PendingProcessUnit = await db.ClientUnits.Where(x => x.ClientId == request.ClientId && x.Id == request.UnitId).ToListAsync();
            List<object> data = new List<object>();
            decimal total = 0m;
            var notificaiton = db.UserNotifications.Where(x => x.Id == request.NotificationId).FirstOrDefault();
            if (notificaiton != null)
            {
                notificaiton.Status = Utilities.NotificationStatus.Read.GetEnumDescription();
                notificaiton.UpdatedBy = request.ClientId;
                notificaiton.UpdatedDate = DateTime.UtcNow;
                notificaiton.UserTypeId = Utilities.UserRoles.Client.GetEnumValue();
                db.SaveChanges();
            }
            foreach (var cunit1 in PendingProcessUnit)
            {
                var desc = db.SubscriptionPlans.First(p => p.Id == cunit1.Id).PlanName;
                var PlanSelectedDisplay = new
                {
                    cunit1.UnitName,
                    PlanName = desc,
                    Description = desc.Substring(0, (desc.Length > 256 ? 256 : desc.Length)),
                    Price = cunit1.PricePerMonth,
                    PlanType = (cunit1.IsSpecialApplied == true ? "Special Offer" : "Recurring"),
                    Id = cunit1.Id
                };
                total = total + cunit1.PricePerMonth.Value;
                data.Add(PlanSelectedDisplay);
            }
            var response = new
            {
                Units = data,
                Total = total,
                Message = (PendingProcessUnit.Count(x => x.IsSpecialApplied == true) == PendingProcessUnit.Count ? "" : "(Recurring Billing occur every month)"),
                ErrMessage = ""
            };
            res.StatusCode = (int)HttpStatusCode.OK;
            res.Message = "Record Found";
            res.Data = response;
            if (updatetoken)
            {
                res.Token = accessToken;
            }
            else
            {
                res.Token = "";
            }
            db.Dispose();
            return Ok(res);
        }

        [AuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("NoShowServiceDetails")]
        public async Task<IHttpActionResult> NoShowServiceDetails([FromBody]NotificationRequest request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            try
            {
                var notificaiton = db.UserNotifications.Where(x => x.Id == request.NotificationId).FirstOrDefault();
                if (notificaiton != null)
                {
                    notificaiton.Status = Utilities.NotificationStatus.Read.GetEnumDescription();
                    notificaiton.UpdatedBy = request.ClientId;
                    notificaiton.UpdatedDate = DateTime.UtcNow;
                    notificaiton.UserTypeId = Utilities.UserRoles.Client.GetEnumValue();
                    db.SaveChanges();
                }
                var UserInfo = db.Clients.Where(x => x.Id == request.ClientId && x.IsDeleted == false).FirstOrDefault();
                if (!UserInfo.IsActive)
                {
                    res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                    res.Message = "Your account was deactivated by Admin";
                    res.Data = null;
                }
                else
                {
                    var service = db.Services.Where(x => x.Id == request.ServiceId).FirstOrDefault();
                    var serviceReport = service.ServiceReports.FirstOrDefault();
                    var noShow = service.ServiceNoShows.FirstOrDefault();
                    var d = new
                    {
                        ServiceId = service.Id,
                        service.ServiceCaseNumber,
                        NoShowAmount = (noShow == null ? 0 : noShow.NoShowAmount),
                        Reason = (serviceReport == null ? "" : serviceReport.WorkPerformed),
                        ScheduleDate = service.ScheduleDate.Value.ToString("dd MMMM, yyyy"),
                        EmpFirstName = (service.Employee == null ? "" : service.Employee.FirstName),
                        EmpLastName = (service.Employee == null ? "" : service.Employee.LastName),
                        CollectPayment = (noShow == null ? false : (noShow.NoShowAmount > 0 ? true : false)),
                        Message = (noShow.NoShowAmount == 0m ? noShow.IsNoShow ?
                                        Utilities.GetSiteSettingValue("NoShowNoPaymentMessage", db) : Utilities.GetSiteSettingValue("LateRescheduleNoPaymentMessage", db)
                                        : noShow.IsNoShow ? Utilities.GetSiteSettingValue("NoShowPaymentMessage", db) : Utilities.GetSiteSettingValue("LateReschedulePaymentMessage", db))
                    };

                    var notificaiton1 = db.UserNotifications.Where(x => x.Id == request.NotificationId).FirstOrDefault();
                    if (notificaiton1 != null)
                    {
                        if (d.CollectPayment == false)
                        {
                            db.UserNotifications.Remove(notificaiton1);
                            db.SaveChanges();
                        }
                    }
                    /*
                        ServiceId = service.Id,
                        NoShowAmount = noShow.NoShowAmount,
                        Reason = (serviceReport == null ? "" : serviceReport.WorkPerformed),
                        ScheduleDate = service.ScheduleDate.Value.ToString("dd MMMM, yyyy"),
                        EmpFirstName = service.Employee.FirstName,
                        EmpLastName = service.Employee.LastName,
                        CollectPayment = (noShow.NoShowAmount == 0 ? false : true),
                        Message = (noShow.NoShowAmount == 0 ? Utilities.GetSiteSettingValue("NoShowNoPaymentMessage", db) : Utilities.GetSiteSettingValue("NoShowPaymentMessage", db))
                     */
                    res.StatusCode = (int)HttpStatusCode.OK;
                    res.Message = "Record Found";
                    res.Data = d;
                }

            }
            catch (Exception ex)
            {
                res.StatusCode = (int)HttpStatusCode.BadRequest;
                res.Message = "Invalid Request";
                res.Data = null;
            }
            if (updatetoken)
            {
                res.Token = accessToken;
            }
            else
            {
                res.Token = "";
            }
            db.Dispose();
            return Ok(res);
        }

        [AuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("NoShowPayment")]
        public async Task<IHttpActionResult> NoShowPayment([FromBody]OtherPaymentRequest request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            string StripeErrMsg = "";
            try
            {
                if (request.NotificationId != null)
                {
                    var notification = db.UserNotifications.Where(x => x.Id == request.NotificationId).ToList();
                    if (notification.Count > 0)
                    {
                        db.UserNotifications.RemoveRange(notification);
                        db.SaveChanges();
                    }
                }
                var UserInfo = db.Clients.Where(x => x.Id == request.ClientId && x.IsDeleted == false).FirstOrDefault();
                if (!UserInfo.IsActive)
                {
                    res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                    res.Message = "Your account was deactivated by Admin";
                    res.Data = null;
                }
                else
                {
                    ClientPaymentMethod cpm1 = new ClientPaymentMethod();

                    if (request.CardNumber.Contains("*"))
                    {
                        cpm1 = db.ClientPaymentMethods.Where(x => x.ClientId == request.ClientId && x.CardNumber == request.CardNumber).FirstOrDefault();
                    }
                    else
                    {
                        cpm1 = null;
                    }
                    try
                    {
                        if (request.CardNumber == "" || request.ExpiryMonth == 0 || request.ExpiryYear == 0 || request.NameOnCard == "")
                        {
                            res.StatusCode = (int)HttpStatusCode.BadRequest;
                            res.Message = "Invalid Request";
                            res.Data = null;
                            db.Dispose();
                            return Ok(res);
                        }
                        if (!string.IsNullOrWhiteSpace(UserInfo.CustomerProfileId))
                        {
                            try
                            {
                                ClientPaymentMethod cpm = new ClientPaymentMethod();
                                if (cpm1 != null)
                                {
                                    cpm = cpm1;
                                }
                                else
                                {
                                    String s = request.CardNumber.Substring(request.CardNumber.Length - 4);
                                    cpm.CardNumber = s.PadLeft(16, '*');
                                    var customerService = new StripeCustomerService(ConfigurationManager.AppSettings["StripeApiKey"].ToString());
                                    var myCustomer = customerService.Get(UserInfo.CustomerProfileId);
                                    // setting up the card
                                    var myCard = new StripeCardCreateOptions();
                                    // setting up the card
                                    myCard.SourceCard = new SourceCard()
                                    {
                                        Number = request.CardNumber.ToString(),
                                        ExpirationYear = request.ExpiryYear.ToString(),
                                        ExpirationMonth = request.ExpiryMonth.ToString(),
                                        Name = request.NameOnCard,
                                        Cvc = request.CVV.ToString()
                                    };

                                    var cardService = new StripeCardService();
                                    StripeCard stripeCard = cardService.Create(UserInfo.CustomerProfileId, myCard);
                                    cpm.CustomerPaymentProfileId = stripeCard.Id;
                                    cpm.CardType = (stripeCard.Brand.Contains("American") ? request.CardType : stripeCard.Brand);
                                    cpm.ClientId = request.ClientId;
                                    cpm.ExpiryMonth = request.ExpiryMonth;
                                    cpm.ExpiryYear = request.ExpiryYear;
                                    cpm.AddedBy = request.ClientId;
                                    cpm.NameOnCard = request.NameOnCard;

                                    cpm.IsDefaultPayment = false;
                                    cpm.AddedByType = (int)Utilities.UserRoles.Client;
                                    cpm.AddedDate = DateTime.UtcNow;
                                    if (UserInfo.ClientPaymentMethods.Count <= 0)
                                    {
                                        cpm.IsDefaultPayment = true;
                                    }
                                    db.ClientPaymentMethods.Add(cpm);

                                    db.SaveChanges();
                                    cpm1 = cpm;
                                }
                                if (string.IsNullOrWhiteSpace(cpm.CustomerPaymentProfileId))
                                {
                                    StripeErrMsg = "Invalid Credit Card";
                                    res.StatusCode = (int)HttpStatusCode.BadRequest;
                                    res.Message = StripeErrMsg;
                                    res.Data = null;
                                }
                                else
                                {
                                    res.Message = "Card Saved";
                                    res.StatusCode = (int)HttpStatusCode.OK;

                                    var service = db.Services.Where(x => x.Id == request.ServiceId).FirstOrDefault();
                                    //var billingAddress = service.ClientAddress;
                                    var noShow = service.ServiceNoShows.FirstOrDefault();
                                    var total = 0m;

                                    BillingHistory bh = new BillingHistory();
                                    bh.BillingFirstName = request.FirstName;
                                    bh.BillingLastName = request.LastName;
                                    bh.Company = request.Company;
                                    bh.BillingAddress = request.Address;
                                    bh.BillingCity = request.City;
                                    bh.BillingState = request.State;
                                    bh.ClientId = request.ClientId;
                                    bh.BillingZipcode = request.ZipCode;
                                    bh.BillingMobileNumber = request.MobileNumber;
                                    bh.BillingPhoneNumber = request.PhoneNumber;
                                    bh.PackageName = "Charge for NoShow Service";
                                    bh.ServiceCaseNumber = service.ServiceCaseNumber;
                                    bh.BillingType = Utilities.BillingTypes.FixedCost.GetEnumDescription();
                                    bh.IsSpecialOffer = false;
                                    bh.PurchasedAmount = bh.OriginalAmount = noShow.NoShowAmount;
                                    bh.PartnerSalesCommisionAmount = 0;
                                    var Description = " NoShow Service : " + service.ServiceCaseNumber;
                                    var sr = Utilities.StripeCharge(true, "", UserInfo.CustomerProfileId, cpm1.CustomerPaymentProfileId, (int)(noShow.NoShowAmount * 100), Description, "", db, request.ClientId, 0);
                                    bh.TransactionId = sr.TransactionId;
                                    bh.TransactionDate = DateTime.UtcNow;
                                    bh.IsPaid = true;
                                    bh.failcode = "";
                                    bh.faildesc = "Payment Success!";
                                    bh.AddedBy = request.ClientId;
                                    bh.AddedDate = DateTime.UtcNow;
                                    total = total + bh.PurchasedAmount.Value;

                                    Order cOrder = new Order();

                                    cOrder.CardNumber = cpm1.CardNumber;
                                    cOrder.NameOnCard = cpm1.NameOnCard;
                                    cOrder.ExpirationMonth = cpm1.ExpiryMonth;
                                    cOrder.ExpirationYear = cpm1.ExpiryYear;
                                    cOrder.CardType = cpm1.CardType;
                                    cOrder.CCEmail = UserInfo.Email;
                                    cOrder.ClientId = request.ClientId;
                                    cOrder.AddedBy = request.ClientId;
                                    cOrder.AddedByType = (int)Utilities.UserRoles.Client;
                                    cOrder.AddedDate = DateTime.UtcNow;
                                    cOrder.IsDeleted = false;
                                    cOrder.OrderType = "Charge";
                                    cOrder.ChargeBy = Utilities.ChargeBy.CCOnFile.GetEnumDescription();
                                    cOrder.OrderAmount = total;
                                    var orderCount = db.Orders.Where(x => x.ClientId == request.ClientId && x.IsDeleted == false).Count();
                                    var ordernumber = UserInfo.AccountNumber + "-" + request.ZipCode + "-O" + (orderCount + 1).ToString();
                                    cOrder.OrderNumber = ordernumber;
                                    cOrder.BillingHistories.Add(bh);
                                    db.Orders.Add(cOrder);
                                    db.SaveChanges();

                                    if (sr.ex != null)
                                    {
                                        res.StatusCode = (int)HttpStatusCode.BadRequest;
                                        res.Message = "Invalid Credit Card";
                                        res.Data = null;
                                    }
                                    else
                                    {
                                        var servicepart = service.ServiceCaseNumber.Split(("-").ToArray());
                                        servicepart[servicepart.Length - 1] = (int.Parse(servicepart[servicepart.Length - 1].ToString()) + 1).ToString().PadLeft(2, '0');
                                        //var svs = db.Services.Where(x => x.ClientId == UserInfo.Id).Count() + 1;
                                        //var serviceNo = UserInfo.AccountNumber + "-" + service.ClientAddress.ZipCode + "-S" + svs.ToString();
                                        var serviceNo = string.Join("-", servicepart);
                                        int days = 0;
                                        if (service.PurposeOfVisit == Utilities.PurposeOfVisit.Maintenance.GetEnumDescription() || service.PurposeOfVisit == Utilities.PurposeOfVisit.Repairing.GetEnumDescription())
                                        {
                                            //days = int.Parse(Utilities.GetSiteSettingValue("MaintenanceAndRepairingServicesWithinDays", db));
                                        }

                                        if (service.PurposeOfVisit == Utilities.PurposeOfVisit.Emergency.GetEnumDescription() || service.PurposeOfVisit == Utilities.PurposeOfVisit.ContinuingPreviousWork.GetEnumDescription())
                                        {
                                            //days = int.Parse(Utilities.GetSiteSettingValue("EmergencyAndOtherServiceWithinDays", db));
                                        }

                                        if (service.Status != Utilities.ServiceTypes.LateCancelled.GetEnumDescription())
                                        {
                                            db.uspa_Services_UpdateNoShowService(service.Id, request.ClientId, Utilities.UserRoles.Client.GetEnumValue(), DateTime.UtcNow);
                                        }
                                        else
                                        {
                                            service.Status = Utilities.ServiceTypes.Cancelled.GetEnumDescription();
                                            db.SaveChanges();
                                        }
                                        EmailTemplate templateclient = db.EmailTemplates.Where(x => x.Name == "NoShowReceiptClient" && x.Status == true).FirstOrDefault();
                                        var strclient = templateclient.EmailBody;

                                        strclient = strclient.Replace("{{FirstName}}", UserInfo.FirstName);
                                        strclient = strclient.Replace("{{LastName}}", UserInfo.LastName);
                                        strclient = strclient.Replace("{{Email}}", UserInfo.Email);
                                        strclient = strclient.Replace("{{MobileNumber}}", UserInfo.MobileNumber);
                                        strclient = strclient.Replace("{{PaymentDate}}", bh.TransactionDate.Value.ToString("MM/dd/yyyy"));
                                        strclient = strclient.Replace("{{ServiceCaseNumber}}", service.ServiceCaseNumber);
                                        strclient = strclient.Replace("{{Amount}}", "$ " + bh.PurchasedAmount.Value.ToString("#.##"));
                                        strclient = strclient.Replace("{{TransactionNumber}}", bh.TransactionId);
                                        Utilities.Send(templateclient.EmailTemplateSubject, UserInfo.Email, strclient, templateclient.FromEmail, db);

                                        res.StatusCode = (int)HttpStatusCode.OK;
                                        res.Message = "Payment Done";
                                        res.Data = new
                                        {
                                            UserInfo.FirstName,
                                            UserInfo.LastName,
                                            UserInfo.Email,
                                            service.ServiceCaseNumber,
                                            noShow.NoShowAmount
                                        };
                                    }
                                }
                            }
                            catch (StripeException exx)
                            {
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                res.StatusCode = (int)HttpStatusCode.BadRequest;
                res.Message = "Invalid Request";
                res.Data = null;
            }
            if (updatetoken)
            {
                res.Token = accessToken;
            }
            else
            {
                res.Token = "";
            }
            db.Dispose();
            return Ok(res);
        }

        [AuthorizationRequired]
        [ResponseType(typeof(ResponseListModel))]
        [HttpPost]
        [Route("NotificationList")]
        public async Task<IHttpActionResult> NotificationList([FromBody]CommonRequest request)
        {
            db = new Aircall_DBEntities1();
            ResponseListModel res = new ResponseListModel();
            int totalRecord = 0;
            int pageCnt = 0;
            int totalPageCount = 0;
            try
            {
                var UserInfo = await db.Clients.Where(x => x.Id == request.ClientId && x.IsDeleted == false).FirstOrDefaultAsync();
                if (UserInfo != null)
                {
                    if (!UserInfo.IsActive)
                    {
                        res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                        res.Message = "Your account was deactivated by Admin";
                        res.Data = null;
                    }
                    else
                    {
                        var ClientNotificationsData = db.uspa_ClientPortal_GetNotificationForDashBoardByUserType(request.ClientId, Utilities.UserRoles.Client.GetEnumValue(), "", 0).ToList();
                        List<NotificationListModel> NotificationData = new List<NotificationListModel>();
                        List<NotificationListModel> result = new List<NotificationListModel>();

                        if (request.LastCallDateTime == null)
                        {
                        }
                        else
                        {
                            ClientNotificationsData = ClientNotificationsData.Where(x => x.AddedDate >= request.LastCallDateTime).ToList();
                        }
                        foreach (var item in ClientNotificationsData)
                        {
                            string Date = "";
                            if (item.AddedDate == null)
                            {

                            }
                            if (item.AddedDate.Date == DateTime.UtcNow.Date)
                            {
                                Date = "Today " + item.AddedDate.ToLocalTime().ToString("hh:mm tt");
                            }
                            else
                            {
                                //DateTime runtimeKnowsThisIsUtc = DateTime.SpecifyKind(item.AddedDate, DateTimeKind.Utc);
                                Date = item.AddedDate.ToLocalTime().ToString("dd MMMM yyyy");
                            }
                            Service service = new Service();
                            if (item.MessageType == Utilities.NotificationType.NoShow.GetEnumDescription() || item.MessageType == Utilities.NotificationType.RateService.GetEnumDescription() || item.MessageType == Utilities.NotificationType.ServiceScheduled.GetEnumDescription() || item.MessageType == Utilities.NotificationType.PeriodicServiceReminder.GetEnumDescription() || item.MessageType == Utilities.NotificationType.ServiceScheduled.GetEnumDescription())
                            {
                                service = db.Services.Where(x => x.Id == item.CommonId).FirstOrDefault();
                            }
                            else
                            {
                                service = null;
                            }
                            var emp = (service != null ? service.Employee : null);
                            Utilities.NotificationType MessageType = Enum.GetValues(typeof(Utilities.NotificationType)).Cast<Utilities.NotificationType>()
                                                               .FirstOrDefault(v => v.GetEnumDescription() == item.MessageType);
                            try
                            {
                                NotificationData.Add(new NotificationListModel()
                                {
                                    ClientId = item.UserId,
                                    CommonId = (item.CommonId.HasValue ? item.CommonId.Value : 0),
                                    NotificationId = item.Id.Value,
                                    NotificationType = MessageType.GetEnumValue(),
                                    Message = item.Message,
                                    DateTime = Date,
                                    ProfileImage = (emp != null ? (string.IsNullOrWhiteSpace(emp.Image) ? ConfigurationManager.AppSettings["SiteAddress"].ToString() + "uploads/profile/employee/defultimage.jpg" : ConfigurationManager.AppSettings["EMPProfileImageURL"].ToString() + emp.Image) : ""),
                                    LastAdded = item.AddedDate,
                                    ScheduleDay = (service == null ? "0" : (service.ScheduleDate.HasValue ? service.ScheduleDate.Value.ToString("dd") : "")),
                                    ScheduleMonth = (service == null ? "" : (service.ScheduleDate.HasValue ? service.ScheduleDate.Value.ToString("MMMM") : "")),
                                    ScheduleYear = (service == null ? "" : (service.ScheduleDate.HasValue ? service.ScheduleDate.Value.ToString("yyyy") : "")),
                                    ScheduleStartTime = (service == null ? "" : (string.IsNullOrWhiteSpace(service.ScheduleStartTime) ? "" : service.ScheduleStartTime)),
                                    ScheduleEndTime = (service == null ? "" : (string.IsNullOrWhiteSpace(service.ScheduleEndTime) ? "" : service.ScheduleEndTime)),
                                    Status = item.Status,
                                    IsRequested = (service == null ? false : (service.RequestedServiceBridges.Count > 0))
                                });
                            }
                            catch (Exception dd)
                            {

                            }
                        }
                        var pageSize = int.Parse(Utilities.GetSiteSettingValue("ApplicationPageSize", db));
                        if (request.PageNumber.HasValue)
                        {
                            result = CreatePagedResults<NotificationListModel, NotificationListModel>(NotificationData.AsQueryable(), request.PageNumber.Value, pageSize, out totalRecord, out pageCnt, out totalPageCount).ToList();
                        }
                        else
                        {
                            result = CreatePagedResults<NotificationListModel, NotificationListModel>(NotificationData.AsQueryable(), 1, pageSize, out totalRecord, out pageCnt, out totalPageCount).ToList();
                        }

                        if (result.Count > 0)
                        {
                            res.Data = result;
                            res.Message = "Records Found";
                            res.StatusCode = HttpStatusCode.OK.GetEnumValue();
                            res.LastCallDateTime = result.Last().LastAdded;
                            res.PageNumber = pageCnt;
                            res.TotalNumberOfPages = totalPageCount;
                            res.TotalNumberOfRecords = totalRecord;
                            foreach (var item in result)
                            {
                                Utilities.NotificationType n = (Utilities.NotificationType)item.NotificationType;
                                switch (n)
                                {
                                    case Utilities.NotificationType.FriendlyReminder:
                                    //case Utilities.NotificationType.PartPurchased:
                                    case Utilities.NotificationType.AdminNotification:
                                    case Utilities.NotificationType.PlanRenewed:
                                    case Utilities.NotificationType.UnitPlanRenew:
                                    case Utilities.NotificationType.UnitPlanCancelled:
                                    case Utilities.NotificationType.SubscriptionInvoicePaymentReminder:
                                    case Utilities.NotificationType.PastDueReminder:
                                        var oldNotification = db.UserNotifications.Find(item.NotificationId);
                                        oldNotification.Status = Utilities.NotificationStatus.Read.GetEnumDescription();
                                        db.SaveChanges();
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                        else
                        {
                            res.Data = NotificationData;
                            res.Message = "No record found";
                            res.StatusCode = HttpStatusCode.NotFound.GetEnumValue();
                            res.PageNumber = pageCnt - 1;
                            res.TotalNumberOfPages = totalPageCount;
                            res.TotalNumberOfRecords = totalRecord;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                res.Data = null;
                res.Message = "Internal Server Error";
                res.StatusCode = HttpStatusCode.InternalServerError.GetEnumValue();
            }
            if (updatetoken)
            {
                res.Token = accessToken;
            }
            else
            {
                res.Token = "";
            }
            db.Dispose();
            return Ok(res);
        }


        [AuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("GetClientUnitByAddressIdPlanType")]
        public async Task<IHttpActionResult> GetClientUnit([FromBody]CommonRequest request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            var UserInfo = await db.Clients.Where(x => x.Id == request.ClientId && x.IsDeleted == false).FirstOrDefaultAsync();
            ClientAddress address = new ClientAddress();
            List<object> data = new List<object>();
            if (UserInfo != null)
            {
                if (!UserInfo.IsActive)
                {
                    res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                    res.Message = "Your account was deactivated by Admin";
                    res.Data = null;
                }
                else
                {
                    int PeraAddressId = 0;
                    if (request.AddressId == null)
                    {
                        address = UserInfo.ClientAddresses.Where(x => x.IsDefaultAddress == true).FirstOrDefault();
                    }
                    if (address != null || request.AddressId != null)
                    {
                        if (request.AddressId == null)
                        {
                            PeraAddressId = address.Id;
                        }
                        else
                        {
                            PeraAddressId = request.AddressId.Value;
                        }

                        //var ClientUnits = db.ClientUnits.Where(x => x.ClientId == request.ClientId && x.IsDeleted == false && x.IsActive == true && x.AddressId == PeraAddressId && x.PlanTypeId == request.PlanTypeId && x.IsPlanRenewedOrCancelled == false).AsEnumerable().Where(x => x.PaymentStatus == Utilities.UnitPaymentTypes.Received.GetEnumDescription()).ToList();
                        var ClientUnits = db.ClientUnits.Where(x => x.IsSubmittedToSubscription == true && x.ClientId == request.ClientId && x.IsDeleted == false && x.IsActive == true && x.AddressId == PeraAddressId && x.PlanTypeId == request.PlanTypeId).ToList();
                        foreach (var cUnit in ClientUnits)
                        {
                            var d = new
                            {
                                Id = cUnit.Id,
                                UnitName = cUnit.UnitName,
                            };
                            data.Add(d);
                        }
                    }
                    else
                    {
                        res.StatusCode = (int)HttpStatusCode.NotFound;
                        res.Message = "No record found";
                        res.Data = null;
                    }

                    if (data.Count > 0)
                    {
                        res.StatusCode = (int)HttpStatusCode.OK;
                        res.Message = "Record Found";
                        res.Data = data;
                    }
                    else
                    {
                        res.StatusCode = (int)HttpStatusCode.NotFound;
                        res.Message = "No record found";
                        res.Data = null;
                    }
                }

            }
            else
            {
                res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                res.Message = "You are not authorized to view this data";
                res.Data = null;
            }
            if (updatetoken)
            {
                res.Token = accessToken;
            }
            else
            {
                res.Token = "";
            }
            db.Dispose();
            return Ok(res);
        }

        [AuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("UpdateClientUnit")]
        public async Task<IHttpActionResult> UpdateClientUnit([FromBody]ClientUnitAddModel request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            try
            {
                var unitExists = db.ClientUnits.Where(x => x.UnitName == request.UnitName && x.ClientId == request.ClientId && x.Id != request.UnitId).FirstOrDefault();
                if (unitExists != null)
                {

                    res.StatusCode = (int)HttpStatusCode.Ambiguous;
                    res.Message = "Unit Name Already Exists";
                    res.Data = null;

                    if (updatetoken)
                    {
                        res.Token = accessToken;
                    }
                    else
                    {
                        res.Token = "";
                    }
                    return Ok(res);
                }
                var Unit = db.ClientUnits.Where(x => x.Id == request.UnitId && x.IsActive == true && x.IsDeleted == false).FirstOrDefault();
                if (Unit != null)
                {
                    Unit.UnitName = request.UnitName;
                    Unit.UpdatedBy = request.ClientId;
                    Unit.UpdatedByType = Utilities.UserRoles.Client.GetEnumValue();
                    Unit.UpdatedDate = DateTime.UtcNow;
                    db.SaveChanges();

                    res.StatusCode = (int)HttpStatusCode.OK;
                    res.Message = "Record Saved";
                    res.Data = null;
                }
                else
                {
                    res.StatusCode = (int)HttpStatusCode.NotFound;
                    res.Message = "Record Not Found";
                    res.Data = null;
                }
            }
            catch (Exception ex)
            {
                res.StatusCode = (int)HttpStatusCode.BadRequest;
                res.Message = "Invalid Request";
                res.Data = null;
            }
            if (updatetoken)
            {
                res.Token = accessToken;
            }
            else
            {
                res.Token = "";
            }
            db.Dispose();
            return Ok(res);
        }

        [AuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpGet]
        [Route("SendSalesAgreement")]
        public async Task<IHttpActionResult> SendSalesAgreement([FromUri]int ClientId)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            var UserInfo = await db.Clients.Where(x => x.Id == ClientId && x.IsActive == true && x.IsDeleted == false).FirstOrDefaultAsync();
            if (UserInfo != null)
            {
                if (UserInfo.IsActive)
                {
                    EmailTemplate templateSendDisclosureEmailClient = db.EmailTemplates.Where(x => x.Name == "SendSalesAgreementEmailClient" && x.Status == true).FirstOrDefault();
                    var strSendDisclosureEmail = templateSendDisclosureEmailClient.EmailBody;

                    var DisclosurePage = db.CMSPages.Where(x => x.Id == 23).FirstOrDefault().Description;
                    strSendDisclosureEmail = strSendDisclosureEmail.Replace("{{FirstName}}", UserInfo.FirstName + " " + UserInfo.FirstName);
                    strSendDisclosureEmail = strSendDisclosureEmail.Replace("{{Message}}", DisclosurePage);

                    Utilities.Send(templateSendDisclosureEmailClient.EmailTemplateSubject, UserInfo.Email, strSendDisclosureEmail, templateSendDisclosureEmailClient.FromEmail, db);

                    res.StatusCode = (int)HttpStatusCode.OK;
                    res.Message = "Success";
                    res.Data = null;
                }
                else
                {
                    res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                    res.Message = "Your account was deactivated by Admin";
                    res.Data = null;
                }
            }
            else
            {
                res.StatusCode = (int)HttpStatusCode.NotFound;
                res.Message = "User not found";
                res.Data = null;
            }
            if (updatetoken)
            {
                res.Token = accessToken;
            }
            else
            {
                res.Token = "";
            }
            db.Dispose();
            return Ok(res);
        }

        [AuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("CancelRequestedServices")]
        public async Task<IHttpActionResult> CancelRequestedServices([FromBody]ServiceRequestModel request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            List<object> data = new List<object>();

            try
            {
                var UserInfo = db.Clients.Where(x => x.Id == request.ClientId && x.IsDeleted == false).FirstOrDefault();
                if (!UserInfo.IsActive)
                {
                    res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                    res.Message = "Your account was deactivated by Admin.";
                    res.Data = null;
                }
                else
                {
                    var notification = db.UserNotifications.AsEnumerable().Where(x => x.CommonId == request.ServiceId && x.MessageType == Utilities.NotificationType.ServiceApproval.GetEnumDescription()).ToList();
                    if (notification.Count > 0)
                    {
                        db.UserNotifications.RemoveRange(notification);
                        db.SaveChanges();
                    }

                    var notification1 = db.UserNotifications.AsEnumerable().Where(x => x.CommonId == request.ServiceId && x.MessageType == Utilities.NotificationType.PeriodicServiceReminder.GetEnumDescription()).ToList();
                    if (notification1.Count > 0)
                    {
                        db.UserNotifications.RemoveRange(notification1);
                        db.SaveChanges();
                    }

                    var notification2 = db.UserNotifications.AsEnumerable().Where(x => x.CommonId == request.ServiceId && x.MessageType == Utilities.NotificationType.ServiceScheduled.GetEnumDescription()).ToList();
                    if (notification2.Count > 0)
                    {
                        db.UserNotifications.RemoveRange(notification2);
                        db.SaveChanges();
                    }
                    var ClientService = db.Services.Where(x => x.ClientId == request.ClientId && x.Id == request.ServiceId).FirstOrDefault();
                    if (ClientService != null)
                    {
                        var isSchedueled = false;
                        if (request.IsCancelled)
                        {
                            ClientService.Status = Utilities.ServiceTypes.Cancelled.GetEnumDescription();
                        }
                        else
                        {
                            if (ClientService.Status == Utilities.ServiceTypes.Scheduled.GetEnumDescription())
                            {
                                isSchedueled = true;
                            }
                            ClientService.Status = Utilities.ServiceTypes.Cancelled.GetEnumDescription();
                        }

                        if (isSchedueled)
                        {
                            var EmpNotification = db.NotificationMasters.Where(x => x.Name == "CancelledServiceSendToEmployee").FirstOrDefault();
                            var message = EmpNotification.Message;
                            message = message.Replace("{{ClientName}}", ClientService.Client.FirstName + " " + ClientService.Client.LastName);
                            message = message.Replace("{{ScheduleDate}}", ClientService.ScheduleDate.Value.ToString("MMMM dd, yyyy"));
                            UserNotification objUserNotification = new UserNotification();
                            objUserNotification.UserId = ClientService.EmployeeId;
                            objUserNotification.UserTypeId = Utilities.UserRoles.Employee.GetEnumValue();
                            objUserNotification.Message = message;
                            objUserNotification.Status = Utilities.NotificationStatus.UnRead.GetEnumDescription();
                            objUserNotification.CommonId = ClientService.Id;
                            objUserNotification.MessageType = Utilities.NotificationType.FriendlyReminder.GetEnumDescription();
                            objUserNotification.AddedDate = DateTime.UtcNow;
                            db.UserNotifications.Add(objUserNotification);
                            db.SaveChanges();

                            var BadgeCount = db.UserNotifications.AsEnumerable().Where(x => x.UserId == ClientService.EmployeeId && x.UserTypeId == Utilities.UserRoles.Employee.GetEnumValue() && x.Status == Utilities.NotificationStatus.UnRead.GetEnumDescription()).ToList().Count;

                            Notifications objNotifications = new Notifications { NId = objUserNotification.Id, NType = Utilities.NotificationType.FriendlyReminder.GetEnumValue(), CommonId = ClientService.Id };
                            List<NotificationModel> notify = new List<NotificationModel>();
                            notify.Add(new NotificationModel { Key = "NId", Value = new object[] { objNotifications.NId } });
                            notify.Add(new NotificationModel { Key = "NType", Value = new object[] { objNotifications.NType } });
                            notify.Add(new NotificationModel { Key = "CommonId", Value = new object[] { objNotifications.CommonId } });
                            var EmpInfo = db.Employees.Where(x => x.Id == ClientService.EmployeeId).FirstOrDefault();
                            if (EmpInfo.DeviceType != null && EmpInfo.DeviceToken != null)
                            {
                                if (EmpInfo.DeviceType.ToLower() == "android")
                                {
                                }
                                else if (EmpInfo.DeviceType.ToLower() == "iphone")
                                {
                                    SendNotifications.SendIphoneNotification(BadgeCount, EmpInfo.DeviceToken, message, notify, "employee", HttpContext.Current);
                                }
                            }
                        }

                        ClientService.Status = request.IsLateReschedule ? Utilities.ServiceTypes.LateCancelled.GetEnumDescription() : ClientService.Status;
                        ClientService.ApprovalEmailUrl = null;
                        ClientService.UrlExpireDate = null;

                        if (request.IsLateReschedule == false)
                        {
                            ClientService.EmployeeId = null;
                            ClientService.WorkAreaId = null;
                            ClientService.ScheduleDate = null;
                            ClientService.ScheduleStartTime = null;
                            ClientService.ScheduleEndTime = null;
                        }
                        ClientService.StatusChangeDate = DateTime.UtcNow;
                        ClientService.UpdatedBy = request.ClientId;
                        ClientService.UpdatedByType = Utilities.UserRoles.Client.GetEnumValue();
                        ClientService.UpdatedDate = DateTime.UtcNow;

                        var es = db.EmployeeSchedules.Where(x => x.ServiceId == request.ServiceId).FirstOrDefault();
                        if (es != null)
                        {
                            db.EmployeeSchedules.Remove(es);
                        }
                        var notifications = db.UserNotifications.AsEnumerable().Where(x => x.CommonId == request.ServiceId && x.MessageType == Utilities.NotificationType.ServiceScheduled.GetEnumDescription()).ToList();
                        if (notifications.Count > 0)
                        {
                            db.UserNotifications.RemoveRange(notifications);
                        }
                        foreach (var parts in ClientService.ServicePartLists)
                        {
                            var pt = parts;
                            if (pt != null)
                            {
                                pt.Part.ReservedQuantity = pt.Part.ReservedQuantity - parts.PartQuantity;
                            }
                        }
                        var removeParts = db.ServicePartLists.Where(x => x.ServiceId == ClientService.Id).ToList();
                        if (removeParts.Count > 0)
                        {
                            db.ServicePartLists.RemoveRange(removeParts);
                            db.SaveChanges();
                        }
                        if (ClientService.EmployeeSchedules.Count > 0)
                        {
                            db.EmployeeSchedules.RemoveRange(ClientService.EmployeeSchedules);
                            db.SaveChanges();
                        }

                        if (ClientService.RequestedServiceBridges.Count > 0)
                        {
                            var rsId = ClientService.RequestedServiceBridges.FirstOrDefault().RequestedServiceId;
                            var requests = db.RequestedServices.Find(rsId);
                            requests.IsDeleted = true;
                            requests.DeletedBy = UserInfo.Id;
                            requests.DeletedByType = Utilities.UserRoles.Client.GetEnumValue();
                            requests.DeletedDate = DateTime.UtcNow;
                        }


                        db.SaveChanges();

                        var unitid = ClientService.ServiceUnits.Select(x => x.UnitId).ToArray();
                        var sUnits = db.ClientUnits.Where(x => unitid.Contains(x.Id)).ToList();
                        if(sUnits.Count() > 0)
                        {
                            sUnits.ForEach(s => s.Status = Utilities.UnitStatus.Serviced.GetEnumValue());
                            db.SaveChanges();

                        }


                        var ServiceRescheduleSuccessMessage = Utilities.GetSiteSettingValue("ServiceCancelSuccessMessage", db);
                        res.StatusCode = HttpStatusCode.OK.GetEnumValue();
                        res.Message = ServiceRescheduleSuccessMessage;
                        res.Data = null;
                    }
                    else
                    {
                        res.StatusCode = HttpStatusCode.NotFound.GetEnumValue();
                        res.Message = "Record Not Found";
                        res.Data = null;
                    }
                }
            }
            catch (Exception ex)
            {
                res.StatusCode = HttpStatusCode.InternalServerError.GetEnumValue();
                res.Message = "Internal Server Error";
                res.Data = null;
            }

            if (updatetoken)
            {
                res.Token = accessToken;
            }
            else
            {
                res.Token = "";
            }
            db.Dispose();
            return Ok(res);
        }
    }
}
