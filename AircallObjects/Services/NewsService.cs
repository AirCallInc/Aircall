using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace Services
{
    public interface INewsService
    {
        void GetAllNews(bool IsActive, ref DataTable dtNews);
        void GetNewsById(int Id, ref DataTable dtNews);
        void GetNewsByUrl(string url, ref DataTable dtNews);
        int SetStatus(bool IsActive, int Id);
        int AddNews(ref BizObjects.News News);
        int UpdateNews(ref BizObjects.News News);
        int DeleteNews(ref BizObjects.News News);
        void GetPageContent(long NEWSId, ref System.Data.DataTable dtResults);
    }
    public class NewsService : INewsService
    {
        IDataLib dbLib;
        public void GetAllNews(bool IsActive, ref DataTable dtNews)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_News_GetAll";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@IsActive", SqlDbType.Bit, IsActive);
                dbLib.RunSP(strsql, ref dtNews);
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
        public void GetNewsById(int Id, ref DataTable dtNews)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_News_GetByID";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@Id", SqlDbType.BigInt, Id);
                dbLib.RunSP(strsql, ref dtNews);
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
        public void GetNewsByUrl(string url, ref DataTable dtNews)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_News_GetByUrl";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@URL", SqlDbType.NVarChar, url);
                dbLib.RunSP(strsql, ref dtNews);
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
        public int SetStatus(bool IsActive, int Id)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_News_SetStatus";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@IsActive", SqlDbType.Bit, IsActive);
                dbLib.AddParameter("@Id", SqlDbType.BigInt, Id);
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
        public int AddNews(ref BizObjects.News News)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_News_Insert";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                News.AddInsertParams(ref dbLib);
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
        public int UpdateNews(ref BizObjects.News News)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_News_Update";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                News.AddUpdateParams(ref dbLib);
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
        public int DeleteNews(ref BizObjects.News News)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_News_DeleteByID";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@Id", SqlDbType.Int, News.Id);
                dbLib.AddParameter("@DeletedBy", SqlDbType.Int, News.DeletedBy);
                dbLib.AddParameter("@DeletedByType", SqlDbType.Int, News.DeletedByType);
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
        public void GetPageContent(long NEWSId, ref System.Data.DataTable dtResults)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();

            strsql = "uspa_tb_NEWS_GetNEWSPageContextByURL";
            try
            {
                dbLib.OpenConnection();
                dbLib.InitParameters();
             //   dbLib.AddParameter("@URL", SqlDbType.NVarChar, url);
                dbLib.AddParameter("@NEWSId", SqlDbType.Int, NEWSId);
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
