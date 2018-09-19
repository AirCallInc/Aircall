using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BizObjects;
using ZWT.DbLib;

namespace Services
{
    public interface IBillingHistoryService
    {
        void GetAllBillingHistoryByClientId(int ClientId, ref DataTable dtClientBilling);
        void GetBillingHistoryById(int BillingId, ref DataTable dtClientBilling);
        int AddBillingHistory(ref BizObjects.BillingHistory BillingHistory);
        int AddFailedBillingHistory(ref BizObjects.FailedBillingHistory BillingHistory);
        int AddClientUnitBillingHistory(ref BizObjects.BillingHistory BillingHistory);
        void GetAllBillingHistory(string ClientName, string StartDate, string EndDate, string Status, ref DataTable dtBilling);
        int UpdateBillingHistoryStatus(bool IsPaid, int BillingId);
    }
    public class BillingHistoryService : IBillingHistoryService
    {
        IDataLib dbLib;

        public int AddBillingHistory(ref BillingHistory BillingHistory)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_BillingHistory_Insert";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                BillingHistory.AddInsertParams(ref dbLib);
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
        public int AddFailedBillingHistory(ref BizObjects.FailedBillingHistory BillingHistory)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_FailedBillingHistory_Insert";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                BillingHistory.AddInsertParams(ref dbLib);
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
        public void GetAllBillingHistoryByClientId(int ClientId, ref DataTable dtClientBilling)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientPortal_GetBillingHistoryByClientId";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ClientId", SqlDbType.Int, ClientId);
                dbLib.RunSP(strsql, ref dtClientBilling);
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
        public void GetBillingHistoryById(int BillingId, ref DataTable dtClientBilling)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientPortal_GetBillingHistoryById";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@BillingId", SqlDbType.Int, BillingId);
                dbLib.RunSP(strsql, ref dtClientBilling);
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
        public int AddClientUnitBillingHistory(ref BizObjects.BillingHistory BillingHistory)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_BillingHistory_AddClientUnitBilling";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ClientId", SqlDbType.Int, BillingHistory.ClientId);
                dbLib.AddParameter("@ClientUnitIds", SqlDbType.NVarChar, BillingHistory.ClientUnitIds);
                dbLib.AddParameter("@ClientUnitSubscriptionId", SqlDbType.Int, BillingHistory.ClientUnitSubscriptionId);
                dbLib.AddParameter("@PaidMonths", SqlDbType.Int, BillingHistory.PaidMonths);
                dbLib.AddParameter("@OrderId", SqlDbType.Int, BillingHistory.OrderId);
                dbLib.AddParameter("@PackageName", SqlDbType.NVarChar, BillingHistory.PackageName);
                dbLib.AddParameter("@BillingType", SqlDbType.NVarChar, BillingHistory.BillingType);
                dbLib.AddParameter("@OriginalAmount", SqlDbType.Decimal, BillingHistory.OriginalAmount);
                dbLib.AddParameter("@PurchasedAmount", SqlDbType.Decimal, BillingHistory.PurchasedAmount);
                dbLib.AddParameter("@IsSpecialOffer", SqlDbType.Bit, BillingHistory.IsSpecialOffer);
                dbLib.AddParameter("@IsPaid", SqlDbType.Bit, BillingHistory.IsPaid);
                dbLib.AddParameter("@TransactionId", SqlDbType.NVarChar, BillingHistory.TransactionId);
                dbLib.AddParameter("@TransactionDate", SqlDbType.DateTime, BillingHistory.TransactionDate);
                dbLib.AddParameter("@AddedBy", SqlDbType.Int, BillingHistory.AddedBy);
                dbLib.AddParameter("@AddedDate", SqlDbType.DateTime, BillingHistory.AddedDate);
                dbLib.AddParameter("@CardId", SqlDbType.Int, BillingHistory.CardId);
                dbLib.AddParameter("@CheckNumbers", SqlDbType.NVarChar, BillingHistory.CheckNumbers);
                dbLib.AddParameter("@CheckAmounts", SqlDbType.NVarChar, BillingHistory.CheckAmounts);
                dbLib.AddParameter("@FailDesc", SqlDbType.NVarChar, BillingHistory.faildesc);
                dbLib.AddParameter("@PO", SqlDbType.NVarChar, BillingHistory.PO);
                dbLib.AddParameter("@TransactionStatus", SqlDbType.NVarChar, BillingHistory.TransactionStatus);
                dbLib.AddParameter("@ResponseReasonDescription", SqlDbType.NVarChar, BillingHistory.ResponseReasonDescription);
                dbLib.AddParameter("@ResponseCode", SqlDbType.Int, BillingHistory.ResponseCode);
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

        public void GetAllBillingHistory(string ClientName, string StartDate, string EndDate, string Status, ref DataTable dtBilling)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_BillingHistory_GetAll";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ClientName", SqlDbType.NVarChar, ClientName);
                dbLib.AddParameter("@StartDate", SqlDbType.NVarChar, StartDate);
                dbLib.AddParameter("@EndDate", SqlDbType.NVarChar, EndDate);
                dbLib.AddParameter("@Status", SqlDbType.NVarChar, Status);
                dbLib.RunSP(strsql, ref dtBilling);
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

        public int UpdateBillingHistoryStatus(bool IsPaid, int BillingId)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_UpdateBillingStatus";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@IsPaid", SqlDbType.Int, IsPaid);
                dbLib.AddParameter("@BillingId", SqlDbType.Int, BillingId);
                
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
