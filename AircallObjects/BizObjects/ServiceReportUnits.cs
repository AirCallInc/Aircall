using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace BizObjects
{
    public class ServiceReportUnits : BizObject
    {
        public int Id { get; set; }
        public Int64 ServiceId { get; set; }
        public int UnitId { get; set; }
        public bool IsCompleted { get; set; }
        public Int64 ServiceReportId { get; set; }

        #region "Constructors"
        public ServiceReportUnits()
        {
        }
        public ServiceReportUnits(ref DataRow drRow)
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
            dataLib.AddParameter("@ServiceId", SqlDbType.BigInt, this.ServiceId);
            dataLib.AddParameter("@UnitId", SqlDbType.Int, this.UnitId);
            dataLib.AddParameter("@IsCompleted", SqlDbType.Bit, this.IsCompleted);
            dataLib.AddParameter("@ServiceReportId", SqlDbType.BigInt, this.ServiceReportId);
        }

        public override void AddUpdateParams(ref ZWT.DbLib.IDataLib dataLib)
        {
            dataLib.AddParameter("@Id", SqlDbType.Int, this.Id);
            dataLib.AddParameter("@ServiceId", SqlDbType.BigInt, this.ServiceId);
            dataLib.AddParameter("@UnitId", SqlDbType.Int, this.UnitId);
            dataLib.AddParameter("@IsCompleted", SqlDbType.Bit, this.IsCompleted);
            dataLib.AddParameter("@ServiceReportId", SqlDbType.BigInt, this.ServiceReportId);
        }

        public override void AddSearchParams(ref ZWT.DbLib.IDataLib dataLib)
        {

        }
        #endregion
    }
}