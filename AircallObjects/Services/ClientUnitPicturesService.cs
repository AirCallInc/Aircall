using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace Services
{
    public interface IClientUnitPicturesService
    {
        void GetUnitPicturesByUnitId(int ClientUnitId, string SplitType, ref DataTable dtPictures);
        int AddClientUnitPictures(ref BizObjects.ClientUnitPictures ClientUnitPictures);
        void DeleteClientUnitPictures(int ClientUnitPictureId, ref DataTable dtPictures);
    }

    public class ClientUnitPicturesService:IClientUnitPicturesService
    {
        IDataLib dbLib;

        public void GetUnitPicturesByUnitId(int ClientUnitId, string SplitType, ref DataTable dtPictures)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientUnitPictures_GetByUnitId";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ClientUnitId", SqlDbType.Int, ClientUnitId);
                dbLib.AddParameter("@SplitType", SqlDbType.NVarChar, SplitType);
                dbLib.RunSP(strsql, ref dtPictures);
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

        public int AddClientUnitPictures(ref BizObjects.ClientUnitPictures ClientUnitPictures)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_ClientUnitPictures_Insert";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                ClientUnitPictures.AddInsertParams(ref dbLib);
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

        public void DeleteClientUnitPictures(int ClientUnitPictureId, ref DataTable dtPictures)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientUnitPictures_DeleteById";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@PictureId", SqlDbType.Int, ClientUnitPictureId);
                dbLib.RunSP(strsql, ref dtPictures);
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
