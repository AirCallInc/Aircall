using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace Services
{
    public interface IEmployeePartRequestMasterService
    {
        void GetAllEmployeePartRequest(ref DataTable dtEmployeeRequests);
        void GetAllEmployeePartRequestByPartTypeId(string EmpName, string PartName, int PartTypeId, int Status,ref DataTable dtEmployeeRequests);
        void GetEmployeePartRequestById(int RequestId, ref DataTable dtEmployeeRequest);
        void CheckInstockAndScheduleService(int RequestId,int AddedBy,int AddedByType,DateTime AddedDate,ref DataTable dtService);
        void ArrangePartAndScheduleService(int RequestId, int AddedBy, int AddedByType, DateTime AddedDate,ref DataTable dtService);
        int AddEmployeePartRequest(ref BizObjects.EmployeePartRequestMaster EmployeePartRequestMaster);
        int UpdateEmployeePartRequest(ref BizObjects.EmployeePartRequestMaster EmployeePartRequestMaster);
        void GetMissingInventoryReportTable(DateTime FromDate, DateTime ToDate, string PartName, string EmpName, ref DataTable dtEmployeeRequest);
        void GetMissingInventoryReportChart(DateTime FromDate, DateTime ToDate, string PartName, string EmpName, ref DataTable dtEmployeeRequest);
    }

    public class EmployeePartRequestMasterService:IEmployeePartRequestMasterService
    {
        IDataLib dbLib;

        public void GetAllEmployeePartRequest(ref DataTable dtEmployeeRequests)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_EmployeePartRequestMaster_SelectAll";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                //dbLib.AddParameter("@EmployeeName", SqlDbType.NVarChar, EmployeeName);
                //dbLib.AddParameter("@StartDate", SqlDbType.NVarChar, StartDate);
                //dbLib.AddParameter("@EndDate", SqlDbType.NVarChar, EndDate);
                dbLib.RunSP(strsql, ref dtEmployeeRequests);
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

        public void GetAllEmployeePartRequestByPartTypeId(string EmpName, string PartName, int PartTypeId, int Status, ref DataTable dtEmployeeRequests)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_EmployeePartRequestMaster_SelectAllByPartTypeId";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@EmpName", SqlDbType.NVarChar, EmpName);
                dbLib.AddParameter("@PartName", SqlDbType.NVarChar, PartName);
                dbLib.AddParameter("@PartTypeId", SqlDbType.Int, PartTypeId);
                dbLib.AddParameter("@Status", SqlDbType.Int, Status);
                dbLib.RunSP(strsql, ref dtEmployeeRequests);
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

        public void GetEmployeePartRequestById(int RequestId, ref DataTable dtEmployeeRequest)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_EmployeePartRequestMaster_SelectByID";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@Id", SqlDbType.Int, RequestId);
                dbLib.RunSP(strsql, ref dtEmployeeRequest);
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

        public void GetMissingInventoryReportTable(DateTime FromDate, DateTime ToDate, string PartName, string EmpName, ref DataTable dtEmployeeRequest)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_GetMIssingInventoryReport";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@FromDate", SqlDbType.DateTime, FromDate);
                dbLib.AddParameter("@ToDate", SqlDbType.DateTime, ToDate);
                dbLib.AddParameter("@PartName", SqlDbType.NVarChar, PartName);
                dbLib.AddParameter("@EmpName", SqlDbType.NVarChar, EmpName);
                dbLib.RunSP(strsql, ref dtEmployeeRequest);
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

        public void GetMissingInventoryReportChart(DateTime FromDate, DateTime ToDate, string PartName, string EmpName, ref DataTable dtEmployeeRequest)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_GetMIssingInventoryReportChart";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@FromDate", SqlDbType.DateTime, FromDate);
                dbLib.AddParameter("@ToDate", SqlDbType.DateTime, ToDate);
                dbLib.AddParameter("@PartName", SqlDbType.NVarChar, PartName);
                dbLib.AddParameter("@EmpName", SqlDbType.NVarChar, EmpName);
                dbLib.RunSP(strsql, ref dtEmployeeRequest);
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

        public void CheckInstockAndScheduleService(int RequestId, int AddedBy, int AddedByType, DateTime AddedDate, ref DataTable dtService)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_CheckInStockAndScheduleService";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@EmployeePartRequestId", SqlDbType.Int, RequestId);
                dbLib.AddParameter("@AddedBy", SqlDbType.Int, AddedBy);
                dbLib.AddParameter("@AddedByType", SqlDbType.Int, AddedByType);
                dbLib.AddParameter("@AddedDate", SqlDbType.DateTime, AddedDate);
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

        public void ArrangePartAndScheduleService(int RequestId, int AddedBy, int AddedByType, DateTime AddedDate, ref DataTable dtService)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ArrangePartAndScheduleService";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@EmployeePartRequestId", SqlDbType.Int, RequestId);
                dbLib.AddParameter("@AddedBy", SqlDbType.Int, AddedBy);
                dbLib.AddParameter("@AddedByType", SqlDbType.Int, AddedByType);
                dbLib.AddParameter("@AddedDate", SqlDbType.DateTime, AddedDate);
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

        public int AddEmployeePartRequest(ref BizObjects.EmployeePartRequestMaster EmployeePartRequestMaster)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_EmployeePartRequestMaster_Insert";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                EmployeePartRequestMaster.AddInsertParams(ref dbLib);
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

        public int UpdateEmployeePartRequest(ref BizObjects.EmployeePartRequestMaster EmployeePartRequestMaster)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_EmployeePartRequestMaster_Update";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                EmployeePartRequestMaster.AddUpdateParams(ref dbLib);
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
