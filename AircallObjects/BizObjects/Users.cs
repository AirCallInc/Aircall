using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace BizObjects
{
    public class Users:BizObject
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Image { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string PasswordUrl { get; set; }
        public bool IsLinkActive { get; set; }
        public DateTime ResetPasswordLinkExpireDate { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime UpdatedDate{get;set;}
        public DateTime DeletedDate { get; set; }

        #region "Constructors"
        public Users()
        {
        }
        public Users(ref DataRow drRow)
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
            dataLib.AddParameter("@RoleId", SqlDbType.Int, this.RoleId);
            dataLib.AddParameter("@FirstName", SqlDbType.NVarChar, this.FirstName);
            dataLib.AddParameter("@LastName", SqlDbType.NVarChar, this.LastName);
            dataLib.AddParameter("@UserName", SqlDbType.NVarChar, this.UserName);
            dataLib.AddParameter("@Password", SqlDbType.NVarChar, this.Password);
            dataLib.AddParameter("@Email", SqlDbType.NVarChar, this.Email);
            dataLib.AddParameter("@Image", SqlDbType.NVarChar, this.Image);
            dataLib.AddParameter("@IsActive", SqlDbType.Bit, this.IsActive);
            dataLib.AddParameter("@AddedDate", SqlDbType.DateTime, this.AddedDate);
        }
        public override void AddUpdateParams(ref ZWT.DbLib.IDataLib dataLib)
        {
            dataLib.AddParameter("@Id", SqlDbType.Int, this.Id);
            dataLib.AddParameter("@RoleId", SqlDbType.Int, this.RoleId);
            dataLib.AddParameter("@FirstName", SqlDbType.NVarChar, this.FirstName);
            dataLib.AddParameter("@LastName", SqlDbType.NVarChar, this.LastName);
            dataLib.AddParameter("@Password", SqlDbType.NVarChar, this.Password);
            dataLib.AddParameter("@Email", SqlDbType.NVarChar, this.Email);
            dataLib.AddParameter("@Image", SqlDbType.NVarChar, this.Image);
            dataLib.AddParameter("@IsActive", SqlDbType.Bit, this.IsActive);
            dataLib.AddParameter("@UpdatedDate", SqlDbType.DateTime, this.UpdatedDate);

        }
        public override void AddSearchParams(ref ZWT.DbLib.IDataLib dataLib)
        {

        }
        #endregion
    }
}
