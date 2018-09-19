using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace BizObjects
{
    public class ClientAddress : BizObject
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string Address { get; set; }
        public int State { get; set; }
        public int City { get; set; }
        public string ZipCode { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public bool IsDefaultAddress { get; set; }
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
        public string CustomerAddressId { get; set; }

        #region "Constructors"
        public ClientAddress()
        {
        }
        public ClientAddress(ref DataRow drRow)
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
            dataLib.AddParameter("@Address", SqlDbType.NVarChar, this.Address);
            dataLib.AddParameter("@State", SqlDbType.Int, this.State);
            dataLib.AddParameter("@City", SqlDbType.Int, this.City);
            dataLib.AddParameter("@ZipCode", SqlDbType.NVarChar, this.ZipCode);
            dataLib.AddParameter("@Latitude", SqlDbType.Decimal, this.Latitude);
            dataLib.AddParameter("@Longitude", SqlDbType.Decimal, this.Longitude);
            dataLib.AddParameter("@IsDefaultAddress", SqlDbType.Bit, this.IsDefaultAddress);
            dataLib.AddParameter("@AddedBy", SqlDbType.Int, this.AddedBy);
            dataLib.AddParameter("@AddedByType", SqlDbType.Int, this.AddedByType);
            dataLib.AddParameter("@AddedDate", SqlDbType.DateTime, this.AddedDate);
            dataLib.AddParameter("@CustomerAddressId", SqlDbType.NVarChar, this.CustomerAddressId);
        }

        public override void AddUpdateParams(ref ZWT.DbLib.IDataLib dataLib)
        {
            dataLib.AddParameter("@Id", SqlDbType.Int, this.Id);
            dataLib.AddParameter("@ClientId", SqlDbType.Int, this.ClientId);
            dataLib.AddParameter("@Address", SqlDbType.NVarChar, this.Address);
            dataLib.AddParameter("@State", SqlDbType.Int, this.State);
            dataLib.AddParameter("@City", SqlDbType.Int, this.City);
            dataLib.AddParameter("@ZipCode", SqlDbType.NVarChar, this.ZipCode);
            dataLib.AddParameter("@IsDefaultAddress", SqlDbType.Bit, this.IsDefaultAddress);
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