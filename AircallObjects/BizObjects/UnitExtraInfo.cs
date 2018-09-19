using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace BizObjects
{
    public class UnitExtraInfo:BizObject
    {
        public int Id{get;set;}
        public int UnitId{get;set;}
        public string ExtraInfoType{get;set;}
        public int PartId{get;set;}
        public Nullable<bool> LocationOfPart { get; set; }
        public long ClientUnitPartId { get; set; }

        #region "Constructors"
        public UnitExtraInfo()
        {
        }
        public UnitExtraInfo(ref DataRow drRow)
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
            dataLib.AddParameter("@UnitId", SqlDbType.Int, this.UnitId);
            dataLib.AddParameter("@ExtraInfoType", SqlDbType.NVarChar, this.ExtraInfoType);
            dataLib.AddParameter("@PartId", SqlDbType.Int, this.PartId);
            dataLib.AddParameter("@LocationOfPart", SqlDbType.Bit, this.LocationOfPart);
            dataLib.AddParameter("@ClientUnitPartId", SqlDbType.BigInt, this.ClientUnitPartId);
        }

        public override void AddUpdateParams(ref ZWT.DbLib.IDataLib dataLib)
        {
            dataLib.AddParameter("@Id", SqlDbType.Int, this.Id);
            dataLib.AddParameter("@UnitId", SqlDbType.Int, this.UnitId);
            dataLib.AddParameter("@ExtraInfoType", SqlDbType.NVarChar, this.ExtraInfoType);
            dataLib.AddParameter("@PartId", SqlDbType.Int, this.PartId);
            dataLib.AddParameter("@LocationOfPart", SqlDbType.Bit, this.LocationOfPart);
        }

        public override void AddSearchParams(ref ZWT.DbLib.IDataLib dataLib)
        {

        }
        #endregion
    }
}
