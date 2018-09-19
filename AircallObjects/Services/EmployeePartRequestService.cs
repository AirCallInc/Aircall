using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace Services
{
    public interface IEmployeePartRequestService
    {
        void GetEmployeePartRequestByRequestId(int RequestId, ref DataTable dtEmployeeRequests);
        void GetPartRequestUnitsByReportId(long ReportId, ref DataTable dtRequestUnits);
        void GetPartsByReportAndUnitId(long ReportId, int UnitId, ref DataTable dtParts);
        int AddEmployeePartRequest(ref BizObjects.EmployeePartRequest EmployeePartRequest);
        int DeleteEmployeePartRequestByRequestId(int RequestId);
    }

    public class EmployeePartRequestService:IEmployeePartRequestService
    {
        IDataLib dbLib;

        public void GetEmployeePartRequestByRequestId(int RequestId, ref DataTable dtEmployeeRequests)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_EmployeePartRequest_GetByEmployeePartRequestId";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@EmployeePartRequestId", SqlDbType.Int, RequestId);
                dbLib.RunSP(strsql, ref dtEmployeeRequests);
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

        public void GetPartRequestUnitsByReportId(long ReportId, ref DataTable dtRequestUnits)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_EmployeePartRequest_GetPartRequetedUnitsByReportId";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ServiceReportId", SqlDbType.BigInt, ReportId);
                dbLib.RunSP(strsql, ref dtRequestUnits);
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
        
        public void GetPartsByReportAndUnitId(long ReportId, int UnitId, ref DataTable dtParts)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_EmployeePartRequest_GetPartsByReportIdAndUnitId";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ServiceReportId", SqlDbType.BigInt, ReportId);
                dbLib.AddParameter("@UnitId", SqlDbType.Int, UnitId);
                dbLib.RunSP(strsql, ref dtParts);
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

        public int AddEmployeePartRequest(ref BizObjects.EmployeePartRequest EmployeePartRequest)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_EmployeePartRequest_Insert";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                EmployeePartRequest.AddInsertParams(ref dbLib);
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

        public int DeleteEmployeePartRequestByRequestId(int RequestId)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_EmployeePartRequest_DeleteByRequestId";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@RequestId", SqlDbType.Int, RequestId);
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
