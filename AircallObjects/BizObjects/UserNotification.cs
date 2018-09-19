using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace BizObjects
{
    public class UserNotification:BizObject
    {
        public Int64 Id {get; set;} 
        public int UserId {get; set;}
        public int UserTypeId { get; set; }
        public string Message {get; set;} 
        public string Status {get; set;} 
        public Int64 CommonId {get; set;} 
        public string MessageType {get; set;} 
        public int EmpId {get; set;} 
        public int RedirectId {get; set;} 
        public string RedirectTag {get; set;} 
        public DateTime AddedDate {get; set;} 
        public int UpdatedBy {get; set;} 
        public DateTime UpdatedDate {get; set;} 
        public int DeletedBy {get; set;} 
        public DateTime DeletedDate {get; set;} 

        #region "Constructors"
        public UserNotification()
        {
        }
        public UserNotification(ref DataRow drRow)
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
            dataLib.AddParameter("@UserId", SqlDbType.Int,this.UserId);
            dataLib.AddParameter("@UserTypeId", SqlDbType.Int, this.UserTypeId);
            dataLib.AddParameter("@Message", SqlDbType.NVarChar,this.Message);
            dataLib.AddParameter("@Status", SqlDbType.NVarChar,this.Status);
            dataLib.AddParameter("@CommonId", SqlDbType.BigInt,this.CommonId);
            dataLib.AddParameter("@MessageType", SqlDbType.NVarChar,this.MessageType);
            dataLib.AddParameter("@EmpId", SqlDbType.Int,this.EmpId);
            dataLib.AddParameter("@AddedDate", SqlDbType.DateTime,this.AddedDate);
        }

        public override void AddUpdateParams(ref ZWT.DbLib.IDataLib dataLib)
        {
            dataLib.AddParameter("@Id", SqlDbType.BigInt,this.Id);
            dataLib.AddParameter("@UserId", SqlDbType.Int,this.UserId);
            dataLib.AddParameter("@UserTypeId", SqlDbType.Int, this.UserTypeId);
            dataLib.AddParameter("@Message", SqlDbType.NVarChar,this.Message);
            dataLib.AddParameter("@Status", SqlDbType.NVarChar,this.Status);
            dataLib.AddParameter("@CommonId", SqlDbType.BigInt,this.CommonId);
            dataLib.AddParameter("@MessageType", SqlDbType.NVarChar,this.MessageType);
            dataLib.AddParameter("@EmpId", SqlDbType.Int,this.EmpId);
            dataLib.AddParameter("@RedirectId", SqlDbType.Int,this.RedirectId);
            dataLib.AddParameter("@RedirectTag", SqlDbType.NVarChar,this.RedirectTag);
            dataLib.AddParameter("@AddedDate", SqlDbType.DateTime,this.AddedDate);
            dataLib.AddParameter("@UpdatedBy", SqlDbType.Int,this.UpdatedBy);
            dataLib.AddParameter("@UpdatedDate", SqlDbType.DateTime,this.UpdatedDate);
            dataLib.AddParameter("@DeletedBy", SqlDbType.Int,this.DeletedBy);
            dataLib.AddParameter("@DeletedDate", SqlDbType.DateTime,this.DeletedDate);
        }

        public override void AddSearchParams(ref ZWT.DbLib.IDataLib dataLib)
        {

        }
        #endregion
    }
}
