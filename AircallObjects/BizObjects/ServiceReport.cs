using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace BizObjects
{
    public class ServiceReport : BizObject
    {
        public Int64 Id { get; set; }
        public string ServiceReportNumber { get; set; }
        public Int64 ServiceId { get; set; }
        public string BillingType { get; set; }
        public string WorkStartedTime { get; set; }
        public string WorkCompletedTime { get; set; }
        public bool IsWorkDone { get; set; }
        public string WorkPerformed { get; set; }
        public string Recommendationsforcustomer { get; set; }
        public string EmployeeNotes { get; set; }
        public string CCEmail { get; set; }
        public bool IsEmailToClient { get; set; }
        public string ClientSignature { get; set; }
        public int AddedBy { get; set; }
        public int AddedByType { get; set; }
        public DateTime AddedDate { get; set; }
        public int UpdatedBy { get; set; }
        public int UpdatedByType { get; set; }
        public DateTime UpdatedDate { get; set; }

        #region "Constructors"
        public ServiceReport()
        {
        }
        public ServiceReport(ref DataRow drRow)
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
            dataLib.AddParameter("@ServiceReportNumber", SqlDbType.NVarChar, this.ServiceReportNumber);
            dataLib.AddParameter("@ServiceId", SqlDbType.BigInt, this.ServiceId);
            dataLib.AddParameter("@WorkStartedTime", SqlDbType.NVarChar, this.WorkStartedTime);
            dataLib.AddParameter("@WorkCompletedTime", SqlDbType.NVarChar, this.WorkCompletedTime);
            dataLib.AddParameter("@IsWorkDone", SqlDbType.Bit, this.IsWorkDone);
            dataLib.AddParameter("@WorkPerformed", SqlDbType.NVarChar, this.WorkPerformed);
            dataLib.AddParameter("@Recommendationsforcustomer", SqlDbType.NVarChar, this.Recommendationsforcustomer);
            dataLib.AddParameter("@EmployeeNotes", SqlDbType.NVarChar, this.EmployeeNotes);
            dataLib.AddParameter("@CCEmail", SqlDbType.NVarChar, this.CCEmail);
            dataLib.AddParameter("@IsEmailToClient", SqlDbType.Bit, this.IsEmailToClient);
            dataLib.AddParameter("@ClientSignature", SqlDbType.NVarChar, this.ClientSignature);
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
            dataLib.AddParameter("@BillingType", SqlDbType.NVarChar, this.BillingType);
            dataLib.AddParameter("@WorkStartedTime", SqlDbType.NVarChar, this.WorkStartedTime);
            dataLib.AddParameter("@WorkCompletedTime", SqlDbType.NVarChar, this.WorkCompletedTime);
            dataLib.AddParameter("@IsWorkDone", SqlDbType.Bit, this.IsWorkDone);
            dataLib.AddParameter("@WorkPerformed", SqlDbType.NVarChar, this.WorkPerformed);
            dataLib.AddParameter("@Recommendationsforcustomer", SqlDbType.NVarChar, this.Recommendationsforcustomer);
            dataLib.AddParameter("@EmployeeNotes", SqlDbType.NVarChar, this.EmployeeNotes);
            dataLib.AddParameter("@CCEmail", SqlDbType.NVarChar, this.CCEmail);
            dataLib.AddParameter("@IsEmailToClient", SqlDbType.Bit, this.IsEmailToClient);
            dataLib.AddParameter("@ClientSignature", SqlDbType.NVarChar, this.ClientSignature);
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