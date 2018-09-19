using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace Services
{
    public interface IClientUnitPartService
    {
        void GetUnitPartByUnitId(int ClientUnitId, ref DataTable dtUnitParts);
        int AddClientUnitPart(ref BizObjects.ClientUnitParts ClientUnitParts);
        int AddClientUnitPartPortal(ref BizObjects.ClientUnitParts ClientUnitParts);
        int DeleteClientUnitPartByUnitId(int ClientUnitId);
    }
    //
    public class ClientUnitPartService:IClientUnitPartService
    {
        IDataLib dbLib;

        public void GetUnitPartByUnitId(int ClientUnitId, ref DataTable dtUnitParts)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientUnitParts_GetByUnitId";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ClientUnitId", SqlDbType.Int, ClientUnitId);
                dbLib.RunSP(strsql, ref dtUnitParts);
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

        public int AddClientUnitPart(ref BizObjects.ClientUnitParts ClientUnitParts)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_ClientUnitParts_Insert";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                ClientUnitParts.AddInsertParams(ref dbLib);
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
        public int AddClientUnitPartPortal(ref BizObjects.ClientUnitParts ClientUnitParts)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_ClientPortal_ClientUnitParts_Insert";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                ClientUnitParts.AddInsertParams(ref dbLib);
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
        public int DeleteClientUnitPartByUnitId(int ClientUnitId)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_ClientUnitParts_DeleteByUnitId";
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
    }
}
