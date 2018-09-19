using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace Services
{
    public interface IServiceDateChangeService
    {
        void GetAllServices(ref DataTable dtServices);
        void GetServiceById(long ServiceId, ref DataTable dtService);
        int UpdateService(long ServiceId, DateTime ExpStartdate, DateTime ExpEnddate);
    }

    public class ServiceDateChangeService : IServiceDateChangeService
    {
        IDataLib dbLib;

        public void GetAllServices(ref DataTable dtServices)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ServiceDate_GetAll";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.RunSP(strsql, ref dtServices);
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

        public void GetServiceById(long ServiceId, ref DataTable dtService)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ServiceDate_GetById";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ServiceId", SqlDbType.BigInt, ServiceId);
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

        public int UpdateService(long ServiceId, DateTime ExpStartdate, DateTime ExpEnddate)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_ServiceDate_Update";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ServiceId", SqlDbType.BigInt, ServiceId);
                dbLib.AddParameter("@ExpStartDate", SqlDbType.DateTime, ExpStartdate);
                dbLib.AddParameter("@ExpEndDate", SqlDbType.DateTime, ExpEnddate);
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
