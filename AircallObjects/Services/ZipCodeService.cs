using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace Services
{
    public interface IZipCodeService
    {
        void GetAllZipCode(bool IsActive, ref DataTable dtZipCodes);
        void GetById(int ZipCodeId, ref DataTable dtZipCode);
        void GetByZipCode(int StateId, int CityId, string ZipCode, ref DataTable dtZipCodes);
        void GetAllZipCodeByCityId(int CityId, bool IncludeInactive,ref DataTable dtZipCodes);
        void GetAllZipCodeByStateIdCityId(int StateId, int CityId,string ZipCode, string SortField, string SortDirection, ref DataTable dtZipCodes);
        void CheckValidZipCode(int StateId,int CityId,string ZipCode,ref DataTable dtZipCode);
        int SetStatus(bool IsActive, int ZipId);
        int AddZipCode(ref BizObjects.ZipCodes ZipCode);
        int UpdateZipCode(ref BizObjects.ZipCodes ZipCode);
        int DeleteZipCode(int ZipId);
    }

    public class ZipCodeService : IZipCodeService
    {
        IDataLib dbLib;

        public void GetAllZipCode(bool IsActive, ref DataTable dtZipCodes)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ZipCode_SelectAll";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@IsActive", SqlDbType.Bit, IsActive);
                dbLib.RunSP(strsql, ref dtZipCodes);
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

        public void GetById(int ZipCodeId, ref DataTable dtZipCode)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ZipCode_SelectByID";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ZipCodeId", SqlDbType.Int, ZipCodeId);
                dbLib.RunSP(strsql, ref dtZipCode);
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

        public void GetByZipCode(int StateId, int CityId, string ZipCode, ref DataTable dtZipCodes)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ZipCode_GetByZipCode";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@StateId", SqlDbType.Int, StateId);
                dbLib.AddParameter("@CityId", SqlDbType.Int, CityId);
                dbLib.AddParameter("@ZipCode", SqlDbType.NVarChar, ZipCode);
                dbLib.RunSP(strsql, ref dtZipCodes);
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

        public void GetAllZipCodeByCityId(int CityId, bool IncludeInactive,ref DataTable dtZipCodes)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ZipCode_GetByCityId";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@CityId", SqlDbType.Int, CityId);
                dbLib.AddParameter("@IncludeInactive", SqlDbType.Bit, IncludeInactive);
                dbLib.RunSP(strsql, ref dtZipCodes);
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

        public void GetAllZipCodeByStateIdCityId(int StateId, int CityId, string ZipCode, string SortField, string SortDirection, ref DataTable dtZipCodes)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ZipCode_SelectAllByStateCity";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@StateId", SqlDbType.Int, StateId);
                dbLib.AddParameter("@CityId", SqlDbType.Int, CityId);
                dbLib.AddParameter("@ZipCode", SqlDbType.NVarChar, ZipCode);
                dbLib.AddParameter("@SortField", SqlDbType.NVarChar, SortField);
                dbLib.AddParameter("@SortDirection", SqlDbType.NVarChar, SortDirection);
                dbLib.RunSP(strsql, ref dtZipCodes);
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

        public void CheckValidZipCode(int StateId,int CityId,string ZipCode,ref DataTable dtZipCode)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ZipCode_CheckValidZipCode";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@StateId", SqlDbType.Int, StateId);
                dbLib.AddParameter("@CityId", SqlDbType.Int, CityId);
                dbLib.AddParameter("@ZipCode", SqlDbType.NVarChar, ZipCode);
                dbLib.RunSP(strsql, ref dtZipCode);
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

        public int SetStatus(bool IsActive, int ZipId)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_ZipCode_SetStatus";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@IsActive", SqlDbType.Bit, IsActive);
                dbLib.AddParameter("@ZipId", SqlDbType.Int, ZipId);
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

        public int AddZipCode(ref BizObjects.ZipCodes ZipCode)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_ZipCode_Insert";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                ZipCode.AddInsertParams(ref dbLib);
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

        public int UpdateZipCode(ref BizObjects.ZipCodes ZipCode)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_ZipCode_Update";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                ZipCode.AddUpdateParams(ref dbLib);
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

        public int DeleteZipCode(int ZipId)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_ZipCode_DeleteByID";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ZipId", SqlDbType.Int, ZipId);
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
