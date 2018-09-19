using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace Services
{
    public interface IPartnerTicketConversationService
    {
        int AddConversation(ref BizObjects.PartnerTicketConversation Conversation);
        void GetConversationByTicketId(int TicketId, ref DataTable dtConversation);
    }

    public class PartnerTicketConversationService : IPartnerTicketConversationService
    {
        IDataLib dbLib;
        public int AddConversation(ref BizObjects.PartnerTicketConversation Conversation)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_PartnerTicketConversation_Insert";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                Conversation.AddInsertParams(ref dbLib);
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
        public void GetConversationByTicketId(int TicketId,ref DataTable dtConversation)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_PartnerTicketConversation_GetByTicketId";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@TicketId", SqlDbType.NVarChar, TicketId);
                dbLib.RunSP(strsql, ref dtConversation);
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

