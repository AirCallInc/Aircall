using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace Services
{
    public interface IServiceReportImagesService
    {
        void GetServiceReportImagesByReportId(long ReportId, ref DataTable dtImage);
        int AddServiceReportImages(ref BizObjects.ServiceReportImages ServiceReportImages);
    }

    public  class ServiceReportImagesService:IServiceReportImagesService
    {
        IDataLib dbLib;

        public void GetServiceReportImagesByReportId(long ReportId, ref DataTable dtImage)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ServiceReportImages_GetByServiceReportId";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ReportId", SqlDbType.BigInt, ReportId);
                dbLib.RunSP(strsql, ref dtImage);
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

        public int AddServiceReportImages(ref BizObjects.ServiceReportImages ServiceReportImages)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_ServiceReportImages_Insert";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                ServiceReportImages.AddInsertParams(ref dbLib);
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
