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
    
    public partial class uspa_Services_CheckApprovalEmailUrl_Result
    {
        public long Id { get; set; }
        public string ServiceCaseNumber { get; set; }
        public string EmployeeName { get; set; }
        public Nullable<int> EmployeeId { get; set; }
        public Nullable<System.DateTime> ScheduleDate { get; set; }
        public string ScheduleStartTime { get; set; }
        public string ScheduleEndTime { get; set; }
        public string ServiceUnits { get; set; }
        public string Image { get; set; }
        public string TimeSlot { get; set; }
        public Nullable<int> ServiceTimeForFirstUnit { get; set; }
        public Nullable<int> ServiceTimeForAdditionalUnits { get; set; }
        public string PurposeOfVisit { get; set; }
        public string Address { get; set; }
        public Nullable<bool> IsRequestedService { get; set; }
    }
}