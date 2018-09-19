using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace Services
{
    public interface IPartsService
    {
        void GetAllParts(bool IsActive, ref DataTable dtParts);
        void GetPartById(int PartId, ref DataTable dtPart);
        void GetAllPartsByPartType(string PartType, ref DataTable dtParts);
        void GetAllPartsByFilter(int PartTypeId, string Partname, string InventoryType,ref DataTable dtParts);
        void GetPartByPartName(string PartName, ref DataTable dtParts);
        void CheckPart(int PartId, string PartName, ref DataTable dtParts);
        int UpdateInStock(int PartId, int Quantity);
        int SetStatus(bool IsActive, int PartId);
        int AddPart(ref BizObjects.Parts Parts);
        int UpdatePart(ref BizObjects.Parts Parts);
        int DeletePart(ref BizObjects.Parts Parts);
        void GetLowStockDetails(ref DataTable dtParts);
    }

    public class PartsService : IPartsService
    {
        IDataLib dbLib;

        public void GetAllParts(bool IsActive, ref DataTable dtParts)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Parts_GetAll";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@IsActive", SqlDbType.Bit, IsActive);
                dbLib.RunSP(strsql, ref dtParts);
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
        public void GetLowStockDetails(ref DataTable dtParts)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_GetLowStockDetails";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.RunSP(strsql, ref dtParts);
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

        public void GetPartById(int PartId, ref DataTable dtPart)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Parts_SelectByID";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@PartId", SqlDbType.Int, PartId);
                dbLib.RunSP(strsql, ref dtPart);
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

        public void GetAllPartsByFilter(int PartTypeId, string Partname, string InventoryType, ref DataTable dtParts)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Parts_GetAllByPartTypeId";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@PartTypeId", SqlDbType.Int, PartTypeId);
                dbLib.AddParameter("@Partname", SqlDbType.NVarChar, Partname);
                dbLib.AddParameter("@InventoryType", SqlDbType.NVarChar, InventoryType);
                dbLib.RunSP(strsql, ref dtParts);
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
        public void GetAllPartsByPartType(string PartType, ref DataTable dtParts)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Parts_GetAllByPartType";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@PartTypeName", SqlDbType.NVarChar, PartType);
                dbLib.RunSP(strsql, ref dtParts);
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

        public void GetPartByPartName(string PartName, ref DataTable dtParts)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Parts_GetByPartName";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@PartName", SqlDbType.NVarChar, PartName);
                dbLib.RunSP(strsql, ref dtParts);
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

        public void CheckPart(int PartId, string PartName, ref DataTable dtParts)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Parts_CheckParts";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@PartId", SqlDbType.Int, PartId);
                //dbLib.AddParameter("@InventoryType", SqlDbType.NVarChar, InventoryType);
                //dbLib.AddParameter("@PartType", SqlDbType.Int, PartType);
                dbLib.AddParameter("@PartName", SqlDbType.NVarChar, PartName);
                //dbLib.AddParameter("@PartSize", SqlDbType.NVarChar, PartSize);
                dbLib.RunSP(strsql, ref dtParts);
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

        public int UpdateInStock(int PartId, int Quantity)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_Parts_UpdateInStock";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@PartId", SqlDbType.Int, PartId);
                dbLib.AddParameter("@Quantity", SqlDbType.Int, Quantity);
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

        public int SetStatus(bool IsActive, int PartId)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_Parts_SetStatus";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@IsActive", SqlDbType.Bit, IsActive);
                dbLib.AddParameter("@PartId", SqlDbType.Int, PartId);
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

        public int AddPart(ref BizObjects.Parts Parts)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_Parts_Insert";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                Parts.AddInsertParams(ref dbLib);
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

        public int UpdatePart(ref BizObjects.Parts Parts)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_Parts_Update";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                Parts.AddUpdateParams(ref dbLib);
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

        public int DeletePart(ref BizObjects.Parts Parts)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_Parts_DeleteByID";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@PartId", SqlDbType.Int, Parts.Id);
                dbLib.AddParameter("@DeletedBy", SqlDbType.Int, Parts.DeletedBy);
                dbLib.AddParameter("@DeletedByType", SqlDbType.Int, Parts.DeletedByType);
                dbLib.AddParameter("@DeletedDate", SqlDbType.DateTime, Parts.DeletedDate);
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
