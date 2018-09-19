using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace Services
{
    public interface IClientPaymentMethodService
    {
        void GetClientPaymentMethodById(int Id, ref DataTable dtCardInfo);
        void GetClientPaymentMethodByClientId(int ClientId, ref DataTable dtClientPayment);
        void GetClientPaymentMethodByOrderId(int OrderId, ref DataTable dtClientPayment);
        void GetClientPaymentMethodByStripeCardId(string StripeCardId, ref DataTable dtClientPayment);
        int AddClientPaymentMethod(ref BizObjects.ClientPaymentMethod ClientPaymentMethod);
        int UpdateClientPaymentMethod(ref BizObjects.ClientPaymentMethod ClientPaymentMethod);

        void CardExpirationScheduler(ref DataTable dtCardExpiration);
        int UpdateIsExpireationFlagByCardId(int CardId);
    }

    public class ClientPaymentMethodService:IClientPaymentMethodService
    {
        IDataLib dbLib;


        public void GetClientPaymentMethodById(int Id, ref DataTable dtCardInfo)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientPortal_ClientPaymentMethod_GetById";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@Id", SqlDbType.Int, Id);
                dbLib.RunSP(strsql, ref dtCardInfo);
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

        public void GetClientPaymentMethodByClientId(int ClientId, ref DataTable dtClientPayment)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientPaymentMethod_GetByClientId";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ClientId", SqlDbType.Int, ClientId);
                dbLib.RunSP(strsql, ref dtClientPayment);
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

        public void GetClientPaymentMethodByOrderId(int OrderId, ref DataTable dtClientPayment)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_GetCardIdFromOrderId";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@OrderId", SqlDbType.Int, OrderId);
                dbLib.RunSP(strsql, ref dtClientPayment);
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

        public void GetClientPaymentMethodByStripeCardId(string StripeCardId, ref DataTable dtClientPayment)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientPaymentMethod_GetByStripeCardId";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@StripeCardId", SqlDbType.NVarChar, StripeCardId);
                dbLib.RunSP(strsql, ref dtClientPayment);
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

        public int AddClientPaymentMethod(ref BizObjects.ClientPaymentMethod ClientPaymentMethod)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_ClientPaymentMethod_Insert";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                ClientPaymentMethod.AddInsertParams(ref dbLib);
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

        public int UpdateClientPaymentMethod(ref BizObjects.ClientPaymentMethod ClientPaymentMethod)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_ClientPortal_ClientPaymentMethod_Update";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                ClientPaymentMethod.AddUpdateParams(ref dbLib);
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

        public void CardExpirationScheduler(ref DataTable dtCardExpiration)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_CardExpirationScheduler";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.RunSP(strsql, ref dtCardExpiration);
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

        public int UpdateIsExpireationFlagByCardId(int CardId)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_ClientPaymentMethod_UpdateIsExpireationFlag";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@CardId", SqlDbType.Int, CardId);
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
