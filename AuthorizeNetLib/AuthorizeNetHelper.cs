using AuthorizeNet.Api.Contracts.V1;
using AuthorizeNet.Api.Controllers;
using AuthorizeNet.Api.Controllers.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Security.Authentication;
using System.Net;

namespace AuthorizeNetLib
{
    public class AuthorizeNetHelper
    {
        private string _ApiLoginID = "";
        private string _ApiTransactionKey = "";
        private bool _SandBox = true;

        public AuthorizeNetHelper(string ApiLoginID, string ApiTransactionKey, bool SandBox)
        {
            _ApiLoginID = ApiLoginID;
            _ApiTransactionKey = ApiTransactionKey;
            _SandBox = SandBox;

            if (_SandBox)
            {
                ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.SANDBOX;
            }
            else
            {
                ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.PRODUCTION;
            }

            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
            {
                name = _ApiLoginID,
                ItemElementName = ItemChoiceType.transactionKey,
                Item = _ApiTransactionKey,
            };
        }

        public AuthorizeNetHelper()
        {
            const SslProtocols _Tls12 = (SslProtocols)0x00000C00;
            const SecurityProtocolType Tls12 = (SecurityProtocolType)_Tls12;
            ServicePointManager.SecurityProtocol = Tls12;

            ServicePointManager.ServerCertificateValidationCallback +=
            (sender, cert, chain, error) =>
            {
                return true;
            };

            _ApiLoginID = ConfigurationManager.AppSettings["ApiLoginID"];
            _ApiTransactionKey = ConfigurationManager.AppSettings["ApiTransactionKey"];
            _SandBox = ConfigurationManager.AppSettings["SandBox"].ToLower() == "true";

            if (_SandBox)
            {
                ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.SANDBOX;
            }
            else
            {
                ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.PRODUCTION;
            }

            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
            {
                name = _ApiLoginID,
                ItemElementName = ItemChoiceType.transactionKey,
                Item = _ApiTransactionKey,
            };
        }

        public void CreateCustomerProfile(string email, string description, string customerId, ref bool isSuccess, ref string customerProfileId, ref string errCode, ref string errText)
        {
            customerProfileType customerProfile = new customerProfileType();
            //customerProfile.merchantCustomerId = customerId;
            customerProfile.email = email;
            customerProfile.description = description;

            var request = new createCustomerProfileRequest { profile = customerProfile, validationMode = validationModeEnum.none };
            var controller = new createCustomerProfileController(request);
            controller.Execute();
            createCustomerProfileResponse response = controller.GetApiResponse();

            if (response != null)
            {
                if (response.messages.resultCode == messageTypeEnum.Ok)
                {
                    isSuccess = true;

                    if (response.messages.message != null)
                    {
                        customerProfileId = response.customerProfileId;
                    }
                }
                else
                {
                    errCode = response.messages.message[0].code;
                    errText = response.messages.message[0].text;
                }
            }
            else
            {
                var errorResponse = controller.GetErrorResponse();

                if (errorResponse.messages.message.Length > 0)
                {
                    errCode = errorResponse.messages.message[0].code;
                    errText = errorResponse.messages.message[0].text;
                }
                else
                {

                }
            }
        }

        public void UpdateCustomerProfile(string customerProfileId, string email, string description, string customerId, ref bool isSuccess, ref string errCode, ref string errText)
        {
            isSuccess = false;

            var profile = new customerProfileExType
            {
                merchantCustomerId = customerId,
                description = description,
                email = email,
                customerProfileId = customerProfileId
            };

            var request = new updateCustomerProfileRequest();
            request.profile = profile;

            var controller = new updateCustomerProfileController(request);
            controller.Execute();

            var response = controller.GetApiResponse();

            if (response != null && response.messages.resultCode == messageTypeEnum.Ok)
            {
                isSuccess = true;
            }
            else if (response != null)
            {
                isSuccess = false;
                errCode = response.messages.message[0].code;
                errText = response.messages.message[0].text;
            }
        }

