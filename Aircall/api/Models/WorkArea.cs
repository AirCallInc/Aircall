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
    
    public partial class WorkArea
    {
        public int Id { get; set; }
        public Nullable<int> AreaId { get; set; }
        public Nullable<int> ZipCodeId { get; set; }
    
        public virtual AreaMaster AreaMaster { get; set; }
        public virtual ZipCode ZipCode { get; set; }
    }
}