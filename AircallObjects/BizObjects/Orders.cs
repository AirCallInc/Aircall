using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
namespace BizObjects
{
    public class Orders : BizObject
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; }
        public string OrderType { get; set; }
        public int ClientId { get; set; }
        public string Description { get; set; }
        public decimal OrderAmount { get; set; }
        public string ChargeBy { get; set; }
        public string ChequeNo { get; set; }
        //public string BankName { get; set; }
        //public string RoutingNo { get; set; }
        public DateTime ChequeDate { get; set; }
        public string NameOnCard { get; set; }
        public string CardType { get; set; }
        public string CardNumber { get; set; }
        public int ExpirationMonth { get; set; }
        public int ExpirationYear { get; set; }
        public string CCEmail { get; set; }
        public bool IsEmailToClient { get; set; }
        public string CustomerRecommendation { get; set; }
        public string ClientSignature { get; set; }
        public string AccountingNotes { get; set; }
        public string ChqueImageFront { get; set; }
        public string ChequeImageBack { get; set; }
        public int AddedBy { get; set; }
        public int AddedByType { get; set; }
        public DateTime AddedDate { get; set; }
        public int UpdatedBy { get; set; }
        public int UpdatedByType { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool IsDeleted { get; set; }
        public int DeletedBy { get; set; }
        public int DeletedByType { get; set; }
        public DateTime DeletedDate { get; set; }
        public override void AddInsertParams(ref ZWT.DbLib.IDataLib dataLib)
        {
            dataLib.AddParameter("@OrderNumber", SqlDbType.NVarChar, this.OrderNumber);
            dataLib.AddParameter("@OrderType", SqlDbType.NVarChar, this.OrderType);
            dataLib.AddParameter("@ClientId", SqlDbType.Int, this.ClientId);
            dataLib.AddParameter("@Description", SqlDbType.NVarChar, this.Description);
            dataLib.AddParameter("@OrderAmount", SqlDbType.Decimal, this.OrderAmount);
            dataLib.AddParameter("@ChargeBy", SqlDbType.NVarChar, this.ChargeBy);
            dataLib.AddParameter("@ChequeNo", SqlDbType.NVarChar, this.ChequeNo);
            //dataLib.AddParameter("@BankName", SqlDbType.NVarChar, this.BankName);
            //dataLib.AddParameter("@RoutingNo", SqlDbType.NVarChar, this.RoutingNo);
            dataLib.AddParameter("@ChequeDate", SqlDbType.DateTime, this.ChequeDate);
            dataLib.AddParameter("@NameOnCard", SqlDbType.NVarChar, this.NameOnCard);
            dataLib.AddParameter("@CardType", SqlDbType.NVarChar, this.CardType);
            dataLib.AddParameter("@CardNumber", SqlDbType.NVarChar, this.CardNumber);
            dataLib.AddParameter("@ExpirationMonth", SqlDbType.SmallInt, this.ExpirationMonth);
            dataLib.AddParameter("@ExpirationYear", SqlDbType.Int, this.ExpirationYear);
            dataLib.AddParameter("@CCEmail", SqlDbType.NVarChar, this.CCEmail);
            dataLib.AddParameter("@IsEmailToClient", SqlDbType.Bit, this.IsEmailToClient);
            dataLib.AddParameter("@CustomerRecommendation", SqlDbType.NVarChar, this.CustomerRecommendation);
            dataLib.AddParameter("@ClientSignature", SqlDbType.NVarChar, this.ClientSignature);
            dataLib.AddParameter("@AccountingNotes", SqlDbType.NVarChar, this.AccountingNotes);
            dataLib.AddParameter("@ChqueImageFront", SqlDbType.NVarChar, this.ChqueImageFront);
            dataLib.AddParameter("@ChequeImageBack", SqlDbType.NVarChar, this.ChequeImageBack);
            dataLib.AddParameter("@AddedBy", SqlDbType.Int, this.AddedBy);
            dataLib.AddParameter("@AddedByType", SqlDbType.Int, this.AddedByType);
            dataLib.AddParameter("@AddedDate", SqlDbType.DateTime, this.AddedDate);            
        }
        public override void AddUpdateParams(ref ZWT.DbLib.IDataLib dataLib)
        {
            dataLib.AddParameter("@Id", SqlDbType.Int, this.Id);
            dataLib.AddParameter("@OrderNumber", SqlDbType.NVarChar, this.OrderNumber);
            dataLib.AddParameter("@OrderType", SqlDbType.NVarChar, this.OrderType);
            dataLib.AddParameter("@ClientId", SqlDbType.Int, this.ClientId);
            dataLib.AddParameter("@Description", SqlDbType.NVarChar, this.Description);
            dataLib.AddParameter("@OrderAmount", SqlDbType.Decimal, this.OrderAmount);
            dataLib.AddParameter("@ChargeBy", SqlDbType.NVarChar, this.ChargeBy);
            dataLib.AddParameter("@ChequeNo", SqlDbType.NVarChar, this.ChequeNo);
            //dataLib.AddParameter("@BankName", SqlDbType.NVarChar, this.BankName);
            //dataLib.AddParameter("@RoutingNo", SqlDbType.NVarChar, this.RoutingNo);
            dataLib.AddParameter("@ChequeDate", SqlDbType.DateTime, this.ChequeDate);
            dataLib.AddParameter("@NameOnCard", SqlDbType.NVarChar, this.NameOnCard);
            dataLib.AddParameter("@CardType", SqlDbType.NVarChar, this.CardType);
            dataLib.AddParameter("@CardNumber", SqlDbType.NVarChar, this.CardNumber);
            dataLib.AddParameter("@ExpirationMonth", SqlDbType.SmallInt, this.ExpirationMonth);
            dataLib.AddParameter("@ExpirationYear", SqlDbType.Int, this.ExpirationYear);
            dataLib.AddParameter("@CCEmail", SqlDbType.NVarChar, this.CCEmail);
            dataLib.AddParameter("@IsEmailToClient", SqlDbType.Bit, this.IsEmailToClient);
            dataLib.AddParameter("@CustomerRecommendation", SqlDbType.NVarChar, this.CustomerRecommendation);
            dataLib.AddParameter("@ClientSignature", SqlDbType.NVarChar, this.ClientSignature);
            dataLib.AddParameter("@AccountingNotes", SqlDbType.NVarChar, this.AccountingNotes);
            dataLib.AddParameter("@ChqueImageFront", SqlDbType.NVarChar, this.ChqueImageFront);
            dataLib.AddParameter("@ChequeImageBack", SqlDbType.NVarChar, this.ChequeImageBack);
            dataLib.AddParameter("@AddedBy", SqlDbType.Int, this.AddedBy);
            dataLib.AddParameter("@AddedByType", SqlDbType.Int, this.AddedByType);
            dataLib.AddParameter("@AddedDate", SqlDbType.DateTime, this.AddedDate);
            dataLib.AddParameter("@UpdatedBy", SqlDbType.Int, this.UpdatedBy);
            dataLib.AddParameter("@UpdatedByType", SqlDbType.Int, this.UpdatedByType);
            dataLib.AddParameter("@UpdatedDate", SqlDbType.DateTime, this.UpdatedDate);
            dataLib.AddParameter("@IsDeleted", SqlDbType.Bit, this.IsDeleted);
            dataLib.AddParameter("@DeletedBy", SqlDbType.Int, this.DeletedBy);
            dataLib.AddParameter("@DeletedByType", SqlDbType.Int, this.DeletedByType);
            dataLib.AddParameter("@DeletedDate", SqlDbType.DateTime, this.DeletedDate);
        }
        protected override void _LoadFromDb(ref DataRow drRow)
        {
        }
        public override void AddSearchParams(ref ZWT.DbLib.IDataLib dataLib)
        {
        }
    }
}
