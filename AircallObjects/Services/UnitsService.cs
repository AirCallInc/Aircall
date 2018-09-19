using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace Services
{
    public interface IUnitsService
    {
        void GetAllUnits(bool IsActive, int AddedBy,string ModelNo, string SerialNo, string Brand,ref DataTable dtUnits);
        void GetByUnitId(int UnitId, ref DataTable dtUnit);
        void FindUnitsByModelNumber(string ModelNumber, ref DataTable dtUnit);
        void GetByModelNumber(string ModelNumber, ref DataTable dtUnit);
        int SetStatus(bool IsActive, int UnitId);
        int AddUnit(ref BizObjects.Units Units);
        int UpdateUnit(ref BizObjects.Units Units);
        int DeleteUnit(ref BizObjects.Units Units);
    }

    public class UnitsService : IUnitsService
    {
        IDataLib dbLib;

        public void GetAllUnits(bool IsActive, int AddedBy, string ModelNo, string SerialNo, string Brand, ref DataTable dtUnits)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Units_SelectAll";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@IsActive", SqlDbType.Bit, IsActive);
                dbLib.AddParameter("@AddedByType", SqlDbType.Int, AddedBy);
                dbLib.AddParameter("@ModelNo", SqlDbType.NVarChar, ModelNo);
                dbLib.AddParameter("@SerialNo", SqlDbType.NVarChar, SerialNo);
                dbLib.AddParameter("@MfgBrand", SqlDbType.NVarChar, Brand);
                dbLib.RunSP(strsql, ref dtUnits);
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

        public void GetByUnitId(int UnitId, ref DataTable dtUnit)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Units_SelectByID";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@UnitId", SqlDbType.Int, UnitId);
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

        public void FindUnitsByModelNumber(string ModelNumber,ref DataTable dtUnit)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Units_GetUnitsByModelNumber";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ModelNumber", SqlDbType.NVarChar, ModelNumber);
                //dbLib.AddParameter("@SerialNumber", SqlDbType.NVarChar, SerialNumber);
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

        public void GetByModelNumber(string ModelNumber, ref DataTable dtUnit)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Units_GetByModelNumber";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ModelNumber", SqlDbType.NVarChar, ModelNumber);
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

        public int SetStatus(bool IsActive, int UnitId)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_Units_SetStatus";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@IsActive", SqlDbType.Bit, IsActive);
                dbLib.AddParameter("@UnitId", SqlDbType.Int, UnitId);
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

        public int AddUnit(ref BizObjects.Units Units)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_Units_Insert";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                Units.AddInsertParams(ref dbLib);
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

        public int UpdateUnit(ref BizObjects.Units Units)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_Units_Update";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                Units.AddUpdateParams(ref dbLib);
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

        public int DeleteUnit(ref BizObjects.Units Units)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_Units_Delete";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@UnitId", SqlDbType.Int, Units.Id);
                dbLib.AddParameter("@DeletedBy", SqlDbType.Int, Units.DeletedBy);
                dbLib.AddParameter("@DeletedByType", SqlDbType.Int, Units.DeletedByType);
                dbLib.AddParameter("@DeletedDate", SqlDbType.DateTime, Units.DeletedDate);
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
