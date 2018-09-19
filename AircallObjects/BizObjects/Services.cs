using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace BizObjects
{
    public class Services : BizObject
    {
        public Int64 Id { get; set; }
        public string ServiceCaseNumber { get; set; }
        public int ClientId { get; set; }
        public int AddressID { get; set; }
        public string PurposeOfVisit { get; set; }
        public int WorkAreaId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime ScheduleDate { get; set; }
        public string ScheduleStartTime { get; set; }
        public string ScheduleEndTime { get; set; }
        public string CustomerComplaints { get; set; }
        public string DispatcherNotes { get; set; }
        public string TechnicianNotes { get; set; }
        public string Status { get; set; }
        public DateTime StatusChangeDate { get; set; }
        public int ScheduledBy { get; set; }
        public DateTime LastScheduleRunDate { get; set; }
        public int AddedBy { get; set; }
        public int AddedByType { get; set; }
        public DateTime AddedDate { get; set; }
        public int UpdatedBy { get; set; }
        public int UpdatedByType { get; set; }
        public DateTime UpdatedDate { get; set; }

        #region "Constructors"
        public Services()
        {
        }
        public Services(ref DataRow drRow)
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
            dataLib.AddParameter("@ServiceCaseNumber", SqlDbType.NVarChar, this.ServiceCaseNumber);
            dataLib.AddParameter("@ClientId", SqlDbType.Int, this.ClientId);
            dataLib.AddParameter("@AddressID", SqlDbType.Int, this.AddressID);
            dataLib.AddParameter("@PurposeOfVisit", SqlDbType.NVarChar, this.PurposeOfVisit);
            dataLib.AddParameter("@WorkAreaId", SqlDbType.Int, this.WorkAreaId);
            dataLib.AddParameter("@EmployeeId", SqlDbType.Int, this.EmployeeId);
            dataLib.AddParameter("@ScheduleDate", SqlDbType.DateTime, this.ScheduleDate);
            dataLib.AddParameter("@ScheduleStartTime", SqlDbType.NVarChar, this.ScheduleStartTime);
            dataLib.AddParameter("@ScheduleEndTime", SqlDbType.NVarChar, this.ScheduleEndTime);
            dataLib.AddParameter("@CustomerComplaints", SqlDbType.NVarChar, this.CustomerComplaints);
            dataLib.AddParameter("@DispatcherNotes", SqlDbType.NVarChar, this.DispatcherNotes);
            dataLib.AddParameter("@TechnicianNotes", SqlDbType.NVarChar, this.TechnicianNotes);
            dataLib.AddParameter("@Status", SqlDbType.NVarChar, this.Status);
            dataLib.AddParameter("@StatusChangeDate", SqlDbType.DateTime, this.StatusChangeDate);
            dataLib.AddParameter("@ScheduledBy", SqlDbType.Int, this.ScheduledBy);
            dataLib.AddParameter("@LastScheduleRunDate", SqlDbType.DateTime, this.LastScheduleRunDate);
            dataLib.AddParameter("@AddedBy", SqlDbType.Int, this.AddedBy);
            dataLib.AddParameter("@AddedByType", SqlDbType.Int, this.AddedByType);
            dataLib.AddParameter("@AddedDate", SqlDbType.DateTime, this.AddedDate);
            dataLib.AddParameter("@UpdatedBy", SqlDbType.Int, this.UpdatedBy);
            dataLib.AddParameter("@UpdatedByType", SqlDbType.Int, this.UpdatedByType);
            dataLib.AddParameter("@UpdatedDate", SqlDbType.DateTime, this.UpdatedDate);
        }

        public override void AddUpdateParams(ref ZWT.DbLib.IDataLib dataLib)
        {
            dataLib.AddParameter("@Id", SqlDbType.BigInt, this.Id);
            dataLib.AddParameter("@ClientId", SqlDbType.Int, this.ClientId);
            dataLib.AddParameter("@AddressID", SqlDbType.Int, this.AddressID);
            dataLib.AddParameter("@PurposeOfVisit", SqlDbType.NVarChar, this.PurposeOfVisit);
            dataLib.AddParameter("@WorkAreaId", SqlDbType.Int, this.WorkAreaId);
            dataLib.AddParameter("@EmployeeId", SqlDbType.Int, this.EmployeeId);
            dataLib.AddParameter("@ScheduleDate", SqlDbType.DateTime, this.ScheduleDate);
            dataLib.AddParameter("@ScheduleStartTime", SqlDbType.NVarChar, this.ScheduleStartTime);
            dataLib.AddParameter("@ScheduleEndTime", SqlDbType.NVarChar, this.ScheduleEndTime);
            dataLib.AddParameter("@CustomerComplaints", SqlDbType.NVarChar, this.CustomerComplaints);
            dataLib.AddParameter("@DispatcherNotes", SqlDbType.NVarChar, this.DispatcherNotes);
            dataLib.AddParameter("@TechnicianNotes", SqlDbType.NVarChar, this.TechnicianNotes);
            dataLib.AddParameter("@Status", SqlDbType.NVarChar, this.Status);
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

