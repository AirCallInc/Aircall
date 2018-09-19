using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace Services
{
    public interface IServiceAttemptCountService
    {
        void GetAttemptCountsByServiceId(long ServiceId, ref DataTable dtAttemptCount);
    }

    public class ServiceAttemptCountService:IServiceAttemptCountService
    {
        IDataLib dbLib;

        public void GetAttemptCountsByServiceId(long ServiceId, ref DataTable dtAttemptCount)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ServiceAttemptCount_GetByServiceId";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ServiceId", SqlDbType.BigInt, ServiceId);
                dbLib.RunSP(strsql, ref dtAttemptCount);
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
