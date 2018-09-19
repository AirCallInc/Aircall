using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace Services
{
    public interface IServiceUnitService
    {
        void GetServiceUnitByServiceId(long ServiceId, ref DataTable dtServiceUnits);
        int AddServiceUnit(ref BizObjects.ServiceUnits ServiceUnits);

        void GetServiceUnitsForPortal(long ServiceId, ref DataTable dtServiceUnits);
    }

    public class ServiceUnitService:IServiceUnitService
    {
        IDataLib dbLib;

        public void GetServiceUnitByServiceId(long ServiceId, ref DataTable dtServiceUnits)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ServiceUnits_GetByServiceId";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ServiceId", SqlDbType.BigInt, ServiceId);
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

        public int AddServiceUnit(ref BizObjects.ServiceUnits ServiceUnits)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_ServiceUnits_Insert";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                ServiceUnits.AddInsertParams(ref dbLib);
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

        public void GetServiceUnitsForPortal(long ServiceId, ref DataTable dtServiceUnits)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientPortal_ServiceUnits_GetServiceUnits";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ServiceId", SqlDbType.BigInt, ServiceId);
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

    }
}
