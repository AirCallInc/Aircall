using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace BizObjects
{
    public class EmployeePartRequest : BizObject
    {
        public int Id { get; set; }
        public int EmployeePartRequestId { get; set; }
        public int UnitId { get; set; }
        public int PartId { get; set; }
        public int RequestedQuantity { get; set; }
        public int ArrangedQuantity { get; set; }
        public string PartName { get; set; }
        public string PartSize { get; set; }
        public string Description { get; set; }

        #region "Constructors"
        public EmployeePartRequest()
        {
        }
        public EmployeePartRequest(ref DataRow drRow)
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
            dataLib.AddParameter("@EmployeePartRequestId", SqlDbType.Int, this.EmployeePartRequestId);
            dataLib.AddParameter("@UnitId", SqlDbType.Int, this.UnitId);
            dataLib.AddParameter("@PartId", SqlDbType.Int, this.PartId);
            dataLib.AddParameter("@RequestedQuantity", SqlDbType.Int, this.RequestedQuantity);
            dataLib.AddParameter("@ArrangedQuantity", SqlDbType.Int, this.ArrangedQuantity);
            dataLib.AddParameter("@PartName", SqlDbType.NVarChar, this.PartName);
            dataLib.AddParameter("@PartSize", SqlDbType.NVarChar, this.PartSize);
            dataLib.AddParameter("@Description", SqlDbType.NVarChar, this.Description);
        }

        public override void AddUpdateParams(ref ZWT.DbLib.IDataLib dataLib)
        {
            dataLib.AddParameter("@Id", SqlDbType.Int, this.Id);
            dataLib.AddParameter("@EmployeePartRequestId", SqlDbType.Int, this.EmployeePartRequestId);
            dataLib.AddParameter("@UnitId", SqlDbType.Int, this.UnitId);
            dataLib.AddParameter("@PartId", SqlDbType.Int, this.PartId);
            dataLib.AddParameter("@RequestedQuantity", SqlDbType.Int, this.RequestedQuantity);
            dataLib.AddParameter("@ArrangedQuantity", SqlDbType.Int, this.ArrangedQuantity);
            dataLib.AddParameter("@PartName", SqlDbType.NVarChar, this.PartName);
            dataLib.AddParameter("@PartSize", SqlDbType.NVarChar, this.PartSize);
            dataLib.AddParameter("@Description", SqlDbType.NVarChar, this.Description);
        }

        public override void AddSearchParams(ref ZWT.DbLib.IDataLib dataLib)
        {

        }
        #endregion
    }
}