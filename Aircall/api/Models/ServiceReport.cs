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
    using System.Collections.Generic;
    
    public partial class ServiceReport
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ServiceReport()
        {
            this.EmployeePartRequestMasters = new HashSet<EmployeePartRequestMaster>();
            this.ServiceReportImages = new HashSet<ServiceReportImage>();
            this.ServiceReportUnits = new HashSet<ServiceReportUnit>();
        }
    
        public long Id { get; set; }
        public string ServiceReportNumber { get; set; }
        public Nullable<long> ServiceId { get; set; }
        public string BillingType { get; set; }
        public string WorkStartedTime { get; set; }
        public string WorkCompletedTime { get; set; }
        public Nullable<bool> IsWorkDone { get; set; }
        public string WorkPerformed { get; set; }
        public string Recommendationsforcustomer { get; set; }
        public string EmployeeNotes { get; set; }
        public string CCEmail { get; set; }
        public Nullable<bool> IsEmailToClient { get; set; }
        public string ClientSignature { get; set; }
        public Nullable<decimal> StartLatitude { get; set; }
        public Nullable<decimal> StartLongitude { get; set; }
        public Nullable<decimal> EndLatitude { get; set; }
        public Nullable<decimal> EndLongitude { get; set; }
        public Nullable<int> AddedBy { get; set; }
        public Nullable<int> AddedByType { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<int> UpdatedByType { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmployeePartRequestMaster> EmployeePartRequestMasters { get; set; }
        public virtual Role Role { get; set; }
        public virtual Role Role1 { get; set; }
        public virtual Service Service { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ServiceReportImage> ServiceReportImages { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ServiceReportUnit> ServiceReportUnits { get; set; }
    }
}
