using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace Services
{
    public interface IEmployeeLeaveService
    {
        void GetAllEmployeeLeave(string EmpName, string StartDate, string EndDate, ref DataTable dtEmpLeave);
        void GetEmployeeLeaveById(int LeaveId, ref DataTable dtLeave);
        int AddEmployeeLeave(ref BizObjects.EmployeeLeave EmployeeLeave);
        int UpdateEmployeeLeave(ref BizObjects.EmployeeLeave EmployeeLeave);
        int DeleteEmployeeLeave(ref BizObjects.EmployeeLeave EmployeeLeave);
    }

    public class EmployeeLeaveService:IEmployeeLeaveService
    {
        IDataLib dbLib;
        public void GetAllEmployeeLeave(string EmpName, string StartDate, string EndDate, ref DataTable dtEmpLeave)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_EmployeeLeave_SelectAll";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@EmpName", SqlDbType.NVarChar, EmpName);
                dbLib.AddParameter("@StartDate", SqlDbType.NVarChar, StartDate);
                dbLib.AddParameter("@EndDate", SqlDbType.NVarChar, EndDate);
                dbLib.RunSP(strsql, ref dtEmpLeave);
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

        public void GetEmployeeLeaveById(int LeaveId, ref DataTable dtLeave)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_EmployeeLeave_SelectByID";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@Id", SqlDbType.Int, LeaveId);
                dbLib.RunSP(strsql, ref dtLeave);
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

        public int AddEmployeeLeave(ref BizObjects.EmployeeLeave EmployeeLeave)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_EmployeeLeave_Insert";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                EmployeeLeave.AddInsertParams(ref dbLib);
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

        public int UpdateEmployeeLeave(ref BizObjects.EmployeeLeave EmployeeLeave)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_EmployeeLeave_Update";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                EmployeeLeave.AddUpdateParams(ref dbLib);
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

        public int DeleteEmployeeLeave(ref BizObjects.EmployeeLeave EmployeeLeave)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_EmployeeLeave_Delete";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@LeaveId", SqlDbType.Int, EmployeeLeave.Id);
                dbLib.AddParameter("@DeletedBy", SqlDbType.Int, EmployeeLeave.DeletedBy);
                dbLib.AddParameter("@DeletedDate", SqlDbType.DateTime, EmployeeLeave.DeletedDate);
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
