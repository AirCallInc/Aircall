using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace BizObjects
{
    public class CMSPages:BizObject
    {
        public int Id{get;set;}
        public string PageTitle { get; set; }
        public string MenuTitle { get; set; }
        public string URL { get; set; }
        public string Description { get; set; }
        public string BannerImage { get; set; }
        public string MetaTitle { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string AdditionalMeta { get; set; }
        public bool Status { get; set; }
        public bool IsDeleted { get; set; }
        public int AddedBy{get;set;}
        public int AddedByType{ get; set; }
        public DateTime AddedDate{get;set;}
        public int UpdatedBy{get;set;}
        public int UpdatedByType { get; set; }
        public DateTime UpdatedDate{get;set;}
        public int DeletedBy{get;set;}
        public int DeletedByType { get; set; }
        public DateTime DeletedDate { get; set; }
        
        #region "Constructors"
        public CMSPages()
        {
        }
        public CMSPages(ref DataRow drRow)
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
            dataLib.AddParameter("@PageTitle", SqlDbType.NVarChar, this.PageTitle);
            dataLib.AddParameter("@MenuTitle", SqlDbType.NVarChar, this.MenuTitle);
            dataLib.AddParameter("@URL", SqlDbType.NVarChar, this.URL);
            dataLib.AddParameter("@Description", SqlDbType.NVarChar, this.Description);
            dataLib.AddParameter("@BannerImage",SqlDbType.NVarChar,this.BannerImage);
            dataLib.AddParameter("@MetaTitle", SqlDbType.NVarChar, this.MetaTitle);
            dataLib.AddParameter("@MetaKeywords", SqlDbType.NVarChar, this.MetaKeywords);
            dataLib.AddParameter("@MetaDescription", SqlDbType.NVarChar, this.MetaDescription);
            dataLib.AddParameter("@AdditionalMeta", SqlDbType.NVarChar, this.AdditionalMeta);
            dataLib.AddParameter("@Status", SqlDbType.Bit, this.Status);
            dataLib.AddParameter("@AddedBy", SqlDbType.Int, this.AddedBy);
            dataLib.AddParameter("@AddedByType", SqlDbType.Int, this.AddedByType);
            dataLib.AddParameter("@AddedDate", SqlDbType.DateTime, this.AddedDate);
        }
        public override void AddUpdateParams(ref ZWT.DbLib.IDataLib dataLib)
        {
            dataLib.AddParameter("@Id", SqlDbType.Int, this.Id);
            dataLib.AddParameter("@PageTitle", SqlDbType.NVarChar, this.PageTitle);
            dataLib.AddParameter("@MenuTitle", SqlDbType.NVarChar, this.MenuTitle);
            dataLib.AddParameter("@URL", SqlDbType.NVarChar, this.URL);
            dataLib.AddParameter("@Description", SqlDbType.NVarChar, this.Description);
            dataLib.AddParameter("@BannerImage", SqlDbType.NVarChar, this.BannerImage);
            dataLib.AddParameter("@MetaTitle", SqlDbType.NVarChar, this.MetaTitle);
            dataLib.AddParameter("@MetaKeywords", SqlDbType.NVarChar, this.MetaKeywords);
            dataLib.AddParameter("@MetaDescription", SqlDbType.NVarChar, this.MetaDescription);
            dataLib.AddParameter("@AdditionalMeta", SqlDbType.NVarChar, this.AdditionalMeta);
            dataLib.AddParameter("@Status", SqlDbType.Bit, this.Status);
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
