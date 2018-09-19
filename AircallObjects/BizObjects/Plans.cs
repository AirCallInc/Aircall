using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace BizObjects
{
    public class Plans : BizObject
    {
        public int Id { get; set; }
        public int PlanTypeId { get; set; }
        public bool PackageType { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string PackageDisplayName { get; set; }
        public string Description { get; set; }
        public decimal PricePerMonth { get; set; }
        public Int16 DurationInMonth { get; set; }
        public Int16 NumberOfService { get; set; }
        public Int16 FirstServiceWithinDays { get; set; }
        public int OtherServiceScheduleDaysGap { get; set; }
        public int Drivetime { get; set; }
        public int ServiceTimeForFirstUnit { get; set; }
        public int ServiceTimeForAdditionalUnits { get; set; }
        public bool ShowSpecialPrice { get; set; }
        public bool ShowAutoRenewal { get; set; }
        public decimal DiscountPrice { get; set; }
        public string Image { get; set; }
        public bool Status { get; set; }
        public string BackGroundColorRGB { get; set; }
        public string BackGroundColorHGS { get; set; }
        public string StripePlanId { get; set; }
        public bool IsVisible { get; set; }
        public bool IsDeleted { get; set; }
        public int AddedBy { get; set; }
        public int AddedByType { get; set; }
        public DateTime AddedDate { get; set; }
        public int UpdatedBy { get; set; }
        public int UpdatedByType { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int DeletedBy { get; set; }
        public int DeletedByTypes { get; set; }
        public DateTime DeletedDate { get; set; }

        #region "Constructors"
        public Plans()
        {
        }
        public Plans(ref DataRow drRow)
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
            dataLib.AddParameter("@PlanTypeId", SqlDbType.Int, this.PlanTypeId);
            dataLib.AddParameter("@PackageType", SqlDbType.Bit, this.PackageType);
            dataLib.AddParameter("@Name", SqlDbType.NVarChar, this.Name);
            dataLib.AddParameter("@ShortDescription", SqlDbType.NVarChar, this.ShortDescription);
            dataLib.AddParameter("@PackageDisplayName", SqlDbType.NVarChar, this.PackageDisplayName);
            dataLib.AddParameter("@Description", SqlDbType.NVarChar, this.Description);
            dataLib.AddParameter("@PricePerMonth", SqlDbType.Decimal, this.PricePerMonth);
            dataLib.AddParameter("@DurationInMonth", SqlDbType.SmallInt, this.DurationInMonth);
            dataLib.AddParameter("@NumberOfService", SqlDbType.SmallInt, this.NumberOfService);
            dataLib.AddParameter("@FirstServiceWithinDays", SqlDbType.SmallInt, this.FirstServiceWithinDays);
            dataLib.AddParameter("@OtherServiceScheduleDaysGap", SqlDbType.Int, this.OtherServiceScheduleDaysGap);
            dataLib.AddParameter("@Drivetime", SqlDbType.Int, this.Drivetime);
            dataLib.AddParameter("@ServiceTimeForFirstUnit", SqlDbType.Int, this.ServiceTimeForFirstUnit);
            dataLib.AddParameter("@ServiceTimeForAdditionalUnits", SqlDbType.Int, this.ServiceTimeForAdditionalUnits);
            dataLib.AddParameter("@ShowSpecialPrice", SqlDbType.Bit, this.ShowSpecialPrice);
            dataLib.AddParameter("@ShowAutoRenewal", SqlDbType.Bit, this.ShowAutoRenewal);
            dataLib.AddParameter("@DiscountPrice", SqlDbType.Decimal, this.DiscountPrice);
            dataLib.AddParameter("@Image", SqlDbType.NVarChar, this.Image);
            dataLib.AddParameter("@Status", SqlDbType.Bit, this.Status);
            dataLib.AddParameter("@BackGroundColorRGB", SqlDbType.VarChar, this.BackGroundColorRGB);
            dataLib.AddParameter("@BackGroundColorHGS", SqlDbType.VarChar, this.BackGroundColorHGS);
            dataLib.AddParameter("@StripePlanId", SqlDbType.NVarChar, this.StripePlanId);
            dataLib.AddParameter("@AddedBy", SqlDbType.Int, this.AddedBy);
            dataLib.AddParameter("@AddedByType", SqlDbType.Int, this.AddedByType);
            dataLib.AddParameter("@AddedDate", SqlDbType.DateTime, this.AddedDate);
        }

        public override void AddUpdateParams(ref ZWT.DbLib.IDataLib dataLib)
        {
            dataLib.AddParameter("@Id", SqlDbType.Int, this.Id);
            dataLib.AddParameter("@PlanTypeId", SqlDbType.Int, this.PlanTypeId);
            dataLib.AddParameter("@PackageType", SqlDbType.Bit, this.PackageType);
            dataLib.AddParameter("@Name", SqlDbType.NVarChar, this.Name);
            dataLib.AddParameter("@ShortDescription", SqlDbType.NVarChar, this.ShortDescription);
            dataLib.AddParameter("@PackageDisplayName", SqlDbType.NVarChar, this.PackageDisplayName);
            dataLib.AddParameter("@Description", SqlDbType.NVarChar, this.Description);
            dataLib.AddParameter("@PricePerMonth", SqlDbType.Decimal, this.PricePerMonth);
            dataLib.AddParameter("@DurationInMonth", SqlDbType.SmallInt, this.DurationInMonth);
            dataLib.AddParameter("@NumberOfService", SqlDbType.SmallInt, this.NumberOfService);
            dataLib.AddParameter("@FirstServiceWithinDays", SqlDbType.SmallInt, this.FirstServiceWithinDays);
            dataLib.AddParameter("@OtherServiceScheduleDaysGap", SqlDbType.Int, this.OtherServiceScheduleDaysGap);
            dataLib.AddParameter("@Drivetime", SqlDbType.Int, this.Drivetime);
            dataLib.AddParameter("@ServiceTimeForFirstUnit", SqlDbType.Int, this.ServiceTimeForFirstUnit);
            dataLib.AddParameter("@ServiceTimeForAdditionalUnits", SqlDbType.Int, this.ServiceTimeForAdditionalUnits);
            dataLib.AddParameter("@ShowSpecialPrice", SqlDbType.Bit, this.ShowSpecialPrice);
            dataLib.AddParameter("@ShowAutoRenewal", SqlDbType.Bit, this.ShowAutoRenewal);
            dataLib.AddParameter("@DiscountPrice", SqlDbType.Decimal, this.DiscountPrice);
            dataLib.AddParameter("@Image", SqlDbType.NVarChar, this.Image);
            dataLib.AddParameter("@Status", SqlDbType.Bit, this.Status);
            dataLib.AddParameter("@BackGroundColorRGB", SqlDbType.VarChar, this.BackGroundColorRGB);
            dataLib.AddParameter("@BackGroundColorHGS", SqlDbType.VarChar, this.BackGroundColorHGS);
            dataLib.AddParameter("@StripePlanId", SqlDbType.NVarChar, this.StripePlanId);
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