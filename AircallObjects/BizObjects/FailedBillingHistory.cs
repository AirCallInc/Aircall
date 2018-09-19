using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace BizObjects
{
    public class FailedBillingHistory : BizObject
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int UnitId { get; set; }        
        public string ServiceCaseNumber { get; set; }
        public string PackageName { get; set; }
        public string BillingType { get; set; }
        public decimal OriginalAmount { get; set; }
        public decimal PurchasedAmount { get; set; }
        public decimal PartnerSalesCommisionAmount { get; set; }
        public string BillingAddress { get; set; }
        public int BillingCity { get; set; }
        public int BillingState { get; set; }
        public string BillingZipcode { get; set; }
        public string BillingPhoneNumber { get; set; }
        public string BillingMobileNumber { get; set; }        
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

        #region "Constructors"
        public FailedBillingHistory()
        {
        }
        public FailedBillingHistory(ref DataRow drRow)
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
            if (this.UnitId != 0)
            {
                dataLib.AddParameter("@UnitId", SqlDbType.Int, this.UnitId);
            }
            dataLib.AddParameter("@ServiceCaseNumber", SqlDbType.NVarChar,this.ServiceCaseNumber);
            dataLib.AddParameter("@PackageName", SqlDbType.NVarChar,this.PackageName);
            dataLib.AddParameter("@BillingType", SqlDbType.NVarChar,this.BillingType);
            dataLib.AddParameter("@OriginalAmount", SqlDbType.Decimal ,this.OriginalAmount);
            dataLib.AddParameter("@PurchasedAmount", SqlDbType.Decimal, this.PurchasedAmount);
            dataLib.AddParameter("@PartnerSalesCommisionAmount", SqlDbType.Decimal, this.PartnerSalesCommisionAmount);
            dataLib.AddParameter("@BillingAddress", SqlDbType.NVarChar,this.BillingAddress);
            dataLib.AddParameter("@BillingCity", SqlDbType.Int,this.BillingCity);
            dataLib.AddParameter("@BillingState", SqlDbType.Int,this.BillingState);
            dataLib.AddParameter("@BillingZipcode", SqlDbType.NVarChar,this.BillingZipcode);
            dataLib.AddParameter("@BillingPhoneNumber", SqlDbType.NVarChar,this.BillingPhoneNumber);
            dataLib.AddParameter("@BillingMobileNumber", SqlDbType.NVarChar,this.BillingMobileNumber);
            dataLib.AddParameter("@TransactionId", SqlDbType.NVarChar,this.TransactionId);
            dataLib.AddParameter("@TransactionDate", SqlDbType.DateTime,this.TransactionDate);
            dataLib.AddParameter("@AddedBy", SqlDbType.Int,this.AddedBy);
            dataLib.AddParameter("@AddedDate", SqlDbType.DateTime,this.AddedDate);
            dataLib.AddParameter("@IsPaid", SqlDbType.Bit, this.IsPaid);
            dataLib.AddParameter("@failcode", SqlDbType.NVarChar, this.failcode);
            dataLib.AddParameter("@faildesc", SqlDbType.NVarChar, this.faildesc);
        }

        public override void AddUpdateParams(ref ZWT.DbLib.IDataLib dataLib)
        {
            dataLib.AddParameter("@Id", SqlDbType.Int,this.Id);
            dataLib.AddParameter("@ClientId", SqlDbType.Int,this.ClientId);
            dataLib.AddParameter("@UnitId", SqlDbType.Int,this.UnitId);
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