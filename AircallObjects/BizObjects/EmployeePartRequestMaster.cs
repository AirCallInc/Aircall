using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace BizObjects
{
    public class EmployeePartRequestMaster : BizObject
    {
        public int Id { get; set; }
        public Int64 ServiceReportId { get; set; }
        public string Status { get; set; }
        public DateTime AddedDate { get; set; }
        public int EmployeeId { get; set; }
        public int ClientId { get; set; }
        public int ClientAddressId { get; set; }
        public DateTime StatusUpdatedDate { get; set; }
        public string EmpNotes { get; set; }
        public string Notes { get; set; }
        public int AddedBy { get; set; }
        public int AddedByType { get; set; }
        public int UpdatedBy { get; set; }
        public int UpdatedByType { get; set; }
        public DateTime UpdatedDate { get; set; }

        #region "Constructors"
        public EmployeePartRequestMaster()
        {
        }
        public EmployeePartRequestMaster(ref DataRow drRow)
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
            dataLib.AddParameter("@Status", SqlDbType.NVarChar, this.Status);
            dataLib.AddParameter("@AddedDate", SqlDbType.DateTime, this.AddedDate);
            dataLib.AddParameter("@EmployeeId", SqlDbType.Int, this.EmployeeId);
            dataLib.AddParameter("@ClientId", SqlDbType.Int, this.ClientId);
            dataLib.AddParameter("@ClientAddressId", SqlDbType.Int, this.ClientAddressId);
            dataLib.AddParameter("@AddedBy", SqlDbType.Int, this.AddedBy);
            dataLib.AddParameter("@AddedByType", SqlDbType.Int, this.AddedByType);
            dataLib.AddParameter("@Notes", SqlDbType.NVarChar, this.Notes);
            dataLib.AddParameter("@EmpNotes", SqlDbType.NVarChar, this.EmpNotes);
        }

        public override void AddUpdateParams(ref ZWT.DbLib.IDataLib dataLib)
        {
            dataLib.AddParameter("@Id", SqlDbType.Int, this.Id);
            dataLib.AddParameter("@Status", SqlDbType.NVarChar, this.Status);
            dataLib.AddParameter("@EmployeeId", SqlDbType.Int, this.EmployeeId);
            dataLib.AddParameter("@ClientId", SqlDbType.Int, this.ClientId);
            dataLib.AddParameter("@ClientAddressId", SqlDbType.Int, this.ClientAddressId);
            dataLib.AddParameter("@Notes", SqlDbType.NVarChar, this.Notes);
            dataLib.AddParameter("@EmpNotes", SqlDbType.NVarChar, this.EmpNotes);
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