        public void CreateCustomerAddress(string customerProfileId, customerAddressType address, ref bool isSuccess, ref string customerAddressId, ref string errCode, ref string errText)
        {
            var request = new createCustomerShippingAddressRequest
            {
                customerProfileId = customerProfileId,
                address = address,
            };

            var controller = new createCustomerShippingAddressController(request);
            controller.Execute();

            createCustomerShippingAddressResponse response = controller.GetApiResponse();
            if (response != null && response.messages.resultCode == messageTypeEnum.Ok)
            {
                isSuccess = true;

                if (response != null && response.messages.message != null)
                {
                    customerAddressId = response.customerAddressId;
                }
            }
            else if (response != null)
            {
                errCode = response.messages.message[0].code;
                errText = response.messages.message[0].text;
            }
        }

        public void UpdateCustomerAddress(string customerProfileId, customerAddressExType address, ref bool isSuccess, ref string errCode, ref string errText)
        {
            var request = new updateCustomerShippingAddressRequest();
            request.customerProfileId = customerProfileId;
            request.address = address;

            var controller = new updateCustomerShippingAddressController(request);
            controller.Execute();

            var response = controller.GetApiResponse();

            if (response != null && response.messages.resultCode == messageTypeEnum.Ok)
            {
                isSuccess = true;
            }
            else if (response != null)
            {
                errCode = response.messages.message[0].code;
                errText = response.messages.message[0].text;
            }
        }

        public void CreateSubscriptionFromCustomerProfile(string customerProfileId, string customerPaymentProfileId, string customerAddressId, paymentScheduleType schedule, decimal amount, ref bool isSuccess, ref string subscriptionId, ref string errCode, ref string errText)
        {
            System.Threading.Thread.Sleep(15000);//In order to solve E000040 error

            customerProfileIdType customerProfile = new customerProfileIdType()
            {
                customerProfileId = customerProfileId,
                customerPaymentProfileId = customerPaymentProfileId,
                customerAddressId = customerAddressId
            };

            ARBSubscriptionType subscriptionType = new ARBSubscriptionType()
            {
                amount = amount,
                paymentSchedule = schedule,
                profile = customerProfile
            };

            var request = new ARBCreateSubscriptionRequest { subscription = subscriptionType };

            var controller = new ARBCreateSubscriptionController(request);
            controller.Execute();

            ARBCreateSubscriptionResponse response = controller.GetApiResponse();

            if (response != null && response.messages.resultCode == messageTypeEnum.Ok)
            {
                if (response != null && response.messages.message != null)
                {
                    isSuccess = true;
                    subscriptionId = response.subscriptionId.ToString();
                }
            }
            else if (response != null)
            {
                errCode = response.messages.message[0].code;
                errText = response.messages.message[0].text;
            }
        }

        public void CreateSubscriptionFromCustomerProfile_2(string customerProfileId, string customerPaymentProfileId, paymentScheduleType schedule, decimal amount, ref bool isSuccess, ref string subscriptionId, ref string errCode, ref string errText)
        {
            System.Threading.Thread.Sleep(15000);//In order to solve E000040 error

            customerProfileIdType customerProfile = new customerProfileIdType()
            {
                customerProfileId = customerProfileId,
                customerPaymentProfileId = customerPaymentProfileId,
            };

            ARBSubscriptionType subscriptionType = new ARBSubscriptionType()
            {
                amount = amount,
                paymentSchedule = schedule,
                profile = customerProfile
            };

            var request = new ARBCreateSubscriptionRequest { subscription = subscriptionType };

            var controller = new ARBCreateSubscriptionController(request);
            controller.Execute();

            ARBCreateSubscriptionResponse response = controller.GetApiResponse();

            if (response != null && response.messages.resultCode == messageTypeEnum.Ok)
            {
                if (response != null && response.messages.message != null)
                {
                    isSuccess = true;
                    subscriptionId = response.subscriptionId.ToString();
                }
            }
            else if (response != null)
            {
                errCode = response.messages.message[0].code;
                errText = response.messages.message[0].text;
            }
        }

        public void UpdateSubscription(string subscriptionId, string customerProfileId, string customerPaymentProfileId, string customerAddressId, decimal amount, ref bool isSuccess, ref string refId, ref string errCode, ref string errText)
        {
            customerProfileIdType customerProfile = new customerProfileIdType()
            {
                customerProfileId = customerProfileId,
                customerPaymentProfileId = customerPaymentProfileId,
                customerAddressId = customerAddressId
            };

            ARBSubscriptionType subscriptionType = new ARBSubscriptionType()
            {
                amount = amount,
                //paymentSchedule = schedule,
                profile = customerProfile
            };

            var request = new ARBUpdateSubscriptionRequest { subscription = subscriptionType, subscriptionId = subscriptionId };

            var controller = new ARBUpdateSubscriptionController(request);
            controller.Execute();

            var response = controller.GetApiResponse();

            if (response != null && response.messages.resultCode == messageTypeEnum.Ok)
            {
                if (response != null && response.messages.message != null)
                {
                    isSuccess = true;
                    refId = response.refId;
                }
            }
            else if (response != null)
            {
                errCode = response.messages.message[0].code;
                errText = response.messages.message[0].text;
            }
        }

