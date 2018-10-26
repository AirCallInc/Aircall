using api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Script.Serialization;

namespace api.ViewModel
{
    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string DeviceType { get; set; }
        public string DeviceToken { get; set; }
        public Nullable<decimal> Latitude { get; set; }
        public Nullable<decimal> Longitude { get; set; }
    }


    public class CommonRequest
    {
        public string Email { get; set; }
        public int ClientId { get; set; }
        public int PlanTypeId { get; set; }
        public int PlanId { get; set; }
        public int PartId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public Nullable<int> AddressId { get; set; }
        public int UnitId { get; set; }
        public int? ServiceId { get; set; }
        public int? RequestedServiceId { get; set; }
        public string PartType { get; set; }
        public int? PartTypeId { get; set; }
        public int PageId { get; set; }
        public bool ShowDeleted { get; set; }
        public string ServiceCaseNumber { get; set; }
        public Nullable<DateTime> LastCallDateTime { get; set; }
        public Nullable<int> PageNumber { get; set; }
        public int pageSize { get; set; }
        public string DeviceType { get; set; }
        public string DeviceToken { get; set; }
        public int? NotificationId { get; set; }
        public int EmployeeId { get;  set; }
        public int? BillingId { get; set; }
    }

    public class NotificationRequest
    {
        public int ClientId { get; set; }
        public int CardId { get; set; }
        public int UnitId { get; set; }
        public int NotificationId { get; set; }
        public int ServiceId { get; set; }
        public bool Status { get; set; }
    }

    public class NotificationListModel
    {
        public int? ClientId { get; set; }
        public long NotificationId { get; set; }
        public long? CommonId { get; set; }
        public string Message { get; set; }
        public int NotificationType { get; set; }
        public string DateTime { get; set; }
        public string ProfileImage { get; set; }
        public DateTime LastAdded { get; set; }
        public string Status { get; set; }
        public string ScheduleDay { get; set; }
        public string ScheduleMonth { get; set; }
        public string ScheduleYear { get; set; }
        public string ScheduleStartTime { get; set; }
        public string ScheduleEndTime { get; set; }
        public bool IsRequested { get; set; }
    }

    public class ClientUnitResponseModel
    {
        public int Id { get; set; }
        public string UnitName { get; set; }
        public string Status { get; set; }
        public string HexColor { get; set; }
        public string PlanName { get; set; }
        public string LastService { get; set; }
        public string UpcomingService { get; set; }
        public string EmpFirstName { get; set; }
        public string EmpLastName { get; set; }
        public int TotalService { get; set; }
        public int RemainingService { get; set; }
        public string UnitAge { get; set; }
        public bool HasPaymentFailedUnit { get; set; }
        public string R { get; set; }
        public string G { get; set; }
        public string B { get; set; }
        public Nullable<DateTime> LastAdded { get; set; }
    }

    public class PastServiceResponseModel
    {
        public long Id { get; set; }
        public string UnitName { get; set; }
        public string PlanName { get; set; }
        public string ServiceCaseNumber { get; set; }
        public string Message { get; set; }
        public Nullable<DateTime> LastAdded { get; set; }
        public bool IsNoShow { get; set; }
        public string Address { get; set; }
    }

    public class ServiceRatingModel
    {
        public int ClientId { get; set; }
        public long ServiceId { get; set; }
        public double Rate { get; set; }
        public string Review { get; set; }
    }

    public class PurposeResponse
    {
        public string ServicePurpose { get; set; }
    }

    public partial class RequestedServiceUnitModel
    {
        public int UnitId { get; set; }
    }

    public class ClientContactModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Message { get; set; }
    }

    public class ServiceRequestModel
    {
        public Nullable<int> EmployeeId { get; set; }
        public Nullable<int> ServiceId { get; set; }
        public int ClientId { get; set; }
        public int AddressId { get; set; }
        public string PurposeOfVisit { get; set; }
        public string ServiceRequestedTime { get; set; }
        public System.DateTime ServiceRequestedOn { get; set; }
        public string Notes { get; set; }
        public string Reason { get; set; }
        public bool IsCancelled { get; set; }
        public List<int> Units { get; set; }
        public long NotificationId { get; set; }
        public bool IsLateReschedule { get; set; }
    }

    public class ServiceRequestListModel
    {
        public int ClientId { get; set; }
        public int ServiceId { get; set; }
        public string PurposeOfVisit { get; set; }
        public string ServiceRequestedTime { get; set; }
        public System.DateTime ServiceRequestedOn { get; set; }
        public string Notes { get; set; }
    }

    public class PaymentRequest
    {
        public int ClientId { get; set; }
        public int EmployeeId { get; set; }
        public string Address { get; set; }
        public int State { get; set; }
        public int City { get; set; }
        public string ZipCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string PhoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public string CardType { get; set; }
        public string NameOnCard { get; set; }
        public string CardNumber { get; set; }
        public int CVV { get; set; }
        public Nullable<int> UnitId { get; set; }
        public Nullable<short> ExpiryMonth { get; set; }
        public Nullable<int> ExpiryYear { get; set; }
        public string CustomerPaymentProfileId { get; set; }
        public string ClientSignature { get;  set; }
        public long NotificationId { get; set; }
        public string PaymentMethod { get; set; }
        public string ChequeImageBack { get; set; }
        public string ChqueImageFront { get; set; }
        public string AccountingNotes { get; set; }
        public DateTime ChequeDate { get; set; }
        public string PONo { get; set; }
        public string ChequeNo { get; set; }
        public string CCEmail { get; set; }
    }

    public class PaymentStatusCheckRequest
    {
        public int ClientId { get; set; }
        public int EmployeeId { get;  set; }
        public int UnitId { get; set; }
    }

    public class SpecialRateModel
    {
        public int ClientId { get; set; }
        public int PlanTypeId { get; set; }
        public string Email { get; set; }
    }

    public class ClientUnitMatchRequestModel
    {
        public int UnitId { get; set; }
        public int EmployeeId { get;  set; }
        public List<ClientUnitMatchModel> Parts { get; set; }
    }
    public class ClientUnitMatchModel
    {
        public string UnitType { get; set; }
        public string ModelNumber { get; set; }
        public string SerialNumber { get; set; }
    }
    public class ClientUnitOptions
    {
        public string SplitType { get; set; }
        public string ModelNumber { get; set; }
        public string SerialNumber { get; set; }
        public string UnitTon { get; set; }
        public int QuantityOfFilter { get; set; }
        public Nullable<System.DateTime> ManufactureDate { get; set; }
        public List<FilterDetails> Filters { get; set; }
        public int QuantityOfFuses { get; set; }
        public List<FuseDetails> FuseTypes { get; set; }
        public Nullable<int> ThermostatTypes { get; set; }
    }

    public class FuseDetails
    {
        public int FuseType { get; set; }
    }

    public class ThermostatDetails
    {
        public int ThermostatType { get; set; }
    }

    public class FilterDetails
    {
        public int size { get; set; }
        public bool LocationOfPart { get; set; }
    }

    public class ClientUnitAddModel
    {
        public int ClientId { get; set; }
        public int UnitId { get; set; }
        public int Qty { get; set; }
        public string UnitName { get; set; }
        public Nullable<System.DateTime> ManufactureDate { get; set; }
        public Nullable<int> PlanTypeId { get; set; }
        public string UnitTon { get; set; }
        public Nullable<int> AddressId { get; set; }
        public Nullable<int> UnitTypeId { get; set; }
        public Nullable<bool> AutoRenewal { get; set; }
        public Nullable<bool> SpecialOffer { get; set; }
        public Nullable<int> Status { get; set; }
        public List<ClientUnitOptions> OptionalInformation { get; set; }
        public bool DeleteOldData { get; set; }
        public int EmployeeId { get; set; }
        public string CurrentPaymentMethod { get; set; }
        public int VisitPerYear { get; set; }
        public decimal PricePerMonth { get; set; }
    }

    public class ClientPaymentModel
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string CardType { get; set; }
        public string NameOnCard { get; set; }
        public string CardNumber { get; set; }
        public string ExpiryMonth { get; set; }
        public int CVV { get; set; }
        public int ExpiryYear { get; set; }
        public Nullable<bool> IsDefaultPayment { get; set; }
        public string CustomerPaymentProfileId { get; set; }
    }

    public class RegisterClientModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Image { get; set; }
        public string MobileNumber { get; set; }
        public string Company { get; set; }
        public string AffiliateId { get; set; }
        public string DeviceType { get; set; }
        public string DeviceToken { get; set; }
        public DateTime AddedDate { get; set; }
        public bool SendDisclosureEmail { get; set; }
    }

    public class ClientProfileModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Image { get; set; }
        public string ProfileImage { get; set; }
        public string PhoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public string OfficeNumber { get; set; }
        public string HomeNumber { get; set; }
        public string Company { get; set; }
    }

    public class EmpCommonModel
    {
        public int NotificationId { get; set; }
        public int EmployeeId { get; set; }
        public int SalesVisitRequestId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int Day { get; set; }
        public int ServiceId { get; set; }
        public int ReportId { get; set; }
        public int UnitId { get; set; }
        public Nullable<decimal> Latitude { get; set; }
        public Nullable<decimal> Longitude { get; set; }
        public string SearchTerm { get; set; }
        public int PageId { get; set; }
        public Nullable<DateTime> LastCallDateTime { get; set; }
        public Nullable<int> PageNumber { get; set; }
        public int pageSize { get; set; }
        public int OrderId { get; set; }
        public Nullable<DateTime> StatDate { get; set; }
        public Nullable<DateTime> EndDate { get; set; }
        public string DeviceType { get; set; }
        public string DeviceToken { get; set; }
        public List<string> ClientEmails { get; set; }
        public string CCEmail { get; set; }
    }

    public class EmpProfileModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Image { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public Nullable<int> StateId { get; set; }
        public string StateName { get; set; }
        public string ZipCode { get; set; }
        public string PhoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public string WorkStartTime { get; set; }
        public string WorkEndTime { get; set; }
        public string WorkingHours { get; set; }
    }

    public class ClientContactInfoModel
    {
        public int Id { get; set; }
        public string MobileNumber { get; set; }
        public string OfficeNumber { get; set; }
        public string HomeNumber { get; set; }
    }

    public class ClientChangePasswordModel
    {
        public int Id { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }

    public class ClientAddressModel
    {
        public int Id { get; set; }
        public Nullable<int> ClientId { get; set; }
        public string Address { get; set; }
        public Nullable<int> State { get; set; }
        public string StateName { get; set; }
        public Nullable<int> City { get; set; }
        public string CityName { get; set; }
        public string ZipCode { get; set; }
        public Nullable<decimal> Latitude { get; set; }
        public Nullable<decimal> Longitude { get; set; }
        public Nullable<bool> IsDefaultAddress { get; set; }
        public bool AllowDelete { get; set; }
        public bool ShowAddressInApp { get; set; }
        public string PhoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public string OfficeNumber { get; set; }
        public string HomeNumber { get; set; }
        public string Email { get; set; }
        public string PDFUrl { get; set; }
        public bool PendingInactive { get; set; }
        public bool? IgnoreDuplicate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class Location
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Geometry
    {
        public Location location { get; set; }
        public string location_type { get; set; }
        public object viewport { get; set; }
    }

    public class Result
    {
        public object address_components { get; set; }
        public string formatted_address { get; set; }
        public Geometry geometry { get; set; }
        public bool partial_match { get; set; }
        public string place_id { get; set; }
        public IList<string> types { get; set; }
    }

    public class Example
    {
        public IList<Result> results { get; set; }
        public string status { get; set; }
    }

    public class ReceiptModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UnitName { get; set; }
        public string Plan { get; set; }
        public decimal Rate { get; set; }
    }

    public class OtherPaymentRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public int? NotificationId { get; set; }
        public int ServiceId { get; set; }
        public int ClientId { get; set; }
        public string Address { get; set; }
        public int State { get; set; }
        public int City { get; set; }
        public string ZipCode { get; set; }
        public string PhoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public string CardType { get; set; }
        public string NameOnCard { get; set; }
        public string CardNumber { get; set; }
        public int CVV { get; set; }
        public Nullable<int> UnitId { get; set; }
        public Nullable<short> ExpiryMonth { get; set; }
        public Nullable<int> ExpiryYear { get; set; }
    }

    public class BillingDetails
    {
        public int Id { get; set; }
        public string PlanName { get; set; }
        public decimal PurchasedAmount { get; set; }
        public string TransactionDate { get; set; }
        public string TransactionId { get; set; }
        public string UnitName { get; set; }
        public string PartName { get; set; }
        public string ServiceCaseNumber { get; set; }
        public List<ordItem> PartsHistory { get; set; }
        public bool IsPaid { get; set; }
        public string Reason { get; set; }
        public string ChargeBy { get; set; }
        public string CardNumber { get; set; }
    }
    public class ordItem
    {
        public int Quantity { get; set; }
        public string PartName { get; set; }
        public string Size { get; set; }
        public decimal Amount { get; set; }
    }
}