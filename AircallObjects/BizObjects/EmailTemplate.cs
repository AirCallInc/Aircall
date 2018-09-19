using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace BizObjects
{ 
    public class EmailTemplate : BizObject
    {
        public int Id{get; set;}
        public string Name{get; set;}
        public string FromEmail{get; set;}
        public string CCEmails { get; set; }
        public string EmailTemlateSubject { get; set; }
        public string EmailBody { get; set; }
        public string AvailableTags { get; set; }
        public int Status { get; set; }
        public int AddedBy { get; set; }
        public int AddedByType { get; set; }
        public DateTime AddedDate { get; set; }
        public int UpdatedBy { get; set; }
        public int UpdatedByType { get; set; }
        public DateTime UpdatedDate { get; set; }

         #region "Constructors"
        public EmailTemplate()
        {
        }
        public EmailTemplate(ref DataRow drRow)
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
            dataLib.AddParameter("@id", SqlDbType.Int, this.Id);
            dataLib.AddParameter("@Name", SqlDbType.NVarChar, this.Name);
            dataLib.AddParameter("@FromEmail", SqlDbType.NVarChar, this.FromEmail);
            dataLib.AddParameter("@EmailTemlateSubject", SqlDbType.NVarChar, this.EmailTemlateSubject);
            dataLib.AddParameter("@EmailBody", SqlDbType.NVarChar, this.EmailBody);
            dataLib.AddParameter("@Status", SqlDbType.Int, this.Status);
            dataLib.AddParameter("@AddedBy", SqlDbType.Int, this.AddedBy);
            dataLib.AddParameter("@AddedByType", SqlDbType.Int, this.AddedByType);
            dataLib.AddParameter("@AddedDate", SqlDbType.DateTime, this.AddedDate);
            dataLib.AddParameter("@UpdatedBy", SqlDbType.Int, this.UpdatedBy);
            dataLib.AddParameter("@UpdatedByType", SqlDbType.Int, this.UpdatedByType);
            dataLib.AddParameter("@UpdatedDate", SqlDbType.DateTime, this.UpdatedDate);
        }

        public override void AddUpdateParams(ref ZWT.DbLib.IDataLib dataLib)
        {
            dataLib.AddParameter("@Id", SqlDbType.Int, this.Id);
            dataLib.AddParameter("@EmailTemlateSubject",SqlDbType.NVarChar,this.EmailTemlateSubject);
            dataLib.AddParameter("@FromEmail", SqlDbType.NVarChar, this.FromEmail);
            dataLib.AddParameter("@CCEmails", SqlDbType.NVarChar, this.CCEmails);
            dataLib.AddParameter("@EmailBody", SqlDbType.NVarChar, this.EmailBody);
            dataLib.AddParameter("@UpdatedBy",SqlDbType.Int,this.UpdatedBy);
            dataLib.AddParameter("@UpdatedByType",SqlDbType.Int,this.UpdatedByType);
            dataLib.AddParameter("@UpdatedDate",SqlDbType.DateTime,this.UpdatedDate);
        }

        public override void AddSearchParams(ref ZWT.DbLib.IDataLib dataLib)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
