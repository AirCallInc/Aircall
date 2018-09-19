using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace BizObjects
{
    public class Employee:BizObject
    {
        public int Id {get; set;} 
        public int RoleId {get; set;} 
        public string FirstName {get; set;} 
        public string LastName {get; set;} 
        public string UserName {get; set;} 
        public string Email {get; set;} 
        public string Password {get; set;} 
        public string Image {get; set;} 
        public string Address {get; set;} 
        public string City {get; set;} 
        public int StateId {get; set;} 
        public string ZipCode {get; set;} 
        public string PhoneNumber {get; set;} 
        public string MobileNumber {get; set;} 
        public string WorkStartTime {get; set;} 
        public string WorkEndTime {get; set;}
        public bool IsSalesPerson { get; set; }
        public bool IsWorkAreaAssigned {get; set;} 
        public bool AppLoginStatus {get; set;} 
        public string DeviceType {get; set;} 
        public string DeviceToken {get; set;} 
        public bool IsActive {get; set;} 
        public string PasswordUrl {get; set;} 
        public bool IsLinkActive {get; set;} 
        public bool IsDeleted {get; set;} 
        public int AddedBy {get; set;} 
        public int AddedByType {get; set;} 
        public DateTime AddedDate {get; set;} 
        public int UpdatedBy {get; set;} 
        public int UpdatedByType {get; set;} 
        public DateTime UpdatedDate {get; set;} 
        public int DeletedBy {get; set;} 
        public int DeletedByType {get; set;} 
        public DateTime DeletedDate {get; set;}

        #region "Constructors"
        public Employee()
        {
        }
        public Employee(ref DataRow drRow)
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
            dataLib.AddParameter("@RoleId", SqlDbType.Int,this.RoleId);
            dataLib.AddParameter("@FirstName", SqlDbType.NVarChar,this.FirstName);
            dataLib.AddParameter("@LastName", SqlDbType.NVarChar,this.LastName);
            dataLib.AddParameter("@UserName", SqlDbType.NVarChar,this.UserName);
            dataLib.AddParameter("@Email", SqlDbType.NVarChar,this.Email);
            dataLib.AddParameter("@Password", SqlDbType.NVarChar,this.Password);
            dataLib.AddParameter("@Image", SqlDbType.NVarChar,this.Image);
            dataLib.AddParameter("@Address", SqlDbType.NVarChar,this.Address);
            dataLib.AddParameter("@City", SqlDbType.NVarChar, this.City);
            dataLib.AddParameter("@StateId", SqlDbType.Int,this.StateId);
            dataLib.AddParameter("@ZipCode", SqlDbType.NVarChar,this.ZipCode);
            dataLib.AddParameter("@PhoneNumber", SqlDbType.NVarChar,this.PhoneNumber);
            dataLib.AddParameter("@MobileNumber", SqlDbType.NVarChar,this.MobileNumber);
            dataLib.AddParameter("@WorkStartTime", SqlDbType.NVarChar,this.WorkStartTime);
            dataLib.AddParameter("@WorkEndTime", SqlDbType.NVarChar,this.WorkEndTime);
            dataLib.AddParameter("@IsSalesPerson", SqlDbType.Bit, this.IsSalesPerson);
            dataLib.AddParameter("@IsActive", SqlDbType.Bit,this.IsActive);
            dataLib.AddParameter("@AddedBy", SqlDbType.Int,this.AddedBy);
            dataLib.AddParameter("@AddedByType", SqlDbType.Int,this.AddedByType);
            dataLib.AddParameter("@AddedDate", SqlDbType.DateTime,this.AddedDate);
        }

        public override void AddUpdateParams(ref ZWT.DbLib.IDataLib dataLib)
        {
            dataLib.AddParameter("@Id", SqlDbType.Int,this.Id);
            dataLib.AddParameter("@FirstName", SqlDbType.NVarChar,this.FirstName);
            dataLib.AddParameter("@LastName", SqlDbType.NVarChar,this.LastName);
            dataLib.AddParameter("@Email", SqlDbType.NVarChar, this.Email);
            dataLib.AddParameter("@Password", SqlDbType.NVarChar,this.Password);
            dataLib.AddParameter("@Image", SqlDbType.NVarChar,this.Image);
            dataLib.AddParameter("@Address", SqlDbType.NVarChar,this.Address);
            dataLib.AddParameter("@City", SqlDbType.NVarChar, this.City);
            dataLib.AddParameter("@StateId", SqlDbType.Int,this.StateId);
            dataLib.AddParameter("@ZipCode", SqlDbType.NVarChar,this.ZipCode);
            dataLib.AddParameter("@PhoneNumber", SqlDbType.NVarChar,this.PhoneNumber);
            dataLib.AddParameter("@MobileNumber", SqlDbType.NVarChar,this.MobileNumber);
            dataLib.AddParameter("@WorkStartTime", SqlDbType.NVarChar,this.WorkStartTime);
            dataLib.AddParameter("@WorkEndTime", SqlDbType.NVarChar,this.WorkEndTime);
            dataLib.AddParameter("@IsActive", SqlDbType.Bit,this.IsActive);
            dataLib.AddParameter("@IsSalesPerson", SqlDbType.Bit, this.IsSalesPerson);
            dataLib.AddParameter("@UpdatedBy", SqlDbType.Int,this.UpdatedBy);
            dataLib.AddParameter("@UpdatedByType", SqlDbType.Int,this.UpdatedByType);
            dataLib.AddParameter("@UpdatedDate", SqlDbType.DateTime,this.UpdatedDate);
        }

        public override void AddSearchParams(ref ZWT.DbLib.IDataLib dataLib)
        {

        }
        #endregion
    }
}