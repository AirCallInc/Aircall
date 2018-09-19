using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace BizObjects
{
    public class Parts : BizObject
    {
        public int Id { get; set; }
        public string InventoryType { get; set; }
        public int DailyPartListMasterId { get; set; }
        public string Name { get; set; }
        public string Size { get; set; }
        public string Description { get; set; }
        public int InboundQuantity { get; set; }
        public int ReceivedQuantity { get; set; }
        public int TotalAcquiredQuantity { get; set; }
        public int InStockQuantity { get; set; }
        public int ReservedQuantity { get; set; }
        public decimal PurchasedPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public int MinReorderQuantity { get; set; }
        public int ReorderQuantity { get; set; }
        public bool Status { get; set; }
        public bool IsDefault { get; set; }
        public bool IsDeleted { get; set; }
        public int AddedBy { get; set; }
        public int AddedByType { get; set; }
        public DateTime AddedDate { get; set; }
        public int UpdatedBy { get; set; }
        public int UpdatedByType { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int DeletedBy { get; set; }
        public int DeletedByType { get; set; }
        public DateTime DeletedDate { get; set; }

        #region "Constructors"
        public Parts()
        {
        }
        public Parts(ref DataRow drRow)
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
            dataLib.AddParameter("@InventoryType", SqlDbType.NVarChar,this.InventoryType);
            dataLib.AddParameter("@DailyPartListMasterId", SqlDbType.Int,this.DailyPartListMasterId);
            dataLib.AddParameter("@Name", SqlDbType.NVarChar,this.Name);
            dataLib.AddParameter("@Size", SqlDbType.NVarChar,this.Size);
            dataLib.AddParameter("@Description", SqlDbType.NVarChar,this.Description);
            dataLib.AddParameter("@InboundQuantity", SqlDbType.Int,this.InboundQuantity);
            dataLib.AddParameter("@ReceivedQuantity", SqlDbType.Int,this.ReceivedQuantity);
            dataLib.AddParameter("@TotalAcquiredQuantity", SqlDbType.Int,this.TotalAcquiredQuantity);
            dataLib.AddParameter("@InStockQuantity", SqlDbType.Int,this.InStockQuantity);
            dataLib.AddParameter("@ReservedQuantity", SqlDbType.Int,this.ReservedQuantity);
            dataLib.AddParameter("@PurchasedPrice",SqlDbType.Decimal ,this.PurchasedPrice);
            dataLib.AddParameter("@SellingPrice", SqlDbType.Decimal, this.SellingPrice);
            dataLib.AddParameter("@MinReorderQuantity", SqlDbType.Int,this.MinReorderQuantity);
            dataLib.AddParameter("@ReorderQuantity", SqlDbType.Int,this.ReorderQuantity);
            dataLib.AddParameter("@Status", SqlDbType.Bit,this.Status);
            dataLib.AddParameter("@IsDefault", SqlDbType.Bit, this.IsDefault);
            dataLib.AddParameter("@AddedBy", SqlDbType.Int,this.AddedBy);
            dataLib.AddParameter("@AddedByType", SqlDbType.Int,this.AddedByType);
            dataLib.AddParameter("@AddedDate", SqlDbType.DateTime,this.AddedDate);
        }

        public override void AddUpdateParams(ref ZWT.DbLib.IDataLib dataLib)
        {
            dataLib.AddParameter("@Id", SqlDbType.Int,this.Id);
            dataLib.AddParameter("@InventoryType", SqlDbType.NVarChar,this.InventoryType);
            dataLib.AddParameter("@DailyPartListMasterId", SqlDbType.Int,this.DailyPartListMasterId);
            dataLib.AddParameter("@Name", SqlDbType.NVarChar,this.Name);
            dataLib.AddParameter("@Size", SqlDbType.NVarChar,this.Size);
            dataLib.AddParameter("@Description", SqlDbType.NVarChar,this.Description);
            dataLib.AddParameter("@InboundQuantity", SqlDbType.Int,this.InboundQuantity);
            dataLib.AddParameter("@ReceivedQuantity", SqlDbType.Int,this.ReceivedQuantity);
            dataLib.AddParameter("@TotalAcquiredQuantity", SqlDbType.Int,this.TotalAcquiredQuantity);
            dataLib.AddParameter("@InStockQuantity", SqlDbType.Int,this.InStockQuantity);
            dataLib.AddParameter("@ReservedQuantity", SqlDbType.Int,this.ReservedQuantity);
            dataLib.AddParameter("@PurchasedPrice", SqlDbType.Decimal,this.PurchasedPrice);
            dataLib.AddParameter("@SellingPrice", SqlDbType.Decimal, this.SellingPrice);
            dataLib.AddParameter("@MinReorderQuantity", SqlDbType.Int,this.MinReorderQuantity);
            dataLib.AddParameter("@ReorderQuantity", SqlDbType.Int,this.ReorderQuantity);
            dataLib.AddParameter("@Status", SqlDbType.Bit,this.Status);
            dataLib.AddParameter("@IsDefault", SqlDbType.Bit, this.IsDefault);
            dataLib.AddParameter("@UpdatedBy", SqlDbType.Int,this.UpdatedBy);
            dataLib.AddParameter("@UpdatedByType", SqlDbType.Int,this.UpdatedByType);
            dataLib.AddParameter("@UpdatedDate", SqlDbType.DateTime,this.UpdatedDate);
        }

        public override void AddSearchParams(ref ZWT.DbLib.IDataLib dataLib)
        {

        }
        #endregion
    }
}

