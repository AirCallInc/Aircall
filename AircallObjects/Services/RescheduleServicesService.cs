using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace Services
{
    public interface IRescheduleServicesService
    {
        void GetAllByServiceId(long ServiceId, ref DataTable dtRescheduleService);
        int AddRescheduleService(ref BizObjects.RescheduleService RescheduleService,string Status);
    }

    public class RescheduleServicesService:IRescheduleServicesService
    {
        IDataLib dbLib;

        public void GetAllByServiceId(long ServiceId, ref DataTable dtRescheduleService)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_RescheduleService_GetAllByServiceId";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ServiceId", SqlDbType.BigInt, ServiceId);
                dbLib.RunSP(strsql, ref dtRescheduleService);
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

        public int AddRescheduleService(ref BizObjects.RescheduleService RescheduleService,string Status)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_RescheduleService_Insert";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                RescheduleService.AddInsertParams(ref dbLib);
                dbLib.AddParameter("@Status", SqlDbType.NVarChar, Status);
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
