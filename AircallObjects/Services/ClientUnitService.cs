using DBUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace Services
{
    public interface IClientUnitService
    {
        //UnitStatuses: Serviced=1,ServiceSoon=2,NeedRepair=3
        void GetClientUnits(string ClientName, int Status, ref DataTable dtUnits);
        void GetClientUnitById(int UnitId, ref DataTable dtUnit);
        void GetClientUnitBySubscriptionId(string SubscriptionId, ref DataTable dtUnit);
        void GetClientUnitForPortalById(int UnitId, int ClientId, ref DataTable dtUnit);
        void GetClientUnitsByClientId(int ClientId, ref DataTable dtUnits);
        void GetUnPaidClientUnitsByClientId(int ClientId, ref DataTable dtUnits);
        void GetClientUnitsByClientIdUnitName(int ClientId, string UnitName, ref DataTable dtUnits);
        void CheckUnitName(int ClientId, int UnitId, string UnitName, ref DataTable dtUnits);
        void GetClientUnitByClientAndAddressId(int ClientId, int AddressId, ref DataTable dtUnits);
        void GetClientUnitByClientAndAddressIdForPortal(int ClientId, int AddressId, ref DataTable dtUnits);
        void GetClientUnitByClientAndAddressIdPlanForPortal(int ClientId, int AddressId, int PlanTypeId, ref DataTable dtUnits);
        void GetUnitTypes(ref DataTable dtUnitTypes);
        void GetClientUnitsForPortal(int ClientId, ref DataTable dtUnits);
        int AddClientUnit(ref BizObjects.ClientUnit ClientUnit);
        int UpdateClientUnit(ref BizObjects.ClientUnit ClientUnit);
        int HardDeleteClientUnit(int ClientUnitId);
        int UpdateClientUnitPortal(int UnitId, string PaymentStatus, string StripeSubscriptionId, int UpdatedBy, int UpdatedByType, DateTime UpdatedDate);
        int UpdateClientUnitNamePortal(int UnitId, string UnitName, int UpdatedBy, int UpdatedByType, DateTime UpdatedDate);
        int SetStatusByServiceId(int Status, long ServiceId);
        int SetPaymentStatusByUnitId(int UnitId, string PaymentStatus, string SubscriptionId);
        void GetUnitsByClientAddressAndPlanId(int ClientId, int AddressId, int PlanTypeId, ref DataTable dtUnits);
        void CheckPlanTypeByClientUnitIds(string ClientUnitIds, ref DataTable dtClientUnits);
        void CheckForAddressChange(int UnitId,ref DataTable dtClientUnit);
        void AddressChangeOfUnitsProcess(int UnitId, int UpdatedBy, int UpdatedByType, DateTime UpdatedDate);
        void GetAllCancelRenewPlanUnits(ref DataTable dtUnits);
        void GetAllCancelRenewPlanUnitsByClientName(string ClientName, ref DataTable dtUnits);
        void ClientUnitPlanExpiration(ref DataTable dtUnitPlanExpiration);
        void CheckForRenewUnitSubscription(int UnitId, ref DataTable dtUnit);
        int RenewUnitSubscription(int UnitId, int AddedBy, int AddedByType, DateTime AddedDate);
        int CancelUnitSubscription(int UnitId, int UserId, int RoleId, DateTime Date);
        void UnsubscribeUnitPlan(ref DataTable dtUnits);
        int DeleteClientUnit(int ClientUnitId);
        //pp
        void GetClientUnitsByClientIdADMIN(int ClientId, string SortField, string SortDirection, int status, ref DataTable dtUnits);
        void GetClientUnitsADMIN(string ClientName, int Status, string SortField, string SortDirection, ref DataTable dtUnits);
        void UpdateUnitPricePerMonth(int unitId, string planTypeId, int visitPerYear);
        decimal GetPricePerMonth(string planTypeId, int visitPerYear);
        void GetUnits(string clientUnitIds, ref DataTable dtUnits);
    }

    public class ClientUnitService : IClientUnitService
    {
        IDataLib dbLib;
        public void GetClientUnits(string ClientName, int Status, ref DataTable dtUnits)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientUnit_GetAll";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ClientName", SqlDbType.NVarChar, ClientName);
                dbLib.AddParameter("@Status", SqlDbType.Int, Status);
                dbLib.RunSP(strsql, ref dtUnits);
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

        public void GetClientUnitById(int UnitId, ref DataTable dtUnit)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientUnit_GetById";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ClientUnitId", SqlDbType.Int, UnitId);
                dbLib.RunSP(strsql, ref dtUnit);
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

        public void GetClientUnitForPortalById(int UnitId, int ClientId, ref DataTable dtUnit)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientPortal_ClientUnit_GetById";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ClientUnitId", SqlDbType.Int, UnitId);
                dbLib.AddParameter("@ClientId", SqlDbType.Int, ClientId);
                dbLib.RunSP(strsql, ref dtUnit);
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

        public void GetClientUnitsByClientId(int ClientId, ref DataTable dtUnits)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientUnit_GetByClientId";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ClientId", SqlDbType.Int, ClientId);
                dbLib.RunSP(strsql, ref dtUnits);
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

        public void GetUnPaidClientUnitsByClientId(int ClientId, ref DataTable dtUnits)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientUnit_UnPaid_GetByClientId";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ClientId", SqlDbType.Int, ClientId);
                dbLib.RunSP(strsql, ref dtUnits);
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

        public void GetClientUnitsByClientIdUnitName(int ClientId,string UnitName, ref DataTable dtUnits)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientUnit_GetByClientIdUnitName";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ClientId", SqlDbType.Int, ClientId);
                dbLib.AddParameter("@UnitName", SqlDbType.NVarChar, UnitName);
                dbLib.RunSP(strsql, ref dtUnits);
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

        public void CheckUnitName(int ClientId, int UnitId, string UnitName, ref DataTable dtUnits)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientUnit_CheckUnitName";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ClientId", SqlDbType.Int, ClientId);
                dbLib.AddParameter("@UnitId", SqlDbType.Int, UnitId);
                dbLib.AddParameter("@UnitName", SqlDbType.NVarChar, UnitName);
                dbLib.RunSP(strsql, ref dtUnits);
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

       
        public void GetClientUnitByClientAndAddressIdForPortal(int ClientId, int AddressId, ref DataTable dtUnits)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientUnit_GetByClientAndAddressIdForPortal";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ClientId", SqlDbType.Int, ClientId);
                dbLib.AddParameter("@AddressId", SqlDbType.Int, AddressId);
                dbLib.RunSP(strsql, ref dtUnits);
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
        public void GetClientUnitByClientAndAddressIdPlanForPortal(int ClientId, int AddressId, int PlanTypeId, ref DataTable dtUnits)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientUnit_GetByClientAndAddressIdPlanIdForPortal";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ClientId", SqlDbType.Int, ClientId);
                dbLib.AddParameter("@AddressId", SqlDbType.Int, AddressId);
                dbLib.AddParameter("@PlanTypeId", SqlDbType.Int, PlanTypeId);
                dbLib.RunSP(strsql, ref dtUnits);
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
        public void GetClientUnitByClientAndAddressId(int ClientId, int AddressId, ref DataTable dtUnits)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientUnit_GetByClientAndAddressId";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ClientId", SqlDbType.Int, ClientId);
                dbLib.AddParameter("@AddressId", SqlDbType.Int, AddressId);
                dbLib.RunSP(strsql, ref dtUnits);
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

        public void GetUnitTypes(ref DataTable dtUnitTypes)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_UnitType_GetAll";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.RunSP(strsql, ref dtUnitTypes);
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

        public void GetClientUnitsForPortal(int ClientId, ref DataTable dtUnits)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientPortal_ClientUnit_Dashboard";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ClientId", SqlDbType.Int, ClientId);
                dbLib.RunSP(strsql, ref dtUnits);
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

        public int AddClientUnit(ref BizObjects.ClientUnit ClientUnit)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_ClientUnit_Insert";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                ClientUnit.AddInsertParams(ref dbLib);
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

        public void UpdateUnitPricePerMonth(int unitId, string planTypeId, int visitPerYear)
        {
            decimal pricePerMonth = GetPricePerMonth(planTypeId, visitPerYear);

            var sql = string.Format("update dbo.ClientUnit set VisitPerYear = {0}, PlanTypeId = {1}, PricePerMonth = {2} where Id = {3}", visitPerYear, planTypeId, pricePerMonth, unitId);
            SQLDBHelper instance = new SQLDBHelper();
            instance.ExecuteSQL(sql, null);
            instance.Close();
        }

        public void GetUnits(string clientUnitIds, ref DataTable dtUnits)
        {
            var sql = string.Format("select A.UnitName, A.VisitPerYear, A.PricePerMonth, B.PlanName from ClientUnit A inner join SubscriptionPlans B on A.PlanTypeId = B.Id where A.Id in ({0})", clientUnitIds);
            SQLDBHelper instance = new SQLDBHelper();
            var ds = instance.Query(sql, null);
            instance.Close();
            dtUnits = ds.Tables[0];
        }

        public decimal GetPricePerMonth(string planTypeId, int visitPerYear)
        {
            var sql = string.Format("select * from SubscriptionPlans where Id = {0}", planTypeId);
            var instance = new SQLDBHelper();
            var ds = instance.Query(sql, null);
            instance.Close();
            var dr = ds.Tables[0].Rows[0];
            decimal pricePerMonth = 0;
            decimal basicFee = Convert.ToDecimal(dr["BasicFee"]);
            decimal feeIncrement = Convert.ToDecimal(dr["FeeIncrement"]);

            if (visitPerYear > 1)
            {
                pricePerMonth = basicFee + (visitPerYear - 1) * feeIncrement;
            }
            else
            {
                pricePerMonth = basicFee;
            }

            return pricePerMonth;
        }

        public int UpdateClientUnit(ref BizObjects.ClientUnit ClientUnit)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_ClientUnit_Update";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                ClientUnit.AddUpdateParams(ref dbLib);
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

        public int HardDeleteClientUnit(int ClientUnitId)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_ClientUnit_HardDeleteClientUnit";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@Id", SqlDbType.Int, ClientUnitId);
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

        public int UpdateClientUnitPortal(int UnitId, string PaymentStatus,string StripeSubscriptionId, int UpdatedBy, int UpdatedByType, DateTime UpdatedDate)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_ClientPortal_ClientUnit_UpdatePaymentStatus";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@Id", SqlDbType.Int, UnitId);
                dbLib.AddParameter("@PaymentStatus", SqlDbType.NVarChar, PaymentStatus);
                dbLib.AddParameter("@StripeSubscriptionId", SqlDbType.NVarChar, StripeSubscriptionId);
                dbLib.AddParameter("@UpdatedBy", SqlDbType.Int, UpdatedBy);
                dbLib.AddParameter("@UpdatedByType", SqlDbType.Int, UpdatedByType);
                dbLib.AddParameter("@UpdatedDate", SqlDbType.DateTime, UpdatedDate);
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
        public int UpdateClientUnitNamePortal(int UnitId, string UnitName, int UpdatedBy, int UpdatedByType, DateTime UpdatedDate)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_ClientPortal_ClientUnit_UpdateUnitName";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@Id", SqlDbType.Int, UnitId);
                dbLib.AddParameter("@UnitName", SqlDbType.NVarChar, UnitName);
                dbLib.AddParameter("@UpdatedBy", SqlDbType.Int, UpdatedBy);
                dbLib.AddParameter("@UpdatedByType", SqlDbType.Int, UpdatedByType);
                dbLib.AddParameter("@UpdatedDate", SqlDbType.DateTime, UpdatedDate);
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
        public int SetStatusByServiceId(int Status, long ServiceId)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_ClientUnit_SetStatusByServiceId";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ServiceId", SqlDbType.BigInt, ServiceId);
                dbLib.AddParameter("@Status", SqlDbType.Int, Status);
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

        public int SetPaymentStatusByUnitId(int UnitId, string PaymentStatus, string SubscriptionId)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_ClientUnit_UpdatePaymentStatus";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@UnitId", SqlDbType.Int, UnitId);
                dbLib.AddParameter("@PaymentStatus", SqlDbType.NVarChar, PaymentStatus);
                dbLib.AddParameter("@SubscriptionId", SqlDbType.NVarChar, SubscriptionId);
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

        public void GetUnitsByClientAddressAndPlanId(int ClientId, int AddressId, int PlanTypeId, ref DataTable dtUnits)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientUnit_GetUnitsByClientAddressAndPlanId";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ClientId", SqlDbType.Int, ClientId);
                dbLib.AddParameter("@AddressId", SqlDbType.Int, AddressId);
                dbLib.AddParameter("@PlanTypeId", SqlDbType.Int, PlanTypeId);
                dbLib.RunSP(strsql, ref dtUnits);
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

        public void CheckPlanTypeByClientUnitIds(string ClientUnitIds, ref DataTable dtClientUnits)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientUnit_CheckPlanTypeByUnitIds";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ClientUnitIds", SqlDbType.NVarChar, ClientUnitIds);
                dbLib.RunSP(strsql, ref dtClientUnits);
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

        public void CheckForAddressChange(int UnitId,ref DataTable dtClientUnit)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientUnit_CheckForAddressChange";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@UnitId", SqlDbType.Int, UnitId);
                dbLib.RunSP(strsql, ref dtClientUnit);
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

        public void AddressChangeOfUnitsProcess(int UnitId, int UpdatedBy, int UpdatedByType, DateTime UpdatedDate)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientUnit_AddressChangeProcess";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@UnitId", SqlDbType.Int, UnitId);
                dbLib.AddParameter("@UpdatedBy", SqlDbType.Int, UpdatedBy);
                dbLib.AddParameter("@UpdatedByType", SqlDbType.Int, UpdatedByType);
                dbLib.AddParameter("@UpdatedDate", SqlDbType.DateTime, UpdatedDate);
                dbLib.ExeSP(strsql);
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

        public void GetAllCancelRenewPlanUnits(ref DataTable dtUnits)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientUnit_GetRenewCancelPlanUnit";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.RunSP(strsql, ref dtUnits);
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

        public void GetAllCancelRenewPlanUnitsByClientName(string ClientName, ref DataTable dtUnits)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientUnit_GetRenewCancelPlanUnitByName";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ClientName", SqlDbType.NVarChar, ClientName);
                dbLib.RunSP(strsql, ref dtUnits);
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

        public void ClientUnitPlanExpiration(ref DataTable dtUnitPlanExpiration)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_PlanExpirationScheduler";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.RunSP(strsql, ref dtUnitPlanExpiration);
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

        public void CheckForRenewUnitSubscription(int UnitId, ref DataTable dtUnit)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_CheckForRenewUnitPlan";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@UnitId", SqlDbType.Int, UnitId);
                dbLib.RunSP(strsql, ref dtUnit);
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

        public int RenewUnitSubscription(int UnitId, int AddedBy, int AddedByType, DateTime AddedDate)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_ClientUnit_RenewPlan";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@UnitId", SqlDbType.Int, UnitId);
                dbLib.AddParameter("@AddedBy", SqlDbType.Int, AddedBy);
                dbLib.AddParameter("@AddedByType", SqlDbType.Int, AddedByType);
                dbLib.AddParameter("@AddedDate", SqlDbType.DateTime, AddedDate);
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

        public int CancelUnitSubscription(int UnitId, int UserId, int RoleId, DateTime Date)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_ClientUnit_CancelPlan";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@UnitId", SqlDbType.Int, UnitId);
                dbLib.AddParameter("@UserId", SqlDbType.Int, UserId);
                dbLib.AddParameter("@RoleId", SqlDbType.Int, RoleId);
                dbLib.AddParameter("@Date", SqlDbType.DateTime, Date);
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

        public int DeleteClientUnit(int ClientUnitId)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_ClientUnit_ClientPortal_DeleteUnit";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ClientUnitId", SqlDbType.Int, ClientUnitId);
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

        public void UnsubscribeUnitPlan(ref DataTable dtUnits)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientUnit_UnsubscribePlanAfterSpecifiedDays";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.RunSP(strsql, ref dtUnits);
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

        public void GetClientUnitBySubscriptionId(string SubscriptionId, ref DataTable dtUnit)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_GetClientUnitBySubscriptionId";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@StripeSubscriptionId", SqlDbType.NVarChar, SubscriptionId);
                dbLib.RunSP(strsql, ref dtUnit);
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

        public void GetClientUnitsByClientIdADMIN(int ClientId, string SortField, string SortDirection, int status, ref DataTable dtUnits)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientUnit_GetByClientIdAdmin";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ClientId", SqlDbType.Int, ClientId);
                dbLib.AddParameter("@Status", SqlDbType.Int, status);
                dbLib.AddParameter("@SortField", SqlDbType.NVarChar, SortField);
                dbLib.AddParameter("@SortDirection", SqlDbType.NVarChar, SortDirection);
                dbLib.RunSP(strsql, ref dtUnits);
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

        public void GetClientUnitsADMIN(string ClientName, int Status, string SortField, string SortDirection, ref DataTable dtUnits)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientUnit_GetAllAdmin";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ClientName", SqlDbType.NVarChar, ClientName);
                dbLib.AddParameter("@Status", SqlDbType.Int, Status);
                dbLib.AddParameter("@SortField", SqlDbType.NVarChar, SortField);
                dbLib.AddParameter("@SortDirection", SqlDbType.NVarChar, SortDirection);
                dbLib.RunSP(strsql, ref dtUnits);
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