        public void CreateCustomerPaymentProfile(string customerProfileId, paymentType echeck, customerAddressType billTo, ref bool isSuccess, ref string customerPaymentProfileId, ref string errCode, ref string errText)
        {
            customerPaymentProfileType echeckPaymentProfile = new customerPaymentProfileType();
            echeckPaymentProfile.payment = echeck;
            echeckPaymentProfile.billTo = billTo;

            var request = new createCustomerPaymentProfileRequest
            {
                customerProfileId = customerProfileId,
                paymentProfile = echeckPaymentProfile,
                validationMode = validationModeEnum.none
            };

            var controller = new createCustomerPaymentProfileController(request);
            controller.Execute();

            createCustomerPaymentProfileResponse response = controller.GetApiResponse();

            if (response != null)
            {
                if (response.messages.resultCode == messageTypeEnum.Ok)
                {
                    isSuccess = true;

                    if (response.messages.message != null)
                    {
                        customerPaymentProfileId = response.customerPaymentProfileId;
                    }
                    else
                    {
                        customerPaymentProfileId = response.customerPaymentProfileId;
                    }
                }
                else
                {
                    errCode = response.messages.message[0].code;
                    errText = response.messages.message[0].text;
                }
            }
            else
            {
                var errorResponse = controller.GetErrorResponse();

                if (errorResponse.messages.message.Length > 0)
                {
                    errCode = errorResponse.messages.message[0].code;
                    errText = errorResponse.messages.message[0].text;
                }
                else
                {

                }
            }
        }

        public void ChargeCustomerProfile(string customerProfileId, string customerPaymentProfileId, decimal amount, ref bool isSuccess, ref string transId, ref string authCode, ref string errCode, ref string errText)
        {
            customerProfilePaymentType profileToCharge = new customerProfilePaymentType();
            profileToCharge.customerProfileId = customerProfileId;
            profileToCharge.paymentProfile = new paymentProfile { paymentProfileId = customerPaymentProfileId };

            var transactionRequest = new transactionRequestType
            {
                transactionType = transactionTypeEnum.authCaptureTransaction.ToString(),
                amount = amount,
                profile = profileToCharge
            };

            var request = new createTransactionRequest { transactionRequest = transactionRequest };

            var controller = new createTransactionController(request);
            controller.Execute();

            var response = controller.GetApiResponse();

            if (response != null)
            {
                if (response.messages.resultCode == messageTypeEnum.Ok)
                {
                    isSuccess = true;

                    if (response.transactionResponse.messages != null)
                    {
                        transId = response.transactionResponse.transId;
                        authCode = response.transactionResponse.authCode;
                    }
                    else
                    {
                        if (response.transactionResponse.errors != null)
                        {
                            errCode = response.transactionResponse.errors[0].errorCode;
                            errText = response.transactionResponse.errors[0].errorText;
                        }
                    }
                }
                else
                {
                    if (response.transactionResponse != null && response.transactionResponse.errors != null)
                    {
                        errCode = response.transactionResponse.errors[0].errorCode;
                        errText = response.transactionResponse.errors[0].errorText;
                    }
                    else
                    {
                        errCode = response.messages.message[0].code;
                        errText = response.messages.message[0].text;
                    }
                }
            }
            else
            {

            }
        }

