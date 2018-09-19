using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace Services
{
    public interface ICalendarService
    {
        void GetCityWorkAreaEmployees(int CityId, int EmployeeId, ref DataTable dtWorkAreaEmployee);
        void GetWorkAreaEmployees(int AreaId, int EmployeeId, ref DataTable dtWorkAreaEmployee);
        void GetEmployeeSchedule(int EmployeeId, ref DataTable dtEmployeeSchedule);
        void GetEmployeeScheduleCityWise(int EmployeeId, int CityId, ref DataTable dtEmployeeSchedule);
        void AssignEmployeeFromCalendar(long ServiceId, int EmployeeId, DateTime ServiceDate, string StartTime, string EndTime, int ScheduleBy, int ScheduleByType,ref DataTable dtService);
        void GetMapDataByEmployeeId(int EmployeeId, ref DataTable dtMapData);
    }
    
    public class CalendarService:ICalendarService
    {
        IDataLib dbLib;
        public void GetCityWorkAreaEmployees(int CityId, int EmployeeId, ref DataTable dtWorkAreaEmployee)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Calendar_GetCityWorkAreaEmployee";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@CitiesId", SqlDbType.Int, CityId);
                dbLib.AddParameter("@EmployeeId", SqlDbType.Int, EmployeeId);
                dbLib.RunSP(strsql, ref dtWorkAreaEmployee);
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
        public void GetWorkAreaEmployees(int AreaId,int EmployeeId,ref DataTable dtWorkAreaEmployee)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Calendar_GetWorkAreaEmployee";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@AreaId", SqlDbType.Int, AreaId);
                dbLib.AddParameter("@EmployeeId", SqlDbType.Int, EmployeeId);
                dbLib.RunSP(strsql, ref dtWorkAreaEmployee);
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

        public void GetEmployeeSchedule(int EmployeeId, ref DataTable dtEmployeeSchedule)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Calendar_GetEmployeeSchedule";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@EmployeeId", SqlDbType.Int, EmployeeId);
                dbLib.RunSP(strsql, ref dtEmployeeSchedule);
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

        public void GetEmployeeScheduleCityWise(int EmployeeId, int CityId, ref DataTable dtEmployeeSchedule)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Calendar_GetEmployeeSchedule_New";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@EmployeeId", SqlDbType.Int, EmployeeId);
                dbLib.AddParameter("@CityId", SqlDbType.Int, CityId);
                dbLib.RunSP(strsql, ref dtEmployeeSchedule);
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

        public void AssignEmployeeFromCalendar(long ServiceId, int EmployeeId, DateTime ServiceDate, string StartTime, string EndTime, int ScheduleBy, int ScheduleByType, ref DataTable dtService)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Calendar_AssignEmployeeToService";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ServiceId", SqlDbType.BigInt, ServiceId);
                dbLib.AddParameter("@EmployeeId", SqlDbType.Int, EmployeeId);
                dbLib.AddParameter("@ScheduleDate", SqlDbType.DateTime, ServiceDate);
                dbLib.AddParameter("@StartTime", SqlDbType.NVarChar, StartTime);
                dbLib.AddParameter("@EndTime", SqlDbType.NVarChar, EndTime);
                dbLib.AddParameter("@UpdatedBy", SqlDbType.Int, ScheduleBy);
                dbLib.AddParameter("@UpdatedByType", SqlDbType.Int, ScheduleByType);
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

        public void GetMapDataByEmployeeId(int EmployeeId, ref DataTable dtMapData)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Calendar_GetMapMarker";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@EmployeeId", SqlDbType.Int, EmployeeId);
                dbLib.RunSP(strsql, ref dtMapData);
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
