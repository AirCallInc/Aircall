using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace Services
{
    public interface IPartnerTicketRequestService
    {
        void GetAllTicketByPartnerId(int PartnerId, ref DataTable dtticket);
        int AddPartnerTicket(ref BizObjects.PartnerTicketRequest PartnerTicket);
        void getTicketDetailByTicketId(int TicketId, ref DataTable dtTicket);
        int UpdateStatusById(int TicketId);
        void GetAllTicket(string Partnername, string StartDate, string EndDate,ref DataTable dtTickets);
    }

    public class PartnerTicketRequestService : IPartnerTicketRequestService
    {
        IDataLib dbLib;
        public void GetAllTicketByPartnerId(int PartnerId, ref DataTable dtticket)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_PartnerTicketRequest_GetAllByPartnerId";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@PartnerId", SqlDbType.NVarChar, PartnerId);
                dbLib.RunSP(strsql, ref dtticket);
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
        public int AddPartnerTicket(ref BizObjects.PartnerTicketRequest PartnerTicket)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_PartnerTicketRequest_Insert";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                PartnerTicket.AddInsertParams(ref dbLib);
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
        public void getTicketDetailByTicketId(int TicketId, ref DataTable dtticket)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_PartnerTicketRequest_GetById";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@TicketId", SqlDbType.Int, TicketId);
                dbLib.RunSP(strsql, ref dtticket);
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
        public int UpdateStatusById(int TicketId)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_PartnerTicketRequest_UpdateStatus";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@TicketId", SqlDbType.Int, TicketId);
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
        
        public void GetAllTicket(string Partnername, string StartDate, string EndDate,ref DataTable dtTickets)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_PartnerTicketRequest_GetAll";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@PartnerName", SqlDbType.NVarChar, Partnername);
                dbLib.AddParameter("@StartDate", SqlDbType.NVarChar, StartDate);
                dbLib.AddParameter("@EndDate", SqlDbType.NVarChar, EndDate);
                dbLib.RunSP(strsql, ref dtTickets);
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
