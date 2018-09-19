using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace Services
{
    public interface IServicePartListService
    {
        void GetServicePartlistByServiceId(long ServiceId, ref DataTable dtServiceParts);
        void GetServicePartlistByServiceIdForEmployeePartList(long ServiceId, ref DataTable dtServiceParts);
        int UpdateIsUsedById(int ServicePartListId, bool IsUsed);
    }
    
    public class ServicePartListService:IServicePartListService
    {
        IDataLib dbLib;

        public void GetServicePartlistByServiceId(long ServiceId, ref DataTable dtServiceParts)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ServicePartList_GetByServiceId";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ServiceId", SqlDbType.BigInt, ServiceId);
                dbLib.RunSP(strsql, ref dtServiceParts);
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

        public void GetServicePartlistByServiceIdForEmployeePartList(long ServiceId, ref DataTable dtServiceParts)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ServicePartList_GetByServiceIdForEmpPartList";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ServiceId", SqlDbType.BigInt, ServiceId);
                dbLib.RunSP(strsql, ref dtServiceParts);
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

        public int UpdateIsUsedById(int ServicePartListId, bool IsUsed)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_ServicePartList_UpdateIsUsedById";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@Id", SqlDbType.Int, ServicePartListId);
                dbLib.AddParameter("@IsUsed", SqlDbType.Bit, IsUsed);
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
