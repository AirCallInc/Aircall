using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace Services
{
    public interface IRequestServicesService
    {
        void GetRequestedServiceById(long RequestServiceId, ref DataTable dtReqService);
        void GetRequestedServiceById(long RequestServiceId, int ClientId, ref DataTable dtReqService);
        void GetServiceByRequestedId(long RequestServiceId, int ClientId, ref DataTable dtReqService);
        void GetRequestedServiceDetailsById(long RequestServiceId, int ClientId, ref DataTable dtReqService);
        void GetRequestedServiceAddressByClientId(long ClientId, ref DataTable dtReqService);
        void GetRequestedServiceByClientId(long ClientId, ref DataTable dtReqService);
        int AddRequestService(ref BizObjects.RequestService RequestService);
        int AddRequestServiceClientPortal(ref BizObjects.RequestService RequestService);
        int UpdateRequestService(ref BizObjects.RequestService RequestService);
        int UpdateServiceIdByReqServiceId(long ServiceId, long ReqServiceId);
        int DeleteRequestedService(long RequestServiceId, int DeletedBy, int DeletedByType, DateTime DeletedDate);

        void GetAllRequestedServiceForSchedule(ref DataTable dtRequestedServices);
        void ScheduleRequestedService(long ServiceId, long RequestedServiceId, int ClientId, int AddressId, string PurposeOfVisit, DateTime RequestedServiceDate, string RequestedServiceTime, ref DataTable dtService);
    }
    // 
    public class RequestServicesService : IRequestServicesService
    {
        IDataLib dbLib;

        public void GetRequestedServiceById(long RequestServiceId, ref DataTable dtReqService)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_RequestedServices_GetById";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@RequestServiceId", SqlDbType.BigInt, RequestServiceId);
                dbLib.RunSP(strsql, ref dtReqService);
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
        public void GetServiceByRequestedId(long RequestServiceId, int ClientId, ref DataTable dtReqService)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_RequestedServices_GetSercicesByRequestId";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@RequestServiceId", SqlDbType.BigInt, RequestServiceId);
                dbLib.AddParameter("@ClientId", SqlDbType.Int, ClientId);
                dbLib.RunSP(strsql, ref dtReqService);
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
        public void GetRequestedServiceById(long RequestServiceId, int ClientId, ref DataTable dtReqService)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_RequestedServices_GetById";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@RequestServiceId", SqlDbType.BigInt, RequestServiceId);
                dbLib.AddParameter("@ClientId", SqlDbType.Int, ClientId);
                dbLib.RunSP(strsql, ref dtReqService);
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
        public void GetRequestedServiceDetailsById(long RequestServiceId, int ClientId, ref DataTable dtReqService)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientPortal_GetRequestedServiceDetails";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ServiceId", SqlDbType.BigInt, RequestServiceId);
                dbLib.AddParameter("@ClientId", SqlDbType.Int, ClientId);
                dbLib.RunSP(strsql, ref dtReqService);
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
        public void GetRequestedServiceAddressByClientId(long ClientId, ref DataTable dtReqService)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientPortal_RequestedServices_GetAddressByClientId";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ClientId", SqlDbType.BigInt, ClientId);
                dbLib.RunSP(strsql, ref dtReqService);
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
        public void GetRequestedServiceByClientId(long ClientId, ref DataTable dtReqService)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientPortal_RequestedServices_GetByClientId";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ClientId", SqlDbType.BigInt, ClientId);
                dbLib.RunSP(strsql, ref dtReqService);
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

        public int AddRequestService(ref BizObjects.RequestService RequestService)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_RequestedServices_Insert";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                RequestService.AddInsertParams(ref dbLib);
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
        public int AddRequestServiceClientPortal(ref BizObjects.RequestService RequestService)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_ClientPortal_RequestedServices_Insert";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                RequestService.AddInsertParams(ref dbLib);
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

        public int UpdateRequestService(ref BizObjects.RequestService RequestService)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_RequestedServices_Update";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                RequestService.AddUpdateParams(ref dbLib);
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

        public int UpdateServiceIdByReqServiceId(long ServiceId, long ReqServiceId)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_RequestedServices_UpdateServiceId";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ServiceId", SqlDbType.BigInt, ServiceId);
                dbLib.AddParameter("@ReqServiceId", SqlDbType.BigInt, ReqServiceId);
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

        public int DeleteRequestedService(long RequestServiceId, int DeletedBy, int DeletedByType, DateTime DeletedDate)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_ClientPortal_RequestedServices_Delete";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@RequestedServiceId", SqlDbType.BigInt, RequestServiceId);
                dbLib.AddParameter("@DeletedBy", SqlDbType.Int, DeletedBy);
                dbLib.AddParameter("@DeletedByType", SqlDbType.Int, DeletedByType);
                dbLib.AddParameter("@DeletedDate", SqlDbType.DateTime, DeletedDate);
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

        public void GetAllRequestedServiceForSchedule(ref DataTable dtRequestedServices)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_GetAllRequestedServiceForSchedule";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.RunSP(strsql, ref dtRequestedServices);
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

        public void ScheduleRequestedService(long ServiceId, long RequestedServiceId, int ClientId, int AddressId, string PurposeOfVisit, DateTime RequestedServiceDate, string RequestedServiceTime, ref DataTable dtService)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_RequestedServiceToServiceScheduler";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ServiceId", SqlDbType.BigInt, ServiceId);
                dbLib.AddParameter("@RequestedServiceId", SqlDbType.BigInt, RequestedServiceId);
                dbLib.AddParameter("@ClientId", SqlDbType.Int, ClientId);
                dbLib.AddParameter("@AddressId", SqlDbType.Int, AddressId);
                dbLib.AddParameter("@PurposeOfVisit", SqlDbType.NVarChar, PurposeOfVisit);
                dbLib.AddParameter("@RequestedServiceDate", SqlDbType.DateTime, RequestedServiceDate);
                dbLib.AddParameter("@RequestedServiceTime", SqlDbType.NVarChar, RequestedServiceTime);
                dbLib.RunSP(strsql, ref dtService);
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
