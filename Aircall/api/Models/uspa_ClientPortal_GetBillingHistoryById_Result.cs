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
    
    public partial class uspa_ClientPortal_GetBillingHistoryById_Result
    {
        public int Id { get; set; }
        public Nullable<decimal> PurchasedAmount { get; set; }
        public Nullable<System.DateTime> TransactionDate { get; set; }
        public string TransactionId { get; set; }
        public string ServiceCaseNumber { get; set; }
        public string ClientUnitIds { get; set; }
        public string BillingType { get; set; }
        public string CheckNumbers { get; set; }
        public string CheckAmounts { get; set; }
        public string CheckAmounts1 { get; set; }
        public string CheckNumbers1 { get; set; }
        public string PO { get; set; }
        public string PackageName { get; set; }
        public string InvoiceNumber { get; set; }
        public Nullable<int> ClientId { get; set; }
        public string ClientName { get; set; }
        public string Company { get; set; }
        public Nullable<int> ClientId1 { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public string OrderNumber { get; set; }
        public Nullable<int> OrderId { get; set; }
        public Nullable<bool> IsPaid { get; set; }
        public string Reason { get; set; }
        public string AccountingNotes { get; set; }
        public string PaymentMethod { get; set; }
        public string CardNumber { get; set; }
    }
}
