using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace BizObjects
{
    public class ClientUnitSubscription : BizObject
    {
        public Int64 Id { get; set; }
        public int ClientId { get; set; }
        public int UnitId { get; set; }
        public string ClientUnitIds { get; set; }
        public int OrderId { get; set; }
        public string PaymentMethod { get; set; }
        public int CardId { get; set; }
        public string PONumber { get; set; }
        public string CheckNumbers { get; set; }
        public string CheckAmounts { get; set; }
        public string FrontImage { get; set; }
        public string BackImage { get; set; }
        public string AccountingNotes { get; set; }
        public decimal Amount { get; set; }
        public decimal PricePerMonth { get; set; }
        public DateTime PaymentDueDate { get; set; }
        public DateTime PaymentReceivedDate { get; set; }
        public string Status { get; set; }
        public int AddedBy { get; set; }
        public int AddedByType { get; set; }
        public DateTime AddedDate { get; set; }
        public int TotalUnits { get; set; }


        #region "Constructors"
        public ClientUnitSubscription()
        {
        }
        public ClientUnitSubscription(ref DataRow drRow)
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
            dataLib.AddParameter("@UnitId", SqlDbType.Int, this.UnitId);
            dataLib.AddParameter("@ClientUnitIds", SqlDbType.NVarChar, this.ClientUnitIds);
            dataLib.AddParameter("@OrderId", SqlDbType.Int, this.OrderId);
            dataLib.AddParameter("@PaymentMethod", SqlDbType.NVarChar, this.PaymentMethod);
            dataLib.AddParameter("@CardId", SqlDbType.Int, this.CardId);
            dataLib.AddParameter("@PONumber", SqlDbType.NVarChar, this.PONumber);
            dataLib.AddParameter("@CheckNumbers", SqlDbType.NVarChar, this.CheckNumbers);
            dataLib.AddParameter("@CheckAmounts", SqlDbType.NVarChar, this.CheckAmounts);
            dataLib.AddParameter("@FrontImage", SqlDbType.NVarChar, this.FrontImage);
            dataLib.AddParameter("@BackImage", SqlDbType.NVarChar, this.BackImage);
            dataLib.AddParameter("@AccountingNotes", SqlDbType.NVarChar, this.AccountingNotes);
            dataLib.AddParameter("@PricePerMonth", SqlDbType.Decimal, this.PricePerMonth);
            dataLib.AddParameter("@Amount", SqlDbType.Decimal, this.Amount);
            dataLib.AddParameter("@AddedBy", SqlDbType.Int, this.AddedBy);
            dataLib.AddParameter("@AddedByType", SqlDbType.Int, this.AddedByType);
            dataLib.AddParameter("@AddedDate", SqlDbType.DateTime, this.AddedDate);
            dataLib.AddParameter("@TotalUnits", SqlDbType.Int, this.TotalUnits);
        }

        public override void AddUpdateParams(ref ZWT.DbLib.IDataLib dataLib)
        {
            dataLib.AddParameter("@Id", SqlDbType.BigInt, this.Id);
            dataLib.AddParameter("@CardId", SqlDbType.Int, this.CardId);
            dataLib.AddParameter("@PONumber", SqlDbType.NVarChar, this.PONumber);
            dataLib.AddParameter("@CheckNumber", SqlDbType.NVarChar, this.CheckNumbers);
            dataLib.AddParameter("@FrontImage", SqlDbType.NVarChar, this.FrontImage);
            dataLib.AddParameter("@BackImage", SqlDbType.NVarChar, this.BackImage);
            dataLib.AddParameter("@AccountingNotes", SqlDbType.NVarChar, this.AccountingNotes);
            dataLib.AddParameter("@Amount", SqlDbType.Decimal, this.Amount);
            dataLib.AddParameter("@Status", SqlDbType.NVarChar, this.Status);
            dataLib.AddParameter("@AddedBy", SqlDbType.Int, this.AddedBy);
            dataLib.AddParameter("@AddedByType", SqlDbType.Int, this.AddedByType);
            dataLib.AddParameter("@AddedDate", SqlDbType.DateTime, this.AddedDate);
        }
        public override void AddSearchParams(ref ZWT.DbLib.IDataLib dataLib)
        {

        }
        #endregion
    }
}
