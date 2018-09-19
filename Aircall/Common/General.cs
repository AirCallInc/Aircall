using Services;
using Stripe;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Aircall.Common
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
                    {
                        return (T)field.GetValue(null);
                    }

                }
                else
                {
                    if (field.Name == description)
                        return (T)field.GetValue(null);
                }
            }
            throw new ArgumentException("Not found.", "description");
            // or return default(T);
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

        public static int GetEnumValue(this Enum value)
        {
            return Convert.ToInt32(value);
        }

        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        public static string GetUnitStatus(int status)
        {
            string StrStatus = string.Empty;

            if (status == General.UnitStatus.Serviced.GetEnumValue())
            {
                StrStatus = General.UnitStatus.Serviced.GetEnumDescription();
            }
            else if (status == General.UnitStatus.ServiceSoon.GetEnumValue())
            {
                StrStatus = General.UnitStatus.ServiceSoon.GetEnumDescription();
            }
            else if (status == General.UnitStatus.NeedRepair.GetEnumValue())
            {
                StrStatus = General.UnitStatus.NeedRepair.GetEnumDescription();
            }
            return StrStatus;
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
    }
    public class ScheduledServiceResult
    {
        public string Id { get; set; }
        public string ScheduleCDate { get; set; }
        public string ScheduleStartTime { get; set; }
        public string ScheduleEndTime { get; set; }
        public string PurposeMessage { get; set; }
        public string Status { get; set; }
        public int ServiceCount { get; set; }
    }

    public class LoginModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public int RoleId { get; set; }
        public string Username { get; set; }
        public string Image { get; set; }
        public string AffiliateId { get; set; }
    }

    public class SubscriptionSearch
    {
        public string ClientName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
        public string PaymentMethod { get; set; }
    }

    public class General
    {
        public enum UserRoles
        {
            [Description("Super Admin")]
            SuperAdmin = 1,
            [Description("Admin")]
            AdminRole = 2,
            [Description("Warehouse User")]
            WarehouseUser = 3,
            [Description("Client")]
            Client = 4,
            [Description("Employee")]
            Employee = 5,
            [Description("Partner")]
            Partner = 6
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

        public enum PaymentStatus
        {
            Received = 1,
            NotReceived = 2,
            PaymentFailed = 3
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

        public enum UnitPaymentTypes
        {
            [Description("Received")]
            Received = 200,
            [Description("NotReceived")]
            NotReceived = 400,
            [Description("Failed")]
            PaymentFailed = 500
        }

        public enum SplitType
        {
            [Description("Heating")]
            Heating = 1,
            [Description("Cooling")]
            Cooling = 2
        }

        public enum UnitExtraInfoType
        {
            Filter = 1,
            Fuses = 2
        }

        public enum PriorityArea
        {
            Priority1 = 1,
            Priority2 = 2
        }
        public enum PurposeOfVisit
        {
            [Description("Repair")]
            [Display(Name = "Repair (Mon-Fri)")]
            Repairing = 0,
            [Description("Emergency")]
            [Display(Name = "Emergency 7 days a week (Mon-Sun) $125")]
            Emergency = 1,
            [Description("Continuing Previous Work")]
            [Display(Name = "Continuing Previous Work (Mon-Fri)")]
            ContinuingPreviousWork = 2,
            [Description("Maintenance Services")]
            [Display(Name = "Maintenance Service (Mon-Fri)")]
            Maintenance = 3
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
            LateCancelled=21
        }

        public enum UnitSubscriptionStatus
        {
            [Description("Paid")]
            Paid = 1,
            [Description("UnPaid")]
            UnPaid = 2,
            [Description("Fail")]
            Fail = 3
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

        public enum ReportBilling
        {
            [Description("Fixed Cost Contract")]
            FixedCostContract = 1,
            [Description("C.O.D")]
            COD = 2,
            [Description("Emergency")]
            Emergency = 3
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
        public enum InventoryType
        {
            [Description("Inventory")]
            Inventory = 1,
            [Description("Non Inventory")]
            NonInventory = 2
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
        public enum NotificationStatus
        {
            [Description("UnRead")]
            UnRead = 1,
            [Description("Read")]
            Read = 2
        }

        public enum TicketTypes
        {
            [Description("Inquiry")]
            Inquiry = 1,
            [Description("Sales Commission Issue")]
            SalesCommissionIssue = 2,
            [Description("Other")]
            Other = 3
        }

        public enum ChargeBy
        {
            [Description("Check")]
            Check = 1,
            [Description("CC on File")]
            CCOnFile = 2,
            [Description("New CC")]
            NewCC = 3
        }
        public static bool CheckTimeValidation(string RequestedTime, int index)
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
        public static int Slottime1(string[] slots1, string[] slots2, Plan p, string lunchtime)
        {
            var lunchtimes = lunchtime.Split(("-").ToArray()).Select(x => new { time = DateTime.Parse(x) }).Select(x => x.time);

            DateTime dtlunchstart = lunchtimes.First();
            DateTime dtlunchend = lunchtimes.Last();

            var lunchMinut = dtlunchend.Subtract(dtlunchstart).TotalMinutes;
            int noofunit = 0;
            if (slots2.Count() > 0 && slots1.Count() > 0)
            {
                DateTime dtStart1 = DateTime.Parse(slots1.First());
                DateTime dtEnd1 = DateTime.Parse(slots1.Last());

                var st2 = slots2.Select(x => new { SlotDt = DateTime.Parse(x) }).OrderBy(x => x.SlotDt).Select(x => new { time = x.SlotDt.ToString("HH:mm") }).Select(x => x.time).ToArray();
                DateTime dtStart2 = DateTime.Parse(st2.First());
                DateTime dtEnd2 = DateTime.Parse(st2.Last());

                TimeSpan t = dtEnd2.Subtract(dtStart1);

                int TotalMinutes = int.Parse(t.TotalMinutes.ToString());

                noofunit = (int)((TotalMinutes - (p.Drivetime + p.ServiceTimeForFirstUnit + lunchMinut)) / p.ServiceTimeForAdditionalUnits) + 1;
                noofunit = (noofunit < 0 ? 0 : noofunit);
            }
            else
            {
                var st2 = slots2.Select(x => new { SlotDt = DateTime.Parse(x) }).OrderBy(x => x.SlotDt).Select(x => new { time = x.SlotDt.ToString("HH:mm") }).Select(x => x.time).ToArray();
                DateTime dtStart2 = DateTime.Parse(st2.First());
                DateTime dtEnd2 = DateTime.Parse(st2.Last());
                TimeSpan t = dtEnd2.Subtract(dtStart2);

                int TotalMinutes = int.Parse(t.TotalMinutes.ToString());

                //noofunit = (int)((TotalMinutes - (p.Drivetime + p.ServiceTimeForFirstUnit)) / p.ServiceTimeForAdditionalUnits) + 1;
                //noofunit = (noofunit < 0 ? 0 : noofunit);
                if ((p.Drivetime + p.ServiceTimeForFirstUnit) < TotalMinutes)
                {
                    noofunit = (int)((TotalMinutes - (p.Drivetime + p.ServiceTimeForFirstUnit)) / p.ServiceTimeForAdditionalUnits) + 1;
                    noofunit = (noofunit < 0 ? 0 : noofunit);
                }
                else
                {
                    noofunit = 0;
                }
            }
            return noofunit;
        }
        public static string GetNotificationMessage(string Name)
        {
            INotificationMasterService objNotificationMasterService = ServiceFactory.NotificationMasterService;
            string Message = string.Empty;
            DataTable dtNotification = new DataTable();
            objNotificationMasterService.GetNotificationByName(Name, ref dtNotification);
            if (dtNotification.Rows.Count > 0)
            {
                Message = dtNotification.Rows[0]["Message"].ToString();
            }
            return Message;
        }

        public static StripeResponse StripeCharge(bool isSpecial, string StripePlanId, string CustomerId, string CardId, int Amount, string Description, string PlanName)
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

                    //StripeInvoiceService inv = new StripeInvoiceService();
                    //var inv1 = inv.Upcoming(CustomerId, new StripeUpcomingInvoiceOptions() { SubscriptionId = sr.TransactionId });
                    //sr.StripeNextPaymentDate = inv1.PeriodEnd.Date;
                }
                catch (StripeException ex)
                {
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
                    sr.ex = null;
                    sr.PaymentStatus = UnitPaymentTypes.Received.GetEnumDescription();
                }
                catch (StripeException ex)
                {
                    sr.ErrorMessage = ex.StripeError.Error;
                    sr.ex = ex;
                    sr.TransactionId = "";
                    sr.PaymentStatus = UnitPaymentTypes.PaymentFailed.GetEnumDescription();
                }
            }
            return sr;
        }

        public static string EncryptVerification(string clearText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
            0x49,
            0x76,
            0x61,
            0x6e,
            0x20,
            0x4d,
            0x65,
            0x64,
            0x76,
            0x65,
            0x64,
            0x65,
            0x76
        });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        public static string Decrypt(string cipherText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
            0x49,
            0x76,
            0x61,
            0x6e,
            0x20,
            0x4d,
            0x65,
            0x64,
            0x76,
            0x65,
            0x64,
            0x65,
            0x76
        });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        public static string GetSitesettingsValue(string SettingName)
        {
            ISiteSettingService objSettings = ServiceFactory.SiteSettingService;
            DataTable dt = new DataTable();
            objSettings.GetSiteSettingByName(SettingName, ref dt);
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0]["Value"].ToString();
            }
            return "";
        }

    }
}