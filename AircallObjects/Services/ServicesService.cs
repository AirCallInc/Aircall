using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;
using System.Data;

namespace Services
{
    public interface IServicesService
    {
        void GetNoShowServicesByStatus(string ServiceStatus, string ServiceCaseNo, string EmpName, string ClientName, string WorkArea, string StartDate, string EndDate, string SortField, string SortDirection, string Req, ref DataTable dtServices);
        void GetServicesByStatus(string ServiceStatus, string ServiceCaseNo, string EmpName, string ClientName, string WorkArea, string StartDate, string EndDate, string SortField, string SortDirection, string Req, ref DataTable dtServices);
        void GetServiceById(long ServiceId, ref DataTable dtService);
        void GetServiceById(long ServiceId, int ClientId, ref DataTable dtService);
        void GetPendingAndRequestedService(string ServiceCaseNo, string ClientName, string StartDate, string EndDate, string SortField, string SortDirection, ref DataTable dtServices);
        void GetCompletedRequestedService(string ServiceCaseNo, string ClientName, string StartDate, string EndDate, string SortField, string SortDirection, ref DataTable dtServices);
        void GetAllCompletedServices(string ServiceCaseNo, string ClientName, string EmployeeName, string StartDate, string EndDate, string SortField, string SortDirection, ref DataTable dtServices);
        void GetMaterialListByServiceAndUnitId(long ServiceId, int UnitId, ref DataTable dtMaterialList);
        long InsertPendingService(ref BizObjects.Services Services);
        long InsertWaitingApprovalService(ref BizObjects.Services Services, long ReqServiceId);
        void ScheduleService(long ServiceId, ref DataTable dtService);
        int UpdateWaitingApprovalService(ref BizObjects.Services Services);
        int UpdateServiceStatus(long ServiceId, string Status);
        long UpdateService(ref BizObjects.Services Services);
        void SchedulePendingServiceFromAdmin(ref BizObjects.Services Services, string Units, ref DataTable dtService);
        void ScheduleRequestedServiceFromAdmin(ref BizObjects.Services Services, string Units, ref DataTable dtService);
        void AssignEmployeeFromAdmin(long ServiceId, int WorkAreaId, int EmployeeId, DateTime ScheduleDate, string StartTime, string EndTime, ref DataTable dtService);

        void GetServiceForClient(int ClientId, string Status, int SelectRow, ref DataTable dtServices);
        void GetServiceForClientById(int ServiceId, int ClientId, ref DataTable dtServices);
        void GetServiceForClientByStatus(int ClientId, string Status, ref DataTable dtServices);
        int UpdateServiceStatus(ref BizObjects.Services Services);
        void GetCompletedServiceDetailsById(long ServiceId, int ClientId, ref DataTable dtService);

        void GetServiceUnits(ref DataTable dtServiceUnits);
        void CheckServiceAlreadyAddedForClientAddress(int ClientId, int AddressId, int PlanTypeId, ref DataTable IsServiceAdded);
        void ScheduleAllServiceForClientUnit(int ClientId, int AddressId, int FirstServiceWithinDays, int DurationInMonth, int NumberOfService, int PlanTypeId, ref DataTable dtServiceId);
        void ScheduleAllServiceForClientUnitWithMerge(int ClientId, int AddressId, int FirstServiceWithinDays, int DurationInMonth, int NumberOfService, int PlanTypeId, ref DataTable dtServiceId);
        int RunWaitingApprovalToPendingServiceScheduler();
        void ScheduledServiceReminder(int NotifyType, bool IsMorning, ref DataTable dtReminder);
        int RunScheduler();
        void GetPendingServiceForScheduler(ref DataTable dtServices);
        void SchedulePendingService(long ServiceId, int ClientId, int AddressId, DateTime StartDate, DateTime EndDate, ref DataTable dtService);
        void SetServiceApprovalUrl(long ServiceId, ref DataTable dtApprovalUrl);
        void CheckServiceApprovalUrl(string Url, ref DataTable dtApprovalUrl);
        void ApproveCancelServiceByUrl(string Url, string Status);
        void RunScheduledServiceToPendingServiceScheduler();

        void GetCompletedServiceById(long ServiceId, ref DataTable dtService);
        long UpdateServiceOfNoShow(ref BizObjects.Services Services);
        void CheckServiceScheduleByUnitAndStatus(int UnitId, string Status, ref DataTable dtService);

