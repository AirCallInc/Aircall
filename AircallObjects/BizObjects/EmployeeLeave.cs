using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace BizObjects
{
    public class EmployeeLeave : BizObject
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool AvailableOnHoliday { get; set; }
        public string Reason { get; set; }
        public bool IsDeleted { get; set; }
        public int AddedBy { get; set; }
        public DateTime AddedDate { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int DeletedBy { get; set; }
        public DateTime DeletedDate { get; set; }

        #region "Constructors"
        public EmployeeLeave()
        {
        }
        public EmployeeLeave(ref DataRow drRow)
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
            dataLib.AddParameter("@EmployeeId", SqlDbType.Int, this.EmployeeId);
            dataLib.AddParameter("@StartDate", SqlDbType.DateTime, this.StartDate);
            dataLib.AddParameter("@EndDate", SqlDbType.DateTime, this.EndDate);
            dataLib.AddParameter("@AvailableOnHoliday", SqlDbType.Bit, this.AvailableOnHoliday);
            dataLib.AddParameter("@Reason", SqlDbType.NVarChar, this.Reason);
            dataLib.AddParameter("@AddedBy", SqlDbType.Int, this.AddedBy);
            dataLib.AddParameter("@AddedDate", SqlDbType.DateTime, this.AddedDate);
        }

        public override void AddUpdateParams(ref ZWT.DbLib.IDataLib dataLib)
        {
            dataLib.AddParameter("@Id", SqlDbType.Int, this.Id);
            dataLib.AddParameter("@EmployeeId", SqlDbType.Int, this.EmployeeId);
            dataLib.AddParameter("@StartDate", SqlDbType.DateTime, this.StartDate);
            dataLib.AddParameter("@EndDate", SqlDbType.DateTime, this.EndDate);
            dataLib.AddParameter("@AvailableOnHoliday", SqlDbType.Bit, this.AvailableOnHoliday);
            dataLib.AddParameter("@Reason", SqlDbType.NVarChar, this.Reason);
            dataLib.AddParameter("@UpdatedBy", SqlDbType.Int, this.UpdatedBy);
            dataLib.AddParameter("@UpdatedDate", SqlDbType.DateTime, this.UpdatedDate);
        }

        public override void AddSearchParams(ref ZWT.DbLib.IDataLib dataLib)
        {

        }
        #endregion

    }
}