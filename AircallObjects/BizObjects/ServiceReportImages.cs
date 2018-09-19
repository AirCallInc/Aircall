using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace BizObjects
{
    public class ServiceReportImages:BizObject
    {
        public int Id { get; set; }
        public long ServiceReportId { get; set; }
        public string ServiceImage { get; set; }

        #region "Constructors"
        public ServiceReportImages()
        {
        }
        public ServiceReportImages(ref DataRow drRow)
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
            dataLib.AddParameter("@ServiceReportId", SqlDbType.BigInt, this.ServiceReportId);
            dataLib.AddParameter("@ServiceImage", SqlDbType.NVarChar, this.ServiceImage);
        }

        public override void AddUpdateParams(ref ZWT.DbLib.IDataLib dataLib)
        {
            dataLib.AddParameter("@Id", SqlDbType.Int, this.Id);
            dataLib.AddParameter("@ServiceReportId", SqlDbType.BigInt, this.ServiceReportId);
            dataLib.AddParameter("@ServiceImage", SqlDbType.NVarChar, this.ServiceImage);
        }

        public override void AddSearchParams(ref ZWT.DbLib.IDataLib dataLib)
        {

        }
        #endregion
    }
}
