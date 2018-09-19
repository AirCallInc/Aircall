using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace Services
{
    public interface IStripeErrorLogService
    {
        int AddStripeErrorLog(ref BizObjects.StripeErrorLog StripeErrorLog);
    }

    public class StripeErrorLogService : IStripeErrorLogService
    {
        IDataLib dbLib;

        public int AddStripeErrorLog(ref BizObjects.StripeErrorLog StripeErrorLog)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_StripeErrorLog_Insert";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                StripeErrorLog.AddInsertParams(ref dbLib);
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
