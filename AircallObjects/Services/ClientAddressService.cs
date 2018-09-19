using AuthorizeNet.Api.Contracts.V1;
using AuthorizeNetLib;
using DBUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace Services
{
    public interface IClientAddressService
    {
        void GetAddressById(int AddressId, ref DataTable dtClientAddress);
        void GetAddressByIdClientId(int AddressId, int ClientId, ref DataTable dtClientAddress);
        void GetClientAddressesByClientId(int ClientId, bool IncludeInActive,ref DataTable dtClientAddresses);
        void GetClientDefaultAddressByClientId(int ClientId, ref DataTable dtClientAddresses);
        int AddClientAddress(ref BizObjects.ClientAddress ClientAddress);
        int UpdateClientAddress(ref BizObjects.ClientAddress ClientAddress);
        int DeleteClientAddress(ref BizObjects.ClientAddress ClientAddress);
        int ValidateZipcode(int State, int City, string Zipcode);
    }

    public class ClientAddressService : IClientAddressService
    {
        IDataLib dbLib;

        public void GetAddressById(int AddressId, ref DataTable dtClientAddress)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientAddress_GetById";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@AddressId", SqlDbType.Int, AddressId);
                dbLib.RunSP(strsql, ref dtClientAddress);
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

        public void GetAddressByIdClientId(int AddressId, int ClientId, ref DataTable dtClientAddress)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientAddress_GetById";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@AddressId", SqlDbType.Int, AddressId);
                dbLib.AddParameter("@ClientId", SqlDbType.Int, ClientId);
                dbLib.RunSP(strsql, ref dtClientAddress);
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

        public void GetClientAddressesByClientId(int ClientId, bool IncludeInActive,ref DataTable dtClientAddresses)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientAddress_GetByClientId";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ClientId", SqlDbType.Int, ClientId);
                dbLib.AddParameter("@IncludeInactive", SqlDbType.Bit, IncludeInActive);
                dbLib.RunSP(strsql, ref dtClientAddresses);
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

        public void GetClientDefaultAddressByClientId(int ClientId, ref DataTable dtClientAddresses)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientAddress_GetDefaultAddressByClientId";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ClientId", SqlDbType.Int, ClientId);
                dbLib.RunSP(strsql, ref dtClientAddresses);
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

        public int AddClientAddress(ref BizObjects.ClientAddress ClientAddress)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_ClientAddress_Insert";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                ClientAddress.AddInsertParams(ref dbLib);
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

        public int UpdateClientAddress(ref BizObjects.ClientAddress ClientAddress)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_ClientPortal_ClientAddress_Update";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                ClientAddress.AddUpdateParams(ref dbLib);
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

        public int DeleteClientAddress(ref BizObjects.ClientAddress ClientAddress)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_ClientPortal_ClientAddress_Delete";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@Id", SqlDbType.Int, ClientAddress.Id);
                dbLib.AddParameter("@DeletedBy", SqlDbType.Int, ClientAddress.DeletedBy);
                dbLib.AddParameter("@DeletedByType", SqlDbType.Int, ClientAddress.DeletedByType);
                dbLib.AddParameter("@DeletedDate", SqlDbType.DateTime, ClientAddress.DeletedDate);
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

        public int ValidateZipcode(int State, int City, string Zipcode)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_ClientPortal_ValidateZipcode";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@State", SqlDbType.Int, State);
                dbLib.AddParameter("@City", SqlDbType.Int, City);
                dbLib.AddParameter("@Zipcode", SqlDbType.NVarChar, Zipcode);                
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