        public ARBGetSubscriptionResponse GetSubscriptionById(string subscriptionId, ref bool isSuccess, ref string errCode, ref string errText)
        {
            isSuccess = false;
            var request = new ARBGetSubscriptionRequest { subscriptionId = subscriptionId, includeTransactions = true, includeTransactionsSpecified = true };
            var controller = new ARBGetSubscriptionController(request);
            controller.Execute();

            ARBGetSubscriptionResponse response = controller.GetApiResponse();

            // validate response
            if (response != null && response.messages.resultCode == messageTypeEnum.Ok)
            {
                if (response.subscription != null)
                {
                    isSuccess = true;
                }
            }
            else if (response != null)
            {
                if (response.messages.message.Length > 0)
                {
                    errCode = response.messages.message[0].code;
                    errText = response.messages.message[0].text;
                }
            }
            else
            {
                if (controller.GetErrorResponse().messages.message.Length > 0)
                {
                    errCode = response.messages.message[0].code;
                    errText = response.messages.message[0].text;
                }
            }

            return response;
        }

        public List<transactionDetailsType> GetSubscriptionTransactions(string subscriptionId, ref bool isSuccess, ref string errCode, ref string errText)
        {
            List<transactionDetailsType> list = new List<transactionDetailsType>();
            isSuccess = false;
            var request = new ARBGetSubscriptionRequest { subscriptionId = subscriptionId, includeTransactions = true, includeTransactionsSpecified = true };
            var controller = new ARBGetSubscriptionController(request);
            controller.Execute();

            ARBGetSubscriptionResponse response = controller.GetApiResponse();

            // validate response
            if (response != null && response.messages.resultCode == messageTypeEnum.Ok)
            {
                if (response.subscription != null)
                {
                    isSuccess = true;

                    if (response.subscription.arbTransactions != null && response.subscription.arbTransactions.Count() > 0)
                    {
                        foreach (var tran in response.subscription.arbTransactions)
                        {
                            var tempSuccess = false;
                            var tempErrCode = "";
                            var tempErrText = "";
                            var transactionDetail = this.GetTransactionDetails(tran.transId, ref tempSuccess, ref tempErrCode, ref tempErrText);
                            if (tempSuccess && transactionDetail.transaction != null)
                            {
                                list.Add(transactionDetail.transaction);
                            }
                        }
                    }
                }
            }
            else if (response != null)
            {
                if (response.messages.message.Length > 0)
                {
                    errCode = response.messages.message[0].code;
                    errText = response.messages.message[0].text;
                }
            }
            else
            {
                if (controller.GetErrorResponse().messages.message.Length > 0)
                {
                    errCode = response.messages.message[0].code;
                    errText = response.messages.message[0].text;
                }
            }

            return list;
        }

        public getTransactionListResponse GetCustomerProfileTransactionList(string customerProfileId, ref bool isSuccess, ref string errCode, ref string errText)
        {
            isSuccess = false;
            var request = new getTransactionListForCustomerRequest();
            request.customerProfileId = customerProfileId;

            var controller = new getTransactionListForCustomerController(request);
            controller.Execute();
            var response = controller.GetApiResponse();

            if (response != null && response.messages.resultCode == messageTypeEnum.Ok)
            {
                isSuccess = true;
                if (response.transactions == null)
                    return response;
            }
            else if (response != null)
            {
                errCode = response.messages.message[0].code;
                errText = response.messages.message[0].text;
            }

            return response;
        }

        public getTransactionDetailsResponse GetTransactionDetails(string transactionId, ref bool isSuccess, ref string errCode, ref string errText)
        {
            isSuccess = false;
            var request = new getTransactionDetailsRequest();
            request.transId = transactionId;
            var controller = new getTransactionDetailsController(request);
            controller.Execute();

            var response = controller.GetApiResponse();

            if (response != null && response.messages.resultCode == messageTypeEnum.Ok)
            {
                isSuccess = true;
                if (response.transaction == null)
                    return response;
            }
            else if (response != null)
            {
                errCode = response.messages.message[0].code;
                errText = response.messages.message[0].text;
            }

            return response;
        }

        public void CancelSubscription(string subscriptionId, ref bool isSuccess, ref string errCode, ref string errText)
        {
            isSuccess = false;
            var request = new ARBCancelSubscriptionRequest { subscriptionId = subscriptionId };
            var controller = new ARBCancelSubscriptionController(request);
            controller.Execute();

            ARBCancelSubscriptionResponse response = controller.GetApiResponse();

            if (response != null && response.messages.resultCode == messageTypeEnum.Ok)
            {
                if (response != null && response.messages.message != null)
                {
                    isSuccess = true;
                }
            }
            else if (response != null)
            {
                isSuccess = false;
                errCode = response.messages.message[0].code;
                errText = response.messages.message[0].text;
            }
        }
    }
}
