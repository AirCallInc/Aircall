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
    
    public partial class uspa_Services_GetAllCompletedService_Result
    {
        public long Id { get; set; }
        public string ServiceCaseNumber { get; set; }
        public string ClientName { get; set; }
        public string EmployeeName { get; set; }
        public Nullable<System.DateTime> ScheduleDate { get; set; }
        public double Ratings { get; set; }
        public System.DateTime RatingDate { get; set; }
        public double EmpAverageRating { get; set; }
        public System.DateTime NotesAddedDate { get; set; }
        public double RatingsReport { get; set; }
        public string ServiceReportNumber { get; set; }
        public long ReportId { get; set; }
        public int ClientId { get; set; }
    }
}
