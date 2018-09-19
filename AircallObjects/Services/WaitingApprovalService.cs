using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace Services
{
    public interface IWaitingApprovalService
    {
        void GetWaitingForApprovalService(string ServiceCaseNumber, string ServiceStatus, string EmpName, string ClientName, string WorkArea, string StartDate, string EndDate, string SortField, string SortDirection, ref DataTable dtService);

    }

    public class WaitingApprovalService : IWaitingApprovalService
    {
        IDataLib dbLib;

        public void GetWaitingForApprovalService(string ServiceCaseNumber, string ServiceStatus, string EmpName, string ClientName, string WorkArea, string StartDate, string EndDate, string SortField, string SortDirection, ref DataTable dtService)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Services_GetWaitingAndCancelService";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ServiceCaseNumber", SqlDbType.NVarChar, ServiceCaseNumber);
                dbLib.AddParameter("@ClientName", SqlDbType.NVarChar, ClientName);
                dbLib.AddParameter("@Status", SqlDbType.NVarChar, ServiceStatus);
                dbLib.AddParameter("@EmpName", SqlDbType.NVarChar, EmpName);
                dbLib.AddParameter("@WorkArea", SqlDbType.NVarChar, WorkArea);
                dbLib.AddParameter("@StartDate", SqlDbType.NVarChar, StartDate);
                dbLib.AddParameter("@EndDate", SqlDbType.NVarChar, EndDate);
                dbLib.AddParameter("@SortField", SqlDbType.NVarChar, SortField);
                dbLib.AddParameter("@SortDirection", SqlDbType.NVarChar, SortDirection);
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
