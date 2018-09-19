using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace BizObjects
{
    public class ContactRequest:BizObject
    {
        public int Id { get; set; }
        public string Name{ get; set; }
        public string Email{ get; set; }
        public string PhoneNumber{ get; set; }
        public string Message{ get; set; }
        public DateTime RequestDate { get; set; }
        public string ResponseEmailSubject{ get; set; }
        public string ResponseBody{ get; set; }
        public int ResponseBy{ get; set; }
        public DateTime ResponseDate{ get; set; }
        public bool Status{ get; set; }

        #region "Constructors"
        public ContactRequest()
        {
        }
        public ContactRequest(ref DataRow drRow)
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
            dataLib.AddParameter("@Name", SqlDbType.NVarChar, this.Name);
            dataLib.AddParameter("@Email", SqlDbType.NVarChar, this.Email);
            dataLib.AddParameter("@PhoneNumber", SqlDbType.NVarChar, this.PhoneNumber);
            dataLib.AddParameter("@Message", SqlDbType.NVarChar, this.Message);
            dataLib.AddParameter("@RequestDate", SqlDbType.DateTime, this.RequestDate);
        }

        public override void AddUpdateParams(ref ZWT.DbLib.IDataLib dataLib)
        {
            dataLib.AddParameter("@Id", SqlDbType.Int, this.Id);
            dataLib.AddParameter("@ResponseEmailSub", SqlDbType.NVarChar, this.ResponseEmailSubject);
            dataLib.AddParameter("@Body", SqlDbType.NVarChar, this.ResponseBody);
            dataLib.AddParameter("@ResponseBy", SqlDbType.Int, this.ResponseBy);
            dataLib.AddParameter("@ResponseDate", SqlDbType.DateTime, this.ResponseDate);
        }
        public override void AddSearchParams(ref ZWT.DbLib.IDataLib dataLib)
        {

        }
        #endregion
    }
}
