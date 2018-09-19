using api.ErrorHelper;
using api.Helpers;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.Http.Tracing;

namespace api.ActionFilters
{
    public class GlobalExceptionAttribute : ExceptionFilterAttribute
    {
        public class ErrorLog
        {
            public ServiceStatus errorStatus { get; set; }

        }
        public override void OnException(HttpActionExecutedContext context)
        {
            GlobalConfiguration.Configuration.Services.Replace(typeof(ITraceWriter), new NLogger());
            var trace = GlobalConfiguration.Configuration.Services.GetTraceWriter();
            trace.Error(context.Request, "Controller : " + context.ActionContext.ControllerContext.ControllerDescriptor.ControllerType.FullName + Environment.NewLine + "Action : " + context.ActionContext.ActionDescriptor.ActionName, context.Exception);

            var exceptionType = context.Exception.GetType();

            if (exceptionType == typeof(ValidationException))
            {
                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(context.Exception.Message), ReasonPhrase = "ValidationException", };
                throw new HttpResponseException(resp);

            }
            else if (exceptionType == typeof(UnauthorizedAccessException))
            {
                ErrorLog _errorLog = new ErrorLog();
                ServiceStatus _serviceStatus = new ServiceStatus();
                _serviceStatus.StatusCode = (int)HttpStatusCode.Unauthorized;
                _serviceStatus.StatusMessage = "UnAuthorized";
                _serviceStatus.ReasonPhrase = "UnAuthorized Access";
                _errorLog.errorStatus = _serviceStatus;
                throw new HttpResponseException(context.Request.CreateResponse(HttpStatusCode.Unauthorized, _errorLog));
            }
            else if (exceptionType == typeof(ApiException))
            {
                ErrorLog _errorLog = new ErrorLog();
                ServiceStatus _serviceStatus = new ServiceStatus();
                var webapiException = context.Exception as ApiException;
                if (webapiException != null)
                {
                    _serviceStatus.StatusCode = webapiException.ErrorCode;
                    _serviceStatus.StatusMessage = webapiException.ErrorDescription;
                    _serviceStatus.ReasonPhrase = webapiException.ReasonPhrase;
                    _errorLog.errorStatus = _serviceStatus;
                    throw new HttpResponseException(context.Request.CreateResponse(webapiException.HttpStatus, _errorLog));
                }
                else
                {
                    _errorLog.errorStatus = _serviceStatus;
                    throw new HttpResponseException(context.Request.CreateResponse(webapiException.HttpStatus, _errorLog));
                }
                    
            }
            else if (exceptionType == typeof(ApiBusinessException))
            {
                ErrorLog _errorLog = new ErrorLog();
                ServiceStatus _serviceStatus = new ServiceStatus();
                var businessException = context.Exception as ApiBusinessException;
                if (businessException != null)
                {
                    _serviceStatus.StatusCode = businessException.ErrorCode;
                    _serviceStatus.StatusMessage = businessException.ErrorDescription;
                    _serviceStatus.ReasonPhrase = businessException.ReasonPhrase;
                    _errorLog.errorStatus = _serviceStatus;
                    throw new HttpResponseException(context.Request.CreateResponse(businessException.HttpStatus, _errorLog));
                }
                else
                {
                    _errorLog.errorStatus = _serviceStatus;
                    throw new HttpResponseException(context.Request.CreateResponse(businessException.HttpStatus, _errorLog));
                }
                   
            }
            else if (exceptionType == typeof(ApiDataException))
            {
                var dataException = context.Exception as ApiDataException;
                ErrorLog _errorLog = new ErrorLog();
                ServiceStatus _serviceStatus = new ServiceStatus();

                if (dataException != null)
                {
                    _serviceStatus.StatusCode = dataException.ErrorCode;
                    _serviceStatus.StatusMessage = dataException.ErrorDescription;
                    _serviceStatus.ReasonPhrase = dataException.ReasonPhrase;
                    _errorLog.errorStatus = _serviceStatus;
                    throw new HttpResponseException(context.Request.CreateResponse(dataException.HttpStatus, _errorLog));
                }
                else
                {

                    _errorLog.errorStatus = _serviceStatus;
                    throw new HttpResponseException(context.Request.CreateResponse(dataException.HttpStatus, _errorLog));
                }
            }
            else
            {
                ErrorLog _errorLog = new ErrorLog();
                ServiceStatus _serviceStatus = new ServiceStatus();
                _serviceStatus.StatusCode = Convert.ToInt32(HttpStatusCode.InternalServerError);
                _serviceStatus.StatusMessage = context.Exception.Message;
                _serviceStatus.ReasonPhrase = "InternalServerError";
                _errorLog.errorStatus = _serviceStatus;
                throw new HttpResponseException(context.Request.CreateResponse(HttpStatusCode.InternalServerError,_errorLog));
            }
        }
    }
}