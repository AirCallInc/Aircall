using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace BizObjects
{
    public class ClientUnit:BizObject
    {
        public int Id {get; set;} 
        public int ClientId {get; set;} 
        public int PlanId {get; set;} 
        public int PlanTypeId {get; set;} 
        public string UnitName {get; set;} 
        public DateTime ManufactureDate {get; set;} 
        public string UnitTon {get; set;} 
        public int AddressId {get; set;} 
        public int UnitTypeId {get; set;} 
        public bool AutoRenewal {get; set;}
        public string Notes {get; set;} 
        public int Status {get; set;} 
        public string PaymentStatus {get; set;} 
        public bool IsMatched {get; set;} 
        public bool IsDeleted {get; set;}
        public bool IsSpecialApplied { get; set; }
        public bool IsServiceAdded { get; set; }
        public string StripeSubscriptionId { get; set; }
        public bool IsPlanRenewedOrCancelled { get; set; }
        public bool IsActive { get; set; }
        public string CurrentPaymentMethod { get; set; }
        public DateTime PaymentMethodChangeDate { get; set; }
        public int OldUnitId { get; set; } 
        public int AddedBy {get; set;} 
        public int AddedByType {get; set;} 
        public DateTime AddedDate {get; set;} 
        public int UpdatedBy {get; set;} 
        public int UpdatedByType {get; set;} 
        public DateTime UpdatedDate {get; set;} 
        public int DeletedBy {get; set;} 
        public int DeletedByType {get; set;} 
        public DateTime DeletedDate {get; set;} 

        #region "Constructors"
        public ClientUnit()
        {
        }
        public ClientUnit(ref DataRow drRow)
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
            dataLib.AddParameter("@PlanTypeId", SqlDbType.Int,this.PlanTypeId);
            dataLib.AddParameter("@UnitName", SqlDbType.NVarChar,this.UnitName);
            dataLib.AddParameter("@ManufactureDate", SqlDbType.DateTime,this.ManufactureDate);
            dataLib.AddParameter("@AddressId", SqlDbType.Int,this.AddressId);
            dataLib.AddParameter("@UnitTypeId", SqlDbType.Int,this.UnitTypeId);
            dataLib.AddParameter("@AutoRenewal", SqlDbType.Bit,this.AutoRenewal);
            dataLib.AddParameter("@IsSpecialApplied", SqlDbType.Bit, this.IsSpecialApplied);
            dataLib.AddParameter("@Status", SqlDbType.Int,this.Status);
            dataLib.AddParameter("@Notes", SqlDbType.NVarChar, this.Notes);
            dataLib.AddParameter("@CurrentPaymentMethod", SqlDbType.NVarChar, this.CurrentPaymentMethod);
            //dataLib.AddParameter("@PaymentStatus", SqlDbType.NVarChar,this.PaymentStatus);
            dataLib.AddParameter("@AddedBy", SqlDbType.Int,this.AddedBy);
            dataLib.AddParameter("@AddedByType", SqlDbType.Int,this.AddedByType);
            dataLib.AddParameter("@AddedDate", SqlDbType.DateTime,this.AddedDate);
        }

        public override void AddUpdateParams(ref ZWT.DbLib.IDataLib dataLib)
        {
            dataLib.AddParameter("@Id", SqlDbType.Int,this.Id);
            dataLib.AddParameter("@ClientId", SqlDbType.Int,this.ClientId);
            dataLib.AddParameter("@PlanTypeId", SqlDbType.Int,this.PlanTypeId);
            dataLib.AddParameter("@UnitName", SqlDbType.NVarChar,this.UnitName);
            dataLib.AddParameter("@ManufactureDate", SqlDbType.DateTime,this.ManufactureDate);
            dataLib.AddParameter("@AddressId", SqlDbType.Int,this.AddressId);
            dataLib.AddParameter("@UnitTypeId", SqlDbType.Int,this.UnitTypeId);
            dataLib.AddParameter("@Status", SqlDbType.Int, this.Status);
            dataLib.AddParameter("@Notes", SqlDbType.NVarChar, this.Notes);
            dataLib.AddParameter("@UpdatedBy", SqlDbType.Int,this.UpdatedBy);
            dataLib.AddParameter("@UpdatedByType", SqlDbType.Int,this.UpdatedByType);
            dataLib.AddParameter("@UpdatedDate", SqlDbType.DateTime,this.UpdatedDate);
        }

        public override void AddSearchParams(ref ZWT.DbLib.IDataLib dataLib)
        {

        }
        #endregion
    }
}