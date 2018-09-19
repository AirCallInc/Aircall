using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace Services
{
    public interface ISalesVisitRequestService
    {
        void GetAllSalesVisitRequest(string ClientName,string EmpName,ref DataTable dtSalesRequest);
        void GetSalesRequestById(int Id, ref DataTable dtSalesRequest);
        int AssignSalesEmployee(ref BizObjects.SalesVisitRequest SalesVisitRequest);
    }

    public class SalesVisitRequestService:ISalesVisitRequestService
    {
        IDataLib dbLib;

        public void GetAllSalesVisitRequest(string ClientName, string EmpName, ref DataTable dtSalesRequest)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_SalesVisitRequest_GetAll";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ClientName", SqlDbType.NVarChar, ClientName);
                dbLib.AddParameter("@EmpName", SqlDbType.NVarChar, EmpName);
                dbLib.RunSP(strsql, ref dtSalesRequest);
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

        public void GetSalesRequestById(int Id, ref DataTable dtSalesRequest)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_SalesVisitRequest_GetById";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@Id", SqlDbType.Int, Id);
                dbLib.RunSP(strsql, ref dtSalesRequest);
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

        public int AssignSalesEmployee(ref BizObjects.SalesVisitRequest SalesVisitRequest)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_SalesVisitRequest_AssignEmployee";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                SalesVisitRequest.AddUpdateParams(ref dbLib);
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
