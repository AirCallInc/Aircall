using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace Services
{
    public interface IServiceNoShowService
    {
        void GetAllByServiceId(long ServiceId, ref DataTable dtServiceNoShow);
        int AddServiceNoShow(ref BizObjects.ServiceNoShow ServiceNoShow);
        void GetNoShowDetailsByNotificationId(long NotificationId, int ClientId, ref DataTable dtServiceNoShow);
    }
    public class ServiceNoShowService:IServiceNoShowService
    {
        IDataLib dbLib;

        public void GetAllByServiceId(long ServiceId, ref DataTable dtServiceNoShow)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ServiceNoShow_GetByServiceId";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ServiceId", SqlDbType.BigInt, ServiceId);
                dbLib.RunSP(strsql, ref dtServiceNoShow);
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
        public void GetNoShowDetailsByNotificationId(long NotificationId, int ClientId, ref DataTable dtServiceNoShow)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientPortal_GetNoShowDetail";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@NotificationId", SqlDbType.BigInt, NotificationId);
                dbLib.AddParameter("@ClientId", SqlDbType.Int, ClientId);
                dbLib.RunSP(strsql, ref dtServiceNoShow);
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
        public int AddServiceNoShow(ref BizObjects.ServiceNoShow ServiceNoShow)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_ServiceNoShow_Insert";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                ServiceNoShow.AddInsertParams(ref dbLib);
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
