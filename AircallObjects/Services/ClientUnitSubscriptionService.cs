using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace Services
{
    public interface IClientUnitSubscriptionService
    {
        void GetClientUnitSubscription(string ClientName, string StartDate, string EndDate, string PaymentMethod, string Status,  ref DataTable dtClientUnitSubscription);
        void GetClientUnitUnPaidSubscription(string ClientName, ref DataTable dtClientUnitSubscription);
        void GetClientUnitUnPaidSubscription(int ClientId, ref DataTable dtClientUnitSubscription);
        void GetClientUnitSubscriptionForScheduler(DateTime PaymentDate, string PaymentMethod, ref DataTable dtClientUnitSubscription);
        void GetClientPaymentDueSubscriptionForScheduler(ref DataTable dtSubscription);
        void GetClientUnitSubscriptionById(Int64 Id, ref DataTable dtSubscription);
        int AddClientUnitSubscriptionService(ref BizObjects.ClientUnitSubscription ClientUnitSubscription, bool IsSpecial);
        void GetPastDueRemindersForScheduler(ref DataTable dtSubscriptionInvoices);
        int UpdateClientUnitSubscriptionService(ref BizObjects.ClientUnitSubscription ClientUnitSubscription, string FailedCode, string FailedDesc);
        int UpdateClientUnitSubscriptionServiceById(ref BizObjects.ClientUnitSubscription ClientUnitSubscription);
        int UpdatePaymentMethodByUnitId(int UnitId,string NewPaymentMethod,DateTime ChangeDate,int CardId,int UserId,int RoleId);
        int AddClientOrder(ref BizObjects.Orders objOrder);
    }

    public class ClientUnitSubscriptionService : IClientUnitSubscriptionService
    {
        IDataLib dbLib;

        public void GetClientUnitSubscription(string ClientName, string StartDate, string EndDate, string PaymentMethod, string Status, ref DataTable dtClientUnitSubscription)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientUnitSubscription_GetSubscription";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ClientName", SqlDbType.NVarChar, ClientName);
                dbLib.AddParameter("@StartDate", SqlDbType.NVarChar, StartDate);
                dbLib.AddParameter("@EndDate", SqlDbType.NVarChar, EndDate);
                dbLib.AddParameter("@PaymentMethod", SqlDbType.NVarChar, PaymentMethod);
                dbLib.AddParameter("@Status", SqlDbType.NVarChar, Status);            
                dbLib.RunSP(strsql, ref dtClientUnitSubscription);
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

        public void GetClientUnitUnPaidSubscription(string ClientName, ref DataTable dtClientUnitSubscription)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientUnitSubscription_GetUnPaidSubscription";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ClientName", SqlDbType.NVarChar, ClientName);
                dbLib.RunSP(strsql, ref dtClientUnitSubscription);
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

        public void GetClientUnitUnPaidSubscription(int ClientId, ref DataTable dtClientUnitSubscription)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientUnitSubscription_GetUnPaidSubscription_By_ClientId";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ClientId", SqlDbType.Int, ClientId);
                dbLib.RunSP(strsql, ref dtClientUnitSubscription);
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

        public void GetClientUnitSubscriptionForScheduler(DateTime PaymentDate, string PaymentMethod, ref DataTable dtClientUnitSubscription)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_GetClientSubscriptionForPaymetScheduler";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@PaymentDate", SqlDbType.DateTime, PaymentDate);
                dbLib.AddParameter("@PaymentMethod", SqlDbType.NVarChar, PaymentMethod);
                dbLib.RunSP(strsql, ref dtClientUnitSubscription);
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

        public void GetClientUnitSubscriptionById(Int64 Id, ref DataTable dtSubscription)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientUnitSubscription_GetById";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@Id", SqlDbType.BigInt, Id);
                dbLib.RunSP(strsql, ref dtSubscription);
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

        public void GetPastDueRemindersForScheduler(ref DataTable dtSubscription)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientUnitSubscription_GetPastDueSubscriptionsForScheduler";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.RunSP(strsql, ref dtSubscription);
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

        public void GetClientPaymentDueSubscriptionForScheduler(ref DataTable dtSubscription)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientUnitSubscription_GetPaymentDueForScheduler";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.RunSP(strsql, ref dtSubscription);
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

        public int AddClientUnitSubscriptionService(ref BizObjects.ClientUnitSubscription ClientUnitSubscription, bool IsSpecial)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_ClientUnitSubscription_Insert";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                ClientUnitSubscription.AddInsertParams(ref dbLib);
                dbLib.AddParameter("@IsSpecial", SqlDbType.Bit, IsSpecial);
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

        public int UpdateClientUnitSubscriptionService(ref BizObjects.ClientUnitSubscription ClientUnitSubscription, string FailedCode, string FailedDesc)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_ClientUnitSubscription_Update";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                ClientUnitSubscription.AddUpdateParams(ref dbLib);
                dbLib.AddParameter("@FailedCode", SqlDbType.NVarChar, FailedCode);
                dbLib.AddParameter("@FailedDesc", SqlDbType.NVarChar, FailedDesc);
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

        public int UpdateClientUnitSubscriptionServiceById(ref BizObjects.ClientUnitSubscription ClientUnitSubscription)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_ClientUnitSubscription_UpdateById";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@Id", SqlDbType.BigInt, ClientUnitSubscription.Id);
                dbLib.AddParameter("@CardId", SqlDbType.Int, ClientUnitSubscription.CardId);
                dbLib.AddParameter("@PONumber", SqlDbType.NVarChar, ClientUnitSubscription.PONumber);
                dbLib.AddParameter("@CheckNumber", SqlDbType.NVarChar, ClientUnitSubscription.CheckNumbers);
                dbLib.AddParameter("@FrontImage", SqlDbType.NVarChar, ClientUnitSubscription.FrontImage);
                dbLib.AddParameter("@BackImage", SqlDbType.NVarChar, ClientUnitSubscription.BackImage);
                dbLib.AddParameter("@AccountingNotes", SqlDbType.NVarChar, ClientUnitSubscription.AccountingNotes);
                dbLib.AddParameter("@Status", SqlDbType.NVarChar, ClientUnitSubscription.Status);
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

        public int UpdatePaymentMethodByUnitId(int UnitId, string NewPaymentMethod, DateTime ChangeDate, int CardId, int UserId, int RoleId)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_ClientUnitSubscription_ChangePaymentMethod";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@UnitId", SqlDbType.Int, UnitId);
                dbLib.AddParameter("@PaymentMethod", SqlDbType.NVarChar, NewPaymentMethod);
                dbLib.AddParameter("@ChangedDate", SqlDbType.DateTime, ChangeDate);
                dbLib.AddParameter("@CardId", SqlDbType.Int, CardId);
                dbLib.AddParameter("@UpdatedBy", SqlDbType.Int, UserId);
                dbLib.AddParameter("@UpdatedByType", SqlDbType.Int, RoleId);
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

        public int AddClientOrder(ref BizObjects.Orders objOrder)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_Orders_Insert";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                objOrder.AddInsertParams(ref dbLib);
                rtn = dbLib.ExeSP(strsql);
                objOrder.Id = rtn;
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
