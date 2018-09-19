using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace Services
{
    public interface IEmployeePlanTypeService
    {
        void GetAllPlanTypeByEmployeeId(int EmployeeId, ref DataTable dtPlanType);
        int AddEmployeePlanType(ref BizObjects.EmployeePlanType EmployeePlanType);
    }

    public class EmployeePlanTypeService:IEmployeePlanTypeService
    {
        IDataLib dbLib;

        public void GetAllPlanTypeByEmployeeId(int EmployeeId, ref DataTable dtPlanType)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_EmployeePlanType_GetByEmployeeId";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@EmployeeId", SqlDbType.Int, EmployeeId);
                dbLib.RunSP(strsql, ref dtPlanType);
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

        public int AddEmployeePlanType(ref BizObjects.EmployeePlanType EmployeePlanType)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_EmployeePlanType_Insert";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                EmployeePlanType.AddInsertParams(ref dbLib);
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
