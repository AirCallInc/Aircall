using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace BizObjects
{
    public class RescheduleService:BizObject
    {
        public int Id {get; set;} 
        public Int64 ServiceId {get; set;} 
        public DateTime RescheduleDate {get; set;} 
        public string Rescheduletime {get; set;} 
        public string Reason {get; set;} 
        public int AddedBy {get; set;} 
        public int AddedByType {get; set;} 
        public DateTime AddedDate {get; set;} 

        #region "Constructors"
        public RescheduleService()
        {
        }
        public RescheduleService(ref DataRow drRow)
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
            dataLib.AddParameter("@ServiceId", SqlDbType.BigInt,this.ServiceId);
            dataLib.AddParameter("@RescheduleDate", SqlDbType.Date,this.RescheduleDate);
            dataLib.AddParameter("@Rescheduletime", SqlDbType.NVarChar,this.Rescheduletime);
            dataLib.AddParameter("@Reason", SqlDbType.NVarChar,this.Reason);
            dataLib.AddParameter("@AddedBy", SqlDbType.Int,this.AddedBy);
            dataLib.AddParameter("@AddedByType", SqlDbType.Int,this.AddedByType);
            dataLib.AddParameter("@AddedDate", SqlDbType.DateTime,this.AddedDate);
        }

        public override void AddUpdateParams(ref ZWT.DbLib.IDataLib dataLib)
        {
            dataLib.AddParameter("@Id", SqlDbType.Int, this.Id);
            dataLib.AddParameter("@ServiceId", SqlDbType.BigInt, this.ServiceId);
            dataLib.AddParameter("@RescheduleDate", SqlDbType.Date, this.RescheduleDate);
            dataLib.AddParameter("@Rescheduletime", SqlDbType.NVarChar, this.Rescheduletime);
            dataLib.AddParameter("@Reason", SqlDbType.NVarChar, this.Reason);
            dataLib.AddParameter("@AddedBy", SqlDbType.Int, this.AddedBy);
            dataLib.AddParameter("@AddedByType", SqlDbType.Int, this.AddedByType);
            dataLib.AddParameter("@AddedDate", SqlDbType.DateTime, this.AddedDate);
        }
        public override void AddSearchParams(ref ZWT.DbLib.IDataLib dataLib)
        {

        }
        #endregion
    }
}
