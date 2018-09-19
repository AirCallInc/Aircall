using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace Services
{
    public interface IUnitManualsService
    {
        void GetUnitManualsByUnitId(int UnitId, ref DataTable dtManuals);
        int AddUnitManuals(ref BizObjects.UnitManuals UnitManuals);
        void DeleteUnitManual(int UnitManualId, ref DataTable dtManual);
    }

    public class UnitManualsService:IUnitManualsService
    {
        IDataLib dbLib;

        public void GetUnitManualsByUnitId(int UnitId, ref DataTable dtManuals)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_UnitManuals_GetByUnitId";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@UnitId", SqlDbType.Int, UnitId);
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

        public int AddUnitManuals(ref BizObjects.UnitManuals UnitManuals)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_UnitManuals_Insert";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                UnitManuals.AddInsertParams(ref dbLib);
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

        public void DeleteUnitManual(int UnitManualId, ref DataTable dtManual)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_UnitManuals_DeleteById";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@UnitManualId", SqlDbType.Int, UnitManualId);
                dbLib.RunSP(strsql, ref dtManual);
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
