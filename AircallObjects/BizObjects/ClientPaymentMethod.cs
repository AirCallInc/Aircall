using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace BizObjects
{
    public class ClientPaymentMethod : BizObject
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string CardType { get; set; }
        public string NameOnCard { get; set; }
        public string CardNumber { get; set; }
        public int ExpiryMonth { get; set; }
        public int ExpiryYear { get; set; }
        public bool IsDefaultPayment { get; set; }
        public int AddedBy { get; set; }
        public int AddedByType { get; set; }
        public DateTime AddedDate { get; set; }
        public int UpdatedBy { get; set; }
        public int UpdatedByType { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool IsDeleted { get; set; }
        public string CustomerPaymentProfileId { get; set; }

        #region "Constructors"
        public ClientPaymentMethod()
        {
        }
        public ClientPaymentMethod(ref DataRow drRow)
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
            dataLib.AddParameter("@CardType", SqlDbType.NVarChar, this.CardType);
            dataLib.AddParameter("@NameOnCard", SqlDbType.NVarChar, this.NameOnCard);
            dataLib.AddParameter("@CardNumber", SqlDbType.NVarChar, this.CardNumber);
            dataLib.AddParameter("@ExpiryMonth", SqlDbType.SmallInt, this.ExpiryMonth);
            dataLib.AddParameter("@ExpiryYear", SqlDbType.Int, this.ExpiryYear);
            dataLib.AddParameter("@IsDefaultPayment", SqlDbType.Bit, this.IsDefaultPayment);
            dataLib.AddParameter("@AddedBy", SqlDbType.Int, this.AddedBy);
            dataLib.AddParameter("@AddedByType", SqlDbType.Int, this.AddedByType);
            dataLib.AddParameter("@AddedDate", SqlDbType.DateTime, this.AddedDate);
            dataLib.AddParameter("@CustomerPaymentProfileId", SqlDbType.NVarChar, this.CustomerPaymentProfileId);
        }

        public override void AddUpdateParams(ref ZWT.DbLib.IDataLib dataLib)
        {
            dataLib.AddParameter("@Id", SqlDbType.Int, this.Id);
            dataLib.AddParameter("@ClientId", SqlDbType.Int, this.ClientId);
            dataLib.AddParameter("@CardType", SqlDbType.NVarChar, this.CardType);
            dataLib.AddParameter("@NameOnCard", SqlDbType.NVarChar, this.NameOnCard);
            //dataLib.AddParameter("@CardNumber", SqlDbType.NVarChar, this.CardNumber);
            dataLib.AddParameter("@ExpiryMonth", SqlDbType.SmallInt, this.ExpiryMonth);
            dataLib.AddParameter("@ExpiryYear", SqlDbType.Int, this.ExpiryYear);
            dataLib.AddParameter("@IsDefaultPayment", SqlDbType.Bit, this.IsDefaultPayment);
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