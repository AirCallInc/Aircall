using api.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace api.ActionFilters
{
    public class RequireHttpsAttribute : AuthorizationFilterAttribute
    {
        
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (actionContext.Request.RequestUri.Scheme != Uri.UriSchemeHttps)
            {
                ErrorLog _errorLog = new ErrorLog();
                ServiceStatus _serviceStatus = new ServiceStatus();
                _serviceStatus.StatusCode = (int)HttpStatusCode.Forbidden;
                _serviceStatus.StatusMessage = "HTTPS Required.Please provide Https service URL.";
                _serviceStatus.ReasonPhrase = "Forbidden";
                var response = actionContext.Request.CreateResponse
                                        (new ErrorLog() { AuthorizeStatus = _serviceStatus });
                response.StatusCode = HttpStatusCode.Forbidden;

                actionContext.Response = response;
            }
            else
            {
                base.OnAuthorization(actionContext);
            }
        }
        public class ErrorLog
        {
            public ServiceStatus AuthorizeStatus { get; set; }

        }
    }
}