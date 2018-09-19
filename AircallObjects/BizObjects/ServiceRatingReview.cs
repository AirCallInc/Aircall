using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
namespace BizObjects
{
    public class ServiceRatingReview : BizObject
    {
        public int Id { get; set; }
        public Int64 ServiceId { get; set; }
        public decimal Rate { get; set; }
        public string Review { get; set; }
        public DateTime ReviewDate { get; set; }
        public string EmployeNotes { get; set; }
        public DateTime NotesAddedDate { get; set; }
        public override void AddInsertParams(ref ZWT.DbLib.IDataLib dataLib)
        {
            dataLib.AddParameter("@ServiceId", SqlDbType.BigInt, this.ServiceId);
            dataLib.AddParameter("@Rate", SqlDbType.Float, this.Rate);
            dataLib.AddParameter("@Review", SqlDbType.NVarChar, this.Review);
            dataLib.AddParameter("@ReviewDate", SqlDbType.DateTime, this.ReviewDate);            
        }
        public override void AddUpdateParams(ref ZWT.DbLib.IDataLib dataLib)
        {
            dataLib.AddParameter("@Id", SqlDbType.Int, this.Id);
            dataLib.AddParameter("@ServiceId", SqlDbType.BigInt, this.ServiceId);
            dataLib.AddParameter("@Rate", SqlDbType.Float, this.Rate);
            dataLib.AddParameter("@Review", SqlDbType.NVarChar, this.Review);
            dataLib.AddParameter("@ReviewDate", SqlDbType.DateTime, this.ReviewDate);
            dataLib.AddParameter("@EmployeNotes", SqlDbType.NVarChar, this.EmployeNotes);
            dataLib.AddParameter("@NotesAddedDate", SqlDbType.DateTime, this.NotesAddedDate);
        }
        protected override void _LoadFromDb(ref DataRow drRow)
        {
        }
        public override void AddSearchParams(ref ZWT.DbLib.IDataLib dataLib)
        {
        }
    }
}
