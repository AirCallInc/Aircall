using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace BizObjects
{
    public class EmployeeWorkArea:BizObject
    {
        public long Id { get; set; }
        public int EmployeeId { get; set; }
        public Int16 PriorityArea { get; set; }
        public int AreaId { get; set; }

        #region "Constructors"
        public EmployeeWorkArea()
        {
        }
        public EmployeeWorkArea(ref DataRow drRow)
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
            dataLib.AddParameter("@PriorityArea", SqlDbType.SmallInt, this.PriorityArea);
            dataLib.AddParameter("@AreaId", SqlDbType.Int, this.AreaId);
        }

        public override void AddUpdateParams(ref ZWT.DbLib.IDataLib dataLib)
        {
            dataLib.AddParameter("@Id", SqlDbType.BigInt, this.Id);
            dataLib.AddParameter("@EmployeeId", SqlDbType.Int, this.EmployeeId);
            dataLib.AddParameter("@PriorityArea", SqlDbType.SmallInt, this.PriorityArea);
            dataLib.AddParameter("@AreaId", SqlDbType.Int, this.AreaId);
        }
        public override void AddSearchParams(ref ZWT.DbLib.IDataLib dataLib)
        {

        }
        #endregion
    }
}
