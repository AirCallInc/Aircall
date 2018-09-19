using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace BizObjects
{
    public class PartnerTicketConversation:BizObject
    {
      public int Id {get; set;}
      public int TicketId { get; set; }
      public string Message { get; set; }
      public int SubmittedBy { get; set; }
      public int SubmittedByType { get; set; }
      public DateTime MessageDate { get; set; }
      

        #region "Constructors"
        public PartnerTicketConversation()
        {
        }
        public PartnerTicketConversation(ref DataRow drRow)
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
            dataLib.AddParameter("@TicketId", SqlDbType.Int, this.TicketId);
            dataLib.AddParameter("@Message", SqlDbType.NVarChar, this.Message);
            dataLib.AddParameter("@SubmittedBy", SqlDbType.Int, this.SubmittedBy);
            dataLib.AddParameter("@SubmittedByType", SqlDbType.Int, this.SubmittedByType);
            dataLib.AddParameter("@MessageDate", SqlDbType.DateTime, this.MessageDate);
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
