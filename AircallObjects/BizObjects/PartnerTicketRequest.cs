using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace BizObjects
{
    public class PartnerTicketRequest:BizObject
    {
      public int Id {get; set;}
      public int PartnerId { get; set; }
      public string TicketType { get; set; }
      public string Subject { get; set; }
      public string Notes { get; set; }
      public DateTime AddedDate {get; set;}
      public bool Status { get; set; }
      

        #region "Constructors"
        public PartnerTicketRequest()
        {
        }
        public PartnerTicketRequest(ref DataRow drRow)
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
            dataLib.AddParameter("@PartnerId", SqlDbType.Int, this.PartnerId);
            dataLib.AddParameter("@TicketType", SqlDbType.NVarChar, this.TicketType);
            dataLib.AddParameter("@Subject", SqlDbType.NVarChar, this.Subject);
            dataLib.AddParameter("@Notes", SqlDbType.NVarChar, this.Notes);
            dataLib.AddParameter("@AddedDate", SqlDbType.DateTime, this.AddedDate);
        }

        public override void AddUpdateParams(ref ZWT.DbLib.IDataLib dataLib)
        {
            dataLib.AddParameter("@Id", SqlDbType.Int, this.Id);
            dataLib.AddParameter("@PartnerId", SqlDbType.Int, this.PartnerId);
            dataLib.AddParameter("@TicketType", SqlDbType.NVarChar, this.TicketType);
            dataLib.AddParameter("@Subject", SqlDbType.NVarChar, this.Subject);
            dataLib.AddParameter("@Notes", SqlDbType.NVarChar, this.Notes);
            dataLib.AddParameter("@AddedDate", SqlDbType.DateTime, this.AddedDate);
            dataLib.AddParameter("@Status", SqlDbType.Bit, this.Status);
    
        }

        public override void AddSearchParams(ref ZWT.DbLib.IDataLib dataLib)
        {
           
        }
        #endregion
    }
}
