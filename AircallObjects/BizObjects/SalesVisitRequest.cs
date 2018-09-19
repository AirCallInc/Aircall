using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace BizObjects
{
    public class SalesVisitRequest : BizObject
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int AddressId { get; set; }
        public string Notes { get; set; }
        public int EmployeeId { get; set; }
        public DateTime RepliedDate { get; set; }
        public int AddedBy { get; set; }
        public int AddedByType { get; set; }
        public DateTime AddedDate { get; set; }
        public int AssignedBy { get; set; }
        public int AssignedByType{ get; set; }

        #region "Constructors"
        public SalesVisitRequest()
        {
        }
        public SalesVisitRequest(ref DataRow drRow)
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
            dataLib.AddParameter("@ClientId", SqlDbType.Int, this.ClientId);
            dataLib.AddParameter("@AddressId", SqlDbType.Int, this.AddressId);
            dataLib.AddParameter("@Notes", SqlDbType.NVarChar, this.Notes);
            dataLib.AddParameter("@EmployeeId", SqlDbType.Int, this.EmployeeId);
            dataLib.AddParameter("@RepliedDate", SqlDbType.DateTime, this.RepliedDate);
            dataLib.AddParameter("@AddedBy", SqlDbType.Int, this.AddedBy);
            dataLib.AddParameter("@AddedByType", SqlDbType.Int, this.AddedByType);
            dataLib.AddParameter("@AddedDate", SqlDbType.DateTime, this.AddedDate);
        }

        public override void AddUpdateParams(ref ZWT.DbLib.IDataLib dataLib)
        {
            dataLib.AddParameter("@Id", SqlDbType.Int, this.Id);
            dataLib.AddParameter("@EmployeeId", SqlDbType.Int, this.EmployeeId);
            dataLib.AddParameter("@RepliedDate", SqlDbType.DateTime, this.RepliedDate);
            dataLib.AddParameter("@AssignedBy", SqlDbType.Int, this.AssignedBy);
            dataLib.AddParameter("@AssignedByType", SqlDbType.Int, this.AssignedByType);
        }

        public override void AddSearchParams(ref ZWT.DbLib.IDataLib dataLib)
        {

        }
        #endregion
    }
}
