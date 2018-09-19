using System;
using System.Data;
using ZWT.DbLib;

namespace Services
{
    public interface IUserNotificationService
    {
        long AddUserNotification(ref BizObjects.UserNotification UserNotification);
        void GetAllNotificationByUserId(int UserId, ref DataTable dtUsers);
        int DeleteNotificationById(int NotificationId);
        void GetAllNotificationForDashboardByUserId(int UserId, int AddressId, ref DataTable dtUsers);
        void GetBadgeCount(int UserId, int RoleId, ref DataTable dtBadgeCount);
        long DeleNotification(long ServiceId);
        long DeleteNotificationByCommonIdType(long CommonId, string MessageType);
        long DeleteNotificationByUserIdType(long UserId, int UserTypeId, string MessageType,long CommonId);
        int UpdateNotificationStatusByClientId(int ClientId);
        int UpdateStatusByClientIdNotificationIdMessageType(int ClientId, long NotificationId, string MessageType);
        int UpdateStatusById(long NotificationId);
        long DeleteNotificationByUserIdTypeScheduler(long UserId, int UserTypeId, string MessageType, long CommonId);
    }
    public class UserNotificationService : IUserNotificationService
    {
        IDataLib dbLib;

        public long AddUserNotification(ref BizObjects.UserNotification UserNotification)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            long rtn;
            strsql = "uspa_UserNotification_Insert";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                UserNotification.AddInsertParams(ref dbLib);
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
        public int DeleteNotificationById(int NotificationId)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_ClientPortal_UserNotification_DeleteById";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@NotificationId", SqlDbType.BigInt, NotificationId);
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
        public void GetAllNotificationByUserId(int UserId, ref DataTable dtUsers)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientPortal_GetNotificationForDashBoardByUserType";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ClientId", SqlDbType.BigInt, UserId);
                dbLib.AddParameter("@UserTypeId", SqlDbType.Int, 4);
                dbLib.AddParameter("@Status", SqlDbType.VarChar, "");
                dbLib.AddParameter("@callFrom", SqlDbType.Int, 0);
                dbLib.RunSP(strsql, ref dtUsers);
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
        public void GetAllNotificationForDashboardByUserId(int UserId, int AddressId, ref DataTable dtUsers)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientPortal_GetNotificationForDashBoardForClient";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ClientId", SqlDbType.BigInt, UserId);
                dbLib.AddParameter("@AddressId", SqlDbType.Int, AddressId);
                dbLib.RunSP(strsql, ref dtUsers);
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
        public void GetBadgeCount(int UserId, int RoleId, ref DataTable dtBadgeCount)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_UserNotification_GetBadgeCount";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@UserId", SqlDbType.Int, UserId);
                dbLib.AddParameter("@UserTypeId", SqlDbType.Int, RoleId);
                dbLib.RunSP(strsql, ref dtBadgeCount);
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

        public long DeleNotification(long ServiceId)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            long rtn;
            strsql = "uspa_UserNotification_DeleteNotification";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ServiceId", SqlDbType.BigInt, ServiceId);
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
        public long DeleteNotificationByCommonIdType(long CommonId, string MessageType)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            long rtn;
            strsql = "uspa_ClientPortal_DeleteNotificationByCommonIDType";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@CommonId", SqlDbType.BigInt, CommonId);
                dbLib.AddParameter("@MessageType", SqlDbType.NVarChar, MessageType);
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
        public long DeleteNotificationByUserIdType(long UserId, int UserTypeId, string MessageType,long CommonId)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            long rtn;
            strsql = "uspa_ClientPortal_DeleteNotificationByUserIDType";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@UserId", SqlDbType.BigInt, UserId);
                dbLib.AddParameter("@UserTypeId", SqlDbType.Int, UserTypeId);
                dbLib.AddParameter("@MessageType", SqlDbType.NVarChar, MessageType);
                dbLib.AddParameter("@CommonId", SqlDbType.BigInt, CommonId);
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

        public int UpdateNotificationStatusByClientId(int ClientId)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_ClientPortal_UserNotification_UpdateStatusByClientId";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ClientId", SqlDbType.Int, ClientId);
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

        public int UpdateStatusByClientIdNotificationIdMessageType(int ClientId, long CommonId, string MessageType)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_ClientPortal_UserNotification_UpdateStatusByClientIdNotificationIdMessageType";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ClientId", SqlDbType.Int, ClientId);
                dbLib.AddParameter("@CommonId", SqlDbType.BigInt, CommonId);
                dbLib.AddParameter("@MessageType", SqlDbType.NVarChar, MessageType);
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

        public int UpdateStatusById(long NotificationId)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_ClientPortal_UserNotificationById";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@NotificationId", SqlDbType.BigInt, NotificationId);
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

        public long DeleteNotificationByUserIdTypeScheduler(long UserId, int UserTypeId, string MessageType, long CommonId)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            long rtn;
            strsql = "uspa_ClientPortal_DeleteNotificationByUserIDTypeForScheduler";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@UserId", SqlDbType.BigInt, UserId);
                dbLib.AddParameter("@UserTypeId", SqlDbType.Int, UserTypeId);
                dbLib.AddParameter("@MessageType", SqlDbType.NVarChar, MessageType);
                dbLib.AddParameter("@CommonId", SqlDbType.BigInt, CommonId);
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
