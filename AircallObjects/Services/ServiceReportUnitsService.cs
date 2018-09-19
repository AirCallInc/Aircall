using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace Services
{
    public interface IServiceReportUnitsService
    {
        void GetServiceReportUnitsByReportId(long ReportId, ref DataTable dtServiceUnits);
        void GetCompletedServiceUnitsByReportId(long ReportId, ref DataTable dtServiceUnits);
        int UpdateServiceReportIsCompletedByUnitAndReportId(ref BizObjects.ServiceReportUnits ServiceReportUnits);

    }

    public class ServiceReportUnitsService:IServiceReportUnitsService
    {
        IDataLib dbLib;

        public void GetServiceReportUnitsByReportId(long ReportId, ref DataTable dtServiceUnits)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ServiceReportUnits_GetByServiceReportId";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ServiceReportId", SqlDbType.BigInt, ReportId);
                dbLib.RunSP(strsql, ref dtServiceUnits);
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

        public void GetCompletedServiceUnitsByReportId(long ReportId, ref DataTable dtServiceUnits)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ServiceReportUnits_GetCompletedUnitsByReportId";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ServiceReportId", SqlDbType.BigInt, ReportId);
                dbLib.RunSP(strsql, ref dtServiceUnits);
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

        public int UpdateServiceReportIsCompletedByUnitAndReportId(ref BizObjects.ServiceReportUnits ServiceReportUnits)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_ServiceReportUnits_SetIsCompleted";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@UnitId", SqlDbType.Int, ServiceReportUnits.UnitId);
                dbLib.AddParameter("@IsCompleted", SqlDbType.Bit, ServiceReportUnits.IsCompleted);
                dbLib.AddParameter("@ServiceReportId", SqlDbType.BigInt, ServiceReportUnits.ServiceReportId);
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
