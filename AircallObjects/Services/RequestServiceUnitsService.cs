using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace Services
{
    public interface IRequestServiceUnitsService
    {
        void GetRequestServiceUnitByRequestServiceId(long RequestServiceId, ref DataTable dtReqUnits);
        int AddRequestServiceUnits(ref BizObjects.RequestServiceUnits RequestServiceUnits);
        int AddRequestServiceUnitsClientPortal(ref BizObjects.RequestServiceUnits RequestServiceUnits);
        int DeleteRequestServiceUnitsByReqServiceId(long RequestServiceId);
    }

    public class RequestServiceUnitsService:IRequestServiceUnitsService
    {
        IDataLib dbLib;

        public void GetRequestServiceUnitByRequestServiceId(long RequestServiceId, ref DataTable dtReqUnits)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_RequestedServiceUnits_GetByRequestedServiceId";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@RequestServiceId", SqlDbType.BigInt, RequestServiceId);
                dbLib.RunSP(strsql, ref dtReqUnits);
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

        public int AddRequestServiceUnits(ref BizObjects.RequestServiceUnits RequestServiceUnits)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_RequestedServiceUnits_Insert";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                RequestServiceUnits.AddInsertParams(ref dbLib);
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
        public int AddRequestServiceUnitsClientPortal(ref BizObjects.RequestServiceUnits RequestServiceUnits)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_ClientPortal_RequestedServiceUnits_Insert";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                RequestServiceUnits.AddInsertParams(ref dbLib);
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

        public int DeleteRequestServiceUnitsByReqServiceId(long RequestServiceId)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_RequestedServiceUnits_DeleteByServiceId";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@RequestServiceId", SqlDbType.BigInt, RequestServiceId);
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
