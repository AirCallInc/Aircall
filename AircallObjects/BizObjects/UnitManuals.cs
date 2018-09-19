using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace BizObjects
{
    public class UnitManuals : BizObject
    {
        public int Id { get; set; }
        public int UnitsId { get; set; }
        public string ManualFileName { get; set; }

        #region "Constructors"
        public UnitManuals()
        {
        }
        public UnitManuals(ref DataRow drRow)
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
            dataLib.AddParameter("@UnitsId", SqlDbType.Int, this.UnitsId);
            dataLib.AddParameter("@ManualFileName", SqlDbType.NVarChar, this.ManualFileName);
        }

        public override void AddUpdateParams(ref ZWT.DbLib.IDataLib dataLib)
        {
            dataLib.AddParameter("@Id", SqlDbType.Int, this.Id);
            dataLib.AddParameter("@UnitsId", SqlDbType.Int, this.UnitsId);
            dataLib.AddParameter("@ManualFileName", SqlDbType.NVarChar, this.ManualFileName);
        }

        public override void AddSearchParams(ref ZWT.DbLib.IDataLib dataLib)
        {

        }
        #endregion
    }
}