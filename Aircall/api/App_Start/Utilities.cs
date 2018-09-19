using api.Models;
using AutoMapper;
using Stripe;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace api.App_Start
{
    public static class DateTimeExtensions
    {
        public static int TotalMonths(this DateTime start, DateTime end)
        {
            return (start.Year * 12 + start.Month) - (end.Year * 12 + end.Month);
        }
    }
    public static class DurationExtensions
    {
        public static string GetEnumDescription(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0) return attributes[0].Description;
            else
                return value.ToString();
        }
        public static int GetEnumValue(this Enum value)
        {
            return Convert.ToInt32(value);
        }
        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
        public static IDictionary<T, string> GetEnumValuesWithDescription<T>(this Type type) where T : struct, IConvertible
        {
            if (!type.IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }

            return type.GetEnumValues()
                    .OfType<T>()
                    .ToDictionary(
                        key => key,
                        val => (val as Enum).GetEnumDescription()
                    );
        }
        public static string Ordinal(this int number)
        {
            string suffix = String.Empty;

            int ones = number % 10;
            int tens = (int)Math.Floor(number / 10M) % 10;

            if (tens == 1)
            {
                suffix = "th";
            }
            else
            {
                switch (ones)
                {
                    case 1:
                        suffix = "st";
                        break;

                    case 2:
                        suffix = "nd";
                        break;

                    case 3:
                        suffix = "rd";
                        break;

                    default:
                        suffix = "th";
                        break;
                }
            }
            return String.Format("{0}{1}", number, suffix);
        }
        public static string GetEnumDisplayName(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DisplayAttribute[] attributes = (DisplayAttribute[])fi.GetCustomAttributes(typeof(DisplayAttribute), false);

            if (attributes != null && attributes.Length > 0) return attributes[0].Name;
            else
                return value.ToString();
        }
        public static T GetValueFromDescription<T>(string description)
        {
            var type = typeof(T);
            if (!type.IsEnum) throw new InvalidOperationException();
            foreach (var field in type.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field,
                    typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (attribute != null)
                {
                    if (attribute.Description == description)
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (T)field.GetValue(null);
                }
            }
            throw new ArgumentException("Not found.", "description");
        }
        public static string GetDisplayNameFromDescription<T>(string description)
        {
            var type = typeof(T);
            if (!type.IsEnum) throw new InvalidOperationException();
            foreach (var field in type.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field,
                    typeof(DescriptionAttribute)) as DescriptionAttribute;
                var attribute1 = Attribute.GetCustomAttribute(field,
                    typeof(DisplayAttribute)) as DisplayAttribute;
                if (attribute != null)
                {
                    if (attribute.Description == description)
                        return attribute1.Name;
                }
                else
                {
                    if (field.Name == description)
                        return "";
                }
            }
            throw new ArgumentException("Not found.", "description");
        }
    }
    public class StripeResponse
    {
        public string TransactionId { get; set; }
        public string PaymentStatus { get; set; }
        public Exception ex { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime StripeNextPaymentDate { get; set; }
    }
    public class StreamResponse
    {
        public int UnitId { get; set; }
        public string PaymentStatus { get; set; }
    }
    public class Utilities
    {
        public enum PurposeOfVisit
        {
            [Description("Repair")]
            [Display(Name = "Repair (Mon-Fri)")]
            Repairing = 0,
            [Description("Emergency")]
            [Display(Name = "Emergency 7 days a week \n(Mon-Sun) $125")]
            Emergency = 1,
            [Description("Continuing Previous Work")]
            [Display(Name = "Continuing Previous Work \n(Mon-Fri)")]
            ContinuingPreviousWork = 2,
            [Description("Maintenance Services")]
            [Display(Name = "Maintenance Service \n(Mon-Fri)")]
            Maintenance = 3
        }
        public enum UnitStatus
        {
            SelectStatus = 0,
            [Description("Serviced")]
            Serviced = 1,
            [Description("Service Soon")]
            ServiceSoon = 2,
            [Description("Need Repair")]
            NeedRepair = 3
        }
        public enum UserRoles
        {
            AdminRole = 2,
            WarehouseUser = 3,
            Client = 4,
            Employee = 5,
            Partner = 6
        }
        public enum NotificationStatus
        {
            [Description("UnRead")]
            UnRead = 1,
            [Description("Read")]
            Read = 2
        }
        public enum NotificationType
        {

            [Description("Service Approval")]
            ServiceApproval = 1,
            [Description("Friendly Reminder")]
            FriendlyReminder = 2,
            [Description("No Show")]
            NoShow = 3,
            [Description("Part Purchased")]
            PartPurchased = 4,
            [Description("Rate Service")]
            RateService = 5,
            [Description("Credit Card Expiration")]
            CreditCardExpiration = 6,
            [Description("Admin Notification")]
            AdminNotification = 7,
            [Description("Plan Expiration")]
            PlanExpiration = 8,
            [Description("Plan Renewed")]
            PlanRenewed = 9,
            [Description("Service Scheduled")]
            ServiceScheduled = 10,
            [Description("Payment Failed")]
            PaymentFailed = 11,
            [Description("Unit Plan Renew")]
            UnitPlanRenew = 12,
            [Description("Unit Plan Cancelled")]
            UnitPlanCancelled = 13,
            [Description("Unit Payment Failed")]
            UnitPaymentFailed = 14,
            [Description("Sales Person Visit")]
            SalesPersonVisit = 15,
            [Description("Periodic Service Reminder")]
            PeriodicServiceReminder = 16,
            [Description("Subscription Invoice Payment Failed")]
            SubscriptionInvoicePaymentFailed = 17,
            [Description("Subscription Invoice Payment Reminder")]
            SubscriptionInvoicePaymentReminder = 18,
            [Description("Subscription Invoice Payment Due Reminder")]
            SubscriptionInvoicePaymentDueReminder = 19,
            [Description("Past Due Reminder")]
            PastDueReminder = 20,
            [Description("Late Cancelled")]
            LateCancelled = 21
        }
        public enum UnitPaymentTypes
        {
            [Description("Received")]
            Received = 200,
            [Description("NotReceived")]
            NotReceived = 400,
            [Description("Failed")]
            PaymentFailed = 500,
            [Description("Processing")]
            Processing = 201
        }
        public enum ChargeBy
        {
            [Description("Check")]
            Check = 1,
            [Description("CC on File")]
            CCOnFile = 2,
            [Description("New CC")]
            NewCC = 3,
        }
        public enum PaymentMethod
        {
            [Description("Check")]
            Check = 1,
            [Description("CC")]
            CC = 2,
            [Description("PO")]
            PO = 3
        }
        public enum BillingTypes
        {
            [Description("NoShow")]
            NoShow = 1,
            [Description("Recurring Purchase")]
            Recurringpurchase = 2,
            [Description("Recurring")]
            Recurring = 3,
            [Description("Special Offer")]
            SpecialOffer = 4,
            [Description("Fixed Cost")]
            FixedCost = 5
        }
        public enum ServiceTypes
        {
            [Description("Waiting Approval")]
            WaitingApproval = 1,
            [Description("Pending")]
            Pending = 2,
            [Description("Cancelled")]
            Cancelled = 3,
            [Description("Scheduled")]
            Scheduled = 4,
            [Description("Completed")]
            Completed = 5,
            [Description("Deleted")]
            Deleted = 6,
            [Description("No Show")]
            NoShow = 7,
            [Description("Rescheduled")]
            Rescheduled = 8,
            [Description("Late Cancelled")]
            LateCancelled = 9
        }
        public enum UnitStatusColorCode
        {
        }
        public enum EmpPartRequestStatus
        {
            [Description("Need to Order")]
            NeedToOrder = 1,
            [Description("Backordered")]
            Backordered = 2,
            [Description("Cancelled")]
            Cancelled = 3,
            [Description("Completed")]
            Completed = 4,
            [Description("Discontinued")]
            Discontinued = 5
        }
        public enum ReportBilling
        {
            [Description("Fixed Cost Contract")]
            FixedCostContract = 1,
            [Description("C.O.D")]
            COD = 2,
            [Description("Emergency")]
            Emergency = 3
        }
        public static StripeResponse StripeCharge(bool isSpecial, string StripePlanId, string CustomerId, string CardId, int Amount, string Description, string PlanName, Aircall_DBEntities1 db, int ClientId, int UnitId)
        {
            StripeResponse sr = new StripeResponse();
            if (!isSpecial)
            {
                try
                {
                    StripeCustomerService scustservcie = new StripeCustomerService();
                    scustservcie.Update(CustomerId, new StripeCustomerUpdateOptions() { DefaultSource = CardId });
                    var subscriptionService1 = new StripeSubscriptionService();
                    StripeSubscription stripeSubscription1 = subscriptionService1.Create(CustomerId, StripePlanId);
                    sr.TransactionId = stripeSubscription1.Id;
                    sr.PaymentStatus = UnitPaymentTypes.Received.GetEnumDescription();
                    sr.ex = null;
                    StripeInvoiceService inv = new StripeInvoiceService();
                    var inv1 = inv.Upcoming(CustomerId, new StripeUpcomingInvoiceOptions() { SubscriptionId = sr.TransactionId });
                    sr.StripeNextPaymentDate = inv1.PeriodEnd.Date;
                }
                catch (StripeException ex)
                {
                    StripeErrorLog err = Mapper.Map<StripeErrorLog>(ex.StripeError);
                    err.Userid = ClientId;
                    err.UnitId = UnitId;
                    err.DateAdded = DateTime.UtcNow;
                    db.StripeErrorLogs.Add(err);
                    db.SaveChanges();
                    sr.ErrorMessage = ex.StripeError.Error;
                    sr.ex = ex;
                    sr.TransactionId = "";
                    sr.PaymentStatus = UnitPaymentTypes.PaymentFailed.GetEnumDescription();
                }
            }
            else
            {
                try
                {
                    var myCharge = new StripeChargeCreateOptions();
                    // always set these properties
                    myCharge.Amount = Amount;
                    myCharge.Currency = "usd";
                    // set this if you want to
                    myCharge.Description = Description;
                    myCharge.SourceTokenOrExistingSourceId = CardId;
                    // set this property if using a customer - this MUST be set if you are using an existing source!
                    myCharge.CustomerId = CustomerId;
                    // (not required) set this to false if you don't want to capture the charge yet - requires you call capture later
                    myCharge.Capture = true;
                    var chargeService = new StripeChargeService();
                    StripeCharge stripeCharge = chargeService.Create(myCharge);
                    sr.TransactionId = stripeCharge.Id;
                    sr.PaymentStatus = UnitPaymentTypes.Received.GetEnumDescription();
                    sr.ex = null;
                }
                catch (StripeException ex)
                {
                    StripeErrorLog err = Mapper.Map<StripeErrorLog>(ex.StripeError);
                    err.Userid = ClientId;
                    err.UnitId = UnitId;
                    err.DateAdded = DateTime.UtcNow;
                    db.StripeErrorLogs.Add(err);
                    db.SaveChanges();
                    sr.ErrorMessage = ex.StripeError.Error;
                    sr.ex = ex;
                    sr.TransactionId = "";
                    sr.PaymentStatus = UnitPaymentTypes.PaymentFailed.GetEnumDescription();
                }
            }
            return sr;
        }
        private static string HexConverter(System.Drawing.Color c)
        {
            try
            {
                return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
            }
            catch (Exception ex)
            {

            }
            return "";
        }
        public static string returncolorfromStatusHex(UnitStatus s)
        {
            try
            {
                System.Drawing.Color c = new System.Drawing.Color();
                switch (s)
                {
                    case UnitStatus.Serviced:
                        c = System.Drawing.Color.Green;
                        break;
                    case UnitStatus.ServiceSoon:
                        c = System.Drawing.Color.Orange;
                        break;
                    case UnitStatus.NeedRepair:
                        c = System.Drawing.Color.Red;
                        break;
                    default:
                        break;
                }
                return HexConverter(c);
            }
            catch (Exception ex)
            {

            }
            return "";

        }
        public static string returncolorfromStatusRGB(UnitStatus s, int part)
        {
            try
            {
                System.Drawing.Color c = new System.Drawing.Color();
                switch (s)
                {
                    case UnitStatus.Serviced:
                        c = System.Drawing.Color.Green;
                        break;
                    case UnitStatus.ServiceSoon:
                        c = System.Drawing.Color.Orange;
                        break;
                    case UnitStatus.NeedRepair:
                        c = System.Drawing.Color.Red;
                        break;
                    default:
                        break;
                }

                return RGBConverter(c, part);
            }
            catch (Exception ex)
            {

            }
            return "";
        }
        private static string RGBConverter(System.Drawing.Color c, int part)
        {
            try
            {
                string part1 = "";
                switch (part)
                {
                    case 1:
                        part1 = c.R.ToString();
                        break;
                    case 2:
                        part1 = c.G.ToString();
                        break;
                    case 3:
                        part1 = c.B.ToString();
                        break;
                    default:
                        break;
                }
                return part1;
            }
            catch (Exception ex)
            {

            }
            return "";
        }
        public static string GetMd5Hash(MD5 md5Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
        public static bool ValidateToken(string tokenId)
        {
            Aircall_DBEntities1 db = new Aircall_DBEntities1();
            var token = db.AppAccessTokens.Where(t => t.AuthToken == tokenId).FirstOrDefault();
            //var token = _unitOfWork.TokenRepository.Get(t => t.AuthToken == tokenId && t.ExpiresOn > DateTime.Now);
            if (token != null && !(DateTime.Now > token.ExpiresOn))
            {
                token.ExpiresOn = token.ExpiresOn.Value.AddSeconds(Convert.ToDouble(ConfigurationManager.AppSettings["AuthTokenExpiry"]));
                db.Entry(token).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return true;
            }
            return false;
        }
        public static void Send(string Subject, string EmailTo, string Body, string FromAddress, Aircall_DBEntities1 db, string filename = "", string CCEmail = "")
        {
            MailMessage mail = new MailMessage();
            SmtpClient client = new SmtpClient();

            mail.From = new MailAddress(FromAddress, "Aircall System");
            mail.Subject = Subject;
            foreach (var s in EmailTo.Split(';'))
            {
                if (!string.IsNullOrEmpty(s))
                {
                    mail.To.Add(s);
                }
            }
            //mail.To.Add(EmailTo);
            if (!string.IsNullOrWhiteSpace(CCEmail))
            {
                var ccs = CCEmail.Split((";").ToCharArray());
                foreach (var item in ccs)
                {
                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        mail.CC.Add(item);
                    }
                }
            }
            mail.Body = Body;
            mail.Priority = MailPriority.High;
            mail.IsBodyHtml = true;
            if (!string.IsNullOrWhiteSpace(filename))
            {
                Attachment pdf = new Attachment(filename);
                mail.Attachments.Add(pdf);
            }
            client.Port = int.Parse(Utilities.GetSiteSettingValue("SMTPPort", db));
            client.EnableSsl = Convert.ToBoolean(Utilities.GetSiteSettingValue("SMTPSSL", db));
            if (String.IsNullOrEmpty(Utilities.GetSiteSettingValue("SMTPUser", db)) && String.IsNullOrEmpty(Utilities.GetSiteSettingValue("SMTPPassword", db)))
            {
                client.UseDefaultCredentials = true;
            }
            else
            {
                client.UseDefaultCredentials = false;
            }
            client.Host = Utilities.GetSiteSettingValue("SMTPHost", db);
            client.Credentials = new NetworkCredential(Utilities.GetSiteSettingValue("SMTPUser", db), Utilities.GetSiteSettingValue("SMTPPassword", db));
            try
            {
                client.Send(mail);
            }
            catch (Exception ex)
            {
            }
        }

        public static async Task SendAsync(string Subject, string EmailTo, string Body, string FromAddress, Aircall_DBEntities1 db, string filename = "", string CCEmail = "")
        {
            db = new Aircall_DBEntities1();
            MailMessage mail = new MailMessage();
            SmtpClient client = new SmtpClient();
            StripeErrorLog log = new StripeErrorLog();
            log.Error = "mail send";
            log.Message = EmailTo;
            db.StripeErrorLogs.Add(log);
            db.SaveChanges();
            mail.From = new MailAddress(FromAddress, "Aircall System");
            mail.Subject = Subject;
            
            var tos = EmailTo.Split((";").ToCharArray());
            foreach (var item in tos)
            {
                if (!string.IsNullOrWhiteSpace(item))
                {
                    mail.To.Add(EmailTo);
                }
            }
            if (!string.IsNullOrWhiteSpace(CCEmail))
            {
                var ccs = CCEmail.Split((";").ToCharArray());
                foreach (var item in ccs)
                {
                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        mail.CC.Add(item);
                    }
                }
            }
            mail.Body = Body;
            mail.Priority = MailPriority.High;
            mail.IsBodyHtml = true;
            if (!string.IsNullOrWhiteSpace(filename))
            {
                Attachment pdf = new Attachment(filename);
                mail.Attachments.Add(pdf);
            }
            client.Port = int.Parse(Utilities.GetSiteSettingValue("SMTPPort", db));
            client.EnableSsl = Convert.ToBoolean(Utilities.GetSiteSettingValue("SMTPSSL", db));
            if (String.IsNullOrEmpty(Utilities.GetSiteSettingValue("SMTPUser", db)) && String.IsNullOrEmpty(Utilities.GetSiteSettingValue("SMTPPassword", db)))
            {
                client.UseDefaultCredentials = true;
            }
            else
            {
                client.UseDefaultCredentials = false;
            }
            client.Host = Utilities.GetSiteSettingValue("SMTPHost", db);
            client.Credentials = new NetworkCredential(Utilities.GetSiteSettingValue("SMTPUser", db), Utilities.GetSiteSettingValue("SMTPPassword", db));
            try
            {
                await client.SendMailAsync(mail);
            }
            catch (Exception ex)
            {
                StripeErrorLog log1 = new StripeErrorLog();
                log1.Error = ex.Message;
                log1.Message = EmailTo;
                db.StripeErrorLogs.Add(log1);
                db.SaveChanges();
            }
        }
        public static string GetSiteSettingValue(string settingName, Aircall_DBEntities1 db)
        {
            try
            {
                return db.SiteSettings.Where(x => x.Name == settingName).FirstOrDefault().Value;
            }
            catch (Exception ex)
            {
                
            }
            return "";
        }
        public static string GetPartDetails(int id, Aircall_DBEntities1 db)
        {
            return db.Parts.Where(x => x.Id == id).Select(x => new
            {
                Name = (x.IsDeleted ? "NA" : x.Name + " " + x.Size ?? "")
            }).FirstOrDefault().Name;
        }
        public static bool CheckTimeValidation(string RequestedTime,int index)
        {
            TimeSpan CurrentTimeSpan = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            string[] slots = RequestedTime.Split('-');
            TimeSpan SelectedTimeSpan = new TimeSpan();
            SelectedTimeSpan = DateTime.ParseExact(slots[index].ToString().Trim(), "hh:mm tt", CultureInfo.CurrentCulture).TimeOfDay;
            if (CurrentTimeSpan.Ticks >= SelectedTimeSpan.Ticks)
                return false;
            else
                return true;
        }
    }
}