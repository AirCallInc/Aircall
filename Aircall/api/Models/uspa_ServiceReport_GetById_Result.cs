//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace api.Models
{
    using System;
    
    public partial class uspa_ServiceReport_GetById_Result
    {
        public long Id { get; set; }
        public string ServiceReportNumber { get; set; }
        public string BillingType { get; set; }
        public string ClientName { get; set; }
        public string Company { get; set; }
        public string Email { get; set; }
        public Nullable<long> ServiceId { get; set; }
        public int ClientId { get; set; }
        public string ClientAddress { get; set; }
        public string PurposeOfVisit { get; set; }
        public int AddressID { get; set; }
        public Nullable<long> ServiceId1 { get; set; }
        public Nullable<System.DateTime> ScheduleDate { get; set; }
        public Nullable<int> AssignedTotalTime { get; set; }
        public string ScheduleStartTime { get; set; }
        public string ScheduleEndTime { get; set; }
        public string EmployeeName { get; set; }
        public string WorkStartedTime { get; set; }
        public string WorkCompletedTime { get; set; }
        public Nullable<int> ExtraTime { get; set; }
        public string ReportUnits { get; set; }
        public string EmployeeNotes { get; set; }
        public string WorkPerformed { get; set; }
        public string MaterialUsed { get; set; }
        public string MaterialNotUsed { get; set; }
        public string Recommendationsforcustomer { get; set; }
        public string ClientSignature { get; set; }
        public string CCEmail { get; set; }
    }
}