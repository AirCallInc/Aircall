using System;
using System.Data;
using ZWT.DbLib;

namespace BizObjects
{
    public class News : BizObject
    {
        public int Id { get; set; }
        public string NewsTitle{ get; set; }
        public string NewsUrl { get; set; }
        public string NewsHeading { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public string MetaTitle { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string AdditionalMeta { get; set; }
        public int AddedBy { get; set; }
        public int AddedByType { get; set; }
        public DateTime AddedDate { get; set; }
        public int UpdatedBy { get; set; }
        public int UpdatedByType { get; set; }
        public DateTime UpdatedDate { get; set; }
        public DateTime PublishDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int DeletedBy { get; set; }
        public int DeletedByType { get; set; }
        public DateTime DeletedDate { get; set; }

        #region "Constructors"
        public News()
        {
        }
        public News(ref DataRow drRow)
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
            dataLib.AddParameter("@NewsTitle", SqlDbType.NVarChar, this.NewsTitle);
            dataLib.AddParameter("@NewsUrl", SqlDbType.NVarChar, this.NewsUrl);
            dataLib.AddParameter("@NewsHeading", SqlDbType.NVarChar, this.NewsHeading);
            dataLib.AddParameter("@ShortDescription", SqlDbType.NVarChar, this.ShortDescription);
            dataLib.AddParameter("@Description", SqlDbType.NVarChar, this.Description);
            dataLib.AddParameter("@MetaTitle", SqlDbType.NVarChar, this.MetaTitle);
            dataLib.AddParameter("@MetaKeywords", SqlDbType.NVarChar, this.MetaKeywords);
            dataLib.AddParameter("@MetaDescription", SqlDbType.NVarChar, this.MetaDescription);
            dataLib.AddParameter("@AdditionalMeta", SqlDbType.NVarChar, this.AdditionalMeta);
            dataLib.AddParameter("@AddedBy", SqlDbType.Int, this.AddedBy);
            dataLib.AddParameter("@AddedByType", SqlDbType.Int, this.AddedByType);
            dataLib.AddParameter("@AddedDate", SqlDbType.DateTime, this.AddedDate);
            dataLib.AddParameter("@IsActive", SqlDbType.Bit, this.IsActive);
            dataLib.AddParameter("@PublishDate", SqlDbType.DateTime, this.PublishDate);
        }

        public override void AddUpdateParams(ref ZWT.DbLib.IDataLib dataLib)
        {
            dataLib.AddParameter("@Id", SqlDbType.Int, this.Id);
            dataLib.AddParameter("@NewsTitle", SqlDbType.NVarChar, this.NewsTitle);
            dataLib.AddParameter("@NewsUrl", SqlDbType.NVarChar, this.NewsUrl);
            dataLib.AddParameter("@NewsHeading", SqlDbType.NVarChar, this.NewsHeading);
            dataLib.AddParameter("@ShortDescription", SqlDbType.NVarChar, this.ShortDescription);
            dataLib.AddParameter("@Description", SqlDbType.NVarChar, this.Description);
            dataLib.AddParameter("@MetaTitle", SqlDbType.NVarChar, this.MetaTitle);
            dataLib.AddParameter("@MetaKeywords", SqlDbType.NVarChar, this.MetaKeywords);
            dataLib.AddParameter("@MetaDescription", SqlDbType.NVarChar, this.MetaDescription);
            dataLib.AddParameter("@AdditionalMeta", SqlDbType.NVarChar, this.AdditionalMeta);
            dataLib.AddParameter("@IsActive", SqlDbType.Bit, this.IsActive);
            dataLib.AddParameter("@PublishDate", SqlDbType.DateTime, this.PublishDate);
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
