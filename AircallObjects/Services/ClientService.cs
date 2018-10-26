using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;
using AuthorizeNetLib;
using DBUtility;
using AuthorizeNet.Api.Contracts.V1;

namespace Services
{
    public interface IClientService
    {
        void GetAllClients(bool IsActive, ref DataTable dtClients);
        void GetClientById(int ClientId, ref DataTable dtClient);
        void insertstriptlog(string insertstriptlog);
        void GetAllClientByName(string ClientName, string SortField, string SortDirection, ref DataTable dtClients);
        void GetClientByName(string ClientName, ref DataTable dtClients);
        void GetClientByEmail(string Email, int ClientId, ref DataTable dtClients);
        int SetStatus(ref BizObjects.Client Client);
        int AddClient(ref BizObjects.Client Client);
        int UpdateClient(ref BizObjects.Client Client);
        void DeleteClient(ref BizObjects.Client Client, ref DataTable dtClientUnits);
        void GetAllClientByPartnerId(int PartnerId, ref DataTable dtClient);
        int AssignAffiliate(ref BizObjects.Client Client);
        int RemoveAffiliate(ref BizObjects.Client Client);

        void Login(string email, string password, ref DataTable dtClient);
        void CheckForForgotPassword(string email, ref DataTable dtClient);
        void CheckResetPasswordLinkExpiration(string passwordurl, ref DataTable dtClient);
        void CheckResetPasswordLinkExpirationEmployee(string passwordurl, ref DataTable dtClient);
        void ResetPassword(string email, string newpassword, ref DataTable dtClient);
        void ResetPasswordEmployee(string email, string newpassword, ref DataTable dtClient);
        void ChangePassword(string OldPassword, string NewPassword, int ClientId, ref DataTable dtClient);
        int UpdateClientProfile(ref BizObjects.Client objClient);
        int UpdateClientContactInfo(ref BizObjects.Client objClient);
        void GetClientScheduleDates(int ClientId, ref DataTable dtClients);
        void GetClientScheduleServicesByDate(int clientId, string date, ref DataTable dtClients);
        void GetClientByStripeId(string StripeCustomerId, ref DataTable dtClient);
        void AllowToDeleteClient(int ClientId,ref DataTable dtClient);
        bool AddClientToAuthorizeNet(BizObjects.Client Client, ref string customerProfileId, ref string errCode, ref string errText);
        bool AddSubscriptionToAuthorizeNet(int clientId, string customerPaymentProfileId, decimal amount, ref string subscriptionId, ref string errCode, ref string errText);
        int AddOrder(int clientId, string customerPaymentProfileId, int addressId, decimal totalAmount, string orderType, string chargeBy);
        bool AddClientToAuthorizeNet(string email, string description, ref string customerProfileId, ref string errCode, ref string errText);
        bool UpdateClientToAuthorizeNet(string email, string description, string customerProfileId, ref string errCode, ref string errText);
        bool CreatePaymentProfile(string firstName, string lastName, string customerProfileId, string cardNumber, string expirationDate, string cardCode, ref string customerPaymentProfileId, ref string errCode, ref string errText);
        bool AddClientAddressToAuthorizeNet(int clientId, string stateName, string cityName, string zip, string addressStr, ref string customerAddressId, ref string errCode, ref string errText);
        int AddBillingAddress(int clientId, string stateName, string cityName, string zip, string address, string firstName, string lastName);
        bool CreatePaymentProfileWithBillingAddress(string firstName, string lastName, string customerProfileId, string cardNumber, string expirationDate, string cardCode, ref string customerPaymentProfileId, ref string errCode, ref string errText, string address, string city, string state, string zip);
    }

    public class ClientService : IClientService
    {
        IDataLib dbLib;

        public void GetAllClients(bool IsActive, ref DataTable dtClients)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Client_SelectAll";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@IsActive", SqlDbType.Bit, IsActive);
                dbLib.RunSP(strsql, ref dtClients);
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

