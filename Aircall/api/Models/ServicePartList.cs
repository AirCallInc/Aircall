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
    
    public partial class ServicePartList
    {
        public int Id { get; set; }
        public long ServiceId { get; set; }
        public Nullable<int> UnitId { get; set; }
        public Nullable<int> PartId { get; set; }
        public Nullable<int> PartQuantity { get; set; }
        public Nullable<int> UsedQuantity { get; set; }
        public Nullable<bool> IsUsed { get; set; }
    
        public virtual ClientUnit ClientUnit { get; set; }
        public virtual Part Part { get; set; }
        public virtual Service Service { get; set; }
    }
}