using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace Services
{
    public interface IClientUnitServiceCountService
    {
        int AddClientUnitServiceCount(ref BizObjects.ClientUnitServiceCount ClientUnitServiceCount);
        int UpdateClientUnitServiceCount(ref BizObjects.ClientUnitServiceCount ClientUnitServiceCount);
        int UpdateClientUnitServiceCountWithRequestedService(long RequestedServiceId);
        void GetUnitSubscriptionCountWithStripe(ref DataTable dtUnitSubscription);
        int UpdateUnitStripeSubscriptionCount(int UnitId, int Count,int UserId,int RoleId,DateTime Date);
    }

    public class ClientUnitServiceCountService : IClientUnitServiceCountService
    {
        IDataLib dbLib;

        public int AddClientUnitServiceCount(ref BizObjects.ClientUnitServiceCount ClientUnitServiceCount)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_ClientUnitServiceCount_Insert";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                ClientUnitServiceCount.AddInsertParams(ref dbLib);
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

        public int UpdateClientUnitServiceCount(ref BizObjects.ClientUnitServiceCount ClientUnitServiceCount)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_ClientUnitServiceCount_Update";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                ClientUnitServiceCount.AddUpdateParams(ref dbLib);
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

        public int UpdateClientUnitServiceCountWithRequestedService(long RequestedServiceId)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_ClientUnitServiceCount_UpdateByRequestedServiceId";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@RequestedServiceId", SqlDbType.BigInt, RequestedServiceId);
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

        public void GetUnitSubscriptionCountWithStripe(ref DataTable dtUnitSubscription)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientUnitServiceCount_GetUnitSubscriptionCountWithStripeScheduler";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.RunSP(strsql, ref dtUnitSubscription);
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

        public int UpdateUnitStripeSubscriptionCount(int UnitId, int Count, int UserId, int RoleId, DateTime Date)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_ClientUnitServiceCount_UpdateStripeSubscriptionCount";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@UnitId", SqlDbType.Int, UnitId);
                dbLib.AddParameter("@Count", SqlDbType.Int, Count);
                dbLib.AddParameter("@UpdatedBy", SqlDbType.Int, UserId);
                dbLib.AddParameter("@UpdatedByType", SqlDbType.Int, RoleId);
                dbLib.AddParameter("@UpdatedDate", SqlDbType.DateTime, Date);
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
