using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace Services
{
    public interface IPartnerService
    {
        void GetAllPartners(string PartnerName,string SortField,string SortDirection,bool IsActive, ref DataTable dtPartners);
        void GetPartnerByAffiliateId(string AffiliateId, ref DataTable dtPartner);
        void GetPartnerById(int PartnerId, ref DataTable dtPartner);
        int SetStatus(bool IsActive, int PartnerId,int UserId,int RoleId,DateTime Date);
        int AddPartner(ref BizObjects.Partner Partner);
        int UpdatePartner(ref BizObjects.Partner Partner);

        void CheckPartnerLogin(string Partnername, string Password, ref DataTable dtPartner);
        void GetPartnerInfoByEmail(string Email, ref DataTable dtPartner);
        void GetPartnerInfoByPartnerName(string Partnername, string Email,ref DataTable dtPartner);
        int SetForgotPasswordLink(string Email, string PasswordUrl);
        void CheckPartnerResetPasswordLink(string PasswordUrl, ref DataTable dtResult);
        void PartnerResetPassword(string PasswordUrl, string Password);
        void ChangePassword(string Username, string Password);
        int UpdatePartnerProfile(ref BizObjects.Partner Partner);
        void GetClientListByPartnerId(int PartnerId, ref DataTable dtClients);
        void GetDashboardGraphByPartnerId(int PartnerId, ref DataTable dtPartnerSales);
        void GetAllClientByPartner(int ParentId, ref DataTable dtClients);
        void GetProfiteByPartner(int ClientId, int AffiliateId, int strMonth, int strYear, ref DataTable dtClient);
        void GetPartnerSaleReportChart(int PartnerId, int Month, int Year, ref DataTable dtMonthlySale);        
    }

    public class PartnerService : IPartnerService
    {
        IDataLib dbLib;

        public void GetPartnerSaleReportChart(int PartnerId, int Month, int Year, ref DataTable dtMonthlySale)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Admin_GetPartnerSaleReportChartById";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@PartnerId", SqlDbType.Int, PartnerId);
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
        public void GetAllPartners(string PartnerName, string SortField, string SortDirection, bool IsActive, ref DataTable dtPartners)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Partner_SelectAll";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@IsActive", SqlDbType.Bit, IsActive);
				 dbLib.AddParameter("@PartnerName", SqlDbType.NVarChar, PartnerName);
                dbLib.AddParameter("@SortField", SqlDbType.NVarChar, SortField);
                dbLib.AddParameter("@SortDirection", SqlDbType.NVarChar, SortDirection);
                dbLib.RunSP(strsql, ref dtPartners);
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

        public void GetPartnerByAffiliateId(string AffiliateId, ref DataTable dtPartner)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Partner_GetByAffiliateId";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@AffiliateId", SqlDbType.NVarChar, AffiliateId);
                dbLib.RunSP(strsql, ref dtPartner);
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

        public void GetPartnerById(int PartnerId, ref DataTable dtPartner)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Partner_GetById";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@PartnerId", SqlDbType.NVarChar, PartnerId);
                dbLib.RunSP(strsql, ref dtPartner);
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

        public int SetStatus(bool IsActive, int PartnerId, int UserId, int RoleId, DateTime Date)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_Partner_SetStatus";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@IsActive", SqlDbType.Bit, IsActive);
                dbLib.AddParameter("@PartnerId", SqlDbType.Int, PartnerId);
                dbLib.AddParameter("@UpdatedBy", SqlDbType.Int, UserId);
                dbLib.AddParameter("@UpdatedByType", SqlDbType.Int, RoleId);
                dbLib.AddParameter("@UpdatedDate", SqlDbType.DateTime, Date);
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

        public int AddPartner(ref BizObjects.Partner Partner)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_Partner_Insert";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                Partner.AddInsertParams(ref dbLib);
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

        public int UpdatePartner(ref BizObjects.Partner Partner)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_Partner_Update";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                Partner.AddUpdateParams(ref dbLib);
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

        public void CheckPartnerLogin(string Partnername, string Password, ref DataTable dtPartner)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Partner_CheckLogin";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@Partnername", SqlDbType.NVarChar, Partnername);
                dbLib.AddParameter("@Password", SqlDbType.NVarChar, Password);
                dbLib.RunSP(strsql, ref dtPartner);
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

        public void GetPartnerInfoByEmail(string Email, ref DataTable dtPartner)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Partner_GetPartnerByEmail";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@Email", SqlDbType.NVarChar, Email);
                dbLib.RunSP(strsql, ref dtPartner);
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

        public void GetPartnerInfoByPartnerName(string Partnername, string Email,ref DataTable dtPartner)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Partner_GetPartnerByPartnerName";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@Partnername", SqlDbType.NVarChar, Partnername);
                dbLib.AddParameter("@Email", SqlDbType.NVarChar, Email);
                dbLib.RunSP(strsql, ref dtPartner);
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

        public int SetForgotPasswordLink(string Email, string PasswordUrl)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_Partner_ForgotPasswordURL";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@Email", SqlDbType.NVarChar, Email);
                dbLib.AddParameter("@PasswordUrl", SqlDbType.NVarChar, PasswordUrl);
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

        public void CheckPartnerResetPasswordLink(string PasswordUrl, ref DataTable dtResult)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Partner_CheckResetPasswordLink";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@PasswordUrl", SqlDbType.NVarChar, PasswordUrl);
                dbLib.RunSP(strsql, ref dtResult);
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

        public void PartnerResetPassword(string PasswordUrl, string Password)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Partner_ResetPassword";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@PasswordUrl", SqlDbType.NVarChar, PasswordUrl);
                dbLib.AddParameter("@NewPassword", SqlDbType.NVarChar, Password);
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

        public void ChangePassword(string Username, string Password)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Partner_ChangePassword";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@Username", SqlDbType.NVarChar, Username);
                dbLib.AddParameter("@Password", SqlDbType.NVarChar, Password);
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

        public int UpdatePartnerProfile(ref BizObjects.Partner Partner)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_Partner_UpdateProfile";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@FirstName", SqlDbType.NVarChar, Partner.FirstName);
                dbLib.AddParameter("@LastName", SqlDbType.NVarChar, Partner.LastName);
                dbLib.AddParameter("@UserName", SqlDbType.NVarChar, Partner.UserName);
                dbLib.AddParameter("@Email", SqlDbType.NVarChar, Partner.Email);
                dbLib.AddParameter("@CompanyName", SqlDbType.NVarChar, Partner.CompanyName);
                dbLib.AddParameter("@Image", SqlDbType.NVarChar, Partner.Image);
                dbLib.AddParameter("@Address", SqlDbType.NVarChar, Partner.Address);
                dbLib.AddParameter("@CitiesId", SqlDbType.Int, Partner.CitiesId);
                dbLib.AddParameter("@StateId", SqlDbType.Int, Partner.StateId);
                dbLib.AddParameter("@ZipCode", SqlDbType.Int, Partner.ZipCode);
                dbLib.AddParameter("@PhoneNumber", SqlDbType.NVarChar, Partner.PhoneNumber);
                dbLib.AddParameter("@UpdatedBy", SqlDbType.Int, Partner.UpdatedBy);
                dbLib.AddParameter("@UpdatedByType", SqlDbType.Int, Partner.UpdatedByType);
                dbLib.AddParameter("@UpdatedDate", SqlDbType.DateTime, Partner.UpdatedDate);
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

        public void GetClientListByPartnerId(int PartnerId, ref DataTable dtClients)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_PartnerPortal_GetClientList";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@PartnerId", SqlDbType.Int, PartnerId);
                dbLib.RunSP(strsql, ref dtClients);
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

        public void GetDashboardGraphByPartnerId(int PartnerId, ref DataTable dtPartnerSales)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_PartnerPortal_GetClientList";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@PartnerId", SqlDbType.Int, PartnerId);
                dbLib.RunSP(strsql, ref dtPartnerSales);
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

        public void GetAllClientByPartner(int PartnerId, ref DataTable dtClients)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_client_basedOnPartner";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@PartnerId", SqlDbType.Int, PartnerId);
                dbLib.RunSP(strsql, ref dtClients);
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

        public void GetProfiteByPartner(int ClientId, int AffiliateId, int strMonth, int strYear, ref DataTable dtClient)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_PartnerClientProfit_GetByClientId";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ClientId", SqlDbType.Int, ClientId);
                dbLib.AddParameter("@AffiliateId", SqlDbType.Int, AffiliateId);
                dbLib.AddParameter("@Month", SqlDbType.Int, strMonth);
                dbLib.AddParameter("@Year", SqlDbType.Int, strYear);
                dbLib.RunSP(strsql, ref dtClient);
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
