using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace Services
{
    public interface IClientUnitManualsService
    {
        void GetManualsByUnitId(int ClientUnitId, string SplitType,  ref DataTable dtManuals);
        int AddClientUnitManuals(ref BizObjects.ClientUnitManuals ClientUnitManuals);
        void DeleteClientUnitManuals(int ClientUnitManualId, ref DataTable dtManuals);
        int DeleteClientUnitManualByUnitId(int ClientUnitId);
        void AddClientUnitManualsFromUnitManuals(int UnitId, int ClientUnitId, string UnitType,int AddedBy, int AddedByType, DateTime AddedDate,ref DataTable dtManulas);
    }

    public class ClientUnitManualsService:IClientUnitManualsService
    {
        IDataLib dbLib;

        public void GetManualsByUnitId(int ClientUnitId, string SplitType, ref DataTable dtManuals)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientUnitManuals_GetByUnitId";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ClientUnitId", SqlDbType.Int, ClientUnitId);
                dbLib.AddParameter("@SplitType", SqlDbType.NVarChar, SplitType);
                dbLib.RunSP(strsql, ref dtManuals);
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

        public int AddClientUnitManuals(ref BizObjects.ClientUnitManuals ClientUnitManuals)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_ClientUnitManuals_Insert";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                ClientUnitManuals.AddInsertParams(ref dbLib);
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

        public void DeleteClientUnitManuals(int ClientUnitManualId, ref DataTable dtManuals)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientUnitManuals_DeleteById";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ManualId",SqlDbType.Int,ClientUnitManualId);
                dbLib.RunSP(strsql, ref dtManuals);
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

        public int DeleteClientUnitManualByUnitId(int ClientUnitId)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_ClientUnitManuals_DeleteByUnitId";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ClientUnitId", SqlDbType.Int, ClientUnitId);
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

        public void AddClientUnitManualsFromUnitManuals(int UnitId, int ClientUnitId,string UnitType, int AddedBy, int AddedByType, DateTime AddedDate,ref DataTable dtManulas)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientUnitManuals_InsertFromUnitManuals";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@UnitId", SqlDbType.Int, UnitId);
                dbLib.AddParameter("@ClientUnitId", SqlDbType.Int, ClientUnitId);
                dbLib.AddParameter("@UnitType", SqlDbType.NVarChar, UnitType);
                dbLib.AddParameter("@AddedBy", SqlDbType.Int, AddedBy);
                dbLib.AddParameter("@AddedByType", SqlDbType.Int, AddedByType);
                dbLib.AddParameter("@AddedDate", SqlDbType.DateTime, AddedDate);
                dbLib.RunSP(strsql, ref dtManulas);
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
