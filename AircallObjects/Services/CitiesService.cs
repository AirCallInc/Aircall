using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace Services
{
    public interface ICitiesService
    {
        void GetAllCities(bool IsActive, ref DataTable dtCities);
        void GetCityByCityName(int StateId, int CityId,string CityName, ref DataTable dtCity);
        void GetByCityId(int CityId, ref DataTable dtCity);
        void GetAllCityByStateId(int StateId, bool IncludeInactive,ref DataTable dtCities);
        void GetAllCityFilter(int StateId, string City, bool IsActive, string SortField, string SortDirection, ref DataTable dtCities);
        int SetStatus(bool IsActive, int CityId);
        int AddCity(ref BizObjects.Cities City);
        int UpdateCity(ref BizObjects.Cities City);
        int DeleteCity(ref BizObjects.Cities City);
    }

    public class CitiesService:ICitiesService
    {
        IDataLib dbLib;

        public void GetAllCities(bool IsActive, ref DataTable dtCities)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Cities_SelectAll";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@IsActive", SqlDbType.Bit, IsActive);
                dbLib.RunSP(strsql, ref dtCities);
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

        public void GetCityByCityName(int StateId, int CityId, string CityName, ref DataTable dtCity)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Cities_GetByName";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@StateId", SqlDbType.Int, StateId);
                dbLib.AddParameter("@CityId", SqlDbType.Int, CityId);
                dbLib.AddParameter("@CityName", SqlDbType.NVarChar, CityName);
                dbLib.RunSP(strsql, ref dtCity);
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

        public void GetByCityId(int CityId, ref DataTable dtCity)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Cities_SelectByID";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@CityId", SqlDbType.Int, CityId);
                dbLib.RunSP(strsql, ref dtCity);
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

        public void GetAllCityByStateId(int StateId, bool IncludeInactive,ref DataTable dtCities)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Cities_GetByStateId";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@StateId", SqlDbType.Int, StateId);
                dbLib.AddParameter("@IncludeInActive", SqlDbType.Bit, IncludeInactive);
                dbLib.RunSP(strsql, ref dtCities);
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

        public void GetAllCityFilter(int StateId, string City, bool IsActive, string SortField, string SortDirection, ref DataTable dtCities)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Cities_GetByStateIdStatus";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@StateId", SqlDbType.Int, StateId);
                dbLib.AddParameter("@City", SqlDbType.NVarChar, City);
                dbLib.AddParameter("@IsActive", SqlDbType.Bit, IsActive);
                dbLib.AddParameter("@SortField", SqlDbType.NVarChar, SortField);
                dbLib.AddParameter("@SortDirection", SqlDbType.NVarChar, SortDirection);
                dbLib.RunSP(strsql, ref dtCities);
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

        public int SetStatus(bool IsActive, int CityId)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_Cities_SetStatus";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@IsActive", SqlDbType.Bit, IsActive);
                dbLib.AddParameter("@CityId", SqlDbType.Int, CityId);
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

        public int AddCity(ref BizObjects.Cities City)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_Cities_Insert";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                City.AddInsertParams(ref dbLib);
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

        public int UpdateCity(ref BizObjects.Cities City)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_Cities_Update";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                City.AddUpdateParams(ref dbLib);
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

        public int DeleteCity(ref BizObjects.Cities City)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_Cities_DeleteByID";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@CityId", SqlDbType.Int, City.Id);
                dbLib.AddParameter("@DeletedBy", SqlDbType.Int, City.DeletedBy);
                dbLib.AddParameter("@DeletedByType", SqlDbType.Int, City.DeletedByType);
                dbLib.AddParameter("@DeletedDate", SqlDbType.DateTime, City.DeletedDate);
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
