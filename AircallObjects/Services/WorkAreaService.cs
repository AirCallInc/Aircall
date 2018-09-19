using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace Services
{
    public interface IWorkAreaService
    {
        void GetAllWorkAreaByAreaId(int AreaId, ref DataTable dtWorkAreas);
        int AddWorkAreaa(ref BizObjects.WorkAreas WorkArea);
    }

    public class WorkAreaService:IWorkAreaService
    {
        IDataLib dbLib;

        public void GetAllWorkAreaByAreaId(int AreaId, ref DataTable dtWorkAreas)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_WorkArea_GetByAreaId";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@AreaId", SqlDbType.Int, AreaId);
                dbLib.RunSP(strsql, ref dtWorkAreas);
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

        public int AddWorkAreaa(ref BizObjects.WorkAreas WorkArea)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_WorkArea_Insert";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                WorkArea.AddInsertParams(ref dbLib);
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
