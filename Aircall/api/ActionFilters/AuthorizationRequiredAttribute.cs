using api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.IdentityModel;
using api.Controllers;
using api.ViewModel;
using System.Threading.Tasks;
using api.App_Start;

namespace api.ActionFilters
{
    public class AuthorizationRequiredAttribute : ActionFilterAttribute
    {
        private const string Token = "bearer";

        public override void OnActionExecuting(HttpActionContext filterContext)
        {
            //  Get API key provider
            //var provider = filterContext.ControllerContext.Configuration
            //    .DependencyResolver.GetService(typeof(ITokenServices)) as ITokenServices;
            
            if (filterContext.Request.Headers.Authorization.Scheme.ToLower().Contains(Token))
            {
                var tokenValue = filterContext.Request.Headers.Authorization.Parameter;
                if (tokenValue != null)
                {
                    if (checkToken(tokenValue))
                    {

                    }
                    else
                    {
                        filterContext.Response = filterContext.Request.CreateResponse((new { AuthorizeStatus = HttpStatusCode.Unauthorized, ReasonPhrase = "Token Validation Failed", StatusCode = HttpStatusCode.Unauthorized, Version = HttpVersion.Version10 }));
                        //filterContext.Response =  new HttpResponseMessage(HttpStatusCode.Unauthorized) { ReasonPhrase = "Invalid Request", StatusCode = HttpStatusCode.Unauthorized, Version = HttpVersion.Version10 };
                    }
                }
            }
            else
            {
                filterContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized) { ReasonPhrase = "Token Not Sent", StatusCode = HttpStatusCode.Unauthorized, Version = HttpVersion.Version10 };
            }

            base.OnActionExecuting(filterContext);
        }
        public bool checkToken(string Token)
        {
            return true;
            Aircall_DBEntities1 _db = new Aircall_DBEntities1();
            var chkToken = _db.AppAccessTokens.AsEnumerable().Where(top => top.AuthToken == Token && top.UserType == Utilities.UserRoles.Client.GetEnumValue()).FirstOrDefault();
            if (chkToken == null)
            {
                return false;
            }
            Client model = _db.Clients.Where(x => x.Id == chkToken.UserId).FirstOrDefault();
            if (chkToken != null)
            {
                if (chkToken.ExpiresOn <= DateTime.UtcNow)
                {
                    //TimeSpan t = new TimeSpan(1, 0, 0, 0, 0);
                    //chkToken.ExpiresOn = DateTime.Now.Add(t);
                    //_db.SaveChanges();
                    BaseClientApiController.updatetoken = true;
                    TokenDetails objToken = api.Helpers.AsyncHelpers.RunSync<TokenDetails>(() => BaseClientApiController.generatToken(model.Email, model.Password, model.DeviceToken));
                    if (String.IsNullOrEmpty(objToken.error))
                    {
                        BaseClientApiController.Add_UpdateToken(model.Id, objToken);
                        BaseClientApiController.accessToken = objToken.access_token;
                    }
                    return true;
                }
                BaseClientApiController.updatetoken = false;
                BaseClientApiController.accessToken = "";
                return true;
            }
            BaseClientApiController.updatetoken = false;
            BaseClientApiController.accessToken = "";
            return false;
        }
    }
    public class EMPAuthorizationRequiredAttribute : ActionFilterAttribute
    {
        private const string Token = "bearer";

        public override void OnActionExecuting(HttpActionContext filterContext)
        {
            //  Get API key provider
            //var provider = filterContext.ControllerContext.Configuration
            //    .DependencyResolver.GetService(typeof(ITokenServices)) as ITokenServices;
            
            if (filterContext.Request.Headers.Authorization.Scheme.ToLower().Contains(Token))
            {
                var tokenValue = filterContext.Request.Headers.Authorization.Parameter;
                if (tokenValue != null)
                {
                    if (checkToken(tokenValue))
                    {

                    }
                    else
                    {
                        filterContext.Response = filterContext.Request.CreateResponse((new { AuthorizeStatus = HttpStatusCode.Unauthorized, ReasonPhrase = "Invalid Request", StatusCode = HttpStatusCode.Unauthorized, Version = HttpVersion.Version10 }));
                        //filterContext.Response =  new HttpResponseMessage(HttpStatusCode.Unauthorized) { ReasonPhrase = "Invalid Request", StatusCode = HttpStatusCode.Unauthorized, Version = HttpVersion.Version10 };
                    }
                }
            }
            else
            {
                filterContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized) { ReasonPhrase = "Invalid Request", StatusCode = HttpStatusCode.Unauthorized, Version = HttpVersion.Version10 };
            }

            base.OnActionExecuting(filterContext);
        }
        public bool checkToken(string Token)
        {
            Aircall_DBEntities1 _db = new Aircall_DBEntities1();
            var chkToken = _db.EmpAppAccessTokens.AsEnumerable().Where(top => top.AuthToken == Token && top.UserType == Utilities.UserRoles.Employee.GetEnumValue()).FirstOrDefault();
            if (chkToken == null)
            {
                return false;
            }
            var model = _db.Employees.Where(x => x.Id == chkToken.UserId).FirstOrDefault();
            if (chkToken != null)
            {
                if (chkToken.ExpiresOn <= DateTime.UtcNow)
                {
                    BaseEmployeeApiController.updatetoken = true;
                    TokenDetails objToken = api.Helpers.AsyncHelpers.RunSync<TokenDetails>(() => BaseEmployeeApiController.generatEmpToken(model.Email, model.Password, model.DeviceToken));
                    if (String.IsNullOrEmpty(objToken.error))
                    {
                        BaseEmployeeApiController.Add_UpdateToken(model.Id, objToken);
                        BaseEmployeeApiController.accessToken = objToken.access_token;
                    }
                    return true;
                }
                BaseEmployeeApiController.updatetoken = false;
                BaseEmployeeApiController.accessToken = "";
                return true;
            }
            BaseEmployeeApiController.updatetoken = false;
            BaseEmployeeApiController.accessToken = "";
            return false;
        }
    }
}