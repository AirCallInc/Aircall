using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;
using DBUtility;

namespace Services
{
    public interface IPlanService
    {
        void GetAllPlanType(ref DataTable dtPlanTypes);
        void GetAllPlanTypeForPlan(ref DataTable dtPlanTypes);
        void GetPlanByPlanType(int PlanTypeId, ref DataTable dtPlans);
        void GetAllPlan(ref DataTable dtPlans);
        void GetAllActivePlan(ref DataTable dtPlans);
        int SetStatus(bool IsActive, int PlanTypeId);
        int AddPlan(ref BizObjects.Plans Plan);
        int UpdatePlan(ref BizObjects.Plans Plan);
        int DeletePlan(int PlanTypeId);
        void GetPlanByAddressId(int AddressId, ref DataTable dtPlans);
        void GetPlanByAddressIdForRecheduled(int AddressId, ref DataTable dtPlans);
        void GetAllPlanForFrontEnd(ref DataTable dtPlanTypes);
        void GetTimeSlot(int PlanType, ref DataTable dtTomeSlot);
        int UpdatePlanTypeTimeSlot(int PlanTypeId, string TimeSlot1, string TimeSlot2);
    }

    public class PlanService : IPlanService
    {
        IDataLib dbLib;

        public void GetAllPlanType_1(ref DataTable dtPlanTypes)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_PlanType_GetAll";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.RunSP(strsql, ref dtPlanTypes);
            }
            catch (Exception ex)
            {
                dbLib.RollbackTransaction();
                throw ex;
            }
            finally
            {
                dbLib.CloseConnection();
            }
        }

        public void GetAllPlanType(ref DataTable dtPlanTypes)
        {
            var sql = "select * from SubscriptionPlans where isnull(IsDeleted, '0') = '0' order by DisplayOrder";
            var instance = new SQLDBHelper();
            var ds = instance.Query(sql, null);
            instance.Close();
            dtPlanTypes = ds.Tables[0];
        }

        public void GetAllPlanTypeForPlan(ref DataTable dtPlanTypes)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_PlanType_GetAllForPlan";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.RunSP(strsql, ref dtPlanTypes);
            }
            catch (Exception ex)
            {
                dbLib.RollbackTransaction();
                throw ex;
            }
            finally
            {
                dbLib.CloseConnection();
            }
        }

        public void GetAllPlanForFrontEnd_1(ref DataTable dtPlanTypes)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientPortal_GetPlanCoverage";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.RunSP(strsql, ref dtPlanTypes);
            }
            catch (Exception ex)
            {
                dbLib.RollbackTransaction();
                throw ex;
            }
            finally
            {
                dbLib.CloseConnection();
            }
        }

        public void GetAllPlanForFrontEnd(ref DataTable dtPlanTypes)
        {
            var sql = "select * from SubscriptionPlans where isnull(IsDeleted, '0') = '0' order by DisplayOrder";
            var instance = new SQLDBHelper();
            var ds = instance.Query(sql, null);
            instance.Close();
            dtPlanTypes = ds.Tables[0];
        }

        public void GetPlanByPlanType(int PlanTypeId, ref DataTable dtPlans)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Plan_GetByPlanTypeId";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@PlanTypeId", SqlDbType.Int, PlanTypeId);
                dbLib.RunSP(strsql, ref dtPlans);
            }
            catch (Exception ex)
            {
                dbLib.RollbackTransaction();
                throw ex;
            }
            finally
            {
                dbLib.CloseConnection();
            }
        }

        public void GetPlanByAddressIdForRecheduled(int AddressId, ref DataTable dtPlans)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientPortal_PlanType_GetByAddressIdForRechedule";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@AddressId", SqlDbType.Int, AddressId);
                dbLib.RunSP(strsql, ref dtPlans);
            }
            catch (Exception ex)
            {
                dbLib.RollbackTransaction();
                throw ex;
            }
            finally
            {
                dbLib.CloseConnection();
            }
        }
        public void GetPlanByAddressId(int AddressId, ref DataTable dtPlans)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientPortal_PlanType_GetByAddressId";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@AddressId", SqlDbType.Int, AddressId);
                dbLib.RunSP(strsql, ref dtPlans);
            }
            catch (Exception ex)
            {
                dbLib.RollbackTransaction();
                throw ex;
            }
            finally
            {
                dbLib.CloseConnection();
            }
        }
        public void GetAllPlan(ref DataTable dtPlans)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            //strsql = "uspa_Plan_GetAll";
            strsql = "uspa_Plan_GetAll_New";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.RunSP(strsql, ref dtPlans);
            }
            catch (Exception ex)
            {
                dbLib.RollbackTransaction();
                throw ex;
            }
            finally
            {
                dbLib.CloseConnection();
            }
        }
        public void GetAllActivePlan(ref DataTable dtPlans)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Plan_GetAllActive";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.RunSP(strsql, ref dtPlans);
            }
            catch (Exception ex)
            {
                dbLib.RollbackTransaction();
                throw ex;
            }
            finally
            {
                dbLib.CloseConnection();
            }
        }
        public int SetStatus(bool IsActive, int PlanTypeId)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_Plan_SetStatus";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@IsActive", SqlDbType.Bit, IsActive);
                dbLib.AddParameter("@PlanTypeId", SqlDbType.Int, PlanTypeId);
                rtn = dbLib.ExeSP(strsql);
                return rtn;
            }
            catch (Exception ex)
            {
                dbLib.RollbackTransaction();
                throw ex;
            }
            finally
            {
                dbLib.CloseConnection();
            }
        }

        public int AddPlan(ref BizObjects.Plans Plan)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_Plan_Insert";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                Plan.AddInsertParams(ref dbLib);
                rtn = dbLib.ExeSP(strsql);
                return rtn;
            }
            catch (Exception ex)
            {
                dbLib.RollbackTransaction();
                throw ex;
            }
            finally
            {
                dbLib.CloseConnection();
            }
        }

        public int UpdatePlan(ref BizObjects.Plans Plan)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_Plan_Update";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                Plan.AddUpdateParams(ref dbLib);
                rtn = dbLib.ExeSP(strsql);
                return rtn;
            }
            catch (Exception ex)
            {
                dbLib.RollbackTransaction();
                throw ex;
            }
            finally
            {
                dbLib.CloseConnection();
            }
        }

        public int DeletePlan(int PlanTypeId)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_Plan_Delete";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@PlanTypeId", SqlDbType.Int, PlanTypeId);
                rtn = dbLib.ExeSP(strsql);
                return rtn;
            }
            catch (Exception ex)
            {
                dbLib.RollbackTransaction();
                throw ex;
            }
            finally
            {
                dbLib.CloseConnection();
            }
        }

        public void GetTimeSlot(int PlanType, ref DataTable dtTomeSlot)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientPortal_PlanType_GetTimeSlot";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@PlanTypeId", SqlDbType.Int, PlanType);
                dbLib.RunSP(strsql, ref dtTomeSlot);
            }
            catch (Exception ex)
            {
                dbLib.RollbackTransaction();
                throw ex;
            }
            finally
            {
                dbLib.CloseConnection();
            }
        }

        public int UpdatePlanTypeTimeSlot(int PlanTypeId, string TimeSlot1, string TimeSlot2)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_PlanType_UpdateTimeSlot";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@PlanTypeId", SqlDbType.Int, PlanTypeId);
                dbLib.AddParameter("@TimeSlot1", SqlDbType.NVarChar, TimeSlot1);
                dbLib.AddParameter("@TimeSlot2", SqlDbType.NVarChar, TimeSlot2);
                rtn = dbLib.ExeSP(strsql);
                return rtn;
            }
            catch (Exception ex)
            {
                dbLib.RollbackTransaction();
                throw ex;
            }
            finally
            {
                dbLib.CloseConnection();
            }
        }
    }
}
