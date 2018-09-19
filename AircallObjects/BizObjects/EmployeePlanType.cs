using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace BizObjects
{
    public class EmployeePlanType:BizObject
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int PlanTypeId { get; set; }

        #region "Constructors"
        public EmployeePlanType()
        {
        }
        public EmployeePlanType(ref DataRow drRow)
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
            dataLib.AddParameter("@PlanTypeId", SqlDbType.Int, this.PlanTypeId);
        }

        public override void AddUpdateParams(ref ZWT.DbLib.IDataLib dataLib)
        {
            dataLib.AddParameter("@Id", SqlDbType.Int, this.Id);
            dataLib.AddParameter("@EmployeeId", SqlDbType.Int, this.EmployeeId);
            dataLib.AddParameter("@PlanTypeId", SqlDbType.Int, this.PlanTypeId);
        }

        public override void AddSearchParams(ref ZWT.DbLib.IDataLib dataLib)
        {

        }
        #endregion
    }
}