        public void GetClientById(int ClientId, ref DataTable dtClient)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Client_SelectByID";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@Id", SqlDbType.Int, ClientId);
                dbLib.RunSP(strsql, ref dtClient);
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

        public void GetAllClientByName(string ClientName, string SortField, string SortDirection, ref DataTable dtClients)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Client_SelectAllByName";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ClientName", SqlDbType.NVarChar, ClientName);
                dbLib.AddParameter("@SortField", SqlDbType.NVarChar, SortField);
                dbLib.AddParameter("@SortDirection", SqlDbType.NVarChar, SortDirection);
                dbLib.RunSP(strsql, ref dtClients);
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

        public void GetClientByName(string ClientName, ref DataTable dtClients)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Client_GetByClientName";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ClientName", SqlDbType.NVarChar, ClientName);
                dbLib.RunSP(strsql, ref dtClients);
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

        public void GetClientByEmail(string Email, int ClientId, ref DataTable dtClients)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Client_GetByEmail";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ClientId", SqlDbType.Int, ClientId);
                dbLib.AddParameter("@Email", SqlDbType.NVarChar, Email);
                dbLib.RunSP(strsql, ref dtClients);
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
        public void insertstriptlog(string insertstripe)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "insertstriptlog";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@stripestring", SqlDbType.NVarChar, insertstripe);                
                rtn = dbLib.ExeSP(strsql);                
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
        public int SetStatus(ref BizObjects.Client Client)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_Client_SetStatus";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ClientId", SqlDbType.Int, Client.Id);
                dbLib.AddParameter("@IsActive", SqlDbType.Bit, Client.IsActive);
                dbLib.AddParameter("@UpdatedBy", SqlDbType.Int, Client.UpdatedBy);
                dbLib.AddParameter("@UpdatedByType", SqlDbType.Int, Client.UpdatedByType);
                dbLib.AddParameter("@UpdatedDate", SqlDbType.DateTime, Client.UpdatedDate);
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

        public int AddClient(ref BizObjects.Client Client)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_Client_Insert";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                Client.AddInsertParams(ref dbLib);
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

        public bool AddClientToAuthorizeNet(BizObjects.Client Client, ref string customerProfileId, ref string errCode, ref string errText)
        {
            var helper = new AuthorizeNetHelper();

            string email = Client.Email;
            bool isSuccess = false;
            var description = Client.FirstName + " " + Client.LastName;
            var customerId = "";
            helper.CreateCustomerProfile(email, description, customerId, ref isSuccess, ref customerProfileId, ref errCode, ref errText);
            if (isSuccess && !string.IsNullOrEmpty(customerProfileId))
            {
                //UpdateCustomerProfileId(clientId, customerProfileId);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool AddClientToAuthorizeNet(string email, string description, ref string customerProfileId, ref string errCode, ref string errText)
        {
            var helper = new AuthorizeNetHelper();
            bool isSuccess = false;
            var customerId = "";
            helper.CreateCustomerProfile(email, description, customerId, ref isSuccess, ref customerProfileId, ref errCode, ref errText);

            if (isSuccess && !string.IsNullOrEmpty(customerProfileId))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool UpdateClientToAuthorizeNet(string email, string description, string customerProfileId, ref string errCode, ref string errText)
        {
            var helper = new AuthorizeNetHelper();
            bool isSuccess = false;
            var customerId = "";
            helper.UpdateCustomerProfile(customerProfileId, email, description, customerId, ref isSuccess, ref errCode, ref errText);

            if (isSuccess)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CreatePaymentProfile(string firstName, string lastName, string customerProfileId, string cardNumber, string expirationDate, string cardCode, ref string customerPaymentProfileId, ref string errCode, ref string errText)
        {
            var creditCard = new creditCardType
            {
                cardNumber = cardNumber,
                expirationDate = expirationDate,
                cardCode = cardCode,
            };

            paymentType echeck = new paymentType { Item = creditCard };

            var billTo = new customerAddressType
            {
                firstName = firstName,
                lastName = lastName
            };

            var helper = new AuthorizeNetHelper();
            bool isSuccess = false;

            helper.CreateCustomerPaymentProfile(customerProfileId, echeck, billTo, ref isSuccess, ref customerPaymentProfileId, ref errCode, ref errText);

            return isSuccess;
        }

        public bool CreatePaymentProfileWithBillingAddress(string firstName, string lastName, string customerProfileId, string cardNumber, string expirationDate, string cardCode, ref string customerPaymentProfileId, ref string errCode, ref string errText, string address, string city, string state, string zip)
        {
            var creditCard = new creditCardType
            {
                cardNumber = cardNumber,
                expirationDate = expirationDate,
                cardCode = cardCode,
            };

            paymentType echeck = new paymentType { Item = creditCard };

            var billTo = new customerAddressType
            {
                firstName = firstName,
                lastName = lastName,
                address = address,
                city = city,
                state = state,
                zip = zip,
            };

            var helper = new AuthorizeNetHelper();
            bool isSuccess = false;

            helper.CreateCustomerPaymentProfile(customerProfileId, echeck, billTo, ref isSuccess, ref customerPaymentProfileId, ref errCode, ref errText);

            return isSuccess;
        }

        private void UpdateCustomerProfileId(int clientId, string customerProfileId)
        {
            string strsql = string.Format("update dbo.Client set CustomerProfileId = '{0}' where Id = {1}", customerProfileId, clientId);
            SQLDBHelper instance = new SQLDBHelper();
            instance.ExecuteSQL(strsql, null);
            instance.Close();
        }

        public int UpdateClient(ref BizObjects.Client Client)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_Client_Update";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                Client.AddUpdateParams(ref dbLib);
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

        public void DeleteClient(ref BizObjects.Client Client, ref DataTable dtClientUnits)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_Client_DeleteByID";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ClientId", SqlDbType.Int, Client.Id);
                dbLib.AddParameter("@DeletedBy", SqlDbType.Int, Client.DeletedBy);
                dbLib.AddParameter("@DeletedByType", SqlDbType.Int, Client.DeletedByType);
                dbLib.AddParameter("@DeletedDate", SqlDbType.DateTime, Client.DeletedDate);
                dbLib.RunSP(strsql, ref dtClientUnits);
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

        public void GetAllClientByPartnerId(int PartnerId, ref DataTable dtClient)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Client_GetByPartnerId";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@PartnerId", SqlDbType.Int, PartnerId);
                dbLib.RunSP(strsql, ref dtClient);
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

        public int AssignAffiliate(ref BizObjects.Client Client)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_Client_AssignAffiliate";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ClientId", SqlDbType.Int, Client.Id);
                dbLib.AddParameter("@AffiliateId", SqlDbType.Int, Client.AffiliateId);
                dbLib.AddParameter("@UpdatedBy", SqlDbType.Int, Client.UpdatedBy);
                dbLib.AddParameter("@UpdatedByType", SqlDbType.Int, Client.UpdatedByType);
                dbLib.AddParameter("@UpdatedDate", SqlDbType.DateTime, Client.UpdatedDate);
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

        public int RemoveAffiliate(ref BizObjects.Client Client)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_Client_RemoveAffiliate";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ClientId", SqlDbType.Int, Client.Id);
                dbLib.AddParameter("@AffilateDeletedBy", SqlDbType.Int, Client.AffilateDeletedBy);
                dbLib.AddParameter("@AffilateDeletedDate", SqlDbType.DateTime, Client.AffilateDeletedDate);
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

        public void Login(string email, string password, ref DataTable dtClient)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientPortal_Client_Login";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@Email", SqlDbType.NVarChar, email);
                dbLib.AddParameter("@Password", SqlDbType.NVarChar, password);
                dbLib.RunSP(strsql, ref dtClient);
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

        public void CheckForForgotPassword(string email, ref DataTable dtClient)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientPortal_Client_CheckForForgotPassword";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@Email", SqlDbType.NVarChar, email);
                dbLib.RunSP(strsql, ref dtClient);
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

        public void CheckResetPasswordLinkExpiration(string passwordurl, ref DataTable dtClient)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientPortal_Client_CheckResetPasswordLinkExpiration";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@PasswordUrl", SqlDbType.NVarChar, passwordurl);
                dbLib.RunSP(strsql, ref dtClient);
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
        public void CheckResetPasswordLinkExpirationEmployee(string passwordurl, ref DataTable dtClient)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientPortal_Employee_CheckResetPasswordLinkExpiration";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@PasswordUrl", SqlDbType.NVarChar, passwordurl);
                dbLib.RunSP(strsql, ref dtClient);
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
        public void ResetPassword(string email, string newpassword, ref DataTable dtClient)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientPortal_Client_ResetPassword";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@Email", SqlDbType.NVarChar, email);
                dbLib.AddParameter("@NewPassword", SqlDbType.NVarChar, newpassword);
                dbLib.RunSP(strsql, ref dtClient);
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
        public void ResetPasswordEmployee(string email, string newpassword, ref DataTable dtClient)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientPortal_Employee_ResetPassword";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@Email", SqlDbType.NVarChar, email);
                dbLib.AddParameter("@NewPassword", SqlDbType.NVarChar, newpassword);
                dbLib.RunSP(strsql, ref dtClient);
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
        public void ChangePassword(string OldPassword, string NewPassword, int ClientId, ref DataTable dtClient)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientPortal_Client_ChangePassword";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ClientId", SqlDbType.Int, ClientId);
                dbLib.AddParameter("@OldPassword", SqlDbType.NVarChar, OldPassword);
                dbLib.AddParameter("@NewPassword", SqlDbType.NVarChar, NewPassword);
                dbLib.RunSP(strsql, ref dtClient);
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

        public int UpdateClientProfile(ref BizObjects.Client objClient)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_ClientPortal_Client_UpdateProfile";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ClientId", SqlDbType.Int, objClient.Id);
                dbLib.AddParameter("@FName", SqlDbType.NVarChar, objClient.FirstName);
                dbLib.AddParameter("@LName", SqlDbType.NVarChar, objClient.LastName);
                dbLib.AddParameter("@Image", SqlDbType.NVarChar, objClient.Image);
                dbLib.AddParameter("@Company", SqlDbType.NVarChar, objClient.Company);
                dbLib.AddParameter("@UpdatedBy", SqlDbType.Int, objClient.UpdatedBy);
                dbLib.AddParameter("@UpdatedByType", SqlDbType.Int, objClient.UpdatedByType);
                dbLib.AddParameter("@UpdatedDate", SqlDbType.DateTime, objClient.UpdatedDate);
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

        public int UpdateClientContactInfo(ref BizObjects.Client objClient)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_ClientPortal_Client_UpdateContactInfo";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ClientId", SqlDbType.Int, objClient.Id);
                dbLib.AddParameter("@MobileNumber", SqlDbType.NVarChar, objClient.MobileNumber);
                dbLib.AddParameter("@OfficeNumber", SqlDbType.NVarChar, objClient.OfficeNumber);
                dbLib.AddParameter("@HomeNumber", SqlDbType.NVarChar, objClient.HomeNumber);
                dbLib.AddParameter("@UpdatedBy", SqlDbType.Int, objClient.UpdatedBy);
                dbLib.AddParameter("@UpdatedByType", SqlDbType.Int, objClient.UpdatedByType);
                dbLib.AddParameter("@UpdatedDate", SqlDbType.DateTime, objClient.UpdatedDate);
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
        public void GetClientScheduleDates(int ClientId, ref DataTable dtClients)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientPortal_GETAllScheduledDate";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ClientId", SqlDbType.BigInt, ClientId);
                dbLib.RunSP(strsql, ref dtClients);
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
        public void GetClientScheduleServicesByDate(int clientId, string date, ref DataTable dtClients)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientPortal_GETAllScheduledServiceByDATE";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@SDate", SqlDbType.NVarChar, date);
                dbLib.AddParameter("@ClientId", SqlDbType.Int, clientId);
                dbLib.RunSP(strsql, ref dtClients);
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

        public void GetClientByStripeId(string StripeCustomerId, ref DataTable dtClient)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_GetCustomerFromStripeId";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@StripeCustomerId", SqlDbType.NVarChar, StripeCustomerId);
                dbLib.RunSP(strsql, ref dtClient);
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

        public void AllowToDeleteClient(int ClientId,ref DataTable dtClient)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Client_CheckAllowToDelete";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ClientId", SqlDbType.NVarChar, ClientId);
                dbLib.RunSP(strsql, ref dtClient);
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

        public bool AddSubscriptionToAuthorizeNet(int clientId, string customerPaymentProfileId, decimal amount, ref string subscriptionId, ref string errCode, ref string errText)
        {
            paymentScheduleTypeInterval interval = new paymentScheduleTypeInterval();

            interval.length = 1;
            interval.unit = ARBSubscriptionUnitEnum.months;

            paymentScheduleType schedule = new paymentScheduleType
            {
                interval = interval,
                startDate = DateTime.Now.AddMinutes(10),
                totalOccurrences = 9999,
            };

            var objClientService = ServiceFactory.ClientService;
            DataTable dtClient = null;
            objClientService.GetClientById(clientId, ref dtClient);

            var objClientAddressService = ServiceFactory.ClientAddressService;
            DataTable dtAddress = new DataTable();
            objClientAddressService.GetClientAddressesByClientId(clientId, true, ref dtAddress);

            var helper = new AuthorizeNetHelper();

            string customerProfileId = dtClient.Rows[0]["CustomerProfileId"].ToString();
            string customerAddressId = dtAddress.Rows[0]["CustomerAddressId"].ToString();

            bool isSuccess = false;

            helper.CreateSubscriptionFromCustomerProfile(customerProfileId, customerPaymentProfileId, customerAddressId, schedule, amount, ref isSuccess, ref subscriptionId, ref errCode, ref errText);

            return isSuccess;
        }

        public int AddOrder(int clientId, string customerPaymentProfileId, int addressId, decimal totalAmount, string orderType, string chargeBy)
        {
            BizObjects.Orders objOrders = new BizObjects.Orders();
            var objOrderService = ServiceFactory.OrderService;
            int orderId = 0;
            objOrders.OrderType = orderType;
            objOrders.ClientId = clientId;
            objOrders.OrderAmount = totalAmount;
            objOrders.ChargeBy = chargeBy;
            objOrders.AddedDate = DateTime.UtcNow;
            orderId = objOrderService.AddClientUnitOrder(ref objOrders, customerPaymentProfileId, addressId);

            return orderId;
        }

        public bool AddClientAddressToAuthorizeNet(int clientId, string stateName, string cityName, string zip, string addressStr, ref string customerAddressId, ref string errCode, ref string errText)
        {
            var objClientService = ServiceFactory.ClientService;
            DataTable dtClient = null;
            objClientService.GetClientById(clientId, ref dtClient);
            DataRow dr = dtClient.Rows[0];
            var firstName = dr["FirstName"].ToString();
            var lastName = dr["LastName"].ToString();
            var customerProfileId = dr["CustomerProfileId"].ToString();

            customerAddressType address = new customerAddressType();
            address.firstName = firstName;
            address.lastName = lastName;
            address.address = addressStr;
            address.city = cityName;
            address.zip = zip;
            address.state = stateName;

            var helper = new AuthorizeNetHelper();
            bool isSuccess = false;
            helper.CreateCustomerAddress(customerProfileId, address, ref isSuccess, ref customerAddressId, ref errCode, ref errText);

            if (isSuccess)
            {
                return true;
            }
            else
            {
                var errMsg = "";

                if (!string.IsNullOrEmpty(errText))
                {
                    errMsg = errText;
                }
                else
                {
                    errMsg = "Add address to AuthorizeNet failed.";
                }

                return false;
            }
        }

        public int AddBillingAddress(int clientId, string stateName, string cityName, string zip, string address, string firstName, string lastName)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_BillingAddress_Insert";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ClientId", SqlDbType.Int, clientId);
                dbLib.AddParameter("@State", SqlDbType.NVarChar, stateName);
                dbLib.AddParameter("@City", SqlDbType.NVarChar, cityName);
                dbLib.AddParameter("@ZipCode", SqlDbType.NVarChar, zip);
                dbLib.AddParameter("@Address", SqlDbType.NVarChar, address);
                dbLib.AddParameter("@FirstName", SqlDbType.NVarChar, firstName);
                dbLib.AddParameter("@LastName", SqlDbType.NVarChar, lastName);
                var rtn = dbLib.ExeSP(strsql);
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
