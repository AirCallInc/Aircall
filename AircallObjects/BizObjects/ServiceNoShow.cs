using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace BizObjects
{
    public class ServiceNoShow:BizObject
    {
        public long Id { get; set; }
        public long ServiceId { get; set; }
        public decimal NoShowAmount { get; set; }
        public bool IsPaymentReceived { get; set; }
        public bool IsNoShow { get; set; }
        public int AddedBy { get; set; }
        public int AddedByType { get; set; }
        public DateTime AddedDate { get; set; }

         #region "Constructors"
        public ServiceNoShow()
        {
        }
        public ServiceNoShow(ref DataRow drRow)
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
            dataLib.AddParameter("@NoShowAmount", SqlDbType.Decimal, this.NoShowAmount);
            dataLib.AddParameter("@IsPaymentReceived", SqlDbType.Bit, this.IsPaymentReceived);
            dataLib.AddParameter("@IsNoShow", SqlDbType.Bit, this.IsNoShow);
            dataLib.AddParameter("@AddedBy", SqlDbType.Int, this.AddedBy);
            dataLib.AddParameter("@AddedByType", SqlDbType.Int, this.AddedByType);
            dataLib.AddParameter("@AddedDate", SqlDbType.DateTime, this.AddedDate);
        }

        public override void AddUpdateParams(ref ZWT.DbLib.IDataLib dataLib)
        {
            
        }

        public override void AddSearchParams(ref ZWT.DbLib.IDataLib dataLib)
        {

        }
        #endregion
    }
}
