using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace Services
{
    public interface ISiteSettingService
    {
        void GetSiteSettingByName(string Name, ref DataTable dtSiteSetting);
        void GetSiteSettingById(int Id, ref DataTable dtSiteSetting);
        int UpdateSiteSettingById(int Id, string Value);
        void GetAllSiteSetting(ref DataTable dtSiteSetting);
    }

    public class SiteSettingService : ISiteSettingService
    {
        IDataLib dbLib;

        public void GetSiteSettingByName(string Name, ref DataTable dtSiteSetting)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_SiteSetting_GetByName";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@Name", SqlDbType.NVarChar, Name);
                dbLib.RunSP(strsql, ref dtSiteSetting);
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
        public void GetSiteSettingById(int Id, ref DataTable dtSiteSetting)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_SiteSetting_GetById";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@Id", SqlDbType.Int, Id);
                dbLib.RunSP(strsql, ref dtSiteSetting);
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
        public int UpdateSiteSettingById(int Id, string Value)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_SiteSetting_UpdateById";
            int ret = 0;
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@Id", SqlDbType.Int, Id);
                dbLib.AddParameter("@Value", SqlDbType.NVarChar, Value);
                ret = dbLib.ExeSP(strsql);
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
            return ret;
        }
        public void GetAllSiteSetting(ref DataTable dtSiteSetting)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_SiteSetting_GetAll";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.RunSP(strsql, ref dtSiteSetting);
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
