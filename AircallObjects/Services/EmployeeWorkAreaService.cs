using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace Services
{
    public interface IEmployeeWorkAreaService
    {
        void Get1stPriorityGroupEmployee(string Employee,string AreaName,ref DataTable dtEmployees);
        void GetWorkAreaByPriority(Int16 Priority,int EmployeeId,ref DataTable dtWorkAreas);
        void GetAllEmployeeByAreaId(int AreaId, string EmpName, bool IncludeSalesPerson, ref DataTable dtEmployees);
        void GetEmployeeWorkAreaByEmployeeId(int EmployeeId, ref DataTable dtEmployeeWorkArea);
        void GetEmployeeByClientAddressId(string EmpName,int AddressId, ref DataTable dtEmployee);
        int AddEmployeeWorkArea(ref BizObjects.EmployeeWorkArea EmployeeWorkArea);
        int DeleteEmployeeWorkAreaByEmployeeId(int EmployeeId);
        int DeleteEmployeeWorkArea(long EmployeeWorkAreaId, int UpdatedBy, int UpdatedByType);
    }

    public class EmployeeWorkAreaService:IEmployeeWorkAreaService
    {
        IDataLib dbLib;

        public void Get1stPriorityGroupEmployee(string Employee,string AreaName,ref DataTable dtEmployees)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_EmployeeWorkarea_SelectAll";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@EmpName", SqlDbType.NVarChar, Employee);
                dbLib.AddParameter("@AreaName", SqlDbType.NVarChar, AreaName);
                dbLib.RunSP(strsql, ref dtEmployees);
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

        public void GetWorkAreaByPriority(Int16 Priority,int EmployeeId,ref DataTable dtWorkAreas)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_EmployeeWorkarea_GetByPriority";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@Priority", SqlDbType.SmallInt, Priority);
                dbLib.AddParameter("@EmployeeId", SqlDbType.Int, EmployeeId);
                dbLib.RunSP(strsql, ref dtWorkAreas);
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

        public void GetAllEmployeeByAreaId(int AreaId, string EmpName,bool IncludeSalesPerson,ref DataTable dtEmployees)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_EmployeeWorkarea_GetEmployeeByAreaId";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@AreaId", SqlDbType.Int, AreaId);
                dbLib.AddParameter("@EmpName", SqlDbType.NVarChar, EmpName);
                dbLib.AddParameter("@IncludeSalesPerson", SqlDbType.Bit, IncludeSalesPerson);
                dbLib.RunSP(strsql, ref dtEmployees);
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

        public void GetEmployeeWorkAreaByEmployeeId(int EmployeeId, ref DataTable dtEmployeeWorkArea)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_EmployeeWorkarea_GetByEmployeeId";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@EmployeeId", SqlDbType.Int, EmployeeId);
                dbLib.RunSP(strsql, ref dtEmployeeWorkArea);
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

        public void GetEmployeeByClientAddressId(string EmpName,int AddressId, ref DataTable dtEmployee)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Employee_GetByClientAddress";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@AddressId", SqlDbType.Int, AddressId);
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

        public int AddEmployeeWorkArea(ref BizObjects.EmployeeWorkArea EmployeeWorkArea)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_EmployeeWorkarea_Insert";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                EmployeeWorkArea.AddInsertParams(ref dbLib);
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

        public int DeleteEmployeeWorkAreaByEmployeeId(int EmployeeId)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_EmployeeWorkarea_DeleteByEmployeeId";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
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

        public int DeleteEmployeeWorkArea(long EmployeeWorkAreaId, int UpdatedBy, int UpdatedByType)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_Employeeworkarea_Delete";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@EmployeeWorkareaId", SqlDbType.BigInt, EmployeeWorkAreaId);
                dbLib.AddParameter("@UpdatedBy", SqlDbType.Int, UpdatedBy);
                dbLib.AddParameter("@UpdatedByType", SqlDbType.Int, UpdatedByType);
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
