using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace Services
{
    public interface IUnitExtraInfoService
    {
        void GetByClientUnitId(int ClientUnitId, long ClientUnitPartId, string ExtraInfoType, ref DataTable dtExtraInfo);
        int AddUnitExtraInfo(ref BizObjects.UnitExtraInfo UnitExtraInfo);
    }

    public class UnitExtraInfoService:IUnitExtraInfoService
    {
        IDataLib dbLib;

        public void GetByClientUnitId(int ClientUnitId, long ClientUnitPartId, string ExtraInfoType, ref DataTable dtExtraInfo)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_UnitExtraInfo_GetByUnitId";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ClientUnitId", SqlDbType.Int, ClientUnitId);
                dbLib.AddParameter("@ClientUnitPartId", SqlDbType.BigInt, ClientUnitPartId);
                dbLib.AddParameter("@ExtraInfoType", SqlDbType.NVarChar, ExtraInfoType);
                dbLib.RunSP(strsql, ref dtExtraInfo);
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

        public int AddUnitExtraInfo(ref BizObjects.UnitExtraInfo UnitExtraInfo)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_UnitExtraInfo_Insert";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                UnitExtraInfo.AddInsertParams(ref dbLib);
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
