using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace Services
{
    public interface IEmailTemplateService
    {
        void GetAllEmailTemplate(string Name,string Subject,ref DataTable dtEmailTemplates);
        void GetByName(string Name, ref DataTable dtEmailTemplate);
        void GetById(int Id, ref DataTable dtEmailTemplate);
        int UpdateEmailTemplate(ref BizObjects.EmailTemplate EmailTemplate);
    }

    public class EmailTemplateService : IEmailTemplateService
    {
        IDataLib dbLib;

        public void GetAllEmailTemplate(string Name,string Subject,ref DataTable dtEmailTemplates)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_EmailTemplate_GetAll";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@Name", SqlDbType.NVarChar, Name);
                dbLib.AddParameter("@Subject", SqlDbType.NVarChar, Subject);
                dbLib.RunSP(strsql, ref dtEmailTemplates);
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

        public void GetByName(string Name, ref DataTable dtEmailTemplate)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_EmailTemplate_GetByName";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@Name", SqlDbType.NVarChar, Name);
                dbLib.RunSP(strsql, ref dtEmailTemplate);
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

        public void GetById(int Id, ref DataTable dtEmailTemplate)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_EmailTemplate_GetById";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@Id", SqlDbType.Int, Id);
                dbLib.RunSP(strsql, ref dtEmailTemplate);
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

        public int UpdateEmailTemplate(ref BizObjects.EmailTemplate EmailTemplate)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_EmailTemplate_Update";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                EmailTemplate.AddUpdateParams(ref dbLib);
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
