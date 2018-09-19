using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace Services
{
    public interface IClientUnitInvoiceService
    {
        long AddClientUnitInvoice(ref BizObjects.ClientUnitInvoice ClientUnitInvoice);
    }

    public class ClientUnitInvoiceService:IClientUnitInvoiceService
    {
        IDataLib dbLib;
        public long AddClientUnitInvoice(ref BizObjects.ClientUnitInvoice ClientUnitInvoice)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            long rtn;
            strsql = "uspa_ClientUnitInvoice_Insert";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                ClientUnitInvoice.AddInsertParams(ref dbLib);
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
