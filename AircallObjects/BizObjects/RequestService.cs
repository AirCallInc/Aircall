using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace BizObjects
{
    public class RequestService:BizObject
    {
        public Int64 Id {get; set;} 
        public string ServiceCaseNumber {get; set;} 
        public int ClientId {get; set;} 
        public int AddressId {get; set;} 
        public string PurposeOfVisit {get; set;} 
        public string ServiceRequestedTime {get; set;} 
        public DateTime ServiceRequestedOn {get; set;} 
        public string Notes {get; set;} 
        public Int64 ServiceId {get; set;} 
        public int AddedBy {get; set;} 
        public int AddedByType {get; set;} 
        public DateTime AddedDate {get; set;} 
        public int UpdatedBy {get; set;} 
        public int UpdatedByType {get; set;} 
        public DateTime UpdatedDate {get; set;} 
        public bool IsDeleted {get; set;} 
        public int DeletedBy {get; set;} 
        public int DeletedByType {get; set;} 
        public DateTime DeletedDate {get; set;} 

        #region "Constructors"
        public RequestService()
        {
        }
        public RequestService(ref DataRow drRow)
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
            dataLib.AddParameter("@AddressId", SqlDbType.Int,this.AddressId);
            dataLib.AddParameter("@PurposeOfVisit", SqlDbType.NVarChar,this.PurposeOfVisit);
            dataLib.AddParameter("@ServiceRequestedTime", SqlDbType.NVarChar,this.ServiceRequestedTime);
            dataLib.AddParameter("@ServiceRequestedOn", SqlDbType.DateTime,this.ServiceRequestedOn);
            dataLib.AddParameter("@Notes", SqlDbType.NVarChar,this.Notes);
            dataLib.AddParameter("@AddedBy", SqlDbType.Int,this.AddedBy);
            dataLib.AddParameter("@AddedByType", SqlDbType.Int,this.AddedByType);
            dataLib.AddParameter("@AddedDate", SqlDbType.DateTime,this.AddedDate);
        }

        public override void AddUpdateParams(ref ZWT.DbLib.IDataLib dataLib)
        {
            dataLib.AddParameter("@Id", SqlDbType.BigInt, this.Id);
            dataLib.AddParameter("@ClientId", SqlDbType.Int, this.ClientId);
            dataLib.AddParameter("@AddressId", SqlDbType.Int, this.AddressId);
            dataLib.AddParameter("@PurposeOfVisit", SqlDbType.NVarChar, this.PurposeOfVisit);
            dataLib.AddParameter("@ServiceRequestedTime", SqlDbType.NVarChar, this.ServiceRequestedTime);
            dataLib.AddParameter("@ServiceRequestedOn", SqlDbType.DateTime, this.ServiceRequestedOn);
            dataLib.AddParameter("@Notes", SqlDbType.NVarChar, this.Notes);
            dataLib.AddParameter("@UpdatedBy", SqlDbType.Int, this.UpdatedBy);
            dataLib.AddParameter("@UpdatedByType", SqlDbType.Int, this.UpdatedByType);
            dataLib.AddParameter("@UpdatedDate", SqlDbType.DateTime, this.UpdatedDate);
        }

        public override void AddSearchParams(ref ZWT.DbLib.IDataLib dataLib)
        {

        }
        #endregion
    }
}
