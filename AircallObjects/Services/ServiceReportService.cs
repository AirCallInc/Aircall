using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace Services
{
    public interface IServiceReportService
    {
        void GetAllServiceReport(string ServiceReportNo, string ClientName,string EmpName,string StartDate,string EndDate, string SortField, string SortDirection, ref DataTable dtServiceReport);
        void GetServiceReportById(long ReportId, ref DataTable dtServiceReport);
        void GetServiceReportByServiceId(long ServiceId, ref DataTable dtServiceReport);
        void GetServiceReportOfUnitsByServiceId(long ServiceId, ref DataTable dtServiceReport);
        void GetServiceReportsByUnitId(int UnitId, ref DataTable dtServiceReport);
        int UpdateServiceReport(ref BizObjects.ServiceReport ServiceReport);
    }

    public class ServiceReportService:IServiceReportService
    {
        IDataLib dbLib;

        public void GetAllServiceReport(string ServiceReportNo, string ClientName, string EmpName, string StartDate, string EndDate, string SortField, string SortDirection, ref DataTable dtServiceReport)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ServiceReport_GetAll";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ServiceReportNo", SqlDbType.NVarChar, ServiceReportNo);
                dbLib.AddParameter("@ClientName", SqlDbType.NVarChar, ClientName);
                dbLib.AddParameter("@EmpName", SqlDbType.NVarChar, EmpName);
                dbLib.AddParameter("@StartDate", SqlDbType.NVarChar, StartDate);
                dbLib.AddParameter("@EndDate", SqlDbType.NVarChar, EndDate);
                dbLib.AddParameter("@SortField", SqlDbType.NVarChar, SortField);
                dbLib.AddParameter("@SortDirection", SqlDbType.NVarChar, SortDirection);
                dbLib.RunSP(strsql, ref dtServiceReport);
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

        public void GetServiceReportById(long ReportId, ref DataTable dtServiceReport)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ServiceReport_GetById";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ReportId", SqlDbType.BigInt, ReportId);
                dbLib.RunSP(strsql, ref dtServiceReport);
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

        public void GetServiceReportByServiceId(long ServiceId, ref DataTable dtServiceReport)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ServiceReport_GetByServiceId";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ServiceId", SqlDbType.BigInt, ServiceId);
                dbLib.RunSP(strsql, ref dtServiceReport);
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

        public void GetServiceReportOfUnitsByServiceId(long ServiceId, ref DataTable dtServiceReport)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ServiceReport_GetUnitsByServiceId";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ServiceId", SqlDbType.BigInt, ServiceId);
                dbLib.RunSP(strsql, ref dtServiceReport);
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

        public void GetServiceReportsByUnitId(int UnitId, ref DataTable dtServiceReport)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ServiceReport_GetServiceReportsByUnitId";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@UnitId", SqlDbType.Int, UnitId);
                dbLib.RunSP(strsql, ref dtServiceReport);
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

        public int UpdateServiceReport(ref BizObjects.ServiceReport ServiceReport)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_ServiceReport_Update";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                ServiceReport.AddUpdateParams(ref dbLib);
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