        void GetServiceForEmployeePartList(string ServiceCaseNumber, string EmployeeName, string StartDate, string EndDate, ref DataTable dtServices);

        void GetCancelledAndReschedulledServices(ref DataTable dtServices);
        void RescheduledServiceSchedule(long ServiceId, int ClientId, int AddressId, ref DataTable dtService);
        void EmergencyRequestedServiceSchedule(long RequestedServiceId, ref DataTable dtService);
        void DeleteService(long ServiceId, int DeletedBy, int DeletedByType, DateTime DeletedDate, ref DataTable dtService);
    }

    public class ServicesService : IServicesService
    {
        IDataLib dbLib;
        
        public void GetNoShowServicesByStatus(string ServiceStatus, string ServiceCaseNo, string EmpName, string ClientName, string WorkArea, string StartDate, string EndDate, string SortField, string SortDirection, string Req, ref DataTable dtServices)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Services_GetNoShowServiceByStatus";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ServiceCaseNo", SqlDbType.NVarChar, ServiceCaseNo);
                dbLib.AddParameter("@ClientName", SqlDbType.NVarChar, ClientName);
                dbLib.AddParameter("@Status", SqlDbType.NVarChar, ServiceStatus);
                dbLib.AddParameter("@EmpName", SqlDbType.NVarChar, EmpName);
                dbLib.AddParameter("@WorkArea", SqlDbType.NVarChar, WorkArea);
                dbLib.AddParameter("@StartDate", SqlDbType.NVarChar, StartDate);
                dbLib.AddParameter("@EndDate", SqlDbType.NVarChar, EndDate);
                dbLib.AddParameter("@Purpose", SqlDbType.NVarChar, Req);
                dbLib.AddParameter("@SortField", SqlDbType.NVarChar, SortField);
                dbLib.AddParameter("@SortDirection", SqlDbType.NVarChar, SortDirection);
                dbLib.RunSP(strsql, ref dtServices);
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

