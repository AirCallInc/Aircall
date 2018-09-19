using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using api.Models;

namespace api.ViewModel
{
    public class ServiceUnits
    {
        public int Id { get; set; }
        public string UnitName { get; set; }
        public bool IsCompleted { get; set; }
        public List<ServiceUnitParts> ServiceUnitParts { get; set; }
    }

    public class ServiceUnitsResponse
    {
        public int Id { get; set; }
        public string UnitName { get; set; }
        public bool IsCompleted { get; set; }
        public List<ServiceUnitPartsResponse> ServiceUnitParts { get; set; }
    }

    public class ServiceUnitParts
    {
        public int PartId { get; set; }
        public string PartName { get; set; }
        public int Qty { get; set; }
    }
    public class ServiceUnitPartsResponse
    {
        public int PartId { get; set; }
        public string PartName { get; set; }
        public int Qty { get; set; }
    }

    public class RequestedParts
    {
        public int UnitId { get; set; }
        public Nullable<int> PartId { get; set; }
        public Nullable<int> Quantity { get; set; }
        public string PartName { get; set; }
        public string PartSize { get; set; }
        public string Description { get; set; }
        public string EmpNotes { get; set; }
    }
    public class ReportResponce
    {
        public long Id { get; set; }
        public string ServiceReportNumber { get; set; }
        public string ReportDate { get; set; }
        public string EmployeeNotes { get; set; }
    }
    public class EmpPartRequestModel
    {
        public int? EmployeePartRequestId { get; set; }
        public int ClientID { get; set; }
        public int EmployeeId { get; set; }
        public int AddressId { get; set; }
        public string EmpNotes { get; set; }
        public List<RequestedParts> RequestedParts { get; set; }
    }

    public class RequestedPartsResponse
    {
        public int UnitId { get; set; }
        public string PartName { get; set; }
        public string PartSize { get; set; }

    }

    public class ServiceReporRequestModel
    {
        public int EmployeeId { get; set; }
        public int ServiceId { get; set; }
        public string ScheduleStartTime { get; set; }
        public string ScheduleEndTime { get; set; }
        public bool IsWorkNotDone { get; set; }
        public string WorkPerformed { get; set; }
        public string Recommendationsforcustomer { get; set; }
        public string BillingType { get; set; }
        public List<string> EmailToClient { get; set; }
        public string CCEmail { get; set; }
        public List<RequestedParts> RequestedParts { get; set; }
        public string EmployeeNotes { get; set; }
        public List<ServiceUnits> Units { get; set; }
        public List<string> Images { get; set; }
        public int? ClientId { get; set; }
        public Nullable<decimal> StartLatitude { get; set; }
        public Nullable<decimal> StartLongitude { get; set; }
        public Nullable<decimal> EndLatitude { get; set; }
        public Nullable<decimal> EndLongitude { get; set; }
        public List<string> ClientEmails { get; set; }
    }

    public class ServiceReporModel
    {
        public string ReportNumber { get; set; }
        public string ClientName { get; set; }
        public string AccountNo { get; set; }
        public string ClientSignature { get; set; }
        public int EmployeeId { get; set; }
        public int ServiceId { get; set; }
        public string ScheduleStartTime { get; set; }
        public string ScheduleEndTime { get; set; }
        public string WorkStartedTime { get; set; }
        public string WorkCompletedTime { get; set; }
        public string ExtraTime { get; set; }
        public bool IsWorkNotDone { get; set; }
        public string WorkPerformed { get; set; }
        public string BillingType { get; set; }
        public string ReportDate { get; set; }
        public string Recommendationsforcustomer { get; set; }
        public string PurposeOfVisit { get; set; }
        public List<string> EmailToClient { get; set; }
        public string CCEmail { get; set; }
        public List<RequestedPartsResponse> RequestedParts { get; set; }
        public string EmployeeNotes { get; set; }
        public List<ServiceUnitsResponse> Units { get; set; }
        public List<string> Images { get; set; }
        public bool IsDifferentTime { get; set; }
        public string AssignedTotalTime { get; set; }
        public string Company { get; set; }
    }

