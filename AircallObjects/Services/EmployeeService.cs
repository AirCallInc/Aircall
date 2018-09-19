using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace Services
{
    public interface IEmployeeService
    {
        void GetAllEmployees(bool IsActive, ref DataTable dtEmployee);
        void GetEmployeeById(int EmployeeId, ref DataTable dtEmployee);
        void GetEmployeeByEmail(string Email, ref DataTable dtEmployee);
        void GetAllSalesEmployeeByName(string EmpName, ref DataTable dtEmployee);
        void GetEmployeeByName(string EmpName, bool IncludeSalesPerson, ref DataTable dtEmployee);
        void GetAllEmployeesByName(string EmpName, string SortField, string SortDirection, ref DataTable dtEmployee);
        int SetStatus(bool IsActive, int EmployeeId);
        int AddEmployee(ref BizObjects.Employee Employee);
        int UpdateEmployee(ref BizObjects.Employee Employee);
        int DeleteEmployee(ref BizObjects.Employee Employee);
    }

    public class EmployeeService:IEmployeeService
    {
        IDataLib dbLib;

        public void GetAllEmployees(bool IsActive, ref DataTable dtEmployee)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Employee_SelectAll";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@IsActive", SqlDbType.Bit, IsActive);
                dbLib.RunSP(strsql, ref dtEmployee);
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

        public void GetEmployeeById(int EmployeeId, ref DataTable dtEmployee)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Employee_SelectByID";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@EmployeeId", SqlDbType.Int, EmployeeId);
                dbLib.RunSP(strsql, ref dtEmployee);
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

        public void GetEmployeeByEmail(string Email, ref DataTable dtEmployee)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Employee_GetByEmail";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@Email", SqlDbType.NVarChar, Email);
                dbLib.RunSP(strsql, ref dtEmployee);
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

        public void GetAllSalesEmployeeByName(string EmpName, ref DataTable dtEmployee)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Employee_GetSalesEmployeeByName";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@EmpName", SqlDbType.NVarChar, EmpName);
                dbLib.RunSP(strsql, ref dtEmployee);
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

        public void GetAllEmployeesByName(string EmpName, string SortField, string SortDirection, ref DataTable dtEmployee)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Employee_SelectAllByName";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@EmpName", SqlDbType.NVarChar, EmpName);
                dbLib.AddParameter("@SortField", SqlDbType.NVarChar, SortField);
                dbLib.AddParameter("@SortDirection", SqlDbType.NVarChar, SortDirection);
                dbLib.RunSP(strsql, ref dtEmployee);
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

        public void GetEmployeeByName(string EmpName, bool IncludeSalesPerson ,ref DataTable dtEmployee)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Employee_GetByEmpName";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@EmpName", SqlDbType.NVarChar, EmpName);
                dbLib.AddParameter("@IncludeSalesPerson",SqlDbType.Bit,IncludeSalesPerson);
                dbLib.RunSP(strsql, ref dtEmployee);
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

        public int SetStatus(bool IsActive, int EmployeeId)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_Employee_SetStatus";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@IsActive", SqlDbType.Bit, IsActive);
                dbLib.AddParameter("@EmployeeId", SqlDbType.Int, EmployeeId);
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

        public int AddEmployee(ref BizObjects.Employee Employee)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_Employee_Insert";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                Employee.AddInsertParams(ref dbLib);
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

        public int UpdateEmployee(ref BizObjects.Employee Employee)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_Employee_Update";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                Employee.AddUpdateParams(ref dbLib);
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

        public int DeleteEmployee(ref BizObjects.Employee Employee)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_Employee_Delete";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@EmployeeId", SqlDbType.Int, Employee.Id);
                dbLib.AddParameter("@DeletedBy", SqlDbType.Int, Employee.DeletedBy);
                dbLib.AddParameter("@DeletedByType", SqlDbType.Int, Employee.DeletedByType);
                dbLib.AddParameter("@DeletedDate", SqlDbType.DateTime, Employee.DeletedDate);
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
