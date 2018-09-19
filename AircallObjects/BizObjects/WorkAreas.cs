using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace BizObjects
{
    public class WorkAreas:BizObject
    {
        public int Id {get; set;} 
        public int AreaId {get; set;} 
        public int ZipCodeId {get; set;} 

        #region "Constructors"
        public WorkAreas()
        {
        }
        public WorkAreas(ref DataRow drRow)
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
            dataLib.AddParameter("@AreaId", SqlDbType.Int,this.AreaId);
            dataLib.AddParameter("@ZipCodeId", SqlDbType.Int,this.ZipCodeId);
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