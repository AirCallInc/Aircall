using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace Services
{
    public interface IAdminService
    {
        void GetDashboard(ref DataTable dtDashboard);
        void GetMonthlySaleReportChart(string Client, int Month, int Year, int PlanType, ref DataTable dtMonthlySale);
        void GetMonthlyRateReportChart(string Employee, string StartDate, string EndDate, ref DataTable dtMonthlyRate);
        void GetUnitReportChart(string ManufactureName, string StartDate, string EndDate,bool IsPackaged, ref DataTable dtUnit);
        void GetUnitAgeWiseCount(ref DataTable dtUnit);
        void GetPartnerSaleReportChart(string Partner, int Month, int Year, ref DataTable dtMonthlySale);
        void GetPartnerSaleReportTable(string Partner, int Month, int Year, ref DataTable dtMonthlySale);
        void GetBillingHistoryReportChart(string Client, string StartDate, string EndDate, ref DataTable dtMonthlyRate);
        void GetBillingHistoryReportTable(string Client, string StartDate, string EndDate, ref DataTable dtMonthlyRate);
    }

    class AdminService : IAdminService
    {
        IDataLib dbLib;
        public void GetDashboard(ref DataTable dtDashboard)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Admin_GetDashboardCount";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.RunSP(strsql, ref dtDashboard);
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

        public void GetMonthlyRateReportChart(string Employee, string StartDate, string EndDate, ref DataTable dtMonthlyRate)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Services_GetAllRatingForReport";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@EmployeeName", SqlDbType.NVarChar, Employee);
                dbLib.AddParameter("@StartDate", SqlDbType.NVarChar, StartDate);
                dbLib.AddParameter("@EndDate", SqlDbType.NVarChar, EndDate);
                dbLib.RunSP(strsql, ref dtMonthlyRate);
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

        public void GetBillingHistoryReportChart(string Client, string StartDate, string EndDate, ref DataTable dtMonthlyRate)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Admin_BillingHistoryReportChart";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ClientName", SqlDbType.NVarChar, Client);
                dbLib.AddParameter("@FromDate", SqlDbType.NVarChar, StartDate);
                dbLib.AddParameter("@ToDate", SqlDbType.NVarChar, EndDate);
                dbLib.RunSP(strsql, ref dtMonthlyRate);
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

        public void GetBillingHistoryReportTable(string Client, string StartDate, string EndDate, ref DataTable dtMonthlyRate)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Admin_BillingHistoryReportTable";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ClientName", SqlDbType.NVarChar, Client);
                dbLib.AddParameter("@FromDate", SqlDbType.NVarChar, StartDate);
                dbLib.AddParameter("@ToDate", SqlDbType.NVarChar, EndDate);
                dbLib.RunSP(strsql, ref dtMonthlyRate);
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

        public void GetUnitReportChart(string ManufactureName, string StartDate, string EndDate,bool IsPackaged, ref DataTable dtUnit)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Services_GetAllUnitServicedReportChart";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ManufactureName", SqlDbType.NVarChar, ManufactureName);
                dbLib.AddParameter("@StartDate", SqlDbType.NVarChar, StartDate);
                dbLib.AddParameter("@EndDate", SqlDbType.NVarChar, EndDate);
                dbLib.AddParameter("@IsPackaged", SqlDbType.Bit, IsPackaged);
                dbLib.RunSP(strsql, ref dtUnit);
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

        public void GetUnitAgeWiseCount(ref DataTable dtUnit)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Admin_GetAllUnitCountAgeWise";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.RunSP(strsql, ref dtUnit);
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

        public void GetMonthlySaleReportChart(string Client, int Month, int Year, int PlanType, ref DataTable dtMonthlySale)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Admin_GetMonthlySaleReport";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@Client", SqlDbType.NVarChar, Client);
                dbLib.AddParameter("@Month", SqlDbType.Int, Month);
                dbLib.AddParameter("@Year", SqlDbType.Int, Year);
                dbLib.AddParameter("@PlanTypeId", SqlDbType.Int, PlanType);
                dbLib.RunSP(strsql, ref dtMonthlySale);
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

        public void GetPartnerSaleReportChart(string Partner, int Month, int Year, ref DataTable dtMonthlySale)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Admin_GetPartnerSaleReportChart";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@Partner", SqlDbType.NVarChar, Partner);
                dbLib.AddParameter("@Month", SqlDbType.Int, Month);
                dbLib.AddParameter("@Year", SqlDbType.Int, Year);
                dbLib.RunSP(strsql, ref dtMonthlySale);
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

        public void GetPartnerSaleReportTable(string Partner, int Month, int Year, ref DataTable dtMonthlySale)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Admin_GetClientListByParner";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@Partner", SqlDbType.NVarChar, Partner);
                dbLib.AddParameter("@Month", SqlDbType.Int, Month);
                dbLib.AddParameter("@Year", SqlDbType.Int, Year);
                dbLib.RunSP(strsql, ref dtMonthlySale);
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