    public class CompletedServiceReportModel
    {
        public long ReportId { get; set; }
        public string ServiceReportNumber { get; set; }
        public string ClientName { get; set; }
        public string PurposeOfVisit { get; set; }
        public string PhoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public string OfficeNumber { get; set; }
        public string WorkCompletedTime { get; set; }
        public DateTime? ReportDateTime { get; set; }
        public DateTime? StatusChangeDate { get; set; }

        public string Status { get; set; }
    }
    public class CompletedServiceModel
    {
        public long ServiceId { get; set; }
        public string UnitName { get; set; }
        public string ClientName { get; set; }
        public DateTime? ServiceDate { get; set; }
        public string ServiceTime { get; set; }
        public string Status { get; set; }
        public bool IsMatched { get; set; }
        public int UnitId { get; set; }
        public string UnitType { get; set; }
        public Nullable<System.DateTime> ManufactureDate { get; set; }
        public bool UnitAge { get; set; }
    }
    public class ServicePartModel
    {
        public long ServiceId { get; set; }
        public string ClientName { get; set; }
        public string ServiceNumber { get; set; }
        public DateTime? ServiceDate { get; set; }
        public List<PartDetails> Parts { get; set; }
        public string Status { get; set; }        
        public ICollection<ServicePartList> ServicePartLists { get; set; }
    }
    public class PartDetails
    {
        public string PartName { get; set; }
        public int Quantity { get; set; }
    }
    public class RatingReviewListModel
    {
        public decimal AverageRating { get; set; }
        public List<RatingReviewModel> RatingReview { get; set; }
    }
    public class RatingReviewModel
    {
        public int RatingReviewId { get; set; }
        public string ClientName { get; set; }
        public string ServiceCaseNumber { get; set; }
        public string PackageName { get; set; }
        public decimal Rating { get; set; }
        public string Review { get; set; }
        public string EmpNotes { get; set; }
    }
    public class SubmitEmployeeNotesModel
    {
        public int RatingReviewId { get; set; }
        public string EmployeeNotes { get; set; }
        public int EmployeeId { get; set; }
    }

    public class OrderPartModel
    {
        public int PartId { get; set; }
        public int Quantity { get; set; }
    }
    public class OrderModel
    {
        public int ClientId { get; set; }
        public string Address { get; set; }
        public int City { get; set; }
        public int State { get; set; }
        public string ZipCode { get; set; }
        public string Email { get; set; }
        public string ChargeBy { get; set; }
        public string CardType { get; set; }
        public string NameOnCard { get; set; }
        public string CardNumber { get; set; }
        public int CVV { get; set; }
        public short ExpiryMonth { get; set; }
        public int ExpiryYear { get; set; }
        public string Recommendation { get; set; }
        public bool EmailToClientEmail { get; set; }
        public string CCEmail { get; set; }
        public string OrderItems { get; set;}
        public string EmployeeId { get; set; }
    }
    public class SalesVisitRequestModel
    {
        public string ClientName { get; set; }
        public string RequestedDate { get; set; }
    }
    public class SubmitSalesVisitRequestModel
    {
        public int ClientId { get; set; }
        public string EmployeeNotes { get; set; }
        public int EmployeeId { get; set; }
        public int AddressId { get; set; }
    }

    public class ClientListModel
    {
        public int Id { get; set; }
        public string ClientName { get; set; }
        public string Email { get; set; }
    }
    public class ClientUnitsModel
    {
        public int UnitId { get; set; }
        public string UnitName { get; set; }
    }
    public class TimeAndPurposeModel
    {
        public string ServiceSlot1 { get; set; }
        public string ServiceSlot2 { get; set; }
        public List<string> PurposeOfVisit { get; set; }
    }

    public class SubmitRequestService
    {
        public int EmployeeId { get; set; }
        public int ClientId { get; set; }
        public int AddressId { get; set; }
        public List<int> UnitIds { get; set; }
        public string TimeSlot { get; set; }
        public DateTime ServiceDate { get; set; }
        public string PurposeOfVisit { get; set; }
        public string Notes { get; set; }
    }

    public class RequstedPartList
    {
        public int EmployeePartRequestId { get; set; }
        public string ClientName { get; set; }
        public string PartId { get; set; }
        public string PartName { get; set; }
        public string Status { get; set; }
        public string RequestedDate { get; set; }
        public string Quantity { get; set; }
        public ICollection<EmployeePartRequest> EmployeePartRequests { get; set; }
    }
}