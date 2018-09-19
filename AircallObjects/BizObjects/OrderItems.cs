using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace BizObjects
{
    public class OrderItems : BizObject
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int PartId { get; set; }
        public string PartName { get; set; }
        public string PartSize { get; set; }
        public decimal Amount { get; set; }
        public int Quantity { get; set; }

        #region "Constructors"
        public OrderItems()
        {
        }
        public OrderItems(ref DataRow drRow)
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
            dataLib.AddParameter("@OrderId", SqlDbType.Int, this.OrderId);
            dataLib.AddParameter("@PartId", SqlDbType.Int, this.PartId);
            dataLib.AddParameter("@Quantity", SqlDbType.Int, this.Quantity);
        }

        public override void AddUpdateParams(ref ZWT.DbLib.IDataLib dataLib)
        {
            dataLib.AddParameter("@Id", SqlDbType.Int, this.Id);
            dataLib.AddParameter("@OrderId", SqlDbType.Int, this.OrderId);
            dataLib.AddParameter("@PartId", SqlDbType.Int, this.PartId);
            dataLib.AddParameter("@PartName", SqlDbType.NVarChar, this.PartName);
            dataLib.AddParameter("@PartSize", SqlDbType.NVarChar, this.PartSize);
            dataLib.AddParameter("@Amount", SqlDbType.Decimal, this.Amount);
            dataLib.AddParameter("@Quantity", SqlDbType.Int, this.Quantity);
        }

        public override void AddSearchParams(ref ZWT.DbLib.IDataLib dataLib)
        {

        }
        #endregion
    }
}
