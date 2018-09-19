using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace BizObjects
{
    public class StripeErrorLog : BizObject
    {
        public Int64 Id { get; set; }
        public string ChargeId { get; set; }
        public string Code { get; set; }
        public string DeclineCode { get; set; }
        public string ErrorType { get; set; }
        public string Error { get; set; }
        public string ErrorSubscription { get; set; }
        public string Message { get; set; }
        public string Parameter { get; set; }
        public int Userid { get; set; }
        public Int64 UnitId { get; set; }
        public DateTime DateAdded { get; set; }

        #region "Constructors"
        public StripeErrorLog()
        {
        }
        public StripeErrorLog(ref DataRow drRow)
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
            dataLib.AddParameter("@ChargeId", SqlDbType.NVarChar, this.ChargeId);
            dataLib.AddParameter("@Code", SqlDbType.NVarChar, this.Code);
            dataLib.AddParameter("@DeclineCode", SqlDbType.NVarChar, this.DeclineCode);
            dataLib.AddParameter("@ErrorType", SqlDbType.NVarChar, this.ErrorType);
            dataLib.AddParameter("@Error", SqlDbType.NVarChar, this.Error);
            dataLib.AddParameter("@ErrorSubscription", SqlDbType.NVarChar, this.ErrorSubscription);
            dataLib.AddParameter("@Message", SqlDbType.NVarChar, this.Message);
            dataLib.AddParameter("@Parameter", SqlDbType.NVarChar, this.Parameter);
            dataLib.AddParameter("@Userid", SqlDbType.Int, this.Userid);
            dataLib.AddParameter("@UnitId", SqlDbType.BigInt, this.UnitId);
            dataLib.AddParameter("@DateAdded", SqlDbType.DateTime, this.DateAdded);
        }

        public override void AddUpdateParams(ref ZWT.DbLib.IDataLib dataLib)
        {
            dataLib.AddParameter("@Id", SqlDbType.BigInt, this.Id);
            dataLib.AddParameter("@ChargeId", SqlDbType.NVarChar, this.ChargeId);
            dataLib.AddParameter("@Code", SqlDbType.NVarChar, this.Code);
            dataLib.AddParameter("@DeclineCode", SqlDbType.NVarChar, this.DeclineCode);
            dataLib.AddParameter("@ErrorType", SqlDbType.NVarChar, this.ErrorType);
            dataLib.AddParameter("@Error", SqlDbType.NVarChar, this.Error);
            dataLib.AddParameter("@ErrorSubscription", SqlDbType.NVarChar, this.ErrorSubscription);
            dataLib.AddParameter("@Message", SqlDbType.NVarChar, this.Message);
            dataLib.AddParameter("@Parameter", SqlDbType.NVarChar, this.Parameter);
            dataLib.AddParameter("@Userid", SqlDbType.Int, this.Userid);
            dataLib.AddParameter("@UnitId", SqlDbType.BigInt, this.UnitId);
            dataLib.AddParameter("@DateAdded", SqlDbType.DateTime, this.DateAdded);
        }
        public override void AddSearchParams(ref ZWT.DbLib.IDataLib dataLib)
        {

        }
        #endregion
    }
}