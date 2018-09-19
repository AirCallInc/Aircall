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
    
    public partial class Order
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Order()
        {
            this.BillingHistories = new HashSet<BillingHistory>();
            this.ClientUnitSubscriptions = new HashSet<ClientUnitSubscription>();
            this.OrderItems = new HashSet<OrderItem>();
        }
    
        public int Id { get; set; }
        public string OrderNumber { get; set; }
        public string OrderType { get; set; }
        public int ClientId { get; set; }
        public string Description { get; set; }
        public decimal OrderAmount { get; set; }
        public string ChargeBy { get; set; }
        public string ChequeNo { get; set; }
        public string BankName { get; set; }
        public string RoutingNo { get; set; }
        public Nullable<System.DateTime> ChequeDate { get; set; }
        public string PONo { get; set; }
        public string NameOnCard { get; set; }
        public string CardType { get; set; }
        public string CardNumber { get; set; }
        public Nullable<short> ExpirationMonth { get; set; }
        public Nullable<int> ExpirationYear { get; set; }
        public string CCEmail { get; set; }
        public Nullable<bool> IsEmailToClient { get; set; }
        public string CustomerRecommendation { get; set; }
        public string ClientSignature { get; set; }
        public string AccountingNotes { get; set; }
        public string ChqueImageFront { get; set; }
        public string ChequeImageBack { get; set; }
        public Nullable<int> AddedBy { get; set; }
        public Nullable<int> AddedByType { get; set; }
        public System.DateTime AddedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<int> UpdatedByType { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<int> DeletedBy { get; set; }
        public Nullable<int> DeletedByType { get; set; }
        public Nullable<System.DateTime> DeletedDate { get; set; }
        public Nullable<decimal> PricePerMonth { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BillingHistory> BillingHistories { get; set; }
        public virtual Client Client { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ClientUnitSubscription> ClientUnitSubscriptions { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}