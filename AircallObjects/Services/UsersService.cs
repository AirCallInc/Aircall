using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace Services
{
    public interface IUsersService
    {
        void GetAllWareHouseUser(ref DataTable dtUsers);
        void GetAllAdminUsers(ref DataTable dtUsers);
        void GetAllAdminUsersByName(string AdminName, ref DataTable dtUsers);
        void GetUserInfoByUsername(string Username, ref DataTable dtUser);
        void GetUserInfoByEmail(string Email, int UserId, ref DataTable dtUser);
        void GetUserById(int UserId, ref DataTable dtUser);
        void CheckAdminLogin(string Username, string Password, ref DataTable dtUser);
        int SetForgotPasswordLink(string Email, string PasswordUrl);
        void CheckResetPasswordLink(string PasswordUrl, ref DataTable dtResult);
        void ResetPassword(string PasswordUrl, string Password);
        void ChangePassword(string Username, string Password);
        int AddUser(ref BizObjects.Users AdminUser);
        int UpdateUser(ref BizObjects.Users AdminUser);
        int SetStatus(bool IsActive, int UserId);
        int DeleteUser(int UserId);
        int UpdateUserProfile(string Username, string Firstname, string Lastname, string Email, string Image);
        void GetAllUserForEmail(bool ClientAdded,bool EmpAdded, bool AdminAdded, bool WarehouseAdded, bool PartnerAdded, ref DataTable dtUser);
    }

    public class UsersService : IUsersService
    {
        IDataLib dbLib;

        public void GetAllWareHouseUser(ref DataTable dtUsers)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_GetAllWareHouseUser";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.RunSP(strsql, ref dtUsers);
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
        public void GetAllAdminUsers(ref DataTable dtUsers)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Users_GetAll";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.RunSP(strsql, ref dtUsers);
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
        public void GetAllAdminUsersByName(string AdminName, ref DataTable dtUsers)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Users_GetAllByName";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@AdminName", SqlDbType.NVarChar, AdminName);
                dbLib.RunSP(strsql, ref dtUsers);
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

        public void GetUserInfoByUsername(string Username, ref DataTable dtUser)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Users_GetUserByUsername";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@Username", SqlDbType.NVarChar, Username);
                dbLib.RunSP(strsql, ref dtUser);
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

        public void GetUserInfoByEmail(string Email, int UserId, ref DataTable dtUser)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Users_GetUserByEmail";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@UserId", SqlDbType.Int, UserId);
                dbLib.AddParameter("@Email", SqlDbType.NVarChar, Email);
                dbLib.RunSP(strsql, ref dtUser);
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

        public void GetUserById(int UserId, ref DataTable dtUser)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Users_GetById";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@UserId", SqlDbType.Int, UserId);
                dbLib.RunSP(strsql, ref dtUser);
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

        public void CheckAdminLogin(string Username, string Password, ref DataTable dtUser)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Users_CheckLogin";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@Username", SqlDbType.NVarChar, Username);
                dbLib.AddParameter("@Password", SqlDbType.NVarChar, Password);
                dbLib.RunSP(strsql, ref dtUser);
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
            strsql = "uspa_Users_ForgotPasswordURL";
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

        public void CheckResetPasswordLink(string PasswordUrl, ref DataTable dtResult)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Users_CheckResetPasswordLink";
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

        public void ResetPassword(string PasswordUrl, string Password)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Users_ResetPassword";
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
            strsql = "uspa_Users_ChangePassword";
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

        public int AddUser(ref BizObjects.Users AdminUser)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_Users_Add";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                AdminUser.AddInsertParams(ref dbLib);
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

        public int UpdateUser(ref BizObjects.Users AdminUser)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_Users_Update";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                AdminUser.AddUpdateParams(ref dbLib);
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

        public int SetStatus(bool IsActive, int UserId)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_Users_SetStatus";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@IsActive", SqlDbType.Bit, IsActive);
                dbLib.AddParameter("@UserId", SqlDbType.Int, UserId);
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

        public int DeleteUser(int UserId)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_Users_Delete";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@UserId", SqlDbType.Int, UserId);
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

        public int UpdateUserProfile(string Username, string Firstname, string Lastname, string Email, string Image)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_Users_UpdateProfile";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@UserName", SqlDbType.NVarChar, Username);
                dbLib.AddParameter("@FirstName", SqlDbType.NVarChar, Firstname);
                dbLib.AddParameter("@LastName", SqlDbType.NVarChar, Lastname);
                dbLib.AddParameter("@Email", SqlDbType.NVarChar, Email);
                dbLib.AddParameter("@Image", SqlDbType.NVarChar, Image);
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

        public void GetAllUserForEmail(bool ClientAdded, bool EmpAdded, bool AdminAdded, bool WarehouseAdded, bool PartnerAdded, ref DataTable dtUser)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_GetAllUserForMail";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ClientAdded", SqlDbType.Bit, ClientAdded);
                dbLib.AddParameter("@EmpAdded", SqlDbType.Bit, EmpAdded);
                dbLib.AddParameter("@AdminAdded", SqlDbType.Bit, AdminAdded);
                dbLib.AddParameter("@WarehouseAdded", SqlDbType.Bit, WarehouseAdded);
                dbLib.AddParameter("@PartnerAdded", SqlDbType.Bit, PartnerAdded);
                dbLib.RunSP(strsql, ref dtUser);
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
