using api.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace api.Controllers
{
    public class ErrorController : ApiController
    {
        //[HttpGet, HttpPost, HttpPut, HttpDelete, HttpHead, HttpOptions, AcceptVerbs("PATCH")]
        public IHttpActionResult Handle404()
        {
            ActionFilters.GlobalExceptionAttribute.ErrorLog _errorLog = new ActionFilters.GlobalExceptionAttribute.ErrorLog();
            ServiceStatus _serviceStatus = new ServiceStatus();

            _serviceStatus.StatusCode = Convert.ToInt32(HttpStatusCode.NotFound);
            _serviceStatus.StatusMessage = "The requested resource is not found";
            _serviceStatus.ReasonPhrase = "404 Resource Not Found";
            _errorLog.errorStatus = _serviceStatus;

            return Ok(_errorLog);
        }
    }
}
