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
    
    public partial class uspa_GetMIssingInventoryReport_Result
    {
        public int Id { get; set; }
        public string EmployeeName { get; set; }
        public string PartName { get; set; }
        public Nullable<int> RequestedQuantity { get; set; }
        public Nullable<System.DateTime> ServiceDate { get; set; }
        public Nullable<long> ReportId { get; set; }
        public string ServiceReportNumber { get; set; }
        public System.DateTime AddedDate { get; set; }
        public string Status { get; set; }
        public Nullable<decimal> SellingPrice { get; set; }
    }
}