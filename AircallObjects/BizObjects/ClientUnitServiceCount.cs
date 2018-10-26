using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace BizObjects
{
    public class ClientUnitServiceCount : BizObject
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int UnitId { get; set; }
        public int PlanType { get; set; }
        public int TotalPlanService { get; set; }
        public int TotalDonePlanService { get; set; }
        public int TotalRequestService { get; set; }
        public int TotalDoneRequestService { get; set; }
        public int TotalBillsGenerated { get; set; }
        public int StripeUnitSubscriptionCount { get; set; }
        public bool IsDeleted { get; set; }
        public int AddedBy { get; set; }
        public int AddedByType { get; set; }
        public DateTime AddedDate { get; set; }
        public int UpdatedBy { get; set; }
        public int UpdatedByType { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int DeletedBy { get; set; }
        public int DeletedByType { get; set; }
        public DateTime DeletedDate { get; set; }
        public int VisitPerYear { get; set; }

        #region "Constructors"
        public ClientUnitServiceCount()
        {
        }
        public ClientUnitServiceCount(ref DataRow drRow)
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
            dataLib.AddParameter("@ClientId", SqlDbType.Int, this.ClientId);
            dataLib.AddParameter("@UnitId", SqlDbType.Int, this.UnitId);
            dataLib.AddParameter("@PlanType", SqlDbType.Int, this.PlanType);
            dataLib.AddParameter("@TotalDonePlanService", SqlDbType.Int, this.TotalDonePlanService);
            dataLib.AddParameter("@TotalRequestService", SqlDbType.Int, this.TotalRequestService);
            dataLib.AddParameter("@TotalDoneRequestService", SqlDbType.Int, this.TotalDoneRequestService);
            dataLib.AddParameter("@TotalBillsGenerated", SqlDbType.Int, this.TotalBillsGenerated);
            dataLib.AddParameter("@StripeUnitSubscriptionCount", SqlDbType.Int, this.StripeUnitSubscriptionCount);
            dataLib.AddParameter("@AddedBy", SqlDbType.Int, this.AddedBy);
            dataLib.AddParameter("@AddedByType", SqlDbType.Int, this.AddedByType);
            dataLib.AddParameter("@AddedDate", SqlDbType.DateTime, this.AddedDate);
            dataLib.AddParameter("@VisitPerYear", SqlDbType.Int, this.VisitPerYear);
        }

        public override void AddUpdateParams(ref ZWT.DbLib.IDataLib dataLib)
        {
            dataLib.AddParameter("@ClientId", SqlDbType.Int, this.ClientId);
            dataLib.AddParameter("@UnitId", SqlDbType.Int, this.UnitId);
            dataLib.AddParameter("@TotalPlanService", SqlDbType.Int, this.TotalPlanService);
            dataLib.AddParameter("@TotalDonePlanService", SqlDbType.Int, this.TotalDonePlanService);
            dataLib.AddParameter("@TotalRequestService", SqlDbType.Int, this.TotalRequestService);
            dataLib.AddParameter("@TotalDoneRequestService", SqlDbType.Int, this.TotalDoneRequestService);
            dataLib.AddParameter("@TotalBillsGenerated", SqlDbType.Int, this.TotalBillsGenerated);
            dataLib.AddParameter("@StripeUnitSubscriptionCount", SqlDbType.Int, this.StripeUnitSubscriptionCount);
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
