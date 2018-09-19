using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace Services
{
    public interface IEmployeeScheduleService
    {
        int AddEmployeeLeaveSchedule(int EmployeeId, DateTime StartDate, DateTime EndDate, int LeaveId);
        int DeleteEmployeeScheduleByLeaveId(int LeaveId);
    }

    public class EmployeeScheduleService:IEmployeeScheduleService
    {
        IDataLib dbLib;
        public int AddEmployeeLeaveSchedule(int EmployeeId, DateTime StartDate, DateTime EndDate, int LeaveId)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_EmployeeSchedule_InsertLeave";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@EmployeeId", SqlDbType.Int, EmployeeId);
                dbLib.AddParameter("@StartDate", SqlDbType.DateTime, StartDate);
                dbLib.AddParameter("@EndDate", SqlDbType.DateTime, EndDate);
                dbLib.AddParameter("@LeaveId", SqlDbType.Int, LeaveId);
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

        public int DeleteEmployeeScheduleByLeaveId(int LeaveId)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_EmployeeSchedule_DeleteByLeaveId";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@LeaveId", SqlDbType.Int, LeaveId);
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