        public void GetServicesByStatus(string ServiceStatus, string ServiceCaseNo,string EmpName, string ClientName, string WorkArea, string StartDate, string EndDate,string SortField, string SortDirection, string Req,ref DataTable dtServices)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Services_GetServiceByStatus";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ServiceCaseNo", SqlDbType.NVarChar, ServiceCaseNo);
                dbLib.AddParameter("@ClientName", SqlDbType.NVarChar, ClientName);
                dbLib.AddParameter("@Status", SqlDbType.NVarChar, ServiceStatus);
                dbLib.AddParameter("@EmpName", SqlDbType.NVarChar, EmpName);
                dbLib.AddParameter("@WorkArea", SqlDbType.NVarChar, WorkArea);
                dbLib.AddParameter("@StartDate", SqlDbType.NVarChar, StartDate);
                dbLib.AddParameter("@EndDate", SqlDbType.NVarChar, EndDate);
                dbLib.AddParameter("@Purpose", SqlDbType.NVarChar, Req);
                dbLib.AddParameter("@SortField", SqlDbType.NVarChar, SortField);
                dbLib.AddParameter("@SortDirection", SqlDbType.NVarChar, SortDirection);
                dbLib.RunSP(strsql, ref dtServices);
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

        public void GetServiceById(long ServiceId, ref DataTable dtService)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Services_SelectByID";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ServiceId", SqlDbType.BigInt, ServiceId);
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

        public void GetServiceById(long ServiceId, int ClientId, ref DataTable dtService)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Services_SelectByID";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ServiceId", SqlDbType.BigInt, ServiceId);
                dbLib.AddParameter("@ClientId", SqlDbType.Int, ClientId);
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

        public void GetPendingAndRequestedService(string ServiceCaseNo, string ClientName, string StartDate, string EndDate, string SortField, string SortDirection, ref DataTable dtServices)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Services_GetPendingAndRequestedService";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ServiceCaseNo", SqlDbType.NVarChar, ServiceCaseNo);
                dbLib.AddParameter("@ClientName", SqlDbType.NVarChar, ClientName);
                dbLib.AddParameter("@StartDate", SqlDbType.NVarChar, StartDate);
                dbLib.AddParameter("@EndDate", SqlDbType.NVarChar, EndDate);
                dbLib.AddParameter("@SortField", SqlDbType.NVarChar, SortField);
                dbLib.AddParameter("@SortDirection", SqlDbType.NVarChar, SortDirection);
                dbLib.RunSP(strsql, ref dtServices);
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

        public void GetMaterialListByServiceAndUnitId(long ServiceId, int UnitId, ref DataTable dtMaterialList)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_GetMaterialListByServiceAndUnitId";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ServiceId", SqlDbType.BigInt, ServiceId);
                dbLib.AddParameter("@UnitId", SqlDbType.Int, UnitId);
                dbLib.RunSP(strsql, ref dtMaterialList);
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

        public long InsertPendingService(ref BizObjects.Services Services)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_Services_InsertPendingService";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ClientId", SqlDbType.Int, Services.ClientId);
                dbLib.AddParameter("@AddressID", SqlDbType.Int, Services.AddressID);
                dbLib.AddParameter("@PurposeOfVisit", SqlDbType.NVarChar, Services.PurposeOfVisit);
                dbLib.AddParameter("@Status", SqlDbType.NVarChar, Services.Status);
                dbLib.AddParameter("@AddedBy", SqlDbType.Int, Services.AddedBy);
                dbLib.AddParameter("@AddedByType", SqlDbType.Int, Services.AddedByType);
                dbLib.AddParameter("@AddedDate", SqlDbType.DateTime, Services.AddedDate);
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

        public long InsertWaitingApprovalService(ref BizObjects.Services Services, long ReqServiceId)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            long rtn;
            strsql = "uspa_Services_InsertWaitingApprovalService";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ReqServiceId", SqlDbType.BigInt, ReqServiceId);
                dbLib.AddParameter("@ClientId", SqlDbType.Int, Services.ClientId);
                dbLib.AddParameter("@AddressID", SqlDbType.Int, Services.AddressID);
                dbLib.AddParameter("@PurposeOfVisit", SqlDbType.NVarChar, Services.PurposeOfVisit);
                dbLib.AddParameter("@WorkAreaId", SqlDbType.Int, Services.WorkAreaId);
                dbLib.AddParameter("@EmployeeId", SqlDbType.Int, Services.EmployeeId);
                dbLib.AddParameter("@ScheduleDate", SqlDbType.DateTime, Services.ScheduleDate);
                dbLib.AddParameter("@ScheduleStartTime", SqlDbType.NVarChar, Services.ScheduleStartTime);
                dbLib.AddParameter("@ScheduleEndTime", SqlDbType.NVarChar, Services.ScheduleEndTime);
                dbLib.AddParameter("@CusNote", SqlDbType.NVarChar, Services.CustomerComplaints);
                dbLib.AddParameter("@DispNotes", SqlDbType.NVarChar, Services.DispatcherNotes);
                dbLib.AddParameter("@EmpNote", SqlDbType.NVarChar, Services.TechnicianNotes);
                dbLib.AddParameter("@Status", SqlDbType.NVarChar, Services.Status);
                dbLib.AddParameter("@ScheduledBy", SqlDbType.Int, Services.ScheduledBy);
                dbLib.AddParameter("@AddedBy", SqlDbType.Int, Services.AddedBy);
                dbLib.AddParameter("@AddedByType", SqlDbType.Int, Services.AddedByType);
                dbLib.AddParameter("@AddedDate", SqlDbType.DateTime, Services.AddedDate);
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

        public void ScheduleService(long ServiceId, ref DataTable dtService)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Services_ScheduleService";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ServiceId", SqlDbType.BigInt, ServiceId);
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

        public void GetAllCompletedServices(string ServiceCaseNo, string ClientName, string EmployeeName, string StartDate, string EndDate, string SortField, string SortDirection, ref DataTable dtServices)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Services_GetAllCompletedService";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ServiceCaseNo", SqlDbType.NVarChar, ServiceCaseNo);
                dbLib.AddParameter("@ClientName", SqlDbType.NVarChar, ClientName);
                dbLib.AddParameter("@EmployeeName", SqlDbType.NVarChar, EmployeeName);
                dbLib.AddParameter("@Startdate", SqlDbType.NVarChar, StartDate);
                dbLib.AddParameter("@EndDate", SqlDbType.NVarChar, EndDate);
                dbLib.AddParameter("@SortField", SqlDbType.NVarChar, SortField);
                dbLib.AddParameter("@SortDirection", SqlDbType.NVarChar, SortDirection);
                dbLib.RunSP(strsql, ref dtServices);
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

        public void GetCompletedRequestedService(string ServiceCaseNo, string ClientName, string StartDate, string EndDate, string SortField, string SortDirection, ref DataTable dtServices)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Services_GetCompletedRequestedService";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ServiceCaseNo", SqlDbType.NVarChar, ServiceCaseNo);
                dbLib.AddParameter("@ClientName", SqlDbType.NVarChar, ClientName);
                dbLib.AddParameter("@Startdate", SqlDbType.NVarChar, StartDate);
                dbLib.AddParameter("@EndDate", SqlDbType.NVarChar, EndDate);
                dbLib.AddParameter("@SortField", SqlDbType.NVarChar, SortField);
                dbLib.AddParameter("@SortDirection", SqlDbType.NVarChar, SortDirection);
                dbLib.RunSP(strsql, ref dtServices);
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

        public int UpdateWaitingApprovalService(ref BizObjects.Services Services)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_Services_UpdateWaitingApprovalService";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ServiceId", SqlDbType.BigInt, Services.Id);
                dbLib.AddParameter("@AddedDate", SqlDbType.DateTime, Services.AddedDate);
                dbLib.AddParameter("@PurposeOfVisit", SqlDbType.NVarChar, Services.PurposeOfVisit);
                dbLib.AddParameter("@WorkAreaId", SqlDbType.Int, Services.WorkAreaId);
                dbLib.AddParameter("@EmployeeId", SqlDbType.Int, Services.EmployeeId);
                dbLib.AddParameter("@ScheduleDate", SqlDbType.DateTime, Services.ScheduleDate);
                dbLib.AddParameter("@ScheduleStartTime", SqlDbType.NVarChar, Services.ScheduleStartTime);
                dbLib.AddParameter("@ScheduleEndTime", SqlDbType.NVarChar, Services.ScheduleEndTime);
                dbLib.AddParameter("@CusNote", SqlDbType.NVarChar, Services.CustomerComplaints);
                dbLib.AddParameter("@DispNotes", SqlDbType.NVarChar, Services.DispatcherNotes);
                dbLib.AddParameter("@EmpNote", SqlDbType.NVarChar, Services.TechnicianNotes);
                dbLib.AddParameter("@Status", SqlDbType.NVarChar, Services.Status);
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

        public int UpdateServiceStatus(long ServiceId, string Status)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_Services_ChangeServiceStatus";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ServiceId", SqlDbType.BigInt, ServiceId);
                dbLib.AddParameter("@Status", SqlDbType.NVarChar, Status);
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

        public long UpdateService(ref BizObjects.Services Services)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            long rtn;
            strsql = "uspa_Services_Update";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                Services.AddUpdateParams(ref dbLib);
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

        public void SchedulePendingServiceFromAdmin(ref BizObjects.Services Services, string Units, ref DataTable dtService)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_PendingServiceScheduleFromAdmin";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ServiceId", SqlDbType.BigInt, Services.Id);
                dbLib.AddParameter("@StatusChangeDate", SqlDbType.DateTime, Services.StatusChangeDate);
                dbLib.AddParameter("@PurposeOfVisit", SqlDbType.NVarChar, Services.PurposeOfVisit);
                dbLib.AddParameter("@WorkAreaId", SqlDbType.Int, Services.WorkAreaId);
                dbLib.AddParameter("@EmployeeId", SqlDbType.Int, Services.EmployeeId);
                dbLib.AddParameter("@ScheduleDate", SqlDbType.DateTime, Services.ScheduleDate);
                dbLib.AddParameter("@ScheduleStartTime", SqlDbType.NVarChar, Services.ScheduleStartTime);
                dbLib.AddParameter("@ScheduleEndTime", SqlDbType.NVarChar, Services.ScheduleEndTime);
                dbLib.AddParameter("@CusNote", SqlDbType.NVarChar, Services.CustomerComplaints);
                dbLib.AddParameter("@DispNotes", SqlDbType.NVarChar, Services.DispatcherNotes);
                dbLib.AddParameter("@EmpNote", SqlDbType.NVarChar, Services.TechnicianNotes);
                dbLib.AddParameter("@Status", SqlDbType.NVarChar, Services.Status);
                dbLib.AddParameter("@ScheduledBy", SqlDbType.Int, Services.ScheduledBy);
                dbLib.AddParameter("@UpdatedBy", SqlDbType.Int, Services.UpdatedBy);
                dbLib.AddParameter("@UpdatedByType", SqlDbType.Int, Services.UpdatedByType);
                dbLib.AddParameter("@UpdatedDate", SqlDbType.DateTime, Services.UpdatedDate);
                dbLib.AddParameter("@Units", SqlDbType.NVarChar, Units);
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

        public void ScheduleRequestedServiceFromAdmin(ref BizObjects.Services Services, string Units, ref DataTable dtService)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_RequestedServiceScheduleFromAdmin";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@RequestedServiceId", SqlDbType.BigInt, Services.Id);
                dbLib.AddParameter("@PurposeOfVisit", SqlDbType.NVarChar, Services.PurposeOfVisit);
                dbLib.AddParameter("@WorkAreaId", SqlDbType.Int, Services.WorkAreaId);
                dbLib.AddParameter("@EmployeeId", SqlDbType.Int, Services.EmployeeId);
                dbLib.AddParameter("@ScheduleDate", SqlDbType.DateTime, Services.ScheduleDate);
                dbLib.AddParameter("@ScheduleTime", SqlDbType.NVarChar, Services.ScheduleStartTime + " - " + Services.ScheduleEndTime);
                //dbLib.AddParameter("@ScheduleEndTime", SqlDbType.NVarChar, Services.ScheduleEndTime);
                dbLib.AddParameter("@Units", SqlDbType.NVarChar, Units);
                dbLib.AddParameter("@CusNote", SqlDbType.NVarChar, Services.CustomerComplaints);
                dbLib.AddParameter("@DispNotes", SqlDbType.NVarChar, Services.DispatcherNotes);
                dbLib.AddParameter("@EmpNote", SqlDbType.NVarChar, Services.TechnicianNotes);
                dbLib.AddParameter("@UpdatedBy", SqlDbType.Int, Services.AddedBy);
                dbLib.AddParameter("@UpdatedByType", SqlDbType.Int, Services.AddedByType);
                dbLib.AddParameter("@UpdatedDate", SqlDbType.DateTime, Services.AddedDate);
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

        public void AssignEmployeeFromAdmin(long ServiceId, int WorkAreaId, int EmployeeId, DateTime ScheduleDate, string StartTime, string EndTime, ref DataTable dtService)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_AssignEmployeeFromAdmin";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ServiceId", SqlDbType.BigInt, ServiceId);
                dbLib.AddParameter("@WorkAreaId", SqlDbType.Int, WorkAreaId);
                dbLib.AddParameter("@EmployeeId", SqlDbType.Int, EmployeeId);
                dbLib.AddParameter("@ScheduleDate", SqlDbType.DateTime, ScheduleDate);
                dbLib.AddParameter("@StartTime", SqlDbType.NVarChar, StartTime);
                dbLib.AddParameter("@EndTime", SqlDbType.NVarChar, EndTime);
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

        public void GetServiceForClient(int ClientId, string Status, int SelectRow, ref DataTable dtServices)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientPortal_Services_GetService";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ClientId", SqlDbType.Int, ClientId);
                dbLib.AddParameter("@Status", SqlDbType.NVarChar, Status);
                dbLib.AddParameter("@SelectRow", SqlDbType.Int, SelectRow);
                dbLib.RunSP(strsql, ref dtServices);
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
        public void GetServiceForClientById(int ServiceId, int ClientId, ref DataTable dtServices)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientPortal_Services_SelectByID";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ServiceId", SqlDbType.Int, ServiceId);
                dbLib.AddParameter("@ClientId", SqlDbType.Int, ClientId);
                dbLib.RunSP(strsql, ref dtServices);
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
        public void GetServiceForClientByStatus(int ClientId, string Status, ref DataTable dtServices)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientPortal_Services_GetByClientId";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ClientId", SqlDbType.Int, ClientId);
                dbLib.AddParameter("@Status", SqlDbType.NVarChar, Status);
                dbLib.RunSP(strsql, ref dtServices);
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

        public int UpdateServiceStatus(ref BizObjects.Services Services)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_ClientPortal_Services_UpdateStatus";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ServiceId", SqlDbType.BigInt, Services.Id);
                dbLib.AddParameter("@Status", SqlDbType.NVarChar, Services.Status);
                dbLib.AddParameter("@UpdatedBy", SqlDbType.Int, Services.UpdatedBy);
                dbLib.AddParameter("@UpdatedByType", SqlDbType.Int, Services.UpdatedByType);
                dbLib.AddParameter("@UpdatedDate", SqlDbType.DateTime, Services.UpdatedDate);
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

        public void GetCompletedServiceDetailsById(long ServiceId, int ClientId, ref DataTable dtService)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientPorta_Services_GetCompletedServiceDetails";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ServiceId", SqlDbType.BigInt, ServiceId);
                dbLib.AddParameter("@ClientId", SqlDbType.Int, ClientId);
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

        public void GetServiceUnits(ref DataTable dtServiceUnits)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_GetServiceUnits";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.RunSP(strsql, ref dtServiceUnits);
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

        public void CheckServiceAlreadyAddedForClientAddress(int ClientId, int AddressId, int PlanTypeId, ref DataTable IsServiceAdded)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_CheckServiceAlreadyAddedForClientAddress";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ClientId", SqlDbType.Int, ClientId);
                dbLib.AddParameter("@AddressId", SqlDbType.Int, AddressId);
                dbLib.AddParameter("@PlanTypeId", SqlDbType.Int, PlanTypeId);
                dbLib.RunSP(strsql, ref IsServiceAdded);
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

        public void ScheduleAllServiceForClientUnit(int ClientId, int AddressId, int FirstServiceWithinDays, int DurationInMonth, int NumberOfService, int PlanTypeId, ref DataTable dtServiceId)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ScheduleAllServicesForClientUnitAndNotify";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ClientId", SqlDbType.Int, ClientId);
                dbLib.AddParameter("@AddressId", SqlDbType.Int, AddressId);
                dbLib.AddParameter("@FirstServiceWithinDays", SqlDbType.Int, FirstServiceWithinDays);
                dbLib.AddParameter("@DurationInMonth", SqlDbType.Int, DurationInMonth);
                dbLib.AddParameter("@NumberOfService", SqlDbType.Int, NumberOfService);
                dbLib.AddParameter("@PlanTypeId", SqlDbType.Int, PlanTypeId);
                dbLib.RunSP(strsql, ref dtServiceId);
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

        public void ScheduleAllServiceForClientUnitWithMerge(int ClientId, int AddressId, int FirstServiceWithinDays, int DurationInMonth, int NumberOfService, int PlanTypeId, ref DataTable dtServiceId)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ScheduleAllServicesForClientUnitWithMergeAndNotify";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ClientId", SqlDbType.Int, ClientId);
                dbLib.AddParameter("@AddressId", SqlDbType.Int, AddressId);
                dbLib.AddParameter("@FirstServiceWithinDays", SqlDbType.Int, FirstServiceWithinDays);
                dbLib.AddParameter("@DurationInMonth", SqlDbType.Int, DurationInMonth);
                dbLib.AddParameter("@NumberOfService", SqlDbType.Int, NumberOfService);
                dbLib.AddParameter("@PlanTypeId", SqlDbType.Int, PlanTypeId);
                dbLib.RunSP(strsql, ref dtServiceId);
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

        public int RunWaitingApprovalToPendingServiceScheduler()
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_ServiceWaitingApprovalToPendingScheduler";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
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

        public void ScheduledServiceReminder(int NotifyType, bool IsMorning, ref DataTable dtReminder)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ReminderToClientForServiceScheduler";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@Notification", SqlDbType.Int, NotifyType);
                dbLib.AddParameter("@InMorning", SqlDbType.Bit, IsMorning);
                dbLib.RunSP(strsql, ref dtReminder);
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

        public int RunScheduler()
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_ScheduleServicesForClient";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
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

        public void GetPendingServiceForScheduler(ref DataTable dtServices)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Services_GetPendingServiceForScheduler";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.RunSP(strsql, ref dtServices);
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

        public void SchedulePendingService(long ServiceId, int ClientId, int AddressId, DateTime StartDate, DateTime EndDate, ref DataTable dtService)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_SchedulePendingServices";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ServiceId", SqlDbType.BigInt, ServiceId);
                dbLib.AddParameter("@ClientId", SqlDbType.Int, ClientId);
                dbLib.AddParameter("@AddressId", SqlDbType.Int, AddressId);
                dbLib.AddParameter("@ExpectedStartDate", SqlDbType.DateTime, StartDate);
                dbLib.AddParameter("@ExpectedEndDate", SqlDbType.DateTime, EndDate);
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

        public void SetServiceApprovalUrl(long ServiceId, ref DataTable dtApprovalUrl)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Services_SetApprovalEmailUrl";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ServiceId", SqlDbType.BigInt, ServiceId);
                dbLib.RunSP(strsql, ref dtApprovalUrl);
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

        public void CheckServiceApprovalUrl(string Url, ref DataTable dtApprovalUrl)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Services_CheckApprovalEmailUrl";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@Url", SqlDbType.NVarChar, Url);
                dbLib.RunSP(strsql, ref dtApprovalUrl);
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

        public void ApproveCancelServiceByUrl(string Url, string Status)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Services_ApproveServiceByUrl";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@Url", SqlDbType.NVarChar, Url);
                dbLib.AddParameter("@Status", SqlDbType.NVarChar, Status);
                dbLib.ExeSP(strsql);
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

        public void RunScheduledServiceToPendingServiceScheduler()
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ServiceScheduledToPendingScheduler";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.ExeSP(strsql);
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

        public void GetCompletedServiceById(long ServiceId, ref DataTable dtService)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Services_GetCompletedServiceByServiceId";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ServiceId", SqlDbType.BigInt, ServiceId);
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

        public long UpdateServiceOfNoShow(ref BizObjects.Services Services)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            long rtn;
            strsql = "uspa_Services_UpdateNoShowService";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@Id", SqlDbType.BigInt, Services.Id);
                dbLib.AddParameter("@UpdatedBy", SqlDbType.Int, Services.UpdatedBy);
                dbLib.AddParameter("@UpdatedByType", SqlDbType.Int, Services.UpdatedByType);
                dbLib.AddParameter("@UpdatedDate", SqlDbType.DateTime, Services.UpdatedDate);
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

        public void CheckServiceScheduleByUnitAndStatus(int UnitId, string Status, ref DataTable dtService)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Services_CheckAnyServiceIsScheduleByUnitAndStatus";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@UnitId", SqlDbType.Int, UnitId);
                dbLib.AddParameter("@Status", SqlDbType.NVarChar, Status);
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

        public void GetServiceForEmployeePartList(string ServiceCaseNumber, string EmployeeName, string StartDate, string EndDate, ref DataTable dtServices)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Services_GetServiceForEmployeePartList";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ServiceCaseNumber", SqlDbType.NVarChar, ServiceCaseNumber);
                dbLib.AddParameter("@EmpName", SqlDbType.NVarChar, EmployeeName);
                dbLib.AddParameter("@StartDate", SqlDbType.NVarChar, StartDate);
                dbLib.AddParameter("@EndDate", SqlDbType.NVarChar, EndDate);
                dbLib.RunSP(strsql, ref dtServices);
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

        public void GetCancelledAndReschedulledServices(ref DataTable dtServices)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Services_GetCancelledAndRescheduledService";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.RunSP(strsql, ref dtServices);
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

        public void RescheduledServiceSchedule(long ServiceId, int ClientId, int AddressId, ref DataTable dtService)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_RescheduledServiceSchedule";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ServiceId", SqlDbType.BigInt, ServiceId);
                dbLib.AddParameter("@ClientId", SqlDbType.Int, ClientId);
                dbLib.AddParameter("@AddressId", SqlDbType.Int, AddressId);
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

        public void EmergencyRequestedServiceSchedule(long RequestedServiceId,ref DataTable dtService)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_EmergencyRequestedServiceSchedule";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@RequestedServiceId", SqlDbType.BigInt, RequestedServiceId);
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

        public void DeleteService(long ServiceId, int DeletedBy, int DeletedByType, DateTime DeletedDate, ref DataTable dtService)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Services_DeleteById";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ServiceId", SqlDbType.BigInt, ServiceId);
                dbLib.AddParameter("@DeletedBy", SqlDbType.Int, DeletedBy);
                dbLib.AddParameter("@DeletedByType", SqlDbType.Int, DeletedByType);
                dbLib.AddParameter("@DeletedDate", SqlDbType.DateTime, DeletedDate);
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
