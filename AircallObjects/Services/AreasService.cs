using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace Services
{
    public interface IAreasService
    {
        void GetAllAreas(bool IsActive, ref DataTable dtAreas);
        void GetAllAreasByClientAddressId(int AddressId,ref DataTable dtAreas);
        void GetAreaById(int AreaId, bool IsActive ,ref DataTable dtArea);
        void GetAreaByName(string AreaName, ref DataTable dtAreas);
        void SearchByAreaName(string AreaName, ref DataTable dtAreas);
        void SearchByAreaNameStateCityZip(string AreaName, int StateId, int CityId,string ZipCode, string SortField, string SortDirection, ref DataTable dtAreas);
        int SetStatus(bool IsActive, int AreaId, int UserId, int RoleId);
        int AddArea(ref BizObjects.Areas Area);
        int UpdateArea(ref BizObjects.Areas Area);
        int DeleteArea(int AreaId,int UserId,int RoleId);
    }

    public class AreasService:IAreasService
    {
        IDataLib dbLib;

        public void GetAllAreas(bool IsActive, ref DataTable dtAreas)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_AreaMaster_SelectAll";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@IsActive", SqlDbType.Bit, IsActive);
                dbLib.RunSP(strsql, ref dtAreas);
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

        public void GetAllAreasByClientAddressId(int AddressId,ref DataTable dtAreas)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_AreaMaster_GetAllAreasByClientAddressId";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@AddressId", SqlDbType.Int, AddressId);
                dbLib.RunSP(strsql, ref dtAreas);
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

        public void GetAreaById(int AreaId, bool IsActive, ref DataTable dtArea)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_AreaMaster_SelectByID";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@AreaId", SqlDbType.Int, AreaId);
                dbLib.AddParameter("@IsActive", SqlDbType.Bit, IsActive);
                dbLib.RunSP(strsql, ref dtArea);
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

        public void GetAreaByName(string AreaName, ref DataTable dtAreas)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_AreaMaster_GetByAreaName";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@AreaName", SqlDbType.NVarChar, AreaName);
                dbLib.RunSP(strsql, ref dtAreas);
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

        public void SearchByAreaName(string AreaName, ref DataTable dtAreas)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_AreaMaster_SearchByAreaName";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@AreaName", SqlDbType.NVarChar, AreaName);
                dbLib.RunSP(strsql, ref dtAreas);
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

        public int SetStatus(bool IsActive, int AreaId, int UserId, int RoleId)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_AreaMaster_SetStatus";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@IsActive", SqlDbType.Bit, IsActive);
                dbLib.AddParameter("@AreaId", SqlDbType.Int, AreaId);
                dbLib.AddParameter("@UpdatedBy", SqlDbType.Int, UserId);
                dbLib.AddParameter("@UpdatedByType", SqlDbType.Int, RoleId);
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

        public int AddArea(ref BizObjects.Areas Area)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_AreaMaster_Insert";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                Area.AddInsertParams(ref dbLib);
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

        public int UpdateArea(ref BizObjects.Areas Area)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_AreaMaster_Update";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                Area.AddUpdateParams(ref dbLib);
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

        public int DeleteArea(int AreaId, int UserId, int RoleId)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_AreaMaster_DeleteByID";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@AreaId", SqlDbType.Int, AreaId);
                dbLib.AddParameter("@DeletedBy", SqlDbType.Int, UserId);
                dbLib.AddParameter("@DeletedByType", SqlDbType.Int, RoleId); 
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

        public void SearchByAreaNameStateCityZip(string AreaName, int StateId, int CityId, string ZipCode, string SortField, string SortDirection, ref DataTable dtAreas)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_AreaMaster_SelectAllByStateCityZipName";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@AreaName", SqlDbType.NVarChar, AreaName);
                dbLib.AddParameter("@StateId", SqlDbType.Int, StateId);
                dbLib.AddParameter("@CityId", SqlDbType.Int, CityId);
                dbLib.AddParameter("@ZipCode", SqlDbType.NVarChar, ZipCode);
                dbLib.AddParameter("@SortField", SqlDbType.NVarChar, SortField);
                dbLib.AddParameter("@SortDirection", SqlDbType.NVarChar, SortDirection);

                dbLib.RunSP(strsql, ref dtAreas);
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
