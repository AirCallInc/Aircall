using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace Services
{
    public interface ICMSPagesService
    {
        int AddNewCMSPage(ref BizObjects.CMSPages objNewCMSPage);
        int SetStatus(bool IsActive, int CMSPageId);
        int DeleteCMSPages(int CMSPageId);
        void GetCMSPageById(int CMSPageId, ref DataTable dtCMSPage);
        int UpdateCMSPage(ref BizObjects.CMSPages objUpdateCMSPage);
        void GetParentList(ref DataTable dtParentList);
        void GetAllBlockList(ref DataTable dtBlockList);
        void GetBlockCMSId(int CMSPageId, ref DataTable dtBlockById);
        int AddBlockListById(long CMSPageId, long BlockId);
        void GetAllCMSPages(string PageTitle, string MenuTitle, string SortExpression, string SortDirection, ref DataTable dtCMSPages);
        int UpdateBlockListById(long CMSPageId, long BlockId);
        int DeletePageBlock(long CMSPageId);
        void GetPageContent(string url, ref DataTable dtResults);
       
    }
    public class CMSPagesService : ICMSPagesService
    {
        IDataLib dbLib;

        public int AddNewCMSPage(ref BizObjects.CMSPages objNewCMSPage)
        {
            dbLib = DataLibFactory.CreateDAL();
            String strsql;
            int rtn;
            strsql = "uspa_CMPPage_Insert";
            {
                try
                {
                    dbLib.OpenConnection();
                    dbLib.BeginTransaction();
                    dbLib.InitParameters();
                    objNewCMSPage.AddInsertParams(ref dbLib);
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
        public void GetAllCMSPages(string PageTitle, string MenuTitle, string SortExpression, string SortDirection, ref DataTable dtCMSPages)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_CMSPages_SelectAll";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@PageTitle", SqlDbType.NVarChar, PageTitle);
                dbLib.AddParameter("@MenuTitle", SqlDbType.NVarChar, MenuTitle);
                dbLib.AddParameter("@SortExpression", SqlDbType.NVarChar, SortExpression);
                dbLib.AddParameter("@SortDirection", SqlDbType.NVarChar, SortDirection);
                dbLib.RunSP(strsql, ref dtCMSPages);
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
        public int SetStatus(bool IsActive, int CMSPageId)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_CMSPage_SetStatus";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@IsActive", SqlDbType.Bit, IsActive);
                dbLib.AddParameter("@CMSPageId", SqlDbType.Int, CMSPageId);
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
        public int DeleteCMSPages(int CMSPageId)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_CMSPage_Delete";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@CMSPageId", SqlDbType.Int, CMSPageId);
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
        public void GetCMSPageById(int CMSPageId, ref DataTable dtCMSPage)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_CMSPage_GetById";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@CMSPageId", SqlDbType.Int, CMSPageId);
                dbLib.RunSP(strsql, ref dtCMSPage);
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
        public int UpdateCMSPage(ref BizObjects.CMSPages objUpdateCMSPage)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_CMSPage_UpdateById";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                objUpdateCMSPage.AddUpdateParams(ref dbLib);
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
        public void GetParentList(ref DataTable dtParentList)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_CMSPage_ParentList";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.RunSP(strsql, ref dtParentList);
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
        public void GetAllBlockList(ref DataTable dtBlockList)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_CMSPage_BlockList";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.RunSP(strsql, ref dtBlockList);
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
        public void GetBlockCMSId(int CMSPageId, ref DataTable dtBlockById)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();

            strsql = "uspa_CMSPage_BlockListByCMSID";
            try
            {
                dbLib.OpenConnection();
                dbLib.InitParameters();
                dbLib.AddParameter("@CMSPageID", SqlDbType.Int, CMSPageId);
                dbLib.RunSP(strsql, ref dtBlockById);
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

        public int AddBlockListById(long CMSPageId, long BlockId)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_AddBlocks_CMSPage";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@CMSPageId", SqlDbType.BigInt, CMSPageId);
                dbLib.AddParameter("@BlockId", SqlDbType.BigInt, BlockId);
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

        public int UpdateBlockListById(long CMSPageId, long BlockId)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_PageBlocks_Insert";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@CMSPageId", SqlDbType.BigInt, CMSPageId);
                dbLib.AddParameter("@BlockId", SqlDbType.BigInt, BlockId);
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

        public int DeletePageBlock(long CMSPageId)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_PageBlocks_Delete";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@CMSPageId", SqlDbType.BigInt, CMSPageId);
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

        public void GetPageContent(string url, ref System.Data.DataTable dtResults)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();

            strsql = "uspa_tb_CMS_GetCMSPageContextByURL";
            try
            {
                dbLib.OpenConnection();
                dbLib.InitParameters();
                dbLib.AddParameter("@URL", SqlDbType.NVarChar, url);
                dbLib.RunSP(strsql, ref dtResults);
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
