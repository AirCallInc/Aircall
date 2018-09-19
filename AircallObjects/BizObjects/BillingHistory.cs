using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace BizObjects
{
    public class BillingHistory : BizObject
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string ClientUnitIds { get; set; }
        public int ClientUnitSubscriptionId { get; set; }
        public int PaidMonths { get; set; }
        public int OrderId { get; set; }
        public string ServiceCaseNumber { get; set; }
        public string PackageName { get; set; }
        public string BillingType { get; set; }
        public decimal OriginalAmount { get; set; }
        public decimal PurchasedAmount { get; set; }
        public decimal PartnerSalesCommisionAmount { get; set; }
        public string BillingFirstName { get; set; }
        public string BillingLastName { get; set; }
        public string Company { get; set; }
        public string BillingAddress { get; set; }
        public int BillingCity { get; set; }
        public int BillingState { get; set; }
        public string BillingZipcode { get; set; }
        public string BillingPhoneNumber { get; set; }
        public string BillingMobileNumber { get; set; }
        public bool IsSpecialOffer { get; set; }
        public string TransactionId { get; set; }
        public DateTime TransactionDate { get; set; }
        public bool IsDeleted { get; set; }
        public int AddedBy { get; set; }
        public DateTime AddedDate { get; set; }
        public int DeletedBy { get; set; }
        public DateTime DeletedDate { get; set; }
        public bool IsPaid { get; set; }
        public string failcode { get; set; }
        public string faildesc { get; set; }
        public int CardId { get; set; }
        public string CheckNumbers { get; set; }
        public string CheckAmounts { get; set; }
        public string PO { get; set; }
        public string TransactionStatus { get; set; }
        public string ResponseReasonDescription { get; set; }
        public int ResponseCode { get; set; }

        #region "Constructors"
        public BillingHistory()
        {
        }
        public BillingHistory(ref DataRow drRow)
        {
            _LoadFromDb(ref drRow);
        }
        #endregion

        #region "Methods overridden of BizObjects"
        protected override void _LoadFromDb(ref DataRow drRow)
        {
            DBUtils dbUtil = new DBUtils(drRow);
        }

        public override void AddInsertParams(ref ZWT.DbLib.IDataLib dataLib)
        {
            dataLib.AddParameter("@ClientId", SqlDbType.Int,this.ClientId);
            if (this.ClientUnitIds != "")
            {
                dataLib.AddParameter("@ClientUnitIds", SqlDbType.Int, this.ClientUnitIds);
            }
            dataLib.AddParameter("@OrderId", SqlDbType.Int,this.OrderId);
            dataLib.AddParameter("@ServiceCaseNumber", SqlDbType.NVarChar,this.ServiceCaseNumber);
            dataLib.AddParameter("@PackageName", SqlDbType.NVarChar,this.PackageName);
            dataLib.AddParameter("@BillingType", SqlDbType.NVarChar,this.BillingType);
            dataLib.AddParameter("@OriginalAmount", SqlDbType.Decimal ,this.OriginalAmount);
            dataLib.AddParameter("@PurchasedAmount", SqlDbType.Decimal, this.PurchasedAmount);
            dataLib.AddParameter("@PartnerSalesCommisionAmount", SqlDbType.Decimal, this.PartnerSalesCommisionAmount);
            dataLib.AddParameter("@BillingFirstName", SqlDbType.NVarChar, this.BillingFirstName);
            dataLib.AddParameter("@BillingLastName", SqlDbType.NVarChar, this.BillingLastName);
            dataLib.AddParameter("@Company", SqlDbType.NVarChar, this.Company);
            dataLib.AddParameter("@BillingAddress", SqlDbType.NVarChar,this.BillingAddress);
            dataLib.AddParameter("@BillingCity", SqlDbType.Int,this.BillingCity);
            dataLib.AddParameter("@BillingState", SqlDbType.Int,this.BillingState);
            dataLib.AddParameter("@BillingZipcode", SqlDbType.NVarChar,this.BillingZipcode);
            dataLib.AddParameter("@BillingPhoneNumber", SqlDbType.NVarChar,this.BillingPhoneNumber);
            dataLib.AddParameter("@BillingMobileNumber", SqlDbType.NVarChar,this.BillingMobileNumber);
            dataLib.AddParameter("@IsSpecialOffer", SqlDbType.Bit,this.IsSpecialOffer);
            dataLib.AddParameter("@TransactionId", SqlDbType.NVarChar,this.TransactionId);
            dataLib.AddParameter("@TransactionDate", SqlDbType.DateTime,this.TransactionDate);
            dataLib.AddParameter("@AddedBy", SqlDbType.Int,this.AddedBy);
            dataLib.AddParameter("@AddedDate", SqlDbType.DateTime,this.AddedDate);
            dataLib.AddParameter("@IsPaid", SqlDbType.Bit, this.IsPaid);
            dataLib.AddParameter("@failcode", SqlDbType.NVarChar, this.failcode);
            dataLib.AddParameter("@faildesc", SqlDbType.NVarChar, this.faildesc);
            //dataLib.AddParameter("@StripeNextPaymentDate", SqlDbType.DateTime, this.StripeNextPaymentDate);
        }

        public override void AddUpdateParams(ref ZWT.DbLib.IDataLib dataLib)
        {
            dataLib.AddParameter("@Id", SqlDbType.Int,this.Id);
            dataLib.AddParameter("@ClientId", SqlDbType.Int,this.ClientId);
            dataLib.AddParameter("@ClientUnitIds", SqlDbType.Int,this.ClientUnitIds);
            dataLib.AddParameter("@OrderId", SqlDbType.Int,this.OrderId);
            dataLib.AddParameter("@ServiceCaseNumber", SqlDbType.NVarChar,this.ServiceCaseNumber);
            dataLib.AddParameter("@PackageName", SqlDbType.NVarChar,this.PackageName);
            dataLib.AddParameter("@BillingType", SqlDbType.NVarChar,this.BillingType);
            dataLib.AddParameter("@OriginalAmount", SqlDbType.Decimal, this.OriginalAmount);
            dataLib.AddParameter("@PurchasedAmount", SqlDbType.Decimal, this.PurchasedAmount);
            dataLib.AddParameter("@PartnerSalesCommisionAmount", SqlDbType.Decimal, this.PartnerSalesCommisionAmount);
            dataLib.AddParameter("@BillingAddress", SqlDbType.NVarChar,this.BillingAddress);
            dataLib.AddParameter("@BillingCity", SqlDbType.Int,this.BillingCity);
            dataLib.AddParameter("@BillingState", SqlDbType.Int,this.BillingState);
            dataLib.AddParameter("@BillingZipcode", SqlDbType.NVarChar,this.BillingZipcode);
            dataLib.AddParameter("@BillingPhoneNumber", SqlDbType.NVarChar,this.BillingPhoneNumber);
            dataLib.AddParameter("@BillingMobileNumber", SqlDbType.NVarChar,this.BillingMobileNumber);
            dataLib.AddParameter("@IsSpecialOffer", SqlDbType.Bit,this.IsSpecialOffer);
            dataLib.AddParameter("@TransactionId", SqlDbType.NVarChar,this.TransactionId);
            dataLib.AddParameter("@TransactionDate", SqlDbType.DateTime,this.TransactionDate);
            dataLib.AddParameter("@IsDeleted", SqlDbType.Bit,this.IsDeleted);
            dataLib.AddParameter("@AddedBy", SqlDbType.Int,this.AddedBy);
            dataLib.AddParameter("@AddedDate", SqlDbType.DateTime,this.AddedDate);
            dataLib.AddParameter("@DeletedBy", SqlDbType.Int,this.DeletedBy);
            dataLib.AddParameter("@DeletedDate", SqlDbType.DateTime,this.DeletedDate);

        }

        public override void AddSearchParams(ref ZWT.DbLib.IDataLib dataLib)
        {

        }
        #endregion

    }
}