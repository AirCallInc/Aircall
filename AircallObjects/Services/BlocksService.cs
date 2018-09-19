using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace Services
{
    public interface IBlocksService
    {
        int AddNewBlocks(ref BizObjects.Blocks objNewBlocks);
        void GetAllBlocks(ref DataTable dtBlocks);
        int SetStatus(bool IsActive, int BlockId);
        int DeleteBlock(int BlockId);
        void GetBlocksById(int BlockId, ref DataTable dtBlock);
        int UpdateBlocks(ref BizObjects.Blocks objUpdateBlocks);
        void GetBlockByCMSId(int CMSId, ref DataTable dtResults);
        void GetBlockContentByBlockName(string BlockName,ref DataTable dtBlock);
    }
    public class BlocksService : IBlocksService
    {
        IDataLib dbLib;

        public int AddNewBlocks(ref BizObjects.Blocks objNewBlocks)
        {
            dbLib = DataLibFactory.CreateDAL();
            String strsql;
            int rtn;
            strsql = "uspa_Block_Insert";
            {
                try
                {
                    dbLib.OpenConnection();
                    dbLib.BeginTransaction();
                    dbLib.InitParameters();
                    objNewBlocks.AddInsertParams(ref dbLib);
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
        public void GetAllBlocks(ref DataTable dtBlocks)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Blocks_SelectAll";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.RunSP(strsql, ref dtBlocks);
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
        public int SetStatus(bool IsActive, int BlockId)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_Block_SetStatus";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@IsActive", SqlDbType.Bit, IsActive);
                dbLib.AddParameter("@BlockId", SqlDbType.Int, BlockId);
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
        public int DeleteBlock(int BlockId)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_Block_Delete";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@BlockId", SqlDbType.Int, BlockId);
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
        public void GetBlocksById(int BlockId, ref DataTable dtBlock)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Block_GetById";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@BlockId", SqlDbType.Int, BlockId);
                dbLib.RunSP(strsql, ref dtBlock);
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
        public int UpdateBlocks(ref BizObjects.Blocks objUpdateBlocks)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_Blocks_UpdateById";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                objUpdateBlocks.AddUpdateParams(ref dbLib);
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
        public void GetBlockByCMSId(int CMSId, ref DataTable dtResults)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_tb_CMS_Block_GetByCMSId";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@CMSId", SqlDbType.Int, CMSId);
                dbLib.RunSP(strsql, ref dtResults);
                dbLib.CloseConnection();
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
        public void GetBlockContentByBlockName(string BlockName, ref DataTable dtBlock)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Blocks_GetByBlockName";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@BlockName", SqlDbType.NVarChar, BlockName);
                dbLib.RunSP(strsql, ref dtBlock);
                dbLib.CloseConnection();
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
